using System;
using System.Threading;

using CMS.Helpers;
using CMS.PortalEngine;
using CMS.Base;
using CMS.UIControls;
using CMS.ExtendedControls.ActionsConfig;

public partial class CMSModules_Widgets_UI_Widget_Edit_Documentation : GlobalAdminPage
{
    #region "Variables"

    private int widgetId = 0;

    #endregion


    #region "Page events"

    /// <summary>
    /// Handles the Load event of the Page control.
    /// </summary>
    protected void Page_Load(object sender, EventArgs e)
    {
        widgetId = QueryHelper.GetInteger("widgetid", 0);

        Title = "Widget part documentation";

        // Resource string
        btnOk.Text = GetString("General.Ok");

        WidgetInfo wi = WidgetInfoProvider.GetWidgetInfo(widgetId);

        // set Documentation header - "View documentation" + "Generate Documentation"
        if (wi != null)
        {

            HeaderAction action = new HeaderAction();
            action.Text = GetString("webparteditdocumentation.view");
            action.RedirectUrl = "~/CMSModules/Widgets/Dialogs/WidgetDocumentation.aspx?widgetid=" + wi.WidgetName;
            action.Target = "_blank";
            CurrentMaster.HeaderActions.AddAction(action);

            if (SystemContext.DevelopmentMode)
            {
                HeaderAction generate = new HeaderAction();
                generate.Text = GetString("webparteditdocumentation.generate");
                generate.RedirectUrl = "~/CMSPages/Dialogs/Documentation.aspx?widget=" + wi.WidgetName;
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
            if (wi != null)
            {
                htmlText.ResolvedValue = wi.WidgetDocumentation;
            }
        }
    }


    /// <summary>
    /// OK click handler, save changes.
    /// </summary>
    protected void btnOk_Click(object sender, EventArgs e)
    {
        WidgetInfo wi = WidgetInfoProvider.GetWidgetInfo(widgetId);
        if (wi != null)
        {
            wi.WidgetDocumentation = htmlText.ResolvedValue;
            WidgetInfoProvider.SetWidgetInfo(wi);

            ShowChangesSaved();
        }
    }

    #endregion
}