using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.Helpers;
using CMS.PortalEngine;
using CMS.UIControls;

public partial class CMSModules_PortalEngine_UI_WebParts_Development_WebPart_Edit_CSS : GlobalAdminPage
{
    protected override void CreateChildControls()
    {
        // Get 'webpartid' from query string.
        int webPartId = QueryHelper.GetInteger("webpartid", 0);
        WebPartInfo wpi = WebPartInfoProvider.GetWebPartInfo(webPartId);
        EditedObject = wpi;
        if (wpi != null)
        {
            ucHierarchy.PreviewObjectName = wpi.WebPartName + "_css";
            ucHierarchy.PreviewURLSuffix = "&previewobjectidentifier=" + wpi.WebPartName;

            UIContext.EditedObject = wpi;
        }

        base.CreateChildControls();
    }
}