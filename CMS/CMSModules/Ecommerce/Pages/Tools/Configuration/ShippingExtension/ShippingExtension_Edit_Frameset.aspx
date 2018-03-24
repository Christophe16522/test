<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_Ecommerce_Pages_Tools_Configuration_ShippingExtension_ShippingExtension_Edit_Frameset"
    CodeFile="ShippingExtension_Edit_Frameset.aspx.cs" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Frameset//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-frameset.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server" enableviewstate="false">
    <title>Shopping extension - Edit</title>
</head>
<frameset border="0" rows="<%=TabsBreadHeadFrameHeight%>, *" id="rowsFrameset">
    <frame name="shippingExtensionHeader" src="ShippingExtension_Edit_Header.aspx?shippingExtensionID=<%=QueryHelper.GetInteger("shippingOptionID", 0)%>&siteId=<%=SiteID%>"
        scrolling="no" frameborder="0" noresize="noresize" />
    <frame name="shippingExtensionContent" src="ShippingExtension_Edit_General.aspx?ShippingExtensionID=<%=QueryHelper.GetInteger("ShippingOptionID", 0)%>&saved=<%=QueryHelper.GetInteger("saved", 0)%> "
        scrolling="auto" frameborder="0" noresize="noresize" />
    <cms:NoFramesLiteral ID="ltlNoFrames" runat="server" />
</frameset>
</html>
