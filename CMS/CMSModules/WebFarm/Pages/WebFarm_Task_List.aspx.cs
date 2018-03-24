using System;
using System.Web.UI.WebControls;
using System.Data;

using CMS.DataEngine;
using CMS.ExtendedControls;
using CMS.ExtendedControls.ActionsConfig;
using CMS.Helpers;
using CMS.Base;
using CMS.UIControls;
using CMS.WebFarmSync;
using CMS.FormEngine;


public partial class CMSModules_WebFarm_Pages_WebFarm_Task_List : GlobalAdminPage
{
    private const string allServers = "##ALL##";
    private string selectedServer = allServers;
    private HeaderAction runAction;
    private HeaderAction clearAction;


    /// <summary>
    /// Messages placeholder
    /// </summary>
    public override MessagesPlaceHolder MessagesPlaceHolder
    {
        get
        {
            return plcMess;
        }
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        UniGrid.OnAction += new OnActionEventHandler(uniGrid_OnAction);
        UniGrid.ZeroRowsText = GetString("WebFarmTasks_List.ZeroRows");
        UniGrid.GridView.DataBound += new EventHandler(GridView_DataBound);
        UniGrid.GridView.RowDataBound += GridView_RowDataBound;

        uniSelector.SpecialFields.Add(new SpecialField() { Text = GetString("WebFarmList.All"), Value = allServers });
        uniSelector.DropDownSingleSelect.AutoPostBack = true;

        if (RequestHelper.IsPostBack())
        {
            selectedServer = ValidationHelper.GetString(uniSelector.Value, allServers);
        }

        if (selectedServer != allServers)
        {
            UniGrid.WhereCondition = "ServerName = '" + SqlHelper.GetSafeQueryString(selectedServer, false) + "'";
        }

        // Prepare header actions
        runAction = new HeaderAction()
        {
            ButtonStyle = ButtonStyle.Primary,
            Text = GetString("WebFarmTasks_List.RunButton"),
            CommandName = "run"
        };
        clearAction = new HeaderAction()
        {
            ButtonStyle = ButtonStyle.Default,
            Text = GetString("WebFarmTasks_List.EmptyButton"),
            CommandName = "clear"
        };
        HeaderActions.AddAction(runAction);
        HeaderActions.AddAction(clearAction);
        HeaderActions.ActionPerformed += HeaderActions_ActionPerformed;
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        UniGrid.GridView.Columns[1].Visible = (selectedServer == allServers);
    }


    protected void GridView_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            string code = ValidationHelper.GetString(((DataRowView)(e.Row.DataItem)).Row["ErrorMessage"], string.Empty);
            if (!String.IsNullOrEmpty(code))
            {
                e.Row.Style.Add("background-color", "#FFDDDD");
            }
        }
    }


    protected void GridView_DataBound(object sender, EventArgs e)
    {
        runAction.Visible = clearAction.Visible = !DataHelper.DataSourceIsEmpty(UniGrid.GridView.DataSource);
    }


    /// <summary>
    /// Handles the UniGrid's OnAction event.
    /// </summary>
    /// <param name="actionName">Name of item (button) that throws event</param>
    /// <param name="actionArgument">ID (value of Primary key) of corresponding data row</param>
    protected void uniGrid_OnAction(string actionName, object actionArgument)
    {
        if (actionName == "delete")
        {
            // Delete object from database
            if (selectedServer == allServers)
            {
                // Delete task object
                WebFarmTaskInfoProvider.DeleteWebFarmTaskInfo(Convert.ToInt32(actionArgument));
            }
            else
            {
                // Get infos for task and server
                WebFarmTaskInfo wfti = WebFarmTaskInfoProvider.GetWebFarmTaskInfo(Convert.ToInt32(actionArgument));
                WebFarmServerInfo wfsi = WebFarmServerInfoProvider.GetWebFarmServerInfo(selectedServer);
                // Delete task binding to server
                WebFarmTaskInfoProvider.DeleteServerTask(wfsi.ServerID, wfti.TaskID);
            }

            UniGrid.ReloadData();
        }
    }


    /// <summary>
    /// Performs header actions.
    /// </summary>
    protected void HeaderActions_ActionPerformed(object sender, CommandEventArgs e)
    {
        switch (e.CommandName.ToLowerCSafe())
        {
            case "run":
                RunTasks();
                break;
            case "clear":
                EmptyTasks();
                break;
        }
    }


    /// <summary>
    /// Clear task list.
    /// </summary>
    private void EmptyTasks()
    {
        // Delete all task for specified server (or all servers)
        switch (selectedServer)
        {
            case allServers:
                // delete all task objects
                WebFarmTaskInfoProvider.DeleteAllTaskInfo();
                break;
            default:
                // delete bindings to specified server
                WebFarmTaskInfoProvider.DeleteAllTaskInfo(SqlHelper.GetSafeQueryString(selectedServer, false));
                break;
        }

        UniGrid.ReloadData();
    }


    /// <summary>
    /// Run task list.
    /// </summary>
    private void RunTasks()
    {
        switch (selectedServer)
        {
            case allServers:
                {
                    WebSyncHelper.NotifyServers(true);
                    WebSyncHelper.ProcessMyTasks();
                }
                break;

            default:
                // Get the server info object
                WebFarmServerInfo wfsi = WebFarmServerInfoProvider.GetWebFarmServerInfo(SqlHelper.GetSafeQueryString(selectedServer, false));
                // If server is enabled
                if (wfsi.ServerEnabled)
                {
                    if (wfsi.ServerName.ToLowerCSafe() == WebSyncHelper.ServerName.ToLowerCSafe())
                    {
                        // Call synchronization method
                        WebSyncHelper.ProcessMyTasks();
                    }
                    else if (WebSyncHelper.IsServerEnabled(wfsi.ServerID))
                    {
                        WebFarmUpdaterAsync wfu = new WebFarmUpdaterAsync();

                        // Add server for sync
                        wfu.Urls.Add(wfsi.ServerURL.TrimEnd('/') + WebSyncHelper.WebFarmUpdaterPage);
                    }
                }
                break;
        }

        UniGrid.ReloadData();

        // Show info label
        ShowInformation(GetString("webfarmtasks.taskexecuted"));
    }
}