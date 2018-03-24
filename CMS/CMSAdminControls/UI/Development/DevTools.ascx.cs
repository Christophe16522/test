using System;

using CMS.Helpers;
using CMS.Membership;
using CMS.PortalEngine;
using CMS.UIControls;
using CMS.Base;

public partial class CMSAdminControls_UI_Development_DevTools : CMSUserControl
{
    protected void Page_Load(object sender, EventArgs e)
    {
        CMSPage page = Parent.Page as CMSPage;

        var developmentMode = (page != null) ? page.CurrentMaster.DevelopmentMode : SystemContext.DevelopmentMode;
        if (developmentMode && MembershipContext.AuthenticatedUser.CheckPrivilegeLevel(UserPrivilegeLevelEnum.GlobalAdmin))
        {
            // Debug
            lnkDebug.NavigateUrl = "~/CMSModules/System/Debug/System_ViewRequest.aspx?guid=" + DebugContext.CurrentRequestLogs.RequestGUID;

            // UI Element
            var elem = UIContext.UIElement;
            if (elem != null)
            {
                ltlActions.Text += UIContextHelper.GetResourceUIElementsLink(elem.ElementResourceID, elem.ElementID);
            }

            // Localization
            btnLocalize.Image.ImageUrl = GetImageUrl("Objects/CMS_Culture/list.png");
            btnLocalize.Image.CausesValidation = false;

            imgDebug.ImageUrl = GetImageUrl("CMSModules/CMS_System/debug.png");
            imgDebug.ToolTip = imgDebug.AlternateText = GetString("Administration-System.Debug");

            // Do not move to the markup - could cause life cycle issues
            btnLocalize.HorizontalPosition = CMS.ExtendedControls.HorizontalPositionEnum.Right;
            btnLocalize.OffsetY = -20;
            btnLocalize.OffsetX = 1;
            btnLocalize.MouseButton = CMS.ExtendedControls.MouseButtonEnum.Both;
            btnLocalize.ContextMenuCssClass = "DevToolsContextMenu";
            btnLocalize.MenuControlPath = "~/CMSAdminControls/UI/Development/Localize.ascx";
        }
        else
        {
            Visible = false;
        }
    }
}
