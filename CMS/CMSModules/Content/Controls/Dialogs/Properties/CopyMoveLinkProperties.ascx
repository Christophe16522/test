<%@ Control Language="C#" AutoEventWireup="true" CodeFile="CopyMoveLinkProperties.ascx.cs"
    Inherits="CMSModules_Content_Controls_Dialogs_Properties_CopyMoveLinkProperties" %>

<%@ Register Src="~/CMSAdminControls/UI/PageElements/PageTitle.ascx" TagName="PageTitle" TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/AsyncControl.ascx" TagName="AsyncControl" TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/AsyncBackground.ascx" TagName="AsyncBackground" TagPrefix="cms" %>

<div>
    <asp:Panel runat="server" ID="pnlLog" Visible="false">
        <cms:AsyncBackground ID="backgroundElem" runat="server" />
        <div class="AsyncLogArea">
            <div>
                <asp:Panel ID="pnlAsyncBody" runat="server" CssClass="PageBody">
                    <asp:Panel ID="pnlTitleAsync" runat="server" CssClass="PageHeader">
                        <cms:PageTitle ID="titleElemAsync" runat="server" />
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
    <div class="DialogInfoArea" id="ContentDiv">
        <asp:Panel runat="server" ID="pnlEmpty" Visible="true" EnableViewState="false">
            <asp:Label runat="server" ID="lblEmpty" EnableViewState="false" />
        </asp:Panel>
        <asp:Panel runat="server" ID="pnlGeneralTab" Visible="false">
            <div class="form-horizontal">
                <div class="form-group">
                    <div class="editing-form-label-cell">
                        <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblCopyMoveInfo" ResourceString="dialogs.copymove.target"
                            EnableViewState="false" DisplayColon="true" AssociatedControlID="lblAliasPath" />
                    </div>
                    <div class="editing-form-value-cell">
                        <asp:Label runat="server" CssClass="form-control-text" ID="lblAliasPath" EnableViewState="false" />
                    </div>
                </div>
                <asp:PlaceHolder ID="plcUnderlying" runat="server">
                    <div class="form-group">
                        <div class="editing-form-label-cell">
                            <cms:LocalizedLabel CssClass="control-label" ID="lblUnderlying" runat="server" DisplayColon="True"
                                ResourceString="contentrequest.linkunderlying" AssociatedControlID="chkUnderlying" />
                        </div>
                        <div class="editing-form-value-cell">
                            <cms:CMSCheckBox ID="chkUnderlying" runat="server" AutoPostBack="true" OnCheckedChanged="chkUnderlying_OnCheckedChanged"
                                Checked="true" />
                        </div>
                    </div>
                </asp:PlaceHolder>
                <asp:PlaceHolder ID="plcCopyPermissions" runat="server">
                    <div class="form-group">
                        <div class="editing-form-label-cell">
                            <cms:LocalizedLabel CssClass="control-label" runat="server" ResourceString="contentrequest.copypermissions"
                                DisplayColon="True" AssociatedControlID="chkCopyPermissions" />
                        </div>
                        <div class="editing-form-value-cell">
                            <cms:CMSCheckBox ID="chkCopyPermissions" runat="server" AutoPostBack="true"
                                OnCheckedChanged="chkCopyPermissions_OnCheckedChanged" Checked="false" />
                        </div>
                    </div>
                </asp:PlaceHolder>
                <asp:PlaceHolder ID="plcPreservePermissions" runat="server">
                    <div class="form-group">
                        <div class="editing-form-label-cell">
                            <cms:LocalizedLabel CssClass="control-label" runat="server" DisplayColon="True"
                                ResourceString="contentrequest.preservepermissions" AssociatedControlID="chkPreservePermissions" />
                        </div>
                        <div class="editing-form-value-cell">
                            <cms:CMSCheckBox ID="chkPreservePermissions" runat="server" AutoPostBack="true"
                                OnCheckedChanged="chkPreservePermissions_OnCheckedChanged" Checked="false" />
                        </div>
                    </div>
                </asp:PlaceHolder>
                <div class="form-group">
                    <div class="editing-form-label-cell">
                        <asp:Label CssClass="control-label" ID="lblDocToCopy" runat="server" DisplayColon="true"
                            EnableViewState="false" AssociatedControlID="lblDocToCopyList" />
                    </div>
                    <div class="editing-form-value-cell">
                        <div class="form-control vertical-scrollable-list">
                            <asp:Label ID="lblDocToCopyList" runat="server" EnableViewState="false" />
                        </div>
                    </div>
                </div>
            </div>
            <asp:Label ID="lblError" runat="server" CssClass="ErrorLabel" EnableViewState="false" />
            <asp:Label ID="lblInfo" runat="server" CssClass="InfoLabel" EnableViewState="false" />
        </asp:Panel>
    </div>
</div>