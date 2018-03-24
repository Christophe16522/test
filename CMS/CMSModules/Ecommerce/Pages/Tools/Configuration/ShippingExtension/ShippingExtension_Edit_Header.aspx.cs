using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.Controls.Configuration;

using CMS.Ecommerce;
using CMS.CMSHelper;
using CMS.GlobalHelper;
using CMS.SiteProvider;
using CMS.UIControls;
using CMS.SettingsProvider;
using CMS.DataEngine;
using CMS.Helpers;
using CMS.Modules;


[Tabs("CMS.Ecommerce", "Configuration.ShippingExtension", "shippingExtensionContent")]
//[Title("Objects/Ecommerce_ShippingOption/object.png", "ShippingOption_EditHeader.TitleText", "newgeneral_tab2")]
[Title("Objects/Ecommerce_ShippingOption/object.png", "Shipping extension properties", "newgeneral_tab2")]
public partial class CMSModules_Ecommerce_Pages_Tools_Configuration_ShippingOptions_ShippingOption_Edit_Header : CMSShippingOptionsPage
{
    #region "Variables"

    protected int mShippingExtensionId = 0;
    protected int editedSiteId = 0;

    #endregion

    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        mShippingExtensionId = QueryHelper.GetInteger("shippingExtensionID", 0);

        string shippingExtensionName = GetShippingOptionName(mShippingExtensionId.ToString());

        // Initializes page title and breadcrumbs
        string[,] breadcrumbs = new string[2, 3];
        breadcrumbs[0, 0] = "Shipping Extension";
        breadcrumbs[0, 1] = "~/CMSModules/Ecommerce/Pages/Tools/Configuration/ShippingExtension/ShippingExtension_List.aspx";
        breadcrumbs[0, 2] = "configEdit";
        breadcrumbs[1, 0] = shippingExtensionName;
        breadcrumbs[1, 1] = "";
        breadcrumbs[1, 2] = "";
        
        CMSMasterPage master = (CMSMasterPage)CurrentMaster;
        master.Title.Breadcrumbs = breadcrumbs;
        master.Tabs.OnTabCreated += Tabs_OnTabCreated;
    }


    private TabItem Tabs_OnTabCreated(UIElementInfo element, TabItem tab, int tabIndex)
    {
        // Add SiteId parameter to each tab
        if (!string.IsNullOrEmpty(tab.RedirectUrl))
        {
            tab.RedirectUrl = URLHelper.AddParameterToUrl(tab.RedirectUrl, "siteId", SiteID.ToString());
        }

        return tab;
    }

    #endregion

    #region "Methods"

    private string GetShippingOptionName(string ShippingOptionID)
    {
        string result = string.Empty;
        GeneralConnection cn = ConnectionHelper.GetConnection();

        string stringQuery = string.Format("SELECT ShippingOptionName,ShippingOptionDisplayName from COM_ShippingOption WHERE ShippingOPtionID={0}", ShippingOptionID);
        DataSet ds = cn.ExecuteQuery(stringQuery, null, QueryTypeEnum.SQLQuery, false);

        if (!DataHelper.DataSourceIsEmpty(ds))
        {
            result = ValidationHelper.GetString (ds.Tables[0].Rows[0]["ShippingOptionDisplayName"], string.Empty);            
        }
        return result;
    }
    #endregion
}