<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_Ecommerce_Pages_Tools_Configuration_ShippingExtension_ShippingExtension_CountryView"
    Theme="Default" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" CodeFile="ShippingExtension_CountryView.aspx.cs" %>

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
                    <ug:column source="CountryDisplayName" caption="Country" wrap="false" width="100%">
                    </ug:column>
                    <ug:column source="From" caption="From" wrap="false">
                    </ug:column>
                    <ug:column source="To" caption="To" wrap="false">
                    </ug:column>
                    <ug:column source="Shippers" caption="Shippers" wrap="false">
                    </ug:column>
                </GridColumns>
            </cms:UniGrid>
        </ContentTemplate>
    </cms:CMSUpdatePanel>
</asp:Content>
