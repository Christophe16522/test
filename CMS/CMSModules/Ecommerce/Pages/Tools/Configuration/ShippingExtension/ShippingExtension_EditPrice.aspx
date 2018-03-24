<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_Ecommerce_Pages_Tools_Configuration_ShippingExtension_ShippingExtension_EditPrice"
    Theme="Default" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" CodeFile="ShippingExtension_EditPrice.aspx.cs" %>

<%@ Register Src="~/CMSFormControls/System/LocalizableTextBox.ascx" TagName="LocalizableTextBox"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/Ecommerce/Controls/UI/PriceSelector.ascx" TagName="PriceSelector"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/AdminControls/Controls/MetaFiles/File.ascx" TagPrefix="cms"
    TagName="File" %>
<%@ Register Src="~/CMSFormControls/System/CodeName.ascx" TagName="CodeName" TagPrefix="cms" %>
<asp:Content ID="Content1" ContentPlaceHolderID="plcContent" runat="Server">
    <table style="vertical-align: top">
        <tr>
            <td class="FieldLabel">
                <cms:LocalizedLabel runat="server" ID="lblShippingUnitFrom" EnableViewState="false"
                    Text="Shipping Unit From:" DisplayColon="true" />
            </td>
            <td>
                <asp:TextBox ID="txtShippingUnitFrom" runat="server" TextMode="SingleLine"
                    EnableViewState="false" Enabled ="false" />
            </td>
        </tr>
        <tr>
            <td class="FieldLabel">
                <cms:LocalizedLabel runat="server" ID="lblShippingUnitTo" EnableViewState="false"
                    Text="Shipping Unit To:" DisplayColon="true" />
            </td>
            <td>
                <asp:TextBox ID="txtShippingUnitTo" runat="server" TextMode="SingleLine"
                    EnableViewState="false" Enabled ="false" />
            </td>
        </tr>
        <tr>
            <td class="FieldLabel">
                <cms:LocalizedLabel runat="server" ID="lblPrice" EnableViewState="false" Text="Price:"
                    DisplayColon="true" />
            </td>
            <td>
                <asp:TextBox ID="txtPrice" runat="server" TextMode="SingleLine"
                    EnableViewState="false" />
            </td>
        </tr>
        <tr>
            <td>
            </td>
            <td>
                <cms:FormSubmitButton runat="server" ID="btnOk" OnClick="btnOK_Click" EnableViewState="false" />
            </td>
        </tr>
    </table>
</asp:Content>
