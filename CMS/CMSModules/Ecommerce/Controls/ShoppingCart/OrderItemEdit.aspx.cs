using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.Ecommerce;
using CMS.GlobalHelper;
using CMS.UIControls;
using CMS.Helpers;

[Title(ImageUrl = "Objects/Ecommerce_OrderItem/object.png", ResourceString = "OrderItemEdit.Title")]
public partial class CMSModules_Ecommerce_Controls_ShoppingCart_OrderItemEdit : CMSOrdersPage
{
    #region "Variables"

    private ShoppingCartInfo mShoppingCart = null;
    private ShoppingCartItemInfo mShoppingCartItem = null;
    private OptionCategoryInfo mOptionCategory = null;
    private SKUInfo mSKU = null;

    #endregion


    #region "Properties"

    /// <summary>
    /// Shopping cart object with order data.
    /// </summary>
    private ShoppingCartInfo ShoppingCart
    {
        get
        {
            if (mShoppingCart == null)
            {
                string cartSessionName = QueryHelper.GetString("cart", String.Empty);
                if (cartSessionName != String.Empty)
                {
                    mShoppingCart = SessionHelper.GetValue(cartSessionName) as ShoppingCartInfo;
                }
            }

            return mShoppingCart;
        }
    }


    /// <summary>
    /// Shopping cart item data.
    /// </summary>
    private ShoppingCartItemInfo ShoppingCartItem
    {
        get
        {
            if (mShoppingCartItem == null)
            {
                if (ShoppingCart != null)
                {
                    Guid cartItemGuid = QueryHelper.GetGuid("itemguid", Guid.Empty);
                    if (cartItemGuid != Guid.Empty)
                    {
                        mShoppingCartItem = ShoppingCart.GetShoppingCartItem(cartItemGuid);
                    }
                }
            }

            return mShoppingCartItem;
        }
    }

    /// <summary>
    /// SKU option category data
    /// </summary>
    private OptionCategoryInfo OptionCategory
    {
        get
        {
            if ((mOptionCategory == null) && (SKU != null))
            {
                mOptionCategory = OptionCategoryInfoProvider.GetOptionCategoryInfo(SKU.SKUOptionCategoryID);
            }

            return mOptionCategory;
        }
    }

    /// <summary>
    /// SKU data
    /// </summary>
    private SKUInfo SKU
    {
        get
        {
            if ((mSKU == null) && (ShoppingCartItem != null))
            {
                mSKU = ShoppingCartItem.SKU;
            }

            return mSKU;
        }
    }

    #endregion


    protected void Page_Load(object sender, EventArgs e)
    {
        // Initialize controls
        lblSKUName.Text = GetString("OrderItemEdit.SKUName");
        lblSKUPrice.Text = GetString("OrderItemEdit.SKUPrice");
        lblSKUUnits.Text = GetString("OrderItemEdit.SKUUnits");
        btnOk.Text = GetString("general.ok");
        btnCancel.Text = GetString("general.cancel");

        // Initialize validators
        rfvSKUName.ErrorMessage = GetString("OrderItemEdit.ErrorSKUName");
        rfvSKUUnits.ErrorMessage = GetString("OrderItemEdit.ErrorSKUUnits");
        txtSKUPrice.EmptyErrorMessage = GetString("OrderItemEdit.ErrorSKUNPrice");
        txtSKUPrice.ValidationErrorMessage = GetString("com.newproduct.skupricenotdouble");

        // Load data
        if (!RequestHelper.IsPostBack())
        {
            LoadData();
        }

        RegisterEscScript();
        RegisterModalPageScripts();
    }


