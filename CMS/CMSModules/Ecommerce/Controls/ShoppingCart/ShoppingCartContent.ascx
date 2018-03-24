<%@ Control Language="C#" AutoEventWireup="true" Debug="true" Inherits="CMSModules_Ecommerce_Controls_ShoppingCart_ShoppingCartContent"
    CodeFile="ShoppingCartContent.ascx.cs" %>
<%@ Register Src="~/CMSModules/Ecommerce/FormControls/CurrencySelector.ascx" TagName="CurrencySelector"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/ECommerce/FormControls/PaymentSelector.ascx" TagName="PaymentSelector"
    TagPrefix="cms" %>
<asp:Panel ID="pnlCartContent" runat="server" DefaultButton="btnUpdate">
    <asp:Label ID="lblTitle" runat="server" CssClass="BlockTitle" EnableViewState="false" />
    <%-- Message labels --%>
    <asp:Label ID="lblInfo" ClientIDMode="Static" runat="server" CssClass="LabelInfo" Visible="false" EnableViewState="false" />
    <asp:Label ID="lblError" runat="server" CssClass="ErrorLabel" EnableViewState="false"  Visible="false" />
    <asp:Label ID="lblPanierVide" runat="server" ForeColor="red" Visible="false" CssClass = "LblErreur"/>
   <asp:Panel ID="test" runat="server">
    <div class="left_block_payement etape_1">
        <asp:Panel ID="pnlCartLeftInnerContent" runat="server">
            <asp:Panel ID="pnlNewItem" runat="server" EnableViewState="false" Visible="false">
                <asp:Image ID="imgNewItem" runat="server" CssClass="NewItemImage" EnableViewState="false" />
                <asp:LinkButton ID="lnkNewItem" runat="server" CssClass="NewItemLink" EnableViewState="false" />
            </asp:Panel>
            <asp:Panel ID="pnlCurrency" runat="server" Visible="False">
                <asp:Label ID="lblCurrency" runat="server" EnableViewState="false" AssociatedControlID="selectCurrency" />
                <cms:CurrencySelector ID="selectCurrency" runat="server" DisplayOnlyWithExchangeRate="true"
                    DoFullPostback="true" RenderInline="true" />
            </asp:Panel>
            <table class="lst_produit">
                <tr>
                    <th class="article">
                        <asp:Label ID="LabelArticle" runat="server" Text="Article"></asp:Label>
                    </th>
                    <th class="qte">
                        <asp:Label ID="LabelQte" runat="server" Text="Quantité"></asp:Label>
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
                                <span class="name_produit"><a href="<%# GetProductNodeAliasPath(Eval("SKUID"))%>">
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
                                                    
                                      <%-- <asp:Literal ID="ltlProductPrice" runat="server"></asp:Literal><em>€</em></span>--%>
                                       <asp:Literal ID="ltlProductPrice" runat="server" Text='<%#GetFormattedValue(Eval("TotalPrice"))%>'></asp:Literal><em>€</em></span>
                                  <%--  <asp:Literal ID="ltlProductPrice" runat="server" Text=''></asp:Literal><em>€</em></span>--%>
                                 
                                   <%-- <asp:Literal ID="ltlProductPrice" runat="server" Text='<%# Double.Parse(GetFormattedValue(Eval("UnitTotalDiscount"))) + Double.Parse(GetFormattedValue(Eval("TotalPrice"))) %>'></asp:Literal><em>€</em></span>--%>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="4" class="white_space">
                            </td>
                        </tr>
                    </ItemTemplate>
                </asp:Repeater>
            </table>
            <asp:Panel ID="pnlMiddleTotalButtons" runat="server">
                <ul class="block_total">
                <!-- total sans coupon-->
                     <li runat="server" id= "sancoupon" visible="false"><span class="txt_ttc">
                        <asp:Literal ID="lblMontantAchatSanscoupon" runat="server" EnableViewState="false"  Visible="false"/>
                        <%--<em>€</em>--%></span><strong class="txt_orange">
                            <asp:Literal ID="LabelMontantDesAchatsSanscoupon" runat="server" Text="montant des achats sans coupon" Visible="false"></asp:Literal></strong>
                        <div class="clr">
                        </div>
                    </li>
                   
                    <!-- total coupon-->
                    <li runat="server" id="totalcoupon"><span class="txt_ttc">
                        <asp:Literal ID="lblMontantCoupon" runat="server" EnableViewState="false" Visible="false" />
                        <%--<em>€</em>--%></span><strong class="txt_orange">
                            <asp:Literal ID="LabelMontantCoupon" runat="server" Text="montant total du coupon" Visible="false"></asp:Literal></strong>
                        <div class="clr">
                        </div>
                    </li>
                     <!-- total avec coupon-->
                    <li><span class="txt_ttc">
                        <asp:Literal ID="lblMontantAchat" runat="server" EnableViewState="false" />
                        <%--<em>€</em>--%></span><strong class="txt_orange">
                            <asp:Literal ID="LabelMontantDesAchats" runat="server" Text="montant des achats"></asp:Literal></strong>
                        <div class="clr">
                        </div>
                    </li>
				<% if(IsShippingNeeded) { %>
                    <li>
						<span>
							<asp:Literal ID="lblShippingPriceValue" runat="server" EnableViewState="false" />
						</span>
						<strong>
							<asp:Literal ID="LabelFraisEnvoi" runat="server" Text="frais d'envoi"></asp:Literal>
						</strong>
                        <div class="clr"></div>
                    </li>
				<% } %>
                </ul>
                <div class="clr">
                </div>
                <%-- Début table code par défaut--%>
                <table width="100%" style="display: block">
                    <tr>
                        <td class="TextLeft">
                        </td>
                   
                        <%-- Discount coupon --%>
                    <td class="TextRight">
                            <asp:PlaceHolder ID="plcCoupon" runat="server" EnableViewState="false">
                                <asp:Label ID="lblCoupon" AssociatedControlID="txtCoupon" runat="server" EnableViewState="false" />&nbsp;
                                <cms:CMSTextBox ID="txtCoupon" runat="server" MaxLength="200" EnableViewState="false"
                                    CssClass="champtexte" />
                                <asp:Button ID="btncoupon" OnClick="btncoupon_Click" runat="server" ClientIDMode="Static"
                                    CssClass="btn_Check" EnableViewState="false" />
                            </asp:PlaceHolder>
                        </td>
                    </tr>
                    <!-- For order discount-->
                  
                   	<asp:PlaceHolder ID="plcOrderDiscount" runat="server" Visible="false">
                        <tr class="TotalShipping">
                            <td style="width: 100%">
                                &nbsp;
                            </td>
                            <td style="white-space: nowrap;">
                                <asp:Label ID="lblOrderDiscount" runat="server" EnableViewState="false" />
                            </td>
                            <td class="TextRight" style="white-space: nowrap;">
                                <asp:Label ID="lblOrderDiscountValue" runat="server" EnableViewState="false" />
                            </td>
                        </tr>
                    </asp:PlaceHolder>
                     <!-- For order discount-->
                    <!--<tr>
                        <td colspan="2">
                            &nbsp;
                        </td>
                    </tr>
                    <tr>
                        <td class="TextLeft" colspan="2">
                            <asp:Panel ID="pnlPrice" runat="server" EnableViewState="true">
                                <table width="100%">
                                    <%-- Shipping price --%>
                                    <asp:PlaceHolder ID="plcShippingPrice" runat="server">
                                        <tr class="TotalShipping">
                                           <td class="col1">
                                                &nbsp;
                                            </td>
                                            <td style="white-space: nowrap;">
                                            </td>
                                            <td class="TextRight" style="white-space: nowrap;">
                                            </td>
                                        </tr>
                                    </asp:PlaceHolder>
                                    <tr runat="server" id="ButtonsEmpty" visible="true">
                                        <%-- Empty action  --%>
                                        <td class="col1">
                                            <cms:CMSButton ID="btnEmpty" runat="server" OnClick="btnEmpty_Click1" CssClass="ContentButton"
                                                EnableViewState="false" />
                                        </td>
                                        <%-- Update action --%>
                                        <td colspan="2" class="TextRight">
                                            <cms:CMSButton ID="btnUpdate" runat="server" OnClick="btnUpdate_Click1" CssClass="ContentButton"
                                                EnableViewState="false" />
                                            <asp:Button ID="Button1" runat="server" OnClick="Button1_Click" Text="Step 3" />
                                        </td>
                                    </tr>
                                </table>
                                <asp:Label ID="lblTotalTax" runat="server" EnableViewState="false" /><br />
                                <asp:Label ID="lblTaxSubtotal" runat="server" EnableViewState="false" /><br />
                                <asp:Label ID="lblRoundedDifference" runat="server" EnableViewState="false" /><br />
                            </asp:Panel>
                        </td>
                    </tr>-->
                </table>
                <asp:Label ID="lblErrorCoupon" runat="server" ForeColor="red" Visible="false" CssClass="LblErreur" />
                <%-- Fin table code par défaut--%>
            </asp:Panel>
        </asp:Panel>
    </div>
    </asp:Panel>
    <asp:Panel ID="pnlCartRightInnerContentAuthenticated" ClientIDMode="static" runat="server">
        <div class="right_block_payement etape_1">
            <div class="pad_40">
            </div>
            <% if(IsShippingNeeded) { %>
            <div class="block_gris_payement first">
                <div class="cont_block_gris first">
                    <h4>
                        <asp:Label ID="LabelAdresseLivraison" runat="server" Text="adresse de livraison"></asp:Label>
                    </h4>
                    <a href="#modif_facturation" class="right fancybox_adress_facturation">
                        <asp:Label ID="LabelModifier" runat="server" Text="modifier"></asp:Label></a>
                    <div class="clr">
                    </div>
                    <div class="facturation_info">
                        <p>
                            <asp:Label ID="lblShippingAddressFullName" runat="server"></asp:Label>
                        </p>
                        <p>
                            <asp:Label ID="lblShippingAddressStreet" runat="server">street</asp:Label>
                        </p>
                        <p>
                            <asp:Label ID="lblShippingAddressZipCode" runat="server">zipcode</asp:Label>
                        </p>
                        <p>
                            <asp:Label ID="lblShippingAddressCityCountry" runat="server">country</asp:Label>
                        </p>
                    </div>
                </div>
            </div>
		
            <div class="block_gris_payement second">
                <div class="cont_block_gris first">
                    <h4 class="left">
                        <asp:Label ID="LabelOptionEnvoi" runat="server" Text="Option d'envoi"></asp:Label>
                    </h4>
                    <em class="left"></em><span class="tit_moyen left">
                        <asp:Literal ID="Literal1" runat="server" Visible="true"></asp:Literal></span>
                    <div class="clr">
                    </div>
                    <a href="#"></a>
                    <div class="clr">
                    </div>
                    <div class="pad_10">
                    </div>
                    <div class="cont_drop_down left_drop nb1">
                        <cms:LocalizedDropDownList ID="ddlShippingOptionAuthenticated" runat="server" CssClass="select_personaliser"
                            OnSelectedIndexChanged="ddlShippingOptionAuthenticated_SelectedIndexChanged"
                            AutoPostBack="true">
                        </cms:LocalizedDropDownList>
                    </div>
                </div>
            </div>
		<% } %>
        </div>
    </asp:Panel>
    <asp:Panel ID="pnlCartRightInnerContent" runat="server">
        <div id="Div1" runat="server" class="right_block_payement etape_1">
            <h3>
                <asp:Label ID="lbletape" runat="server" Text="Etape obligatoire"></asp:Label></h3>
            <div class="info_requier">
                <label for="payement_option" class="first">
                    <asp:Label ID="LabelMoyenDePaiement" runat="server" Text="Moyen de paiement"></asp:Label>
                </label>
                <div class="cont_drop_down nb1">
                    <cms:LocalizedDropDownList ID="ddlPaymentOption" runat="server" CssClass="select_personaliser"
                        OnSelectedIndexChanged="ddlPaymentOption_SelectedIndexChanged" AutoPostBack="false">
                    </cms:LocalizedDropDownList>
                </div>
                <label for="payement_option">
                    <!-- ajout label-->
                    <span class="textefrais">
                        <asp:Label ID="lbltextcalculfrais" runat="server" Text="Calcul frais de livraison"></asp:Label></span>
                    <br />
                    <!-- fin label-->
                    <asp:Label ID="LabelPaysDeLivraison" runat="server" Text="Pays de livraison"></asp:Label>
                </label>
                <div class="cont_drop_down nb2">
                    <asp:DropDownList ID="ddlShippingCountry" runat="server" CssClass="select_personaliser"
                        OnSelectedIndexChanged="ddlShippingCountry_SelectedIndexChanged" AutoPostBack="true"
                        EnableViewState="true">
                    </asp:DropDownList>
                </div>
                <asp:Panel runat="server" ID="pnlShippingOption">
                    <label for="payement_option">
                        <asp:Label ID="LabelMethodeDeLivraison" runat="server" Text="methode de livraison"></asp:Label>
                    </label>
                    <div class="cont_drop_down nb3">
                        <cms:LocalizedDropDownList ID="ddlShippingOption" runat="server" CssClass="select_personaliser"
                            OnSelectedIndexChanged="ddlShippingOption_SelectedIndexChanged" AutoPostBack="true">
                        </cms:LocalizedDropDownList>
                    </div>
                </asp:Panel>
                <%--<div class="cont_drop_down nb2">
                <select name="payement_option" id="payement_option2" class="select_personaliser">
                    <option>dhl</option>
                    <option>Priorité</option>
                    <option>Chronopost</option>
                </select>
            </div> --%>
                <%--<div class="cont_drop_down nb3">
                <select name="payement_option" id="payement_option3" class="select_personaliser">
                    <option>Belgique</option>
                    <option>France</option>
                    <option>Allemange</option>
                    <option>Etats-unis</option>
                </select>
            </div>--%>
            </div>
        </div>
        <asp:CheckBox ID="chkSendEmail" runat="server" Visible="false" EnableViewState="false" />
        <asp:Literal ID="ltlAlert" runat="server" EnableViewState="false" Visible="false" />
        <asp:Literal ID="ltlScript" runat="server" EnableViewState="true" Visible="false" />
    </asp:Panel>
