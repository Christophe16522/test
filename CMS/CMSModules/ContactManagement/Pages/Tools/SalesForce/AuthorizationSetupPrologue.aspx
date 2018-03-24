<%@ Page Language="C#" AutoEventWireup="true" CodeFile="AuthorizationSetupPrologue.aspx.cs" Inherits="CMSModules_ContactManagement_Pages_Tools_SalesForce_AuthorizationSetupPrologue" MasterPageFile="~/CMSMasterPages/UI/Dialogs/ModalDialogPage.master" EnableEventValidation="false" Theme="Default" %>
<asp:Content ID="MainContent" ContentPlaceHolderID="plcContent" runat="Server" EnableViewState="false">
	<asp:Panel ID="MainPanel" runat="server" EnableViewState="false">
	    <cms:AlertLabel runat="server" ID="ConfirmationElement" AlertType="Confirmation" Style="display:none;" />
	    <cms:AlertLabel runat="server" ID="ErrorElement" AlertType="Error" Style="display:none;" />
		<p id="ProgressElement"><%= GetString("sf.authorizationprologue.progress") %></p>
		<script type="text/javascript">

		    jQuery(document).ready(function () {
		        var continueButtonId = "<%= ContinueButton.ClientID %>";
		        var authorizationSetupUrl = "<%= AuthorizationSetupUrl %>";
		        var authorizationSetupHandlerUrl = "<%= AuthorizationSetupHandlerUrl %>";

			    jQuery.ajax(authorizationSetupHandlerUrl, {
			        dataType: "jsonp",
			        timeout: 5000,
			        complete: function (request, status) {
			            jQuery("#ProgressElement").hide();
			            if (status == "success") {
			                jQuery("#<%= ConfirmationElement.ClientID %>").show();
			                window.setTimeout(function () {
			                    window.location.href = authorizationSetupUrl;
			                }, 1000);
			            }
			            else {
			                jQuery("#<%= ErrorElement.ClientID %>").show();
			                jQuery("#" + continueButtonId).removeClass("Hidden").click(function (e) {
			                    window.location.href = authorizationSetupUrl;
			                    e.preventDefault();
			                });

			            }
			        }
			    });
			});
	
		</script>
	</asp:Panel>
</asp:Content>
<asp:Content ID="FooterContent" ContentPlaceHolderID="plcFooter" runat="server">
	<div id="FooterPanel" runat="server" class="FloatRight">
		<cms:LocalizedButton ID="ContinueButton" runat="server" ButtonStyle="Primary" CssClass="Hidden" EnableViewState="False" ResourceString="general.continue" />
	</div>
</asp:Content>