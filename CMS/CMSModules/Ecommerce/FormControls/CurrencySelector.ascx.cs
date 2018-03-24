using CMS.DataEngine;
using CMS.Ecommerce;
using CMS.ExtendedControls;
using CMS.Helpers;
using CMS.UIControls;
using CMS.FormEngine;


public partial class CMSModules_Ecommerce_FormControls_CurrencySelector : SiteSeparatedObjectSelector
{
    #region "Properties"

    /// <summary>
    ///  If true, selected value is CurrencyName, if false, selected value is CurrencyID.
    /// </summary>
    public override bool UseNameForSelection
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("UseCurrencyNameForSelection"), base.UseNameForSelection);
        }
        set
        {
            SetValue("UseCurrencyNameForSelection", value);
            base.UseNameForSelection = value;
        }
    }


    /// <summary>
    /// Indicates if only currencies with exchange rate will be displayed. Main currency will be included. Default value is false. 
    /// </summary>
    public bool DisplayOnlyWithExchangeRate
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("DisplayOnlyWithExchangeRate"), false);
        }
        set
        {
            SetValue("DisplayOnlyWithExchangeRate", value);
        }
    }


    /// <summary>
    /// Gets or sets the display name format.
    /// </summary>
    public string DisplayNameFormat
    {
        get
        {
            return uniSelector.DisplayNameFormat;
        }
        set
        {
            uniSelector.DisplayNameFormat = value;
        }
    }


    /// <summary>
    /// Returns inner CMSDropDownList control.
    /// </summary>
    public CMSDropDownList DropDownSingleSelect
    {
        get
        {
            return uniSelector.DropDownSingleSelect;
        }
    }


    /// <summary>
    /// Returns inner UniSelector control.
    /// </summary>
    public override UniSelector UniSelector
    {
        get
        {
            return uniSelector;
        }
    }


    /// <summary>
    /// Convert given currency name to its ID for specified site.
    /// </summary>
    /// <param name="name">Name of the currency to be converted.</param>
    /// <param name="siteName">Name of the site of the currency.</param>
    protected override int GetID(string name, string siteName)
    {
        CurrencyInfo currency = CurrencyInfoProvider.GetCurrencyInfo(name, siteName);

        return (currency != null) ? currency.CurrencyID : 0;
    }


    /// <summary>
    /// Indicates whether to show all items ("more items" is not displayed).
    /// </summary>
    public bool ShowAllItems
    {
        get;
        set;
    }


    /// <summary>
    /// Add (select) record to the drop-down list.
    /// </summary>
    public bool AddSelectRecord
    {
        get;
        set;
    }


    /// <summary>
    /// Indicates whether to add current site default currency.
    /// </summary>
    public bool AddSiteDefaultCurrency
    {
        get;
        set;
    }


    /// <summary>
    /// Indicates whether to exclude current site default currency.
    /// </summary>
    public bool ExcludeSiteDefaultCurrency
    {
        get;
        set;
    }


    /// <summary>
    /// Indicates whether update panel is enabled.
    /// </summary>
    public bool DoFullPostback
    {
        get;
        set;
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Selector initialization.
    /// </summary>
    protected override void InitSelector()
    {
        base.InitSelector();

        if (ShowAllItems)
        {
            uniSelector.MaxDisplayedItems = 1000;
        }

        if (DoFullPostback)
        {
            // Disable update panel
            ControlsHelper.RegisterPostbackControl(uniSelector.DropDownSingleSelect);
        }

        if (AddSelectRecord)
        {
            // Add (select) item to the UniSelector
            uniSelector.SpecialFields.Add(new SpecialField { Text = GetString("currencyselector.select"), Value = "-1" });

        }
    }


    /// <summary>
    /// Generates where condition for uniselector.
    /// </summary>
    protected override string GenerateWhereCondition()
    {
        CurrencyInfo main = CurrencyInfoProvider.GetMainCurrency(SiteID);
        int mainCurrencyId = (main != null) ? main.CurrencyID : 0;

        // Prepare where condition
        string where = "";
        if (DisplayOnlyWithExchangeRate)
        {
            ExchangeTableInfo tableInfo = ExchangeTableInfoProvider.GetLastExchangeTableInfo(SiteID);
            if (tableInfo != null)
            {
                where = "(CurrencyID = " + mainCurrencyId + " OR CurrencyID IN (SELECT ExchangeRateToCurrencyID FROM COM_CurrencyExchangeRate WHERE COM_CurrencyExchangeRate.ExchangeTableID = " + tableInfo.ExchangeTableID + ") AND CurrencyEnabled = 1)";
            }
            else
            {
                where = "(0=1)";
            }
        }

        // Add site main currency when required
        if (AddSiteDefaultCurrency && (main != null))
        {
            where = SqlHelper.AddWhereCondition(where, "CurrencyID = " + mainCurrencyId, "OR");
        }

        // Exclude site main currency when required
        if (ExcludeSiteDefaultCurrency && (main != null))
        {
            where = SqlHelper.AddWhereCondition(where, "(NOT CurrencyID = " + mainCurrencyId + ")");
        }

        // Add base where condition
        return SqlHelper.AddWhereCondition(where, base.GenerateWhereCondition());
    }

    #endregion
}