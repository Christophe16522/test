<%@ Control Language="C#" AutoEventWireup="true" CodeFile="SimpleCaptcha.ascx.cs"
    Inherits="Servranx_Controls_SimpleCaptcha" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="ajax" %>
<%--<div>
    <cms:LocalizedLabel ID="lblSecurityCode" runat="server" EnableViewState="false" Visible="false" />
</div>--%>
<cms:LocalizedLabel ID="lblSecurityCode" runat="server" EnableViewState="false" Visible="false" />
<fieldset class="left FieldWidth">
	<div class="BgCodeCtct">
		<asp:Image ID="imgSecurityCode" AlternateText="Security code" runat="server"
                EnableViewState="false" ImageUrl="~/App_Themes/Servranx/images/ImgCode.gif" CssClass="BoxTextImg"/></div>
</fieldset>
<fieldset class="right FieldWidth">
	<div class="BgBoxTxtLvl1">
		<cms:CMSTextBox ID="txtSecurityCode" runat="server" CssClass="BoxTextLvl1" />
        <ajax:TextBoxWatermarkExtender ID="txtSecurityCode_TextBoxWatermarkExtender" 
                        runat="server" Enabled="True" TargetControlID="txtSecurityCode"
                        WatermarkCssClass="BoxTextLvl1" WatermarkText="COPIER LE CODE">
        </ajax:TextBoxWatermarkExtender>
    </div>
</fieldset>
<div class="clr">
	&nbsp;</div>

<asp:PlaceHolder runat="server" ID="plcAfterText" Visible="false">
    <asp:Label ID="lblAfterText" runat="server" EnableViewState="false" />
</asp:PlaceHolder>

