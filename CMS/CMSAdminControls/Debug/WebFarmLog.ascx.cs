using System;

using CMS.Helpers;
using CMS.UIControls;

public partial class CMSAdminControls_Debug_WebFarmLog : WebFarmLog
{
    #region "Variables"

    protected string cmsVersion = null;

    #endregion


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        Visible = false;

        var dt = GetLogData();
        if (dt != null)
        {
            Visible = true;

            cmsVersion = GetString("Footer.Version") + "&nbsp;" + CMS.Base.CMSVersion.GetVersion(true, true, true, true);

            gridQueries.Columns[1].HeaderText = GetString("WebFarmLog.TaskType");
            gridQueries.Columns[2].HeaderText = GetString("WebFarmLog.Target");
            gridQueries.Columns[3].HeaderText = GetString("WebFarmLog.TextData");
            gridQueries.Columns[4].HeaderText = GetString("General.Context");

            if (DisplayHeader)
            {
                ltlInfo.Text = "<div class=\"LogInfo\">" + GetString("WebFarmLog.Info") + "</div>";
            }

            gridQueries.DataSource = dt;
            gridQueries.DataBind();
        }
    }
}