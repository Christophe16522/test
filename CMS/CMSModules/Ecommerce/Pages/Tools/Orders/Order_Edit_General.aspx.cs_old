using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.CMSHelper;
using CMS.Ecommerce;
using CMS.GlobalHelper;
using CMS.UIControls;

[Security(Resource = "CMS.Ecommerce", UIElements = "Orders.General")]
public partial class CMSModules_Ecommerce_Pages_Tools_Orders_Order_Edit_General : CMSOrdersPage
{
    #region "Variables"

    protected int orderId = 0;
    protected int originalStatusId = 0;
    protected int originalCompanyAddressId = 0;
    protected int customerId = 0;

    #endregion


    #region "Page events"

    protected override void OnPreInit(EventArgs e)
    {
        customerId = QueryHelper.GetInteger("customerid", 0);
        CustomerID = customerId;
        base.OnPreInit(e);
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        // Register the dialog script
        ScriptHelper.RegisterDialogScript(this);
        ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "RefreshPageScript", ScriptHelper.GetScript("function RefreshPage() { window.location.replace(window.location.href); }"));
        ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "AddressChange", ScriptHelper.GetScript("   function AddressChange(AddressId) { if (AddressId > 0) { document.getElementById('" + hdnAddress.ClientID + "').value = AddressId; " + ClientScript.GetPostBackEventReference(btnChange, "") + " } } "));

        string urlsScript = string.Format("var customerEditUrl = {0}; var addressEditUrl = {1};", ScriptHelper.GetString(URLHelper.ResolveUrl("Order_Edit_Customer.aspx")), ScriptHelper.GetString(URLHelper.ResolveUrl("Order_Edit_Address.aspx")));
        ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "DialogUrls", ScriptHelper.GetScript(urlsScript));

        addressElem.DropDownSingleSelect.AutoPostBack = true;
        addressElem.DropDownSingleSelect.SelectedIndexChanged += new EventHandler(DropDownSingleSelect_SelectedIndexChanged);

        // controls initialization
        lblOrderId.Text = GetString("order_edit.orderidlabel");
        lblOrderDate.Text = GetString("order_edit.orderdatelabel");
        lblInvoiceNumber.Text = GetString("order_edit.invoicenumberlabel");
        lblStatus.Text = GetString("order_edit.orderstatuslabel");
        lblCustomer.Text = GetString("order_edit.customerlabel");
        lblNotes.Text = GetString("order_edit.ordernotelabel");
        btnEditCustomer.Text = GetString("general.edit");
        lblCompanyAddress.Text = GetString("order_edit.lblCompanyAddress");
        btnNewAddress.Text = GetString("general.new");
        btnEditAddress.Text = GetString("general.edit");

        btnEditCustomer.Visible = ECommerceContext.IsUserAuthorizedForPermission("ReadCustomer");

        // get order ID from url
        orderId = QueryHelper.GetInteger("orderid", 0);
        // get order info from database and fill the form
        OrderInfo oi = OrderInfoProvider.GetOrderInfo(orderId);

        if (oi != null)
        {
            // Check order site ID
            CheckOrderSiteID(oi.OrderSiteID);

            originalCompanyAddressId = oi.OrderCompanyAddressID;
            originalStatusId = oi.OrderStatusID;
            customerId = oi.OrderCustomerID;

            statusElem.SiteID = oi.OrderSiteID;
            addressElem.CustomerID = customerId;

            // Initialize javascript to button clicks
            btnEditCustomer.OnClientClick = "EditCustomer(" + customerId + "); return false;";
            btnNewAddress.OnClientClick = "AddAddress('" + customerId + "'); return false;";

            if (!RequestHelper.IsPostBack())
            {
                // initialize form
                InitializeForm(oi);

                // show that the Order was updated successfully
                if (QueryHelper.GetString("saved", "") == "1")
                {
                    ShowChangesSaved();
                }
            }
        }

        // Enable edit address only if address selected
        btnEditAddress.Enabled = addressElem.AddressID != 0;

        // If order is paid
        if ((oi != null) && (oi.OrderIsPaid))
        {
            // Disable specific controls
            orderDate.Enabled = false;
            btnEditCustomer.Enabled = false;
            addressElem.Enabled = false;
            btnEditAddress.Enabled = false;
            btnNewAddress.Enabled = false;
        }
    }


    protected override void OnPreRender(EventArgs e)
    {
        if (addressElem.AddressID > 0)
        {
            btnEditAddress.OnClientClick = "EditAddress('" + customerId + "','" + addressElem.AddressID + "'); return false;";
        }

        base.OnPreRender(e);
    }

    #endregion


    #region "Event handlers"

    protected void DropDownSingleSelect_SelectedIndexChanged(object sender, EventArgs e)
    {
        hdnAddress.Value = addressElem.AddressID.ToString();
        // Enable edit address only if address selected
        btnEditAddress.Enabled = addressElem.AddressID != 0;
    }


    /// <summary>
    /// On btnOK button click.
    /// </summary>
    /// <param name="sender">Sender</param>
    /// <param name="e">Event argument</param>
    protected void btnOK_Click(object sender, EventArgs e)
    {
        // check 'EcommerceModify' permission
        if (!ECommerceContext.IsUserAuthorizedForPermission("ModifyOrders"))
        {
            RedirectToAccessDenied("CMS.Ecommerce", "EcommerceModify OR ModifyOrders");
        }

        string errorMessage = ValidateForm();

        if (errorMessage == "")
        {
            OrderInfo oi = OrderInfoProvider.GetOrderInfo(orderId);
            if (oi != null)
            {
                oi.OrderDate = orderDate.SelectedDateTime;
                oi.OrderNote = txtNotes.Text;
                oi.OrderStatusID = statusElem.OrderStatusID;
                oi.OrderCompanyAddressID = addressElem.AddressID;

                // update orderinfo
                OrderInfoProvider.SetOrderInfo(oi);

                URLHelper.Redirect("Order_Edit_General.aspx?orderid=" + Convert.ToString(oi.OrderID) + "&saved=1");
            }
            else
            {
                // Show error message
                ShowError(GetString("order_edit.ordernotexist"));
            }
        }

        // Show error message
        if (errorMessage != "")
        {
            // Show error message
            ShowError(errorMessage);
        }
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Initialize the form with order values.
    /// </summary>
    protected void InitializeForm(OrderInfo orderInfo)
    {
        lblOrderIdValue.Text = Convert.ToString(orderInfo.OrderID);
        orderDate.SelectedDateTime = orderInfo.OrderDate;
        lblInvoiceNumberValue.Text = HTMLHelper.HTMLEncode(Convert.ToString(orderInfo.OrderInvoiceNumber));
        txtNotes.Text = orderInfo.OrderNote;

        CustomerInfo ci = CustomerInfoProvider.GetCustomerInfo(customerId);
        if (ci != null)
        {
            lblCustomerName.Text = HTMLHelper.HTMLEncode(ci.CustomerFirstName + " " + ci.CustomerLastName);
        }

        statusElem.OrderStatusID = originalStatusId;
        addressElem.AddressID = originalCompanyAddressId;
    }


    /// <summary>
    /// Validates form input fields.
    /// </summary>
    protected string ValidateForm()
    {
        // Validate order date for emptiness
        string errorMessage = (orderDate.SelectedDateTime.CompareTo(DataHelper.DATETIME_NOT_SELECTED) == 0) ? GetString("order_edit.dateerr") : "";

        if (errorMessage == "")
        {
            if (!orderDate.IsValidRange())
            {
                errorMessage = GetString("general.errorinvaliddatetimerange");
            }

            // Validate order date for wrong format
            if (ValidationHelper.GetDateTime(orderDate.SelectedDateTime, DataHelper.DATETIME_NOT_SELECTED) == DataHelper.DATETIME_NOT_SELECTED)
            {
                errorMessage = GetString("order_edit.datewrongformat");
            }
            if ((originalCompanyAddressId > 0) && (errorMessage == ""))
            {
                if (addressElem.AddressID == 0)
                {
                    errorMessage = GetString("order_edit.emptycompanyaddress");
                }
            }
        }

        return errorMessage;
    }

    #endregion
}