using System;
using System.Web.UI.WebControls;
using System.Collections.Generic;

using CMS.DataEngine;
using CMS.Helpers;
using CMS.SiteProvider;
using CMS.Membership;
using CMS.UIControls;
using CMS.TranslationServices;
using CMS.ExtendedControls;
using CMS.ExtendedControls.ActionsConfig;


// Edited object
[EditedObject(TranslationSubmissionInfo.OBJECT_TYPE, "submissionId")]

// Breadcrumbs
[Breadcrumbs]
[Breadcrumb(0, "translationservice.translationsubmission.list", "~/CMSModules/Translations/Pages/Tools/TranslationSubmission/List.aspx", null)]
[Breadcrumb(1, Text = "{%EditedObject.DisplayName%}", ExistingObject = true)]
[Breadcrumb(1, ResourceString = "translationservice.translationsubmission.new", NewObject = true)]

// Title
[Title(ResourceString = "translationservice.translationsubmission.edit", HelpTopic = "translationservices_translationsubmission_edit", ExistingObject = true)]
[Title(ResourceString = "translationservice.translationsubmission.new", HelpTopic = "translationservices_translationsubmission_edit", NewObject = true)]

public partial class CMSModules_Translations_Pages_Tools_TranslationSubmission_Edit : CMSTranslationServicePage
{
    #region "Variables"

    TranslationSubmissionInfo mSubmissionInfo;

    #endregion


    #region "Properties"

    /// <summary>
    /// Returns submission info being edited.
    /// </summary>
    public TranslationSubmissionInfo SubmissionInfo
    {
        get
        {
            return mSubmissionInfo ?? (mSubmissionInfo = (TranslationSubmissionInfo)editElem.UIFormControl.EditedObject);
        }
    }

    #endregion


    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        btnImportTranslations.Click += btnImportTranslations_Click;
        btnShowMessage.Click += btnShowMessage_Click;
        CurrentMaster.HeaderActions.ActionPerformed += HeaderActions_ActionPerformed;
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        if (SubmissionInfo == null)
        {
            return;
        }

        bool allowed = CheckModifyPermissions(false);

        HeaderAction processAction = new HeaderAction
        {
            OnClientClick = "if (confirm(" + ScriptHelper.GetString(GetString("translationservice.confirmprocesstranslations")) + ")) { " + ControlsHelper.GetPostBackEventReference(btnImportTranslations) + " }",
            Tooltip = GetString("translationservice.importtranslationstooltip"),
            Text = GetString("translationservice.importtranslations"),
            Enabled = allowed && ((SubmissionInfo.SubmissionStatus == TranslationStatusEnum.TranslationReady) || (SubmissionInfo.SubmissionStatus == TranslationStatusEnum.TranslationCompleted) || (SubmissionInfo.SubmissionStatus == TranslationStatusEnum.ProcessingError))
        };

        HeaderAction resubmitAction = new HeaderAction
        {
            CommandName = "resubmit",
            Tooltip = GetString("translationservice.resubmittooltip"),
            Text = GetString("translationservice.resubmit"),
            Enabled = allowed && ((SubmissionInfo.SubmissionStatus == TranslationStatusEnum.WaitingForTranslation) || (SubmissionInfo.SubmissionStatus == TranslationStatusEnum.SubmissionError))
        };

        HeaderAction updateAction = new HeaderAction
        {
            CommandName = "update",
            Tooltip = GetString("translationservice.updateandresubmittooltip"),
            Text = GetString("translationservice.updateandresubmit"),
            Enabled = allowed && ((SubmissionInfo.SubmissionStatus == TranslationStatusEnum.WaitingForTranslation) || (SubmissionInfo.SubmissionStatus == TranslationStatusEnum.SubmissionError))
        };
        
        HeaderAction saveAction = new SaveAction(Page);
        saveAction.Enabled = allowed;

        List<HeaderAction> actions = CurrentMaster.HeaderActions.ActionsList;
        actions.Add(saveAction);
        actions.Add(updateAction);
        actions.Add(resubmitAction);
        actions.Add(processAction);

