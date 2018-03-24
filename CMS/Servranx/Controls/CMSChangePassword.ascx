<%@ Control Language="C#" AutoEventWireup="true" CodeFile="CMSChangePassword.ascx.cs" Inherits="CMSChangePassword" %>

<%@ Register Src="~/CMSModules/Membership/FormControls/Passwords/PasswordStrength.ascx"  TagName="PasswordStrength" TagPrefix="cms" %>

<cms:MessagesPlaceholder ID="plcMess" runat="server" IsLiveSite="false" />
<asp:Panel runat="server" ID="pnlReset">
	<div class="cont_client">
		<cms:PasswordStrength runat="server" ID="passStrength" ValidationGroup="PasswordReset" TextBoxClass="champtexte left" />
		<div class="clr"> </div>
		<cms:CMSTextBox ID="txtConfirmPassword" runat="server" TextMode="Password" MaxLength="200" CssClass="champtexte left clickClear"></cms:CMSTextBox>
        <cms:CMSRequiredFieldValidator ID="rfvConfirmPassword" runat="server" ControlToValidate="txtConfirmPassword" ValidationGroup="PasswordReset" Display="Dynamic"></cms:CMSRequiredFieldValidator>
	</div>

    <div class="cont_border left btn_gris formular center">
        <cms:CMSButton runat="server" ID="btnReset" EnableViewState="false" CssClass="btn_envoyer" ValidationGroup="PasswordReset" />
    </div>
</asp:Panel>
