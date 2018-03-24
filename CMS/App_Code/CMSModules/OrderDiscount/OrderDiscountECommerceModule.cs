using System;

using CMS.GlobalHelper;
using CMS.SettingsProvider;
using CMS.Ecommerce;
using CMS.Base;
using CMS.Core;

/// <summary>
/// Sample e-commerce module class. Partial class ensures correct registration.
/// </summary>
[OrderDiscountECommerceModuleLoader]
public partial class CMSModuleLoader : CMSModuleLoaderBase
{
    #region "Macro methods loader attribute"

    /// <summary>
    /// Module registration
    /// </summary>
    private class OrderDiscountECommerceModuleLoaderAttribute : CMSLoaderAttribute
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public OrderDiscountECommerceModuleLoaderAttribute()
        {
            // Require E-commerce module to load properly
            RequiredModules = new string[] { CMS.Core.ModuleName.ECOMMERCE }; 
        }


        /// <summary>
        /// Initializes the module
        /// </summary>
        public override void Init()
        {
            // -- Uncomment this line to register the CustomShoppingCartInfoProvider programatically
            ShoppingCartInfoProvider.ProviderObject = new OrderDiscount();
        }
    }

    #endregion
}
