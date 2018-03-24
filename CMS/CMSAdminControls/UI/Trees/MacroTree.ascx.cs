using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections.Generic;
using System.Linq;

using CMS.EventLog;
using CMS.Helpers;
using CMS.IO;
using CMS.Membership;
using CMS.Base;
using CMS.OnlineForms;
using CMS.UIControls;
using CMS.DocumentEngine;
using CMS.MacroEngine;
using CMS.DataEngine;
using CMS.CustomTables;
using CMS.ExtendedControls;

using TreeNode = System.Web.UI.WebControls.TreeNode;

public partial class CMSAdminControls_UI_Trees_MacroTree : CMSUserControl
{
    #region "Variables"

    private MacroResolver mContextResolver;

    private object mRoot;

    private bool mDisplayValues = true;
    private bool mVirtualMode = true;

    private int mMaxDepth = 10;

    public CMSAdminControls_UI_Trees_MacroTree()
    {
        OnNodeClickHandler = null;
    }

    #endregion


    #region "Properties"

    /// <summary>
    /// If true, object text is displayed in root
    /// </summary>
    public bool DisplayObjectInRoot
    {
        get;
        set;
    }


    /// <summary>
    /// If true, the control is displayed in the virtual mode
    /// </summary>
    public bool VirtualMode
    {
        get
        {
            return mVirtualMode;
        }
        set
        {
            mVirtualMode = value;
        }
    }


    /// <summary>
    /// Tree view control.
    /// </summary>
    public TreeView TreeControl
    {
        get
        {
            return treeElem;
        }
    }


    /// <summary>
    /// Gets or sets the name of the resolver to use. This property is used if ContextResolver property is not explicitly set.
    /// </summary>
    public string ResolverName
    {
        get
        {
            return ValidationHelper.GetString(GetValue("ResolverName"), "");
        }
        set
        {
            SetValue("ResolverName", value);
        }
    }


    /// <summary>
    /// Gets or sets ContextResolver object which is used to evaluate macro expression. If nothing is specified, MacroResolver.Current is used.
    /// </summary>
    public MacroResolver ContextResolver
    {
        get
        {
            if (mContextResolver == null)
            {
                if (!string.IsNullOrEmpty(ResolverName))
                {
                    mContextResolver = MacroResolverStorage.GetRegisteredResolver(ResolverName);
                }
                else
                {
                    // Initialize the default context resolver
                    mContextResolver = MacroContext.CurrentResolver.CreateChild();
                }
                mContextResolver.Settings.VirtualMode = VirtualMode;
            }

            return mContextResolver;
        }
        set
        {
            mContextResolver = value;
        }
    }


    /// <summary>
    /// Gets or sets macro expression to evaluate. The result of the evaluation is used as a root of the tree structure.
    /// </summary>
    public string MacroExpression
    {
        get;
        set;
    }


    /// <summary>
    /// Gets or sets JavaScript function which will be called when a node in the tree is clicked. Macro of the node is passed as a parameter to this function.
    /// </summary>
    public string OnNodeClickHandler
    {
        get;
        set;
    }


    /// <summary>
    /// Gets or sets maximal tree depth.
    /// </summary>
    public int MaxDepth
    {
        get
        {
            return mMaxDepth;
        }
        set
        {
            mMaxDepth = value;
        }
    }


    /// <summary>
    /// Indicates whether to display values of the properties.
    /// </summary>
    public bool DisplayValues
    {
        get
        {
            return mDisplayValues && !VirtualMode;
        }
        set
        {
            mDisplayValues = value;
        }
    }


    /// <summary>
    /// Gets a root of the tree structure (result of the evauation of given macro).
    /// </summary>
    public object Root
    {
        get
        {
            if (mRoot == null)
            {
                EvaluationResult result = ContextResolver.ResolveMacroExpression(MacroExpression, true);
                if (result != null)
                {
                    mRoot = result.Result;
                }
            }
            return mRoot;
        }
    }


    /// <summary>
    /// Gets an macro expression specifying currently selected node.
    /// </summary>
    public string SelectedMacroExpression
    {
        get
        {
            if (treeElem.SelectedNode != null)
            {
                return treeElem.SelectedNode.Value.Replace('/', '.').Replace(".[", "[");
            }
            else
            {
                return MacroExpression;
            }
        }
    }


