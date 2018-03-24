
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

[Title("Objects/Ecommerce_ShippingOption/object.png", "Edit price", "newgeneral_tab2")]
public partial class CMSModules_Ecommerce_Pages_Tools_Configuration_ShippingExtension_ShippingExtension_EditPrice : CMSShippingOptionsPage
{
    protected int mShippingExtensionPricingID = QueryHelper.GetInteger("ItemID", 0);
    protected int mPriceID = QueryHelper.GetInteger("PriceID", 0);
    protected int mShippingOptionID = -1, mShippingUnitFrom = -1, mShippingUnitTo = -1;
    protected string ShippingOptionDisplayName, ShippingCountryDisplayName;
    protected decimal price = 0;


    protected void Page_Load(object sender, EventArgs e)
    {

        GetShippingOptionName(mShippingExtensionPricingID.ToString());
        string[,] breadcrumbs = new string[4, 3];
        breadcrumbs[0, 0] = "Shipping Extension";
        breadcrumbs[0, 1] = "~/CMSModules/Ecommerce/Pages/Tools/Configuration/ShippingExtension/ShippingExtension_List.aspx";
        breadcrumbs[0, 2] = "configEdit";
        breadcrumbs[1, 0] = ShippingOptionDisplayName;
        breadcrumbs[1, 1] = string.Format("~/CMSModules/Ecommerce/Pages/Tools/Configuration/ShippingExtension/ShippingExtension_Edit_Country.aspx?shippingExtensionID={0}", mShippingOptionID.ToString());
        breadcrumbs[1, 2] = "configEdit";
        breadcrumbs[2, 0] = ShippingCountryDisplayName;
        breadcrumbs[2, 1] = string.Format("~/CMSModules/Ecommerce/Pages/Tools/Configuration/ShippingExtension/ShippingExtension_Edit_Pricing.aspx?ItemID={0}", mShippingExtensionPricingID.ToString());
        breadcrumbs[2, 2] = "configEdit";
        breadcrumbs[3, 0] = "Edit price";
        breadcrumbs[3, 1] = "";
        breadcrumbs[3, 2] = "";


        CMSMasterPage master = (CMSMasterPage)CurrentMaster;
        master.Title.Breadcrumbs = breadcrumbs;

        txtShippingUnitTo.Text = mShippingUnitTo.ToString();
        txtShippingUnitFrom.Text = mShippingUnitFrom.ToString();
        if (!IsPostBack)
        {
            txtPrice.Text = price.ToString();
        }
    }

    /// <summary>
    /// Sets data to database.
    /// </summary>
    protected void btnOK_Click(object sender, EventArgs e)
    {
        // VALIDATION RULES   
        int shippingUnitTo = -1, shippingUnitFrom = Convert.ToInt32(txtShippingUnitFrom.Text);

        if (!string.IsNullOrEmpty(txtShippingUnitTo.Text))
        {
            txtShippingUnitTo.Text = txtShippingUnitTo.Text.Trim();

            // Ensures integer value entered in txtShippingUnitTo
            try
            {
                shippingUnitTo = Convert.ToInt32(txtShippingUnitTo.Text);
            }
            catch
            {
                ShowError(string.Format("<b>{0}</b> is not valid for <u><b>Shipping unit to</b></u>", txtShippingUnitTo.Text));
                return;
            }

            // Ensures txtShippingUnitTo > txtShippingUnitFrom
            if (shippingUnitTo <= shippingUnitFrom && shippingUnitTo != -1)
            {
                ShowError(string.Format("<u><b>Shipping unit to</b></u> must be greater than {0}", txtShippingUnitFrom.Text));
                return;
            }

            // Ensures price exists
            if (string.IsNullOrEmpty(txtPrice.Text.Trim()))
            {
                ShowError("Price must be entered");
                return;
            }
        }

        // Ensures decimal value entered in txtPrice
        try
        {
            price = Convert.ToDecimal(txtPrice.Text);
        }
        catch
        {
            ShowError(string.Format("<u><b>Price</b></u> is not valid", txtPrice.Text));
            return;
        }

        // Saving changes in DB
        Exception ex= SaveChange();
        if (ex == null)
        {
            // Changes Saved
            ShowChangesSaved();
            Response.Redirect(string.Format("ShippingExtension_Edit_Pricing.aspx?ItemID={0}", mShippingExtensionPricingID.ToString()));
        }
        else
        {
            // Unexpected error happened
            ShowError(ex.Message);
        }

    }

    private Exception SaveChange()
    {
        Exception ex = null;
        try
        {
            GeneralConnection cn = ConnectionHelper.GetConnection();
            string stringCommand = string.Format("UPDATE customtable_shippingextensionpricing SET ShippingUnitPrice='{0}'WHERE ItemID={1}", price.ToString(), mPriceID);
            cn.ExecuteNonQuery(stringCommand, null, QueryTypeEnum.SQLQuery, false);
        }
        catch (Exception e)
        {
            ex = e;
        }
        return ex;
    }

    private void GetShippingOptionName(string ShippingOptionID)
    {
        GeneralConnection cn = ConnectionHelper.GetConnection();
        string stringQuery = string.Format("SELECT S.ItemID, C.ShippingOptionID, S.ShippingCountryId, C.ShippingOptionDisplayName, dbo.CMS_Country.CountryDisplayName FROM dbo.customtable_shippingextensioncountry AS S INNER JOIN dbo.COM_ShippingOption AS C ON C.ShippingOptionID = S.ShippingOptionId INNER JOIN dbo.CMS_Country ON S.ShippingCountryId = dbo.CMS_Country.CountryID WHERE S.ItemID = {0}", mShippingExtensionPricingID.ToString());
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