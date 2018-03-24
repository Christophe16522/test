using System;
using System.Linq;

using CMS.Core;
using CMS.DataEngine;
using CMS.UIControls;
using CMS.ExtendedControls;
using CMS.PortalEngine;
using CMS.Helpers;
using CMS.DocumentEngine;
using CMS.Modules;
using CMS.PortalControls;
using CMS.Base;

[UIElement(ModuleName.CMS, "Modules.UserInterface.Design")]
public partial class CMSModules_Modules_Pages_Module_UserInterface_Design : CMSUIPage
{
    #region "Public properties"

    /// <summary>
    /// Document manager
    /// </summary>
    public override ICMSDocumentManager DocumentManager
    {
        get
        {
            // Enable document manager
            docMan.Visible = true;
            docMan.StopProcessing = false;
            docMan.MessagesPlaceHolder.UseRelativePlaceHolder = false;
            return docMan;
        }
    }


    /// <summary>
    /// Local page messages placeholder
    /// </summary>
    public override MessagesPlaceHolder MessagesPlaceHolder
    {
        get
        {
            return DocumentManager.MessagesPlaceHolder;
        }
    }

    #endregion


    #region "Page methods"

    /// <summary>
    /// PreInit event handler.
    /// </summary>
    protected override void OnPreInit(EventArgs e)
    {
        base.OnPreInit(e);

        // Init the page components
        manPortal.SetMainPagePlaceholder(plc);

        var ui = UIElementInfoProvider.GetUIElementInfo(QueryHelper.GetInteger("elementid", 0));

        // Clear UIContext data of element "Modules.UserInterface.Design" (put by UIElement attribute to check permissions)
        UIContext.Data = null;

        if (ui != null)
        {
            UIContext.UIElement = ui;

            // Store resource name
            UIContext.ResourceName = UIContextHelper.GetResourceName(ui.ElementResourceID);

            int pageTemplateId = ui.ElementPageTemplateID;

            // Prepare the page info
            PageInfo pi = PageInfoProvider.GetVirtualPageInfo(pageTemplateId);
            pi.DocumentNamePath = "/" + ResHelper.GetString("edittabs.design");

            DocumentContext.CurrentPageInfo = pi;

            // Set the design mode
            bool enable = (SystemContext.DevelopmentMode || (ui.ElementResourceID == QueryHelper.GetInteger("moduleId", 0) && ui.ElementIsCustom));
            PortalContext.SetRequestViewMode(ViewModeEnum.Design);

            // If displayed module is not selected, disable design mode
            if (!enable)
            {
                plc.ViewMode = ViewModeEnum.DesignDisabled;
            }

            ContextHelper.Add("DisplayContentInDesignMode", PortalHelper.DisplayContentInUIElementDesignMode, true, false, false, DateTime.MinValue);

            ManagersContainer = plcManagers;
            ScriptManagerControl = manScript;
        }
    }

    #endregion
}
