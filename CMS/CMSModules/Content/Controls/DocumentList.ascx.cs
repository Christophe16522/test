using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;

using CMS.CMSHelper;
using CMS.ExtendedControls;
using CMS.GlobalHelper;
using CMS.SettingsProvider;
using CMS.SiteProvider;
using CMS.DocumentEngine;
using CMS.UIControls;
using CMS.WorkflowEngine;
using CMS.DataEngine;
using CMS.TranslationServices;
using CMS.EventLog;
using CMS.LicenseProvider;

using TreeNode = CMS.DocumentEngine.TreeNode;
using CMS.Membership;
using CMS.Helpers;
using CMS.Localization;
using CMS.Base;
using CMS.PortalEngine;
using CMS.Globalization;

public partial class CMSModules_Content_Controls_DocumentList : CMSUserControl, ICallbackEventHandler, IPostBackEventHandler
{
    #region "Private and protected variables & enums"

    // Private fields
    private CurrentUserInfo currentUserInfo = null;
    private SiteInfo currentSiteInfo = null;
    private TreeProvider mTree = null;
    private TreeNode mNode = null;
    private ArrayList mFlagsControls = null;
    private DataSet mSiteCultures = null;
    private DialogConfiguration mConfig = null;
    private string mCultureCode = null;
    private bool isRootDocument = false;
    private bool isCurrentDocument = false;
    private int refreshNodeId = 0;

    private string mDefaultSiteCulture = null;
    private string currentSiteName = null;
    private int mNodeId = 0;
    private int? mWOpenerNodeId = null;
    private bool dataLoaded = false;
    private bool allSelected = false;
    private bool checkPermissions = false;
    private bool mShowDocumentTypeIcon = true;
    private bool mDocumentNameAsLink = true;
    private bool mShowDocumentMarks = true;
    private string aliasPath = null;
    private string urlParameter = string.Empty;
    private string mAdditionalColumns = null;
    private string mSelectItemJSFunction = "SelectItem";
    private string mWhereCodition = string.Empty;
    private string mOrderBy = "NodeOrder ASC, NodeName ASC, NodeAlias ASC";
    private string mDeleteReturnUrl = string.Empty;
    private string mPublishReturnUrl = string.Empty;
    private string mArchiveReturnUrl = string.Empty;
    private string mSelectLanguageJSFunction = null;

    private Action callbackAction = Action.Move;

    private ClientScriptManager mClientScript = null;

    // Possible actions
    private enum Action
    {
        SelectAction = 0,
        Move = 1,
        Copy = 2,
        Link = 3,
        Delete = 4,
        Publish = 5,
        Archive = 6,
        Translate = 7
    }

    // Action scope
    private enum What
    {
        SelectedDocuments = 0,
        AllDocuments = 1
    }

    // Action scope for callback handling
    private enum Argument
    {
        Action = 0,
        AllSelected = 1,
        Items = 2
    }

    #endregion


    #region "Private properties"

    /// <summary>
    /// Default culture of the site.
    /// </summary>
    private string DefaultSiteCulture
    {
        get
        {
            return mDefaultSiteCulture ?? (mDefaultSiteCulture = CultureHelper.GetDefaultCultureCode(currentSiteName));
        }
    }


    /// <summary>
    /// Hashtable with document flags controls.
    /// </summary>
    private ArrayList FlagsControls
    {
        get
        {
            return mFlagsControls ?? (mFlagsControls = new ArrayList());
        }
    }


    /// <summary>
    /// Site cultures.
    /// </summary>
    private DataSet SiteCultures
    {
        get
        {
            if (mSiteCultures == null)
            {
                mSiteCultures = CultureSiteInfoProvider.GetSiteCultures(currentSiteName).Copy();
                if (!DataHelper.DataSourceIsEmpty(mSiteCultures))
                {
                    DataTable cultureTable = mSiteCultures.Tables[0];
                    DataRow[] defaultCultureRow = cultureTable.Select("CultureCode='" + DefaultSiteCulture + "'");

                    // Ensure default culture to be first
                    DataRow dr = cultureTable.NewRow();
                    if (defaultCultureRow.Length > 0)
                    {
                        dr.ItemArray = defaultCultureRow[0].ItemArray;
                        cultureTable.Rows.InsertAt(dr, 0);
                        cultureTable.Rows.Remove(defaultCultureRow[0]);
                    }
                }
            }
            return mSiteCultures;
        }
    }


    /// <summary>
    /// Gets the configuration for Copy and Move dialog.
    /// </summary>
    private DialogConfiguration Config
    {
        get
        {
            if (mConfig == null)
            {
                mConfig = new DialogConfiguration();
                mConfig.ContentSelectedSite = SiteContext.CurrentSiteName;
                mConfig.OutputFormat = OutputFormatEnum.Custom;
                mConfig.SelectableContent = SelectableContentEnum.AllContent;
                mConfig.HideAttachments = false;
            }
            return mConfig;
        }
    }


    /// <summary>
    /// Holds current where condition of filter.
    /// </summary>
    private string CurrentWhereCondition
    {
        get
        {
            return ValidationHelper.GetString(ViewState["CurrentWhereCondition"], string.Empty);
        }
        set
        {
            ViewState["CurrentWhereCondition"] = value;
        }
    }


    /// <summary>
    /// Dialog control identifier.
    /// </summary>
    private string Identifier
    {
        get
        {
            string identifier = hdnIdentifier.Value;
            if (string.IsNullOrEmpty(identifier))
            {
                identifier = "DocumentListDialogIdentifier";
                hdnIdentifier.Value = identifier;
            }

            return identifier;
        }
    }


    /// <summary>
    /// Tree provider for current user
    /// </summary>
    private TreeProvider Tree
    {
        get
        {
            return mTree ?? (mTree = new TreeProvider(MembershipContext.AuthenticatedUser)
            {
                PreferredCultureCode = CultureCode
            });
        }
    }


    /// <summary>
    /// Client script manager from containing page.
    /// </summary>
    private ClientScriptManager ClientScript
    {
        get
        {
            return mClientScript ?? (mClientScript = Page.ClientScript);
        }
    }


