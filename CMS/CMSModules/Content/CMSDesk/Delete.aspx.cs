using System;
using System.Collections;
using System.Data;
using System.Security.Principal;
using System.Threading;
using System.Web.UI.WebControls;

using CMS.DataEngine;
using CMS.EventLog;
using CMS.Helpers;
using CMS.LicenseProvider;
using CMS.Base;
using CMS.Localization;
using CMS.SiteProvider;
using CMS.Membership;
using CMS.DocumentEngine;
using CMS.UIControls;

using TreeNode = CMS.DocumentEngine.TreeNode;


public partial class CMSModules_Content_CMSDesk_Delete : CMSContentPage
{
    #region "Private variables"

    private readonly ArrayList nodeIds = new ArrayList();
    private string[] nodeIdsArr;
    private int cancelNodeId;
    private string mCultureCode;
    private CurrentUserInfo currentUser;
    private SiteInfo currentSite;

    private static readonly Hashtable mErrors = new Hashtable();
    private Hashtable mParameters;

    private string currentCulture = CultureHelper.DefaultUICultureCode;

    bool hasChildren;

    #endregion


    #region "Properties"

    /// <summary>
    /// Current log context.
    /// </summary>
    public LogContext CurrentLog
    {
        get
        {
            return EnsureLog();
        }
    }


    /// <summary>
    /// Current Error.
    /// </summary>
    private string CurrentError
    {
        get
        {
            return ValidationHelper.GetString(mErrors["DeleteError_" + ctlAsync.ProcessGUID], string.Empty);
        }
        set
        {
            mErrors["DeleteError_" + ctlAsync.ProcessGUID] = value;
        }
    }


    /// <summary>
    /// Indicates whether action is multiple.
    /// </summary>
    private bool IsMultipleAction
    {
        get
        {
            return QueryHelper.GetBoolean("multiple", false);
        }
    }


    /// <summary>
    /// Indicates whether action is multiple.
    /// </summary>
    private bool AllLevels
    {
        get
        {
            return QueryHelper.GetBoolean("alllevels", false);
        }
    }


    /// <summary>
    /// Gets selected class ID.
    /// </summary>
    private int ClassID
    {
        get
        {
            return QueryHelper.GetInteger("classid", 0);
        }
    }


    /// <summary>
    /// Culture to delete. (When deleting specific culture.)
    /// </summary>
    public override string CultureCode
    {
        get
        {
            return mCultureCode ?? (mCultureCode = QueryHelper.GetString("culture", LocalizationContext.PreferredCultureCode));
        }
    }


    /// <summary>
    /// Where condition used for multiple actions.
    /// </summary>
    private string WhereCondition
    {
        get
        {
            string where = string.Empty;
            if (Parameters != null)
            {
                where = ValidationHelper.GetString(Parameters["where"], string.Empty);
            }
            return where;
        }
    }


    /// <summary>
    /// Hashtable containing dialog parameters.
    /// </summary>
    private Hashtable Parameters
    {
        get
        {
            if (mParameters == null)
            {
                string identifier = QueryHelper.GetString("params", null);
                mParameters = (Hashtable)WindowHelper.GetItem(identifier);
            }
            return mParameters;
        }
    }


    /// <summary>
    /// Indicates if deleting products/products section in ecommerce context.
    /// </summary>
    private bool IsProductsMode
    {
        get
        {
            return QueryHelper.GetString("mode", "").ToLowerCSafe() == "productssection";
        }
    }


    /// <summary>
    /// Indicates if page shown in products UI
    /// </summary>
    protected override bool IsProductsUI
    {
        get
        {
            return IsProductsMode;
        }
    }

    #endregion


    #region "Page events"

