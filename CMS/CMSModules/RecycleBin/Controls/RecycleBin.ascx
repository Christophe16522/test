<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_RecycleBin_Controls_RecycleBin"
    CodeFile="RecycleBin.ascx.cs" %>

<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" TagName="UniGrid" TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/UI/PageElements/PageTitle.ascx" TagName="PageTitle"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/AsyncControl.ascx" TagName="AsyncControl" TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/AsyncBackground.ascx" TagName="AsyncBackground"
    TagPrefix="cms" %>

<asp:Panel runat="server" ID="pnlLog" Visible="false">
    <cms:AsyncBackground ID="backgroundElem" runat="server" />
    <div class="AsyncLogArea">
        <div>
            <asp:Panel ID="pnlAsyncBody" runat="server" CssClass="PageBody">
                <asp:Panel ID="pnlTitleAsync" runat="server" CssClass="PageHeader" EnableViewState="false">
                    <cms:PageTitle ID="titleElemAsync" runat="server" EnableViewState="false" />
                </asp:Panel>
                <asp:Panel ID="pnlCancel" runat="server" CssClass="header-panel" EnableViewState="false">
                    <cms:LocalizedButton runat="server" ID="btnCancel" ResourceString="general.cancel"
                        ButtonStyle="Primary" EnableViewState="false" />
                </asp:Panel>
                <asp:Panel ID="pnlAsyncContent" runat="server" CssClass="PageContent">
                    <cms:AsyncControl ID="ctlAsync" runat="server" />
                </asp:Panel>
            </asp:Panel>
        </div>
    </div>
</asp:Panel>
<cms:CMSUpdatePanel ID="pnlUpdate" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
        <cms:MessagesPlaceHolder ID="plcMess" runat="server" />
        <div>
            <cms:UniGrid ID="ugRecycleBin" runat="server" GridName="~/CMSModules/RecycleBin/Controls/RecycleBin.xml"
                IsLiveSite="false" HideControlForZeroRows="true" />
            <asp:Panel ID="pnlFooter" runat="server" CssClass="form-horizontal mass-action">
                <div class="form-group">
                    <div class="mass-action-value-cell">
                        <cms:CMSDropDownList ID="drpWhat" runat="server" />
                        <cms:CMSDropDownList ID="drpAction" runat="server" />
                        <cms:LocalizedButton ID="btnOk" runat="server" ResourceString="general.ok" ButtonStyle="Primary"
                            EnableViewState="false" OnClick="btnOk_OnClick" />
                        <asp:Label ID="lblValidation" runat="server" CssClass="InfoLabel" EnableViewState="false"
                            Style="display: none;" />
                    </div>
                </div>
            </asp:Panel>
        </div>
    </ContentTemplate>
</cms:CMSUpdatePanel>