using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.CMSHelper;
using CMS.Ecommerce;
using CMS.EcommerceProvider;
using CMS.GlobalHelper;
using CMS.SettingsProvider;
using CMS.SiteProvider;
using CMS.Helpers;
using CMS.Globalization;
using CMS.DataEngine;

public partial class CMSModules_Ecommerce_Controls_ShoppingCart_ShoppingCartOrderAddresses : ShoppingCartStep
{
    #region "ViewState Constants"

    // Constants for billing address
    private const string BILLING_ADDRESS_ID = "BillingAddressID";
    private const string BILLING_ADDRESS_NAME = "BillingAddressName";
    private const string BILLING_ADDRESS_LINE1 = "BillingAddressLine1";
    private const string BILLING_ADDRESS_LINE2 = "BillingAddressLine2";
    private const string BILLING_ADDRESS_CITY = "BillingAddressCity";
    private const string BILLING_ADDRESS_ZIP = "BillingAddressZIP";
    private const string BILLING_ADDRESS_COUNTRY_ID = "BillingAddressCountryID";
    private const string BILLING_ADDRESS_STATE_ID = "BillingAddressStateID";
    private const string BILLING_ADDRESS_PHONE = "BillingAddressPhone";

    // Constants for shipping address
    private const string SHIPPING_ADDRESS_CHECKED = "ShippingAddressChecked";
    private const string SHIPPING_ADDRESS_ID = "ShippingAddressID";
    private const string SHIPPING_ADDRESS_NAME = "ShippingAddressName";
    private const string SHIPPING_ADDRESS_LINE1 = "ShippingAddressLine1";
    private const string SHIPPING_ADDRESS_LINE2 = "ShippingAddressLine2";
    private const string SHIPPING_ADDRESS_CITY = "ShippingAddressCity";
    private const string SHIPPING_ADDRESS_ZIP = "ShippingAddressZIP";
    private const string SHIPPING_ADDRESS_COUNTRY_ID = "ShippingAddressCountryID";
    private const string SHIPPING_ADDRESS_STATE_ID = "ShippingAddressStateID";
    private const string SHIPPING_ADDRESS_PHONE = "ShippingAddressPhone";

    // Constants for company address
    private const string COMPANY_ADDRESS_CHECKED = "CompanyAddressChecked";
    private const string COMPANY_ADDRESS_ID = "CompanyAddressID";
    private const string COMPANY_ADDRESS_NAME = "CompanyAddressName";
    private const string COMPANY_ADDRESS_LINE1 = "CompanyAddressLine1";
    private const string COMPANY_ADDRESS_LINE2 = "CompanyAddressLine2";
    private const string COMPANY_ADDRESS_CITY = "CompanyAddressCity";
    private const string COMPANY_ADDRESS_ZIP = "CompanyAddressZIP";
    private const string COMPANY_ADDRESS_COUNTRY_ID = "CompanyAddressCountryID";
    private const string COMPANY_ADDRESS_STATE_ID = "CompanyAddressStateID";
    private const string COMPANY_ADDRESS_PHONE = "CompanyAddressPhone";

    #endregion


    #region "Temporary values operations"

    /// <summary>
    /// Removes billing address values from ShoppingCart ViewState.
    /// </summary>
    private void RemoveBillingTempValues()
    {
        // Billing address values
        ShoppingCartControl.SetTempValue(BILLING_ADDRESS_ID, null);
        ShoppingCartControl.SetTempValue(BILLING_ADDRESS_CITY, null);
        ShoppingCartControl.SetTempValue(BILLING_ADDRESS_COUNTRY_ID, null);
        ShoppingCartControl.SetTempValue(BILLING_ADDRESS_LINE1, null);
        ShoppingCartControl.SetTempValue(BILLING_ADDRESS_LINE2, null);
        ShoppingCartControl.SetTempValue(BILLING_ADDRESS_NAME, null);
        ShoppingCartControl.SetTempValue(BILLING_ADDRESS_PHONE, null);
        ShoppingCartControl.SetTempValue(BILLING_ADDRESS_STATE_ID, null);
        ShoppingCartControl.SetTempValue(BILLING_ADDRESS_ZIP, null);
    }


    /// <summary>
    /// Removes shipping address values from ShoppingCart ViewState.
    /// </summary>
    private void RemoveShippingTempValues()
    {
        // Shipping address values
        ShoppingCartControl.SetTempValue(SHIPPING_ADDRESS_CHECKED, null);
        ShoppingCartControl.SetTempValue(SHIPPING_ADDRESS_ID, null);
        ShoppingCartControl.SetTempValue(SHIPPING_ADDRESS_CITY, null);
        ShoppingCartControl.SetTempValue(SHIPPING_ADDRESS_COUNTRY_ID, null);
        ShoppingCartControl.SetTempValue(SHIPPING_ADDRESS_LINE1, null);
        ShoppingCartControl.SetTempValue(SHIPPING_ADDRESS_LINE2, null);
        ShoppingCartControl.SetTempValue(SHIPPING_ADDRESS_NAME, null);
        ShoppingCartControl.SetTempValue(SHIPPING_ADDRESS_PHONE, null);
        ShoppingCartControl.SetTempValue(SHIPPING_ADDRESS_STATE_ID, null);
        ShoppingCartControl.SetTempValue(SHIPPING_ADDRESS_ZIP, null);
    }


