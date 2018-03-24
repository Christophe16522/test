<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSWebParts_Membership_Registration_registrationformservranx"
    CodeFile="~/CMSWebParts/Membership/Registration/registrationformservranx.ascx.cs" %>
<%@ Register Src="~/CMSFormControls/Captcha/SecurityCode.ascx" TagName="SecurityCode"
    TagPrefix="cms" %>
<%@ Register Src="~/Servranx/Controls/PasswordStrength.ascx" TagName="PasswordStrength"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSFormControls/CountrySelector.ascx" TagName="CountrySelector"
    TagPrefix="cms" %>
<asp:Panel ID="pnlForm" runat="server" DefaultButton="btnOK">
    <asp:Label ID="lblError" runat="server" ForeColor="red" EnableViewState="false" />
    <asp:Label ID="lblText" runat="server" Visible="false" EnableViewState="false" />
    <div class="cont_client">
        <div class="cont_post left">
            <div class="cont_drop_down nb1 left civilite">
                <asp:DropDownList runat="server" ID="ddlFrom" CssClass="select_personaliser">
                </asp:DropDownList>
            </div>
                <%--  prénom--%>
         <asp:Label ID="lblFirstName" runat="server" AssociatedControlID="txtFirstName" EnableViewState="false"
                Visible="false" />
            <cms:ExtendedTextBox ID="txtFirstName" EnableEncoding="true" runat="server" CssClass="champtexte right threequarter clickClear"
                MaxLength="100" ToolTip="Prénom" />
            <ajaxToolkit:TextBoxWatermarkExtender ID="wmFirstName" runat="server" TargetControlID="txtFirstName"
                WatermarkText="Prénom" />
        </div>

            <%--  nom--%>
                    <asp:Label ID="lblLastName" runat="server" AssociatedControlID="txtLastName" Visible="false" />
        <cms:ExtendedTextBox ID="txtLastName" EnableEncoding="true" runat="server" CssClass="champtexte right"
            MaxLength="100" ToolTip="Nom" />
        <ajaxToolkit:TextBoxWatermarkExtender ID="wmLastName" runat="server" TargetControlID="txtLastName"
            WatermarkText="Nom" />
        <div class="clr">
        </div>
           
    
        <%-- <cms:ExtendedTextBox ID="txtnumero" EnableEncoding="true" runat="server" CssClass=" champtexte left clickClear"
            MaxLength="100" ToolTip="Numéro"/>
        <ajaxToolkit:TextBoxWatermarkExtender ID="wmnumero" runat="server" TargetControlID="txtnumero"
            WatermarkText="Numero" />--%>
        <%-- N°--%>
        <div class="cont_post left">
            <asp:TextBox runat="server" CssClass="champtexte left quarter" ID="txtnumero">
            </asp:TextBox>
            <ajaxToolkit:TextBoxWatermarkExtender ID="wmnumero" runat="server" TargetControlID="txtnumero"
                WatermarkText="Numero" />
            <ajaxToolkit:FilteredTextBoxExtender ID="txtnumero_FilteredTextBoxExtender" runat="server"
                Enabled="True" FilterType="Numbers" TargetControlID="txtnumero">
            </ajaxToolkit:FilteredTextBoxExtender>
            <%-- Adresse 1 --%>
            <cms:ExtendedTextBox ID="txtadresse1" EnableEncoding="true" runat="server" CssClass="champtexte right threequarter clickClear"
                MaxLength="100" ToolTip="Adresse 1" />
            <ajaxToolkit:TextBoxWatermarkExtender ID="wmadresse1" runat="server" TargetControlID="txtadresse1"
                WatermarkText="Adresse 1" />
        </div>
        <%-- Adresse 2 --%>
        <cms:ExtendedTextBox ID="txtadresse2" EnableEncoding="true" runat="server" CssClass="champtexte right clickClear"
            MaxLength="100" ToolTip="Adresse 2" />
        <ajaxToolkit:TextBoxWatermarkExtender ID="wmadresse2" runat="server" TargetControlID="txtadresse2"
            WatermarkText="Adresse 2" />
        <div class="clr">
        </div>
        <div class="cont_post left">
            <%-- cp--%>
            <cms:ExtendedTextBox ID="txtcp" EnableEncoding="true" runat="server" CssClass="champtexte left quarter clickClear"
                MaxLength="100" ToolTip="CP"  />
            <ajaxToolkit:TextBoxWatermarkExtender ID="wmcp" runat="server" TargetControlID="txtcp"
                WatermarkText="CP" />
            <%-- ville --%>
            <cms:ExtendedTextBox ID="txtville" EnableEncoding="true" runat="server" CssClass="champtexte right threequarter clickClear"
                MaxLength="100" ToolTip="Ville" />
            <ajaxToolkit:TextBoxWatermarkExtender ID="wmville" runat="server" TargetControlID="txtville"
                WatermarkText="Ville" />
        </div>
        <%-- pays--%>
        <div class="cont_drop_down nb1 right">
            <asp:DropDownList ID="ddlBillingCountry" runat="server" CssClass="select_personaliser"
                EnableViewState="true">
            </asp:DropDownList>
        </div>
        <div class="clr">
        </div>
        <%-- mail--%>
        <asp:Label ID="lblEmail" runat="server" AssociatedControlID="txtEmail" EnableViewState="false"
            Visible="false" />
        <cms:ExtendedTextBox ID="txtEmail" EnableEncoding="true" runat="server" CssClass="champtexte left"
            MaxLength="100" ToolTip="E-mail" />
        <ajaxToolkit:TextBoxWatermarkExtender ID="wmmail" runat="server" TargetControlID="txtEmail"
            WatermarkText="Email" />
        <%--Telephone--%>
        <asp:TextBox ID="txtTelephone" runat="server" CssClass="champtexte right" MaxLength="100"
            EnableViewState="false" ToolTip="Telephone" Visible="true" />
        <ajaxToolkit:TextBoxWatermarkExtender ID="wmTelephone" runat="server" TargetControlID="txtTelephone"
            WatermarkText="Telephone">
        </ajaxToolkit:TextBoxWatermarkExtender>
        <div class="clr">
        </div>
        <%-- password--%>
        <cms:LocalizedLabel ID="lblPassword" runat="server" EnableViewState="false" Visible="false" />
        <cms:PasswordStrength runat="server" ID="passStrength" ShowValidationOnNewLine="true"
            TextBoxClass="champtexte left clickClear" />
        <%-- confirm password--%>
        <asp:Label ID="lblConfirmPassword" runat="server" AssociatedControlID="txtConfirmPassword"
            EnableViewState="false" Visible="false" />
        <cms:CMSTextBox ID="txtConfirmPassword" runat="server" TextMode="Password" CssClass="champtexte right clickClear"
            MaxLength="100" ToolTip="Confirmer le mot de passe" />
        <div class="clr">
        </div>
        <%--  société--%>
        <div class="cont_box_different">
            <label>
                <asp:Label ID="lblSociete" runat="server" Text="Vous êtes une société" CssClass="cont_box_different"></asp:Label>
            </label>
            <label>
                &nbsp;&nbsp;&nbsp;&nbsp;</label>
            <label>
                <asp:RadioButton ID="rbnon" runat="server" Checked="true" OnCheckedChanged="rbnom_CheckedChanged"
                    AutoPostBack="false" GroupName="TVA" ClientIDMode="Static" EnableViewState="true" />
                <asp:Label ID="lblnon" runat="server" Visible="false" Text="NON" CssClass="cont_box_different"></asp:Label>
            </label>
            <label>
                &nbsp;&nbsp;&nbsp;&nbsp;</label>
            <label>
                <asp:RadioButton ID="rboui" runat="server" Checked="false" OnCheckedChanged="rboui_CheckedChanged"
                    AutoPostBack="false" GroupName="TVA" ClientIDMode="Static" EnableViewState="true" />
                <asp:Label ID="lbloui" runat="server" Visible="false" Text="OUI" CssClass="cont_box_different"></asp:Label>
            </label>
            <div class="clr">
            </div>
        </div>
        <div class="clr">
        </div>
        <div class="tvaBox" style="display: none;">
            <%-- nom société--%>
            <cms:ExtendedTextBox ID="txtnomsociete" EnableEncoding="true" runat="server" CssClass="champtexte left clickClear"
                MaxLength="100" ToolTip="Nom société" />
            <ajaxToolkit:TextBoxWatermarkExtender ID="wmnomsociete" runat="server" TargetControlID="txtnomsociete"
                WatermarkText="Nom société" />
            <%-- <cms:CMSRequiredFieldValidator ID="rfvnomsociete" runat="server" ControlToValidate="txtnomsociete"
            Display="Dynamic" EnableViewState="false" CssClass="left" />--%>
            <%--  tva--%>
            <%-- <cms:CMSRequiredFieldValidator ID="rfvtva" runat="server" ControlToValidate="txtTva"
            Display="Dynamic" EnableViewState="false" CssClass="right" />--%>
            <asp:UpdatePanel ID="test" runat="server">
            <ContentTemplate>
            <cms:ExtendedTextBox ID="txtTva" EnableEncoding="true" runat="server" CssClass=" champtexte right clickClear"   MaxLength="100" ToolTip="Tva" ontextchanged="txttva_TextChanged" AutoPostBack="true"/>
            <ajaxToolkit:TextBoxWatermarkExtender ID="wmtva" runat="server" TargetControlID="txtTva"
                WatermarkText="Tva" />
                <asp:Label ID="lblerror1" runat="server" ForeColor="red" EnableViewState="false" style="float:left"/>
                </ContentTemplate>
                </asp:UpdatePanel>
        </div>
        <div class="clr">
        </div>
        <%-- shipping--%>
        <div class="cont_box_different">
            <label>
                <asp:CheckBox ID="chkShippingAddr" runat="server" Checked="false" AutoPostBack="false"
                    OnCheckedChanged="chkShippingAddr_CheckedChanged" ClientIDMode="Static" />
                <asp:Label ID="LabelAdresse" runat="server" Text="Adresse de livraison différente de l’adresse de facturation"></asp:Label>
            </label>
            <div class="clr">
            </div>
        </div>
        <div class="divShipping">
            <asp:PlaceHolder ID="plhShipping" runat="server">
                <asp:Label ID="lblErrorShipping" runat="server" ForeColor="red" EnableViewState="false" />
                <div class="clr">
                </div>
                <div class="cont_post left">
                    <%-- N° shipping--%>
                    <asp:TextBox runat="server" CssClass="champtexte left quarter" ID="txtnumeroshipping">
                    </asp:TextBox>
                    <ajaxToolkit:TextBoxWatermarkExtender ID="wmnumeroshipping" runat="server" TargetControlID="txtnumeroshipping"
                        WatermarkText="Numero" />
                    <ajaxToolkit:FilteredTextBoxExtender ID="txtnumeroshipping_FilteredTextBoxExtender"
                        runat="server" Enabled="True" FilterType="Numbers" TargetControlID="txtnumeroshipping">
                    </ajaxToolkit:FilteredTextBoxExtender>
                    <cms:ExtendedTextBox ID="txtadresse1shipping" EnableEncoding="true" runat="server"
                        CssClass=" champtexte right threequarter clickClear" MaxLength="100" ToolTip="Adresse 1" />
                    <ajaxToolkit:TextBoxWatermarkExtender ID="wmadresse1shipping" runat="server" TargetControlID="txtadresse1shipping"
                        WatermarkText="Adresse 1" />
                </div>
                <%--<cms:ExtendedTextBox ID="txtnumeroshipping" EnableEncoding="true" runat="server" CssClass=" champtexte left clickClear"
            MaxLength="100" ToolTip="Numéro"/>
        <ajaxToolkit:TextBoxWatermarkExtender ID="wmnumeroshipping" runat="server" TargetControlID="txtnumeroshipping"
            WatermarkText="Numero" />--%>
                <%--<cms:CMSRequiredFieldValidator ID="rfvnumeroshipping" runat="server" ControlToValidate="txtnumeroshipping"
            Display="Dynamic" EnableViewState="false" CssClass="left" Text="*" />--%>
                <%-- Adresse 1 shipping --%>
                <%--<cms:CMSRequiredFieldValidator ID="rfvadresse1shipping" runat="server" ControlToValidate="txtadresse1shipping"
            Display="Dynamic" EnableViewState="false" CssClass="right" Text="*"/>--%>
                <%-- Adresse 2 shipping --%>
                <cms:ExtendedTextBox ID="txtadresse2shipping" EnableEncoding="true" runat="server"
                    CssClass=" champtexte right clickClear" MaxLength="100" ToolTip="Adresse 2" />
                <ajaxToolkit:TextBoxWatermarkExtender ID="wmadresse2shipping" runat="server" TargetControlID="txtadresse2shipping"
                    WatermarkText="Adresse 2" />
                <%-- <cms:CMSRequiredFieldValidator ID="rfvadresse2shipping" runat="server" ControlToValidate="txtadresse2shipping"
            Display="Dynamic" EnableViewState="false" CssClass="left" Text="*"/>     --%>
                <div class="clr">
                </div>
                <%--cp shipping>--%>
                <div class="cont_post left">
                    <cms:ExtendedTextBox ID="txtcpshipping" EnableEncoding="true" runat="server" CssClass=" champtexte left clickClear"
                        MaxLength="100" ToolTip="CP" />
                    <ajaxToolkit:TextBoxWatermarkExtender ID="wmcpshipping" runat="server" TargetControlID="txtcpshipping"
                        WatermarkText="Code Postal" />
                    <%--ville shipping--%>
                    <cms:ExtendedTextBox ID="txtvilleshipping" EnableEncoding="true" runat="server" CssClass=" champtexte right clickClear"
                        MaxLength="100" ToolTip="Ville" />
                    <ajaxToolkit:TextBoxWatermarkExtender ID="wmvilleshipping" runat="server" TargetControlID="txtvilleshipping"
                        WatermarkText="Code Postal" />
                </div>
                <%-- pays shipping --%>
                <div class="cont_drop_down nb1 right">
                    <asp:DropDownList ID="ddlShippingCountry" runat="server" CssClass="select_personaliser"
                        EnableViewState="true">
                    </asp:DropDownList>
                </div>
                <div class="clr">
                </div>
            </asp:PlaceHolder>
        </div>
        <div class="clr">
        </div>
        <div class="cont_post  left cont_check_condition">
            <label>
                <asp:CheckBox ID="chkAccept" runat="server" Checked="true" />
                <asp:Label ID="LabelAccept" runat="server" Text="Oui, j'accepte"></asp:Label>
                <br />
            </label>
            &nbsp;&nbsp;&nbsp; <a class="fancybox_condition_utilisation" href="#block_condition">
                <asp:Label ID="LabelCondition" runat="server" Text="les Conditions Générales de Vente"></asp:Label>
            </a>
            <div style="display: none; padding: 20px" id="block_condition">
                condition utilisation
            </div>
        </div>
        <div class="cont_post right cont_check_condition">
            <label>
                <asp:CheckBox ID="chkNewsletter" runat="server" Checked="false" />
                <asp:Label ID="LabelEmailInfo" runat="server" Text="Oui, je veux recevoir par e-mail des infos sur les"></asp:Label>
                <br />
                <asp:Label ID="LabelLastArticle" runat="server" Text="dérnières articles."></asp:Label>
            </label>
        </div>
        <div class="clr">
        </div>
        <cms:CMSButton ID="btnOk" runat="server" OnClick="btnOK_Click" CssClass="ContentButton btn_continued right"
            EnableViewState="false" />
    </div>
    <asp:PlaceHolder runat="server" ID="plcCaptcha">
        <asp:Label ID="lblCaptcha" runat="server" AssociatedControlID="scCaptcha" EnableViewState="false" />
        <cms:SecurityCode ID="scCaptcha" GenerateNumberEveryTime="false" runat="server" />
    </asp:PlaceHolder>
</asp:Panel>
<script type="text/javascript">
    jQuery(document).ready(function () {
        jQuery('input:radio').bind('change', function () {
            var showOrHide = (jQuery(this).val() == 'rboui') ? true : false;
            //jQuery('.tvaBox').toggle(showOrHide);
            jQuery('.tvaBox').slideToggle();
        });

        jQuery('#chkShippingAddr').bind('change', function () {
            jQuery('.divShipping').slideToggle();
        });
        var radio1 = jQuery('#rboui');
        if (radio1.is(':checked') == true) {
            jQuery('.tvaBox').show();
        };
        if (radio1.is(':checked') == false) {
            jQuery('.tvaBox').hide();
        };

        var check1 = jQuery('#chkShippingAddr');
        if (check1.is(':checked ') == true) {
            jQuery('.divShipping').show();
        };
        if (check1.is(':checked ') == false) {
            jQuery('.divShipping').hide();
        };

    });
</script>
