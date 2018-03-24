using System;
using System.Data;
using System.Text;
using System.Web.UI.WebControls;

using CMS.CMSHelper;
using CMS.Ecommerce;
using CMS.EcommerceProvider;
using CMS.ExtendedControls;
using CMS.GlobalHelper;
using CMS.PortalEngine;
using CMS.SiteProvider;
using CMS.URLRewritingEngine;
using CMS.WebAnalytics;
using CMS.EventLog;

public partial class CMSModules_Ecommerce_Controls_ShoppingCart_ShoppingCartContent : ShoppingCartStep
{
    #region "Variables"

    protected Button btnReload = null;
    protected Button btnAddProduct = null;
    protected HiddenField hidProductID = null;
    protected HiddenField hidQuantity = null;
    protected HiddenField hidOptions = null;

    protected Nullable<bool> mEnableEditMode = null;
    protected bool checkInventory = false;

    #endregion


    #region Properties

    /// <summary>
    /// Indicates whether there are another checkout process steps after the current step, except payment.
    /// </summary>
    private bool ExistAnotherStepsExceptPayment
    {
        get
        {
            return (ShoppingCartControl.CurrentStepIndex + 2 <= ShoppingCartControl.CheckoutProcessSteps.Count - 1);
        }
    }

    #endregion


    /// <summary>
    /// Child control creation.
    /// </summary>
    protected override void CreateChildControls()
    {
        // Add product button
        btnAddProduct = new CMSButton();
        btnAddProduct.Attributes["style"] = "display: none";
        Controls.Add(btnAddProduct);
        btnAddProduct.Click += new EventHandler(btnAddProduct_Click);
        selectCurrency.UniSelector.OnSelectionChanged += delegate(object sender, EventArgs args)
        {
            btnUpdate_Click1(null, null);
        };
        selectCurrency.DropDownSingleSelect.AutoPostBack = true;

        // Add the hidden fields for productId, quantity and product options
        hidProductID = new HiddenField();
        hidProductID.ID = "hidProductID";
        Controls.Add(hidProductID);

        hidQuantity = new HiddenField();
        hidQuantity.ID = "hidQuantity";
        Controls.Add(hidQuantity);

        hidOptions = new HiddenField();
        hidOptions.ID = "hidOptions";
        Controls.Add(hidOptions);
    }


    protected override void OnPreRender(EventArgs e)
    {
        // Register add product script
        ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "AddProductScript",
                                               ScriptHelper.GetScript(
                                                   "function setProduct(val) { document.getElementById('" + hidProductID.ClientID + "').value = val; } \n" +
                                                   "function setQuantity(val) { document.getElementById('" + hidQuantity.ClientID + "').value = val; } \n" +
                                                   "function setOptions(val) { document.getElementById('" + hidOptions.ClientID + "').value = val; } \n" +
                                                   "function setPrice(val) { document.getElementById('" + hdnPrice.ClientID + "').value = val; } \n" +
                                                   "function setIsPrivate(val) { document.getElementById('" + hdnIsPrivate.ClientID + "').value = val; } \n" +
                                                   "function AddProduct(productIDs, quantities, options, price, isPrivate) { \n" +
                                                   "setProduct(productIDs); \n" +
                                                   "setQuantity(quantities); \n" +
                                                   "setOptions(options); \n" +
                                                   "setPrice(price); \n" +
                                                   "setIsPrivate(isPrivate); \n" +
                                                   Page.ClientScript.GetPostBackEventReference(btnAddProduct, null) +
                                                   ";} \n" +
                                                   "function RefreshCart() {" +
                                                   Page.ClientScript.GetPostBackEventReference(btnAddProduct, null) +
                                                   ";} \n"
                                                   ));

        // Register dialog script
        ScriptHelper.RegisterDialogScript(Page);

        // Hide columns with identifiers
        gridData.Columns[0].Visible = false;
        gridData.Columns[1].Visible = false;
        gridData.Columns[2].Visible = false;
        gridData.Columns[3].Visible = false;

        // Hide actions column
        gridData.Columns[5].Visible = (ShoppingCartControl.CheckoutProcessType == CheckoutProcessEnum.CMSDeskOrderItems) ||
                                      (ShoppingCartControl.CheckoutProcessType == CheckoutProcessEnum.CMSDeskOrder);

        // Disable specific controls
        if (!Enabled)
        {
            lnkNewItem.Enabled = false;
            lnkNewItem.OnClientClick = "";
            selectCurrency.Enabled = false;
            btnEmpty.Enabled = false;
            btnUpdate.Enabled = false;
            chkSendEmail.Enabled = false;
        }

        // Show/Hide dropdownlist with currencies
        pnlCurrency.Visible &= (selectCurrency.UniSelector.HasData && selectCurrency.DropDownSingleSelect.Items.Count > 1);

