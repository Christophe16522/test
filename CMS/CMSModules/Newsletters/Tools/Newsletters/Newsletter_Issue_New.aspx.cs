using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Linq;

using CMS.ExtendedControls;
using CMS.Helpers;
using CMS.Newsletters;
using CMS.Base;
using CMS.Membership;
using CMS.SiteProvider;
using CMS.UIControls;
using CMS.ExtendedControls.ActionsConfig;
using CMS.DataEngine;

[Security(Resource = "CMS.Newsletter", UIElements = "Newsletters;Newsletter;EditNewsletterProperties;Newsletter.Issues;EditIssueProperties")]
public partial class CMSModules_Newsletters_Tools_Newsletters_Newsletter_Issue_New : CMSToolsModalPage
{
    #region "Constants"

    private const string mAttachmentsActionClass = "attachments-header-action";

    #endregion


    #region "Variables"

    /// <summary>
    /// Current step index
    /// </summary>
    protected int currentStep = 0;


    /// <summary>
    /// Number of steps
    /// </summary>
    protected const int steps = 3;


    /// <summary>
    /// Newsletter ID
    /// </summary>
    protected int newsletterId = 0;


    /// <summary>
    /// Selected template ID
    /// </summary>
    protected int templateId = 0;


    /// <summary>
    /// Newsletter issue ID
    /// </summary>
    protected int issueId = 0;


    /// <summary>
    /// Indicates if step was succesfully loaded
    /// </summary>
    protected bool stepLoaded = false;


    /// <summary>
    /// Indicates if template selection was skipped
    /// </summary>
    protected bool? selectionSkipped = null;

    #endregion


    #region "Properties"

    /// <summary>
    /// Current step index, indexed from 1
    /// </summary>
    public int CurrentStepIndex
    {
        get
        {
            if (currentStep == 0)
            {
                if (RequestHelper.IsPostBack())
                {
                    currentStep = ValidationHelper.GetInteger(hdnCurrent.Value, currentStep);
                }
            }

            // Ensure the correct step index
            if (currentStep > steps)
            {
                currentStep = steps;
            }
            if (currentStep < 1)
            {
                currentStep = 1;
            }

            return currentStep;
        }
        set
        {
            currentStep = value;
        }
    }


    /// <summary>
    /// Number of the steps
    /// </summary>
    protected int Steps
    {
        get
        {
            return steps;
        }
    }


    /// <summary>
    /// Indicates if template selection step was skipped
    /// </summary>
    protected bool SelectionSkipped
    {
        get
        {
            if (selectionSkipped == null)
            {
                selectionSkipped = ValidationHelper.GetBoolean(hdn1stStepSkipped.Value, false);
            }
            return (bool)selectionSkipped;
        }
        set
        {
            selectionSkipped = value;
        }
    }


    /// <summary>
    /// Issue ID
    /// </summary>
    protected int IssueId
    {
        get
        {
            if (issueId == 0)
            {
                if (RequestHelper.IsPostBack())
                {
                    issueId = ValidationHelper.GetInteger(hdnIssueID.Value, issueId);
                }
            }

            return issueId;
        }
        set
        {
            issueId = value;
        }
    }


    /// <summary>
    /// Messages placeholder
    /// </summary>
    public override MessagesPlaceHolder MessagesPlaceHolder
    {
        get
        {
            return plcMessages;
        }
    }

    #endregion


    #region "Page events"

    /// <summary>
    /// Init event handler
    /// </summary>
    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        // Register the events
        ComponentEvents.RequestEvents.RegisterForEvent(ComponentEvents.LOAD_STEP, InitStep);
        ComponentEvents.RequestEvents.RegisterForEvent(ComponentEvents.STEP_LOADED, StepLoaded);
        ComponentEvents.RequestEvents.RegisterForEvent(ComponentEvents.VALIDATE_STEP, ValidateStep);
        ComponentEvents.RequestEvents.RegisterForEvent(ComponentEvents.FINISH_STEP, FinishStep);
        ComponentEvents.RequestEvents.RegisterForEvent(ComponentEvents.NEXT, NextStep);
        ComponentEvents.RequestEvents.RegisterForEvent(ComponentEvents.PREVIOUS, PreviousStep);
        ComponentEvents.RequestEvents.RegisterForEvent(ComponentEvents.FINISH, Finish);
        ComponentEvents.RequestEvents.RegisterForEvent(ComponentEvents.SAVE, SaveAndClose);

