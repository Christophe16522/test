using System;
using System.Data;
using System.Threading;
using System.Collections;
using System.Security.Principal;
using System.Collections.Generic;

using CMS.Base;
using CMS.Helpers;
using CMS.Localization;
using CMS.SiteProvider;
using CMS.Membership;
using CMS.UIControls;
using CMS.DocumentEngine;
using CMS.EventLog;
using CMS.DataEngine;
using CMS.TranslationServices;
using CMS.ExtendedControls;

public partial class CMSModules_Translations_Pages_TranslateDocuments : CMSTranslationServiceModalPage
{
    #region "Private variables"

    private string targetCulture;
    private bool isSelect;
    private readonly List<int> nodeIds = new List<int>();
    private string[] nodeIdsArr;
    private int cancelNodeId;
    private CurrentUserInfo currentUser;
    private SiteInfo currentSite;

    private static readonly Hashtable mErrors = new Hashtable();
    private Hashtable mParameters;

    private string currentCulture = CultureHelper.DefaultUICultureCode;

    #endregion


    #region "Properties"

    /// <summary>
    /// Returns messages placeholder.
    /// </summary>
    public override MessagesPlaceHolder MessagesPlaceHolder
    {
        get
        {
            return plcMessages;
        }
    }


    /// <summary>
    /// Current log context.
    /// </summary>
    public LogContext CurrentLog
    {
        get
        {
            return EnsureLog();
        }
    }


    /// <summary>
    /// Current Error.
    /// </summary>
    private string CurrentError
    {
        get
        {
            return ValidationHelper.GetString(mErrors["TranslateError_" + ctlAsync.ProcessGUID], string.Empty);
        }
        set
        {
            mErrors["TranslateError_" + ctlAsync.ProcessGUID] = value;
        }
    }


    /// <summary>
    /// Indicates whether action is multiple.
    /// </summary>
    private bool AllLevels
    {
        get
        {
            return QueryHelper.GetBoolean("alllevels", false);
        }
    }


    /// <summary>
    /// Gets selected class ID.
    /// </summary>
    private int ClassID
    {
        get
        {
            return QueryHelper.GetInteger("classid", 0);
        }
    }


    /// <summary>
    /// Where condition used for multiple actions.
    /// </summary>
    private string WhereCondition
    {
        get
        {
            string where = string.Empty;
            if (Parameters != null)
            {
                where = ValidationHelper.GetString(Parameters["where"], string.Empty);
            }
            return where;
        }
    }


    /// <summary>
    /// Hashtable containing dialog parameters.
    /// </summary>
    private Hashtable Parameters
    {
        get
        {
            if (mParameters == null)
            {
                string identifier = QueryHelper.GetString("params", null);
                mParameters = (Hashtable)WindowHelper.GetItem(identifier);
            }
            return mParameters;
        }
    }

    #endregion


    #region "Page events"

