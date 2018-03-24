<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_PortalEngine_UI_PageTemplates_Scopes_PageTemplateScopes_List"
    MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Title="Page Template Edit - Scopes list"
    Theme="Default" CodeFile="PageTemplateScopes_List.aspx.cs" %>

<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" TagName="UniGrid" TagPrefix="cms" %>
<%@ Register Src="~/CMSFormControls/Sites/SiteSelector.ascx" TagName="SiteSelector"
    TagPrefix="cms" %>
<asp:Content ID="cntHeader" ContentPlaceHolderID="plcSiteSelector" runat="server">
    <div class="form-horizontal form-filter">
        <div class="form-group">
            <div class="radio-list-vertical">
                <cms:CMSRadioButton ID="radAllPages" runat="server" GroupName="groupTemplate" Checked="true"
                    ResourceString="template.scopes.allpages" AutoPostBack="true" OnCheckedChanged="radAllPages_CheckedChanged" />
                <cms:CMSRadioButton ID="radSelectedScopes" runat="server" GroupName="groupTemplate"
                    ResourceString="template.scopes.selectedscopes" AutoPostBack="true" OnCheckedChanged="radSelectedScopes_CheckedChanged" />
            </div>
        </div>
    </div>
</asp:Content>
<asp:Content ID="cntSiteSelector" ContentPlaceHolderID="plcControls" runat="server">
    <asp:Panel ID="pnlSite" runat="server">
         <div class="form-horizontal form-filter">
             <div class="form-group">
                 <div class="editing-form-label-cell">
                     <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblSite" DisplayColon="true" ResourceString="general.site"
                         EnableViewState="false"></cms:LocalizedLabel>
                 </div>
                 <div class="editing-form-value-cell">
                     <cms:SiteSelector runat="server" ID="selectSite" IsLiveSite="false" />
                 </div>
            </div>
        </div>
    </asp:Panel>
</asp:Content>
<asp:Content ID="cntContent" ContentPlaceHolderID="plcContent" runat="Server">
    <asp:Panel ID="pnlContent" runat="server">
        <cms:CMSUpdatePanel runat="server" ID="pnlUpdate" UpdateMode="Conditional">
            <ContentTemplate>
                <cms:UniGrid runat="server" ID="unigridScopes" GridName="PageTemplateScopes_List.xml"
                    IsLiveSite="false" OrderBy="PageTemplateScopePath" />
            </ContentTemplate>
        </cms:CMSUpdatePanel>
    </asp:Panel>
</asp:Content>
