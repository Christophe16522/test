<%@ Page Language="C#" Debug="true" AutoEventWireup="true" CodeFile="ShippingExtension_AddCountry.aspx.cs"
    MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Theme="Default" Inherits="CMSModules_Ecommerce_Pages_Tools_Configuration_ShippingExtension_ShippingExtension_AddCountry" %>

<%@ Register Src="~/CMSAdminControls/UI/UniSelector/UniSelector.ascx" TagName="UniSelector"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" TagName="UniGrid" TagPrefix="cms" %>
<%@ Register Namespace="CMS.UIControls.UniGridConfig" TagPrefix="ug" Assembly="CMS.UIControls" %>
<%@ Register Src="~/CMSFormControls/Sites/SiteOrGlobalSelector.ascx" TagName="SiteSelector"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSWebParts/Viewers/Query/querydatagrid.ascx" TagName="Grid"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/UI/PageElements/HeaderActions.ascx" TagName="HeaderActions"
    TagPrefix="cms" %>
<asp:Content ID="cntActions" runat="server" ContentPlaceHolderID="plcActions">
    <cms:CMSUpdatePanel ID="pnlActons" runat="server">
        <ContentTemplate>
            <div class="LeftAlign">
                <cms:HeaderActions ID="hdrActions" runat="server" IsLiveSite="false" />
            </div>
            <div class="ClearBoth">
            </div>
        </ContentTemplate>
    </cms:CMSUpdatePanel>
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="plcContent" runat="Server">
    <cms:CMSUpdatePanel ID="pnlUpdate" runat="server">
        <ContentTemplate>
            <cms:UniGrid runat="server" ID="UniGrid" ShortID="g" OrderBy="CountryDisplayName"
                Columns="CountryID, CountryDisplayName, CountryTwoLetterCode, CountryThreeLetterCode"
                IsLiveSite="false" EditActionUrl="Frameset.aspx?countryid={0}" ObjectType="cms.country">
                <GridActions Parameters="CountryID">
                    <ug:Action name="add" caption="$General.Add$" icon="add.png" />
                </GridActions>
                <GridColumns>
                    <ug:Column Source="CountryDisplayName" Caption="$Unigrid.Country.Columns.CountryDisplayName$"
                        Wrap="false" Localize="true">
                        <Filter Type="text" />
                    </ug:Column>
                    <ug:Column Source="CountryTwoLetterCode" Caption="$Unigrid.Country.Columns.CountryTwoLetterCode$"
                        Wrap="false">
                        <Filter Type="text" />
                    </ug:Column>
                    <ug:Column Source="CountryThreeLetterCode" Caption="$Unigrid.Country.Columns.CountryThreeLetterCode$"
                        Wrap="false">
                        <Filter Type="text" />
                    </ug:Column>
                    <ug:Column Width="100%" />
                </GridColumns>
                <GridOptions DisplayFilter="true" />
            </cms:UniGrid>
        </ContentTemplate>
    </cms:CMSUpdatePanel>
</asp:Content>
