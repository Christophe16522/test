using System;

using CMS.Base;
using CMS.Helpers;
using CMS.PortalEngine;
using CMS.UIControls;

public partial class CMSAdminControls_CodeMirror_dialogs_InsertMacro_Tab_InsertMacroTree : CMSModalPage
{
    private string mEditorName = null;
    private bool mIsMixedMode = true;

    protected void Page_Load(object sender, EventArgs e)
    {
        macroTree.ResolverName = QueryHelper.GetString("resolver", "");
        mEditorName = QueryHelper.GetString("editorname", "");
        mIsMixedMode = QueryHelper.GetBoolean("ismixedmode", true);

        PageTitle.TitleText = GetString("insertmacro.dialogtitle");
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
    var mixedmode = " + mIsMixedMode.ToString().ToLowerCSafe() + @";
    if (wopener != null)
    {
        if (mixedmode) { macro = '{% ' + macro + ' %}' };
        var editor = wopener[" + ScriptHelper.GetString(mEditorName) + @"];
        if (editor != null) {
            editor.replaceSelection(macro);
            editor.focus();
        }
    }
    return CloseDialog();
}";

        ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "InsertMacroScript", script, true);
    }
}