    /// <summary>
    /// Gets the WOpenerNodeID from the url query.
    /// </summary>
    private int WOpenerNodeID
    {
        get
        {
            if (mWOpenerNodeId == null)
            {
                mWOpenerNodeId = QueryHelper.GetInteger("wopenernodeid", 0);
            }

            return mWOpenerNodeId.Value;
        }
    }

    #endregion


    #region "Events"

    /// <summary>
    /// Event raised when external source data required.
    /// </summary>
    public event OnExternalDataBoundEventHandler OnExternalAdditionalDataBound;

    /// <summary>
    /// Event raised when creating document flags control.
    /// </summary>
    public event OnExternalDataBoundEventHandler OnDocumentFlagsCreating;

    #endregion


    #region "Public properties"

    /// <summary>
    /// ID of the node which child nodes are to be displayed.
    /// </summary>
    public int NodeID
    {
        get
        {
            return mNodeId;
        }
        set
        {
            mNodeId = value;
            mNode = null;
        }
    }


    /// <summary>
    /// Culture to consider as preferred.
    /// </summary>
    private string CultureCode
    {
        get
        {
            return mCultureCode ?? (mCultureCode = QueryHelper.GetString("culture", LocalizationContext.PreferredCultureCode));
        }
    }


    /// <summary>
    /// TreeNode object specified by NodeID property.
    /// </summary>
    public TreeNode Node
    {
        get
        {
            return mNode ?? (mNode = Tree.SelectSingleNode(NodeID, TreeProvider.ALL_CULTURES, true));
        }
        set
        {
            mNode = value;

            if (value != null)
            {
                mNodeId = value.NodeID;
            }
        }
    }


    /// <summary>
    /// Unigrid object used for listing documents.
    /// </summary>
    public UniGrid Grid
    {
        get
        {
            return gridDocuments;
        }
    }


    /// <summary>
    /// Additional column names for listing separated by coma ','.
    /// </summary>
    public string AdditionalColumns
    {
        get
        {
            return mAdditionalColumns;
        }
        set
        {
            mAdditionalColumns = value;
        }
    }


    /// <summary>
    /// Where condition used to restrict selection of documents.
    /// </summary>
    public string WhereCodition
    {
        get
        {
            return mWhereCodition;
        }
        set
        {
            mWhereCodition = value;
        }
    }


    /// <summary>
    /// Default order by clause.
    /// </summary>
    public string OrderBy
    {
        get
        {
            return mOrderBy;
        }
        set
        {
            mOrderBy = value;
        }
    }


    /// <summary>
    /// Name of the javascript function used for selecting items. Current node id and parent node id are supplied as parameters.
    /// </summary>
    public string SelectItemJSFunction
    {
        get
        {
            return mSelectItemJSFunction;
        }
        set
        {
            mSelectItemJSFunction = value;
        }
    }


    /// <summary>
    /// Name of the javascript function called when flag is clicked.
    /// </summary>
    public string SelectLanguageJSFunction
    {
        get
        {
            return mSelectLanguageJSFunction;
        }
        set
        {
            mSelectLanguageJSFunction = value;
        }
    }


    /// <summary>
    /// Return URL for delete action
    /// </summary>
    public string DeleteReturnUrl
    {
        get
        {
            return mDeleteReturnUrl;
        }
        set
        {
            mDeleteReturnUrl = value;
        }
    }


    /// <summary>
    /// Return URL for publish action
    /// </summary>
    public string PublishReturnUrl
    {
        get
        {
            return mPublishReturnUrl;
        }
        set
        {
            mPublishReturnUrl = value;
        }
    }


    /// <summary>
    /// Return URL for archive action
    /// </summary>
    public string ArchiveReturnUrl
    {
        get
        {
            return mArchiveReturnUrl;
        }
        set
        {
            mArchiveReturnUrl = value;
        }
    }


    /// <summary>
    /// Return URL for translate action
    /// </summary>
    public string TranslateReturnUrl
    {
        get;
        set;
    }


    /// <summary>
    /// Indicates whether all levels of children are to be searched.
    /// </summary>
    public bool ShowAllLevels
    {
        get;
        set;
    }


    /// <summary>
    /// Indicates whether document type icon will be shown before document name. True by default.
    /// </summary>
    public bool ShowDocumentTypeIcon
    {
        get
        {
            return mShowDocumentTypeIcon;
        }
        set
        {
            mShowDocumentTypeIcon = value;
        }

    }


    /// <summary>
    /// Indicates whether tooltip containing document type name will be added to document type icon.
    /// </summary>
    public bool ShowDocumentTypeIconTooltip
    {
        get;
        set;
    }


    /// <summary>
    /// Indicates whether document names will be rendered as links. True by default.
    /// </summary>
    public bool DocumentNameAsLink
    {
        get
        {
            return mDocumentNameAsLink;
        }
        set
        {
            mDocumentNameAsLink = value;
        }

    }


    /// <summary>
    /// Indicates whether document marks will be shown after document name. True by default.
    /// </summary>
    public bool ShowDocumentMarks
    {
        get
        {
            return mShowDocumentMarks;
        }
        set
        {
            mShowDocumentMarks = value;
        }
    }


    /// <summary>
    /// Path where content tree in copy/move/link dialogs will start.
    /// </summary>
    public string CopyMoveLinkStartingPath
    {
        get;
        set;
    }


    /// <summary>
    /// Gets or sets the value that indicates whether the page is displayed as dialog. 
    /// </summary>
    public bool RequiresDialog
    {
        get;
        set;
    }

    #endregion


    #region "Page events"

    /// <summary>
    /// Init event handler.
    /// </summary>
    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        currentSiteName = SiteContext.CurrentSiteName;
        currentUserInfo = MembershipContext.AuthenticatedUser;

