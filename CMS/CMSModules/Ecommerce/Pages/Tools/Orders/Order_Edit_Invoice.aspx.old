<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_Ecommerce_Pages_Tools_Orders_Order_Edit_Invoice"
    Theme="Default" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" CodeFile="Order_Edit_Invoice.aspx.cs" %>

<asp:Content ID="cntHeader" ContentPlaceHolderID="plcBeforeContent" runat="server">
    <asp:Literal ID="ltlScript" runat="server" EnableViewState="false" />
    <asp:Panel ID="pnlHeader" runat="server" CssClass="PageHeaderLine SiteHeaderLine"
        EnableViewState="false">
        <asp:Label ID="lblInvoiceNumber" runat="server" EnableViewState="false" />
        <cms:CMSTextBox ID="txtInvoiceNumber" runat="server" MaxLength="200" EnableViewState="false" />
        <cms:CMSButton ID="btnGenerate" runat="server" OnClick="btnGenerate_Click" CssClass="LongButton"
            EnableViewState="false" />
        <cms:CMSButton ID="btnPrintPreview" runat="server" OnClientClick="showPrintPreview(); return false;"
            CssClass="LongButton" EnableViewState="false" />
    </asp:Panel>
</asp:Content>
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <asp:Label ID="lblInvoice" runat="server" EnableViewState="false" />
</asp:Content>
