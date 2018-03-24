using CMS.Controls;
using CMS.Core;
using CMS.OnlineMarketing;
using CMS.Base;
using CMS.WorkflowEngine;

/// <summary>
/// Online marketing functions loader (registers online marketing functions to macro resolver).
/// </summary>
[OnlineMarketingModuleLoader]
public partial class CMSModuleLoader : CMSModuleLoaderBase
{
    /// <summary>
    /// Attribute class ensuring correct initialization of methods in macro resolver.
    /// </summary>
    private class OnlineMarketingModuleLoaderAttribute : CMSLoaderAttribute
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public OnlineMarketingModuleLoaderAttribute()
        {
            // Require Online marketing module to load properly
            RequiredModules = new[] { ModuleName.ONLINEMARKETING };
        }
    }
}