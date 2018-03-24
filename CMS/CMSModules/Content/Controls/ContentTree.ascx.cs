using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.CMSHelper;
using CMS.CMSSiteMapProvider;
using CMS.EventLog;
using CMS.ExtendedControls.DragAndDrop;
using CMS.GlobalHelper;
using CMS.SettingsProvider;
using CMS.DocumentEngine;
using CMS.WorkflowEngine;
using CMS.UIControls;

using TreeNode = CMS.DocumentEngine.TreeNode;
using CMS.Localization;
using CMS.Helpers;
using CMS.SiteProvider;
using CMS.Membership;
using CMS.DataEngine;
using CMS.Base;

public partial class CMSModules_Content_Controls_ContentTree : ContentActionsControl, ICallbackEventHandler
{
    #region "Variables"

    private string mNodeTextTemplate = "##ICON####NODENAME##";
    private string mSelectedNodeTextTemplate = "##ICON####NODENAME##";
    private int mSelectedNodeId = 0;
    private int mExpandNodeId = 0;

    private bool selectedRendered = false;
    private readonly Hashtable allNodes = new Hashtable();

    private string mPath = "/";
    private string mAlternativePath = "/";
    private string mBasePath = null;
    private bool? mCheckPermissions = null;

    protected TreeSiteMapProvider mMapProvider = null;
    private TreeProvider mMapTreeProvider = null;

    private int mMaxTreeNodes = 0;
    private string mMaxTreeNodeText = null;
    private string mPreferredCulture = null;
    private bool mAllowMarks = true;
    private string mCMSFileIconSet = string.Empty;
    private bool mUseCMSFileIcons = true;
    private bool mDeniedNodePostback = true;
    private bool mAllowDragAndDrop = false;
    private string mCallbackResult = null;

    private TreeNode mSelectedNode = null;


    /// <summary>
    /// Nodes to expand.
    /// </summary>
    protected List<int> expandNodes = new List<int>();

    #endregion


    #region "Events"

    public event EventHandler RootNodeCreated;

    #endregion


    #region "Properties"

    /// <summary>
    /// If set to TRUE, file type icons are used instead of the class icons for 'cms.file' documents. 
    /// File type icon corresponds to the extension of the attachment which is included in 'cms.file' document.
    /// </summary>
    public bool UseCMSFileIcons
    {
        get
        {
            return mUseCMSFileIcons;
        }
        set
        {
            mUseCMSFileIcons = value;
        }
    }


    /// <summary>
    /// Folder name where file type icons are located. "List" icon set is used by default.
    /// </summary>
    public string CMSFileIconSet
    {
        get
        {
            if (mCMSFileIconSet == string.Empty)
            {
                return "List";
            }

            return mCMSFileIconSet;
        }
        set
        {
            mCMSFileIconSet = value;
        }
    }


    /// <summary>
    /// True if the special marks (NOTTRANSLATED, REDIRECTION, ...) should be rendered.
    /// </summary>
    public bool AllowMarks
    {
        get
        {
            return mAllowMarks;
        }
        set
        {
            mAllowMarks = value;
        }
    }


    /// <summary>
    /// Preferred tree culture.
    /// </summary>
    public string PreferredCulture
    {
        get
        {
            return mPreferredCulture ?? (mPreferredCulture = LocalizationContext.PreferredCultureCode);
        }
        set
        {
            mPreferredCulture = value;
        }
    }


    /// <summary>
    /// Culture the tree is working with. Uses preferred culture when not set.
    /// </summary>
    public string Culture
    {
        get
        {
            if (ViewState["Culture"] == null)
            {
                ViewState["Culture"] = PreferredCulture;
            }
            return (string)ViewState["Culture"];
        }
        set
        {
            ViewState["Culture"] = value;
            // Set the culture to MapTreeProvider in case it already has been initialized
            MapTreeProvider.PreferredCultureCode = value;
        }
    }


    /// <summary>
    /// Template of the node text, use {0} to insert the original node text, {1} to insert the Node ID.
    /// </summary>
    public string NodeTextTemplate
    {
        get
        {
            return mNodeTextTemplate;
        }
        set
        {
            mNodeTextTemplate = value;
        }
    }


