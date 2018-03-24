using System;

using CMS.UIControls;
using CMS.ExtendedControls;
using CMS.PortalEngine;
using CMS.Helpers;

public partial class CMSPages_PortalTemplate : PortalPage
{
    #region "Properties"

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
            docMan.RegisterSaveChangesScript = (PortalContext.ViewMode.IsOneOf(ViewModeEnum.Edit, ViewModeEnum.EditLive));
            return docMan;
        }
    }

    #endregion


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        // Init the header tags
        tags.Text = HeaderTags;

        if (PortalContext.ViewMode.IsWireframe())
        {
            CSSHelper.RegisterWireframesMode(this);
        }
    }
}