    /// <summary>
    /// Removes company address values from ShoppingCart ViewState.
    /// </summary>
    private void RemoveCompanyTempValues()
    {
        // Company address values
        ShoppingCartControl.SetTempValue(COMPANY_ADDRESS_CHECKED, null);
        ShoppingCartControl.SetTempValue(COMPANY_ADDRESS_ID, null);
        ShoppingCartControl.SetTempValue(COMPANY_ADDRESS_CITY, null);
        ShoppingCartControl.SetTempValue(COMPANY_ADDRESS_COUNTRY_ID, null);
        ShoppingCartControl.SetTempValue(COMPANY_ADDRESS_LINE1, null);
        ShoppingCartControl.SetTempValue(COMPANY_ADDRESS_LINE2, null);
        ShoppingCartControl.SetTempValue(COMPANY_ADDRESS_NAME, null);
        ShoppingCartControl.SetTempValue(COMPANY_ADDRESS_PHONE, null);
        ShoppingCartControl.SetTempValue(COMPANY_ADDRESS_STATE_ID, null);
        ShoppingCartControl.SetTempValue(COMPANY_ADDRESS_ZIP, null);
    }

    /// <summary>
    /// Loads shipping address temp values.
    /// </summary>
    private void LoadShippingFromViewState()
    {
        /*
        txtShippingName.Text = Convert.ToString(ShoppingCartControl.GetTempValue(SHIPPING_ADDRESS_NAME));
        txtShippingAddr1.Text = Convert.ToString(ShoppingCartControl.GetTempValue(SHIPPING_ADDRESS_LINE1));
        txtShippingAddr2.Text = Convert.ToString(ShoppingCartControl.GetTempValue(SHIPPING_ADDRESS_LINE2));
        txtShippingCity.Text = Convert.ToString(ShoppingCartControl.GetTempValue(SHIPPING_ADDRESS_CITY));
        txtShippingZip.Text = Convert.ToString(ShoppingCartControl.GetTempValue(SHIPPING_ADDRESS_ZIP));
        txtShippingPhone.Text = Convert.ToString(ShoppingCartControl.GetTempValue(SHIPPING_ADDRESS_PHONE));
        CountrySelector2.CountryID = ValidationHelper.GetInteger(ShoppingCartControl.GetTempValue(SHIPPING_ADDRESS_COUNTRY_ID), 0);
        CountrySelector2.StateID = ValidationHelper.GetInteger(ShoppingCartControl.GetTempValue(SHIPPING_ADDRESS_STATE_ID), 0);*/
    }

