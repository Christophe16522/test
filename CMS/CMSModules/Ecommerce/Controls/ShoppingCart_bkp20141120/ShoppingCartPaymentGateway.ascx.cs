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
using CMS.EventLog;
using CMS.Helpers;
using CMS.DocumentEngine;

public partial class CMSModules_Ecommerce_Controls_ShoppingCart_ShoppingCartPaymentGateway : ShoppingCartStep
{
	private readonly EventLogProvider p = new EventLogProvider();
    protected void Page_Load(object sender, EventArgs e)
    {
		p.LogEvent("I", DateTime.Now, "ShoppingCartPaymentGateway "  , "");
        // No payment provider loaded -> skip payment
        if (ShoppingCartControl.PaymentGatewayProvider == null)
        {
            // Clean current order payment result when editing existing order and payment was skipped
            if ((ShoppingCartControl.CheckoutProcessType == CheckoutProcessEnum.CMSDeskOrderItems) &&
                !ShoppingCartControl.IsCurrentStepPostBack)
            {
				p.LogEvent("I", DateTime.Now, "CleanUpOrderPaymentResult "  , "");
                CleanUpOrderPaymentResult();
            }

            // Raise payment skipped
            ShoppingCartControl.RaisePaymentSkippedEvent();

            // When on the live site
            if (!ShoppingCartControl.IsInternalOrder)
            {
                // Get Url the user should be redirected to
                string url = ShoppingCartControl.GetRedirectAfterPurchaseUrl();

                // Remove shopping cart data from database and from session
                ShoppingCartControl.CleanUpShoppingCart();

                if (!string.IsNullOrEmpty(url))
                {
                    URLHelper.Redirect(url);
                }
                else
                {
                    URLHelper.Redirect(ShoppingCartControl.PreviousPageUrl);
                }
            }
            return;
        }
        else if (ShoppingCart != null)
        {
			p.LogEvent("I", DateTime.Now, "ShoppingCartControl.PaymentGatewayProvider != null "  , "");
            LoadData();
        }

        lblTitle.Text = GetString("PaymentSummary.Title");
        lblTotalPrice.Text = GetString("PaymentSummary.TotalPrice");
        lblOrderId.Text = GetString("PaymentSummary.OrderId");
        lblPayment.Text = GetString("PaymentSummary.Payment");
       this.ShoppingCartControl.ButtonNextClickAction(); 
    }


    protected void Page_PreRender(object sender, EventArgs e)
    {
        // Set buttons properties
        if (!(ShoppingCartControl.PaymentGatewayProvider.IsPaymentCompleted) ||
            (ShoppingCartControl.CheckoutProcessType == CheckoutProcessEnum.CMSDeskOrderItems))
        {
            if (ShoppingCartControl.IsInternalOrder)
            {
                // Show 'Skip payment' button
                ShoppingCartControl.ButtonBack.CssClass = "LongButton";
                ShoppingCartControl.ButtonBack.Text = GetString("ShoppingCart.PaymentGateway.SkipPayment");
            }
            else
            {
                ShoppingCartControl.ButtonBack.Visible = false;
            }

            // Show 'Finish payment' button
            ShoppingCartControl.ButtonNext.CssClass = "LongButton";
            ShoppingCartControl.ButtonNext.Text = GetString("ShoppingCart.PaymentGateway.FinishPayment");
        }
    }


    public override void ButtonNextClickAction()
    {
        // Standard action - Process payment
        base.ButtonNextClickAction();

        if (ShoppingCartControl.PaymentGatewayProvider.IsPaymentCompleted)
        {
            // Remove current shopping cart data from session and from database
            ShoppingCartControl.CleanUpShoppingCart();

            // Live site
            if (!ShoppingCartControl.IsInternalOrder)
            {
                string url = "";
                if (ShoppingCartControl.RedirectAfterPurchase != "")
                {
                    url = DocumentURLProvider.GetUrl(ShoppingCartControl.RedirectAfterPurchase) + "?pid=" + ShoppingCart.PaymentOption.PaymentOptionDisplayName;
                }
                else
                {
                    url = DocumentURLProvider.GetUrl("/");
                }

                URLHelper.Redirect(url);
            }
        }
    }


