<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_Ecommerce_Pages_Tools_Orders_Order_Edit_AdrPrint" Theme="Default" CodeFile="Order_Edit_AdrPrint.aspx.cs" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server" enableviewstate="false">
    <title></title>
    <base target="_self" />
    <style type="text/css">
        body
        {
            margin: 0px;
            padding: 0px;
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