    /// <summary>
    /// Template of the SelectedNode text, use {0} to insert the original SelectedNode text, {1} to insert the SelectedNode ID.
    /// </summary>
    public string SelectedNodeTextTemplate
    {
        get
        {
            return mSelectedNodeTextTemplate;
        }
        set
        {
            mSelectedNodeTextTemplate = value;
        }
    }


    /// <summary>
    /// Gets or sets the current node ID.
    /// </summary>
    public int SelectedNodeID
    {
        get
        {
            return mSelectedNodeId;
        }
        set
        {
            mSelectedNodeId = value;
        }
    }


    /// <summary>
    /// Selected node document.
    /// </summary>
    public TreeNode SelectedNode
    {
        get
        {
            if (mSelectedNode == null)
            {
                // Set preferred culture
                TreeProvider.PreferredCultureCode = Culture;
                // Get the document
                mSelectedNode = TreeProvider.SelectSingleNode(SelectedNodeID, TreeProvider.ALL_CULTURES, true);

                if (!SelectPublishedData)
                {
                    mSelectedNode = DocumentHelper.GetDocument(mSelectedNode, TreeProvider);
                }

                // Fall back to root if selected node doesn't exist or its alias path is outside of user's starting path 
                if (UseFallbackToRoot(mSelectedNode))
                {
                    mSelectedNode = TreeProvider.SelectSingleNode(RootNodeID, TreeProvider.ALL_CULTURES, true);
                    ScriptHelper.RegisterStartupScript(this, typeof(string), "fallbackToRoot", GetFallBackToRootScript(false), true);
                }
            }

            return mSelectedNode;
        }
    }


    /// <summary>
    /// Gets or sets the node ID to expand.
    /// </summary>
    public int ExpandNodeID
    {
        get
        {
            return mExpandNodeId;
        }
        set
        {
            mExpandNodeId = value;
        }
    }


    /// <summary>
    /// Site name to display.
    /// </summary>
    public string SiteName
    {
        get
        {
            string siteName = ValidationHelper.GetString(ViewState["SiteName"], string.Empty);
            if ((siteName == string.Empty) && (SiteContext.CurrentSite != null))
            {
                siteName = SiteContext.CurrentSiteName;
            }
            return siteName;
        }
        set
        {
            ViewState["SiteName"] = value;
            MapProvider.SiteName = value;
        }
    }


    /// <summary>
    /// Nodes selecting path.
    /// </summary>
    public string Path
    {
        get
        {
            return mPath;
        }
        set
        {
            mPath = value;
            MapProvider.Path = value;
        }
    }


    /// <summary>
    /// Alternative path when the defined path not found.
    /// </summary>
    public string AlternativePath
    {
        get
        {
            return mAlternativePath;
        }
        set
        {
            mAlternativePath = value;
            MapProvider.AlternativePath = value;
        }
    }


    /// <summary>
    /// Indicates if the permissions should be checked.
    /// </summary>
    public bool CheckPermissions
    {
        get
        {
            if (mCheckPermissions == null)
            {
                mCheckPermissions = TreeProvider.CheckDocumentUIPermissions(SiteName);
            }
            return mCheckPermissions.Value;
        }
        set
        {
            mCheckPermissions = value;
            MapProvider.CheckPermissions = value;
        }
    }


    /// <summary>
    /// Tree provider.
    /// </summary>
    private TreeProvider MapTreeProvider
    {
        get
        {
            if (mMapTreeProvider == null)
            {
                mMapTreeProvider = new TreeProvider(MembershipContext.AuthenticatedUser);
                mMapTreeProvider.SelectQueryName = "selecttree";
                mMapTreeProvider.PreferredCultureCode = Culture;
            }

            return mMapTreeProvider;
        }
    }


