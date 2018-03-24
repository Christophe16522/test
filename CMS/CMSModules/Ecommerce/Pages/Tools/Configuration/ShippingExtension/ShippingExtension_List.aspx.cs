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


//[Title("Objects/Ecommerce_ShippingOption/object.png", "COM_ShippingOption_List.HeaderCaption", "shipping_options_list")]
[Title("Objects/Ecommerce_ShippingOption/object.png", "Shipping extension", "")]
public partial class CMSModules_Ecommerce_Pages_Tools_Configuration_ShippingExtension_ShippingExtension_List : CMSShippingOptionsPage //CMSToolsPage
{
    #region "Page Events"

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        // Init Unigrid
        ShippingExtensionGrid.OnAction += new OnActionEventHandler(uniGrid_OnAction);

        ShippingExtensionGrid.ZeroRowsText = GetString("general.nodatafound");
        if (AllowGlobalObjects && ExchangeTableInfoProvider.IsExchangeRateFromGlobalMainCurrencyMissing(CMSContext.CurrentSiteID))
        {
            ShowWarning(GetString("com.NeedExchangeRateFromGlobal"), null, null);
        }
    }

    
    protected void Page_Load(object sender, EventArgs e)
    {
        ShippingExtensionGrid.OnExternalDataBound += new OnExternalDataBoundEventHandler(UniGrid_OnExternalDataBound);
        
        if (!IsPostBack)
        {
            CheckIfExtensionDeletedInOptions();
        }
        // Prepare the new shipping option header element
        hdrActions.ActionsList.Add(new HeaderAction()
        {
            //Text = GetString("COM_ShippingOption_List.NewItemCaption"),
            Text = "New shipping extension",
            //RedirectUrl = ResolveUrl("ShippingExtension_New.aspx?siteId=0" + SelectSite.SiteID),
            RedirectUrl = ResolveUrl("ShippingExtension_New.aspx"),
            ImageUrl = GetImageUrl("Objects/Ecommerce_ShippingOption/add.png"),
            ControlType = HeaderActionTypeEnum.Hyperlink
        });

        hdrActions.ActionsList.Add(new HeaderAction()
        {
            //Text = GetString("COM_ShippingOption_List.NewItemCaption"),
            Text = "Country View Summary",
            //RedirectUrl = ResolveUrl("ShippingExtension_New.aspx?siteId=0" + SelectSite.SiteID),
            RedirectUrl = ResolveUrl("ShippingExtension_CountryView.aspx"),
            ImageUrl = GetImageUrl("CMSModules/CMS_Countries/List.png"),
            ControlType = HeaderActionTypeEnum.Hyperlink
        });


        CurrentMaster.DisplaySiteSelectorPanel = AllowGlobalObjects;
        //GetAndUpdateCustomTableItem();
        GetAndUpdateCustomTableQueryItem();
    }

    private void GetAndUpdateCustomTableQueryItem()
    {
        GeneralConnection cn = ConnectionHelper.GetConnection();

        DataSet ds = cn.ExecuteQuery("customtable.shippingextension.FullName", null);

        if (!DataHelper.DataSourceIsEmpty(ds))
        {
            ShippingExtensionGrid.DataSource = ds;

        }
    }

    protected override void OnPreRender(EventArgs e)
    {
        /*bool both = (SelectSite.SiteID == UniSelector.US_GLOBAL_OR_SITE_RECORD);

        // Hide header actions if (both) selected
        hdrActions.Enabled = !both;
        lblWarnNew.Visible = both;*/

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
        if (actionName == "edit")
        {
            //URLHelper.Redirect("ShippingExtension_Edit_Frameset.aspx?shippingOptionID=" + Convert.ToString(actionArgument) + "&siteId=" + SelectSite.SiteID);
            URLHelper.Redirect("ShippingExtension_Edit_Frameset.aspx?shippingOptionID=" + Convert.ToString(actionArgument));
        }
        else if (actionName == "country")
        {
            //URLHelper.Redirect("ShippingExtension_Edit_Country.aspx?shippingExtensionID=" + Convert.ToString(actionArgument) + "&siteId=" + SelectSite.SiteID);
            URLHelper.Redirect("ShippingExtension_Edit_Country.aspx?shippingExtensionID=" + Convert.ToString(actionArgument));
        }
        else if (actionName == "delete")
        {
            DeleteExtension(actionArgument.ToString());
            Response.Redirect(Request.Url.ToString());
        }
    }


    protected object UniGrid_OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        string sImageURL = string.Empty;
        sImageURL = GetImageUrl("Objects/Ecommerce_ShippingOption/add.png");
        switch (sourceName.ToLowerCSafe())
        {
            case "country2":
                ImageButton btn = ((ImageButton)sender);
                btn.ImageUrl = sImageURL;
                return btn;
        }

        return parameter;
    }

    /*
    /// <summary>
    /// Handles the SiteSelector's selection changed event.
    /// </summary>
    protected void Selector_SelectedIndexChanged(object sender, EventArgs e)
    {
        InitWhereCondition();
        UniGrid.ReloadData();

        // Save selected value
        StoreSiteFilterValue(SelectSite.SiteID);
    }
    */
    #endregion


    #region "Methods"
    /*
    /// <summary>
    /// Creates where condition for UniGrid and reloads it.
    /// </summary>
    private void InitWhereCondition()
    {
        UniGrid.WhereCondition = SelectSite.GetSiteWhereCondition("ShippingOptionSiteID");
    }
    */
    private void DeleteExtension(string ShippingExtensionId)
    {
        GeneralConnection cn = ConnectionHelper.GetConnection();

        // Delete prices defined for this extension
        cn.ExecuteNonQuery(string.Format("DELETE FROM customtable_shippingextensionpricing WHERE ShippingExtensionCountryID IN (SELECT ItemID FROM customtable_shippingextensioncountry WHERE ShippingOptionID={0})", ShippingExtensionId), null, QueryTypeEnum.SQLQuery, false);

        // Delete countries defined for this extension
        cn.ExecuteNonQuery(string.Format("DELETE FROM customtable_shippingextensioncountry WHERE ShippingOptionID={0}", ShippingExtensionId), null, QueryTypeEnum.SQLQuery, false);

        // Delete the extension
        cn.ExecuteNonQuery(string.Format("DELETE FROM customtable_shippingextension WHERE ShippingOptionID={0}", ShippingExtensionId), null, QueryTypeEnum.SQLQuery, false);

    }

    private void CheckIfExtensionDeletedInOptions()
    {
        GeneralConnection cn = ConnectionHelper.GetConnection();
        DataSet ds = cn.ExecuteQuery("select shippingOptionId from customtable_shippingextension WHERE shippingOptionId NOT IN (SELECT ShippingOptionID FROM COM_ShippingOption)", null, QueryTypeEnum.SQLQuery, false);
        if (!DataHelper.DataSourceIsEmpty(ds))
        {
            foreach (DataRow drow in ds.Tables[0].Rows)
            {
                DeleteExtension(drow["ShippingOptionId"].ToString());
            }
        }

    }

    #endregion

}