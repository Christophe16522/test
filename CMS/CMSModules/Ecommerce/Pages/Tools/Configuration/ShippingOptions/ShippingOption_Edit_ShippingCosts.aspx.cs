using System;
using System.Data;
using System.Web.UI.WebControls;

using CMS.Core;
using CMS.Ecommerce;
using CMS.Helpers;
using CMS.Base;
using CMS.UIControls;
using CMS.ExtendedControls;

// Edited object
[ParentObject(ShippingOptionInfo.OBJECT_TYPE, "objectid")]
// Actions
[Action(0, "com.ui.shippingcost.edit_new", "ShippingOption_Edit_ShippingCosts_Edit.aspx?objectid={%EditedObjectParent.ID%}&siteId={?siteId?}")]
// Help
[Help("shippingcosts_list", "helpTopic")]
public partial class CMSModules_Ecommerce_Pages_Tools_Configuration_ShippingOptions_ShippingOption_Edit_ShippingCosts : CMSShippingOptionsPage
{
    #region "Variables"

    protected ShippingOptionInfo shippingOption = null;
    protected CurrencyInfo currency = null;

    #endregion


    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        if (IsMultiStoreConfiguration)
        {
            CheckUIElementAccessHierarchical(ModuleName.ECOMMERCE, "Ecommerce.GlobalShippingOptions.ShippingCosts");
        }
        else
        {
            CheckUIElementAccessHierarchical(ModuleName.ECOMMERCE, "Configuration.ShippingOptions.ShippingCosts");
        }

        shippingOption = EditedObjectParent as ShippingOptionInfo;
        if (shippingOption != null)
        {
            CheckEditedObjectSiteID(shippingOption.ShippingOptionSiteID);
            currency = CurrencyInfoProvider.GetMainCurrency(shippingOption.ShippingOptionSiteID);
        }

        // Init unigrid
        gridElem.OnAction += gridElem_OnAction;
        gridElem.OnExternalDataBound += gridElem_OnExternalDataBound;
        gridElem.OnAfterRetrieveData += gridElem_OnAfterRetrieveData;
        gridElem.ZeroRowsText = GetString("com.ui.shippingcost.edit_nodata");
        gridElem.GridView.AllowSorting = false;
    }

    #endregion


    #region "Event handlers"

    /// <summary>
    /// Handles the UniGrid's OnAction event.
    /// </summary>
    /// <param name="actionName">Name of item (button) that throws event</param>
    /// <param name="actionArgument">ID (value of Primary key) of corresponding data row</param>
    protected void gridElem_OnAction(string actionName, object actionArgument)
    {
        switch (actionName.ToLowerCSafe())
        {
            case "delete":
                // Check permissions
                CheckConfigurationModification(shippingOption.ShippingOptionSiteID);

                // Delete ShippingCostInfo object from database
                ShippingCostInfoProvider.DeleteShippingCostInfo(Convert.ToInt32(actionArgument));
                break;
        }
    }


    /// <summary>
    /// Handles the UniGrid's OnAfterRetrieveData event. Appends cost for zero weight.
    /// </summary>
    /// <param name="ds">Input data</param>
    protected DataSet gridElem_OnAfterRetrieveData(DataSet ds)
    {
        if (!DataHelper.DataSourceIsEmpty(ds) && (shippingOption != null))
        {
            // Create row for zero weight shipping cost
            DataTable table = ds.Tables[0];
            DataRow zeroWeightRow = table.NewRow();

            zeroWeightRow["ShippingCostID"] = 0;
            zeroWeightRow["ShippingCostMinWeight"] = 0;
            zeroWeightRow["ShippingCostValue"] = shippingOption.ShippingOptionCharge;

            // Insert record at first position
            table.Rows.InsertAt(zeroWeightRow, 0);
        }

        return ds;
    }


    /// <summary>
    /// Handles the UniGrid's OnExternalDataBound event.
    /// </summary>
    /// <param name="sender">Sender</param>
    /// <param name="sourceName">Source name</param>
    /// <param name="parameter">Parameter</param>
    protected object gridElem_OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        switch (sourceName.ToLowerCSafe())
        {
            case "shippingcostvalue":
                double value = ValidationHelper.GetDouble(parameter, 0);

                return CurrencyInfoProvider.GetFormattedPrice(value, currency);

            case "edit":
            case "delete":
                if (sender is CMSGridActionButton)
                {
                    // Hide editing/deleting of zero-row (cost from general tab)
                    int shippingCostId = ValidationHelper.GetInteger(((DataRowView)((GridViewRow)parameter).DataItem)[0], 0);
                    if (shippingCostId == 0)
                    {
                        ((CMSGridActionButton)sender).Visible = false;
                    }
                }

                break;
        }
        return parameter;
    }

    #endregion
}