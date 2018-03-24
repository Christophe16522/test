using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.Helpers;
using CMS.UIControls;

public partial class CMSModules_Forums_CMSPages_ForumPostApprove : CMSForumsPage
{
    #region "Page events"

    /// <summary>
    /// Handles the Load event of the Page control.
    /// </summary>
    protected void Page_Load(object sender, EventArgs e)
    {
        // Setup the modal dialog
        SetCulture();
        RegisterEscScript();

        // Get post ID
        int postId = QueryHelper.GetInteger("postid", 0);
        string mode = QueryHelper.GetString("mode", "approval");
        int userId = QueryHelper.GetInteger("userid", 0);

        if (mode.ToLowerInvariant() == "subscription")
        {
            PostApproveFooter.Mode = "subscription";
        }

        // Set the post ID
        PostApprove.PostID = PostApproveFooter.PostID = postId;
        PostApprove.UserID = PostApproveFooter.UserID = userId;

        // Page title
        PageTitle.TitleText = GetString("forums_forumnewpost_header.preview");
    }


    /// <summary>
    /// Raises the <see cref="E:PreRender"/> event.
    /// </summary>
    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        // Setup the modal dialog
        RegisterModalPageScripts();
    }

    #endregion
}