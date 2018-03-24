using System;
using System.Data;
using System.Collections;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.CMSHelper;
using CMS.Ecommerce;
using CMS.EcommerceProvider;
using CMS.GlobalHelper;
using CMS.SiteProvider;
using CMS.WebAnalytics;
using CMS.EventLog;
using CMS.Globalization;
using CMS.Localization;
using CMS.Helpers;
using CMS.DocumentEngine;

public partial class CMSModules_Ecommerce_Controls_ShoppingCart_ShoppingCartPreview : ShoppingCartStep
{
    #region "ViewState Constants"

    private const string ORDER_NOTE = "OrderNote";

    #endregion


    private SiteInfo currentSite = null;
    private int mAddressCount = 1;
	private bool? mIsShippingNeeded = null;
	
	#region "Properties"
	protected bool IsShippingNeeded
    {
        get
        {
            if (mIsShippingNeeded.HasValue)
            {
                return mIsShippingNeeded.Value;
            }
            else
            {
                if (ShoppingCart != null)
                {
                    // Figure out from shopping cart
                    mIsShippingNeeded = ShippingOptionInfoProvider.IsShippingNeeded(ShoppingCart);
                    return mIsShippingNeeded.Value;
                }
                else
                {
                    return true;
                }
            }
        }
    }
	#endregion


    protected void Page_Load(object sender, EventArgs e)
    {
        currentSite = SiteContext.CurrentSite;

        lblTitle.Text = GetString("ShoppingCartPreview.Title");

        if ((ShoppingCart != null) && (ShoppingCart.CountryID == 0) && (currentSite != null))
        {
            string countryName = ECommerceSettings.DefaultCountryName(currentSite.SiteName);
            CountryInfo ci = CountryInfoProvider.GetCountryInfo(countryName);
            ShoppingCart.CountryID = (ci != null) ? ci.CountryID : 0;
        }

        ShoppingCartControl.ButtonNext.Text = GetString("Ecommerce.OrderPreview.NextButtonText");

        // Addresses initialization
        pnlBillingAddress.GroupingText = GetString("Ecommerce.CartPreview.BillingAddressPanel");
        if(IsShippingNeeded) pnlShippingAddress.GroupingText = GetString("Ecommerce.CartPreview.ShippingAddressPanel");
        pnlCompanyAddress.GroupingText = GetString("Ecommerce.CartPreview.CompanyAddressPanel");

        FillBillingAddressForm(ShoppingCart.ShoppingCartBillingAddressID);
        if(IsShippingNeeded) FillShippingAddressForm(ShoppingCart.ShoppingCartShippingAddressID);

        // Load company address
        if (ShoppingCart.ShoppingCartCompanyAddressID > 0)
        {
            lblCompany.Text = OrderInfoProvider.GetAddress(ShoppingCart.ShoppingCartCompanyAddressID);
            mAddressCount++;
            tdCompanyAddress.Visible = true;
        }
        else
        {
            tdCompanyAddress.Visible = false;
        }

        // Enable sending order notifications when creating order from CMSDesk
        if ((ShoppingCartControl.CheckoutProcessType == CheckoutProcessEnum.CMSDeskOrder) ||
            ShoppingCartControl.CheckoutProcessType == CheckoutProcessEnum.CMSDeskCustomer)
        {
            chkSendEmail.Visible = true;
            chkSendEmail.Checked = ECommerceSettings.SendOrderNotification(currentSite.SiteName);
            chkSendEmail.Text = GetString("ShoppingCartPreview.SendNotification");
        }

        // Show tax registration ID and organization ID
        InitializeIDs();

        // shopping cart content table initialization
        gridData.Columns[4].HeaderText = GetString("Ecommerce.ShoppingCartContent.SKUName");
        gridData.Columns[5].HeaderText = GetString("Ecommerce.ShoppingCartContent.SKUUnits");
        gridData.Columns[6].HeaderText = GetString("Ecommerce.ShoppingCartContent.UnitPrice");
        gridData.Columns[7].HeaderText = GetString("Ecommerce.ShoppingCartContent.UnitDiscount");
        gridData.Columns[8].HeaderText = GetString("Ecommerce.ShoppingCartContent.Tax");
        gridData.Columns[9].HeaderText = GetString("Ecommerce.ShoppingCartContent.Subtotal");

        // Product tax summary table initialiazation
        gridTaxSummary.Columns[0].HeaderText = GetString("Ecommerce.CartContent.TaxDisplayName");
        gridTaxSummary.Columns[1].HeaderText = GetString("Ecommerce.CartContent.TaxSummary");

        // Shipping tax summary table initialiazation
        gridShippingTaxSummary.Columns[0].HeaderText = GetString("com.CartContent.ShippingTaxDisplayName");
        gridShippingTaxSummary.Columns[1].HeaderText = GetString("Ecommerce.CartContent.TaxSummary");

        ReloadData();

        // Order note initialization
        lblNote.Text = GetString("ecommerce.cartcontent.notelabel");
        if (!ShoppingCartControl.IsCurrentStepPostBack)
        {
            // Try to select payment from ViewState first
            object viewStateValue = ShoppingCartControl.GetTempValue(ORDER_NOTE);
            if (viewStateValue != null)
            {
                txtNote.Text = Convert.ToString(viewStateValue);
            }
            else
            {
                txtNote.Text = ShoppingCart.ShoppingCartNote;
            }
        }

        // Display/Hide column with applied discount
        gridData.Columns[7].Visible = ShoppingCart.IsDiscountApplied;

        if (mAddressCount == 2)
        {
            tblAddressPreview.Attributes["class"] = "AddressPreviewWithTwoColumns";
        }
        else if (mAddressCount == 3)
        {
            tblAddressPreview.Attributes["class"] = "AddressPreviewWithThreeColumns";
        }
        ButtonNextClickAction();
    }