    /// <summary>
    /// Sitemap provider that the tree uses.
    /// </summary>
    public TreeSiteMapProvider MapProvider
    {
        get
        {
            if (mMapProvider == null)
            {
                mMapProvider = new TreeSiteMapProvider();

                mMapProvider.TreeProvider = MapTreeProvider;
                mMapProvider.CultureCode = TreeProvider.ALL_CULTURES;
                mMapProvider.SiteName = SiteName;
                mMapProvider.BindNodeData = true;
                mMapProvider.OrderBy = "NodeOrder ASC, NodeName ASC, NodeAlias ASC";
                mMapProvider.SelectOnlyPublished = false;
                mMapProvider.CombineWithDefaultCulture = true;
                mMapProvider.Path = Path;
                mMapProvider.AlternativePath = AlternativePath;
                mMapProvider.MaxRelativeLevel = 1;
                mMapProvider.MaxTreeNodes = MaxTreeNodes + 1;
                mMapProvider.CheckPermissions = CheckPermissions;
            }

            return mMapProvider;
        }
    }


    /// <summary>
    /// Root node.
    /// </summary>
    public TreeSiteMapNode RootNode
    {
        get
        {
            return (TreeSiteMapNode)MapProvider.RootNode;
        }
    }


    /// <summary>
    /// Root node identifier.
    /// </summary>
    public int RootNodeID
    {
        get
        {
            return (int)RootNode.GetValue("NodeID");
        }
    }


    /// <summary>
    /// Maximum number of tree nodes displayed within the tree.
    /// </summary>
    public int MaxTreeNodes
    {
        get
        {
            if (mMaxTreeNodes <= 0)
            {
                mMaxTreeNodes = SettingsKeyInfoProvider.GetIntValue(SiteContext.CurrentSiteName + ".CMSMaxTreeNodes");
            }
            return mMaxTreeNodes;
        }
        set
        {
            mMaxTreeNodes = value;
            MapProvider.MaxTreeNodes = value + 1;
        }
    }


    /// <summary>
    /// Text to appear within the latest node when max tree nodes applied.
    /// </summary>
    public string MaxTreeNodeText
    {
        get
        {
            return mMaxTreeNodeText ?? (mMaxTreeNodeText = GetString("ContentTree.SeeListing"));
        }
        set
        {
            mMaxTreeNodeText = value;
        }
    }


    /// <summary>
    /// Indicates whether only published document should be displayed.
    /// </summary>
    public bool SelectOnlyPublished
    {
        get
        {
            return MapProvider.SelectOnlyPublished;
        }
        set
        {
            MapProvider.SelectOnlyPublished = value;
        }
    }


    /// <summary>
    /// Indicates whether published document data should be displayed.
    /// </summary>
    public bool SelectPublishedData
    {
        get
        {
            return MapProvider.SelectPublishedData;
        }
        set
        {
            MapProvider.SelectPublishedData = value;
        }
    }


    /// <summary>
    /// Indicates whether access denied node causes postback.
    /// </summary>
    public bool DeniedNodePostback
    {
        get
        {
            return mDeniedNodePostback;
        }
        set
        {
            mDeniedNodePostback = value;
        }
    }


    /// <summary>
    /// True if dragging / dropping of the items is allowed
    /// </summary>
    public bool AllowDragAndDrop
    {
        get
        {
            return mAllowDragAndDrop;
        }
        set
        {
            mAllowDragAndDrop = value;
        }
    }

    #endregion


    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        // Enable or disable link action according to user's UI Profile
        if (MembershipContext.AuthenticatedUser.IsAuthorizedPerUIElement("CMS.Content", "New") &&
            MembershipContext.AuthenticatedUser.IsAuthorizedPerUIElement("CMS.Content", "New.LinkExistingDocument"))
        {
            ltlCaptureCueCtrlShift.Text = ScriptHelper.GetScript("var captureCueCtrlShift = true;");
        }
        else
        {
            ltlCaptureCueCtrlShift.Text = ScriptHelper.GetScript("var captureCueCtrlShift = false;");
        }

        bool isRTL = CultureHelper.IsUICultureRTL();
        if (IsLiveSite)
        {
            isRTL = CultureHelper.IsPreferredCultureRTL();
        }

