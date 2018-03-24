using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.Helpers;
using CMS.LicenseProvider;
using CMS.Membership;
using CMS.OnlineMarketing;
using CMS.Base;
using CMS.SiteProvider;
using CMS.UIControls;
using CMS.WebAnalytics;

// Edited object
[EditedObject(MVTVariantInfo.OBJECT_TYPE, "variantid")]
// Breadcrumbs
[Breadcrumbs()]
[Breadcrumb(0, "mvtvariant.list", "~/CMSModules/OnlineMarketing/Pages/Content/MVTVariant/List.aspx?nodeid={?nodeid?}", null)]
[Breadcrumb(1, Text = "{%EditedObject.DisplayName%}", ExistingObject = true)]
[Breadcrumb(1, ResourceString = "mvtvariant.new", NewObject = true)]
// Context help
[Help("mvtvariant_edit")]
public partial class CMSModules_OnlineMarketing_Pages_Content_MVTVariant_Edit : CMSMVTestPage
{
    #region "Page events"

    /// <summary>
    /// Handles the Load event of the Page control.
    /// </summary>
    protected void Page_Load(object sender, EventArgs e)
    {
        // Check UI Permissions
        if ((MembershipContext.AuthenticatedUser == null) || (!MembershipContext.AuthenticatedUser.IsAuthorizedPerUIElement("CMS.Content", "OnlineMarketing.MVTVariants")))
        {
            RedirectToUIElementAccessDenied("CMS.Content", "OnlineMarketing.MVTVariants");
        }

        String siteName = SiteContext.CurrentSiteName;

        // Set disabled module info
        ucDisabledModule.SettingsKeys = "CMSAnalyticsEnabled;CMSMVTEnabled";
        ucDisabledModule.InfoTexts.Add(GetString("WebAnalytics.Disabled") + "</br>");
        ucDisabledModule.InfoTexts.Add(GetString("mvt.disabled"));
        ucDisabledModule.ParentPanel = pnlDisabled;
    }

    #endregion
}