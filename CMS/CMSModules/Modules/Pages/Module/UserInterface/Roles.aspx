<%@ Page Title="Module edit - User interface - Roles" Language="C#" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master"
    AutoEventWireup="true" Inherits="CMSModules_Modules_Pages_Module_UserInterface_Roles"
    Theme="Default" CodeFile="Roles.aspx.cs" %>

<%@ Register Src="~/CMSFormControls/Sites/SiteSelector.ascx" TagName="SiteSelector"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniMatrix.ascx" TagName="UniMatrix"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/Membership/FormControls/Users/SelectUser.ascx" TagName="UserSelector"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/Basic/DisabledModuleInfo.ascx" TagPrefix="cms"
    TagName="DisabledModule" %>

<asp:Content ID="cntFilter" ContentPlaceHolderID="plcBeforeContent" runat="Server">
    <asp:Panel ID="pnlFilter" runat="server" CssClass="cms-edit-menu">
        <div class="form-horizontal form-filter">
            <asp:PlaceHolder runat="server" ID="plcSiteSelector">
                <div class="form-group">
                    <div class="filter-form-label-cell">
                        <cms:LocalizedLabel CssClass="control-label" ID="lblSites" runat="server" ResourceString="general.site" DisplayColon="true" />
                    </div>
                    <div class="filter-form-value-cell">
                        <cms:SiteSelector ID="siteSelector" runat="server" AllowAll="false" OnlyRunningSites="false" />
                    </div>
                </div>
            </asp:PlaceHolder>
            <asp:PlaceHolder ID="plcUser" runat="server">
                <div class="form-group">
                    <div class="filter-form-label-cell">
                        <cms:LocalizedLabel CssClass="control-label" ID="lblUser" runat="server" ResourceString="Administration-Permissions_Header.User"
                            DisplayColon="false" EnableViewState="false" AssociatedControlID="userSelector" />
                    </div>
                    <div class="filter-form-value-cell">
                        <cms:UserSelector ID="userSelector" runat="server" SelectionMode="SingleDropDownList" IsLiveSite="false" />
                        <cms:CMSUpdatePanel ID="pnlUpdateUsers" runat="server" UpdateMode="Always">
                            <ContentTemplate>
                                <cms:CMSCheckBox ID="chkUserOnly" runat="server" AutoPostBack="true" />
                            </ContentTemplate>
                        </cms:CMSUpdatePanel>
                    </div>
                </div>
            </asp:PlaceHolder>
        </div>
    </asp:Panel>
</asp:Content>
<asp:Content ID="cntMatrix" ContentPlaceHolderID="plcContent" runat="Server">
    <cms:CMSUpdatePanel ID="pnlUpdate" runat="server">
        <ContentTemplate>
            <cms:DisabledModule runat="server" ID="ucDisabledModule" SettingsKeys="CMSPersonalizeUserInterface" />
            <cms:LocalizedHeading runat="server" ID="headTitle" Level="4" EnableViewState="false" />
            <asp:PlaceHolder ID="plcUpdate" runat="server">
                <cms:UniMatrix ID="gridMatrix" CssClass="permission-matrix table-width-30" runat="server" QueryName="CMS.UIElement.getpermissionmatrix"
                    RowItemIDColumn="RoleID" ColumnItemIDColumn="ElementID" RowItemDisplayNameColumn="RoleDisplayName"
                    ColumnItemDisplayNameColumn="ElementDisplayName" RowTooltipColumn="RowDisplayName" FirstColumnClass="first-column"
                    ColumnTooltipColumn="PermissionDescription" ItemTooltipColumn="PermissionDescription" />
            </asp:PlaceHolder>
        </ContentTemplate>
    </cms:CMSUpdatePanel>
</asp:Content>