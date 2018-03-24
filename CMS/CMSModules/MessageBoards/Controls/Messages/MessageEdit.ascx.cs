using System;
using System.Collections.Generic;
using System.Data;
using System.Web;
using System.Web.UI;

using CMS.CMSHelper;
using CMS.ExtendedControls;
using CMS.GlobalHelper;
using CMS.MessageBoard;
using CMS.PortalEngine;
using CMS.SettingsProvider;
using CMS.SiteProvider;
using CMS.DocumentEngine;
using CMS.UIControls;
using CMS.WebAnalytics;
using CMS.MessageBoards;
using CMS.Helpers;
using CMS.Protection;
using CMS.Membership;
using CMS.Globalization;

public partial class CMSModules_MessageBoards_Controls_Messages_MessageEdit : CMSAdminEditControl
{
    #region "Events"

    public event OnAfterMessageSavedEventHandler OnAfterMessageSaved;
    public event OnBeforeMessageSavedEventHandler OnBeforeMessageSaved;

    #endregion


    #region "Variables"

    private bool mAdvancedMode = false;
    private bool mCheckFloodProtection = false;
    private int mMessageID = 0;
    private int mMessageBoardID = 0;
    private bool mModalMode = false;

    private AbstractRatingControl ratingControl = null;
    private BoardProperties mBoardProperties = new BoardProperties();
    private BoardMessageInfo messageInfo = null;
    private BoardInfo mBoard = null;

    #endregion


    #region "Properties"

    /// <summary>
    /// Messages placeholder
    /// </summary>
    public override MessagesPlaceHolder MessagesPlaceHolder
    {
        get
        {
            return plcMess;
        }
    }


    /// <summary>
    /// Indicates if control is used on live site.
    /// </summary>
    public override bool IsLiveSite
    {
        get
        {
            return base.IsLiveSite;
        }
        set
        {
            plcMess.IsLiveSite = value;
            base.IsLiveSite = value;
        }
    }


    /// <summary>
    /// Advance mode.
    /// </summary>
    public bool AdvancedMode
    {
        get
        {
            return mAdvancedMode;
        }
        set
        {
            mAdvancedMode = value;
        }
    }


    /// <summary>
    /// Advance mode.
    /// </summary>
    public bool CheckFloodProtection
    {
        get
        {
            return mCheckFloodProtection;
        }
        set
        {
            mCheckFloodProtection = value;
        }
    }


    /// <summary>
    /// Message Id.
    /// </summary>
    public int MessageID
    {
        get
        {
            return mMessageID;
        }
        set
        {
            mMessageID = value;
        }
    }


    /// <summary>
    /// Message board Id.
    /// </summary>
    public int MessageBoardID
    {
        get
        {
            if (mMessageBoardID == 0)
            {
                mMessageBoardID = (messageInfo != null) ? messageInfo.MessageBoardID : 0;
            }
            return mMessageBoardID;
        }
        set
        {
            mMessageBoardID = value;

            mBoard = null;
        }
    }


    /// <summary>
    /// Message board object.
    /// </summary>
    public BoardInfo Board
    {
        get
        {
            return mBoard ?? (mBoard = BoardInfoProvider.GetBoardInfo(MessageBoardID));
        }
    }


    /// <summary>
    /// Message board properties.
    /// </summary>
    public BoardProperties BoardProperties
    {
        get
        {
            return mBoardProperties;
        }
        set
        {
            mBoardProperties = value;
        }
    }


    /// <summary>
    /// Indicates if message edit is in modal dialog.
    /// </summary>
    public bool ModalMode
    {
        get
        {
            return mModalMode;
        }
        set
        {
            mModalMode = value;
        }
    }

    #endregion


    protected void Page_Load(object sender, EventArgs e)
    {
        // Let parent check 'Modify' permission if required
        if (!RaiseOnCheckPermissions(PERMISSION_MODIFY, this))
        {
            // Parent page doesn't check permissions
        }

        SetContext();

        // Initialize the controls
        SetupControls();

        // Reload data if necessary
        if (!URLHelper.IsPostback())
        {
            ReloadData();
        }

        ReleaseContext();
    }


