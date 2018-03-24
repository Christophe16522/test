<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master"
    Inherits="CMSModules_SystemDevelopment_Development_Resources_UICulture_StringsDefault_List"
    Theme="Default" Title="Strings - Strings List" CodeFile="UICulture_StringsDefault_List.aspx.cs" %>

<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" TagName="UniGrid" TagPrefix="cms" %>
<%@ Register Namespace="CMS.UIControls.UniGridConfig" TagPrefix="ug" Assembly="CMS.UIControls" %>

<asp:Content ID="controls" runat="server" ContentPlaceHolderID="plcBeforeContent">
    <asp:Panel ID="pnlCultures" runat="server" CssClass="header-panel">
        <div class="form-horizontal form-filter">
            <div class="form-group">
                <div class="filter-form-label-cell">
                    <cms:LocalizedLabel ID="lblAvailableCultures" runat="server" CssClass="control-label" EnableViewState="false"
                        AssociatedControlID="ddlAvailableCultures" ResourceString="sysdev.resources.availablecultures" DisplayColon="True" />
                </div>
                <div class="filter-form-value-cell">
                    <cms:CMSDropDownList ID="ddlAvailableCultures" runat="server" Visible="true" AutoPostBack="true"
                        CssClass="DropDownField" />
                </div>
            </div>
        </div>
    </asp:Panel>
</asp:Content>
<asp:Content ContentPlaceHolderID="plcContent" ID="content" runat="server">
    <cms:LocalizedLabel ID="lblInfo" runat="server" Visible="false" EnableViewState="false" ResourceString="sysdev.resources.nostring" />
    <cms:UniGrid ID="UniGridStrings" runat="server" Visible="True" IsLiveSite="false">
        <GridActions>
            <ug:Action Name="edit" Caption="$general.edit$" FontIconClass="icon-edit" FontIconStyle="Allow" />
            <ug:Action Name="delete" Caption="$general.delete$" FontIconClass="icon-bin" FontIconStyle="Critical" Confirmation="$General.ConfirmDelete$" />
        </GridActions>
        <GridColumns>
            <ug:Column Source="Data" Caption="$Unigrid.Strings.Columns.StringKey$" Wrap="false">
                <Filter Type="text" />
            </ug:Column>
            <ug:Column Source="Value" Caption="$Unigrid.Strings.Columns.StringEnglish$" Wrap="false"
                CssClass="main-column-100">
                <Filter Type="text" />
            </ug:Column>
        </GridColumns>
        <GridOptions DisplayFilter="true" />
    </cms:UniGrid>
</asp:Content>