    /// <summary>
    /// Back button actions.
    /// </summary>
    public override void ButtonBackClickAction()
    {
        /*
        // Billing address values
        ShoppingCartControl.SetTempValue(BILLING_ADDRESS_ID, drpBillingAddr.SelectedValue);
        ShoppingCartControl.SetTempValue(BILLING_ADDRESS_CITY, txtBillingCity.Text);
        ShoppingCartControl.SetTempValue(BILLING_ADDRESS_COUNTRY_ID, CountrySelector1.CountryID);
        ShoppingCartControl.SetTempValue(BILLING_ADDRESS_LINE1, txtBillingAddr1.Text);
        ShoppingCartControl.SetTempValue(BILLING_ADDRESS_LINE2, txtBillingAddr2.Text);
        ShoppingCartControl.SetTempValue(BILLING_ADDRESS_NAME, txtBillingName.Text);
        ShoppingCartControl.SetTempValue(BILLING_ADDRESS_PHONE, txtBillingPhone.Text);
        ShoppingCartControl.SetTempValue(BILLING_ADDRESS_STATE_ID, CountrySelector1.StateID);
        ShoppingCartControl.SetTempValue(BILLING_ADDRESS_ZIP, txtBillingZip.Text);

        // Shipping address values
        ShoppingCartControl.SetTempValue(SHIPPING_ADDRESS_CHECKED, chkShippingAddr.Checked);
        ShoppingCartControl.SetTempValue(SHIPPING_ADDRESS_ID, drpShippingAddr.SelectedValue);
        ShoppingCartControl.SetTempValue(SHIPPING_ADDRESS_CITY, txtShippingCity.Text);
        ShoppingCartControl.SetTempValue(SHIPPING_ADDRESS_COUNTRY_ID, CountrySelector2.CountryID);
        ShoppingCartControl.SetTempValue(SHIPPING_ADDRESS_LINE1, txtShippingAddr1.Text);
        ShoppingCartControl.SetTempValue(SHIPPING_ADDRESS_LINE2, txtShippingAddr2.Text);
        ShoppingCartControl.SetTempValue(SHIPPING_ADDRESS_NAME, txtShippingName.Text);
        ShoppingCartControl.SetTempValue(SHIPPING_ADDRESS_PHONE, txtShippingPhone.Text);
        ShoppingCartControl.SetTempValue(SHIPPING_ADDRESS_STATE_ID, CountrySelector2.StateID);
        ShoppingCartControl.SetTempValue(SHIPPING_ADDRESS_ZIP, txtShippingZip.Text);

        // Company address values
        ShoppingCartControl.SetTempValue(COMPANY_ADDRESS_CHECKED, chkCompanyAddress.Checked);
        ShoppingCartControl.SetTempValue(COMPANY_ADDRESS_ID, drpCompanyAddress.SelectedValue);
        ShoppingCartControl.SetTempValue(COMPANY_ADDRESS_CITY, txtCompanyCity.Text);
        ShoppingCartControl.SetTempValue(COMPANY_ADDRESS_COUNTRY_ID, CountrySelector3.CountryID);
        ShoppingCartControl.SetTempValue(COMPANY_ADDRESS_LINE1, txtCompanyLine1.Text);
        ShoppingCartControl.SetTempValue(COMPANY_ADDRESS_LINE2, txtCompanyLine2.Text);
        ShoppingCartControl.SetTempValue(COMPANY_ADDRESS_NAME, txtCompanyName.Text);
        ShoppingCartControl.SetTempValue(COMPANY_ADDRESS_PHONE, txtCompanyPhone.Text);
        ShoppingCartControl.SetTempValue(COMPANY_ADDRESS_STATE_ID, CountrySelector3.StateID);
        ShoppingCartControl.SetTempValue(COMPANY_ADDRESS_ZIP, txtCompanyZip.Text);
        */
        base.ButtonBackClickAction();
    }

    #endregion


    /// <summary>
    /// Private properties.
    /// </summary>
    private int mCustomerId = 0;