        treeElem.LineImagesFolder = GetImageUrl(isRTL ? "RTL/Design/Controls/Tree" : "Design/Controls/Tree", IsLiveSite, false);
        treeElem.ImageSet = TreeViewImageSet.Custom;
        treeElem.ExpandImageToolTip = GetString("general.expand");
        treeElem.CollapseImageToolTip = GetString("general.collapse");
        mBasePath = URLHelper.Url.LocalPath;

        if (!RequestHelper.IsCallback() && !RequestHelper.IsPostBack())
        {
            // Register tree progress icon
            ScriptHelper.RegisterTreeProgress(Page);

            if (AllowDragAndDrop)
            {
                ScriptHelper.RegisterStartupScript(this, typeof(string), "reinitDrag", ScriptHelper.GetScript(
                    @"
if (TreeView_ProcessNodeData) { base_TreeView_ProcessNodeData = TreeView_ProcessNodeData };
TreeView_ProcessNodeData = function(result, context) {
    if (base_TreeView_ProcessNodeData) { base_TreeView_ProcessNodeData(result, context) }
    if (window.lastDragAndDropBehavior) { lastDragAndDropBehavior._initializeDraggableItems(); } 
}
"));
            }
        }
    }


    protected override void CreateChildControls()
    {
        base.CreateChildControls();

        if (!Page.IsCallback)
        {
            plcDrag.Visible = AllowDragAndDrop;

            if (AllowDragAndDrop)
            {
                // Create drag and drop extender
                DragAndDropExtender extDragDrop = new DragAndDropExtender();

                extDragDrop.ID = "extDragDrop";
                extDragDrop.TargetControlID = "pnlTree";
                extDragDrop.DragItemClass = "DDItem";
                extDragDrop.DragItemHandleClass = "DDHandle";
                extDragDrop.DropCueID = "pnlCue";
                extDragDrop.OnClientDrop = "OnDropNode";

                plcDrag.Controls.Add(extDragDrop);

                pnlCue.Style.Add("display", "none");

                string script =
                    @"
function MoveNodeAsync(nodeId, targetNodeId, position, copy, link) {
    var param = '';
    if (link) {
        if (position) {
            param = 'LinkNodePosition';
        } else {
            param = 'LinkNode';
        }
    } else if (copy) {
        if (position) {
            param = 'CopyNodePosition';
        } else {
            param = 'CopyNode';
        }
    } else {
        if (position) {
            param = 'MoveNodePosition';
        } else {
            param = 'MoveNode';
        }
    }
    param += ';' + nodeId + ';' + targetNodeId;" +
                    GetEventReference("param") + @"
}"
                    ;

                ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "ContentTree", ScriptHelper.GetScript(script));
            }
            else
            {
                pnlCue.Visible = false;
            }
        }
    }


    /// <summary>
    /// Gets the event reference to the action.
    /// </summary>
    /// <param name="eventArgument">Action to perform</param>
    protected string GetEventReference(string eventArgument)
    {
        return Page.ClientScript.GetCallbackEventReference(this, eventArgument, "DragActionDone", null);
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        if (!Page.IsCallback)
        {
            ReloadData();

            // If selected node was not rendered (on original load), load it
            if (!selectedRendered && (SelectedNode != null))
            {
                // Ensure all parents of the selected node
                foreach (int nodeId in expandNodes)
                {
                    EnsureNode(null, nodeId);
                }

                // Ensure the node itself
                EnsureNode(SelectedNode, 0);
            }
        }
    }