    protected override void OnPreInit(EventArgs e)
    {
        base.OnPreInit(e);

        if (IsDialog)
        {
            MasterPageFile = "~/CMSMasterPages/UI/Dialogs/ModalSimplePage.master";
        }
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        // Register script files
        ScriptHelper.RegisterCMS(this);
        ScriptHelper.RegisterScriptFile(this, "~/CMSModules/Content/CMSDesk/Operation.js");

        if (QueryHelper.ValidateHash("hash"))
        {
            // Setup page title text and image
            PageTitle.TitleText = GetString("Content.TranslateTitle");
            EnsureDocumentBreadcrumbs(PageBreadcrumbs, action: PageTitle.TitleText);

            if (IsDialog)
            {
                RegisterModalPageScripts();
                RegisterEscScript();

                plcInfo.Visible = false;

                pnlAsyncBody.CssClass = "DialogPageBody";
                pnlTitleAsync.CssClass = "DialogsPageHeader";
                pnlContent.CssClass = "PageContent";
                pnlButtons.CssClass = "FloatRight PageFooterLine";
            }

            if (!TranslationServiceHelper.IsTranslationAllowed(CurrentSiteName))
            {
                pnlContent.Visible = false;
                ShowError(GetString("translations.translationnotallowed"));
                return;
            }

            // Set current UI culture
            currentCulture = CultureHelper.PreferredUICultureCode;
            // Initialize current user
            currentUser = MembershipContext.AuthenticatedUser;
            // Initialize current site
            currentSite = SiteContext.CurrentSite;

            // Initialize events
            ctlAsync.OnFinished += ctlAsync_OnFinished;
            ctlAsync.OnError += ctlAsync_OnError;
            ctlAsync.OnRequestLog += ctlAsync_OnRequestLog;
            ctlAsync.OnCancel += ctlAsync_OnCancel;

            targetCulture = QueryHelper.GetString("targetculture", LocalizationContext.PreferredCultureCode);
            isSelect = QueryHelper.GetBoolean("select", false);

            if (isSelect)
            {
                pnlDocList.Visible = false;
                pnlDocSelector.Visible = true;
                translationElem.DisplayMachineServices = false;
                translationElem.DisplayTargetlanguage = true;
            }

            if (!RequestHelper.IsCallback())
            {
                // If not in select mode, load all the document IDs and check permissions
                // In select mode, documents are checked when the button is clicked
                if (!isSelect)
                {
                    DataSet allDocs = null;
                    TreeProvider tree = new TreeProvider(currentUser);

                    // Current Node ID to translate
                    string parentAliasPath = string.Empty;
                    if (Parameters != null)
                    {
                        parentAliasPath = ValidationHelper.GetString(Parameters["parentaliaspath"], string.Empty);
                        nodeIdsArr = ValidationHelper.GetString(Parameters["nodeids"], string.Empty).Trim('|').Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                    }

                    if (string.IsNullOrEmpty(parentAliasPath))
                    {
                        if (nodeIdsArr == null)
                        {
                            // One document translation is requested
                            string nodeIdQuery = QueryHelper.GetString("nodeid", "");
                            if (nodeIdQuery != "")
                            {
                                // Mode of single node translation
                                pnlList.Visible = false;
                                chkSkipTranslated.Checked = false;

                                translationElem.NodeID = ValidationHelper.GetInteger(nodeIdQuery, 0);

                                nodeIdsArr = new[] { nodeIdQuery };
                            }
                            else
                            {
                                nodeIdsArr = new string[] { };
                            }
                        }

                        foreach (string nodeId in nodeIdsArr)
                        {
                            int id = ValidationHelper.GetInteger(nodeId, 0);
                            if (id != 0)
                            {
                                nodeIds.Add(id);
                            }
                        }
                    }
                    else
                    {
                        // Exclude root of the website from multiple translation requested by document listing bulk action
                        string where = "ClassName <> 'CMS.Root'";
                        if (!string.IsNullOrEmpty(WhereCondition))
                        {
                            where = SqlHelper.AddWhereCondition(where, WhereCondition);
                        }

                        allDocs = tree.SelectNodes(currentSite.SiteName, parentAliasPath.TrimEnd(new[] { '/' }) + "/%",
                                                   TreeProvider.ALL_CULTURES, true, ClassID > 0 ? DataClassInfoProvider.GetClassName(ClassID) : TreeProvider.ALL_CLASSNAMES, where,
                                                   "DocumentName", AllLevels ? TreeProvider.ALL_LEVELS : 1, false, 0,
                                                   TreeProvider.SELECTNODES_REQUIRED_COLUMNS + ",DocumentName,NodeParentID,NodeSiteID,NodeAliasPath");

                        if (!DataHelper.DataSourceIsEmpty(allDocs))
                        {
                            foreach (DataTable table in allDocs.Tables)
                            {
                                foreach (DataRow row in table.Rows)
                                {
                                    nodeIds.Add(ValidationHelper.GetInteger(row["NodeID"], 0));
                                }
                            }
                        }
                    }

                    if (nodeIds.Count > 0)
                    {
                        // Set the target settings
                        translationElem.TranslationSettings = new TranslationSettings
                            {
                                TargetLanguage = targetCulture
                            };

                        var where = new WhereCondition().WhereIn("NodeID", nodeIds).ToString(true);
                        DataSet ds = allDocs ?? tree.SelectNodes(currentSite.SiteName, "/%", TreeProvider.ALL_CULTURES, true, null, where, "DocumentName", TreeProvider.ALL_LEVELS, false);

                        if (!DataHelper.DataSourceIsEmpty(ds))
                        {
                            string docList = null;

                            if (string.IsNullOrEmpty(parentAliasPath))
                            {
                                cancelNodeId = ValidationHelper.GetInteger(DataHelper.GetDataRowValue(ds.Tables[0].Rows[0], "NodeParentID"), 0);
                            }
                            else
                            {
                                cancelNodeId = TreePathUtils.GetNodeIdByAliasPath(currentSite.SiteName, parentAliasPath);
                            }

                            foreach (DataTable table in ds.Tables)
                            {
                                foreach (DataRow dr in table.Rows)
                                {
                                    bool isLink = (dr["NodeLinkedNodeID"] != DBNull.Value);
                                    string name = (string)dr["DocumentName"];
                                    docList += HTMLHelper.HTMLEncode(name);
                                    if (isLink)
                                    {
                                        docList += DocumentHelper.GetDocumentMarkImage(Page, DocumentMarkEnum.Link);
                                    }
                                    docList += "<br />";
                                    lblDocuments.Text = docList;

                                    // Set visibility of checkboxes
                                    TreeNode node = TreeNode.New(ValidationHelper.GetString(dr["ClassName"], string.Empty), dr);

                                    if (!IsUserAuthorizedToTranslateDocument(node, currentSite.SiteName, targetCulture))
                                    {
                                        HideUI();
                                        plcMessages.AddError(String.Format(GetString("cmsdesk.notauthorizedtotranslatedocument"), HTMLHelper.HTMLEncode(node.NodeAliasPath)));
                                    }
                                }
                            }
                        }

                        // Display check box for separate submissions for each document if there is more than one document
                        pnlSeparateSubmissions.Visible = (nodeIds.Count > 1);
                    }
                    else
                    {
                        // Hide everything
                        pnlContent.Visible = false;
                    }
                }

                btnCancel.Text = GetString("general.cancel");
                btnCancel.Attributes.Add("onclick", ctlAsync.GetCancelScript(true) + "return false;");

                // Register the dialog script
                ScriptHelper.RegisterDialogScript(this);

                titleElemAsync.TitleText = GetString("ContentTranslate.TranslatingDocuments");
                // Set visibility of panels
                pnlContent.Visible = true;
                pnlLog.Visible = false;
            }
        }
        else
        {
            pnlContent.Visible = false;
            ShowError(GetString("dialogs.badhashtext"));
        }
    }


