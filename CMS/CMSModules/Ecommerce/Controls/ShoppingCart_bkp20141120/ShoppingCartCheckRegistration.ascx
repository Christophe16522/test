<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_Ecommerce_Controls_ShoppingCart_ShoppingCartCheckRegistration"
    CodeFile="ShoppingCartCheckRegistration.ascx.cs" %>
<%@ Register Src="~/CMSModules/Membership/FormControls/Passwords/PasswordStrength.ascx"
    TagName="PasswordStrength" TagPrefix="cms" %>
<asp:Label ID="lblTitle" runat="server" CssClass="BlockTitle" EnableViewState="false"
    Visible="false" />
<div class="left_block_payement etape_2">
    <h4>
        <asp:Label ID="LabelAlreadyAccount" runat="server" Text="j'ai d�j� un compte"></asp:Label></h4>
    <asp:TextBox ID="txtLogin" runat="server" CssClass="champtexte" MaxLength="100" EnableViewState="false"
        ToolTip="adresse email" />
    <asp:RequiredFieldValidator ID="rfvTxtLogin" runat="server" ControlToValidate="txtLogin"
        ErrorMessage="*" EnableViewState="false" CssClass="leftRequired"></asp:RequiredFieldValidator>
    <ajaxToolkit:TextBoxWatermarkExtender ID="wmLogin" runat="server" TargetControlID="txtLogin"
        WatermarkText="adresse email">
    </ajaxToolkit:TextBoxWatermarkExtender>
    <asp:TextBox ID="txtMotDePasse" runat="server" TextMode="password" CssClass="champtexte clickClear"
        MaxLength="100" EnableViewState="false" ToolTip="mot de passe" />
    <asp:RequiredFieldValidator ID="rfvTxtMotDePasse" runat="server" ErrorMessage="*"
        ControlToValidate="txtMotDePasse" EnableViewState="false" CssClass="leftRequired"></asp:RequiredFieldValidator>
    <asp:Label ID="LabelErrorMessage" runat="server" CssClass="msgErreurLogin"></asp:Label>
    <asp:Button Text="&nbsp;" CssClass="btn_conect" runat="server" ID="BtnLogin" OnClick="BtnLogin_Click"
        CausesValidation="false" />
    <p>
        <asp:LinkButton ID="lbPasswd" runat="server" CssClass="linkPasswdRetrieval" CausesValidation="false"
            OnClick="lbPasswd_Click" Visible="true" />
    </p>
    <asp:Panel ID="PanelRetrievePassword" runat="server" CssClass="LoginPanelPasswordRetrieval"
        Visible="false">
        <%--DefaultButton="btnPasswordRetrieval"--%>
        <table>
            <tr>
                <td>
                    <asp:Label ID="Label1" runat="server" EnableViewState="false" AssociatedControlID="txtPasswordRetrieval"
                        CssClass="labelPasswdRetrieval" Visible="false" />
                </td>
            </tr>
            <tr>
                <td>
                    <cms:CMSTextBox ID="txtBoxPasswordRetrieval" runat="server" CssClass="champtexte" />
                    <ajaxToolkit:TextBoxWatermarkExtender ID="tbwmPwdRetrieval" runat="server" TargetControlID="txtBoxPasswordRetrieval"
                        WatermarkText="Adresse E-mail" />
                    <cms:CMSButton ID="btnPasswordRetrieval" runat="server" EnableViewState="false" CssClass="btn_envoyer"
                        CausesValidation="false" OnClick="btnPasswordRetrieval_Click" /><br />
                    <cms:CMSRequiredFieldValidator ID="CMSRequiredFieldValidator1" runat="server" ControlToValidate="txtBoxPasswordRetrieval"
                        EnableViewState="false" />
                </td>
            </tr>
        </table>
        <asp:Label ID="LabelResult" runat="server" Visible="false" CssClass="msgErreurLogin" EnableViewState="false" />
    </asp:Panel>
