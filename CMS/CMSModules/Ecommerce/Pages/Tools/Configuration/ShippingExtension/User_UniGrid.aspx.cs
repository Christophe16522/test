using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.SiteProvider;
using CMS.CMSHelper;
using CMS.GlobalHelper; 
using CMS.SettingsProvider;
using CMS.CustomTables;
using CMS.Membership;
using CMS.DataEngine;
using CMS.Helpers;

public partial class CMSModules_Ecommerce_Pages_Tools_Configuration_ShippingExtension_User_UniGrid : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        GetAndUpdateCustomTableItem();
    }

    private bool GetAndUpdateCustomTableItem()
    {
        // Create new Custom table item provider
        CustomTableItemProvider customTableProvider = new CustomTableItemProvider(MembershipContext.AuthenticatedUser);

        string customTableClassName = "customtable.shippingextension";

        // Check if Custom table 'Sample table' exists
        DataClassInfo customTable = DataClassInfoProvider.GetDataClass(customTableClassName);
        if (customTable != null)
        {
            DataSet dataSet = customTableProvider.GetItems(customTableClassName, null, null);
            if (!DataHelper.DataSourceIsEmpty(dataSet))
            {
                UniGrid2.DataSource = dataSet;
            }
        }
        return false;
    }
}