    private SiteInfo mCurrentSite = null;
    /// <summary>
    /// Reloads the form data.
    /// </summary>
    /// <summary>
    /// Page load.
    /// </summary>
    protected void Page_Load(object sender, EventArgs e)
    {
        // *** SERVRANX START***
        ShowPaymentList();
        if (!rdbVisa.Checked && !rdbMaestro.Checked && !rdbMastercard.Checked)
        {
            rdbVisa.Checked = true;
            rdoBtn_CheckedChanged(null, null);
        }
        ddlPaymentOption.SelectedValue = SessionHelper.GetValue("PaymentID").ToString();
        ddlPaymentOption_SelectedIndexChanged(null, null);

        string where = string.Format("AddressCustomerID={0} AND AddressIsBilling=1", ECommerceContext.CurrentCustomer.CustomerID.ToString());
        string orderby = "AddressID";
        DataSet ds = AddressInfoProvider.GetAddresses(where, orderby);
        if (!DataHelper.DataSourceIsEmpty(ds))
        {
            AddressInfo ai = new AddressInfo(ds.Tables[0].Rows[0]);
            lblBillingAddressFullName.Text = ai.AddressPersonalName;
            lblBillingAddressStreet.Text = string.IsNullOrEmpty(ai.AddressLine2) ? ai.AddressLine1 : string.Format("{0}, {1}", ai.AddressLine1, ai.AddressLine2);
            lblBillingAddressZipCode.Text = ai.AddressZip;
            lblBillingAddressCityCountry.Text = string.Format("{0}, {1}", ai.AddressCity, CountryInfoProvider.GetCountryInfo(ai.AddressCountryID).CountryDisplayName);
            ShoppingCart.ShoppingCartBillingAddressID = ai.AddressID;
        }

        where = string.Format("AddressCustomerID={0} AND AddressIsShipping=1", ECommerceContext.CurrentCustomer.CustomerID.ToString());
        ds = AddressInfoProvider.GetAddresses(where, orderby);
        if (!DataHelper.DataSourceIsEmpty(ds))
        {
            AddressInfo ai = new AddressInfo(ds.Tables[0].Rows[0]);
            lblShippingAddressFullName.Text = ai.AddressPersonalName;
            lblShippingAddressStreet.Text = string.IsNullOrEmpty(ai.AddressLine2) ? ai.AddressLine1 : string.Format("{0}, {1}", ai.AddressLine1, ai.AddressLine2);
            lblShippingAddressZipCode.Text = ai.AddressZip;
            lblShippingAddressCityCountry.Text = string.Format("{0}, {1}", ai.AddressCity, CountryInfoProvider.GetCountryInfo(ai.AddressCountryID).CountryDisplayName);
            ShoppingCart.ShoppingCartShippingAddressID = ai.AddressID;
        }
        else
        {
            // NO SHIPPING ADDRESS DEFINED- PICK FIRST BILLING ADDRESS    
            AddressInfo ai_shipping = AddressInfoProvider.GetAddressInfo(ShoppingCart.ShoppingCartBillingAddressID);
            ai_shipping.AddressIsShipping = true;
            AddressInfoProvider.SetAddressInfo(ai_shipping);
            where = string.Format("AddressCustomerID={0} AND AddressIsShipping=1", ECommerceContext.CurrentCustomer.CustomerID.ToString());
            ds = AddressInfoProvider.GetAddresses(where, orderby);
            if (!DataHelper.DataSourceIsEmpty(ds))
            {
                AddressInfo ai = new AddressInfo(ds.Tables[0].Rows[0]);
                lblShippingAddressFullName.Text = ai.AddressPersonalName;
                lblShippingAddressStreet.Text = string.IsNullOrEmpty(ai.AddressLine2) ? ai.AddressLine1 : string.Format("{0}, {1}", ai.AddressLine1, ai.AddressLine2);
                lblShippingAddressZipCode.Text = ai.AddressZip;
                lblShippingAddressCityCountry.Text = string.Format("{0}, {1}", ai.AddressCity, CountryInfoProvider.GetCountryInfo(ai.AddressCountryID).CountryDisplayName);
                ShoppingCart.ShoppingCartShippingAddressID = ai.AddressID;
            }

        }
        ReloadData();
        // *** SERVRANX END***
        mCurrentSite = SiteContext.CurrentSite;


        //lblBillingTitle.Text = GetString("ShoppingCart.BillingAddress");
        //lblShippingTitle.Text = GetString("ShoppingCart.ShippingAddress");
        //lblCompanyAddressTitle.Text = GetString("ShoppingCart.CompanyAddress");

        // Initialize labels.
        // LabelInitialize();
        //this.TitleText = GetString("Order_new.ShoppingCartOrderAddresses.Title");

        // Get customer ID from ShoppingCartInfoObj
        mCustomerId = ShoppingCart.ShoppingCartCustomerID;


        // Get customer info.
        CustomerInfo ci = CustomerInfoProvider.GetCustomerInfo(mCustomerId);

        if (ci != null)
        {
            // Display customer addresses if customer is not anonymous
            if (ci.CustomerID > 0)
            {
                if (!ShoppingCartControl.IsCurrentStepPostBack)
                {
                    // Initialize customer billing and shipping addresses
                    InitializeAddresses();
                }
            }
        }

        // If shopping cart does not need shipping
        if (!ShippingOptionInfoProvider.IsShippingNeeded(ShoppingCart))
        {
            // Hide title
            lblBillingTitle.Visible = false;

            // Change current checkout process step caption
            ShoppingCartControl.CheckoutProcessSteps[ShoppingCartControl.CurrentStepIndex].Caption = GetString("order_new.shoppingcartorderaddresses.titlenoshipping");
        }
    }


    /// <summary>
    /// Initialize customer's addresses in billing and shipping dropdown lists.
    /// </summary>
    protected void InitializeAddresses()
    {
        // add new item <(new), 0>
        ListItem li = new ListItem(GetString("ShoppingCartOrderAddresses.NewAddress"), "0");
        li = new ListItem(GetString("ShoppingCartOrderAddresses.NewAddress"), "0");

        LoadBillingSelectedValue();

        // Try remove same shipping address as selected billing address
        // (if the selected value is not "0")
        LoadShippingSelectedValue();
        LoadCompanySelectedValue();

        LoadBillingAddressInfo();
        LoadShippingAddressInfo();
    }


    protected void LoadBillingSelectedValue()
    {
        try
        {
            int lastBillingAddressId = 0;

            // Get last used shipping and billing addresses in the order
            DataSet ds = OrderInfoProvider.GetOrders("OrderCustomerID=" + mCustomerId, "OrderDate DESC");
            if (!DataHelper.DataSourceIsEmpty(ds))
            {
                OrderInfo oi = new OrderInfo(ds.Tables[0].Rows[0]);
                lastBillingAddressId = oi.OrderBillingAddressID;
            }
        }
        catch
        {
        }
    }


    protected void LoadShippingSelectedValue()
    {
        try
        {
            int lastShippingAddressId = 0;

            // Get last used shipping and billing addresses in the order
            DataSet ds = OrderInfoProvider.GetOrders("OrderCustomerID=" + mCustomerId, "OrderDate DESC");
            if (!DataHelper.DataSourceIsEmpty(ds))
            {
                OrderInfo oi = new OrderInfo(ds.Tables[0].Rows[0]);
                lastShippingAddressId = oi.OrderShippingAddressID;
            }

            // Try to select shipping address from ViewState first
            object viewStateValue = ShoppingCartControl.GetTempValue(SHIPPING_ADDRESS_ID);
            bool viewStateChecked = ValidationHelper.GetBoolean(ShoppingCartControl.GetTempValue(SHIPPING_ADDRESS_CHECKED), false);
        }
        catch
        {
        }
    }


