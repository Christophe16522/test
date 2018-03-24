using System;
using System.Data;
using System.Web;
using System.Web.UI.WebControls;

using CMS.CMSHelper;
using CMS.Ecommerce;
using CMS.GlobalHelper;
using CMS.SettingsProvider;
using CMS.DocumentEngine;

using TreeNode = CMS.DocumentEngine.TreeNode;
using CMS.SiteProvider;
using CMS.Helpers;
using CMS.Membership;
using CMS.DataEngine;
using CMS.Localization;

/// <summary>
/// Summary description for Functions.
/// </summary>
public static class EcommerceFunctions
{
    #region "Price - obsolete methods"

    /// <summary>
    /// Returns formatted price using current shopping cart data from CMS context. Customer discount level is not applied to the product price.
    /// </summary>
    /// <param name="SKUPrice">SKU price</param>
    [Obsolete("Use SKUInfoProvider.GetSKUFormattedPrice(SKUInfo, null, false, false) in your web project or GetSKUFormattedPrice(false, false) in transformation.")]
    public static string GetFormatedPrice(object SKUPrice)
    {
        return GetFormatedPrice(SKUPrice, 0, null, 0, true);
    }


    /// <summary>
    /// Returns formatted price using current shopping cart data from CMS context. Customer discount level is applied to the product price when this discount level is assigned to the specified product department.
    /// </summary>
    /// <param name="SKUPrice">SKU price</param>    
    /// <param name="SKUDepartmentId">SKU department ID</param>
    [Obsolete("Use SKUInfoProvider.GetSKUFormattedPrice(SKUInfo, null, true, false) in your web project or GetSKUFormattedPrice(true, false) in transformation.")]
    public static string GetFormatedPrice(object SKUPrice, object SKUDepartmentId)
    {
        return GetFormatedPrice(SKUPrice, SKUDepartmentId, null, 0, true);
    }


    /// <summary>
    /// Returns formatted price using specified shopping cart data.
    /// </summary>
    /// <param name="SKUPrice">SKU price</param>
    /// <param name="SKUDepartmentId">SKU department ID</param>
    /// <param name="cart">Shopping cart object with required data for price formatting, if it is NULL shopping cart data from CMS context are used</param>
    [Obsolete("Use SKUInfoProvider.GetSKUFormattedPrice(SKUInfo, ShoppingCartInfo, true, false) in your web project or GetSKUFormattedPrice(true, false) in transformation.")]
    public static string GetFormatedPrice(object SKUPrice, object SKUDepartmentId, ShoppingCartInfo cart)
    {
        return GetFormatedPrice(SKUPrice, SKUDepartmentId, cart, 0, true);
    }


    /// <summary>
    /// Returns formatted price using specified shopping cart data.
    /// </summary>
    /// <param name="SKUPrice">SKU price</param>
    /// <param name="SKUDepartmentId">SKU department ID</param>
    /// <param name="SKUId">Product ID</param>
    [Obsolete("Use SKUInfoProvider.GetSKUFormattedPrice(SKUInfo, null, true, true) in your web project or GetSKUFormattedPrice(true, true) in transformation.")]
    public static string GetFormatedPrice(object SKUPrice, object SKUDepartmentId, object SKUId)
    {
        return GetFormatedPrice(SKUPrice, SKUDepartmentId, null, SKUId, true);
    }


    /// <summary>
    /// Returns formatted price using specified shopping cart data.
    /// </summary>
    /// <param name="SKUPrice">SKU price</param>
    /// <param name="SKUDepartmentId">SKU department ID</param>
    /// <param name="cart">Shopping cart object with required data for price formatting, if it is NULL shopping cart data from CMS context are used</param>
    /// <param name="SKUId">Product ID</param>
    [Obsolete("Use SKUInfoProvider.GetSKUFormattedPrice(SKUInfo, ShoppingCartInfo, true, true) in your web project or GetSKUFormattedPrice(true, true) in transformation.")]
    public static string GetFormatedPrice(object SKUPrice, object SKUDepartmentId, ShoppingCartInfo cart, object SKUId)
    {
        return GetFormatedPrice(SKUPrice, SKUDepartmentId, cart, SKUId, true);
    }