    /// <summary>
    /// Ensures the given node within the tree.
    /// </summary>
    /// <param name="node">Node to ensure</param>
    /// <param name="nodeId">Ensure by NodeID</param>
    protected void EnsureNode(TreeNode node, int nodeId)
    {
        if (node == null)
        {
            // If not already exists, do not add
            if (allNodes[nodeId] != null)
            {
                return;
            }
            else
            {
                // Get the node
                node = TreeProvider.SelectSingleNode(nodeId, TreeProvider.ALL_CULTURES, true);

                if (!SelectPublishedData)
                {
                    node = DocumentHelper.GetDocument(node, TreeProvider);
                }
            }
        }
        else
        {
            nodeId = node.NodeID;
        }

        if (node != null)
        {
            // Get the correct parent node
            System.Web.UI.WebControls.TreeNode parentNode = (System.Web.UI.WebControls.TreeNode)allNodes[node.NodeParentID];
            if (parentNode != null)
            {
                // Expand the parent
                parentNode.Expanded = true;

                // If still not present, add the node
                if (allNodes[nodeId] == null)
                {
                    TreeSiteMapNode sourceNode = new TreeSiteMapNode(MapProvider, nodeId.ToString());
                    sourceNode.TreeNode = node;

                    System.Web.UI.WebControls.TreeNode newNode = CreateNode(sourceNode, 0, true);

                    parentNode.ChildNodes.Add(newNode);
                }
            }
            else
            {
                // Get the correct node and add it to list of processed nodes
                TreeSiteMapNode targetNode = MapProvider.GetNodeByAliasPath(node.NodeAliasPath);

                if (targetNode != null)
                {
                    List<int> procNodes = new List<int>();
                    procNodes.Add((int)targetNode.NodeData["NodeID"]);

                    if (targetNode.ParentNode != null)
                    {
                        // Repeat until existing parent node in allNodes is found
                        do
                        {
                            int targetParentNodeId = (int)((TreeSiteMapNode)targetNode.ParentNode).NodeData["NodeID"];
                            procNodes.Add(targetParentNodeId);
                            targetNode = (TreeSiteMapNode)targetNode.ParentNode;
                        } while ((targetNode.ParentNode != null) && (allNodes[(int)(((TreeSiteMapNode)(targetNode.ParentNode)).NodeData["NodeID"])] == null));
                    }

                    // Process nodes in reverse order
                    procNodes.Reverse();
                    if (!procNodes.Any(p => (p <= 0)))
                    {
                        foreach (int nodeID in procNodes)
                        {
                            EnsureNode(null, nodeID);
                        }
                    }
                }
            }
        }
    }

    #endregion


    #region "Tree management methods"

    public void ReloadData()
    {
        try
        {
            MapProvider.ReloadData();

            // Expand current node parent
            if ((ExpandNodeID <= 0) && (SelectedNode != null))
            {
                ExpandNodeID = SelectedNode.NodeParentID;
            }

            // If expand node set, set the node to expand
            if (ExpandNodeID > 0)
            {
                // Get node list to expand
                expandNodes.Clear();

                TreeNode node = TreeProvider.SelectSingleNode(ExpandNodeID, TreeProvider.ALL_CULTURES);
                if (node != null)
                {
                    TreeSiteMapNode targetNode = MapProvider.GetNodeByAliasPath(node.NodeAliasPath);
                    if (targetNode != null)
                    {
                        int targetNodeId = (int)targetNode.NodeData["NodeID"];
                        expandNodes.Add(targetNodeId);
                        while (targetNode.ParentNode != null)
                        {
                            int targetParentNodeId = (int)((TreeSiteMapNode)targetNode.ParentNode).NodeData["NodeID"];
                            expandNodes.Add(targetParentNodeId);
                            targetNode = (TreeSiteMapNode)targetNode.ParentNode;
                        }
                    }
                }
            }

            treeElem.Nodes.Clear();

            // Add root node
            treeElem.Nodes.Add(CreateNode(RootNode, 0, false));

            // Raise root node created event
            RaiseRootNodeCreated();
        }
        catch (Exception ex)
        {
            lblError.Text = GetString("ContentTree.FailedLoad") + ": " + ex.Message;
            lblError.ToolTip = EventLogProvider.GetExceptionLogMessage(ex);
        }
    }


    protected void RaiseRootNodeCreated()
    {

        if (RootNodeCreated != null)
        {
            RootNodeCreated(treeElem, EventArgs.Empty);
        }
    }