</div>
<div class="right_block_payement etape_2">
    <h4>
        <asp:Label ID="LabelNewCustomer" runat="server" Text="je suis un nouveau client"></asp:Label></h4>
    <asp:Label ID="lblError" runat="server" ForeColor="red" EnableViewState="false" />
    <%--Civilit�--%>
    <div class="cont_client">
        <div class="cont_post left">
            <div class="cont_drop_down nb1 left civilite">
                <asp:DropDownList runat="server" ID="ddlFrom" CssClass="select_personaliser" >
                </asp:DropDownList>
            </div>
            <%--Pr�nom --%>
            <asp:TextBox ID="txtFirstName" runat="server" CssClass="champtexte right threequarter" MaxLength="100"
                EnableViewState="false" ToolTip="Pr�nom" />
            <ajaxToolkit:TextBoxWatermarkExtender ID="wmFirstName" runat="server" TargetControlID="txtFirstName">
            </ajaxToolkit:TextBoxWatermarkExtender>
            <%--<asp:RequiredFieldValidator ID="rfvLastName" runat="server" ControlToValidate="txtLastName" Display="Dynamic" EnableViewState="false" CssClass="left" ErrorMessage="*"/>--%>
        </div>        
        <%--nom--%>
        <asp:TextBox ID="txtLastName" runat="server" CssClass="champtexte right" MaxLength="100"
            EnableViewState="false" ToolTip="Nom" />
        <ajaxToolkit:TextBoxWatermarkExtender ID="wmLastName" runat="server" TargetControlID="txtLastName">
        </ajaxToolkit:TextBoxWatermarkExtender>

        <%--<asp:RequiredFieldValidator ID="rfvFirstName" runat="server" ControlToValidate="txtFirstName" ErrorMessage="*" EnableViewState="false" CssClass="right" />--%>
        <div class="clr">
        </div>
        <div class="cont_post left">
            <%-- N�--%>
            <asp:TextBox runat="server" CssClass="champtexte left quarter" ID="txtnumero">
            </asp:TextBox>
            <ajaxToolkit:TextBoxWatermarkExtender ID="wmnumero" runat="server" TargetControlID="txtnumero"
                WatermarkText="Num" />
            <ajaxToolkit:FilteredTextBoxExtender ID="txtnumero_FilteredTextBoxExtender" runat="server"
                Enabled="True" FilterType="Numbers" TargetControlID="txtnumero">
            </ajaxToolkit:FilteredTextBoxExtender>
            <%-- Adresse 1 --%>
            <cms:ExtendedTextBox ID="txtadresse1" EnableEncoding="true" runat="server" CssClass="champtexte right threequarter"
                MaxLength="100" ToolTip="Rue" />
            <ajaxToolkit:TextBoxWatermarkExtender ID="wmadresse1" runat="server" TargetControlID="txtadresse1"
                WatermarkText="Rue" />
        </div>
        <%-- Adresse 2 --%>
        <cms:ExtendedTextBox ID="txtadresse2" EnableEncoding="true" runat="server" CssClass="champtexte right"
            MaxLength="100" ToolTip="Compl�ment d'adresse" />
        <ajaxToolkit:TextBoxWatermarkExtender ID="wmadresse2" runat="server" TargetControlID="txtadresse2"
            WatermarkText="Compl�ment d'adresse" />
        <div class="clr">
        </div>
        <div class="cont_post left">
            <%--CP--%>
            <asp:TextBox ID="txtCodePostale" runat="server" CssClass="champtexte left quarter" MaxLength="100"
                EnableViewState="false" ToolTip="Code Postal" />
            <ajaxToolkit:TextBoxWatermarkExtender ID="wmCodePostale" runat="server" TargetControlID="txtCodePostale">
            </ajaxToolkit:TextBoxWatermarkExtender>
            <%--<asp:RequiredFieldValidator ID="rfvTxtCodePostale" runat="server" ErrorMessage="*" ControlToValidate="txtCodePostale" Display="Dynamic" EnableViewState="false" CssClass="left"></asp:RequiredFieldValidator>--%>
            <%--Ville--%>
            <asp:TextBox ID="txtVille" runat="server" CssClass="champtexte right threequarter" MaxLength="100"
                EnableViewState="false" ToolTip="Ville" />
            <ajaxToolkit:TextBoxWatermarkExtender ID="wmVille" runat="server" TargetControlID="txtVille">
            </ajaxToolkit:TextBoxWatermarkExtender>
            <%--<asp:RequiredFieldValidator ID="rfvTxtVille" runat="server" ErrorMessage="*" ControlToValidate="txtVille" Display="Dynamic" EnableViewState="false" CssClass="right"></asp:RequiredFieldValidator>--%>
        </div>
        <%-- Pays--%>
        <div class="cont_drop_down nb1 right">
            <asp:DropDownList ID="ddlBillingCountry" runat="server" CssClass="select_personaliser"
                EnableViewState="true">
            </asp:DropDownList>
        </div>

        <div class="clr">
        </div>
                <%--Email--%>
        <asp:TextBox ID="txtEmailRegistration" runat="server" CssClass="champtexte left"
            MaxLength="100" EnableViewState="false" ToolTip="Adresse e-mail" Visible="true" />
        <ajaxToolkit:TextBoxWatermarkExtender ID="wmEmailRegistration" runat="server" TargetControlID="txtEmailRegistration">
        </ajaxToolkit:TextBoxWatermarkExtender>
        <%--<asp:RequiredFieldValidator ID="rfvEmail" runat="server" ControlToValidate="txtEmailRegistration" Display="Dynamic" EnableViewState="false" CssClass="left" ErrorMessage="*"/>--%>

        <%--Telephone--%>
        <asp:TextBox ID="txtTelephone" runat="server" CssClass="champtexte right"
            MaxLength="100" EnableViewState="false" ToolTip="Telephone" Visible="true" />
        <ajaxToolkit:TextBoxWatermarkExtender ID="wmTelephone" runat="server" TargetControlID="txtTelephone" WatermarkText ="Telephone">
        </ajaxToolkit:TextBoxWatermarkExtender>
        <%--<asp:RequiredFieldValidator ID="rfvEmail" runat="server" ControlToValidate="txtEmailRegistration" Display="Dynamic" EnableViewState="false" CssClass="left" ErrorMessage="*"/>--%>
        <div class="clr">
        </div>

        <%-- Mot de passe --%>
        <asp:TextBox ID="txtMotDePasseRegistration" runat="server" TextMode="password" CssClass="champtexte left clickClear"
            MaxLength="100" EnableViewState="true" ToolTip="mot de passe" />
        <%-- <asp:RequiredFieldValidator ID="rfvTxtMotDePasseRegistration" runat="server" ControlToValidate="txtMotDePasseRegistration" Display="Dynamic" EnableViewState="false" CssClass="left" ErrorMessage="*" />--%>
        <%-- Mot de passe de confirmation  --%>
        <%--<asp:RequiredFieldValidator ID="rfvTxtMotDePasseConfirmation" runat="server" ErrorMessage="*" ControlToValidate="txtMotDePasseConfirmation" Display="Dynamic" EnableViewState="false" CssClass="right"></asp:RequiredFieldValidator>--%>
        <asp:TextBox ID="txtMotDePasseConfirmation" runat="server" TextMode="password" CssClass="champtexte right clickClear"
            MaxLength="100" EnableViewState="true" ToolTip="confirmer mot de passe" />
        <asp:CompareValidator ID="CompareValidator1" runat="server" ErrorMessage="Mot de passe non identique"
            ControlToValidate="txtMotDePasseRegistration" ControlToCompare="txtMotDePasseConfirmation"
            EnableClientScript="False" Type="String"></asp:CompareValidator>
        <div class="clr">
        </div>
        <%--  soci�t�--%>
        <div class="cont_box_different">
            <label>
                <asp:Label ID="lblSociete" runat="server" Text="Vous �tes une soci�t�" CssClass="cont_box_different"></asp:Label>
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
            <%-- nom soci�t�--%>
            <cms:ExtendedTextBox ID="txtnomsociete" EnableEncoding="true" runat="server" CssClass="champtexte left"
                MaxLength="100" ToolTip="Nom soci�t�" />
            <ajaxToolkit:TextBoxWatermarkExtender ID="wmnomsociete" runat="server" TargetControlID="txtnomsociete"
                WatermarkText="Nom soci�t�" />
            <%-- <cms:CMSRequiredFieldValidator ID="rfvnomsociete" runat="server" ControlToValidate="txtnomsociete"
            Display="Dynamic" EnableViewState="false" CssClass="left" />--%>
            <%--  tva--%>
            <%-- <cms:CMSRequiredFieldValidator ID="rfvtva" runat="server" ControlToValidate="txtTva"
            Display="Dynamic" EnableViewState="false" CssClass="right" />--%>
             <asp:UpdatePanel ID="test" runat="server">
            <ContentTemplate>
            <cms:ExtendedTextBox ID="txtTva" EnableEncoding="true" runat="server" CssClass=" champtexte right"
                MaxLength="100" ToolTip="Tva" ontextchanged="txttva_TextChanged" AutoPostBack="true"/>
            <ajaxToolkit:TextBoxWatermarkExtender ID="wmtva" runat="server" TargetControlID="txtTva"
                WatermarkText="Tva" />
                <asp:Label ID="lblerror1" runat="server" ForeColor="red" EnableViewState="false" style="float:left" />
                </ContentTemplate>
                </asp:UpdatePanel>
        </div>
        <div class="clr">
        </div>
        <div class="cont_box_different">
            <label>
                <asp:CheckBox ID="chkShippingAddr" runat="server" Checked="false" AutoPostBack="false"
                    OnCheckedChanged="chkShippingAddr_CheckedChanged" ClientIDMode="Static" />
                <asp:Label ID="LabelAdresseDifference" runat="server" Text="Adresse de livraison diff�rente de l�adresse de facturation"></asp:Label>
            </label>
            <div class="clr">
            </div>
        </div>
        <div class="divShipping">
            <asp:PlaceHolder ID="plhShipping" runat="server" Visible="true">
                <asp:Label ID="lblErrorShipping" runat="server" ForeColor="red" EnableViewState="false" />
                <div class="clr">
                </div>
                <div class="cont_post left">
                    <%-- N� shipping--%>
                    <asp:TextBox runat="server" CssClass="champtexte left quarter" ID="txtnumeroshipping">
                    </asp:TextBox>
                    <ajaxToolkit:TextBoxWatermarkExtender ID="wmnumeroshipping" runat="server" TargetControlID="txtnumeroshipping"
                        WatermarkText="Numero" />
                    <ajaxToolkit:FilteredTextBoxExtender ID="txtnumeroshipping_FilteredTextBoxExtender"
                        runat="server" Enabled="True" FilterType="Numbers" TargetControlID="txtnumeroshipping">
                    </ajaxToolkit:FilteredTextBoxExtender>
                    <%-- Adresse 1 shipping --%>
                    <cms:ExtendedTextBox ID="txtadresse1shipping" EnableEncoding="true" runat="server"
                        CssClass=" champtexte right threequarter clickClear" MaxLength="100" ToolTip="Adresse 1" />
                    <ajaxToolkit:TextBoxWatermarkExtender ID="wmadresse1shipping" runat="server" TargetControlID="txtadresse1shipping"
                        WatermarkText="Adresse 1" />
                    <%--<cms:CMSRequiredFieldValidator ID="rfvadresse1shipping" runat="server" ControlToValidate="txtadresse1shipping"
                        Display="Dynamic" EnableViewState="false" CssClass="right" Text="*"/>--%>
                </div>
                <%-- Adresse 2 shipping --%>
                <cms:ExtendedTextBox ID="txtadresse2shipping" EnableEncoding="true" runat="server"
                    CssClass=" champtexte right clickClear" MaxLength="100" ToolTip="Adresse 2" />
                <ajaxToolkit:TextBoxWatermarkExtender ID="wmadresse2shipping" runat="server" TargetControlID="txtadresse2shipping"
                    WatermarkText="Adresse 2" />
                <%-- <cms:CMSRequiredFieldValidator ID="rfvadresse2shipping" runat="server" ControlToValidate="txtadresse2shipping"
                        Display="Dynamic" EnableViewState="false" CssClass="left" Text="*"/>     --%>
                <div class="clr">
                </div>
                <%--CP shipping--%>
                <div class="cont_post left">
                    <asp:TextBox ID="txtcpshipping" runat="server" CssClass="champtexte left" MaxLength="100"
                        EnableViewState="false" ToolTip="Code Postale" />
                    <ajaxToolkit:TextBoxWatermarkExtender ID="wmcpshipping" runat="server" TargetControlID="txtcpshipping">
                    </ajaxToolkit:TextBoxWatermarkExtender>
                    <%--<asp:RequiredFieldValidator ID="rfvTxtcpshipping" runat="server" ErrorMessage="*" ControlToValidate="txtcpshipping" CssClass="left"></asp:RequiredFieldValidator>--%>
                    <%-- Ville shipping--%>
                    <asp:TextBox ID="txtvilleshipping" runat="server" CssClass="champtexte right" MaxLength="100"
                        EnableViewState="false" ToolTip="Ville" />
                    <ajaxToolkit:TextBoxWatermarkExtender ID="wmvilleshipping" runat="server" TargetControlID="txtvilleshipping">
                    </ajaxToolkit:TextBoxWatermarkExtender>
                    <%-- <asp:RequiredFieldValidator ID="rfvTxtvilleshipping" runat="server" ErrorMessage="*" CssClass="right" ControlToValidate="txtvilleshipping"></asp:RequiredFieldValidator> --%>
                </div>
                <%-- pays shipping --%>
                <div class="cont_drop_down nb1 right">
                    <asp:DropDownList ID="ddlShippingCountry" runat="server" CssClass="select_personaliser"
                        EnableViewState="true">
                    </asp:DropDownList>
                </div>
                <div class="clr">
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
                <asp:Label ID="LabelAgree" runat="server" Text="Oui, j'accepte"></asp:Label>
                <br />
            </label>
            <a href="#block_condition" class="fancybox_condition_utilisation">
                <asp:Label ID="LabelCondition" runat="server" Text="les Conditions G�n�rales de Vente"></asp:Label></a>
            <div id="block_condition" style="display: none; padding: 20px">
                <%--   condition utilisation--%>
                <h1>
                    Conditions g�n�rales d&rsquo;offre et de vente</h1>
                <div>
                    de la S. P. R. L. Servranx &ndash; �ditions et laboratoires &ndash; 23-25 rue G.
                    Biot &ndash; 1050 Bruxelles &ndash; Belgique<br />
                    T�l. 00 32 26 49 18 40 &ndash; N&deg; d&rsquo;entreprise BE 0450.834.719<br />
                    &nbsp;<br />
                    &nbsp;<br />
                    <strong>1 &ndash; Droits (copyright&copy;)</strong><br />
                    Ce site est la propri�t� de la S.P.R.L.* Servranx en sa totalit�, ainsi que l&rsquo;ensemble
                    des droits aff�rents. Toute reproduction, int�grale ou partielle, est syst�matiquement
                    soumise � l&rsquo;autorisation des propri�taires. Toutefois, les liaisons de type
                    hypertextes vers le site sont autoris�es sans demandes sp�cifiques.<br />
                    <strong>2 &ndash; Illustrations</strong><br />
                    Les photographies illustrant les produits ne sont pas contractuelles. Si des erreurs
                    s&rsquo;y sont gliss�es, en aucun cas, la responsabilit� de la S.P.R.L. Servranx
                    ne pourra �tre engag�e.<br />
                    <strong>3 &ndash; Offres et ventes</strong><br />
                    a&nbsp;) Nos conditions d&rsquo;offre et de vente aux consommateurs sont soumises
                    � la loi du 6 avril 2010 relative aux pratiques du march� et � la protection de
                    consommateur (LPMC en abr�g�) ainsi que de ses arr�t�s d&rsquo;ex�cution, et des
                    arr�t�s vis�s � l&rsquo;article 139 de cette loi &ndash; Voir&nbsp;:&nbsp;<a href="http://www.economie.fgov.be/">http://www.economie.fgov.be</a><br />
                    b) Cette loi pr�cise entre autre que le consommateur&nbsp;b�n�ficie d&rsquo;un droit
                    de r�tractation de 14 jours calendrier � partir de la r�ception de la marchandise
                    command�e.<br />
                    c) � tout moment de la transaction, les prix indiqu�s sont nets, TVA incluse, hors
                    frais d&rsquo;envoi et de paiement.<br />
                    Lors de l&rsquo;achat et de la finalisation du paiement&nbsp;:<br />
                    &ndash; �tape 1 de 6&nbsp;: prix net et TVA indiqu�s s�par�ment + sous-total.<br />
                    &ndash; �tape 3 de 6&nbsp;: toute livraison hors de l&rsquo;Union europ�enne permet
                    de b�n�ficier de l&rsquo;exon�ration de la TVA.&nbsp;<br />
                    &ndash; �tape 5 de 6&nbsp;: r�capitulation comprenant tous les montants de la facture
                    finale&nbsp;: prix de la marchandise, prix du port ainsi que les diff�rents montants
                    de TVA applicables (6 et 21 %).<br />
                    Les frais d&rsquo;envoi sont calcul�s automatiquement et ajout�s � la facture. Chaque
                    modification ou ajout d&rsquo;un achat modifie le calcul des frais d&rsquo;envoi.<br />
                    d) Les commandes peuvent �tre modifi�es ou annul�es � tout moment avant la validation
                    finale faite par le consommateur.<br />
                    e)&nbsp;Les marchandises refus�es en vertu du droit de r�tractation du consommateur
                    doivent �tre retourn�es au vendeur avant remboursement. Elles doivent �tre retourn�es
                    au vendeur en parfait �tat, dans leur&nbsp;emballage d&rsquo;origine, sans la moindre
                    alt�ration ou le moindre dommage et ne peuvent porter la moindre marque ou mention
                    autre que les indications d&rsquo;origines voulues par le propri�taire de la marque
                    au moment de leur fabrication.&nbsp;Ces marchandises ne peuvent avoir �t� utilis�es.<br />
                    f) Le remboursement des marchandises refus�es en vertu du droit de r�tractation
                    du consommateur se fera au prix net indiqu� sur la facture d&rsquo;achat remise
                    au consommateur. Les frais de port ne sont pas rembours�s.<br />
                    g) Les co�ts de remboursement (transferts de fonds, virements de toute nature, �missions
                    de ch�que, etc.) sont aux frais du consommateur � l&rsquo;origine de la demande
                    dudit remboursement.<br />
                    h) Toute commande valid�e par le consommateur implique l&rsquo;accord entier et
                    inconditionnel du consommateur aux termes des&nbsp;<strong>conditions g�n�rales d&rsquo;offre
                        et de vente</strong>&nbsp;de la S. P. R. L. Servranx.<br />
                    <strong>4 &ndash; Commande</strong><br />
                    Les syst�mes d&rsquo;enregistrement automatique sont consid�r�s comme valant preuve
                    de la nature, du contenu et de la date de la commande. La S.P.R.L. Servranx confirme
                    la commande � l&rsquo;adresse �lectronique communiqu�e par le client. Les informations
                    �nonc�es par l&rsquo;acheteur lors de la prise de commande engagent celui-ci&nbsp;:
                    en cas d&rsquo;erreur dans le libell� des coordonn�es du destinataire, le vendeur
                    ne saurait �tre tenu responsable de l&rsquo;impossibilit� dans laquelle il pourrait
                    �tre de livrer le produit.<br />
                    <strong>5 &ndash; Disponibilit� des articles</strong><br />
                    La majorit� de nos produits propos�s sur le site commercial sont disponibles au
                    comptoir de vente dans nos bureaux.<br />
                    <strong>6 &ndash; Livraison</strong><br />
                    La S. P. R. L. Servranx s&rsquo;engage � remettre � son transporteur toutes les
                    r�f�rences command�es par l&rsquo;acheteur, et ce, dans les meilleurs d�lais. Ce
                    transporteur est responsable de la livraison � l&rsquo;adresse de livraison fournie
                    par l&rsquo;acheteur. De plus, en ce qui concerne les livraisons hors de l&rsquo;Union
                    europ�enne, le client s&rsquo;engage � r�gler toutes les taxes dues � l&rsquo;importation
                    de produits, droit de douane, taxe sur la valeur ajout�e, et toutes autres taxes
                    dues en vertu des lois applicables dans le pays de r�ception de la commande. La
                    livraison sera effectu�e par La Poste en port colis normal ou prioritaire, recommand�
                    ou non, ou par UPS (United Parcel Service) standard ou Expedited (U.S.A.) suivant
                    le choix du client et selon les disponibilit�s vers le pays de destination.<br />
                    Tout est mis en �uvre pour que les commandes arrivent en bon �tat et dans les temps
                    jug�s normaux. Les retards �ventuels du transporteur n&rsquo;engagent pas la responsabilit�
                    du vendeur et ne donnent pas droit � quelque remboursement que ce soit ni en dommages
                    ni en int�r�ts.<br />
                    Sont consid�r�s comme cas de force majeure d�chargeant le vendeur de son obligation
                    de livrer : la guerre, l&rsquo;�meute, l&rsquo;incendie, les gr�ves, les accidents
                    et l&rsquo;impossibilit� d&rsquo;�tre approvisionn�s.<br />
                    Les marchandises voyagent toujours aux risques et p�rils du destinataire. Celui-ci
                    doit v�rifier son colis � l&rsquo;arriv�e et dispose d&rsquo;un d�lai de 48 heures
                    pour faire d&rsquo;�ventuelles r�serves aupr�s du transporteur en cas de perte partielle
                    ou de d�gradation de notre envoi. Dans les cas d&rsquo;un produit technique, le
                    client veillera particuli�rement � v�rifier le bon fonctionnement de l&rsquo;appareil
                    livr�, � lire le mode d&rsquo;emploi fourni. En cas de d�fauts apparents, l&rsquo;acheteur
                    s&rsquo;engage � le signaler au vendeur dans les 4 jours ouvrables.<br />
                    Pour des raisons de disponibilit�, une commande peut �tre livr�e en plusieurs colis.
                    Le client ne r�gle alors qu&rsquo;une seule livraison. Si le client souhaite deux
                    lieux de livraison, il passe deux commandes, avec les frais de livraison li�s.<br />
                    <strong>7 &ndash; Prix</strong><br />
                    Les prix de tous les produits s&rsquo;entendent net, en euros, toutes taxes comprises,
                    hors frais de port. Ils sont r�visables � tout moment&nbsp;sans pr�avis. Les articles
                    sont factur�s en euros sur la base des prix indiqu�s sur le site au moment de la
                    commande. Une facture est jointe � toute commande. Si une �ventuelle disparit� apparaissait
                    dans un prix, l&#39;acheteur et le vendeur auront recours � la derni�re liste officielle
                    des prix publi�e par la maison sur son site. Le prix d�finitif de la commande inclut
                    les prix nets des marchandises, TVA incluse, ainsi que le prix, TVA incluse, de
                    tous les services aff�rents � l&rsquo;envoi desdites marchandises, manutentions
                    et emballages inclus.<br />
                    <strong>8 &ndash; TVA</strong><br />
                    La r�glementation est fix�e par l&rsquo;Union europ�enne.<br />
                    Exp�dition � destination d&rsquo;un pays de l&rsquo;Union europ�enne&nbsp;: (pour
                    tout envoi � l&rsquo;adresse de facturation) 1) Si le destinataire n&rsquo;est pas
                    assujetti � la TVA : facturation incluant la TVA belge. 2) Si le destinataire est
                    assujetti � la TVA : facturation hors TVA belge avec v�rification du num�ro d&rsquo;identification
                    (TVA Intracommunautaire) du destinataire. Exp�dition � destination d&rsquo;un pays
                    hors de l&rsquo;Union europ�enne&nbsp;: facturation exon�r�e de la TVA belge. &nbsp;<br />
                    <strong>9 &ndash; Paiement</strong><br />
                    Les articles sont payables au moment de la commande. Le paiement s&rsquo;effectue
                    par carte de cr�dit, comme propos� dans le module de commande en ligne. En ligne,
                    OGONE permet de r�gler via un serveur bancaire du groupe Bank Card Company BCC dans
                    un environnement s�curis�. Le num�ro de carte bancaire du client est dirig� vers
                    les serveurs de la banque, le r�glement s&rsquo;effectue donc directement � une
                    banque dans un environnement s�curis� sans passer par le serveur de la boutique.
                    Les num�ros de carte ne sont connus que du partenaire bancaire (BCC), jamais du
                    vendeur. La commande valid�e par le client ne sera consid�r�e effective que lorsque
                    les centres de paiements bancaires concern�s auront donn� leur accord (Ogone, Paypal,
                    etc.). En cas de refus desdits centres, la commande sera automatiquement annul�e
                    et le client sera pr�venu par message �lectronique. Par ailleurs, la S.P.R.L Servranx
                    se r�serve le droit de refuser toute commande d&rsquo;un client avec lequel il existerait
                    un litige. Le remboursement d&#39;un paiement fait sur le site OGONE&nbsp;ou Paypal
                    occasionnant des frais importants, celui-ci ne sera pas accept� si la raison est
                    une erreur imputable au client.<br />
                    <strong>10 &ndash; Litiges</strong><br />
                    Le pr�sent contrat est soumis au droit belge. La S.P.R.L Servranx ne peut �tre tenue
                    pour responsable des dommages de quelque nature qu&rsquo;ils soient, tant mat�riels
                    qu&rsquo;immat�riels ou corporels, qui pourraient r�sulter de mauvaises manipulations
                    des produits commercialis�s. Il en est de m�me pour les �ventuelles modifications
                    des produits du chef de nos fournisseurs. La responsabilit� de la S.P.R.L Servranx
                    sera, en tout �tat de cause, limit�e au montant de la commande et ne saurait �tre
                    mise en cause pour de simples erreurs qui auraient pu subsister malgr� toutes les
                    pr�cautions prises dans la pr�sentation des produits. En cas de difficult�s dans
                    l&rsquo;application du pr�sent contrat, l&rsquo;acheteur a la possibilit�, avant
                    toute action en justice, de rechercher une solution amiable notamment avec l&rsquo;aide
                    d&rsquo;une association professionnelle de la branche, d&rsquo;une association de
                    consommateurs ou de tout autre conseil de son choix. Il est rappel� que la recherche
                    de la solution amiable n&rsquo;interrompt pas le &laquo; bref d�lai &raquo; de la
                    garantie l�gale, ni la dur�e de la garantie contractuelle. Il est rappel� qu&rsquo;en
                    r�gle g�n�rale et sous r�serve de l&rsquo;appr�ciation des tribunaux, le respect
                    des dispositions du pr�sent contrat relatives � la garantie contractuelle suppose
                    que l&rsquo;acheteur honore ses engagements financiers envers le vendeur. Les r�clamations
                    ou contestations, qui doivent �tre introduites par �crit dans les huit jours ouvrables,
                    seront toujours re�ues avec une bienveillance attentive, la bonne foi �tant toujours
                    pr�sum�e chez celui qui prend la peine d&rsquo;exposer ces situations. En cas de
                    litige, le client s&rsquo;adressera par priorit� � l&rsquo;entreprise pour obtenir
                    une solution amiable. (Service clients &ndash; T�l. : + 32 2 649 18 40, du lundi
                    au vendredi inclus ou courriel (E-mail) � info@servranx.com). � d�faut, le tribunal
                    de commerce de Bruxelles est seul comp�tent, quels que soient le lieu de livraison
                    et le mode de paiement accept�. Dans tous les cas, la S.P.R.L Servranx ne pourra
                    �tre tenu pour responsable pour non-respect des dispositions r�glementaires et l�gislatives
                    en vigueur dans le pays de r�ception, la responsabilit� de la S.P.R.L Servranx est
                    syst�matiquement limit�e � la valeur du produit mis en cause, valeur � la date de
                    la vente. Si l&rsquo;acheteur s&rsquo;adresse aux tribunaux, il doit le faire dans
                    un &laquo; bref d�lai &raquo; � compter de la d�couverte du d�faut cach�.<br />
                    <strong>11 &ndash; Informations l�gales</strong><br />
                    Le renseignement des informations nominatives collect�es aux fins de la vente �
                    distance est obligatoire, ces informations �tant indispensables pour le traitement
                    et l&rsquo;acheminement des commandes, l&rsquo;�tablissement des factures et contrats
                    de garantie. Le d�faut de renseignement entra�ne la non-validation de la commande.
                    Conform�ment � la loi, le client dispose d&rsquo;un droit d&rsquo;acc�s, de modification,
                    de rectification et de suppression des donn�es qui le concernent, qu&rsquo;il peut
                    exercer aupr�s de la S.P.R.L Servranx.<br />
                    <strong>12 &ndash;&nbsp; La commande</strong><br />
                    Le fait de passer commande � la S.P.R.L Servranx implique l&rsquo;accord sur tous
                    les points ci-dessus.<br />
                    * S.P.R.L. : soci�t� priv�e � responsabilit� limit�e.</div>
                <%--   fin--%>
            </div>
        </div>
        <div class="cont_post right cont_check_condition">
            <label>
                <asp:CheckBox ID="chkNewsletter" runat="server" Checked="false" />
                <asp:Label ID="LabelReceive" runat="server" Text="Oui, je veux recevoir par e-mail des infos sur les"></asp:Label>
                <br />
                <asp:Label ID="LabelLastArticle" runat="server" Text="d�rni�res articles."></asp:Label></label>
        </div>
        <div class="clr">
        </div>
        <asp:Button Text="&nbsp;" CssClass="btn_continued right" runat="server" ID="BtnCreatNewAccount"
            OnClick="BtnCreatNewAccount_Click" CausesValidation="false" />
        <p>
            <asp:Label ID="LabelErrorMessageCreation" runat="server" CssClass="msgErreurLogin"></asp:Label>
        </p>
    </div>
