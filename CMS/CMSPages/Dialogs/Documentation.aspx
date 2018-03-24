<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSPages_Dialogs_Documentation"
    CodeFile="Documentation.aspx.cs" %>

<!DOCTYPE html>
<html>
<head runat="server" enableviewstate="false">
    <title>WebParts documentation</title>
    <style media="print" type="text/css">
        .noprint
        {
            display: none;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <a name="top"></a>
    <table style="width: 100%; background-color: #EDF4F9;">
        <tr>
            <td>
                <h1>
                    <%=documentationTitle%></h1>
            </td>
        </tr>
    </table>
    <asp:Panel ID="pnlContent" runat="server" >
        <asp:Literal runat="server" ID="ltlContent" />
    </asp:Panel>
    <asp:Panel ID="pnlInfo" runat="server" Visible="false" >
        <br />
        <b>
            <cms:LocalizedLabel runat="server" ID="lblInfoTitle" ResourceString="Webpartdocumentation.titleinfo" />
        </b>
        <br />
        <br />
        <cms:LocalizedLabel runat="server" ID="lblWebparts" ResourceString="Webpartdocumentation.webparts"
            DisplayColon="true" />
        <a href="documentation.aspx?allwebparts=true">Web parts</a>
        &nbsp;
        <a href="documentation.aspx?allwireframes=true">Wireframes</a>
        <br />
        <cms:LocalizedLabel runat="server" ID="lblWidgets" ResourceString="Webpartdocumentation.widgets"
            DisplayColon="true" />
        <a href="documentation.aspx?allwidgets=true">Widgets</a>
    </asp:Panel>
    </form>
</body>
</html>
