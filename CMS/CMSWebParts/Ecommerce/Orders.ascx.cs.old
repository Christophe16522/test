using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.CMSHelper;
using CMS.Ecommerce;
using CMS.ExtendedControls;
using CMS.GlobalHelper;
using CMS.PortalControls;
using CMS.SettingsProvider;
using CMS.SiteProvider;

public partial class CMSWebParts_Ecommerce_Orders : CMSAbstractWebPart
{
    #region "Constants"

    private const int SQL_DATE_TIME_LIMIT = 73000;

    private const string PAY_STATUS_ALL = "all";
    private const string PAY_STATUS_NOT_PAID = "0";
    private const string PAY_STATUS_PAID = "1";

    private const string ORDERBY_DATE_DESC = "OrderDate DESC";
    private const string ORDERBY_DATE_ASC = "OrderDate ASC";
    private const string ORDERBY_PRICE_DESC = "OrderTotalPriceInMainCurrency ASC";
    private const string ORDERBY_PRICE_ASC = "OrderTotalPriceInMainCurrency DESC";

    private const string ACTION_EDIT = "edit";
    private const string ACTION_DELETE = "delete";
    private const string ACTION_STATUS_MOVE_PREVIOUS = "statusmoveprevious";
    private const string ACTION_STATUS_MOVE_NEXT = "statusmovenext";
    private const string ACTIONS_STATUS_MOVE = "statusmove";

    private const string COLUMN_ID_AND_INVOICE = "IDANDINVOICE";
    private const string COLUMN_CUSTOMER = "CUSTOMER";
    private const string COLUMN_DATE = "DATE";
    private const string COLUMN_PRICE = "PRICE";
    private const string COLUMN_STATUS = "STATUS";
    private const string COLUMN_PAYMENT = "PAYMENT";
    private const string COLUMN_IS_PAID = "ISPAID";
    private const string COLUMN_SHIPPING = "SHIPPING";
    private const string COLUMN_TRACKING_NUMBER = "TRACKINGNUMBER";
    private const string COLUMN_NOTE = "NOTE";

    #endregion


    #region "Variables"

    private List<string> visibleActionsList = new List<string>()
        {
            ACTION_EDIT,
            ACTION_DELETE,
            ACTIONS_STATUS_MOVE
        };

    #endregion


    #region "Properties"

    /// <summary>
    /// Order status.
    /// </summary>
    public string OrderStatus
    {
        get
        {
            return ValidationHelper.GetString(this.GetValue("OrderStatus"), "");
        }
        set
        {
            this.SetValue("OrderStatus", value);
        }
    }


    /// <summary>
    /// Customer or company like.
    /// </summary>
    public string CustomerOrCompany
    {
        get
        {
            return ValidationHelper.GetString(this.GetValue("CustomerOrCompany"), "");
        }
        set
        {
            this.SetValue("CustomerOrCompany", value);
        }
    }


    /// <summary>
    /// Orders with note only.
    /// </summary>
    public bool HasNote
    {
        get
        {
            return ValidationHelper.GetBoolean(this.GetValue("HasNote"), false);
        }
        set
        {
            this.SetValue("HasNote", value);
        }
    }


    /// <summary>
    /// Payment method.
    /// </summary>
    public string PaymentMethod
    {
        get
        {
            return ValidationHelper.GetString(this.GetValue("PaymentMethod"), "");
        }
        set
        {
            this.SetValue("PaymentMethod", value);
        }
    }


    /// <summary>
    /// Payment status.
    /// </summary>
    public string PaymentStatus
    {
        get
        {
            return ValidationHelper.GetString(this.GetValue("PaymentStatus"), "all");
        }
        set
        {
            this.SetValue("PaymentStatus", value);
        }
    }


    /// <summary>
    /// Currency.
    /// </summary>
    public string Currency
    {
        get
        {
            return ValidationHelper.GetString(this.GetValue("Currency"), "");
        }
        set
        {
            this.SetValue("Currency", value);
        }
    }


    /// <summary>
    /// Total price in main currency from.
    /// </summary>
    public double MinPriceInMainCurrency
    {
        get
        {
            return ValidationHelper.GetDouble(this.GetValue("MinPriceInMainCurrency"), 0);
        }
        set
        {
            this.SetValue("MinPriceInMainCurrency", value);
        }
    }


