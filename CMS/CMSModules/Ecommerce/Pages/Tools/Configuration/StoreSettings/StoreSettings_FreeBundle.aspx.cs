﻿using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.DataEngine;
using CMS.CMSHelper;
using CMS.Ecommerce;
using CMS.EcommerceProvider;
using CMS.GlobalHelper;
using CMS.SettingsProvider;
using CMS.SiteProvider;
using CMS.UIControls;
using CMS.ExtendedControls;
using CMS.Helpers;

// [Security(Resource = "CMS.Ecommerce", UIElements = "Configuration.Settings.CheckoutProcess")]
public partial class CMSModules_Ecommerce_Pages_Tools_Configuration_StoreSettings_StoreSettings_FreeBundle : CMSEcommerceStoreSettingsPage
{
    private SiteInfo configuredSite = null;

    /// <summary>
    /// Indicates if header actions are visible.
    /// </summary>
    public bool ActionsVisible
    {
        get
        {
            return CurrentMaster.HeaderActions.Visible;
        }
        set
        {
            CurrentMaster.HeaderActionsPlaceHolder.Visible = value;
        }
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        configuredSite = SiteInfoProvider.GetSiteInfo(ConfiguredSiteID);

        // Set up header
        CurrentMaster.HeaderActions.Actions = GetHeaderActions();
        CurrentMaster.HeaderActions.ActionPerformed += HeaderActions_ActionPerformed;

        // Modify page content css class
        Panel pnl = CurrentMaster.PanelBody.FindControl("pnlContent") as Panel;
        if (pnl != null)
        {
            pnl.CssClass = "";
        }

        // Hide buttons - this page has custom header action
        //ucCheckoutProcess.ShowActions = false;
        //ucCheckoutProcess.OnModeChanged += new CMSModules_Ecommerce_FormControls_CheckoutProcess.ModeChangedHandler(ucCheckoutProcess_OnModeChanged);

        // Register javascript to confirm generate default checkout process
        //string script = "function ConfirmGlobalProcess() {return confirm(" + ScriptHelper.GetString(GetString("CheckoutProcess.ConfirmGlobalProcess")) + ");}";
        //ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "ConfirmGlobalProcess", ScriptHelper.GetScript(script));

        //ucCheckoutProcess.OnCheckoutProcessDefinitionUpdate += new OnCheckoutProcessDefinitionUpdateEventHandler(ucCheckoutProcess_OnCheckoutProcessDefinitionUpdate);

        // Load data
        if (!RequestHelper.IsPostBack())
        {
            if (configuredSite != null)
            {
                //ucCheckoutProcess.Value = ECommerceSettings.CheckoutProcess(configuredSite.SiteName);
            }
            else
            {
                //ucCheckoutProcess.Value = ECommerceSettings.CheckoutProcess(null);
            }

            //ucCheckoutProcess.EnableDefaultCheckoutProcessTypes = true;
        }
        GetAndUpdateCustomTableQueryItem();
    }

    private void GetAndUpdateCustomTableQueryItem()
    {
        GeneralConnection cn = ConnectionHelper.GetConnection();

        DataSet ds = cn.ExecuteQuery("customtable.customBundle.CustomBundleList", null);

        if (!DataHelper.DataSourceIsEmpty(ds))
        {
            StoreSettings_FreeBundleList.DataSource = ds;

        }
    }


    /// <summary>
    /// Handles checkout process controls mode change event.
    /// </summary>
    /// <param name="isListingMode">Listing mode flag</param>
    private void ucCheckoutProcess_OnModeChanged(bool isListingMode)
    {
        ActionsVisible = isListingMode;
    }


    /// <summary>
    /// Gets string array representing header actions.
    /// </summary>
    /// <returns>Array of strings</returns>
    private string[,] GetHeaderActions()
    {
        string[,] actions = null;
        
        if (ConfiguredSiteID == 0)
        {
            actions = new string[1, 9];
        }
        else
        {
            actions = new string[1, 9];

            /*actions[2, 0] = HeaderActions.TYPE_HYPERLINK;
            actions[2, 1] = GetString("CheckoutProcess.FromGlobalProcess");
            actions[2, 2] = "return ConfirmGlobalProcess();";
            actions[2, 3] = null;
            actions[2, 4] = null;
            actions[2, 5] = GetImageUrl("CMSModules/CMS_Ecommerce/processfromglobal.png");
            actions[2, 6] = "fromGlobalProcess";
            actions[2, 7] = String.Empty;
            actions[2, 8] = true.ToString();*/
        }

        actions[0, 0] = HeaderActions.TYPE_HYPERLINK;
        actions[0, 1] = "New code";// GetString("CheckoutProcess.NewStep");
        actions[0, 2] = "";
        actions[0, 3] = null;
        actions[0, 4] = null;
        actions[0, 5] = GetImageUrl("CMSModules/CMS_Ecommerce/addstep.png");
        actions[0, 6] = "newStep";
        actions[0, 7] = String.Empty;
        actions[0, 8] = true.ToString();

        /*actions[1, 0] = HeaderActions.TYPE_HYPERLINK;
        actions[1, 1] = GetString("CheckoutProcess.DefaultProcess");
        actions[1, 2] = "return ConfirmDefaultProcess();";
        actions[1, 3] = null;
        actions[1, 4] = null;
        actions[1, 5] = GetImageUrl("CMSModules/CMS_Ecommerce/processreset.png");
        actions[1, 6] = "defaultProcess";
        actions[1, 7] = String.Empty;
        actions[1, 8] = true.ToString();*/

        return actions;
    }


    /// <summary>
    /// Handles actions performed on the master header.
    /// </summary>
    protected void HeaderActions_ActionPerformed(object sender, CommandEventArgs e)
    {
        switch (e.CommandName.ToLowerCSafe())
        {
            case "newstep":
                URLHelper.Redirect("StoreSettings_FreeBundle/StoreSettings_FreeBundle_Add.aspx");
                break;
        }
    }


    private void ucCheckoutProcess_OnCheckoutProcessDefinitionUpdate(string action)
    {
        /*
        // Check 'EcommerceModify' permission
        if (!CMSContext.CurrentUser.IsAuthorizedPerResource("CMS.Ecommerce", "ConfigurationModify"))
        {
            RedirectToCMSDeskAccessDenied("CMS.Ecommerce", "ConfigurationModify");
        }

        switch (action.ToLowerCSafe())
        {
            case "update":
            case "delete":
            case "moveup":
            case "movedown":
                // Update checkout process xml definition in database
                SaveProcess();
                break;

            case "defaultprocess":
                // Set default checkout process

                ucCheckoutProcess.Value = ShoppingCart.DEFAULT_CHECKOUT_PROCESS;
                ucCheckoutProcess.ReloadData();
                SaveProcess();
                break;

            case "fromglobalprocess":
                // Set default checkout process
                if (configuredSite != null)
                {
                    ucCheckoutProcess.Value = ECommerceSettings.CheckoutProcess(null);
                    ucCheckoutProcess.ReloadData();
                    SaveProcess();
                }
                break;
        }*/
    }


    private void SaveProcess()
    {
        /*
        if (configuredSite != null)
        {
            SettingsKeyProvider.SetValue(configuredSite.SiteName + "." + ECommerceSettings.CHECKOUT_PROCESS, ucCheckoutProcess.Value.ToString());
        }
        else
        {
            SettingsKeyProvider.SetValue(ECommerceSettings.CHECKOUT_PROCESS, ucCheckoutProcess.Value.ToString());
        }
        ucCheckoutProcess.Information = GetString("General.ChangesSaved");
        */
    }
}