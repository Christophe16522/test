<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_Permissions_Pages_Administration_Default"
    MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" CodeFile="Default.aspx.cs" Theme="default" %>

<%@ Register Src="~/CMSModules/Permissions/Controls/PermissionsFilter.ascx" TagName="PermissionsFilter" TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/Permissions/Controls/PermissionsMatrix.ascx" TagName="PermissionsMatrix" TagPrefix="cms" %>
 
<asp:Content ID="cntBeforeCnt" runat="server" ContentPlaceHolderID="plcBeforeContent">
    <cms:PermissionsFilter ID="prmhdrHeader" runat="server" IsLiveSite="false" />
</asp:Content>
<asp:Content ID="cntContent" ContentPlaceHolderID="plcContent" runat="server">
    <cms:PermissionsMatrix ID="prmMatrix" ShortID="mx" runat="server" QueryName="cms.permission.getpermissionMatrix" ColumnItemTooltipColumn="PermissionDescription" 
        RowItemIDColumn="RoleID" ColumnItemIDColumn="PermissionID" RowItemDisplayNameColumn="RoleDisplayName" ColumnItemDisplayNameColumn="PermissionDisplayName" 
        RowItemTooltipColumn="RoleDisplayName" ItemTooltipColumn="PermissionDescription" IsLiveSite="false"/>            
</asp:Content>

