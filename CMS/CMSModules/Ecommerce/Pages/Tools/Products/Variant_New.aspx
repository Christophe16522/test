<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_Ecommerce_Pages_Tools_Products_Variant_New"
    Theme="Default" CodeFile="Variant_New.aspx.cs" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" %>

<%@ Register TagPrefix="cms" Src="~/CMSModules/Ecommerce/Controls/UI/ProductOptions/SelectVariantCategory.ascx" TagName="SelectVariantCategory" %>
<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" TagName="UniGrid" TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/Ecommerce/Controls/Filters/ProductVariantFilter.ascx" TagName="ProductVariantFilter" TagPrefix="cms" %>
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
<asp:Content ContentPlaceHolderID="plcContent" ID="content" runat="server">
    <asp:Panel runat="server" ID="pnlContent" CssClass="FormPanel">
        <cms:CMSUpdatePanel ID="updatePanel" runat="server">
            <ContentTemplate>
                <cms:MessagesPlaceHolder runat="server" ID="plcMessages" />
                <cms:CMSPanel ID="pnlVariantSelector" runat="server" CssClass="ContentPanel">
                    <cms:LocalizedHeading runat="server" ID="headCategories" Level="4" ResourceString="com.variants.selectorgroupingtext" EnableViewState="false" />
                    <cms:SelectVariantCategory ID="CategorySelector" runat="server" />
                </cms:CMSPanel>
                <cms:CMSPanel ID="pnlAviableVariants" runat="server" CssClass="ContentPanel">
                    <cms:LocalizedHeading runat="server" ID="headVariants" Level="4" ResourceString="com.variants.AviableVariantsgroupingtext" EnableViewState="false" />
                    <cms:LocalizedLinkButton
                        ID="btnFilter" runat="server" EnableViewState="false" ResourceString="general.showfilter" OnClick="btnFilter_Click" />
                    <p>
                        <asp:PlaceHolder ID="plcFilter" runat="server" Visible="false">
                            <cms:ProductVariantFilter ID="VariantFilter" runat="server" ShowNameNumberSearch="false" ShowOnlyUsedCategories="false" />
                        </asp:PlaceHolder>
                    </p>
                    <cms:UniGrid ID="VariantGrid" runat="server" ShowObjectMenu="false" ShowActionsMenu="false" DelayedReload="true" OnOnExternalDataBound="VariantGrid_OnExternalDataBound">
                        <GridColumns />
                        <GridOptions />
                    </cms:UniGrid>
                    <cms:LocalizedLabel runat="server" ID="lblSpecify" ResourceString="com.variants.specify" Visible="False" />
                </cms:CMSPanel>
            </ContentTemplate>
        </cms:CMSUpdatePanel>
    </asp:Panel>
</asp:Content>
