using System;
using System.Linq;
using System.Text;

using CMS.Core;
using CMS.DataEngine;
using CMS.DocumentEngine;
using CMS.Helpers;
using CMS.Membership;
using CMS.PortalEngine;
using CMS.SiteProvider;
using CMS.TranslationServices;
using CMS.UIControls;

public partial class CMSModules_Content_Controls_NewCultureVersion : CMSUserControl
{
    #region "Variables"

    TreeNode node;

    #endregion


    #region "Properties"

    /// <summary>
    /// Gets and sets the target culture code.
    /// </summary>
    public string RequiredCulture
    {
        get;
        set;
    }


    /// <summary>
    /// Gets a value that indicates if the page is loaded inside a split view.
    /// </summary>
    public bool IsInCompare
    {
        get
        {
            return QueryHelper.GetBoolean("compare", false);
        }
    }

    /// <summary>
    /// Indicates whether the page is displayed as dialog.
    /// </summary>
    public bool RequiresDialog
    {
        get;
        set;
    }


    /// <summary>
    /// Tree provider object.
    /// </summary>
    public TreeProvider Tree
    {
        get;
        set;
    }


    /// <summary>
    /// Gets and sets node ID of current document.
    /// </summary>
    public int NodeID
    {
        get;
        set;
    }


    /// <summary>
    /// Gets and sets current node.
    /// </summary>
    public TreeNode Node
    {
        get;
        set;
    }


    /// <summary>
    /// Mode query parameter value.
    /// </summary>
    public string Mode
    {
        get;
        set;
    }

    #endregion


    #region "Life cycle"

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        StringBuilder sb = new StringBuilder();
        sb.Append("var radCopyElem = document.getElementById('", radCopy.ClientID, "');\n",
            "var radTranslations = document.getElementById('", radTranslate.ClientID, "');\n",
            "function FramesRefresh(selectNodeId, mode) { parent.RefreshTree(selectNodeId, selectNodeId); parent.SelectNode(selectNodeId); }");


        ltlScript.Text += ScriptHelper.GetScript(sb.ToString());

        radCopy.Text = GetString("ContentNewCultureVersion.Copy");
        radEmpty.Text = GetString("ContentNewCultureVersion.Empty");
        radTranslate.Text = GetString("ContentNewCultureVersion.Translate");

        radCopy.Attributes.Add("onclick", "ShowSelection();");
        radEmpty.Attributes.Add("onclick", "ShowSelection()");
        radTranslate.Attributes.Add("onclick", "ShowSelection()");

        btnCreateDocument.Text = GetString("ContentNewCultureVersion.Create");
        btnTranslate.Text = GetString("ContentNewCultureVersion.TranslateButton");
        btnTranslate.Click += btnTranslate_Click;
        btnCreateDocument.Click += btnCreateDocument_Click;

        if (NodeID > 0)
        {
            // Fill in the existing culture versions
            node = Tree.SelectSingleNode(NodeID, TreeProvider.ALL_CULTURES);
            if (node != null)
            {
                bool translationAllowed = SettingsKeyInfoProvider.GetBoolValue(node.NodeSiteName + ".CMSEnableTranslations");
                if (translationAllowed)
                {
                    translationElem.TranslationSettings = new TranslationSettings
                    {
                        TargetLanguage = RequiredCulture
                    };
                    translationElem.NodeID = node.NodeID;
                }
                else
                {
                    translationElem.StopProcessing = true;
                    plcTranslationServices.Visible = false;
                }

                if (!MembershipContext.AuthenticatedUser.IsAuthorizedToCreateNewDocument(node.NodeParentID, node.NodeClassName))
                {
                    pnlNewVersion.Visible = false;
                    headNewCultureVersion.Visible = false;
                    ShowError(GetString("accessdenied.notallowedtocreatenewcultureversion"));
                }
                else
                {
                    SiteInfo si = SiteInfoProvider.GetSiteInfo(node.NodeSiteID);
                    if (si != null)
                    {
                        TreeNode originalNode = Tree.GetOriginalNode(node);
                        copyCulturesElem.UniSelector.DisplayNameFormat = "{% CultureName %}{% if (CultureCode == \"" + CultureHelper.GetDefaultCultureCode(si.SiteName) + "\") { \" \" +\"" + GetString("general.defaultchoice") + "\" } %}";
                        copyCulturesElem.AdditionalWhereCondition = "CultureCode IN (SELECT DocumentCulture FROM CMS_Document WHERE DocumentNodeID = " + originalNode.NodeID + ")";

                        if (!MembershipContext.AuthenticatedUser.IsCultureAllowed(RequiredCulture, si.SiteName))
                        {
                            pnlNewVersion.Visible = false;
                            headNewCultureVersion.Visible = false;
                            ShowError(GetString("transman.notallowedcreate"));
                        }
                    }
                }
            }
        }

