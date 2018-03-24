using System;

using CMS.Core;
using CMS.Helpers;
using CMS.OnlineMarketing;
using CMS.SiteProvider;
using CMS.UIControls;

// Title
[Title("om.contactgroup.new")]
[UIElement(ModuleName.ONLINEMARKETING, "ContactGroups")]
public partial class CMSModules_ContactManagement_Pages_Tools_ContactGroup_New : CMSContactManagementContactGroupsPage
{
    private int siteId = 0;


    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        siteId = QueryHelper.GetInteger("siteid", SiteContext.CurrentSiteID);

        string url = ResolveUrl("~/CMSModules/ContactManagement/Pages/Tools/ContactGroup/List.aspx");
        url = URLHelper.AddParameterToUrl(url, "siteid", siteId.ToString());
        if (IsSiteManager)
        {
            url = URLHelper.AddParameterToUrl(url, "issitemanager", "1");
        }

        SetBreadcrumb(0, GetString("om.contactgroup.list"), url, null, null);
        SetBreadcrumb(1, GetString("om.contactgroup.new"), null, null, null);

        editElem.SiteID = siteId;
    }

    #endregion
}