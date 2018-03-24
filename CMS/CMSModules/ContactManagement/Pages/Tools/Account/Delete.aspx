<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Delete.aspx.cs" Inherits="CMSModules_ContactManagement_Pages_Tools_Account_Delete"
    Theme="Default" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Title="Account - Delete" %>

<%@ Register Src="~/CMSAdminControls/UI/PageElements/PageTitle.ascx" TagName="PageTitle"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/AsyncControl.ascx" TagName="AsyncControl" TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/AsyncBackground.ascx" TagName="AsyncBackground"
    TagPrefix="cms" %>

<asp:Content ContentPlaceHolderID="plcBeforeBody" runat="server" ID="cntBeforeBody">
    <asp:Panel runat="server" ID="pnlLog" Visible="false">
        <cms:AsyncBackground ID="backgroundElem" runat="server" />
        <div class="AsyncLogArea">
            <div>
                <asp:Panel ID="pnlAsyncBody" runat="server" CssClass="PageBody">
                    <asp:Panel ID="pnlTitleAsync" runat="server" CssClass="PageHeader">
                        <cms:PageTitle ID="titleElemAsync" runat="server" SetWindowTitle="false" HideTitle="true" />
                    </asp:Panel>
                    <asp:Panel ID="pnlCancel" runat="server" CssClass="header-panel">
                        <cms:LocalizedButton runat="server" ID="btnCancel" ButtonStyle="Primary" EnableViewState="false"
                            ResourceString="general.cancel" />
                    </asp:Panel>
                    <asp:Panel ID="pnlAsyncContent" runat="server" CssClass="PageContent">
                        <cms:AsyncControl ID="ctlAsync" runat="server" MaxLogLines="1000" />
                    </asp:Panel>
                </asp:Panel>
            </div>
        </div>
    </asp:Panel>
</asp:Content>
<asp:Content ID="plcContent" ContentPlaceHolderID="plcBeforeContent" runat="server"
    EnableViewState="false">
    <asp:Panel runat="server" ID="pnlContent" CssClass="PageContent" EnableViewState="false">
        <asp:Panel ID="pnlDelete" runat="server" EnableViewState="false">
            <cms:LocalizedHeading runat="server" ID="headQuestion" Level="4" EnableViewState="false" ResourceString="om.account.deletequestion" />
            <asp:Panel ID="pnlAccountList" runat="server" Visible="false" CssClass="form-control vertical-scrollable-list"
                EnableViewState="false">
                <asp:Label ID="lblAccounts" runat="server" EnableViewState="true" />
            </asp:Panel>
            <asp:Panel ID="plcCheck" runat="server" EnableViewState="false" CssClass="checkbox-list-vertical content-block-50">
                <cms:CMSCheckBox ID="chkChildren" runat="server"
                    EnableViewState="false" ResourceString="om.account.deletechildaccounts" Checked="true" />
                <cms:CMSCheckBox ID="chkBranches" runat="server"
                    EnableViewState="false" ResourceString="om.account.deletesubsidiaries" />
            </asp:Panel>
            <div class="btn-actions">
                <cms:LocalizedButton ID="btnOk" runat="server" ButtonStyle="Primary" OnClick="btnOK_Click"
                    ResourceString="general.yes" EnableViewState="false" />
                <cms:LocalizedButton ID="btnNo" runat="server" ButtonStyle="Primary" ResourceString="general.no"
                    EnableViewState="false" />
            </div>
        </asp:Panel>
    </asp:Panel>
    <asp:Literal ID="ltlScript" runat="server" EnableViewState="false" />
</asp:Content>
