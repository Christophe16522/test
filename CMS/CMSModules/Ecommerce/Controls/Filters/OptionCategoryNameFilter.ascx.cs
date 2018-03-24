using System;

using CMS.Controls;
using CMS.DataEngine;
using CMS.ExtendedControls;
using CMS.Helpers;


public partial class CMSModules_Ecommerce_Controls_Filters_OptionCategoryNameFilter : CMSAbstractDataFilterControl
{
    /// <summary>
    /// Where condition.
    /// </summary>
    public override string WhereCondition
    {
        get
        {
            base.WhereCondition = GenerateWhereCondition();
            return base.WhereCondition;
        }
        set
        {
            base.WhereCondition = value;
        }
    }


    #region "Methods"

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
        if (!RequestHelper.IsPostBack())
        {
            Reload();
        }
    }


    /// <summary>
    /// Reloads control.
    /// </summary>
    protected void Reload()
    {
        filter.Items.Clear();
        ControlsHelper.FillListWithTextSqlOperators(filter);
    }


    /// <summary>
    /// Generates WHERE condition.
    /// </summary>
    private string GenerateWhereCondition()
    {
        if (String.IsNullOrEmpty(txtName.Text))
        {
            return null;
        }

        string op = WhereBuilder.LIKE;
        string name = SqlHelper.GetSafeQueryString(txtName.Text, false);

        // Get filter operator (LIKE, NOT LIKE, =, <>)
        if (filter.SelectedValue != null)
        {
            op = filter.SelectedValue;
        }

        if ((op == WhereBuilder.LIKE) || (op == WhereBuilder.NOT_LIKE))
        {
            name = "%" + name + "%";
        }

        if ((op == WhereBuilder.NOT_LIKE) || (op == WhereBuilder.NOT_EQUAL))
        {
            return string.Format("(((CategoryDisplayName {0} '{1}') OR (CategoryDisplayName IS NULL)) "
                                 + "AND ((CategoryLiveSiteDisplayName {0} '{1}') OR (CategoryLiveSiteDisplayName IS NULL)))", op, name);
        }

        if ((op == WhereBuilder.LIKE) || (op == WhereBuilder.EQUAL))
        {
            return string.Format("((CategoryDisplayName {0} '{1}') OR (CategoryLiveSiteDisplayName {0} '{1}'))", op, name);
        }

        return null;
    }

    #endregion


    #region "State management"

    /// <summary>
    /// Stores filter state to the specified object.
    /// </summary>
    /// <param name="state">The object that holds the filter state.</param>
    public override void StoreFilterState(FilterState state)
    {
        state.AddValue("condition", filter.SelectedValue);
        state.AddValue("categoryName", txtName.Text);
    }


    /// <summary>
    /// Restores filter state from the specified object.
    /// </summary>
    /// <param name="state">The object that holds the filter state.</param>
    public override void RestoreFilterState(FilterState state)
    {
        filter.SelectedValue = state.GetString("condition");
        txtName.Text = state.GetString("categoryName");
    }


    /// <summary>
    /// Resets the filter settings.
    /// </summary>
    public override void ResetFilter()
    {
        filter.SelectedIndex = 0;
        txtName.Text = String.Empty;
    }

    #endregion

}