        gridDocuments.ZeroRowsText = GetString("content.nochilddocumentsfound");
        gridDocuments.OnFilterFieldCreated += gridDocuments_OnFilterFieldCreated;
        gridDocuments.FilterLimit = 1;
    }


    /// <summary>
    /// Handles the Load event of the Page control.
    /// </summary>
    protected void Page_Load(object sender, EventArgs e)
    {
        gridDocuments.StopProcessing = StopProcessing;
        if (StopProcessing)
        {
            return;
        }

        if (NodeID > 0)
        {
            checkPermissions = Tree.CheckDocumentUIPermissions(currentSiteName);

            if (Node != null)
            {
                if (currentUserInfo.IsAuthorizedPerDocument(Node, NodePermissionsEnum.ExploreTree) != AuthorizationResultEnum.Allowed)
                {
                    CMSPage.RedirectToCMSDeskAccessDenied("CMS.Content", "exploretree");
                }

                aliasPath = Node.NodeAliasPath;
            }

            ScriptHelper.RegisterProgress(Page);
            ScriptHelper.RegisterDialogScript(Page);
            ScriptHelper.RegisterJQuery(Page);

            InitDropdownLists();

            // Prepare JavaScript for actions
            StringBuilder actionScript = new StringBuilder();
            actionScript.Append(
                @"function PerformAction(selectionFunction, selectionField, dropId){
    var label = document.getElementById('", lblInfo.ClientID, @"');
    var whatDrp = document.getElementById('", drpWhat.ClientID, @"');
    var action = document.getElementById(dropId).value;
    var selectionFieldElem = document.getElementById(selectionField);
    var allSelected = ", (int)What.SelectedDocuments, @";
    if (action == '", (int)Action.SelectAction, @"'){
        label.innerHTML = '", GetString("massaction.selectsomeaction"), @"';
        return false;
    }
    if(whatDrp.value == '", (int)What.AllDocuments, @"'){
        allSelected = ", (int)What.AllDocuments, @";
    }
    var items = selectionFieldElem.value;
    if(!eval(selectionFunction) || whatDrp.value == '", (int)What.AllDocuments, @"'){
        var argument = '|' + allSelected + '|' + items;
        switch(action){
            case '", (int)Action.Move, @"':
                argument = '", Action.Move, "' + argument;", ClientScript.GetCallbackEventReference(this, "argument", "OpenModal", string.Empty), @";
                break;

            case '", (int)Action.Copy, @"':
                argument = '", Action.Copy, "' + argument;", ClientScript.GetCallbackEventReference(this, "argument", "OpenModal", string.Empty), @";
                break;

            case '", (int)Action.Link, @"':
                argument = '", Action.Link, "' + argument;", ClientScript.GetCallbackEventReference(this, "argument", "OpenModal", string.Empty), @";
                break;

            case '", (int)Action.Delete, @"':
                argument = '", Action.Delete, "' + argument;", ClientScript.GetCallbackEventReference(this, "argument", "Redirect", string.Empty), @";
                break;

            case '", (int)Action.Publish, @"':
                argument = '", Action.Publish, "' + argument;", ClientScript.GetCallbackEventReference(this, "argument", "Redirect", string.Empty), @";
                break;

            case '", (int)Action.Archive, @"':
                argument = '", Action.Archive, "' + argument;", ClientScript.GetCallbackEventReference(this, "argument", "Redirect", string.Empty), @";
                break;

            case '", (int)Action.Translate, @"':
                argument = '", Action.Translate, "' + argument;", ClientScript.GetCallbackEventReference(this, "argument", "Redirect", string.Empty), @";
                break;

            default:
                return false;
        }
    }
    else{
        label.innerHTML = '", GetString("documents.selectdocuments"), @"';
    }
    return false;
}

function OpenModal(arg, context){
    modalDialog(arg,'actionDialog','90%', '85%');
}

function Redirect(arg, context){
    document.location.replace(arg);
}

function MoveNode(action, nodeId){
    document.getElementById('", hdnMoveId.ClientID, @"').value = action + ';' + nodeId ;
    ", Page.ClientScript.GetPostBackEventReference(this, "move"), @"  
}

");

            ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "actionScript", ScriptHelper.GetScript(actionScript.ToString()));

            // Add action to button
            btnOk.OnClientClick = "return PerformAction('" + gridDocuments.GetCheckSelectionScript() + "','" + gridDocuments.GetSelectionFieldClientID() + "','" + drpAction.ClientID + "');";

            // Setup the grid
            gridDocuments.OrderBy = OrderBy;
            gridDocuments.OnExternalDataBound += gridDocuments_OnExternalDataBound;
            gridDocuments.OnDataReload += gridDocuments_OnDataReload;
            gridDocuments.GridView.RowDataBound += new GridViewRowEventHandler(GridView_RowDataBound);
            gridDocuments.GridView.RowCreated += new GridViewRowEventHandler(GridView_RowCreated);
            gridDocuments.ShowActionsMenu = true;

            // Setup where condition
            string where = string.Empty;
            if (Node != null)
            {
                if (ShowAllLevels)
                {
                    string path = aliasPath ?? string.Empty;
                    path = path.TrimEnd('/');
                    where = string.Format("NodeAliasPath LIKE N'{0}/%'", SecurityHelper.GetSafeQueryString(path));
                }
                else
                {
                    where = string.Format("NodeParentID = {0} AND NodeLevel = {1}", Node.NodeID, Node.NodeLevel + 1);

                    // Extend the where condition to include the root document
                    if (RequiresDialog && (Node != null) && (Node.NodeParentID == 0))
                    {
                        where = SqlHelper.AddWhereCondition(where, "NodeParentID = 0", "OR");
                        OrderBy = "NodeParentID ASC" + ((OrderBy.Length > 0) ? "," : "") + OrderBy;
                    }
                }
            }
            where = SqlHelper.AddWhereCondition(where, WhereCodition);
            if (!string.IsNullOrEmpty(where))
            {
                gridDocuments.WhereCondition = where;
            }

            // Initialize columns
            string columns = @"DocumentLastVersionName, DocumentName, NodeParentID,
                    ClassDisplayName, DocumentModifiedWhen, Published, DocumentLastVersionNumber, DocumentMenuRedirectURL, DocumentLastVersionMenuRedirectUrl, DocumentIsArchived, DocumentCheckedOutByUserID,
                    DocumentPublishedVersionHistoryID, DocumentWorkflowStepID, DocumentCheckedOutVersionHistoryID, DocumentNamePath, DocumentPublishFrom, DocumentType, DocumentLastVersionType, NodeAliasPath";

            if (checkPermissions)
            {
                columns = SqlHelper.MergeColumns(columns, TreeProvider.SECURITYCHECK_REQUIRED_COLUMNS);
            }
            gridDocuments.Columns = SqlHelper.MergeColumns(columns, AdditionalColumns);

            // Store the refresh node id. It will be used for refreshing the dialog after dialog actions are performed (move, delete...)
            refreshNodeId = NodeID;
            StringBuilder refreshScripts = new StringBuilder();
            refreshScripts.Append(@"
function RefreshTree()
{
    if((parent != null) && (parent.RefreshTree != null))
    {
        ", (!RequiresDialog)
            ? ("parent.RefreshTree(" + NodeID + @"," + NodeID + ");")
            : ControlsHelper.GetPostBackEventReference(this, "refresh", false, false), @"
    }
}

function ClearSelection()
{ 
", gridDocuments.GetClearSelectionScript(), @"
}
function RefreshGrid()
{
    ClearSelection();
    RefreshTree();
", gridDocuments.GetReloadScript(), @"
}");
            // Register refresh scripts
            string refreshScript = ScriptHelper.GetScript(refreshScripts.ToString());
            ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "refreshListing", refreshScript);

            // Get all possible columns to retrieve
            IDataClass nodeClass = DataClassFactory.NewDataClass("CMS.Tree");
            DocumentInfo di = new DocumentInfo();
            gridDocuments.AllColumns = SqlHelper.MergeColumns(SqlHelper.MergeColumns(di.ColumnNames), SqlHelper.MergeColumns(nodeClass.ColumnNames));
        }
    }


    /// <summary>
    /// Handles the RowCreated event of the GridView control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.Web.UI.WebControls.GridViewRowEventArgs"/> instance containing the event data.</param>
    protected void GridView_RowCreated(object sender, GridViewRowEventArgs e)
    {
        // Reset the indicator
        isRootDocument = false;
        isCurrentDocument = false;
    }


    /// <summary>
    /// Handles the RowDataBound event of the GridView control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.Web.UI.WebControls.GridViewRowEventArgs"/> instance containing the event data.</param>
    protected void GridView_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            if (isRootDocument)
            {
                // Colorize the root row
                e.Row.CssClass += " OnSiteGridRoot";

                // Hide the action checkbox
                if (e.Row.Cells.Count > 0)
                {
                    e.Row.Cells[0].Controls.Clear();
                }
            }

            if (isCurrentDocument)
            {
                // Colorize the current document row
                e.Row.CssClass += " OnSiteGridCurrentDocument";
            }
        }
    }


    /// <summary>
    /// OnPreRender.
    /// </summary>
    protected override void OnPreRender(EventArgs e)
    {
        gridDocuments.StopProcessing = StopProcessing;
        if (StopProcessing)
        {
            return;
        }

        if (!dataLoaded)
        {
            gridDocuments.ReloadData();
        }

        // Hide column with languages if only one culture is assigned to the site
        if (DataHelper.DataSourceIsEmpty(SiteCultures) || (SiteCultures.Tables[0].Rows.Count <= 1))
        {
            // Hide column with flags
            if (gridDocuments.NamedColumns.ContainsKey("documentculture"))
            {
                gridDocuments.NamedColumns["documentculture"].Visible = false;
            }

            // Hide language filter 
            if (gridDocuments.FilterFields.ContainsKey("NodeID"))
            {
                gridDocuments.FilterFields["NodeID"].FilterRow.Visible = false;
            }
        }
        else
        {
            if (FlagsControls.Count != 0)
            {
                // Get all document node IDs
                string nodeIds = null;
                foreach (DocumentFlagsControl ucDocFlags in FlagsControls)
                {
                    nodeIds += ucDocFlags.NodeID + ",";
                }

                if (nodeIds != null)
                {
                    nodeIds = nodeIds.TrimEnd(',');
                }

                // Get all documents
                Tree.SelectQueryName = "SelectVersions";

                string columns = "NodeID, VersionNumber, DocumentCulture, DocumentModifiedWhen, DocumentLastPublished";

                if (checkPermissions)
                {
                    columns = SqlHelper.MergeColumns(columns, TreeProvider.SECURITYCHECK_REQUIRED_COLUMNS);
                }

                DataSet dsDocs = null;
                if (!checkPermissions || (MembershipContext.AuthenticatedUser.IsAuthorizedPerDocument(Node, NodePermissionsEnum.Read) == AuthorizationResultEnum.Allowed))
                {
                    dsDocs = Tree.SelectNodes(currentSiteName, "/%", TreeProvider.ALL_CULTURES, false, null, "NodeID IN (" + nodeIds + ")", null, -1, false, 0, columns);
                }

                // Check permissions
                if (checkPermissions)
                {
                    dsDocs = TreeSecurityProvider.FilterDataSetByPermissions(dsDocs, NodePermissionsEnum.Read, currentUserInfo);
                }

                if (!DataHelper.DataSourceIsEmpty(dsDocs))
                {
                    GroupedDataSource gDSDocs = new GroupedDataSource(dsDocs, "NodeID");

                    // Initialize the document flags controls
                    foreach (DocumentFlagsControl ucDocFlags in FlagsControls)
                    {
                        ucDocFlags.DataSource = gDSDocs;
                        ucDocFlags.ReloadData();
                    }
                }
            }
        }

        base.OnPreRender(e);
    }

    #endregion


    #region "Control events"

    /// <summary>
    /// Ensures correct style for document filter.
    /// </summary>
    protected void gridDocuments_OnFilterFieldCreated(string columnName, UniGridFilterField filter)
    {
        switch (columnName.ToLowerCSafe())
        {
            case "classdisplayname":
            case "documentname":
                if (filter.ValueControl != null)
                {
                    ((TextBox)filter.ValueControl).CssClass = "LongFilterTextBox";
                }
                if (filter.OptionsControl != null)
                {
                    ((DropDownList)filter.OptionsControl).CssClass = "LongDropDownList";
                }

                break;
        }
    }


    protected DataSet gridDocuments_OnDataReload(string completeWhere, string currentOrder, int currentTopN, string columns, int currentOffset, int currentPageSize, ref int totalRecords)
    {
        dataLoaded = true;

        if (Node != null)
        {
            // Get the site
            SiteInfo si = SiteInfoProvider.GetSiteInfo(Node.NodeSiteID);
            if (si != null)
            {
                DataSet ds = null;
                if (!checkPermissions || (MembershipContext.AuthenticatedUser.IsAuthorizedPerDocument(Node, NodePermissionsEnum.Read) == AuthorizationResultEnum.Allowed))
                {
                    // Get documents
                    columns = SqlHelper.MergeColumns(TreeProvider.SELECTNODES_REQUIRED_COLUMNS, columns);
                    ds = DocumentHelper.GetDocuments(currentSiteName, TreeProvider.ALL_DOCUMENTS, TreeProvider.ALL_CULTURES, true, null, completeWhere, OrderBy, -1, false, gridDocuments.TopN, columns, Tree);
                }
                else
                {
                    gridDocuments.ZeroRowsText = GetString("ContentTree.ReadDocumentDenied");
                }

                // Check permissions
                if (checkPermissions)
                {
                    ds = TreeSecurityProvider.FilterDataSetByPermissions(ds, NodePermissionsEnum.Read, currentUserInfo);
                }

                // Hide footer when no data
                pnlFooter.Visible = !DataHelper.DataSourceIsEmpty(ds);

                // Set the data source
                return ds;
            }
        }
        return null;
    }

    private string GetNewCultureScript(int nodeId, string cultureCode)
    {
        return "wopener.NewDocumentCulture(" + nodeId + ", '" + cultureCode + "'); CloseDialog(); return false;";
    }

    /// <summary>
    /// External data binding handler.
    /// </summary>
    protected object gridDocuments_OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        int currentNodeId = 0;

        sourceName = sourceName.ToLowerCSafe();
        switch (sourceName)
        {
            case "view":
                {
                    // Dialog view item
                    DataRowView data = ((DataRowView)((GridViewRow)parameter).DataItem);
                    ImageButton btn = ((ImageButton)sender);
                    // Current row is the Root document
                    isRootDocument = (ValidationHelper.GetInteger(data["NodeParentID"], 0) == 0);
                    isCurrentDocument = (ValidationHelper.GetInteger(data["NodeID"], 0) == WOpenerNodeID);
                    string url = string.Empty;
                    string culture = ValidationHelper.GetString(data["DocumentCulture"], string.Empty);

                    // Existing document culture
                    if (culture.ToLowerCSafe() == CultureCode.ToLowerCSafe())
                    {
                        if (!isRootDocument)
                        {
                            url = ResolveUrl(DocumentURLProvider.GetUrl(Convert.ToString(data["NodeAliasPath"])));
                        }
                        else
                        {
                            url = ResolveUrl("~/");
                        }

                        btn.OnClientClick = "ViewItem(" + ScriptHelper.GetString(url) + "); return false;";
                    }
                    // New culture version
                    else
                    {
                        currentNodeId = ValidationHelper.GetInteger(data["NodeID"], 0);
                        btn.OnClientClick = "wopener.NewDocumentCulture(" + currentNodeId + ", '" + CultureCode + "'); CloseDialog(); return false;";
                    }
                }
                break;

            case "edit":
                {
                    DataRowView data = ((DataRowView)((GridViewRow)parameter).DataItem);
                    ImageButton btn = ((ImageButton)sender);
                    string culture = ValidationHelper.GetString(data["DocumentCulture"], string.Empty);
                    currentNodeId = ValidationHelper.GetInteger(data["NodeID"], 0);
                    int nodeParentId = ValidationHelper.GetInteger(data["NodeParentID"], 0);

                    if (!RequiresDialog || (culture.ToLowerCSafe() == CultureCode.ToLowerCSafe()))
                    {
                        // Go to the selected document or create a new culture version when not used in a dialog
                        btn.OnClientClick = "EditItem(" + currentNodeId + ", " + nodeParentId + "); return false;";
                    }
                    else
                    {
                        // New culture version in a dialog
                        btn.OnClientClick = "wopener.NewDocumentCulture(" + currentNodeId + ", '" + CultureCode + "'); CloseDialog(); return false;";
                    }
                }
                break;

            case "delete":
                {
                    // Delete button
                    ImageButton btn = ((ImageButton)sender);

                    // Hide the delete button for the root document
                    btn.Visible = !isRootDocument;
                }
                break;

            case "contextmenu":
                {
                    // Dialog context menu item
                    DataRowView data = ((DataRowView)((GridViewRow)parameter).DataItem);
                    ImageButton btn = ((ImageButton)sender);

                    // Hide the context menu for the root document
                    btn.Visible = !isRootDocument;
                }
                break;

            case "published":
                {
                    // Published state
                    return UniGridFunctions.ColoredSpanYesNo(parameter);
                }

            case "versionnumber":
                {
                    // Version number
                    if (parameter == DBNull.Value)
                    {
                        parameter = "-";
                    }
                    parameter = HTMLHelper.HTMLEncode(parameter.ToString());

                    return parameter;
                }

            case "documentname":
                {
                    // Document name
                    DataRowView data = (DataRowView)parameter;
                    string className = ValidationHelper.GetString(data["ClassName"], string.Empty);
                    string classDisplayName = ValidationHelper.GetString(data["classdisplayname"], null);
                    string name = ValidationHelper.GetString(data["DocumentName"], string.Empty);
                    string culture = ValidationHelper.GetString(data["DocumentCulture"], string.Empty);
                    string cultureString = null;

                    currentNodeId = ValidationHelper.GetInteger(data["NodeID"], 0);
                    int nodeParentId = ValidationHelper.GetInteger(data["NodeParentID"], 0);

                    if (isRootDocument)
                    {
                        // User site name for the root document
                        name = SiteContext.CurrentSiteName;
                    }

                    // Default culture
                    if (culture.ToLowerCSafe() != CultureCode.ToLowerCSafe())
                    {
                        cultureString = " (" + culture + ")";
                    }

                    string tooltip = UniGridFunctions.DocumentNameTooltip(data);

                    StringBuilder sb = new StringBuilder();

                    if (ShowDocumentTypeIcon)
                    {
                        string imageUrl = null;
                        if (className.EqualsCSafe("cms.file", true))
                        {
                            string extension = ValidationHelper.GetString(data["DocumentType"], string.Empty);
                            imageUrl = GetFileIconUrl(extension, "List");
                        }
                        // Use class icons
                        else
                        {
                            imageUrl = ResolveUrl(GetDocumentTypeIconUrl(className));
                        }

                        // Prepare tooltip for document type icon
                        string iconTooltip = "";
                        if (ShowDocumentTypeIconTooltip && (classDisplayName != null))
                        {
                            iconTooltip = string.Format("onmouseout=\"UnTip()\" onmouseover=\"Tip('{0}')\"", HTMLHelper.HTMLEncode(ResHelper.LocalizeString(classDisplayName)));
                        }

                        sb.Append("<img src=\"", imageUrl, "\" class=\"UnigridActionButton\" alt=\"\" ", iconTooltip, "/> ");
                    }

                    string safeName = HTMLHelper.HTMLEncode(TextHelper.LimitLength(name, 50));
                    if (DocumentNameAsLink && !isRootDocument)
                    {
                        string selectFunction = SelectItemJSFunction + "(" + currentNodeId + ", " + nodeParentId + ");";
                        sb.Append("<a href=\"javascript: ", selectFunction, "\"");

                        // Ensure onclick action on mobile devices. This is necessary due to Tip/UnTip functions. They block default click behavior on mobile devices.
                        if (DeviceContext.CurrentDevice.IsMobile)
                        {
                            sb.Append(" ontouchend=\"", selectFunction, "\"");
                        }

                        sb.Append(" onmouseout=\"UnTip()\" onmouseover=\"Tip('", tooltip, "')\">", safeName, cultureString, "</a>");
                    }
                    else
                    {
                        sb.Append(safeName, cultureString);
                    }

                    if (ShowDocumentMarks)
                    {
                        // Prepare parameters
                        int workflowStepId = ValidationHelper.GetInteger(DataHelper.GetDataRowViewValue(data, "DocumentWorkflowStepID"), 0);
                        WorkflowStepTypeEnum stepType = WorkflowStepTypeEnum.Undefined;

                        if (workflowStepId > 0)
                        {
                            WorkflowStepInfo stepInfo = WorkflowStepInfoProvider.GetWorkflowStepInfo(workflowStepId);
                            if (stepInfo != null)
                            {
                                stepType = stepInfo.StepType;
                            }
                        }

                        // Create data container
                        IDataContainer container = new DataRowContainer(data);

                        // Add icons
                        sb.Append(" ", DocumentHelper.GetDocumentMarks(Page, currentSiteName, CultureCode, stepType, container));
                    }

                    return sb.ToString();
                }

            case "documentculture":
                {
                    DocumentFlagsControl ucDocFlags = null;

                    if (OnDocumentFlagsCreating != null)
                    {
                        // Raise event for obtaining custom DocumentFlagControl
                        object result = OnDocumentFlagsCreating(this, sourceName, parameter);
                        ucDocFlags = result as DocumentFlagsControl;

                        // Check if something other than DocumentFlagControl was returned
                        if ((ucDocFlags == null) && (result != null))
                        {
                            return result;
                        }
                    }

                    // Dynamically load document flags control when not created
                    if (ucDocFlags == null)
                    {
                        ucDocFlags = this.LoadUserControl("~/CMSAdminControls/UI/DocumentFlags.ascx") as DocumentFlagsControl;
                    }

                    // Set document flags properties
                    if (ucDocFlags != null)
                    {
                        DataRowView data = (DataRowView)parameter;

                        // Get node ID
                        currentNodeId = ValidationHelper.GetInteger(data["NodeID"], 0);

                        if (!string.IsNullOrEmpty(SelectLanguageJSFunction))
                        {
                            ucDocFlags.SelectJSFunction = SelectLanguageJSFunction;
                        }

                        ucDocFlags.ID = "docFlags" + currentNodeId;
                        ucDocFlags.SiteCultures = SiteCultures;
                        ucDocFlags.NodeID = currentNodeId;
                        ucDocFlags.StopProcessing = true;
                        ucDocFlags.ItemUrl = ResolveUrl(DocumentURLProvider.GetUrl(Convert.ToString(data["NodeAliasPath"])));

                        // Keep the control for later usage
                        FlagsControls.Add(ucDocFlags);
                        return ucDocFlags;
                    }
                }
                break;

            case "modifiedwhen":
            case "modifiedwhentooltip":
                // Modified when
                if (string.IsNullOrEmpty(parameter.ToString()))
                {
                    return string.Empty;
                }
                else
                {
                    DateTime modifiedWhen = ValidationHelper.GetDateTime(parameter, DateTimeHelper.ZERO_TIME);
                    currentUserInfo = currentUserInfo ?? MembershipContext.AuthenticatedUser;
                    currentSiteInfo = currentSiteInfo ?? SiteContext.CurrentSite;

                    bool displayGMT = (sourceName == "modifiedwhentooltip");
                    return TimeZoneHelper.ConvertToUserTimeZone(modifiedWhen, displayGMT, currentUserInfo, currentSiteInfo);
                }

            case "classdisplayname":
            case "classdisplaynametooltip":
                // Localize class display name
                if (!string.IsNullOrEmpty(parameter.ToString()))
                {
                    return HTMLHelper.HTMLEncode(ResHelper.LocalizeString(parameter.ToString()));
                }

                return string.Empty;

            default:
                if (OnExternalAdditionalDataBound != null)
                {
                    return OnExternalAdditionalDataBound(sender, sourceName, parameter);
                }

                break;
        }

        return parameter;
    }

    #endregion


    #region "Private methods"

    private void InitDropdownLists()
    {
        // Init actions and subjects
        if (!RequestHelper.IsPostBack())
        {
            drpAction.Items.Add(new ListItem(GetString("general." + Action.SelectAction), Convert.ToInt32(Action.SelectAction).ToString()));
            drpAction.Items.Add(new ListItem(GetString("general." + Action.Move), Convert.ToInt32(Action.Move).ToString()));
            drpAction.Items.Add(new ListItem(GetString("general." + Action.Copy), Convert.ToInt32(Action.Copy).ToString()));
            drpAction.Items.Add(new ListItem(GetString("general." + Action.Link), Convert.ToInt32(Action.Link).ToString()));
            drpAction.Items.Add(new ListItem(GetString("general." + Action.Delete), Convert.ToInt32(Action.Delete).ToString()));
            if (LicenseHelper.CheckFeature(URLHelper.GetCurrentDomain(), FeatureEnum.TranslationServices) && TranslationServiceHelper.IsTranslationAllowed(currentSiteName) && TranslationServiceHelper.AnyServiceAvailable(currentSiteName))
            {
                drpAction.Items.Add(new ListItem(GetString("general." + Action.Translate), Convert.ToInt32(Action.Translate).ToString()));
            }
            if (currentUserInfo.IsGlobalAdministrator || currentUserInfo.IsAuthorizedPerResource("CMS.Content", "ManageWorkflow"))
            {
                drpAction.Items.Add(new ListItem(GetString("general." + Action.Publish), Convert.ToInt32(Action.Publish).ToString()));
                drpAction.Items.Add(new ListItem(GetString("general." + Action.Archive), Convert.ToInt32(Action.Archive).ToString()));
            }

            drpWhat.Items.Add(new ListItem(GetString("contentlisting." + What.SelectedDocuments), Convert.ToInt32(What.SelectedDocuments).ToString()));
            drpWhat.Items.Add(new ListItem(GetString("contentlisting." + What.AllDocuments), Convert.ToInt32(What.AllDocuments).ToString()));
        }
    }


    /// <summary>
    /// Gets the refresh node ID.
    /// This method checks all parent nodes and indicates whether any of them is contained in the selected node ids list.
    /// If none of the parent nodes is contained in the list, this method returns 0 (it means that the current wopener node can be used for refreshing the dialog).
    /// If any of the parent nodes is contained in the list, this method returns the parent node id (this node will be used for refreshing the dialog).
    /// </summary>
    /// <param name="node">The node</param>
    /// <param name="selectedNodeIds">The selected node ids</param>
    private int GetRefreshNodeID(TreeNode node, List<int> selectedNodeIds)
    {
        if (selectedNodeIds.Contains(node.NodeID) || (node.NodeParentID == 0))
        {
            return node.NodeParentID;
        }

        return GetRefreshNodeID(node.Parent, selectedNodeIds);
    }


    /// <summary>
    /// Sets the refresh node id in the given parameters hash table.
    /// </summary>
    /// <param name="parameters">The parameters</param>
    private void SetRefreshNodeId(Hashtable parameters)
    {
        // Get the current node
        TreeNode wOpenerNode = TreeHelper.SelectSingleNode(WOpenerNodeID);
        if (wOpenerNode != null)
        {
            if (allSelected)
            {
                // Include the parent node in the selected node ids
                List<int> selectedNodeIds = new List<int>();
                selectedNodeIds.Add(NodeID);

                // Get the refresh node id (checks also its parent nodes)
                refreshNodeId = GetRefreshNodeID(wOpenerNode, selectedNodeIds);
            }
            // urlParameter - selected node ids (separated by '|')
            else if (!string.IsNullOrEmpty(urlParameter))
            {
                // Get the selected documents
                string[] selectedNodeIdsString = urlParameter.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                List<int> selectedNodeIds = new List<int>(Array.ConvertAll<string, int>(selectedNodeIdsString, e => ValidationHelper.GetInteger(e, 0)));

                // Get the refresh node id (checks also its parent nodes)
                refreshNodeId = GetRefreshNodeID(wOpenerNode, selectedNodeIds);
            }

            // Set the refreshNodeId to the current document if the current document is not selected (nor any of its parent nodes is selected) for the move action
            if ((refreshNodeId == 0) && (Node != null))
            {
                refreshNodeId = wOpenerNode.NodeID;
            }

            parameters["refreshnodeid"] = refreshNodeId;
        }
    }

    #endregion


    #region "ICallbackEventHandler Members"

    /// <summary>
    /// Returns the results of a callback event that targets a control.
    /// </summary>
    public string GetCallbackResult()
    {
        string returnUrl = string.Empty;
        Hashtable parameters = new Hashtable();

        switch (callbackAction)
        {
            case Action.Copy:
            case Action.Move:
            case Action.Link:

                // Get default dialog URL
                Config.CustomFormatCode = callbackAction.ToString().ToLowerCSafe();

                // Set dialog starting path
                if (!string.IsNullOrEmpty(CopyMoveLinkStartingPath))
                {
                    Config.ContentStartingPath = Server.UrlEncode(CopyMoveLinkStartingPath);
                }

                returnUrl = CMSDialogHelper.GetDialogUrl(Config, false, false, null, false);

                // Adjust URL to our needs
                returnUrl = URLHelper.RemoveParameterFromUrl(returnUrl, "hash");
                returnUrl = URLHelper.AddParameterToUrl(returnUrl, "multiple", "true");

                // Update the refresh node when the Move action is performed
                if ((callbackAction == Action.Move) && RequiresDialog)
                {
                    SetRefreshNodeId(parameters);
                }

                // Process parameters
                if (!string.IsNullOrEmpty(urlParameter))
                {
                    returnUrl = URLHelper.AddParameterToUrl(returnUrl, "sourcenodeids", Server.UrlEncode(urlParameter));
                }
                else if (!string.IsNullOrEmpty(aliasPath))
                {
                    parameters["parentalias"] = aliasPath;
                }

                if (!string.IsNullOrEmpty(CurrentWhereCondition))
                {
                    parameters["where"] = SqlHelper.AddWhereCondition(CurrentWhereCondition, "", "OR");
                }

                break;

            case Action.Delete:
                returnUrl = DeleteReturnUrl;

                // Process parameters
                if (allSelected)
                {
                    if (!string.IsNullOrEmpty(aliasPath))
                    {
                        parameters["parentaliaspath"] = aliasPath;
                    }
                    if (!string.IsNullOrEmpty(CurrentWhereCondition))
                    {
                        parameters["where"] = CurrentWhereCondition;
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(urlParameter))
                    {
                        returnUrl = URLHelper.AddParameterToUrl(returnUrl, "nodeId", Server.UrlEncode(urlParameter));
                    }
                }

                // Update the refresh node when the Delete action is performed
                if (RequiresDialog)
                {
                    SetRefreshNodeId(parameters);
                }

                break;

            case Action.Archive:
            case Action.Publish:
                returnUrl = callbackAction == Action.Archive ? ArchiveReturnUrl : PublishReturnUrl;
                returnUrl = URLHelper.AddParameterToUrl(returnUrl, "action", callbackAction.ToString());

                // Process parameters
                if (allSelected)
                {
                    if (!string.IsNullOrEmpty(aliasPath))
                    {
                        parameters["parentaliaspath"] = aliasPath;
                    }
                    if (!string.IsNullOrEmpty(CurrentWhereCondition))
                    {
                        parameters["where"] = CurrentWhereCondition;
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(urlParameter))
                    {
                        parameters["nodeids"] = urlParameter;
                    }
                }
                break;

            case Action.Translate:
                returnUrl = TranslateReturnUrl;

                // Process parameters
                if (allSelected)
                {
                    if (!string.IsNullOrEmpty(aliasPath))
                    {
                        parameters["parentaliaspath"] = aliasPath;
                    }
                    if (!string.IsNullOrEmpty(CurrentWhereCondition))
                    {
                        parameters["where"] = CurrentWhereCondition;
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(urlParameter))
                    {
                        parameters["nodeids"] = urlParameter;
                    }
                }
                break;
        }

        // Store parameters to window helper
        WindowHelper.Add(Identifier, parameters);

        // Add parameters identifier and hash, encode query string
        returnUrl = URLHelper.AddParameterToUrl(returnUrl, "params", Identifier);
        returnUrl = ResolveUrl(returnUrl);
        returnUrl = URLHelper.AddParameterToUrl(returnUrl, "hash", QueryHelper.GetHash(URLHelper.GetQuery(returnUrl)));

        return returnUrl;
    }


    /// <summary>
    /// Processes a callback event that targets a control.
    /// </summary>
    public void RaiseCallbackEvent(string eventArgument)
    {
        string[] arguments = eventArgument.Trim('|').Split('|');
        if (arguments.Length > 1)
        {
            // Parse callback arguments
            callbackAction = (Action)Enum.Parse(typeof(Action), arguments[(int)Argument.Action]);
            allSelected = ValidationHelper.GetBoolean(arguments[(int)Argument.AllSelected], false);
            if (!allSelected)
            {
                // Get selected node identifiers
                for (int i = (int)Argument.Items; i < arguments.Length; i++)
                {
                    urlParameter += arguments[i] + "|";
                }
            }
        }
    }

    #endregion


    #region "IPostBackEventHandler members"

    public void RaisePostBackEvent(string eventArgument)
    {
        if (eventArgument == "move")
        {
            // Keep current user object
            CurrentUserInfo cu = MembershipContext.AuthenticatedUser;

            // Parse input value
            string[] values = hdnMoveId.Value.Split(';');

            // Create tree provider
            TreeProvider tree = new TreeProvider(cu);

            // Get tree node object
            int nodeId = ValidationHelper.GetInteger(values[1], 0);
            TreeNode node = tree.SelectSingleNode(nodeId);

            // Check whether node exists
            if (node == null)
            {
                ShowError(GetString("ContentRequest.ErrorMissingSource"));
                return;
            }

            try
            {
                // Check permissions
                if (cu.IsAuthorizedPerDocument(node, NodePermissionsEnum.Modify) == AuthorizationResultEnum.Allowed)
                {
                    // Switch by action
                    switch (values[0])
                    {
                        case "up":
                            node = tree.MoveNodeUp(nodeId);
                            break;

                        case "down":
                            node = tree.MoveNodeDown(nodeId);
                            break;

                        case "top":
                            node = tree.SelectSingleNode(nodeId);
                            tree.SetNodeOrder(nodeId, DocumentOrderEnum.First);
                            break;

                        case "bottom":
                            node = tree.SelectSingleNode(nodeId);
                            tree.SetNodeOrder(nodeId, DocumentOrderEnum.Last);
                            break;
                    }

                    if (node != null)
                    {
                        if (!RequiresDialog)
                        {
                            ScriptHelper.RegisterStartupScript(this, typeof(string), "refreshAfterMove", ScriptHelper.GetScript("parent.RefreshTree(" + node.NodeParentID + ", " + node.NodeParentID + ");"));
                        }

                        // Log the synchronization tasks for the entire tree level
                        DocumentSynchronizationHelper.LogDocumentChangeOrder(node.NodeSiteName, node.NodeAliasPath, tree);
                    }
                    else
                    {
                        ShowError(GetString("ContentRequest.MoveFailed"));
                    }
                }
                else
                {
                    ShowError(GetString("ContentRequest.MoveDenied"));
                }
            }
            catch (Exception ex)
            {
                EventLogProvider log = new EventLogProvider();
                log.LogEvent(EventLogProvider.ERROR, DateTime.Now, "Content", "MOVE", cu.UserID, cu.UserName, nodeId, node.DocumentName, RequestContext.UserHostAddress, EventLogProvider.GetExceptionLogMessage(ex), SiteContext.CurrentSite.SiteID, HTTPHelper.GetAbsoluteUri());

                ShowError(GetString("ContentRequest.MoveFailed") + " : " + ex.Message);
            }
        }
        else if (eventArgument == "refresh")
        {
            // Register the refresh script after the 'move' action is performed
            Hashtable parameters = WindowHelper.GetItem(Identifier) as Hashtable;
            if ((parameters != null) && (parameters.Count > 0))
            {
                int refreshNodeId = ValidationHelper.GetInteger(parameters["refreshnodeid"], 0);
                string refreshScript = "parent.RefreshTree(" + refreshNodeId + ", " + refreshNodeId + ")";
                ScriptHelper.RegisterStartupScript(this, typeof(string), "refreshAfterMove", refreshScript, true);
            }
            else
            {
                // If node id not found refresh whole content tree
                ScriptHelper.RegisterStartupScript(this, typeof(string), "refreshAllAfterMove", "parent.RefreshTree(" + NodeID + ", " + NodeID + ")", true);
            }
        }
    }

    #endregion
}
