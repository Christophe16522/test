using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Collections;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.CMSHelper;
using CMS.Ecommerce;
using CMS.GlobalHelper;
using CMS.PortalEngine;
using CMS.SettingsProvider;
using CMS.UIControls;
using CMS.WebAnalytics;

public partial class CMSModules_Ecommerce_Controls_ProductOptions_ShoppingCartItemSelector : CMSUserControl
{
    #region "Variables"

    // String variable
    private string mAddToCartLinkText = "";
    private string mAddToCartText = "shoppingcart.addtoshoppingcart";
    private string mAddToCartTooltip = "";
    private string mAddToWishlistImageButton = "";
    private string mAddToWishlistLinkText = "";
    private string mImageFolder = "";
    private string mAddToCartImageButton = "";
    private string mShoppingCartUrl = "";
    private string mWishlistUrl = "";

    // Bool variables
    private bool? mSKUHasOptions = null;
    private bool mSKUEnabled = true;
    private bool mDataLoaded = false;
    private bool mShowUnitsTextBox = false;
    private bool mShowProductOptions = false;
    private bool mShowDonationProperties = false;
    private bool mShowWishlistLink = true;
    private bool mShowPriceIncludingTax = ECommerceSettings.DisplayPriceIncludingTaxes(CMSContext.CurrentSiteName);
    private bool mShowTotalPrice = false;
    private bool mAlwaysShowTotalPrice = false;
    private bool mDialogMode = false;
    private bool? mRedirectToShoppingCart = null;
    private bool mRedirectToDetailsEnabled = true;

    private int mDefaultQuantity = 1;

    private ShoppingCartInfo mShoppingCart = null;
    private SKUInfo mSKU = null;

    private Hashtable mProductOptions = null;

    #endregion


    #region "Properties"

    /// <summary>
    /// Product ID (SKU ID).
    /// </summary>
    public int SKUID
    {
        get
        {
            return ValidationHelper.GetInteger(ViewState["SKUID"], 0);
        }
        set
        {
            ViewState["SKUID"] = value;

            // Invalidate SKU data
            mSKU = null;
            mSKUHasOptions = null;
        }
    }


    /// <summary>
    /// Product SKU data
    /// </summary>
    public SKUInfo SKU
    {
        get
        {
            if (mSKU == null)
            {
                mSKU = SKUInfoProvider.GetSKUInfo(SKUID);
            }

            return mSKU;
        }
    }


    /// <summary>
    /// Indicates if the product has product options
    /// </summary>
    public bool SKUHasOptions
    {
        get
        {
            if (!mSKUHasOptions.HasValue)
            {
                if (IsLiveSite)
                {
                    // Get cache minutes
                    int cacheMinutes = SettingsKeyProvider.GetIntValue(CMSContext.CurrentSiteName + ".CMSCacheMinutes");

                    // Try to get data from cache
                    using (CachedSection<bool?> cs = new CachedSection<bool?>(ref mSKUHasOptions, cacheMinutes, true, null, "skuhasoptions", SKUID))
                    {
                        if (cs.LoadData)
                        {
                            // Get the data
                            mSKUHasOptions = SKUInfoProvider.HasSKUEnabledOptions(SKUID);

                            // Save to the cache
                            if (cs.Cached)
                            {
                                // Get dependencies
                                List<string> dependencies = new List<string>();
                                dependencies.Add("ecommerce.sku|byid|" + SKUID);
                                dependencies.Add("ecommerce.sku|all");
                                dependencies.Add("ecommerce.optioncategory|all");

                                // Set dependencies
                                cs.CacheDependency = CacheHelper.GetCacheDependency(dependencies);
                            }

                            cs.Data = mSKUHasOptions;
                        }
                    }
                }
                else
                {
                    // Get the data
                    mSKUHasOptions = SKUInfoProvider.HasSKUEnabledOptions(SKUID);
                }
            }

            return mSKUHasOptions.Value;
        }
    }


