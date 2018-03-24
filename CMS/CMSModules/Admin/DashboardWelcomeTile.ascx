<%@ Control Language="C#" AutoEventWireup="true" CodeFile="DashboardWelcomeTile.ascx.cs" Inherits="CMSModules_Admin_DashboardWelcomeTile" %>
<li class="tile">
    <div class="welcome-tile">
        <cms:CMSAccessibleLinkButton ID="lnkClose" runat="server" IconCssClass="icon-modal-close" OnClientClick="return false;" CssClass="js-close-button" />
        <h2><%= GetString("cms.dashboard.introduction.welcome") %></h2>
        <p class="lead"><%= GetString("cms.dashboard.introduction.info") %></p>
        <ul>
            <li><i aria-hidden="true" class="icon-kentico"></i><span><a class="js-app-list-link" href="#"><%= GetString("cms.dashboard.introduction.applications") %></a></span></li>
            <li><i aria-hidden="true" class="icon-question-circle"></i><span><a class="js-context-help-link" href="#"><%= GetString("cms.dashboard.introduction.help") %></a></span></li>
        </ul>
    </div>
</li>
