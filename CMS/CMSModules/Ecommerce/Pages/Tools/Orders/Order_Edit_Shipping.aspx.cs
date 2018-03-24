using System;

using CMS.Core;
using CMS.Ecommerce;
using CMS.Helpers;
using CMS.UIControls;

[EditedObject(OrderInfo.OBJECT_TYPE, "orderId")]
[UIElement(ModuleName.ECOMMERCE, "Orders.Shipping")]
public partial class CMSModules_Ecommerce_Pages_Tools_Orders_Order_Edit_Shipping : CMSEcommercePage
{
    #region "Properties"

    /// <summary>
    /// Editing order object
    /// </summary>
    private OrderInfo Order
    {
        get
        {
            return orderShippingForm.EditedObject as OrderInfo;
        }
    }


    private bool IsTaxBasedOnShippingAddress
    {
        get
        {
            return (ECommerceSettings.ApplyTaxesBasedOn(Order.OrderSiteID) == ApplyTaxBasedOnEnum.ShippingAddress);
        }
    }


    private SiteSeparatedObjectSelector ShippingOptionSelector
    {
        get
        {
            return orderShippingForm.FieldControls["OrderShippingOptionID"] as SiteSeparatedObjectSelector;
        }
    }


    private UniSelector ShippingAddressSelector
    {
        get
        {
            return orderShippingForm.FieldControls["OrderShippingAddressID"] as UniSelector;
        }
    }

    #endregion


    #region "Page events"

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        orderShippingForm.OnCheckPermissions += CheckPermissions;
        orderShippingForm.OnBeforeSave += OnBeforeSave;
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        // Check if order is not edited from another site
        CheckEditedObjectSiteID(Order.OrderSiteID);

        // Hide Select and Clear button which are visible by default for UniSelector
        if (ShippingAddressSelector != null)
        {
            ShippingAddressSelector.ButtonSelect.Visible = false;
            ShippingAddressSelector.ButtonClear.Visible = false;
        }
    }

    #endregion


    #region "Event handlers"

    protected void CheckPermissions(object sender, EventArgs e)
    {
        if (!ECommerceContext.IsUserAuthorizedForPermission("ModifyOrders"))
        {
            RedirectToAccessDenied("CMS.Ecommerce", "EcommerceModify OR ModifyOrders");
        }
    }


    protected void OnBeforeSave(object sender, EventArgs e)
    {
        if ((Order == null) || (ShippingAddressSelector == null) || (ShippingOptionSelector == null))
        {
            return;
        }

        // Get shopping cart from order
        ShoppingCartInfo sci = ShoppingCartInfoProvider.GetShoppingCartInfoFromOrder(Order.OrderID);

        // Get current values
        int addressID = ValidationHelper.GetInteger(ShippingAddressSelector.Value, 0);
        int shippingOptionID = ValidationHelper.GetInteger(ShippingOptionSelector.Value, 0);

        // Is shipping needed?
        bool isShippingNeeded = ((sci != null) && sci.IsShippingNeeded);

        // If shipping address is required
        if (isShippingNeeded || IsTaxBasedOnShippingAddress)
        {
            // If shipping address is not set
            if (addressID <= 0)
            {
                // Show error message
                ShowError(GetString("Order_Edit_Shipping.NoAddress"));
                return;
            }
        }

        try
        {
            // Shipping option changed 
            if ((sci != null) && (Order.OrderShippingOptionID != shippingOptionID))
            {
                // Shipping option and payment method combination is not allowed 
                if (PaymentShippingInfoProvider.GetPaymentShippingInfo(Order.OrderPaymentOptionID, shippingOptionID) == null)
                {
                    PaymentOptionInfo payment = PaymentOptionInfoProvider.GetPaymentOptionInfo(Order.OrderPaymentOptionID);

                    // Check if payment is allowed with no shipping
                    if ((payment != null) && !(payment.PaymentOptionAllowIfNoShipping && shippingOptionID == 0))
                    {
                        // Set payment method to none and display warning
                        sci.ShoppingCartPaymentOptionID = 0;

                        string paymentMethodName = ResHelper.LocalizeString(payment.PaymentOptionDisplayName, null, true);
                        string shippingOptionName = HTMLHelper.HTMLEncode(ShippingOptionSelector.ValueDisplayName);

                        ShowWarning(String.Format(ResHelper.GetString("com.shippingoption.paymentsetnone"), paymentMethodName, shippingOptionName));
                    }

                }

                // Set order new properties
                sci.ShoppingCartShippingOptionID = shippingOptionID;

                // Evaluate order data
                ShoppingCartInfoProvider.EvaluateShoppingCart(sci);

                // Update order data
                ShoppingCartInfoProvider.SetOrder(sci, true);
            }

            // Update tracking number
            Order.OrderTrackingNumber = ValidationHelper.GetString(orderShippingForm.FieldEditingControls["OrderTrackingNumber"].DataValue, String.Empty).Trim();
            OrderInfoProvider.SetOrderInfo(Order);

            // Show message
            ShowChangesSaved();

            // Update shipping charge in selector
            if (IsTaxBasedOnShippingAddress)
            {
                var shippingSelector = orderShippingForm.FieldControls["OrderShippingOptionID"] as SiteSeparatedObjectSelector;
                if (shippingSelector != null)
                {
                    shippingSelector.SetValue("ShoppingCart", sci);
                    shippingSelector.Reload(false);
                }
            }

            // Stop automatic saving action
            orderShippingForm.StopProcessing = true;
        }
        catch (Exception ex)
        {
            // Show error message
            ShowError(ex.Message);
        }
    }

    #endregion
}