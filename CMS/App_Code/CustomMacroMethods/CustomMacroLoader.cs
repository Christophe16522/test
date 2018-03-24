using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using CMS.Ecommerce;
using CMS.FormEngine;
using CMS.SettingsProvider;
using CMS.GlobalHelper;
using System.Data;
using CMS.CMSHelper;
using CMS.DataEngine;
using CMS.EventLog;
using CMS.Base;
using CMS.MacroEngine;
using CMS.Helpers;

[CustomMacroLoader]
public partial class CMSModuleLoader : CMSModuleLoaderBase //: CMS.ExtendedControls.AbstractUserControl 
{


    /// <summary>
    /// Attribute class ensuring the registration of macro handlers.
    /// </summary>
    private class CustomMacroLoader : CMSLoaderAttribute
    {

        /// <summary>
        /// Called automatically when the application starts.
        /// </summary>
        public override void Init()
        {
            // Assigns a custom macro resolving handler.
            MacroResolver.OnResolveCustomMacro += MacroResolver_OnResolveCustomMacro;
           
          
        }

        private string GetDiscountValueById(int orderID)
        {
            var ev = new EventLogProvider();
            try
            {
                string discountCouponValue = string.Empty;
                string sql = "SELECT [DiscountCouponID],[DiscountCouponValue] FROM [COM_DiscountCoupon] " +
                             "INNER JOIN [COM_Order] ON [COM_Order].[OrderDiscountCouponID] = [COM_DiscountCoupon].[DiscountCouponID] " +
                             "WHERE [COM_Order].[OrderID] = @orderID";
                var param = new QueryDataParameters();
                param.Add(new DataParameter("@orderID", orderID));
                DataSet ds = ConnectionHelper.ExecuteQuery(sql, param, QueryTypeEnum.SQLQuery);
                if (!DataHelper.DataSourceIsEmpty(ds))
                {
                    foreach (DataRow reader in ds.Tables[0].Rows)
                    {
                        discountCouponValue = ValidationHelper.GetString(reader["DiscountCouponValue"], string.Empty);
                        return discountCouponValue;
                    }
                }
                return discountCouponValue;
            }
            catch (Exception ex)
            {
                CMS.EventLog.EventLogProvider.LogEvent("E", "", "", ex.Message, "CMSModuleLoader.GetDiscountValueById", 0, "", 0, "", "", 0, "", "", "", DateTime.Now);
               // ev.LogEvent("E", DateTime.Now, "CMSModuleLoader.GetDiscountValueById", ex.Message);
            }
            return null;
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
                        discountCouponInfo.DiscountCouponValue = ValidationHelper.GetDouble(reader["DiscountCouponValue"], 0);
                        discountCouponInfo.DiscountCouponValidTo = ValidationHelper.GetDateTime(reader["DiscountCouponValidTo"], DateTime.MinValue);
                        return discountCouponInfo;
                    }
                }
                return discountCouponInfo;
            }
            catch (Exception ex)
            {
                CMS.EventLog.EventLogProvider.LogEvent("E", "", "", ex.Message, "CMSModuleLoader.GetDiscountValueById", 0, "", 0, "", "", 0, "", "", "", DateTime.Now);
                // ev.LogEvent("E", DateTime.Now, "CMSModuleLoader.GetDiscountValueById", ex.Message);
            }
            return null;
        }

        private double CalculateDiscount(DiscountCouponInfo discountCoupon, double price)
        {
            double result = 0;
            double totalPrice = price;
           
                if (discountCoupon.DiscountCouponIsFlatValue)
                    result = discountCoupon.DiscountCouponValue;
                else
                    result = totalPrice*discountCoupon.DiscountCouponValue/100.0;
         
            return result;
        }

        private string GetSkuProductTypeBySkuId(int skuid)
        {
            var ev = new EventLogProvider();
            try
            {
                string typeproduct = string.Empty;
                string sql = "SELECT [SKUID],[SKUProductType] FROM [COM_SKU] WHERE skuid = @skuid";
                var param = new QueryDataParameters();
                param.Add(new DataParameter("@skuid", skuid));
                DataSet ds = ConnectionHelper.ExecuteQuery(sql, param, QueryTypeEnum.SQLQuery);
                if(!DataHelper.DataSourceIsEmpty(ds))
                {
                    foreach(DataRow reader in ds.Tables[0].Rows)
                    {
                        typeproduct = ValidationHelper.GetString(reader["SKUProductType"], string.Empty);
                        return typeproduct;
                    }
                }
                return typeproduct;
            }
            catch(Exception ex)
            {
                CMS.EventLog.EventLogProvider.LogEvent("E", "", "", ex.Message, "CMSModuleLoader.GetSkuProductTypeBySkuId", 0, "", 0, "", "", 0, "", "", "", DateTime.Now);
               // ev.LogEvent("E", DateTime.Now, "CMSModuleLoader.GetSkuProductTypeBySkuId", ex.Message);
            }
            return null;
        }

        private List<int> GetSkuidByCustomerId(int shopcartcustomerid)
        {
            var ev = new EventLogProvider();
            try
            {
                List<int> list = null;
                string sql = "SELECT [SKUID]  FROM [COM_ShoppingCartSKU] as ss";
                sql += " INNER JOIN [COM_ShoppingCart] as s";
                sql += " ON s.[ShoppingCartID] = ss.[ShoppingCartID]";
                sql += " WHERE s.[ShoppingCartCustomerID] = @shopcartcustomerid";
                var param = new QueryDataParameters();
                param.Add(new DataParameter("@shopcartcustomerid", shopcartcustomerid));
                DataSet ds = ConnectionHelper.ExecuteQuery(sql, param, QueryTypeEnum.SQLQuery);
                if(!DataHelper.DataSourceIsEmpty(ds))
                {
                    list = new List<int>();
                    foreach(DataRow reader in ds.Tables[0].Rows)
                    {
                        int sku = ValidationHelper.GetInteger(reader["SKUID"], 0);
                        list.Add(sku);
                    }
                }
                return list;
            }
            catch(Exception ex)
            {
                CMS.EventLog.EventLogProvider.LogEvent("E", "", "", ex.Message, "CMSModuleLoader.GetSkuidByUserId", 0, "", 0, "", "", 0, "", "", "", DateTime.Now);
             //   ev.LogEvent("E", DateTime.Now, "CMSModuleLoader.GetSkuidByUserId", ex.Message);
            }
            return null;
        }

       


        /// <summary>
        /// Resolves custom macros.
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">Event arguments representing the resolved macro</param>
        private void MacroResolver_OnResolveCustomMacro(object sender, MacroEventArgs e)
        {
            var ev = new EventLogProvider();
            string discountValue = null;
            string orderId = QueryHelper.GetInteger("orderID", 0).ToString();
            MacroResolver shoppingCartInfoObMacroResolver = (MacroResolver)sender;

            // Checks that the macro is not resolved yet.
            if (!e.Match)
            {
                // Defines the return values of specific custom macro expressions.
                switch (e.Expression.ToLower())
                {

                    // Handles the {#CustomExpression#} macro.
                    case "firstletter":
                        string firstletter = CMS.MacroEngine.MacroContext.CurrentResolver.ResolveMacros("{%CurrentUser.LastName%}");
                        string substr = string.Empty;
                        if (!string.IsNullOrEmpty(firstletter))
                            substr = firstletter.Substring(0, 1);

                        e.Match = true;
                        e.Result = substr;
                        break;

                    case "priceht":


                        //Get Shopping Cart Object from resolver
                        MacroResolver test = (MacroResolver)sender;

                        CMS.Ecommerce.ShoppingCartInfo cartObj = test.SourceObject as CMS.Ecommerce.ShoppingCartInfo;
                        CMS.Ecommerce.CurrencyInfo currentCurrency = null;
                        currentCurrency = cartObj.Currency;
                        double tax = Convert.ToDouble(cartObj.TotalTax);
                        double price = Convert.ToDouble(cartObj.TotalPrice);
                        double resultat = price - tax;
                        e.Match = true;
                        e.Result = CMS.Ecommerce.CurrencyInfoProvider.GetFormattedValue(resultat, currentCurrency).ToString();
                        break;
                    case "taxe":
                        MacroResolver test1 = (MacroResolver)sender;
                        CMS.Ecommerce.ShoppingCartInfo cartObj1 = test1.SourceObject as CMS.Ecommerce.ShoppingCartInfo;
                        double taxe = Convert.ToDouble(cartObj1.TotalTax);
                        currentCurrency = cartObj1.Currency;
                        e.Match = true;
                        e.Result = CMS.Ecommerce.CurrencyInfoProvider.GetFormattedValue(taxe, currentCurrency).ToString();
                        break;
                    case "invoice":
                        int num1 = 0;
                        var cn = ConnectionHelper.GetConnection();
                        DataSet orders = cn.ExecuteQuery("Ecommerce.Transformations.invoice", null, null);//CMS.Ecommerce.OrderInfoProvider.GetOrders(null, null);
                       // DataSet orders = cn.ExecuteQuery("Ecommerce.Transformations.invoice", null, null, null, 1);//CMS.Ecommerce.OrderInfoProvider.GetOrders(null, null);

                        if (!DataHelper.DataSourceIsEmpty(orders))
                        {
                            num1 = Convert.ToInt32(orders.Tables[0].Rows[0]["max"]) + 1;
                            // int nb = orders.Tables[0].Rows.Count;
                            //// Create object from DataRow
                            ////  ev.LogEvent("E", DateTime.Now, "nb", nb.ToString());
                            ////78
                            // for (int i = 0; i < nb; i++)
                            // {
                            //     CMS.Ecommerce.OrderInfo order = new CMS.Ecommerce.OrderInfo(orders.Tables[0].Rows[i]);
                            //   //  ev.LogEvent("E", DateTime.Now, "nb", order.OrderInvoiceNumber);
                            //     if (order.OrderInvoiceNumber != null)
                            //     {
                            //         num = int.Parse(order.OrderInvoiceNumber) + 1;
                            //        // i = nb + 1;
                            //         break;
                            //     }

                            //     ev.LogEvent("E", DateTime.Now, "nb1", num.ToString());
                            // }
                            CMS.EventLog.EventLogProvider.LogEvent("E", "", "", num1.ToString(), "CMSModuleLoader.GetSkuidByUserId", 0, "", 0, "", "", 0, "", "", "", DateTime.Now);
                           // ev.LogEvent("E", DateTime.Now, "nb1", num1.ToString());
                        }
                        e.Match = true;
                        e.Result = num1.ToString();
                        break;
                    case "value1":
                        //string orderID = CMS.CMSHelper.CMSContext.CurrentResolver.ResolveMacros("{%Order.OrderID%}");
                        // string wherc="OrderID ="+orderID;
                        // DataSet ordersitem = CMS.Ecommerce.OrderItemInfoProvider.GetOrderItems(wherc, null);
                        //  string value1 = string.Empty;
                        // string orderID = CMS.CMSHelper.CMSContext.CurrentResolver.ResolveMacros("{%OrderID%}");
                        // if (!DataHelper.DataSourceIsEmpty(ordersitem))
                        //  {
                        //  foreach (DataRow orderItemDr in ordersitem.Tables[0].Rows)
                        //  {
                        // Create object from DataRow
                        //   CMS.Ecommerce.OrderItemInfo orderItem = new CMS.Ecommerce.OrderItemInfo(ordersitem.Tables[0].Rows[0]);

                        // value1 = orderItem.OrderItemPrice.ToString();
                        // ev.LogEvent("E", DateTime.Now, "TVA", value1);
                        // }



                        //  }
                        e.Match = true;
                        e.Result = "";
                        break;
                    case "titreinvoice":
                        string value = string.Empty;
                        string facture = CMS.MacroEngine.MacroContext.CurrentResolver.ResolveMacros("{%Order.facture%}");
                        string commande = CMS.MacroEngine.MacroContext.CurrentResolver.ResolveMacros("{%Order.OrderID%}");
                        if (string.IsNullOrEmpty(facture))
                        {
                            value = "Commande N° " + commande;
                        }
                        else
                        {
                            value = "Facture N° " + facture;
                        }
                        e.Match = true;
                        e.Result = value;
                        break;

                    case "six":
                        double taxe6 = 0;
                        MacroResolver test6 = (MacroResolver)sender;
                        CMS.Ecommerce.ShoppingCartInfo cartObj6 = test6.SourceObject as CMS.Ecommerce.ShoppingCartInfo;
                        foreach (CMS.Ecommerce.ShoppingCartItemInfo ci in cartObj6.CartItems)
                        {
                            DataTable t = ci.TaxesTable;
                            foreach (DataRow drow in t.Rows)
                            {
                                //double theTax = Convert.ToDouble(drow["TaxValue"]);
                                double theTax = (double)drow["TaxValue"];
                                if (theTax == 6)
                                {
                                    taxe6 += ci.TotalPrice - ci.TotalTax;
                                    break;
                                }
                            }
                        }
                        e.Match = true;
                        //e.Result = CMS.Ecommerce.CurrencyInfoProvider.GetFormattedValue(taxe6, currentCurrency).ToString();
                        e.Result = taxe6;
                        break;
                    case "unsix":
                        double taxe7 = 0;
                        MacroResolver test7 = (MacroResolver)sender;
                        CMS.Ecommerce.ShoppingCartInfo cartObj7 = test7.SourceObject as CMS.Ecommerce.ShoppingCartInfo;
                        foreach (CMS.Ecommerce.ShoppingCartItemInfo ci in cartObj7.CartItems)
                        {
                            DataTable t = ci.TaxesTable;
                            foreach (DataRow drow in t.Rows)
                            {
                                //double theTax = Convert.ToDouble(drow["TaxValue"]);
                                double theTax = (double)drow["TaxValue"];
                                if (theTax != 6)
                                {
                                    taxe7 += ci.TotalPrice - ci.TotalTax;
                                    break;
                                }
                            }
                        }
                        e.Match = true;
                        //e.Result = CMS.Ecommerce.CurrencyInfoProvider.GetFormattedValue(taxe6, currentCurrency).ToString();
                        e.Result = taxe7;
                        break;
                    case "hastax":
                        double taxe8 = 0;
                        MacroResolver test8 = (MacroResolver)sender;
                        CMS.Ecommerce.ShoppingCartInfo cartObj8 = test8.SourceObject as CMS.Ecommerce.ShoppingCartInfo;
                        foreach (CMS.Ecommerce.ShoppingCartItemInfo ci in cartObj8.CartItems)
                        {
                            DataTable t = ci.TaxesTable;
                            foreach (DataRow drow in t.Rows)
                            {
                                //double theTax = Convert.ToDouble(drow["TaxValue"]);
                                double theTax = (double)drow["TaxValue"];
                                if (theTax >0)
                                {
                                    taxe8 = 1;
                                    break;
                                }
                            }
                        }
                        e.Match = true;
                        //e.Result = CMS.Ecommerce.CurrencyInfoProvider.GetFormattedValue(taxe6, currentCurrency).ToString();
                        e.Result = taxe8;
                        break;

                    case "infoeproduct":
                        e.Match = true;
                        string txt = string.Empty;
                        var bdResolver = (MacroResolver)sender;
                        CMS.EventLog.EventLogProvider.LogEvent("I", "", "", "", "infoeproduct", 0, "", 0, "", "", 0, "", "", "", DateTime.Now);
                       // ev.LogEvent("I", DateTime.Now, "infoeproduct", "");
                        if(bdResolver != null)
                        {
                            CMS.EventLog.EventLogProvider.LogEvent("I", "", "", "", "bdResolver not null", 0, "", 0, "", "", 0, "", "", "", DateTime.Now);
                           // ev.LogEvent("I", DateTime.Now, "bdResolver not null", "");
                            var sCart = (ShoppingCartInfo)bdResolver.SourceObject;
                            if(sCart != null)
                            {
                                CMS.EventLog.EventLogProvider.LogEvent("I", "", "", "", "sCart not null", 0, "", 0, "", "", 0, "", "", "", DateTime.Now);
                          
                              //  ev.LogEvent("I", DateTime.Now, "sCart not null", "");
                                CMS.EventLog.EventLogProvider.LogEvent("I", "", "", "", "OrderId : " + sCart.ShoppingCartCustomerID.ToString(), 0, "", 0, "", "", 0, "", "", "", DateTime.Now);
                          
                               // ev.LogEvent("I", DateTime.Now, "OrderId : " + sCart.ShoppingCartCustomerID.ToString(), "");

                                if(sCart.ShoppingCartCustomerID > 0)
                                {
                                    List<int> skuids = GetSkuidByCustomerId(sCart.ShoppingCartCustomerID);
                                    foreach(var skuid in skuids)
                                    {
                                        string typeproduct = GetSkuProductTypeBySkuId(skuid);
                                        if(typeproduct != string.Empty && typeproduct == "EPRODUCT")
                                        {
                                            txt = "Vous allez recevoir le code de téléchargement par courriel séparé d’ici un instant.";
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                        e.Result = txt;
                        break;

                    case "freebundle":
                        string bundle = string.Empty, bundledString = string.Empty;
                        MacroResolver bundleResolver = (MacroResolver)sender;
                        ShoppingCartInfo sc = bundleResolver.SourceObject as ShoppingCartInfo;
                        if (sc != null)
                        {
                            bundledString = sc.GetStringValue("ShoppingCartBundleData", string.Empty);
                            if (string.IsNullOrEmpty(bundledString))
                            {
                                OrderInfo oi = OrderInfoProvider.GetOrderInfo(sc.OrderId);
                                if (oi != null)
                                {
                                    bundledString = oi.GetStringValue("OrderBundleData", string.Empty);
                                }
                            }
                            if (!string.IsNullOrEmpty(bundledString))
                            {
                                bundle = string.Format("<b><u>Liste de vos produits gratuits:</u></b><br/> {0}", CustomBundle.TranslateBundle(bundledString));
                            }
                        }

                                                
                        e.Match = true;
                        e.Result = bundle;
                        break;

                    case "discountvalue":
                        
                        if (!string.IsNullOrEmpty(orderId))
                        {
                            discountValue = GetDiscountValueById(int.Parse(orderId));
                        }
                        
                        e.Match = true;
                        e.Result = discountValue;
                        break;


                    case "totalitemspriceinmaincurrency":
                        double discountvaluepercent = 0;
                       
                        CMS.Ecommerce.ShoppingCartInfo shoppingCartInfo = shoppingCartInfoObMacroResolver.SourceObject as CMS.Ecommerce.ShoppingCartInfo;
                       
                        if (shoppingCartInfo != null)
                        {
                            discountvaluepercent = shoppingCartInfo.TotalItemsPriceInMainCurrency;
                            
                        }

                        e.Match = true;
                        e.Result = discountvaluepercent;
                        break;


                    case "getdiscountvalue":
                        double discountCoupon = 0;
                        CMS.Ecommerce.ShoppingCartInfo cartInfo = shoppingCartInfoObMacroResolver.SourceObject as CMS.Ecommerce.ShoppingCartInfo;
                        if (!string.IsNullOrEmpty(orderId))
                        {
                            DiscountCouponInfo currentDiscountCouponInfo = GetDiscountByOrderId(int.Parse(orderId));
                            discountCoupon = CalculateDiscount(currentDiscountCouponInfo,
                                cartInfo.TotalItemsPriceInMainCurrency);
                        }

                        e.Match = true;
                        e.Result = discountCoupon;
                        break;
                          

                }

          
            }
        }
    }
}