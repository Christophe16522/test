using System;

using CMS.Helpers;
using CMS.UIControls;

public partial class CMSAdminControls_Debug_AllLog : AllLog
{
    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        Visible = false;

        if (!StopProcessing)
        {
            var dt = MergeLogs(Logs, Page, ShowCompleteContext);

            if (!DataHelper.DataSourceIsEmpty(dt))
            {
                Visible = true;

                // Setup header texts
                gridDebug.Columns[1].HeaderText = GetString("AllLog.DebugType");
                gridDebug.Columns[2].HeaderText = GetString("AllLog.Information");
                gridDebug.Columns[3].HeaderText = GetString("AllLog.Result");
                gridDebug.Columns[4].HeaderText = GetString("General.Context");
                gridDebug.Columns[5].HeaderText = GetString("AllLog.TotalDuration");
                gridDebug.Columns[6].HeaderText = GetString("AllLog.Duration");

                gridDebug.DataSource = dt;
                gridDebug.DataBind();
            }
        }
    }
}