    protected void Page_Prerender(object sender, EventArgs e)
    {
        // Hide columns with identifiers
        gridData.Columns[0].Visible = false;
        gridData.Columns[1].Visible = false;
        gridData.Columns[2].Visible = false;
        gridData.Columns[3].Visible = false;

        // Disable default button in the order preview to 
        // force approvement of the order by mouse click
        if (ShoppingCartControl.ShoppingCartContainer != null)
        {
            ShoppingCartControl.ShoppingCartContainer.DefaultButton = "";
        }

        // Display/hide error message
        lblError.Visible = !string.IsNullOrEmpty(lblError.Text.Trim());
    }


    protected void ReloadData()
    {
        gridData.DataSource = ShoppingCart.ContentTable;
        gridData.DataBind();

        gridTaxSummary.DataSource = ShoppingCart.ContentTaxesTable;
        gridTaxSummary.DataBind();

		if(IsShippingNeeded) {
			gridShippingTaxSummary.DataSource = ShoppingCart.ShippingTaxesTable;
			gridShippingTaxSummary.DataBind();
		}

        // shipping option, payment method initialization
        InitPaymentShipping();
    }


    /// <summary>
    /// Fills billing address form.
    /// </summary>
    /// <param name="addressId">Billing address id</param>
    protected void FillBillingAddressForm(int addressId)
    {
        lblBill.Text = OrderInfoProvider.GetAddress(addressId);
    }


    /// <summary>
    /// Fills shipping address form.
    /// </summary>
    /// <param name="addressId">Shipping address id</param>
    protected void FillShippingAddressForm(int addressId)
    {
        lblShip.Text = OrderInfoProvider.GetAddress(addressId);
    }


    /// <summary>
    /// Back button actions.
    /// </summary>
    public override void ButtonBackClickAction()
    {
        // Save the values to ShoppingCart ViewState
        ShoppingCartControl.SetTempValue(ORDER_NOTE, txtNote.Text);

        base.ButtonBackClickAction();
    }


    /// <summary>
    /// Validates shopping cart content.
    /// </summary>    
    public override bool IsValid()
    {
        // Force loading current values         
        ShoppingCartInfoProvider.EvaluateShoppingCart(ShoppingCart);

        // Check inventory
        ShoppingCartCheckResult checkResult = ShoppingCartInfoProvider.CheckShoppingCart(ShoppingCart);

        if (checkResult.CheckFailed)
        {
            lblError.Text = checkResult.GetHTMLFormattedMessage();

            return false;
        }

        return true;
    }