    /// <summary>
    /// Loads product data to the form fields.
    /// </summary>
    private void LoadData()
    {
        SKUInfo sku = SKU;

        if ((ShoppingCartItem == null) || (sku == null))
        {
            // Do not process
            EditedObject = null;
            return;
        }

        OrderInfo order = ShoppingCart.Order;

        // Is form editing enabled?
        bool editingEnabled = ((order == null) || !order.OrderIsPaid) && ECommerceSettings.EnableOrderItemEditing && !ECommerceSettings.UseCurrentSKUData;

        txtSKUName.Text = sku.SKUName;
        txtSKUName.Enabled = editingEnabled;

        txtSKUPrice.Price = ShoppingCartItem.UnitPrice;
        txtSKUPrice.Currency = ShoppingCart.Currency;
        txtSKUPrice.Enabled = editingEnabled;

        txtSKUUnits.Text = ShoppingCartItem.CartItemUnits.ToString();
        txtSKUUnits.Enabled = editingEnabled && !(ShoppingCartItem.IsProductOption || ShoppingCartItem.IsBundleItem);

        chkIsPrivate.Enabled = editingEnabled;

        txtItemText.Enabled = editingEnabled;
        txtItemMultiText.Enabled = editingEnabled;

        btnOk.Enabled = editingEnabled;

        // If cart item represents donation
        if (sku.SKUProductType == SKUProductTypeEnum.Donation)
        {
            // Apply additional price editing conditions
            txtSKUPrice.AllowZero = false;
            txtSKUPrice.Enabled &= !((sku.SKUMinPrice == sku.SKUPrice) && (sku.SKUMaxPrice == sku.SKUPrice));

            // Display is private checkbox
            if (sku.SKUPrivateDonation)
            {
                chkIsPrivate.Checked = ShoppingCartItem.CartItemIsPrivate;
                plcIsPrivate.Visible = true;
            }
        }

        // If cart item represents text option
        if (sku.SKUProductType == SKUProductTypeEnum.Text)
        {
            // Get option category
            if (OptionCategory != null)
            {
                // Display cart item text textbox
                if (OptionCategory.CategorySelectionType == OptionCategorySelectionTypeEnum.TextBox)
                {
                    txtItemText.Text = ShoppingCartItem.CartItemText;
                    txtItemText.Visible = true;
                }
                else
                {
                    txtItemMultiText.Text = ShoppingCartItem.CartItemText;
                    txtItemMultiText.Visible = true;
                }

                plcItemText.Visible = true;
            }
        }

        // If cart item represents e-product
        if (sku.SKUProductType == SKUProductTypeEnum.EProduct)
        {
            // Display valid to information
            if ((order != null) && order.OrderIsPaid)
            {
                if (ShoppingCartItem.CartItemValidTo.CompareTo(DateTimeHelper.ZERO_TIME) == 0)
                {
                    lblValidTo.Text = GetString("general.unlimited");
                }
                else
                {
                    lblValidTo.Text = ShoppingCartItem.CartItemValidTo.ToString();
                }
            }
            else
            {
                lblValidTo.Text = GetString("com.orderitemedit.validtonotset");
            }

            plcValidTo.Visible = true;
        }
    }


