using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.Helpers;
using CMS.PortalEngine;
using CMS.SiteProvider;
using CMS.Membership;
using CMS.UIControls;

public partial class CMSAdminControls_UI_Macros_Dialogs_Tab_InsertMacroTree : CMSModalPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        macroTree.ResolverName = QueryHelper.GetString("resolver", "");
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        if (PortalContext.ViewMode.IsLiveSite())
        {
            // Register custom css if exists
            RegisterDialogCSSLink();
            SetLiveDialogClass();
        }

        string script = @"
function InsertMacro(macro) {
    if((wopener!=null) && (wopener.CMSPlugin.insertHtml))
    {
        wopener.CMSPlugin.insertHtml('{% ' + macro + ' %}');
    }
    return CloseDialog();
}";

        ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "InsertMacroScript", script, true);
    }
}