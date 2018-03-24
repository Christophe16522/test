<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_Ecommerce_Controls_UI_OrderList"
    CodeFile="OrderList.ascx.cs" %>
<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" TagName="UniGrid" TagPrefix="cms" %>
<cms:CMSUpdatePanel ID="pnlUpdate" runat="server" UpdateMode="Always">
    <ContentTemplate>
        <cms:UniGrid runat="server" ID="gridElem" GridName="Order_List.xml" OrderBy="OrderDate DESC" FilterLimit="0"  DisplayFilter="true"
            RememberStateByParam="customerId" RememberDefaultState="true" Columns="OrderID,facture,OrderInvoiceNumber,CustomerFirstName,CustomerLastName,CustomerCompany,CustomerEmail,OrderDate,
            OrderTotalPriceInMainCurrency,CurrencyIsMain,CurrencyFormatString,OrderTotalPrice,StatusDisplayName,StatusColor,OrderPaymentOptionID,OrderIsPaid,OrderShippingOptionID,OrderTrackingNumber,OrderNote,OrderSiteID" />
   </ContentTemplate>
</cms:CMSUpdatePanel>