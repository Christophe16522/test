<%@ Control Language="C#" AutoEventWireup="true" CodeFile="DisabledModuleInfo.ascx.cs"
    Inherits="CMSAdminControls_Basic_DisabledModuleInfo" %>
<div class="alert alert-warning">
    <span class="alert-icon">
        <cms:CMSIcon ID="iconAlert" runat="server" CssClass="icon-exclamation-triangle" AlternativeText="{$general.warning$}" />
    </span>
    <div class="alert-label">
        <cms:LocalizedLabel runat="server" ID="lblText" EnableViewState="false" />
        <div class="alert-buttons">
            <cms:CMSButton runat="server" ID="btnGlobal" ButtonStyle="Default" OnClick="btnGlobal_clicked" />
            <cms:CMSButton runat="server" ID="btnSite" ButtonStyle="Default" OnClick="btnSiteOnly_clicked" />
        </div>
    </div>
</div>