</div>
<div class="clr">
</div>
<div class="clr">
</div>
<%--<div class="BlocDroitConso"> Droits du consommateur : d�lai de r�tractation l�gal de 14 jours.Nos prix sont TVA incluse hors frais d'envoi et de paiement. 
Pour conna�tre nos conditions d'offres et de ventes, <a href="#">cliquez ici</a>. </div>--%>
<div class="BlockContent" style="display: none">
    <%--Sign In--%>
    <asp:PlaceHolder ID="plcAccount" runat="server">
        <table>
            <tr>
                <td colspan="3">
                    <asp:RadioButton ID="radSignIn" runat="server" GroupName="RadButtons" Checked="true" />
                </td>
            </tr>
            <tr>
                <td>
                    <table id="tblSignIn">
                        <tr>
                            <td rowspan="4" style="width: 25px;">
                                &nbsp;
                            </td>
                            <td class="FieldLabel">
                                <asp:Label ID="lblUsername" AssociatedControlID="txtUsername" runat="server" EnableViewState="false" />
                            </td>
                            <td>
                                <cms:CMSTextBox ID="txtUsername" runat="server" CssClass="TextBoxField" MaxLength="100"
                                    EnableViewState="false" />
                                <asp:Label ID="lblMark1" runat="server" EnableViewState="false" />
                            </td>
                        </tr>
                        <tr>
                            <td class="FieldLabel">
                                <asp:Label ID="lblPsswd1" AssociatedControlID="txtPsswd1" runat="server" EnableViewState="false" />
                            </td>
                            <td>
                                <cms:CMSTextBox ID="txtPsswd1" runat="server" TextMode="password" CssClass="TextBoxField"
                                    MaxLength="100" EnableViewState="false" />
                                <asp:Label ID="lblMark2" runat="server" EnableViewState="false" />
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:LinkButton ID="lnkPasswdRetrieval" runat="server" EnableViewState="false" />
                            </td>
                        </tr>
                        <tr>
                            <td colspan="2">
                                <asp:Panel ID="pnlPasswdRetrieval" runat="server" CssClass="LoginPanelPasswordRetrieval">
                                    <table cellpadding="0" cellspacing="0">
                                        <tr>
                                            <td>
                                                <asp:Label ID="lblPasswdRetrieval" AssociatedControlID="txtPasswordRetrieval" runat="server"
                                                    EnableViewState="false" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <cms:CMSTextBox ID="txtPasswordRetrieval" runat="server" EnableViewState="false"
                                                    MaxLength="100" />
                                                <asp:Label ID="lblMark3" runat="server" EnableViewState="false" />
                                                <cms:CMSButton ID="btnPasswdRetrieval" runat="server" ValidationGroup="PsswdRetrieval"
                                                    CssClass="ButtonSendPassword" EnableViewState="false" />
                                                <br />
                                                <cms:CMSRequiredFieldValidator ID="rqValue" runat="server" ControlToValidate="txtPasswordRetrieval"
                                                    ValidationGroup="PsswdRetrieval" EnableViewState="false" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <asp:PlaceHolder ID="plcResult" Visible="false" runat="server" EnableViewState="false">
                                                    <asp:Label ID="lblResult" runat="server" EnableViewState="false" CssClass="InfoLabel" />
                                                </asp:PlaceHolder>
                                                <asp:PlaceHolder ID="plcErrorResult" Visible="false" runat="server" EnableViewState="false">
                                                    <asp:Label ID="lblErrorResult" runat="server" EnableViewState="false" CssClass="ErrorLabel" />
                                                </asp:PlaceHolder>
                                            </td>
                                        </tr>
                                    </table>
                                </asp:Panel>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
            <%--END: Sign In--%>
            <%--New registration--%>
            <tr>
                <td colspan="3">
                    <asp:RadioButton ID="radNewReg" runat="server" GroupName="RadButtons" />
                </td>
            </tr>
            <tr>
                <td>
                    <table id="tblRegistration">
                        <tr>
                            <td rowspan="4" style="width: 25px;">
                                &nbsp;
                            </td>
                            <td class="FieldLabel">
                                <asp:Label ID="lblFirstName1" AssociatedControlID="txtFirstName1" runat="server"
                                    EnableViewState="false" />
                            </td>
                            <td>
                                <cms:ExtendedTextBox ID="txtFirstName1" runat="server" CssClass="TextBoxField" MaxLength="100"
                                    EnableViewState="false" />
                                <asp:Label ID="lblMark4" runat="server" EnableViewState="false" />
                            </td>
                        </tr>
                        <tr>
                            <td class="FieldLabel">
                                <asp:Label ID="lblLastName1" runat="server" AssociatedControlID="txtLastName1" EnableViewState="false" />
                            </td>
                            <td>
                                <cms:ExtendedTextBox ID="txtLastName1" runat="server" CssClass="TextBoxField" MaxLength="100"
                                    EnableViewState="false" />
                                <asp:Label ID="lblMark5" runat="server" EnableViewState="false" />
                            </td>
                        </tr>
                        <tr>
                            <td class="FieldLabel">
                                <asp:Label ID="lblEmail2" runat="server" AssociatedControlID="txtEmail2" EnableViewState="false" />
                            </td>
                            <td>
                                <cms:CMSTextBox ID="txtEmail2" runat="server" CssClass="TextBoxField" MaxLength="100"
                                    EnableViewState="false" />
                                <asp:Label ID="lblMark6" runat="server" EnableViewState="false" />
                                <asp:Label ID="lblEmail2Err" runat="server" EnableViewState="false" Visible="false"
                                    CssClass="LineErrorLabel" />
                            </td>
                        </tr>
                        <tr>
                            <td class="FieldLabel">
                                <asp:Label ID="lblCorporateBody" AssociatedControlID="chkCorporateBody" runat="server"
                                    EnableViewState="false" />
                            </td>
                            <td>
                                <asp:CheckBox runat="server" ID="chkCorporateBody" AutoPostBack="true" OnCheckedChanged="chkCorporateBody_CheckChanged" />
                            </td>
                        </tr>
                        <asp:Panel runat="server" ID="pnlCompanyAccount1" Visible="false">
                            <tr>
                                <td>
                                    &nbsp;
                                </td>
                                <td class="FieldLabel">
                                    <cms:LocalizedLabel ID="lblCompany1" AssociatedControlID="txtCompany1" runat="server"
                                        EnableViewState="false" ResourceString="com.companyname" DisplayColon="true" />
                                </td>
                                <td>
                                    <cms:ExtendedTextBox ID="txtCompany1" runat="server" CssClass="TextBoxField" MaxLength="100"
                                        EnableViewState="false" /><asp:Label ID="lblMark15" runat="server" EnableViewState="false"
                                            Visible="false" />
                                </td>
                            </tr>
                            <asp:PlaceHolder ID="plcOrganizationID" runat="server" Visible="false" EnableViewState="false">
                                <tr>
                                    <td>
                                        &nbsp;
                                    </td>
                                    <td class="FieldLabel">
                                        <asp:Label ID="lblOrganizationID" AssociatedControlID="txtOrganizationID" runat="server"
                                            EnableViewState="false" />
                                    </td>
                                    <td>
                                        <cms:ExtendedTextBox ID="txtOrganizationID" runat="server" CssClass="TextBoxField"
                                            MaxLength="50" EnableViewState="false" />
                                        <asp:Label ID="lblMark16" runat="server" EnableViewState="false" Visible="false" />
                                    </td>
                                </tr>
                            </asp:PlaceHolder>
                            <asp:PlaceHolder ID="plcTaxRegistrationID" runat="server" Visible="false" EnableViewState="false">
                                <tr>
                                    <td>
                                        &nbsp;
                                    </td>
                                    <td class="FieldLabel">
                                        <asp:Label ID="lblTaxRegistrationID" AssociatedControlID="txtTaxRegistrationID" runat="server"
                                            EnableViewState="false" />
                                    </td>
                                    <td>
                                        <cms:ExtendedTextBox ID="txtTaxRegistrationID" runat="server" CssClass="TextBoxField"
                                            MaxLength="50" EnableViewState="false" />
                                        <asp:Label ID="lblMark17" runat="server" EnableViewState="false" Visible="false" />
                                    </td>
                                </tr>
                            </asp:PlaceHolder>
                        </asp:Panel>
                        <tr>
                            <td>
                                &nbsp;
                            </td>
                            <td class="FieldLabel FieldLabelTop">
                                <asp:Label ID="lblPsswd2" runat="server" AssociatedControlID="passStrength" EnableViewState="false" />
                            </td>
                            <td>
                                <cms:PasswordStrength runat="server" ID="passStrength" />
                                <div>
                                    <asp:Label ID="lblPsswdErr" runat="server" Visible="false" EnableViewState="false"
                                        CssClass="LineErrorLabel" />
                                </div>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                &nbsp;
                            </td>
                            <td class="FieldLabel">
                                <asp:Label ID="lblConfirmPsswd" AssociatedControlID="txtConfirmPsswd" runat="server"
                                    EnableViewState="false" />
                            </td>
                            <td>
                                <cms:CMSTextBox ID="txtConfirmPsswd" runat="server" TextMode="password" CssClass="TextBoxField"
                                    MaxLength="100" EnableViewState="false" />
                                <asp:Label ID="lblMark8" runat="server" EnableViewState="false" />
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
            <%--END: New registration--%>
            <%--Continue as anonymous--%>
            <asp:PlaceHolder ID="plhAnonymous" runat="server" Visible="false">
                <tr>
                    <td colspan="3">
                        <asp:RadioButton ID="radAnonymous" runat="server" GroupName="RadButtons" />
                    </td>
                </tr>
                <tr>
                    <td>
                        <table id="tblAnonymous">
                            <tr>
                                <td rowspan="5" style="width: 25px;">
                                    &nbsp;
                                </td>
                                <td class="FieldLabel">
                                    <asp:Label ID="lblFirstName2" AssociatedControlID="txtFirstName2" runat="server"
                                        EnableViewState="false" />
                                </td>
                                <td>
                                    <cms:ExtendedTextBox ID="txtFirstName2" runat="server" CssClass="TextBoxField" MaxLength="100"
                                        EnableViewState="false" />
                                    <asp:Label ID="lblMark9" runat="server" EnableViewState="false" />
                                </td>
                            </tr>
                            <tr>
                                <td class="FieldLabel">
                                    <asp:Label ID="lblLastName2" AssociatedControlID="txtLastName2" runat="server" EnableViewState="false" />
                                </td>
                                <td>
                                    <cms:ExtendedTextBox ID="txtLastName2" runat="server" CssClass="TextBoxField" MaxLength="100"
                                        EnableViewState="false" />
                                    <asp:Label ID="lblMark10" runat="server" EnableViewState="false" />
                                </td>
                            </tr>
                            <tr>
                                <td class="FieldLabel">
                                    <cms:LocalizedLabel ID="lblEmail3" AssociatedControlID="txtEmail3" runat="server"
                                        EnableViewState="false" ResourceString="general.email" DisplayColon="true" />
                                </td>
                                <td>
                                    <cms:ExtendedTextBox ID="txtEmail3" runat="server" CssClass="TextBoxField" MaxLength="100"
                                        EnableViewState="false" />
                                    <asp:Label ID="lblMark11" runat="server" EnableViewState="false" />
                                    <asp:Label ID="lblEmail3Err" runat="server" EnableViewState="false" Visible="false"
                                        CssClass="LineErrorLabel" />
                                </td>
                            </tr>
                            <tr>
                                <td class="FieldLabel">
                                    <cms:LocalizedLabel ID="lblCorporateBody2" AssociatedControlID="chkCorporateBody2"
                                        runat="server" EnableViewState="false" ResourceString="shoppingcartcheckregistration.companyrequired" />
                                </td>
                                <td>
                                    <asp:CheckBox runat="server" ID="chkCorporateBody2" AutoPostBack="true" OnCheckedChanged="chkCorporateBody2_CheckChanged" />
                                </td>
                            </tr>
                            <asp:PlaceHolder runat="server" ID="plcCompanyAccount3" Visible="false">
                                <tr>
                                    <td class="FieldLabel">
                                        <cms:LocalizedLabel ID="lblCompany2" AssociatedControlID="txtCompany2" runat="server"
                                            EnableViewState="false" ResourceString="com.companyname" DisplayColon="true" />
                                    </td>
                                    <td>
                                        <cms:ExtendedTextBox ID="txtCompany2" runat="server" CssClass="TextBoxField" MaxLength="100"
                                            EnableViewState="false" />
                                        <asp:Label ID="lblMark21" runat="server" EnableViewState="false" Visible="false" />
                                    </td>
                                </tr>
                                <asp:PlaceHolder ID="plcOrganizationID2" runat="server" Visible="false" EnableViewState="false">
                                    <tr>
                                        <td>
                                            &nbsp;
                                        </td>
                                        <td class="FieldLabel">
                                            <asp:Label ID="lblOrganizationID2" AssociatedControlID="txtOrganizationID2" runat="server"
                                                EnableViewState="false" />
                                        </td>
                                        <td>
                                            <cms:ExtendedTextBox ID="txtOrganizationID2" runat="server" CssClass="TextBoxField"
                                                MaxLength="50" EnableViewState="false" />
                                            <asp:Label ID="lblMark22" runat="server" EnableViewState="false" Visible="false" />
                                        </td>
                                    </tr>
                                </asp:PlaceHolder>
                                <asp:PlaceHolder ID="plcTaxRegistrationID2" runat="server" Visible="false" EnableViewState="false">
                                    <tr>
                                        <td>
                                            &nbsp;
                                        </td>
                                        <td class="FieldLabel">
                                            <asp:Label ID="lblTaxRegistrationID2" AssociatedControlID="txtTaxRegistrationID2"
                                                runat="server" EnableViewState="false" />
                                        </td>
                                        <td>
                                            <cms:ExtendedTextBox ID="txtTaxRegistrationID2" runat="server" CssClass="TextBoxField"
                                                MaxLength="50" EnableViewState="false" />
                                            <asp:Label ID="lblMark23" runat="server" EnableViewState="false" Visible="false" />
                                        </td>
                                    </tr>
                                </asp:PlaceHolder>
                            </asp:PlaceHolder>
                        </table>
                    </td>
                </tr>
            </asp:PlaceHolder>
            <%--END: Continue as anonymous--%>
        </table>
    </asp:PlaceHolder>
    <asp:PlaceHolder runat="server" ID="plcEditCustomer" EnableViewState="false">
        <table>
            <tr>
                <td class="FieldLabel" style="width: 170px">
                    <asp:Label ID="lblEditFirst" AssociatedControlID="txtEditFirst" runat="server" EnableViewState="false" />
                </td>
                <td>
                    <cms:ExtendedTextBox ID="txtEditFirst" runat="server" CssClass="TextBoxField" MaxLength="100" /><asp:Label
                        ID="lblMark12" runat="server" EnableViewState="false" />
                </td>
            </tr>
            <tr>
                <td class="FieldLabel" style="width: 170px">
                    <asp:Label ID="lblEditLast" AssociatedControlID="txtEditLast" runat="server" EnableViewState="false" />
                </td>
                <td>
                    <cms:ExtendedTextBox ID="txtEditLast" runat="server" CssClass="TextBoxField" MaxLength="100" /><asp:Label
                        ID="lblMark13" runat="server" EnableViewState="false" />
                </td>
            </tr>
            <tr>
                <td class="FieldLabel" style="width: 170px">
                    <cms:LocalizedLabel ID="lblEditEmail" AssociatedControlID="txtEditEmail" runat="server"
                        EnableViewState="false" ResourceString="general.email" DisplayColon="true" />
                </td>
                <td>
                    <cms:ExtendedTextBox ID="txtEditEmail" runat="server" CssClass="TextBoxField" MaxLength="100" /><asp:Label
                        ID="lblMark14" runat="server" EnableViewState="false" />
                    <asp:Label ID="lblEditEmailError" runat="server" EnableViewState="false" Visible="false"
                        CssClass="LineErrorLabel" />
                </td>
            </tr>
            <tr>
                <td class="FieldLabel" style="width: 170px">
                    <asp:Label ID="lblEditCorpBody" AssociatedControlID="chkEditCorpBody" runat="server"
                        EnableViewState="false" />
                </td>
                <td>
                    <asp:CheckBox runat="server" ID="chkEditCorpBody" AutoPostBack="true" OnCheckedChanged="chkEditCorpBody_CheckChanged" />
                </td>
            </tr>
            <asp:Panel runat="server" ID="pnlCompanyAccount2" Visible="false">
                <tr>
                    <td class="FieldLabel" style="width: 170px">
                        <asp:Label ID="lblEditCompany" AssociatedControlID="txtEditCompany" runat="server"
                            EnableViewState="false" />
                    </td>
                    <td>
                        <cms:ExtendedTextBox ID="txtEditCompany" runat="server" CssClass="TextBoxField" MaxLength="100" /><asp:Label
                            ID="lblMark18" runat="server" EnableViewState="false" />
                    </td>
                </tr>
                <asp:PlaceHolder ID="plcEditOrgID" runat="server" Visible="false" EnableViewState="false">
                    <tr>
                        <td class="FieldLabel" style="width: 170px">
                            <asp:Label ID="lblEditOrgID" AssociatedControlID="txtEditOrgID" runat="server" EnableViewState="false" />
                        </td>
                        <td>
                            <cms:ExtendedTextBox ID="txtEditOrgID" runat="server" CssClass="TextBoxField" MaxLength="50" /><asp:Label
                                ID="lblMark19" runat="server" EnableViewState="false" />
                        </td>
                    </tr>
                </asp:PlaceHolder>
                <asp:PlaceHolder ID="plcEditTaxRegID" runat="server" Visible="false" EnableViewState="false">
                    <tr>
                        <td class="FieldLabel" style="width: 170px">
                            <asp:Label ID="lblEditTaxRegID" AssociatedControlID="txtEditTaxRegID" runat="server"
                                EnableViewState="false" />
                        </td>
                        <td>
                            <cms:ExtendedTextBox ID="txtEditTaxRegID" runat="server" CssClass="TextBoxField"
                                MaxLength="50" /><asp:Label ID="lblMark20" runat="server" EnableViewState="false" />
                        </td>
                    </tr>
                </asp:PlaceHolder>
            </asp:Panel>
        </table>
    </asp:PlaceHolder>
</div>
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