    /// <summary>
    /// Total price in main currency to.
    /// </summary>
    public double MaxPriceInMainCurrency
    {
        get
        {
            return ValidationHelper.GetDouble(this.GetValue("MaxPriceInMainCurrency"), 0);
        }
        set
        {
            this.SetValue("MaxPriceInMainCurrency", value);
        }
    }


    /// <summary>
    /// Shipping option.
    /// </summary>
    public string ShippingOption
    {
        get
        {
            return ValidationHelper.GetString(this.GetValue("ShippingOption"), "");
        }
        set
        {
            this.SetValue("ShippingOption", value);
        }
    }


    /// <summary>
    /// Shipping country.
    /// </summary>
    public string ShippingCountry
    {
        get
        {
            return ValidationHelper.GetString(this.GetValue("ShippingCountry"), "");
        }
        set
        {
            this.SetValue("ShippingCountry", value);
        }
    }


    /// <summary>
    /// How old orders.
    /// </summary>
    public int HowOld
    {
        get
        {
            return ValidationHelper.GetInteger(this.GetValue("HowOld"), 365);
        }
        set
        {
            this.SetValue("HowOld", value);
        }
    }


    /// <summary>
    /// Older than (days).
    /// </summary>
    public int OlderThan
    {
        get
        {
            return ValidationHelper.GetInteger(this.GetValue("OlderThan"), 0);
        }
        set
        {
            this.SetValue("OlderThan", value);
        }
    }


    /// <summary>
    /// If you specify this number, paging will be disabled.
    /// </summary>
    public int TopN
    {
        get
        {
            return ValidationHelper.GetInteger(this.GetValue("TopN"), 0);
        }
        set
        {
            this.SetValue("TopN", value);
        }
    }


    /// <summary>
    /// Sort orders by.
    /// </summary>
    public string OrderBy
    {
        get
        {
            return ValidationHelper.GetString(this.GetValue("OrderBy"), "");
        }
        set
        {
            this.SetValue("OrderBy", value);
        }
    }


    /// <summary>
    /// Page size.
    /// </summary>
    public int PageSize
    {
        get
        {
            return ValidationHelper.GetInteger(this.GetValue("PageSize"), 0);
        }
        set
        {
            this.SetValue("PageSize", value);
        }
    }


    /// <summary>
    /// Visible columns in listing.
    /// </summary>
    public string VisibleColumns
    {
        get
        {
            return ValidationHelper.GetString(this.GetValue("VisibleColumns"), "");
        }
        set
        {
            this.SetValue("VisibleColumns", value);
        }
    }


    /// <summary>
    /// Visible order actions.
    /// </summary>
    public string VisibleActions
    {
        get
        {
            return ValidationHelper.GetString(this.GetValue("VisibleActions"), "");
        }
        set
        {
            this.SetValue("VisibleActions", value);
        }
    }


    /// <summary>
    /// Gets the messages placeholder.
    /// </summary>
    public override MessagesPlaceHolder MessagesPlaceHolder
    {
        get
        {
            return plcMessages;
        }
    }

