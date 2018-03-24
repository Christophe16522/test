using System;
using System.Web;
using System.Web.Caching;
using System.Web.UI.WebControls;

using CMS.Base;
using CMS.ExtendedControls.ActionsConfig;
using CMS.Helpers;
using CMS.UIControls;

[Title("ViewObject.Title")]
public partial class CMSModules_System_Debug_System_ViewObject : CMSDebugPage
{
    #region "Variables"

    protected string key = null;
    protected string source = null;

    protected bool wasDeleted = false;

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        PageTitle.IsDialog = true;

        ScriptHelper.RegisterWOpenerScript(this);

        // Delete all action
        CurrentMaster.HeaderActions.AddAction(new HeaderAction()
        {
            Text = GetString("general.delete"),
            OnClientClick = "if (!confirm(" + ScriptHelper.GetString(GetString("general.confirmdelete")) + ")) return false;",
            Tooltip = GetString("general.delete"),
            CommandName = "delete"
        });
        CurrentMaster.HeaderActions.ActionPerformed += actionsElem_ActionPerformed;

        gridDependencies.OnItemDeleted += gridDependencies_OnItemDeleted;

        source = QueryHelper.GetString("source", "");
        key = QueryHelper.GetString("key", "");

        ReloadData();
    }


    protected void gridDependencies_OnItemDeleted(object sender, EventArgs e)
    {
        wasDeleted = true;

        // Close the window
        ScriptHelper.CloseWindow(Page, true);
    }


    protected void ReloadData()
    {
        object obj = null;

        switch (source.ToLowerCSafe())
        {
            case "cache":
                {
                    // Get the item from cache
                    obj = HttpRuntime.Cache[key];

                    // Take the object from the cache
                    if (obj != null)
                    {
                        if (obj is CacheItemContainer)
                        {
                            // Setup the advanced information
                            CacheItemContainer container = (CacheItemContainer)obj;
                            obj = container.Data;

                            // Get the inner value
                            obj = CacheHelper.GetInnerValue(obj);

                            ltlKey.Text = key;
                            ltlPriority.Text = container.Priority.ToString();
                            if (container.AbsoluteExpiration != Cache.NoAbsoluteExpiration)
                            {
                                ltlExpiration.Text = container.AbsoluteExpiration.ToString();
                            }
                            else
                            {
                                ltlExpiration.Text = container.SlidingExpiration.ToString();
                            }

                            // Additional info
                            if (!RequestHelper.IsPostBack())
                            {
                                gridDependencies.PagerControl.UniPager.PageSize = 10;
                            }

                            if (container.Dependencies != null)
                            {
                                gridDependencies.AllItems = container.Dependencies.CacheKeys;
                                gridDependencies.ReloadData();
                            }

                            gridDependencies.Visible = gridDependencies.TotalItems > 0;
                            ltlDependencies.Visible = gridDependencies.TotalItems == 0;

                            pnlCacheItem.Visible = true;
                        }
                    }
                    else if (wasDeleted)
                    {
                        ShowError(GetString("general.wasdeleted"));
                    }
                    else
                    {
                        ShowError(GetString("general.objectnotfound"));
                    }
                }
                break;
        }

        objElem.Object = obj;
    }


    private void actionsElem_ActionPerformed(object sender, CommandEventArgs e)
    {
        switch (e.CommandName.ToLowerCSafe())
        {
            case "delete":
                // Delete the item from the cache
                if (!string.IsNullOrEmpty(key))
                {
                    CacheHelper.Remove(key);

                    ReloadData();
                    CurrentMaster.HeaderActions.Visible = false;

                    // Close the window
                    ScriptHelper.CloseWindow(Page, true);
                }
                break;
        }
    }

    #endregion
}