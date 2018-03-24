<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSWebParts_Membership_Registration_registrationform_1"
    CodeFile="~/CMSWebParts/Membership/Registration/registrationform_1.ascx.cs" %>
<%@ Register Src="~/CMSFormControls/Captcha/SecurityCode.ascx" TagName="SecurityCode"
    TagPrefix="cms" %>
<%@ Register Src="~/Servranx/Controls/Password.ascx" TagName="PasswordStrength" TagPrefix="cms" %>
<asp:Panel ID="pnlForm" runat="server" DefaultButton="btnOK">
    <asp:Label ID="lblError" runat="server" ForeColor="red" EnableViewState="false" />
    <asp:Label ID="lblText" runat="server" Visible="false" EnableViewState="false" />
    <div class="cont_client">
        <div class="cont_post left">
            <%--  civilité--%>
            <div class="cont_drop_down nb1 left civilite">
                <asp:DropDownList runat="server" ID="payement_option" CssClass="select_personaliser1">
                </asp:DropDownList>
            </div>
            <%--  prénom--%>
            <cms:ExtendedTextBox ID="txtFirstName" EnableEncoding="true" runat="server" CssClass="champtexte right threequarter"
                MaxLength="100" ToolTip="Prénom" />
            <ajaxToolkit:TextBoxWatermarkExtender ID="wmFirstName" runat="server" TargetControlID="txtFirstName"
                WatermarkText="Prénom" />
        </div>
        <%--   nom--%>
        <cms:ExtendedTextBox ID="txtLastName" EnableEncoding="true" runat="server" CssClass="champtexte right"
            MaxLength="100" ToolTip="Nom" />
        <ajaxToolkit:TextBoxWatermarkExtender ID="wmLastName" runat="server" TargetControlID="txtLastName"
            WatermarkText="Nom" />
        <div class="clr">
        </div>
        <%--  email--%>
        <cms:ExtendedTextBox ID="txtEmail" runat="server" CssClass="champtexte left" MaxLength="100" />
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
        <div class="BlocDroitConso">
            <asp:HyperLink runat="server" ID="lnkPassword" NavigateUrl="~/Special-Page/password.aspx"></asp:HyperLink>
        </div>
        <%--  Mot de passe--%>
        <asp:Panel ID="pnlPassword" runat="server" Visible="false">
            <cms:PasswordStrength runat="server" ID="txtPassword" ShowValidationOnNewLine="true"
                TextBoxClass="champtexte left clickClear1" />
            <%--  Confirmer Mot de passe--%>
            <cms:CMSTextBox ID="txtConfirmPassword" TextMode="Password" runat="server" CssClass="champtexte right clickClear1"
                MaxLength="100" ToolTip="Confirmer Mot de passe" />
            <div class="clr">
            </div>
        </asp:Panel>
        <%--  société--%>
        <div class="cont_box_different">
            <label>
                <asp:Label ID="lblSociete" runat="server" Text="Vous êtes une société" CssClass="cont_box_different"></asp:Label>
            </label>
            <label>
                &nbsp;&nbsp;&nbsp;&nbsp;</label>
            <label>
                <asp:RadioButton ID="rbnon" runat="server" Checked="true" AutoPostBack="false" GroupName="TVA"
                    ClientIDMode="Static" EnableViewState="true" />
                <asp:Label ID="lblnon" runat="server" Text="NON" Visible="false" CssClass="cont_box_different"></asp:Label>
            </label>
            <label>
                &nbsp;&nbsp;&nbsp;&nbsp;</label>
            <label>
                <asp:RadioButton ID="rboui" runat="server" Checked="false" AutoPostBack="false" GroupName="TVA"
                    ClientIDMode="Static" EnableViewState="true" />
                <asp:Label ID="lbloui" runat="server" Text="OUI" Visible="false" CssClass="cont_box_different"></asp:Label>
            </label>
            <div class="clr">
            </div>
        </div>
        <div class="clr">
        </div>
        <div class="tvaBox" style="display: none;">
            <%-- nom société--%>
            <cms:ExtendedTextBox ID="txtnomsociete" EnableEncoding="true" runat="server" CssClass="champtexte left"
                MaxLength="100" ToolTip="Nom société" />
            <ajaxToolkit:TextBoxWatermarkExtender ID="wmnomsociete" runat="server" TargetControlID="txtnomsociete"
                WatermarkText="Nom société" />
            <%-- Tva --%>
            <asp:UpdatePanel ID="test" runat="server">
            <ContentTemplate>
            <asp:TextBox runat="server" CssClass="champtexte right" ID="txtTva" ontextchanged="txttva_TextChanged" AutoPostBack="true">
            </asp:TextBox>
            <ajaxToolkit:TextBoxWatermarkExtender ID="wmTva" runat="server" TargetControlID="txtTva"
                WatermarkText="Tva" />
                <asp:Label ID="lblerror1" runat="server" ForeColor="red" EnableViewState="false" style="float:left" />
                </ContentTemplate>
                </asp:UpdatePanel>

        </div>
        <div class="clr">
        </div>
        <div class="TitreAdresse">
            <h1>
                <asp:Label ID ="lblAdresses" runat ="server"></asp:Label> </h1>
        </div>
        <div class="clr">
        </div>
        <div class="cont_box_different">
            <div id="modif_adress" class="">
                <asp:Repeater runat="server" ID="rptAdress" OnItemDataBound="rptAdressItemDataBound"
                    OnItemCommand="rptAdressItemCommand" EnableViewState="true">
                    <ItemTemplate>
                        <ul class="adress_list">
                            <li><span class="adress_name">
                                <asp:Literal ID="ltlAdress" runat="server"></asp:Literal>
                            </span><span class="option"><a class="editer" data="<%# Container.ItemIndex + 1 %>">
                            </a>
                                <asp:LinkButton ID="LinkButtonUpdate" runat="server" CommandName="Update" CommandArgument='<%# Eval("AddressID") %>'
                                    CssClass="editer" ToolTip="Editer l'adresse" EnableViewState="false" Visible="false">                                   
                                </asp:LinkButton>
                                <asp:LinkButton ID="LinkButtonDelete" runat="server" CommandName="Remove" CommandArgument='<%# Eval("AddressID") %>'
                                    CssClass="supprimer" ToolTip="Supprimer l'adresse" EnableViewState="false">                                
                                </asp:LinkButton>
                                <ajaxToolkit:ConfirmButtonExtender ID="ConfirmButtonDelete" TargetControlID="LinkButtonDelete"
                                    ConfirmText="Voulez-vous vraiment supprimer cette adresse?" runat="server" />
                            </span>
                                <div class="clr">
                                </div>
                                <div data="<%# Container.ItemIndex + 1 %>" class="slidediv">
                                    <asp:Panel ID="PnlInsertAdress" runat="server">
                                        <asp:Label ID="lblErrorAdress" runat="server" ForeColor="red" EnableViewState="false"
                                            CssClass="lblErrorAdress" />
                                        <div class="clr">
                                        </div>
                                        <cms:ExtendedTextBox ID="txtIdAdresse" EnableEncoding="true" runat="server" MaxLength="100"
                                            Visible="false" />
                                        <div class="cont_post left">
                                            <%-- N°--%>
                                            <asp:TextBox runat="server" CssClass="champtexte left quarter" ID="txtnumero">
                                            </asp:TextBox>
                                            <ajaxToolkit:TextBoxWatermarkExtender ID="wmnumero" runat="server" TargetControlID="txtnumero"
                                                WatermarkText="Numero" />
                                            <ajaxToolkit:FilteredTextBoxExtender ID="txtnumero_FilteredTextBoxExtender" runat="server"
                                                Enabled="True" FilterType="Numbers" TargetControlID="txtnumero">
                                            </ajaxToolkit:FilteredTextBoxExtender>
                                            <%-- Adresse 1 --%>
                                            <asp:TextBox runat="server" CssClass="champtexte right threequarter" ID="txtadresse1">
                                            </asp:TextBox>
                                            <ajaxToolkit:TextBoxWatermarkExtender ID="wmadresse1" runat="server" TargetControlID="txtadresse1"
                                                WatermarkText="Adresse 1" />
                                        </div>
                                        <%-- Adresse 2 --%>
                                        <asp:TextBox runat="server" CssClass="champtexte right" ID="txtadresse2">
                                        </asp:TextBox>
                                        <ajaxToolkit:TextBoxWatermarkExtender ID="wmadresse2" runat="server" TargetControlID="txtadresse2"
                                            WatermarkText="Adresse 2" />
                                        <div class="clr">
                                        </div>
                                        <div class="cont_post left">
                                            <%-- CP --%>
                                            <cms:ExtendedTextBox ID="txtcp" EnableEncoding="true" runat="server" CssClass="champtexte left quarter"
                                                MaxLength="100" ToolTip="Code Postal" />
                                            <ajaxToolkit:TextBoxWatermarkExtender ID="wmcp" runat="server" TargetControlID="txtcp"
                                                WatermarkText="Code Postal" />
                                            <%-- Ville --%>
                                            <cms:ExtendedTextBox ID="txtville" EnableEncoding="true" runat="server" CssClass="champtexte right"
                                                MaxLength="100" ToolTip="Ville" />
                                            <ajaxToolkit:TextBoxWatermarkExtender ID="wmville" runat="server" TargetControlID="txtville"
                                                WatermarkText="Ville" />
                                        </div>
                                        <%-- pays--%>
                                        <asp:TextBox runat="server" CssClass="champtexte right" ID="txtCountry" Enabled="false">
                                        </asp:TextBox>
                                        <div class="clr">
                                        </div>
                                        <label>
                                            <asp:CheckBox ID="chkShippingAddr" runat="server" Checked="false" />
                                            <asp:Label ID="Lblshipping" runat="server" Text="Adresse de livraison"></asp:Label>
                                        </label>
                                        <label>
                                            &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</label>
                                        <label>
                                            <asp:CheckBox ID="chkBillingAddr" runat="server" Checked="false" />
                                            <asp:Label ID="Lblbilling" runat="server" Text="Adresse de facturation"></asp:Label>
                                        </label>
                                        <asp:LinkButton ID="buttonUpdate" runat="server" CssClass="btn_update right" CommandName="Update"
                                            CommandArgument='<%# Eval("AddressID") %>'></asp:LinkButton>
                                        <div class="clr">
                                        </div>
                                    </asp:Panel>
                                </div>
                            </li>
                        </ul>
                    </ItemTemplate>
                </asp:Repeater>
                 <asp:Label ID="Label1" runat="server" ForeColor="red" EnableViewState="false"  Visible="false" />
                <a class="btn_new_address">
               
                   <%-- <asp:CheckBox ID="chkNewAddress" runat="server" ClientIDMode="Static" EnableViewState="true"
                        Style="position: absolute; left: -20px; z-index: 2; opacity: 0; filter: alpha(opacity=0);" />--%>
                </a>
                <%-- nouvelle adresse--%>
                <div class="new_address">
                    <br />
                    <div class="clr">
                    </div>
                    <br />
                    <asp:Label ID="lblErrorAdress" runat="server" ForeColor="red" EnableViewState="false" />
                    <div class="clr">
                    </div>
                    <cms:ExtendedTextBox ID="txtIdAdresse" EnableEncoding="true" runat="server" MaxLength="100"
                        Visible="false" />
                    <div class="cont_post left">
                        <%-- N°--%>
                        <asp:TextBox runat="server" CssClass="champtexte left quarter" ID="txtnumero">
                        </asp:TextBox>
                        <ajaxToolkit:TextBoxWatermarkExtender ID="wmnumero" runat="server" TargetControlID="txtnumero"
                            WatermarkText="Numero" />
                        <ajaxToolkit:FilteredTextBoxExtender ID="txtnumero_FilteredTextBoxExtender" runat="server"
                            Enabled="True" FilterType="Numbers" TargetControlID="txtnumero">
                        </ajaxToolkit:FilteredTextBoxExtender>
                        <%-- Adresse 1 --%>
                        <asp:TextBox runat="server" CssClass="champtexte right threequarter" ID="txtadresse1">
                        </asp:TextBox>
                        <ajaxToolkit:TextBoxWatermarkExtender ID="wmadresse1" runat="server" TargetControlID="txtadresse1"
                            WatermarkText="Adresse 1" />
                    </div>
                    <%-- Adresse 2 --%>
                    <asp:TextBox runat="server" CssClass="champtexte right" ID="txtadresse2">
                    </asp:TextBox>
                    <ajaxToolkit:TextBoxWatermarkExtender ID="wmadresse2" runat="server" TargetControlID="txtadresse2"
                        WatermarkText="Adresse 2" />
                    <div class="clr">
                    </div>
                    <div class="cont_post left">
                        <%-- CP --%>
                        <cms:ExtendedTextBox ID="txtcp" EnableEncoding="true" runat="server" CssClass="champtexte left"
                            MaxLength="100" ToolTip="Code Postal" />
                        <ajaxToolkit:TextBoxWatermarkExtender ID="wmcp" runat="server" TargetControlID="txtcp"
                            WatermarkText="Code Postal" />
                        <%-- Ville --%>
                        <cms:ExtendedTextBox ID="txtville" EnableEncoding="true" runat="server" CssClass="champtexte right"
                            MaxLength="100" ToolTip="Ville" />
                        <ajaxToolkit:TextBoxWatermarkExtender ID="wmville" runat="server" TargetControlID="txtville"
                            WatermarkText="Ville" />
                    </div>
                    <%-- pays--%>
                    <div class="cont_drop_down nb1 right">
                        <asp:DropDownList ID="ddlShippingCountry" runat="server" CssClass="select_personaliser1"
                            EnableViewState="true">
                        </asp:DropDownList>
                    </div>
                    <div class="clr">
                    </div>
                    <label>
                        <asp:CheckBox ID="chkShippingAddr" runat="server" Checked="false" />
                        <asp:Label ID="Lblshipping" runat="server" Text="Adresse de livraison"></asp:Label>
                    </label>
                    <label>
                        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</label>
                    <label>
                        <asp:CheckBox ID="chkBillingAddr" runat="server" Checked="false" />
                        <asp:Label ID="Lblbilling" runat="server" Text="Adresse de facturation"></asp:Label>
                    </label>
                    <div class="clr">
                    </div>
                </div>
                <cms:CMSButton ID="btnOk" runat="server" OnClick="btnOK_Click" CssClass="valider_info right"
                    EnableViewState="false" />
            </div>
        </div>
        <asp:PlaceHolder runat="server" ID="plcCaptcha">
            <asp:Label ID="lblCaptcha" runat="server" AssociatedControlID="scCaptcha" EnableViewState="false" />
            <cms:SecurityCode ID="scCaptcha" GenerateNumberEveryTime="false" runat="server" />
        </asp:PlaceHolder>
    </div>