    public override void ButtonBackClickAction()
    {
        // Clean current order payment result when editing existing order and payment was skipped
        //if (this.ShoppingCartControl.CheckoutProcessType == CheckoutProcessEnum.CMSDeskOrderItems)
        //{
        //    CleanUpOrderPaymentResult();
        //}

        // Payment was skipped
        ShoppingCartControl.RaisePaymentSkippedEvent();

        // Remove current shopping cart data from session and from database
        ShoppingCartControl.CleanUpShoppingCart();

        // Live site - skip payment
        if (!ShoppingCartControl.IsInternalOrder)
        {
            string url = "";
            if (ShoppingCartControl.RedirectAfterPurchase != "")
            {
                url = DocumentURLProvider.GetUrl(ShoppingCartControl.RedirectAfterPurchase);
            }
            else
            {
                url = DocumentURLProvider.GetUrl("/");
            }

            URLHelper.Redirect(url);
        }
    }


    public override bool IsValid()
    {
        return ((ShoppingCartControl.PaymentGatewayProvider != null) && (ShoppingCartControl.PaymentGatewayProvider.ValidateCustomData() == ""));
    }


    public override bool ProcessStep()
    {
        if (ShoppingCartControl.PaymentGatewayProvider != null)
        {
            // Proces current step payment gateway custom data
            ShoppingCartControl.PaymentGatewayProvider.ProcessCustomData();

            // Skip payment when already payed except when editing existing order
            if ((!ShoppingCartControl.PaymentGatewayProvider.IsPaymentCompleted) ||
                (ShoppingCartControl.CheckoutProcessType == CheckoutProcessEnum.CMSDeskOrderItems))
            {
                // Process payment 
                ShoppingCartControl.PaymentGatewayProvider.ProcessPayment();
            }

            // Show info message
            if (ShoppingCartControl.PaymentGatewayProvider.InfoMessage != "")
            {
                lblInfo.Visible = true;
                lblInfo.Text = ShoppingCartControl.PaymentGatewayProvider.InfoMessage;
            }

            // Show error message
            if (ShoppingCartControl.PaymentGatewayProvider.ErrorMessage != "")
            {
                lblError.Visible = true;
                lblError.Text = ShoppingCartControl.PaymentGatewayProvider.ErrorMessage;
                return false;
            }

            if (ShoppingCartControl.PaymentGatewayProvider.IsPaymentCompleted)
            {
                // Raise payment completed event
                ShoppingCartControl.RaisePaymentCompletedEvent();
                return true;
            }
        }
        return false;
    }


    private void LoadData()
    {
        // Payment summary
        lblTotalPriceValue.Text = CurrencyInfoProvider.GetFormattedPrice(ShoppingCart.RoundedTotalPrice, ShoppingCart.Currency);
        lblOrderIdValue.Text = Convert.ToString(ShoppingCart.OrderId);
        if (ShoppingCart.PaymentOption != null)
        {
            lblPaymentValue.Text = ResHelper.LocalizeString(ShoppingCart.PaymentOption.PaymentOptionDisplayName);
        }

        // Add payment gateway custom data
        ShoppingCartControl.PaymentGatewayProvider.AddCustomData();

        // Show "Order saved" info message
        if (!ShoppingCartControl.IsCurrentStepPostBack)
        {
            //if (this.ShoppingCartControl.IsInternalOrder)
            //{
            //    lblInfo.Text = GetString("General.ChangesSaved");
            //}
            //else
            //{
            //    lblInfo.Text = GetString("ShoppingCart.PaymentGateway.OrderSaved");
            //}
            lblInfo.Text = GetString("ShoppingCart.PaymentGateway.OrderSaved");
            lblInfo.Visible = true;
        }
        else
        {
            lblInfo.Text = "";
        }
    }


    /// <summary>
    /// Clean up current order payment result.
    /// </summary>
    private void CleanUpOrderPaymentResult()
    {
        if (ShoppingCart != null)
        {
            OrderInfo oi = OrderInfoProvider.GetOrderInfo(ShoppingCart.OrderId);
            if (oi != null)
            {
                oi.OrderPaymentResult = null;
                OrderInfoProvider.SetOrderInfo(oi);
            }
        }
    }
}