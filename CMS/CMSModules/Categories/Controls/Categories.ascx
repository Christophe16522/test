<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Categories.ascx.cs" Inherits="CMSModules_Categories_Controls_Categories" %>
<%@ Register Src="~/CMSAdminControls/UI/Trees/UniTree.ascx" TagName="UniTree" TagPrefix="cms" %>
<%@ Register Src="~/CMSFormControls/Sites/SiteSelector.ascx" TagName="SiteSelector"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/Categories/Controls/CategoryEdit.ascx" TagName="CategoryEdit"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/AdminControls/Controls/Documents/Documents.ascx" TagName="Documents"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSFormControls/Filters/DocumentFilter.ascx" TagName="DocumentFilter"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/UI/PageElements/PageTitle.ascx" TagName="PageTitle"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" TagName="UniGrid" TagPrefix="cms" %>
<%@ Register Namespace="CMS.UIControls.UniGridConfig" TagPrefix="ug" Assembly="CMS.UIControls" %>
<asp:Literal ID="ltlScript" runat="server" EnableViewState="false" />
<script type="text/javascript">
    //<![CDATA[
    var selectedTreeNode = '';

    function SelectNode(elementName) {
        // Set selected item in tree
        selectedTreeNode = elementName;
        $j('span[id^="node_"]').each(function () {
            var jThis = $j(this);
            jThis.removeClass('ContentTreeSelectedItem');
            if (!jThis.hasClass('ContentTreeItem')) {
                jThis.addClass('ContentTreeItem');
            }
            if (this.id == 'node_' + elementName) {
                jThis.addClass('ContentTreeSelectedItem');
            }
        });
    }
    //]]>
</script>
<cms:CMSUpdatePanel ID="pnlUpdateHidden" runat="server" RenderMode="Inline">
    <ContentTemplate>
        <asp:HiddenField ID="hidSelectedElem" runat="server" />
        <asp:Button ID="hdnButton" runat="server" OnClick="hdnButton_OnClick" CssClass="HiddenButton"
            EnableViewState="false" />
    </ContentTemplate>
</cms:CMSUpdatePanel>
<asp:PlaceHolder ID="plcSelectSite" runat="server">
    <div class="header-panel">
        <div class="form-horizontal form-filter">
            <div class="form-group">
                <div class="filter-form-label-cell">
                    <cms:LocalizedLabel ID="lblSite" runat="server" ResourceString="General.Site" DisplayColon="true" CssClass="control-label" />
                </div>
                <div class="filter-form-value-cell-wide">
                    <cms:SiteSelector ID="SelectSite" runat="server" IsLiveSite="false" AllowAll="false"
                        AllowEmpty="false" AllowGlobal="true" />
                </div>
            </div>
        </div>
    </div>
