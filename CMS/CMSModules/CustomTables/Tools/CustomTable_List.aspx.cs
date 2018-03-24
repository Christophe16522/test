using System;
using System.Data;
using System.Collections;

using CMS.CustomTables;
using CMS.Helpers;
using CMS.Membership;
using CMS.SiteProvider;
using CMS.UIControls;
using CMS.DataEngine;

[UIElement("CMS.CustomTables", "CustomTables")]
public partial class CMSModules_CustomTables_Tools_CustomTable_List : CMSCustomTablesToolsPage
{
    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        // Initialize master page
        PageTitle.TitleText = GetString("customtable.list.Title");
        // Initialize unigrid
        uniGrid.OnAction += uniGrid_OnAction;
        uniGrid.ZeroRowsText = GetString("customtable.notable");
        uniGrid.OnDataReload += uniGrid_OnDataReload;
        uniGrid.WhereCondition = "ClassID IN (SELECT ClassID FROM CMS_ClassSite WHERE SiteID = " + SiteContext.CurrentSite.SiteID + ")";
    }

    #endregion


    #region "Unigrid events"

    /// <summary>
    /// Data reloading event handler.
    /// </summary>
    /// <param name="completeWhere">Complete where condition</param>
    /// <param name="currentOrder">Current order by clause</param>
    /// <param name="currentTopN">Current top N value</param>
    /// <param name="columns">Currently selected columns</param>
    /// <param name="currentOffset">Current page offset</param>
    /// <param name="currentPageSize">Current size of page</param>
    /// <param name="totalRecords">Returns number of returned records</param>
    protected DataSet uniGrid_OnDataReload(string completeWhere, string currentOrder, int currentTopN, string columns, int currentOffset, int currentPageSize, ref int totalRecords)
    {
        var data = CustomTableHelper.GetFilteredTablesByPermission(completeWhere, currentOrder, currentTopN, columns);

        totalRecords = data.Tables[0].Rows.Count;

        // Redirect to access denied page if user doesn't have permission to any custom table only if filter is not set
        if ((totalRecords == 0) && !uniGrid.FilterIsSet)
        {
            MissingPermissionsRedirect();
        }

        return data;
    }


    /// <summary>
    /// Handles the UniGrid's OnAction event.
    /// </summary>
    /// <param name="actionName">Name of item (button) that throws event</param>
    /// <param name="actionArgument">ID (value of Primary key) of corresponding data row</param>
    protected void uniGrid_OnAction(string actionName, object actionArgument)
    {
        if (actionName == "edit")
        {
            int classId = ValidationHelper.GetInteger(actionArgument, 0);
            DataClassInfo dci = DataClassInfoProvider.GetDataClassInfo(classId);
            if (dci != null)
            {
                // Check if custom table class hasn't set specific listing page
                if (dci.ClassListPageURL != String.Empty)
                {
                    URLHelper.Redirect(dci.ClassListPageURL + "?objectid=" + classId);
                }
                else
                {
                    URLHelper.Redirect("CustomTable_Data_List.aspx?objectid=" + classId);
                }
            }
        }
    }

    #endregion


    #region "Other methods"

    /// <summary>
    /// Redirects to access denied page with appropriate message.
    /// </summary>
    private void MissingPermissionsRedirect()
    {
        RedirectToAccessDenied(GetString("customtable.anytablepermiss"));
    }

    #endregion
}