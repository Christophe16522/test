<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_Ecommerce_Controls_ShoppingCart_ShoppingCartCheckRegistrationNew1"
    CodeFile="ShoppingCartCheckRegistrationNew.ascx.cs" %>
<%@ Register Src="~/CMSModules/Membership/FormControls/Passwords/PasswordStrength.ascx"
    TagName="PasswordStrength" TagPrefix="cms" %>

<asp:Label ID="lblTitle" runat="server" CssClass="BlockTitle" EnableViewState="false" />
<div class="adresspage">
    <asp:Panel ID="Panel1" runat="server" CssClass="member" DefaultButton="btnSignIn">
        <h2>
            <asp:Label runat="server" ID="lblSignInTitle" Text="J’ai déjà un compte chez Porte d'Orient"></asp:Label></h2>
        <div class="field-item">
            <div class="label">
                <asp:Label ID="lblUsername" AssociatedControlID="txtUsername" runat="server" EnableViewState="false" /><asp:Label
                    ID="lblMark1" runat="server" EnableViewState="false" />
            </div>
            <cms:ExtendedTextBox ID="txtUsername" runat="server" CssClass="input_text" MaxLength="100"
                EnableViewState="false" ValidationGroup="SignIn" />
            <asp:Label ID="errorusername" runat="server" EnableViewState="false" Visible="False"
                CssClass="ErrorLabel" />
            <%--<asp:RequiredFieldValidator runat="server" ValidationGroup="SignIn" ControlToValidate="txtUsername" SetFocusOnError="True" ErrorMessage="Email est obligatoire" CssClass="ErrorLabel">*</asp:RequiredFieldValidator>--%>
        </div>
        <div class="field-item">
            <div class="label">
                <asp:Label ID="lblPsswd1" AssociatedControlID="txtPsswd1" runat="server" EnableViewState="false" /><asp:Label
                    ID="lblMark2" runat="server" EnableViewState="false" />
            </div>
            <cms:CMSTextBox ID="txtPsswd1" runat="server" TextMode="password" CssClass="input_text"
                MaxLength="100" EnableViewState="false" ValidationGroup="SignIn" />
        </div>
        <asp:Button runat="server" Text="Se connecter" CssClass="submit" ID="btnSignIn" OnClick="OnBtnSignInClick"
            ValidationGroup="SignIn" />
        <asp:UpdatePanel ID="uPanelMain" runat="server">
            <ContentTemplate>
                <div class="zone_mdp">
                    <asp:LinkButton ID="lnkPasswdRetrieval" runat="server" EnableViewState="false" />
                    <asp:LinkButton ID="lnkSignout" runat="server" EnableViewState="false" Visible="False" />
                    <asp:Panel ID="pnlPasswdRetrieval" runat="server" CssClass="LoginPanelPasswordRetrieval">
                        <div class="text-mpd">
                            <%= ResHelper.LocalizeString("{$Texte_MotDePasseOublier$}")%></div>
                        <div class="field-item">
                            <div class="label">
                                <asp:Label ID="lblPasswdRetrieval" AssociatedControlID="txtPasswordRetrieval" runat="server"
                                    EnableViewState="false" />
                            </div>
                            <cms:CMSTextBox ID="txtPasswordRetrieval" CssClass="input_text" runat="server" EnableViewState="false"
                                MaxLength="100" />
                        </div>
                        <asp:Label ID="lblMark3" runat="server" EnableViewState="false" />
                        <cms:CMSButton ID="btnPasswdRetrieval" CssClass="submit" runat="server" ValidationGroup="PsswdRetrieval"
                            EnableViewState="false" />
                        <div class="txt_errormdp">
                            <cms:CMSRequiredFieldValidator ID="rqValue" runat="server" ControlToValidate="txtPasswordRetrieval"
                                ValidationGroup="PsswdRetrieval" EnableViewState="false" />
                        </div>
                        <div class="txt_errormdp">
                            <asp:PlaceHolder ID="plcResult" Visible="false" runat="server" EnableViewState="false">
                                <asp:Label ID="lblResult" runat="server" EnableViewState="false" CssClass="InfoLabel" />
                            </asp:PlaceHolder>
                        </div>
                        <div class="txt_errormdp">
                            <asp:PlaceHolder ID="plcErrorResult" Visible="false" runat="server" EnableViewState="false">
                                <asp:Label ID="lblErrorResult" runat="server" EnableViewState="false" CssClass="ErrorLabel" />
                            </asp:PlaceHolder>
                        </div>
                    </asp:Panel>
                </div>
                <div class="field-item errorField">
                    <asp:Label ID="lblSignInresult" runat="server" EnableViewState="false" Visible="False"
                        CssClass="ErrorLabel" />
                    <%--<asp:ValidationSummary runat="server" ValidationGroup="SignIn" DisplayMode="SingleParagraph"/>--%>
                </div>
            </ContentTemplate>
        </asp:UpdatePanel>
        <div class="strong-points">
            <ul>
                <li>Livraison rapide</li>
                <li>Protection du client</li>
                <li>Retour sous 30 jours</li>
                <li>Paiement sécurisé par le protocole SSL</li>
            </ul>
        </div>
        <%--
        <div class="payment-methods">
            <ul>
                <li>Payment methods</li>
                <li><asp:Label runat="server" ID="lblPaymentMethod"></asp:Label></li>
            </ul>
        </div>--%>
    </asp:Panel>
    <asp:Panel ID="Panel2" runat="server" CssClass="nouveau-client" DefaultButton="btnSubmit">
        <h2>
            <asp:Label runat="server" ID="lblCustomerInfoTitle" Text="Je suis un nouveau client"></asp:Label>
        </h2>
        <div class="field-item">
            <div class="label">
                Civilité*
            </div>
            <asp:RadioButton CssClass="radio_fem" runat="server" GroupName="Civilite" ID="rdbF"
                Checked="True" Text="Madame / Mademoiselle" />
            <asp:RadioButton runat="server" GroupName="Civilite" ID="rdbM" Text="Monsieur" />
        </div>
        <div class="field-item">
            <div class="label">
                <asp:Label ID="lblFirstName1" AssociatedControlID="txtFirstName1" runat="server"
                    EnableViewState="false" />*
            </div>
            <cms:ExtendedTextBox ID="txtFirstName1" runat="server" CssClass="input_text" MaxLength="100"
                EnableViewState="false" />
            <asp:Label ID="lblMark4" runat="server" EnableViewState="false" />
            <div class="validitem">
                <asp:CustomValidator ID="CustomValidator1" runat="server" ControlToValidate="txtFirstName1"
                    ValidateEmptyText="true" ClientValidationFunction="Validate"></asp:CustomValidator>
            </div>
        </div>
        <div class="field-item">
            <div class="label">
                <asp:Label ID="lblLastName1" runat="server" AssociatedControlID="txtLastName1" EnableViewState="false" />
            </div>
            <cms:ExtendedTextBox ID="txtLastName1" runat="server" CssClass="input_text" MaxLength="100"
                EnableViewState="false" />
            <asp:Label ID="lblMark5" runat="server" EnableViewState="false" />
        </div>
        <div class="field-item">
            Adresse de facturation</div>
        <div class="field-item">
            <div class="label">
                <asp:Label ID="lblBillingAddrLine" runat="server" CssClass="ContentLabel" EnableViewState="false"
                    AssociatedControlID="txtBillingAddr1" />*
            </div>
            <cms:ExtendedTextBox ID="txtBillingAddr1" runat="server" CssClass="street input_text"
                MaxLength="100" />
            
            <asp:Label ID="lblMark21" runat="server" EnableViewState="false" />
            <cms:ExtendedTextBox ID="txtBillingNumber" runat="server" CssClass="streetnumber input_text"
                MaxLength="5" />
         <div class="validitem">
            <asp:CustomValidator ID="CustomValidator2" runat="server" ControlToValidate="txtBillingAddr1"
                    ValidateEmptyText="true" ClientValidationFunction="Validate"></asp:CustomValidator>
                <asp:CustomValidator ID="CustomValidator3" runat="server" ControlToValidate="txtBillingNumber"
                    ValidateEmptyText="true" ClientValidationFunction="Validate" ></asp:CustomValidator>
                 
            </div>
        </div>
        <div class="field-item">
            <div class="label">
                <asp:Label ID="lblBillingAddrLine2" runat="server" CssClass="HiddenLabel" EnableViewState="false"
                    AssociatedControlID="txtBillingAddr2" />
            </div>
            <cms:ExtendedTextBox ID="txtBillingAddr2" runat="server" CssClass="input_text" MaxLength="100"
                EnableViewState="false" />
        </div>
        <div class="field-item">
            <div class="label">
                <asp:Label ID="lblBillingZip" runat="server" CssClass="ContentLabel" EnableViewState="false"
                    AssociatedControlID="txtBillingZip" />*
            </div>
            <cms:ExtendedTextBox ID="txtBillingZip" runat="server" CssClass="input_text" MaxLength="20" /><asp:Label
                ID="lblMark19" runat="server" EnableViewState="false" />
                <div class="validitem">
                <asp:CustomValidator ID="CustomValidator4"
                    runat="server" ControlToValidate="txtBillingZip" ValidateEmptyText="true" ClientValidationFunction="Validate"></asp:CustomValidator></div>
        </div>
        <div class="field-item">
            <div class="label">
                <asp:Label ID="lblBillingCity" runat="server" CssClass="ContentLabel" EnableViewState="false"
                    AssociatedControlID="txtBillingCity" />*
            </div>
            <cms:ExtendedTextBox ID="txtBillingCity" runat="server" CssClass="input_text" MaxLength="100" />
            <asp:Label ID="lblMark22" runat="server" EnableViewState="false" />
            <div class="validitem">
                <asp:CustomValidator ID="CustomValidator5" runat="server" ControlToValidate="txtBillingCity"
                    ValidateEmptyText="true" ClientValidationFunction="Validate"></asp:CustomValidator></div>
        </div>
        <div class="field-item">
            <div class="label">
                <asp:Label ID="lblEmail2" runat="server" AssociatedControlID="txtEmail2" EnableViewState="false" />*
            </div>
            <cms:CMSTextBox ID="txtEmail2" runat="server" CssClass="input_text" MaxLength="100"
                EnableViewState="false" />
            <asp:Label ID="lblMark6" runat="server" EnableViewState="false" />
            <asp:Label ID="lblEmail2Err" runat="server" EnableViewState="false" Visible="false"
                CssClass="LineErrorLabel" />
            <div class="validitem">
                <asp:CustomValidator ID="CustomValidator6" runat="server" ControlToValidate="txtEmail2"
                    ValidateEmptyText="true" ClientValidationFunction="Validate"></asp:CustomValidator></div>
        </div>
        <asp:PlaceHolder runat="server" ID="plcPassword">
            <div class="field-item">
                <div class="label">
                    <asp:Label ID="lblPsswd2" runat="server" AssociatedControlID="passStrength" EnableViewState="false" />*
                </div>
                <cms:PasswordStrength runat="server" ID="passStrength" TextBoxClass="input_text"
                    />
                <asp:Label ID="lblPsswdErr" runat="server" Visible="false" EnableViewState="false"
                    CssClass="ErrorLabel" />
            </div>
            <div class="field-item">
                <div class="label">
                    <asp:Label ID="lblConfirmPsswd" AssociatedControlID="txtConfirmPsswd" runat="server"
                        EnableViewState="false" />
                </div>
                <cms:CMSTextBox ID="txtConfirmPsswd" runat="server" TextMode="password" CssClass="input_text"
                    MaxLength="100" EnableViewState="false" />
                <asp:Label ID="lblMark8" runat="server" EnableViewState="false" />
            </div>
        </asp:PlaceHolder>
        <div class="field-item">
            <div class="label">
                Adresse de facturation et adresse de livraison identiques</div>
            <asp:RadioButton runat="server" GroupName="SameAddress" ID="rdbSameAddress" Text="Oui"
                AutoPostBack="false" Checked="True" /><%--OnCheckedChanged="OnRdbSameAddressCheckedChange"--%>
            <asp:RadioButton runat="server" GroupName="SameAddress" ID="rdbDifferentAddress"
                Text="Non" AutoPostBack="false" /><%--OnCheckedChanged="OnRdbDifferentAddressCheckedChange"--%>
        </div>
        <asp:Panel ID="pnlShippingAdress" runat="server" Style="display: block">
            <div class="field-item">
                Adresse de Livraison</div>
            <div class="field-item">
                <div class="label">
                    <asp:Label ID="lblShippingAddr1" runat="server" CssClass="ContentLabel" EnableViewState="false"
                        AssociatedControlID="txtShippingAddr1" />*
                </div>
                <cms:ExtendedTextBox ID="txtShippingAddr1" runat="server" CssClass="street input_text"
                    MaxLength="100" />
                
                <asp:Label ID="lblMarkShipAddr1" runat="server" EnableViewState="false" />
                <cms:ExtendedTextBox ID="txtShippingNumber" runat="server" CssClass="streetnumber input_text"
                    MaxLength="5" />
                <div class="validitem">
                    <asp:CustomValidator ID="CustomValidator8" runat="server" ControlToValidate="txtShippingNumber"
                        ValidateEmptyText="true" ClientValidationFunction="Validate" Enabled="false"></asp:CustomValidator>
                      <asp:CustomValidator ID="CustomValidator7" runat="server" ControlToValidate="txtShippingAddr1"
                        ValidateEmptyText="true" ClientValidationFunction="Validate" Enabled="false"></asp:CustomValidator>   
                        </div>
            </div>
            <div class="field-item">
                <div class="label">
                    <asp:Label ID="lblShippingAddr2" runat="server" CssClass="HiddenLabel" EnableViewState="false"
                        AssociatedControlID="txtShippingAddr2" />
                </div>
                <cms:ExtendedTextBox ID="txtShippingAddr2" runat="server" CssClass="input_text" MaxLength="100"
                    EnableViewState="false" />
            </div>
            <div class="field-item">
                <div class="label">
                    <asp:Label ID="lblShippingZip" runat="server" CssClass="ContentLabel" EnableViewState="false"
                        AssociatedControlID="txtShippingZip" />*
                </div>
                <cms:ExtendedTextBox ID="txtShippingZip" runat="server" CssClass="input_text" MaxLength="20" />
                <div class="validitem">
                    <asp:CustomValidator ID="CustomValidator9" runat="server" ControlToValidate="txtShippingZip"
                        ValidateEmptyText="true" ClientValidationFunction="Validate" Enabled="false"></asp:CustomValidator></div>
                <asp:Label ID="lblMarkShipZip" runat="server" EnableViewState="false" />
            </div>
            <div class="field-item">
                <div class="label">
                    <asp:Label ID="lblShippingCity" runat="server" CssClass="ContentLabel" EnableViewState="false"
                        AssociatedControlID="txtShippingCity" />*
                </div>
                <cms:ExtendedTextBox ID="txtShippingCity" runat="server" CssClass="input_text" MaxLength="100" />
                <div class="validitem">
                    <asp:CustomValidator ID="CustomValidator10" runat="server" ControlToValidate="txtShippingCity"
                        ValidateEmptyText="true" ClientValidationFunction="Validate" Enabled="false"></asp:CustomValidator></div>
                <asp:Label ID="lblMarkShipCity" runat="server" EnableViewState="false" />
            </div>
        </asp:Panel>
        <div class="field-item hide">
            <asp:Label ID="lblCorporateBody" AssociatedControlID="chkCorporateBody" runat="server"
                EnableViewState="false" />
            <asp:CheckBox runat="server" ID="chkCorporateBody" AutoPostBack="true" OnCheckedChanged="chkCorporateBody_CheckChanged" />
            <asp:Panel runat="server" ID="pnlCompanyAccount1" Visible="false">
                <asp:Label ID="lblCompany1" AssociatedControlID="txtCompany1" runat="server" EnableViewState="false" />
                <cms:ExtendedTextBox ID="txtCompany1" runat="server" CssClass="input_text" MaxLength="100"
                    EnableViewState="false" /><asp:Label ID="lblMark15" runat="server" EnableViewState="false"
                        Visible="false" />
                <asp:PlaceHolder ID="plcOrganizationID" runat="server" Visible="false" EnableViewState="false">
                    <asp:Label ID="lblOrganizationID" AssociatedControlID="txtOrganizationID" runat="server"
                        EnableViewState="false" />
                    <cms:ExtendedTextBox ID="txtOrganizationID" runat="server" CssClass="input_text"
                        MaxLength="50" EnableViewState="false" />
                    <asp:Label ID="lblMark16" runat="server" EnableViewState="false" Visible="false" />
                </asp:PlaceHolder>
                <asp:PlaceHolder ID="plcTaxRegistrationID" runat="server" Visible="false" EnableViewState="false">
                    <asp:Label ID="lblTaxRegistrationID" AssociatedControlID="txtTaxRegistrationID" runat="server"
                        EnableViewState="false" />
                    <cms:ExtendedTextBox ID="txtTaxRegistrationID" runat="server" CssClass="input_text"
                        MaxLength="50" EnableViewState="false" />
                    <asp:Label ID="lblMark17" runat="server" EnableViewState="false" Visible="false" />
                </asp:PlaceHolder>
            </asp:Panel>
        </div>
        <div class="field-item">
            <div class="label">
                &nbsp;</div>
            <div class="checkboxes">
                <ul>
                    <li>
                        <asp:CheckBox runat="server" ID="chkAcceptCGV" CssClass="checkbox" />
                        <asp:Label runat="server" ID="lblAcceptCGV" Text="Oui, j’accepte les Conditions Générales de Vente "></asp:Label>
                    </li>
                    <li>
                        <asp:CheckBox runat="server" ID="chkSubscribe" CssClass="checkbox" />
                        <asp:Label runat="server" ID="lblSubscribe">
                            Oui, je veux recevoir par e-mail des infos sur les dernières tendances. Désinscription possible à tout moment.
                        </asp:Label>
                    </li>
                </ul>
            </div>
        </div>
        <div class="field-item">
            <asp:Button runat="server" Text="Continuer" CssClass="submit" ID="btnSubmit" OnClick="OnBtnSubmitClick" />
        </div>
        <div class="field-item errorField">
            <asp:Label ID="lblError" runat="server" EnableViewState="false" Visible="false" CssClass="ErrorLabel" />
        </div>
    </asp:Panel>
    <div class="how-find">
        <div class="label">
            Comment avez-vous connu Porte d'orient ?
        </div>
        <asp:DropDownList runat="server" ID="ddlFrom" CssClass="how_find_us">
            <asp:ListItem Selected="True" Value="">---Choisissez---</asp:ListItem>
            <asp:ListItem Value="FRIENDS"> Recommandation d'un ami / connaissance </asp:ListItem>
            <asp:ListItem Value="SEARCH_ENGINES"> Google / autre moteur de recherche </asp:ListItem>
            <asp:ListItem Value="BANNER_LINKS"> Bannière / lien sur un autre site </asp:ListItem>
            <asp:ListItem Value="NEWSLETTER"> Annonce dans une newsletter </asp:ListItem>
            <asp:ListItem Value="TV"> TV </asp:ListItem>
            <asp:ListItem Value="MAGAZINE"> Magazines </asp:ListItem>
            <asp:ListItem Value="OTHER"> Autres </asp:ListItem>
        </asp:DropDownList>
    </div>
