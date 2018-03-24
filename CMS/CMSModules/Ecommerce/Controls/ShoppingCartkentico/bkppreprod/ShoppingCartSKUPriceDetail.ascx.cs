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

public partial class CMSModules_Ecommerce_Controls_ShoppingCart_ShoppingCartSKUPriceDetail_Control : CMSUserControl
{
    #region "Variables"

    private Guid mCartItemGuid = Guid.Empty;
    private ShoppingCartInfo mShoppingCart = null;
    private ShoppingCartItemInfo mShoppingCartItem = null;

    #endregion


    #region "Properties"

    /// <summary>
    /// Shopping cart.
    /// </summary>
    public ShoppingCartInfo ShoppingCart
    {
        get
        {
            return mShoppingCart;
        }
        set
        {
            mShoppingCart = value;
        }
    }


    /// <summary>
    /// Shopping cart item GUID.
    /// </summary>
    public Guid CartItemGuid
    {
        get
        {
            return mCartItemGuid;
        }
        set
        {
            mCartItemGuid = value;
        }
    }


    /// <summary>
    /// Shopping cart item.
    /// </summary>
    public ShoppingCartItemInfo ShoppingCartItem
    {
        get
        {
            if (mShoppingCartItem == null)
            {
                if ((CartItemGuid != Guid.Empty) && (ShoppingCart != null))
                {
                    // Get item from shopping cart
                    mShoppingCartItem = ShoppingCart.GetShoppingCartItem(CartItemGuid);
                }
            }

            return mShoppingCartItem;
        }
    }

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        // Initialize controls        
        lblPriceAfterDiscount.Text = GetString("ProductPriceDetail.PriceAfterDiscount");
        lblPriceWithTax.Text = GetString("ProductPriceDetail.PriceWithTax");
        lblProductUnits.Text = GetString("ProductPriceDetail.ProductUnits");
        lblTotalDiscount.Text = GetString("ProductPriceDetail.TotalDiscount");
        lblTotalTax.Text = GetString("ProductPriceDetail.TotalTax");
        lblTotalPrice.Text = GetString("ProductPriceDetail.TotalPrice");

        // Set alternate header text when grid is empty and grid header is hidden
        lblTaxes.Text = GetString("ProductPriceDetail.Taxes");
        lblDiscounts.Text = GetString("ProductPriceDetail.Discounts");

        // Discounts table header
        gridDiscounts.Columns[0].HeaderText = lblDiscounts.Text;
        gridDiscounts.Columns[1].HeaderText = GetString("ProductPriceDetail.PerUnit");
        gridDiscounts.Columns[3].HeaderText = GetString("ProductPriceDetail.DiscountedUnits");
        gridDiscounts.Columns[4].HeaderText = GetString("ProductPriceDetail.Subtotal");

        // Taxees table header
        gridTaxes.Columns[0].HeaderText = lblTaxes.Text;
        gridTaxes.Columns[1].HeaderText = GetString("ProductPriceDetail.PerUnit");
        gridTaxes.Columns[3].HeaderText = GetString("ProductPriceDetail.Subtotal");

        // Discounts
        gridDiscounts.DataSource = ShoppingCartItem.DiscountsTable;
        gridDiscounts.DataBind();

        // Taxes
        gridTaxes.DataSource = ShoppingCartItem.TaxesTable;
        gridTaxes.DataBind();

