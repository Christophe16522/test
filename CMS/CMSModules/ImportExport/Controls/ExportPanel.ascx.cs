using System;

using CMS.CMSImportExport;
using CMS.EventLog;
using CMS.Helpers;
using CMS.IO;
using CMS.UIControls;
using CMS.ExtendedControls;
using CMS.DataEngine;

using TreeNode = System.Web.UI.WebControls.TreeNode;

public partial class CMSModules_ImportExport_Controls_ExportPanel : CMSUserControl
{
    #region "Variables"

    // Position of the tree scrollbar
    protected string mScrollPosition = "0";

    // Additional settings control
    private ImportExportControl settingsControl;

    private SiteExportSettings mSettings;

    #endregion


    #region "Public properties"

    /// <summary>
    /// Export settings.
    /// </summary>
    public SiteExportSettings Settings
    {
        get
        {
            return mSettings;
        }
        set
        {
            mSettings = value;
            gvObjects.Settings = value;
            gvTasks.Settings = value;
        }
    }


    /// <summary>
    /// Selected  node value.
    /// </summary>
    public string SelectedNodeValue
    {
        get
        {
            return ValidationHelper.GetString(ViewState["SelectedNodeValue"], ObjectHelper.GROUP_OBJECTS);
        }
        set
        {
            ViewState["SelectedNodeValue"] = value;
        }
    }


    /// <summary>
    /// If true, node is site node.
    /// </summary>
    public bool SiteNode
    {
        get
        {
            return ValidationHelper.GetBoolean(ViewState["SiteNode"], false);
        }
        set
        {
            ViewState["SiteNode"] = value;
        }
    }


    /// <summary>
    /// If true, site node is generated.
    /// </summary>
    public bool SiteGenerated
    {
        get
        {
            return ValidationHelper.GetBoolean(ViewState["SiteGenerated"], false);
        }
        set
        {
            ViewState["SiteGenerated"] = value;
        }
    }

