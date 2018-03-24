using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.Controls;
using CMS.DataEngine;
using CMS.Ecommerce;
using CMS.Base;
using CMS.SiteProvider;
using CMS.UIControls;

public partial class CMSModules_Ecommerce_Controls_Filters_SimpleProductFilter : CMSAbstractDataFilterControl
{
    #region "Properties"

    /// <summary>
    /// Gets or sets the SQL condition for filtering the order list.
    /// </summary>
    public override string WhereCondition
    {
        get
        {
            base.WhereCondition = GetWhereCondition();
            return base.WhereCondition;
        }
        set
        {
            base.WhereCondition = value;
        }
    }

    #endregion


    #region "Page Events"

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        // If this control is associated with an UniGrid control, disable UniGrid's button
        UniGrid grid = FilteredControl as UniGrid;
        if (grid != null)
        {
            grid.HideFilterButton = true;
        }
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        btnShow.Text = GetString("general.filter");
        btnReset.Text = GetString("general.reset");
    }

    #endregion


    #region "Event Handlers"

    protected void btnShow_Click(object sender, EventArgs e)
    {
        UniGrid grid = FilteredControl as UniGrid;
        if (grid != null)
        {
            grid.ApplyFilter(sender, e);
        }
    }


    protected void btnReset_Click(object sender, EventArgs e)
    {
        UniGrid grid = FilteredControl as UniGrid;
        if (grid != null)
        {
            grid.Reset();
        }
    }

    #endregion


    #region "Methods"
    
    /// <summary>
    /// Get where condition for unigrid
    /// </summary>
    /// <returns>Where condition</returns>
    private string GetWhereCondition()
    {
        string productNameFilter = SqlHelper.GetSafeQueryString(txtProductName.Text);
        string productCodeFilter = SqlHelper.GetSafeQueryString(txtProductCode.Text);

        // To display ONLY product - not product options
        string where = "(SKUEnabled = 1) AND (SKUOptionCategoryID IS NULL)";
        string tempCondition = "";

        if (!string.IsNullOrEmpty(productNameFilter))
        {
            // Condition to get also products with variants that contains 
            string variantCondition = string.Format("SKUID IN (SELECT SKUParentSKUID FROM COM_SKU WHERE SKUName LIKE N'%{0}%')", productNameFilter);

            tempCondition = string.Format("(SKUName LIKE N'%{0}%')", productNameFilter);
            tempCondition = SqlHelper.AddWhereCondition(tempCondition, variantCondition, "OR");

            where = SqlHelper.AddWhereCondition(where, tempCondition);
        }
        if (productCodeFilter != "")
        {
            // Condition to get also products with variants that contains 
            string variantCondition = string.Format("SKUID IN (SELECT SKUParentSKUID FROM COM_SKU WHERE SKUNumber LIKE N'%{0}%')", productCodeFilter);

            tempCondition = string.Format("(SKUNumber LIKE N'%{0}%')", productCodeFilter);
            tempCondition = SqlHelper.AddWhereCondition(tempCondition, variantCondition, "OR");

            where = SqlHelper.AddWhereCondition(where, tempCondition);
        }
        if (departmentElem.SelectedID > 0)
        {
            where = SqlHelper.AddWhereCondition(where," SKUDepartmentID = " + departmentElem.SelectedID);
        }

        where = SKUInfoProvider.ProviderObject.AddSiteWhereCondition(where, SiteContext.CurrentSiteID, ECommerceSettings.ALLOW_GLOBAL_PRODUCTS, true);

        return where;
    }


    /// <summary>
    /// Resets filter to the default state.
    /// </summary>
    public override void ResetFilter()
    {
        txtProductName.Text = String.Empty;
        txtProductCode.Text = String.Empty;
        departmentElem.SelectedID = -1;
    }

    #endregion
}