    #region "Events handling"

    protected void ratingControl_RatingEvent(AbstractRatingControl sender)
    {
        ViewState["ratingvalue"] = sender.CurrentRating;
    }


    protected void btnOk_Click(object sender, EventArgs e)
    {
        // Let the parent control now new message is being saved
        if (OnBeforeMessageSaved != null)
        {
            OnBeforeMessageSaved();
        }

        // Check banned IP
        if (!BannedIPInfoProvider.IsAllowed(SiteContext.CurrentSiteName, BanControlEnum.AllNonComplete))
        {
            ShowError(GetString("General.BannedIP"));
            return;
        }

        // Validate form
        string errorMessage = ValidateForm();

        if (errorMessage == "")
        {
            // Check flooding when message being inserted through the LiveSite
            if (CheckFloodProtection && IsLiveSite && FloodProtectionHelper.CheckFlooding(SiteContext.CurrentSiteName, MembershipContext.AuthenticatedUser))
            {
                ShowError(GetString("General.FloodProtection"));
                return;
            }

            CurrentUserInfo currentUser = MembershipContext.AuthenticatedUser;

            BoardMessageInfo messageInfo = null;

            if (MessageID > 0)
            {
                // Get message info
                messageInfo = BoardMessageInfoProvider.GetBoardMessageInfo(MessageID);
                MessageBoardID = messageInfo.MessageBoardID;
            }
            else
            {
                // Create new info
                messageInfo = new BoardMessageInfo();

                // User IP address
                messageInfo.MessageUserInfo.IPAddress = RequestContext.UserHostAddress;
                // User agent
                messageInfo.MessageUserInfo.Agent = Request.UserAgent;
            }

            // Setup message info
            messageInfo.MessageEmail = txtEmail.Text.Trim();
            messageInfo.MessageText = txtMessage.Text.Trim();

            // Handle message URL
            string url = txtURL.Text.Trim();
            if ((url != "http://") && (url != "https://") && (url != ""))
            {
                if ((!url.ToLowerCSafe().StartsWithCSafe("http://")) && (!url.ToLowerCSafe().StartsWithCSafe("https://")))
                {
                    url = "http://" + url;
                }
            }
            else
            {
                url = "";
            }
            messageInfo.MessageURL = url;
            messageInfo.MessageURL = messageInfo.MessageURL.ToLowerCSafe().Replace("javascript", "_javascript");

            messageInfo.MessageUserName = txtUserName.Text.Trim();
            if ((MessageID <= 0 ) && (!currentUser.IsPublic()))
            {
                messageInfo.MessageUserID = currentUser.UserID;
            }

            messageInfo.MessageIsSpam = ValidationHelper.GetBoolean(chkSpam.Checked, false);

            if (BoardProperties.EnableContentRating && (ratingControl != null) &&
                (ratingControl.GetCurrentRating() > 0))
            {
                messageInfo.MessageRatingValue = ratingControl.CurrentRating;

                // Remember rating in cookie
                string docs = CookieHelper.GetValue(CookieName.RatedDocuments) ?? "|";
                if (!docs.Contains("|" + DocumentContext.CurrentDocument.DocumentID + "|"))
                {
                    docs += DocumentContext.CurrentDocument.DocumentID + "|";

                    // Actualize the cookie
                    CookieHelper.SetValue(CookieName.RatedDocuments, docs, DateTime.Now.AddYears(1));
                }
            }

            BoardInfo boardInfo = null;

            // If there is message board
            if (MessageBoardID > 0)
            {
                // Load message board
                boardInfo = Board;
            }
            else
            {
                // Create new message board according to webpart properties
                boardInfo = new BoardInfo(BoardProperties);
                BoardInfoProvider.SetBoardInfo(boardInfo);

                // Update information on current message board
                MessageBoardID = boardInfo.BoardID;

                // Set board-role relationship                
                BoardRoleInfoProvider.SetBoardRoles(MessageBoardID, BoardProperties.BoardRoles);

                // Set moderators
                BoardModeratorInfoProvider.SetBoardModerators(MessageBoardID, BoardProperties.BoardModerators);
            }

            if (boardInfo != null)
            {
                // If the very new message is inserted
                if (MessageID == 0)
                {
                    // If creating message set inserted to now and assign to board
                    messageInfo.MessageInserted = currentUser.DateTimeNow;
                    messageInfo.MessageBoardID = MessageBoardID;

                    // Handle auto approve action
                    bool isAuthorized = BoardInfoProvider.IsUserAuthorizedToManageMessages(boardInfo);
                    if (isAuthorized)
                    {
                        messageInfo.MessageApprovedByUserID = currentUser.UserID;
                        messageInfo.MessageApproved = true;
                    }
                    else
                    {
                        // Is board moderated ?
                        messageInfo.MessageApprovedByUserID = 0;
                        messageInfo.MessageApproved = !boardInfo.BoardModerated;
                    }
                }
                else
                {
                    if (chkApproved.Checked)
                    {
                        // Set current user as approver
                        messageInfo.MessageApproved = true;
                        messageInfo.MessageApprovedByUserID = currentUser.UserID;
                    }
                    else
                    {
                        messageInfo.MessageApproved = false;
                        messageInfo.MessageApprovedByUserID = 0;
                    }
                }

                if (!AdvancedMode)
                {
                    if (!BadWordInfoProvider.CanUseBadWords(MembershipContext.AuthenticatedUser, SiteContext.CurrentSiteName))
                    {
                        // Columns to check
                        Dictionary<string, int> collumns = new Dictionary<string, int>();
                        collumns.Add("MessageText", 0);
                        collumns.Add("MessageUserName", 250);

                        // Perform bad words check 
                        errorMessage = BadWordsHelper.CheckBadWords(messageInfo, collumns, "MessageApproved", "MessageApprovedByUserID",
                                                                    messageInfo.MessageText, currentUser.UserID, () => { return ValidateMessage(messageInfo); });

                        // Additionally check empty fields
                        if (errorMessage == string.Empty)
                        {
                            if (!ValidateMessage(messageInfo))
                            {
                                errorMessage = GetString("board.messageedit.emptybadword");
                            }
                        }
                    }
                }

                // Subscribe this user to message board
                if (chkSubscribe.Checked)
                {
                    string email = messageInfo.MessageEmail;

                    // Check for duplicate e-mails
                    DataSet ds = BoardSubscriptionInfoProvider.GetSubscriptions("((SubscriptionApproved = 1) OR (SubscriptionApproved IS NULL)) AND SubscriptionBoardID=" + MessageBoardID +
                                                                                " AND SubscriptionEmail='" + SecurityHelper.GetSafeQueryString(email, false) + "'", null);
                    if (DataHelper.DataSourceIsEmpty(ds))
                    {
                        BoardSubscriptionInfo bsi = new BoardSubscriptionInfo();
                        bsi.SubscriptionBoardID = MessageBoardID;
                        bsi.SubscriptionEmail = email;
                        if (!currentUser.IsPublic())
                        {
                            bsi.SubscriptionUserID = currentUser.UserID;
                        }
                        BoardSubscriptionInfoProvider.Subscribe(bsi, DateTime.Now, true, true);
                        ClearForm();

                        if (bsi.SubscriptionApproved)
                        {
                            LogSubscribingActivity(bsi, boardInfo);
                        }
                    }
                    else
                    {
                        errorMessage = GetString("board.subscription.emailexists");
                    }
                }

                if (errorMessage == "")
                {
                    try
                    {
                        // Save message info
                        BoardMessageInfoProvider.SetBoardMessageInfo(messageInfo);

                        LogCommentActivity(messageInfo, boardInfo);

                        if (BoardProperties.EnableContentRating && (ratingControl != null) && (ratingControl.GetCurrentRating() > 0))
                        {
                            LogRatingActivity(ratingControl.CurrentRating);
                        }

                        // If the message is not approved let the user know message is waiting for approval
                        if (messageInfo.MessageApproved == false)
                        {
                            ShowError(GetString("board.messageedit.waitingapproval"));
                        }

                        // Rise after message saved event
                        if (OnAfterMessageSaved != null)
                        {
                            OnAfterMessageSaved(messageInfo);
                        }

                        // Hide message form if user has rated and empty rating is not allowed
                        if (!BoardProperties.AllowEmptyRating && TreeProvider.HasRated(DocumentContext.CurrentDocument))
                        {
                            pnlMessageEdit.Visible = false;
                            lblAlreadyrated.Visible = true;
                        }
                        else
                        {
                            // Hide rating form if user has rated
                            if (BoardProperties.EnableContentRating && (ratingControl != null) && ratingControl.GetCurrentRating() > 0)
                            {
                                plcRating.Visible = false;
                            }
                        }

                        // Clear form content
                        ClearForm();
                    }
                    catch (Exception ex)
                    {
                        errorMessage = ex.Message;
                    }
                }
            }
        }


        if (!String.IsNullOrEmpty(errorMessage))
        {
            ShowError(errorMessage);
        }
    }

