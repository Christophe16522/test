<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_Ecommerce_Pages_Tools_Configuration_ShippingExtension_ShippingExtension_Edit_Processing"
    Theme="Default" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" CodeFile="ShippingExtension_Edit_Processing.aspx.cs" %>

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
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="plcContent" runat="Server">
    <table style="vertical-align: top">
        <tr>
            <td class="FieldLabel">
            </td>
            <td>
                <asp:Panel ID="pnlRro" runat="server" GroupingText="Processing Mode:">
                    <asp:RadioButton ID="rdoByUnit" runat="server" Text="By Shipping Unit" EnableViewState="false"
                        GroupName="rdo" AutoPostBack="false" /><br />
                    <asp:RadioButton ID="rdoByRange" runat="server" Text="By Shipping Range" Checked="true"
                        GroupName="rdo" EnableViewState="false" AutoPostBack="false" /><br />
                    <br />
                    <asp:CheckBox ID="chkChangeCountry" runat="server" CssClass="CheckBoxMovedLeft" Checked="True"
                        EnableViewState="true" />
                </asp:Panel>
            </td>
        </tr>
    </table>
</asp:Content>
