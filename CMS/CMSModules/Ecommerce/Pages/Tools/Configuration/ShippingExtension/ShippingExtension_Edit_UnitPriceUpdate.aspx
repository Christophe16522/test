<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_Ecommerce_Pages_Tools_Configuration_ShippingExtension_ShippingExtension_Edit_UnitPriceUpdate"
    Theme="Default" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" CodeFile="ShippingExtension_Edit_UnitPriceUpdate.aspx.cs" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxControl" %>
<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" TagName="UniGrid" TagPrefix="cms" %>
<%@ Register Namespace="CMS.UIControls.UniGridConfig" TagPrefix="ug" Assembly="CMS.UIControls" %>
<%@ Register Src="~/CMSFormControls/Sites/SiteOrGlobalSelector.ascx" TagName="SiteSelector"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSWebParts/Viewers/Query/querydatagrid.ascx" TagName="Grid"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/UI/PageElements/HeaderActions.ascx" TagName="HeaderActions"
    TagPrefix="cms" %>
<asp:Content ID="cntActions" runat="server" ContentPlaceHolderID="plcActions">
    <div class="LeftAlign">
        <cms:HeaderActions ID="hdrActions" runat="server" IsLiveSite="false" />
    </div>
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="plcContent" runat="Server">
    <cms:FormSubmitButton runat="server" ID="btnOk" EnableViewState="false" ClientIDMode="Static"
        Enabled="true" />
    <div class="WebPartForm">
        <table class="EditingFormCategoryTableHeader" cellpadding="0" cellspacing="0">
            <tbody>
                <tr class="EditingFormCategoryRow">
                    <td class="EditingFormLeftBorder">
                        &nbsp;
                    </td>
                    <td colspan="2" class="EditingFormCategory" id="Accounts">
                        <cms:LocalizedLabel ID="lblUpdateTitle" runat="server" EnableViewState="false" Text="Update options"
                            DisplayColon="false" />
                    </td>
                    <td class="EditingFormRightBorder">
                        &nbsp;
                    </td>
                </tr>
            </tbody>
        </table>
        <div>
            <table class="EditingFormCategoryTableContent" border="0" cellpadding="0" cellspacing="0">
                <tbody>
                    <tr class="EditingFormRow">
                        <td class="EditingFormLeftBorder">
                            &nbsp;
                        </td>
                        <td class="EditingFormLabelCell" style="width: 400px;">
                            <asp:Panel runat="server" ID="pnlPriceChoice" GroupingText="Price to update">
                                <table>
                                    <tr>
                                        <td>
                                            <asp:RadioButton runat="server" ID="rdoUnitPrice" Text="Unit price" GroupName="base"
                                                ClientIDMode="Static" Checked="true" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <asp:RadioButton runat="server" ID="rdoShippingBase" Text="Shipping base" GroupName="base"
                                                ClientIDMode="Static" />
                                        </td>
                                    </tr>
                                </table>
                            </asp:Panel>
                        </td>
                        <td style="width: 25px;">
                        </td>
                        <td class="EditingFormValueCell" style="width: 400px;">
                            <asp:Panel ID="pnlModeChoice" runat="server" GroupingText="Update mode:">
                                <table>
                                    <tr>
                                        <td>
                                            <asp:RadioButton runat="server" ID="rdoAmount" Text="by amount of" GroupName="mode"
                                                CssClass="m" ClientIDMode="Static" Checked="true" />
                                        </td>
                                        <td>
                                            <asp:TextBox runat="server" ID="txtAmount" ClientIDMode="Static" CssClass="amount"></asp:TextBox>
                                            <ajaxControl:FilteredTextBoxExtender ID="txtAmount_FilteredTextBoxExtender" runat="server"
                                                Enabled="True" TargetControlID="txtAmount" FilterType="Numbers, Custom" ValidChars=".-">
                                            </ajaxControl:FilteredTextBoxExtender>
                                        </td>
                                        <td>
                                            <asp:Label runat="server" ID="lblAmount" ClientIDMode="Static" CssClass="amount">€</asp:Label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <asp:RadioButton runat="server" ID="rdoPercent" Text="by percentage of" GroupName="mode"
                                                CssClass="m" ClientIDMode="Static" />
                                        </td>
                                        <td>
                                            <asp:TextBox runat="server" ID="txtPercent" ClientIDMode="Static" CssClass="percent"></asp:TextBox>
                                        </td>
                                        <td>
                                            <asp:Label runat="server" ID="lblPercent" ClientIDMode="Static" CssClass="percent">%</asp:Label>
                                        </td>
                                    </tr>
                                </table>
                            </asp:Panel>
                        </td>
                        <td>
                            <asp:Button ID="testbutton" runat="server" Text="S" OnClick="btnOK_Click" ClientIDMode="Static"
                                Style="position: absolute; left: -20px; z-index: 2; opacity: 0; filter: alpha(opacity=0);" />
                            <ajaxControl:ConfirmButtonExtender runat="server" ID="cbeOK" TargetControlID="testbutton"
                                ConfirmText="Confirm Update ?">
                            </ajaxControl:ConfirmButtonExtender>
                        </td>
                        <td>
                        </td>
                        <td class="EditingFormRightBorder">
                            &nbsp;
                        </td>
                    </tr>
                </tbody>
            </table>
        </div>
        <table cellpadding="0" cellspacing="0">
            <tbody>
                <tr class="EditingFormFooterRow">
                    <td class="EditingFormLeftBorder">
                        &nbsp;
                    </td>
                    <td class="EditingFormLabelCell">
                        &nbsp;
                    </td>
                    <td class="EditingFormValueCell">
                        &nbsp;
                    </td>
                    <td class="EditingFormRightBorder">
                        &nbsp;
                    </td>
                </tr>
            </tbody>
        </table>
    </div>
    <div class="WebPartForm">
        <table class="EditingFormCategoryTableHeader" cellpadding="0" cellspacing="0">
            <tbody>
                <tr class="EditingFormCategoryRow">
                    <td class="EditingFormLeftBorder">
                        &nbsp;
                    </td>
                    <td colspan="2" class="EditingFormCategory" id="Td1">
                        <cms:LocalizedLabel ID="lblCountryTitle" runat="server" EnableViewState="false" Text="Countries"
                            DisplayColon="false" />
                    </td>
                    <td class="EditingFormRightBorder">
                        &nbsp;
                    </td>
                </tr>
            </tbody>
        </table>
        <div>
            <asp:UpdatePanel runat="server" ID="pnlAjax" UpdateMode="Always">
                <ContentTemplate>
                    <table class="EditingFormCategoryTableContent" border="0" cellpadding="0" cellspacing="0">
                        <tbody valign="top">
                            <tr class="EditingFormRow">
                                <td class="EditingFormLeftBorder">
                                    &nbsp;
                                </td>
                                <td class="EditingFormLabelCell" style="width: 400px;">
                                    <asp:Panel ID="pnlAvailable" runat="server" GroupingText="Available countries">
                                        <cms:UniGrid runat="server" ID="UniGridAvailable" ShortID="g" OrderBy="CountryDisplayName"
                                            Columns="CountryID, CountryDisplayName, CountryTwoLetterCode, CountryThreeLetterCode"
                                            IsLiveSite="false" EditActionUrl="Frameset.aspx?countryid={0}" ObjectType="cms.country"
                                            FilterLimit="1">
                                            <GridActions Parameters="CountryID">
                                                <ug:Action Name="add" Caption="$General.Add$" Icon="add.png" />
                                            </GridActions>
                                            <GridColumns>
                                                <ug:Column Source="CountryDisplayName" Caption="$Unigrid.Country.Columns.CountryDisplayName$"
                                                    Wrap="false" Localize="true">
                                                    <Filter Type="text" />
                                                </ug:Column>
                                                <ug:Column Source="UnitPrice" Caption="Unit Price" Wrap="false">
                                                </ug:Column>
                                                <ug:Column Source="ShippingBase" Caption="Shipping Base" Wrap="false">
                                                </ug:Column>
                                            </GridColumns>
                                            <GridOptions DisplayFilter="true" />
                                        </cms:UniGrid>
                                    </asp:Panel>
                                </td>
                                <td style="width: 25px;">
                                </td>
                                <td class="EditingFormValueCell" style="width: 400px;">
                                    <asp:Panel ID="Updated" runat="server" GroupingText="Countries to update">
                                        <cms:UniGrid runat="server" ID="UniGridUpdated" ShortID="h" OrderBy="CountryDisplayName"
                                            Columns="CountryID, CountryDisplayName, CountryTwoLetterCode, CountryThreeLetterCode"
                                            IsLiveSite="false" EditActionUrl="Frameset.aspx?countryid={0}" FilterLimit="1">
                                            <GridActions Parameters="CountryID">
                                                <ug:Action Name="Delete" Caption="$General.Delete$" Icon="delete.png" />
                                            </GridActions>
                                            <GridColumns>
                                                <ug:Column Source="CountryDisplayName" Caption="Country" Wrap="false" Localize="true">
                                                    <Filter Type="text" />
                                                </ug:Column>
                                                <ug:Column Source="UnitPrice" Caption="Unit Price" Wrap="false">
                                                </ug:Column>
                                                <ug:Column Source="ShippingBase" Caption="Shipping Base" Wrap="false">
                                                </ug:Column>
                                            </GridColumns>
                                            <GridOptions DisplayFilter="true" />
                                        </cms:UniGrid>
                                    </asp:Panel>
                                </td>
                                <td>
                                </td>
                                <td>
                                </td>
                                <td class="EditingFormRightBorder">
                                    &nbsp;
                                </td>
                            </tr>
                        </tbody>
                    </table>
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
    </div>
    <script type="text/javascript">
        jQuery(document).ready(function () {
            jQuery('.amount').hide();
            jQuery('.percent').hide();

            var radio1 = jQuery('#rdoAmount');
            if (radio1.is(':checked') == true) {
                jQuery('.amount').show();
            };
            if (radio1.is(':checked') == false) {
                jQuery('.percent').show();
            };

            jQuery('.m').bind('change', function () {
                var showOrHide = (jQuery(this).val() == 'rboui') ? true : false;
                //jQuery('.tvaBox').toggle(showOrHide);
                jQuery('.amount').slideToggle();
                jQuery('.percent').slideToggle();
            });

            jQuery('.MenuItemEdit').click(function () {
                //alert('clicked');
                jQuery('#testbutton').click();
            });
            /*
            jQuery('input:radio').bind('change', function () {
            var showOrHide = (jQuery(this).val() == 'rboui') ? true : false;
            //jQuery('.tvaBox').toggle(showOrHide);
            jQuery('.amount').slideToggle();
            jQuery('.percent').slideToggle();
            });
            */
        });
    </script>
</asp:Content>