    #endregion

    #region "Private methods"

    /// <summary>
    /// Initializes the controls.
    /// </summary>
    private void SetupControls()
    {
        lblRating.Text = GetString("board.messageedit.rating");
        lblEmail.Text = GetString("board.messageedit.email");
        lblMessage.Text = GetString("board.messageedit.message");
        lblURL.Text = GetString("board.messageedit.url");
        lblUserName.Text = GetString("board.messageedit.username");

        chkSubscribe.Text = GetString("board.messageedit.subscribe");

        rfvMessage.ErrorMessage = GetString("board.messageedit.rfvmessage");
        rfvUserName.ErrorMessage = GetString("board.messageedit.rfvusername");
        revEmailValid.ErrorMessage = GetString("board.messageedit.revemail");
        rfvEmail.ErrorMessage = GetString("board.messageedit.rfvemail");

        // Ensure unique validation group name in case of multiple controls in one page
        string valGroup = UniqueID;

        txtUserName.ValidationGroup = valGroup;
        rfvUserName.ValidationGroup = valGroup;

        txtEmail.ValidationGroup = valGroup;
        rfvEmail.ValidationGroup = valGroup;
        revEmailValid.ValidationGroup = valGroup;
        revEmailValid.ValidationExpression = @"^([\w0-9_\-\+]+(\.[\w0-9_\-\+]+)*@[\w0-9_-]+(\.[\w0-9_-]+)+)*$";

        txtMessage.ValidationGroup = valGroup;
        rfvMessage.ValidationGroup = valGroup;

        btnOk.ValidationGroup = valGroup;
        btnOkFooter.ValidationGroup = valGroup;

        // Fields visibility  
        plcUserName.Visible = BoardProperties.ShowNameField;
        plcEmail.Visible = BoardProperties.ShowEmailField;
        plcUrl.Visible = BoardProperties.ShowURLField;

        if (PortalContext.ViewMode == ViewModeEnum.LiveSite)
        {
            lblMessage.CssClass = "";
        }

        // Load message board
        if (BoardProperties != null)
        {
            if (!BoardProperties.BoardRequireEmails)
            {
                rfvEmail.Enabled = false;
            }

            if ((BoardProperties.BoardUseCaptcha) && (!AdvancedMode))
            {
                // Show captcha text and control
                pnlCaptcha.Visible = true;
            }
        }

        plcRating.Visible = false;

        // Show rating form only if user has not rated yet
        if (!AdvancedMode && BoardProperties.EnableContentRating && !TreeProvider.HasRated(DocumentContext.CurrentDocument))
        {
            if (DocumentContext.CurrentDocument != null)
            {
                plcRating.Visible = true;
                try
                {
                    // Insert rating control to page
                    ratingControl = (AbstractRatingControl)(Page.LoadUserControl(AbstractRatingControl.GetRatingControlUrl(BoardProperties.RatingType + ".ascx")));
                }
                catch (Exception ex)
                {
                    Controls.Add(new LiteralControl(ex.Message));
                    return;
                }

                // Init values
                ratingControl.ID = ID + "_RatingControl";
                ratingControl.MaxRating = BoardProperties.MaxRatingValue;
                ratingControl.Visible = true;
                ratingControl.Enabled = true;
                ratingControl.RatingEvent += ratingControl_RatingEvent;
                ratingControl.CurrentRating = ValidationHelper.GetDouble(ViewState["ratingvalue"], 0);
                ratingControl.ExternalManagement = true;
                pnlRating.Controls.Clear();
                pnlRating.Controls.Add(ratingControl);
            }
        }

        if (AdvancedMode)
        {
            // Initialize advanced controls
            plcAdvanced.Visible = true;
            lblApproved.Text = GetString("board.messageedit.approved");
            lblSpam.Text = GetString("board.messageedit.spam");
            lblInsertedCaption.Text = GetString("board.messageedit.inserted");
            btnOk.ResourceString = "general.ok";
            btnOkFooter.ResourceString = "general.ok";

            // Show or hide "Inserted" label
            bool showInserted = (MessageID > 0);
            lblInsertedCaption.Visible = showInserted;
            lblInserted.Visible = showInserted;
            chkSubscribe.Visible = false;
        }
        else
        {
            // If is not moderated then autocheck approve
            if (!BoardProperties.BoardModerated)
            {
                chkApproved.Checked = true;
            }
        }

        if (ModalMode)
        {
            plcFooter.Visible = true;
            pnlOkButton.Visible = false;
        }
        else
        {
            plcFooter.Visible = false;
            pnlOkButton.Visible = true;
        }

        // Show/hide subscription option
        plcChkSubscribe.Visible = BoardProperties.BoardEnableSubscriptions && BoardProperties.ShowEmailField;

        // For new message hide Is approved chkbox (auto approve)
        if (MessageID <= 0)
        {
            plcApproved.Visible = false;
        }

        // Hide message form if user has rated and empty rating is not allowed
        if (!BoardProperties.AllowEmptyRating && TreeProvider.HasRated(DocumentContext.CurrentDocument))
        {
            pnlMessageEdit.Visible = false;
            lblAlreadyrated.Visible = true;
        }
    }


