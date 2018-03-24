using System;

using CMS.Helpers;
using CMS.UIControls;

public partial class CMSAdminControls_Debug_SecurityLog : SecurityLog
{
    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        Visible = false;

        var dt = GetLogData();
        if (dt != null)
        {
            Visible = true;

            // Setup header columns
            gridSec.Columns[1].HeaderText = GetString("SecurityLog.UserName");
            gridSec.Columns[2].HeaderText = GetString("SecurityLog.Operation");
            gridSec.Columns[3].HeaderText = GetString("SecurityLog.Result");
            gridSec.Columns[4].HeaderText = GetString("SecurityLog.Resource");
            gridSec.Columns[5].HeaderText = GetString("SecurityLog.Name");
            gridSec.Columns[6].HeaderText = GetString("SecurityLog.Site");
            gridSec.Columns[7].HeaderText = GetString("General.Context");

            if (DisplayHeader)
            {
                ltlInfo.Text = "<div class=\"LogInfo\">" + GetString("SecurityLog.Info") + "</div>";
            }

            gridSec.DataSource = dt;
            gridSec.DataBind();
        }
    }
}