        // Subtotals and Totals
        if (ShoppingCartItem.IsPartialDiscountApplied)
        {
            // Display total price without tax
            lblPriceWithoutTaxValue.Text = ShoppingCart.GetFormattedPrice(ShoppingCartItem.UnitPrice * ShoppingCartItem.CartItemUnits);
            lblPriceWithoutTax.Text = GetString("ProductPriceDetail.TotalPriceWithoutTax");

            // Display price after total discount
            lblPriceAfterDiscountValue.Text = ShoppingCart.GetFormattedPrice(ShoppingCartItem.UnitPriceAfterDiscount * ShoppingCartItem.CartItemUnits);
            lblTotalDiscountValue.Text = ShoppingCart.GetFormattedPrice(ShoppingCartItem.TotalDiscount);

            // Display total tax
            lblTotalTaxValue.Text = ShoppingCart.GetFormattedPrice(ShoppingCartItem.TotalTax);

            // Hide price with tax and units
            plcPriceWithTax.Visible = false;
            plcUnits.Visible = false;
        }
        else
        {
            // Display unit price without tax
            lblPriceWithoutTaxValue.Text = ShoppingCart.GetFormattedPrice(ShoppingCartItem.UnitPrice);
            lblPriceWithoutTax.Text = GetString("ProductPriceDetail.PriceWithoutTax");

            // Displya unit price after discount
            lblPriceAfterDiscountValue.Text = ShoppingCart.GetFormattedPrice(ShoppingCartItem.UnitPriceAfterDiscount);
            lblTotalDiscountValue.Text = ShoppingCart.GetFormattedPrice(ShoppingCartItem.UnitTotalDiscount);

            // Display unit total tax and price with tax
            lblTotalTaxValue.Text = ShoppingCart.GetFormattedPrice(ShoppingCartItem.UnitTotalTax);
            lblPriceWithTaxValue.Text = ShoppingCart.GetFormattedPrice(ShoppingCartItem.UnitTotalPrice);
        }

        lblTotalPriceValue.Text = ShoppingCart.GetFormattedPrice(ShoppingCartItem.TotalPrice, true);

        // Product name and units
        if (((ShoppingCartItem != null)) && (ShoppingCartItem.SKU != null))
        {
            lblProductName.Text = HTMLHelper.HTMLEncode(ResHelper.LocalizeString(ShoppingCartItem.SKU.SKUName));
            lblProductUnitsValue.Text = ShoppingCartItem.CartItemUnits.ToString();
        }

        // Show subtotals
        plcTotalDiscount.Visible = (ShoppingCartItem.DiscountsTable.Rows.Count != 1);
        plcTotalTax.Visible = (ShoppingCartItem.TaxesTable.Rows.Count != 1);

        // Show alternate header
        plcTaxes.Visible = (gridTaxes.Rows.Count == 0);
        plcDiscounts.Visible = (gridDiscounts.Rows.Count == 0);

        if (gridDiscounts.HeaderRow != null)
        {
            if (ShoppingCartItem.IsPartialDiscountApplied)
            {
                // View with partial discount
                gridDiscounts.Columns[2].Visible = false;
                gridDiscounts.Columns[3].Visible = true;
                gridDiscounts.Columns[4].Visible = true;
                gridDiscounts.HeaderRow.Cells[1].ColumnSpan = 1;
            }
            else
            {
                // Standard view
                gridDiscounts.HeaderRow.Cells[1].ColumnSpan = 2;
                gridDiscounts.HeaderRow.Cells[1].Width = 160;
                gridDiscounts.Columns[2].Visible = true;
                gridDiscounts.Columns[3].Visible = false;
                gridDiscounts.Columns[4].Visible = false;
            }

            gridDiscounts.HeaderRow.Cells[2].Visible = false;
        }

        if (gridTaxes.HeaderRow != null)
        {
            if (ShoppingCartItem.IsPartialDiscountApplied)
            {
                // View with partial discount
                gridTaxes.Columns[2].Visible = false;
            }
            else
            {
                // Standard view
                gridTaxes.HeaderRow.Cells[1].ColumnSpan = 2;
                gridTaxes.HeaderRow.Cells[1].Width = 160;
                gridTaxes.HeaderRow.Cells[2].Visible = false;
                gridTaxes.Columns[3].Visible = false;
            }
        }
    }


    /// <summary>
    /// Returns formatted tax/discount name.
    /// </summary>
    /// <param name="name">Tax/discount name</param>
    protected string GetFormattedName(object name)
    {
        return HTMLHelper.HTMLEncode(" - " + ResHelper.LocalizeString(Convert.ToString(name)));
    }


    /// <summary>
    /// Returns formatted value string.
    /// </summary>
    /// <param name="value">Value to be formatted</param>
    /// <param name="isFlat">True - it is a flat value, False - it is a relative value</param>
    protected string GetFormattedValue(object value, object isFlat)
    {
        bool mIsFlat = ValidationHelper.GetBoolean(isFlat, false);
        double mValue = ValidationHelper.GetDouble(value, 0);

        if (mIsFlat)
        {
            // Flat value
            return ShoppingCart.GetFormattedPrice(mValue);
        }
        else
        {
            // Relative value
            return mValue.ToString() + "%";
        }
    }

    #endregion
}