    protected override void OnPreInit(EventArgs e)
    {
        base.OnPreInit(e);

        EnsureDocumentManager = false;

        // Perform check manually
        CheckDocPermissions = false;
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        // Register script files
        ScriptHelper.RegisterCMS(this);
        ScriptHelper.RegisterScriptFile(this, "~/CMSModules/Content/CMSDesk/Operation.js");

        // Set current UI culture
        currentCulture = CultureHelper.PreferredUICultureCode;
        // Initialize current user
        currentUser = MembershipContext.AuthenticatedUser;
        // Initialize current site
        currentSite = SiteContext.CurrentSite;

        // Initialize events
        ctlAsync.OnFinished += ctlAsync_OnFinished;
        ctlAsync.OnError += ctlAsync_OnError;
        ctlAsync.OnRequestLog += ctlAsync_OnRequestLog;
        ctlAsync.OnCancel += ctlAsync_OnCancel;

        if (!RequestHelper.IsCallback())
        {
            DataSet allDocs = null;
            TreeProvider tree = new TreeProvider(currentUser);
            btnCancel.Text = GetString("general.cancel");

            // Current Node ID to delete
            string parentAliasPath = string.Empty;
            if (Parameters != null)
            {
                parentAliasPath = ValidationHelper.GetString(Parameters["parentaliaspath"], string.Empty);
            }
            if (string.IsNullOrEmpty(parentAliasPath))
            {
                nodeIdsArr = QueryHelper.GetString("nodeid", string.Empty).Trim('|').Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string nodeId in nodeIdsArr)
                {
                    int id = ValidationHelper.GetInteger(nodeId, 0);
                    if (id != 0)
                    {
                        nodeIds.Add(id);
                    }
                }
            }
            else
            {
                string where = "ClassName <> 'CMS.Root'";
                if (!string.IsNullOrEmpty(WhereCondition))
                {
                    where = SqlHelper.AddWhereCondition(where, WhereCondition);
                }
                allDocs = tree.SelectNodes(currentSite.SiteName, parentAliasPath.TrimEnd(new char[] { '/' }) + "/%",
                    TreeProvider.ALL_CULTURES, true, ClassID > 0 ? DataClassInfoProvider.GetClassName(ClassID) : TreeProvider.ALL_CLASSNAMES, where,
                                           "DocumentName", TreeProvider.ALL_LEVELS, false, 0,
                                           TreeProvider.SELECTNODES_REQUIRED_COLUMNS + ",DocumentName,NodeParentID,NodeSiteID,NodeAliasPath,NodeSKUID");

                if (!DataHelper.DataSourceIsEmpty(allDocs))
                {
                    foreach (DataTable table in allDocs.Tables)
                    {
                        foreach (DataRow row in table.Rows)
                        {
                            nodeIds.Add(ValidationHelper.GetInteger(row["NodeID"], 0));
                        }
                    }
                }
            }

            // Setup page title text and image
            PageTitle.TitleText = GetString("Content.DeleteTitle");
            EnsureDocumentBreadcrumbs(PageBreadcrumbs, action: PageTitle.TitleText);

            btnCancel.Attributes.Add("onclick", ctlAsync.GetCancelScript(true) + "return false;");

            // Register the dialog script
            ScriptHelper.RegisterDialogScript(this);

            titleElemAsync.TitleText = GetString("ContentDelete.DeletingDocuments");
            // Set visibility of panels
            pnlContent.Visible = true;
            pnlLog.Visible = false;

            bool isMultilingual = CultureSiteInfoProvider.IsSiteMultilingual(currentSite.SiteName);
            if (!isMultilingual)
            {
                // Set all cultures checkbox
                chkAllCultures.Checked = true;
                chkAllCultures.Visible = false;
            }

            if (nodeIds.Count > 0)
            {
                if (nodeIds.Count == 1)
                {
                    // Single document deletion
                    int nodeId = ValidationHelper.GetInteger(nodeIds[0], 0);
                    TreeNode node = null;

                    if (string.IsNullOrEmpty(parentAliasPath))
                    {
                        // Get any culture if current not found
                        node = tree.SelectSingleNode(nodeId, CultureCode) ?? tree.SelectSingleNode(nodeId, TreeProvider.ALL_CULTURES);
                    }
                    else
                    {
                        if (allDocs != null)
                        {
                            DataRow dr = allDocs.Tables[0].Rows[0];
                            node = TreeNode.New(ValidationHelper.GetString(dr["ClassName"], string.Empty), dr, tree);
                        }
                    }

                    if (node != null)
                    {
                        bool rootDeleteDisabled = false;

                        if (IsProductsMode)
                        {
                            string startingPath = SettingsKeyInfoProvider.GetStringValue(CurrentSiteName + ".CMSStoreProductsStartingPath");
                            if (node.NodeAliasPath.CompareToCSafe(startingPath) == 0)
                            {
                                string closeLink = "<a href=\"#\"><span style=\"cursor: pointer;\" " +
                                       "onclick=\"SelectNode(" + node.NodeID + "); return false;\">" + GetString("general.back") +
                                       "</span></a>";

                                ShowError(string.Format(GetString("com.productsection.deleteroot"), closeLink, ""));
                                pnlDelete.Visible = false;
                                rootDeleteDisabled = true;
                            }
                        }

                        if (node.IsRoot() && isMultilingual)
                        {
                            // Hide 'Delete all cultures' checkbox
                            chkAllCultures.Visible = false;

                            if (!URLHelper.IsPostback())
                            {
                                // Check if there are any documents in another culture or current culture has some documents
                                pnlDeleteRoot.Visible = IsAnyDocumentInAnotherCulture(node) && (tree.SelectNodesCount(SiteContext.CurrentSiteName, "/%", LocalizationContext.PreferredCultureCode, false, null, null, null, TreeProvider.ALL_LEVELS, false) > 0);

                                if (pnlDeleteRoot.Visible)
                                {
                                    // Insert 'Delete current root' option if current root node is translated to current culture
                                    if (node.DocumentCulture == LocalizationContext.PreferredCultureCode)
                                    {
                                        rblRoot.Items.Add(new ListItem(GetString("rootdeletion.currentroot"), "current"));
                                    }

                                    rblRoot.Items.Add(new ListItem(GetString("rootdeletion.currentculture"), "allculturepages"));
                                    rblRoot.Items.Add(new ListItem(GetString("rootdeletion.allpages"), "allpages"));
                                }
                                else
                                {
                                    rblRoot.Items.Add(new ListItem(GetString("rootdeletion.allpages"), "allpages"));
                                }

                                if (rblRoot.SelectedIndex < 0)
                                {
                                    rblRoot.SelectedIndex = 0;
                                }
                            }
                        }

                        // Display warning for root node
                        if (!rootDeleteDisabled && node.IsRoot())
                        {
                            if (!currentUser.IsGlobalAdministrator)
                            {
                                pnlDelete.Visible = false;

                                ShowInformation(GetString("delete.rootonlyglobaladmin"));
                            }
                            else
                            {
                                if ((rblRoot.SelectedValue == "allpages") || !isMultilingual || ((rblRoot.SelectedValue == "allculturepages") && !IsAnyDocumentInAnotherCulture(node)))
                                {
                                    messagesPlaceholder.ShowWarning(GetString("Delete.RootWarning"));

                                    plcDeleteRoot.Visible = true;
                                }
                                else
                                {
                                    plcDeleteRoot.Visible = false;
                                }
                            }
                        }

                        hasChildren = node.NodeChildNodesCount > 0;

                        bool authorizedToDeleteSKU = !node.HasSKU || IsUserAuthorizedToModifySKU(node);
                        if (!RequestHelper.IsPostBack())
                        {
                            bool authorizedToDeleteDocument = IsUserAuthorizedToDeleteDocument(node);
                            if (!authorizedToDeleteDocument || !authorizedToDeleteSKU)
                            {
                                pnlDelete.Visible = false;
                                RedirectToAccessDenied(String.Format(GetString("cmsdesk.notauthorizedtodeletedocument"), HTMLHelper.HTMLEncode(node.NodeAliasPath)));
                            }
                        }

                        if (node.IsLink)
                        {
                            PageTitle.TitleText = GetString("Content.DeleteTitleLink") + " \"" + HTMLHelper.HTMLEncode(ResHelper.LocalizeString(node.GetDocumentName())) + "\"";
                            headQuestion.Text = GetString("ContentDelete.QuestionLink");
                            chkAllCultures.Checked = true;
                            plcCheck.Visible = false;
                        }
                        else
                        {
                            string nodeName = HTMLHelper.HTMLEncode(node.GetDocumentName());
                            // Get name for root document
                            if (node.NodeClassName.ToLowerCSafe() == "cms.root")
                            {
                                nodeName = HTMLHelper.HTMLEncode(currentSite.DisplayName);
                            }
                            PageTitle.TitleText = GetString("Content.DeleteTitle") + " \"" + HTMLHelper.HTMLEncode(ResHelper.LocalizeString(nodeName)) + "\"";

                            bool showSKUGroup = false;
                            if (NodeHasChildWithProduct(tree, node))
                            {
                                // Deleting product section
                                lblSKUActionInfo.Text = GetString("ContentDelete.SectionAssignedSKUInfo");
                                headDeleteSKU.Text = GetString("ContentDelete.AssignedSKUs");

                                showSKUGroup = true;
                            }
                            else if (node.HasSKU && authorizedToDeleteSKU)
                            {
                                // Deleting product
                                if (!NodeSharesSKUWithOtherNode(tree, node))
                                {
                                    lblSKUActionInfo.Text = GetString("contentdelete.assignedskuinfo");
                                    headDeleteSKU.Text = GetString("ContentDelete.AssignedSKU");

                                    showSKUGroup = true;
                                }
                            }

                            pnlDeleteSKU.Visible = showSKUGroup;
                            rblSKUAction.Visible = showSKUGroup;
                        }

                        // Show or hide checkbox
                        chkDestroy.Visible = CanDestroy(node);

                        cancelNodeId = IsMultipleAction ? node.NodeParentID : node.NodeID;

                        if (node.IsRoot())
                        {
                            // Change SEO panel if root is selected
                            pnlSeo.Visible = false;
                        }
                    }
                    else
                    {
                        if (!RequestHelper.IsPostBack())
                        {
                            URLHelper.Redirect(UIHelper.GetInformationUrl("editeddocument.notexists"));
                        }
                        else
                        {
                            // Hide everything
                            pnlContent.Visible = false;
                        }
                    }

                    headQuestion.Text = GetString("ContentDelete.Question");
                    chkAllCultures.Text = GetString("ContentDelete.AllCultures");
                    chkDestroy.Text = GetString("ContentDelete.Destroy");
                    headDeleteDocument.Text = GetString("ContentDelete.Document");
                }
                else if (nodeIds.Count > 1)
                {
                    pnlDocList.Visible = true;
                    string where = "NodeID IN (";
                    foreach (int nodeID in nodeIds)
                    {
                        where += nodeID + ",";
                    }

                    where = where.TrimEnd(',') + ")";
                    DataSet ds = allDocs ?? tree.SelectNodes(currentSite.SiteName, "/%", TreeProvider.ALL_CULTURES, true, null, where, "DocumentName", -1, false);

                    if (!DataHelper.DataSourceIsEmpty(ds))
                    {
                        string docList = null;

                        if (string.IsNullOrEmpty(parentAliasPath))
                        {
                            cancelNodeId = ValidationHelper.GetInteger(DataHelper.GetDataRowValue(ds.Tables[0].Rows[0], "NodeParentID"), 0);
                        }
                        else
                        {
                            cancelNodeId = TreePathUtils.GetNodeIdByAliasPath(currentSite.SiteName, parentAliasPath);
                        }

                        bool canDestroy = true;
                        bool permissions = true;

                        foreach (DataTable table in ds.Tables)
                        {
                            foreach (DataRow dr in table.Rows)
                            {
                                bool isLink = (dr["NodeLinkedNodeID"] != DBNull.Value);
                                string name = (string)dr["DocumentName"];
                                docList += HTMLHelper.HTMLEncode(name);
                                if (isLink)
                                {
                                    docList += DocumentHelper.GetDocumentMarkImage(Page, DocumentMarkEnum.Link);
                                }
                                docList += "<br />";
                                lblDocuments.Text = docList;

                                // Set visibility of checkboxes
                                TreeNode node = TreeNode.New(ValidationHelper.GetString(dr["ClassName"], string.Empty), dr);

                                if (!IsUserAuthorizedToDeleteDocument(node))
                                {
                                    permissions = false;
                                    AddError(String.Format(
                                        GetString("cmsdesk.notauthorizedtodeletedocument"),
                                        HTMLHelper.HTMLEncode(node.NodeAliasPath)), null);
                                }

                                // Can destroy if "can destroy all previous AND current"
                                canDestroy = CanDestroy(node) && canDestroy;

                                if (!hasChildren)
                                {
                                    hasChildren = node.NodeChildNodesCount > 0;
                                }

                                if ((node.HasSKU && IsUserAuthorizedToModifySKU(node)) || NodeHasChildWithProduct(tree, node))
                                {
                                    pnlDeleteSKU.Visible = true;
                                    rblSKUAction.Visible = true;
                                }
                            }
                        }

                        pnlDelete.Visible = permissions;
                        chkDestroy.Visible = canDestroy;
                    }
                    else
                    {
                        if (!RequestHelper.IsPostBack())
                        {
                            URLHelper.Redirect(UIHelper.GetInformationUrl("editeddocument.notexists"));
                        }
                        else
                        {
                            // Hide everything
                            pnlContent.Visible = false;
                        }
                    }

                    headQuestion.Text = GetString("ContentDelete.QuestionMultiple");
                    PageTitle.TitleText = GetString("Content.DeleteTitleMultiple");
                    chkAllCultures.Text = GetString("ContentDelete.AllCulturesMultiple");
                    chkDestroy.Text = GetString("ContentDelete.DestroyMultiple");
                    headDeleteDocument.Text = GetString("ContentDelete.Documents");
                    headDeleteSKU.Text = GetString("ContentDelete.AssignedSKUs");
                    lblSKUActionInfo.Text = GetString("ContentDelete.AssignedSKUsInfo");
                }

                // Init product actions
                if (!RequestHelper.IsPostBack())
                {
                    rblSKUAction.Items.Add(new ListItem(GetString("ContentDelete.SKU.deleteordisable"), "deleteordisable"));
                    rblSKUAction.Items.Add(new ListItem(GetString("ContentDelete.SKU.delete"), "delete"));
                    rblSKUAction.Items.Add(new ListItem(GetString("ContentDelete.SKU.disable"), "disable"));
                    rblSKUAction.Items.Add(new ListItem(GetString("ContentDelete.SKU.noaction"), "noaction"));

                    rblSKUAction.SelectedValue = "deleteordisable";
                }

                lblAltPath.AssociatedControlClientID = selAltPath.PathTextBox.ClientID;

                chkUseDeletedPath.CheckedChanged += chkUseDeletedPath_CheckedChanged;
                if (!RequestHelper.IsPostBack())
                {
                    selAltPath.Enabled = false;
                    chkAltSubNodes.Enabled = false;
                    chkAltAliases.Enabled = false;

                    // Set default path if is defined
                    selAltPath.Value = SettingsKeyInfoProvider.GetStringValue(CurrentSiteName + ".CMSDefaultDeletedNodePath");

                    if (!hasChildren)
                    {
                        chkAltSubNodes.Checked = false;
                        chkAltSubNodes.Enabled = false;
                    }
                }

                // If user has allowed cultures specified
                if (currentUser.UserHasAllowedCultures)
                {
                    // Get all site cultures
                    DataSet siteCultures = CultureSiteInfoProvider.GetSiteCultures(currentSite.SiteName);
                    bool denyAllCulturesDeletion = false;
                    // Check that user can edit all site cultures
                    foreach (DataRow culture in siteCultures.Tables[0].Rows)
                    {
                        string cultureCode = ValidationHelper.GetString(DataHelper.GetDataRowValue(culture, "CultureCode"), string.Empty);
                        if (!currentUser.IsCultureAllowed(cultureCode, currentSite.SiteName))
                        {
                            denyAllCulturesDeletion = true;
                        }
                    }
                    // If user can't edit all site cultures
                    if (denyAllCulturesDeletion)
                    {
                        // Hide all cultures selector
                        chkAllCultures.Visible = false;
                        chkAllCultures.Checked = false;
                    }
                }
                pnlDeleteDocument.Visible = chkAllCultures.Visible || chkDestroy.Visible;
            }
            else
            {
                // Hide everything
                pnlContent.Visible = false;
            }
        }
    }

    /// <summary>
    /// Change controls usability for alternate document
    /// </summary>
    void chkUseDeletedPath_CheckedChanged(object sender, EventArgs e)
    {
        selAltPath.Enabled = chkUseDeletedPath.Checked;
        chkAltSubNodes.Enabled = hasChildren & chkUseDeletedPath.Checked;
        chkAltAliases.Enabled = chkUseDeletedPath.Checked;
        plcDeleteSettings.Visible = chkUseDeletedPath.Checked;
    }


    protected override void OnPreRender(EventArgs e)
    {
        // Set visibility of controls
        pnlDeleteSKU.Visible = rblSKUAction.Visible;

        // Overwrite cancelNodeId variable if sub-levels are visible
        if (AllLevels && Parameters.ContainsKey("refreshnodeid"))
        {
            cancelNodeId = ValidationHelper.GetInteger(Parameters["refreshnodeid"], 0);
        }

        if (RequiresDialog)
        {
            btnNo.OnClientClick = "CloseDialog(); return false";
        }
        else
        {
            btnNo.OnClientClick = "SelectNode(" + cancelNodeId + "); return false";
        }

        string refreshCurrent = "function RefreshCurrent(){ RefreshTree(" + cancelNodeId + "," + cancelNodeId + "); }";
        ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "refreshCurrent", ScriptHelper.GetScript(refreshCurrent));

        base.OnPreRender(e);
    }

    #endregion


    #region "Button actions"

    protected void btnOK_Click(object sender, EventArgs e)
    {
        // For root, the additional checkbox must be checked
        if (plcDeleteRoot.Visible && !chkDeleteRoot.Checked)
        {
            ShowError(ResHelper.GetString("delete.rootnotchecked"));
            return;
        }

        // Check whether path is selected
        if (chkUseDeletedPath.Checked && String.IsNullOrEmpty(Convert.ToString(selAltPath.Value)))
        {
            ShowError(ResHelper.GetString("content.delete.altpathempty"));
            return;
        }

        string deleteMode = pnlDeleteRoot.Visible ? "rootoptions" : "documentoptions";

        pnlLog.Visible = true;
        pnlContent.Visible = false;

        CurrentError = string.Empty;
        CurrentLog.Close();
        EnsureLog();

        ctlAsync.Parameter = CultureCode + ";" + currentSite.SiteName + ";" + IsMultipleAction + ";" + AllLevels + ";" + deleteMode;
        ctlAsync.RunAsync(Delete, WindowsIdentity.GetCurrent());
    }

    #endregion


    #region "Async methods"

    /// <summary>
    /// Deletes document(s).
    /// </summary>
    private void Delete(object parameter)
    {
        if (parameter == null || nodeIds.Count < 1)
        {
            return;
        }

        if (!LicenseHelper.LicenseVersionCheck(RequestContext.CurrentDomain, FeatureEnum.Blogs, ObjectActionEnum.Edit))
        {
            AddError(ResHelper.GetString("cmsdesk.blogdeletelicenselimitations", currentCulture));
            return;
        }

        if (!LicenseHelper.LicenseVersionCheck(RequestContext.CurrentDomain, FeatureEnum.Documents, ObjectActionEnum.Edit))
        {
            AddError(ResHelper.GetString("cmsdesk.documentdeletelicenselimitations", currentCulture));
            return;
        }
        int refreshId = 0;

        TreeProvider tree = new TreeProvider(currentUser);
        tree.AllowAsyncActions = false;

        string[] parameters = ((string)parameter).Split(';');

        bool allLevelsSelected = ValidationHelper.GetBoolean(parameters[3], false);

        try
        {
            string siteName = parameters[1];
            bool isMultipleDelete = ValidationHelper.GetBoolean(parameters[2], false);

            // Prepare the where condition
            string where = SqlHelper.GetWhereCondition("NodeID", (int[])nodeIds.ToArray(typeof(int)));
            string columns = SqlHelper.MergeColumns(TreeProvider.SELECTNODES_REQUIRED_COLUMNS, "NodeAliasPath, ClassName, DocumentCulture, NodeParentID");

            bool combineWithDefaultCulture = false;
            string cultureCode = parameters[0];

            switch (parameters[4])
            {
                // Normalne page deletion
                case "documentoptions":
                    combineWithDefaultCulture = chkAllCultures.Checked;
                    cultureCode = combineWithDefaultCulture ? TreeProvider.ALL_CULTURES : parameters[0];
                    break;

                // Root page deletion
                case "rootoptions":
                    cultureCode = rblRoot.SelectedValue == "allpages" ? TreeProvider.ALL_CULTURES : parameters[0];
                    tree.DeleteChildNodes = rblRoot.SelectedValue != "current";
                    where = rblRoot.SelectedValue == "allculturepages" ? String.Empty : where;
                    break;
            }

            // Begin log
            AddLog(ResHelper.GetString("ContentDelete.DeletingDocuments", currentCulture));

            string orderBy = "NodeAliasPath DESC";
            if (cultureCode == TreeProvider.ALL_CULTURES)
            {
                // Default culture has to be selected on last position
                string defaultCulture = CultureHelper.GetDefaultCultureCode(siteName);
                orderBy += ", CASE WHEN DocumentCulture = '" + SqlHelper.GetSafeQueryString(defaultCulture) + "' THEN 1 ELSE 0 END";
            }

            // Get the documents
            DataSet ds = tree.SelectNodes(siteName, "/%", cultureCode, combineWithDefaultCulture, null, where, orderBy, TreeProvider.ALL_LEVELS, false, 0, columns);
            if (!DataHelper.DataSourceIsEmpty(ds))
            {
                string altPath = Convert.ToString(selAltPath.Value);
                TreeNode altNode = null;
                if (chkUseDeletedPath.Checked && !String.IsNullOrEmpty(altPath))
                {
                    NodeSelectionParameters nsp = new NodeSelectionParameters();
                    nsp.AliasPath = altPath;
                    nsp.CultureCode = TreeProvider.ALL_CULTURES;
                    nsp.ClassNames = TreeProvider.ALL_CLASSNAMES;
                    nsp.CombineWithDefaultCulture = true;
                    nsp.SiteName = siteName;
                    nsp.MaxRelativeLevel = TreeProvider.ALL_LEVELS;
                    nsp.TopN = 1;

                    altNode = DocumentHelper.GetDocument(nsp, tree);

                    // Check whether user is authorized to use alternating document
                    if (altNode != null)
                    {
                        if (currentUser.IsAuthorizedPerDocument(altNode, NodePermissionsEnum.Modify) == AuthorizationResultEnum.Denied)
                        {
                            throw new Exception(GetString("contentdelete.notallowedalternating"));
                        }
                    }
                }

                // Delete the documents
                foreach (DataRow nodeRow in ds.Tables[0].Rows)
                {
                    // Get the current document
                    string className = nodeRow["ClassName"].ToString();
                    string aliasPath = nodeRow["NodeAliasPath"].ToString();
                    string docCulture = nodeRow["DocumentCulture"].ToString();
                    refreshId = ValidationHelper.GetInteger(nodeRow["NodeParentID"], 0);
                    if (refreshId == 0)
                    {
                        refreshId = ValidationHelper.GetInteger(nodeRow["NodeID"], 0);
                    }
                    TreeNode node = DocumentHelper.GetDocument(siteName, aliasPath, docCulture, false, className, null, null, TreeProvider.ALL_LEVELS, false, null, tree);

                    if (node == null)
                    {
                        AddLog(string.Format(ResHelper.GetString("ContentRequest.DocumentNoLongerExists", currentCulture), HTMLHelper.HTMLEncode(aliasPath)));
                        continue;
                    }

                    // Ensure current parent ID
                    int parentId = node.NodeParentID;

                    // Check if bound SKU can be deleted (if any)
                    bool authorizedToDeleteSKU = !node.HasSKU || IsUserAuthorizedToModifySKU(node);

                    // Check delete permissions
                    if (IsUserAuthorizedToDeleteDocument(node) && (CanDestroy(node) || !chkDestroy.Checked) && authorizedToDeleteSKU)
                    {
                        // Delete the document
                        if (parentId <= 0)
                        {
                            parentId = node.NodeID;
                        }

                        // Prepare action for affected products
                        DeleteProductActionEnum deleteSKUsAction = DeleteProductActionEnum.NoAction;

                        switch (rblSKUAction.SelectedValue)
                        {
                            case "delete":
                                deleteSKUsAction = DeleteProductActionEnum.DeleteSKU;
                                break;

                            case "disable":
                                deleteSKUsAction = DeleteProductActionEnum.DisableSKU;
                                break;

                            case "deleteordisable":
                                deleteSKUsAction = DeleteProductActionEnum.DeleteOrDisableSKU;
                                break;
                        }

                        // Prepare settings for delete
                        DeleteDocumentSettings settings = new DeleteDocumentSettings(node, tree, chkAllCultures.Checked, chkDestroy.Checked, deleteSKUsAction);

                        // Add additional settings if alternating document is specified
                        if (altNode != null)
                        {
                            settings.AlternatingDocument = altNode;
                            settings.AlternatingDocumentCopyAllPaths = chkAltAliases.Checked;
                            settings.AlternatingDocumentMaxLevel = chkAltSubNodes.Checked ? -1 : node.NodeLevel;
                        }

                        // Delete document
                        refreshId = DocumentHelper.DeleteDocument(settings) || isMultipleDelete ? parentId : node.NodeID;
                    }
                    // Access denied - not authorized to delete the document
                    else
                    {
                        AddError(string.Format(ResHelper.GetString("cmsdesk.notauthorizedtodeletedocument", currentCulture), HTMLHelper.HTMLEncode(node.NodeAliasPath)));
                    }
                }
            }
            else
            {
                AddError(ResHelper.GetString("DeleteDocument.CultureNotExists", currentCulture));
            }
        }
        catch (ThreadAbortException ex)
        {
            string state = ValidationHelper.GetString(ex.ExceptionState, string.Empty);
            if (state == CMSThread.ABORT_REASON_STOP)
            {
                // When canceled
                ShowError(ResHelper.GetString("DeleteDocument.DeletionCanceled", currentCulture));
            }
            else
            {
                // Log error
                LogExceptionToEventLog(ex);
            }
        }
        catch (Exception ex)
        {
            // Log error
            LogExceptionToEventLog(ex);
        }
        finally
        {
            if (string.IsNullOrEmpty(CurrentError))
            {
                // Overwrite refreshId variable if sub-levels are visible
                if (allLevelsSelected && Parameters.ContainsKey("refreshnodeid"))
                {
                    refreshId = ValidationHelper.GetInteger(Parameters["refreshnodeid"], 0);
                }

                // Refresh tree or page (on-site editing)
                if (!RequiresDialog)
                {
                    ctlAsync.Parameter = "RefreshTree(" + refreshId + ", " + refreshId + "); \n" + "SelectNode(" + refreshId + ");";
                }
                else
                {
                    // Go to the root by default
                    string url = URLHelper.ResolveUrl("~/");

                    // Update the refresh node id when set in the parent dialog
                    if (Parameters != null)
                    {
                        int refreshNodeId = ValidationHelper.GetInteger(Parameters["refreshnodeid"], 0);
                        if (refreshNodeId > 0)
                        {
                            refreshId = refreshNodeId;
                        }
                    }

                    // Try go to the parent document
                    if (refreshId > 0)
                    {
                        TreeProvider tp = new TreeProvider(MembershipContext.AuthenticatedUser);
                        TreeNode tn = DocumentHelper.GetDocument(refreshId, TreeProvider.ALL_CULTURES, tp);
                        if (tn != null)
                        {
                            url = URLHelper.ResolveUrl(DocumentURLProvider.GetUrl(tn.NodeAliasPath));
                        }
                    }

                    ctlAsync.Parameter = "window.refreshPageOnClose = true; window.reloadPageUrl = " + ScriptHelper.GetString(url) + "; CloseDialog();";
                }
            }
            else
            {
                ctlAsync.Parameter = "RefreshTree(null, null);";
            }
        }
    }

    #endregion


    #region "Help methods"

    /// <summary>
    /// Return true if any document is in different culture than current.
    /// </summary>
    /// <param name="node">Tree node</param>
    private bool IsAnyDocumentInAnotherCulture(TreeNode node)
    {
        return Tree.SelectNodesCount(node.NodeSiteName, "/%", TreeProvider.ALL_CULTURES, true, null, "DocumentCulture NOT LIKE '" + node.DocumentCulture + "'", null, TreeProvider.ALL_LEVELS, false) > 0;
    }


    /// <summary>
    /// When exception occurs, log it to event log.
    /// </summary>
    /// <param name="ex">Exception to log</param>
    private void LogExceptionToEventLog(Exception ex)
    {
        EventLogProvider.LogEvent(EventType.ERROR, "Content", "DELETEDOC", EventLogProvider.GetExceptionLogMessage(ex), RequestContext.RawURL, currentUser.UserID, currentUser.UserName, 0, null, RequestContext.UserHostAddress, currentSite.SiteID);

        AddError(ResHelper.GetString("ContentRequest.DeleteFailed", currentCulture) + ": " + ex.Message);
    }


    /// <summary>
    /// Adds the script to the output request window.
    /// </summary>
    /// <param name="script">Script to add</param>
    public override void AddScript(string script)
    {
        ltlScript.Text += ScriptHelper.GetScript(script);
    }


    /// <summary>
    /// Indicates whether specified node can be destroyed by current user.
    /// </summary>
    /// <param name="node">Tree node to check</param>
    private bool CanDestroy(TreeNode node)
    {
        bool canDestroy = currentUser.IsAuthorizedPerDocument(node, NodePermissionsEnum.Destroy) == AuthorizationResultEnum.Allowed;
        if (node.HasSKU)
        {
            canDestroy &= currentUser.IsAuthorizedPerResource("CMS.Ecommerce", "Destroy");
        }

        return (canDestroy);
    }


    /// <summary>
    /// Checks whether the user is authorized to delete document.
    /// </summary>
    /// <param name="node">Document node</param>
    protected bool IsUserAuthorizedToDeleteDocument(TreeNode node)
    {
        // Check delete permission for document
        return (currentUser.IsAuthorizedPerDocument(node, new NodePermissionsEnum[] { NodePermissionsEnum.Delete, NodePermissionsEnum.Read }) == AuthorizationResultEnum.Allowed);
    }


    /// <summary>
    /// Checks whether the user is authorized to delete SKU bound to given node.
    /// </summary>
    /// <param name="node">Node to be checked</param>
    protected bool IsUserAuthorizedToModifySKU(TreeNode node)
    {
        bool authorized = false;

        if ((node != null) && (node.HasSKU))
        {
            var product = BaseAbstractInfoProvider.GetInfoById(PredefinedObjectType.SKU, node.NodeSKUID);
            if (product != null)
            {
                authorized = product.CheckPermissions(PermissionsEnum.Delete, node.NodeSiteName, currentUser);
            }
        }

        return authorized;
    }


    /// <summary>
    /// Returns true when given node has product related child.
    /// </summary>
    /// <param name="tree">Tree provider to use</param>
    /// <param name="node">Node to check</param>
    protected bool NodeHasChildWithProduct(TreeProvider tree, TreeNode node)
    {
        string aliasPath = node.NodeAliasPath.TrimEnd('/') + "/%";

        return 0 < tree.SelectNodesCount(node.NodeSiteName, aliasPath, TreeProvider.ALL_CULTURES, true, null, "NodeLinkedNodeID IS NULL AND NodeSKUID IS NOT NULL", null, TreeProvider.ALL_LEVELS, false);
    }


    /// <summary>
    /// Returns true when given node is bound to the same SKU as some other node does.
    /// </summary>
    /// <param name="tree">Tree provider to use</param>
    /// <param name="node">Node to check</param>
    protected bool NodeSharesSKUWithOtherNode(TreeProvider tree, TreeNode node)
    {
        string where = "NodeLinkedNodeID IS NULL AND NodeSKUID = " + node.NodeSKUID + " AND NodeID <> " + node.NodeID;
        return 0 < tree.SelectNodesCount(TreeProvider.ALL_SITES, "/%", TreeProvider.ALL_CULTURES, true, null, where, null, TreeProvider.ALL_LEVELS, false);
    }

    #endregion


    #region "Handling async thread"

    private void ctlAsync_OnCancel(object sender, EventArgs e)
    {
        ctlAsync.Parameter = null;
        string cancel = GetString("DeleteDocument.DeletionCanceled");
        AddLog(cancel);
        ltlScript.Text += ScriptHelper.GetScript("var __pendingCallbacks = new Array();RefreshCurrent();");
        ShowConfirmation(cancel);
        CurrentLog.Close();
    }


    private void ctlAsync_OnRequestLog(object sender, EventArgs e)
    {
        ctlAsync.Log = CurrentLog.Log;
    }


    private void ctlAsync_OnError(object sender, EventArgs e)
    {
        if (ctlAsync.Status == AsyncWorkerStatusEnum.Running)
        {
            ctlAsync.Stop();
        }
        ctlAsync.Parameter = null;
        ShowError(CurrentError);
        CurrentLog.Close();
    }


    private void ctlAsync_OnFinished(object sender, EventArgs e)
    {
        ShowError(CurrentError);
        CurrentLog.Close();

        if (!string.IsNullOrEmpty(CurrentError))
        {
            ctlAsync.Parameter = null;
            AddScript("RefreshTree(null, null);");
        }

        if (ctlAsync.Parameter != null)
        {
            AddScript(ctlAsync.Parameter.ToString());

            // Do not set the window title anymore
            PageTitle.SetWindowTitle = false;
        }
    }


    /// <summary>
    /// Ensures the logging context.
    /// </summary>
    protected LogContext EnsureLog()
    {
        LogContext log = LogContext.EnsureLog(ctlAsync.ProcessGUID);
        return log;
    }


    /// <summary>
    /// Adds the log information.
    /// </summary>
    /// <param name="newLog">New log information</param>
    protected void AddLog(string newLog)
    {
        EnsureLog();
        LogContext.AppendLine(newLog);
    }


    /// <summary>
    /// Adds the error to collection of errors.
    /// </summary>
    /// <param name="error">Error message</param>
    protected void AddError(string error)
    {
        AddLog(error);
        CurrentError = (error + "<br />" + CurrentError);
    }

    #endregion
}