using System;

using CMS.Ecommerce;
using CMS.Helpers;
using CMS.Membership;
using CMS.UIControls;

public partial class CMSModules_Ecommerce_CMSPages_GetInvoice : LivePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        int orderId = QueryHelper.GetInteger("orderid", 0);
        OrderInfo oi = OrderInfoProvider.GetOrderInfo(orderId);

        if (oi != null)
        {
            CustomerInfo customer = CustomerInfoProvider.GetCustomerInfoByUserID(MembershipContext.AuthenticatedUser.UserID);

            if (((customer != null) && (oi.OrderCustomerID == customer.CustomerID)) || MembershipContext.AuthenticatedUser.IsGlobalAdministrator || (MembershipContext.AuthenticatedUser.IsAuthorizedPerResource("CMS.Ecommerce", "EcommerceRead")))
            {
                ltlInvoice.Text = URLHelper.MakeLinksAbsolute(oi.OrderInvoice);
            }
        }
    }
}