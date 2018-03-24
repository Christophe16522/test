using System;

using CMS.Community;
using CMS.SettingsProvider;
using CMS.UIControls;
using CMS.DataEngine;

[EditedObject(PredefinedObjectType.GROUP, "groupid")]
public partial class CMSModules_Groups_Tools_Group_Edit_CustomFields : CMSGroupPage
{
    private GroupInfo mEditedGroup;

    private GroupInfo EditedGroup
    {
        get
        {
            return mEditedGroup ?? (mEditedGroup = EditedObject as GroupInfo);
        }
    }


    protected void Page_Init(object sender, EventArgs e)
    {
        if (EditedGroup != null)
        {
            CheckGroupPermissions(EditedGroup.GroupID, CMSAdminControl.PERMISSION_READ);

            formCustomFields.Info = EditedGroup;
            formCustomFields.BasicForm.HideSystemFields = true;
        }

        formCustomFields.OnBeforeSave += formCustomFields_OnBeforeSave;
    }


    void formCustomFields_OnBeforeSave(object sender, EventArgs e)
    {
        CheckGroupPermissions(EditedGroup.GroupID, CMSAdminControl.PERMISSION_MANAGE);
    }
}