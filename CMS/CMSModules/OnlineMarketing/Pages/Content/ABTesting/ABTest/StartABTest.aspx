<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_OnlineMarketing_Pages_Content_ABTesting_ABTest_StartABTest"
    Theme="default" MasterPageFile="~/CMSMasterPages/UI/Dialogs/ModalDialogPage.master"
    CodeFile="StartABTest.aspx.cs" %>

<asp:Content ContentPlaceHolderID="plcContent" runat="server">
    <div class="form-horizontal">
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel runat="server" CssClass="control-label" ResourceString="abtesting.starttest" DisplayColon="True" />
            </div>
            <div class="editing-form-value-cell">
                <div class="control-group-inline">
                    <cms:CMSRadioButton ID="radNow" runat="server" GroupName="Start" AutoPostBack="true"
                        OnCheckedChanged="radGroupStart_CheckedChanged" ResourceString="calendar.now"
                        Checked="true" />
                </div>
                <div class="control-group-inline">
                    <cms:CMSRadioButton ID="radLater" runat="server" GroupName="Start" AutoPostBack="true"
                        OnCheckedChanged="radGroupStart_CheckedChanged" ResourceString="calendar.later" />
                    <cms:DateTimePicker ID="calendarControl" runat="server" Enabled="false" SupportFolder="~/CMSAdminControls/Calendar" />
                </div>
            </div>
        </div>
    </div>
</asp:Content>