</div>
<%--<script type="text/javascript">
    //<![CDATA[

    /* Shows and hides tables with forms*/
    function showElemAdv(id, show) {
        var style = document.getElementById(id).style;
        if (show == "y") {
            style.display = 'block';
            var myVal1 = document.getElementById('<%= CustomValidator7.ClientID %>');
            var myVal2 = document.getElementById('<%= CustomValidator8.ClientID %>');
            var myVal3 = document.getElementById('<%= CustomValidator9.ClientID %>');
            var myVal4 = document.getElementById('<%= CustomValidator10.ClientID %>');
            ValidatorEnable(myVal1, true);
            ValidatorEnable(myVal2, true);
            ValidatorEnable(myVal3, true);
            ValidatorEnable(myVal4, true);

        }
        else {
            style.display = 'none';
            var myVal1 = document.getElementById('<%= CustomValidator7.ClientID %>');
            var myVal2 = document.getElementById('<%= CustomValidator8.ClientID %>');
            var myVal3 = document.getElementById('<%= CustomValidator9.ClientID %>');
            var myVal4 = document.getElementById('<%= CustomValidator10.ClientID %>');
            ValidatorEnable(myVal1, false);
            ValidatorEnable(myVal2, false);
            ValidatorEnable(myVal3, false);
            ValidatorEnable(myVal4, false);

        }

        //style.display = (style.display == 'block') ? 'none' : 'block';
        return false;
    }
    /* Validation*/
    function Validate(sender, args) {
        if (document.getElementById(sender.controltovalidate).style.display != 'none') {
            if (document.getElementById(sender.controltovalidate).value != "") {
                args.IsValid = true;
                document.getElementById(sender.controltovalidate).style.borderColor = '#CCCCCC';
            } else {
                args.IsValid = false;
                document.getElementById(sender.controltovalidate).style.borderColor = 'red';


            }
        }

    }
    /**Enable validation
    function fnEnableDisableValidator(varValidatorID) {

    var radioButtons = document.getElementsByName('<%=rdbSameAddress%>');
    if (radioButtons.checked = true)
    ValidatorEnable(document.getElementById(varValidatorID), false);
    else
    ValidatorEnable(document.getElementById(varValidatorID), true);
    }**/

//]]>
</script>--%>
