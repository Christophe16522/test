﻿using System;
using System.Linq;
using System.Web.UI;
using System.Data;
using System.Web.UI.WebControls;

using CMS.ExtendedControls;
using CMS.Helpers;
using CMS.DataEngine;
using CMS.Membership;
using CMS.PortalEngine;
using CMS.SiteProvider;
using CMS.UIControls;
using CMS.Base;
using CMS.Modules;

public partial class CMSModules_AdminControls_Controls_UIControls_GeneralTree : InlineUserControl, IPostBackEventHandler
{
    #region "Variables"

    protected String itemLocation = String.Empty;
    protected String categoryLocation = String.Empty;
    protected String objectType = String.Empty;
    protected String tabIndexStr = String.Empty;

    private String categoryPathColumn = String.Empty;
    private String categoryParentColumn = String.Empty;
    private String categoryObjectType = String.Empty;
    private String itemParentColumn = String.Empty;
    private String newItem = String.Empty;

    /// <summary>
    /// Index used for item count under one node.
    /// </summary>
    private int mIndex = -1;

    #endregion


    #region "Custom events"

    /// <summary>
    /// On selected item event handler.
    /// </summary>    
    public delegate void ItemSelectedEventHandler(string selectedValue);

    /// <summary>
    /// On selected item event handler.
    /// </summary>
    public event ItemSelectedEventHandler OnItemSelected;

    #endregion


    #region "Properties"