    /// <summary>
    /// Gets an object specifying currently selected node.
    /// </summary>
    public object SelectedObject
    {
        get
        {
            EvaluationResult result = ContextResolver.ResolveMacroExpression(SelectedMacroExpression, true);
            if (result != null)
            {
                return result.Result;
            }
            return null;
        }
    }

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        // Setup the design
        bool isRTL = CultureHelper.IsUICultureRTL();
        if (IsLiveSite)
        {
            isRTL = CultureHelper.IsPreferredCultureRTL();
        }

        treeElem.LineImagesFolder = GetImageUrl(isRTL ? "RTL/Design/Controls/Tree" : "Design/Controls/Tree", IsLiveSite, true);

        treeElem.ImageSet = TreeViewImageSet.Custom;
        treeElem.ExpandImageToolTip = GetString("ContentTree.Expand");
        treeElem.CollapseImageToolTip = GetString("ContentTree.Collapse");

        treeElemPriority.LineImagesFolder = treeElem.LineImagesFolder;
        treeElemPriority.ImageSet = treeElem.ImageSet;
        treeElemPriority.ExpandImageToolTip = treeElem.ExpandImageToolTip;
        treeElemPriority.CollapseImageToolTip = treeElem.CollapseImageToolTip;

        ContextResolver.Settings.UserName = MembershipContext.AuthenticatedUser.UserName;
        ContextResolver.Settings.CheckIntegrity = false;

