<%@ Control Language="C#" AutoEventWireup="true" CodeFile="DevTools.ascx.cs" Inherits="CMSAdminControls_UI_Development_DevTools" %>
    <div class="DevTools cms-bootstrap">
        <div class="DevContent">
            <asp:Literal runat="server" ID="ltlActions" EnableViewState="False" />
            <asp:HyperLink runat="server" ID="lnkDebug" EnableViewState="false" CssClass="ContextMenuButton" Target = "_blank">
                <asp:Image runat="server" ID="imgDebug" EnableViewState="false" />
            </asp:HyperLink>
            <cms:ContextMenuButton runat="server" ID="btnLocalize" />
        </div>
    </div>
