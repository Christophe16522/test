<%@ Control Language="C#" AutoEventWireup="true" Debug="true" Inherits="CMSModules_Ecommerce_Controls_ShoppingCart_ShoppingCartOrderAddresses"
    CodeFile="ShoppingCartOrderAddresses.ascx.cs" %>
<%@ Register Src="~/CMSFormControls/CountrySelector.ascx" TagName="CountrySelector"
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
                        adresse de facturation</h4>
                    <a href="#modif_adress" class="right fancybox_list_adress">modifier</a>
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
                        adresse de livraison</h6>
                    <a href="#modif_facturation" class="right fancybox_adress_facturation">modifier</a>
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
            <div class="block_gris_payement second">
                <div class="cont_block_gris first">
                    <h4 class="left">
                        moyen de paiement</h4>
                    <em class="left">/</em><span class="tit_moyen left"><asp:Literal ID="lblPayment"
                        runat="server" Visible="true"></asp:Literal></span>
                    <div class="clr">
                    </div>
                    <a href="#"></a>
                    <form action="" method="post">
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
                    </form>
                </div>
                <asp:Panel ID="pnlPaymentOption" runat="server">
                    <div class="separateur">
                    </div>
                    <div class="cont_block_gris second">
                        <h4 class="left">
                            carte de paiement</h4>
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
            <asp:Panel ID="pnlCartLeftInnerContent" runat="server">
                <table class="lst_produit">
                    <tr>
                        <th class="article">
                            Article
                        </th>
                        <th class="qte">
                            Quantité
                        </th>
                        <th class="suppr">
                            &nbsp;
                        </th>
                        <th class="prix">
                            Prix Total
                        </th>
                    </tr>
                    <asp:Repeater ID="rptCart" runat="server" EnableViewState="true" OnItemDataBound="RptCartItemDataBound"
                        OnItemCommand="RptCartItemCommand">
                        <ItemTemplate>
                            <tr>
                                <td class="first">
                                    <div class="produit_horizonal">
                                        <%--<img src="~/App_Themes/Servranx/images/img_produit_panier_horiz.jpg" width="63" height="47" alt="Image produit horizontal" />--%>
                                        <asp:Literal ID="ltlProductImage" runat="server"></asp:Literal>
                                    </div>
                                    <span class="name_produit"><a href="#">
                                        <asp:Literal ID="ltlProductName" runat="server"></asp:Literal></a></span>
                                </td>
                                <td>
                                    <span class="cont_line">
                                        <asp:LinkButton ID="LinkButtonDecrease" CssClass="decremente" runat="server" CommandName="Decrease"
                                            CommandArgument='<%# Eval("CartItemGuid") %>'></asp:LinkButton>
                                        <asp:TextBox ID="txtProductCount" runat="server" CssClass="nombre_produit"></asp:TextBox>
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
                    <li><span>
                        <asp:Literal ID="lblMontantAchat" runat="server" EnableViewState="false" />
                        <%--<em>€</em>--%></span><strong>sous total<em>(TTC)</em></strong>
                        <div class="clr">
                        </div>
                    </li>
                    <li><span>
                        <asp:Literal ID="lblShippingPriceValue" runat="server" EnableViewState="false" />
                    </span><strong>frais d'envoi</strong>
                        <div class="clr">
                        </div>
                    </li>
                    <li><span class="txt_ttc">
                        <asp:Literal ID="lblTotalPriceValue" runat="server" EnableViewState="false"></asp:Literal></span><strong
                            class="txt_orange">Montant total <em>(TTC)</em></strong>
                        <div class="clr">
                        </div>
                    </li>
                </ul>
            </asp:Panel>
        </div>
        <div class="clr">
        </div>
        <div id="modif_adress" class="block_popup_modif_adress">
            <a class="btn_new_address"></a>
            <ul class="adress_list">
                <asp:Repeater runat="server" ID="RptPickBillingAddress" OnItemDataBound="RptPickBillingAddressItemDataBound">
                    <ItemTemplate>
                        <li><span class="adress_name">
                            <asp:Literal runat="server" ID="ltlAddress"></asp:Literal></span><span class="option"><a
                                href="#" class="supprimer"></a><a href="#" class="editer"></a></span><div class="clr">
                                </div>
                        </li>
                    </ItemTemplate>
                </asp:Repeater>
            </ul>
            <!--<input name="" type="submit" value=" " class="valider_info" />-->
            <input name="" type="submit" value=" " class="btn_update right" />
            <div class="clr">
            </div>
        </div>
        <div id="modif_facturation" class="block_popup_modif_adress">
            <a class="btn_new_address"></a>
            <ul class="adress_list">
                <asp:Repeater runat="server" ID="RptPickShippingAddress" OnItemDataBound="RptPickShippingAddressItemDataBound">
                    <ItemTemplate>
                        <li><span class="adress_name">
                            <asp:Literal runat="server" ID="ltlAddress"></asp:Literal></span><span class="option"><a
                                href="#" class="supprimer"></a><a href="#" class="editer"></a></span><div class="clr">
                                </div>
                        </li>
                    </ItemTemplate>
                </asp:Repeater>
            </ul>
            <!--<input name="" type="submit" value=" " class="valider_info" />-->
            <input name="" type="submit" value=" " class="btn_update right" />
            <div class="clr">
            </div>
        </div>
        <div class="clr">
        </div>
        <div class="cont_bgt_panier">
            <a class="btn_continue_achat" href="#"></a>
            <asp:Button runat="server" ID="btn_valid_order" CssClass="btn_valid_order right"
                OnClick="btn_Valid_Order_Click" />
            <div class="clr">
            </div>
        </div>
        <div class="clr">
        </div>
    </div>
</div>
