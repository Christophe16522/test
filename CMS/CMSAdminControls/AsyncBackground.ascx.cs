using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.Helpers;
using CMS.UIControls;

public partial class CMSAdminControls_AsyncBackground : CMSUserControl
{
    #region "Page events"

    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        // Register dialog script
        ScriptHelper.RegisterJQueryDialog(Page);
        string resizeScript = "showModalBackground('" + pnlAsyncBackground.ClientID + "');";
        ScriptHelper.RegisterStartupScript(this, typeof(string), "asyncBackground" + ClientID, ScriptHelper.GetScript(resizeScript));
    }

    #endregion
}