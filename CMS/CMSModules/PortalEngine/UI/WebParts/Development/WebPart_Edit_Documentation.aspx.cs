using System;
using System.Threading;

using CMS.Helpers;
using CMS.PortalEngine;
using CMS.Base;
using CMS.UIControls;
using CMS.ExtendedControls.ActionsConfig;

public partial class CMSModules_PortalEngine_UI_WebParts_Development_WebPart_Edit_Documentation : GlobalAdminPage
{
    private int webpartId = 0;


    protected void Page_Load(object sender, EventArgs e)
    {
        webpartId = QueryHelper.GetInteger("webpartid", 0);

        Title = "Web part documentation";

        // Resource string
        btnOk.Text = GetString("General.Ok");
        
        WebPartInfo wpi = WebPartInfoProvider.GetWebPartInfo(webpartId);
        EditedObject = wpi;
        if (wpi != null)
        {
            HeaderAction action = new HeaderAction();
            action.Text = GetString("webparteditdocumentation.view");
            action.RedirectUrl = "~/CMSModules/PortalEngine/UI/WebParts/WebPartDocumentationPage.aspx?webpartid=" + wpi.WebPartName;
            action.Target = "_blank";
            CurrentMaster.HeaderActions.AddAction(action);

            if (SystemContext.DevelopmentMode)
            {
                HeaderAction generate = new HeaderAction();
                generate.Text = GetString("webparteditdocumentation.generate");
                generate.RedirectUrl = "~/CMSPages/Dialogs/Documentation.aspx?webpart=" + wpi.WebPartName;
                generate.Target = "_blank";
                CurrentMaster.HeaderActions.AddAction(generate);
            }
        }

        // HTML editor settings        
        htmlText.AutoDetectLanguage = false;
        htmlText.DefaultLanguage = Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName;
        htmlText.EditorAreaCSS = "";
        htmlText.ToolbarSet = "SimpleEdit";

        // Load data
        if (!RequestHelper.IsPostBack())
        {
            if (wpi != null)
            {
                htmlText.ResolvedValue = wpi.WebPartDocumentation;
            }
        }
    }


    /// <summary>
    /// OK click handler, save changes.
    /// </summary>
    protected void btnOk_Click(object sender, EventArgs e)
    {
        WebPartInfo wpi = WebPartInfoProvider.GetWebPartInfo(webpartId);
        if (wpi != null)
        {
            wpi.WebPartDocumentation = htmlText.ResolvedValue;
            WebPartInfoProvider.SetWebPartInfo(wpi);

            ShowChangesSaved();
        }
    }
}