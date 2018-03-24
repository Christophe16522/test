<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_Ecommerce_Pages_Tools_Configuration_StoreSettings_StoreSettings_FreeBundle"
    Theme="Default" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" CodeFile="StoreSettings_FreeBundle.aspx.cs" %>

<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" TagName="UniGrid" TagPrefix="cms" %>
<%@ Register Namespace="CMS.UIControls.UniGridConfig" TagPrefix="ug" Assembly="CMS.UIControls" %>
<asp:Content ID="Content1" ContentPlaceHolderID="plcContent" runat="Server">
    <div class="PageContent">
        <cms:CMSUpdatePanel ID="pnlUpdate" runat="server">
            <ContentTemplate>
                <cms:UniGrid runat="server" ID="StoreSettings_FreeBundleList" GridName="StoreSettings_FreeBundle/StoreSettings_FreeBundleList.xml"
                    IsLiveSite="false" />
            </ContentTemplate>
        </cms:CMSUpdatePanel>
    </div>
</asp:Content>
