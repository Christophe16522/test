using System;
using System.Text;
using System.Data;

using CMS.DocumentEngine;
using CMS.Helpers;
using CMS.Localization;
using CMS.SiteProvider;
using CMS.UIControls;
using CMS.TranslationServices;

using TreeNode = CMS.DocumentEngine.TreeNode;
using CMS.DataEngine;

public partial class CMSModules_Translations_Controls_TranslationServiceSelector : CMSUserControl
{
    #region "Variables"

    private readonly string defaultCulture = CultureHelper.GetDefaultCultureCode(SiteContext.CurrentSiteName);
    private bool mAnyServiceAvailable;
    private bool mDisplayMachineServices = true;
    private string mMachineServiceSuffix;

    #endregion


    #region "Properties"

    /// <summary>
    /// Gets or sets TreeProvider to use.
    /// </summary>
    public TreeProvider TreeProvider
    {
        get;
        set;
    }


    /// <summary>
    /// Gets number of services displayed.
    /// </summary>
    public int DisplayedServicesCount
    {
        get;
        private set;
    }


    /// <summary>
    /// If true, the name of the service is displayed when only one is available.
    /// </summary>
    public bool DisplayOnlyServiceName
    {
        get;
        set;
    }


    /// <summary>
    /// Gets name of the service displayed (if only one is displayed, if more services are displayed, null is returned)
    /// </summary>
    public string DisplayedServiceName
    {
        get;
        private set;
    }


    /// <summary>
    /// Gets or sets NodeID which to translates
    /// </summary>
    public int NodeID
    {
        get;
        set;
    }


    /// <summary>
    /// Gets or sets TranslationSettings object which will be used to process/submit translation.
    /// </summary>
    public TranslationSettings TranslationSettings
    {
        get;
        set;
    }


    /// <summary>
    /// Returns if the translation should process binary fields.
    /// </summary>
    public bool ProcessBinary
    {
        get
        {
            return chkProcessBinary.Checked;
        }
        set
        {
            chkProcessBinary.Checked = value;
        }
    }


    /// <summary>
    /// Returns the instructions of the submission.
    /// </summary>
    public string Instructions
    {
        get
        {
            return txtInstruction.Text;
        }
        set
        {
            txtInstruction.Text = value;
        }
    }


    /// <summary>
    /// Gets or sets source language of translation.
    /// </summary>
    public string FromLanguage
    {
        get
        {
            return selectCultureElem.Value.ToString();
        }
        set
        {
            selectCultureElem.Value = value;
        }
    }


    /// <summary>
    /// Gets or sets target language of translation.
    /// </summary>
    public string TargetLanguage
    {
        get
        {
            return selectTargetCultureElem.Value.ToString();
        }
        set
        {
            selectTargetCultureElem.Value = value;
        }
    }


    /// <summary>
    /// Returns the instructions of the submission.
    /// </summary>
    public int Priority
    {
        get
        {
            return ValidationHelper.GetInteger(drpPriority.Value, 1);
        }
        set
        {
            drpPriority.Value = value.ToString();
        }
    }


    /// <summary>
    /// Returns the instructions of the submission.
    /// </summary>
    public DateTime Deadline
    {
        get
        {
            return dtDeadline.SelectedDateTime;
        }
        set
        {
            dtDeadline.SelectedDateTime = value;
        }
    }


    /// <summary>
    /// Gets or sets the selected service name.
    /// </summary>
    public string SelectedService
    {
        get
        {
            return hdnSelectedName.Value;
        }
        set
        {
            hdnSelectedName.Value = value;
        }
    }


    /// <summary>
    /// Determines whether to display machine translation services.
    /// </summary>
    public bool DisplayMachineServices
    {
        get
        {
            return mDisplayMachineServices;
        }
        set
        {
            mDisplayMachineServices = value;
        }
    }


    /// <summary>
    /// Determines whether to display machine translation services.
    /// </summary>
    public bool DisplayTargetlanguage
    {
        get
        {
            return plcTargetLang.Visible;
        }
        set
        {
            plcTargetLang.Visible = value;
        }
    }


    /// <summary>
    /// String which will be addad as a suffix of each machine translation service.
    /// </summary>
    public string MachineServiceSuffix
    {
        get
        {
            if (mMachineServiceSuffix == null)
            {
                return GetString("translationservice.machineservicesuffix");
            }
            return mMachineServiceSuffix;
        }
        set
        {
            mMachineServiceSuffix = value;
        }
    }


