using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.Membership;
using CMS.SiteProvider;
using CMS.UIControls;

public partial class CMSModules_MyDesk_RecycleBin_RecycleBin_Documents : CMSContentManagementPage
{
    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        // Check UIProfile
        if (!MembershipContext.AuthenticatedUser.IsAuthorizedPerUIElement("CMS.Content", new string[] { "MyRecycleBin", "MyRecycleBin.Documents" }, SiteContext.CurrentSiteName))
        {
            RedirectToUIElementAccessDenied("CMS.Content", "MyRecycleBin;MyRecycleBin.Documents");
        }

        recycleBin.SiteName = SiteContext.CurrentSiteName;
    }

    #endregion
}