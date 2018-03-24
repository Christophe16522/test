
using System;
using System.Data;
using System.Collections;
using System.Collections.Generic;
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
using CMS.ExtendedControls.ActionsConfig;
using CMS.Helpers;
using CMS.Globalization;

//[Title("Objects/Ecommerce_ShippingOption/object.png", "Shipping extension edit processing", "newgeneral_tab2")]
//[Security(Resource = "CMS.Ecommerce", UIElements = "Configuration.ShippingOptions.General")]
public partial class CMSModules_Ecommerce_Pages_Tools_Configuration_ShippingExtension_ShippingExtension_Edit_UnitPriceUpdate : CMSShippingOptionsPage
{
    private static List<int> iCountry = new List<int>();
    protected static int mShippingExtensionID = 0;
    protected int mEditedSiteId = -1;
    private static DataSet dsAvailable = null;
    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
        // Init Unigrid
        UniGridUpdated.OnAction += new OnActionEventHandler(uniGrid_OnAction);
        UniGridAvailable.OnAction += new OnActionEventHandler(uniGrid_OnAction);
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        int newID = QueryHelper.GetInteger("shippingExtensionID", 0);
        if (newID != mShippingExtensionID)
        {
            iCountry.Clear();
        }
        mShippingExtensionID = newID;

