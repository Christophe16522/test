<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_FormControls_Pages_Development_List"
    Theme="Default" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Title="Form User Controls - Form User Control List"
    CodeFile="List.aspx.cs" %>

<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" TagName="UniGrid" TagPrefix="cms" %>
<%@ Register Namespace="CMS.UIControls.UniGridConfig" TagPrefix="ug" Assembly="CMS.UIControls" %>

<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:UniGrid runat="server" ID="grdList" ShortID="g" OrderBy="UserControlDisplayName"
        IsLiveSite="false" ObjectType="cms.formusercontrol" Columns="UserControlID, UserControlDisplayName, UserControlType, UserControlIsSystem, UserControlPriority, UserControlParentID, UserControlDescription">
        <GridActions>
            <ug:Action Name="edit" Caption="$General.Edit$" FontIconClass="icon-edit" FontIconStyle="Allow" />
            <ug:Action Name="delete" Caption="$General.Delete$" FontIconClass="icon-bin" FontIconStyle="Critical" Confirmation="$General.ConfirmDelete$" />
        </GridActions>
        <GridColumns>
            <ug:Column Source="UserControlDisplayName" Caption="$general.displayname$" Wrap="false" Localize="true">
                <Filter Type="text" />
                <Tooltip Source="UserControlDescription" />
            </ug:Column>
            <ug:Column Source="UserControlType" ExternalSourceName="controltype" Caption="$formcontrols.type$" Wrap="false">
                <Filter Type="custom" Path="~/CMSFormControls/System/UserControlTypeSelector.ascx" DefaultValue="0" />
            </ug:Column>
            <ug:Column Source="UserControlPriority" ExternalSourceName="#yesno" Caption="$formcontrols.highpriority$" Wrap="false" />
            <ug:Column Source="UserControlIsSystem" ExternalSourceName="#yesno" Caption="$formcontrols.issystem$" Wrap="false" Visible="false" />
            <ug:Column Source="UserControlParentID" ExternalSourceName="#yesno" Caption="$general.inherited$" Wrap="false">
                <Filter Type="custom" Path="~/CMSModules/FormControls/Controls/InheritedFilter.ascx" />
            </ug:Column>
            <ug:Column Width="100%" />
        </GridColumns>
        <GridOptions DisplayFilter="true" />
    </cms:UniGrid>
</asp:Content>
