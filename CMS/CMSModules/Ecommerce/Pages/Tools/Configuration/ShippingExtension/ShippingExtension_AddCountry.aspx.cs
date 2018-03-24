using System;
using System.Data;
using CMS.CMSHelper;
using CMS.DataEngine;
using CMS.Ecommerce;
using CMS.GlobalHelper;
using CMS.SettingsProvider;
using CMS.SiteProvider;
using CMS.UIControls;
using CMS.Helpers;
using CMS.Globalization;
using CMS.CustomTables;
using CMS.Membership;

[Title("Objects/Ecommerce_ShippingOption/object.png", "Add country to shipping extension", "")]
public partial class CMSModules_Ecommerce_Pages_Tools_Configuration_ShippingExtension_ShippingExtension_AddCountry : CMSShippingOptionsPage
{
    #region "Page Events"

    private int shippingExtensionID = QueryHelper.GetInteger("shippingExtensionID", 0);

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
        string shippingExtensionName = GetShippingOptionName(shippingExtensionID.ToString());
        string[,] breadcrumbs = new string[3, 3];
        breadcrumbs[0, 0] = "Shipping Extension";
        breadcrumbs[0, 1] = "~/CMSModules/Ecommerce/Pages/Tools/Configuration/ShippingExtension/ShippingExtension_List.aspx";
        breadcrumbs[0, 2] = "configEdit";
        breadcrumbs[1, 0] = shippingExtensionName;
        breadcrumbs[1, 1] = string.Format("~/CMSModules/Ecommerce/Pages/Tools/Configuration/ShippingExtension/ShippingExtension_Edit_Country.aspx?shippingExtensionID={0}", shippingExtensionID.ToString());
        breadcrumbs[1, 2] = "configEdit";
        breadcrumbs[2, 0] = "Add a country";
        breadcrumbs[2, 1] = "";
        breadcrumbs[2, 2] = "configEdit";

        CMSMasterPage master = (CMSMasterPage)CurrentMaster;
        master.Title.Breadcrumbs = breadcrumbs;
    }

    private void GetAndUpdateCustomTableQueryItem()
    {
        string query = string.Format("SELECT CountryID, CountryDisplayName, CountryName, CountryTwoLetterCode, CountryThreeLetterCode FROM dbo.CMS_Country WHERE CountryID NOT IN (SELECT ShippingCountryID FROM customtable_shippingextensioncountry WHERE ShippingOptionID={0})", shippingExtensionID.ToString());
        GeneralConnection cn = ConnectionHelper.GetConnection();
        DataSet ds = cn.ExecuteQuery(query, null, QueryTypeEnum.SQLQuery, false);

        if (!DataHelper.DataSourceIsEmpty(ds))
        {
            UniGrid.DataSource = ds;
        }
    }

    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);
    }

    #endregion "Page Events"

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
            CountryInfo country = CountryInfoProvider.GetCountryInfo(Convert.ToInt32(actionArgument));
            // Check if country can be added in the ShippingExtension
            if (CheckCountry(Convert.ToInt32(actionArgument)))
            {
                // Add itemId in customtable_shippingextension.shippingoptionid
                // Creates new Custom table item provider
                CustomTableItemProvider customTableProvider = new CustomTableItemProvider(MembershipContext.AuthenticatedUser);

                // Prepares the parameters
                string customTableClassName = "customtable.shippingextensioncountry";

                // Checks if Custom table exists
                DataClassInfo customTable = DataClassInfoProvider.GetDataClass(customTableClassName);
                if (customTable != null)
                {
                    // Creates new custom table item
                    CustomTableItem newCustomTableItem = CustomTableItem.New(customTableClassName, customTableProvider);

                    // Sets the ItemText field value
                    newCustomTableItem.SetValue("ShippingOptionId", shippingExtensionID.ToString());
                    newCustomTableItem.SetValue("ShippingCountryId", actionArgument.ToString());
                    newCustomTableItem.SetValue("ShippingBase", 0);
                    newCustomTableItem.SetValue("LocalContact", string.Empty);
                    newCustomTableItem.SetValue("Enabled", true);
                    newCustomTableItem.SetValue("ProcessingMode", 1);
                    newCustomTableItem.SetValue("UnitPrice", 0);
                    // Inserts the custom table item into database
                    newCustomTableItem.Insert();
                }
                //Response.Redirect(Request.Url.ToString());
                ShowInformation(string.Format("<b>{0}</b> added for <b>{1}</b>", country.CountryName, GetShippingOptionName(shippingExtensionID.ToString())));
            }
            else
            {
                ShowError(string.Format("<b>{0}</b> is already defined for <b>{1}</b>", country.CountryName, GetShippingOptionName(shippingExtensionID.ToString())));
            }
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

    #endregion "Event Handlers"

    #region "Methods"

    private string GetShippingOptionName(string ShippingOptionID)
    {
        string result = string.Empty;
        GeneralConnection cn = ConnectionHelper.GetConnection();
        string stringQuery = string.Format("SELECT ShippingOptionName,ShippingOptionDisplayName from COM_ShippingOption WHERE ShippingOPtionID={0}", ShippingOptionID);
        DataSet ds = cn.ExecuteQuery(stringQuery, null, QueryTypeEnum.SQLQuery, false);
        if (!DataHelper.DataSourceIsEmpty(ds))
        {
            result = ValidationHelper.GetString(ds.Tables[0].Rows[0]["ShippingOptionDisplayName"], string.Empty);
        }
        return result;
    }

    private bool CheckCountry(int countryId)
    {
        bool result = true;
        GeneralConnection cn = ConnectionHelper.GetConnection();
        string stringQuery = string.Format("SELECT ShippingCountryId from customtable_shippingextensioncountry WHERE ShippingOPtionID={0} AND ShippingCountryId={1}", shippingExtensionID.ToString(), countryId.ToString());
        DataSet ds = cn.ExecuteQuery(stringQuery, null, QueryTypeEnum.SQLQuery, false);
        result = DataHelper.DataSourceIsEmpty(ds);
        return result;
    }

    #endregion "Methods"
}