    private static bool ValidateMessage(BoardMessageInfo messageInfo)
    {
        if ((messageInfo.MessageText == null) || (messageInfo.MessageUserName == null))
        {
            return false;
        }

        return ((messageInfo.MessageText.Trim() != "") && (messageInfo.MessageUserName.Trim() != ""));
    }


    /// <summary>
    /// Validate message form and return error message if is some.
    /// </summary>
    private string ValidateForm()
    {
        txtUserName.Text = txtUserName.Text.Trim();
        txtEmail.Text = txtEmail.Text.Trim();
        txtMessage.Text = txtMessage.Text.Trim();

        string errorMessage = "";

        // Check rating value
        if (!BoardProperties.AllowEmptyRating && (ratingControl != null) && (ratingControl.GetCurrentRating() <= 0))
        {
            errorMessage = GetString("board.messageedit.emptyrating");
        }

        // Check user name field
        if (string.IsNullOrEmpty(errorMessage) && plcUserName.Visible)
        {
            errorMessage = new Validator().NotEmpty(txtUserName.Text, rfvUserName.ErrorMessage).Result;
        }

        // Check e-mail field
        if (string.IsNullOrEmpty(errorMessage) && plcEmail.Visible)
        {
            if (BoardProperties.BoardRequireEmails)
            {
                // Check e-mail address if board require
                errorMessage = new Validator()
                    .NotEmpty(txtEmail.Text, rfvEmail.ErrorMessage)
                    .IsEmail(txtEmail.Text, revEmailValid.ErrorMessage).Result;
            }
            else
            {
                if (txtEmail.Text != "")
                {
                    // Check e-mail address if is some
                    errorMessage = new Validator()
                        .IsEmail(txtEmail.Text, revEmailValid.ErrorMessage).Result;
                }
            }
        }

        // Check message text field
        if (string.IsNullOrEmpty(errorMessage))
        {
            errorMessage = new Validator().NotEmpty(txtMessage.Text, rfvMessage.ErrorMessage).Result;
        }

        // Check e-mail if subscribing
        if (string.IsNullOrEmpty(errorMessage) && plcChkSubscribe.Visible && chkSubscribe.Checked)
        {
            errorMessage = new Validator()
                .NotEmpty(txtEmail.Text, GetString("board.messageedit.rfvemail"))
                .IsEmail(txtEmail.Text, GetString("board.messageedit.revemail")).Result;
        }

        // Check security code
        if (string.IsNullOrEmpty(errorMessage) && BoardProperties.BoardUseCaptcha && !captchaElem.IsValid())
        {
            errorMessage = captchaElem.ValidationError;
        }

        return errorMessage;
    }


