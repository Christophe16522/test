﻿using System;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.CMSHelper;
using CMS.Ecommerce;
using CMS.EcommerceProvider;
using CMS.ExtendedControls;
using CMS.GlobalHelper;
using CMS.SettingsProvider;
using CMS.SiteProvider;
using CMS.Helpers;
using CMS.Protection;

public partial class CMSModules_Ecommerce_Controls_ShoppingCart_ShoppingCart : ShoppingCart
{
    private bool mDisplayStepImages = false;
    private bool mDisplayStepIndexes = true;


    /// <summary>
    /// Indicates whether step images should be displayed.
    /// </summary>
    public bool DisplayStepImages
    {
        get
        {
            return mDisplayStepImages;
        }
        set
        {
            mDisplayStepImages = value;
        }
    }


    /// <summary>
    /// Indicates whether step indexes should be displayed.
    /// </summary>
    public bool DisplayStepIndexes
    {
        get
        {
            return mDisplayStepIndexes;
        }
        set
        {
            mDisplayStepIndexes = value;
        }
    }


    /// <summary>
    /// Back button.
    /// </summary>
    public override CMSButton ButtonBack
    {
        get
        {
            return btnBack;
        }
        set
        {
            btnBack = value;
        }
    }


    /// <summary>
    /// Next button.
    /// </summary>
    public override CMSButton ButtonNext
    {
        get
        {
            return btnNext;
        }
        set
        {
            btnNext = value;
        }
    }


    /// <summary>
    /// Page load.
    /// </summary>
    protected void Page_Load(object sender, EventArgs e)
    {
        // If shopping cart is created -> create empty one
        if ((ShoppingCartInfoObj == null) && (SiteContext.CurrentSite != null))
        {
            ShoppingCartInfoObj = ShoppingCartInfoProvider.CreateShoppingCartInfo(CMSContext.CurrentSiteID);

            // Set customer preffered options   
            CustomerInfo currentCustomer = ECommerceContext.CurrentCustomer;
            if ((currentCustomer != null) && (currentCustomer.CustomerUser != null))
            {
                ShoppingCartInfoObj.ShoppingCartCurrencyID = currentCustomer.CustomerUser.GetUserPreferredCurrencyID(SiteContext.CurrentSiteName);
            }
        }

        if (CurrentStepIndex == 0)
        {
            ShoppingCartInfoObj.PrivateDataCleared = false;
        }

        // Display / hide checkout process images
        plcCheckoutProcess.Visible = DisplayStepImages;

        // Load current step data
        LoadCurrentStep();

        // If shopping cart information exist
        if (ShoppingCartInfoObj != null)
        {
            // Get order information
            OrderInfo oi = OrderInfoProvider.GetOrderInfo(ShoppingCartInfoObj.OrderId);

            // If order is paid
            if ((oi != null) && (oi.OrderIsPaid))
            {
                // Disable specific controls
                btnNext.Enabled = false;
                CurrentStepControl.Enabled = false;
            }
        }
    }


    /// <summary>
    /// On page pre-render event.
    /// </summary>
    protected void Page_Prerender(object sender, EventArgs e)
    {
        // Back and Next button disabling script (to prevent multiple click)
        ButtonNext.OnClientClick = "if ((typeof(Page_ClientValidate) == 'function') && Page_ClientValidate('" + ButtonNext.ValidationGroup + "')) { this.disabled = true; } " + Page.ClientScript.GetPostBackEventReference(ButtonNext, null) + "; return false;";
        ButtonBack.OnClientClick = "if ((typeof(Page_ClientValidate) == 'function') && Page_ClientValidate('" + ButtonBack.ValidationGroup + "')) { this.disabled = true; }" + Page.ClientScript.GetPostBackEventReference(ButtonBack, null) + "; return false;";

        if ((CheckoutProcessSteps != null) && (CurrentStepControl != null))
        {
            if (DisplayStepIndexes)
            {
                lblStepTitle.Text = HTMLHelper.HTMLEncode(String.Format(GetString("Order_New.CurrentStep"), CurrentStepIndex + 1, CheckoutProcessSteps.Count) + " - " + ResHelper.LocalizeString(CheckoutProcessSteps[CurrentStepIndex].Caption));
            }
            else
            {
                lblStepTitle.Text = HTMLHelper.HTMLEncode(ResHelper.LocalizeString(CheckoutProcessSteps[CurrentStepIndex].Caption));
            }
        }
        else
        {
            ButtonBack.Visible = false;
            ButtonNext.Visible = false;

            // Display error mesage, when no steps found
            if ((CheckoutProcessSteps == null) || (CheckoutProcessSteps.Count == 0))
            {
                lblError.Text = GetString("com.checkoutprocess.nosteps");
                lblError.Visible = true;
            }
        }

        // Save previous page url
        if (!RequestHelper.IsPostBack() && (Request.UrlReferrer != null))
        {
            string path = URLHelper.GetAppRelativePath(Request.UrlReferrer);
            if (!URLHelper.IsExcludedSystem(path))
            {
                // It previous page was another shopping cart step
                PreviousPageUrl = CMSContext.RawUrl.ToLowerCSafe() == Request.UrlReferrer.PathAndQuery.ToLowerCSafe() ? "~/" : Request.UrlReferrer.AbsoluteUri;
            }
        }
        else
        {
            // Try to find the Previeous page in session
            string prevPage = ValidationHelper.GetString(SessionHelper.GetValue("ShoppingCartUrlReferrer"), "");
            if (!String.IsNullOrEmpty(prevPage))
            {
                PreviousPageUrl = prevPage;
            }
        }
    }


