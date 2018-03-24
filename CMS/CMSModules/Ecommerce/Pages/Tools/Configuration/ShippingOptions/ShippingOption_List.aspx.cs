using System;
using System.Data;

using CMS.Core;
using CMS.DataEngine;
using CMS.Ecommerce;
using CMS.ExtendedControls;
using CMS.ExtendedControls.ActionsConfig;
using CMS.FormEngine;
using CMS.Helpers;
using CMS.Base;
using CMS.PortalEngine;
using CMS.SiteProvider;
using CMS.UIControls;

[Title("COM_ShippingOption_List.HeaderCaption")]
public partial class CMSModules_Ecommerce_Pages_Tools_Configuration_ShippingOptions_ShippingOption_List : CMSShippingOptionsPage
{
    #region "Page Events"

    protected void Page_Load(object sender, EventArgs e)
    {
        string newElementName;

        if (IsMultiStoreConfiguration)
        {
            newElementName = "new.configuration.globalshippingoptions";
            CheckUIElementAccessHierarchical(ModuleName.ECOMMERCE, "Ecommerce.GlobalShippingOptions");
        }
        else
        {
            newElementName = "new.Configuration.ShippingOptions";
            CheckUIElementAccessHierarchical(ModuleName.ECOMMERCE, "Configuration.ShippingOptions");
        }

        // Header actions
        HeaderActions actions = CurrentMaster.HeaderActions;
        actions.ActionsList.Add(new HeaderAction
        {
            Text = GetString("COM_ShippingOption_List.NewItemCaption"),
            RedirectUrl = GetRedirectURL(newElementName),
        });

        // Init Unigrid
        UniGrid.OnAction += uniGrid_OnAction;
        UniGrid.OnExternalDataBound += UniGrid_OnExternalDataBound;
        HandleGridsSiteIDColumn(UniGrid);

        if (UseGlobalObjects && ExchangeTableInfoProvider.IsExchangeRateFromGlobalMainCurrencyMissing(SiteContext.CurrentSiteID))
        {
            ShowWarning(GetString("com.NeedExchangeRateFromGlobal"));
        }

        InitWhereCondition();
    }

    #endregion


    #region "Event Handlers"

    /// <summary>
    /// Handles the UniGrid's OnAction event.
    /// </summary>
    /// <param name="actionName">Name of item (button) that throws event</param>
    /// <param name="actionArgument">ID (value of Primary key) of corresponding data row</param>
    protected void uniGrid_OnAction(string actionName, object actionArgument)
    {
        if (actionName == "edit")
        {
            var editElementName = IsMultiStoreConfiguration ? "Edit.Ecommerce.GlobalShippingOptions.Properties" : "Edit.Configuration.ShippingOptionProperties";
            URLHelper.Redirect(UIContextHelper.GetElementUrl("CMS.Ecommerce", editElementName, false, actionArgument.ToInteger(0)));
        }
        else if (actionName == "delete")
        {
            var shippingInfoObj = ShippingOptionInfoProvider.GetShippingOptionInfo(ValidationHelper.GetInteger(actionArgument, 0));
            // Nothing to delete
            if (shippingInfoObj == null)
            {
                return;
            }

            // Check permissions
            CheckConfigurationModification(shippingInfoObj.ShippingOptionSiteID);

            // Check dependencies
            if (shippingInfoObj.Generalized.CheckDependencies())
            {
                // Show error message
                ShowError(GetString("Ecommerce.DeleteDisabled"));

                return;
            }
            // Delete ShippingOptionInfo object from database
            ShippingOptionInfoProvider.DeleteShippingOptionInfo(shippingInfoObj);
        }
    }


    private object UniGrid_OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        switch (sourceName.ToLowerCSafe())
        {
            case "shippingoptioncharge":
                DataRowView row = (DataRowView)parameter;
                double value = ValidationHelper.GetDouble(row["ShippingOptionCharge"], 0);
                int siteId = ValidationHelper.GetInteger(row["ShippingOptionSiteID"], 0);

                return CurrencyInfoProvider.GetFormattedPrice(value, siteId);
        }

        return parameter;
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Creates where condition for UniGrid and reloads it.
    /// </summary>
    private void InitWhereCondition()
    {
        string where;

        if (IsMultiStoreConfiguration)
        {
            where = "ShippingOptionSiteID IS NULL";
        }
        else
        {
            where = "ShippingOptionSiteID = " + SiteContext.CurrentSiteID;
            if (UseGlobalObjects)
            {
                where = SqlHelper.AddWhereCondition(where, "ShippingOptionSiteID IS NULL", "OR");
            }
        }

        UniGrid.WhereCondition = where;
    }


    /// <summary>
    /// Generates redirection url with query string parameters.
    /// </summary>
    /// <param name="uiElementName">Name of ui element to redirect to.</param>
    private string GetRedirectURL(string uiElementName)
    {
        string url = UIContextHelper.GetElementUrl("cms.ecommerce", uiElementName, false);
        // Only global object can be created from site manager       
        if (IsMultiStoreConfiguration)
        {
            url = URLHelper.AddParameterToUrl(url, "siteid", SpecialFieldValue.GLOBAL.ToString());
        }

        return url;
    }

    #endregion
}