    protected void btnOk_Click(object sender, EventArgs e)
    {
        // Check 'ModifyOrders' permission
        if (!ECommerceContext.IsUserAuthorizedForPermission("ModifyOrders"))
        {
            RedirectToCMSDeskAccessDenied("CMS.Ecommerce", "EcommerceModify OR ModifyOrders");
        }

        string errorMessage = ValidateForm();

        if (errorMessage == "")
        {
            // Clone Shopping cart item SKU to prevent "real SKU" to be modified
            ShoppingCartItem.SKU = ShoppingCartItem.SKU.Clone();

            ShoppingCartItem.SKU.SKUName = txtSKUName.Text;

            // Get new price
            double rate = (ShoppingCartItem.SKU.IsGlobal) ? ShoppingCart.ExchangeRateForGlobalItems : ShoppingCart.ExchangeRate;
            double newPrice = ExchangeRateInfoProvider.ApplyExchangeRate(txtSKUPrice.Price, 1 / rate);

            // Update price
            if (ShoppingCartItem.SKU.SKUProductType == SKUProductTypeEnum.Donation
                || ShoppingCartItem.SKU.SKUProductType == SKUProductTypeEnum.Text)
            {
                ShoppingCartItem.CartItemPrice = newPrice;
            }
            else
            {
                ShoppingCartItem.SKU.SKUPrice = newPrice;
            }

            // Update units
            ShoppingCartItem.CartItemUnits = ValidationHelper.GetInteger(txtSKUUnits.Text, 0);

            // Update is private information
            if (plcIsPrivate.Visible)
            {
                ShoppingCartItem.CartItemIsPrivate = chkIsPrivate.Checked;
            }

            // Update product text when visible
            if (plcItemText.Visible == true)
            {
                ShoppingCartItem.CartItemText = txtItemText.Visible ? txtItemText.Text : txtItemMultiText.Text;
            }

            // Update units of the product options
            foreach (ShoppingCartItemInfo option in ShoppingCartItem.ProductOptions)
            {
                option.CartItemUnits = ShoppingCartItem.CartItemUnits;
            }

            // Evaluate shopping cart content
            ShoppingCartInfoProvider.EvaluateShoppingCart(ShoppingCart);

            // Close dialog window and refresh parent window
            string url = "~/CMSModules/Ecommerce/Pages/Tools/Orders/Order_Edit_OrderItems.aspx?orderid=" + ShoppingCart.OrderId + "&cartexist=1";
            ltlScript.Text = ScriptHelper.GetScript("CloseAndRefresh('" + ResolveUrl(url) + "')");
        }
        else
        {
            // Show error message
            ShowError(errorMessage);
        }
    }


    /// <summary>
    /// Validates form data.
    /// </summary>
    private string ValidateForm()
    {
        string error = new Validator().NotEmpty(txtSKUName.Text.Trim(), rfvSKUName.ErrorMessage).
            NotEmpty(txtSKUUnits.Text.Trim(), rfvSKUUnits.ErrorMessage).Result;

        // Validate product price
        if (error == "")
        {
            error = txtSKUPrice.Validate(ShoppingCartItem.IsProductOption);
        }

        // Validate price for donation
        if ((error == "") && (SKU.SKUProductType == SKUProductTypeEnum.Donation))
        {
            // Get min and max price in cart currency
            double minPrice = ShoppingCart.ApplyExchangeRate(SKU.SKUMinPrice, SKU.IsGlobal);
            double maxPrice = ShoppingCart.ApplyExchangeRate(SKU.SKUMaxPrice, SKU.IsGlobal);

            if ((minPrice > 0) && (txtSKUPrice.Price < minPrice) || ((maxPrice > 0) && (txtSKUPrice.Price > maxPrice)))
            {
                // Get formatted min and max price
                string fMinPrice = ShoppingCart.GetFormattedPrice(minPrice);
                string fMaxPrice = ShoppingCart.GetFormattedPrice(maxPrice);

                // Set correct error message
                if ((minPrice > 0.0) && (maxPrice > 0.0))
                {
                    error = String.Format(GetString("com.orderitemedit.pricerange"), fMinPrice, fMaxPrice);
                }
                else if (minPrice > 0.0)
                {
                    error = String.Format(GetString("com.orderitemedit.pricerangemin"), fMinPrice);
                }
                else if (maxPrice > 0.0)
                {
                    error = String.Format(GetString("com.orderitemedit.pricerangemax"), fMaxPrice);
                }
            }
        }

        // Validate product units
        if (error == "")
        {
            if (ValidationHelper.GetInteger(txtSKUUnits.Text.Trim(), -1) < 0)
            {
                error = GetString("OrderItemEdit.SKUUnitsNotPositiveInteger");
            }
        }

        // Validata text product option        
        if ((error == "") && (SKU.SKUProductType == SKUProductTypeEnum.Text))
        {
            int maxLength = OptionCategory.CategoryTextMaxLength;
            if (maxLength > 0)
            {
                // Check text length
                if ((txtItemText.Visible && (txtItemText.Text.Length > maxLength)) ||
                    (txtItemMultiText.Visible && (txtItemMultiText.Text.Length > maxLength)))
                {
                    error = string.Format(GetString("com.optioncategory.maxtextlengthexceeded"), maxLength);
                }
            }
        }

        return error;
    }
}