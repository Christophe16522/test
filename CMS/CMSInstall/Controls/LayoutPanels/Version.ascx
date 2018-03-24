<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Version.ascx.cs" Inherits="CMSInstall_Controls_LayoutPanels_Version" %>
<asp:Panel ID="pnlVersion" runat="server" CssClass="InstallerFooter" EnableViewState="False">
    <div class="AppSupport">
        <asp:Label ID="lblSupport" runat="server" />
    </div>
    <div class="AppVersion">
        <asp:Label ID="lblVersion" runat="server" />
    </div>
    <div class="ClearBoth">
    </div>
</asp:Panel>
