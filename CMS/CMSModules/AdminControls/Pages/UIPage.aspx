<%@ Page Language="C#" AutoEventWireup="true" CodeFile="UIPage.aspx.cs" MasterPageFile="~/CMSMasterPages/UI/UIPage.master"
    EnableEventValidation="false" Inherits="CMSModules_AdminControls_Pages_UIPage" Theme="Default" %>

<asp:Content runat="server" ID="cplcContent" ContentPlaceHolderID="plcContent">
    <asp:PlaceHolder runat="server" ID="plcManagers">
        <cms:CMSPortalManager ID="manPortal" ShortID="m" runat="server" EnableViewState="false" />
    </asp:PlaceHolder>
    <cms:CMSPlaceHolder ID="plcHeader" runat="server" />
    <cms:CMSPagePlaceholder ID="plc" runat="server" Root="true" />
    <cms:CMSPlaceHolder ID="plcFooter" runat="server" />
</asp:Content>
