using System;

using CMS.UIControls;

public partial class CMSAdminControls_Debug_HandlersLog : HandlersLog
{
    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        Visible = false;

        var dt = GetLogData();
        if (dt != null)
        {
            Visible = true;

            // Setup headers text
            gridStates.Columns[1].HeaderText = GetString("HandlersLog.Name");
            gridStates.Columns[2].HeaderText = GetString("General.Context");
            
            if (DisplayHeader)
            {
                ltlInfo.Text = "<div class=\"LogInfo\">" + GetString("HandlersLog.Info") + "</div>";
            }

            gridStates.DataSource = dt;
            gridStates.DataBind();
        }
    }
}