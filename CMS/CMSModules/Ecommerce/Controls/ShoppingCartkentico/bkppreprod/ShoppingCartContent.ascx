<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_Ecommerce_Controls_ShoppingCart_ShoppingCartContent"
    CodeFile="ShoppingCartContent.ascx.cs" %>
<%@ Register Src="~/CMSModules/Ecommerce/FormControls/CurrencySelector.ascx" TagName="CurrencySelector"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/ECommerce/FormControls/PaymentSelector.ascx" TagName="PaymentSelector"
    TagPrefix="cms" %>
<asp:Panel ID="pnlCartContent" runat="server" DefaultButton="btnUpdate">
    <asp:Label ID="lblTitle" runat="server" CssClass="BlockTitle" EnableViewState="false" />
    <div class="left_block_payement etape_1">
        <%-- Message labels --%>
        <asp:Label ID="lblInfo" runat="server" CssClass="LabelInfo" Visible="false" EnableViewState="false" />
        <asp:Label ID="lblError" runat="server" CssClass="ErrorLabel" EnableViewState="false"
            Visible="false" />
        <asp:Panel ID="pnlCartLeftInnerContent" runat="server">
            <asp:Panel ID="pnlNewItem" runat="server" EnableViewState="false" Visible="false">
                <asp:Image ID="imgNewItem" runat="server" CssClass="NewItemImage" EnableViewState="false" />
                <asp:LinkButton ID="lnkNewItem" runat="server" CssClass="NewItemLink" EnableViewState="false" />
            </asp:Panel>
            <asp:Panel ID="pnlCurrency" runat="server" Visible="False">
                s
                <asp:Label ID="lblCurrency" runat="server" EnableViewState="false" AssociatedControlID="selectCurrency" />
                <cms:CurrencySelector ID="selectCurrency" runat="server" DisplayOnlyWithExchangeRate="true"
                    DoFullPostback="true" RenderInline="true" />
            </asp:Panel>
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
                                <span class="name_produit"><a href="<%# GetProductNodeAliasPath(Eval("SKUID"))%>">
                                    <asp:Literal ID="ltlProductName" runat="server"></asp:Literal></a></span>
                            </td>
                            <td>
                                <span class="cont_line">
                                    <asp:LinkButton ID="LinkButtonDecrease" CssClass="decremente" runat="server" CommandName="Decrease"
                                        CommandArgument='<%# Eval("CartItemGuid") %>'></asp:LinkButton>
                                    <asp:TextBox ID="txtProductCount" runat="server" CssClass="nombre_produit" OnTextChanged="txtProductCount_TextChanged" ReadOnly="true"></asp:TextBox>
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
                <li><span class="txt_ttc">
                    <asp:Literal ID="lblMontantAchat" runat="server" EnableViewState="false" />
                    <%--<em>€</em>--%></span><strong class="txt_orange">montant des achats <em>(TTC)</em></strong>
                    <div class="clr">
                    </div>
                </li>
                <li><span>
                    <asp:UpdatePanel ID="updatePanelShippingPriceValue" runat="server">
                        <ContentTemplate>
                            <asp:Literal ID="lblShippingPriceValue" runat="server" EnableViewState="false" />
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </span><strong>frais d'envoi</strong>
                    <div class="clr">
                    </div>
                </li>
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
                            <cms:CMSTextBox ID="txtCoupon" runat="server" MaxLength="200" EnableViewState="false" />
                        </asp:PlaceHolder>
                    </td>
                </tr>
                <tr>
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
                </tr>
            </table>
            <%-- Fin table code par défaut--%></asp:Panel>
    </div>
    <asp:Panel ID="pnlCartRightInnerContent" runat="server">
        <div runat="server" class="right_block_payement etape_1">
            <div class="info_requier">
                <label for="payement_option" class="first">
                    Moyen de payement</label>
                <div class="cont_drop_down nb1">
                    <cms:LocalizedDropDownList ID="ddlPaymentOption" runat="server" CssClass="select_personaliser"
                        OnSelectedIndexChanged="ddlPaymentOption_SelectedIndexChanged" AutoPostBack="true">
                    </cms:LocalizedDropDownList>
                    <asp:UpdatePanel ID="updatePanelPaymentOption" runat="server" Visible="false">
                        <ContentTemplate>
                            <asp:Label ID="lblPayment" runat="server" Visible="false"></asp:Label>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>
                <label for="payement_option">
                    Pays de livraison</label>
                <div class="cont_drop_down nb2">
                    <asp:DropDownList ID="ddlShippingCountry" runat="server" CssClass="select_personaliser"
                        OnSelectedIndexChanged="ddlShippingCountry_SelectedIndexChanged" AutoPostBack="true"
                        EnableViewState="true">
                    </asp:DropDownList>
                    <asp:UpdatePanel ID="updatePanelShippingCountry" runat="server" Visible="false">
                        <ContentTemplate>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>
                <label for="payement_option">
                    methode de livraison</label>
                <div class="cont_drop_down nb3">
                    <cms:LocalizedDropDownList ID="ddlShippingOption" runat="server" CssClass="select_personaliser"
                        OnSelectedIndexChanged="ddlShippingOption_SelectedIndexChanged" AutoPostBack="true">
                    </cms:LocalizedDropDownList>
                    <asp:UpdatePanel ID="updatePanelShippingOption" runat="server">
                        <ContentTemplate>
                            <asp:Label ID="lblPriceID" runat="server" Visible="false">TEST PRICE ID</asp:Label>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>
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
    <asp:Panel ID="pnlBtnNext" runat="server">
        <a class="btn_pass_commande" href="#">
            <cms:CMSButton ID="btnNext" runat="server" OnClick="btnNext_Click" CssClass="btn_pass_commande"
                ValidationGroup="ButtonNext" RenderScript="true" EnableViewState="false" Width="265px"
                Height="46px" Style="position: absolute; z-index: 2; opacity: 0; filter: alpha(opacity=0);" />
        </a>
    </asp:Panel>
    <asp:Panel runat="server" ID="pnlNextButton">
    </asp:Panel>
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