    /// <summary>
    /// Reloads the form data.
    /// </summary>
    public override void ReloadData()
    {
        base.ReloadData();

        if (MessageID > 0)
        {
            messageInfo = BoardMessageInfoProvider.GetBoardMessageInfo(MessageID);
            if (messageInfo != null)
            {
                EditedObject = messageInfo;

                // Check whether edited message belongs to a board from current site
                if ((Board != null) && (Board.BoardSiteID != CMSContext.CurrentSiteID))
                {
                    EditedObject = null;
                }

                // Set textfields and checkboxes
                txtEmail.Text = messageInfo.MessageEmail;
                txtMessage.Text = messageInfo.MessageText;
                txtURL.Text = messageInfo.MessageURL;
                txtUserName.Text = messageInfo.MessageUserName;
                chkApproved.Checked = messageInfo.MessageApproved;
                chkSpam.Checked = messageInfo.MessageIsSpam;
                lblInserted.Text = TimeZoneMethods.ConvertDateTime(messageInfo.MessageInserted, this).ToString();
            }
        }
        else
        {
            ClearForm();
        }
    }


    /// <summary>
    /// Clears all input boxes.
    /// </summary>
    public override void ClearForm()
    {
        txtUserName.Text = String.Empty;
        txtEmail.Text = String.Empty;
        txtMessage.Text = String.Empty;
        txtURL.Text = "http://";

        if (!MembershipContext.AuthenticatedUser.IsPublic())
        {
            txtUserName.Text = !DataHelper.IsEmpty(MembershipContext.AuthenticatedUser.UserNickName) ? MembershipContext.AuthenticatedUser.UserNickName : MembershipContext.AuthenticatedUser.FullName;
            txtEmail.Text = MembershipContext.AuthenticatedUser.Email;
        }
    }


