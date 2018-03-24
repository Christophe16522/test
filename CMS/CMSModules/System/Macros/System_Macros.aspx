<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_System_Macros_System_Macros" Theme="Default" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Title="System - Macros" CodeFile="System_Macros.aspx.cs" %>

<%@ Register TagPrefix="cms" TagName="AsyncBackground" Src="~/CMSAdminControls/AsyncBackground.ascx" %>
<%@ Register TagPrefix="cms" TagName="AsyncControl" Src="~/CMSAdminControls/AsyncControl.ascx" %>
<%@ Register TagPrefix="cms" TagName="PageTitle" Src="~/CMSAdminControls/UI/PageElements/PageTitle.ascx" %>

<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:LocalizedHeading runat="server" ID="headSignatures" Level="4" ResourceString="macros.refreshsecurityparams" EnableViewState="false" />
    <cms:LocalizedLabel runat="server" ResourceString="macros.refreshsecurityparams.description" />
    <%-- Form --%>
    <div class="form-horizontal">
        <%-- Old salt --%>
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" runat="server" ResourceString="macros.refreshsecurityparams.oldsalt" DisplayColon="true" AssociatedControlID="txtOldSalt" />
            </div>
            <div class="editing-form-value-cell">
                <div class="control-group-inline">
                    <cms:CMSCheckBox ID="chkRefreshAll" runat="server" ResourceString="macros.refreshsecurityparams.refreshall"
                        ToolTipResourceString="macros.refreshsecurityparams.refreshalltooltip" Checked="false" AutoPostBack="true" />
                </div>
                <div class="control-group-inline">
                    <cms:CMSTextBox ID="txtOldSalt" runat="server" />
                </div>
            </div>
        </div>
        <%-- New salt --%>
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" runat="server" ResourceString="macros.refreshsecurityparams.newsalt" DisplayColon="true" AssociatedControlID="txtNewSalt" />
            </div>
            <div class="editing-form-value-cell">
                <div class="control-group-inline">
                    <cms:CMSCheckBox ID="chkUseCurrentSalt" runat="server" ResourceString="macros.refreshsecurityparams.usecurrentsalt" Checked="true" AutoPostBack="true" />
                </div>
                <div class="control-group-inline">
                    <cms:CMSTextBox ID="txtNewSalt" runat="server" />
                </div>
            </div>
        </div>
        <%-- Submit --%>
        <div class="form-group">
            <div class="editing-form-value-cell editing-form-value-cell-offset">
                <cms:CMSButton ID="btnRefreshSecurityParams" runat="server" ButtonStyle="Primary" />
            </div>
        </div>
    </div>
    <%-- Async log --%>
    <asp:PlaceHolder ID="plcAsyncLog" runat="server" Visible="false">
        <cms:AsyncBackground runat="server" />
        <div class="AsyncLogArea">
            <div class="PageBody">
                <div class="PageHeader">
                    <cms:PageTitle ID="ucTitle" runat="server" HideTitle="true" />
                </div>
                <div class="header-panel">
                    <cms:CMSButton ID="btnCancel" runat="server" ButtonStyle="Primary" />
                </div>
                <div class="PageContent">
                    <cms:AsyncControl ID="ucAsync" runat="server" />
                </div>
            </div>
        </div>
    </asp:PlaceHolder>
</asp:Content>
