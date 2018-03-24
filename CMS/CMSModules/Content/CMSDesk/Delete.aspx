<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_Content_CMSDesk_Delete"
    Theme="Default" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Title="Content - Delete"
    CodeFile="Delete.aspx.cs" %>

<%@ Register Src="~/CMSAdminControls/UI/PageElements/PageTitle.ascx" TagName="PageTitle"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/AsyncControl.ascx" TagName="AsyncControl" TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/AsyncBackground.ascx" TagName="AsyncBackground"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/Content/FormControls/Documents/SelectPath.ascx"
    TagName="SelectPath" TagPrefix="cms" %>
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
                        <cms:CMSButton runat="server" ID="btnCancel" ButtonStyle="Primary" />
                    </asp:Panel>
                    <asp:Panel ID="pnlAsyncContent" runat="server" CssClass="PageContent">
                        <cms:AsyncControl ID="ctlAsync" runat="server" />
                    </asp:Panel>
                </asp:Panel>
            </div>
        </div>
    </asp:Panel>
</asp:Content>
<asp:Content ID="plcContent" ContentPlaceHolderID="plcContent" runat="server">
    <asp:Panel runat="server" ID="pnlContent">
        <asp:Panel ID="pnlDelete" runat="server">
            <cms:LocalizedHeading runat="server" ID="headQuestion" Level="3" EnableViewState="false" />
            <asp:Panel ID="pnlDocList" runat="server" Visible="false" CssClass="content-block-50 form-control vertical-scrollable-list"
                EnableViewState="false">
                <asp:Label ID="lblDocuments" runat="server" CssClass="ContentLabel" EnableViewState="true" />
            </asp:Panel>
            <asp:PlaceHolder ID="plcCheck" runat="server" EnableViewState="true">
                <cms:CMSPanel ID="pnlDeleteRoot" runat="server" CssClass="content-block-50" Visible="false">
                    <cms:LocalizedHeading runat="server" ID="LocalizedHeading1" Level="4" ResourceString="root.deletesettings" EnableViewState="false" />
                    <div>
                        <cms:CMSRadioButtonList ID="rblRoot" runat="server" AutoPostBack="true" />
                    </div>
                </cms:CMSPanel>
                <asp:Panel ID="pnlDeleteDocument" runat="server" EnableViewState="false" CssClass="content-block-50">
                    <cms:LocalizedHeading runat="server" ID="headDeleteDocument" Level="4" EnableViewState="false" />
                    <div>
                        <cms:CMSCheckBox ID="chkDestroy" runat="server" CssClass="ContentCheckBox" Visible="false"
                            EnableViewState="false" />
                    </div>
                    <div>
                        <cms:CMSCheckBox ID="chkAllCultures" runat="server" CssClass="ContentCheckBox" EnableViewState="false" />
                    </div>
                </asp:Panel>
            </asp:PlaceHolder>
            <asp:Panel ID="pnlDeleteSKU" runat="server" Visible="false" CssClass="content-block-50">
                <cms:LocalizedHeading runat="server" ID="headDeleteSKU" Level="4" EnableViewState="false" />
                <asp:Label ID="lblSKUActionInfo" runat="server" CssClass="InfoLabel" EnableViewState="true" />
                <div>
                    <cms:CMSRadioButtonList ID="rblSKUAction" runat="server" RepeatDirection="Vertical" />
                </div>
            </asp:Panel>
            <asp:Panel ID="pnlSeo" runat="server" CssClass="content-block-50">
                <cms:LocalizedHeading runat="server" ID="head" Level="4" ResourceString="ContentDelete.Seo" EnableViewState="false" />
                <cms:CMSUpdatePanel runat="server" ID="pnlAltPath" UpdateMode="Conditional">
                    <ContentTemplate>
                        <div class="content-block">
                            <cms:CMSCheckBox ID="chkUseDeletedPath" runat="server" ResourceString="content.delete.usealtpath"
                                AutoPostBack="true" CssClass="ContentCheckBox" EnableViewState="true" />
                        </div>
                        <asp:PlaceHolder runat="server" ID="plcDeleteSettings" Visible="false">
                            <div class="form-horizontal">
                                <div class="form-group">
                                    <div class="editing-form-label-cell">
                                        <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblAltPath" ResourceString="content.delete.altpath"
                                            DisplayColon="true" />
                                    </div>
                                    <div class="editing-form-value-cell">
                                        <cms:SelectPath ID="selAltPath" runat="server" IsLiveSite="false" SinglePathMode="true" />
                                    </div>
                                </div>
                                <div class="form-group">
                                    <div class="editing-form-label-cell">
                                        <cms:LocalizedLabel CssClass="control-label" runat="server" AssociatedControlID="chkAltAliases" ID="lblAltAliases"
                                            ResourceString="content.delete.altaliases" DisplayColon="true" />
                                    </div>
                                    <div class="editing-form-value-cell">
                                        <cms:CMSCheckBox runat="server" ID="chkAltAliases" Checked="true" />
                                    </div>
                                </div>
                                <div class="form-group">
                                    <div class="editing-form-label-cell">
                                        <cms:LocalizedLabel CssClass="control-label" runat="server" AssociatedControlID="chkAltSubNodes" ID="lblAltUrl"
                                            ResourceString="content.delete.altsub" DisplayColon="true" />
                                    </div>
                                    <div class="editing-form-value-cell">
                                        <cms:CMSCheckBox runat="server" ID="chkAltSubNodes" Checked="true" />
                                    </div>
                                </div>
                            </div>
                        </asp:PlaceHolder>
                    </ContentTemplate>
                </cms:CMSUpdatePanel>
            </asp:Panel>
            <asp:PlaceHolder runat="server" ID="plcDeleteRoot" Visible="false">
                <div class="content-block-50">
                    <cms:MessagesPlaceHolder runat="server" ID="messagesPlaceholder" />
                    <cms:CMSCheckBox ID="chkDeleteRoot" runat="server" CssClass="ContentCheckBox"
                        EnableViewState="false" ResourceString="Delete.RootConfirm" />
                </div>
            </asp:PlaceHolder>
            <div class="btn-actions">
                <cms:LocalizedButton ID="btnOk" runat="server" ButtonStyle="Primary" OnClick="btnOK_Click"
                    ResourceString="general.yes" EnableViewState="false" />
                <cms:LocalizedButton ID="btnNo" runat="server" ButtonStyle="Primary" ResourceString="general.no" EnableViewState="false" />
            </div>
        </asp:Panel>
    </asp:Panel>
    <asp:Literal ID="ltlScript" runat="server" EnableViewState="false" />
</asp:Content>