        if (!RequestHelper.IsCallback())
        {
            // Register tree progress icon
            ScriptHelper.RegisterTreeProgress(Page);
        }
    }


    protected void Page_PreRender(object sender, EventArgs e)
    {
        if (!StopProcessing)
        {
            ReloadData();

            if (ContextResolver.ShowOnlyPrioritized)
            {
                treeElem.Visible = false;
            }
        }
    }


    /// <summary>
    /// Reloads the tree.
    /// </summary>
    public void ReloadData()
    {
        // Setup the root
        if (!String.IsNullOrEmpty(MacroExpression))
        {
            TreeNode root = new TreeNode();

            root.NavigateUrl = "javascript: " + OnNodeClickHandler + "('');";
            root.Value = HttpUtility.UrlEncode(MacroExpression);
            if (IsSimpleValue(Root))
            {
                // Macro result is only simple value, no children
                root.ToolTip = GetValueString(Root, true);
            }
            else
            {
                root.ToolTip = GetValueString(Root, false);
                root.PopulateOnDemand = true;
            }

            string text = "<span class=\"ContentTreeItem\"><span class=\"Name\"><strong>" + (DisplayObjectInRoot ? root.ToolTip : GetString("objecttasks.root")) + "</strong></span></span>";
            root.Text = text;

            treeElem.Nodes.Add(root);
        }

        // Initialize priority tree
        HashSet<string> prioritiesSet = ContextResolver.GetPrioritizedDataNames();
        if (prioritiesSet.Count > 0)
        {
            List<string> priorities = new List<string>(prioritiesSet);
            priorities.Sort();

            string text = "<span class=\"ContentTreeItem\"><span class=\"Name\"><strong>" + GetString("objecttasks.contextspecific") + "</strong></span></span>";
            TreeNode rootPriority = new TreeNode(text);
            rootPriority.NavigateUrl = "javascript: " + OnNodeClickHandler + "('');";
            rootPriority.Value = "";

            foreach (string item in priorities)
            {
                EvaluationResult evalResult = ContextResolver.ResolveMacroExpression(item, true);
                if (evalResult != null)
                {
                    AppendChild(rootPriority, item, evalResult.Result, false, true);
                }
            }

            treeElemPriority.Nodes.Add(rootPriority);
            rootPriority.Expanded = true;
        }
        else
        {
            pnlPriorityTree.Visible = false;
        }

        // Hide the tree if there are no nodes
        bool empty = (treeElem.Nodes.Count == 0);
        if (!empty)
        {
            treeElem.Nodes[0].Expanded = true;
            if (treeElem.Nodes[0].ChildNodes.Count == 0)
            {
                treeElem.Nodes[0].Text = ValidationHelper.GetString(Root, "null");
                //treeElem.Visible = false;
            }
            else
            {
                // Expand all tree by default only if there are no context values
                if (treeElem.Nodes[0] != null)
                {
                    treeElem.Nodes[0].Expanded = (ContextResolver.GetPrioritizedDataNames().Count == 0);
                }
            }
        }
        else
        {
            treeElem.Visible = false;
        }
    }


    protected void treeElem_TreeNodePopulate(object sender, TreeNodeEventArgs e)
    {
        TreeNodePopulate(e, false);
    }


    protected void treeElemPriority_TreeNodePopulate(object sender, TreeNodeEventArgs e)
    {
        TreeNodePopulate(e, true);
    }


    private void TreeNodePopulate(TreeNodeEventArgs e, bool isPriorityTree)
    {
        // Get the macro expression and evaluate it
        string macro = HttpUtility.UrlDecode(e.Node.ValuePath.Replace('/', '.')).Replace(".[", "[").TrimStart('.');

        EvaluationResult result = ContextResolver.ResolveMacroExpression(macro, true);
        if (result != null)
        {
            if (result.SecurityPassed)
            {
                BindObject(result.Result, e.Node, isPriorityTree);
            }
            else
            {
                TreeNode child = new TreeNode(GetString("macrodesigner.accessdenied"));
                child.Value = "";
                e.Node.ChildNodes.Add(child);
            }
        }
    }


    private void BindObject(object obj, TreeNode root, bool isPriorityTree)
    {
        int index = 0;

        try
        {
            // DataRow source, bind column names
            var dr = obj as DataRow;
            if (dr != null)
            {

                // Create tree structure
                foreach (DataColumn col in dr.Table.Columns)
                {
                    // Stop on max nodes
                    if (index++ >= MacroStaticSettings.MaxMacroNodes)
                    {
                        AppendMore(root);
                        break;
                    }

                    // Add the column
                    object childObj = dr[col.ColumnName];
                    AppendChild(root, col.ColumnName, childObj, false, isPriorityTree);
                }
            }
            else
            {
                // DataRowView source, bind column names
                var drv = obj as DataRowView;
                if (drv != null)
                {
                    // Create tree structure
                    foreach (DataColumn col in drv.DataView.Table.Columns)
                    {
                        // Stop on max nodes
                        if (index++ >= MacroStaticSettings.MaxMacroNodes)
                        {
                            AppendMore(root);
                            break;
                        }

                        // Add the column
                        object childObj = drv[col.ColumnName];
                        AppendChild(root, col.ColumnName, childObj, false, isPriorityTree);
                    }
                }
                else
                {
                    // Hierarchical object source
                    var hc = obj as IHierarchicalObject;
                    if (hc != null)
                    {
                        BaseInfo info = obj as BaseInfo;

                        var props = new List<string>(hc.Properties);

                        var fields = MacroFieldContainer.GetFieldsForObject(hc);
                        if (fields != null)
                        {
                            props.AddRange(fields.Select(x => x.Name));
                        }
                        //props.Sort();

                        if ((info != null) && (info.TypeInfo.SensitiveColumns != null))
                        {
                            props = props.Except(info.TypeInfo.SensitiveColumns).ToList();
                        }

                        // Create tree structure
                        foreach (string col in props)
                        {
                            // Stop on max nodes
                            if (index++ >= MacroStaticSettings.MaxMacroNodes)
                            {
                                AppendMore(root);
                                break;
                            }

                            // Add the property
                            AddProperty(root, isPriorityTree, hc, col);
                        }
                    }
                    else
                    {
                        // Data container source
                        var dc = obj as IDataContainer;
                        if (dc != null)
                        {
                            // Create tree structure
                            foreach (string col in dc.ColumnNames)
                            {
                                // Stop on max nodes
                                if (index++ >= MacroStaticSettings.MaxMacroNodes)
                                {
                                    AppendMore(root);
                                    break;
                                }

                                // Add the column
                                object childObj;
                                dc.TryGetValue(col, out childObj);

                                AppendChild(root, col, childObj, false, isPriorityTree);
                            }
                        }
                    }
                }
            }

            // Enumerable objects
            if ((obj is IEnumerable) && !(obj is string))
            {
                IEnumerable collection = (IEnumerable)obj;
                IEnumerator enumerator = null;

                bool indexByName = false;

                INamedEnumerable namedCol = null;
                if (obj is INamedEnumerable)
                {
                    // Collection with name enumerator
                    namedCol = (INamedEnumerable)collection;
                    if (namedCol.ItemsHaveNames)
                    {
                        enumerator = namedCol.GetNamedEnumerator<object>();
                        indexByName = true;
                    }
                }

                if (!indexByName)
                {
                    // Standard collection
                    enumerator = collection.GetEnumerator();
                }

                int i = 0;

                List<string> addedItems = new List<string>();
                addedItems.Add("trees");
                addedItems.Add("documents");

                while (enumerator.MoveNext())
                {
                    // Stop on max nodes
                    if (index++ >= MacroStaticSettings.MaxMacroNodes)
                    {
                        AppendMore(root);
                        break;
                    }

                    // Add the item
                    object item = MacroResolver.EncapsulateObject(enumerator.Current);
                    if (indexByName)
                    {
                        // Convert the name with dot to indexer
                        string name = namedCol.GetObjectName(item);
                        if (!ValidationHelper.IsIdentifier(name))
                        {
                            name = "[\"" + name + "\"]";
                        }

                        string nameToLower = name.ToLowerCSafe();
                        if (!addedItems.Contains(nameToLower))
                        {
                            addedItems.Add(nameToLower);
                            AppendChild(root, name, item, false, isPriorityTree);
                        }
                    }
                    else
                    {
                        // Indexed item
                        AppendChild(root, i.ToString(), item, true, isPriorityTree);
                    }

                    i++;
                }
            }
        }
        catch (Exception ex)
        {
            EventLogProvider.LogException("MacroTree", "EXPAND", ex);

            throw;
        }
    }


    /// <summary>
    /// Appends the object property
    /// </summary>
    /// <param name="root">Root node</param>
    /// <param name="isPriorityTree">Flag indicating if the tree is a priority tree</param>
    /// <param name="hc">Object</param>
    /// <param name="col">Property name</param>
    private void AddProperty(TreeNode root, bool isPriorityTree, IHierarchicalObject hc, string col)
    {
        // Add the property
        try
        {
            // Disable license error when macro evaluation tries to load objects that are not included in current license
            using (new CMSActionContext { EmptyDataForInvalidLicense = true })
            {
                EvaluationResult result = ContextResolver.GetObjectValue(hc, col, new EvaluationContext(ContextResolver, ""));

                // Append the child value
                AppendChild(root, col, result.Result, false, isPriorityTree);
            }
        }
        catch (Exception ex)
        {
            EventLogProvider.LogException("MacroTree", "EXPAND", ex);

            // Append the exception as child value
            AppendChild(root, col, ex, false, isPriorityTree);
        }
    }

    #endregion


    #region "Helper methods"

    /// <summary>
    /// Appends the "more" node (for case when there is more items than max nodes
    /// </summary>
    /// <param name="root">Root node</param>
    private void AppendMore(TreeNode root)
    {
        string iconUrl = GetImagePath("Design/Controls/MacroEditor/info.png");

        string icon = "<img src=\"" + URLHelper.ResolveUrl(iconUrl) + "\" alt=\"\" class=\"cms-icon-80\" />";

        string text = "<span class=\"ContentTreeItem\" onclick=\"return false;\">" + icon + "<span class=\"Name\">" + GetString("MacroTree.More") + "</span></span>";

        TreeNode child = new TreeNode(text);

        child.ToolTip = GetString("MacroTree.MoreInfo");
        child.Value = "";
        child.PopulateOnDemand = false;

        root.ChildNodes.Add(child);
    }


    /// <summary>
    /// Appends given child object to the root node.
    /// </summary>
    /// <param name="root">Root node</param>
    /// <param name="colName">Name of the column</param>
    /// <param name="childObj">Child object to append</param>
    /// <param name="enumerate">Indicates if it's enumeration item</param>
    /// <param name="isPriorityTree">If true, nodes are appended to a priority tree</param>
    private void AppendChild(TreeNode root, string colName, object childObj, bool enumerate, bool isPriorityTree)
    {
        // Convert object property to value
        var objProp = childObj as ObjectProperty;
        if (objProp != null)
        {
            object value = objProp.Value;

            AppendChild(root, colName, value, enumerate, isPriorityTree);

            return;
        }

        // Skip page context macros
        if (ContextResolver.Settings.DisablePageContextMacros && ((colName.ToLowerCSafe() == "currentdocument") || (colName.ToLowerCSafe() == "currentpageinfo")))
        {
            return;
        }

        // Skip contexts
        if (ContextResolver.Settings.DisableContextObjectMacros && colName.EndsWithCSafe("context", true))
        {
            return;
        }

        bool isSimple = IsSimpleValue(childObj);

        // Append JS onclick action if defined, pass macro expression to the handler
        string jsHandler = "";
        if (!string.IsNullOrEmpty(OnNodeClickHandler))
        {
            string macroArg = root.ValuePath.Replace('/', '.') + (enumerate ? "[" + colName + "]" : "." + colName);
            macroArg = HttpUtility.UrlDecode(macroArg);
            macroArg = macroArg.Replace(".[", "[").TrimStart('.');

            if (isPriorityTree)
            {
                jsHandler = OnNodeClickHandler + "(" + ScriptHelper.GetString(macroArg, true) + ");";
            }
            else
            {
                jsHandler = OnNodeClickHandler + "(" + ScriptHelper.GetString(macroArg.Substring(MacroExpression.Length + 1), true) + ");";
            }
        }

        var name = GetObjectName(colName, childObj, enumerate);
        var icon = GetObjectIcon(childObj, colName);
        var iconHtml = GetIconHtml(name, icon, jsHandler);

        // Create the node
        TreeNode child = new TreeNode(iconHtml);
        
        child.ToolTip = GetValueString(childObj, isSimple);
        child.Value = HttpUtility.UrlEncode(enumerate ? "[" + colName + "]" : colName);
        child.PopulateOnDemand = !isSimple;
        child.Expanded = false;

        root.ChildNodes.Add(child);
    }


    /// <summary>
    /// Get html for icon.
    /// </summary>
    /// <param name="name">Name</param>
    /// <param name="icon">CMSIcon</param>
    /// <param name="jsHandler">Handler for on click event.</param>
    private static string GetIconHtml(string name, CMSIcon icon, string jsHandler)
    {
        var label = new Label();
        label.Text = name;

        var panel = new CMSPanel();
        panel.Controls.Add(icon);
        panel.Controls.Add(label);
        panel.Attributes.Add("onclick", jsHandler + "return false;");
        string iconHtml = panel.GetRenderedHTML();
        return iconHtml;
    }


    /// <summary>
    /// Gets the object name
    /// </summary>
    /// <param name="colName">Column name</param>
    /// <param name="childObj">Processed object</param>
    /// <param name="enumerate">Flag whether enumeration occurs</param>
    private string GetObjectName(string colName, object childObj, bool enumerate)
    {
        string name = colName;

        if (enumerate)
        {
            var col = childObj as InfoObjectCollection;
            if (col != null)
            {
                name = col.Name;
            }
            else if (childObj != null)
            {
                name = "[" + colName + "]" + (DisplayValues ? " " + HTMLHelper.HTMLEncode(childObj.ToString()) : "");
            }
        }

        // Document
        var node = childObj as CMS.DocumentEngine.TreeNode;
        if (node != null)
        {
            string docName = node.GetDocumentName();

            if (enumerate)
            {
                name = "[" + colName + "] " + HTMLHelper.HTMLEncode(docName);
            }
            else if (DisplayValues)
            {
                name += " <span class=\"Info\">(" + HTMLHelper.HTMLEncode(docName) + ")</span>";
            }
        }
        else
        {
            // Info object
            var info = childObj as BaseInfo;
            if (info != null)
            {
                string objName = info.Generalized.ObjectDisplayName;
                if (!String.IsNullOrEmpty(objName))
                {
                    if (enumerate)
                    {
                        name = "[" + colName + "] " + (DisplayValues ? " " + HTMLHelper.HTMLEncode(objName) : "");
                    }
                    else if (DisplayValues)
                    {
                        name += " <span class=\"Info\">(" + HTMLHelper.HTMLEncode(objName) + ")</span>";
                    }
                }
            }
            else if ((childObj != null) && IsSimpleValue(childObj) && (DisplayValues))
            {
                // Simple value
                name += " <span class=\"Info\">(" + HTMLHelper.HTMLEncode(childObj.ToString()) + ")</span>";
            }
        }

        name = TextHelper.LimitLength(ResHelper.LocalizeString(name), 150, null, true);

        return name;
    }


    /// <summary>
    /// Get icon class according to object type.
    /// </summary>
    /// <param name="childObj">Child object</param>
    /// <returns>Icon class</returns>
    private string GetIconClassByObjectType(object childObj)
    {
        if (childObj == null)
        {
            return String.Empty;
        }
       
        if ((childObj is SettingsCategoryContainer) ||
            (childObj is InfoObjectCollection) ||
            (childObj is TreeNode) ||
            (childObj is CustomTableItem) ||
            (childObj is BizFormItem))
        {
            return "icon-me-abstractobjectcollection";
        }
       
        if ((childObj is BaseInfo) ||
            (childObj is PageInfo))
        {
            return "icon-me-info";
        }
        
        if (childObj is IContext)
        {
            return "icon-me-icontext";
        }
        
        if (childObj is CollectionPropertyWrapper)
        {
            return "icon-me-ilist";
        }
        
        if (childObj is Exception)
        {
            return "icon-me-exception";
        }
        return String.Empty;
    }


    /// <summary>
    /// Get icon class according to column.
    /// </summary>
    /// <param name="col">Column</param>
    /// <returns>Icon class</returns>
    private string GetIconClassByColumn(string col)
    {
        switch (col.ToLowerCSafe())
        {
           // Children
           case "children":
                return "icon-me-children";
           
            // Referring objects
           case "referringobjects":
                return "icon-me-referring";
           
            // Parent
           case "parent":
                return "icon-me-parent";
            
           default:
                return String.Empty;
        }
    }


    /// <summary>
    /// Get icon class according to primitive class.
    /// </summary>
    /// <param name="childObj">Child object</param>
    /// <returns>Icon class</returns>
    private string GetIconClassByPrimitiveClass(object childObj)
    {
        // Null
        if (childObj == null)
        {
            return "icon-me-null";
        }

        // Complex object
        if (!IsSimpleValue(childObj))
        {
            return "icon-me-abstractobjectcollection";
        }

        // Simple value
        if (childObj is string)
        {
            return "icon-me-string";
        }

        // Simple value
        if ((childObj is int) || (childObj is double) || (childObj is long))
        {
            return "icon-me-double";
        }

        if (childObj is bool)
        {
            // Bool value
            if ((bool)childObj)
            {
                return "icon-me-true";
            }
            return "icon-me-false";
        }

        // Simple value
        if (childObj is DateTime)
        {
            return "icon-me-datetime";
        }
        // Simple value
        return "icon-me-value";
    }


    /// <summary>
    /// Gets the object icon
    /// </summary>
    /// <param name="childObj">Object to process</param>
    /// <param name="col">Column name</param>
    private string GetIconClass(object childObj, string col)
    {
        // Prepare the icon
        if (childObj is DebugContainer)
        {
            childObj = ((DebugContainer)childObj).Object;
        }
        
        string iconClass = GetIconClassByObjectType(childObj);
        if (iconClass == String.Empty)
        {
            iconClass = GetIconClassByColumn(col);
        }
    
        if (iconClass == String.Empty)
        {
            iconClass = GetIconClassByPrimitiveClass(childObj);
        }
        return iconClass;
    }


    /// <summary>
    /// Get object icon.
    /// </summary>
    /// <param name="childObj">Object to process</param>
    /// <param name="col">Column name</param>
    private CMSIcon GetObjectIcon(object childObj, string col)
    {
        string iconClass = GetIconClass(childObj, col);
        
        var icon = new CMSIcon
        {
            CssClass = iconClass
        };
        return icon;
    }

    
    /// <summary>
    /// Returns formated value of given value.
    /// </summary>
    /// <param name="obj">Value</param>
    /// <param name="isSimple">Indicates if the value is simple object without children</param>
    private string GetValueString(object obj, bool isSimple)
    {
        if (isSimple)
        {
            if (obj != null)
            {
                string stringValue = TextHelper.LimitLength(ValidationHelper.GetString(obj, ""), 200, null, true);

                return "(" + obj.GetType() + "): " + stringValue;
            }
        }
        else if (obj is IMacroObject)
        {
            return ((IMacroObject)obj).ToMacroString();
        }
        else
        {
            return "(" + obj.GetType() + ")" + (obj is IList ? " Count: " + ((IList)obj).Count : "");
        }

        return "null";
    }


    /// <summary>
    /// Returns true if the object is simple value (value which has no children).
    /// </summary>
    /// <param name="obj">Object to check</param>
    private bool IsSimpleValue(object obj)
    {
        if (obj == null)
        {
            return true;
        }

        if ((obj is IDataContainer) || (obj is IHierarchicalObject) || (obj is DataRowView) || (obj is DataRow))
        {
            return false;
        }

        if ((obj is IEnumerable) && !(obj is string) && !(obj is byte[]))
        {
            //// If it's enumeration, return false only if there are some items
            //IEnumerable collection = (IEnumerable)obj;
            //IEnumerator enumerator = collection.GetEnumerator();
            //if (enumerator.MoveNext())
            //{
            //    return false;
            //}
            return false;
        }

        return true;
    }

    #endregion
}