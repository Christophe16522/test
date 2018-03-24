<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_Ecommerce_Controls_ShoppingCart_ShoppingCart"
    CodeFile="ShoppingCart.ascx.cs" %>
<asp:Panel ID="pnlShoppingCart" runat="server" DefaultButton="btnNext">
    <div class="MainContent">
        <asp:Label ID="lblError" runat="server" EnableViewState="false" CssClass="ErrorLabel"
            Visible="false" />
        <asp:Label ID="lblInfo" runat="server" EnableViewState="false" CssClass="InfoLabel"
            Visible="false" />
        <div class="BgMainContL1">
        </div>
        <div class="BgMainContR">
        </div>
        <div class="BlocMainContent">
            <div class="NormalPage">
                <div class="BgTitrePane tit_payement">
                    <h1>
                        <asp:Literal ID="lblStepTitle" runat="server" EnableViewState="false">
                        </asp:Literal>
                    </h1>
                </div>
                <%--<table class="CartTable" style="width: 100%" cellpadding="0" cellspacing="0">
                        <tr>
                            <td colspan="2">
                                <table class="CartStepTable" style="width: 100%" cellspacing="0" cellpadding="3"
                                    border="0">
                                    <tr class="UniGridHead">
                                    </tr>
                                    <asp:PlaceHolder ID="plcCheckoutProcess" runat="server" EnableViewState="false">
                                        <tr class="CartStepBody">
                                            <td colspan="2" align="center" style="padding: 0px; text-align: center;">
                                                <asp:PlaceHolder ID="plcStepImages" runat="server" EnableViewState="false" />
                                            </td>
                                        </tr>
                                    </asp:PlaceHolder>
                                    <tr class="CartStepBody">
                                        <td style="padding-top: 0px;">
                                            <asp:Panel ID="plcCartStep" runat="server" CssClass="CartStepPanel">
                                                <asp:Panel ID="pnlCartStepInner" runat="server" CssClass="CartStepInnerPanel" />
                                            </asp:Panel>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                    </table>--%>
                <div class="cont_payemet">
                    <asp:PlaceHolder ID="plcCheckoutProcess" runat="server" EnableViewState="false">
                        <asp:PlaceHolder ID="plcStepImages" runat="server" EnableViewState="false" />
                    </asp:PlaceHolder>
                    <asp:Panel ID="plcCartStep" runat="server" CssClass="CartStepPanel">
                        <asp:Panel ID="pnlCartStepInner" runat="server" CssClass="CartStepInnerPanel" />
                    </asp:Panel>
                    <div class="clr">
                    </div>
                    <asp:Panel ID="pnlStepButtons" runat="server">
                        <div class="cont_bgt_panier" style="visibility: visible">
                            
                                <cms:CMSButton ID="btnBack" runat="server" OnClick="btnBack_Click" CssClass="btn_pass_commande"
                                ValidationGroup="ButtonBack"  RenderScript="true" EnableViewState="false" />
                            
                                <cms:CMSButton ID="btnNext" runat="server" OnClick="btnNext_Click" CssClass="btn_pass_commande"
                                    ValidationGroup="ButtonNext" RenderScript="true" EnableViewState="false" />
                            
                        </div>
                    </asp:Panel>
                    <div class="clr">
                    </div>
                </div>
            </div>
            <div class="BlocDroitConso">
                <asp:Label ID="lbldroit" runat="server" Text="Label"></asp:Label>
            </div>
        </div>
    </div>
</asp:Panel>
