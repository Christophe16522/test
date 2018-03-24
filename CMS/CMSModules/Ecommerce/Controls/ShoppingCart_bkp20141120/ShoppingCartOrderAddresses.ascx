<%@ Control Language="C#" AutoEventWireup="true" Debug="true" Inherits="CMSModules_Ecommerce_Controls_ShoppingCart_ShoppingCartOrderAddresses"
    CodeFile="ShoppingCartOrderAddresses.ascx.cs" %>
<%@ Register Src="~/CMSFormControls/CountrySelector.ascx" TagName="CountrySelector"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/Ecommerce/FormControls/CurrencySelector.ascx" TagName="CurrencySelector"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/ECommerce/FormControls/PaymentSelector.ascx" TagName="PaymentSelector"
    TagPrefix="cms" %>
<asp:Label ID="lblBillingTitle" runat="server" CssClass="BlockTitle" EnableViewState="false" />
<div class="BlockContent">
    <asp:Label ID="lblError" runat="server" Visible="false" CssClass="ErrorLabel" EnableViewState="false" />
    <div class="cont_payemet">
        <div class="left_block_payement etape_2">
            <div class="pad_40">
            </div>
            <div class="block_gris_payement first">
                <div class="cont_block_gris first">
                    <h4>
                        <asp:Literal ID="LiteralAdresseDeFacturation" runat="server" Text="adresse de facturation"></asp:Literal>
                    </h4>
                    <a href="#modif_adress" class="right fancybox_list_adress">
                        <asp:Literal ID="LiteralModifier" runat="server" Text="modifier"></asp:Literal></a>
                    <div class="clr">
                    </div>
                    <div class="facturation_info">
                        <p>
                            <asp:Label ID="lblBillingAddressFullName" runat="server"></asp:Label>
                        </p>
                        <p>
                            <asp:Label ID="lblBillingAddressStreet" runat="server"></asp:Label>
                        </p>
                        <p>
                            <asp:Label ID="lblBillingAddressZipCode" runat="server"></asp:Label>
                        </p>
                        <p>
                            <asp:Label ID="lblBillingAddressCityCountry" runat="server"></asp:Label>
                        </p>
                    </div>
                </div>
                <div class="separateur">
                </div>
                <div class="cont_block_gris second">
                    <h6 class="left">
                        <asp:Literal ID="ltlAdresseLivraison" runat="server" Text="adresse de livraison"></asp:Literal>
                    </h6>
                    <a href="#modif_facturation" class="right fancybox_adress_facturation">
                        <asp:Literal ID="ltlModifier" runat="server" Text="modifier"></asp:Literal></a>
                    <div class="clr">
                    </div>
                    <div class="facturation_info">
                        <p>
                            <asp:Label ID="lblShippingAddressFullName" runat="server"></asp:Label>
                        </p>
                        <p>
                            <asp:Label ID="lblShippingAddressStreet" runat="server"></asp:Label>
                        </p>
                        <p>
                            <asp:Label ID="lblShippingAddressZipCode" runat="server"></asp:Label>
                        </p>
                        <p>
                            <asp:Label ID="lblShippingAddressCityCountry" runat="server"></asp:Label>
                        </p>
                    </div>
                </div>
            </div>
			
		<% if (IsShippingNeeded) { %>
            <div class="block_gris_payement second" style="display: block">
                <div class="cont_block_gris first">
                    <h4 class="left">
                        <asp:Literal ID="LiteralOptionEnvoi" runat="server" Text="Option d'envoi"></asp:Literal>
                    </h4>
                    <em class="left"></em><span class="tit_moyen left">
                        <asp:Literal ID="Literal1" runat="server" Visible="true"></asp:Literal></span>
                    <div class="clr">
                    </div>
                    <div class="clr">
                    </div>
                    <div class="pad_10">
                    </div>
                    <div class="cont_drop_down left_drop nb1">
                        <cms:LocalizedDropDownList ID="ddlShippingOption" runat="server" CssClass="select_personaliser"
                            OnSelectedIndexChanged="ddlShippingOption_SelectedIndexChanged" AutoPostBack="true">
                        </cms:LocalizedDropDownList>
                        <%--<asp:UpdatePanel ID="updatePanelPaymentOption" runat="server">
                            <ContentTemplate>
                            </ContentTemplate>
                        </asp:UpdatePanel> --%>
                    </div>
                </div>
            </div>
		 <% } %>	
            <div class="block_gris_payement second">
                <div class="cont_block_gris first">
                    <h4 class="left">
                        <asp:Literal ID="LiteralMoyenDePaiement" runat="server" Text="moyen de paiement"></asp:Literal>
                    </h4>
                    <em class="left">/</em><span class="tit_moyen left"><asp:Literal ID="lblPayment"
                        runat="server" Visible="true"></asp:Literal></span>
                    <div class="clr">
                    </div>
                    <a href="#"></a>
                  <%--  <form action="" method="post">--%>
                    <div class="clr">
                    </div>
                    <div class="pad_10">
                    </div>
                    <div class="cont_drop_down left_drop nb1">
                        <cms:LocalizedDropDownList ID="ddlPaymentOption" runat="server" CssClass="select_personaliser"
                            OnSelectedIndexChanged="ddlPaymentOption_SelectedIndexChanged" AutoPostBack="true">
                        </cms:LocalizedDropDownList>
                        <%--<asp:UpdatePanel ID="updatePanelPaymentOption" runat="server">
                            <ContentTemplate>
                            </ContentTemplate>
                        </asp:UpdatePanel> --%>
                    </div>
                   <%-- </form>--%>
                </div>
                <asp:Panel ID="pnlPaymentOption" runat="server" Visible="false">
                    <div class="separateur">
                    </div>
                    <div class="cont_block_gris second">
                        <h4 class="left">
                            <asp:Literal ID="LiteralCartePaiement" runat="server" Text="carte de paiement"></asp:Literal>
                        </h4>
                        <em class="left">/</em><span class="tit_moyen left"><asp:Literal ID="lblCardName"
                            runat="server"></asp:Literal></span>
                        <div class="clr">
                        </div>
                        <!--a href="#">modifier</a-->
                        <ul>
                            <li>
                                <asp:RadioButton runat="server" GroupName="PaymentMethod" AutoPostBack="true" OnCheckedChanged="rdoBtn_CheckedChanged"
                                    ID="rdbVisa" />
                                <img src="~/App_Themes/Servranx/Images/visa.jpg" alt="visa" /><label for="Visa">Visa</label>
                            </li>
                            <li>
                                <asp:RadioButton runat="server" GroupName="PaymentMethod" AutoPostBack="true" OnCheckedChanged="rdoBtn_CheckedChanged"
                                    ID="rdbMastercard" />
                                <img src="~/App_Themes/Servranx/Images/mastercard.jpg" alt="mastercard" /><label
                                    for="Mastercard">Mastercard</label>
                            </li>
                            <li>
                                <asp:RadioButton runat="server" GroupName="PaymentMethod" AutoPostBack="true" OnCheckedChanged="rdoBtn_CheckedChanged"
                                    ID="rdbMaestro" />
                                <img src="~/App_Themes/Servranx/Images/maestro.jpg" alt="maestro" /><label for="Maestro">Maestro</label>
                            </li>
                        </ul>
                    </div>
                </asp:Panel>
            </div>
        </div>
        <div class="right_block_payement etape_3">
            <asp:Panel ID="pnlCurrency" runat="server" Visible="False">
                <asp:Label ID="lblCurrency" runat="server" EnableViewState="false" AssociatedControlID="selectCurrency" />
                <cms:CurrencySelector ID="selectCurrency" runat="server" DisplayOnlyWithExchangeRate="true"
                    DoFullPostback="true" RenderInline="true" />
            </asp:Panel>
            <asp:Panel ID="pnlCartLeftInnerContent" runat="server" Height="373px">
                <table class="lst_produit">
                    <tr>
                        <th class="article">
                            <asp:Label ID="LabelArticle" runat="server" Text="Article"></asp:Label>
                        </th>
                        <th class="qte">
                            <asp:Label ID="LabelQuantite" runat="server" Text="Quantité"></asp:Label>
                        </th>
                        <th class="suppr">
                            &nbsp;
                        </th>
                        <th class="prix">
                            <asp:Label ID="LabelPrixTotal" runat="server" Text="Prix Total"></asp:Label>
                        </th>
                    </tr>
                    <asp:Repeater ID="rptCart" runat="server" EnableViewState="true" OnItemDataBound="RptCartItemDataBound"
                        OnItemCommand="RptCartItemCommand">
                        <ItemTemplate>
                            <tr>
                                <td class="first">
                                    <!--div class="produit_horizonal"-->
                                    <%--<img src="~/App_Themes/Servranx/images/img_produit_panier_horiz.jpg" width="63" height="47" alt="Image produit horizontal" />--%>
                                    <asp:Literal ID="ltlProductImage" runat="server"></asp:Literal>
                                    <!--/div-->
                                    <span class="name_produit"><a href="#">
                                        <asp:Literal ID="ltlProductName" runat="server"></asp:Literal></a></span>
                                        <asp:TextBox ID="iditem" runat="server" Visible="false" Text='<%# Eval("SKUID") %>'></asp:TextBox>
                                </td>
                                <td>
                                    <span class="cont_line">
                                        <asp:LinkButton ID="LinkButtonDecrease" CssClass="decremente" runat="server" CommandName="Decrease"
                                            CommandArgument='<%# Eval("CartItemGuid") %>'></asp:LinkButton>
                                        <asp:TextBox ID="txtProductCount" runat="server" CssClass="nombre_produit" ReadOnly="true"></asp:TextBox>
                                        <asp:LinkButton ID="LinkButtonIncrease" CssClass="incremente" runat="server" CommandName="Increase"
                                            CommandArgument='<%# Eval("CartItemGuid") %>'></asp:LinkButton>
                                    </span>
                                </td>
                                <td>
                                    <span class="cont_line suppr">
                                        <asp:LinkButton ID="LinkButtonDelete" Text="Supprimer le produit de mon panier" runat="server"
                                            CommandName="Remove" CommandArgument='<%# Eval("CartItemGuid") %>'>
                                    <img src="~/App_Themes/Servranx/images/supprimer.jpg" width="24" height="27" alt="Supprimer" />
                                        </asp:LinkButton>
                                    </span>&nbsp;
                                </td>
                                <td class="last">
                                    <span class="txt_prix">
                                        <asp:Literal ID="ltlProductPrice" runat="server"></asp:Literal></span>
                                </td>
                            </tr>
                            <tr>
                                <td colspan="4" class="white_space">
                                </td>
                            </tr>
                        </ItemTemplate>
                    </asp:Repeater>
                </table>
                <ul class="block_total">
                   <!-- total sans coupon-->
                     <li id="sanscoupon2" runat ="server" visible ="false"><span class="txt_ttc">
                        <asp:Literal ID="lblMontantAchatSanscoupon" runat="server" EnableViewState="false" />
                        <%--<em>€</em>--%></span><strong class="txt_orange">
                            <asp:Literal ID="LabelMontantDesAchatsSanscoupon" runat="server" Text="montant des achats sans coupon"></asp:Literal></strong>
                        <div class="clr">
                        </div>
                    </li>
                      <!-- total coupon-->
                    <li id="totalcoupon2" runat="server" visible ="false"><span class="txt_ttc">
                        <asp:Literal ID="lblMontantCoupon" runat="server" EnableViewState="false" />
                        <%--<em>€</em>--%></span><strong class="txt_orange">
                            <asp:Literal ID="LabelMontantCoupon" runat="server" Text="montant total du coupon"></asp:Literal></strong>
                        <div class="clr">
                        </div>
                    </li>
                     <!-- total avec coupon-->
                    <li><span>
                        <asp:Literal ID="lblMontantAchat" runat="server" EnableViewState="false" />
                        <%--<em>€</em>--%></span><strong><asp:Literal ID="LabelSousTotal" runat="server"
                            Text="sous total"></asp:Literal></strong>
                        <div class="clr">
                        </div>
                    </li>
                    <!-- Frais -->
				<% if (IsShippingNeeded) { %>
                    <li><span>
                        <asp:Literal ID="lblShippingPriceValue" runat="server" EnableViewState="false" />
                    </span><strong>
                        <asp:Literal ID="LabelFraisEnvoi" runat="server" Text="frais d'envoi"></asp:Literal></strong>
                        <div class="clr">
                        </div>
                    </li>
				<% } %>
                    <!-- Prix total avec frais-->
                    <li><span class="txt_ttc">
                        <asp:Literal ID="lblTotalPriceValue" runat="server" EnableViewState="false"></asp:Literal></span><strong
                            class="txt_orange"><asp:Literal ID="LabelMontantTotal" runat="server" Text="Montant total"></asp:Literal></strong>
                        <div class="clr">
                        </div>
                    </li>
                </ul>
            </asp:Panel>
            <div class="clr"></div>
            <asp:Panel ID="lnkBundle" runat="server">
                <p class="InfoCadeau">
                    <b>*</b>
                    <asp:Literal runat="server" ID="lblBundle"></asp:Literal>
                    <br />
                    <span> 
                        <asp:Literal ID="lblBundleBody" runat ="server"></asp:Literal>
                    </span>
                </p>
            </asp:Panel>
            <asp:Panel ID="pnlCartBundleContent" CssClass="BlcXtGratuit" runat="server" Visible="false">
                <h3>
                    <asp:Label ID="lblBundleHeader" runat="server"></asp:Label></h3>
                <ul>
                    <asp:Repeater runat="server" ID="rptBundledProducts" OnItemDataBound="RptBundleProductsItemDataBound"
                        OnItemCommand="RptBundleProductsItemCommand">
                        <ItemTemplate>
                            <li>
                                <asp:Label runat="server" ID="litProduct"></asp:Label>
                                <asp:LinkButton ID="buttonShippingEdit" runat="server" CssClass="" CommandName="Delete"
                                    CommandArgument='<%# Eval("ProductID") %>'><img width="24" height="27" alt="Supprimer" src="~/App_Themes/Servranx/images/supprimer.jpg"></asp:LinkButton>
                                <div class="clr">
                                </div>
                            </li>
                        </ItemTemplate>
                    </asp:Repeater>
                </ul>
                <asp:Label ID="lblErrorBundle" runat="server" ForeColor="Red" Visible="false" EnableViewState="false"></asp:Label>
            </asp:Panel>
        </div>
        <div class="clr">
        </div>
        <div id="modif_adress" class="block_popup_modif_adress">
            <%--<a class="btn_new_addressfacture"></a>--%>
          <%--  <a class="btn_new_address"></a>--%>
         <a class='<%= ResHelper.LocalizeString("{$btn.adressefacture$}")%>'></a>
            <div class="new_address">
                <asp:Label ID="lblErrorBillingAdress" runat="server" ForeColor="red" EnableViewState="false" />
                <div class="clr">
                </div>
                <div class="cont_post left">
                    <cms:ExtendedTextBox ID="txtBillingnumero" EnableEncoding="true" runat="server" CssClass="champtexte left quarter" MaxLength="5" />
                    <ajaxToolkit:FilteredTextBoxExtender ID="filtertxtBillingnumero" runat="server" FilterType ="Numbers" TargetControlID = "txtBillingnumero"></ajaxToolkit:FilteredTextBoxExtender>
                    <ajaxToolkit:TextBoxWatermarkExtender ID="wmBillingNumero" runat="server" TargetControlID="txtBillingnumero">
                    </ajaxToolkit:TextBoxWatermarkExtender>
                    <cms:ExtendedTextBox ID="txtBillingadresse1" EnableEncoding="true" runat="server"
                        CssClass="champtexte right threequarter" MaxLength="100" />
                    <ajaxToolkit:TextBoxWatermarkExtender ID="wmBillingadresse1" runat="server" TargetControlID="txtBillingadresse1">
                    </ajaxToolkit:TextBoxWatermarkExtender>
                </div>
                <cms:ExtendedTextBox ID="txtIdBillingAdresse" EnableEncoding="true" runat="server"
                    MaxLength="100" Visible="false" />
                <cms:ExtendedTextBox ID="txtBillingadresse2" EnableEncoding="true" runat="server"
                    CssClass="champtexte right" MaxLength="100" />
                <ajaxToolkit:TextBoxWatermarkExtender ID="wmBillingadresse2" runat="server" TargetControlID="txtBillingadresse2">
                </ajaxToolkit:TextBoxWatermarkExtender>
                <div class="clr">
                </div>
                <div class="cont_post left">
                    <cms:ExtendedTextBox ID="txtBillingcp" EnableEncoding="true" runat="server" CssClass="champtexte left"
                        MaxLength="100" />
                    <ajaxToolkit:TextBoxWatermarkExtender ID="wmBillingcp" runat="server" TargetControlID="txtBillingcp">
                    </ajaxToolkit:TextBoxWatermarkExtender>
                    <cms:ExtendedTextBox ID="txtBillingville" EnableEncoding="true" runat="server" CssClass="champtexte right"
                        MaxLength="100" />
                    <ajaxToolkit:TextBoxWatermarkExtender ID="wmBillingville" runat="server" TargetControlID="txtBillingville">
                    </ajaxToolkit:TextBoxWatermarkExtender>
                </div>
                <div class="cont_drop_down nb1 right">
                    <asp:DropDownList ID="ddlBillingCountry" runat="server" CssClass="select_personaliser"
                        EnableViewState="true">
                    </asp:DropDownList>
                </div>
                <br />
                <div class="clr">
                </div>
                <asp:CheckBox ID="chkBillingBillingAddr" runat="server" Text="Adresse de facturation"
                    Checked="true" Enabled="false" />
                <asp:CheckBox ID="chkBillingShippingAddr" runat="server" Checked="false" Text="Adresse de livraison" />
                <asp:LinkButton ID="buttonNewBillingAddress" runat="server" CssClass="valider_info right"
                    OnClick="buttonNewBillingAddress_Click"></asp:LinkButton>
                <div class="clr">
                </div>
            </div>
            <ul class="adress_list">
                <asp:Repeater runat="server" ID="RptPickBillingAddress" OnItemDataBound="RptPickBillingAddressItemDataBound"
                    OnItemCommand="RptPickBillingAddressItemCommand" EnableViewState="true">
                    <ItemTemplate>
                        <li><span class="adress_name">
                            <asp:Literal runat="server" ID="ltlBillingAddress"></asp:Literal>
                        </span><span class="option"><a class="editer" data="<%# Container.ItemIndex + 1 %>">
                        </a>
                            <asp:LinkButton ID="buttonBillingEdit" runat="server" CssClass="selectonner" CommandName="Select"
                                CommandArgument='<%# Eval("AddressID") %>'></asp:LinkButton></span>
                            <div class="clr">
                            </div>
                            <div data="<%# Container.ItemIndex + 1 %>" class="slidediv">
                                <asp:Panel ID="pnlUpdateBillingAddress" runat="server" Style="margin-left: -560px;">
                                    <asp:Label ID="lblErrorBillingAdress" runat="server" ForeColor="red" EnableViewState="false" />
                                    <div class="clr">
                                    </div>
                                    <div class="cont_post_popup right">
                                        <div class="cont_post left">
                                            <cms:ExtendedTextBox ID="txtNumeroBillingAdresse" EnableEncoding="true" runat="server"
                                                CssClass="champtexte left quarter" MaxLength="100" />
                                            <ajaxToolkit:TextBoxWatermarkExtender ID="wmNumeroBilling" runat="server" TargetControlID="txtNumeroBillingAdresse" />
                                            <cms:ExtendedTextBox ID="txtBillingadresse1" EnableEncoding="true" runat="server"
                                                CssClass="champtexte right threequarter" MaxLength="100" />
                                            <ajaxToolkit:TextBoxWatermarkExtender ID="wmBillingadresse1" runat="server" TargetControlID="txtBillingadresse1" />
                                        </div>
                                        <cms:ExtendedTextBox ID="txtBillingadresse2" EnableEncoding="true" runat="server"
                                            CssClass="champtexte right" MaxLength="100" />
                                        <ajaxToolkit:TextBoxWatermarkExtender ID="wmBillingadresse2" runat="server" TargetControlID="txtBillingadresse2" />
                                        <div class="cont_post left">
                                            <asp:TextBox ID="txtBillingcp" EnableEncoding="true" runat="server" CssClass="champtexte left"
                                                EnableViewState="true" MaxLength="100" />
                                            <ajaxToolkit:TextBoxWatermarkExtender ID="wmBillingcp" runat="server" TargetControlID="txtBillingcp" />
                                            <cms:ExtendedTextBox ID="txtBillingville" EnableEncoding="true" runat="server" CssClass="champtexte right"
                                                MaxLength="100" />
                                            <ajaxToolkit:TextBoxWatermarkExtender ID="wmBillingville" runat="server" TargetControlID="txtBillingville" />
                                        </div>
                                        <div class="cont_post right">
                                            <cms:ExtendedTextBox ID="txtBillingcountry" EnableEncoding="true" runat="server"
                                                CssClass="champtexte_popup" MaxLength="100" Style="display: block; width: 262px;" />
                                            <ajaxToolkit:TextBoxWatermarkExtender ID="wmShippingcountry" runat="server" TargetControlID="txtBillingcountry"
                                                WatermarkText="Pays" />
                                        </div>
                                    </div>
                                    <div class="clr">
                                    </div>
                                    <asp:CheckBox ID="chk_BillingAddr" runat="server" Text="Adresse de facturation" Checked="true"
                                        Enabled="false" />
                                    <asp:CheckBox ID="chk_ShippingAddr" runat="server" Checked="false" Text="Adresse de livraison" />
                                    <asp:LinkButton ID="buttonUpdate" runat="server" CssClass="btn_update right" CommandName="Update"
                                        CommandArgument='<%# Eval("AddressID") %>'></asp:LinkButton>
                                </asp:Panel>
                            </div>
                            <div class="clr">
                            </div>
                        </li>
                    </ItemTemplate>
                </asp:Repeater>
            </ul>
        </div>
        <div id="modif_facturation" class="block_popup_modif_adress">
            <a class="btn_new_address"></a>
            <div class="new_address">
                <asp:Label ID="lblErrorShippingAdress" runat="server" ForeColor="red" EnableViewState="false" />
                <div class="clr">
                </div>
                <div class="cont_post left">
                    <cms:ExtendedTextBox ID="txtShippingnumero" EnableEncoding="true" runat="server"
                        CssClass="champtexte left quarter" MaxLength="5" />
                    <ajaxToolkit:FilteredTextBoxExtender ID="filtertxtShippingnumero" runat="server" FilterType ="Numbers" TargetControlID = "txtShippingnumero"></ajaxToolkit:FilteredTextBoxExtender>
                    <ajaxToolkit:TextBoxWatermarkExtender ID="wmShippingNumero" runat="server" TargetControlID="txtShippingnumero">
                    </ajaxToolkit:TextBoxWatermarkExtender>
                    <cms:ExtendedTextBox ID="txtShippingadresse1" EnableEncoding="true" runat="server"
                        CssClass="champtexte right threequarter" MaxLength="100" />
                    <ajaxToolkit:TextBoxWatermarkExtender ID="wmShippingadresse1" runat="server" TargetControlID="txtShippingadresse1" />
                </div>
                <cms:ExtendedTextBox ID="txtIdShippingAdresse" EnableEncoding="true" runat="server"
                    MaxLength="100" Visible="false" />
                <cms:ExtendedTextBox ID="txtShippingadresse2" EnableEncoding="true" runat="server"
                    CssClass="champtexte right" MaxLength="100" />
                <ajaxToolkit:TextBoxWatermarkExtender ID="wmShippingadresse2" runat="server" TargetControlID="txtShippingadresse2" />
                <div class="clr">
                </div>
                <div class="cont_post left">
                    <cms:ExtendedTextBox ID="txtShippingcp" EnableEncoding="true" runat="server" CssClass="champtexte left"
                        MaxLength="100" />
                    <ajaxToolkit:TextBoxWatermarkExtender ID="wmShippingcp" runat="server" TargetControlID="txtShippingcp" />
                    <cms:ExtendedTextBox ID="txtShippingville" EnableEncoding="true" runat="server" CssClass="champtexte right"
                        MaxLength="100" />
                    <ajaxToolkit:TextBoxWatermarkExtender ID="wmShippingville" runat="server" TargetControlID="txtShippingville" />
                </div>
                <div class="cont_drop_down nb1 right">
                    <asp:DropDownList ID="ddlShippingCountry" runat="server" CssClass="select_personaliser"
                        EnableViewState="true">
                    </asp:DropDownList>
                </div>
                <br />
                <div class="clr">
                </div>
                <asp:CheckBox ID="chkShippingBillingAddr" runat="server" Text="Adresse de facturation"
                    Checked="true" />
                <asp:CheckBox ID="chkShippingShippingAddr" runat="server" Checked="true" Text="Adresse de livraison"
                    Enabled="false" />
                <asp:LinkButton ID="buttonNewShippingAddress" runat="server" CssClass="valider_info right"
                    OnClick="buttonNewShippingAddress_Click"></asp:LinkButton>
                <div class="clr">
                </div>
            </div>
            <ul class="adress_list">
                <asp:Repeater runat="server" ID="RptPickShippingAddress" OnItemDataBound="RptPickShippingAddressItemDataBound"
                    OnItemCommand="RptPickShippingAddressItemCommand">
                    <ItemTemplate>
                        <li><span class="adress_name">
                            <asp:Literal runat="server" ID="ltlShippingAddress"></asp:Literal></span><span class="option">
                                <a class="editer" data="<%# Container.ItemIndex + 1 %>"></a>
                                <asp:LinkButton ID="buttonShippingEdit" runat="server" CssClass="selectonner" CommandName="Select"
                                    CommandArgument='<%# Eval("AddressID") %>'></asp:LinkButton>
                            </span>
                            <div class="clr">
                            </div>
                            <div data="<%# Container.ItemIndex + 1 %>" class="slidediv">
                                <asp:Panel ID="pnlUpdateAddress" runat="server" Style="margin-left: -560px;">
                                    <asp:Label ID="lblErrorAdress" runat="server" ForeColor="red" EnableViewState="false" />
                                    <div class="clr">
                                    </div>
                                    <cms:ExtendedTextBox ID="txtIdAdresse" EnableEncoding="true" runat="server" MaxLength="100"
                                        Visible="false" />
                                    <div class="cont_post_popup right">
                                        <div class="cont_post left">
                                            <cms:ExtendedTextBox ID="txtShippingNumero" EnableEncoding="true" runat="server"
                                                MaxLength="100" CssClass="champtexte left quarter" />
                                            <ajaxToolkit:TextBoxWatermarkExtender ID="wmNumero" runat="server" TargetControlID="txtShippingNumero" />
                                            <cms:ExtendedTextBox ID="txtShippingadresse1" EnableEncoding="true" runat="server"
                                                CssClass="champtexte right threequarter" MaxLength="100" />
                                            <ajaxToolkit:TextBoxWatermarkExtender ID="wmShipadresse1" runat="server" TargetControlID="txtShippingadresse1" />
                                        </div>
                                        <cms:ExtendedTextBox ID="txtShippingadresse2" EnableEncoding="true" runat="server"
                                            CssClass="champtexte right" MaxLength="100" />
                                        <ajaxToolkit:TextBoxWatermarkExtender ID="wmShipadresse2" runat="server" TargetControlID="txtShippingadresse2" />
                                        <div class="cont_post left">
                                            <asp:TextBox ID="txtShippingcp" EnableEncoding="true" runat="server" CssClass="champtexte left"
                                                EnableViewState="true" MaxLength="100" />
                                            <ajaxToolkit:TextBoxWatermarkExtender ID="wmShipcp" runat="server" TargetControlID="txtShippingcp" />
                                            <cms:ExtendedTextBox ID="txtShippingville" EnableEncoding="true" runat="server" CssClass="champtexte right"
                                                MaxLength="100" />
                                            <ajaxToolkit:TextBoxWatermarkExtender ID="wmShipville" runat="server" TargetControlID="txtShippingville" />
                                        </div>
                                        <div class="cont_post right">
                                            <cms:ExtendedTextBox ID="txtShippingcountry" EnableEncoding="true" runat="server"
                                                CssClass="champtexte_popup" MaxLength="100" Style="display: block; width: 262px;" />
                                            <ajaxToolkit:TextBoxWatermarkExtender ID="wmShippingcountry" runat="server" TargetControlID="txtShippingcountry"
                                                WatermarkText="Pays" />
                                        </div>
                                    </div>
                                    <div class="clr">
                                    </div>
                                    <div style="display: none">
                                        <label>
                                            <asp:CheckBox ID="chk_ShippingAddr" runat="server" Checked="false" />
                                            <asp:Label ID="LabelAdresseDeLivraison" runat="server" Text="Adresse de livraison"></asp:Label>
                                        </label>
                                        <label>
                                            &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</label>
                                        <label>
                                            <asp:CheckBox ID="chk_BillingAddr" runat="server" Checked="false" Enabled="false" />
                                            <asp:Label ID="LabelAdresseFacturation" runat="server" Text="Adresse de facturation"></asp:Label>
                                        </label>
                                    </div>
                                    <asp:LinkButton ID="buttonUpdate" runat="server" CssClass="btn_update right" CommandName="Update"
                                        CommandArgument='<%# Eval("AddressID") %>'></asp:LinkButton>
                                </asp:Panel>
                            </div>
                            <div class="clr">
                            </div>
                        </li>
                    </ItemTemplate>
                </asp:Repeater>
            </ul>
        </div>
        <div id="popup_bundle" class="BlocPPXtGratuit">
            <h3><asp:Literal ID="ltlPopupBundleHeader" runat="server"></asp:Literal></h3>
            <ul class="adress_list" style="height: 352px; overflow-y: scroll">
                <asp:Repeater runat="server" ID="rptBundleSelector" OnItemDataBound="RptBundleSelectorItemDataBound"
                    OnItemCommand="RptBundleSelectorItemCommand">
                    <ItemTemplate>
                        <li><span class="adress_name" style="width: 500px;">
                            <asp:Literal runat="server" ID="litProduct"></asp:Literal></span><span class="option">
                                <asp:LinkButton ID="buttonShippingEdit" runat="server" CssClass="selectonner" CommandName="Select"
                                    CommandArgument='<%# Eval("ProductID") %>'></asp:LinkButton></span>
                            <div class="clr">
                            </div>
                        </li>
                    </ItemTemplate>
                </asp:Repeater>
            </ul>
        </div>
        <div class="clr">
        </div>
        <div class="cont_bgt_panier">
            <a class="btn_continue_achat" href="~/"></a>
            <asp:Button runat="server" ID="btn_valid_order" CssClass="btn_valid_order right"
                OnClick="btn_Valid_Order_Click" />
            <div class="clr">
            </div>
        </div>
        <div class="clr">
        </div>
    </div>
</div>
<script type="text/javascript">

    jQuery(document).ready(function () {
        jQuery('.slidediv').hide();
        jQuery('.new_address').hide();
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
        jQuery('.btn_new_address').click(function () {
            jQuery('.slidediv').hide();
            jQuery('.new_address').slideToggle();

        });
        jQuery('.btn_new_addressfacture').click(function () {
            jQuery('.slidediv').hide();
            jQuery('.new_address').slideToggle();

        });
        jQuery('.block_popup_modif_adress').click(function () {
            //jQuery('slidediv').hide();
            //jQuery('.new_address').hide();
        });
    });
</script>