</asp:PlaceHolder>
<div id="categories" class="Categories">
    <asp:Panel ID="pnlLeftContent" runat="server" CssClass="DialogLeftBlock">
        <cms:CMSUpdatePanel ID="pnlUpdateActions" runat="server" UpdateMode="Conditional">
            <ContentTemplate>
                <div class="tree-actions-panel">
                    <div class="tree-actions">
                        <cms:CMSAccessibleButton runat="server" ID="btnNew" IconCssClass="icon-plus" IconOnly="true" OnClick="btnNewElem_Click" />
                        <cms:CMSAccessibleButton runat="server" ID="btnDelete" IconCssClass="icon-bin" IconOnly="true" OnClick="btnDeleteElem_Click" OnClientClick="if (!deleteConfirm()) return false;" />
                        <cms:CMSAccessibleButton runat="server" ID="btnUp" IconCssClass="icon-chevron-up" IconOnly="true" OnClick="btnUpElem_Click" />
                        <cms:CMSAccessibleButton runat="server" ID="btnDown" IconCssClass="icon-chevron-down" IconOnly="true" OnClick="btnDownElem_Click" />
                    </div>
                </div>
            </ContentTemplate>
        </cms:CMSUpdatePanel>
        <asp:Panel ID="pnlTree" runat="server" class="DialogTreeArea" Height="100%">
            <div class="CategoryTrees" style="overflow: auto;">
                <cms:CMSUpdatePanel ID="pnlUpdateTree" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <div class="InnerCategoryTrees">
                            <cms:UniTree runat="server" ID="treeElemG" ShortID="tg" Localize="true" IsLiveSite="false"
                                UsePostBack="false" EnableRootAction="false" />
                            <cms:UniTree runat="server" ID="treeElemP" ShortID="tp" Localize="true" IsLiveSite="false"
                                UsePostBack="false" EnableRootAction="false" />
                        </div>
                    </ContentTemplate>
                </cms:CMSUpdatePanel>
            </div>
        </asp:Panel>
    </asp:Panel>
    <div class="DialogTreeAreaSeparator">
    </div>
    <asp:Panel ID="pnlRightContent" runat="server" CssClass="DialogRightBlock">
        <cms:CMSUpdatePanel ID="pnlUpdateContent" runat="server" UpdateMode="Conditional">
            <ContentTemplate>
                <asp:Panel ID="pnlHeader" runat="server" CssClass="PageHeader">
                    <cms:PageTitle ID="titleElem" ShortID="pt" runat="server" EnableViewState="false" HideTitle="true" />
                </asp:Panel>
                <div id="divDialogView" runat="server">
                    <asp:PlaceHolder ID="plcEdit" runat="server">
                        <cms:JQueryTabContainer ID="pnlTabs" runat="server" CssClass="Dialog_Tabs LightTabs BreadTabs">
                            <cms:JQueryTab ID="tabCategories" runat="server">
                                <ContentTemplate>
                                    <div class="HeaderSeparator">
                                        &nbsp;
                                    </div>
                                    <div class="PageContent">
                                        <cms:UniGrid runat="server" ID="gridSubCategories" ShortID="g" OrderBy="CategorySiteID, CategoryOrder"
                                            Columns="CategoryID, CategoryDisplayName, CategoryUserID, CategorySiteID,CategoryParentID, CategoryEnabled, (SELECT COUNT(DocumentID) FROM CMS_DocumentCategory WHERE CMS_DocumentCategory.CategoryID = CMS_Category.CategoryID) AS DocumentCount"
                                            ObjectType="cms.categorylist" CheckRelative="true" DelayedReload="true">
                                            <GridActions Parameters="CategoryID;CategoryParentID">
                                                <ug:Action Name="edit" Caption="$general.edit$" FontIconClass="icon-edit" FontIconStyle="Allow" OnClick="NodeSelected({0},{1}, true);return false;" />
                                                <ug:Action Name="delete" ExternalSourceName="Delete" Caption="$General.Delete$" FontIconClass="icon-bin"
                                                    Confirmation="$General.ConfirmDelete$" FontIconStyle="Critical" />
                                            </GridActions>
                                            <GridColumns>
                                                <ug:Column Source="CategoryDisplayName" Caption="$categories.category$" Wrap="false"
                                                    Localize="true" AllowSorting="true">
                                                    <Filter Type="text" />
                                                </ug:Column>
                                                <ug:Column Source="CategoryEnabled" ExternalSourceName="#yesno" Caption="$general.enabled$"
                                                    Wrap="false" Localize="true" AllowSorting="true">
                                                </ug:Column>
                                                <ug:Column Source="DocumentCount" Caption="$category_edit.documents$" Wrap="false"
                                                    AllowSorting="false">
                                                </ug:Column>
                                                <ug:Column Width="100%" />
                                            </GridColumns>
                                            <GridOptions DisplayFilter="true" AllowSorting="true" />
                                        </cms:UniGrid>
                                    </div>
                                </ContentTemplate>
                            </cms:JQueryTab>
                            <cms:JQueryTab ID="tabGeneral" runat="server">
                                <ContentTemplate>
                                    <cms:CategoryEdit ID="catEdit" runat="server" Visible="true" AllowDisabledParents="true"
                                        PanelCssClass="PageContent" ComponentName="CategoryEdit" />
                                </ContentTemplate>
                            </cms:JQueryTab>
                            <cms:JQueryTab ID="tabDocuments" runat="server">
                                <ContentTemplate>
                                    <div class="HeaderSeparator">
                                        &nbsp;
                                    </div>
                                    <div class="PageContent">
                                        <asp:PlaceHolder ID="plcFilter" runat="server">
                                            <cms:DocumentFilter ID="filterDocuments" runat="server" LoadSites="true" />
                                            <br />
                                            <br />
                                        </asp:PlaceHolder>
                                        <cms:LocalizedHeading runat="server" ID="headTitle" Level="4" ResourceString="categories_edit.documents.used" DisplayColon="true"
                                            CssClass="listing-title" EnableViewState="false" />
                                        <cms:Documents ID="gridDocuments" runat="server" ListingType="CategoryDocuments" />
                                    </div>
                                </ContentTemplate>
                            </cms:JQueryTab>
                        </cms:JQueryTabContainer>
                    </asp:PlaceHolder>
                    <asp:PlaceHolder ID="plcNew" runat="server" Visible="false">
                        <cms:CategoryEdit ID="catNew" runat="server" Visible="true" AllowDisabledParents="true"
                            PanelCssClass="PageContent" ComponentName="CategoryNew" EnableViewState="false" />
                    </asp:PlaceHolder>
                    <asp:PlaceHolder ID="plcInfo" runat="server" Visible="false">
                        <div class="PageContent">
                            <cms:LocalizedLabel ID="lblInfo" runat="server" ResourceString="categories.SelectOrCreateInfo"
                                CssClass="InfoLabel" EnableViewState="false" />
                        </div>
                    </asp:PlaceHolder>
                </div>
            </ContentTemplate>
        </cms:CMSUpdatePanel>
    </asp:Panel>
</div>
<script type="text/javascript" language="javascript">
    //<![CDATA[
    (function () {
        function initDesign() {
            var docHeight = $j('body').height();

            var container = $j('#categories').parent('.TabsContent');
            if (container.length) {
                container.css('padding', '0');
                container.height(500);
                docHeight = 500;
            }

            var titleHeight = $j('.PageHeader:not(#categories .PageHeader)').outerHeight();
            var siteHeight = $j('.header-panel').outerHeight();
            var contentHeight = docHeight - titleHeight - siteHeight;

            $j('#categories').height(contentHeight);
            $j('.DialogTreeAreaSeparator').height(contentHeight);
            var menuHeight = $j('.tree-actions-panel').outerHeight();
            $j('.CategoryTrees').height(contentHeight - menuHeight);

            var tabsHeight = $j('div.JqueryUITabs>div>ul.ui-tabs-nav').outerHeight();
            var $breadcrumbs = $j('.breadcrumb');

            var breadcrumbsHeight = $breadcrumbs.outerHeight();
            $j('.ui-tabs-panel').height(contentHeight - tabsHeight - breadcrumbsHeight - 12);
            $j('.ui-tabs-panel').css('overflow', 'auto');
        }

        $j(initDesign);
        $j(window).resize(initDesign);
    }())
    //]]>
</script>