    private void HideUI()
    {
        pnlButtons.Visible = false;
        pnlSettings.Visible = false;
        pnlList.Visible = false;
        plcInfo.Visible = false;
    }


    protected override void OnPreRender(EventArgs e)
    {
        if (!translationElem.SourceCultureAvailable)
        {
            pnlSettings.Visible = false;
            btnOk.Visible = false;
            ShowError(GetString("translations.nosourceculture"));
        }

        if (translationElem.DisplayedServicesCount == 1)
        {
            headTranslate.ResourceString = null;
            headTranslate.Text = HTMLHelper.HTMLEncode(translationElem.DisplayedServiceName);
        }

        // Overwrite cancelNodeId variable if sub-levels are visible
        if (AllLevels && Parameters.ContainsKey("refreshnodeid"))
        {
            cancelNodeId = ValidationHelper.GetInteger(Parameters["refreshnodeid"], 0);
        }

        string refreshCurrent;

        if (IsDialog)
        {
            btnNo.Visible = false;
            refreshCurrent = "function RefreshCurrent(){ CloseDialog(); RefreshTree(" + cancelNodeId + "," + cancelNodeId + "); }";
        }
        else
        {
            btnNo.OnClientClick = "SelectNode(" + cancelNodeId + "); return false";
            refreshCurrent = "function RefreshCurrent(){ RefreshTree(" + cancelNodeId + "," + cancelNodeId + "); }";
        }

        ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "refreshCurrent", ScriptHelper.GetScript(refreshCurrent));