    /// <summary>
    /// Indicates whether current product (SKU) is enabled, TRUE - button/link for adding product to the shopping cart is rendered, otherwise it is not rendered.
    /// </summary>
    public bool SKUEnabled
    {
        get
        {
            return mSKUEnabled;
        }
        set
        {
            mSKUEnabled = value;
            InitializeControls();
        }
    }


    /// <summary>
    /// File name of the image which is used as a source for image button to add product to the shopping cart, default image folder is '~/App_Themes/(Skin_Folder)/Images/ShoppingCart/'.
    /// </summary>
    public string AddToCartImageButton
    {
        get
        {
            return mAddToCartImageButton;
        }
        set
        {
            mAddToCartImageButton = value;
        }
    }


    /// <summary>
    /// Simple string or localizable string of the link to add product to the shopping cart.
    /// </summary>
    public string AddToCartLinkText
    {
        get
        {
            return mAddToCartLinkText;
        }
        set
        {
            mAddToCartLinkText = value;
        }
    }


    /// <summary>
    /// Simple string or localizable string of add to shopping cart link or button.
    /// </summary>
    public string AddToCartText
    {
        get
        {
            return mAddToCartText;
        }
        set
        {
            mAddToCartText = value;
        }
    }


    /// <summary>
    /// Simple string or localizable string of add to shopping cart link or button tooltip.
    /// </summary>
    public string AddToCartTooltip
    {
        get
        {
            return mAddToCartTooltip;
        }
        set
        {
            mAddToCartTooltip = value;
        }
    }


    /// <summary>
    /// Indicates if textbox for entering number of units to add to the shopping cart should be displayed, if it is hidden number of units is equal to 1.
    /// </summary>
    public bool ShowUnitsTextBox
    {
        get
        {
            return mShowUnitsTextBox;
        }
        set
        {
            mShowUnitsTextBox = value;
        }
    }


    /// <summary>
    /// Indicates if product options of the current product should be displayed.
    /// </summary>
    public bool ShowProductOptions
    {
        get
        {
            return mShowProductOptions;
        }
        set
        {
            mShowProductOptions = value;
        }
    }


    /// <summary>
    /// Indicates if donation properties control is displayed.
    /// </summary>
    public bool ShowDonationProperties
    {
        get
        {
            return mShowDonationProperties;
        }
        set
        {
            mShowDonationProperties = value;
        }
    }


    /// <summary>
    /// Indicates if "Add to Wishlist" link or image should be displayed. Default value is true.
    /// </summary>
    public bool ShowWishlistLink
    {
        get
        {
            return mShowWishlistLink;
        }
        set
        {
            mShowWishlistLink = value;
        }
    }


    /// <summary>
    /// Default quantity when adding product to the shopping cart.
    /// </summary>
    public int DefaultQuantity
    {
        get
        {
            return mDefaultQuantity;
        }
        set
        {
            mDefaultQuantity = value;
        }
    }


    /// <summary>
    /// True - total price is shown when product has product options.
    /// </summary>
    public bool ShowTotalPrice
    {
        get
        {
            return mShowTotalPrice;
        }
        set
        {
            mShowTotalPrice = value;
        }
    }


    /// <summary>
    /// True - total price is shown always, no matter if product has product options
    /// </summary>
    public bool AlwaysShowTotalPrice
    {
        get
        {
            return mAlwaysShowTotalPrice;
        }
        set
        {
            mAlwaysShowTotalPrice = value;
        }
    }


    /// <summary>
    /// Text which is displayed next to the formatted total price. If empty, default text is used.
    /// </summary>
    public string TotalPriceLabel
    {
        get;
        set;
    }


    /// <summary>
    /// Quantity of the specified product to add to the shopping cart.
    /// </summary>
    public int Quantity
    {
        get
        {
            if (ShowUnitsTextBox)
            {
                return ValidationHelper.GetInteger(txtUnits.Text.Trim(), DefaultQuantity);
            }
            else
            {
                return DefaultQuantity;
            }
        }
    }


    /// <summary>
    /// Price of the product set by user.
    /// </summary>
    public double Price
    {
        get
        {
            return donationProperties.DonationAmount;
        }
    }