        // Check session parameters for inventory check
        if (ValidationHelper.GetBoolean(SessionHelper.GetValue("checkinventory"), false))
        {
            checkInventory = true;
            SessionHelper.Remove("checkinventory");
        }

        // Check inventory
        if (checkInventory)
        {
            ShoppingCartCheckResult checkResult = ShoppingCartInfoProvider.CheckShoppingCart(ShoppingCart);

            if (checkResult.CheckFailed)
            {
                lblError.Text = checkResult.GetHTMLFormattedMessage();
            }
        }

        // Display messages if required
        lblError.Visible = !string.IsNullOrEmpty(lblError.Text.Trim());
        lblInfo.Visible = !string.IsNullOrEmpty(lblInfo.Text.Trim());

        base.OnPreRender(e);
    }


    protected override void OnLoad(EventArgs e)
    {
        lblTitle.Text = GetString("shoppingcart.cartcontent");

        if ((ShoppingCart != null) && (ShoppingCart.CountryID == 0) && (CMSContext.CurrentSite != null))
        {
            string countryName = ECommerceSettings.DefaultCountryName(CMSContext.CurrentSite.SiteName);
            CountryInfo ci = CountryInfoProvider.GetCountryInfo(countryName);
            ShoppingCart.CountryID = (ci != null) ? ci.CountryID : 0;

            // Set currency selectors site ID
            selectCurrency.SiteID = ShoppingCart.ShoppingCartSiteID;
        }

        imgNewItem.ImageUrl = GetImageUrl("Objects/Ecommerce_OrderItem/add.png");
        lblCurrency.Text = GetString("ecommerce.shoppingcartcontent.currency");
        lblCoupon.Text = GetString("ecommerce.shoppingcartcontent.coupon");
        lnkNewItem.Text = GetString("ecommerce.shoppingcartcontent.additem");
        imgNewItem.AlternateText = lnkNewItem.Text;
        btnUpdate.Text = GetString("ecommerce.shoppingcartcontent.btnupdate");
        btnEmpty.Text = GetString("ecommerce.shoppingcartcontent.btnempty");
        btnEmpty.OnClientClick = string.Format("return confirm({0});", ScriptHelper.GetString(GetString("ecommerce.shoppingcartcontent.emptyconfirm")));
        lnkNewItem.OnClientClick = string.Format("OpenAddProductDialog('{0}');return false;", GetCMSDeskShoppingCartSessionName());

        gridData.Columns[4].HeaderText = GetString("general.remove");
        gridData.Columns[5].HeaderText = GetString("ecommerce.shoppingcartcontent.actions");
        gridData.Columns[6].HeaderText = GetString("ecommerce.shoppingcartcontent.skuname");
        gridData.Columns[7].HeaderText = GetString("ecommerce.shoppingcartcontent.skuunits");
        gridData.Columns[8].HeaderText = GetString("ecommerce.shoppingcartcontent.unitprice");
        gridData.Columns[9].HeaderText = GetString("ecommerce.shoppingcartcontent.unitdiscount");
        gridData.Columns[10].HeaderText = GetString("ecommerce.shoppingcartcontent.tax");
        gridData.Columns[11].HeaderText = GetString("ecommerce.shoppingcartcontent.subtotal");
        gridData.RowDataBound += new GridViewRowEventHandler(gridData_RowDataBound);

        // Register product price detail dialog script
        StringBuilder script = new StringBuilder();
        script.Append("function ShowProductPriceDetail(cartItemGuid, sessionName) {");
        script.Append("if (sessionName != \"\"){sessionName =  \"&cart=\" + sessionName;}");
        string detailUrl = (IsLiveSite) ? "~/CMSModules/Ecommerce/CMSPages/ShoppingCartSKUPriceDetail.aspx" : "~/CMSModules/Ecommerce/Controls/ShoppingCart/ShoppingCartSKUPriceDetail.aspx";
        script.Append(string.Format("modalDialog('{0}?itemguid=' + cartItemGuid + sessionName, 'ProductPriceDetail', 750, 500); }}", ResolveUrl(detailUrl)));
        ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "ProductPriceDetail", ScriptHelper.GetScript(script.ToString()));

        // Register add order item dialog script
        script = new StringBuilder();
        script.Append("function OpenOrderItemDialog(cartItemGuid, sessionName) {");
        script.Append("if (sessionName != \"\"){sessionName =  \"&cart=\" + sessionName;}");
        script.Append(string.Format("modalDialog('{0}?itemguid=' + cartItemGuid + sessionName, 'OrderItemEdit', 500, 350); }}", ResolveUrl("~/CMSModules/Ecommerce/Controls/ShoppingCart/OrderItemEdit.aspx")));
        ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "OrderItemEdit", ScriptHelper.GetScript(script.ToString()));

        script = new StringBuilder();
        string addProductUrl = CMSContext.ResolveDialogUrl("~/CMSModules/Ecommerce/Pages/Tools/Orders/Order_Edit_AddItems.aspx");
        script.AppendFormat("var addProductDialogURL = '{0}'", URLHelper.GetAbsoluteUrl(addProductUrl));
        ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "AddProduct", ScriptHelper.GetScript(script.ToString()));


        // Hide "add product" action for live site order
        if (!ShoppingCartControl.IsInternalOrder)
        {
            pnlNewItem.Visible = false;

            ShoppingCartControl.ButtonBack.Text = GetString("ecommerce.cartcontent.buttonbacktext");
            ShoppingCartControl.ButtonBack.CssClass = "LongButton";
            ShoppingCartControl.ButtonNext.Text = GetString("ecommerce.cartcontent.buttonnexttext");

            if (!ShoppingCartControl.IsCurrentStepPostBack)
            {
                // Get shopping cart item parameters from URL
                ShoppingCartItemParameters itemParams = ShoppingCartItemParameters.GetShoppingCartItemParameters();

                // Set item in the shopping cart
                AddProducts(itemParams);
            }
        }

        // Set sending order notification when editing existing order according to the site settings
        if (ShoppingCartControl.CheckoutProcessType == CheckoutProcessEnum.CMSDeskOrderItems)
        {
            if (!ShoppingCartControl.IsCurrentStepPostBack)
            {
                if (!string.IsNullOrEmpty(ShoppingCart.SiteName))
                {
                    chkSendEmail.Checked = ECommerceSettings.SendOrderNotification(ShoppingCart.SiteName);
                }
            }
            // Show send email checkbox
            chkSendEmail.Visible = true;
            chkSendEmail.Text = GetString("shoppingcartcontent.sendemail");

            // Setup buttons
            ShoppingCartControl.ButtonBack.Visible = false;
            ShoppingCart.CheckAvailableItems = false;
            ShoppingCartControl.ButtonNext.Text = GetString("general.next");

            if ((!ExistAnotherStepsExceptPayment) && (ShoppingCartControl.PaymentGatewayProvider == null))
            {
                ShoppingCartControl.ButtonNext.Text = GetString("general.ok");
            }
        }

        // Fill dropdownlist
        if (!ShoppingCartControl.IsCurrentStepPostBack)
        {
            if ((ShoppingCart.CartItems.Count > 0) || ShoppingCartControl.IsInternalOrder)
            {
                if (ShoppingCart.ShoppingCartCurrencyID == 0)
                {
                    // Select customer preferred currency                    
                    if (ShoppingCart.Customer != null)
                    {
                        CustomerInfo customer = ShoppingCart.Customer;
                        ShoppingCart.ShoppingCartCurrencyID = (customer.CustomerUser != null) ? customer.CustomerUser.GetUserPreferredCurrencyID(CMSContext.CurrentSiteName) : 0;
                    }
                }

                if (ShoppingCart.ShoppingCartCurrencyID == 0)
                {
                    if (CMSContext.CurrentSite != null)
                    {
                        ShoppingCart.ShoppingCartCurrencyID = CMSContext.CurrentSite.SiteDefaultCurrencyID;
                    }
                }

                selectCurrency.CurrencyID = ShoppingCart.ShoppingCartCurrencyID;

                if (ShoppingCart.ShoppingCartDiscountCouponID > 0)
                {
                    // Fill textbox with discount coupon code
                    DiscountCouponInfo dci = DiscountCouponInfoProvider.GetDiscountCouponInfo(ShoppingCart.ShoppingCartDiscountCouponID);
                    if (dci != null)
                    {
                        if (ShoppingCart.IsCreatedFromOrder || dci.IsValid)
                        {
                            txtCoupon.Text = dci.DiscountCouponCode;
                        }
                        else
                        {
                            ShoppingCart.ShoppingCartDiscountCouponID = 0;
                        }
                    }
                }
            }

            ReloadData();
        }

        // Check if customer is enabled
        if ((ShoppingCart.Customer != null) && (!ShoppingCart.Customer.CustomerEnabled))
        {
            HideCartContent(GetString("ecommerce.cartcontent.customerdisabled"));
        }

        // Turn on available items checking after content is loaded
        ShoppingCart.CheckAvailableItems = true;

        base.OnLoad(e);
    }


    private void gridData_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            // Set enabled for order item units editing
            e.Row.Cells[7].Enabled = Enabled;
        }
    }


    protected void btnUpdate_Click1(object sender, EventArgs e)
    {
        if (ShoppingCart != null)
        {
            ShoppingCart.ShoppingCartCurrencyID = ValidationHelper.GetInteger(selectCurrency.CurrencyID, 0);

            // Skip if method was called by btnAddProduct
            if (sender != btnAddProduct)
            {
                foreach (GridViewRow row in gridData.Rows)
                {
                    // Get shopping cart item Guid
                    Guid cartItemGuid = ValidationHelper.GetGuid(((Label)row.Cells[1].Controls[1]).Text, Guid.Empty);

                    // Try to find shopping cart item in the list
                    ShoppingCartItemInfo cartItem = ShoppingCart.GetShoppingCartItem(cartItemGuid);
                    if (cartItem != null)
                    {
                        // If product and its product options should be removed
                        if (((CheckBox)row.Cells[4].Controls[1]).Checked && (sender != null))
                        {
                            // Remove product and its product option from list
                            ShoppingCart.RemoveShoppingCartItem(cartItemGuid);

                            if (!ShoppingCartControl.IsInternalOrder)
                            {
                                // Log activity
                                if (!cartItem.IsProductOption && !cartItem.IsBundleItem)
                                {
                                    Activity activity = new ActivityProductRemovedFromShoppingCart(cartItem, ResHelper.LocalizeString(cartItem.SKU.SKUName), ContactID, CMSContext.ActivityEnvironmentVariables);
                                    activity.Log();
                                }

                                // Delete product from database
                                ShoppingCartItemInfoProvider.DeleteShoppingCartItemInfo(cartItem);
                            }
                        }
                        // If product units has changed
                        else if (!cartItem.IsProductOption)
                        {
                            // Get number of units
                            int itemUnits = ValidationHelper.GetInteger(((TextBox)(row.Cells[7].Controls[1])).Text.Trim(), 0);
                            if ((itemUnits > 0) && (cartItem.CartItemUnits != itemUnits))
                            {
                                // Update units of the parent product
                                cartItem.CartItemUnits = itemUnits;

                                // Update units of the child product options
                                foreach (ShoppingCartItemInfo option in cartItem.ProductOptions)
                                {
                                    option.CartItemUnits = itemUnits;
                                }

                                // Update units of child bundle items
                                foreach (ShoppingCartItemInfo bundleItem in cartItem.BundleItems)
                                {
                                    bundleItem.CartItemUnits = itemUnits;
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
                            }
                        }
                    }
                }
            }

            CheckDiscountCoupon();

            ReloadData();

            if ((ShoppingCart.ShoppingCartDiscountCouponID > 0) && (!ShoppingCart.IsDiscountCouponApplied))
            {
                // Discount coupon code is valid but not applied to any product of the shopping cart
                lblError.Text = GetString("shoppingcart.discountcouponnotapplied");
            }

            // Inventory shloud be checked
            checkInventory = true;
        }
    }


    protected void btnEmpty_Click1(object sender, EventArgs e)
    {
        if (ShoppingCart != null)
        {
            // Log activity "product removed" for all items in shopping cart
            string siteName = CMSContext.CurrentSiteName;
            if (!ShoppingCartControl.IsInternalOrder)
            {
                ShoppingCartControl.TrackActivityAllProductsRemovedFromShoppingCart(ShoppingCart, siteName, ContactID);
            }

            ShoppingCartInfoProvider.EmptyShoppingCart(ShoppingCart);

            ReloadData();
        }
    }


    /// <summary>
    /// Validates this step.
    /// </summary>
    public override bool IsValid()
    {
        // Check modify permissions
        if (ShoppingCartControl.CheckoutProcessType == CheckoutProcessEnum.CMSDeskOrderItems)
        {
            // Check 'ModifyOrders' permission
            if (!ECommerceContext.IsUserAuthorizedForPermission("ModifyOrders"))
            {
                CMSEcommercePage.RedirectToCMSDeskAccessDenied("CMS.Ecommerce", "EcommerceModify OR ModifyOrders");
            }
        }

        // Allow to go to the next step only if shopping cart contains some products
        bool IsValid = (ShoppingCart.CartItems.Count > 0);

        if (!IsValid)
        {
            HideCartContentWhenEmpty();
        }

        if (ShoppingCart.IsCreatedFromOrder)
        {
            IsValid = true;
        }

        if (!IsValid)
        {
            lblError.Text = GetString("ecommerce.error.insertsomeproducts");
        }

        return IsValid;
    }


    /// <summary>
    /// Process this step.
    /// </summary>
    public override bool ProcessStep()
    {
        // Shopping cart units are already saved in database (on "Update" or on "btnAddProduct_Click" actions) 
        bool isOK = false;

        if (ShoppingCart != null)
        {
            // Reload data
            ReloadData();

            // Check available items before "Check out"
            ShoppingCartCheckResult checkResult = ShoppingCartInfoProvider.CheckShoppingCart(ShoppingCart);

            if (checkResult.CheckFailed)
            {
                lblError.Text = checkResult.GetHTMLFormattedMessage();
            }
            else if (ShoppingCartControl.CheckoutProcessType == CheckoutProcessEnum.CMSDeskOrderItems)
            {
                // Indicates wheter order saving process is successful
                isOK = true;

                try
                {
                    ShoppingCartInfoProvider.SetOrder(ShoppingCart);
                }
                catch (Exception ex)
                {
                    // Log exception
                    EventLogProvider.LogException("Shopping cart", "SAVEORDER", ex, ShoppingCart.ShoppingCartSiteID, null);
                    isOK = false;
                }

                if (isOK)
                {
                    lblInfo.Text = GetString("general.changessaved");

                    // Send order notification when editing existing order
                    if (ShoppingCartControl.CheckoutProcessType == CheckoutProcessEnum.CMSDeskOrderItems)
                    {
                        if (chkSendEmail.Checked)
                        {
                            OrderInfoProvider.SendOrderNotificationToAdministrator(ShoppingCart);
                            OrderInfoProvider.SendOrderNotificationToCustomer(ShoppingCart);
                        }
                    }
                    // Send order notification emails when on the live site
                    else if (ECommerceSettings.SendOrderNotification(CMSContext.CurrentSite.SiteName))
                    {
                        OrderInfoProvider.SendOrderNotificationToAdministrator(ShoppingCart);
                        OrderInfoProvider.SendOrderNotificationToCustomer(ShoppingCart);
                    }
                }
                else
                {
                    lblError.Text = GetString("ecommerce.orderpreview.errorordersave");
                }
            }
            // Go to the next step
            else
            {
                // Save other options
                if (!ShoppingCartControl.IsInternalOrder)
                {
                    ShoppingCartInfoProvider.SetShoppingCartInfo(ShoppingCart);
                }

                isOK = true;
            }
        }

        return isOK;
    }


    private void btnAddProduct_Click(object sender, EventArgs e)
    {
        // Get strings with productIDs and quantities separated by ';'
        string productIDs = ValidationHelper.GetString(hidProductID.Value, "");
        string quantities = ValidationHelper.GetString(hidQuantity.Value, "");
        string options = ValidationHelper.GetString(hidOptions.Value, "");
        double price = ValidationHelper.GetDouble(hdnPrice.Value, -1);
        bool isPrivate = ValidationHelper.GetBoolean(hdnIsPrivate.Value, false);

        // Add new products to shopping cart
        if ((productIDs != "") && (quantities != ""))
        {
            string[] arrID = productIDs.TrimEnd(';').Split(';');
            string[] arrQuant = quantities.TrimEnd(';').Split(';');
            int[] intOptions = ValidationHelper.GetIntegers(options.Split(','), 0);

            lblError.Text = "";

            for (int i = 0; i < arrID.Length; i++)
            {
                int skuId = ValidationHelper.GetInteger(arrID[i], 0);

                SKUInfo skuInfo = SKUInfoProvider.GetSKUInfo(skuId);
                if (skuInfo != null)
                {
                    int quant = ValidationHelper.GetInteger(arrQuant[i], 0);

                    ShoppingCartItemParameters cartItemParams = new ShoppingCartItemParameters(skuId, quant, intOptions);

                    // If product is donation
                    if (skuInfo.SKUProductType == SKUProductTypeEnum.Donation)
                    {
                        // Get donation properties
                        if (price < 0)
                        {
                            cartItemParams.Price = SKUInfoProvider.GetSKUPrice(skuInfo, ShoppingCart, false, false);
                        }
                        else
                        {
                            cartItemParams.Price = price;
                        }

                        cartItemParams.IsPrivate = isPrivate;
                    }

                    // Add product to the shopping cart
                    ShoppingCart.SetShoppingCartItem(cartItemParams);

                    // Log activity
                    string siteName = CMSContext.CurrentSiteName;
                    if (!ShoppingCartControl.IsInternalOrder)
                    {
                        ShoppingCartControl.TrackActivityProductAddedToShoppingCart(skuId, ResHelper.LocalizeString(skuInfo.SKUName), ContactID, siteName, URLHelper.CurrentRelativePath, quant);
                    }

                    // Show empty button
                    btnEmpty.Visible = true;
                }
            }
        }

        // Invalidate values
        hidProductID.Value = "";
        hidOptions.Value = "";
        hidQuantity.Value = "";
        hdnPrice.Value = "";

        // Update values in table
        btnUpdate_Click1(btnAddProduct, e);

        // Hide cart content when empty
        if (DataHelper.DataSourceIsEmpty(ShoppingCart.ContentTable))
        {
            HideCartContentWhenEmpty();
        }
        else
        {
            // Inventory shloud be checked
            checkInventory = true;
        }
    }


    /// <summary>
    /// Checks whether entered dsicount coupon code is usable for this cart. Returns true if so.
    /// </summary>
    private bool CheckDiscountCoupon()
    {
        bool success = true;

        if (txtCoupon.Text.Trim() != "")
        {
            // Get discount info
            DiscountCouponInfo dci = DiscountCouponInfoProvider.GetDiscountCouponInfo(txtCoupon.Text.Trim(), ShoppingCart.SiteName);

            // Do not validate coupon when editing existing order and coupon code was not changed, otherwise process validation
            if ((dci != null) && (((ShoppingCart.IsCreatedFromOrder) && (ShoppingCart.ShoppingCartDiscountCouponID == dci.DiscountCouponID)) || dci.IsValid))
            {
                ShoppingCart.ShoppingCartDiscountCouponID = dci.DiscountCouponID;
            }
            else
            {
                ShoppingCart.ShoppingCartDiscountCouponID = 0;

                // Discount coupon is not valid                
                lblError.Text = GetString("ecommerce.error.couponcodeisnotvalid");

                success = false;
            }
        }
        else
        {
            ShoppingCart.ShoppingCartDiscountCouponID = 0;
        }

        return success;
    }


    // Displays total price
    protected void DisplayTotalPrice()
    {
        pnlPrice.Visible = true;
        lblTotalPriceValue.Text = CurrencyInfoProvider.GetFormattedPrice(ShoppingCart.RoundedTotalPrice, ShoppingCart.Currency);
        lblTotalPrice.Text = GetString("ecommerce.cartcontent.totalpricelabel");

        lblShippingPrice.Text = GetString("ecommerce.cartcontent.shippingpricelabel");
        lblShippingPriceValue.Text = CurrencyInfoProvider.GetFormattedPrice(ShoppingCart.TotalShipping, ShoppingCart.Currency);
    }


    /// <summary>
    /// Sets product in the shopping cart.
    /// </summary>
    /// <param name="itemParams">Shoppping cart item parameters</param>
    protected void AddProducts(ShoppingCartItemParameters itemParams)
    {
        // Get main product info
        int productId = itemParams.SKUID;
        int quantity = itemParams.Quantity;

        if ((productId > 0) && (quantity > 0))
        {
            // Check product/options combination
            if (ShoppingCartInfoProvider.CheckNewShoppingCartItems(ShoppingCart, itemParams))
            {
                // Get requested SKU info object from database
                SKUInfo skuObj = SKUInfoProvider.GetSKUInfo(productId);
                if (skuObj != null)
                {
                    // On the live site
                    if (!ShoppingCartControl.IsInternalOrder)
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

                        // Set item in the shopping cart
                        ShoppingCartItemInfo product = ShoppingCart.SetShoppingCartItem(itemParams);

                        // Update shopping cart item in database
                        ShoppingCartItemInfoProvider.SetShoppingCartItemInfo(product);

                        // Update product options in database
                        foreach (ShoppingCartItemInfo option in product.ProductOptions)
                        {
                            ShoppingCartItemInfoProvider.SetShoppingCartItemInfo(option);
                        }

                        // Update bundle items in database
                        foreach (ShoppingCartItemInfo bundleItem in product.BundleItems)
                        {
                            ShoppingCartItemInfoProvider.SetShoppingCartItemInfo(bundleItem);
                        }

                        // Track add to shopping cart conversion
                        ECommerceHelper.TrackAddToShoppingCartConversion(product);
                    }
                    // In CMSDesk
                    else
                    {
                        // Set item in the shopping cart
                        ShoppingCart.SetShoppingCartItem(itemParams);
                    }
                }
            }

            // Avoid adding the same product after page refresh
            if (lblError.Text == "")
            {
                string url = URLRewriter.CurrentURL;

                if (!string.IsNullOrEmpty(URLHelper.GetUrlParameter(url, "productid")) ||
                    !string.IsNullOrEmpty(URLHelper.GetUrlParameter(url, "quantity")) ||
                    !string.IsNullOrEmpty(URLHelper.GetUrlParameter(url, "options")))
                {
                    // Remove parameters from URL
                    url = URLHelper.RemoveParameterFromUrl(url, "productid");
                    url = URLHelper.RemoveParameterFromUrl(url, "quantity");
                    url = URLHelper.RemoveParameterFromUrl(url, "options");
                    URLHelper.Redirect(url);
                }
            }
        }
    }


    /// <summary>
    /// Hides cart content controls when no items are in shopping cart.
    /// </summary>
    protected void HideCartContentWhenEmpty()
    {
        HideCartContent(null);
    }


    /// <summary>
    /// Hides cart content controls and displays given message.
    /// </summary>
    protected void HideCartContent(string message)
    {
        pnlNewItem.Visible = ShoppingCartControl.IsInternalOrder;
        pnlPrice.Visible = false;
        btnEmpty.Visible = false;
        plcCoupon.Visible = false;

        if (!ShoppingCartControl.IsInternalOrder)
        {
            pnlCurrency.Visible = false;
            gridData.Visible = false;
            ShoppingCartControl.ButtonNext.Visible = false;

            message = message ?? GetString("ecommerce.shoppingcartempty");
            lblInfo.Text = message + "<br />";
        }
    }


    /// <summary>
    /// Reloads the form data.
    /// </summary>
    protected void ReloadData()
    {
        //gridData.DataSource = ShoppingCartInfoObj.ShoppingCartContentTable;
        gridData.DataSource = ShoppingCart.ContentTable;

        // Hide coupon placeholder when no coupons are defined
        plcCoupon.Visible = AreDiscountCouponsAvailableOnSite();

        // Bind data
        gridData.DataBind();

        if (!DataHelper.DataSourceIsEmpty(ShoppingCart.ContentTable))
        {
            // Display total price
            DisplayTotalPrice();

            // Display/hide discount column
            gridData.Columns[9].Visible = ShoppingCart.IsDiscountApplied;
        }
        else
        {
            // Hide some items
            HideCartContentWhenEmpty();
        }

        if (!ShippingOptionInfoProvider.IsShippingNeeded(ShoppingCart))
        {
            plcShippingPrice.Visible = false;
        }
    }


    /// <summary>
    /// Determines if the discount coupons are available for the current site.
    /// </summary>
    private bool AreDiscountCouponsAvailableOnSite()
    {
        string siteName = ShoppingCart.SiteName;

        // Check site and global discount coupons
        string where = "DiscountCouponSiteID = " + SiteInfoProvider.GetSiteID(siteName);
        if (ECommerceSettings.AllowGlobalDiscountCoupons(siteName))
        {
            where += " OR DiscountCouponSiteID IS NULL";
        }

        // Coupons are available if found any
        DataSet ds = DiscountCouponInfoProvider.GetDiscountCoupons(where, null, -1, "count(DiscountCouponID)");
        if (!DataHelper.DataSourceIsEmpty(ds))
        {
            return (ValidationHelper.GetInteger(ds.Tables[0].Rows[0][0], 0) > 0);
        }

        return false;
    }


    /// <summary>
    /// Returns price detail link.
    /// </summary>
    protected string GetPriceDetailLink(object value)
    {
        if ((ShoppingCartControl.EnableProductPriceDetail) && (ShoppingCart.ContentTable != null))
        {
            Guid cartItemGuid = ValidationHelper.GetGuid(value, Guid.Empty);
            if (cartItemGuid != Guid.Empty)
            {
                return string.Format("<img src=\"{0}\" onclick=\"javascript: ShowProductPriceDetail('{1}', '{2}')\" alt=\"{3}\" class=\"ProductPriceDetailImage\" style=\"cursor:pointer;\" />",
                    GetImageUrl("Design/Controls/UniGrid/Actions/detail.png"),
                    cartItemGuid, GetCMSDeskShoppingCartSessionName(),
                    GetString("shoppingcart.productpricedetail"));
            }
        }

        return "";
    }


    /// <summary>
    /// Returns shopping cart session name.
    /// </summary>
    private string GetCMSDeskShoppingCartSessionName()
    {
        switch (ShoppingCartControl.CheckoutProcessType)
        {
            case CheckoutProcessEnum.CMSDeskOrder:
                return "CMSDeskNewOrderShoppingCart";

            case CheckoutProcessEnum.CMSDeskCustomer:
                return "CMSDeskNewCustomerOrderShoppingCart";

            case CheckoutProcessEnum.CMSDeskOrderItems:
                return "CMSDeskOrderItemsShoppingCart";

            case CheckoutProcessEnum.LiveSite:
            case CheckoutProcessEnum.Custom:
            default:
                return "";
        }
    }


    public override void ButtonBackClickAction()
    {
        if ((!ShoppingCartControl.IsInternalOrder) && (ShoppingCartControl.CurrentStepIndex == 0))
        {
            // Continue shopping
            URLHelper.Redirect(ShoppingCartControl.PreviousPageUrl);
        }
        else
        {
            // Standard action
            base.ButtonBackClickAction();
        }
    }


    #region "Binding methods"

    /// <summary>
    /// Returns formatted currency value.
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
    /// <param name="skuId">SKU ID</param>
    /// <param name="skuSiteId">SKU site ID</param>
    /// <param name="value">SKU name</param>
    /// <param name="isProductOption">Indicates if cart item is product option</param>
    /// <param name="isBundleItem">Indicates if cart item is bundle item</param>
    protected string GetSKUName(object skuId, object skuSiteId, object value, object isProductOption, object isBundleItem, object cartItemIsPrivate, object itemText, object productType)
    {
        string name = ResHelper.LocalizeString((string)value);
        bool isPrivate = ValidationHelper.GetBoolean(cartItemIsPrivate, false);
        string text = itemText as string;
        StringBuilder skuName = new StringBuilder();
        SKUProductTypeEnum type = SKUInfoProvider.GetSKUProductTypeEnum(productType as string);

        // If it is a product option or bundle item
        if (ValidationHelper.GetBoolean(isProductOption, false) || ValidationHelper.GetBoolean(isBundleItem, false))
        {
            skuName.Append("<span style=\"font-size: 90%\"> - ");
            skuName.Append(HTMLHelper.HTMLEncode(name));

            if (!string.IsNullOrEmpty(text))
            {
                skuName.Append(" '" + HTMLHelper.HTMLEncode(text) + "'");
            }

            skuName.Append("</span>");
        }
        // If it is a parent product
        else
        {
            // Add private donation suffix
            if ((type == SKUProductTypeEnum.Donation) && (isPrivate))
            {
                name += string.Format(" ({0})", GetString("com.shoppingcartcontent.privatedonation"));
            }

            // In CMS Desk
            if (ShoppingCartControl.IsInternalOrder)
            {
                // Display SKU name
                skuName.Append(HTMLHelper.HTMLEncode(name));
            }
            // On live site
            else
            {
                if (type == SKUProductTypeEnum.Donation)
                {
                    // Display donation name without link
                    skuName.Append(HTMLHelper.HTMLEncode(name));
                }
                else
                {
                    // Display link to product page
                    skuName.Append(string.Format("<a href=\"{0}?productid={1}\" class=\"CartProductDetailLink\">{2}</a>", ResolveUrl("~/CMSPages/GetProduct.aspx"), skuId.ToString(), HTMLHelper.HTMLEncode(name)));
                }
            }
        }

        return skuName.ToString();
    }


    protected static bool IsProductOption(object isProductOption)
    {
        return ValidationHelper.GetBoolean(isProductOption, false);
    }


    protected static bool IsBundleItem(object isBundleItem)
    {
        return ValidationHelper.GetBoolean(isBundleItem, false);
    }


    /// <summary>
    /// Returns order item edit action HTML.
    /// </summary>
    protected string GetOrderItemEditAction(object value)
    {
        Guid itemGuid = ValidationHelper.GetGuid(value, Guid.Empty);

        if (itemGuid != Guid.Empty)
        {
            return string.Format("<img src=\"{0}\" onclick=\"javascript: OpenOrderItemDialog('{1}', '{2}')\" alt=\"\" title=\"{3}\" class=\"OrderItemEditLink\" style=\"cursor: pointer;\" />",
                GetImageUrl("Objects/Ecommerce_OrderItem/edit.png"),
                itemGuid,
                GetCMSDeskShoppingCartSessionName(),
                GetString("shoppingcart.editorderitem"));
        }

        return "";
    }


    /// <summary>
    /// Returns SKU edit action HTML.
    /// </summary>
    protected string GetSKUEditAction(object skuId, object skuSiteId, object isProductOption, object isBundleItem)
    {
        if (!ValidationHelper.GetBoolean(isProductOption, false) && !ValidationHelper.GetBoolean(isBundleItem, false) && ShoppingCartControl.IsInternalOrder)
        {
            string url = ResolveUrl("~/CMSModules/Ecommerce/Pages/Tools/Products/Product_Edit_Dialog.aspx");
            url = URLHelper.AddParameterToUrl(url, "productid", skuId.ToString());
            url = URLHelper.AddParameterToUrl(url, "siteid", skuSiteId.ToString());
            url = URLHelper.AddParameterToUrl(url, "dialogmode", "1");

            return string.Format("<img src=\"{0}\" onclick=\"modalDialog('{1}', 'SKUEdit', '95%', '95%'); return false;\" alt=\"\" title=\"{2}\" class=\"OrderItemEditLink\" style=\"cursor: pointer;\" />",
                GetImageUrl("Objects/Ecommerce_OrderItem/editsku.png"),
                url,
                GetString("shoppingcart.editproduct"));
        }

        return "";
    }


    /// <summary>
    /// Returns formatted child cart item units. Returns empty string if it is not product option or bundle item.
    /// </summary>
    /// <param name="skuUnits">SKU units</param>
    /// <param name="isProductOption">Indicates if cart item is product option</param>
    /// <param name="isBundleItem">Indicates if cart item is bundle item</param>
    protected static string GetChildCartItemUnits(object skuUnits, object isProductOption, object isBundleItem)
    {
        if (ValidationHelper.GetBoolean(isProductOption, false) || ValidationHelper.GetBoolean(isBundleItem, false))
        {
            return string.Format("<span>{0}</span>", skuUnits);
        }

        return "";
    }

    #endregion
}

