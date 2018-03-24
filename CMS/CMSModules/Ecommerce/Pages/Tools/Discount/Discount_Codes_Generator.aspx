<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_Ecommerce_Pages_Tools_Discount_Discount_Codes_Generator"
    Theme="Default" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master"
    CodeFile="Discount_Codes_Generator.aspx.cs" %>

<%@ Register Src="~/CMSAdminControls/AsyncControl.ascx" TagName="AsyncControl" TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/AsyncBackground.ascx" TagName="AsyncBackground" TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/UI/PageElements/PageTitle.ascx" TagName="PageTitle" TagPrefix="cms" %>

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
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:CMSPanel ID="pnlGeneral" runat="server" CssClass="ContentPanel">
        <cms:LocalizedHeading ID="headGeneral" runat="server" Level="4" ResourceString="com.generatecodes.generalInfo" EnableViewState="false" />
        <div class="form-horizontal">
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <cms:LocalizedLabel ID="lblnumberOfCodes" runat="server" EnableViewState="false" ResourceString="com.couponcodes.numberof"
                        DisplayColon="true" AssociatedControlID="txtNumberOfCodes" CssClass="control-label editing-form-label" ShowRequiredMark="true" />
                </div>
                <div class="editing-form-value-cell">
                    <cms:CMSTextBox runat="server" ID="txtNumberOfCodes" MaxLength="6" />
                </div>
            </div>
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <cms:LocalizedLabel ID="lblPrefix" runat="server" EnableViewState="false" ResourceString="com.couponcodes.prefix"
                        DisplayColon="true" CssClass="control-label editing-form-label" AssociatedControlID="txtPrefix" />
                </div>
                <div class="editing-form-value-cell">
                    <cms:CMSTextBox runat="server" ID="txtPrefix" MaxLength="100" />
                </div>
            </div>
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <cms:LocalizedLabel ID="lblTimesToUse" runat="server" EnableViewState="false" ResourceString="com.couponcodes.EachCanBeUsed"
                        DisplayColon="true" CssClass="control-label editing-form-label" AssociatedControlID="txtTimesToUse" />
                </div>
                <div class="editing-form-value-cell">
                    <div class="control-group-inline">
                    <cms:CMSTextBox runat="server" ID="txtTimesToUse" CssClass="input-width-20" WatermarkText="{$com.couponcode.unlimited$}" Text="1" MaxLength="6" />
                    <span class="form-control-text"><%= GetString("com.couponcodes.times") %></span>
                        </div>
                </div>
            </div>
        </div>
    </cms:CMSPanel>
</asp:Content>
