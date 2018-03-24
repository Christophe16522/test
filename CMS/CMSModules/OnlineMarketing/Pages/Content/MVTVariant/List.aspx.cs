using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.Membership;
using CMS.UIControls;
using CMS.DocumentEngine;

public partial class CMSModules_OnlineMarketing_Pages_Content_MVTVariant_List : CMSMVTestPage
{
    #region "Methods"

    /// <summary>
    /// Handles the Load event of the Page control.
    /// </summary>
    protected void Page_Load(object sender, EventArgs e)
    {
        var cui = MembershipContext.AuthenticatedUser;

        // Check UI Permissions
        if (!cui.IsAuthorizedPerUIElement("CMS.Content", "OnlineMarketing.MVTVariants"))
        {
            RedirectToUIElementAccessDenied("CMS.Content", "OnlineMarketing.MVTVariants");
        }

        if (!cui.IsAuthorizedPerResource("cms.mvtest", "Read"))
        {
            RedirectToAccessDenied(String.Format(GetString("general.permissionresource"), "Read", "MVT testing"));
        }
        
        // Set disabled module info
        ucDisabledModule.SettingsKeys = "CMSAnalyticsEnabled;CMSMVTEnabled";
        ucDisabledModule.InfoTexts.Add(GetString("WebAnalytics.Disabled") + "</br>");
        ucDisabledModule.InfoTexts.Add(GetString("mvt.disabled"));
        ucDisabledModule.ParentPanel = pnlDisabled;

        // Get the PageTemplateID of the current node
        if (Node != null)
        {
            // Set NodeID in order to check the access to the document
            listElem.NodeID = Node.NodeID;
            listElem.PageTemplateID = Node.GetUsedPageTemplateId();
        }
        EditedObject = Node;
    }

    #endregion
}