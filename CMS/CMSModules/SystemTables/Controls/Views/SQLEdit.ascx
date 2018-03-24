<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_SystemTables_Controls_Views_SQLEdit"
    CodeFile="SQLEdit.ascx.cs" %>

<cms:MessagesPlaceHolder ID="plcMess" runat="server" />
<asp:PlaceHolder ID="plcGenerate" runat="server" Visible="false">
    <cms:LocalizedButton ID="btnGenerate" runat="server" ButtonStyle="Default" OnClick="btnGenerate_Click"
        ResourceString="sysdev.views.generateskeleton" EnableViewState="false" />
    <br />
    <br />
</asp:PlaceHolder>
<asp:Label ID="lblCreateLbl" runat="server" />
<cms:CMSTextBox ID="txtObjName" runat="server" MaxLength="128" />
<div class="control-group-inline">
    <cms:CMSCheckBox runat="server" ID="chkWithBinding" Visible="False" Text="WITH SCHEMABINDING" />
</div>
<br />
<asp:PlaceHolder ID="plcParams" runat="server">
    <cms:CMSTextArea ID="txtParams" runat="server" Rows="7" Width="100%" /><br />
</asp:PlaceHolder>
<asp:Literal ID="lblBegin" runat="server" />
<cms:ExtendedTextArea runat="server" ID="txtSQLText" EnableViewState="false" EditorMode="Advanced"
    Language="SQL" Rows="27" Width="100%" />
<asp:Label ID="lblEnd" runat="server" />
<br />
<br />
<br />
<cms:FormSubmitButton runat="server" ID="btnOk" EnableViewState="false" OnClick="btnOK_Click" />