    protected void LoadCompanySelectedValue()
    {
        try
        {
            int lastCompanyAddressId = 0;

            // Get last used shipping and billing addresses in the order
            DataSet ds = OrderInfoProvider.GetOrders("OrderCustomerID=" + mCustomerId, "OrderDate DESC");
            if (!DataHelper.DataSourceIsEmpty(ds))
            {
                OrderInfo oi = new OrderInfo(ds.Tables[0].Rows[0]);
                lastCompanyAddressId = oi.OrderCompanyAddressID;
            }

            // Try to select company address from ViewState first
            object viewStateValue = ShoppingCartControl.GetTempValue(COMPANY_ADDRESS_ID);
            bool viewStateChecked = ValidationHelper.GetBoolean(ShoppingCartControl.GetTempValue(COMPANY_ADDRESS_CHECKED), false);
        }
        catch
        {
        }
    }

    /// <summary>
    /// On drpBillingAddr selected index changed.
    /// </summary>
    private void drpBillingAddr_SelectedIndexChanged(object sender, EventArgs e)
    {
        LoadBillingAddressInfo();
    }

    /// <summary>
    /// Clean specified part of the form.
    /// </summary>    
    private void CleanForm(bool billing, bool shipping, bool company)
    {
        int defaultCountryId = 0;
        int defaultStateId = 0;

        // Prefill country from customer if any
        if ((ShoppingCart != null) && (ShoppingCart.Customer != null))
        {
            defaultCountryId = ShoppingCart.Customer.CustomerCountryID;
            defaultStateId = ShoppingCart.Customer.CustomerStateID;
        }

        // Prefill default store country if customers country not found
        if ((defaultCountryId <= 0) && (SiteContext.CurrentSite != null))
        {
            string countryName = ECommerceSettings.DefaultCountryName(SiteContext.CurrentSite.SiteName);
            CountryInfo ci = CountryInfoProvider.GetCountryInfo(countryName);
            defaultCountryId = (ci != null) ? ci.CountryID : 0;
            defaultStateId = 0;
        }
    }


    /// <summary>
    /// Loads selected billing  address info.
    /// </summary>
    protected void LoadBillingAddressInfo()
    {
        /*
        // Try to select company address from ViewState first
        if (!ShoppingCartControl.IsCurrentStepPostBack && ShoppingCartControl.GetTempValue(BILLING_ADDRESS_ID) != null)
        {
            // LoadBillingFromViewState();
        }
        else
        {
            int addressId = 0;

            if (drpBillingAddr.SelectedValue != "0")
            {
                addressId = Convert.ToInt32(drpBillingAddr.SelectedValue);
            }
            else
            {
                // Clean billing part of the form
                CleanForm(true, false, false);
            }
        }*/
    }


    /// <summary>
    /// Loads selected shipping  address info.
    /// </summary>
    protected void LoadShippingAddressInfo()
    {
        /*
        int addressId = 0;

        // Load shipping info only if shipping part is visible
        if (plhShipping.Visible)
        {
            // Try to select company address from ViewState first
            if (!ShoppingCartControl.IsCurrentStepPostBack && ShoppingCartControl.GetTempValue(SHIPPING_ADDRESS_ID) != null)
            {
                LoadShippingFromViewState();
            }
            else
            {
                if (drpShippingAddr.SelectedValue != "0")
                {
                    addressId = Convert.ToInt32(drpShippingAddr.SelectedValue);
                }
                else
                {
                    // clean shipping part of the form
                    CleanForm(false, true, false);
                }
            }
        }*/
    }


    /// <summary>
    /// Check if the form is well filled.
    /// </summary>
    /// <returns>True or false.</returns>
    public override bool IsValid()
    {
        Validator val = new Validator();

        return true;
    }