    /// <summary>
    /// Indicates if product is added to shopping cart as private.
    /// </summary>
    public bool IsPrivate
    {
        get
        {
            return donationProperties.DonationIsPrivate;
        }
    }


    /// <summary>
    /// Product options selected by inner selectors (string of SKUIDs separated byt the comma).
    /// </summary>
    public string ProductOptions
    {
        get
        {
            string options = "";

            // Get product options from selectors
            foreach (Control selector in pnlSelectors.Controls)
            {
                if (selector is ProductOptionSelector)
                {
                    options += ((ProductOptionSelector)selector).GetSelectedSKUOptions() + ",";
                }
            }

            return options.TrimEnd(',');
        }
    }


    /// <summary>
    /// List containing product options selected by inner selectors.
    /// </summary>
    public List<ShoppingCartItemParameters> ProductOptionsParameters
    {
        get
        {
            List<ShoppingCartItemParameters> options = new List<ShoppingCartItemParameters>();

            // Get product options from selectors
            foreach (Control selector in pnlSelectors.Controls)
            {
                if (selector is ProductOptionSelector)
                {
                    options.AddRange(((ProductOptionSelector)selector).GetSelectedOptionsParameters());
                }
            }

            return options;
        }
    }


    /// <summary>
    /// Image folder, default image folder is '~/App_Themes/(Skin_Folder)/Images/ShoppingCart/'.
    /// </summary>
    public string ImageFolder
    {
        get
        {
            if (mImageFolder == "")
            {
                // Set default image folder
                mImageFolder = GetImageUrl("ShoppingCart/");
            }

            return mImageFolder;
        }
        set
        {
            mImageFolder = value;
        }
    }


    /// <summary>
    /// Shopping cart url. By default Shopping cart url from SiteManager settings is returned.
    /// </summary>
    public string ShoppingCartUrl
    {
        get
        {
            if (mShoppingCartUrl == "")
            {
                mShoppingCartUrl = ECommerceSettings.ShoppingCartURL(CMSContext.CurrentSiteName);
            }
            return mShoppingCartUrl;
        }
        set
        {
            mShoppingCartUrl = value;
        }
    }


    /// <summary>
    /// Inidicates if user has to be redirected to shopping cart after adding an item to cart. Default value is taken from
    /// Ecommerce settings (keyname "CMSStoreRedirectToShoppingCart").
    /// </summary>
    public bool RedirectToShoppingCart
    {
        get
        {
            if (!mRedirectToShoppingCart.HasValue)
            {
                mRedirectToShoppingCart = ECommerceSettings.RedirectToShoppingCart(CMSContext.CurrentSiteName);
            }

            return mRedirectToShoppingCart.Value;
        }
        set
        {
            mRedirectToShoppingCart = value;
        }
    }


    /// <summary>
    /// Wishlist url. By default WIshlist url from SiteManager settings is returned.
    /// </summary>
    public string WishlistUrl
    {
        get
        {
            if (mWishlistUrl == "")
            {
                mWishlistUrl = ECommerceSettings.WishListURL(CMSContext.CurrentSiteName);
            }
            return mWishlistUrl;
        }
        set
        {
            mWishlistUrl = value;
        }
    }


    /// <summary>
    /// File name of the image which is used as a source for image button to add product to the wishlist, default image folder is '~/App_Themes/(Skin_Folder)/Images/ShoppingCart/'.
    public string AddToWishlistImageButton
    {
        get
        {
            return mAddToWishlistImageButton;
        }
        set
        {
            mAddToWishlistImageButton = value;
        }
    }


    /// <summary>
    /// Simple string or localizable string of the link to add product to the wishlist.
    /// </summary>
    public string AddToWishlistLinkText
    {
        get
        {
            return mAddToWishlistLinkText;
        }
        set
        {
            mAddToWishlistLinkText = value;
        }
    }


    /// <summary>
    /// Shopping cart object required for formatting of the displayed prices.
    /// If it is not set, current shopping cart from E-commerce context is used.
    /// </summary>
    public ShoppingCartInfo ShoppingCart
    {
        get
        {
            if (mShoppingCart == null)
            {
                mShoppingCart = ECommerceContext.CurrentShoppingCart;
            }

            return mShoppingCart;
        }
        set
        {
            mShoppingCart = value;
        }
    }


