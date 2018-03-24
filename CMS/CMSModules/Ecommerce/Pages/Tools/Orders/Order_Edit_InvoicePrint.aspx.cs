using System;

using CMS.Core;
using CMS.Ecommerce;
using CMS.Helpers;
using CMS.UIControls;

[UIElement(ModuleName.ECOMMERCE, "Orders.Invoice")]
public partial class CMSModules_Ecommerce_Pages_Tools_Orders_Order_Edit_InvoicePrint : CMSEcommercePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        int orderId = QueryHelper.GetInteger("orderid", 0);
        OrderInfo order = OrderInfoProvider.GetOrderInfo(orderId);
        if (order != null)
        {
            // Check if order is not edited from another site
            CheckEditedObjectSiteID(order.OrderSiteID);

            lblInvoice.Text = URLHelper.MakeLinksAbsolute(order.OrderInvoice);
        }
    }
}