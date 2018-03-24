using System;

using CMS.Helpers;
using CMS.SiteProvider;
using CMS.UIControls;
using CMS.DataEngine;


public partial class CMSPages_Dialogs_UserRegistration : CMSPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        PageTitle title = PageTitle;
        title.TitleText = ResHelper.GetString("mem.reg.approvaltext");

        // Set administrator e-mail
        registrationApproval.AdministratorEmail = SettingsKeyInfoProvider.GetStringValue(SiteContext.CurrentSiteName + ".CMSAdminEmailAddress");
        registrationApproval.FromAddress = SettingsKeyInfoProvider.GetStringValue(SiteContext.CurrentSiteName + ".CMSNoreplyEmailAddress");
    }
}