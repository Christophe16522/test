using System;
using System.Data;
using System.Collections;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.CMSHelper;
using CMS.Community;
using CMS.FormEngine;
using CMS.GlobalHelper;
using CMS.LicenseProvider;
using CMS.SettingsProvider;
using CMS.SiteProvider;
using CMS.UIControls;
using CMS.Helpers;
using CMS.DataEngine;
using CMS.Modules;

public partial class CMSModules_Groups_Tools_Group_Edit_Header : CMSGroupPage
{
    protected int groupId = 0;
    protected string groupDisplayName = "";


    protected void Page_Load(object sender, EventArgs e)
    {
        // Get the group ID and the group InfoObject
        groupId = QueryHelper.GetInteger("groupid", 0);
        GroupInfo gi = GroupInfoProvider.GetGroupInfo(groupId);
        if (gi != null)
        {
            groupDisplayName = HTMLHelper.HTMLEncode(gi.GroupDisplayName);
        }

        // Page title
        CurrentMaster.Title.TitleText = GetString("Group.EditHeaderCaption");
        CurrentMaster.Title.TitleImage = GetImageUrl("Objects/Community_Group/object.png");

        CurrentMaster.Title.HelpTopicName = "group_general";
        CurrentMaster.Title.HelpName = "helpTopic";

        // Pagetitle breadcrumbs		
        string[,] pageTitleTabs = new string[2,3];
        pageTitleTabs[0, 0] = GetString("Group.ItemListLink");
        pageTitleTabs[0, 1] = "~/CMSModules/Groups/Tools/Group_List.aspx";
        pageTitleTabs[0, 2] = "_parent";
        pageTitleTabs[1, 0] = groupDisplayName;
        pageTitleTabs[1, 1] = "";
        pageTitleTabs[1, 2] = "";
        CurrentMaster.Title.Breadcrumbs = pageTitleTabs;

        // Tabs
        string[,] tabs = new string[10,4];
        
        // General
        tabs[0, 0] = GetString("General.General");
        tabs[0, 1] = "SetHelpTopic('helpTopic', 'group_general');";
        tabs[0, 2] = "Group_Edit_General.aspx?groupID=" + groupId;

        // Custom fields
        FormInfo formInfo = FormHelper.GetFormInfo(PredefinedObjectType.GROUP, false);
        if ((formInfo != null) && formInfo.GetFormElements(true, false, true).Any())
        {
            tabs[1, 0] = GetString("general.customfields");
            tabs[1, 2] = "Group_Edit_CustomFields.aspx?groupID=" + groupId;
        }

        // Security
        tabs[2, 0] = GetString("General.Security");
        tabs[2, 1] = "SetHelpTopic('helpTopic', 'group_security');";
        tabs[2, 2] = "Security/Security.aspx?groupID=" + groupId;
        
        // Members
        tabs[3, 0] = GetString("Group.Members");
        tabs[3, 1] = "SetHelpTopic('helpTopic', 'group_members_list');";
        tabs[3, 2] = "Members/Member_List.aspx?groupID=" + groupId;

        if (ResourceSiteInfoProvider.IsResourceOnSite("CMS.Roles", SiteContext.CurrentSiteName))
        {
            tabs[4, 0] = GetString("general.roles");
            tabs[4, 1] = "SetHelpTopic('helpTopic', 'group_roles_list');";
            tabs[4, 2] = "Roles/Role_List.aspx?groupID=" + groupId;
        }

        if (ResourceSiteInfoProvider.IsResourceOnSite("CMS.Forums", SiteContext.CurrentSiteName))
        {
            tabs[5, 0] = GetString("group_general.forums");
            tabs[5, 1] = "SetHelpTopic('helpTopic', 'forum_list');";
            tabs[5, 2] = "Forums/Groups/ForumGroups_List.aspx?groupid=" + groupId;
        }

        if (ResourceSiteInfoProvider.IsResourceOnSite("CMS.MediaLibrary", SiteContext.CurrentSiteName))
        {
            tabs[6, 0] = GetString("Group.MediaLibrary");
            tabs[6, 1] = "SetHelpTopic('helpTopic', 'library_list');";
            tabs[6, 2] = "MediaLibrary/Library_List.aspx?groupid=" + groupId;
        }

        if (ResourceSiteInfoProvider.IsResourceOnSite("CMS.MessageBoards", SiteContext.CurrentSiteName))
        {
            tabs[7, 0] = GetString("Group.MessageBoards");
            tabs[7, 1] = "SetHelpTopic('helpTopic', 'group_messageboard');";
            tabs[7, 2] = "MessageBoards/Boards_Default.aspx?groupid=" + groupId;
        }

        if (ResourceSiteInfoProvider.IsResourceOnSite("CMS.Polls", SiteContext.CurrentSiteName))
        {
            tabs[8, 0] = GetString("Group.Polls");
            tabs[8, 1] = "SetHelpTopic('helpTopic', 'polls_list');";
            tabs[8, 2] = "Polls/Polls_List.aspx?groupID=" + groupId;
        }

        // Check whether license for project management is avilable
        // if no hide project management tab
        if (LicenseHelper.CheckFeature(URLHelper.GetCurrentDomain(), FeatureEnum.ProjectManagement))
        {
            // Check site availability
            if (ResourceSiteInfoProvider.IsResourceOnSite("CMS.ProjectManagement", SiteContext.CurrentSiteName))
            {
                tabs[9, 0] = ResHelper.GetString("pm.project.list");
                tabs[9, 1] = "SetHelpTopic('helpTopic', 'CMS_ProjectManagement_Projects');";
                tabs[9, 2] = "ProjectManagement/Project/List.aspx?groupid=" + groupId;
            }
        }

        CurrentMaster.Tabs.Tabs = tabs;
        CurrentMaster.Tabs.UrlTarget = "content";
    }
}