</asp:Panel>
<div class="cont_bgt_panier">
    <a class="btn_continue_achat" href="~/Accueil.aspx" style="clear: both"></a>
    <%-- <asp:Panel ID="Panel1" runat="server" Visible="false">
        
            <asp:Button ID="Button2" runat="server" OnClick="btnNext_Click" ClientIDMode="Static" CssClass="btn_pass_commande"
                ValidationGroup="ButtonNext" EnableViewState="false"  />
      
    </asp:Panel>--%>
    <asp:Panel ID="pnlBtnNext" ClientIDMode="Static" runat="server" Visible="false">
        <asp:Button ID="btnNext" runat="server" OnClick="btnNext_Click" ClientIDMode="Static"
            CssClass="btn_EtpSuivant" ValidationGroup="ButtonNext" EnableViewState="false"
            Visible="false" />
    </asp:Panel>
    <asp:Panel ID="pnlBtnNext1" runat="server" Visible="false" ClientIDMode="Static">
        <div id="modal_dialog" style="display: none">
            <asp:Label ID="lblInsertionPays" runat="server" Text="Pays"></asp:Label></div>
        <asp:Button ID="btnNext1" runat="server" CssClass="btn_EtpSuivant" Visible="false"  ClientIDMode="Static"  ValidationGroup="ButtonNext" EnableViewState="false" />
    </asp:Panel>
    <asp:Panel runat="server" ID="pnlNextButton">
    </asp:Panel>