    /// <summary>
    /// TRUE - product option price is displayed including tax, FALSE - product option price is displayed without tax.
    /// </summary>
    public bool ShowPriceIncludingTax
    {
        get
        {
            return mShowPriceIncludingTax;
        }
        set
        {
            mShowPriceIncludingTax = value;
        }
    }


    /// <summary>
    /// Indicates if shopping car item selector is modal dialog.
    /// </summary>
    public bool DialogMode
    {
        get
        {
            return mDialogMode;
        }
        set
        {
            mDialogMode = value;
        }
    }


    /// <summary>
    /// Control that is used to add the product to shopping cart.
    /// </summary>
    public Control AddToCartControl
    {
        get;
        private set;
    }


    /// <summary>
    /// Indicates if redirect to product details is enabled if other conditions are met.
    /// Set to true by default.
    /// </summary>
    public bool RedirectToDetailsEnabled
    {
        get
        {
            return mRedirectToDetailsEnabled;
        }
        set
        {
            mRedirectToDetailsEnabled = value;
        }
    }

    #endregion


    #region "Events"

    /// <summary>
    /// Fires when "Add to shopping cart" button is clicked, overrides original action.
    /// </summary>
    public event CancelEventHandler OnAddToShoppingCart;

    #endregion


    #region "Lifecycle"

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        lnkAdd.Click += new EventHandler(lnkAdd_Click);
        btnAdd.Click += new EventHandler(btnAdd_Click);
        lnkWishlist.Click += new EventHandler(lnkWishlist_Click);
        btnWishlist.Click += new ImageClickEventHandler(btnWishlist_Click);

