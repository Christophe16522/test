
using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.Ecommerce;
using CMS.CMSHelper;
using CMS.GlobalHelper;
using CMS.SiteProvider;
using CMS.UIControls;
using CMS.SettingsProvider;
using CMS.DataEngine;
using CMS.Helpers;
using CMS.Globalization;

[Title("Objects/Ecommerce_ShippingOption/object.png", "Shipping extension country view details", "newgeneral_tab2")]
public partial class CMSModules_Ecommerce_Pages_Tools_Configuration_ShippingExtension_ShippingExtension_CountryViewDetail : CMSShippingOptionsPage
{
    protected int mShippingExtensionCountryID = QueryHelper.GetInteger("shippingCountryID", 0);
    protected int mPriceID = QueryHelper.GetInteger("PriceID", 0);
    protected int mShippingOptionID = -1, mShippingUnitFrom = -1, mShippingUnitTo = -1;
    protected string ShippingOptionDisplayName, ShippingCountryDisplayName;
    protected decimal price = 0;

    protected object userGrid_OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        // User ID is used as actions parameter editedItemId
        string editedItemId = "";
        bool isAdmin = false;
        object param = null;

        if (parameter is System.Web.UI.WebControls.GridViewRow)
        {
            param = ((System.Web.UI.WebControls.GridViewRow)parameter).DataItem;
        }

        editedItemId = ((System.Data.DataRowView)(param)).Row["UserId"].ToString();
        isAdmin = ValidationHelper.GetBoolean(((System.Data.DataRowView)(param)).Row["UserIsGlobalAdministrator"], false);

        string sImageUrl = GetImageUrl("Design/Controls/UniGrid/Actions/Delete.png");
        if (isAdmin)
        {
            sImageUrl = GetImageUrl("Design/Controls/UniGrid/Actions/Edit.png");
        }

        switch (sourceName)
        {
            case "deleteaction":
                ImageButton btn = ((ImageButton)sender);
                btn.ImageUrl = sImageUrl;
                return btn;
        }
        return parameter;
    }

    private DataSet GetDataSource()
    {
        DataSet result;
        string query = @"SELECT     dbo.customtable_shippingextensioncountry.ItemID, dbo.customtable_shippingextensioncountry.ShippingOptionId, 
                      dbo.customtable_shippingextensioncountry.LocalContact, CASE WHEN Enabled = 1 THEN 'Yes' ELSE 'No' END AS Enabled, 
                      dbo.customtable_shippingextensioncountry.ShippingBase, dbo.customtable_shippingextensioncountry.ProcessingMode, 
                      dbo.customtable_shippingextensioncountry.UnitPrice, dbo.COM_ShippingOption.ShippingOptionDisplayName,CASE WHEN ProcessingMode = 0 THEN 'By Range' ELSE 'By Unit' END AS Processing
                      FROM         dbo.customtable_shippingextensioncountry INNER JOIN
                                   dbo.COM_ShippingOption ON dbo.customtable_shippingextensioncountry.ShippingOptionId = dbo.COM_ShippingOption.ShippingOptionID
                      WHERE        (dbo.customtable_shippingextensioncountry.ShippingCountryId = {0})";
        
        GeneralConnection cn = ConnectionHelper.GetConnection();
        result = cn.ExecuteQuery(string.Format(query, mShippingExtensionCountryID), null, QueryTypeEnum.SQLQuery, false); 

        return result;
    }

    private void OnAction(string actionName, object actionArgument)
    {
        switch (actionName.ToLower())
        {
            case "deleteaction":
                break;
        }
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        CountryGrid.DataSource = GetDataSource();
        CountryGrid.OnAction += OnAction;
        ShippingCountryDisplayName = CountryInfoProvider.GetCountryInfo(mShippingExtensionCountryID).CountryDisplayName;
        string[,] breadcrumbs = new string[3, 3];
        breadcrumbs[0, 0] = "Shipping Extension";
        breadcrumbs[0, 1] = "~/CMSModules/Ecommerce/Pages/Tools/Configuration/ShippingExtension/ShippingExtension_List.aspx";
        breadcrumbs[0, 2] = "configEdit";
        breadcrumbs[1, 0] = "Country view summary";
        breadcrumbs[1, 1] = "~/CMSModules/Ecommerce/Pages/Tools/Configuration/ShippingExtension/ShippingExtension_CountryView.aspx";
        breadcrumbs[1, 2] = "configEdit";
        breadcrumbs[2, 0] = ShippingCountryDisplayName;
        breadcrumbs[2, 1] = "";
        breadcrumbs[2, 2] = ""; /*
        breadcrumbs[3, 0] = "Edit price";
        breadcrumbs[3, 1] = "";
        breadcrumbs[3, 2] = "";*/


        CMSMasterPage master = (CMSMasterPage)CurrentMaster;
        master.Title.Breadcrumbs = breadcrumbs;

        if (!IsPostBack)
        {
        }
    }

    
    private void GetShippingOptionName(string ShippingCountryID)
    {
        GeneralConnection cn = ConnectionHelper.GetConnection();
        string stringQuery = string.Format("SELECT .ItemID, C.ShippingOptionID, S.ShippingCountryId, C.ShippingOptionDisplayName, dbo.CMS_Country.CountryDisplayName FROM dbo.customtable_shippingextensioncountry AS S INNER JOIN dbo.COM_ShippingOption AS C ON C.ShippingOptionID = S.ShippingOptionId INNER JOIN dbo.CMS_Country ON S.ShippingCountryId = dbo.CMS_Country.CountryID WHERE S.ItemID = {0}", mShippingExtensionCountryID.ToString());
        DataSet ds = cn.ExecuteQuery(stringQuery, null, QueryTypeEnum.SQLQuery, false);

        if (!DataHelper.DataSourceIsEmpty(ds))
        {
            ShippingOptionDisplayName = ValidationHelper.GetString(ds.Tables[0].Rows[0]["ShippingOptionDisplayName"], string.Empty);
            ShippingCountryDisplayName = ValidationHelper.GetString(ds.Tables[0].Rows[0]["CountryDisplayName"], string.Empty);
            mShippingOptionID = ValidationHelper.GetInteger(ds.Tables[0].Rows[0]["ShippingOptionID"], -1);
        }

        string stringPrice = string.Format("select ShippingUnitPrice, ShippingUnitFrom, ShippingUnitTo  from customtable_shippingextensionpricing where ItemID={0}", mPriceID.ToString());
        DataSet dsPrice = cn.ExecuteQuery(stringPrice, null, QueryTypeEnum.SQLQuery, false);
        if (!DataHelper.DataSourceIsEmpty(dsPrice))
        {
            mShippingUnitTo = ValidationHelper.GetInteger(dsPrice.Tables[0].Rows[0]["ShippingUnitTo"], 0);
            mShippingUnitFrom = ValidationHelper.GetInteger(dsPrice.Tables[0].Rows[0]["ShippingUnitFrom"], 0);
            price = (decimal)ValidationHelper.GetDouble(dsPrice.Tables[0].Rows[0]["ShippingUnitPrice"], 0);
        }
    }
}