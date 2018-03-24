using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.CMSHelper;
using CMS.Ecommerce;
using CMS.ExtendedControls;
using CMS.GlobalHelper;
using CMS.SettingsProvider;
using CMS.UIControls;
using CMS.Helpers;
using CMS.DataEngine;

public partial class CMSModules_Ecommerce_Controls_UI_OrderList : CMSAdminListControl
{
    #region "Variables"

    private int customerId = 0;

    #endregion


    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        customerId = QueryHelper.GetInteger("customerId", 0);

        gridElem.IsLiveSite = IsLiveSite;
        gridElem.OnExternalDataBound += gridElem_OnExternalDataBound;
        gridElem.OnAction += gridElem_OnAction;
        gridElem.GridView.RowDataBound += GridView_RowDataBound;
        gridElem.WhereCondition = GetWhereCondition();
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);
        if (customerId > 0)
        {
            gridElem.GridView.Columns[2].Visible = false;
            gridElem.GridView.Columns[3].Visible = false;
        }
    }

    #endregion


    #region "Event handlers"

    private void GridView_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            string color = ValidationHelper.GetString(((DataRowView)(e.Row.DataItem)).Row["StatusColor"], string.Empty);
            if (color != string.Empty)
            {
                e.Row.Style.Add("background-color", color);
            }
        }
    }


    private object gridElem_OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        DataRowView dr = null;
        bool orderCurrencyIsMain = false;

        switch (sourceName.ToLowerCSafe())
        {
            case "idandinvoice":
                dr = (DataRowView)parameter;
                int orderId = ValidationHelper.GetInteger(dr["OrderID"], 0);
                string invoiceNumber = ValidationHelper.GetString(dr["OrderInvoiceNumber"], "");

                // Show OrderID and invoice number in brackets if InvoiceNumber is different from OrderID
                if (!string.IsNullOrEmpty(invoiceNumber) && (invoiceNumber != orderId.ToString()))
                {
                    return HTMLHelper.HTMLEncode(orderId + " (" + invoiceNumber + ")");
                }
                return orderId;

            case "customer":
                dr = (DataRowView)parameter;
                string customerName = dr["CustomerFirstName"] + " " + dr["CustomerLastName"];
                string customerCompany = ValidationHelper.GetString(dr["CustomerCompany"], "");

                // Show customer name and company in brakcets, if company specified
                if (!string.IsNullOrEmpty(customerCompany))
                {
                    return HTMLHelper.HTMLEncode(customerName + " (" + customerCompany + ")");
                }
                return HTMLHelper.HTMLEncode(customerName);

            //
            case "email":
                dr = (DataRowView)parameter;
                string mailCustomer = ValidationHelper.GetString(dr["CustomerEmail"],"");
                // Show customer mail             
                return HTMLHelper.HTMLEncode(mailCustomer);
           //

            case "totalpriceinmaincurrency":
                dr = (DataRowView)parameter;
                double totalPriceInMainCurrency = ValidationHelper.GetDouble(dr["OrderTotalPriceInMainCurrency"], 0);
                orderCurrencyIsMain = ValidationHelper.GetBoolean(dr["CurrencyIsMain"], false);

                // Format currency
                string priceInMainCurrencyFormatted = "";
                if (orderCurrencyIsMain)
                {
                    priceInMainCurrencyFormatted = String.Format(dr["CurrencyFormatString"].ToString(), totalPriceInMainCurrency);
                }
                else
                {
                    int siteId = ValidationHelper.GetInteger(dr["OrderSiteID"], 0);
                    priceInMainCurrencyFormatted = CurrencyInfoProvider.GetFormattedPrice(totalPriceInMainCurrency, siteId);
                }

                return HTMLHelper.HTMLEncode(priceInMainCurrencyFormatted);

            case "totalpriceinorderprice":
                dr = (DataRowView)parameter;
                orderCurrencyIsMain = ValidationHelper.GetBoolean(dr["CurrencyIsMain"], false);

                if (orderCurrencyIsMain)
                {
                    return "-";
                }

                // If order is not in main currency, show order price
                double orderTotalPrice = ValidationHelper.GetDouble(dr["OrderTotalPrice"], 0);
                string priceFormatted = String.Format(dr["CurrencyFormatString"].ToString(), orderTotalPrice);

                // Formated currency
                return HTMLHelper.HTMLEncode(priceFormatted);

            case "orderpaymentoptionid":
                // Tranform to display name and localize
                int paymentOptionId = ValidationHelper.GetInteger(parameter, 0);
                PaymentOptionInfo paymentOption = PaymentOptionInfoProvider.GetPaymentOptionInfo(paymentOptionId);

                if (paymentOption != null)
                {
                    return HTMLHelper.HTMLEncode(ResHelper.LocalizeString(paymentOption.PaymentOptionDisplayName));
                }
                break;

            case "ordershippingoptionid":
                // Tranform to display name and localize
                int shippingOptionId = ValidationHelper.GetInteger(parameter, 0);
                ShippingOptionInfo shippingOption = ShippingOptionInfoProvider.GetShippingOptionInfo(shippingOptionId);

                if (shippingOption != null)
                {
                    return HTMLHelper.HTMLEncode(ResHelper.LocalizeString(shippingOption.ShippingOptionDisplayName));
                }
                break;

            case "note":
                string note = ValidationHelper.GetString(parameter, "");

                if (string.IsNullOrEmpty(note))
                {
                    return "-";
                }
                // Display link, note is in tooltip
                return "<a>" + GetString("general.view") + "</a>";
        }
        return parameter;
    }


    /// <summary>
    /// Handles the gridElem's OnAction event.
    /// </summary>
    /// <param name="actionName">Name of item (button) that throws event</param>
    /// <param name="actionArgument">ID (value of Primary key) of corresponding data row</param>
    protected void gridElem_OnAction(string actionName, object actionArgument)
    {
        int orderId = ValidationHelper.GetInteger(actionArgument, 0);
        OrderInfo oi = null;
        OrderStatusInfo osi = null;

        switch (actionName.ToLowerCSafe())
        {
            case "edit":
                string redirectToUrl = "Order_Edit.aspx?orderID=" + orderId.ToString();
                if (customerId > 0)
                {
                    redirectToUrl += "&customerId=" + customerId.ToString();
                }
                URLHelper.Redirect(redirectToUrl);
                break;


            case "delete":
                // Check 'ModifyOrders' and 'EcommerceModify' permission
                if (!ECommerceContext.IsUserAuthorizedForPermission("ModifyOrders"))
                {
                    AccessDenied("CMS.Ecommerce", "EcommerceModify OR ModifyOrders");
                }

                // delete OrderInfo object from database
                OrderInfoProvider.DeleteOrderInfo(orderId);
                break;

            case "previous":
                // Check 'ModifyOrders' and 'EcommerceModify' permission
                if (!ECommerceContext.IsUserAuthorizedForPermission("ModifyOrders"))
                {
                    AccessDenied("CMS.Ecommerce", "EcommerceModify OR ModifyOrders");
                }

                oi = OrderInfoProvider.GetOrderInfo(orderId);
                if (oi != null)
                {
                    osi = OrderStatusInfoProvider.GetPreviousEnabledStatus(oi.OrderStatusID);
                    if (osi != null)
                    {
                        oi.OrderStatusID = osi.StatusID;
                        // Save order status changes
                        OrderInfoProvider.SetOrderInfo(oi);
                    }
                }
                break;

            case "next":
                // Check 'ModifyOrders' and 'EcommerceModify' permission
                if (!ECommerceContext.IsUserAuthorizedForPermission("ModifyOrders"))
                {
                    AccessDenied("CMS.Ecommerce", "EcommerceModify OR ModifyOrders");
                }

                oi = OrderInfoProvider.GetOrderInfo(orderId);
                if (oi != null)
                {
                    osi = OrderStatusInfoProvider.GetNextEnabledStatus(oi.OrderStatusID);
                    if (osi != null)
                    {
                        oi.OrderStatusID = osi.StatusID;
                        // Save order status changes
                        OrderInfoProvider.SetOrderInfo(oi);
                    }
                }
                break;
        }
    }

    #endregion


    #region "Other methods"

    /// <summary>
    /// Creates where condition based on query string.
    /// </summary>
    private string GetWhereCondition()
    {
        string where = "";
        if (customerId > 0)
        {
            where = SqlHelper.AddWhereCondition(where, "CustomerID = " + customerId);
        }

        return where;
    }

    #endregion
}