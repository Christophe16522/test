using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.Helpers;
using CMS.SiteProvider;
using CMS.UIControls;
using CMS.WebAnalytics;
using CMS.ExtendedControls;

public partial class CMSModules_WebAnalytics_Tools_Disabled : CMSWebAnalyticsPage
{
    #region "Properties"

    /// <summary>
    /// Messages placeholder
    /// </summary>
    public override MessagesPlaceHolder MessagesPlaceHolder
    {
        get
        {
            return plcMess;
        }
    }

    #endregion


    protected void Page_Load(object sender, EventArgs e)
    {
        if (AnalyticsHelper.AnalyticsEnabled(SiteContext.CurrentSiteName))
        {
            URLHelper.Redirect("default.aspx");
        }
        else
        {
            ShowWarning(GetString("WebAnalytics.Disabled"), null, null);
        }
    }
}