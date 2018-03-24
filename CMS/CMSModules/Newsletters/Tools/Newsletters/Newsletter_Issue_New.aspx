<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Newsletter_Issue_New.aspx.cs"
    Inherits="CMSModules_Newsletters_Tools_Newsletters_Newsletter_Issue_New" Theme="Default"
    EnableEventValidation="false" ValidateRequest="false" MasterPageFile="~/CMSMasterPages/UI/Dialogs/ModalDialogPage.master"
    Title="Newsletter - New issue" %>

<%@ Register Src="~/CMSAdminControls/Wizard/Header.ascx" TagPrefix="cms" TagName="WizardHeader" %>
<%@ Register Src="~/CMSAdminControls/UI/PageElements/HeaderActions.ascx" TagName="HeaderActions" TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/UI/UniControls/UniButton.ascx" TagPrefix="cms" TagName="UniButton" %>
<%@ Register Src="~/CMSModules/Newsletters/Controls/TemplateFlatSelector.ascx" TagPrefix="cms" TagName="TemplateFlatSelector" %>
<%@ Register Src="~/CMSModules/Newsletters/Controls/EditIssue.ascx" TagPrefix="cms" TagName="EditIssue" %>
<%@ Register Src="~/CMSModules/Newsletters/Controls/SendIssue.ascx" TagPrefix="cms" TagName="SendIssue" %>
<%@ Register Src="~/CMSModules/Newsletters/Controls/SendVariantIssue.ascx" TagPrefix="cms" TagName="SendVariant" %>
<%@ Register Src="~/CMSModules/Newsletters/Controls/VariantDialog.ascx" TagPrefix="cms" TagName="VariantDialog" %>
<%@ Register Src="~/CMSModules/Newsletters/Controls/VariantSlider.ascx" TagPrefix="cms" TagName="VariantSlider" %>

<asp:Content ID="cntTop" runat="server" ContentPlaceHolderID="plcHeaderTabs">
    <%-- Wizard header --%>
    <table class="GlobalWizard NewsletterWizard" cellspacing="0">
        <tr class="Top">
            <td class="Left">
                &nbsp;
            </td>
            <td class="Center">
                <cms:WizardHeader ID="ucHeader" runat="server" />
            </td>
            <td class="Right">
                &nbsp;
            </td>
        </tr>
    </table>
    <asp:Panel ID="pnlSlider" runat="server" CssClass="header-panel" EnableViewState="false"
        Visible="false">
        <cms:VariantSlider ID="ucVariantSlider" runat="server" />
    </asp:Panel>
    <%-- Actions panel --%>
    <asp:Panel ID="pnlActions" runat="server" CssClass="cms-edit-menu" EnableViewState="false"
        Visible="false">
        <cms:HeaderActions ID="hdrActions" runat="server" ShortID="ha" IsLiveSite="false" />
    </asp:Panel>
    <%-- A/B test variant dialog --%>
    <cms:VariantDialog ID="ucVariantDialog" runat="server" Visible="false" />
</asp:Content>
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:CMSUpdateProgress ID="up" runat="server" HandlePostback="true" />
    <cms:CMSUpdatePanel runat="server" UpdateMode="Always">
        <ContentTemplate>
            <cms:MessagesPlaceHolder ID="plcMessages" runat="server" OffsetX="16" OffsetY="16" UseRelativePlaceHolder="false" IsLiveSite="false" />
        </ContentTemplate>
    </cms:CMSUpdatePanel>
    <%-- Template selection step --%>
    <asp:PlaceHolder ID="plcSelectTemplate" runat="server" Visible="true">
        <%-- Template selector --%>
        <cms:TemplateFlatSelector ID="selectElem" runat="server" ShortID="t" />
        <%-- Template step buttons --%>
        <div class="PageFooterLine" style="position: fixed">
            <div class="FloatRight">
                <cms:UniButton ID="btnTemplateNext" runat="server" ShowAsButton="true" />
                <cms:LocalizedButton ID="btnTemplateClose" runat="server" ButtonStyle="Primary"
                    OnClientClick="RefreshPage();setTimeout('CloseDialog()',200);return false;" ResourceString="general.close" EnableViewState="false" />
            </div>
            <div class="ClearBoth">
                &nbsp;
            </div>
        </div>
    </asp:PlaceHolder>
    <%-- Content edit step --%>
    <asp:PlaceHolder ID="plcContent" runat="server" Visible="false">
        <cms:EditIssue ID="editElem" runat="server" ShortID="e" Enabled="true" />
    </asp:PlaceHolder>
    <%-- Send step --%>
    <asp:PlaceHolder ID="plcSend" runat="server" Visible="false">
        <div class="NewsletterWizardStep">
            <cms:SendIssue ID="sendElem" runat="server" ShowScheduler="true" ShowSendDraft="false"
                ShowSendLater="true" ShortID="s" Visible="false" />
            <cms:SendVariant ID="sendVariant" runat="server" ShortID="sv" Visible="false" />
        </div>
    </asp:PlaceHolder>
    <asp:HiddenField runat="server" ID="hdnCurrent" EnableViewState="false" />
    <asp:HiddenField runat="server" ID="hdn1stStepSkipped" EnableViewState="false" />
    <asp:HiddenField runat="server" ID="hdnIssueID" EnableViewState="false" />
</asp:Content>
<asp:Content ID="cntFooter" runat="server" ContentPlaceHolderID="plcFooter">
    <%-- Footer buttons - hidden in template selection step --%>
    <asp:PlaceHolder ID="plcFooterButtons" runat="server">
        <div class="FloatRight">
            <cms:LocalizedButton ID="btnClose" runat="server" ButtonStyle="Default" OnClientClick="RefreshPage();setTimeout('CloseDialog()',200);return false;"
                ResourceString="general.cancel" />
            <cms:UniButton ID="btnBack" runat="server" ShowAsButton="true" ButtonStyle="Default" />
            <cms:UniButton ID="btnNext" runat="server" ShowAsButton="true" ButtonStyle="Primary" />
            <cms:UniButton ID="btnSave" runat="server" ShowAsButton="true" Visible="false" ButtonStyle="Primary" />
            <cms:UniButton ID="btnSendClose" runat="server" ShowAsButton="true" Visible="false" ButtonStyle="Primary" />
            <cms:UniButton ID="btnFinish" runat="server" ShowAsButton="true" ButtonStyle="Primary" />
        </div>
    </asp:PlaceHolder>
</asp:Content>