using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.Forums;
using CMS.GlobalHelper;
using CMS.Helpers;

public partial class CMSModules_Forums_Controls_Layouts_Flat_Attachments : ForumViewer
{
    protected void Page_Load(object sender, EventArgs e)
    {
        int moderated = QueryHelper.GetInteger("moderated", 0);
        if (moderated != 0)
        {
            plcModerationRequired.Visible = true;
            lblModerationInfo.Text = GetString("forums.requiresmoderationafteraction");
        }
    }
}