    /// <summary>
    /// Process valid values of this step.
    /// </summary>
    public override bool ProcessStep()
    {
        // AddressInfo ai = null;
        bool newAddress = false;
        if (mCustomerId > 0)
        {
            // Clean the viewstate
            RemoveBillingTempValues();
            RemoveShippingTempValues();
            RemoveCompanyTempValues();

            // Process billing address
            /*if (ai == null)
            {
                ai = new AddressInfo();
                newAddress = true;
            }

            if (newAddress)
            {
                ai.AddressIsBilling = true;
                ai.AddressEnabled = true;
            }
            ai.AddressCustomerID = mCustomerId;
            ai.AddressName = AddressInfoProvider.GetAddressName(ai);
            
            // Save address and set it's ID to ShoppingCartInfoObj
            AddressInfoProvider.SetAddressInfo(ai);*/

            // Update current contact's address
            ModuleCommands.OnlineMarketingMapAddress(AddressInfoProvider.GetAddressInfo(ShoppingCart.ShoppingCartBillingAddressID), ContactID);

            // If shopping cart does not need shipping
            if (!ShippingOptionInfoProvider.IsShippingNeeded(ShoppingCart))
            {
                ShoppingCart.ShoppingCartShippingAddressID = 0;
            }
            // If shipping address is different from billing address
            /*
            else if (chkShippingAddr.Checked)
            {

                newAddress = false;
                // Process shipping address
                //-------------------------
                if (ai == null)
                {
                    ai = new AddressInfo();
                    newAddress = true;
                }

                if (newAddress)
                {
                    ai.AddressIsShipping = true;
                    ai.AddressEnabled = true;
                    ai.AddressIsBilling = false;
                    ai.AddressIsCompany = false;
                    ai.AddressEnabled = true;
                }
                ai.AddressCustomerID = mCustomerId;
                ai.AddressName = AddressInfoProvider.GetAddressName(ai);

                // Save address and set it's ID to ShoppingCartInfoObj
                AddressInfoProvider.SetAddressInfo(ai);
                ShoppingCart.ShoppingCartShippingAddressID = ai.AddressID;
            }
            // Shipping address is the same as billing address
            else
            {
                ShoppingCart.ShoppingCartShippingAddressID = ShoppingCart.ShoppingCartBillingAddressID;
            }*/

            try
            {
                // Update changes in database only when on the live site
                if (!ShoppingCartControl.IsInternalOrder)
                {
                    ShoppingCartInfoProvider.SetShoppingCartInfo(ShoppingCart);
                }
                return true;
            }
            catch (Exception ex)
            {
                // Show error message
                lblError.Visible = true;
                lblError.Text = ex.Message;
                return false;
            }
        }
        else
        {
            lblError.Visible = true;
            lblError.Text = GetString("Ecommerce.NoCustomerSelected");
            return false;
        }
    }


    protected override void Render(HtmlTextWriter writer)
    {
        if (!ShoppingCartControl.IsCurrentStepPostBack)
        {
            // Load values
            LoadShippingSelectedValue();
            LoadBillingSelectedValue();
            LoadCompanySelectedValue();
        }
        base.Render(writer);
    }

