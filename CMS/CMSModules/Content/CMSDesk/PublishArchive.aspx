<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master"
    Inherits="CMSModules_Content_CMSDesk_PublishArchive" Title="Publishes or archives multiple documents"
    ValidateRequest="false" Theme="Default" CodeFile="PublishArchive.aspx.cs" %>

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
                        <cms:PageTitle ID="titleElemAsync" runat="server" HideTitle="true" />
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
</asp:Content>
<asp:Content ID="cntContent" ContentPlaceHolderID="plcContent" runat="server" EnableViewState="false">
    <asp:Panel runat="server" ID="pnlContent" EnableViewState="false">
        <asp:Panel ID="pnlPublish" runat="server" EnableViewState="false">
            <cms:LocalizedHeading runat="server" ID="headQuestion" Level="4" EnableViewState="false" />
            <asp:Panel ID="pnlDocList" runat="server" Visible="false" CssClass="form-control vertical-scrollable-list"
                EnableViewState="false">
                <asp:Label ID="lblDocuments" runat="server" CssClass="ContentLabel" EnableViewState="false" />
            </asp:Panel>
            <div class="checkbox-list-vertical">
                <asp:PlaceHolder ID="plcCheck" runat="server" EnableViewState="false">
                    <asp:PlaceHolder ID="plcAllCultures" runat="server">
                        <cms:CMSCheckBox ID="chkAllCultures" runat="server" CssClass="ContentCheckbox"
                            EnableViewState="false" Checked="true" />
                    </asp:PlaceHolder>
                    <cms:CMSCheckBox ID="chkUnderlying" runat="server" CssClass="ContentCheckbox"
                        EnableViewState="false" Checked="true" />
                    <asp:PlaceHolder ID="plcUndoCheckOut" runat="server">
                        <cms:CMSCheckBox ID="chkUndoCheckOut" runat="server" CssClass="ContentCheckbox"
                            EnableViewState="false" ResourceString="content.undocheckedoutdocs" />
                    </asp:PlaceHolder>
                </asp:PlaceHolder>
            </div>
            <br />
            <div class="control-group-inline">
                <div class="keep-white-space-fixed">
                    <cms:LocalizedButton ID="btnOk" runat="server" ButtonStyle="Primary" OnClick="btnOK_Click"
                        ResourceString="general.yes" EnableViewState="false" />
                    <cms:LocalizedButton ID="btnNo" runat="server" ButtonStyle="Primary" OnClick="btnNo_Click"
                        ResourceString="general.no" EnableViewState="false" />
                </div>
            </div>
        </asp:Panel>
    </asp:Panel>
    <script type="text/javascript">
        //<![CDATA[

        // Display the document
        function SelectNode(nodeId) {
            if (parent != null) {
                if (parent.SelectNode != null) {
                    parent.SelectNode(nodeId);
                }
                RefreshTree(nodeId);
            }
        }

        function RefreshTree(nodeId) {
            if (parent != null) {
                if (parent.RefreshTree != null) {
                    parent.RefreshTree(nodeId, nodeId);
                }
            }

        }
        //]]>                        
    </script>
    <asp:Literal runat="server" ID="ltlScript" EnableViewState="false" />
</asp:Content>
