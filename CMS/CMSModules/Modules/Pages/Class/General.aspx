<%@ Page Language="C#" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" AutoEventWireup="true"
    Inherits="CMSModules_Modules_Pages_Class_General"
    Title="Class- Edit- General" Theme="Default" CodeFile="General.aspx.cs" %>

<%@ Register Src="~/CMSAdminControls/UI/Selectors/LoadGenerationSelector.ascx" TagName="LoadGenerationSelector"
    TagPrefix="uc1" %>
<%@ Register Src="~/CMSFormControls/Basic/SelectConnectionString.ascx" TagName="SelectString"
    TagPrefix="cms" %>
<asp:Content ID="Content1" ContentPlaceHolderID="plcContent" runat="Server">
    <cms:UIForm ID="editElem" runat="server" ObjectType="cms.class" />
</asp:Content>