        base.OnPreRender(e);
    }

    #endregion


    #region "Button actions"

    protected void btnTranslate_Click(object sender, EventArgs e)
    {
        if (isSelect)
        {
            // If in select mode, prepare node IDs now
            TreeProvider tree = new TreeProvider(currentUser);
            DataSet ds = tree.SelectNodes(CurrentSiteName, pathElem.Value.ToString(), TreeProvider.ALL_CULTURES, true, TreeProvider.ALL_CLASSNAMES, null,
                "DocumentName", TreeProvider.ALL_LEVELS, false, 0, TreeProvider.SELECTNODES_REQUIRED_COLUMNS + ",DocumentName,NodeParentID,NodeSiteID,NodeAliasPath");

            if (!DataHelper.DataSourceIsEmpty(ds))
            {
                foreach (DataTable table in ds.Tables)
                {
                    foreach (DataRow dr in table.Rows)
                    {
                        TreeNode node = TreeNode.New(ValidationHelper.GetString(dr["ClassName"], string.Empty), dr);

                        if (IsUserAuthorizedToTranslateDocument(node, currentSite.SiteName, translationElem.TargetLanguage))
                        {
                            nodeIds.Add(ValidationHelper.GetInteger(dr["NodeID"], 0));
                        }
                        else
                        {
                            HideUI();
                            plcMessages.AddError(String.Format(GetString("cmsdesk.notauthorizedtotranslatedocument"), HTMLHelper.HTMLEncode(node.NodeAliasPath)));
                            return;
                        }
                    }
                }
            }
            else
            {
                ShowError(GetString("translationservice.nodocumentstotranslate"));
                return;
            }

            targetCulture = translationElem.TargetLanguage;
        }

        pnlLog.Visible = true;
        pnlContent.Visible = false;

        CurrentError = string.Empty;
        CurrentLog.Close();
        EnsureLog();

        ctlAsync.Parameter = AllLevels;
        ctlAsync.RunAsync(Translate, WindowsIdentity.GetCurrent());
    }

    #endregion


    #region "Async methods"

    /// <summary>
    /// Translates document(s).
    /// </summary>
    private void Translate(object parameter)
    {
        if (parameter == null || nodeIds.Count < 1)
        {
            return;
        }

        int refreshId = 0;

        TreeProvider tree = new TreeProvider(currentUser);
        tree.AllowAsyncActions = false;

        try
        {
            // Begin log
            AddLog(ResHelper.GetString("contentrequest.starttranslate", currentCulture));

            bool oneSubmission = chkSeparateSubmissions.Checked;

            // Prepare translation settings
            TranslationSettings settings = new TranslationSettings();
            settings.TargetLanguage = targetCulture;
            settings.TranslateWebpartProperties = SettingsKeyInfoProvider.GetBoolValue(SiteContext.CurrentSiteName + ".CMSTranslateWebpartProperties");
            settings.SourceLanguage = translationElem.FromLanguage;
            settings.Instructions = translationElem.Instructions;
            settings.Priority = translationElem.Priority;
            settings.TranslateAttachments = translationElem.ProcessBinary;
            settings.ProcessBinary = translationElem.ProcessBinary;
            settings.TranslationDeadline = translationElem.Deadline;
            settings.TranslationServiceName = translationElem.SelectedService;

            using (var tr = new CMSTransactionScope())
            {
                // Get the translation provider
                AbstractMachineTranslationService machineService = null;
                AbstractHumanTranslationService humanService = null;
                TranslationSubmissionInfo submission = null;
                TranslationServiceInfo ti = TranslationServiceInfoProvider.GetTranslationServiceInfo(translationElem.SelectedService);
                if (ti != null)
                {
                    // Set if we need target tag (Translations.com workaround)
                    settings.GenerateTargetTag = ti.TranslationServiceGenerateTargetTag;

                    if (oneSubmission)
                    {
                        if (ti.TranslationServiceIsMachine)
                        {
                            machineService = AbstractMachineTranslationService.GetTranslationService(ti, CurrentSiteName);
                        }
                        else
                        {
                            humanService = AbstractHumanTranslationService.GetTranslationService(ti, CurrentSiteName);
                        }
                    }

                    bool langSupported = true;
                    if (humanService != null)
                    {
                        if (!humanService.IsLanguageSupported(settings.TargetLanguage))
                        {
                            AddError(ResHelper.GetString("translationservice.targetlanguagenotsupported"));
                            langSupported = false;
                        }
                        else
                        {
                            submission = TranslationServiceHelper.CreateSubmissionInfo(settings, ti, MembershipContext.AuthenticatedUser.UserID, SiteContext.CurrentSiteID, "Document submission " + DateTime.Now);
                        }
                    }

                    if (langSupported)
                    {
                        if (!oneSubmission || (machineService != null) || (humanService != null))
                        {
                            const string columns = "NodeID, NodeAliasPath, DocumentCulture, NodeParentID";
                            string submissionFileName = "";
                            string submissionName = "";
                            int charCount = 0;
                            int wordCount = 0;
                            int docCount = 0;

                            // Prepare the where condition
                            var where = new WhereCondition().WhereIn("NodeID", nodeIds);

                            // Get the documents in target culture to be able to check if "Skip already translated" option is on
                            // Combine both, source and target culture (at least one hit has to be found - to find the source of translation)
                            where.WhereIn("DocumentCulture", new[] { settings.SourceLanguage, settings.TargetLanguage });

                            DataSet ds = tree.SelectNodes(SiteContext.CurrentSiteName, "/%", TreeProvider.ALL_CULTURES, true, null, where.ToString(true), "NodeAliasPath DESC", TreeProvider.ALL_LEVELS, false, 0, columns);
                            if (!DataHelper.DataSourceIsEmpty(ds))
                            {
                                List<int> processedNodes = new List<int>();

                                // Translate the documents
                                foreach (DataRow dr in ds.Tables[0].Rows)
                                {
                                    refreshId = ValidationHelper.GetInteger(dr["NodeParentID"], 0);
                                    int nodeId = ValidationHelper.GetInteger(dr["NodeID"], 0);

                                    if (!processedNodes.Contains(nodeId))
                                    {
                                        processedNodes.Add(nodeId);

                                        string aliasPath = ValidationHelper.GetString(dr["NodeAliasPath"], "");
                                        string culture = ValidationHelper.GetString(dr["DocumentCulture"], "");

                                        if (chkSkipTranslated.Checked)
                                        {
                                            if (culture == settings.TargetLanguage)
                                            {
                                                // Document already exists in requested culture, skip it
                                                AddLog(string.Format(ResHelper.GetString("content.translatedalready"), HTMLHelper.HTMLEncode(aliasPath + " (" + culture + ")")));
                                                continue;
                                            }
                                        }

                                        // Get document in source language
                                        TreeNode node = DocumentHelper.GetDocument(nodeId, settings.SourceLanguage, true, null);
                                        if (node == null)
                                        {
                                            // Document doesn't exist in source culture, skip it
                                            continue;
                                        }

                                        AddLog(string.Format(ResHelper.GetString("content.translating"), HTMLHelper.HTMLEncode(aliasPath + " (" + culture + ")")));

                                        // Save the first document as a base for submission name
                                        if (string.IsNullOrEmpty(submissionName))
                                        {
                                            submissionName = node.GetDocumentName();
                                        }
                                        if (string.IsNullOrEmpty(submissionFileName))
                                        {
                                            submissionFileName = node.NodeAlias;
                                        }

                                        docCount++;

                                        // Submit the document
                                        if (machineService != null)
                                        {
                                            TranslationServiceHelper.Translate(machineService, settings, node);
                                        }
                                        else
                                        {
                                            if (oneSubmission)
                                            {
                                                TreeNode targetNode = TranslationServiceHelper.CreateTargetCultureNode(node, settings.TargetLanguage, true, false, !settings.TranslateAttachments);
                                                TranslationSubmissionItemInfo submissionItem = TranslationServiceHelper.CreateSubmissionItemInfo(settings, submission, node, targetNode.DocumentID);

                                                charCount += submissionItem.SubmissionItemCharCount;
                                                wordCount += submissionItem.SubmissionItemWordCount;
                                            }
                                            else
                                            {
                                                TranslationServiceHelper.SubmitToTranslation(settings, node, out submission);
                                            }
                                        }
                                    }
                                }

                                if (docCount > 0)
                                {
                                    if (oneSubmission && (humanService != null))
                                    {
                                        AddLog(ResHelper.GetString("content.submitingtranslation"));

                                        // Set submission name
                                        int itemCount = docCount;
                                        if (itemCount > 1)
                                        {
                                            submissionName += " " + string.Format(GetString("translationservices.submissionnamesuffix"), itemCount - 1);
                                        }
                                        submission.SubmissionName = submissionName;
                                        submission.SubmissionCharCount = charCount;
                                        submission.SubmissionWordCount = wordCount;
                                        submission.SubmissionItemCount = itemCount;
                                        submission.SubmissionParameter = submissionFileName;

                                        string err = humanService.CreateSubmission(submission);
                                        if (!string.IsNullOrEmpty(err))
                                        {
                                            AddError(LocalizationHelper.GetString("ContentRequest.TranslationFailed") + " \"" + err + "\"");
                                        }

                                        // Save submission with ticket
                                        TranslationSubmissionInfoProvider.SetTranslationSubmissionInfo(submission);
                                    }
                                }
                                else
                                {
                                    TranslationSubmissionInfoProvider.DeleteTranslationSubmissionInfo(submission);
                                    AddError(ResHelper.GetString("TranslateDocument.DocumentsAlreadyTranslated", currentCulture));
                                }
                            }
                            else
                            {
                                TranslationSubmissionInfoProvider.DeleteTranslationSubmissionInfo(submission);
                                AddError(ResHelper.GetString("TranslateDocument.NoSourceDocuments", currentCulture));
                            }
                        }
                        else
                        {
                            AddError(ResHelper.GetString("TranslateDocument.TranslationServiceNotFound", currentCulture));
                        }
                    }
                }

                tr.Commit();
            }
        }
        catch (ThreadAbortException ex)
        {
            string state = ValidationHelper.GetString(ex.ExceptionState, string.Empty);
            if (state == CMSThread.ABORT_REASON_STOP)
            {
                // When canceled
                AddError(ResHelper.GetString("TranslateDocument.TranslationCanceled", currentCulture));
            }
            else
            {
                // Log error
                LogExceptionToEventLog(ex);
            }
        }
        catch (Exception ex)
        {
            // Log error
            LogExceptionToEventLog(ex);
        }
        finally
        {
            if (IsDialog)
            {
                ctlAsync.Parameter = "wopener.location.replace(wopener.location); CloseDialog(); wopener.RefreshTree(null, null);";
            }
            else
            {
                if (string.IsNullOrEmpty(CurrentError))
                {
                    // Overwrite refreshId variable if sub-levels are visible
                    if (ValidationHelper.GetBoolean(parameter, false) && Parameters.ContainsKey("refreshnodeid"))
                    {
                        refreshId = ValidationHelper.GetInteger(Parameters["refreshnodeid"], 0);
                    }

                    // Refresh tree
                    ctlAsync.Parameter = "RefreshTree(" + refreshId + ", " + refreshId + "); \n" + "SelectNode(" + refreshId + ");";
                }
                else
                {
                    ctlAsync.Parameter = "RefreshTree(null, null);";
                }
            }
        }

    }

    #endregion


    #region "Help methods"

    /// <summary>
    /// When exception occurs, log it to event log.
    /// </summary>
    /// <param name="ex">Exception to log</param>
    private void LogExceptionToEventLog(Exception ex)
    {
        EventLogProvider.LogEvent(EventType.ERROR, "Content", "TRANSLATEDOC", EventLogProvider.GetExceptionLogMessage(ex), RequestContext.RawURL, currentUser.UserID, currentUser.UserName, 0, null, RequestContext.UserHostAddress, currentSite.SiteID);

        AddError(ResHelper.GetString("ContentRequest.TranslationFailed", currentCulture) + ex.Message);
    }


    /// <summary>
    /// Adds the script to the output request window.
    /// </summary>
    /// <param name="script">Script to add</param>
    public override void AddScript(string script)
    {
        ltlScript.Text += ScriptHelper.GetScript(script);
    }


    /// <summary>
    /// Checks whether the user is authorized to translate document.
    /// </summary>
    /// <param name="node">Document node</param>
    /// <param name="siteName">Site name</param>
    /// <param name="targetCultureCode">Culture code of the document which should be checked</param>
    protected bool IsUserAuthorizedToTranslateDocument(TreeNode node, string siteName, string targetCultureCode)
    {
        // Check create permission for document
        return (currentUser.IsAuthorizedPerDocument(node, new[] { NodePermissionsEnum.Create, NodePermissionsEnum.Read }, true, targetCultureCode) == AuthorizationResultEnum.Allowed) &&
               (currentUser.IsAuthorizedPerResource("CMS.Content", "SubmitForTranslation"));
    }

    #endregion


    #region "Handling async thread"

    private void ctlAsync_OnCancel(object sender, EventArgs e)
    {
        ctlAsync.Parameter = null;
        string canceled = GetString("TranslateDocument.TranslationCanceled");
        AddLog(canceled);
        ShowConfirmation(canceled);
        ltlScript.Text += ScriptHelper.GetScript("var __pendingCallbacks = new Array();RefreshCurrent();");
        if (!String.IsNullOrEmpty(CurrentError))
        {
            ShowError(CurrentError);
        }
        CurrentLog.Close();
    }


    private void ctlAsync_OnRequestLog(object sender, EventArgs e)
    {
        ctlAsync.Log = CurrentLog.Log;
    }


    private void ctlAsync_OnError(object sender, EventArgs e)
    {
        if (ctlAsync.Status == AsyncWorkerStatusEnum.Running)
        {
            ctlAsync.Stop();
        }
        ctlAsync.Parameter = null;
        ShowError(CurrentError);
        CurrentLog.Close();
    }


    private void ctlAsync_OnFinished(object sender, EventArgs e)
    {
        ShowError(CurrentError);
        CurrentLog.Close();

        if (!string.IsNullOrEmpty(CurrentError))
        {
            ctlAsync.Parameter = null;
            ShowError(CurrentError);
        }

        if (ctlAsync.Parameter != null)
        {
            AddScript(ctlAsync.Parameter.ToString());

            // Do not set the window title anymore
            PageTitle.SetWindowTitle = false;
        }
    }


    /// <summary>
    /// Ensures the logging context.
    /// </summary>
    protected LogContext EnsureLog()
    {
        LogContext log = LogContext.EnsureLog(ctlAsync.ProcessGUID);
        return log;
    }


    /// <summary>
    /// Adds the log information.
    /// </summary>
    /// <param name="newLog">New log information</param>
    protected void AddLog(string newLog)
    {
        EnsureLog();
        LogContext.AppendLine(newLog);
    }


    /// <summary>
    /// Adds the error to collection of errors.
    /// </summary>
    /// <param name="error">Error message</param>
    protected void AddError(string error)
    {
        AddLog(error);
        CurrentError = (error + "<br />" + CurrentError);
    }

    #endregion
}