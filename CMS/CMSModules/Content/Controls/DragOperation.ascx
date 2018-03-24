<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_Content_Controls_DragOperation"
    CodeFile="DragOperation.ascx.cs" %>
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
                <asp:Panel ID="pnlTitleAsync" runat="server" CssClass="PageHeader">
                    <cms:PageTitle ID="titleElemAsync" runat="server" />
                </asp:Panel>
                <asp:Panel ID="pnlCancel" runat="server" CssClass="header-panel">
                    <cms:LocalizedButton runat="server" ID="btnCancel" ResourceString="General.Cancel"
                        ButtonStyle="Primary" />
                </asp:Panel>
                <asp:Panel ID="pnlAsyncContent" runat="server" CssClass="PageContent">
                    <cms:AsyncControl ID="ctlAsync" runat="server" />
                </asp:Panel>
            </asp:Panel>
        </div>
    </div>
</asp:Panel>
<asp:Panel runat="server" ID="pnlContent" CssClass="PageContent">
    <cms:MessagesPlaceHolder ID="plcMess" runat="server" />
    <asp:Panel ID="pnlAction" runat="server" EnableViewState="false">
        <cms:LocalizedHeading runat="server" ID="headQuestion" Level="4" EnableViewState="false" />
        <cms:LocalizedLabel ID="lblTarget" runat="server" CssClass="ContentLabel" EnableViewState="false" />
        <br />
        <br />
        <asp:PlaceHolder ID="plcCopyCheck" runat="server" EnableViewState="false" Visible="false">
            <div>
                <cms:CMSCheckBox ID="chkChild" runat="server" CssClass="ContentCheckbox" EnableViewState="false"
                    Checked="true" />
            </div>
        </asp:PlaceHolder>
        <asp:PlaceHolder ID="plcCopyPerm" runat="server" EnableViewState="false">
            <div>
                <cms:CMSCheckBox ID="chkCopyPerm" runat="server" CssClass="ContentCheckbox"
                    EnableViewState="false" Checked="false" />
            </div>
        </asp:PlaceHolder>
        <br />
        <cms:LocalizedButton ID="btnOk" runat="server" ButtonStyle="Primary" OnClick="btnOK_Click"
            ResourceString="general.yes" EnableViewState="false" /><cms:LocalizedButton ID="btnNo"
            runat="server" ButtonStyle="Primary" ResourceString="general.no" EnableViewState="false" />
    </asp:Panel>
</asp:Panel>
<asp:Literal ID="ltlScript" runat="server" EnableViewState="false" />