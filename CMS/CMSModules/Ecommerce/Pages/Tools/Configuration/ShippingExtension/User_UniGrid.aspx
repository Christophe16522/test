<%@ Page Language="C#" AutoEventWireup="true" CodeFile="User_UniGrid.aspx.cs" Inherits="CMSModules_Ecommerce_Pages_Tools_Configuration_ShippingExtension_User_UniGrid"
    Theme="Default" %>

<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" TagName="UniGrid" TagPrefix="cms" %>
<%@ Register Namespace="CMS.UIControls.UniGridConfig" TagPrefix="ug" Assembly="CMS.UIControls" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <ajaxToolkit:ToolkitScriptManager ID="manScript" runat="server" EnableViewState="false" />
        <asp:Label runat="server" ID="lblInfo" EnableViewState="false" Visible="false" />
        <cms:UniGrid ID="UniGrid2" runat="server" >
            <GridActions>
                <ug:Action Name="edit" Caption="$General.Edit$" Icon="Edit.png" />
            </GridActions>
            <GridColumns>
                <ug:Column Source="itemID" Caption="itemID"/>
                <ug:Column Source ="ShippingOptionId" Caption ="ShippingOptionid"/>
                <ug:Column Source ="LocalContact" Caption ="Contact"/>
                <ug:Column Source ="Enabled" Caption ="Enabled" Width="100%"/>
            </GridColumns>
        </cms:UniGrid>
    </div>
    </form>
</body>
</html>
