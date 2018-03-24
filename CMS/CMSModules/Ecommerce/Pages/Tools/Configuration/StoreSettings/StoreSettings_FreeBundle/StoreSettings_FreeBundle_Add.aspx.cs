using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.DataEngine;
using CMS.CMSHelper;
using CMS.Ecommerce;
using CMS.EcommerceProvider;
using CMS.GlobalHelper;
using CMS.SettingsProvider;
using CMS.SiteProvider;
using CMS.UIControls;
using CMS.ExtendedControls;
using CMS.CustomTables;
using CMS.Membership;

public partial class CMSModules_Ecommerce_Pages_Tools_Configuration_StoreSettings_StoreSettings_FreeBundle_StoreSettings_FreeBundle_Add : CMSEcommerceStoreSettingsPage
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }

    protected void btnOK_Click(object sender, EventArgs e)
    {
        string Description = txtDescription.Text.Trim();
        int Quantity = 0;
        if (!string.IsNullOrEmpty(txtQuantity.Text.Trim()))
        {
            Quantity = int.Parse(txtQuantity.Text);
        }
        if (Quantity > 0)
        {
            if (CreateCustomTableItem(Description, Quantity))
            {
                txtQuantity.Text = string.Empty;
                txtDescription.Text = string.Empty;
                ShowChangesSaved();
            }
            else
            {
                ShowError("An error occured");
            }
        }
        else
        {
            ShowError("The quantity must exists");
        }

    }

    private bool CreateCustomTableItem(string Desc, int Qty)
    {
        // Creates new Custom table item provider
        CustomTableItemProvider customTableProvider = new CustomTableItemProvider(MembershipContext.AuthenticatedUser);

        // Prepares the parameters
        string customTableClassName = "customtable.customBundle";

        // Checks if Custom table 'Sample table' exists
        DataClassInfo customTable = DataClassInfoProvider.GetDataClass(customTableClassName);
        if (customTable != null)
        {
            // Creates new custom table item
            CustomTableItem newCustomTableItem = CustomTableItem.New(customTableClassName, customTableProvider);

            // Sets the ItemText field value
            newCustomTableItem.SetValue("Quantity", Qty);
            newCustomTableItem.SetValue("Description", Desc);
            newCustomTableItem.SetValue("Enabled", true);
            // Inserts the custom table item into database
            newCustomTableItem.Insert();

            return true;
        }

        return false;

    }
}