        //mShippingExtensionID = QueryHelper.GetInteger("shippingExtensionID", 0);
        //GetAndUpdateCustomTableQueryItem();
        DisplayUpdatedCountries();
        CMSMasterPage master = (CMSMasterPage)CurrentMaster;
    }

    protected void uniGrid_OnAction(string actionName, object actionArgument)
    {
        if (actionName.ToLower() == "add")
        {
            AddCountry(actionArgument.ToString());
        }
        if (actionName.ToLower() == "delete")
        {
            // actionArgument is ItemID here
            DeleteCountry(actionArgument.ToString());
            //Response.Redirect(Request.Url.ToString());
        }
    }

    /// <summary>
    /// Sets data to database.
    /// </summary>
    protected void btnOK_Click(object sender, EventArgs e)
    {
        decimal qty = 0;
        string error = string.Empty;
        if (UniGridUpdated.RowsCount == 0)
        {
            error = "The list of country to update is empty";
        }
        if (string.IsNullOrEmpty(error))
        {
            if (rdoAmount.Checked)
            {
                try
                {
                    qty = decimal.Parse(txtAmount.Text);
                }
                catch
                {
                    error = "Error: invalid amount";
                }
            }
            else
            {
                try
                {
                    qty = decimal.Parse(txtPercent.Text);
                }
                catch
                {
                    error = "Error: invalid percentage";
                }
            }
        }
        if (!string.IsNullOrEmpty(error))
        {
            ShowError(error);
        }
        else
        {
            if (UpdatePrice(rdoUnitPrice.Checked, rdoAmount.Checked, qty))
            {
                txtAmount.Text = string.Empty;
                txtPercent.Text = string.Empty; 
                iCountry.Clear();
                DisplayUpdatedCountries();
                UniGridUpdated.ReBind();
                ShowChangesSaved();
            }
            else
            {
                ShowWarning("Some error occured", "Update not performed", string.Empty);
            }
        }
    }

    private void DisplayUpdatedCountries()
    {
        GeneralConnection cn = ConnectionHelper.GetConnection();
        string query = getSQL();
        DataSet ds = cn.ExecuteQuery(query, null, QueryTypeEnum.SQLQuery, false);
        ds.Tables[0].PrimaryKey = new DataColumn[1] { ds.Tables[0].Columns["CountryID"] };
        DataTable table = ds.Tables[0];
        table.Rows.Clear();

        GetAndUpdateCustomTableQueryItem();
        //DataSet dsAvailable = (DataSet)UniGridAvailable.DataSource;
        //dsAvailable.Tables[0].PrimaryKey = new DataColumn[1] { dsAvailable.Tables[0].Columns["CountryID"] };

        foreach (int i in iCountry)
        {
            DataRow drow = table.NewRow();
            DataRow findrow = dsAvailable.Tables[0].Rows.Find(i.ToString());
            if (findrow != null)
            {
                drow["CountryId"] = findrow["CountryID"];
                drow["CountryDisplayName"] = findrow["CountryDisplayName"];
                drow["UnitPrice"] = findrow["UnitPrice"];
                drow["ShippingBase"] = findrow["ShippingBase"];

                table.Rows.Add(drow);
            }
            DataRow d = dsAvailable.Tables[0].Rows.Find(i.ToString());
            if (d != null)
            {
                dsAvailable.Tables[0].Rows.Remove(d);
            }
        }

        UniGridUpdated.DataSource = ds;
        UniGridUpdated.DataBind();
        UniGridAvailable.ReBind();
        SetConfirmText(null, null);
    }

    private void GetAndUpdateCustomTableQueryItem()
    {
        
        // string query = string.Format("SELECT CountryID, CountryDisplayName, CountryName, CountryTwoLetterCode, CountryThreeLetterCode FROM dbo.CMS_Country WHERE CountryID IN (SELECT ShippingCountryID FROM customtable_shippingextensioncountry WHERE ShippingOptionID={0})", mShippingExtensionID.ToString());
        /*string query = string.Format(@"SELECT dbo.CMS_Country.CountryID, dbo.CMS_Country.CountryDisplayName, dbo.CMS_Country.CountryName, dbo.CMS_Country.CountryTwoLetterCode, dbo.CMS_Country.CountryThreeLetterCode, dbo.customtable_shippingextensioncountry.ShippingOptionId, dbo.customtable_shippingextensioncountry.UnitPrice, dbo.customtable_shippingextensioncountry.ShippingBase 
                                       FROM dbo.CMS_Country INNER JOIN dbo.customtable_shippingextensioncountry ON dbo.CMS_Country.CountryID = dbo.customtable_shippingextensioncountry.ShippingCountryId
                                       WHERE (dbo.customtable_shippingextensioncountry.ShippingOptionId = {0})", mShippingExtensionID.ToString());*/

        string query = getSQL();
        GeneralConnection cn = ConnectionHelper.GetConnection();
        dsAvailable = cn.ExecuteQuery(query, null, QueryTypeEnum.SQLQuery, false);
        dsAvailable.Tables[0].PrimaryKey = new DataColumn[1] { dsAvailable.Tables[0].Columns["CountryID"] };
        if (!DataHelper.DataSourceIsEmpty(dsAvailable))
        {
            UniGridAvailable.ObjectType = null;
            UniGridAvailable.DataSource = dsAvailable;
            UniGridAvailable.DataBind();
        }
    }

    private string getSQL()
    {
        string result = string.Empty;
        result = string.Format(@"SELECT dbo.CMS_Country.CountryID, dbo.CMS_Country.CountryDisplayName, dbo.CMS_Country.CountryName, dbo.CMS_Country.CountryTwoLetterCode, dbo.CMS_Country.CountryThreeLetterCode, dbo.customtable_shippingextensioncountry.ShippingOptionId, dbo.customtable_shippingextensioncountry.UnitPrice, dbo.customtable_shippingextensioncountry.ShippingBase 
                                       FROM dbo.CMS_Country INNER JOIN dbo.customtable_shippingextensioncountry ON dbo.CMS_Country.CountryID = dbo.customtable_shippingextensioncountry.ShippingCountryId
                                       WHERE (dbo.customtable_shippingextensioncountry.ShippingOptionId = {0})", mShippingExtensionID.ToString());
        return result;
    }

    private void AddCountry(string ItemID)
    {
        int CountryID = int.Parse(ItemID);
        foreach (int i in iCountry)
        {
            if (i == CountryID)
            {
                return;
            }
        }
        iCountry.Add(CountryID);
        DisplayUpdatedCountries();
    }

    private void DeleteCountry(string ItemID)
    {
        int CountryID = int.Parse(ItemID);
        foreach (int i in iCountry)
        {
            if (i == CountryID)
            {
                iCountry.Remove(i);
                break;
            }
        }
        DisplayUpdatedCountries();
    }

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

    private bool UpdatePrice(bool isUnitPrice, bool isAmount, decimal quantity)
    {
        bool result = false;
        string field = isUnitPrice ? "Unitprice" : "ShippingBase";
        string operand = isAmount ? string.Format ("{0}={0}+{1}", field, quantity.ToString()) : string.Format("{0}={0}*(1+({1}/100))", field, quantity.ToString()) ;
        using (GeneralConnection cn = ConnectionHelper.GetConnection())
        {
            foreach (int i in iCountry)
            {
                string cmd = string.Format("UPDATE customtable_shippingextensioncountry SET {0} WHERE ShippingCountryID={1} AND ShippingOptionID={2}", operand, i.ToString(),mShippingExtensionID.ToString());
                result = (cn.ExecuteNonQuery(cmd, null, QueryTypeEnum.SQLQuery, false) > 0);
            }
        }
        return result;
    }

    protected void SetConfirmText(object sender, EventArgs e)
    {
        string country = string.Empty, confirm = string.Empty;
        foreach (int i in iCountry)
        {
            country = string.Concat(country,"- ", CountryInfoProvider.GetCountryInfo(i).CountryDisplayName, Environment.NewLine);
        }
        confirm = string.Format("Confirm update for: {0}{1}", Environment.NewLine, country);
        cbeOK.ConfirmText = confirm;
        
    }
    #endregion

}