    /// <summary>
    /// Indicates whether use max node limit stored in settings.
    /// </summary>
    public bool UseMaxNodeLimit
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("UseMaxNodeLimit"), true);
        }
        set
        {
            SetValue("UseMaxNodeLimit", value);
        }
    }


    /// <summary>
    /// If false tree wont show items but just categories.
    /// </summary>
    public bool ShowItems
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("ShowItems"), true);
        }
        set
        {
            SetValue("ShowItems", value);
        }
    }


    /// <summary>
    /// Maximum tree nodes shown under parent node - this value can be ignored if UseMaxNodeLimit set to false.
    /// </summary>
    public int MaxTreeNodes
    {
        get
        {
            int max = ValidationHelper.GetInteger(GetValue("MaxTreeNodes"), -1);
            if (max < 0)
            {
                max = SettingsKeyInfoProvider.GetIntValue(SiteContext.CurrentSiteName + ".CMSMaxUITreeNodes");
            }
            return max;
        }
        set
        {
            SetValue("MaxTreeNodes", value);
        }
    }


    /// <summary>
    /// Indicates whether tree displays clone button
    /// </summary>
    public bool DisplayCloneButton
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("DisplayCloneButton"), false);
        }
        set
        {
            SetValue("DisplayCloneButton", value);
        }
    }


    /// <summary>
    /// Element's resource name
    /// </summary>
    private String ResourceName
    {
        get
        {
            return ValidationHelper.GetString(UIContext["ResourceName"], String.Empty);
        }
    }

    #endregion


    #region "Dynamic controls"

    private UniTree mTree;
    private MessagesPlaceHolder mMessagesPlaceHolder;
    private CMSMoreOptionsButton mAddButton;
    private CMSAccessibleButton mDeleteItemButton;
    private CMSAccessibleButton mExportItemButton;
    private CMSAccessibleButton mCloneItemButton;


    /// <summary>
    /// Messages placeholder
    /// </summary>
    public override MessagesPlaceHolder MessagesPlaceHolder
    {
        get
        {
            return mMessagesPlaceHolder ?? (mMessagesPlaceHolder = (MessagesPlaceHolder)paneTree.FindControl("pnlMessages"));
        }
    }


    /// <summary>
    /// Messages placeholder
    /// </summary>
    private UniTree Tree
    {
        get
        {
            return mTree ?? (mTree = (UniTree)paneTree.FindControl("t"));
        }
    }


    /// <summary>
    /// Add item button
    /// </summary>
    protected CMSMoreOptionsButton AddButon
    {
        get
        {
            return mAddButton ?? (mAddButton = (CMSMoreOptionsButton)paneMenu.FindControl("btnAdd"));
        }
    }


    /// <summary>
    /// Delete item button
    /// </summary>
    protected CMSAccessibleButton DeleteItemButton
    {
        get
        {
            return mDeleteItemButton ?? (mDeleteItemButton = (CMSAccessibleButton)paneMenu.FindControl("btnDelete"));
        }
    }


    /// <summary>
    /// Export item button
    /// </summary>
    protected CMSAccessibleButton ExportItemButton
    {
        get
        {
            return mExportItemButton ?? (mExportItemButton = (CMSAccessibleButton)paneMenu.FindControl("btnExport"));
        }
    }


    /// <summary>
    /// Clone item button
    /// </summary>
    protected CMSAccessibleButton CloneItemButton
    {
        get
        {
            return mCloneItemButton ?? (mCloneItemButton = (CMSAccessibleButton)paneMenu.FindControl("btnClone"));
        }
    }

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        objectType = UIContextHelper.GetObjectType(UIContext);
        CloneItemButton.Visible = DisplayCloneButton;

        // Pass tabindex
        int tabIndex = ValidationHelper.GetInteger(UIContext["TabIndex"], 0);
        if (tabIndex != 0)
        {
            tabIndexStr = "&tabindex=" + tabIndex;
        }

        String editElem = ValidationHelper.GetString(UIContext["itemEdit"], String.Empty);
        String categoryElem = ValidationHelper.GetString(UIContext["categoryedit"], String.Empty);
        categoryPathColumn = ValidationHelper.GetString(UIContext["pathColumn"], String.Empty);
        categoryParentColumn = ValidationHelper.GetString(UIContext["categoryparentcolumn"], String.Empty);
        categoryObjectType = ValidationHelper.GetString(UIContext["parentobjecttype"], String.Empty);
        itemParentColumn = ValidationHelper.GetString(UIContext["parentColumn"], String.Empty);
        newItem = ValidationHelper.GetString(UIContext["newitem"], String.Empty);

        if (!ValidateInput())
        {
            return;
        }

        itemLocation = URLHelper.AppendQuery(UIContextHelper.GetElementUrl(ResourceName, editElem), "?tabslayout=horizontal&displaytitle=false");
        categoryLocation = URLHelper.AppendQuery(UIContextHelper.GetElementUrl(ResourceName, categoryElem), "?tabslayout=horizontal&displaytitle=false");

        RegisterExportScript();
        RegisterTreeScript();
        InitTree();

        // Setup menu action scripts
        String newItemText = newItem.StartsWithCSafe("javascript", true) ? newItem : "NewItem('item'); return false;";

        AddButon.Actions.Add(new CMSButtonAction
        {
            Text = GetString(objectType + ".newitem"),
            OnClientClick = newItemText
        });
        AddButon.Actions.Add(new CMSButtonAction
        {
            Text = GetString("development.tree.newcategory"),
            OnClientClick = "NewItem('category'); return false;"
        });

        DeleteItemButton.OnClientClick = "DeleteItem(); return false;";
        ExportItemButton.OnClientClick = "ExportObject(); return false;";
        CloneItemButton.OnClientClick = "if ((selectedItemId > 0) && (selectedItemType == 'item')) { modalDialog('" + URLHelper.ResolveUrl("~/CMSModules/Objects/Dialogs/CloneObjectDialog.aspx?reloadall=1&displaytitle=" + UIContext["displaytitle"] + "&objecttype=" + objectType + "&objectid=") + "' + selectedItemId, 'Clone item', 750, 470); } return false;";

        // Tooltips
        DeleteItemButton.ToolTip = GetString("development.tree.deleteselected");
        ExportItemButton.ToolTip = GetString("exportobject.title");
        CloneItemButton.ToolTip = GetString(objectType + ".clone");

        // URLs for menu actions
        String script = "var doNotReloadContent = false;\n";

        // Script for deleting widget or category
        string delPostback = ControlsHelper.GetPostBackEventReference(this, "##");
        string deleteScript = "function DeleteItem() { \n" +
                              " if ((selectedItemId > 0) && (selectedItemParent > 0) && " +
                              " confirm(" + ScriptHelper.GetLocalizedString("general.deleteconfirmation") + ")) {\n " +
                              delPostback.Replace("'##'", "selectedItemType+';'+selectedItemId+';'+selectedItemParent") + ";\n" +
                              "}\n" +
                              "}\n";
        script += deleteScript;

        // Preselect tree item
        if (!RequestHelper.IsPostBack())
        {
            int parentobjectid = QueryHelper.GetInteger("parentobjectid", 0);
            int objectID = QueryHelper.GetInteger("objectID", 0);

            // Select category
            if (parentobjectid > 0)
            {
                BaseInfo biParent = BaseAbstractInfoProvider.GetInfoById(categoryObjectType, parentobjectid);
                if (biParent != null)
                {
                    String path = ValidationHelper.GetString(biParent.GetValue(categoryPathColumn), String.Empty);
                    int parentID = ValidationHelper.GetInteger(biParent.GetValue(categoryParentColumn), 0);
                    script += SelectAfterLoad(path, parentobjectid, "category", parentID, true, true);
                }
            }
            // Select item
            else if (objectID > 0)
            {
                BaseInfo bi = BaseAbstractInfoProvider.GetInfoById(objectType, objectID);
                if (bi != null)
                {
                    script += SelectItem(bi);
                }
            }
            else
            {
                // Selection by hierarchy URL
                BaseInfo biSel = UIContext.EditedObject as BaseInfo;
                BaseInfo biParent = UIContext.EditedObjectParent as BaseInfo;

                // Check for category selection
                if ((biParent != null) && (biParent.ObjectType == categoryObjectType))
                {
                    String path = ValidationHelper.GetString(biParent.GetValue(categoryPathColumn), String.Empty);
                    int parentID = ValidationHelper.GetInteger(biParent.GetValue(categoryParentColumn), 0);
                    script += SelectAfterLoad(path, biParent.Generalized.ObjectID, "category", parentID, false, true);
                }
                // Check for item selection
                else if ((biSel != null) && (biSel.ObjectType == objectType))
                {
                    script += SelectItem(biSel);
                }
                else
                {
                    // Select root by default
                    BaseInfo bi = BaseAbstractInfoProvider.GetInfoByName(categoryObjectType, "/");

                    if (bi != null)
                    {
                        script += SelectAfterLoad("/", bi.Generalized.ObjectID, "category", 0, true, true);
                    }
                }
            }
        }


        ltlScript.Text += ScriptHelper.GetScript(script);


    }


    /// <summary>
    /// Validates input from UI Context
    /// </summary>
    private bool ValidateInput()
    {
        // Validate object type and parent object type
        if (String.IsNullOrEmpty(objectType))
        {
            ShowError(GetString("generaltree.noobjecttype"));
            return false;
        }

        if (String.IsNullOrEmpty(categoryObjectType))
        {
            ShowError(GetString("generaltree.noparentobjecttype"));
            return false;
        }

        return true;
    }


    /// <summary>
    /// Selects base info item
    /// </summary>
    /// <param name="bi">Base info item definition</param>
    private String SelectItem(BaseInfo bi)
    {
        String script = String.Empty;
        int parentID = ValidationHelper.GetInteger(bi.GetValue(itemParentColumn), 0);
        BaseInfo biParent = BaseAbstractInfoProvider.GetInfoById(categoryObjectType, parentID);
        if (biParent != null)
        {
            String categoryPath = ValidationHelper.GetString(biParent.GetValue(categoryPathColumn), String.Empty);
            string path = categoryPath + "/" + bi.Generalized.ObjectCodeName;
            script += SelectAfterLoad(path, bi.Generalized.ObjectID, "item", parentID, true, true);
        }

        return script;
    }



    protected override void OnPreRender(EventArgs e)
    {
        if (StopProcessing)
        {
            return;
        }

        Tree.ReloadData();

        base.OnPreRender(e);
    }


    private void RegisterExportScript()
    {
        ScriptHelper.RegisterDialogScript(Page);
        string script = "function OpenExportObject(type, id, siteid){ var siteQuery = ''; if(siteid){siteQuery = '&siteId=' + siteid;} modalDialog('" + ResolveUrl("~/CMSModules/ImportExport/Pages/ExportObject.aspx") + "?objectType=' + type + '&objectId=' + id + siteQuery, 'ExportObject', " + CMSPage.EXPORT_OBJECT_WIDTH + ", " + CMSPage.EXPORT_OBJECT_HEIGHT + "); } \n";

        ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "ExportScript", ScriptHelper.GetScript(script));
    }


    /// <summary>
    /// Handles delete action.
    /// </summary>
    /// <param name="eventArgument">Objecttype(item or itemcategory);objectid</param>
    public void RaisePostBackEvent(string eventArgument)
    {
        string[] values = eventArgument.Split(';');
        if (values.Length == 3)
        {
            int id = ValidationHelper.GetInteger(values[1], 0);
            int parentId = ValidationHelper.GetInteger(values[2], 0);
            BaseInfo biParent = BaseAbstractInfoProvider.GetInfoById(categoryObjectType, parentId);

            if (biParent == null)
            {
                return;
            }

            String categoryPath = ValidationHelper.GetString(biParent.GetValue(categoryPathColumn), String.Empty);
            int parentID = ValidationHelper.GetInteger(biParent.GetValue(categoryParentColumn), 0);

            //Test permission
            if (!MembershipContext.AuthenticatedUser.IsAuthorizedPerResource(ResourceName, "Modify"))
            {
                String url = CMSPage.GetAccessDeniedUrl(UIHelper.ACCESSDENIED_PAGE, ResourceName, "modify", String.Empty, String.Empty);

                // Display access denied page in conent frame and select deleted item again
                String denyScript = @"$j(document).ready(function(){ frames['paneContentTMain'].location = '" + ResolveUrl(url) + "';" + SelectAfterLoad(categoryPath + "/", id, values[0], parentId, false, false) + "});";

                ltlScript.Text += ScriptHelper.GetScript(denyScript);
                return;
            }

            string script = String.Empty;

            switch (values[0])
            {
                case "item":
                    BaseInfo bi = BaseAbstractInfoProvider.GetInfoById(objectType, id);
                    if (bi != null)
                    {
                        bi.Delete();
                    }

                    break;
                case "category":
                    try
                    {
                        BaseInfo biCate = BaseAbstractInfoProvider.GetInfoById(categoryObjectType, id);
                        if (biCate != null)
                        {
                            biCate.Delete();
                        }
                    }
                    catch (Exception ex)
                    {
                        // Make alert with exception message, most probable cause is deleting category with subcategories
                        script = String.Format("alert('{0}');\n", ex.Message);

                        // Current node stays selected
                        parentId = id;
                    }
                    break;
            }

            // After delete select first tab always
            tabIndexStr = String.Empty;
            script = SelectAfterLoad(categoryPath + "/", parentId, "category", parentID, true, true) + script;
            ltlScript.Text += ScriptHelper.GetScript(script);
        }
    }


    /// <summary>
    /// Expands tree at specified path and selects tree item by javascript.
    /// </summary>
    /// <param name="path">Selected path</param>
    /// <param name="itemId">ID of selected tree item</param>
    /// <param name="type">Type of tree item</param>
    /// <param name="parentId">ID of parent</param>
    /// <param name="updateRightPanel">Indicates whether update right panel</param>    
    /// <param name="encapsulate">Indicates whether script in encapsulated in jquery rdy event</param>    
    private string SelectAfterLoad(string path, int itemId, string type, int parentId, bool updateRightPanel, bool encapsulate)
    {
        Tree.SelectPath = path;
        Tree.ExpandPath = path;

        String script = String.Format("SelectNode({0},'{1}',{2},{3},'{4}');", itemId, type, parentId, updateRightPanel.ToString().ToLowerCSafe(), tabIndexStr);


        String suffix = ValidationHelper.GetString(UIContext["selectionSuffix"], String.Empty);

        // Add suffix to first page URL
        if (suffix != String.Empty)
        {
            script += String.Format("selectStartPage({0},'{1}',{2},'{3}');", itemId, type, parentId, suffix);
        }
        else
        {
            // New item no ID category selection (under root)
            int selectedItemID = ValidationHelper.GetInteger(UIContext["SelectedItemID"], 0);
            if (selectedItemID != 0)
            {
                UIElementInfo elem = UIElementInfoProvider.GetUIElementInfo(selectedItemID);
                String newCategory = ValidationHelper.GetString(UIContext["newCategory"], String.Empty);
                if ((elem != null) && (newCategory.ToLowerCSafe() == elem.ElementName.ToLowerCSafe()))
                {
                    script += String.Format("NewItem('category')");
                }
            }
        }

        if (encapsulate)
        {
            script = "$j(document).ready(function() {" + script + "});";
        }

        return script;
    }


    private void RegisterTreeScript()
    {
        String newItemLocation = newItem.StartsWithCSafe("javascript", true) ? "" : UIContextHelper.GetElementUrl(ResourceName, newItem);

        String newCategory = ValidationHelper.GetString(UIContext["newCategory"], String.Empty);
        String newCategoryLocation = UIContextHelper.GetElementUrl(ResourceName, newCategory);

        String currentLocation = UIContextHelper.GetElementUrl(UIContext.UIElement, UIContext);
        currentLocation = URLHelper.RemoveParameterFromUrl(currentLocation, "parentobjectid");

        newItemLocation = URLHelper.UpdateParameterInUrl(newItemLocation, "displaytitle", "false");
        newCategoryLocation = URLHelper.UpdateParameterInUrl(newCategoryLocation, "displaytitle", "false");

        String script =
            @"function NewItem(type) {
                if ((selectedItemId <= 0) || (selectedItemType == 'item')) return;
                // Under category (not root)
                if ((type == 'item') && (selectedItemParent > 0)) {
                        frames['paneContentTMain'].location = '" + newItemLocation + @"&parentobjectid=' + selectedItemId;
                }
                else if (type == 'category') {
                        frames['paneContentTMain'].location = '" + newCategoryLocation + @"&parentobjectid=' + selectedItemId;
                }
             }
             function selectTreeItem(id) {
                    window.location = '" + currentLocation + @"&objectid='+id;
             }
             function selectTreeCategory(id) {
                    window.location = '" + currentLocation + @"&parentobjectid='+id;
             }
            ";

        ScriptHelper.RegisterClientScriptBlock(Page, typeof(String), "TreeScript", ScriptHelper.GetScript(script));
    }


    private void InitTree()
    {
        ScriptHelper.RegisterJQuery(Page);

        String prefix = String.Empty;
        if (objectType.ToLowerCSafe() == "cms.widget")
        {
            prefix = "Widget";
        }

        // Create and set category provider
        UniTreeProvider categoryProvider = new UniTreeProvider();
        categoryProvider.DisplayNameColumn = "DisplayName";
        categoryProvider.IDColumn = "ObjectID";
        categoryProvider.LevelColumn = "ObjectLevel";
        categoryProvider.OrderColumn = prefix + "CategoryOrder";
        categoryProvider.ParentIDColumn = "ParentID";
        categoryProvider.PathColumn = "ObjectPath";
        categoryProvider.ValueColumn = "ObjectID";
        categoryProvider.ChildCountColumn = "CompleteChildCount";
        categoryProvider.QueryName = categoryObjectType + ".selectallview";
        categoryProvider.ObjectTypeColumn = "ObjectType";
        categoryProvider.Columns = "DisplayName, ObjectID, ObjectLevel," + prefix + "CategoryOrder,ParentID, ObjectPath, CompleteChildCount,ObjectType," + prefix + "CategoryChildCount, " + prefix + "CategoryImagePath";
        categoryProvider.ImageColumn = prefix + "CategoryImagePath";

        if (!ShowItems)
        {
            categoryProvider.WhereCondition = "ObjectType LIKE '%category%'";
            categoryProvider.ChildCountColumn = "CategoryChildCount";
            categoryProvider.ObjectTypeColumn = "";
            Tree.NodeTemplate = "<span id=\"##OBJECTTYPE##_##NODEID##\" onclick=\"SelectNode(##NODEID##,'##OBJECTTYPE##', ##PARENTNODEID##,true);\" name=\"treeNode\" class=\"ContentTreeItem\">##ICON##<span class=\"Name\">##NODENAME##</span></span>";
            Tree.SelectedNodeTemplate = "<span id=\"##OBJECTTYPE##_##NODEID##\" onclick=\"SelectNode(##NODEID##,'##OBJECTTYPE##', ##PARENTNODEID##,true);\" name=\"treeNode\" class=\"ContentTreeItem ContentTreeSelectedItem\">##ICON##<span class=\"Name\">##NODENAME##</span></span>";
        }
        else
        {
            categoryProvider.OrderBy = "ObjectType DESC, DisplayName ASC";
            Tree.NodeTemplate = "<span id=\"##OBJECTTYPE##_##NODEID##\" onclick=\"SelectNode(##NODEID##,'##OBJECTTYPE##', ##PARENTNODEID##,true);\" name=\"treeNode\" class=\"ContentTreeItem\">##ICON##<span class=\"Name\">##NODENAME##</span></span>";
            Tree.SelectedNodeTemplate = "<span id=\"##OBJECTTYPE##_##NODEID##\" onclick=\"SelectNode(##NODEID##,'##OBJECTTYPE##', ##PARENTNODEID##,true);\" name=\"treeNode\" class=\"ContentTreeItem ContentTreeSelectedItem\">##ICON##<span class=\"Name\">##NODENAME##</span></span>";
        }

        // Set up tree 
        Tree.ProviderObject = categoryProvider;
        Tree.IsLiveSite = false;

        // Setup event handler
        Tree.OnItemSelected += treeElem_OnItemSelected;

        Tree.UsePostBack = false;
        Tree.OnNodeCreated += treeElem_OnNodeCreated;
    }

    #endregion


    #region "Tree events"

    /// <summary>
    ///  On selected item event.
    /// </summary>
    /// <param name="selectedValue">Selected value</param>
    protected void treeElem_OnItemSelected(string selectedValue)
    {
        if (OnItemSelected != null)
        {
            OnItemSelected(selectedValue);
        }
    }


    /// <summary>
    /// Used for maxnodes in collapsed node.
    /// </summary>
    private TreeNode treeElem_OnNodeCreated(DataRow itemData, TreeNode defaultNode)
    {
        if (UseMaxNodeLimit && (MaxTreeNodes > 0))
        {
            //Get parentID from data row
            int parentID = ValidationHelper.GetInteger(itemData.ItemArray[4], 0);
            string dataObjectType = ValidationHelper.GetString(itemData.ItemArray[7], String.Empty);

            //Dont use maxnodes limitation for categories
            if (dataObjectType.ToLowerCSafe().Contains("category"))
            {
                return defaultNode;
            }

            //Increment index count in collapsing
            mIndex++;
            if (mIndex == MaxTreeNodes)
            {
                //Load parentid
                int parentParentID = 0;
                BaseInfo biParent = BaseAbstractInfoProvider.GetInfoById(categoryObjectType, parentID);

                if (biParent != null)
                {
                    parentParentID = ValidationHelper.GetInteger(biParent.GetValue(categoryParentColumn), 0);
                }

                TreeNode node = new TreeNode();
                node.Text = "<span class=\"ContentTreeItem\" onclick=\"SelectNode(" + parentID + " ,'category'," + parentParentID + ",true ); return false;\"><span class=\"Name\">" + GetString("general.seelisting") + "</span></span>";
                return node;
            }
            if (mIndex > MaxTreeNodes)
            {
                return null;
            }
        }
        return defaultNode;
    }

    #endregion
}
