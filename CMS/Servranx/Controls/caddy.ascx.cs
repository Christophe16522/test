using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CMS.CMSHelper;
using CMS.GlobalHelper;
using CMS.Ecommerce;
using CMS.EcommerceProvider;
using CMS.URLRewritingEngine;
using CMS.SiteProvider;
using CMS.UIControls;
using CMS.ExtendedControls;
using System.Text;
using System.Data;
using System.Data.Linq;
using CMS.Helpers;

public partial class Servranx_Controls_caddy : System.Web.UI.UserControl
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }
      protected override void OnPreRender(EventArgs e)
    {

        BindData();
        base.OnPreRender(e);
       

    }
    protected string GetFormattedValue(object value)
    {
        double price = ValidationHelper.GetDouble(value, 0);
        return CurrencyInfoProvider.GetFormattedValue(price, ECommerceContext.CurrentShoppingCart.Currency);
    }


    private void BindData()
    {


        if (ECommerceContext.CurrentShoppingCart != null)
        {

            // ECommerceContext.CurrentShoppingCart.Update();
            var panier = ECommerceContext.CurrentShoppingCart;
            if (panier != null)
            {

                List<ProductItem> items = new List<ProductItem>();
                foreach (DataRow row in panier.ContentTable.Rows)
                {
                    var item = new ProductItem
                    {
                        ID = (int)row["SKUID"],
                        Nom = (string)row["SKUName"],
                     
                      
                        Qte = int.Parse(row["Units"].ToString()),
                       
                    };
                    // item.SetPrixTotal();
                    SKUInfo sku = SKUInfoProvider.GetSKUInfo(item.ID);
                    var type = sku.GetValue("SKUType");

                    if (type == null)
                    {
                        item.Qte = item.Qte;

                    }
                    else
                        if (type != null && !String.IsNullOrEmpty(type.ToString()) && Convert.ToInt32(type) == 1)
                        {
                            item.Qte = item.Qte;

                        }


                    items.Add(item);
                }




                             List<NombreItem> data = new List<NombreItem>();
                data.Add(new NombreItem
                {
                    NumberOfItem = ECommerceContext.CurrentShoppingCart.CartItems.Count
                });

                rptNombre.DataSource = data;
                rptNombre.DataBind();

            }
        }
    }
    


  
}

public class ItemTotalData
{
    public double Total { get; set; }
}

public class NombreItem
{
    public int NumberOfItem { get; set; }
}
