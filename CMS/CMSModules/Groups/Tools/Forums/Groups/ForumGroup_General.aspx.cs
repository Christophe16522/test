using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.Forums;
using CMS.Helpers;
using CMS.UIControls;

public partial class CMSModules_Groups_Tools_Forums_Groups_ForumGroup_General : CMSGroupForumPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        groupEdit.GroupID = QueryHelper.GetInteger("forumgroupid", 0);
        groupEdit.OnCheckPermissions += new CMSAdminControl.CheckPermissionsEventHandler(groupEdit_OnCheckPermissions);
        groupEdit.IsLiveSite = false;
    }


    private void groupEdit_OnCheckPermissions(string permissionType, CMSAdminControl sender)
    {
        int groupId = 0;

        ForumGroupInfo fgi = ForumGroupInfoProvider.GetForumGroupInfo(groupEdit.GroupID);
        if (fgi != null)
        {
            groupId = fgi.GroupGroupID;
        }

        // Check permissions
        CheckPermissions(groupId, CMSAdminControl.PERMISSION_MANAGE);
    }
}