    /// <summary>
    /// Returns product price including all its taxes.
    /// </summary>
    /// <param name="SKUPrice">SKU price</param>
    /// <param name="SKUDepartmentId">SKU department ID</param>
    /// <param name="cart">Shopping cart with shopping data</param>
    /// <param name="SKUId">SKU ID</param>
    [Obsolete("Use SKUInfoProvider.GetSKUPrice(SKUInfo, ShoppingCartInfo, true, true) instead.")]
    public static double GetPrice(object SKUPrice, object SKUDepartmentId, ShoppingCartInfo cart, object SKUId)
    {
        string price = GetFormatedPrice(SKUPrice, SKUDepartmentId, cart, SKUId, false);
        return ValidationHelper.GetDouble(price, 0);
    }


    /// <summary>
    /// Returns product price without its taxes.
    /// </summary>
    /// <param name="SKUPrice">SKU price</param>
    /// <param name="SKUDepartmentId">SKU department ID</param>
    /// <param name="cart">Shopping cart with shopping data</param>
    [Obsolete("Use SKUInfoProvider.GetSKUPrice(SKUInfo, ShoppingCartInfo, true, false) instead.")]
    public static double GetPrice(object SKUPrice, object SKUDepartmentId, ShoppingCartInfo cart)
    {
        string price = GetFormatedPrice(SKUPrice, SKUDepartmentId, cart, 0, false);
        return ValidationHelper.GetDouble(price, 0);
    }


    /// <summary>
    /// Returns formatted price using specified shopping cart data.
    /// </summary>
    /// <param name="SKUPrice">SKU price</param>
    /// <param name="SKUDepartmentId">SKU department ID</param>
    /// <param name="cart">Shopping cart object with required data for price formatting, if it is NULL shopping cart data from CMS context are used</param>
    /// <param name="SKUId">Product ID</param>
    /// <param name="FormatPrice">Format output price</param>
    /// <param name="globalSKU">True if price belongs to global product.</param>
    [Obsolete("Use SKUInfoProvider.GetSKUFormattedPrice(SKUInfo, ShoppingCartInfo, true, true) in your web project or GetSKUFormattedPrice(true, true) in transformation.")]
    public static string GetFormatedPrice(object SKUPrice, object SKUDepartmentId, ShoppingCartInfo cart, object SKUId, bool FormatPrice)
    {
        double price = ValidationHelper.GetDouble(SKUPrice, 0);
        int departmentId = ValidationHelper.GetInteger(SKUDepartmentId, 0);
        int skuId = ValidationHelper.GetInteger(SKUId, 0);

        // When on the live site
        if (cart == null)
        {
            cart = ECommerceContext.CurrentShoppingCart;
        }

        if (departmentId > 0)
        {
            // Try site discount level
            DiscountLevelInfo discountLevel = cart.SiteDiscountLevel;
            bool valid = (discountLevel != null) && discountLevel.IsInDepartment(departmentId) && discountLevel.IsValid;

            if (!valid)
            {
                // Try global discount level
                discountLevel = cart.DiscountLevel;
                valid = (discountLevel != null) && discountLevel.IsInDepartment(departmentId) && discountLevel.IsValid;
            }

            // Apply discount to product price 
            if (valid)
            {
                // Remember price before discount
                double oldPrice = price;

                // Get new price after discount
                price = price * (1 - discountLevel.DiscountLevelValue / 100);

                if ((oldPrice > 0) && (price < 0))
                {
                    price = 0;
                }
            }
        }

        int stateId = cart.StateID;
        int countryId = cart.CountryID;
        bool isTaxIDSupplied = ((cart.Customer != null) && (cart.Customer.CustomerTaxRegistrationID != ""));

        if ((skuId > 0) && ((stateId > 0) || (countryId > 0)))
        {
            // Apply taxes
            price += GetSKUTotalTax(price, skuId, stateId, countryId, isTaxIDSupplied);
        }

        // Apply exchange rate
        price = cart.ApplyExchangeRate(price);

        if (FormatPrice)
        {
            // Get formatted price
            return cart.GetFormattedPrice(price, true);
        }
        else
        {
            // Get not-formated price
            return price.ToString();
        }
    }


