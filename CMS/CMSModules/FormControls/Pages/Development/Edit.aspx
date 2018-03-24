<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_FormControls_Pages_Development_Edit"
    Theme="Default" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Title="Form User Control Edit"
    CodeFile="Edit.aspx.cs" %>

<%@ Register Src="~/CMSFormControls/System/LocalizableTextBox.ascx" TagName="LocalizableTextBox"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSFormControls/Dialogs/FileSystemSelector.ascx" TagPrefix="cms"
    TagName="FileSystemSelector" %>
<%@ Register Src="~/CMSFormControls/SelectModule.ascx" TagPrefix="cms" TagName="SelectModule" %>
<%@ Register Src="~/CMSFormControls/System/UserControlTypeSelector.ascx" TagPrefix="cms"
    TagName="TypeSelector" %>
<%@ Register Src="~/CMSModules/AdminControls/Controls/MetaFiles/File.ascx" TagName="File"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSFormControls/System/CodeName.ascx" TagName="CodeName" TagPrefix="cms" %>

<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <div class="ui-form label-width-200">
        <cms:LocalizedHeading runat="server" ID="headGeneral" Level="4" EnableViewState="false" ResourceString="general.general" />
        <div class="form-horizontal">
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <cms:LocalizedLabel CssClass="control-label" ID="lblDisplayName" runat="server" EnableViewState="false"
                        ResourceString="general.displayname" DisplayColon="true" AssociatedControlID="tbDisplayName" />
                </div>
                <div class="editing-form-value-cell">
                    <cms:LocalizableTextBox ID="tbDisplayName" runat="server"
                        MaxLength="100" />
                    <cms:CMSRequiredFieldValidator ID="rfvDisplayName" runat="server" ControlToValidate="tbDisplayName:cntrlContainer:textbox"
                        Display="Dynamic" EnableViewState="false"></cms:CMSRequiredFieldValidator>
                </div>
            </div>
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <cms:LocalizedLabel CssClass="control-label" ID="lblCodeName" runat="server" EnableViewState="false"
                        ResourceString="general.codename" DisplayColon="true" AssociatedControlID="tbCodeName" />
                </div>
                <div class="editing-form-value-cell">
                    <cms:CodeName ID="tbCodeName" runat="server" MaxLength="100" />
                    <cms:CMSRequiredFieldValidator ID="rfvCodeName" runat="server" EnableViewState="false"
                        ControlToValidate="tbCodeName" Display="dynamic"></cms:CMSRequiredFieldValidator>
                </div>
            </div>
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <cms:LocalizedLabel CssClass="control-label" ID="lblType" runat="server" EnableViewState="false"
                        ResourceString="formcontrols.type" DisplayColon="true" AssociatedControlID="drpTypeSelector" />
                </div>
                <div class="editing-form-value-cell">
                    <cms:TypeSelector ID="drpTypeSelector" runat="server" CssClass="DropDownField" IncludeAllItem="false" />
                </div>
            </div>
            <asp:PlaceHolder ID="plcFileName" runat="server">
                <div class="form-group">
                    <div class="editing-form-label-cell">
                        <cms:LocalizedLabel CssClass="control-label" ID="lblFileName" runat="server" EnableViewState="false"
                            ResourceString="general.filename" DisplayColon="true" AssociatedControlID="tbFileName" />
                    </div>
                    <div class="editing-form-value-cell">
                        <cms:FileSystemSelector ID="tbFileName" runat="server" />
                    </div>
                </div>
            </asp:PlaceHolder>
            <asp:PlaceHolder ID="plcFormControls" runat="server" Visible="false">
                <div class="form-group">
                    <div class="editing-form-label-cell">
                        <cms:LocalizedLabel CssClass="control-label" ID="lblInheritedWebPart" runat="server"
                            EnableViewState="false" ResourceString="DevelopmentWebPartGeneral.InheritedWebPart"
                            DisplayColon="true" AssociatedControlID="txtParent" />
                    </div>
                    <div class="editing-form-value-cell">
                        <cms:CMSTextBox ID="txtParent" runat="server" Enabled="false" />
                    </div>
                </div>
            </asp:PlaceHolder>
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblDescription" EnableViewState="false" DisplayColon="true"
                        ResourceString="General.Description" AssociatedControlID="txtDescription" />
                </div>
                <div class="editing-form-value-cell">
                    <cms:LocalizableTextBox TextMode="MultiLine" runat="server" ID="txtDescription" Rows="8" />
                </div>
            </div>
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblIcon" EnableViewState="false" DisplayColon="true"
                        ResourceString="General.Thumbnail" AssociatedControlID="UploadFile" />
                </div>
                <div class="editing-form-value-cell">
                    <cms:File ID="UploadFile" runat="server" />
                </div>
            </div>
            <asp:PlaceHolder runat="server" ID="plcDevelopment">
                <div class="form-group">
                    <div class="editing-form-label-cell">
                        <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblModule" EnableViewState="false" ResourceString="General.Module" AssociatedControlID="drpModule" DisplayColon="true" />
                    </div>
                    <div class="editing-form-value-cell">
                        <cms:SelectModule ID="drpModule" runat="server" DisplayNone="true" DisplayAllModules="true"
                            IsLiveSite="false" />
                    </div>
                </div>
            </asp:PlaceHolder>
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblHighPriority" EnableViewState="false" DisplayColon="true"
                        ResourceString="formcontrols.highpriority" AssociatedControlID="chkHighPriority" />
                </div>
                <div class="editing-form-value-cell">
                    <cms:CMSCheckBox ID="chkHighPriority" runat="server" />
                </div>
            </div>
            <cms:LocalizedHeading runat="server" ID="headControlScope" Level="4" EnableViewState="false" ResourceString="development.formcontrols.controlscope" />
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblUseControlFor" EnableViewState="false"
                        DisplayColon="true" ResourceString="development.formcontrols.usefor" />
                </div>
                <div class="editing-form-value-cell">
                    <div class="checkbox-list-vertical">
                        <cms:CMSCheckBox ID="chkForText" runat="server" ResourceString="Development_FormUserControl_Edit.lblForText" />
                        <cms:CMSCheckBox ID="chkForDateTime" runat="server" ResourceString="Development_FormUserControl_Edit.lblForDateTime" />
                        <cms:CMSCheckBox ID="chkForLongText" runat="server" ResourceString="Development_FormUserControl_Edit.lblForLongText" />
                        <cms:CMSCheckBox ID="chkForBoolean" runat="server" ResourceString="Development_FormUserControl_Edit.lblForBoolean" />
                        <cms:CMSCheckBox ID="chkForDecimal" runat="server" ResourceString="Development_FormUserControl_Edit.lblForDecimal" />
                        <cms:CMSCheckBox ID="chkForGuid" runat="server" ResourceString="templatedesigner.fieldtypes.guid" />
                        <cms:CMSCheckBox ID="chkForInteger" runat="server" ResourceString="Development_FormUserControl_Edit.lblForInteger" />
                        <cms:CMSCheckBox ID="chkForFile" runat="server" ResourceString="Development_FormUserControl_Edit.lblForFile" />
                        <cms:CMSCheckBox ID="chkForLongInteger" runat="server" ResourceString="Development_FormUserControl_Edit.lblForLongInteger" />
                        <cms:CMSCheckBox ID="chkForDocAtt" runat="server" ResourceString="Development_FormUserControl_Edit.lblForDocAtt" />
                        <cms:CMSCheckBox ID="chkForVisibility" runat="server" ResourceString="Development_FormUserControl_Edit.lblForVisibility" />
                    </div>
                </div>
            </div>
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <cms:LocalizedLabel runat="server" CssClass="control-label" ID="lblShowControlIn" EnableViewState="false"
                        DisplayColon="true" ResourceString="development.formcontrols.showin" />
                </div>
                <div class="editing-form-value-cell">
                    <cms:CMSUpdatePanel runat="server">
                        <ContentTemplate>
                            <div class="checkbox-list-vertical">
                                <cms:CMSCheckBox ID="chkShowInDocumentTypes" runat="server" ResourceString="Development_FormUserControl_Edit.lblShowInDocumentTypes" />
                                <cms:CMSCheckBox ID="chkForBizForms" runat="server" AutoPostBack="True" OnCheckedChanged="chkForBizForms_CheckedChanged"
                                    ResourceString="Development_FormUserControl_Edit.lblForBizForms" />
                                <div class="selector-subitem">
                                    <div class="form-group">
                                        <div class="editing-form-label-cell">
                                            <cms:LocalizedLabel CssClass="control-label" ID="lblDataType" runat="server" EnableViewState="false" ResourceString="Development_FormUserControl_Edit.lblDataType"
                                                DisplayColon="true" />
                                        </div>
                                        <div class="editing-form-value-cell">
                                            <cms:CMSDropDownList ID="drpDataType" runat="server" CssClass="SmallDropDown" AutoPostBack="True"
                                                OnSelectedIndexChanged="drpDataType_SelectedIndexChanged" />
                                        </div>
                                    </div>
                                    <div class="form-group">
                                        <div class="editing-form-label-cell">
                                            <cms:LocalizedLabel CssClass="control-label" ID="lblColumnSize" runat="server" EnableViewState="false" ResourceString="Development_FormUserControl_Edit.lblColumnSize"
                                                DisplayColon="true" />
                                        </div>
                                        <div class="editing-form-value-cell">
                                            <cms:CMSTextBox ID="tbColumnSize" runat="server">0</cms:CMSTextBox>
                                            <cms:LocalizedLabel ID="lblErrorSize" runat="server" CssClass="ErrorLabel" Visible="False"
                                                EnableViewState="false" ResourceString="Development_FormUserControl_Edit.lblErrorSize" />
                                        </div>
                                    </div>
                                </div>
                                <cms:CMSCheckBox ID="chkShowInCustomTables" runat="server" ResourceString="Development_FormUserControl_Edit.lblShowInCustomTables" />
                                <cms:CMSCheckBox ID="chkShowInSystemTables" runat="server" ResourceString="Development_FormUserControl_Edit.lblShowInSystemTables" />
                                <cms:CMSCheckBox ID="chkShowInReports" runat="server" ResourceString="Development_FormUserControl_Edit.lblShowInReports" />
                                <cms:CMSCheckBox ID="chkShowInControls" runat="server" ResourceString="Development_FormUserControl_Edit.lblShowInControls" />
                            </div>
                        </ContentTemplate>
                    </cms:CMSUpdatePanel>
                </div>
            </div>

        </div>
    </div>
    <cms:FormSubmitButton ID="FormSubmitButton1" runat="server" OnClick="btnOK_Click"
        EnableViewState="false" ResourceString="general.ok" />
</asp:Content>
