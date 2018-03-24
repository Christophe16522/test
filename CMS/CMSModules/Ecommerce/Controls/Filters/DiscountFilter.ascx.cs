using System;
using System.Web.UI.WebControls;
using CMS.Controls;
using CMS.DataEngine;
using CMS.Helpers;
using CMS.UIControls;
using CMS.ExtendedControls;

public partial class CMSModules_Ecommerce_Controls_Filters_DiscountFilter : CMSAbstractDataFilterControl
{
    #region "Public properties"

    /// <summary>
    /// Gets or sets the SQL condition for filtering the discount list.
    /// </summary>
    public override string WhereCondition
    {
        get
        {
            base.WhereCondition = GetFilterWhereCondition();
            return base.WhereCondition;
        }
        set
        {
            base.WhereCondition = value;
        }
    }

    #endregion


    #region "Page events"
    
    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
        InitializeDropDownList(drpStatus);
    }

    #endregion


    #region "Event handlers"

    protected void btnFilter_Click(object sender, EventArgs e)
    {
        ApplyFilter(sender, e);
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


    #region "Private methods"

    /// <summary>
    /// Applies filter to unigrid.
    /// </summary>
    /// <param name="sender">Sender.</param>
    /// <param name="e">Event args.</param>
    private void ApplyFilter(object sender, EventArgs e)
    {
        UniGrid grid = FilteredControl as UniGrid;
        if (grid != null)
        {
            grid.ApplyFilter(sender, e);
        }
    }


    /// <summary>
    /// Initializes the specified controls with values.
    /// </summary>
    /// <param name="control">A control to initialize.</param>
    private void InitializeDropDownList(CMSDropDownList control)
    {
        control.Items.Add(new ListItem(GetString("general.selectall"), "-1"));
        control.Items.Add(new ListItem(GetString("com.discountstatus.active"), "0"));
        control.Items.Add(new ListItem(GetString("com.discountstatus.disabled"), "1"));
        control.Items.Add(new ListItem(GetString("com.discountstatus.finished"), "2"));
        control.Items.Add(new ListItem(GetString("com.discountstatus.notstarted"), "3"));
    }


    /// <summary>
    /// Builds a SQL condition for filtering the discount list, and returns it.
    /// </summary>
    /// <returns>A SQL condition for filtering the discount list.</returns>
    private string GetFilterWhereCondition()
    {
        string condition = String.Empty;

        string discountStatus = drpStatus.SelectedValue;
        /* Active discounts */
        if (discountStatus == "0")
        {
            condition = SqlHelper.AddWhereCondition(condition, "DiscountValidFrom < CURRENT_TIMESTAMP OR DiscountValidFrom IS NULL");
            condition = SqlHelper.AddWhereCondition(condition, "DiscountValidTo > CURRENT_TIMESTAMP OR DiscountValidTo IS NULL");
            condition = SqlHelper.AddWhereCondition(condition, "DiscountEnabled = 1");
        }
        /* Disabled discounts */
        else if (discountStatus == "1")
        {
            condition = SqlHelper.AddWhereCondition(condition, "DiscountEnabled = 0");
        }
        /* Finished discounts */
        else if (discountStatus == "2")
        {
            condition = SqlHelper.AddWhereCondition(condition, "DiscountValidTo < CURRENT_TIMESTAMP");
            condition = SqlHelper.AddWhereCondition(condition, "DiscountEnabled = 1");
        }
        /* Not started discounts */
        else if (discountStatus == "3")
        {
            condition = SqlHelper.AddWhereCondition(condition, "DiscountValidFrom > CURRENT_TIMESTAMP");
            condition = SqlHelper.AddWhereCondition(condition, "DiscountEnabled = 1");
        }

        return condition;
    }

    #endregion


    #region "State management"

    /// <summary>
    /// Resets filter to the default state.
    /// </summary>
    public override void ResetFilter()
    {
        drpStatus.SelectedValue = "-1";
    }


    /// <summary>
    /// Stores filter state to the specified object.
    /// </summary>
    /// <param name="state">The object that holds the filter state.</param>
    public override void StoreFilterState(FilterState state)
    {
        base.StoreFilterState(state);

        // Store additional state properties
        state.AddValue("drpStatus", drpStatus.SelectedValue);
    }


    /// <summary>
    /// Restores filter state from the specified object.
    /// </summary>
    /// <param name="state">The object that holds the filter state.</param>
    public override void RestoreFilterState(FilterState state)
    {
        base.RestoreFilterState(state);

        // Restore additional state properties
        drpStatus.SelectedValue = state.GetString("drpStatus", "-1");
    }



    #endregion
}
