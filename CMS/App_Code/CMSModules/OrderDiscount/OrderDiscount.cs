using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using CMS.Ecommerce;

/// <summary>
/// Sample shopping cart info provider. 
/// Can be registered either by replacing the ShoppingCartInfoProvider.ProviderObject (uncomment the line in SampleECommerceModule.cs) or through cms.extensibility section of the web.config
/// </summary>
public class OrderDiscount : ShoppingCartInfoProvider
{
    /// <summary>
    /// Calculates discount which should be applied to the total items price.
    /// </summary>
    /// <param name="cart">Shopping cart</param>        
    protected override double CalculateOrderDiscountInternal(ShoppingCartInfo cart)
    {
        double result = base.CalculateOrderDiscountInternal(cart);
        if (cart.DiscountCoupon != null)
        {
            if (cart.DiscountCoupon.GetBooleanValue("DiscountCouponIsForOrder", false))
            {
                if (cart.DiscountCoupon.DiscountCouponIsFlatValue)
                {
                    result = result + cart.DiscountCoupon.DiscountCouponValue;
                }
                else
                {
                    result = result + cart.TotalItemsPriceInMainCurrency * (cart.DiscountCoupon.DiscountCouponValue / 100.0);
                }
            }
            if (result > cart.TotalItemsPriceInMainCurrency)
            {
                result = cart.TotalItemsPriceInMainCurrency;
            }
        }
        return result;
    }
}