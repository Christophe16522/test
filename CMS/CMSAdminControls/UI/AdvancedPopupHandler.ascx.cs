using System;

using CMS.GlobalHelper;
using CMS.UIControls;
using CMS.CMSHelper;
using CMS.Helpers;
using CMS.PortalEngine;

public partial class CMSAdminControls_UI_AdvancedPopupHandler : CMSUserControl
{
    #region "Variables"

    private bool mSetTitle = true;

    #endregion


    #region "Properties"

    /// <summary>
    /// Determines whether to use autotitle script.
    /// </summary>
    public bool SetTitle
    {
        get
        {
            return mSetTitle;
        }
        set
        {
            mSetTitle = value;
        }
    }

    #endregion


    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
			ScriptHelper.RegisterJQueryUI(Page);
        ScriptHelper.RegisterScriptFile(Page, "DragAndDrop/dragiframe.js");

        // Use classic dialogs for devices with touch screen
        bool useClassicDialogs = UIHelper.ClassicDialogs;
        useClassicDialogs |= DeviceContext.CurrentDevice.IsMobile;

        // When this control is not visible, classic dialogs will be used
        Visible &= !useClassicDialogs;

    }

    #endregion
}