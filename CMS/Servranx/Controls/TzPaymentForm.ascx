<%@ Control Language="C#" AutoEventWireup="true" CodeFile="TzPaymentForm.ascx.cs"
    Inherits="Servranx_Controls_TzPaymentForm" %>

<script type="text/javascript">
    function doAction() {
        var val = document.forms[0].action = "http://www.google.fr";
        document.forms[0].submit();
        document.forms[0].method = "POST";
        alert(val);
    }

    function setValues() {
        var order = document.getElementById("<%= fake.ClientID %>").value;
        var amount = document.getElementById("<%= fakeAmount.ClientID %>").value;
        var sha = document.getElementById("<%= fakeSHA.ClientID %>").value;
        var language = document.getElementById("<%= fakeLanguage.ClientID %>").value;

        document.getElementById("orderID").value = order;
        document.getElementById("amount").value = amount;
        document.getElementById("SHASign").value = sha;
        document.getElementById("language").value = language;

        var btnFake = document.getElementById("<%= btnFake.ClientID %>");
        btnFake.click();
    }
    
    
</script>


<input type="hidden" name="fake" id="fake" runat="server" />
<input type="hidden" name="fakeAmount" id="fakeAmount" runat="server" />
<input type="hidden" name="fakeSHA" id="fakeSHA" runat="server" />
<input type="hidden" name="fakeLanguage" id="fakeLanguage" runat="server" />

<input type="button" name="btnFake" id="btnFake" runat="server" style="display:none;"  onserverclick="btnFakeClick" />

<div>
    <input type="hidden" name="PSPID" value="wazosa" />
    <input type="hidden" name="orderID" id="orderID" />
    <input type="hidden" name="amount" id="amount" />
    <input type="hidden" name="currency" value="EUR" />
    <input type="hidden" name="language" id="language" />
    <input type="hidden" name="CN" value="" />
    <input type="hidden" name="EMAIL" value="" />
    <input type="hidden" name="ownerZIP" value="" />
    <input type="hidden" name="owneraddress" value="" />
    <input type="hidden" name="ownercty" value="" />
    <input type="hidden" name="ownertown" value="" />
    <input type="hidden" name="ownertelno" value="" />
    <!-- vérification avant le paiement : voir: Sécurité : vérification avant le paiement -->
    <input type="hidden" name="SHASign" id="SHASign" />
    <!-- apparence et impression: voir Apparence de la page de paiement -->
    <input type="hidden" name="TITLE" value="" />
    <input type="hidden" name="BGCOLOR" value="" />
    <input type="hidden" name="TXTCOLOR" value="" />
    <input type="hidden" name="TBLBGCOLOR" value="" />
    <input type="hidden" name="TBLTXTCOLOR" value="" />
    <input type="hidden" name="BUTTONBGCOLOR" value="" />
    <input type="hidden" name="BUTTONTXTCOLOR" value="" />
    <input type="hidden" name="LOGO" value="" />
    <input type="hidden" name="FONTTYPE" value="" />
    <!--redirection après la transaction : voir Feedback au client sur la transaction -->
    <input type="hidden" name="accepturl" value="" />
    <input type="hidden" name="declineurl" value="" />
    <input type="hidden" name="exceptionurl" value="" />
    <input type="hidden" name="cancelurl" value="" />
</div>
<div>
    <asp:Button ID="btnSubmit" runat="server" Text="paiment" CssClass="ButtonGreenLevel3"  Visible="false"
       OnClientClick="setValues();" PostBackUrl="https://secure.ogone.com/ncol/test/orderstandard.asp"   />
    <%--<input type="submit" value="Test" title="tttt" class="ButtonGreenLevel3" onclick="doAction();" />--%>
</div>
