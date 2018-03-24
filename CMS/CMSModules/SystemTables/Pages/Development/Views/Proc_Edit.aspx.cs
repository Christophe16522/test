using System;
using System.Web.UI.WebControls;

using CMS.Core;
using CMS.DataEngine;
using CMS.ExtendedControls;
using CMS.Helpers;
using CMS.UIControls;
using CMS.ExtendedControls.ActionsConfig;
using CMS.Base;
using CMS.PortalEngine;

public partial class CMSModules_SystemTables_Pages_Development_Views_Proc_Edit : GlobalAdminPage
{
    #region "Private variables"

    private string objName = null;

    #endregion


    protected void Page_Load(object sender, EventArgs e)
    {
        // Query param validation
        string hash = QueryHelper.GetString("hash", null);

        string paramName = "new";
        string newView = QueryHelper.GetString(paramName, null);
        if (String.IsNullOrEmpty(newView))
        {
            paramName = "objname";
        }

        if (!QueryHelper.ValidateHashString(QueryHelper.GetString(paramName, null), hash))
        {
            ShowError(GetString("sysdev.views.corruptedparameters"));
            editSQL.Visible = false;
            return;
        }

        if (QueryHelper.GetInteger("saved", 0) == 1)
        {
            ShowChangesSaved();
        }

        objName = QueryHelper.GetString("objname", null);

        TableManager tm = new TableManager(null);

        if (!String.IsNullOrEmpty(objName) && !tm.StoredProcedureExists(objName))
        {
            EditedObject = null;
        }

        // Init edit area
        editSQL.ObjectName = objName;
        editSQL.HideSaveButton = objName != null;
        editSQL.IsView = false;
        editSQL.OnSaved += editSQL_OnSaved;
        bool loadedCorrectly = true;
        if (!RequestHelper.IsPostBack())
        {
            loadedCorrectly = editSQL.SetupControl();
        }

        // Create breadcrumbs
        CreateBreadcrumbs();

        // Edit menu
        if (objName != null)
        {
            // Save button
            HeaderActions.AddAction(new SaveAction(Page)
            {
                Enabled = loadedCorrectly,
                RegisterShortcutScript = loadedCorrectly
            });

            // Undo button
            if(editSQL.RollbackAvailable){
            HeaderActions.AddAction(new HeaderAction
            {
                Text = GetString("General.Rollback"),
                CommandName = "rollback"
            });
            }

            HeaderActions.ActionPerformed += HeaderActions_ActionPerformed;
        }
    }


    /// <summary>
    /// Creates breadcrumbs
    /// </summary>
    private void CreateBreadcrumbs()
    {
        var isNew = String.IsNullOrEmpty(objName);
        PageBreadcrumbs.AddBreadcrumb(new BreadcrumbItem
        {
            Text = GetString("sysdev.procedures"),
            RedirectUrl = UIContextHelper.GetElementUrl(ModuleName.CMS, "StoredProcedures", false),
        });
        PageBreadcrumbs.AddBreadcrumb(new BreadcrumbItem
        {
            Text = isNew ? GetString("sysdev.procedures.createprocedure") : objName
        });

        // Ensure suffix
        UIHelper.SetBreadcrumbsSuffix(isNew ? "" : GetString("sysdev.procedure"));
    }


    #region "Event handlers"

    private void HeaderActions_ActionPerformed(object sender, CommandEventArgs e)
    {
        switch (e.CommandName)
        {
            case "rollback":
                editSQL.Rollback();
                break;

            case ComponentEvents.SAVE:
                editSQL.SaveQuery();
                break;
        }
    }


    private void editSQL_OnSaved(object sender, EventArgs e)
    {
        if (objName == null)
        {
            string url = UIContextHelper.GetElementUrl(ModuleName.CMS, "EditStoredProcedures", false);
            url = URLHelper.AddParameterToUrl(url, "objname", HTMLHelper.HTMLEncode(editSQL.ObjectName));
            url = URLHelper.AddParameterToUrl(url, "hash", QueryHelper.GetHash(editSQL.ObjectName, true));
            url = URLHelper.AddParameterToUrl(url, "saved", "1");
            URLHelper.Redirect(url);
        }

        ShowChangesSaved();
    }

    #endregion
}