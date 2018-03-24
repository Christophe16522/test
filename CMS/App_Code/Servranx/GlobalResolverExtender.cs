using System;
using System.Data;
//using System.IdentityModel.Policy;
using CMS.SettingsProvider;
using CMS.GlobalHelper;
using CMS.CMSHelper;
using CMS.Ecommerce;
using CMS.EventLog;
using CMS.SiteProvider;
using CMS.Base;
using CMS.MacroEngine;
using CMS.Membership;
using CMS.DataEngine;
using CMS.Helpers;

[GlobalResolverExtender]
public partial class CMSModuleLoader : CMSModuleLoaderBase
{
  /// <summary>
  /// Module registration
  /// </summary>
  private class GlobalResolverExtenderAttribute : CMSLoaderAttribute
   {
       /// <summary>
       /// Called automatically when the application starts
       /// </summary>
       public override void Init()
       {
          //A Revoir
           // CMS.MacroEngine.MacroResolver.SetNamedSourceDataCallback("MyAge", MyAgeEvaluator,false); 
           //CMS.MacroEngine.MacroResolver.SetNamedSourceDataCallback("GetTotalPrice", GetTotalShoppingCart,false);
           //CMS.MacroEngine.MacroResolver.SetNamedSourceDataCallback("GetDiscountValue", GetDiscountValueShoppingCart,false);  
       }
      
       private object MyAgeEvaluator(MacroResolver resolver)
       {
           System.DateTime dateOfBirth = MembershipContext.AuthenticatedUser.UserSettings.UserDateOfBirth;
           System.DateTime now = DateTime.Now;

           int age = now.Year - dateOfBirth.Year;
           if (dateOfBirth > now.AddYears(-age))
           {
               age--;
           }

           CMS.MacroEngine.MacroResolver res = MacroContext.CurrentResolver.CreateContextChild();
           string discountCouponID = res.ResolveMacros("The current user is: {% CurrentDocument %}");

           string resolvedText = "";
 
         
 
        
 
          // Use the resolver to resolve macros in text
           resolvedText = res.ResolveMacros("Example: {% MyProperty %}");
           return age +"ddd "+discountCouponID;


       }

       private DiscountCouponInfo GetDiscountByOrderId(int orderID)
        {
            var ev = new EventLogProvider();
            try
            {
                DiscountCouponInfo discountCouponInfo = new DiscountCouponInfo();
                string sql = "SELECT [DiscountCouponID],[DiscountCouponValue],[DiscountCouponIsFlatValue],[DiscountCouponValidTo] FROM [COM_DiscountCoupon] " +
                             "INNER JOIN [COM_Order] ON [COM_Order].[OrderDiscountCouponID] = [COM_DiscountCoupon].[DiscountCouponID] " +
                             "WHERE [COM_Order].[OrderID] = @orderID";
                var param = new QueryDataParameters();
                param.Add(new DataParameter("@orderID", orderID));
                DataSet ds = ConnectionHelper.ExecuteQuery(sql, param, QueryTypeEnum.SQLQuery);
                if (!DataHelper.DataSourceIsEmpty(ds))
                {
                    foreach (DataRow reader in ds.Tables[0].Rows)
                    {
                        discountCouponInfo.DiscountCouponID = ValidationHelper.GetInteger(reader["DiscountCouponID"], 0);
                        discountCouponInfo.DiscountCouponIsFlatValue = ValidationHelper.GetBoolean(reader["DiscountCouponIsFlatValue"], false);
                        discountCouponInfo.DiscountCouponValue = ValidationHelper.GetDouble(reader["DiscountCouponValue"],0);
                        discountCouponInfo.DiscountCouponValidTo = ValidationHelper.GetDateTime(reader["DiscountCouponValidTo"], DateTime.MinValue);
                        return discountCouponInfo;
                    }
                }
                return discountCouponInfo;
            }
            catch (Exception ex)
            {
                ev.LogEvent("E", DateTime.Now, "CMSModuleLoader.GetDiscountValueById", ex.Message);
            }
            return null;
        }


      /// <summary>
      /// Returns total price of products in current shopping cart.
      /// </summary>
      private object GetTotalShoppingCart(MacroResolver resolver)
      {
          string currentOrderId = QueryHelper.GetInteger("orderID", 0).ToString();
          double totalpriceht = 0;

          CMS.Ecommerce.ShoppingCartInfo totalPriceHtShoppingCartInfo =
              resolver.SourceObject as CMS.Ecommerce.ShoppingCartInfo;
          CMS.Ecommerce.CurrencyInfo currentTotalPriceHtCurrency = null;

          DiscountCouponInfo currentDiscountCouponInfo = GetDiscountByOrderId(int.Parse(currentOrderId));
          if (totalPriceHtShoppingCartInfo != null)
          {

              currentTotalPriceHtCurrency = totalPriceHtShoppingCartInfo.Currency;
              totalpriceht = Convert.ToDouble(totalPriceHtShoppingCartInfo.TotalItemsPriceInMainCurrency);
             
              if (currentDiscountCouponInfo != null && currentDiscountCouponInfo.DiscountCouponValidTo < DateTime.Now)
              {
                  totalpriceht = totalPriceHtShoppingCartInfo.TotalPrice;

              }
          }
          return CMS.Ecommerce.CurrencyInfoProvider.GetFormattedValue(totalpriceht, currentTotalPriceHtCurrency).ToString();

      }

      /// <summary>
      /// Returns DiscountCoupnValue of products in current shopping cart.
      /// </summary>
      private object GetDiscountValueShoppingCart(MacroResolver resolver)
      {
         
          var currentOrderId = resolver.ResolveMacros("{% Order.OrderID %}");
        
          double discountCoupon = 0;

          CMS.Ecommerce.ShoppingCartInfo shoppingCartInfo =
              resolver.SourceObject as CMS.Ecommerce.ShoppingCartInfo;
          CMS.Ecommerce.CurrencyInfo currentCurrency = null;

          DiscountCouponInfo currentDiscountCouponInfo = GetDiscountByOrderId(int.Parse(currentOrderId));
          if (shoppingCartInfo != null)
          {
              currentCurrency = shoppingCartInfo.Currency;
              discountCoupon = currentDiscountCouponInfo.DiscountCouponValue;
              if (currentDiscountCouponInfo.DiscountCouponIsFlatValue)
                  return discountCoupon + " €";

              return discountCoupon + " %";
          }
          return string.Empty;

      }
   }
}