    /// <summary>
    /// Saves order information from ShoppingCartInfo object to database as new order.
    /// </summary>
    public override bool ProcessStep()
    {
        // Load first step if there is no address
        if (IsShippingNeeded && ShoppingCart.ShoppingCartBillingAddressID <= 0)
        {
            ShoppingCartControl.LoadStep(0);
            return false;
        }

        // Check if customer is enabled
        if ((ShoppingCart.Customer != null) && (!ShoppingCart.Customer.CustomerEnabled))
        {
            lblError.Text = GetString("ecommerce.cartcontent.customerdisabled");
            return false;
        }

        // Deal with order note
        ShoppingCartControl.SetTempValue(ORDER_NOTE, null);
        ShoppingCart.ShoppingCartNote = txtNote.Text.Trim();

        try
        {
            // Set order culture
            ShoppingCart.ShoppingCartCulture = LocalizationContext.PreferredCultureCode;

            // Update customer preferences
            CustomerInfoProvider.SetCustomerPreferredSettings(ShoppingCart);

            // Create order
            ShoppingCartInfoProvider.SetOrder(ShoppingCart);
            ShoppingCart.Order.SetValue("OrderBundleData", ShoppingCart.GetStringValue("ShoppingCartBundleData", string.Empty)); 
        }
        catch (Exception ex)
        {
            // Show error
            lblError.Text = GetString("Ecommerce.OrderPreview.ErrorOrderSave");

            // Log exception
            EventLogProvider.LogException("Shopping cart", "SAVEORDER", ex, ShoppingCart.ShoppingCartSiteID, null);
            return false;
        }

        // Track order items conversions
        ECommerceHelper.TrackOrderItemsConversions(ShoppingCart);

        // Track order conversion        
        string name = ShoppingCartControl.OrderTrackConversionName;
        ECommerceHelper.TrackOrderConversion(ShoppingCart, name);

        // Track order activity
        string siteName = SiteContext.CurrentSiteName;

        if (LogActivityForCustomer)
        {
            ShoppingCartControl.TrackActivityPurchasedProducts(ShoppingCart, siteName, ContactID);
            ShoppingCartControl.TrackActivityPurchase(ShoppingCart.OrderId, ContactID,
                                                      SiteContext.CurrentSiteName, RequestContext.CurrentRelativePath,
                                                      ShoppingCart.TotalPriceInMainCurrency, CurrencyInfoProvider.GetFormattedPrice(ShoppingCart.TotalPriceInMainCurrency,
                                                                                                                                           CurrencyInfoProvider.GetMainCurrency(CMSContext.CurrentSiteID)));
        }

        // Raise finish order event
        ShoppingCartControl.RaiseOrderCompletedEvent();
        string currentCultureCode = DocumentContext.CurrentDocument.DocumentCulture;
        string template = "Ecommerce.OrderNotificationToCustomer" + currentCultureCode;
        // When in CMSDesk
        if (ShoppingCartControl.IsInternalOrder)
        {
            if (chkSendEmail.Checked)
            {
                // Send order notification emails
                OrderInfoProvider.SendOrderNotificationToAdministrator(ShoppingCart);
               
                
                OrderInfoProvider.SendOrderNotificationToCustomer(ShoppingCart,template);
            }
        }
        // When on the live site
        else if (ECommerceSettings.SendOrderNotification(SiteContext.CurrentSite.SiteName))
        {
            // Send order notification emails
            OrderInfoProvider.SendOrderNotificationToAdministrator(ShoppingCart);
          
            
            OrderInfoProvider.SendOrderNotificationToCustomer(ShoppingCart, template);
        }

        /*desactiver le coupon code*/
        if (Session["CouponValue"] != null)
        {
            DiscountCouponInfo dci = DiscountCouponInfoProvider.GetDiscountCouponInfo(Session["CouponValue"].ToString(), ShoppingCart.SiteName);
            if ((dci != null) && (((ShoppingCart.IsCreatedFromOrder) && (ShoppingCart.ShoppingCartDiscountCouponID == dci.DiscountCouponID)) || dci.IsValid))
            {
                DateTime d = DateTime.Now;
                d = d.AddMinutes(5);
                dci.DiscountCouponValidTo = d;
                Session["CouponValue"] = null;
            }
        }
        /**/

       return true;
    }


