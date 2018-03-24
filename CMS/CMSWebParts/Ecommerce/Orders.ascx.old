<%@ Control Language="C#" AutoEventWireup="true" CodeFile="~/CMSWebParts/Ecommerce/Orders.ascx.cs" Inherits="CMSWebParts_Ecommerce_Orders" %>
<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" TagName="UniGrid" TagPrefix="cms" %>
<%@ Register Namespace="CMS.UIControls.UniGridConfig" TagPrefix="ug" Assembly="CMS.UIControls" %>
<%@ Register TagPrefix="cms" Assembly="CMS.ExtendedControls" Namespace="CMS.ExtendedControls" %>
<cms:MessagesPlaceHolder ID="plcMessages" runat="server" WrapperControlID="pnlMessages" />
<cms:UniGrid runat="server" ID="gridElem" ObjectType="ecommerce.orderlist" RememberStateByParam="customerId"
    RememberDefaultState="true" Columns="OrderID,OrderInvoiceNumber,CustomerFirstName,CustomerLastName,CustomerCompany,CustomerEmail,OrderDate,
            OrderTotalPriceInMainCurrency,CurrencyIsMain,CurrencyFormatString,OrderTotalPrice,StatusDisplayName,StatusColor,OrderPaymentOptionID,OrderIsPaid,OrderShippingOptionID,OrderTrackingNumber,OrderNote,OrderSiteID">
    <GridActions>
        <ug:Action Name="edit" ExternalSourceName="Edit" Caption="$General.Edit$" Icon="Edit.png" />
        <ug:Action Name="delete" ExternalSourceName="Delete" Caption="$General.Delete$" Icon="Delete.png"
            Confirmation="$General.ConfirmDelete$" />
        <ug:Action Name="previous" ExternalSourceName="StatusMovePrevious" Caption="$Unigrid.Order.Actions.PreviousStatus$"
            Icon="Left.png" />
        <ug:Action Name="next" ExternalSourceName="StatusMoveNext" Caption="$Unigrid.Order.Actions.NextStatus$"
            Icon="Right.png" />
    </GridActions>
    <GridColumns>
        <ug:Column Name="IDAndInvoice" Source="##ALL##" ExternalSourceName="IDAndInvoice"
            Caption="$Unigrid.Order.Columns.OrderID$" Sort="OrderID" Wrap="false" />
        <ug:Column Name="Customer" Source="##ALL##" ExternalSourceName="Customer" Caption="$Unigrid.Order.Columns.OrderCustomerFullName$"
            Sort="CustomerLastName" Wrap="false">
            <Tooltip Encode="true" Source="CustomerEmail" />
        </ug:Column>
        <ug:Column Name="Date" Source="OrderDate" Caption="$Unigrid.Order.Columns.OrderDate$"
            Wrap="false" />
        <ug:Column Name="MainCurrencyPrice" Source="##ALL##" ExternalSourceName="TotalPriceInMainCurrency" Caption="$Unigrid.Order.Columns.OrderTotalPrice$"
            Sort="OrderTotalPriceInMainCurrency" Wrap="false" CssClass="TextRight" />
        <ug:Column Name="OrderPrice" Source="##ALL##" ExternalSourceName="TotalPriceInOrderPrice" Caption="$com.orderlist.ordercurrencycaption$"
            Sort="OrderTotalPrice" Wrap="false" CssClass="TextRight" />
        <ug:Column Name="OrderStatus" Source="StatusDisplayName" ExternalSourceName="StatusDisplayName"
            Caption="$Unigrid.Order.Columns.OrderStatusID$" Wrap="false" />
        <ug:Column Name="PaymentOption" Source="OrderPaymentOptionID" ExternalSourceName="OrderPaymentOptionID"
            Caption="$com.orderswidget.paymentmethod$" Wrap="false" AllowSorting="false" />
        <ug:Column Name="IsPaid" Source="OrderIsPaid" ExternalSourceName="#yesno" Caption="$Unigrid.Order.Columns.OrderIsPaid$"
            Wrap="false" />
        <ug:Column Name="ShippingOption" Source="OrderShippingOptionID" ExternalSourceName="OrderShippingOptionID"
            Caption="$com.orderswidget.shippingoption$" Wrap="false" AllowSorting="false" />
        <ug:Column Name="TrackingNumber" Source="OrderTrackingNumber" Caption="$com.orderswidget.trackingnumber$"
            Wrap="false" />
        <ug:Column Name="Note" Source="OrderNote" ExternalSourceName="Note" Caption="$com.orderlist.notecaption$"
            Wrap="false">
            <Tooltip Encode="true" Source="OrderNote" />
        </ug:Column>
        <ug:Column Width="100%" />
    </GridColumns>
</cms:UniGrid>
