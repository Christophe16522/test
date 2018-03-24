using System;

using CMS.Base;
using CMS.Helpers;
using CMS.PortalEngine;
using CMS.UIControls;

public partial class CMSAdminControls_UI_ContextHelp : CMSUserControl
{
    protected void Page_Load(object sender, EventArgs e)
    {
        InitializeVersion();

        ScriptHelper.RegisterModule(Page, "CMS/ContextHelp", new
        {
            wrapperId = ClientID,
            toolbarId = pnlToolbar.ClientID,
            helpTopicsMenuItemId = helpTopics.ClientID,
            searchMenuItemId = search.ClientID,
            searchUrlPattern = UIContextHelper.GetDocumentationSearchUrlPattern(),
            descriptionMenuItemId = description.ClientID
        });
    }


    private void InitializeVersion()
    {
        string version = "v";

        if (SystemContext.DevelopmentMode)
        {
            version += CMSVersion.GetVersion(true, true, true, false, true);
        }
        else
        {
            if (ValidationHelper.GetInteger(CMSVersion.HotfixVersion, 0) > 0)
            {
                version += CMSVersion.GetVersion(true, true, true, true);
            }
            else
            {
                version += CMSVersion.MainVersion;
            }
        }
        lblVersion.Text = version;
    }
}