</div>
<div class="BlockContent" id="popup" runat="server">
    <div class="cont_payemet">
        <div id="modif_facturation" class="block_popup_modif_adress">
            <a class="btn_new_address"></a>
            <div class="new_address">
                <asp:Label ID="lblErrorShippingAdress" runat="server" ForeColor="red" EnableViewState="false" />
                <div class="clr">
                </div>
                <div class="cont_post left">
                    <cms:ExtendedTextBox ID="txtnumero" EnableEncoding="true" runat="server" CssClass="champtexte left quarter"
                        MaxLength="5" />
                    <ajaxToolkit:FilteredTextBoxExtender ID="filterTxtNumero" runat="server" FilterType="Numbers"
                        TargetControlID="txtnumero">
                    </ajaxToolkit:FilteredTextBoxExtender>
                    <cms:ExtendedTextBox ID="txtIdShippingAdresse" EnableEncoding="true" runat="server"
                        MaxLength="100" Visible="false" /><cms:ExtendedTextBox ID="txtShippingadresse1" EnableEncoding="true"
                            runat="server" CssClass="champtexte right threequarter" MaxLength="100" />
                </div>
                <cms:ExtendedTextBox ID="txtShippingadresse2" EnableEncoding="true" runat="server"
                    CssClass="champtexte right" MaxLength="100" />
                <div class="clr">
                </div>
                <div class="cont_post left">
                    <cms:ExtendedTextBox ID="txtShippingcp" EnableEncoding="true" runat="server" CssClass="champtexte left"
                        MaxLength="100" />
                    <cms:ExtendedTextBox ID="txtShippingville" EnableEncoding="true" runat="server" CssClass="champtexte right"
                        MaxLength="100" />
                </div>
                <div class="cont_drop_down nb1 right">
                    <asp:DropDownList ID="ddlShippingCountryAuthenticated" runat="server" CssClass="select_personaliser"
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
                                    <div class="cont_post_popup right">
                                        <div class="cont_post left">
                                            <cms:ExtendedTextBox ID="txtShippingNumero" EnableEncoding="true" runat="server"
                                                MaxLength="100" CssClass="champtexte left quarter" />
                                            <ajaxToolkit:TextBoxWatermarkExtender ID="wmNumero" runat="server" TargetControlID="txtShippingNumero">
                                            </ajaxToolkit:TextBoxWatermarkExtender>
                                            <cms:ExtendedTextBox ID="txtShippingadresse1" EnableEncoding="true" runat="server"
                                                CssClass="champtexte right threequarter" MaxLength="100" />
                                            <ajaxToolkit:TextBoxWatermarkExtender ID="wmShipadresse1" runat="server" TargetControlID="txtShippingadresse1">
                                            </ajaxToolkit:TextBoxWatermarkExtender>
                                        </div>
                                        <cms:ExtendedTextBox ID="txtShippingadresse2" EnableEncoding="true" runat="server"
                                            CssClass="champtexte right" MaxLength="100" />
                                        <ajaxToolkit:TextBoxWatermarkExtender ID="wmShipadresse2" runat="server" TargetControlID="txtShippingadresse2"
                                            WatermarkText="Adresse2" />
                                        <div class="cont_post left">
                                            <asp:TextBox ID="txtShippingcp" EnableEncoding="true" runat="server" CssClass="champtexte left"
                                                EnableViewState="true" MaxLength="100" />
                                            <ajaxToolkit:TextBoxWatermarkExtender ID="wmShipcp" runat="server" TargetControlID="txtShippingcp"
                                                WatermarkText="CP" />
                                            <cms:ExtendedTextBox ID="txtShippingville" EnableEncoding="true" runat="server" CssClass="champtexte right"
                                                MaxLength="100" />
                                            <ajaxToolkit:TextBoxWatermarkExtender ID="wmShipville" runat="server" TargetControlID="txtShippingville"
                                                WatermarkText="Ville" />
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
                                            <asp:Label ID="LabelAdresseDeFacturation" runat="server" Text="Adresse de facturation"></asp:Label>
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
    </div>
