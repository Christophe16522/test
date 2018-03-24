using System;
using System.Linq;

using CMS.Helpers;
using CMS.Membership;
using CMS.SiteProvider;
using CMS.UIControls;
using CMS.TranslationServices;
using CMS.ExtendedControls.ActionsConfig;
using CMS.ExtendedControls;
using CMS.DataEngine;

// Title
[Title("translationservice.translationsubmission.list")]
public partial class CMSModules_Translations_Pages_Tools_TranslationSubmission_List : CMSTranslationServicePage
{
    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        btnUpdateStatuses.Click += btnUpdateStatuses_Click;
    }


    protected void btnUpdateStatuses_Click(object sender, EventArgs e)
    {
        if (!MembershipContext.AuthenticatedUser.IsAuthorizedPerResource("CMS.TranslationServices", "Modify"))
        {
            RedirectToAccessDenied("CMS.TranslationServices", "Modify");
        }

        string err = TranslationServiceHelper.CheckAndDownloadTranslations(SiteContext.CurrentSiteName);
        if (!string.IsNullOrEmpty(err))
        {
            ShowError(err);
        }

        listElem.Grid.ReloadData();
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        ScriptHelper.RegisterDialogScript(Page);

        HeaderAction updateAction = new HeaderAction
        {
            OnClientClick = ControlsHelper.GetPostBackEventReference(btnUpdateStatuses),
            Tooltip = GetString("translationservice.updatestatusestooltip"),
            Text = GetString("translationservice.updatestatuses"),
            Enabled = MembershipContext.AuthenticatedUser.IsAuthorizedPerResource("CMS.TranslationServices", "Modify") && !listElem.Grid.IsEmpty
        };

        string translateUrl = AuthenticationHelper.ResolveDialogUrl("~/CMSModules/Translations/Pages/TranslateDocuments.aspx") + "?select=1&dialog=1";
        translateUrl = URLHelper.AddParameterToUrl(translateUrl, "hash", QueryHelper.GetHash(URLHelper.GetQuery(translateUrl)));

        // Check if any human translation is enabled
        bool enabled = TranslationServiceInfoProvider.GetTranslationServices("(TranslationServiceEnabled = 1) AND (TranslationServiceIsMachine = 0)", null, 0, "TranslationServiceID, TranslationServiceName").Any(t => TranslationServiceHelper.IsServiceAvailable(t.TranslationServiceName, SiteContext.CurrentSiteName));

        HeaderAction submitAction = new HeaderAction
        {
            OnClientClick = "modalDialog('" + translateUrl + "', 'SubmitTranslation', 988, 634);",
            Tooltip = GetString(enabled ? "translationservice.submittranslationtooltip" : "translationservice.noenabledservices"),
            Text = GetString("translationservice.submittranslation"),
            Enabled = enabled && MembershipContext.AuthenticatedUser.IsAuthorizedPerResource("CMS.Content", "SubmitForTranslation")
        };

        AddHeaderAction(submitAction);
        AddHeaderAction(updateAction);

        CurrentMaster.HeaderActions.ReloadData();

        if (!listElem.Grid.IsEmpty)
        {
            string statusCheck = SettingsKeyInfoProvider.GetStringValue("CMSTranslationsLastStatusCheck");
            if (string.IsNullOrEmpty(statusCheck))
            {
                statusCheck = GetString("general.notavailable");
            }

            ShowInformation(string.Format(GetString("translationservice.laststatuscheck"), statusCheck));
        }
    }

    #endregion
}
