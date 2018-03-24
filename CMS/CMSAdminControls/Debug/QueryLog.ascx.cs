using System;

using CMS.Helpers;
using CMS.UIControls;

public partial class CMSAdminControls_Debug_QueryLog : QueryLog
{
    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        Visible = false;

        var dt = GetLogData();
        if (dt != null)
        {
            Visible = true;

            // Setup header texts
            gridQueries.Columns[1].HeaderText = GetString("QueryLog.QueryText");
            gridQueries.Columns[2].HeaderText = GetString("General.Context");
            gridQueries.Columns[3].HeaderText = GetString("QueryLog.QueryDuration");

            if (DisplayHeader)
            {
                ltlInfo.Text = "<div class=\"LogInfo\">" + GetString("QueryLog.Info") + "</div>";
            }

            // Override maximum size with parameters if larger
            int paramSize = DataHelper.GetMaximumValue<int>(dt, "QueryParametersSize");
            if (paramSize > MaxSize)
            {
                MaxSize = paramSize;
            }

            gridQueries.DataSource = dt;
            gridQueries.DataBind();
        }
    }
}