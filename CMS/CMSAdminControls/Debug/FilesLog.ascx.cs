using System;
using System.Data;

using CMS.Helpers;
using CMS.IO;
using CMS.UIControls;

public partial class CMSAdminControls_Debug_FilesLog : FilesLog
{
    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        Visible = false;

        var dt = GetLogData();
        if (dt != null)
        {
            DataView dv;
            var providerSet = !String.IsNullOrEmpty(ProviderName);

            lock (dt)
            {
                dv = new DataView(dt);

                if (providerSet)
                {
                    dv.RowFilter = "ProviderName = '" + ProviderName + "'";
                }
            }

            if (!DataHelper.DataSourceIsEmpty(dv))
            {
                Visible = true;

                // Setup headers text
                gridStates.Columns[1].HeaderText = GetString("FilesLog.Operation");
                gridStates.Columns[2].HeaderText = GetString("FilesLog.FilePath");
                gridStates.Columns[3].HeaderText = GetString("FilesLog.OperationType");
                gridStates.Columns[4].HeaderText = GetString("General.Context");

                // Hide the operation type column if only specific operation type is selected
                if (providerSet)
                {
                    gridStates.Columns[3].Visible = false;
                }

                if (DisplayHeader)
                {
                    ltlInfo.Text = "<div class=\"LogInfo\">" + GetString("FilesLog.Info") + "</div>";
                }

                gridStates.DataSource = dv;
                gridStates.DataBind();
            }
        }
    }
}