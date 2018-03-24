using System;

using CMS.Helpers;
using CMS.UIControls;
using CMS.MacroEngine;
using CMS.ExtendedControls;

public partial class CMSFormControls_Macros_ConditionBuilderDialog : DesignerPage
{
    #region "Constants"

    /// <summary>
    /// Short link to help page.
    /// </summary>
    private const string HELP_TOPIC_LINK = "R4E8";

    #endregion


    string clientId = null;

    protected void Page_Load(object sender, EventArgs e)
    {
        RegisterESCScript = false;

        clientId = QueryHelper.GetString("clientid", "");

        SetTitle(GetString("conditionbuilder.title"));
        PageTitle.HelpTopicName = HELP_TOPIC_LINK;

        Save += btnSave_Click;

        designerElem.RuleCategoryNames = QueryHelper.GetString("module", "");
        designerElem.DisplayRuleType = QueryHelper.GetInteger("ruletype", 0);
        designerElem.ShowGlobalRules = QueryHelper.GetBoolean("showglobal", true);

        // Set correct resolver to the control
        string resolverName = ValidationHelper.GetString(SessionHelper.GetValue("ConditionBuilderResolver" + clientId), "");
        if (!string.IsNullOrEmpty(resolverName))
        {
            designerElem.ResolverName = resolverName;
        }

        // Set correct default condition text
        string defaultText = ValidationHelper.GetString(SessionHelper.GetValue("ConditionBuilderDefaultText" + clientId), "");
        if (!string.IsNullOrEmpty(defaultText))
        {
            designerElem.DefaultConditionText = defaultText;
        }

        if (!RequestHelper.IsPostBack())
        {
            string condition = MacroProcessor.RemoveDataMacroBrackets(ValidationHelper.GetString(SessionHelper.GetValue("ConditionBuilderCondition" + clientId), ""));
            designerElem.Value = condition;
        }

        CurrentMaster.PanelContent.RemoveCssClass("dialog-content");
    }


    protected void btnSave_Click(object sender, EventArgs e)
    {
        // Clean-up the session
        SessionHelper.Remove("ConditionBuilderCondition" + clientId);
        SessionHelper.Remove("ConditionBuilderResolver" + clientId);
        SessionHelper.Remove("ConditionBuilderDefaultText" + clientId);

        try
        {
            string text = ValidationHelper.GetString(designerElem.Value, "");
            ltlScript.Text = ScriptHelper.GetScript("wopener.InsertMacroCondition" + clientId + "(" + ScriptHelper.GetString(text) + "); CloseDialog();");
        }
        catch { }
    }
}