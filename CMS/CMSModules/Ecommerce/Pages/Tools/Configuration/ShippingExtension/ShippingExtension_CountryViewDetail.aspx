<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_Ecommerce_Pages_Tools_Configuration_ShippingExtension_ShippingExtension_CountryViewDetail"
    Theme="Default" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" CodeFile="ShippingExtension_CountryViewDetail.aspx.cs" %>

<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" TagName="UniGrid" TagPrefix="cms" %>
<%@ Register Namespace="CMS.UIControls.UniGridConfig" TagPrefix="ug" Assembly="CMS.UIControls" %>
<%@ Register Src="~/CMSFormControls/System/LocalizableTextBox.ascx" TagName="LocalizableTextBox"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/Ecommerce/Controls/UI/PriceSelector.ascx" TagName="PriceSelector"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/AdminControls/Controls/MetaFiles/File.ascx" TagPrefix="cms"
    TagName="File" %>
<%@ Register Src="~/CMSFormControls/System/CodeName.ascx" TagName="CodeName" TagPrefix="cms" %>
<asp:Content ID="Content1" ContentPlaceHolderID="plcContent" runat="Server">
    <cms:CMSUpdatePanel ID="CMSUpdatePanel2" runat="server">
        <ContentTemplate>
            <cms:UniGrid ID="CountryGrid" runat="server">
                <GridActions>
                    <ug:action name="Details" caption="Details" icon="View.png" />
                </GridActions>
                <GridColumns>
                    <ug:column source="ShippingOptionDisplayName" caption="Shipper" wrap="false" width="100%">
                    </ug:column>
                    <ug:column source="Processing" caption="Processing" wrap="false">
                    </ug:column>
                    <ug:column source="UnitPrice" caption="Unit Price" wrap="false">
                    </ug:column>
                    <ug:column source="ShippingBase" caption="Base Price" wrap="false">
                    </ug:column>
                    <ug:column source="Enabled" caption="Enabled" wrap="false">
                    </ug:column>
                </GridColumns>
            </cms:UniGrid>
        </ContentTemplate>
    </cms:CMSUpdatePanel>
</asp:Content>