    protected void treeElem_TreeNodePopulate(object sender, TreeNodeEventArgs e)
    {
        e.Node.ChildNodes.Clear();
        e.Node.PopulateOnDemand = false;

        int nodeId = ValidationHelper.GetInteger(e.Node.Value, 0);
        TreeNode node = TreeProvider.SelectSingleNode(nodeId);

        // Check explore tree permission for current node
        bool userHasExploreTreePermission = (MembershipContext.AuthenticatedUser.IsAuthorizedPerDocument(node, NodePermissionsEnum.ExploreTree) == AuthorizationResultEnum.Allowed);
        if (userHasExploreTreePermission)
        {
            SiteMapNodeCollection childNodes = MapProvider.GetChildNodes(nodeId);
            int index = 0;
            foreach (TreeSiteMapNode childNode in childNodes)
            {
                int childNodeId = (int)childNode.NodeData["NodeID"];
                if (childNodeId != nodeId)
                {
                    System.Web.UI.WebControls.TreeNode newNode = CreateNode(childNode, index, true);
                    e.Node.ChildNodes.Add(newNode);
                    index++;
                }
            }
        }
        else
        {
            // Add 'access denied node'
            System.Web.UI.WebControls.TreeNode tempNode = new System.Web.UI.WebControls.TreeNode();
            tempNode.Text = GetString("ContentTree.ExploreChildsDenied");
            tempNode.NavigateUrl = (DeniedNodePostback ? mBasePath + "#" : string.Empty);
            e.Node.ChildNodes.Add(tempNode);
        }
    }


