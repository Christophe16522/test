using System;

using CMS.Core;
using CMS.Ecommerce;
using CMS.Helpers;
using CMS.UIControls;
using CMS.WorkflowEngine;

[Title("com.ui.productsworkflow")]
[UIElement(ModuleName.ECOMMERCE, "Products.Workflow")]
public partial class CMSModules_Ecommerce_Pages_Tools_Products_Product_Edit_Workflow : CMSProductsPage
{
    #region "Variables"

    private WorkflowInfo workflow = null;

    #endregion


    #region "Page events"

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        // Disable confirm changes checking
        DocumentManager.RegisterSaveChangesScript = false;

        // Init node
        workflowElem.Node = Node;

        workflow = DocumentManager.Workflow;
        if (workflow != null)
        {
            menuElem.OnClientStepChanged = ClientScript.GetPostBackEventReference(pnlUp, null);

            // Backward compatibility - Display Archive button for all steps
            menuElem.ForceArchive = workflow.IsBasic;
        }

        // Enable split mode
        EnableSplitMode = true;
    }


    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        pnlContainer.Enabled = !DocumentManager.ProcessingAction;
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        if (workflow != null)
        {
            // Backward compatibility
            if (workflow.WorkflowAutoPublishChanges)
            {
                string message = DocumentManager.GetDocumentInfo(true);
                if (!string.IsNullOrEmpty(message))
                {
                    message += "<br />";
                }
                message += GetString("WorfklowProperties.AutoPublishChanges");
                DocumentManager.DocumentInfo = message;
            }
        }

        // Register the scripts
        if (!DocumentManager.RefreshActionContent)
        {
            ScriptHelper.RegisterLoader(Page);
        }
    }

    #endregion
}
