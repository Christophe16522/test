using System;

using CMS.Helpers;
using CMS.UIControls;

public partial class CMSAdminControls_Debug_RequestLog : RequestProcessLog
{
    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        Visible = false;

        if (Log != null)
        {
            // Log request values
            RequestDebug.LogRequestValues(true, true, true);

            var dt = GetLogData();
            if (dt != null)
            {
                Visible = true;

                // Ensure value collections
                if (Log.ValueCollections != null)
                {
                    tblResC.Title = GetString("RequestLog.ResponseCookies");
                    tblResC.Table = Log.ValueCollections.Tables["ResponseCookies"];

                    tblReqC.Title = GetString("RequestLog.RequestCookies");
                    tblReqC.Table = Log.ValueCollections.Tables["RequestCookies"];

                    tblVal.Title = GetString("RequestLog.Values");
                    tblVal.Table = Log.ValueCollections.Tables["Values"];
                }

                // Ensure header texts
                gridCache.Columns[1].HeaderText = GetString("RequestLog.Operation");
                gridCache.Columns[2].HeaderText = GetString("RequestLog.Parameter");
                gridCache.Columns[3].HeaderText = GetString("RequestLog.FromStart");
                gridCache.Columns[4].HeaderText = GetString("RequestLog.Duration");

                if (DisplayHeader)
                {
                    ltlInfo.Text = "<div class=\"LogInfo\">" + GetString("RequestLog.Info") + "</div>";
                }

                // Bind the data
                gridCache.DataSource = dt;
                gridCache.DataBind();
            }
        }
    }
}