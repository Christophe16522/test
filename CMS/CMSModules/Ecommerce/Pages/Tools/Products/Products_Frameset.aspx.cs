using System;

using CMS.Core;
using CMS.DocumentEngine;
using CMS.Ecommerce;
using CMS.Helpers;
using CMS.Membership;
using CMS.PortalEngine;
using CMS.Base;
using CMS.SiteProvider;
using CMS.UIControls;

[UIElement(ModuleName.ECOMMERCE, "Products")]
public partial class CMSModules_Ecommerce_Pages_Tools_Products_Products_Frameset : CMSProductsPage
{
    #region "Variables"

    private int? mResultNodeID;
    private int? mResultDocumentID;
    private string mResultDevice;

    #endregion


    #region "Properties"

    private int SelectedNodeID
    {
        get
        {
            return ValidationHelper.GetInteger(Request.Params["selectedNodeId"], 0);
        }
    }


    private int ExpandNodeID
    {
        get
        {
            return QueryHelper.GetInteger("expandnodeid", 0);
        }
    }


    private int SelectedDocumentID
    {
        get
        {
            return ValidationHelper.GetInteger(Request.Params["selectedDocId"], 0);
        }
    }


    private string SelectedCulture
    {
        get
        {
            return ValidationHelper.GetString(Request.Params["selectedCulture"], MembershipContext.AuthenticatedUser.PreferredCultureCode);
        }
    }


    private string SelectedDevice
    {
        get
        {
            return ValidationHelper.GetString(Request.Params["selectedDevice"], null);
        }
    }


    private TreeNode RootNode
    {
        get
        {
            // Root
            string baseDoc = "/";
            if (ProductsStartingPath != String.Empty)
            {
                // Change to user's root page
                baseDoc = ProductsStartingPath;
            }
            // Try to get culture-specific root node
            TreeNode rootNode = Tree.SelectSingleNode(SiteContext.CurrentSiteName, baseDoc, MembershipContext.AuthenticatedUser.PreferredCultureCode, false, null, false);

            if (rootNode == null)
            {
                // Get root node
                rootNode = Tree.SelectSingleNode(SiteContext.CurrentSiteName, baseDoc, TreeProvider.ALL_CULTURES, false, null, false);
            }

            return rootNode;
        }
    }


    protected int ResultNodeID
    {
        get
        {
            if (mResultNodeID == null)
            {
                // Get ID from query string
                mResultNodeID = NodeID;
                if (mResultNodeID <= 0)
                {
                    // Get ID selected by user
                    mResultNodeID = SelectedNodeID;
                    if (mResultNodeID <= 0)
                    {
                        // If no node specified, add the root node id
                        if (NodeID <= 0)
                        {
                            TreeNode rootNode = RootNode;
                            if (rootNode != null)
                            {
                                mResultNodeID = rootNode.NodeID;
                            }
                        }
                    }
                }
            }
            return mResultNodeID.Value;
        }
    }


    protected int ResultDocumentID
    {
        get
        {
            if (mResultDocumentID == null)
            {
                // Get ID from query string
                mResultDocumentID = DocumentID;
                if (mResultDocumentID <= 0)
                {
                    // Get ID selected by user
                    mResultDocumentID = SelectedDocumentID;
                    if ((mResultDocumentID <= 0) && (NodeID <= 0))
                    {
                        TreeNode rootNode = RootNode;
                        // If the culture match with selected culture
                        if ((rootNode != null) && rootNode.DocumentCulture.EqualsCSafe(SelectedCulture))
                        {
                            // Get identifier from the root node
                            mResultDocumentID = rootNode.DocumentID;
                        }
                    }
                }
            }
            return mResultDocumentID.Value;
        }
    }


    /// <summary>
    /// Resulting device. Prefers user choice over query string setting.
    /// </summary>
    protected string ResultDevice
    {
        get
        {
            if (mResultDevice == null)
            {
                mResultDevice = SelectedDevice;
                if (mResultDevice == null)
                {
                    mResultDevice = Device ?? DeviceContext.CurrentDeviceProfileName;
                }
            }
            return mResultDevice;
        }
    }

    #endregion


    #region "Page events"

    /// <summary>
    /// Constructor
    /// </summary>
    public CMSModules_Ecommerce_Pages_Tools_Products_Products_Frameset()
    {
        new ContentUrlRetriever(this, ProductUIHelper.GetProductPageUrl);
    }


    protected override void OnPreInit(EventArgs e)
    {
        base.OnPreInit(e);

        // Do not include document manager to the controls collection
        EnsureDocumentManager = false;
    }


    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
        
        string contentUrl = "Product_List.aspx" + RequestContext.CurrentQueryString;

        // Display product list if display tree of product sections is not allowed
        if (ECommerceSettings.ProductsTree(SiteContext.CurrentSiteID) == ProductsTreeModeEnum.None)
        {
            URLHelper.Redirect(URLHelper.ResolveUrl(contentUrl));
        }

        contenttree.Values.AddRange(new[] { new UILayoutValue("NodeID", ResultNodeID), new UILayoutValue("ExpandNodeID", ExpandNodeID), new UILayoutValue("Culture", SelectedCulture) });

        if (NodeID <= 0)
        {
            // Root
            string baseDoc = "/";
            if (!string.IsNullOrEmpty(ProductsStartingPath))
            {
                // Change to products root node
                baseDoc = ProductsStartingPath.TrimEnd('/');
            }

            // Get the root node
            TreeNode rootNode = Tree.SelectSingleNode(SiteContext.CurrentSiteName, baseDoc, TreeProvider.ALL_CULTURES, false, null, false);
            if (rootNode != null)
            {
                string nodeString = rootNode.NodeID.ToString();
                contentUrl = URLHelper.AddParameterToUrl(contentUrl, "nodeId", nodeString);

                // Set default live site URL in header link
                string liveURL = URLHelper.ResolveUrl(rootNode.RelativeURL) + "?viewmode=livesite";
                ScriptHelper.RegisterStartupScript(this, typeof(string), "SetDefaultLiveSiteURL", ScriptHelper.GetScript("SetLiveSiteURL('" + liveURL + "');"));
            }
        }

        contentview.Src = contentUrl;

        ScriptHelper.RegisterScriptFile(Page, "~/CMSModules/Content/CMSDesk/Content.js");

        // Override content functions
        AddScript(
@"
function SetMode(mode, passive) {
    if (!CheckChanges()) {
        return false;
    }
 
    SetSelectedMode(mode);
    if (!passive) {
        DisplayDocument();
    }
    return true;
}

function DragOperation(nodeId, targetNodeId, operation) {
    window.PerformContentRedirect(null, 'drag', nodeId, '&action=' + operation + '&targetnodeid=' + targetNodeId + '&mode=productssection');
}
");
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Adds the script to the output request window.
    /// </summary>
    /// <param name="script">Script to add</param>
    public override void AddScript(string script)
    {
        ScriptHelper.RegisterStartupScript(this, typeof(string), script.GetHashCode().ToString(), ScriptHelper.GetScript(script));
    }

    #endregion
}