    /// <summary>
    /// Creates the tree node.
    /// </summary>
    /// <param name="sourceNode">Source node</param>
    /// <param name="index">Node index</param>
    /// <param name="childNode">True if the node is child node</param>
    protected System.Web.UI.WebControls.TreeNode CreateNode(TreeSiteMapNode sourceNode, int index, bool childNode)
    {
        System.Web.UI.WebControls.TreeNode newNode = new System.Web.UI.WebControls.TreeNode();
        ISimpleDataContainer container = sourceNode;

        int nodeId = (int)container.GetValue("NodeID");
        int nodeLevel = (int)container.GetValue("NodeLevel");

        if (nodeId < 0)
        {
            newNode.SelectAction = TreeNodeSelectAction.None;
            newNode.Text = GetString("ContentTree.ReadDocumentDenied");
            newNode.NavigateUrl = (DeniedNodePostback ? mBasePath + "#" : string.Empty);
            return newNode;
        }

        if ((index < MaxTreeNodes) || (nodeLevel <= MapProvider.RootNodeLevel + 1))
        {
            allNodes[nodeId] = newNode;

            // Set the base data
            newNode.Value = nodeId.ToString();
            newNode.NavigateUrl = "javascript:void(0);";

            int classId = ValidationHelper.GetInteger(container.GetValue("NodeClassID"), 0);
            DataClassInfo ci = DataClassInfoProvider.GetDataClass(classId);
            if (ci == null)
            {
                throw new Exception("[ContentTree.CreateNode]: Node class not found.");
            }

            string className = ci.ClassName.ToLowerCSafe();
            string imageUrl = string.Empty;
            string tooltip = string.Empty;

            // Use file type icons for cms.file
            if (UseCMSFileIcons && (className == "cms.file"))
            {
                string extension = ValidationHelper.GetString(container.GetValue("DocumentType"), string.Empty);
                imageUrl = GetFileIconUrl(extension, CMSFileIconSet);
                tooltip = " title=\"" + extension.ToLowerCSafe().TrimStart('.') + "\" ";
            }
            // Use class icons
            else
            {
                imageUrl = GetDocumentTypeIconUrl(className);
            }

            StringBuilder sb = new StringBuilder();
            sb.Append("<img src=\"", imageUrl, "\" alt=\"\" style=\"border:0px;vertical-align:middle;\" onclick=\"return false;\"", tooltip, " class=\"", (className == "cms.root" ? "Image20" : "Image16"), "\" />");
            string imageTag = sb.ToString();

            string nodeName = HttpUtility.HtmlEncode(ValidationHelper.GetString(container.GetValue("DocumentName"), string.Empty));
            string nodeNameJava = ScriptHelper.GetString(nodeName);

            // Render special marks only if allowed
            if (AllowMarks)
            {
                int workflowStepId = ValidationHelper.GetInteger(container.GetValue("DocumentWorkflowStepID"), 0);
                WorkflowStepTypeEnum stepType = WorkflowStepTypeEnum.Undefined;

                if (workflowStepId > 0)
                {
                    WorkflowStepInfo stepInfo = WorkflowStepInfoProvider.GetWorkflowStepInfo(workflowStepId);
                    if (stepInfo != null)
                    {
                        stepType = stepInfo.StepType;
                    }
                }

                // Add icons
                nodeName += DocumentHelper.GetDocumentMarks(Page, SiteName, Culture, stepType, sourceNode);
            }

            string template = null;

            if ((SelectedNode != null) && (nodeId == SelectedNode.NodeID))
            {
                template = SelectedNodeTextTemplate;
                selectedRendered = true;
            }
            else
            {
                template = NodeTextTemplate;
            }

            // Prepare the node text
            newNode.Text = ResolveNode(template, nodeName, imageTag, nodeNameJava, nodeId);

            int childNodesCount = ValidationHelper.GetInteger(container.GetValue("NodeChildNodesCount"), 0);
            newNode.Text = newNode.Text.Replace("##NODECHILDNODESCOUNT##", childNodesCount.ToString());

            // Drag and drop envelope
            if (AllowDragAndDrop)
            {
                sb.Length = 0;

                if (childNode)
                {
                    sb.Append("<span id=\"target_", nodeId, "\"><span class=\"DDItem\" id=\"node_", nodeId, "\"><span class=\"DDHandle\" id=\"handle_", nodeId, "\" onmousedown=\"return false;\" onclick=\"return false;\">", newNode.Text, "</span></span></span>");
                }
                else
                {
                    sb.Append("<span id=\"target_", nodeId, "\" class=\"RootNode\"><span class=\"DDItem\" id=\"node_", nodeId, "\">", newNode.Text, "</span></span>");
                }

                newNode.Text = sb.ToString();
            }

            // Check if can expand
            if (childNodesCount == 0)
            {
                newNode.PopulateOnDemand = false;
                newNode.Expanded = true;
            }
            else
            {
                if ((sourceNode.ChildNodes.Count > 0) || !sourceNode.ChildNodesLoaded)
                {
                    newNode.PopulateOnDemand = true;
                }
            }

            // Set expanded status
            string aliasPath = ValidationHelper.GetString(container.GetValue("NodeAliasPath"), string.Empty);
            newNode.Expanded = (aliasPath.ToLowerCSafe() == MapProvider.UsedPath.ToLowerCSafe()) || (expandNodes.Contains(nodeId));
        }
        else
        {
            string parentNodeId = ValidationHelper.GetString(container.GetValue("NodeParentID"), string.Empty);
            newNode.Value = nodeId.ToString();
            newNode.Text = MaxTreeNodeText.Replace("##PARENTNODEID##", parentNodeId);
            newNode.NavigateUrl = "#";
        }

        return newNode;
    }


    private string ResolveNode(string template, string nodeName, string imageTag, string nodeNameJava, int sourceNodeId)
    {
        return template.Replace("##NODEID##", sourceNodeId.ToString()).Replace("##NODENAMEJAVA##", nodeNameJava).Replace("##NODENAME##", nodeName).Replace("##ICON##", imageTag);
    }


    public string GetFallBackToRootScript(bool refreshTree)
    {
        if (RootNode != null)
        {
            string script = null;
            if (refreshTree)
            {
                script += "RefreshTree(0, " + RootNodeID + ");";
            }
            script += "SelectNode(" + RootNodeID + ");";
            return script;
        }
        return null;
    }

    #endregion


    #region "Action handling"

    /// <summary>
    /// Adds the alert error message to the response.
    /// </summary>
    /// <param name="message">Message</param>
    protected override void AddError(string message)
    {
        mCallbackResult += "alert(" + ScriptHelper.GetString(message) + ");";
    }


