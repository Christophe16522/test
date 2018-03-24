﻿<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Password.ascx.cs" Inherits="Servranx_Controls_Password" %>
<cms:CMSTextBox runat="server" ID="txtPassword" TextMode="Password" CssClass="clickClear champtexte left" ToolTip="Mot de passe" /><asp:Label
    ID="lblRequiredFieldMark" runat="server" Text="" Visible="false" />
<div class="PasswordStrengthText" style="display:none">
    <cms:LocalizedLabel runat="server" ID="lblPasswStregth" CssClass="PasswordStrengthHint"
        ResourceString="Membership.PasswordStrength" />
    <asp:Label runat="server" ID="lblEvaluation" EnableViewState="false" />
</div>
<asp:Panel runat="server" ID="pnlPasswStrengthIndicator" CssClass="PasswStrenghtIndicator" style="display:none">
    <asp:Panel runat="server" ID="pnlPasswIndicator">
        &nbsp;
    </asp:Panel>
</asp:Panel>
<cms:CMSRequiredFieldValidator ID="rfvPassword" runat="server" ControlToValidate="txtPassword"
    Display="Dynamic" EnableViewState="false" CssClass="left" Visible ="false"/>


