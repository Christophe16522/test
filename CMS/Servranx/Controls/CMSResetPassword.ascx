<%@ Control Language="C#" AutoEventWireup="true" CodeFile="CMSResetPassword.ascx.cs" Inherits="CMSResetPassword" %>

<%@ Register Src="~/CMSModules/Membership/FormControls/Passwords/PasswordStrength.ascx"  TagName="PasswordStrength" TagPrefix="cms" %>

<cms:MessagesPlaceholder ID="plcMess" runat="server" IsLiveSite="false" />
<asp:Panel runat="server" ID="pnlReset">
    <div class="cont_champ1 center">
        <cms:LocalizedLabel runat="server" ID="lblPwd" EnableViewState="False" CssClass="lbltxt" Text="Mot de passe"></cms:LocalizedLabel>
        <cms:PasswordStrength runat="server" ID="passStrength" ValidationGroup="PasswordReset" TextBoxClass="champ_txt1"/>
	</div>
    
    <div class="cont_champ1 center">
        <cms:LocalizedLabel runat="server" ID="lblConfirmPwd" EnableViewState="False" CssClass="lbltxt" Text="Confirmer le mot de passe"></cms:LocalizedLabel>
        <cms:CMSTextBox ID="txtConfirmPassword" runat="server" TextMode="Password" MaxLength="200" CssClass="champ_txt1"></cms:CMSTextBox>
        <cms:CMSRequiredFieldValidator ID="rfvConfirmPassword" runat="server" ControlToValidate="txtConfirmPassword" ValidationGroup="PasswordReset" Display="Dynamic"></cms:CMSRequiredFieldValidator>
    </div>
    
    <div class="cont_border left btn_gris formular center">
        <cms:CMSButton runat="server" ID="btnReset" EnableViewState="false" CssClass="FormButton btn_stand" ValidationGroup="PasswordReset" Text="Envoyer"/>
    </div>
</asp:Panel>
