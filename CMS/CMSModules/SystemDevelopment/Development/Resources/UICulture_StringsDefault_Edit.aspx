<%@ Page Language="C#" AutoEventWireup="true"
    MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Inherits="CMSModules_SystemDevelopment_Development_Resources_UICulture_StringsDefault_Edit"
    Theme="Default" Title="Default strings - Edit" CodeFile="UICulture_StringsDefault_Edit.aspx.cs" %>

<asp:Content ContentPlaceHolderID="plcContent" ID="content" runat="server">
    <div class="form-horizontal">
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" ID="lblKey" runat="server" ResourceString="culture.key" DisplayColon="true" EnableViewState="false" AssociatedControlID="txtKey" />
            </div>
            <div class="editing-form-value-cell">
                <cms:CMSTextBox ID="txtKey" runat="server" />
                <cms:CMSRequiredFieldValidator ID="rfvKey" runat="server" ControlToValidate="txtKey"
                    Display="Dynamic">
                </cms:CMSRequiredFieldValidator>
            </div>
        </div>
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" ID="lblText" runat="server" ResourceString="culture.text"
                    DisplayColon="true" EnableViewState="false" AssociatedControlID="txtText" />
            </div>
            <div class="editing-form-value-cell">
                <cms:CMSTextBox ID="txtText" runat="server" TextMode="multiline" />
            </div>
        </div>
        <div class="form-group">
            <div class="editing-form-value-cell editing-form-value-cell-offset">
                <cms:FormSubmitButton ID="btnOk" runat="server" OnClick="btnOK_Click" ResourceString="general.ok" EnableViewState="false" />
            </div>
        </div>
    </div>
</asp:Content>