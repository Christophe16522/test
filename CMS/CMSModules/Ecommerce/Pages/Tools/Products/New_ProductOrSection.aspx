<%@ Page Language="C#" AutoEventWireup="true" CodeFile="New_ProductOrSection.aspx.cs"
    Theme="Default" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Inherits="CMSModules_Ecommerce_Pages_Tools_Products_New_ProductOrSection" %>

<%@ Register TagPrefix="cms" TagName="DocTypeSelector" Src="~/CMSModules/Content/Controls/DocTypeSelection.ascx" %>
<asp:Content ContentPlaceHolderID="plcContent" ID="content" runat="server">
    <cms:CMSPanel runat="server" ID="pnlProductOrSectionType" class="ProductOrSectionType">
        <cms:LocalizedHeading runat="server" ID="headTitle" Level="3" ResourceString="com.NewProductOrSectionInfo" EnableViewState="false" />
        <table style="padding: 2px;">
            <tr>
                <td style="vertical-align: top;">
                    <div class="ProductTypeSelection">
                        <cms:DocTypeSelector runat="server" ID="ProductTypes" AllowNewABTest="false" AllowNewLink="false"
                            Where="ClassIsProduct = 1" NoDataAsError="false" />
                    </div>
                </td>
                <td class="TypeSelectionSeparator">
                </td>
                <td style="vertical-align: top;">
                    <div class="SectionTypeSelection">
                        <cms:DocTypeSelector runat="server" ID="SectionTypes" AllowNewABTest="false" AllowNewLink="false"
                            Where="ClassIsProductSection = 1" NoDataAsError="false" />
                    </div>
                </td>
            </tr>
        </table>
        <asp:Panel runat="server" ID="pnlFooter" CssClass="PageSeparator">
            <asp:HyperLink runat="server" ID="lnkNewLink" CssClass="content-new-link cms-icon-link" EnableViewState="false">
                <cms:CMSIcon ID="iconNewLink" runat="server" CssClass="icon-chain" EnableViewState="false" />
                <cms:LocalizedLabel ID="lblNewLink" runat="server" ResourceString="com.LinkProductOrSection"
                    EnableViewState="false" />
            </asp:HyperLink>
        </asp:Panel>
    </cms:CMSPanel>
</asp:Content>