    #endregion


    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!RequestHelper.IsCallback())
        {
            objectTree.TreeView.SelectedNodeChanged += treeElem_SelectedNodeChanged;
            objectTree.TreeView.ExpandImageToolTip = GetString("contenttree.expand");
            objectTree.TreeView.CollapseImageToolTip = GetString("contenttree.collapse");
            objectTree.TreeView.NodeStyle.CssClass = "ContentTreeItem";
            objectTree.TreeView.SelectedNodeStyle.CssClass = "ContentTreeSelectedItem";

            objectTree.NodeTextTemplate = "<span class=\"Name\">##NODENAME##</span>";
            objectTree.SelectedNodeTextTemplate = "<span class=\"Name\">##NODENAME##</span>";
            objectTree.ValueTextTemplate = "##OBJECTTYPE##";

            objectTree.PreselectObjectType = SelectedNodeValue;
            objectTree.IsPreselectedObjectTypeSiteObject = SiteNode;

            ScriptHelper.RegisterStartupScript(this, typeof(string), "borderScript", ScriptHelper.GetScript("var rtl='" + CultureHelper.IsUICultureRTL() + "';"));

            if (Settings != null)
            {
                if (SelectedNodeValue != CMS.DocumentEngine.TreeNode.OBJECT_TYPE)
                {
                    // Set grid view properties
                    gvObjects.Visible = true;
                    gvObjects.ObjectType = SelectedNodeValue;
                    gvObjects.Settings = Settings;
                    gvObjects.SiteObject = SiteNode;

                    gvTasks.Visible = true;
                    gvTasks.ObjectType = SelectedNodeValue;
                    gvTasks.Settings = Settings;
                    gvTasks.SiteObject = SiteNode;
                }
                else
                {
                    gvObjects.Visible = false;
                    gvTasks.Visible = false;
                }
            }

            // Reload data
            ReloadData();

            // Load settings control
            LoadSettingsControl();
        }
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        if (!RequestHelper.IsCallback())
        {
            if (Settings != null)
            {
                if (SelectedNodeValue != CMS.DocumentEngine.TreeNode.OBJECT_TYPE)
                {
                    // Bind grid view
                    gvObjects.Visible = true;
                    gvObjects.ObjectType = SelectedNodeValue;
                    gvObjects.Settings = Settings;
                    gvObjects.SiteObject = SiteNode;
                    gvObjects.Bind();

                    gvTasks.Visible = true;
                    gvTasks.ObjectType = SelectedNodeValue;
                    gvTasks.Settings = Settings;
                    gvTasks.SiteObject = SiteNode;
                    gvTasks.Bind();
                }
                else
                {
                    gvObjects.Visible = false;
                    gvTasks.Visible = false;
                }

                // Reload settings control
                if (settingsControl != null)
                {
                    settingsControl.Settings = Settings;
                    settingsControl.ReloadData();
                }
            }

            pnlError.Visible = (lblError.Text != "");

            // Save scrollbar position
            mScrollPosition = ValidationHelper.GetString(Page.Request.Params["hdnexDivScrollBar"], "0");
        }
    }

    #endregion


    #region "Methods"

    private void LoadSettingsControl()
    {
        try
        {
            if (Settings != null)
            {
                plcControl.Controls.Clear();
                plcControl.Visible = false;
                settingsControl = null;

                if (!string.IsNullOrEmpty(SelectedNodeValue))
                {
                    lblError.Text = "";
                    string virtualPath = "~/CMSModules/ImportExport/Controls/Export/" + (SiteNode ? "Site/" : "") + ImportExportHelper.GetSafeObjectTypeName(SelectedNodeValue) + ".ascx";
                    string filePath = Server.MapPath(virtualPath);

                    if (File.Exists(filePath))
                    {
                        // Load control
                        settingsControl = (ImportExportControl)Page.LoadUserControl(virtualPath);
                        settingsControl.EnableViewState = true;
                        settingsControl.ID = "settingControl";
                        settingsControl.ShortID = "s";
                        settingsControl.Settings = Settings;

                        plcControl.Controls.Add(settingsControl);
                        plcControl.Visible = true;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            lblError.Text = "[ExportPanel.LoadSettingsControl]: Error loading settings control for object type '" + SelectedNodeValue + "'. " + EventLogProvider.GetExceptionLogMessage(ex);
        }
    }


    public void ReloadData()
    {
        if (Settings != null)
        {
            // Genearate items of the tree
            GenerateTreeItems();

            if (!objectTree.ContainsObjectType(SelectedNodeValue))
            {
                SelectedNodeValue = "##OBJECTS##";
            }
        }
    }


    public bool ApplySettings()
    {
        // Save last selection
        SaveSelection();

        // Check if any object is selected
        if (Settings.IsEmptySelection())
        {
            lblError.Text = GetString("ExportPanel.NoObjectSelected");
            return false;
        }

        return true;
    }


    protected void SaveSelection()
    {
        // Save additional settings
        if (settingsControl != null)
        {
            settingsControl.SaveSettings();
        }
    }


    // Handle node selection
    protected void treeElem_SelectedNodeChanged(object sender, EventArgs e)
    {
        // Save selected objects
        SaveSelection();

        SelectedNodeValue = objectTree.TreeView.SelectedValue;
        SiteNode = IsSiteNode(objectTree.TreeView.SelectedNode);

        // Load settings control
        LoadSettingsControl();

        // Reset the pagers
        gvObjects.PagerControl.Reset();
        gvTasks.PagerControl.Reset();
    }


    public bool IsSiteNode(TreeNode node)
    {
        if ((node == null) || (node.Parent == null))
        {
            return false;
        }
        else if (node.Value == ObjectHelper.GROUP_SITE)
        {
            return true;
        }
        else
        {
            return IsSiteNode(node.Parent);
        }
    }


    /// <summary>
    /// Genearate items of the tree.
    /// </summary>
    private void GenerateTreeItems()
    {
        if ((Settings == null) || ((objectTree.TreeView.Nodes.Count > 0) && (SiteGenerated == (Settings.SiteId > 0))))
        {
            return;
        }

        objectTree.RootNode = ImportExportHelper.ObjectTree;
        objectTree.SiteID = Settings.SiteId;
        objectTree.ReloadData();

        SiteGenerated = (Settings.SiteId > 0);
    }

    #endregion
}