        // Initialize uni buttons
        btnTemplateNext.LinkText = GetString("general.next");
        btnTemplateNext.LinkEvent = ComponentEvents.NEXT;

        btnBack.LinkText = GetString("general.back");
        btnBack.LinkEvent = ComponentEvents.PREVIOUS;

        btnNext.LinkText = GetString("general.next");
        btnNext.LinkEvent = ComponentEvents.NEXT;
        btnNext.OnClientClick = "if (GetContent != null) {return GetContent();}";

        btnFinish.LinkText = GetString("general.finish");
        btnFinish.LinkEvent = ComponentEvents.FINISH;

        btnSendClose.LinkText = GetString("newsletter_send.sendandclose");
        btnSendClose.LinkEvent = ComponentEvents.FINISH;

        btnSave.LinkText = GetString("newsletter_send.savewosending");
        btnSave.LinkEvent = ComponentEvents.SAVE;

        // Set update mode to ALWAYS
        hdrActions.UpdatePanel.UpdateMode = UpdatePanelUpdateMode.Always;

        // Remove css style from master page to fix padding
        CurrentMaster.PanelContent.RemoveCssClass("dialog-content");
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        newsletterId = QueryHelper.GetInteger("parentobjectid", 0);
        var newsletter = NewsletterInfoProvider.GetNewsletterInfo(newsletterId);

        var siteName = SiteInfoProvider.GetSiteName(newsletter.NewsletterSiteID);
        if (!CurrentUser.IsAuthorizedPerResource(newsletter.TypeInfo.ModuleName, "AuthorIssues", siteName))
        {
            RedirectToAccessDenied(newsletter.TypeInfo.ModuleName, "AuthorIssues");
        }

        // Register script for refreshing parent page
        ScriptHelper.RegisterStartupScript(this, GetType(), "RefreshParent", "function RefreshPage() {if((wopener!=null)&&(wopener.RefreshPage!=null)){wopener.RefreshPage();}}", true);

        // Load the initial step
        LoadStep(null, CurrentStepIndex);
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        hdnCurrent.Value = CurrentStepIndex.ToString();
        hdn1stStepSkipped.Value = SelectionSkipped.ToString();
        hdnIssueID.Value = IssueId.ToString();