    /// <summary>
    /// Returns product total tax in site main currency.
    /// </summary>
    /// <param name="skuPrice">SKU price</param>
    /// <param name="skuId">SKU ID</param>
    /// <param name="stateId">Customer billing address state ID</param>
    /// <param name="countryId">Customer billing addres country ID</param>
    /// <param name="isTaxIDSupplied">Indicates if customer tax registration ID is supplied</param>    
    private static double GetSKUTotalTax(double skuPrice, int skuId, int stateId, int countryId, bool isTaxIDSupplied)
    {
        double totalTax = 0;
        int cacheMinutes = 0;

        // Try to get data from cache
        using (CachedSection<double> cs = new CachedSection<double>(ref totalTax, cacheMinutes, true, null, "skutotaltax|", skuId, skuPrice, stateId, countryId, isTaxIDSupplied))
        {
            if (cs.LoadData)
            {
                // Get all the taxes and their values which are applied to the specified product
                DataSet ds = TaxClassInfoProvider.GetTaxes(skuId, countryId, stateId, null);
                if (!DataHelper.DataSourceIsEmpty(ds))
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        bool zeroTax = ValidationHelper.GetBoolean(dr["TaxClassZeroIfIDSupplied"], false);
                        if (!(isTaxIDSupplied && zeroTax))
                        {
                            double taxValue = ValidationHelper.GetDouble(dr["TaxValue"], 0);
                            bool isFlat = ValidationHelper.GetBoolean(dr["TaxIsFlat"], false);

                            // Add tax value                        
                            totalTax += TaxClassInfoProvider.GetTaxValue(skuPrice, taxValue, isFlat);
                        }
                    }
                }

                // Cache the data
                cs.Data = totalTax;
            }
        }

        return totalTax;
    }

    #endregion


    #region "Images - obsolete methods"

    /// <summary>
    /// Returns complete HTML code of the specified product image, if not such image exists, default image is returned.
    /// </summary>
    /// <param name="imageUrl">Product image URL</param>    
    /// <param name="alt">Image alternate text</param>
    [Obsolete("In transformation, use <img /> tag with src parameter set to GetSKUImageUrl() method instead.")]
    public static string GetProductImage(object imageUrl, object alt)
    {
        return GetImage(imageUrl, 0, 0, 0, alt);
    }


    /// <summary>
    /// Returns complete HTML code of the specified resized product image, if not such image exists, default image is returned.
    /// </summary>
    /// <param name="imageUrl">Product image URL</param>
    /// <param name="height">Height of image</param>
    /// <param name="alt">Image alternate text</param>
    /// <param name="width">Width of image</param>
    [Obsolete("In transformation, use <img /> tag with src parameter set to GetSKUImageUrl(width, height) method instead.")]
    public static string GetProductImage(object imageUrl, object width, object height, object alt)
    {
        return GetImage(imageUrl, width, height, 0, alt);
    }


    /// <summary>
    /// Returns complete HTML code of the specified resized product image, if such image does not exist, default image is returned.
    /// </summary>
    /// <param name="imageUrl">Product image URL</param>
    /// <param name="maxSideSize">Max side size</param>   
    /// <param name="alt">Image alternate text</param>
    [Obsolete("In transformation, use <img /> tag with src parameter set to GetSKUImageUrl(maxSideSize) method instead.")]
    public static string GetProductImage(object imageUrl, object maxSideSize, object alt)
    {
        return GetImage(imageUrl, 0, 0, maxSideSize, alt);
    }


    /// <summary>
    /// Returns complete HTML code of the specified resized product image, if not such image exists, default image is returned.
    /// </summary>
    /// <param name="imageUrl">Product image URL</param>
    /// <param name="width">Width of image</param>
    /// <param name="height">Height of image</param>
    /// <param name="alt">Image alternate text</param>
    /// <param name="siteId">ID of the site the product belongs to</param>
    [Obsolete("In transformation, use <img /> tag with src parameter set to GetSKUImageUrl(width, height) method instead.")]
    public static string GetProductImageForSite(object imageUrl, object width, object height, object alt, object siteId)
    {
        return GetImage(imageUrl, width, height, 0, alt, siteId);
    }


    /// <summary>
    /// Returns complete HTML code of the specified resized product image, if such image does not exist, default image is returned.
    /// </summary>
    /// <param name="imageUrl">Product image URL</param>
    /// <param name="maxSideSize">Max side size</param>   
    /// <param name="alt">Image alternate text</param>
    /// <param name="siteId">ID of the site the product belongs to</param>
    [Obsolete("In transformation, use <img /> tag with src parameter set to GetSKUImageUrl(maxSideSize) method instead.")]
    public static string GetProductImageForSite(object imageUrl, object maxSideSize, object alt, object siteId)
    {
        return GetImage(imageUrl, 0, 0, maxSideSize, alt, siteId);
    }


    private static string GetImage(object imageUrl, object width, object height, object maxSideSize, object alt)
    {
        return GetImage(imageUrl, width, height, maxSideSize, alt, CMSContext.CurrentSiteID);
    }


    private static string GetImage(object imageUrl, object width, object height, object maxSideSize, object alt, object siteId)
    {
        // Get image tooltip
        string tooltip = ValidationHelper.GetString(alt, "");
        if (tooltip != "")
        {
            tooltip = HTMLHelper.HTMLEncode(ResHelper.LocalizeString(tooltip));
            tooltip = string.Format(" alt=\"{0}\" title=\"{1}\"", tooltip, tooltip);
        }

        // Get SKU image URL
        string url = GetSKUImageUrl(imageUrl, width, height, maxSideSize, siteId);

        if (!string.IsNullOrEmpty(url))
        {
            return string.Format("<img src=\"{0}\" border=\"0\"{1} />", HTMLHelper.HTMLEncode(url), tooltip);
        }

        return "";
    }

    #endregion


    #region "Links"

    /// <summary>
    /// Returns link to "add to shopping cart".
    /// </summary>
    /// <param name="productId">Product ID</param>
    /// <param name="enabled">Indicates whether product is enabled or not</param>
    public static string GetAddToShoppingCartLink(object productId, object enabled)
    {
        return GetAddToShoppingCartLink(productId, enabled, null);
    }


    /// <summary>
    /// Returns link to "add to shopping cart".
    /// </summary>
    /// <param name="productId">Product ID</param>
    /// <param name="enabled">Indicates whether product is enabled or not</param>
    /// <param name="imageUrl">Image URL</param>
    public static string GetAddToShoppingCartLink(object productId, object enabled, string imageUrl)
    {
        if (ValidationHelper.GetBoolean(enabled, false) && (ValidationHelper.GetInteger(productId, 0) != 0))
        {
            // Get default image URL
            imageUrl = imageUrl ?? "CMSModules/CMS_Ecommerce/addorder.png";
            return "<img src=\"" + UIHelper.GetImageUrl(null, imageUrl) + "\" alt=\"Add to cart\" /><a href=\"" + ShoppingCartURL(SiteContext.CurrentSiteName) + "?productId=" + Convert.ToString(productId) + "&amp;quantity=1\">" + ResHelper.GetString("EcommerceFunctions.AddToShoppingCart") + "</a>";
        }
        else
        {
            return "";
        }
    }


    /// <summary>
    /// Returns link to "add to shopping cart".
    /// </summary>
    /// <param name="productId">Product ID</param>
    public static string GetAddToShoppingCartLink(object productId)
    {
        return GetAddToShoppingCartLink(productId, true);
    }


    /// <summary>
    /// Returns link to add specified product to the user's wish list.
    /// </summary>
    /// <param name="productId">Product ID</param>
    public static string GetAddToWishListLink(object productId)
    {
        return GetAddToWishListLink(productId, null);
    }


    /// <summary>
    /// Returns link to add specified product to the user's wish list.
    /// </summary>
    /// <param name="productId">Product ID</param>
    /// <param name="imageUrl">Image URL</param>
    public static string GetAddToWishListLink(object productId, string imageUrl)
    {
        if (ValidationHelper.GetInteger(productId, 0) != 0)
        {
            // Get default image URL
            imageUrl = imageUrl ?? "CMSModules/CMS_Ecommerce/addtowishlist.png";
            return "<img src=\"" + UIHelper.GetImageUrl(null, imageUrl) + "\" alt=\"Add to wishlist\" /><a href=\"" + WishlistURL(SiteContext.CurrentSiteName) + "?productId=" + Convert.ToString(productId) + "\">" + ResHelper.GetString("EcommerceFunctions.AddToWishlist") + "</a>";
        }
        else
        {
            return "";
        }
    }


    /// <summary>
    /// Returns link to remove specified product from the user's wish list.
    /// </summary>
    /// <param name="productId">Product ID</param>
    public static string GetRemoveFromWishListLink(object productId)
    {
        if ((productId != DBNull.Value) && (!MembershipContext.AuthenticatedUser.IsPublic()))
        {
            return "<a href=\"javascript:onclick=RemoveFromWishlist(" + Convert.ToString(productId) + ")\" class=\"RemoveFromWishlist\">" + ResHelper.GetString("Wishlist.RemoveFromWishlist") + "</a>";
        }
        else
        {
            return "";
        }
    }

    #endregion


    #region "URLs"

    /// <summary>
    /// Returns URL to the shopping cart.
    /// </summary>
    /// <param name="siteName">Site name</param>
    public static string ShoppingCartURL(string siteName)
    {
        return URLHelper.ResolveUrl(ECommerceSettings.ShoppingCartURL(siteName));
    }


    /// <summary>
    /// Returns URL to the wish list.
    /// </summary>
    /// <param name="siteName">Site name</param>
    public static string WishlistURL(string siteName)
    {
        return URLHelper.ResolveUrl(ECommerceSettings.WishListURL(siteName));
    }


    /// <summary>
    /// Returns product URL.
    /// </summary>
    /// <param name="SKUID">SKU ID</param>
    public static string GetProductUrl(object SKUID)
    {
        return URLHelper.ResolveUrl("~/CMSPages/GetProduct.aspx?productId=" + Convert.ToString(SKUID));
    }


    /// <summary>
    /// Returns user friendly URL of the specified SKU and site name.
    /// </summary>
    /// <param name="skuGuid">SKU Guid</param>
    /// <param name="skuName">SKU Name</param>
    /// <param name="siteNameObj">Site Name</param>
    public static string GetProductUrl(object skuGuid, object skuName, object siteNameObj)
    {
        Guid guid = ValidationHelper.GetGuid(skuGuid, Guid.Empty);
        string name = Convert.ToString(skuName);
        string siteName = ValidationHelper.GetString(siteNameObj, null);

        return URLHelper.ResolveUrl(SKUInfoProvider.GetSKUUrl(guid, name, siteName));
    }


    /// <summary>
    /// Returns user friendly URL of the specified SKU.
    /// </summary>
    /// <param name="skuGuid">SKU Guid</param>
    /// <param name="skuName">SKU Name</param>
    public static string GetProductUrl(object skuGuid, object skuName)
    {
        Guid guid = ValidationHelper.GetGuid(skuGuid, Guid.Empty);
        string name = Convert.ToString(skuName);

        return URLHelper.ResolveUrl(SKUInfoProvider.GetSKUUrl(guid, name));
    }


    /// <summary>
    /// Returns SKU image URL including dimension's modifiers (width, height or maxsidesize) and site name parameter if product is from different site than current. If image URL is not specified, SKU default image URL is used.
    /// </summary>
    /// <param name="imageUrl">SKU image URL</param>
    /// <param name="width">Image requested width, has no effect if maxsidesize is specified</param>
    /// <param name="height">Image requested height, has no effect if maxsidesize is specified</param>
    /// <param name="maxsidesize">Image requested maximum side size</param>
    /// <param name="siteId">SKU site ID. If empty, current site ID is used.</param>    
    public static string GetSKUImageUrl(object imageUrl, object width, object height, object maxsidesize, object siteId)
    {
        int iSiteId = ValidationHelper.GetInteger(siteId, 0);
        bool notCurrentSite = ((iSiteId > 0) && (iSiteId != CMSContext.CurrentSiteID));
        string siteName = null;

        // Get site name        
        if (notCurrentSite)
        {
            siteName = SiteInfoProvider.GetSiteName(iSiteId);
        }
        else
        {
            siteName = SiteContext.CurrentSiteName;
        }

        // Get product image URL  
        string url = ValidationHelper.GetString(imageUrl, null);

        if (String.IsNullOrEmpty(url))
        {
            // Get default product image URL                      
            url = ECommerceSettings.DefaultProductImageURL(siteName);
        }

        if (!String.IsNullOrEmpty(url))
        {
            // Resolve URL
            url = URLHelper.ResolveUrl(url);

            int slashIndex = url.LastIndexOfCSafe('/');
            if (slashIndex >= 0)
            {
                string urlStartPart = url.Substring(0, slashIndex);
                string urlEndPart = url.Substring(slashIndex);

                url = urlStartPart + HttpUtility.UrlPathEncode(urlEndPart);

                // Add site name if not current
                if (notCurrentSite)
                {
                    url = URLHelper.AddParameterToUrl(url, "siteName", siteName);
                }

                // Add max side size
                int iMaxSideSize = ValidationHelper.GetInteger(maxsidesize, 0);
                if (iMaxSideSize > 0)
                {
                    url = URLHelper.AddParameterToUrl(url, "maxsidesize", iMaxSideSize.ToString());
                }
                else
                {
                    // Add width
                    int iWidth = ValidationHelper.GetInteger(width, 0);
                    if (iWidth > 0)
                    {
                        url = URLHelper.AddParameterToUrl(url, "width", iWidth.ToString());
                    }

                    // Add height
                    int iHeight = ValidationHelper.GetInteger(height, 0);
                    if (iHeight > 0)
                    {
                        url = URLHelper.AddParameterToUrl(url, "height", iHeight.ToString());
                    }
                }

                // Encode URL
                url = HTMLHelper.HTMLEncode(url);
            }
        }

        return url;
    }

    #endregion


    #region "SKU related objects' properties"

    /// <summary>
    /// Returns the public SKU status display name.
    /// </summary>
    /// <param name="statusId">Status ID</param>
    [Obsolete("Use GetPublicStatusProperty(\"PublicStatusDisplayName\") in transformation instead.")]
    public static string GetPublicStatusName(object statusId)
    {
        int sid = ValidationHelper.GetInteger(statusId, 0);
        if (sid > 0)
        {
            PublicStatusInfo si = PublicStatusInfoProvider.GetPublicStatusInfo(sid);
            if (si != null)
            {
                return ResHelper.LocalizeString(si.PublicStatusDisplayName);
            }
        }

        return "";
    }


    /// <summary>
    /// Gets object from the specified column of the manufacturer with specific ID.
    /// </summary>
    /// <param name="Id">Manufacturer ID</param>
    /// <param name="column">Column name</param>
    public static object GetManufacturer(object Id, string column)
    {
        int id = ValidationHelper.GetInteger(Id, 0);
        if ((id > 0) && !DataHelper.IsEmpty(column))
        {
            // Get manufacturer
            ManufacturerInfo mi = ManufacturerInfoProvider.GetManufacturerInfo(id);

            return GetColumnValue(mi, column);
        }

        return "";
    }


    /// <summary>
    /// Gets object from the specified column of the department with specific ID.
    /// </summary>
    /// <param name="Id">Department ID</param>
    /// <param name="column">Column name</param>
    public static object GetDepartment(object Id, string column)
    {
        int id = ValidationHelper.GetInteger(Id, 0);

        if (id > 0 && !DataHelper.IsEmpty(column))
        {
            // Get department
            DepartmentInfo di = DepartmentInfoProvider.GetDepartmentInfo(id);

            return GetColumnValue(di, column);
        }

        return "";
    }


    /// <summary>
    /// Gets object from the specified column of the supplier with specific ID.
    /// </summary>
    /// <param name="Id">Supplier ID</param>
    /// <param name="column">Column name</param>
    public static object GetSupplier(object Id, string column)
    {
        int id = ValidationHelper.GetInteger(Id, 0);
        if ((id > 0) && !DataHelper.IsEmpty(column))
        {
            // Get supplier
            SupplierInfo si = SupplierInfoProvider.GetSupplierInfo(id);

            return GetColumnValue(si, column);
        }

        return "";
    }


    /// <summary>
    /// Gets object from the specified column of the internal status with specific ID.
    /// </summary>
    /// <param name="Id">Internal status ID</param>
    /// <param name="column">Column name</param>
    public static object GetInternalStatus(object Id, string column)
    {
        int id = ValidationHelper.GetInteger(Id, 0);

        if ((id > 0) && !DataHelper.IsEmpty(column))
        {
            // Get internal status
            InternalStatusInfo status = InternalStatusInfoProvider.GetInternalStatusInfo(id);

            return GetColumnValue(status, column);
        }

        return "";
    }


    /// <summary>
    /// Gets object from the specified column of the public status with specific ID.
    /// </summary>
    /// <param name="Id">Public status ID</param>
    /// <param name="column">Column name</param>
    public static object GetPublicStatus(object Id, string column)
    {
        int id = ValidationHelper.GetInteger(Id, 0);

        if ((id > 0) && !DataHelper.IsEmpty(column))
        {
            // Get public status
            PublicStatusInfo status = PublicStatusInfoProvider.GetPublicStatusInfo(id);

            return GetColumnValue(status, column);
        }

        return "";
    }


    /// <summary>
    /// Returns value of the given object column.
    /// </summary>
    /// <param name="info">Object data</param>
    /// <param name="columnName">Column name</param>    
    private static object GetColumnValue(BaseInfo info, string columnName)
    {
        if ((info != null) && info.ContainsColumn(columnName))
        {
            return info.GetValue(columnName);
        }

        return null;
    }


    /// <summary>
    /// Gets document name of specified node id.
    /// </summary>
    public static string GetDocumentName(object nodeIdent)
    {
        int nodeId = ValidationHelper.GetInteger(nodeIdent, 0);
        if (nodeId != 0)
        {
            TreeProvider tree = new TreeProvider(MembershipContext.AuthenticatedUser);
            TreeNode node = tree.SelectSingleNode(nodeId, LocalizationContext.PreferredCultureCode);
            if (node != null)
            {
                return node.GetDocumentName();
            }
        }
        return String.Empty;
    }

    #endregion


    #region "SKU properties"

    /// <summary>
    /// Returns value of the specified product public status column.
    /// If the product is evaluated as a new product in the store, public status set by 'CMSStoreNewProductStatus' setting is used, otherwise product public status is used.
    /// </summary>
    /// <param name="sku">SKU data</param>
    /// <param name="column">Name of the product public status column the value should be retrieved from</param>
    public static object GetSKUIndicatorProperty(SKUInfo sku, string column)
    {
        // Do not process
        if (sku == null)
        {
            return null;
        }

        PublicStatusInfo status = null;
        string siteName = SiteInfoProvider.GetSiteName(sku.SKUSiteID);
        string statusName = ECommerceSettings.NewProductStatus(siteName);

        if (!string.IsNullOrEmpty(statusName) && SKUInfoProvider.IsSKUNew(sku))
        {
            // Get 'new product' status            
            status = PublicStatusInfoProvider.GetPublicStatusInfo(statusName, siteName);
        }
        else
        {
            // Get product public status
            if (sku.SKUPublicStatusID > 0)
            {
                status = PublicStatusInfoProvider.GetPublicStatusInfo(sku.SKUPublicStatusID);
            }
        }

        // Get specified column value
        return GetColumnValue(status, column);
    }


    /// <summary>
    /// Returns amount of saved money based on the difference between product seller price and product retail price.
    /// </summary> 
    /// <param name="discounts">Indicates if discounts should be applied to the seller price before the saved amount is calculated</param>
    /// <param name="taxes">Indicates if taxes should be applied to both retail price and seller price before the saved amount is calculated</param>
    /// <param name="column1">Name of the column from which the seller price is retrieved, if empty SKUPrice column is used</param>
    /// <param name="column2">Name of the column from which the retail price is retrieved, if empty SKURetailPrice column is used</param>
    /// <param name="percentage">True - result is percentage, False - result is in the current currency</param>
    public static double GetSKUPriceSaving(SKUInfo sku, bool discounts, bool taxes, string column1, string column2, bool percentage)
    {
        // Do not process
        if (sku == null)
        {
            return 0;
        }

        // Ensure columns
        column1 = string.IsNullOrEmpty(column1) ? "SKUPrice" : column1;
        column2 = string.IsNullOrEmpty(column2) ? "SKURetailPrice" : column2;

        // Prices        
        double price = SKUInfoProvider.GetSKUPrice(sku, null, discounts, taxes, false, column1);
        double retailPrice = SKUInfoProvider.GetSKUPrice(sku, null, false, taxes, false, column2);

        // If shopping cart is in context - apply exchange rates
        if (ECommerceContext.CurrentShoppingCart != null)
        {
            retailPrice = ECommerceContext.CurrentShoppingCart.ApplyExchangeRate(retailPrice);
            price = ECommerceContext.CurrentShoppingCart.ApplyExchangeRate(price);
        }

        // Round prices using current currency
        price = CurrencyInfoProvider.RoundTo(price, ECommerceContext.CurrentCurrency);
        retailPrice = CurrencyInfoProvider.RoundTo(retailPrice, ECommerceContext.CurrentCurrency);
        
        // Saved amount
        double savedAmount = retailPrice - price;

        // When seller price is greater than retail price
        if (((price > 0) && (savedAmount < 0)) || ((price < 0) && (savedAmount > 0)))
        {
            // Zero saved amount
            savedAmount = 0;
        }
        else if (percentage)
        {
            // Percentage saved amount
            savedAmount = ((retailPrice == 0) ? 0 : Math.Round(100 * savedAmount / retailPrice));
        }

        return savedAmount;
    }

    #endregion


    #region "UI methods"

    /// <summary>
    /// Sets different css styles to enabled and disabled dropdownlist items.
    /// </summary>
    /// <param name="drpTemp">Dropdownlist control</param>
    /// <param name="valueFieldName">Field name with ID value</param>
    /// <param name="statusFieldName">Field name with status value</param>
    /// <param name="itemEnabledStyle">Enabled item style</param>
    /// <param name="itemDisabledStyle">Disabled item style</param>
    public static void MarkEnabledAndDisabledItems(DropDownList drpTemp, string valueFieldName, string statusFieldName, string itemEnabledStyle, string itemDisabledStyle)
    {
        itemEnabledStyle = (itemEnabledStyle == "") ? "DropDownItemEnabled" : itemEnabledStyle;
        itemDisabledStyle = (itemDisabledStyle == "") ? "DropDownItemDisabled" : itemDisabledStyle;

        if (!DataHelper.DataSourceIsEmpty(drpTemp.DataSource))
        {
            if (drpTemp.DataSource is DataSet)
            {
                ListItem li = null;

                foreach (DataRow row in ((DataSet)drpTemp.DataSource).Tables[0].Rows)
                {
                    li = drpTemp.Items.FindByValue(Convert.ToString(row[valueFieldName]));
                    if ((li != null) && (li.Value != "0"))
                    {
                        // Item is enabled
                        if (ValidationHelper.GetBoolean(row[statusFieldName], false))
                        {
                            li.Attributes.Add("class", itemEnabledStyle);
                        }
                        // Item is disabled
                        else
                        {
                            li.Attributes.Add("class", itemDisabledStyle);
                        }
                    }
                }
            }
        }
    }


    /// <summary>
    /// Sets different css styles to enabled and disabled dropdownlist items.
    /// </summary>
    /// <param name="drpTemp">Dropdownlist control</param>
    /// <param name="valueFieldName">Field name with ID value</param>
    /// <param name="statusFieldName">Field name with status value</param>
    public static void MarkEnabledAndDisabledItems(DropDownList drpTemp, string valueFieldName, string statusFieldName)
    {
        MarkEnabledAndDisabledItems(drpTemp, valueFieldName, statusFieldName, "", "");
    }

    #endregion
}