using System;

using CMS.Helpers;
using CMS.UIControls;

public partial class CMSAdminControls_Debug_MacroLog : MacroLog
{
    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        Visible = false;

        var dt = GetLogData();
        if (dt != null)
        {
            ScriptHelper.RegisterTooltip(Page);

            Visible = true;

            // Setup header texts
            gridMacros.Columns[1].HeaderText = GetString("MacroLog.Expression");
            gridMacros.Columns[2].HeaderText = GetString("MacroLog.Result");
            gridMacros.Columns[3].HeaderText = GetString("General.User");
            gridMacros.Columns[4].HeaderText = GetString("General.Context");
            gridMacros.Columns[5].HeaderText = GetString("MacroLog.Duration");

            if (DisplayHeader)
            {
                ltlInfo.Text = "<div class=\"LogInfo\">" + GetString("MacroLog.Info") + "</div>";
            }

            gridMacros.DataSource = dt;
            gridMacros.DataBind();
        }
    }
}