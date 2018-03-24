<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_Ecommerce_CMSPages_GetInvoice" Theme="Default" CodeFile="GetInvoice.aspx.cs" %>

<!DOCTYPE html>
<html>
<head runat="server" enableviewstate="false">
    <title>Invoice</title>
	<meta http-equiv="content-type" content="text/html; charset=UTF-8" />
    <style type="text/css">
        body
        {
            margin: 0;
            padding: 0;
            height: 100%;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <asp:Literal runat="server" ID="ltlInvoice" EnableViewState="false" />
    </div>
    </form>
</body>
</html>
