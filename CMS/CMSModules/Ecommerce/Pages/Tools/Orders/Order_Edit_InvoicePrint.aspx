<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_Ecommerce_Pages_Tools_Orders_Order_Edit_InvoicePrint" Theme="Default" CodeFile="Order_Edit_InvoicePrint.aspx.cs" %>

<!DOCTYPE html>
<html>
<head runat="server" enableviewstate="false">
    <title></title>
    <base target="_self" />
    <style type="text/css">
        body
        {
            margin: 0;
            padding: 0;
            height: 100%;
        }
    </style>
</head>
<body onload="window.print();" class="<%=mBodyClass%>">
    <form id="form1" runat="server">
    <div class="BodyScrollArea">
        <div style="text-align: center; margin: 0px; padding: 0px;">
            <asp:Label ID="lblInvoice" runat="server" EnableViewState="false" />
        </div>
    </div>
    </form>
</body>
</html>
