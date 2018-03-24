using System;
using System.Data;

using CMS.EventLog;
using CMS.Helpers;
using CMS.UIControls;

public partial class CMSAdminControls_Debug_ViewState : ViewStateLog
{
    protected void Page_Load(object sender, EventArgs e)
    {
        EnableViewState = false;
        Visible = true;
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        Visible = false;

        try
        {
            if (Log != null)
            {
                // Get the log table
                DataTable dt = Log.LogTable;
                DataView dv = new DataView(dt);

                if (!DataHelper.DataSourceIsEmpty(dv))
                {
                    Visible = true;

                    gridStates.Columns[1].HeaderText = GetString("ViewStateLog.ID");
                    gridStates.Columns[2].HeaderText = GetString("ViewStateLog.IsDirty");
                    gridStates.Columns[3].HeaderText = GetString("ViewStateLog.ViewState");
                    gridStates.Columns[4].HeaderText = GetString("ViewStateLog.Size");

                    if (DisplayHeader)
                    {
                        ltlInfo.Text = "<div class=\"LogInfo\">" + GetString("ViewStateLog.Info") + "</div>";
                    }

                    // Bind to the grid
                    if (DisplayOnlyDirty)
                    {
                        dv.RowFilter = "HasDirty = 1";
                    }

                    MaxSize = DataHelper.GetMaximumValue<int>(dv, "ViewStateSize");

                    gridStates.DataSource = dv;
                    gridStates.DataBind();
                }
            }
        }
        catch (Exception ex)
        {
            ltlInfo.Text = "Unable to acquire ViewState from the controls collection: " + ex.Message;
            Visible = true;

            EventLogProvider.LogException("Debug", "GETVIEWSTATE", ex);
        }
    }
}