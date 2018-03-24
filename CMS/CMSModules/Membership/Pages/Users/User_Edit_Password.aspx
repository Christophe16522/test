<%@ Page Language="C#" AutoEventWireup="true"
    Inherits="CMSModules_Membership_Pages_Users_User_Edit_Password" Theme="Default"
    MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Title="User Edit - Password" CodeFile="User_Edit_Password.aspx.cs" %>

<%@ Register Src="~/CMSModules/Membership/FormControls/Passwords/PasswordStrength.ascx" TagName="PasswordStrength"
    TagPrefix="cms" %>

<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <asp:PlaceHolder ID="plcTable" runat="server">
        <div class="form-horizontal">
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <asp:Label ID="LabelPassword" runat="server" Text="Label" CssClass="control-label" AssociatedControlID="passStrength" />
                </div>
                <div class="editing-form-value-cell">
                    <cms:PasswordStrength runat="server" ID="passStrength" AllowEmpty="true" />
                </div>
            </div>
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <asp:Label ID="LabelConfirmPassword" runat="server" Text="Label" CssClass="control-label" AssociatedControlID="TextBoxConfirmPassword" />
                </div>
                <div class="editing-form-value-cell">
                    <cms:CMSTextBox ID="TextBoxConfirmPassword" runat="server" TextMode="Password" 
                        MaxLength="100" />
                </div>
            </div>
            <div class="form-group">
                <div class="editing-form-value-cell-offset">
                    <cms:CMSCheckBox ID="chkSendEmail" runat="server" Visible="true" CssClass="CheckBoxMovedLeft"
                        Checked="true" />
                </div>
            </div>
            <div class="form-group">
                <div class="editing-form-value-cell-offset editing-form-value-cell">
                    <cms:CMSButton ID="ButtonSetPassword" runat="server" Text="" OnClick="ButtonSetPassword_Click"
                        ButtonStyle="Primary" EnableViewState="false" />
               </div>
            </div>
        </div>
    </asp:PlaceHolder>
</asp:Content>