using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.Ecommerce;
using CMS.GlobalHelper;
using CMS.UIControls;
using CMS.Helpers;

[Title(ImageUrl = "CMSModules/CMS_Ecommerce/pricedetail.png", ResourceString = "ProductPriceDetail.Title")]
public partial class CMSModules_Ecommerce_Controls_ShoppingCart_ShoppingCartSKUPriceDetail : CMSEcommerceModalPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // Initialize product price detail
        InitializeSKUPriceDetailControl();

        btnClose.Text = GetString("General.Close");
        btnClose.OnClientClick = "Close(); return false;";
    }


    /// <summary>
    /// Initializes properties of the control which display current sku price detail.
    /// </summary>
    private void InitializeSKUPriceDetailControl()
    {
        // Set current SKU ID
        ucSKUPriceDetail.CartItemGuid = QueryHelper.GetGuid("itemguid", Guid.Empty);

        // Get local shopping cart session name
        string sessionName = QueryHelper.GetString("cart", String.Empty);
        if (sessionName != String.Empty)
        {
            // Get local shopping cart when in CMSDesk
            object obj = SessionHelper.GetValue(sessionName);
            if (obj != null)
            {
                ucSKUPriceDetail.ShoppingCart = (ShoppingCartInfo)obj;
            }
        }
    }
}