<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_Membership_Pages_Users_User_New"
    Theme="Default" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Title="Users - New User" CodeFile="User_New.aspx.cs" %>

<%@ Register Src="~/CMSModules/Membership/FormControls/Users/UserName.ascx" TagName="UserName" TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/Membership/FormControls/Passwords/PasswordStrength.ascx" TagName="PasswordStrength"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSFormControls/System/EnumSelector.ascx" TagName="PrivilegeLevel"
    TagPrefix="cms" %>

<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <div class="form-horizontal">
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel AssociatedControlID="ucUserName" CssClass="control-label" ID="lblUserName" runat="server" EnableViewState="false" ResourceString="general.username"
                    DisplayColon="true" ShowRequiredMark="true" />
            </div>
            <div class="editing-form-value-cell">
                <cms:UserName ID="ucUserName" runat="server" />
            </div>
        </div>
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel AssociatedControlID="TextBoxFullName" CssClass="control-label" ID="LabelFullName" runat="server" EnableViewState="false" ResourceString="Administration-User_New.FullName" DisplayColon="true" ShowRequiredMark="true" />
            </div>
            <div class="editing-form-value-cell">
                <cms:CMSTextBox ID="TextBoxFullName" runat="server" MaxLength="200" />
                <cms:CMSRequiredFieldValidator ID="RequiredFieldValidatorFullName" runat="server" EnableViewState="false"
                    ControlToValidate="TextBoxFullName" Display="dynamic" />
            </div>
        </div>
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel AssociatedControlID="TextBoxEmail" CssClass="control-label" ID="LabelEmail" runat="server" EnableViewState="false" ResourceString="general.email"
                    DisplayColon="true" />
            </div>
            <div class="editing-form-value-cell">
                <cms:CMSTextBox ID="TextBoxEmail" runat="server" MaxLength="100" />
            </div>
        </div>
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel AssociatedControlID="CheckBoxEnabled" CssClass="control-label" ID="lblEnabled" runat="server" EnableViewState="false" ResourceString="general.enabled"
                    DisplayColon="true" />
            </div>
            <div class="editing-form-value-cell">
                <cms:CMSCheckBox ID="CheckBoxEnabled" runat="server" />
            </div>
        </div>
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" ID="lblPrivilegeLevel" ResourceString="user.privilegelevel" runat="server" EnableViewState="false" DisplayColon="true" />
            </div>
            <div class="editing-form-value-cell">
                <cms:PrivilegeLevel runat="server" ID="drpPrivilegeLevel" AssemblyName="CMS.Membership" TypeName="CMS.Membership.UserPrivilegeLevelEnum" />
            </div>
        </div>
        <div class="form-group">
            <div class="editing-form-label-cell">
                <asp:Label AssociatedControlID="passStrength" CssClass="control-label" ID="LabelPassword" runat="server" EnableViewState="false" Text="aa" />
            </div>
            <div class="editing-form-value-cell">
                <cms:PasswordStrength runat="server" ID="passStrength" AllowEmpty="true" />
            </div>
        </div>
        <div class="form-group">
            <div class="editing-form-label-cell">
                <asp:Label AssociatedControlID="TextBoxConfirmPassword" CssClass="control-label" ID="LabelConfirmPassword" runat="server" EnableViewState="false" />
            </div>
            <div class="editing-form-value-cell">
                <cms:CMSTextBox ID="TextBoxConfirmPassword" runat="server" TextMode="password"
                    MaxLength="100" />
            </div>
        </div>
        <div class="form-group">
            <div class="editing-form-value-cell editing-form-value-cell-offset">
                <cms:FormSubmitButton ID="ButtonOK" runat="server" OnClick="ButtonOK_Click"
                    EnableViewState="false" ResourceString="general.ok" />
            </div>
        </div>
    </div>
</asp:Content>