        ScriptHelper.RegisterStartupScript(Page, typeof(string), "ShowSelection", "ShowSelection();", true);
    }


    /// <summary>
    /// Checks if translation service is available.
    /// </summary>
    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        // Hide translations if no service is available
        if (!translationElem.AnyServiceAvailable)
        {
            plcTranslationServices.Visible = false;
        }
    }

    #endregion


    #region "Control events"

    /// <summary>
    /// Creates new culture version of object. 
    /// </summary>
    protected void btnCreateDocument_Click(object sender, EventArgs e)
    {
        if (radCopy.Checked)
        {
            string sourceCulture = copyCulturesElem.Value.ToString();
            TreeNode actualSourceNode = DocumentHelper.GetDocument(NodeID, sourceCulture, Tree);
            TreeNode sourceNode = actualSourceNode.IsLink ? DocumentHelper.GetDocument(Tree.GetOriginalNode(actualSourceNode), Tree) : actualSourceNode;

            if (sourceNode != null)
            {
                if (chkSaveBeforeEditing.Checked && (node != null))
                {
                    // Create the version first
                    TreeNode newCulture = TreeNode.New(node.ClassName);
                    DocumentHelper.CopyNodeData(sourceNode, newCulture, new CopyNodeDataSettings(true, null));
                    NewCultureDocumentSettings settings = new NewCultureDocumentSettings
                    {
                        Node = newCulture,
                        CultureCode = RequiredCulture,
                        CopyAttachments = true,
                        CopyCategories = true,
                        ClearAttachmentFields = false
                    };

                    try
                    {
                        DocumentHelper.InsertNewCultureVersion(settings);
                    }
                    catch (Exception ex)
                    {
                        // Catch possible exceptions 
                        LogAndShowError("Content", "NEWCULTUREVERSION", ex);
                        return;
                    }

                    // Make sure document is published when versioning without workflow is applied
                    var workflow = newCulture.GetWorkflow();
                    if ((workflow != null) && workflow.WorkflowAutoPublishChanges && !workflow.UseCheckInCheckOut(newCulture.NodeSiteName))
                    {
                        newCulture.MoveToPublishedStep();
                    }

                    // Refresh page
                    if (RequiresDialog)
                    {
                        string url = URLHelper.ResolveUrl(DocumentURLProvider.GetUrl(newCulture.NodeAliasPath) + "?" + URLHelper.LanguageParameterName + "=" + RequiredCulture);
                        ScriptHelper.RegisterStartupScript(this, typeof(string), "NewCultureRefreshAction", ScriptHelper.GetScript(" wopener.location = " + ScriptHelper.GetString(url) + "; CloseDialog();"));
                    }
                    else
                    {
                        ViewModeEnum mode = ViewModeEnum.Edit;
                        if (!TreePathUtils.IsMenuItemType(node.NodeClassName) && (PortalContext.ViewMode != ViewModeEnum.EditLive))
                        {
                            mode = ViewModeEnum.EditForm;
                        }
                        ScriptHelper.RegisterStartupScript(this, typeof(string), "NewCultureRefreshAction", ScriptHelper.GetScript("if (FramesRefresh) { FramesRefresh(" + node.NodeID + ", '" + mode + "'); }"));
                    }
                }
                else
                {
                    var url = GetEditUrl(node);
                    url = URLHelper.AddParameterToUrl(url, "sourcedocumentid", sourceNode.DocumentID.ToString());

                    if (RequiresDialog)
                    {
                        // Reload new page after save
                        url = URLHelper.AddParameterToUrl(url, "reloadnewpage", "true");
                    }

                    // Provide information about actual node
                    if (actualSourceNode.IsLink)
                    {
                        url = URLHelper.AddParameterToUrl(url, "sourcenodeid", actualSourceNode.NodeID.ToString());
                    }
                    Response.Redirect(url);
                }
            }
            else
            {
                ShowError(GetString("transman.notallowedcreate"));
            }
        }
        else
        {
            var url = GetEditUrl(node);

            if (RequiresDialog)
            {
                // Reload new page after save
                url = URLHelper.AddParameterToUrl(url, "reloadnewpage", "true");
            }

            Response.Redirect(url);
        }
    }


    /// <summary>
    /// Translates object to new culture.
    /// </summary>
    protected void btnTranslate_Click(object sender, EventArgs e)
    {
        if (TranslationServiceHelper.IsAuthorizedToTranslateDocument(node, MembershipContext.AuthenticatedUser))
        {
            try
            {
                // Submits the document to translation service
                string err = translationElem.SubmitToTranslation();
                if (string.IsNullOrEmpty(err))
                {
                    // Refresh page
                    if (RequiresDialog)
                    {
                        string url = URLHelper.ResolveUrl(DocumentURLProvider.GetUrl(node.NodeAliasPath) + "?" + URLHelper.LanguageParameterName + "=" + RequiredCulture);
                        ScriptHelper.RegisterStartupScript(this, typeof(string), "NewCultureRefreshAction", ScriptHelper.GetScript(" window.top.location = " + ScriptHelper.GetString(url)));
                    }
                    else
                    {
                        ViewModeEnum mode = ViewModeEnum.Edit;
                        if (!TreePathUtils.IsMenuItemType(node.NodeClassName) && (PortalContext.ViewMode != ViewModeEnum.EditLive))
                        {
                            mode = ViewModeEnum.EditForm;
                        }
                        ScriptHelper.RegisterStartupScript(this, typeof(string), "NewCultureRefreshAction", ScriptHelper.GetScript("if (FramesRefresh) { FramesRefresh(" + node.NodeID + ", '" + mode + "'); }"));
                    }
                }
                else
                {
                    ShowError(err);
                }
            }
            catch (Exception ex)
            {
                ShowError(GetString("ContentRequest.TranslationFailed"), ex.Message);
                TranslationServiceHelper.LogEvent(ex);
            }
        }
        else
        {
            RedirectToAccessDenied("CMS.Content", "SubmitForTranslation");
        }
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Creates editing url based on object type (document, product, product section).
    /// </summary>
    /// <param name="currentNode">Edited node.</param>
    private string GetEditUrl(TreeNode currentNode)
    {
        string url;
        if (currentNode.HasSKU && ModuleEntryManager.IsModuleLoaded(ModuleName.ECOMMERCE))
        {
            url = "~/CMSModules/Ecommerce/Pages/Content/Product/Product_Edit_General.aspx";
        }
        else
        {
            url = "~/CMSModules/Content/CMSDesk/Edit/Edit.aspx";
        }

        url = URLHelper.AddParameterToUrl(url, "nodeid", NodeID.ToString());
        url = URLHelper.AddParameterToUrl(url, "action", "newculture");
        url = URLHelper.AddParameterToUrl(url, "mode", Mode);
        url = URLHelper.AddParameterToUrl(url, "parentculture", RequiredCulture);
        url = URLHelper.AddParameterToUrl(url, "culture", RequiredCulture);
        url = URLHelper.AddParameterToUrl(url, "parentnodeid", currentNode.NodeParentID.ToString());

        if (IsInCompare)
        {
            url = URLHelper.AddParameterToUrl(url, "compare", "1");
        }

        if (RequiresDialog)
        {
            url = URLHelper.AddParameterToUrl(url, "dialog", "1");
        }

        return url;
    }

    #endregion
}
