<%@ Control Language="C#" AutoEventWireup="true" CodeFile="SendVariantIssue.ascx.cs"
    Inherits="CMSModules_Newsletters_Controls_SendVariantIssue" %>
<%@ Register Src="~/CMSModules/Newsletters/Controls/GroupSizeSlider.ascx" TagPrefix="cms"
    TagName="GroupSlider" %>
<%@ Register Src="~/CMSModules/Newsletters/Controls/WinnerOptions.ascx" TagPrefix="cms"
    TagName="WinnerOptions" %>
<%@ Register Src="~/CMSModules/Newsletters/Controls/VariantMailout.ascx" TagPrefix="cms"
    TagName="VariantMailout" %>
<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" TagName="UniGrid" TagPrefix="cms" %>
<%@ Register Namespace="CMS.UIControls.UniGridConfig" TagPrefix="ug" Assembly="CMS.UIControls" %>

<cms:LocalizedLabel runat="server" ID="lblTestGroup" ResourceString="newsletterissue_send.sizeoftestgroup"
    EnableViewState="false" CssClass="control-label" />
<asp:Panel runat="server" ID="pnlSlider">
    <cms:GroupSlider ID="ucGroupSlider" runat="server" />
</asp:Panel>
<br />
<asp:Label ID="lblAdditionalInfo" runat="server" CssClass="InfoLabel" EnableViewState="false" />
<cms:VariantMailout ID="ucMailout" runat="server" />
<br />
<cms:WinnerOptions ID="ucWO" runat="server" Visible="false" />
<br />
<cms:CMSUpdatePanel runat="server" ID="pnlU2">
    <ContentTemplate>
        <asp:Label runat="server" ID="lblWillBeSent" EnableViewState="false" />
    </ContentTemplate>
</cms:CMSUpdatePanel>