        // Initialize variant slider
        if (pnlActions.Visible)
        {
            InitVariantSlider(IssueId);
        }
    }

    #endregion


    #region "Wizard events"

    /// <summary> 
    /// Moves the wizard to the next step
    /// </summary>
    protected void NextStep(object sender, EventArgs e)
    {
        ChangeStep(1);
    }


    /// <summary>
    /// Moves the wizard to the next step
    /// </summary>
    protected void PreviousStep(object sender, EventArgs e)
    {
        ChangeStep(-1);
    }


    /// <summary>
    /// Finishes the wizard
    /// </summary>
    protected void Finish(object sender, EventArgs e)
    {
        // Fire the event to validate current step
        var args = new StepEventArgs(Steps, CurrentStepIndex);

        ComponentEvents.RequestEvents.RaiseEvent(this, args, ComponentEvents.VALIDATE_STEP);

        if (args.CancelEvent)
        {
            // Cancel if not validated
            return;
        }

        // Fire the event to finish current step
        ComponentEvents.RequestEvents.RaiseEvent(this, args, ComponentEvents.FINISH_STEP);

        if (args.CancelEvent)
        {
            // Cancel if not finished
            return;
        }

        // Close dialog and refresh parent page
        ScriptHelper.RegisterStartupScript(this, GetType(), "ClosePage", "RefreshPage(); setTimeout('CloseDialog()',200);", true);
    }


    /// <summary>
    /// Saves current settings and close dialog.
    /// </summary>
    protected void SaveAndClose(object sender, EventArgs e)
    {
        // Fire the event to validate current step
        var args = new StepEventArgs(Steps, CurrentStepIndex);

        ComponentEvents.RequestEvents.RaiseEvent(this, args, ComponentEvents.VALIDATE_STEP);

        if (args.CancelEvent)
        {
            // Cancel if not validated
            return;
        }

        // Save newsletter settings
        string errMessage = SaveSettings();
        if (!String.IsNullOrEmpty(errMessage))
        {
            args.CancelEvent = true;
            // Display error message
            MessagesPlaceHolder.ShowError(errMessage);
        }



        if (args.CancelEvent)
        {
            // Cancel if not finished
            return;
        }

        // Close dialog and refresh parent page
        ScriptHelper.RegisterStartupScript(this, GetType(), "ClosePage", "RefreshPage(); setTimeout('CloseDialog()',200);", true);
    }

    #endregion


    #region "Wizard methods"

    /// <summary>
    /// Changes the current step
    /// </summary>
    /// <param name="offset">Offset by which the step should move</param>
    protected void ChangeStep(int offset)
    {
        if (offset == 0)
        {
            return;
        }

        int numberOfSteps = Steps;
        int original = CurrentStepIndex;

        // Fire the event to validate current step
        var args = new StepEventArgs(numberOfSteps, original);

        if (stepLoaded)
        {
            ComponentEvents.RequestEvents.RaiseEvent(this, args, ComponentEvents.VALIDATE_STEP);

            if (args.CancelEvent)
            {
                // Cancel if not validated
                return;
            }

            // Fire the event to finish current step
            ComponentEvents.RequestEvents.RaiseEvent(this, args, ((offset > 0) ? ComponentEvents.FINISH_STEP : ComponentEvents.CANCEL_STEP));

            if (args.CancelEvent)
            {
                // Cancel if not finished
                return;
            }
        }

        // Reevaluate the current item index
        int current = original + offset;
        if (current < 1)
        {
            current = 1;
        }
        if (current > Steps)
        {
            current = Steps;
        }

        // Move the step by the given index
        CurrentStepIndex = current;

        // Change the step
        if (original != current)
        {
            // Load the step
            LoadStep(args, current);
        }
    }


    /// <summary>
    /// Loads the given step.
    /// </summary>
    /// <param name="args">Step arguments</param>
    /// <param name="current">Step index</param>
    private void LoadStep(StepEventArgs args, int current)
    {
        // Ensure the event arguments
        if (args == null)
        {
            args = new StepEventArgs(Steps, current);
        }

        args.CurrentStep = current;

        // Fire the event to finish current step
        ComponentEvents.RequestEvents.RaiseEvent(this, args, ComponentEvents.LOAD_STEP);

        // Skip the current step if requested
        if (args.Skip)
        {
            ChangeStep(1);
        }
        else
        {
            // Use the custom step header if set
            string header = GetHeader(current);
            if (!String.IsNullOrEmpty(header))
            {
                ucHeader.Description = header;
            }
            ucHeader.Title = String.Format(GetString("newsletterissue_new.step"), current - (SelectionSkipped ? 1 : 0), SelectionSkipped ? 2 : 3);
            ucHeader.Header = GetString("newsletterissue_new.newissue");

            // Handle the next button display
            if (args.Steps == current)
            {
                args.HideNextButton = true;
            }

            // Fire the event to notify that the step was loaded
            ComponentEvents.RequestEvents.RaiseEvent(this, args, ComponentEvents.STEP_LOADED);
        }
    }


    /// <summary>
    /// Gets the header for the given step.
    /// </summary>
    /// <param name="stepIndex">Step index</param>
    protected string GetHeader(int stepIndex)
    {
        string header = null;

        switch (stepIndex)
        {
            case 1:
                header = GetString("newsletterissue_new.templateselection");
                break;
            case 2:
                header = GetString("newsletterissue_new.editcontent");
                break;
            case 3:
                header = GetString("newsletterissue_send.header");
                break;
        }

        return header;
    }

    #endregion


    #region "Wizard event handlers"

    /// <summary>
    /// Fired when a validation is required before moving to a new step.
    /// </summary>
    protected void ValidateStep(object sender, EventArgs e)
    {
        StepEventArgs args = (StepEventArgs)e;

        switch (args.CurrentStep)
        {
            case 1:
                if (ValidationHelper.GetInteger(selectElem.SelectedItem, 0) <= 0)
                {
                    args.CancelEvent = true;

                    // Display error message
                    MessagesPlaceHolder.ShowError(GetString("newsletter_edit.noemailtemplateselected"));
                }
                break;
            case 2:
                if (!editElem.IsValid())
                {
                    args.CancelEvent = true;

                    // Display error message
                    MessagesPlaceHolder.ShowError(editElem.ErrorMessage);

                    // Refresh content of editable regions in the issue body
                    editElem.RefreshEditableRegions();
                }
                break;
        }
    }


    /// <summary>
    /// Fired when current step was succesfully validated before moving to a new step.
    /// </summary>
    protected void FinishStep(object sender, EventArgs e)
    {
        StepEventArgs args = (StepEventArgs)e;

        switch (args.CurrentStep)
        {
            case 1:
                // Get selected template ID
                templateId = ValidationHelper.GetInteger(selectElem.SelectedItem, 0);
                break;
            case 2:
                // Save the content
                if (SaveChanges())
                {
                    // Get issue ID
                    IssueId = editElem.IssueID;
                }
                else
                {
                    args.CancelEvent = true;
                }
                break;
            case 3:
                // Send an issue (according to the control settings)
                string errMessage = Send();
                if (!String.IsNullOrEmpty(errMessage))
                {
                    args.CancelEvent = true;

                    // Display error message
                    MessagesPlaceHolder.ShowError(errMessage);
                }
                break;
        }
    }


    /// <summary>
    /// Fired when the step is being loaded.
    /// </summary>
    protected void InitStep(object sender, EventArgs e)
    {
        // Drop flag
        stepLoaded = false;

        StepEventArgs args = (StepEventArgs)e;

        switch (args.CurrentStep)
        {
            case 1:
                // TEMPLATE SELECTION
                selectElem.StopProcessing = (newsletterId <= 0);

                // Check if at least one additional template is available for the newsletter
                // ... otherwise skip the first step
                if (newsletterId > 0)
                {
                    string where = String.Format("TemplateType='{1}' AND (NOT TemplateID IN (SELECT NewsletterTemplateID FROM Newsletter_Newsletter WHERE NewsletterID={0}))" +
                        "AND (TemplateID IN (SELECT TemplateID FROM Newsletter_EmailTemplateNewsletter WHERE NewsletterID={0}))", newsletterId, EmailTemplateType.Issue);
                    var ds = EmailTemplateInfoProvider.GetEmailTemplates().Where(where).Column("TemplateID").TopN(1);

                    if (ds.Any())
                    {
                        // Inicialize template selector
                        selectElem.NewsletterId = newsletterId;

                        // Register JS function for double-click template selection
                        string javascript = "function SelectTemplate(value, skipDialog){" + Page.ClientScript.GetPostBackEventReference(btnTemplateNext, null) + "; return false;}";
                        ScriptHelper.RegisterStartupScript(this, typeof(string), "TemplateSelector", ScriptHelper.GetScript(javascript));

                        selectElem.SelectFunction = "SelectTemplate";
                    }
                    else
                    {
                        // Skip this step
                        args.Skip = SelectionSkipped = true;
                    }
                }
                break;
            case 2:
                // CONTENT EDITING
                // Get variant issue ID if A/B testing is ON
                IssueId = InitVariantSlider(IssueId);

                editElem.StopProcessing = (newsletterId <= 0);
                editElem.NewsletterID = newsletterId;
                editElem.IssueID = IssueId;
                editElem.TemplateID = templateId;
                editElem.ReloadData(false);

                // Initialize action menu
                InitHeaderActions();

                sendVariant.ForceReloadNeeded = true;  // control needs to be reloaded in the next step
                break;
            case 3:
                // SENDING CONFIGURATION
                var issue = IssueInfoProvider.GetIssueInfo(IssueId);
                bool isIssueVariant = (issue != null) && issue.IssueIsABTest;
                if (!isIssueVariant)
                {
                    sendElem.StopProcessing = ((newsletterId <= 0) || (IssueId <= 0));
                    sendElem.NewsletterID = newsletterId;
                    sendElem.IssueID = IssueId;
                }
                else
                {
                    sendVariant.StopProcessing = (IssueId <= 0);
                    sendVariant.IssueID = IssueId;
                    sendVariant.ReloadData();
                }
                sendElem.Visible = !isIssueVariant;
                sendVariant.Visible = isIssueVariant;
                break;
        }
    }


    /// <summary>
    /// Fired when the step is loaded.
    /// </summary>
    protected void StepLoaded(object sender, EventArgs e)
    {
        StepEventArgs args = (StepEventArgs)e;
        int currentStepIndex = args.CurrentStep;

        // Display/hide place holders and buttons
        plcSelectTemplate.Visible = (currentStepIndex == 1);
        plcContent.Visible = plcContent.EnableViewState = pnlActions.Visible = ucVariantDialog.Visible = (currentStepIndex == 2);
        if (currentStepIndex != 2)
        {
            pnlSlider.Visible = false;
        }
        plcSend.Visible = (currentStepIndex == 3);
        bool isIssueVariant = false;
        if (currentStepIndex == 3)
        {
            var issue = IssueInfoProvider.GetIssueInfo(IssueId);
            isIssueVariant = (issue != null) && issue.IssueIsABTest;
            sendElem.Visible = !isIssueVariant;
            sendVariant.Visible = !sendElem.Visible;
        }

        btnBack.Visible = !args.HideBackButton && (currentStep == Steps);
        btnNext.Visible = !args.HideNextButton;
        btnFinish.Visible = (currentStepIndex == Steps) && !isIssueVariant;
        btnSendClose.Visible = (currentStepIndex == Steps) && isIssueVariant;
        btnTemplateNext.Visible = (newsletterId > 0);

        btnClose.Visible = (currentStepIndex != 3);

        btnSave.Visible = (currentStepIndex == 3) && isIssueVariant;

        // Raise flag
        stepLoaded = true;
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Initializes header action control.
    /// </summary>
    private void InitHeaderActions()
    {
        bool isNew = (IssueId == 0);
        var issue = IssueInfoProvider.GetIssueInfo(IssueId);
        bool isIssueVariant = !isNew && (issue != null) && issue.IssueIsABTest;

        hdrActions.ActionsList.Clear();

        // Init save button
        hdrActions.ActionsList.Add(new SaveAction(this)
        {
            OnClientClick = "if (GetContent != null) {return GetContent();} else {return false;}"
        });

        // Ensure spell check action
        hdrActions.ActionsList.Add(new HeaderAction
        {
            Text = GetString("EditMenu.IconSpellCheck"),
            Tooltip = GetString("EditMenu.SpellCheck"),
            OnClientClick = "var frame = GetFrame(); if ((frame != null) && (frame.contentWindow.SpellCheck_" + ClientID + " != null)) {frame.contentWindow.SpellCheck_" + ClientID + "();} return false;"
        });

        // Init send draft button
        hdrActions.ActionsList.Add(new HeaderAction
        {
            Text = GetString("newsletterissue_content.senddraft"),
            Tooltip = (isNew) ? GetString("newsletterissue_new.issueunsaved") : GetString("newsletterissue_content.senddraft"),
            OnClientClick = (isNew) ? "return false;" : string.Format(@"if (modalDialog) {{modalDialog('{0}?objectid={1}', 'SendDraft', '700', '300');}}", ResolveUrl(@"~/CMSModules/Newsletters/Tools/Newsletters/Newsletter_Issue_SendDraft.aspx"), IssueId) + " return false;",
            Enabled = !isNew
        });

        // Init preview button
        hdrActions.ActionsList.Add(new HeaderAction
        {
            Text = GetString("general.preview"),
            Tooltip = (isNew) ? GetString("newsletterissue_new.issueunsaved") : GetString("general.preview"),
            OnClientClick = (isNew) ? "return false;" : string.Format(@"if (modalDialog) {{modalDialog('{0}?objectid={1}', 'Preview', '90%', '90%');}}", ResolveUrl(@"~/CMSModules/Newsletters/Tools/Newsletters/Newsletter_Issue_Preview.aspx"), IssueId) + " return false;",
            Enabled = !isNew
        });

        int attachCount = 0;
        string metaFileDialogUrl = null;
        if (!isNew)
        {
            ScriptHelper.RegisterDialogScript(Page);

            // Get number of attachments
            InfoDataSet<MetaFileInfo> ds = MetaFileInfoProvider.GetMetaFiles(IssueId,
                (isIssueVariant ? IssueInfo.OBJECT_TYPE_VARIANT : IssueInfo.OBJECT_TYPE),
                ObjectAttachmentsCategories.ISSUE, null, null, "MetafileID", -1);
            attachCount = ds.Items.Count;

            // Register attachments count update module
            ScriptHelper.RegisterModule(this, "CMS/AttachmentsCountUpdater", new { Selector = "." + mAttachmentsActionClass, Text = ResHelper.GetString("general.attachments") });

            // Prepare metafile dialog URL
            metaFileDialogUrl = ResolveUrl(@"~/CMSModules/AdminControls/Controls/MetaFiles/MetaFileDialog.aspx");
            string query = string.Format("?objectid={0}&objecttype={1}", IssueId, (isIssueVariant ? IssueInfo.OBJECT_TYPE_VARIANT : IssueInfo.OBJECT_TYPE));
            metaFileDialogUrl += string.Format("{0}&category={1}&hash={2}", query, ObjectAttachmentsCategories.ISSUE, QueryHelper.GetHash(query));
        }

        // Init attachment button
        hdrActions.ActionsList.Add(new HeaderAction
        {
            Text = GetString("general.attachments") + ((attachCount > 0) ? " (" + attachCount + ")" : string.Empty),
            Tooltip = (isNew) ? GetString("newsletterissue_new.issueunsaved") : GetString("general.attachments"),
            OnClientClick = (isNew) ? "return false;" : string.Format(@"if (modalDialog) {{modalDialog('{0}', 'Attachments', '700', '500');}}", metaFileDialogUrl) + " return false;",
            Enabled = !isNew,
            CssClass = mAttachmentsActionClass
        });

        // Init create A/B test button - online marketing, open email and click through tracking are required
        if (isNew || !isIssueVariant)
        {
            if (NewsletterHelper.IsABTestingAvailable())
            {
                // Check that trackings are enabled
                NewsletterInfo news = NewsletterInfoProvider.GetNewsletterInfo(newsletterId);
                bool trackingsEnabled = (news != null) && news.NewsletterTrackOpenEmails && news.NewsletterTrackClickedLinks;

                hdrActions.ActionsList.Add(new HeaderAction
                {
                    Text = GetString("newsletterissue_content.createabtest"),
                    Tooltip = (isNew) ? GetString("newsletterissue_new.issueunsaved") : (trackingsEnabled ? GetString("newsletterissue_content.createabtest") : GetString("newsletterissue_content.abtesttooltip")),
                    OnClientClick = (isNew || !trackingsEnabled) ? "return false;" : "ShowVariantDialog_" + ucVariantDialog.ClientID + "('addvariant', ''); return false;",
                    Enabled = !isNew && trackingsEnabled
                });
                ucVariantDialog.IssueID = IssueId;
                ucVariantDialog.OnAddVariant += ucVariantSlider_OnVariantAdded;
            }
        }

        hdrActions.ActionPerformed += HeaderActions_ActionPerformed;
        hdrActions.ReloadData();
        pnlActions.Attributes.Add("onmouseover", "if (RememberFocusedRegion) {RememberFocusedRegion();}");
    }


    /// <summary>
    /// Actions handler.
    /// </summary>
    protected void HeaderActions_ActionPerformed(object sender, CommandEventArgs e)
    {
        switch (e.CommandName.ToLowerInvariant())
        {
            // Validate and save the content
            case "save":
                // Fire the event to validate current step
                var args = new StepEventArgs(Steps, CurrentStepIndex);

                ComponentEvents.RequestEvents.RaiseEvent(this, args, ComponentEvents.VALIDATE_STEP);

                if (args.CancelEvent)
                {
                    // Cancel if not validated
                    return;
                }

                ComponentEvents.RequestEvents.RaiseEvent(this, args, ComponentEvents.FINISH_STEP);

                if (args.CancelEvent)
                {
                    // Cancel if not successfully saved
                    return;
                }

                // Reload edit control
                editElem.ReloadData(true);

                // Initialize action menu
                InitHeaderActions();

                // Show save message
                ShowChangesSaved();

                ComponentEvents.RequestEvents.RaiseEvent(this, args, ComponentEvents.STEP_LOADED);
                break;
        }
    }


    protected bool SaveChanges()
    {
        // Check permission
        if (!MembershipContext.AuthenticatedUser.IsAuthorizedPerResource("CMS.Newsletter", "AuthorIssues"))
        {
            RedirectToAccessDenied("CMS.Newsletter", "AuthorIssues");
        }

        return editElem.Save();
    }


    /// <summary>
    /// Sends an issue. Returns error message if sending fail.
    /// </summary>
    protected string Send()
    {
        // Check permission
        if (!MembershipContext.AuthenticatedUser.IsAuthorizedPerResource("cms.newsletter", "authorissues"))
        {
            RedirectToAccessDenied("cms.newsletter", "authorissues");
        }

        if (sendElem.Visible)
        {
            if (!sendElem.SendIssue())
            {
                return sendElem.ErrorMessage;
            }
        }
        else if (sendVariant.Visible)
        {
            if (!sendVariant.SendIssue())
            {
                return sendVariant.ErrorMessage;
            }
        }
        return null;
    }


    /// <summary>
    /// Saves newsletter settings. Returns error message if saving fail.
    /// </summary>
    protected string SaveSettings()
    {
        if (sendVariant.Visible)
        {
            if (!sendVariant.SaveIssue())
            {
                return sendVariant.ErrorMessage;
            }
        }
        return null;
    }

    #endregion


    #region "A/B test methods"

    /// <summary>
    /// Inits variant slider. Returns selected variant issue ID.
    /// </summary>
    /// <param name="issueId">Issue ID (currently edited)</param>
    private int InitVariantSlider(int issueId)
    {
        // Initialize variant slider
        ucVariantSlider.IssueID = issueId;
        pnlSlider.Visible = ucVariantSlider.Variants.Count > 0;
        if (pnlSlider.Visible)
        {
            int variantIndex = ucVariantSlider.CurrentVariant;
            if (variantIndex >= ucVariantSlider.Variants.Count)
            {
                variantIndex = ucVariantSlider.Variants.Count - 1;
            }
            IssueABVariantItem issueVariant = ucVariantSlider.Variants[(variantIndex < 0 ? 0 : variantIndex)];

            ucVariantSlider.OnVariantDeleted -= ucVariantSlider_OnVariantDeleted;
            ucVariantSlider.OnVariantDeleted += ucVariantSlider_OnVariantDeleted;
            ucVariantSlider.OnVariantAdded -= ucVariantSlider_OnVariantAdded;
            ucVariantSlider.OnVariantAdded += ucVariantSlider_OnVariantAdded;
            ucVariantSlider.OnSelectionChanged -= ucVariantSlider_OnSelectionChanged;
            ucVariantSlider.OnSelectionChanged += ucVariantSlider_OnSelectionChanged;
            return issueVariant.IssueID;
        }
        return issueId;
    }


    /// <summary>
    /// Reloads newly added variant to edit control.
    /// </summary>
    protected void ucVariantSlider_OnVariantAdded(object sender, EventArgs e)
    {
        int currentIssueId;
        if (sender == ucVariantDialog)
        {
            if (!(e is VariantEventArgs)) return;
            VariantEventArgs args = (VariantEventArgs)e;
            currentIssueId = args.ID;
            InitVariantSlider(currentIssueId);
            ucVariantSlider.SetVariant(currentIssueId);
        }
        else
        {
            currentIssueId = ucVariantSlider.IssueID;
        }

        IssueId = currentIssueId;
        editElem.IssueID = currentIssueId;
        editElem.ReloadData(true);
        InitHeaderActions();
    }


    protected void ucVariantSlider_OnVariantDeleted(object sender, EventArgs e)
    {
        int currentIssueId;
        if (ucVariantSlider.Variants.Count > 1)
        {
            IssueABVariantItem issueVariant = ucVariantSlider.Variants[0];
             currentIssueId= issueVariant.IssueID;
        }
        else
        {
             currentIssueId= ucVariantSlider.OriginalIssueID;
        }

        IssueId = currentIssueId;
        hdnIssueID.Value = currentIssueId.ToString();
        InitVariantSlider(currentIssueId);
        ucVariantSlider.SetVariant(currentIssueId);
        InitHeaderActions();
        editElem.IssueID = IssueId;
        editElem.ReloadData(true);
    }


    protected void ucVariantSlider_OnSelectionChanged(object sender, EventArgs e)
    {
        int currentIssueId = IssueId;
        int variantIndex = ucVariantSlider.CurrentVariant;
        if (variantIndex >= 0 && variantIndex < ucVariantSlider.Variants.Count)
        {
            IssueABVariantItem item = ucVariantSlider.Variants[variantIndex];
            currentIssueId = item.IssueID;
        }

        // CONTENT EDITING
        editElem.NewsletterID = newsletterId;
        editElem.IssueID = currentIssueId;
        editElem.ReloadData(true);
        editElem.UpdateContent();
    }

    #endregion
}
