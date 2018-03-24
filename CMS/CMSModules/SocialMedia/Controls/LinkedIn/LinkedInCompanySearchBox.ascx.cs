using System;

using CMS.Controls;
using CMS.DataEngine;
using CMS.UIControls;

public partial class CMSModules_SocialMedia_Controls_LinkedIn_LinkedInCompanySearchBox : CMSAbstractBaseFilterControl
{
    #region "State management"

    /// <summary>
    /// Applies filter on associated UniGrid control.
    /// </summary>
    protected void btnSearch_Click(object sender, EventArgs e)
    {
        string where = null;
        if (!string.IsNullOrEmpty(txtSearch.Text))
        {
            where = "CompanyName LIKE '%" + SqlHelper.GetSafeQueryString(txtSearch.Text, false) + "%'";
        }
        WhereCondition = where;

        //Raise OnFilterChange event
        RaiseOnFilterChanged();
    }

    #endregion
}