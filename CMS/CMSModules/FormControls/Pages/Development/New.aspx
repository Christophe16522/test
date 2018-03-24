<%@ Page Language="C#" AutoEventWireup="true" CodeFile="New.aspx.cs" Inherits="CMSModules_FormControls_Pages_Development_New"
    MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Title="Form Controls - New form control"
    Theme="Default" %>

<%@ Register Src="~/CMSFormControls/System/LocalizableTextBox.ascx" TagName="LocalizableTextBox"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSFormControls/Dialogs/FileSystemSelector.ascx" TagPrefix="cms"
    TagName="FileSystemSelector" %>
<%@ Register Src="~/CMSFormControls/System/UserControlTypeSelector.ascx" TagPrefix="cms"
    TagName="TypeSelector" %>
<%@ Register Src="~/CMSModules/FormControls/FormControls/FormControlSelector.ascx" TagName="FormControlSelector"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSFormControls/System/CodeName.ascx" TagName="CodeName" TagPrefix="cms" %>

<asp:Content ContentPlaceHolderID="plcSiteSelector" runat="server"
    Visible="true">
    <div class="radio-list-vertical">
        <cms:CMSRadioButton runat="server" ID="radNewControl" ResourceString="developmentformcontroleditnew"
            GroupName="fcSelect" Checked="true" OnCheckedChanged="radNewFormControl_CheckedChanged"
            AutoPostBack="true" />
        <cms:CMSRadioButton runat="server" ID="radInheritedControl" GroupName="fcSelect" ResourceString="developmentformcontrolinherited"
            OnCheckedChanged="radNewFormControl_CheckedChanged" AutoPostBack="true" />
    </div>
</asp:Content>
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <div class="form-horizontal">
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" ID="lblDisplayName" runat="server" EnableViewState="false"
                    ResourceString="general.displayname" ShowRequiredMark="true" DisplayColon="true" AssociatedControlID="txtControlDisplayName" />
            </div>
            <div class="editing-form-value-cell">
                <cms:LocalizableTextBox ID="txtControlDisplayName" runat="server"
                    MaxLength="100" />
                <cms:CMSRequiredFieldValidator ID="rfvControlDisplayName" runat="server" ControlToValidate="txtControlDisplayName:cntrlContainer:textbox"
                    EnableViewState="false" />
            </div>
        </div>
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" ID="lblCodeName" runat="server" EnableViewState="false"
                    ResourceString="general.codename" ShowRequiredMark="true" DisplayColon="true" AssociatedControlID="txtControlName" />
            </div>
            <div class="editing-form-value-cell">
                <cms:CodeName ID="txtControlName" runat="server" MaxLength="100" />
                <cms:CMSRequiredFieldValidator ID="rfvControlName" runat="server" ControlToValidate="txtControlName"
                    EnableViewState="false" />
            </div>
        </div>
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" ID="lblType" runat="server" EnableViewState="false"
                    ResourceString="formcontrols.type" ShowRequiredMark="true" DisplayColon="true" AssociatedControlID="drpTypeSelector" />
            </div>
            <div class="editing-form-value-cell">
                <cms:TypeSelector ID="drpTypeSelector" IncludeAllItem="false" runat="server" CssClass="DropDownField" />
            </div>
        </div>
        <asp:PlaceHolder ID="plcFileName" runat="server">
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <cms:LocalizedLabel CssClass="control-label" ID="lblFileName" runat="server" EnableViewState="false"
                        ResourceString="general.filename" DisplayColon="true" ShowRequiredMark="true" AssociatedControlID="tbFileName" />
                </div>
                <div class="editing-form-value-cell">
                    <cms:FileSystemSelector ID="tbFileName" runat="server" />
                </div>
            </div>
        </asp:PlaceHolder>
        <asp:PlaceHolder ID="plcFormControls" runat="server" Visible="false">
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <cms:LocalizedLabel CssClass="control-label" ID="lblInheritControl" runat="server" EnableViewState="false"
                        ResourceString="developmentwebpartedit.inheritedwebpart" ShowRequiredMark="true" DisplayColon="true" AssociatedControlID="drpFormControls" />
                </div>
                <div class="editing-form-value-cell">
                    <cms:FormControlSelector ID="drpFormControls" runat="server" ShowInheritedControls="false" />
                </div>
            </div>
        </asp:PlaceHolder>
        <div class="form-group">
            <div class="editing-form-value-cell editing-form-value-cell-offset">
                <cms:FormSubmitButton runat="server" ID="btnOk" OnClick="btnOK_Click" EnableViewState="false"
                    ResourceString="general.ok" />
            </div>
        </div>
    </div>
</asp:Content>