    #endregion


    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        gridElem.IsLiveSite = IsLiveSite;
        gridElem.OnExternalDataBound += gridElem_OnExternalDataBound;
        gridElem.OnAction += gridElem_OnAction;
        gridElem.GridView.RowDataBound += GridView_RowDataBound;
    }

    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        // If no action is visible, hides actions column
        if (visibleActionsList.Count <= 0)
        {
            gridElem.GridView.Columns[0].Visible = false;
        }

        DisplayColumns();
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

            case "statusdisplayname":
                return HTMLHelper.HTMLEncode(ResHelper.LocalizeString(Convert.ToString(parameter)));

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
                return "<span style=\"text-decoration: underline\">" + GetString("general.view") + "</span>";

            case ACTION_EDIT:
                ShowOrHideAction(sender, ACTION_EDIT);
                break;

            case ACTION_DELETE:
                ShowOrHideAction(sender, ACTION_DELETE);
                break;

            case ACTION_STATUS_MOVE_PREVIOUS:
            case ACTION_STATUS_MOVE_NEXT:
                ShowOrHideAction(sender, ACTIONS_STATUS_MOVE);
                break;
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
                string redirectToUrl = "~/CMSModules/Ecommerce/Pages/Tools/Orders/Order_Edit.aspx?orderID=" + orderId.ToString();
                URLHelper.Redirect(redirectToUrl);
                break;

            case "delete":
                // Check 'ModifyOrders' and 'EcommerceModify' permission
                if (!ECommerceContext.IsUserAuthorizedForPermission("ModifyOrders"))
                {
                    return;
                }

                // Delete OrderInfo object from database
                OrderInfoProvider.DeleteOrderInfo(orderId);
                break;

            case "previous":
                // Check 'ModifyOrders' and 'EcommerceModify' permission
                if (!ECommerceContext.IsUserAuthorizedForPermission("ModifyOrders"))
                {
                    return;
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
                    return;
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


    #region "Methods - WebPart"

    /// <summary>
    /// Content loaded event handler.
    /// </summary>
    public override void OnContentLoaded()
    {
        base.OnContentLoaded();

        // Check module permissions
        if (!ECommerceContext.IsUserAuthorizedForPermission("ReadOrders"))
        {
            ShowError(String.Format(GetString("CMSMessages.AccessDeniedResource"), "EcommerceRead OR ReadOrders"));
            gridElem.Visible = false;
            return;
        }

        SetupControl();
    }


    /// <summary>
    /// Initializes the control properties.
    /// </summary>
    protected void SetupControl()
    {
        if (this.StopProcessing)
        {
            // Do not process
        }
        else
        {
            ReloadData();
        }
    }


    /// <summary>
    /// Reloads the control data.
    /// </summary>
    public override void ReloadData()
    {
        base.ReloadData();

        // Assign filter to unigrid
        gridElem.Query = "ecommerce.order.selectorderlist";
        gridElem.WhereCondition = GetWhereCondition();
        gridElem.QueryParameters = GetQueryParameters();
        gridElem.OrderBy = GetOrderBy();
        SetTopN();
        gridElem.Pager.DefaultPageSize = GetPageSize();
    }

    #endregion


    #region "Methods - private"

    /// <summary>
    /// Hides specific order action (edit, delete, ...) from unigrid, if it should be hidden.
    /// </summary>
    /// <param name="sender">Sender from unigrid's ExternaDataBound.</param>
    /// <param name="actionName">Name of action to show/hide.</param>
    private void ShowOrHideAction(object sender, string actionName)
    {
        string[] visibleActionsArray = VisibleActions.Split('|');
        bool hideAction = true;

        // Determine, if action should be shown or hidden
        foreach (var action in visibleActionsArray)
        {
            if (action == actionName)
            {
                hideAction = false;
            }
        }
        // Do not hide only if user has permissions
        if (!hideAction && ECommerceContext.IsUserAuthorizedForPermission("ModifyOrders"))
        {
            return;
        }

        // Hiding
        ImageButton btn = sender as ImageButton;
        if (btn != null)
        {
            btn.Visible = false;
            visibleActionsList.Remove(actionName);
        }
    }


    /// <summary>
    /// Displays or hides columns based on VisibleColumns property.
    /// </summary>
    private void DisplayColumns()
    {
        string[] visibleColumns = VisibleColumns.Split('|');

        // Hide all first
        foreach (var item in gridElem.NamedColumns.Values)
        {
            item.Visible = false;
        }

        // Show columns that should be visible
        foreach (var item in visibleColumns)
        {
            string key = null;
            switch (item)
            {
                case COLUMN_ID_AND_INVOICE:
                    key = "IDAndInvoice";
                    break;

                case COLUMN_CUSTOMER:
                    key = "Customer";
                    break;

                case COLUMN_DATE:
                    key = "Date";
                    break;

                case COLUMN_PRICE:
                    key = "MainCurrencyPrice";
                    gridElem.NamedColumns["OrderPrice"].Visible = MoreThanOneCurrencyUsed();
                    break;

                case COLUMN_STATUS:
                    key = "OrderStatus";
                    break;

                case COLUMN_PAYMENT:
                    key = "PaymentOption";
                    break;

                case COLUMN_IS_PAID:
                    key = "IsPaid";
                    break;

                case COLUMN_SHIPPING:
                    key = "ShippingOption";
                    break;

                case COLUMN_TRACKING_NUMBER:
                    key = "TrackingNumber";
                    break;

                case COLUMN_NOTE:
                    key = "Note";
                    break;
            }

            // Show column
            if (key != null)
            {
                gridElem.NamedColumns[key].Visible = true;
            }
        }
    }


    /// <summary>
    /// Returns true, if more than one currency is used and enabled, otherwise false.
    /// </summary>
    private bool MoreThanOneCurrencyUsed()
    {
        bool? moreUsed = RequestStockHelper.GetItem("MoreThanOneCurrencyUsed") as bool?;

        if (!moreUsed.HasValue)
        {
            InfoDataSet<CurrencyInfo> dataSet = CurrencyInfoProvider.GetCurrencies("CurrencyEnabled = 1", null, 2, "CurrencyID", CMSContext.CurrentSiteID);
            moreUsed = dataSet.Items.Count > 1;

            RequestStockHelper.Add("MoreThanOneCurrencyUsed", moreUsed);
        }

        return moreUsed.Value;
    }


    /// <summary>
    /// Returns where condition based on webpart fields.
    /// </summary>
    private string GetWhereCondition()
    {
        // Orders from current site
        string where = "OrderSiteID = " + CMSContext.CurrentSiteID;

        // Order status filter
        OrderStatusInfo status = OrderStatusInfoProvider.GetOrderStatusInfo(OrderStatus, CMSContext.CurrentSiteName);
        if (status != null)
        {
            where = SqlHelperClass.AddWhereCondition(where, "OrderStatusID = " + status.StatusID);
        }

        // Customer or company like filter
        if (!string.IsNullOrEmpty(CustomerOrCompany))
        {
            string safeQueryStr = SqlHelperClass.GetSafeQueryString(CustomerOrCompany);
            where = SqlHelperClass.AddWhereCondition(where, "OrderCustomerID  IN (SELECT CustomerID FROM COM_Customer WHERE ((CustomerFirstName + ' ' + CustomerLastName + ' ' + CustomerFirstName) LIKE N'%" + safeQueryStr + "%') OR (CustomerCompany LIKE N'%" + safeQueryStr + "%'))");
        }

        // Filter for orders with note
        if (HasNote)
        {
            where = SqlHelperClass.AddWhereCondition(where, "(OrderNote != '') AND (OrderNote IS NOT NULL)");
        }

        // Payment method filter
        PaymentOptionInfo payment = PaymentOptionInfoProvider.GetPaymentOptionInfo(PaymentMethod, CMSContext.CurrentSiteName);
        if (payment != null)
        {
            where = SqlHelperClass.AddWhereCondition(where, "OrderPaymentOptionID = " + payment.PaymentOptionID);
        }

        // Payment status filter
        switch (PaymentStatus.ToLowerCSafe())
        {
            case PAY_STATUS_NOT_PAID:
                where = SqlHelperClass.AddWhereCondition(where, "(OrderIsPaid = 0) OR (OrderIsPaid IS NULL)");
                break;

            case PAY_STATUS_PAID:
                where = SqlHelperClass.AddWhereCondition(where, "OrderIsPaid = 1");
                break;
        }

        // Currency filter
        CurrencyInfo currencyObj = CurrencyInfoProvider.GetCurrencyInfo(Currency, CMSContext.CurrentSiteName);
        if (currencyObj != null)
        {
            where = SqlHelperClass.AddWhereCondition(where, "OrderCurrencyID = " + currencyObj.CurrencyID);
        }

        // Min price in main currency filter
        if (MinPriceInMainCurrency > 0)
        {
            where = SqlHelperClass.AddWhereCondition(where, "OrderTotalPriceInMainCurrency >= " + MinPriceInMainCurrency);
        }

        // Max price in main currency filter
        if (MaxPriceInMainCurrency > 0)
        {
            where = SqlHelperClass.AddWhereCondition(where, "OrderTotalPriceInMainCurrency <= " + MaxPriceInMainCurrency);
        }

        // Shipping option filter
        ShippingOptionInfo shipping = ShippingOptionInfoProvider.GetShippingOptionInfo(ShippingOption, CMSContext.CurrentSiteName);
        if (shipping != null)
        {
            where = SqlHelperClass.AddWhereCondition(where, "OrderShippingOptionID = " + shipping.ShippingOptionID);
        }

        // Shipping country filter
        where = SqlHelperClass.AddWhereCondition(where, GetCountryWhereCondition());

        // Date filter
        where = SqlHelperClass.AddWhereCondition(where, GetDateWhereCondition());

        return where;
    }


    /// <summary>
    /// Returns where condition filtering ShippingAddress, Country and State.
    /// </summary>
    private string GetCountryWhereCondition()
    {
        if (!string.IsNullOrEmpty(ShippingCountry) && ShippingCountry != "0")
        {
            string subWhere = "1 == 1";
            string[] split = ShippingCountry.Split(';');

            if ((split.Length >= 1) && (split.Length <= 2))
            {
                // Country filter
                CountryInfo country = CountryInfoProvider.GetCountryInfo(split[0]);
                if (country != null)
                {
                    int countryID = country.CountryID;
                    subWhere = "(AddressCountryID = " + countryID + ")";

                    if (split.Length == 2)
                    {
                        // State filter
                        StateInfo state = StateInfoProvider.GetStateInfo(split[1]);
                        if (state != null)
                        {
                            int stateID = state.StateID;
                            subWhere += " AND (AddressStateID = " + stateID + ")";
                        }
                    }
                }
            }

            return "OrderShippingAddressID  IN (SELECT AddressID FROM COM_Address WHERE (" + subWhere + "))";
        }
        return "";
    }


    /// <summary>
    /// Returns where condition filtering OlderThan or HowOld.
    /// </summary>
    private string GetDateWhereCondition()
    {
        if (OlderThan > 0)
        {
            return "OrderDate <= @OlderThan";
        }
        else if (HowOld > 0 && HowOld < SQL_DATE_TIME_LIMIT)
        {
            return "OrderDate >= @From";
        }

        return "";
    }


    /// <summary>
    /// Returns query parameters.
    /// </summary>
    private QueryDataParameters GetQueryParameters()
    {
        QueryDataParameters parameters = new QueryDataParameters();

        // Date parameters
        if (OlderThan > 0)
        {
            // OlderThan parameter
            if (OlderThan > SQL_DATE_TIME_LIMIT)
            {
                OlderThan = SQL_DATE_TIME_LIMIT;
            }
            DateTime param = DateTime.Now.AddDays(-Math.Abs(OlderThan));
            parameters.AddDateTime("@OlderThan", param);
        }
        else if (HowOld > 0 && HowOld < SQL_DATE_TIME_LIMIT)
        {
            // HowOld parameter
            DateTime from = DateTime.Now.AddDays(-Math.Abs(HowOld));
            parameters.AddDateTime("@From", from);
        }

        return parameters;
    }


    /// <summary>
    /// Set TopN property to unigrid and disables Pager, if TopN field is specified.
    /// </summary>
    private void SetTopN()
    {
        if (TopN > 0)
        {
            gridElem.TopN = TopN;
            gridElem.Pager.DisplayPager = false;
        }
    }


    /// <summary>
    /// Returns page size for unigrid.
    /// </summary>
    private int GetPageSize()
    {
        switch (PageSize)
        {
            case -1:
            case 10:
            case 25:
            case 50:
            case 100:
                return PageSize;
            default:
                return 25;
        }
    }


    /// <summary>
    /// Returns string for ORDER BY clause and disables remembering unigrid state.
    /// </summary>
    private string GetOrderBy()
    {
        string orderBy = null;

        // OrderBy specified by drop-down list
        switch (OrderBy)
        {
            case ORDERBY_DATE_ASC:
                orderBy = SqlHelperClass.AddOrderBy(orderBy, ORDERBY_DATE_ASC);
                break;

            case ORDERBY_PRICE_ASC:
                orderBy = SqlHelperClass.AddOrderBy(orderBy, ORDERBY_PRICE_ASC);
                break;

            case ORDERBY_PRICE_DESC:
                orderBy = SqlHelperClass.AddOrderBy(orderBy, ORDERBY_PRICE_DESC);
                break;

            case ORDERBY_DATE_DESC:
            default:
                orderBy = SqlHelperClass.AddOrderBy(orderBy, ORDERBY_DATE_DESC);
                break;
        }

        // Disables remembering unigrid state
        gridElem.RememberState = false;
        return orderBy;
    }

    #endregion
}