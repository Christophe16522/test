<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_Ecommerce_Pages_Tools_DiscountCoupons_DiscountCoupon_Edit_General"
    Theme="Default" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Title="Discount coupon - Edit"
    CodeFile="DiscountCoupon_Edit_General.aspx.cs" %>

<%@ Register Src="~/CMSFormControls/System/LocalizableTextBox.ascx" TagName="LocalizableTextBox"
    TagPrefix="cms" %>
<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <table style="vertical-align: top">
        <tr>
            <td class="FieldLabel">
                <cms:LocalizedLabel runat="server" ID="lblDiscountCouponDisplayName" EnableViewState="false"
                    ResourceString="general.displayname" DisplayColon="true" />
            </td>
            <td>
                <cms:LocalizableTextBox ID="txtDiscountCouponDisplayName" runat="server" CssClass="TextBoxField"
                    MaxLength="200" EnableViewState="false" />
                <cms:CMSRequiredFieldValidator ID="rfvDisplayName" runat="server" Display="Dynamic"
                    ValidationGroup="Discount" ControlToValidate="txtDiscountCouponDisplayName:textbox"
                    EnableViewState="false" />
            </td>
        </tr>
        <tr>
            <td class="FieldLabel">
                <cms:LocalizedLabel runat="server" ID="lblDiscountCouponCode" EnableViewState="false"
                    ResourceString="DiscounCoupon_Edit.DiscountCouponCodeLabel" />
            </td>
            <td>
                <cms:CMSTextBox ID="txtDiscountCouponCode" runat="server" CssClass="TextBoxField"
                    MaxLength="200" EnableViewState="false" />
                <cms:CMSRequiredFieldValidator ID="rfvCouponCode" runat="server" Display="Dynamic"
                    ValidationGroup="Discount" ControlToValidate="txtDiscountCouponCode" EnableViewState="false" />
            </td>
        </tr>
        <tr>
            <td class="FieldLabel">
                <asp:Label runat="server" ID="Label1" EnableViewState="false" />
            </td>
            <td>
                <asp:RadioButton runat="server" ID="radFixed" GroupName="radValues" Checked="true" /><asp:RadioButton
                    runat="server" ID="radPercentage" GroupName="radValues" />
            </td>
        </tr>
        <tr>
            <td class="FieldLabel">
                <cms:LocalizedLabel runat="server" ID="lblDiscountCouponValue" EnableViewState="false"
                    ResourceString="DiscounCoupon_Edit.DiscountCouponAbsoluteValueLabel" />
            </td>
            <td>
                <cms:CMSTextBox ID="txtDiscountCouponValue" runat="server" CssClass="TextBoxField"
                    MaxLength="10" EnableViewState="false" />&nbsp;<asp:Label ID="lblCurrency" runat="server" />
            </td>
        </tr>
        <tr>
            <td class="FieldLabel">
                <cms:LocalizedLabel runat="server" ID="lblDiscountCouponValidFrom" EnableViewState="false"
                    ResourceString="DiscounCoupon_Edit.DiscountCouponValidFromLabel" />
            </td>
            <td>
                <cms:DateTimePicker ID="dtPickerDiscountCouponValidFrom" runat="server" EnableViewState="false" />
            </td>
        </tr>
        <tr>
            <td class="FieldLabel">
                <cms:LocalizedLabel runat="server" ID="lblDiscountCouponValidTo" EnableViewState="false"
                    ResourceString="DiscounCoupon_Edit.DiscountCouponValidToLabel" />
            </td>
            <td>
                <cms:DateTimePicker ID="dtPickerDiscountCouponValidTo" runat="server" EnableViewState="false" />
            </td>
        </tr>
        <tr>
            <td>
            </td>
            <td>
                <cms:FormSubmitButton runat="server" ID="btnOk" OnClick="btnOK_Click" EnableViewState="false" ValidationGroup="Discount" />
            </td>
        </tr>
    </table>
</asp:Content>
