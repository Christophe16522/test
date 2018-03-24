using System;

using CMS.OnlineMarketing;
using CMS.UIControls;
using CMS.WorkflowEngine;

// Edited object
[EditedObject(WorkflowActionInfo.OBJECT_TYPE_AUTOMATION, "objectId")]
public partial class CMSModules_ContactManagement_Pages_Tools_Automation_Action_Tab_General : CMSContactManagementConfigurationPage
{
    protected override void OnPreInit(EventArgs e)
    {
        base.OnPreInit(e);

        // Only global administrator can access automation process actions
        if (!CurrentUser.IsGlobalAdministrator)
        {
            RedirectToAccessDenied(GetString("security.accesspage.onlyglobaladmin"));
        }
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        editElem.ObjectType = WorkflowActionInfo.OBJECT_TYPE_AUTOMATION;
    }
}