</asp:Panel>
<asp:HiddenField ID="hidPanelVisible" runat="server" ClientIDMode="Static" EnableViewState="true" />
<script type="text/javascript">
    jQuery(document).ready(function () {
        //jQuery('.btn_new_address').show();
        jQuery('.new_address').hide();
        jQuery('input:radio').bind('change', function () {
            var showOrHide = (jQuery(this).val() == 'rboui') ? true : false;
            jQuery('.tvaBox').slideToggle();
        });

        jQuery('.btn_new_address').click(function () {
            jQuery('.slidediv').hide();
            jQuery('.new_address').slideToggle(1);
            jQuery('.new_address').show();
            //jQuery('#chkNewAddress').click();
            /*var panel = jQuery('.new_address');
            if (panel.is(':visible') == true) {
            alert('visible');
            }
            else {
            alert('hidden');
            }*/

        });

        jQuery('.slidediv').hide();
        jQuery('.editer').click(function () {
            var index = jQuery(this).attr('data');
            jQuery('.new_address').hide();
            jQuery(".slidediv").each(function () {
                if (jQuery(this).attr('data') === index)
                    jQuery(this).slideToggle();
                else
                    jQuery(this).hide();
            });
        });

        var error1 = jQuery('.lblErrorAdress');
        if (error1 != null) {
            if (error1.is(':visible') == true) {
                (error1.parent()).parent().show();
            }
        }

        var radio1 = jQuery('#rboui');
        if (radio1.is(':checked') == true) {
            jQuery('.tvaBox').show();
        };
        if (radio1.is(':checked') == false) {
            jQuery('.tvaBox').hide();
        };

//        var check1 = jQuery('#chkNewAddress');
//        if (check1.is(':checked') == true) {
//            //jQuery('.new_address').show();
//        };
//        if (check1.is(':checked ') == false) {
//            //jQuery('.new_address').hide();
//        };

    });
</script>