    /// <summary>
    /// Sets the expanded node ID.
    /// </summary>
    /// <param name="nodeId">Node ID to set</param>
    protected override void SetExpandedNode(int nodeId)
    {
        ExpandNodeID = nodeId;
    }

    #endregion


    #region "Private methods"

    /// <summary>
    /// Indicates if there is needed fallback to root document.
    /// </summary>
    /// <param name="selectedNode">Selected node</param>
    private bool UseFallbackToRoot(TreeNode selectedNode)
    {
        if (selectedNode != null)
        {
            string userStartingPath = CurrentUser.UserStartingAliasPath;

            return !String.IsNullOrEmpty(userStartingPath) && !selectedNode.NodeAliasPath.StartsWithCSafe(userStartingPath, true)
                   && (TreeProvider.SelectSingleNode(selectedNode.NodeSiteName, userStartingPath, TreeProvider.ALL_CULTURES, true) != null);
        }

        return true;
    }

    #endregion


    #region "ICallbackEventHandler Members"

    /// <summary>
    /// Gets the callback result.
    /// </summary>
    string ICallbackEventHandler.GetCallbackResult()
    {
        return mCallbackResult;
    }


    /// <summary>
    /// Processes the callback action.
    /// </summary>
    void ICallbackEventHandler.RaiseCallbackEvent(string eventArgument)
    {
        if (eventArgument == null)
        {
            return;
        }

        eventArgument = eventArgument.ToLowerCSafe();
        string[] parameters = eventArgument.Split(';');
        if (parameters.Length < 3)
        {
            return;
        }

        // Get the arguments
        string action = parameters[0];
        int nodeId = ValidationHelper.GetInteger(parameters[1], 0);
        int targetId = ValidationHelper.GetInteger(parameters[2], 0);

        // Get the target node
        TreeNode targetNode = TreeProvider.SelectSingleNode(targetId, TreeProvider.ALL_CULTURES);
        if (targetNode == null)
        {
            AddError(GetString("ContentRequest.ErrorMissingTarget") + " " + eventArgument);
            mCallbackResult += GetFallBackToRootScript(true);
            return;
        }

        // Get the node
        TreeNode node = TreeProvider.SelectSingleNode(nodeId);
        if (node == null)
        {
            AddError(GetString("ContentRequest.ErrorMissingSource"));
            mCallbackResult += GetFallBackToRootScript(true);
            return;
        }

        // Get new parent ID
        int newParentId = targetNode.NodeID;
        if (action.Contains("position") && !targetNode.NodeClassName.EqualsCSafe("CMS.Root", true))
        {
            newParentId = targetNode.NodeParentID;
        }
        else if (node.NodeParentID == newParentId)
        {
            // Move/Copy/Link as the first node under the same parent
            if (action.EndsWithCSafe("position"))
            {
                action = action.Substring(0, action.Length - 8);
            }

            action += "first";
        }

        bool copy = (action.Contains("copy"));
        bool link = (action.Contains("link"));

        // Do not allow to move or copy under itself
        if ((node.NodeID == newParentId) && !copy && !link)
        {
            AddError(GetString("ContentRequest.CannotMoveToItself"));
            return;
        }

        // Local action - Only position change
        if ((node.NodeParentID == newParentId) && !copy && !link)
        {
            // Local action - Only position change
            int originalPosition = node.NodeOrder;
            TreeNode newNode = ProcessAction(node, targetNode, action, false, false, true);
            if ((newNode != null) && (originalPosition != newNode.NodeOrder))
            {
                // Log the synchronization tasks for the entire tree level
                DocumentSynchronizationHelper.LogDocumentChangeOrder(SiteContext.CurrentSiteName, newNode.NodeAliasPath, TreeProvider);

                mCallbackResult += "CancelDragOperation(); RefreshTree(" + newNode.NodeParentID + ", currentNodeId);";
            }
        }
        else
        {
            // Different parent
            mCallbackResult += "DragOperation(" + nodeId + ", " + targetId + ", '" + action + "');";
        }
    }

    #endregion
}