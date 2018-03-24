<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="CMSModules_Widgets_FormControls_SelectZoneType" CodeFile="SelectZoneType.ascx.cs" %>
<script type="text/javascript">
    jQuery(document).ready(function () {
        var radioBtns = jQuery("#<%=rblOptions.ClientID %> [type=radio]");
        var warningPanel = jQuery(".zone-type-change-warning");

        // Display the warning at the top of the dialog
        warningPanel.prependTo("#divContent");

        // Show when zone type is changed
        radioBtns
            .on("change", function () {
                warningPanel.show();
            });


        jQuery(".alert-close")
            .click(function () {
                warningPanel.hide();
            });
    });
</script>

<cms:CMSRadioButtonList ID="rblOptions" runat="server" RepeatDirection="Vertical" />

<!--Warning panel-->
<div class="zone-type-change-warning alert-dismissable alert-warning alert">
    <span class="alert-icon">
        <i class="icon-exclamation-triangle"></i>
        <span class="sr-only">Warning</span>
    </span>
    <div class="alert-label">
        <%=ResHelper.GetString("widgets.zonetypechangewarning")%>
    </div>
    <span class="alert-close" >
        <i class="close icon-modal-close"></i>
        <span class="sr-only">Close</span>
    </span>
</div>
