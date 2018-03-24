using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using System.Data;
using CMS.CMSHelper;
using CMS.DocumentEngine;
using CMS.Ecommerce;
using CMS.EcommerceProvider;
using CMS.GlobalHelper;
using CMS.Newsletter;
using CMS.SettingsProvider;
using CMS.SiteProvider;
using CMS.UIControls;
using CMS.WorkflowEngine;
using CMS.Helpers;

public partial class Servranx_Controls_addtax : System.Web.UI.UserControl
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }
    protected void Button1_Click(object sender, EventArgs e)
    {
        RemoveTaxClassFromProduct();
    }
    protected void AddTaxClassToProduct()
    {
        // Prepare the parameters
        //  string where = "SKUName LIKE '%%'";
        SKUInfo product = null;

        // Get the product
        DataSet products = SKUInfoProvider.GetSKUs(null, null);
        if (!DataHelper.DataSourceIsEmpty(products))
        {
            int nb = products.Tables[0].Rows.Count;
            for (int i = 0; i < nb; i++)
            {
                //product = new SKUInfo(products.Tables[0].Rows[0]);
                DataRow row = products.Tables[0].Rows[i];
                product = new SKUInfo(row);
                // Get the tax class
                TaxClassInfo taxClass = TaxClassInfoProvider.GetTaxClassInfo("TVA_EU21", SiteContext.CurrentSiteName);
                TaxClassInfo taxClass1 = TaxClassInfoProvider.GetTaxClassInfo("VAT", SiteContext.CurrentSiteName);

                if ((product != null) && (taxClass != null))
                {
                    // Add tax class to product
                    SKUTaxClassInfoProvider.AddTaxClassToSKU(taxClass.TaxClassID, product.SKUID);

                    SKUTaxClassInfoProvider.AddTaxClassToSKU(taxClass1.TaxClassID, product.SKUID);

                }
            }
        }


    }
    protected void RemoveTaxClassFromProduct()
    {
        // Prepare the parameters
        //  string where = "SKUName LIKE N'MyNew%'";
        SKUInfo product = null;

        // Get the tax class
        TaxClassInfo taxClass = TaxClassInfoProvider.GetTaxClassInfo("VAT", SiteContext.CurrentSiteName);
       // TaxClassInfo taxClass1 = TaxClassInfoProvider.GetTaxClassInfo("VAT", CMSContext.CurrentSiteName);

        // Get the product
        DataSet products = SKUInfoProvider.GetSKUs(null, null);
        if (!DataHelper.DataSourceIsEmpty(products))
        {
            // product = new SKUInfo(products.Tables[0].Rows[0]);
            // }
            int nb = products.Tables[0].Rows.Count;
            for (int i = 0; i < nb; i++)
            {
                DataRow row = products.Tables[0].Rows[i];
                product = new SKUInfo(row);
                if ((product != null) && (taxClass != null))
                {
                    // Get the tax class added to product
                    SKUTaxClassInfo skuTaxClass = SKUTaxClassInfoProvider.GetSKUTaxClassInfo(taxClass.TaxClassID, product.SKUID);

                    // Remove tax class from product
                    SKUTaxClassInfoProvider.DeleteSKUTaxClassInfo(skuTaxClass);


                }


                // }
            }

        }
    }
}
   