        // Check if current service supports canceling
        TranslationServiceInfo service = TranslationServiceInfoProvider.GetTranslationServiceInfo(SubmissionInfo.SubmissionServiceID);
        if (service != null)
        {
            bool serviceSupportsCancel = service.TranslationServiceSupportsCancel;

            HeaderAction cancelAction = new HeaderAction
            {
                CommandName = "cancel",
                Tooltip = serviceSupportsCancel ? GetString("translationservice.cancelsubmissiontooltip") : String.Format(GetString("translationservice.cancelnotsupported"), service.TranslationServiceDisplayName),
                Text = GetString("translationservice.cancelsubmission"),
                Enabled = allowed && (SubmissionInfo.SubmissionStatus == TranslationStatusEnum.WaitingForTranslation) && serviceSupportsCancel
            };

            actions.Add(cancelAction);
        }

        CurrentMaster.HeaderActions.ReloadData();

        ScriptHelper.RegisterClientScriptBlock(Page, typeof(string), "ShowUploadSuccess", "function ShowUploadSuccess() { " + ControlsHelper.GetPostBackEventReference(btnShowMessage) + " }", true);
    }


    protected void HeaderActions_ActionPerformed(object sender, CommandEventArgs e)
    {
        CheckModifyPermissions(true);

        switch (e.CommandName)
        {
            case ComponentEvents.SAVE:
                if (!editElem.UIFormControl.SaveData(null))
                {
                    ShowError(GetString("translationservice.savesubmissionfailed"));
                }
                break;

            case "resubmit":
                string errResubmit = TranslationServiceHelper.ResubmitSubmission(SubmissionInfo);
                if (string.IsNullOrEmpty(errResubmit))
                {
                    ShowInformation(GetString("translationservice.translationresubmitted"));
                }
                else
                {
                    ShowError(errResubmit);
                }
                editElem.UIFormControl.ReloadData();
                break;

            case "update":
                if (editElem.UIFormControl.SaveData(null))
                {
                    string errUpdate = TranslationServiceHelper.ResubmitSubmission(SubmissionInfo);
                    if (string.IsNullOrEmpty(errUpdate))
                    {
                        ShowInformation(GetString("translationservice.translationupdatedandresubmitted"));
                    }
                    else
                    {
                        ShowError(errUpdate);
                    }
                }
                editElem.UIFormControl.ReloadData();
                break;

            case "cancel":
                string errCancel = TranslationServiceHelper.CancelSubmission(SubmissionInfo);
                if (string.IsNullOrEmpty(errCancel))
                {
                    ShowInformation(GetString("translationservice.submissioncanceled"));
                }
                else
                {
                    ShowError(errCancel);
                }
                editElem.UIFormControl.ReloadData();
                break;
        }
    }


    /// <summary>
    /// Checks Modify permissions for given translation submission.
    /// </summary>
    /// <param name="redirect">If true, redirects user to Access denied</param>
    private bool CheckModifyPermissions(bool redirect)
    {
        TranslationSubmissionInfo submission = EditedObject as TranslationSubmissionInfo;
        if (submission != null)
        {
            if (!MembershipContext.AuthenticatedUser.IsAuthorizedPerObject(PermissionsEnum.Modify, submission, SiteInfoProvider.GetSiteName(submission.SubmissionSiteID)))
            {
                if (redirect)
                {
                    RedirectToAccessDenied("CMS.TranslationServices", "Modify");
                }
                return false;
            }
        }
        return true;
    }


    protected void btnShowMessage_Click(object sender, EventArgs e)
    {
        ShowInformation(GetString("translationservice.translationuploadedsuccessfully"));

        // Reload is required because status of the submission could have been changed and form has to be reloaded
        editElem.UIFormControl.ReloadData();
    }

    protected void btnImportTranslations_Click(object sender, EventArgs e)
    {
        string err = TranslationServiceHelper.ProcessSubmission(SubmissionInfo);
        if (string.IsNullOrEmpty(err))
        {
            ShowInformation(GetString("translationservice.translationsimported"));
        }
        else
        {
            ShowError(err);
        }
        editElem.UIFormControl.ReloadData();
    }

    #endregion
}
