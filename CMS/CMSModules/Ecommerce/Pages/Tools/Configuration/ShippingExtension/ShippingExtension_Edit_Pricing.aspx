<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_Ecommerce_Pages_Tools_Configuration_ShippingExtension_ShippingExtension_Edit_Pricing"
    Theme="Default" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" CodeFile="ShippingExtension_Edit_Pricing.aspx.cs" %>

<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" TagName="UniGrid" TagPrefix="cms" %>
<%@ Register Namespace="CMS.UIControls.UniGridConfig" TagPrefix="ug" Assembly="CMS.UIControls" %>
<%@ Register Src="~/CMSFormControls/Sites/SiteOrGlobalSelector.ascx" TagName="SiteSelector"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSWebParts/Viewers/Query/querydatagrid.ascx" TagName="Grid"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/UI/PageElements/HeaderActions.ascx" TagName="HeaderActions"
    TagPrefix="cms" %>
<asp:Content ID="cntControls" runat="server" ContentPlaceHolderID="plcSiteSelector">
    <cms:LocalizedLabel runat="server" ID="lblSite" EnableViewState="false" DisplayColon="true"
        ResourceString="General.Site" />
    <cms:SiteSelector ID="SelectSite" runat="server" IsLiveSite="false" />
</asp:Content>
<asp:Content ID="cntActions" runat="server" ContentPlaceHolderID="plcActions">
    <cms:FormSubmitButton runat="server" ID="btnOk" OnClick="btnOK_Click" EnableViewState="false" />
    <div class="LeftAlign">
        <cms:HeaderActions ID="hdrActions" runat="server" IsLiveSite="false" />
    </div>
    <cms:CMSUpdatePanel ID="pnlActons" runat="server">
        <ContentTemplate>
            <cms:LocalizedLabel ID="lblWarnNew" runat="server" ResourceString="com.chooseglobalorsite"
                EnableViewState="false" Visible="false" CssClass="ActionsInfoLabel" />
            <div class="ClearBoth">
            </div>
        </ContentTemplate>
    </cms:CMSUpdatePanel>
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="plcContent" runat="Server">
    <asp:Panel ID="pnlPriceParams" runat="server">
        <table style="vertical-align: top; display: block">
            <tr>
                <td class="FieldLabel">
                    <cms:LocalizedLabel runat="server" ID="lblShippingBase" EnableViewState="false" Text="Shipping base cost:"></cms:LocalizedLabel>
                </td>
                <td>
                    <asp:TextBox ID="txtBasePrice" runat="server" TextMode="SingleLine" EnableViewState="false" />
                </td>
                <td>
                </td>
            </tr>
            <tr>
                <td class="FieldLabel">
                    <cms:LocalizedLabel runat="server" ID="lblUnitPrice" EnableViewState="false" Text="Unit Price:"></cms:LocalizedLabel>
                </td>
                <td>
                    <asp:TextBox ID="txtUnitPrice" runat="server" TextMode="SingleLine" EnableViewState="false" />
                </td>
                <td>
                </td>
            </tr>
            <tr>
                <td class="FieldLabel">
                    <cms:LocalizedLabel runat="server" ID="lblProcessMode" EnableViewState="false" Text="Processing:"></cms:LocalizedLabel>
                </td>
                <td class="FieldLabel" style="display: block">
                    <asp:RadioButton ID="rdoByUnit" runat="server" Text="By Shipping Unit" EnableViewState="false"
                        GroupName="rdo" AutoPostBack="false" />
                </td>
                <td>
                    <asp:RadioButton ID="rdoByRange" runat="server" Text="By Shipping Range" Checked="true"
                        GroupName="rdo" EnableViewState="false" AutoPostBack="false" />
                </td>
            </tr>
        </table>
    </asp:Panel>
    <br />
    <br />
    <asp:Panel ID="pnlPriceRange" runat="server" GroupingText="Price range detail">
        <div class="CMSEditMenu">
            <cms:HeaderActions ID="grdAction" runat="server" />
        </div>
        <br />
        <cms:CMSUpdatePanel ID="pnlUpdate" runat="server">
            <ContentTemplate>
                <cms:UniGrid runat="server" ID="UniGrid" GridName="ShippingExtension_Edit_Pricing.xml"
                    IsLiveSite="false" />
            </ContentTemplate>
        </cms:CMSUpdatePanel>
    </asp:Panel>
</asp:Content>
