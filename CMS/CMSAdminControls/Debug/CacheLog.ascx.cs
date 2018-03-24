using System;

using CMS.UIControls;

public partial class CMSAdminControls_Debug_CacheLog : CacheLog
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
            gridCache.Columns[1].HeaderText = GetString("CacheLog.Operation");
            gridCache.Columns[2].HeaderText = GetString("CacheLog.Data");
            gridCache.Columns[3].HeaderText = GetString("General.Context");

            if (DisplayHeader)
            {
                ltlInfo.Text = "<div class=\"LogInfo\">" + GetString("CacheLog.Info") + "</div>";
            }

            gridCache.DataSource = dt;
            gridCache.DataBind();
        }
    }
}