using System;

using CMS.Core;
using CMS.Helpers;
using CMS.OnlineMarketing;
using CMS.SiteProvider;
using CMS.UIControls;

// Edited object
[EditedObject(ContactGroupInfo.OBJECT_TYPE, "groupId")]
[Security(Resource = ModuleName.ONLINEMARKETING, UIElements = "EditContactGroup;ContactGroupGeneral")]
public partial class CMSModules_ContactManagement_Pages_Tools_ContactGroup_Tab_General : CMSContactManagementContactGroupsPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        editElem.SiteID = QueryHelper.GetInteger("siteid", SiteContext.CurrentSiteID);
    }
}