        ltlScript.Text = ScriptHelper.GetScript("function ShoppingCartItemAddedHandler(message) { alert(message); }");
    }


    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        ReloadData();
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        if (SKUID > 0)
        {
            InitializeControls();

            if (!mDataLoaded)
            {
                ReloadData();
            }

            // Get count of the product options            
            if (AlwaysShowTotalPrice || (ShowTotalPrice && SKUHasOptions))
            {
                CalculateTotalPrice();

                if (!string.IsNullOrEmpty(TotalPriceLabel))
                {
                    // Use custom label
                    lblPrice.Text = GetString(TotalPriceLabel);
                }
                else
                {
                    // Use default label
                    if (ShowPriceIncludingTax)
                    {
                        lblPrice.Text = GetString("ShoppingCartItemSelector.TotalPriceIncludeTax");
                    }
                    else
                    {
                        lblPrice.Text = GetString("ShoppingCartItemSelector.TotalPriceWithoutTax");
                    }
                }

                pnlPrice.Visible = true;
            }
            else
            {
                // Hide total price container
                pnlPrice.Visible = false;
            }

            if (DialogMode)
            {
                pnlButton.CssClass += " PageFooterLine";
            }
        }

        lblUnits.Style.Add("display", "none");

        // Show panel only when some selectors loaded
        pnlSelectors.Visible = (ShowProductOptions && SKUHasOptions) || donationProperties.Visible;
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Reloads shopping cart item selector data.
    /// </summary>
    public void ReloadData()
    {
        if (SKUID <= 0)
        {
            return;
        }

        DebugHelper.SetContext("ShoppingCartItemSelector");

        InitializeControls();

        if (ShowProductOptions)
        {
            LoadProductOptions();
        }

        // If donation properties should be shown and SKU product type is donation
        if (ShowDonationProperties && (SKU != null) && (SKU.SKUProductType == SKUProductTypeEnum.Donation))
        {
            donationProperties.Visible = true;
            donationProperties.StopProcessing = false;
            donationProperties.SKU = SKU;
            donationProperties.ShoppingCart = ShoppingCart;
            donationProperties.ReloadData();
        }


        // Fill units textbox with default quantity
        if (ShowUnitsTextBox)
        {
            if (txtUnits.Text.Trim() == "")
            {
                txtUnits.Text = DefaultQuantity.ToString();
            }
        }

        mDataLoaded = true;
        DebugHelper.ReleaseContext();
    }


    /// <summary>
    /// Initializes controls.
    /// </summary>
    private void InitializeControls()
    {
        if (lnkAdd == null)
        {
            upnlAjax.LoadContainer();
        }

        if (SKUEnabled)
        {
            // Display/hide units textbox with default quantity
            if (ShowUnitsTextBox)
            {
                txtUnits.Visible = true;
                lblUnits.Visible = true;
                lblUnits.AssociatedControlID = "txtUnits";
            }

            // Dispaly link button
            if (!String.IsNullOrEmpty(AddToCartLinkText))
            {
                lnkAdd.Visible = true;
                lnkAdd.Text = GetString(AddToCartLinkText);
                lnkAdd.ToolTip = GetString(AddToCartTooltip);

                AddToCartControl = lnkAdd;
            }
            // Display image button
            else if (!String.IsNullOrEmpty(AddToCartImageButton))
            {
                btnAddImage.ImageUrl = GetImageButtonUrl(AddToCartImageButton);
                btnAddImage.Visible = true;
                btnAddImage.AlternateText = GetString(AddToCartTooltip);
                btnAddImage.ToolTip = GetString(AddToCartTooltip);

                AddToCartControl = btnAddImage;
            }
            // Display classic button
            else
            {
                btnAdd.Visible = true;
                btnAdd.Text = GetString(AddToCartText);
                btnAdd.ToolTip = GetString(AddToCartTooltip);

                AddToCartControl = btnAdd;
            }
        }

        // Display "Add to Wishlist" link 
        if (ShowWishlistLink)
        {
            if (AddToWishlistLinkText != "")
            {
                lnkWishlist.Visible = true;
                lnkWishlist.Text = ResHelper.LocalizeString(AddToWishlistLinkText);
                lnkWishlist.ToolTip = ResHelper.LocalizeString(AddToWishlistLinkText);
            }
            // Display "Add to Wishlist" image button
            else if (AddToWishlistImageButton != "")
            {
                // Image button
                btnWishlist.Visible = true;
                btnWishlist.ImageUrl = GetImageButtonUrl(AddToWishlistImageButton);
                btnWishlist.AlternateText = GetString("ShoppingCart.AddToWishlistToolTip");
                btnWishlist.ToolTip = GetString("ShoppingCart.AddToWishlistToolTip");
            }
        }
    }


    /// <summary>
    /// Returns path of the image which is used for the shopping cart selector button.
    /// </summary>
    /// <param name="imageButton">Image name or image relative path.</param>
    private string GetImageButtonUrl(string imageButton)
    {
        if (imageButton.StartsWithCSafe("~"))
        {
            return URLHelper.ResolveUrl(imageButton);
        }
        else
        {
            return ImageFolder.TrimEnd('/') + "/" + imageButton;
        }
    }


    /// <summary>
    /// Sets donation properties.
    /// </summary>
    /// <param name="amount">Donation amount in shopping cart currency.</param>
    /// <param name="isPrivate">Donation is private</param>
    /// <param name="units">Donation units</param>
    public void SetDonationProperties(double amount, bool isPrivate, int units)
    {
        donationProperties.DonationAmount = (amount > 0.0) ? amount : donationProperties.DonationAmount;
        donationProperties.DonationIsPrivate = isPrivate;
        DefaultQuantity = units;
    }


    /// <summary>
    /// Returns selected shopping cart item parameters containig product option parameters.
    /// </summary>
    public ShoppingCartItemParameters GetShoppingCartItemParameters()
    {
        // Get product options
        List<ShoppingCartItemParameters> options = ProductOptionsParameters;

        // Create params
        ShoppingCartItemParameters cartItemParams = new ShoppingCartItemParameters(SKUID, Quantity, options);

        // Ensure minimum allowed number of items is met
        if (SKU.SKUMinItemsInOrder > Quantity)
        {
            cartItemParams.Quantity = SKU.SKUMinItemsInOrder;
        }

        if (donationProperties.Visible || !RedirectToDetailsEnabled)
        {
            // Get exchange rate from cart currency to site main currency         
            double rate = (SKU.IsGlobal) ? ShoppingCart.ExchangeRateForGlobalItems : ShoppingCart.ExchangeRate;

            // Set donation specific shopping cart item parameters
            cartItemParams.IsPrivate = donationProperties.DonationIsPrivate;

            // Get donation amount in site main currency
            cartItemParams.Price = ExchangeRateInfoProvider.ApplyExchangeRate(donationProperties.DonationAmount, 1 / rate);
        }

        return cartItemParams;
    }


    private void lnkWishlist_Click(object sender, EventArgs e)
    {
        // Add product to wishlist
        AddProductToWishlist();
    }


    private void btnWishlist_Click(object sender, ImageClickEventArgs e)
    {
        // Add product to wishlist
        AddProductToWishlist();
    }


    private void lnkAdd_Click(object sender, EventArgs e)
    {
        // Add product to shoppingcart
        AddProductToShoppingCart();
    }


    protected void btnAddImage_Click(object sender, ImageClickEventArgs e)
    {
        // Add product to shoppingcart
        AddProductToShoppingCart();
    }


    private void btnAdd_Click(object sender, EventArgs e)
    {
        // Add product to shoppingcart
        AddProductToShoppingCart();
    }


    private void AddProductToWishlist()
    {
        SessionHelper.SetValue("ShoppingCartUrlReferrer", URLHelper.CurrentURL);
        URLHelper.Redirect(WishlistUrl + "?productid=" + SKUID);
    }


    /// <summary>
    /// Validates shopping cart item selector input data.
    /// </summary>    
    private bool IsValid()
    {
        // Validates all product options
        if (ShowProductOptions)
        {
            foreach (Control selector in pnlSelectors.Controls)
            {
                if (selector is ProductOptionSelector)
                {
                    // End if invalid option found
                    if (!((ProductOptionSelector)selector).IsValid())
                    {
                        return false;
                    }
                }
            }
        }

        return true;
    }


    /// <summary>
    /// Adds product to the shopping cart.
    /// </summary>
    private void AddProductToShoppingCart()
    {
        // Validate input data
        if (!IsValid() || (SKU == null))
        {
            // Do not proces
            return;
        }

        if (RedirectToDetailsEnabled)
        {
            if (!ShowProductOptions && !ShowDonationProperties)
            {
                // Does product have some enabled product option categories?
                bool hasOptions = !DataHelper.DataSourceIsEmpty(OptionCategoryInfoProvider.GetSKUOptionCategories(SKUID, true));

                // Is product a customizable donation?
                bool isCustomizableDonation = ((SKU != null)
                    && (SKU.SKUProductType == SKUProductTypeEnum.Donation)
                    && (!((SKU.SKUPrice == SKU.SKUMinPrice) && (SKU.SKUPrice == SKU.SKUMaxPrice)) || SKU.SKUPrivateDonation));

                if (hasOptions || isCustomizableDonation)
                {
                    // Redirect to product details
                    URLHelper.Redirect("~/CMSPages/GetProduct.aspx?productid=" + SKUID);
                }
            }
        }

        // Get cart item parameters
        ShoppingCartItemParameters cartItemParams = GetShoppingCartItemParameters();

        string error = null;

        // Check if customer is enabled
        if ((ShoppingCart.Customer != null) && (!ShoppingCart.Customer.CustomerEnabled))
        {
            error = GetString("ecommerce.cartcontent.customerdisabled");
        }

        // Check if it is possible to add this item to shopping cart
        if ((error == null) && !ShoppingCartInfoProvider.CheckNewShoppingCartItems(ShoppingCart, cartItemParams))
        {
            error = String.Format(GetString("ecommerce.cartcontent.productdisabled"), SKU.SKUName);
        }

        if (!string.IsNullOrEmpty(error))
        {
            // Show error message and cancel adding the product to shopping cart
            ScriptHelper.RegisterStartupScript(Page, typeof(string), "ShoppingCartAddItemErrorAlert", ScriptHelper.GetAlertScript(error));
            return;
        }

        // If donation properties are used and donation properties form is not valid
        if (donationProperties.Visible && !String.IsNullOrEmpty(donationProperties.Validate()))
        {
            return;
        }

        // Fire on add to shopping cart event
        CancelEventArgs eventArgs = new CancelEventArgs();
        if (OnAddToShoppingCart != null)
        {
            OnAddToShoppingCart(this, eventArgs);
        }

        // If adding to shopping cart was cancelled
        if (eventArgs.Cancel)
        {
            return;
        }

        // Get cart item parameters in case something changed
        cartItemParams = GetShoppingCartItemParameters();

        // Log activity
        LogProductAddedToSCActivity(SKUID, SKU.SKUName, Quantity);

        if (ShoppingCart != null)
        {
            bool updateCart = false;

            // Assign current shopping cart to current user
            CurrentUserInfo ui = CMSContext.CurrentUser;
            if (!ui.IsPublic())
            {
                ShoppingCart.User = ui;
                updateCart = true;
            }

            // Shopping cart is not saved yet
            if (ShoppingCart.ShoppingCartID == 0)
            {
                updateCart = true;
            }

            // Update shopping cart when required
            if (updateCart)
            {
                ShoppingCartInfoProvider.SetShoppingCartInfo(ShoppingCart);
            }

            // Add item to shopping cart
            ShoppingCartItemInfo addedItem = ShoppingCart.SetShoppingCartItem(cartItemParams);

            if (addedItem != null)
            {
                // Update shopping cart item in database
                ShoppingCartItemInfoProvider.SetShoppingCartItemInfo(addedItem);

                // Update product options in database
                foreach (ShoppingCartItemInfo option in addedItem.ProductOptions)
                {
                    ShoppingCartItemInfoProvider.SetShoppingCartItemInfo(option);
                }

                // Update bundle items in database
                foreach (ShoppingCartItemInfo bundleItem in addedItem.BundleItems)
                {
                    ShoppingCartItemInfoProvider.SetShoppingCartItemInfo(bundleItem);
                }

                // Track 'Add to shopping cart' conversion
                ECommerceHelper.TrackAddToShoppingCartConversion(addedItem);

                // If user has to be redirected to shopping cart
                if (RedirectToShoppingCart)
                {
                    // Set shopping cart referrer
                    SessionHelper.SetValue("ShoppingCartUrlReferrer", URLHelper.CurrentURL);

                    // Ensure shopping cart update
                    SessionHelper.SetValue("checkinventory", true);

                    // Redirect to shopping cart
                    URLHelper.Redirect(ShoppingCartUrl);
                }
                else
                {
                    // Localize SKU name
                    string skuName = (addedItem.SKU != null) ? ResHelper.LocalizeString(addedItem.SKU.SKUName) : "";

                    // Check inventory
                    ShoppingCartCheckResult checkResult = ShoppingCartInfoProvider.CheckShoppingCart(ShoppingCart);
                    string checkInventoryMessage = checkResult.GetFormattedMessage();

                    // Get prodcut added message
                    string message = String.Format(GetString("com.productadded"), skuName);

                    // Add inventory check message
                    if (!String.IsNullOrEmpty(checkInventoryMessage))
                    {
                        message += "\n\n" + checkInventoryMessage;
                    }

                    // Count and show total price with options
                    CalculateTotalPrice();

                    // Register the call of JS handler informing about added product
                    ScriptHelper.RegisterStartupScript(Page, typeof(string), "ShoppingCartItemAddedHandler", "if (typeof ShoppingCartItemAddedHandler == 'function') { ShoppingCartItemAddedHandler(" + ScriptHelper.GetString(message) + "); }", true);
                }
            }
        }
    }


    /// <summary>
    /// Loads product options.
    /// </summary>
    private void LoadProductOptions()
    {
        DataSet dsCategories = null;

        if (IsLiveSite)
        {
            // Get cache minutes
            int cacheMinutes = SettingsKeyProvider.GetIntValue(CMSContext.CurrentSiteName + ".CMSCacheMinutes");

            // Try to get data from cache
            using (CachedSection<DataSet> cs = new CachedSection<DataSet>(ref dsCategories, cacheMinutes, true, null, "skuoptioncategories", SKUID))
            {
                if (cs.LoadData)
                {
                    // Get the data
                    dsCategories = OptionCategoryInfoProvider.GetSKUOptionCategories(SKUID, true);

                    // Save to the cache
                    if (cs.Cached)
                    {
                        // Get dependencies
                        List<string> dependencies = new List<string>();
                        dependencies.Add("ecommerce.sku|byid|" + SKUID);
                        dependencies.Add("ecommerce.optioncategory|all");

                        // Set dependencies
                        cs.CacheDependency = CacheHelper.GetCacheDependency(dependencies);
                    }

                    cs.Data = dsCategories;
                }
            }
        }
        else
        {
            // Get the data
            dsCategories = OptionCategoryInfoProvider.GetSKUOptionCategories(SKUID, true);
        }

        // Initialize product option selectors
        if (!DataHelper.DataSourceIsEmpty(dsCategories))
        {
            mProductOptions = new Hashtable();

            foreach (DataRow dr in dsCategories.Tables[0].Rows)
            {
                try
                {
                    // Load control for selection product options
                    ProductOptionSelector selector = (ProductOptionSelector)LoadUserControl("~/CMSModules/Ecommerce/Controls/ProductOptions/ProductOptionSelector.ascx");

                    // Add control to the collection
                    selector.ID = "opt" + ValidationHelper.GetInteger(dr["CategoryID"], 0);

                    // Init selector
                    selector.LocalShoppingCartObj = ShoppingCart;
                    selector.ShowPriceIncludingTax = ShowPriceIncludingTax;
                    selector.IsLiveSite = IsLiveSite;
                    selector.SKUID = SKUID;
                    selector.OptionCategory = new OptionCategoryInfo(dr);

                    // Load all product options
                    foreach (DictionaryEntry entry in selector.ProductOptions)
                    {
                        mProductOptions[entry.Key] = entry.Value;
                    }

                    if (AlwaysShowTotalPrice || ShowTotalPrice)
                    {
                        ListControl lc = selector.SelectionControl as ListControl;
                        if (lc != null)
                        {
                            // Add Index change handler
                            lc.AutoPostBack = true;
                        }
                    }

                    pnlSelectors.Controls.Add(selector);
                }
                catch
                {
                }
            }
        }
    }


    /// <summary>
    /// Calculate total price with product options prices.
    /// </summary>
    private void CalculateTotalPrice()
    {
        // Add SKU price        
        double price = SKUInfoProvider.GetSKUPrice(SKU, ShoppingCart, true, ShowPriceIncludingTax);

        // Add prices of all product options
        List<ShoppingCartItemParameters> optionParams = ProductOptionsParameters;

        foreach (ShoppingCartItemParameters optionParam in optionParams)
        {
            SKUInfo sku = null;

            if (mProductOptions.Contains(optionParam.SKUID))
            {
                // Get option SKU data
                sku = (SKUInfo)mProductOptions[optionParam.SKUID];

                if (sku.SKUProductType != SKUProductTypeEnum.Text)
                {
                    // Add option price                
                    price += SKUInfoProvider.GetSKUPrice(sku, ShoppingCart, true, ShowPriceIncludingTax);
                }
            }
        }

        // Convert to shopping cart currency
        price = ShoppingCart.ApplyExchangeRate(price);

        lblPriceValue.Text = CurrencyInfoProvider.GetFormattedPrice(price, ShoppingCart.Currency);
    }


    /// <summary>
    /// Logs activity
    /// </summary>
    /// <param name="skuId">Product ID</param>
    /// <param name="skuName">Product name</param>
    /// <param name="Quantity">Quantity</param>
    private void LogProductAddedToSCActivity(int skuId, string skuName, int Quantity)
    {
        Activity activity = new ActivityProductAddedToShoppingCart(skuName, Quantity, skuId, CMSContext.ActivityEnvironmentVariables);
        activity.Log();
    }

    #endregion
}