    protected void RptPickBillingAddressItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        var drv = (System.Data.DataRowView)e.Item.DataItem;
        if (drv != null)
        {
            int addressID = ValidationHelper.GetInteger(drv["AddressID"], 0);
            if (addressID > 0)
            {
                AddressInfo ai = AddressInfoProvider.GetAddressInfo(addressID);
                var ltlAddress = e.Item.FindControl("ltlAddress") as Literal;
                if (ltlAddress != null)
                {
                    string addressline, addresstown;
                    addressline = string.IsNullOrEmpty(ai.AddressLine2) ? ai.AddressLine1 : string.Format("{0} {1}", ai.AddressLine1, ai.AddressLine2);
                    addresstown = string.Format("{0} {1}", ai.AddressZip, ai.AddressCity);
                    ltlAddress.Text = string.Format("{0}, {1} - {2}, {3}", ai.AddressPersonalName, addressline, addresstown, CountryInfoProvider.GetCountryInfo(ai.AddressCountryID).CountryDisplayName);
                }
            }
        }
    }

    protected void RptPickShippingAddressItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        var drv = (System.Data.DataRowView)e.Item.DataItem;
        if (drv != null)
        {
            int addressID = ValidationHelper.GetInteger(drv["AddressID"], 0);
            if (addressID > 0)
            {
                AddressInfo ai = AddressInfoProvider.GetAddressInfo(addressID);
                var ltlAddress = e.Item.FindControl("ltlAddress") as Literal;
                if (ltlAddress != null)
                {
                    string addressline, addresstown;
                    addressline = string.IsNullOrEmpty(ai.AddressLine2) ? ai.AddressLine1 : string.Format("{0} {1}", ai.AddressLine1, ai.AddressLine2);
                    addresstown = string.Format("{0} {1}", ai.AddressZip, ai.AddressCity);
                    ltlAddress.Text = string.Format("{0}, {1} - {2}, {3}", ai.AddressPersonalName, addressline, addresstown, CountryInfoProvider.GetCountryInfo(ai.AddressCountryID).CountryDisplayName);
                }
            }
        }
    }


    protected void RptCartItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        var drv = (System.Data.DataRowView)e.Item.DataItem;
        if (drv != null)
        {
            int currentSKUID = ValidationHelper.GetInteger(drv["SKUID"], 0);
            if (currentSKUID > 0)
            {
                SKUInfo sku = SKUInfoProvider.GetSKUInfo(currentSKUID);
                if (sku != null)
                {
                    int subTotal = 0;
					double remise = 0;
                    //Display product image
                    var ltlProductImage = e.Item.FindControl("ltlProductImage") as Literal;
                    if (ltlProductImage != null)
                        //<%--# EcommerceFunctions.GetProductImage(Eval("SKUImagePath"), Eval("SKUName"))--%>
                        ltlProductImage.Text = EcommerceFunctions.GetProductImage(sku.SKUImagePath, sku.SKUName);

                    var ltlProductName = e.Item.FindControl("ltlProductName") as Literal;
                    if (ltlProductName != null)
                        ltlProductName.Text = sku.SKUName;

                    var txtProductCount = e.Item.FindControl("txtProductCount") as TextBox;
                    if (txtProductCount != null)
                    {
                        foreach (ShoppingCartItemInfo shoppingCartItem in ShoppingCart.CartItems)
                        {
                            if (shoppingCartItem.SKUID == sku.SKUID)
                            {
							    remise = shoppingCartItem.UnitTotalDiscount;
                                txtProductCount.Text = shoppingCartItem.CartItemUnits.ToString();
                                subTotal = shoppingCartItem.CartItemUnits;
                                break;
                            }
                        }
                    }

                    var ltlProductPrice = e.Item.FindControl("ltlProductPrice") as Literal;
                    if (ltlProductPrice != null)
                    {
                        //ltlProductPrice.Text = (sku.SKUPrice * subTotal).ToString();
                        ltlProductPrice.Text = EcommerceFunctions.GetFormatedPrice((sku.SKUPrice - remise) * subTotal, sku.SKUDepartmentID, sku.SKUID);

                        //ltlProductPrice.Text = string.Format("{0} <em>�</em>", CurrencyInfoProvider.GetFormattedValue(sku.SKUPrice * subTotal, ShoppingCart.Currency).ToString());
                        ltlProductPrice.Text = string.Format("{0}<em>{1}</em>", ltlProductPrice.Text.Substring(0, ltlProductPrice.Text.Length - 1).Trim(), ltlProductPrice.Text.Substring(ltlProductPrice.Text.Length - 1, 1).Trim());
                    }
                }
            }
        }
    }

    protected void RptPickBillingAddressItemCommand(object sender, RepeaterItemEventArgs e)
    {
    }
    protected void RptCartItemCommand(object source, RepeaterCommandEventArgs e)
    {
        if (e.CommandName.Equals("Remove"))
        {
            var cartItemGuid = new Guid(e.CommandArgument.ToString());
            // Remove product and its product option from list
            this.ShoppingCart.RemoveShoppingCartItem(cartItemGuid);

            if (!this.ShoppingCartControl.IsInternalOrder)
            {
                // Delete product from database
                ShoppingCartItemInfoProvider.DeleteShoppingCartItemInfo(cartItemGuid);
            }
            ReloadData();
        }
        if (e.CommandName.Equals("Decrease"))
        {
            var cartItemGuid = new Guid(e.CommandArgument.ToString());
            ShoppingCartItemInfo cartItem = ShoppingCart.GetShoppingCartItem(cartItemGuid);
            if (cartItem != null)
            {
                if (cartItem.CartItemUnits - 1 > 0)
                {
                    cartItem.CartItemUnits--;
                    // Update units of child bundle items
                    foreach (ShoppingCartItemInfo bundleItem in cartItem.BundleItems)
                    {
                        bundleItem.CartItemUnits--;
                    }

                    if (!ShoppingCartControl.IsInternalOrder)
                    {
                        ShoppingCartItemInfoProvider.SetShoppingCartItemInfo(cartItem);

                        // Update product options in database
                        foreach (ShoppingCartItemInfo option in cartItem.ProductOptions)
                        {
                            ShoppingCartItemInfoProvider.SetShoppingCartItemInfo(option);
                        }
                    }
                    ReloadData();
                }
            }
        }
        if (e.CommandName.Equals("Increase"))
        {
            var cartItemGuid = new Guid(e.CommandArgument.ToString());
            ShoppingCartItemInfo cartItem = ShoppingCart.GetShoppingCartItem(cartItemGuid);
            if (cartItem != null)
            {
                if (cartItem.CartItemUnits + 1 > 0)
                {
                    cartItem.CartItemUnits++;
                    // Update units of child bundle items
                    foreach (ShoppingCartItemInfo bundleItem in cartItem.BundleItems)
                    {
                        bundleItem.CartItemUnits++;
                    }

                    if (!ShoppingCartControl.IsInternalOrder)
                    {
                        ShoppingCartItemInfoProvider.SetShoppingCartItemInfo(cartItem);

                        // Update product options in database
                        foreach (ShoppingCartItemInfo option in cartItem.ProductOptions)
                        {
                            ShoppingCartItemInfoProvider.SetShoppingCartItemInfo(option);
                        }
                    }
                    ReloadData();
                }
            }
        }
    }

    private void ReloadBillingAdresses()
    {
        //RptPickBillingAddress
        string where = string.Format("AddressCustomerID={0} AND AddressIsBilling=1", ECommerceContext.CurrentCustomer.CustomerID.ToString());
        string orderby = "AddressID";
        DataSet ds = AddressInfoProvider.GetAddresses(where, orderby);
        if (!DataHelper.DataSourceIsEmpty(ds))
        {
        }
        RptPickBillingAddress.DataSource = ds;
        RptPickBillingAddress.DataBind();
    }

    private void ReloadShippingAdresses()
    {
        //RptPickShippingAddress
        string where = string.Format("AddressCustomerID={0} AND AddressIsShipping=1", ECommerceContext.CurrentCustomer.CustomerID.ToString());
        string orderby = "AddressID";
        DataSet ds = AddressInfoProvider.GetAddresses(where, orderby);
        if (!DataHelper.DataSourceIsEmpty(ds))
        {
        }
        RptPickShippingAddress.DataSource = ds;
        RptPickShippingAddress.DataBind();
    }

    /// <summary>
    /// Reloads the form data.
    /// </summary>
    protected void ReloadData()
    {
        rptCart.DataSource = ShoppingCart.ContentTable;

        // Hide coupon placeholder when no coupons are defined
        // ***  plcCoupon.Visible = AreDiscountCouponsAvailableOnSite();

        // Bind data
        rptCart.DataBind();
        ReloadBillingAdresses();
        ReloadShippingAdresses();

        if (!DataHelper.DataSourceIsEmpty(ShoppingCart.ContentTable))
        {
            // Display total price
            DisplayTotalPrice();

            // Display/hide discount column
            //gridData.Columns[9].Visible = ShoppingCart.IsDiscountApplied;
        }
        else
        {
            // Hide some items
            // ***  HideCartContentWhenEmpty();
        }

        if (!ShippingOptionInfoProvider.IsShippingNeeded(ShoppingCart))
        {
            // ***  plcShippingPrice.Visible = false;
        }
    }
    // Displays PaymentList
    private void ShowPaymentList()
    {
        DataSet ds = PaymentOptionInfoProvider.GetPaymentOptions(CurrentSite.SiteID, true);
        if (!DataHelper.DataSourceIsEmpty(ds))
        {
            ddlPaymentOption.DataSource = ds;
            ddlPaymentOption.DataTextField = "PaymentOptionDisplayName";
            ddlPaymentOption.DataValueField = "PaymentOptionId";
            ddlPaymentOption.DataBind();
            ddlPaymentOption.SelectedValue = ValidationHelper.GetString(SessionHelper.GetValue("PaymentID"), string.Empty);
            ddlPaymentOption_SelectedIndexChanged(null, null);
        }
    }

    // Displays total price
    protected void DisplayTotalPrice()
    {
        //Error
       // ShippingExtendedInfoProvider.RecalculateExtendedShipping(ShoppingCart);
        lblTotalPriceValue.Text = string.Format("{0} <em>�</em>", CurrencyInfoProvider.GetFormattedValue(ShoppingCart.RoundedTotalPrice, ShoppingCart.Currency).ToString());
        lblShippingPriceValue.Text = string.Format("{0} <em>�</em>", CurrencyInfoProvider.GetFormattedValue(ShoppingCart.TotalShipping, ShoppingCart.Currency).ToString());
        double bulkPrice = ShoppingCart.RoundedTotalPrice - ShoppingCart.TotalShipping;
        lblMontantAchat.Text = string.Format("{0} <em>�</em>", CurrencyInfoProvider.GetFormattedValue(bulkPrice, ShoppingCart.Currency).ToString());
        btn_valid_order.Visible = (ShoppingCart.TotalShipping > 0);
    }

    protected void ddlPaymentOption_SelectedIndexChanged(object sender, EventArgs e)
    {
        string PaymentID = ValidationHelper.GetString(ddlPaymentOption.SelectedValue, "");
        SessionHelper.SetValue("PaymentID", PaymentID);
        pnlPaymentOption.Visible = CheckPaymentIsGateway(PaymentID);
        lblPayment.Text = ddlPaymentOption.SelectedItem.Text;
    }

    protected void rdoBtn_CheckedChanged(object sender, EventArgs e)
    {
        if (rdbVisa.Checked) this.ShoppingCart.PaymentGatewayCustomData["BRAND"] = "VISA";
        if (rdbMastercard.Checked) this.ShoppingCart.PaymentGatewayCustomData["BRAND"] = "MasterCard";
        if (rdbMaestro.Checked) this.ShoppingCart.PaymentGatewayCustomData["BRAND"] = "Maestro";
        lblCardName.Text = this.ShoppingCart.PaymentGatewayCustomData["BRAND"].ToString();
    }

    protected void btn_Valid_Order_Click(object sender, EventArgs e)
    {
        ButtonNextClickAction();
        ButtonNextClickAction();
    }

    private bool CheckPaymentIsGateway(string paymentID)
    {
        bool result = false;
        int paymentid = int.Parse(paymentID); 
        PaymentOptionInfo payment = PaymentOptionInfoProvider.GetPaymentOptionInfo(paymentid);
        result = string.Equals(payment.PaymentOptionDescription, "PAYMENTGATEWAY");
        return result;
    }
}