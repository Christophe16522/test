<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Dashboard.aspx.cs" Inherits="CMSModules_Admin_Dashboard" Theme="Default" EnableEventValidation="false" MaintainScrollPositionOnPostback="true"
    MasterPageFile="~/CMSMasterPages/UI/EmptyPage.master" Title="Dashboard" EnableViewState="false" %>

<%@ Register Src="~/CMSModules/Admin/DashboardWelcomeTile.ascx" TagPrefix="cms" TagName="DashboardWelcomeTile" %>


<asp:Content ID="cntBody" ContentPlaceHolderID="plcContent" runat="server">
    <div class="dashboard">
            <div class="dashboard-inner">
            <asp:PlaceHolder ID="plcDashboard" runat="server">
                <ul>
                    <cms:DashboardWelcomeTile runat="server" id="DashboardWelcomeTile" />
                    <asp:Repeater ID="repApps" runat="server">
                        <ItemTemplate>
                            <li class="tile">
                                <a class="tile-btn <%# ApplicationHelper.GetApplicationIconCssClass(EvalGuid("ElementGUID")) %>" href="<%# GetApplicationPath(Eval("ElementResourceID"), Eval("ElementName")) %>" target="_top">
                                    <%# UIHelper.GetAccessibleImageMarkup(Page, EvalText("ElementIconClass", "icon-app-default"), EvalText("ElementIconPath"), size: FontIconSizeEnum.Dashboard) %>
                                    <h3><%# EvalText("ElementDisplayName") %></h3>
                                </a>
                            </li>
                        </ItemTemplate>
                    </asp:Repeater>
                </ul>
            </asp:PlaceHolder>
            <asp:PlaceHolder ID="plcEmpty" runat="server" Visible="false">
                <div class="empty">
                    <div class="tile"></div>
                    <div class="info">
                        <h2>
                            <cms:LocalizedLabel ID="lblTitle" runat="server" ResourceString="cms.dashboard.empty.title"></cms:LocalizedLabel></h2>
                        <p>
                            <cms:LocalizedLabel ID="lblInfo" runat="server" ResourceString="cms.dashboard.empty.info"></cms:LocalizedLabel>
                        </p>
                    </div>
                </div>
            </asp:PlaceHolder>
        </div>
    </div>
</asp:Content>
