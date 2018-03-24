using System;
using System.Text;

using CMS.UIControls;

public partial class CMSAdminControls_Debug_AnalyticsLog : AnalyticsLog
{
    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        Visible = false;

        var dt = GetLogData();
        if (Log != null)
        {
            // Update user agent and IP
            gridAnalytics.RowDataBound += GetIPAndAgent;

            Visible = true;

            // Setup header texts
            gridAnalytics.Columns[1].HeaderText = GetString("General.CodeName");
            gridAnalytics.Columns[2].HeaderText = GetString("General.Object");
            gridAnalytics.Columns[3].HeaderText = GetString("General.Count");
            gridAnalytics.Columns[4].HeaderText = GetString("General.SiteName");
            gridAnalytics.Columns[5].HeaderText = GetString("General.Context");

            if (DisplayHeader)
            {
                ltlInfo.Text = "<div class=\"LogInfo\">" + GetString("AnalyticsLog.Info") + "</div>";
            }

            gridAnalytics.DataSource = dt;
            gridAnalytics.DataBind();
        }
    }
}