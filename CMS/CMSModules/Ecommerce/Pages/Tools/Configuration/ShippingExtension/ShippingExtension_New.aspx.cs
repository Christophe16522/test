using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

using CMS.CMSHelper;
using CMS.DataEngine;
using CMS.Ecommerce;
using CMS.GlobalHelper;
using CMS.SettingsProvider;
using CMS.UIControls;
using CMS.ExtendedControls.ActionsConfig;
using CMS.SiteProvider;
using CMS.Helpers;
using CMS.CustomTables;
using CMS.Membership;

[Title("Objects/Ecommerce_ShippingOption/object.png", "New Shipping extension", "")]
public partial class CMSModules_Ecommerce_Pages_Tools_Configuration_ShippingExtension_ShippingExtension_New : CMSShippingOptionsPage
{
    #region "Page Events"

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        // Init Unigrid
        UniGrid.OnAction += new OnActionEventHandler(uniGrid_OnAction);
        UniGrid.OnExternalDataBound += new OnExternalDataBoundEventHandler(UniGrid_OnExternalDataBound);
        UniGrid.ZeroRowsText = GetString("general.nodatafound");

    }


    protected void Page_Load(object sender, EventArgs e)
    {
        CurrentMaster.DisplaySiteSelectorPanel = AllowGlobalObjects;
        GetAndUpdateCustomTableQueryItem();
        string[,] breadcrumbs = new string[2, 3];
        breadcrumbs[0, 0] = "Shipping Extension";
        breadcrumbs[0, 1] = "~/CMSModules/Ecommerce/Pages/Tools/Configuration/ShippingExtension/ShippingExtension_List.aspx";
        breadcrumbs[0, 2] = "configEdit";
        breadcrumbs[1, 0] = "New shipping extension";
        breadcrumbs[1, 1] = "";
        breadcrumbs[1, 2] = "";

        CMSMasterPage master = (CMSMasterPage)CurrentMaster;
        master.Title.Breadcrumbs = breadcrumbs;

    }

    private void GetAndUpdateCustomTableQueryItem()
    {
        GeneralConnection cn = ConnectionHelper.GetConnection();
        DataSet ds = cn.ExecuteQuery("customtable.shippingextension.Available", null);

        if (!DataHelper.DataSourceIsEmpty(ds))
        {
            UniGrid.DataSource = ds;
        }
    }

    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);
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
        if (actionName == "add")
        {
            // Add itemId in customtable_shippingextension.shippingoptionid

            // Creates new Custom table item provider
            CustomTableItemProvider customTableProvider = new CustomTableItemProvider(MembershipContext.AuthenticatedUser);

            // Prepares the parameters
            string customTableClassName = "customtable.shippingextension";

            // Checks if Custom table exists
            DataClassInfo customTable = DataClassInfoProvider.GetDataClass(customTableClassName);
            if (customTable != null)
            {
                // Creates new custom table item
                CustomTableItem newCustomTableItem = CustomTableItem.New(customTableClassName, customTableProvider);

                // Sets the ItemText field value
                newCustomTableItem.SetValue("ShippingOptionId", actionArgument.ToString());
                newCustomTableItem.SetValue("LocalContact", string.Empty);
                newCustomTableItem.SetValue("Enabled", true);
                newCustomTableItem.SetValue("ProcessingMode", 0);

                // Inserts the custom table item into database
                newCustomTableItem.Insert();

            }
            //Response.Redirect("ShippingExtension_List.aspx");
            Response.Redirect(Request.Url.ToString());
            
        }
    }


    private object UniGrid_OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        switch (sourceName.ToLowerCSafe())
        {
            case "shippoptenabled":
                return UniGridFunctions.ColoredSpanYesNo(parameter);
            case "shippingoptionsiteid":
                return UniGridFunctions.ColoredSpanYesNo(parameter == DBNull.Value);
            case "shippoptcharge":
                DataRowView row = (DataRowView)parameter;
                double value = ValidationHelper.GetDouble(row["ShippingOptionCharge"], 0);
                int siteId = ValidationHelper.GetInteger(row["ShippingOptionSiteID"], 0);

                return CurrencyInfoProvider.GetFormattedPrice(value, siteId);
        }

        return parameter;
    }


    /// <summary>
    /// Handles the SiteSelector's selection changed event.
    /// </summary>
    #endregion
}