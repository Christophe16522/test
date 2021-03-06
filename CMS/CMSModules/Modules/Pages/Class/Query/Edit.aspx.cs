using System;

using CMS.Core;
using CMS.Helpers;
using CMS.PortalEngine;
using CMS.UIControls;
using CMS.DataEngine;


[EditedObject(QueryInfo.OBJECT_TYPE, "objectid")]

[SaveAction(1)]
public partial class CMSModules_Modules_Pages_Class_Query_Edit : GlobalAdminPage
{
    /// <summary>
    /// Edited query object.
    /// </summary>
    private QueryInfo QueryObject
    {
        get
        {
            return (QueryInfo)EditedObject;
        }
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        // Get the current class ID
        var classID = queryEdit.ClassID = QueryHelper.GetInteger("parentobjectid", 0);
        var moduleID = queryEdit.ModuleID = QueryHelper.GetInteger("moduleid", 0);

        // Init breadcrumbs
        PageBreadcrumbs.AddBreadcrumb(new BreadcrumbItem()
        {
            Index = 0,
            Text = GetString("sysdev.class_edit_query_edit.queries"),
            RedirectUrl = URLHelper.AppendQuery(UIContextHelper.GetElementUrl(ModuleName.CMS, "Queries"), "parentobjectid=" + classID + "&moduleid=" + moduleID + "&displaytitle=false")
        });

        if (QueryObject == null)
        {
            PageBreadcrumbs.AddBreadcrumb(new BreadcrumbItem()
            {
                Index = 1,
                Text = GetString("sysdev.class_edit_query_edit.newquery"),
            });
        }

        // If the existing query is being edited
        if (QueryObject != null)
        {
            queryEdit.QueryID = QueryObject.QueryID;

            // Add the current item
            PageBreadcrumbs.AddBreadcrumb(new BreadcrumbItem()
            {
                Text = GetString(QueryObject.QueryName),
            });
        }
        else
        {
            // If the new query is being created        
            queryEdit.QueryID = 0;
        }
    }
}