    /// <summary>
    /// Log activity (subscribing).
    /// </summary>
    /// <param name="bsi">Board subscription info object</param>
    /// <param name="bi">Message board info</param>
    private void LogSubscribingActivity(BoardSubscriptionInfo bsi, BoardInfo bi)
    {
        Activity activity = new ActivitySubscriptionMessageBoard(bi, bsi, DocumentContext.CurrentDocument, AnalyticsContext.ActivityEnvironmentVariables);
        activity.Log();
    }


    /// <summary>
    /// Log activity posting
    /// </summary>
    /// <param name="bmi">Board subscription info object</param>
    /// <param name="bi">Message board info</param>
    private void LogCommentActivity(BoardMessageInfo bmi, BoardInfo bi)
    {
        Activity activity = new ActivityMessageBoardComment(bmi, bi, DocumentContext.CurrentDocument, AnalyticsContext.ActivityEnvironmentVariables);
        activity.Log();
    }


    /// <summary>
    /// Logs rating activity
    /// </summary>
    /// <param name="value">Rating value</param>
    private void LogRatingActivity(double value)
    {
        TreeNode currentDoc = DocumentContext.CurrentDocument;
        if (currentDoc != null)
        {
            string title = String.Format("{0} ({1})", value.ToString(), currentDoc.GetDocumentName());
            Activity activity = new ActivityRating(title, value, currentDoc, AnalyticsContext.ActivityEnvironmentVariables);
            activity.Log();
        }
    }

    #endregion
}