    /// <summary>
    /// Back button clicked.
    /// </summary>
    protected void btnBack_Click(object sender, EventArgs e)
    {
        // Load first checkout process step if private data was cleared
        if (ShoppingCartInfoObj.PrivateDataCleared && (CurrentStepIndex > 0))
        {
            ShoppingCartInfoObj.PrivateDataCleared = false;
            LoadStep(0);

            lblError.Visible = true;
            lblError.Text = GetString("com.shoppingcart.sessiontimedout");
            return;
        }

        CurrentStepControl.ButtonBackClickAction();
    }


    /// <summary>
    /// Next button clicked.
    /// </summary>
    protected void btnNext_Click(object sender, EventArgs e)
    {
        // Check private data
        if (ShoppingCartInfoObj.PrivateDataCleared && (CurrentStepIndex > 0))
        {
            // Go to the first step
            ShoppingCartInfoObj.PrivateDataCleared = false;
            LoadStep(0);

            // Display error
            lblError.Visible = true;
            lblError.Text = GetString("com.shoppingcart.sessiontimedout");
            return;
        }

        // Check banned IP
        if (!BannedIPInfoProvider.IsAllowed(SiteContext.CurrentSiteName, BanControlEnum.AllNonComplete))
        {
            // Go to the first step
            LoadStep(0);

            // Display error
            lblError.Visible = true;
            lblError.Text = GetString("General.BannedIP");
            return;
        }

        CurrentStepControl.ButtonNextClickAction();
    }


    /// <summary>
    /// Loads current step control.
    /// </summary>    
    public override void LoadCurrentStep()
    {
        if ((CurrentStepIndex >= 0) && (CurrentStepIndex < CheckoutProcessSteps.Count))
        {
            // Shopping cart container
            ShoppingCartContainer = pnlShoppingCart;

            // Default button settings
            ButtonBack.Enabled = true;
            ButtonNext.Enabled = true;
            ButtonBack.Visible = true;
            ButtonNext.Visible = true;
            ButtonBack.Text = GetString("general.back");
            ButtonNext.Text = GetString("general.next");
            ButtonBack.CssClass = "SubmitButton";
            ButtonNext.CssClass = "SubmitButton";

            if (CurrentStepControl != null)
            {
                // Display checkout process images
                if (DisplayStepImages)
                {
                    LoadCheckoutProcessImages();
                }

                // Set shopping cart step container
                CurrentStepControl.StepContainer = pnlCartStepInner;

                // Display current control      
                pnlCartStepInner.Controls.Clear();
                pnlCartStepInner.Controls.Add(CurrentStepControl);
            }
            else
            {
                lblError.Visible = true;
                lblError.Text = GetString("ShoppingCart.ErrorLoadingStep");
            }
        }
    }


    /// <summary>
    /// Loads checkout process images.
    /// </summary>
    private void LoadCheckoutProcessImages()
    {
        if ((CurrentStepControl != null) && (CurrentStepControl.CheckoutProcessStep != null))
        {
            // Get current step code name
            string currentStepName = CurrentStepControl.CheckoutProcessStep.Name.ToLowerCSafe();

            // Clears image collection
            plcStepImages.Controls.Clear();

            // Go through the checkout process steps
            foreach (CheckoutProcessStepInfo step in CheckoutProcessSteps)
            {
                Image imgStep = new Image();
                string imageName = "";

                // If step is equal to Current step
                if (currentStepName == step.Name.ToLowerCSafe())
                {
                    int dotIndex = step.Icon.IndexOfCSafe('.');
                    if (dotIndex > 1)
                    {
                        // Image name = [filename]_selected.[extension]
                        imageName = step.Icon.Insert(dotIndex, "_selected");
                    }
                    else
                    {
                        // Image name = [filename]_selected
                        imageName = step.Icon + "_selected";
                    }
                }
                // If step is different from Current step
                else
                {
                    // Image name = [filename].[extension]
                    imageName = step.Icon;
                }

                // Add step image to the collection
                imgStep.ID = "img" + step.Name;
                imgStep.ImageUrl = ImageFolderPath.TrimEnd('/') + "/" + imageName;
                imgStep.CssClass = "ShoppingCartStepImage";
                imgStep.ToolTip = imgStep.AlternateText = ResHelper.LocalizeString(step.Caption);
                plcStepImages.Controls.Add(imgStep);

                // Add image step separator
                if ((step.StepIndex < CheckoutProcessSteps.Count - 1) && (ImageStepSeparator != ""))
                {
                    LiteralControl separator = new LiteralControl(ImageStepSeparator);
                    plcStepImages.Controls.Add(separator);
                }
            }
        }
    }
}