    /// <summary>
    /// Indicates if there is any enabled service available in the selector.
    /// </summary>
    public bool AnyServiceAvailable
    {
        get
        {
            return mAnyServiceAvailable;
        }
    }


    /// <summary>
    /// Returns true if there is at least one source culture which can be used for translation.
    /// </summary>
    public bool SourceCultureAvailable
    {
        get
        {
            return selectCultureElem.UniSelector.HasData;
        }
    }

    #endregion


    #region "Page Events"

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!StopProcessing && !RequestHelper.IsCallback())
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(@"
function SelectService(serviceName, supportsInstructions, supportsPriority, supportsAttachments, supportsDeadline) {
    var nameElem = document.getElementById('", hdnSelectedName.ClientID, @"');
    if (nameElem != null) {
        nameElem.value = serviceName;
    }
  
    document.getElementById('pnlInstructions').style.display = (supportsInstructions ? '' : 'none');
    document.getElementById('pnlPriority').style.display = (supportsPriority ? '' : 'none');
    document.getElementById('pnlDeadline').style.display = (supportsDeadline ? '' : 'none');
    document.getElementById('pnlProcessBinary').style.display = (supportsAttachments ? '' : 'none');
    var selectButton = document.getElementById('rad' + serviceName);
    if (selectButton != null) {
        selectButton.checked = 'checked';
    }
}");

            ScriptHelper.RegisterClientScriptBlock(Page, typeof(string), "TranslationServiceSelector", sb.ToString(), true);

            string format = "{% CultureName %}{% if (CultureCode == \"" + defaultCulture + "\") { \" \" +\"" + GetString("general.defaultchoice") + "\" } %}";
            selectCultureElem.UniSelector.DisplayNameFormat = format;
            selectTargetCultureElem.UniSelector.DisplayNameFormat = format;

            if (!CurrentUser.IsGlobalAdministrator && CurrentUser.UserHasAllowedCultures)
            {
                selectTargetCultureElem.AdditionalWhereCondition = "CultureID IN (SELECT CultureID FROM CMS_UserCulture WHERE UserID = " + CurrentUser.UserID + " AND SiteID = " + CurrentSite.SiteID + ")";
            }

            // Preselect 'Normal' priority
            if (!URLHelper.IsPostback())
            {
                drpPriority.Value = 1;
            }

            if (TranslationSettings != null)
            {
                string where = "NOT CultureCode = '" + SqlHelper.GetSafeQueryString(TranslationSettings.TargetLanguage) + "'";
                if (NodeID > 0)
                {
                    where = SqlHelper.AddWhereCondition(where, "CultureCode IN (SELECT DocumentCulture FROM CMS_Document WHERE DocumentNodeID = " + NodeID + ")", "AND");
                }
                selectCultureElem.AdditionalWhereCondition = where;
            }

            ReloadData();
        }
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        if (!StopProcessing)
        {
            if (!RequestHelper.IsPostBack())
            {
                dtDeadline.SelectedDateTime = DateTime.Now.AddDays(7);
                selectCultureElem.Value = defaultCulture;
            }
        }
    }


    /// <summary>
    /// Selects correct value.
    /// </summary>
    private void ReloadData()
    {
        string where = "TranslationServiceEnabled = 1";
        if (!DisplayMachineServices)
        {
            where += " AND TranslationServiceIsMachine = 0";
        }

        bool allowBinary = SettingsKeyInfoProvider.GetBoolValue(SiteContext.CurrentSiteName + ".CMSAllowAttachmentTranslation");

        DataSet ds = TranslationServiceInfoProvider.GetTranslationServices(where, "TranslationServiceIsMachine DESC, TranslationServiceDisplayName ASC", 0, "TranslationServiceDisplayName, TranslationServiceName, TranslationServiceIsMachine, TranslationServiceSupportsPriority, TranslationServiceSupportsInstructions, TranslationServiceSupportsDeadline, TranslationServiceGenerateTargetTag");
        if (!DataHelper.DataSourceIsEmpty(ds))
        {
            string selected = hdnSelectedName.Value;
            string lastDisplayName = null;

            int i = 0;

            string machSelectScript = null;
            ltlServices.Text += "<div class=\"radio-list-vertical\">";

            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                string codeName = ValidationHelper.GetString(dr["TranslationServiceName"], "");

                // Check availability
                if (TranslationServiceHelper.IsServiceAvailable(codeName, SiteContext.CurrentSiteName))
                {
                    if (string.IsNullOrEmpty(selected) && (i == 0))
                    {
                        selected = codeName;
                    }

                    bool isMachine = ValidationHelper.GetBoolean(dr["TranslationServiceIsMachine"], false);
                    string displayName = ValidationHelper.GetString(dr["TranslationServiceDisplayName"], "");
                    bool supportsInstructions = ValidationHelper.GetBoolean(dr["TranslationServiceSupportsInstructions"], false);
                    bool supportsPriority = ValidationHelper.GetBoolean(dr["TranslationServiceSupportsPriority"], false);
                    bool supportsDeadline = ValidationHelper.GetBoolean(dr["TranslationServiceSupportsDeadline"], false);

                    if (isMachine && !string.IsNullOrEmpty(MachineServiceSuffix))
                    {
                        displayName += MachineServiceSuffix;
                    }

                    string selectScript = "SelectService(" + ScriptHelper.GetString(codeName) + ", " + (supportsInstructions ? "true" : "false") + ", " +
                                          (supportsPriority ? "true" : "false") + ", " + (!isMachine && allowBinary ? "true" : "false") + ", " +
                                          (supportsDeadline ? "true" : "false") + ")";


                    bool isSelected = selected.Equals(codeName, StringComparison.CurrentCultureIgnoreCase);
                    if (isSelected)
                    {
                        hdnSelectedName.Value = selected;
                        if (string.IsNullOrEmpty(machSelectScript))
                        {
                            machSelectScript = selectScript;
                        }
                    }

                    string radioBtn = "<div class=\"radio\"><input id=\"rad" + codeName + "\" " + (isSelected ? "checked=\"checked\"" : "") + " type=\"radio\" name=\"services\" value=\"" + codeName + "\" onclick=\"" + selectScript + "\" />";
                    radioBtn += "<label for=\"rad" + codeName + "\">" + HTMLHelper.HTMLEncode(displayName) + "</label></div>";
                    lastDisplayName = displayName;

                    mAnyServiceAvailable = true;

                    ltlServices.Text += radioBtn;
                    i++;
                }
            }

            ltlServices.Text += "</div>";

            // If only one service is available, display it in a different way
            if (i == 1)
            {
                DisplayedServiceName = lastDisplayName;
                ltlServices.Text = DisplayOnlyServiceName ? "<strong>" + HTMLHelper.HTMLEncode(lastDisplayName) + "</strong>" : String.Empty;
            }

            if (!String.IsNullOrEmpty(machSelectScript))
            {
                // Register selection script for first service
                ScriptHelper.RegisterStartupScript(Page, typeof(string), "TranslationServiceSelectorSelection", machSelectScript, true);
            }

            DisplayedServicesCount = i;
        }
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Validates the data. Returns error msg if something is not ok.
    /// </summary>
    private string ValidateData()
    {
        if (dtDeadline.SelectedDateTime != DateTimeHelper.ZERO_TIME)
        {
            if (dtDeadline.SelectedDateTime < DateTime.Now)
            {
                return GetString("translationservice.invaliddeadline");
            }
        }
        return null;
    }


    /// <summary>
    /// Submits the node for translation. Does not check the permissions, you need to chek it before calling this method.
    /// </summary>
    public string SubmitToTranslation()
    {
        string err = ValidateData();
        if (!string.IsNullOrEmpty(err))
        {
            return err;
        }

        TranslationSettings settings = TranslationSettings ?? new TranslationSettings();
        if (string.IsNullOrEmpty(settings.TargetLanguage))
        {
            settings.TargetLanguage = LocalizationContext.PreferredCultureCode;
        }
        settings.TranslateWebpartProperties = SettingsKeyInfoProvider.GetBoolValue(SiteContext.CurrentSiteName + ".CMSTranslateWebpartProperties");
        settings.SourceLanguage = FromLanguage;
        settings.Instructions = Instructions;
        settings.Priority = Priority;
        settings.ProcessBinary = ProcessBinary;
        settings.TranslateAttachments = ProcessBinary;
        settings.TranslationDeadline = Deadline;
        settings.TranslationServiceName = hdnSelectedName.Value;

        TreeProvider tree = TreeProvider ?? new TreeProvider();
        TreeNode node = DocumentHelper.GetDocument(NodeID, settings.SourceLanguage, true, tree);

        TranslationSubmissionInfo submissionInfo;

        return TranslationServiceHelper.SubmitToTranslation(settings, node, out submissionInfo);
    }

    #endregion
}