<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSWebParts_Membership_Profile_changepassword_Servranx"
    CodeFile="~/CMSWebParts/Membership/Profile/changepassword_Servranx.ascx.cs" %>
<%@ Register Src="~/Servranx/Controls/PasswordStrength.ascx" TagName="PasswordStrength"
    TagPrefix="cms" %>
<asp:Panel ID="pnlWebPart" runat="server" DefaultButton="btnOk">
    <asp:Label runat="server" ID="lblInfo" CssClass="InfoLabel" EnableViewState="false"
        Visible="false" />
    <asp:Label runat="server" ID="lblError" CssClass="ErrorLabel" EnableViewState="false"
        Visible="false" />
    <table class="ChangePasswordTable">
        <tr>
            <td class="FieldLabel">
                <asp:Label ID="lblOldPassword" AssociatedControlID="txtOldPassword" runat="server"
                    Visible="false" />
            </td>
            <td class="FieldInput">
                <cms:CMSTextBox ID="txtOldPassword" runat="server" TextMode="Password" CssClass="champtexte left clickClear" />
                <ajaxToolkit:TextBoxWatermarkExtender ID="wmOldPassword" runat="server" TargetControlID="txtOldPassword">
                </ajaxToolkit:TextBoxWatermarkExtender>
            </td>
        </tr>
        <tr>
            <td class="FieldLabel FieldLabelTop">
                <cms:LocalizedLabel ID="lblNewPassword" runat="server" Visible="false" />
            </td>
            <td class="FieldInput">
                <cms:PasswordStrength runat="server" ID="passStrength" AllowEmpty="true" TextBoxClass="champtexte left clickClear" IsPasswordUpdate="true" />
            </td>
        </tr>
        <tr>
            <td class="FieldLabel">
                <asp:Label ID="lblConfirmPassword" AssociatedControlID="txtConfirmPassword" runat="server" Visible="false" />
            </td>
            <td class="FieldInput">
                <cms:CMSTextBox ID="txtConfirmPassword" runat="server" TextMode="Password" CssClass="champtexte left clickClear" />
                <ajaxToolkit:TextBoxWatermarkExtender ID="wmConfirmPassword" runat="server" TargetControlID="txtConfirmPassword">
                </ajaxToolkit:TextBoxWatermarkExtender>
            </td>
        </tr>
        <tr>
            <td colspan="2" class="ChangeButton">
                <br />
                <cms:CMSButton ID="btnOk" runat="server" OnClick="btnOk_Click" CssClass="btn_update"
                    ValidationGroup="PasswordChange" />
            </td>
        </tr>
    </table>
</asp:Panel>
