
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
using CMS.ExtendedControls.ActionsConfig;
using CMS.Helpers;

[Title("Objects/Ecommerce_ShippingOption/object.png", "Shipping extension country list", "newgeneral_tab2")]
//[Security(Resource = "CMS.Ecommerce", UIElements = "Configuration.ShippingOptions.General")]
public partial class CMSModules_Ecommerce_Pages_Tools_Configuration_ShippingExtension_ShippingExtension_Edit_Country : CMSShippingOptionsPage 
{
    protected int mShippingExtensionID = 0;
    protected int mEditedSiteId = -1;

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        // Init Unigrid
        UniGrid.OnAction += new OnActionEventHandler(uniGrid_OnAction);
        //UniGrid.OnExternalDataBound += new OnExternalDataBoundEventHandler(UniGrid_OnExternalDataBound);
        UniGrid.ZeroRowsText = GetString("general.nodatafound");
    }


    protected void Page_Load(object sender, EventArgs e)
    {

        mShippingExtensionID = QueryHelper.GetInteger("shippingExtensionID", 0);
        hdrActions.ActionsList.Add(new HeaderAction()
        {
            //Text = GetString("COM_ShippingOption_List.NewItemCaption"),
            Text = "Add a country",
            RedirectUrl = ResolveUrl("ShippingExtension_AddCountry.aspx?shippingExtensionID=" + mShippingExtensionID),
            ImageUrl = GetImageUrl("Objects/CMS_Country/add.png"),
            ControlType = HeaderActionTypeEnum.LinkButton
        });
        GetAndUpdateCustomTableQueryItem();
        // Initializes page title and breadcrumbs
        string shippingExtensionName = GetShippingOptionName(mShippingExtensionID.ToString());
        string[,] breadcrumbs = new string[2, 3];
        breadcrumbs[0, 0] = "Shipping Extension";
        breadcrumbs[0, 1] = "~/CMSModules/Ecommerce/Pages/Tools/Configuration/ShippingExtension/ShippingExtension_List.aspx";
        breadcrumbs[0, 2] = "configEdit";
        breadcrumbs[1, 0] = shippingExtensionName;
        breadcrumbs[1, 1] = "";
        breadcrumbs[1, 2] = "";

        CMSMasterPage master = (CMSMasterPage)CurrentMaster;
        master.Title.Breadcrumbs = breadcrumbs;

    }

    protected void uniGrid_OnAction(string actionName, object actionArgument)
    {
        if (actionName == "edit")
        {
            URLHelper.Redirect("ShippingExtension_Edit_Pricing.aspx?ItemID=" + Convert.ToString(actionArgument));
        }
        if (actionName == "delete")
        {
            // actionArgument is ItemID here
            DeleteCountry(actionArgument.ToString());
            Response.Redirect(Request.Url.ToString());
        }
    }

    /// <summary>
    /// Sets data to database.
    /// </summary>
    protected void btnOK_Click(object sender, EventArgs e)
    {
        /*mShippingExtensionID = QueryHelper.GetInteger("shippingExtensionID", 0);
        GeneralConnection cn = ConnectionHelper.GetConnection();
        string stringCommand = string.Format("UPDATE customtable_shippingextension SET Localcontact='{0}',Enabled={1} WHERE itemID={2}", txtLocalContact.Text, Convert.ToInt32(chkShippingExtensionEnabled.Checked), mShippingExtensionID.ToString());
        cn.ExecuteNonQuery(stringCommand, null,QueryTypeEnum.SQLQuery ,false);
        ShowChangesSaved();*/
    }

    private void GetAndUpdateCustomTableQueryItem()
    {
        GeneralConnection cn = ConnectionHelper.GetConnection();

        DataSet ds = cn.ExecuteQuery(GetQuery(), null, QueryTypeEnum.SQLQuery, false);
        if (!DataHelper.DataSourceIsEmpty(ds))
        {
            UniGrid.DataSource = ds;
        }
    }

    private string GetQuery()
    {
        string query = @"SELECT     dbo.customtable_shippingextensioncountry.ItemID, dbo.customtable_shippingextensioncountry.ShippingBase, 
                      dbo.customtable_shippingextensioncountry.ShippingCountryId, dbo.customtable_shippingextensioncountry.ItemID AS Expr1, 
                      dbo.customtable_shippingextensioncountry.ShippingOptionId, dbo.CMS_Country.CountryDisplayName, dbo.CMS_Country.CountryName, 
                      dbo.CMS_Country.CountryTwoLetterCode, dbo.CMS_Country.CountryThreeLetterCode, dbo.customtable_shippingextensioncountry.LocalContact, 
                      dbo.customtable_shippingextensioncountry.Enabled, Min, Max, Ranges, dbo.customtable_shippingextensioncountry.UnitPrice,
                      dbo.customtable_shippingextensioncountry.ProcessingMode, CASE WHEN dbo.customtable_shippingextensioncountry.ProcessingMode = 0 THEN 'By Range' ELSE 'By Unit' END AS Processing
                FROM         dbo.customtable_shippingextensioncountry INNER JOIN
                      dbo.CMS_Country ON dbo.customtable_shippingextensioncountry.ShippingCountryId = dbo.CMS_Country.CountryID LEFT OUTER JOIN
                      
                      (SELECT     TOP (100) PERCENT ShippingExtensionCountryId, MIN(ShippingUnitFrom) AS Min, MAX(ShippingUnitTo) AS Max, COUNT(ShippingExtensionCountryId) 
                      AS Ranges
                FROM        dbo.customtable_shippingextensionpricing
                            GROUP BY ShippingExtensionCountryId
                            HAVING      (MAX(ShippingUnitTo) <> - 1)
                            ORDER BY ShippingExtensionCountryId
                            ) B
                      ON dbo.customtable_shippingextensioncountry.ItemID = B.ShippingExtensionCountryId
                WHERE     (dbo.customtable_shippingextensioncountry.ShippingOptionId = {0})";
        string result = string.Format(query, mShippingExtensionID.ToString());

        return result;
    }

    private void DeleteCountry(string ItemID)
    {
        GeneralConnection cn = ConnectionHelper.GetConnection();
        cn.ExecuteNonQuery(string.Format("DELETE FROM  dbo.customtable_shippingextensionPricing WHERE ShippingExtensionCountryID={0}", ItemID), null, QueryTypeEnum.SQLQuery, false);
        cn.ExecuteNonQuery(string.Format("DELETE FROM  dbo.customtable_shippingextensioncountry WHERE ItemID={0}", ItemID), null, QueryTypeEnum.SQLQuery, false); 
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
    #endregion

}