using System;
using System.Data;
using System.Web.UI.WebControls;

using CMS.CMSHelper;
using CMS.Ecommerce;
using CMS.EcommerceProvider;
using CMS.SiteProvider;

public partial class CMSModules_Ecommerce_Controls_ShoppingCart_ShoppingCartPaymentShipping : ShoppingCartStep
{
    #region "ViewState Constants"

    private const string SHIPPING_OPTION_ID = "OrderShippingOptionID";
    private const string PAYMENT_OPTION_ID = "OrderPaymenOptionID";

    #endregion


    #region "Variables"

    private bool? mIsShippingNeeded = null;

    #endregion


    #region "Properties"

    /// <summary>
    /// Returns true if shopping cart items need shipping.
    /// </summary>
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


    #region "Page methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        // Init labels
        lblTitle.Text = GetString("shoppingcart.shippingpaymentoptions");
        lblPayment.Text = GetString("shoppingcartpaymentshipping.payment");
        lblShipping.Text = GetString("shoppingcartpaymentshipping.shipping");

        selectShipping.IsLiveSite = IsLiveSite;
        selectPayment.IsLiveSite = IsLiveSite;

        if ((ShoppingCart != null) && (CMSContext.CurrentSite != null))
        {
            if (ShoppingCart.CountryID == 0)
            {
                string countryName = ECommerceSettings.DefaultCountryName(CMSContext.CurrentSite.SiteName);
                CountryInfo ci = CountryInfoProvider.GetCountryInfo(countryName);
                ShoppingCart.CountryID = (ci != null) ? ci.CountryID : 0;
            }

            selectShipping.ShoppingCart = ShoppingCart;
        }

        if (!ShoppingCartControl.IsCurrentStepPostBack)
        {
            if (IsShippingNeeded)
            {
                SelectShippingOption();
            }
            else
            {
                // Don't use shipping selection
                selectShipping.StopProcessing = true;

                // Hide title
                lblTitle.Visible = false;

                // Change current checkout process step caption
                ShoppingCartControl.CheckoutProcessSteps[ShoppingCartControl.CurrentStepIndex].Caption = GetString("order_new.paymentshipping.titlenoshipping");
            }
        }
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        if (selectShipping.HasData)
        {
            // Show shipping selection
            plcShipping.Visible = true;

            // Initialize payment options for selected shipping option
            selectPayment.ShippingOptionID = selectShipping.ShippingID;
            selectPayment.PaymentID = -1;
            selectPayment.DisplayOnlyAllowedIfNoShipping = false;
        }
        else
        {
            selectPayment.DisplayOnlyAllowedIfNoShipping = true;
        }

        selectPayment.ReloadData();

        SelectPaymentOption();

        plcPayment.Visible = selectPayment.HasData;
    }

    #endregion


    /// <summary>
    /// Back button actions.
    /// </summary>
    public override void ButtonBackClickAction()
    {
        // Save the values to ShoppingCart ViewState
        ShoppingCartControl.SetTempValue(SHIPPING_OPTION_ID, selectShipping.ShippingID);
        ShoppingCartControl.SetTempValue(PAYMENT_OPTION_ID, selectPayment.PaymentID);

        base.ButtonBackClickAction();
    }


    public override bool ProcessStep()
    {
        try
        {
            // Cleanup the ShoppingCart ViewState
            ShoppingCartControl.SetTempValue(SHIPPING_OPTION_ID, null);
            ShoppingCartControl.SetTempValue(PAYMENT_OPTION_ID, null);

            ShoppingCart.ShoppingCartShippingOptionID = selectShipping.ShippingID;
            ShoppingCart.ShoppingCartPaymentOptionID = selectPayment.PaymentID;

            // Update changes in database only when on the live site
            if (!ShoppingCartControl.IsInternalOrder)
            {
                ShoppingCartInfoProvider.SetShoppingCartInfo(ShoppingCart);
            }
            return true;
        }
        catch (Exception ex)
        {
            lblError.Visible = true;
            lblError.Text = ex.Message;
            return false;
        }
    }


    /// <summary>
    /// Preselects payment option.
    /// </summary>
    protected void SelectPaymentOption()
    {
        try
        {
            // Try to select payment from ViewState first
            object viewStateValue = ShoppingCartControl.GetTempValue(PAYMENT_OPTION_ID);
            if (viewStateValue != null)
            {
                selectPayment.PaymentID = Convert.ToInt32(viewStateValue);
            }
            // Try to select payment option according to saved option in shopping cart object
            else if (ShoppingCart.ShoppingCartPaymentOptionID > 0)
            {
                selectPayment.PaymentID = ShoppingCart.ShoppingCartPaymentOptionID;
            }
            // Try to select payment option according to user preffered option
            else
            {
                CustomerInfo customer = ShoppingCart.Customer;
                int paymentOptionId = (customer.CustomerUser != null) ? customer.CustomerUser.GetUserPreferredPaymentOptionID(CMSContext.CurrentSiteName) : 0;
                if (paymentOptionId > 0)
                {
                    selectPayment.PaymentID = paymentOptionId;
                }
            }
        }
        catch
        {
        }
    }


    /// <summary>
    /// Preselects shipping option.
    /// </summary>
    protected void SelectShippingOption()
    {
        try
        {
            // Try to select shipping from ViewState first
            object viewStateValue = ShoppingCartControl.GetTempValue(SHIPPING_OPTION_ID);
            if (viewStateValue != null)
            {
                selectShipping.ShippingID = Convert.ToInt32(viewStateValue);
            }
            // Try to select shipping option according to saved option in shopping cart object
            else if (ShoppingCart.ShoppingCartShippingOptionID > 0)
            {
                selectShipping.ShippingID = ShoppingCart.ShoppingCartShippingOptionID;
            }
            // Try to select shipping option according to user preffered option
            else
            {
                CustomerInfo customer = ShoppingCart.Customer;
                int shippingOptionId = (customer.CustomerUser != null) ? customer.CustomerUser.GetUserPreferredShippingOptionID(CMSContext.CurrentSiteName) : 0;
                if (shippingOptionId > 0)
                {
                    selectShipping.ShippingID = shippingOptionId;
                }
            }
        }
        catch
        {
        }
    }


    public override bool IsValid()
    {
        string errorMessage = "";

        // If shipping is required
        if (plcShipping.Visible)
        {
            if (selectShipping.ShippingID <= 0)
            {
                errorMessage = GetString("Order_New.NoShippingOption");
            }
        }

        // If payment is required
        if (plcPayment.Visible)
        {
            if ((errorMessage == "") && (selectPayment.PaymentID <= 0))
            {
                errorMessage = GetString("Order_New.NoPaymentMethod");
            }
        }

        if (errorMessage == "")
        {
            // Form is valid
            return true;
        }

        // Form is not valid
        lblError.Visible = true;
        lblError.Text = errorMessage;
        return false;
    }
}