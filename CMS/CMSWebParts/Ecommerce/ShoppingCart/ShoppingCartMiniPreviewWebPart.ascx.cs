using System;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.DataEngine;
using CMS.Ecommerce;
using CMS.Helpers;
using CMS.PortalControls;
using CMS.Base;

public partial class CMSWebParts_Ecommerce_ShoppingCart_ShoppingCartMiniPreviewWebPart : CMSAbstractWebPart
{    
    protected string rtlFix = "";

    #region "Web part properties"


    /// <summary>
    /// Shopping cart icon image URL.
    /// </summary>
    public string IconImageUrl
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("IconImageUrl"), GetImageUrl("CMSModules/CMS_Ecommerce/cart.png"));
        }
        set
        {
            SetValue("IconImageUrl", value);
            imgCartIcon.ImageUrl = value;
        }
    }


    /// <summary>
    /// Indicates if shopping cart icon should be displayed.
    /// </summary>
    public bool ShowIconImage
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("ShowIconImage"), ShowTotalPrice);
        }
        set
        {
            SetValue("ShowIconImage", value);
            imgCartIcon.Visible = value;
        }
    }


    /// <summary>
    /// Shopping cart link URL.
    /// </summary>
    public string ShoppingCartLinkUrl
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("ShoppingCartLinkUrl"), ECommerceSettings.ShoppingCartURL(SiteName));
        }
        set
        {
            SetValue("ShoppingCartLinkUrl", value);
            lnkShoppingCart.NavigateUrl = value;
        }
    }


    /// <summary>
    /// Shopping cart link text.
    /// </summary>
    public string ShoppingCartLinkText
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("ShoppingCartLinkText"), GetString("ShopingCartMiniPreview.ShoppingCartLinkText"));
        }
        set
        {
            SetValue("ShoppingCartLinkText", value);
            lnkShoppingCart.Text = value;
        }
    }


    /// <summary>
    /// Indicates if link to the shopping cart content page should be displayed.
    /// </summary>
    public bool ShowShoppingCartLink
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("ShowShoppingCartLink"), true);
        }
        set
        {
            SetValue("ShowShoppingCartLink", value);
            plcShoppingCart.Visible = value;
        }
    }


    /// <summary>
    /// My account link URL.
    /// </summary>
    public string MyAccountLinkUrl
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("MyAccountLinkUrl"), SettingsKeyInfoProvider.GetStringValue(SiteName + ".CMSMyAccountURL"));
        }
        set
        {
            SetValue("MyAccountLinkUrl", value);
            lnkMyAccount.NavigateUrl = value;
        }
    }


    /// <summary>
    /// My account link text.
    /// </summary>
    public string MyAccountLinkText
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("MyAccountLinkText"), GetString("ShopingCartMiniPreview.MyAccountLinkText"));
        }
        set
        {
            SetValue("MyAccountLinkText", value);
            lnkMyAccount.Text = value;
        }
    }


    /// <summary>
    /// Indicates if link to user's account page should be displayed.
    /// </summary>
    public bool ShowMyAccountLink
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("ShowMyAccountLink"), true);
        }
        set
        {
            SetValue("ShowMyAccountLink", value);
            plcMyAccount.Visible = value;
        }
    }


    /// <summary>
    /// My wish list link URL.
    /// </summary>
    public string MyWishlistLinkUrl
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("WishlistLinkUrl"), ECommerceSettings.WishListURL(SiteName));
        }
        set
        {
            SetValue("WishlistLinkUrl", value);
            lnkMyWishlist.NavigateUrl = value;
        }
    }


    /// <summary>
    /// My wish list link text.
    /// </summary>
    public string MyWishlistLinkText
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("WishlistLinkText"), GetString("ShopingCartMiniPreview.MyWishlistLinkText"));
        }
        set
        {
            SetValue("WishlistLinkText", value);
            lnkMyWishlist.Text = value;
        }
    }


    /// <summary>
    /// Indicates if link to the user's wish list page should be displayed.
    /// </summary>
    public bool ShowMyWishlistLink
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("ShowWishlistLink"), true);
        }
        set
        {
            SetValue("ShowWishlistLink", value);
            plcMyWishlist.Visible = value;
        }
    }


    /// <summary>
    /// Link separator
    /// </summary>
    public string Separator
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("Separator"), "|&nbsp;");
        }
        set
        {
            SetValue("Separator", value);
        }
    }


    /// <summary>
    /// Total price title text.
    /// </summary>
    public string TotalPriceTitleText
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("TotalPriceTitleText"), GetString("ShopingCartMiniPreview.TotalPriceTitleText"));
        }
        set
        {
            SetValue("TotalPriceTitleText", value);
            lblTotalPriceTitle.Text = value;
        }
    }


    /// <summary>
    /// Indicates if shopping cart total price title should be displayed.
    /// </summary>
    public bool ShowTotalPriceTitle
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("ShowTotalPriceTitle"), ShowTotalPrice);
        }
        set
        {
            SetValue("ShowTotalPriceTitle", value);            
            lblTotalPriceTitle.Visible = value;
        }
    }



    /// <summary>
    /// Indicates if shopping cart total price value should be displayed.
    /// </summary>
    public bool ShowTotalPrice
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("ShowTotalPrice"), true);
        }
        set
        {
            SetValue("ShowTotalPrice", value);            
            lblTotalPriceValue.Visible = value;
        }
    }



    /// <summary>
    /// Text which is displayed when shopping cart is empty.
    /// </summary>
    public string EmptyShoppingCartText
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("EmptyShoppingCartText"), GetString("ShoppingCart.Empty"));
        }
        set
        {
            SetValue("EmptyShoppingCartText", value);
        }
    }


    /// <summary>
    /// Indicates if text given by EmptyShoppingCartText property is displayed when shopping is empty.
    /// </summary>
    public bool ShowEmptyShoppingCartText
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("ShowEmptyShoppingCartText"), true);
        }
        set
        {
            SetValue("ShowEmptyShoppingCartText", value);
        }
    }


    #endregion


    #region "Other properties"

    /// <summary>
    /// Gets the site name.
    /// </summary>
    protected string SiteName
    {
        get
        {
            return ECommerceContext.CurrentShoppingCart.SiteName;
        }
    }


    /// <summary>
    /// Gets formatted shopping cart total price value.
    /// </summary>
    public string TotalPriceValue
    {
        get
        {
            ShoppingCartInfo sc = ECommerceContext.CurrentShoppingCart;
            if ((sc != null) && (!sc.IsEmpty))
            {
                return sc.GetFormattedPrice(sc.RoundedTotalPrice);
            }
            else
            {
                return "";
            }
        }
    }


    #endregion


    /// <summary>
    /// Content loaded event handler.
    /// </summary>
    public override void OnContentLoaded()
    {
        base.OnContentLoaded();
    }


    /// <summary>
    /// Initializes links
    /// </summary>
    protected void SetLinks()
    {
        // Shopping cart icon
        if (ShowIconImage)
        {
            imgCartIcon.Visible = true;
            imgCartIcon.ImageUrl = ResolveUrl(IconImageUrl);
            imgCartIcon.AlternateText = GetString("ShoppingcartPreview.Icon");

            // Check if RTL fix must be applied
            if (CultureHelper.IsPreferredCultureRTL())
            {
                rtlFix = "<span style=\"visibility:hidden;\">.</span>";
            }
        }
        else
        {
            imgCartIcon.Visible = false;
        }

        // Link to user's shopping cart
        if (ShowShoppingCartLink && (ShoppingCartLinkUrl != ""))
        {
            lnkShoppingCart.NavigateUrl = URLHelper.ResolveUrl(URLHelper.AddPrefixToUrl(ShoppingCartLinkUrl));
            lnkShoppingCart.Text = ShoppingCartLinkText;
        }
        else
        {
            plcShoppingCart.Visible = false;
        }

        // Link to user's wish list
        if (ShowMyWishlistLink && (MyWishlistLinkUrl != ""))
        {
            lnkMyWishlist.NavigateUrl = URLHelper.ResolveUrl(URLHelper.AddPrefixToUrl(MyWishlistLinkUrl));
            lnkMyWishlist.Text = MyWishlistLinkText;
        }
        else
        {
            plcMyWishlist.Visible = false;
        }

        // Link to user's account
        if (ShowMyAccountLink && (MyAccountLinkUrl != ""))
        {
            lnkMyAccount.NavigateUrl = URLHelper.ResolveUrl(URLHelper.AddPrefixToUrl(MyAccountLinkUrl));
            lnkMyAccount.Text = MyAccountLinkText;
        }
        else
        {
            plcMyAccount.Visible = false;
        }      
    }


    /// OnPreRender override
    /// </summary>
    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        if (!StopProcessing)
        {
            SetLinks();
            
            SetShoppingCartPreviewText();
        }
    }


    private void SetShoppingCartPreviewText()
    {
        // Hide total price info by default
        plcTotalPrice.Visible = false;
        lblTotalPriceTitle.Visible = false;
        lblTotalPriceValue.Visible = false;

        // Is cart empty?
        ShoppingCartInfo sc = ECommerceContext.CurrentShoppingCart;
        bool cartEmpty = (sc != null) && sc.IsEmpty;
        
        if (!cartEmpty || !ShowEmptyShoppingCartText)
        {
            // Display title
            if (ShowTotalPriceTitle)
            {                                
                plcTotalPrice.Visible = true;
                lblTotalPriceTitle.Visible = true;
                lblTotalPriceTitle.Text = TotalPriceTitleText;
            }

            // Display total price
            if (ShowTotalPrice)
            {                
                plcTotalPrice.Visible = true;
                lblTotalPriceValue.Visible = true;
                lblTotalPriceValue.Text = TotalPriceValue;
            }
        }
        else if (ShowEmptyShoppingCartText)
        {
            // Display 'cart empty' text
            plcTotalPrice.Visible = true;
            lblTotalPriceTitle.Visible = true;          
            lblTotalPriceTitle.Text = EmptyShoppingCartText;
        }
    }
}