    protected void InitPaymentShipping()
    {
        // shipping option and payment method
        lblShippingOption.Text = GetString("Ecommerce.CartContent.ShippingOption");
        lblPaymentMethod.Text = GetString("Ecommerce.CartContent.PaymentMethod");
        lblShipping.Text = GetString("Ecommerce.CartContent.Shipping");

        if (IsShippingNeeded && currentSite != null)
        {
            // get shipping option name
            ShippingOptionInfo shippingObj = ShoppingCart.ShippingOption;
            if (shippingObj != null)
            {
                mAddressCount++;
                //plcShippingAddress.Visible = true;
                tdShippingAddress.Visible = true;
                plcShipping.Visible = true;
                plcShippingOption.Visible = true;
                lblShippingOptionValue.Text = HTMLHelper.HTMLEncode(shippingObj.ShippingOptionDisplayName);
                lblShippingValue.Text = CurrencyInfoProvider.GetFormattedPrice(ShoppingCart.TotalShipping, ShoppingCart.Currency);
            }
            else
            {
                //plcShippingAddress.Visible = false;
                tdShippingAddress.Visible = false;
                plcShippingOption.Visible = false;
                lblShippingValue.Text = CurrencyInfoProvider.GetFormattedPrice(ShoppingCart.TotalShipping, ShoppingCart.Currency);
                //plcShipping.Visible = false;
            }
        }

        // get payment method name
        PaymentOptionInfo paymentObj = PaymentOptionInfoProvider.GetPaymentOptionInfo(ShoppingCart.ShoppingCartPaymentOptionID);
        if (paymentObj != null)
        {
            lblPaymentMethodValue.Text = HTMLHelper.HTMLEncode(paymentObj.PaymentOptionDisplayName);
        }


        // total price initialization
        lblTotalPrice.Text = GetString("ecommerce.cartcontent.totalprice");
        lblTotalPriceValue.Text = CurrencyInfoProvider.GetFormattedPrice(ShoppingCart.RoundedTotalPrice, ShoppingCart.Currency);
    }


    /// <summary>
    /// Displays product error message in shopping cart content table.
    /// </summary>
    /// <param name="skuErrorMessage">Error message to be displayed</param>
    protected string DisplaySKUErrorMessage(object skuErrorMessage)
    {
        string err = ValidationHelper.GetString(skuErrorMessage, "");
        if (err != "")
        {
            return "<br /><span class=\"ItemsNotAvailable\">" + err + "</span>";
        }
        return "";
    }


    /// <summary>
    /// Initializes tax registration ID and orgranization ID.
    /// </summary>
    protected void InitializeIDs()
    {
        SiteInfo si = SiteContext.CurrentSite;
        if (si != null)
        {
            if ((ECommerceSettings.ShowOrganizationID(si.SiteName)) && (ShoppingCart.Customer != null))
            {
                // Initialize organization ID
                plcIDs.Visible = true;
                lblOrganizationID.Text = GetString("OrderPreview.OrganizationID");
                lblOrganizationIDVal.Text = HTMLHelper.HTMLEncode(ShoppingCart.Customer.CustomerOrganizationID);
            }
            else
            {
                lblOrganizationID.Visible = false;
                lblOrganizationIDVal.Visible = false;
            }
            if ((ECommerceSettings.ShowTaxRegistrationID(si.SiteName)) && (ShoppingCart.Customer != null))
            {
                // Initialize tax registration ID
                plcIDs.Visible = true;
                lblTaxRegistrationID.Text = GetString("OrderPreview.TaxRegistrationID");
                lblTaxRegistrationIDVal.Text = HTMLHelper.HTMLEncode(ShoppingCart.Customer.CustomerTaxRegistrationID);
            }
            else
            {
                lblTaxRegistrationID.Visible = false;
                lblTaxRegistrationIDVal.Visible = false;
            }
        }
    }


    /// <summary>
    /// Returns formated value string.
    /// </summary>
    /// <param name="value">Value to format</param>
    protected string GetFormattedValue(object value)
    {
        double price = ValidationHelper.GetDouble(value, 0);
        return CurrencyInfoProvider.GetFormattedValue(price, ShoppingCart.Currency);
    }


    /// <summary>
    /// Returns formatted and localized SKU name.
    /// </summary>
    /// <param name="value">SKU name</param>
    /// <param name="isProductOption">Indicates if cart item is product option</param>
    /// <param name="isBundleItem">Indicates if cart item is bundle item</param>
    protected string GetSKUName(object value, object isProductOption, object isBundleItem, object itemText)
    {
        string name = ResHelper.LocalizeString((string)value);
        string text = itemText as string;

        // If it is a product option or bundle item
        if (ValidationHelper.GetBoolean(isProductOption, false) || ValidationHelper.GetBoolean(isBundleItem, false))
        {
            StringBuilder skuName = new StringBuilder("<span style=\"font-size:90%\"> - ");
            skuName.Append(HTMLHelper.HTMLEncode(name));

            if (!string.IsNullOrEmpty(text))
            {
                skuName.Append(" '" + HTMLHelper.HTMLEncode(text) + "'");
            }

            skuName.Append("</span>");
            return skuName.ToString();
        }
        // If it is a parent product
        else
        {
            return HTMLHelper.HTMLEncode(name);
        }
    }
    protected void Button1_Click(object sender, EventArgs e)
    {
        ButtonNextClickAction();
    }
}