using System;
using System.Data;

using CMS.Forums;
using CMS.Helpers;
using CMS.Membership;
using CMS.UIControls;

public partial class CMSModules_Forums_Controls_Posts_PostApproveFooter : CMSAdminEditControl
{
    #region "Variables"

    // Current PostID
    private string mMode = "approval";


    public CMSModules_Forums_Controls_Posts_PostApproveFooter()
    {
        PostID = 0;
        UserID = 0;
    }

    #endregion


    #region "Public properties"

    /// <summary>
    /// Gets or sets the post ID.
    /// </summary>
    public int PostID
    {
        get;
        set;
    }


    /// <summary>
    /// Gets or sets the user ID.
    /// </summary>
    public int UserID
    {
        get;
        set;
    }


    /// <summary>
    /// Gets or sets mode of footer control
    /// </summary>
    public string Mode
    {
        get
        {
            return mMode;
        }
        set
        {
            mMode = value;
        }
    }

    #endregion


    #region "Page events"

    /// <summary>
    /// Handles the Load event of the Page control.
    /// </summary>
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Mode.ToLowerInvariant() != "subscription")
        {
            // Button titles
            btnApprove.Text = GetString("general.approve");
            btnDelete.Text = GetString("general.delete");
            
            
            // Button actions
            btnDelete.OnClientClick = "return confirm(" + ScriptHelper.GetString(GetString("forummanage.deleteconfirm")) + ");";
            
            btnUnsubscribe.Visible = false;
        }
        else
        {
            btnApprove.Visible = false;
            btnDelete.Visible = false;

            btnUnsubscribe.Text = GetString("general.unsubscription_confirmbutton");
            btnUnsubscribe.OnClientClick = "return confirm(" + ScriptHelper.GetString(GetString("forumpost.confirmunsubscribe")) + ");";
            
        }
    }


    /// <summary>
    /// Handles the Click event of the btnApprove control.
    /// </summary>
    protected void btnUnsubscribe_Click(object sender, EventArgs e)
    {
        if (UserID != MembershipContext.AuthenticatedUser.UserID)
        {
            // Check permissions
            if (!CheckPermissions("cms.forums", PERMISSION_MODIFY))
            {
                return;
            }
        }

        DataSet ds = ForumSubscriptionInfoProvider.GetSubscriptions("(SubscriptionUserID = " + UserID + ") AND (SubscriptionPostID = " + PostID + ") AND (ISNULL(SubscriptionApproved, 1) = 1)", null, 0, "SubscriptionID");
        if (!DataHelper.DataSourceIsEmpty(ds))
        {
            ForumSubscriptionInfo fsi = new ForumSubscriptionInfo(ds.Tables[0].Rows[0]);
            ForumSubscriptionInfoProvider.DeleteForumSubscriptionInfo(fsi);
            RefreshParentWindow();
        }
        
    }


    /// <summary>
    /// Handles the Click event of the btnApprove control.
    /// </summary>
    protected void btnApprove_Click(object sender, EventArgs e)
    {
        // Check permissions
        if (!CheckPermissions("cms.forums", PERMISSION_MODIFY))
        {
            return;
        }

        // Approve the post
        ForumPostInfo fpi = ForumPostInfoProvider.GetForumPostInfo(ValidationHelper.GetInteger(PostID, 0));
        if (fpi != null)
        {
            fpi.PostApprovedByUserID = MembershipContext.AuthenticatedUser.UserID;
            fpi.PostApproved = true;
            ForumPostInfoProvider.SetForumPostInfo(fpi);
        }

        // Reload the parent window
        RefreshParentWindow();
    }


    /// <summary>
    /// Handles the Click event of the btnDelete control.
    /// </summary>
    protected void btnDelete_Click(object sender, EventArgs e)
    {
        // Check permissions
        if (!CheckPermissions("cms.forums", PERMISSION_MODIFY))
        {
            return;
        }

        // Delete the post
        ForumPostInfoProvider.DeleteForumPostInfo(ValidationHelper.GetInteger(PostID, 0));

        // Reload the parent window
        RefreshParentWindow();
    }

    #endregion


    #region "Private methods"

    /// <summary>
    /// Closes this dialog and reloads the parent window.
    /// </summary>
    private void RefreshParentWindow()
    {
        string script = @"
            function RefreshParentWindow()
            {
                if (wopener.RefreshPage) {
                    wopener.RefreshPage();
                }
                CloseDialog();
            }

            window.onload = RefreshParentWindow;";

        ltrScript.Text = ScriptHelper.GetScript(script);
    }

    #endregion
}