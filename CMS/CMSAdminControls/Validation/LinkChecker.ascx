<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSAdminControls_Validation_LinkChecker"
    CodeFile="LinkChecker.ascx.cs" %>
<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" TagPrefix="cms" TagName="UniGrid" %>
<%@ Register Src="~/CMSAdminControls/AsyncControl.ascx" TagName="AsyncControl" TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/AsyncBackground.ascx" TagName="AsyncBackground"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/UI/PageElements/PageTitle.ascx" TagName="PageTitle"
    TagPrefix="cms" %>
<%@ Register Namespace="CMS.UIControls.UniGridConfig" TagPrefix="ug" Assembly="CMS.UIControls" %>
<asp:Panel runat="server" ID="pnlLog" Visible="false">
    <cms:AsyncBackground ID="backgroundElem" runat="server" />
    <div class="AsyncLogArea">
        <div>
            <asp:Panel ID="pnlAsyncBody" runat="server" CssClass="PageBody">
                <asp:Panel ID="pnlTitleAsync" runat="server" CssClass="PageHeader">
                    <cms:PageTitle ID="titleElemAsync" runat="server" SetWindowTitle="false" />
                </asp:Panel>
                <asp:Panel ID="pnlCancel" runat="server" CssClass="header-panel">
                    <cms:CMSButton runat="server" ID="btnCancel" ButtonStyle="Primary" />
                </asp:Panel>
                <asp:Panel ID="pnlAsyncContent" runat="server" CssClass="PageContent">
                    <cms:AsyncControl ID="ctlAsync" runat="server" />
                </asp:Panel>
            </asp:Panel>
        </div>
    </div>
</asp:Panel>
<asp:Literal ID="ltlScript" runat="server" EnableViewState="false" />
<asp:Panel ID="pnlGrid" runat="server" CssClass="Validation">
    <cms:MessagesPlaceholder ID="plcMess" runat="server" IsLiveSite="false" />
    <cms:LocalizedLabel ID="lblResults" runat="server" EnableViewState="false" class="Results"
        Visible="false" DisplayColon="true" />
    <cms:UniGrid ID="gridValidationResult" runat="server" ExportFileName="link_checker_results" ShowActionsLabel="false">
        <GridColumns>
            <ug:Column Source="statuscode" ExternalSourceName="statuscode" Caption="$validation.link.statuscode$"
                Wrap="false" Sort="statuscodevalue"/>
            <ug:Column Source="type" Caption="$validation.link.type$" ExternalSourceName="type" />
            <ug:Column Source="message" ExternalSourceName="message" Caption="$validation.link.message$"
                Width="60%" AllowSorting="false" />
            <ug:Column Source="url" ExternalSourceName="url" Caption="$validation.link.url$"
                Width="30%" AllowSorting="false" />
            <ug:Column Source="time" Caption="$validation.link.time$" ExternalSourceName="time" Sort="timeint" />
        </GridColumns>
        <PagerConfig DefaultPageSize="10" />
    </cms:UniGrid>
    <asp:HiddenField ID="hdnHTML" runat="server" EnableViewState="false" />
</asp:Panel>