</div>
<script type="text/javascript">
    //<![CDATA[
    // Opens "Add order items" dialog
    function OpenAddProductDialog(cartSessionKey) {
        var cart = "";
        if ((cartSessionKey != null) & (cartSessionKey != "")) {
            cart = "?cart=" + cartSessionKey;
        }
        modalDialog(addProductDialogURL + cart, 'addProduct', 800, 620);
    }
    //]]>
</script>
<asp:HiddenField runat="server" ID="hdnPrice" />
<asp:HiddenField runat="server" ID="hdnIsPrivate" />
<asp:HiddenField runat="server" ID="hdncoupon" ClientIDMode="static" />
<script src="http://ajax.aspnetcdn.com/ajax/jquery.ui/1.8.9/jquery-ui.js" type="text/javascript"></script>
<link href="http://ajax.aspnetcdn.com/ajax/jquery.ui/1.8.9/themes/start/jquery-ui.css" rel="stylesheet" type="text/css" />
<script type="text/javascript">

    jQuery(document).ready(function () {

        if (jQuery("#lblPanierVide").text() != "") {

            jQuery("#pnlBtnNext").hide();
            jQuery("#pnlBtnNext1").hide();
            jQuery("#btnNext1").hide();
            jQuery("#btnNext").hide();
            jQuery("#test").hide();
            jQuery(".btn_EtpSuivant btn_EtpSuivantfr-FR").hide();
            jQuery(".btn_EtpSuivant btn_EtpSuivanten-US").hide();
        }

        jQuery("#txtCoupon").blur(function () {
            var coupon = jQuery("#txtCoupon").val();
            jQuery("#hdncoupon").val(coupon);
        });

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
        jQuery('.block_popup_modif_adress').click(function () {
            //jQuery('slidediv').hide();
            //jQuery('.new_address').hide();
        });

        //        $("#modal_dialog").css("background-color", "#FF8C00");
        //        $("a.ui-dialog-titlebar-close ui-corner-all").remove();

    });



    $("[id*=btnNext1]").live("click", function () {
        $("#modal_dialog").dialog({
            //   title: "Message",
            resizable: false,
            width: "230px",

            buttons: {
                OK: function () {
                    $(this).dialog('close');
                }
            },

            open: function (event, ui) {
                $(this).parents(".ui-dialog:first").find(".ui-widget-header").removeClass("ui-widget-header").addClass("ui-widget-header-custom");
                $(this).parents(".ui-dialog:first").find(".ui-button-text").addClass("dialog_style2");

            },


            modal: true
        });
        return false;
    });
  
 
</script>

 <style type="text/css">
   .dialog_style1{
    background-color: #FF8C00;
    
   }
   
    .dialog_style2{
    background-color:#483D8B;
     font:  #FF8C00;
   }
         
   
      .ui-dialog .ui-dialog-content
        {
            position: relative;
            border: 0;
            height : 150px;
            padding: .5em 1em;
            background: none;
            overflow: auto;
         /*   zoom: 1;*/
            background-color: #534F56!important;
            border-bottom: solid 1px #fff;
	  /*  min-height:auto!important;*/
	        min-height:53px!important;
	        color:#fff
        }

        .ui-dialog .ui-dialog-titlebar
        {
            display:none;
        }

        .ui-widget-content
        {
            border:none;
        }
        
	    .ui-state-default, .ui-widget-content .ui-state-default, .ui-widget-header .ui-state-default{ border:0}
	    .dialog_style2{ background:#fff; color:#534F56}
	    
        /*add*/
        .ui-dialog-buttonpane
        {
             background-color: #534F56; 
	         margin-top:0!important
        }
    
        /*bouton ok*/
        .ui-button
        {
            border: none;
        }
               
   
  </style>