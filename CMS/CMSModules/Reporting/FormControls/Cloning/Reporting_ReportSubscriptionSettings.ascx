<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Reporting_ReportSubscriptionSettings.ascx.cs"
    Inherits="CMSModules_Reporting_FormControls_Cloning_Reporting_ReportSubscriptionSettings" %>

<div class="form-horizontal">
    <div class="form-group">
        <div class="editing-form-label-cell">
            <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblEmail" ResourceString="general.email" EnableViewState="false"
                DisplayColon="true" />
        </div>
        <div class="editing-form-value-cell">
            <cms:CMSTextBox runat="server" ID="txtEmail" />
        </div>
    </div>
</div>