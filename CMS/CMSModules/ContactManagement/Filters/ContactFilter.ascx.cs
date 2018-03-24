using System;

using CMS.Controls;
using CMS.DataEngine;

public partial class CMSModules_ContactManagement_Filters_ContactFilter : CMSAbstractBaseFilterControl
{
    #region "Methods"

    /// <summary>
    /// Create WHERE condition.
    /// </summary>
    private string CreateWhereCondition()
    {
        string text = txtContact.Text.Trim();
        return "(ContactFirstName LIKE N'%" + SqlHelper.GetSafeQueryString(text) + "%' OR ContactMiddleName LIKE N'%" + SqlHelper.GetSafeQueryString(text) + "%' OR ContactLastName LIKE N'%" + SqlHelper.GetSafeQueryString(text) + "%')";
    }


    /// <summary>
    /// Button search click.
    /// </summary>
    protected void btnSearch_Click(object sender, EventArgs e)
    {
        WhereCondition = CreateWhereCondition();
        RaiseOnFilterChanged();
    }

    #endregion
}