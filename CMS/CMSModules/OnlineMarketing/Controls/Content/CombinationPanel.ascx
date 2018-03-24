<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_OnlineMarketing_Controls_Content_CombinationPanel"
    CodeFile="CombinationPanel.ascx.cs" %>
<%@ Register Src="~/CMSModules/OnlineMarketing/FormControls/SelectMVTCombination.ascx"
    TagName="CombinationSelector" TagPrefix="cms" %>
<cms:CMSUpdateProgress ID="loading" runat="server" HandlePostback="true" EnableViewState="false" />
<asp:Panel ID="pnlMvtCombination" runat="server" CssClass="MVTCombinationPanel">
    <div class="Item">
        <cms:LocalizedLabel ID="lblCombination" runat="server" AssociatedControlID="combinationSelector" DisplayColon="true" />
    </div>
    <div class="Item">
        <cms:CombinationSelector ID="combinationSelector" runat="server" HighlightEnabled="true" />
    </div>
    <asp:PlaceHolder ID="plcEditCombination" runat="server">
        <div class="Item MVTCombinationEnabled">
            <cms:CMSCheckBox ID="chkEnabled" runat="server" />
        </div>
        <div class="Item">
            <cms:LocalizedLabel ID="lblCustomName" runat="server" AssociatedControlID="txtCustomName" DisplayColon="true" />
        </div>
        <div class="Item">
            <cms:CMSTextBox ID="txtCustomName" runat="server" MaxLength="200" CssClass="CustomName" />
        </div>
        <div class="Item">
            <cms:LocalizedButton ID="btnChange" runat="server" ButtonStyle="Default" />
        </div>
        <asp:PlaceHolder ID="plcUseCombination" runat="server" Visible="false">
            <div class="Item">
                <cms:LocalizedButton ID="btnUseCombination" runat="server" ButtonStyle="Primary" />
            </div>
        </asp:PlaceHolder>
        <div class="Item">
            <cms:LocalizedLabel ID="lblSaved" runat="server"></cms:LocalizedLabel>
        </div>
    </asp:PlaceHolder>
    <asp:HiddenField ID="hdnCurrentCombination" runat="server" />
</asp:Panel>
<asp:PlaceHolder ID="plcRunningTestWarning" runat="server" Visible="false">
    <div class="MVTCombinationPanelWarning alert alert-warning">
        <i class="icon-exclamation-triangle"></i>
        <cms:LocalizedLabel ID="lblWarning" runat="server"></cms:LocalizedLabel>
    </div>
</asp:PlaceHolder>
