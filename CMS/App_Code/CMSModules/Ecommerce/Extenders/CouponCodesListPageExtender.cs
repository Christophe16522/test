using System;
using System.Linq;

using CMS;
using CMS.Base;
using CMS.DataEngine;
using CMS.Ecommerce;
using CMS.ExtendedControls;
using CMS.ExtendedControls.ActionsConfig;
using CMS.Helpers;
using CMS.Membership;
using CMS.SiteProvider;
using CMS.UIControls;

[assembly: RegisterCustomClass("CouponCodesListPageExtender", typeof(CouponCodesListPageExtender))]

/// <summary>
/// Discount coupon list page extender
/// </summary>
public class CouponCodesListPageExtender : PageExtender<CMSPage>
{
    public override void OnInit()
    {
        Page.Load += Page_Load;
    }


    private void Page_Load(object sender, EventArgs e)
    {
        // Find parent discount object
        if (Page.EditedObjectParent != null)
        {
            var parentDiscount = (DiscountInfo)Page.EditedObjectParent;

            // Check if user is allowed to read discount
            if (!DiscountInfoProvider.IsUserAuthorizedToReadDiscount(new SiteInfoIdentifier(parentDiscount.DiscountSiteID), MembershipContext.AuthenticatedUser))
            {
                Page.EditedObjectParent = null;
            }

            // Add action for coupon codes generation
            Page.AddHeaderAction(new HeaderAction
               {
                   Text = ResHelper.GetString("com.discount.generatecoupons"),
                   RedirectUrl = URLHelper.ResolveUrl("~/CMSModules/Ecommerce/Pages/Tools/Discount/Discount_Codes_Generator.aspx?discountId=" + parentDiscount.DiscountID),
                   Index = 1,
                   Enabled = parentDiscount.DiscountUsesCoupons && DiscountInfoProvider.IsUserAuthorizedToModifyDiscount(SiteContext.CurrentSiteName, MembershipContext.AuthenticatedUser)
               });
        }
    }
}
