using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;

using CMS.Base;
using CMS.Controls;
using CMS.DataEngine;
using CMS.DocumentEngine;
using CMS.EventLog;
using CMS.ExtendedControls;
using CMS.ExtendedControls.ActionsConfig;
using CMS.FormControls;
using CMS.FormEngine;
using CMS.Helpers;
using CMS.IO;
using CMS.MacroEngine;
using CMS.Membership;
using CMS.OnlineForms;
using CMS.PortalEngine;
using CMS.Search;
using CMS.SiteProvider;
using CMS.Synchronization;
using CMS.UIControls;

public partial class CMSModules_AdminControls_Controls_Class_FieldEditor_FieldEditor : CMSUserControl
{
    #region "Events"

    /// <summary>
    /// Event raised when OK button is clicked and before xml definition is changed.
    /// </summary>
    public event EventHandler OnBeforeDefinitionUpdate;


    /// <summary>
    /// Event raised when OK button is clicked and after xml definition is changed.
    /// </summary>
    public event EventHandler OnAfterDefinitionUpdate;


    /// <summary>
    /// Event raised when new field is created and form definition is saved.
    /// </summary>
    public event OnFieldCreatedEventHandler OnFieldCreated;


    /// <summary>
    /// Event raised when field name was changed.
    /// </summary>
    public event OnFieldNameChangedEventHandler OnFieldNameChanged;


    /// <summary>
    /// Event raised when selected item (field or category) is deleted. Second parameter is of <see cref="CMS.UIControls.FieldEditorEventArgs"/> type.
    /// </summary>
    public event AfterItemDeletedEventHandler AfterItemDeleted;

    #endregion


    #region "Variables"

    private FormInfo fi;
    private FormFieldInfo ffi;
    private FormCategoryInfo fci;
    private IEnumerable<string> columnNames;
    private bool mAllowDummyFields;
    private bool mAllowExtraFields;
    private bool mShowFieldVisibility;
    private bool mDevelopmentMode;
    private string mClassName = String.Empty;
    private string mImageDirectoryPath;
    private bool mDisplaySourceFieldSelection = true;
    private int mWebPartId;
    private FieldEditorModeEnum mMode;
    private FieldEditorControlsEnum mDisplayedControls = FieldEditorControlsEnum.ModeSelected;
    private bool mEnableSystemFields;
    private bool mEnableMacrosForDefaultValue = true;
    private string mDisplayIn = String.Empty;
    private SaveAction btnSave;
    private HeaderAction btnReset;
    private bool mIsWizard;
    private bool disableSaveAction;
    private bool mEnabled = true;
    private bool mShowQuickLinks = true;

    #endregion


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


    /// <summary>
    /// Indicates if control is used on live site.
    /// </summary>
    public override bool IsLiveSite
    {
        get
        {
            return base.IsLiveSite;
        }
        set
        {
            hdrActions.IsLiveSite = value;
            plcMess.IsLiveSite = value;
            base.IsLiveSite = value;
        }
    }


    /// <summary>
    /// Adjust the context in which the attribute can be displayed.
    /// </summary>
    public string DisplayIn
    {
        get
        {
            return mDisplayIn;
        }
        set
        {
            mDisplayIn = value;
        }
    }


    /// <summary>
    /// Indicates if system fields from tables CMS_Tree and CMS_Document are offered to the user.
    /// </summary>
    public bool EnableSystemFields
    {
        get
        {
            return mEnableSystemFields;
        }
        set
        {
            mEnableSystemFields = value;
            databaseConfiguration.EnableSystemFields = value;
            fieldTypeSelector.EnableSystemFields = value;
        }
    }


    /// <summary>
    /// Indicates if field visibility selector should be displayed.
    /// </summary>
    public bool ShowFieldVisibility
    {
        get
        {
            return mShowFieldVisibility;
        }
        set
        {
            mShowFieldVisibility = value;
            fieldAppearance.ShowFieldVisibility = value;
        }
    }


    /// <summary>
    /// Indicates if field editor works in development mode.
    /// </summary>
    public bool DevelopmentMode
    {
        get
        {
            return mDevelopmentMode;
        }
        set
        {
            mDevelopmentMode = value;
            fieldAppearance.DevelopmentMode = value;
            fieldTypeSelector.DevelopmentMode = value;
        }
    }


    /// <summary>
    /// Class name.
    /// </summary>
    public string ClassName
    {
        get
        {
            return mClassName;
        }
        set
        {
            mClassName = value;
            fieldAppearance.ClassName = value;
        }
    }


    /// <summary>
    /// Coupled class name.
    /// </summary>
    public string CoupledClassName
    {
        get;
        set;
    }


    /// <summary>
    /// Header actions control.
    /// </summary>
    public override HeaderActions HeaderActions
    {
        get
        {
            if (!UseCustomHeaderActions)
            {
                return base.HeaderActions;
            }

            return hdrActions;
        }
    }


    /// <summary>
    /// Directory path for images.
    /// </summary>
    public string ImageDirectoryPath
    {
        get
        {
            if (String.IsNullOrEmpty(mImageDirectoryPath))
            {
                mImageDirectoryPath = "CMSModules/CMS_Class/";
            }
            return mImageDirectoryPath;
        }
        set
        {
            if (!value.EndsWithCSafe("/"))
            {
                mImageDirectoryPath = value + "/";
            }
            else
            {
                mImageDirectoryPath = value;
            }
        }
    }


    /// <summary>
    /// Indicates if display source field selection.
    /// </summary>
    public bool DisplaySourceFieldSelection
    {
        get
        {
            return mDisplaySourceFieldSelection;
        }
        set
        {
            mDisplaySourceFieldSelection = value;
        }
    }


    /// <summary>
    /// Webpart ID.
    /// </summary>
    public int WebPartId
    {
        get
        {
            return mWebPartId;
        }
        set
        {
            mWebPartId = value;
        }
    }


    /// <summary>
    /// Field editor mode.
    /// </summary>
    public FieldEditorModeEnum Mode
    {
        get
        {
            return mMode;
        }
        set
        {
            mMode = value;
            fieldAppearance.Mode = value;
            fieldTypeSelector.Mode = value;
        }
    }


    /// <summary>
    /// Type of custom controls that can be selected from the control list in FieldEditor.
    /// </summary>
    public FieldEditorControlsEnum DisplayedControls
    {
        get
        {
            return mDisplayedControls;
        }
        set
        {
            mDisplayedControls = value;
            fieldAppearance.DisplayedControls = value;
        }
    }


    /// <summary>
    /// Form XML definition.
    /// </summary>
    public string FormDefinition
    {
        get
        {
            return ValidationHelper.GetString(ViewState["FormDefinition"], string.Empty);
        }
        set
        {
            ViewState["FormDefinition"] = value;
        }
    }


    /// <summary>
    /// Form XML definition of original object.
    /// </summary>
    public string OriginalFormDefinition
    {
        get
        {
            return ValidationHelper.GetString(ViewState["OriginalFormDefinition"], string.Empty);
        }
        set
        {
            ViewState["OriginalFormDefinition"] = value;
        }
    }


    /// <summary>
    /// Enable or disable the option to use macros as default value.
    /// </summary>
    public bool EnableMacrosForDefaultValue
    {
        get
        {
            return mEnableMacrosForDefaultValue;
        }
        set
        {
            mEnableMacrosForDefaultValue = value;
        }
    }


    /// <summary>
    /// Gets or sets value indicating if control is placed in wizard.
    /// </summary>
    public bool IsWizard
    {
        get
        {
            return mIsWizard;
        }
        set
        {
            mIsWizard = value;
            fieldAdvancedSettings.IsWizard = value;
        }
    }


    /// <summary>
    /// Indicates if current form definition is inherited.
    /// </summary>
    private bool IsInheritedForm
    {
        get
        {
            switch (Mode)
            {
                case FieldEditorModeEnum.InheritedFormControl:
                case FieldEditorModeEnum.Widget:
                case FieldEditorModeEnum.InheritedWebPartProperties:
                case FieldEditorModeEnum.SystemWebPartProperties:
                    return true;

                default:
                    return false;
            }
        }
    }


    /// <summary>
    /// Indicates if fields without database representation can be created.
    /// </summary>
    public bool AllowDummyFields
    {
        get
        {
            return mAllowDummyFields;
        }
        set
        {
            mAllowDummyFields = value;
            fieldTypeSelector.AllowDummyFields = value;
        }
    }


    /// <summary>
    /// Indicates if extra fields can be created.
    /// </summary>
    public bool AllowExtraFields
    {
        get
        {
            return mAllowExtraFields;
        }
        set
        {
            mAllowExtraFields = value;
            fieldTypeSelector.AllowExtraFields = value;
        }
    }


    /// <summary>
    /// Indicates if Field Editor is used as alternative form.
    /// </summary>
    public bool IsAlternativeForm
    {
        get
        {
            switch (Mode)
            {
                case FieldEditorModeEnum.AlternativeBizFormDefinition:
                case FieldEditorModeEnum.AlternativeClassFormDefinition:
                case FieldEditorModeEnum.AlternativeCustomTable:
                case FieldEditorModeEnum.AlternativeSystemTable:
                    return true;

                default:
                    return false;
            }
        }
    }


    /// <summary>
    /// Gets or sets alternative form full name.
    /// </summary>
    public string AlternativeFormFullName
    {
        get;
        set;
    }


    /// <summary>
    /// Shows in what control is this basic form used.
    /// </summary>
    public FormTypeEnum FormType
    {
        get
        {
            return controlSettings.FormType;
        }
        set
        {
            controlSettings.FormType = value;
        }
    }


    /// <summary>
    /// Enables or disables to edit <see cref='CMS.FormEngine.FormFieldInfo.Inheritable' /> settings.
    /// </summary>
    public bool ShowInheritanceSettings
    {
        get
        {
            return fieldAppearance.ShowInheritanceSettings;
        }
        set
        {
            fieldAppearance.ShowInheritanceSettings = value;
        }
    }


    /// <summary>
    /// Indicates if custom header actions should be used. Actions of current master page are used by default.
    /// </summary>
    public bool UseCustomHeaderActions
    {
        get;
        set;
    }


    /// <summary>
    /// Indicates if the control is enabled. When set False, the control is in read-only mode.
    /// </summary>
    public bool Enabled
    {
        get
        {
            return mEnabled;
        }
        set
        {
            mEnabled = value;
        }
    }


    /// <summary>
    /// Gets or sets Name of the macro rule category(ies) which should be displayed in Rule designer. Items should be separated by semicolon.
    /// </summary>
    public string ValidationRulesCategory
    {
        get
        {
            return ValidationHelper.GetString(UIContext["ConditionsCategory"], string.Empty);
        }
        set
        {
            UIContext["ConditionsCategory"] = value;
        }
    }


    /// <summary>
    /// Indicates if quick links can be displayed under the attribute list for selected fields.
    /// </summary>
    public bool ShowQuickLinks
    {
        get
        {
            return mShowQuickLinks;
        }
        set
        {
            mShowQuickLinks = value;
        }
    }

    #endregion


    #region "Private properties"

    /// <summary>
    /// Returns True if system fields are enabled and one of them is selected.
    /// </summary>
    private bool IsSystemFieldSelected
    {
        get
        {
            return databaseConfiguration.IsSystemFieldSelected;
        }
    }


    /// <summary>
    /// Indicates whether new item is edited.
    /// </summary>
    private bool IsNewItemEdited
    {
        get
        {
            return ValidationHelper.GetBoolean(ViewState["IsNewItemEdited"], false);
        }
        set
        {
            ViewState["IsNewItemEdited"] = value;
            fieldTypeSelector.IsNewItemEdited = value;
            databaseConfiguration.IsNewItemEdited = value;
        }
    }


    /// <summary>
    /// Selected item name.
    /// </summary>
    private string SelectedItemName
    {
        get
        {
            return ValidationHelper.GetString(ViewState["SelectedItemName"], string.Empty);
        }
        set
        {
            ViewState["SelectedItemName"] = value;
        }
    }


    /// <summary>
    /// Selected item type.
    /// </summary>
    private FieldEditorSelectedItemEnum SelectedItemType
    {
        get
        {
            object obj = ViewState["SelectedItemType"];
            return (obj == null) ? 0 : (FieldEditorSelectedItemEnum)obj;
        }
        set
        {
            ViewState["SelectedItemType"] = value;
        }
    }


    /// <summary>
    /// Is field primary.
    /// </summary>
    private bool IsPrimaryField
    {
        get
        {
            return ValidationHelper.GetBoolean(ViewState["IsPrimaryField"], false);
        }
        set
        {
            ViewState["IsPrimaryField"] = value;
        }
    }


    /// <summary>
    /// Indicates if field has no representation in database.
    /// </summary>
    private bool IsDummyField
    {
        get
        {
            return ValidationHelper.GetBoolean(ViewState["IsDummyField"], false);
        }
        set
        {
            ViewState["IsDummyField"] = value;
        }
    }


    /// <summary>
    /// Indicates if dummy field is in original or alternative form
    /// </summary>
    private bool IsDummyFieldFromMainForm
    {
        get
        {
            return ValidationHelper.GetBoolean(ViewState["IsDummyFieldFromMainForm"], false);
        }
        set
        {
            ViewState["IsDummyFieldFromMainForm"] = value;
        }
    }


    /// <summary>
    /// Indicates if field is extra field (field is not in orignal form definition).
    /// </summary>
    private bool IsExtraField
    {
        get
        {
            return ValidationHelper.GetBoolean(ViewState["IsExtraField"], false);
        }
        set
        {
            ViewState["IsExtraField"] = value;
        }
    }


    /// <summary>
    /// Returns True if system fields are enabled and one of them is selected.
    /// </summary>
    private bool IsSystemField
    {
        get;
        set;
    }


    /// <summary>
    /// Indicates if document type is edited.
    /// </summary>
    private bool IsDocumentType
    {
        get
        {
            object obj = ViewState["IsDocumentType"];
            if ((obj == null) && (!string.IsNullOrEmpty(ClassName)))
            {
                DataClassInfo dci = DataClassInfoProvider.GetDataClassInfo(mClassName);
                ViewState["IsDocumentType"] = ((dci != null) && dci.ClassIsDocumentType);
            }
            return ValidationHelper.GetBoolean(ViewState["IsDocumentType"], false);
        }
    }


    /// <summary>
    /// Gets or sets value indicating what item was selected in field type drop-down list.
    /// </summary>
    private string PreviousField
    {
        get
        {
            return ValidationHelper.GetString(ViewState["PreviousValue"], string.Empty);
        }
        set
        {
            ViewState["PreviousValue"] = value;
        }
    }


    /// <summary>
    /// Gets or sets value indicating if detailed controls are visible.
    /// </summary>
    private bool FieldDetailsVisible
    {
        get
        {
            return ValidationHelper.GetBoolean(ViewState["FieldDetailsVisible"], false);
        }
        set
        {
            ViewState["FieldDetailsVisible"] = value;
        }
    }


    /// <summary>
    /// Gets macro resolver name.
    /// </summary>
    private string ResolverName
    {
        get
        {
            string formName = string.IsNullOrEmpty(AlternativeFormFullName) ? ClassName : AlternativeFormFullName;
            return !string.IsNullOrEmpty(formName) ? FormHelper.FORM_PREFIX + formName : FormHelper.FORMDEFINITION_PREFIX + FormDefinition;
        }
    }

    #endregion


    #region "Global definitions"

    // Constants
    private const string newCategPreffix = "#categ##new#";
    private const string newFieldPreffix = "#field##new#";
    private const string categPreffix = "#categ#";
    private const string fieldPreffix = "#field#";
    private const int preffixLength = 7; // Length of categPreffix = length of fieldPreffix = 7
    private const string controlPrefix = "#uc#";

    #endregion


    #region "Page events"

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        if (StopProcessing)
        {
            return;
        }

        var page = Page as CMSPage;
        if (page != null)
        {
            page.EnsureScriptManager();
            if (page.ScriptManagerControl != null)
            {
                var script = new ScriptReference("~/CMSScripts/RestoreLostFocus.js");
                page.ScriptManagerControl.Scripts.Add(script);
            }
        }

        CreateHeaderActions();

        // Set method delegates
        fieldAppearance.GetControls = GetControls;
    }


    protected override void LoadViewState(object savedState)
    {
        base.LoadViewState(savedState);

        if (ViewState["CurrentFormFields"] != null)
        {
            // Refresh uicontext data
            UIContext["CurrentFormFields"] = ViewState["CurrentFormFields"];
        }
    }


    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        if (StopProcessing)
        {
            return;
        }

        fieldAdvancedSettings.ResolverName = controlSettings.ResolverName = cssSettings.ResolverName = htmlEnvelope.ResolverName = ResolverName;
        validationSettings.ResolverName = fieldAppearance.ResolverName = databaseConfiguration.ResolverName = categoryEdit.ResolverName = ResolverName;

        fieldAdvancedSettings.ShowDisplayInSimpleModeCheckBox = ((Mode == FieldEditorModeEnum.FormControls) || (Mode == FieldEditorModeEnum.InheritedFormControl));

        lnkCode.Visible = SystemContext.DevelopmentMode && !IsWizard;
        lnkFormDef.Visible = SystemContext.DevelopmentMode && !IsWizard;

        // Set images url
        btnDeleteItem.ToolTip = GetString("TemplateDesigner.DeleteItem");
        btnDownAttribute.ToolTip = GetString("TemplateDesigner.DownAttribute");
        btnUpAttribute.ToolTip = GetString("TemplateDesigner.UpAttribute");

        if (CMSString.Compare(DisplayIn, FormInfo.DISPLAY_CONTEXT_DASHBOARD, true) == 0)
        {
            pnlDisplayInDashBoard.Visible = true;
        }
        ltlConfirmText.Text = "<input type=\"hidden\" id=\"confirmdelete\" value=\"" + GetString("TemplateDesigner.ConfirmDelete") + "\"/>";
        btnDeleteItem.OnClientClick = "if (!confirmDelete()) { return false; }";
        btnUpAttribute.Enabled = true;
        btnDownAttribute.Enabled = true;
        btnDeleteItem.Enabled = true;

        btnAdd.Actions.Add(new CMSButtonAction
        {
            Text = GetString("TemplateDesigner.NewAttribute"),
            OnClientClick = ControlsHelper.GetPostBackEventReference(btnNewAttribute) + "; return false;"
        });
        btnAdd.Actions.Add(new CMSButtonAction
        {
            Text = GetString("TemplateDesigner.NewCategory"),
            OnClientClick = ControlsHelper.GetPostBackEventReference(btnNewCategory) + "; return false;"
        });

        databaseConfiguration.LoadGroupField();

        if (!URLHelper.IsPostback())
        {
            // Preselect field if query-string parameter is set
            string preselectedField = QueryHelper.GetString("selectedfield", string.Empty).Replace("%23", "#");
            Reload(preselectedField);
        }
        else
        {
            LoadFormDefinition();
            ffi = fi.GetFormField(SelectedItemName);
            LoadControlSettings(PreviousField);
            LoadValidationSettings();
        }

        // Register event handlers
        fieldTypeSelector.OnSelectionChanged += fieldTypeSelector_OnSelectionChanged;
        databaseConfiguration.DropChanged += databaseConfiguration_DropChanged;
        databaseConfiguration.AttributeChanged += databaseConfiguration_AttributeChanged;
        fieldAppearance.OnFieldSelected += control_FieldSelected;
        documentSource.OnSourceFieldChanged += documentSource_OnSourceFieldChanged;

        plcValidation.Visible = true;
        plcQuickValidation.Visible = true;
        plcSettings.Visible = true;
        pnlContent.Enabled = true;

        // Ensure save action and disable it if necessary
        EnsureHeaderActions(false);
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        // Display controls and quick links according to current mode
        bool displayDetails = FieldDetailsVisible && chkDisplayInForm.Checked;

        fieldAppearance.Visible = plcQuickAppearance.Visible = displayDetails;
        fieldAdvancedSettings.Visible = displayDetails;
        cssSettings.Visible = plcQuickStyles.Visible = displayDetails;
        htmlEnvelope.Visible = plcQuickHtmlEnvelope.Visible = displayDetails;
        validationSettings.DisplayControls();
        validationSettings.Visible = plcQuickValidation.Visible = displayDetails && validationSettings.Visible;
        controlSettings.CheckVisibility();
        controlSettings.Visible = plcQuickSettings.Visible = displayDetails && controlSettings.Visible;

        chkDisplayInDashBoard.Enabled = chkDisplayInForm.Checked;
        plcFieldType.Visible = IsNewItemEdited;

        // Display action buttons
        DisplayOrHideActions();

        // Display and store last value
        PreviousField = fieldAppearance.FieldType;

        // Hide quick links if selected item is not field or of only 'database quick link' should be displayed
        pnlQuickSelect.Visible = ShowQuickLinks && (SelectedItemType == FieldEditorSelectedItemEnum.Field) && (plcQuickAppearance.Visible || plcQuickSettings.Visible || plcQuickValidation.Visible || plcQuickStyles.Visible);

        var master = Page.Master as ICMSMasterPage;
        if (master != null)
        {
            var contentPanel = master.PanelContent;
            if (!IsWizard && (contentPanel != null))
            {
                contentPanel.CssClass = string.Empty;
            }
        }

        // Highlight attribute list items
        HighlightListItems();

        // Ensure save action and disable it if necessary
        EnsureHeaderActions(true);
    }

    #endregion


    #region "Action events"

    /// <summary>
    /// Actions handler.
    /// </summary>
    protected void HeaderActions_ActionPerformed(object sender, CommandEventArgs e)
    {
        switch (e.CommandName.ToLowerCSafe())
        {
            case "save":
                SaveField();
                break;
            case "reset":
                ResetFormElementToOriginal();
                break;
        }
    }


    /// <summary>
    /// Handle Save's OnClick event if edited object supports locking.
    /// </summary>
    protected void ObjectManager_OnSaveData(object sender, SimpleObjectManagerEventArgs e)
    {
        SaveField();
    }


    /// <summary>
    /// Reloads data after undocheckout.
    /// </summary>
    protected void ObjectManager_OnAfterAction(object sender, SimpleObjectManagerEventArgs e)
    {
        switch (e.ActionName)
        {
            case ComponentEvents.UNDO_CHECKOUT:
                // Reload the page and preselect current field/category
                URLHelper.ResponseRedirect(URLHelper.AddParameterToUrl(RequestContext.CurrentURL, "selectedfield", lstAttributes.SelectedValue.Replace("#", "%23")));
                break;
            case ComponentEvents.CHECKOUT:
                // Set control settings section
                controlSettings.AllowModeSwitch = true;
                controlSettings.SimpleMode = true;
                break;
        }
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Reload field editor.
    /// </summary>
    /// <param name="selectedValue">Selected field in field list</param>
    public void Reload(string selectedValue)
    {
        Reload(selectedValue, false);
    }


    /// <summary>
    /// Reload field editor.
    /// </summary>
    /// <param name="selectedValue">Selected field in field list</param>
    /// <param name="partialReload">Indicates if not all controls need to be reloaded</param>
    private void Reload(string selectedValue, bool partialReload)
    {
        bool isModeSelected = false;
        bool isItemSelected = false;

        // Check for alternative form mode
        if ((IsAlternativeForm) || (IsInheritedForm))
        {
            if (!string.IsNullOrEmpty(FormDefinition))
            {
                isModeSelected = true;
            }
            else
            {
                // Clear item list
                lstAttributes.Items.Clear();
            }
        }
        else
        {
            switch (mMode)
            {
                case FieldEditorModeEnum.General:
                case FieldEditorModeEnum.FormControls:
                case FieldEditorModeEnum.ProcessActions:
                    if (!string.IsNullOrEmpty(FormDefinition))
                    {
                        isModeSelected = true;
                    }
                    else
                    {
                        // Clear item list
                        lstAttributes.Items.Clear();
                    }
                    break;

                case FieldEditorModeEnum.ClassFormDefinition:
                case FieldEditorModeEnum.BizFormDefinition:
                case FieldEditorModeEnum.SystemTable:
                case FieldEditorModeEnum.CustomTable:
                    if (!string.IsNullOrEmpty(mClassName))
                    {
                        isModeSelected = true;
                    }
                    else
                    {
                        ShowError(GetString("fieldeditor.noclassname"));
                    }
                    break;

                case FieldEditorModeEnum.WebPartProperties:
                case FieldEditorModeEnum.SystemWebPartProperties:
                    if ((mWebPartId > 0))
                    {
                        isModeSelected = true;
                    }
                    else
                    {
                        ShowError(GetString("fieldeditor.nowebpartid"));
                    }
                    break;

                case FieldEditorModeEnum.PageTemplateProperties:
                    isModeSelected = true;
                    break;

                default:
                    ShowError(GetString("fieldeditor.nomode"));
                    break;
            }
        }

        if (!partialReload)
        {
            // Display controls if mode is determined
            ShowOrHideFieldDetails(false);
        }

        if (isModeSelected)
        {
            isItemSelected = LoadInnerControls(selectedValue, partialReload);
        }

        // Hide controls when item isn't selected
        if ((!partialReload) && (!isItemSelected))
        {
            HideAllPanels();
            disableSaveAction = true;
            btnUpAttribute.Enabled = false;
            btnDownAttribute.Enabled = false;
            btnDeleteItem.Enabled = false;
            btnNewAttribute.Enabled = true; // Only new items can be added
            btnNewCategory.Enabled = true; // Only new items can be added
            ShowInformation(GetString("fieldeditor.nofieldsdefined"));
        }

        if (documentSource.VisibleContent)
        {
            documentSource.Reload();
        }

        // Show or hide field visibility selector
        fieldAppearance.ShowFieldVisibility = ShowFieldVisibility;

        // Prepare Reset button and information messages
        PrepareResetButton();
        DisplayFieldInformation();
    }


    /// <summary>
    /// Creates header actions - save and reset buttons.
    /// </summary>
    private void CreateHeaderActions()
    {
        if (!UseCustomHeaderActions && HeaderActions == null)
        {
            return;
        }

        // Create Reset button
        btnReset = new HeaderAction
        {
            CommandName = "reset",
            Visible = false
        };

        if (UseCustomHeaderActions)
        {
            // Add save action
            btnSave = new SaveAction(Page);
            btnSave.Enabled = Enabled;
            HeaderActions.AddAction(btnSave);
            HeaderActions.AddAction(btnReset);
            pnlHeaderActions.Visible = pnlHeaderPlaceHolder.Visible = true;
            pnlContentPadding.CssClass += " Menu";
        }
        else
        {
            ObjectEditMenu menu = (ObjectEditMenu)ControlsHelper.GetChildControl(Page, typeof(ObjectEditMenu));
            if (menu != null)
            {
                menu.AddExtraAction(btnReset);
                menu.AllowSave = Enabled;
            }
            else if (HeaderActions != null)
            {
                HeaderActions.AddAction(btnReset);
            }
        }

        controlSettings.BasicForm.EnsureMessagesPlaceholder(MessagesPlaceHolder);
    }


    /// <summary>
    /// Ensures header actions.
    /// </summary>
    private void EnsureHeaderActions(bool reload)
    {
        if (HeaderActions == null)
        {
            return;
        }

        if (HeaderActions.ActionsList.Exists(a => (a is SaveAction && a.Visible)))
        {
            // Get save from default page actions
            var saveAction = (SaveAction)HeaderActions.ActionsList.Single(a => a is SaveAction);
            if ((btnSave == null) || (btnSave != saveAction))
            {
                btnSave = saveAction;
            }
        }

        if (btnSave == null)
        {
            // Create new action
            btnSave = new SaveAction(Page);
            btnSave.Enabled = Enabled;

            HeaderActions.InsertAction(-2, btnSave);
            if (reload)
            {
                HeaderActions.ReloadData();
            }
        }

        if (!UseCustomHeaderActions)
        {
            if (HeaderActions.MessagesPlaceHolder != null)
            {
                HeaderActions.MessagesPlaceHolder.OffsetX = 250;
            }

            var manager = CMSObjectManager.GetCurrent(Page);
            if (manager != null)
            {
                manager.ShowPanel = true;
                manager.OnSaveData += ObjectManager_OnSaveData;
                manager.OnAfterAction += ObjectManager_OnAfterAction;
            }
        }

        HeaderActions.ActionPerformed += HeaderActions_ActionPerformed;
        HeaderActions.PerformFullPostBack = false;

        if (reload)
        {
            if (btnSave != null && disableSaveAction)
            {
                // Disable save action
                btnSave.Enabled = false;
            }

            // Hide action buttons if edited object is checked in or checked out by another user
            if (((btnSave == null || !btnSave.Enabled) && (IsObjectChecked())) || !Enabled)
            {
                // Disable control elements
                pnlContent.Enabled = btnAdd.Enabled = documentSource.Enabled = btnUpAttribute.Enabled = btnDownAttribute.Enabled = btnNewCategory.Enabled
                    = btnNewAttribute.Enabled = btnDeleteItem.Enabled = false;

                // Disable mode switch for disabled dialog
                controlSettings.AllowModeSwitch = false;
                controlSettings.SimpleMode = false;
            }
        }
    }


    /// <summary>
    /// Returns true if edited object is checked in or checked out by other user.
    /// </summary>
    private bool IsObjectChecked()
    {
        bool result = false;

        ObjectEditMenu menu = (ObjectEditMenu)ControlsHelper.GetChildControl(Page, typeof(ObjectEditMenu));
        if (menu != null)
        {
            result = ((menu.ShowCheckOut || ((menu.InfoObject != null) && (menu.InfoObject.Generalized.IsCheckedOut)))
                && SynchronizationHelper.UseCheckinCheckout);
        }

        return result;
    }


    /// <summary>
    /// Highlights attribute list items like categories, primary keys and hidden fields.
    /// </summary>
    private void HighlightListItems()
    {
        if (fi == null)
        {
            LoadFormDefinition();
        }

        if ((lstAttributes.Items.Count > 0) && (fi != null))
        {
            foreach (ListItem li in lstAttributes.Items)
            {
                // Mark category item with different color
                string cssClass;
                if (li.Value.StartsWithCSafe(categPreffix))
                {
                    cssClass = "field-editor-category-item";

                    FormCategoryInfo formCategory = fi.GetFormCategory(li.Value.Substring(preffixLength));
                    if ((formCategory != null) && (formCategory.IsDummy || formCategory.IsExtra))
                    {
                        // Highlight dummy categories
                        cssClass += " field-editor-dummy-category";
                    }

                    li.Attributes.Add("class", cssClass);
                }
                else
                {
                    cssClass = string.Empty;

                    // Get form field info
                    FormFieldInfo formField = fi.GetFormField(li.Value.Substring(preffixLength));
                    if (formField != null)
                    {
                        if (DevelopmentMode && formField.PrimaryKey)
                        {
                            // Highlight primary keys in the list
                            cssClass = "field-editor-primary-attribute";
                        }
                        if (!formField.Visible)
                        {
                            if (!string.IsNullOrEmpty(cssClass))
                            {
                                cssClass += " ";
                            }
                            // Highlight fields that are not visible
                            cssClass += "field-editor-hidden-item";
                        }
                        if ((formField.IsDummyField) || (formField.IsExtraField))
                        {
                            if (!string.IsNullOrEmpty(cssClass))
                            {
                                cssClass += " ";
                            }
                            // Highlight dummy fields
                            cssClass += "field-editor-dummy-field";
                        }
                        if (!string.IsNullOrEmpty(cssClass))
                        {
                            li.Attributes.Add("class", cssClass);
                        }
                    }
                }
            }
        }
    }


    /// <summary>
    /// Loads inner FieldEditor controls.
    /// </summary>
    /// <returns>Returns TRUE if any item is selected</returns>
    private bool LoadInnerControls(string selectedValue, bool partialReload)
    {
        bool isItemSelected = false;
        LoadFormDefinition();
        if (!partialReload)
        {
            LoadAttributesList(selectedValue);
        }

        documentSource.IsAlternativeForm = IsAlternativeForm;
        documentSource.IsInheritedForm = IsInheritedForm;
        documentSource.ClassName = ClassName;

        fieldAppearance.IsAlternativeForm = IsAlternativeForm;
        fieldAppearance.AlternativeFormFullName = AlternativeFormFullName;

        if (SelectedItemType == FieldEditorSelectedItemEnum.Field)
        {
            isItemSelected = true;
            DisplaySelectedTabContent();
            ffi = fi.GetFormField(SelectedItemName);
            LoadSelectedField(partialReload);
        }
        else if (SelectedItemType == FieldEditorSelectedItemEnum.Category)
        {
            isItemSelected = true;
            LoadSelectedCategory();
        }
        return isItemSelected;
    }


    /// <summary>
    /// Load xml definition of the form.
    /// </summary>
    /// <returns>True if the load is successful.</returns>
    private bool LoadFormDefinition()
    {
        bool isError = false;

        if ((!IsAlternativeForm) && (!IsInheritedForm))
        {
            switch (mMode)
            {
                case FieldEditorModeEnum.General:
                    // Definition is loaded from external xml
                    break;

                case FieldEditorModeEnum.WebPartProperties:
                    // Load xml definition from web part info
                    WebPartInfo wpi = WebPartInfoProvider.GetWebPartInfo(mWebPartId);
                    if (wpi != null)
                    {
                        FormDefinition = wpi.WebPartProperties;
                    }
                    else
                    {
                        isError = true;
                    }
                    break;

                case FieldEditorModeEnum.ClassFormDefinition:
                case FieldEditorModeEnum.BizFormDefinition:
                case FieldEditorModeEnum.SystemTable:
                case FieldEditorModeEnum.CustomTable:
                    // Load xml definition from Class info
                    DataClassInfo dci = DataClassInfoProvider.GetDataClassInfo(mClassName);
                    if (dci != null)
                    {
                        FormDefinition = dci.ClassFormDefinition;
                    }
                    else
                    {
                        isError = true;
                    }
                    break;
            }
        }
        else
        {
            isError = String.IsNullOrEmpty(FormDefinition);
        }

        if (isError)
        {
            ShowError("[ FieldEditor.LoadFormDefinition() ]: " + GetString("FieldEditor.XmlDefinitionNotLoaded"));
        }

        fi = new FormInfo(FormDefinition);

        InitUIContext(fi);

        return (!isError);
    }


    /// <summary>
    /// Fill attribute list.
    /// </summary>
    /// <param name="selectedValue">Selected value in attribute list, null if first item is selected</param>
    private void LoadAttributesList(string selectedValue)
    {
        ListItem li = null;

        // Reload list only if new item is not edited
        if (!IsNewItemEdited)
        {
            // Clear item list
            lstAttributes.Items.Clear();

            // Get all list items (fields and categories)        
            var itemList = fi.GetFormElements(true, true);

            if (itemList != null)
            {
                MacroResolver resolver = MacroResolverStorage.GetRegisteredResolver(ResolverName);
                foreach (IField item in itemList)
                {
                    string itemDisplayName;
                    string itemCodeName;
                    if (item is FormFieldInfo)
                    {
                        FormFieldInfo formField = ((FormFieldInfo)(item));

                        itemDisplayName = formField.Name;
                        if (!formField.AllowEmpty)
                        {
                            itemDisplayName += ResHelper.RequiredMark;
                        }

                        itemCodeName = fieldPreffix + formField.Name;

                        li = new ListItem(itemDisplayName, itemCodeName);
                    }
                    else if (item is FormCategoryInfo)
                    {
                        FormCategoryInfo formCategory = ((FormCategoryInfo)(item));

                        itemDisplayName = ResHelper.LocalizeString(formCategory.GetPropertyValue(FormCategoryPropertyEnum.Caption, resolver));
                        itemCodeName = categPreffix + formCategory.CategoryName;

                        li = new ListItem(itemDisplayName, itemCodeName);
                    }

                    // Load list box
                    if (li != null)
                    {
                        lstAttributes.Items.Add(li);
                    }
                }
            }

            // Set selected item
            if (lstAttributes.Items.Count > 0)
            {
                if (!string.IsNullOrEmpty(selectedValue) && lstAttributes.Items.FindByValue(selectedValue) != null)
                {
                    lstAttributes.SelectedValue = selectedValue;
                }
                else
                {
                    // Select first item of the list       
                    lstAttributes.SelectedIndex = 0;
                }
            }

            // Default values - list is empty
            SelectedItemName = null;
            SelectedItemType = 0;

            // Save selected item info
            if (lstAttributes.SelectedValue != null)
            {
                if (lstAttributes.SelectedValue.StartsWithCSafe(fieldPreffix))
                {
                    SelectedItemName = lstAttributes.SelectedValue.Substring(preffixLength);
                    SelectedItemType = FieldEditorSelectedItemEnum.Field;
                }
                else if (lstAttributes.SelectedValue.StartsWithCSafe(categPreffix))
                {
                    SelectedItemName = lstAttributes.SelectedValue.Substring(preffixLength);
                    SelectedItemType = FieldEditorSelectedItemEnum.Category;
                }
            }
        }
    }


    /// <summary>
    /// Sets all values of the category edit form to defaults.
    /// </summary>
    private void LoadDefaultCategoryEditForm()
    {
        plcCategory.Visible = true;
        plcField.Visible = false;
        categoryEdit.Reload();
        LoadFormDefinition();
    }


    /// <summary>
    /// Sets all values of form to defaults.
    /// </summary>
    /// <param name="partialReload">True - indicates that only some controls should be loaded, False - reload all controls</param>
    private void LoadDefaultAttributeEditForm(bool partialReload)
    {
        ffi = null;
        plcCategory.Visible = false;
        chkDisplayInForm.Checked = true;

        if (!partialReload)
        {
            databaseConfiguration.DevelopmentMode = DevelopmentMode;
            databaseConfiguration.ShowSystemFields = IsSystemField;
            databaseConfiguration.IsDocumentType = IsDocumentType;
            databaseConfiguration.Mode = Mode;
            databaseConfiguration.ClassName = ClassName;
            databaseConfiguration.CoupledClassName = CoupledClassName;
            databaseConfiguration.IsAlternativeForm = IsAlternativeForm;
            databaseConfiguration.IsInheritedForm = IsInheritedForm;
            databaseConfiguration.IsDummyField = IsDummyField;
            databaseConfiguration.IsDummyFieldFromMainForm = IsDummyFieldFromMainForm;
            databaseConfiguration.IsExtraField = IsExtraField;
            databaseConfiguration.Reload("", IsNewItemEdited);
            databaseConfiguration.ShowDefaultControl();
        }

        if (IsSystemField)
        {
            LoadSystemField();
        }

        string defaultControl = FormHelper.GetFormFieldDefaultControlType(databaseConfiguration.AttributeType);

        fieldAppearance.AttributeType = databaseConfiguration.AttributeType;
        fieldAppearance.FieldType = defaultControl;
        fieldAppearance.LoadFieldTypes(IsPrimaryField);
        fieldAppearance.ClassName = ClassName;
        fieldAppearance.Reload();
        fieldAdvancedSettings.Reload();
        cssSettings.Reload();
        htmlEnvelope.Reload();
        LoadValidationSettings();
        validationSettings.DisplayControls();
        validationSettings.Reload();
    }


    /// <summary>
    /// Fill form with selected category data.
    /// </summary>    
    private void LoadSelectedCategory()
    {
        plcField.Visible = false;
        plcCategory.Visible = true;

        fci = fi.GetFormCategory(SelectedItemName);

        categoryEdit.CategoryInfo = fci;
        categoryEdit.Reload();

        if (fci != null)
        {
            HandleInherited(fci.IsInherited);
            IsDummyField = fci.IsDummy;
            IsExtraField = fci.IsExtra;
            IsDummyFieldFromMainForm = false;
        }
        else
        {
            LoadDefaultCategoryEditForm();
        }
    }


    /// <summary>
    /// Displays controls for field editing.
    /// </summary>
    private void ShowFieldOptions()
    {
        plcCategory.Visible = false;
        databaseConfiguration.Visible = true;
        controlSettings.Visible = true;
        fieldAppearance.Visible = true;
        fieldAdvancedSettings.Visible = true;
        validationSettings.Visible = true;
        cssSettings.Visible = true;
        htmlEnvelope.Visible = true;
        FieldDetailsVisible = true;
    }


    /// <summary>
    /// Handles the inheritance of the field.
    /// </summary>
    private void HandleInherited(bool inherited)
    {
        pnlField.Enabled = true;
        btnDeleteItem.Visible = true;

        ShowInformation(String.Empty);

        if (inherited && !IsAlternativeForm)
        {
            // Get information on inherited class
            DataClassInfo dci = DataClassInfoProvider.GetDataClassInfo(mClassName);
            if (dci != null)
            {
                DataClassInfo parentCi = DataClassInfoProvider.GetDataClassInfo(dci.ClassInheritsFromClassID);
                if (parentCi != null)
                {
                    pnlField.Enabled = false;
                    btnDeleteItem.Visible = false;
                    disableSaveAction = true;

                    ShowInformation(String.Format(GetString("DocumentType.FieldIsInherited"), parentCi.ClassDisplayName));
                }
            }
        }
    }


    /// <summary>
    /// Handles inheritance of selected form item.
    /// </summary>
    private void HandleItemInheritance()
    {
        bool inherited = false;
        switch (SelectedItemType)
        {
            case FieldEditorSelectedItemEnum.Field:
                inherited = fi.GetFormField(SelectedItemName).IsInherited;
                break;
            case FieldEditorSelectedItemEnum.Category:
                inherited = fi.GetFormCategory(SelectedItemName).IsInherited;
                break;
        }
        HandleInherited(inherited);
    }


    /// <summary>
    /// Fill form with selected attribute data.
    /// </summary>    
    /// <param name="partialReload">Indicates if only some controls should be reloaded</param>
    private void LoadSelectedField(bool partialReload)
    {
        // Fill form
        if (ffi != null)
        {
            HandleInherited(ffi.IsInherited);

            IsPrimaryField = ffi.PrimaryKey;
            IsDummyField = ffi.IsDummyField;
            IsDummyFieldFromMainForm = ffi.IsDummyFieldFromMainForm;
            IsExtraField = ffi.IsExtraField;

            if (!partialReload)
            {
                bool controlIsInvalid = !ffi.Visible || (CMSString.Compare(ffi.Settings["controlname"] as String, FormHelper.GetFormFieldControlTypeString(FormFieldControlTypeEnum.Unknown), true) != 0);
                chkDisplayInForm.Checked = ffi.Visible && controlIsInvalid;
                chkDisplayInDashBoard.Checked = (CMSString.Compare(ffi.DisplayIn, DisplayIn, true) == 0);
            }

            DisplaySelectedTabContent();
            ShowFieldOptions();

            if (!partialReload)
            {
                databaseConfiguration.DevelopmentMode = DevelopmentMode;
                databaseConfiguration.ShowSystemFields = ffi.External;
                databaseConfiguration.FieldInfo = ffi;
                databaseConfiguration.IsDocumentType = IsDocumentType;
                databaseConfiguration.Mode = Mode;
                databaseConfiguration.IsAlternativeForm = IsAlternativeForm;
                databaseConfiguration.IsInheritedForm = IsInheritedForm;
                databaseConfiguration.ClassName = ClassName;
                databaseConfiguration.CoupledClassName = CoupledClassName;
                databaseConfiguration.IsDummyField = ffi.IsDummyField;
                databaseConfiguration.IsDummyFieldFromMainForm = ffi.IsDummyFieldFromMainForm;
                databaseConfiguration.IsExtraField = ffi.IsExtraField;
                databaseConfiguration.Reload(ffi.Name, IsNewItemEdited);
            }

            if (chkDisplayInForm.Checked && fieldAppearance.Visible)
            {
                fieldAppearance.Mode = Mode;
                fieldAppearance.ClassName = ClassName;
                fieldAppearance.FieldInfo = ffi;
                fieldAppearance.AttributeType = databaseConfiguration.AttributeType;
                fieldAppearance.Reload();
            }

            if (chkDisplayInForm.Checked && fieldAdvancedSettings.Visible)
            {
                fieldAdvancedSettings.FieldInfo = ffi;
                fieldAdvancedSettings.Reload();
            }

            if (chkDisplayInForm.Checked && validationSettings.Visible)
            {
                LoadValidationSettings();
                validationSettings.DisplayControls();
                validationSettings.Reload();
            }

            if (chkDisplayInForm.Checked && cssSettings.Visible)
            {
                cssSettings.FieldInfo = ffi;
                cssSettings.Reload();
                cssSettings.Enabled = true;
            }

            if (chkDisplayInForm.Checked && htmlEnvelope.Visible)
            {
                htmlEnvelope.FieldInfo = ffi;
                htmlEnvelope.Reload();
                htmlEnvelope.Enabled = true;
            }
        }
        else
        {
            IsSystemField = false;
            LoadDefaultAttributeEditForm(partialReload);
        }

        LoadControlSettings();
    }


    /// <summary>
    /// Loads validation settings.
    /// </summary>
    private void LoadValidationSettings()
    {
        validationSettings.IsPrimary = IsPrimaryField;
        validationSettings.FieldInfo = ffi;
        validationSettings.Mode = Mode;
    }


    /// <summary>
    /// Displays or hides actions according to the selected mode.
    /// </summary>
    protected void DisplayOrHideActions()
    {
        // Hide actions if new fields are not allowed
        if ((!AllowDummyFields && IsAlternativeForm) || (!AllowExtraFields && IsInheritedForm))
        {
            plcMainButtons.Visible = false;
        }
        else
        {
            // Deny delete original field in alternative or inherited form
            if (((!IsDummyField || IsDummyFieldFromMainForm) && IsAlternativeForm) || (!IsExtraField && IsInheritedForm))
            {
                btnDeleteItem.Visible = false;
            }
        }

        if (Mode == FieldEditorModeEnum.SystemWebPartProperties)
        {
            // Hide all buttons in webpart system properties
            pnlButtons.Visible = false;
        }
    }


    protected void lnkCode_Click(object sender, EventArgs e)
    {
        GenerateCode(false);
    }


    protected void lnkFormDef_Click(object sender, EventArgs e)
    {
        GetFormDefinition();
    }


    /// <summary>
    /// Outputs the form definition XML of the current view
    /// </summary>
    protected void GetFormDefinition()
    {
        LoadFormDefinition();

        if (fi != null)
        {
            string xml = HTMLHelper.ReformatHTML(fi.GetXmlDefinition());

            // Write directly to response
            Response.Clear();
            Response.AddHeader("Content-Disposition", "attachment; filename=formDefinition.xml");
            Response.ContentType = "text/xml";
            Response.Write(xml);
            Response.End();
        }
    }


    /// <summary>
    /// Generates the class code.
    /// </summary>
    /// <param name="file">If true, the code is generated to a file, if false, it gets directly to output</param>
    protected void GenerateCode(bool file)
    {
        switch (mMode)
        {
            case FieldEditorModeEnum.WebPartProperties:
            case FieldEditorModeEnum.SystemTable:
            case FieldEditorModeEnum.FormControls:
            case FieldEditorModeEnum.ProcessActions:
            case FieldEditorModeEnum.InheritedFormControl:
            case FieldEditorModeEnum.AlternativeSystemTable:
            case FieldEditorModeEnum.Widget:
            case FieldEditorModeEnum.SystemWebPartProperties:
            case FieldEditorModeEnum.PageTemplateProperties:
                return;
        }

        try
        {
            // Prepare the folders
            string templateFile = Server.MapPath("~/App_Data/CodeTemplates/");

            string codeFile = Server.MapPath("~/App_Code");
            if (!Directory.Exists(codeFile))
            {
                codeFile = Server.MapPath("~/Old_App_Code");
            }

            codeFile += "/Global/AutoGenerated/";

            // Ensure the directory
            if (file)
            {
                Directory.CreateDirectory(codeFile);
            }

            // Save xml string to CMS_Class table
            DataClassInfo dci = DataClassInfoProvider.GetDataClassInfo(mClassName);
            if (dci != null)
            {
                // Prepare the class name
                string className = dci.ClassName;

                int dotIndex = className.LastIndexOfCSafe('.');
                if (dotIndex >= 0)
                {
                    className = className.Substring(dotIndex + 1);
                }

                className = ValidationHelper.GetIdentifier(className, "");
                className = className[0].ToString().ToUpperCSafe() + className.Substring(1);

                string originalClassName;

                if (dci.ClassIsDocumentType)
                {
                    // Document type
                    className += "Document";
                    codeFile += "DocumentTypes/";
                    templateFile += "DocType.template";
                    originalClassName = "DocType";
                }
                else if (dci.ClassIsCustomTable)
                {
                    // Custom table
                    className += "Item";
                    codeFile += "CustomTables/";
                    templateFile += "CustomTableType.template";
                    originalClassName = "CustomTableType";
                }
                else
                {
                    // BizForm
                    className += "Item";
                    codeFile += "BizForms/";
                    templateFile += "BizFormType.template";
                    originalClassName = "BizFormType";
                }

                // Generate the code
                string code = File.ReadAllText(templateFile);

                if (fi == null)
                {
                    fi = new FormInfo(dci.ClassFormDefinition);
                }
                string propertiesCode = CodeGenerator.GetPropertiesCode(fi, false, null, null, false);

                // Replace in code
                code = code.Replace("##CLASSNAME##", dci.ClassName);
                code = code.Replace(originalClassName, className);
                code = code.Replace("// ##PROPERTIES##", propertiesCode);
                code = code.Replace("##SUMMARY##", dci.ClassDisplayName);

                codeFile += className + ".cs";

                if (file)
                {
                    File.WriteAllText(codeFile, code);
                }
                else
                {
                    // Write directly to response
                    Response.Clear();
                    Response.AddHeader("Content-Disposition", string.Format("attachment; filename=\"{0}\"", HTTPHelper.GetDispositionFilename(className) + ".cs"));
                    Response.ContentType = "text/plain";
                    Response.Write(code);
                    Response.End();
                }
            }
            else
            {
                ShowError("[FieldEditor.UpdateFormDefinition]: " + GetString("FieldEditor.ClassNotFound"));
            }
        }
        catch (Exception ex)
        {
            // Log the error silently
            EventLogProvider.LogException("FieldEditor", "CODEGEN", ex);
        }
    }


    protected void lnkHideAllFields_OnClick(object sender, EventArgs e)
    {
        // Make visible fields invisible
        ChangeFieldsVisibility(false);
    }


    protected void lnkShowAllFields_OnClick(object sender, EventArgs e)
    {
        // Make invisible fields visible
        ChangeFieldsVisibility(true);
    }


    /// <summary>
    /// Changes visible attribute of all fields in the form definition (except the primary-key field).
    /// </summary>
    /// <param name="makeVisible">Makes fields visible if true</param>
    private void ChangeFieldsVisibility(bool makeVisible)
    {
        // Raise on before definition update event
        if (OnBeforeDefinitionUpdate != null)
        {
            OnBeforeDefinitionUpdate(this, EventArgs.Empty);
        }

        LoadFormDefinition();

        // Get visible or invisible fields
        var fields = fi.GetFields(!makeVisible, makeVisible);

        foreach (FormFieldInfo field in fields)
        {
            if (!makeVisible || !field.PrimaryKey)
            {
                field.Visible = makeVisible;
            }
        }

        SaveFormDefinition();

        // Raise on after definition update event
        if (OnAfterDefinitionUpdate != null)
        {
            OnAfterDefinitionUpdate(this, EventArgs.Empty);
        }

        ClearHashtables();

        ShowChangesSaved();

        // Reload to apply visibility and inheriting changes
        Reload(lstAttributes.SelectedValue);
    }


    /// <summary>
    /// Sets properties of Reset button.
    /// </summary>
    protected void PrepareResetButton()
    {
        if (IsAlternativeForm || IsInheritedForm)
        {
            btnReset.Visible = true;
            btnReset.Enabled = false;

            if (SelectedItemType == FieldEditorSelectedItemEnum.Field)
            {
                btnReset.Text = GetString("FieldEditor.ResetField");
                btnReset.Tooltip = GetString("FieldEditor.ResetFieldTooltip");
                btnReset.OnClientClick = "return confirm('" + GetString("fieldeditor.ResetFieldToDefaultConfirmation") + "')";
            }
            else
            {
                btnReset.Text = GetString("FieldEditor.ResetCategory");
                btnReset.Tooltip = GetString("FieldEditor.ResetCategoryTooltip");
                btnReset.OnClientClick = "return confirm('" + GetString("fieldeditor.ResetCategoryToDefaultConfirmation") + "')";
            }

            if ((!String.IsNullOrEmpty(SelectedItemName)) && (!string.IsNullOrEmpty(OriginalFormDefinition)))
            {
                if ((!IsDummyField || IsDummyFieldFromMainForm) && !IsExtraField)
                {
                    // Get difference between current and original form definition
                    string diff = FormHelper.GetFormDefinitionDifference(OriginalFormDefinition, FormDefinition, true);

                    if (!string.IsNullOrEmpty(diff))
                    {
                        XmlDocument document = new XmlDocument();
                        document.LoadXml(diff);

                        // Select proper node
                        XmlNode item = document.SelectSingleNode((SelectedItemType == FieldEditorSelectedItemEnum.Field ? "//field[@column='" : "//category[@name='") + SelectedItemName + "']");
                        if (item != null)
                        {
                            // Item is modified if there are any child nodes or if there are any other attributes other than 'column' and 'order'
                            btnReset.Enabled = ((item.ChildNodes.Count > 0) || ((item.Attributes["order"] == null) && (item.Attributes.Count > 1) || (item.Attributes.Count > 2))) && Enabled;
                        }
                    }
                }
            }
        }
    }


    /// <summary>
    /// Resets form element properties to original form.
    /// </summary>
    protected void ResetFormElementToOriginal()
    {
        // Get difference between current and original form definitions
        string diff = FormHelper.GetFormDefinitionDifference(OriginalFormDefinition, FormDefinition, true);

        if (!string.IsNullOrEmpty(diff))
        {
            XmlDocument document = new XmlDocument();
            document.LoadXml(diff);

            // Select proper node
            XmlNode element = document.SelectSingleNode((SelectedItemType == FieldEditorSelectedItemEnum.Field ? "//field[@column='" : "//category[@name='") + SelectedItemName + "']");
            if (element != null)
            {
                // Check if the element is in the original definition
                if (ElementInOriginalDefinition())
                {
                    // Get 'order attribute if set
                    int order = ValidationHelper.GetInteger(XmlHelper.GetAttributeValue(element, "order"), -1);

                    // Clear the element
                    element.RemoveAll();
                    element.Attributes.RemoveAll();

                    // Append 'column' or 'name' attribute to node
                    XmlAttribute columnAttr = document.CreateAttribute(SelectedItemType == FieldEditorSelectedItemEnum.Field ? "column" : "name");
                    columnAttr.Value = SelectedItemName;
                    element.Attributes.Append(columnAttr);

                    if (order >= 0)
                    {
                        // Append 'order' attribute
                        XmlAttribute orderAttr = document.CreateAttribute("order");
                        orderAttr.Value = order.ToString();
                        element.Attributes.Append(orderAttr);
                    }
                }
                else
                {
                    // Remove corrupted element
                    document.FirstChild.RemoveChild(element);
                }

                diff = document.OuterXml;
            }
            FormDefinition = FormHelper.MergeFormDefinitions(OriginalFormDefinition, diff);

            if (OnAfterDefinitionUpdate != null)
            {
                OnAfterDefinitionUpdate(this, EventArgs.Empty);
            }
        }

        Reload(lstAttributes.SelectedValue);
        ShowChangesSaved();
    }


    /// <summary>
    /// Returns true if currently selected element (field/category) is in the original form definition.
    /// </summary>
    private bool ElementInOriginalDefinition()
    {
        // Try to find the element in the original definition
        XmlDocument origDocument = new XmlDocument();
        origDocument.LoadXml(OriginalFormDefinition);

        XmlNode origElem = origDocument.SelectSingleNode((SelectedItemType == FieldEditorSelectedItemEnum.Field ? "//field[@column='" : "//category[@name='") + SelectedItemName + "']");

        return (origElem != null);
    }


    /// <summary>
    /// Displays information messages for form elements.
    /// </summary>
    protected void DisplayFieldInformation()
    {
        bool isField = SelectedItemType == FieldEditorSelectedItemEnum.Field;

        if (IsDummyField)
        {
            AddInformation(isField ? GetString("fieldeditor.dummyfield") : GetString("fieldeditor.extracategory"));
        }

        if (IsExtraField)
        {
            AddInformation(isField ? GetString("fieldeditor.extrafield") : GetString("fieldeditor.extracategory"));
        }

        if ((!IsExtraField && (!IsDummyField || IsDummyFieldFromMainForm)) && (IsAlternativeForm || IsInheritedForm))
        {
            // Special text for system webpart(widget) properties
            if (Mode == FieldEditorModeEnum.SystemWebPartProperties)
            {
                if (btnReset.Enabled)
                {
                    AddInformation(isField ? GetString("fieldeditor.propertysystemismodified") : GetString("fieldeditor.categorysystemismodified"), " ");
                }
                else
                {
                    AddInformation(isField ? GetString("fieldeditor.propertydefaultsystemsettings") : GetString("fieldeditor.categorydefaultsystemsettings"), " ");
                }
            }
            else
            {
                AddInformation(isField ? GetString("FieldEditor.FieldIsInherited") : GetString("FieldEditor.CategoryIsInherited"), " ");

                if (btnReset.Enabled)
                {
                    AddInformation(isField ? GetString("FieldEditor.FieldIsModified") : GetString("FieldEditor.CategoryIsModified"), " ");
                }
            }
        }
    }


    /// <summary>
    /// Saves the form definition and refreshes the form.
    /// </summary>
    protected void SaveFormDefinition()
    {
        // Update form definition
        FormDefinition = fi.GetXmlDefinition();

        if ((!IsAlternativeForm) && (!IsInheritedForm))
        {
            switch (mMode)
            {
                case FieldEditorModeEnum.WebPartProperties:
                    // Save xml string to CMS_WebPart table
                    var wpi = WebPartInfoProvider.GetWebPartInfo(mWebPartId);
                    if (wpi != null)
                    {
                        wpi.WebPartProperties = FormDefinition;
                        WebPartInfoProvider.SetWebPartInfo(wpi);
                    }
                    else
                    {
                        ShowError("[FieldEditor.UpdateFormDefinition]: " + GetString("FieldEditor.WebpartNotFound"));
                    }
                    break;

                case FieldEditorModeEnum.ClassFormDefinition:
                case FieldEditorModeEnum.BizFormDefinition:
                case FieldEditorModeEnum.SystemTable:
                case FieldEditorModeEnum.CustomTable:
                    // Save xml string to CMS_Class table
                    var dci = DataClassInfoProvider.GetDataClassInfo(mClassName);
                    if (dci != null)
                    {
                        dci.ClassFormDefinition = FormDefinition;

                        using (CMSActionContext context = new CMSActionContext())
                        {
                            // Do not log synchronization for BizForm
                            if (mMode == FieldEditorModeEnum.BizFormDefinition)
                            {
                                context.DisableLogging();
                            }

                            // Save the class data
                            DataClassInfoProvider.SetDataClassInfo(dci);

                            // Update inherited classes with new fields
                            FormHelper.UpdateInheritedClasses(dci);
                        }
                    }
                    else
                    {
                        ShowError("[FieldEditor.UpdateFormDefinition]: " + GetString("FieldEditor.ClassNotFound"));
                    }
                    break;
            }
        }

        // Reload attribute list
        LoadAttributesList(lstAttributes.SelectedValue);
    }


    /// <summary>
    /// When attribute up button is clicked.
    /// </summary>
    protected void btnUpAttribute_Click(Object sender, EventArgs e)
    {
        // Raise on before definition update event
        if (OnBeforeDefinitionUpdate != null)
        {
            OnBeforeDefinitionUpdate(this, EventArgs.Empty);
        }

        if (Mode == FieldEditorModeEnum.BizFormDefinition)
        {
            // Check 'EditForm' permission
            if (!MembershipContext.AuthenticatedUser.IsAuthorizedPerResource("cms.form", "EditForm"))
            {
                RedirectToAccessDenied("cms.form", "EditForm");
            }
        }

        LoadFormDefinition();

        // First item of the attribute list cannot be moved higher
        if (string.IsNullOrEmpty(lstAttributes.SelectedValue) || (lstAttributes.SelectedIndex == 0))
        {
            return;
        }
        // 'new (not saved)' attribute cannot be moved
        else if ((SelectedItemName == newCategPreffix) || (SelectedItemName == newFieldPreffix))
        {
            ShowMessage(GetString("TemplateDesigner.AlertNewAttributeCannotBeMoved"));
            return;
        }

        if (SelectedItemType == FieldEditorSelectedItemEnum.Field)
        {
            // Move attribute up in attribute list                        
            fi.MoveFormFieldUp(SelectedItemName);
        }
        else if (SelectedItemType == FieldEditorSelectedItemEnum.Category)
        {
            // Move category up in attribute list                        
            fi.MoveFormCategoryUp(SelectedItemName);
        }

        // Update the form definition
        SaveFormDefinition();

        if (OnAfterDefinitionUpdate != null)
        {
            OnAfterDefinitionUpdate(this, EventArgs.Empty);
        }

        // Prepare Reset button and information messages
        PrepareResetButton();
        DisplayFieldInformation();
        HandleItemInheritance();
    }


    /// <summary>
    /// When attribute down button is clicked.
    /// </summary>
    protected void btnDownAttribute_Click(Object sender, EventArgs e)
    {
        // Raise on before definition update event
        if (OnBeforeDefinitionUpdate != null)
        {
            OnBeforeDefinitionUpdate(this, EventArgs.Empty);
        }

        if (Mode == FieldEditorModeEnum.BizFormDefinition)
        {
            // Check 'EditForm' permission
            if (!MembershipContext.AuthenticatedUser.IsAuthorizedPerResource("cms.form", "EditForm"))
            {
                RedirectToAccessDenied("cms.form", "EditForm");
            }
        }

        LoadFormDefinition();

        // Last item of the attribute list cannot be moved lower
        if (string.IsNullOrEmpty(lstAttributes.SelectedValue) || lstAttributes.SelectedIndex >= lstAttributes.Items.Count - 1)
        {
            return;
        }
        // 'new and not saved' attribute cannot be moved
        else if ((SelectedItemName == newCategPreffix) || (SelectedItemName == newFieldPreffix))
        {
            ShowMessage(GetString("TemplateDesigner.AlertNewAttributeCannotBeMoved"));
            return;
        }

        if (SelectedItemType == FieldEditorSelectedItemEnum.Field)
        {
            // Move attribute down in attribute list                        
            fi.MoveFormFieldDown(SelectedItemName);
        }
        else if (SelectedItemType == FieldEditorSelectedItemEnum.Category)
        {
            // Move category down in attribute list                        
            fi.MoveFormCategoryDown(SelectedItemName);
        }

        // Update the form definition
        SaveFormDefinition();

        // Raise on after definition update event
        if (OnAfterDefinitionUpdate != null)
        {
            OnAfterDefinitionUpdate(this, EventArgs.Empty);
        }

        // Prepare Reset button and information messages
        PrepareResetButton();
        DisplayFieldInformation();
        HandleItemInheritance();
    }


    /// <summary>
    /// When chkDisplayInForm checkbox checked changed.
    /// </summary>
    protected void chkDisplayInForm_CheckedChanged(Object sender, EventArgs e)
    {
        ShowOrHideFieldDetails();
    }


    /// <summary>
    /// Selected attribute changed event handler.
    /// </summary>
    protected void lstAttributes_SelectedIndexChanged(Object sender, EventArgs e)
    {
        bool isNewCreated = false;

        // Check if new attribute is edited -> select it and avoid selecting another attribute
        foreach (ListItem item in lstAttributes.Items)
        {
            switch (item.Value)
            {
                case newCategPreffix:
                    isNewCreated = true;
                    lstAttributes.SelectedValue = newCategPreffix;
                    break;

                case newFieldPreffix:
                    isNewCreated = true;
                    lstAttributes.SelectedValue = newFieldPreffix;
                    break;
            }

            if (isNewCreated)
            {
                ShowMessage(GetString("TemplateDesigner.AlertSaveNewItemOrDeleteItFirst"));
                if (IsSystemFieldSelected)
                {
                    databaseConfiguration.DisableFieldEditing(true, false);
                }
                else
                {
                    databaseConfiguration.EnableFieldEditing();
                }
                return;
            }
        }

        // Reload data
        Reload(lstAttributes.SelectedValue);
    }


    /// <summary>
    /// Show or hide details according to chkDisplayInForm checkbox is checked or not.
    /// </summary>   
    /// <param name="reload">Indicates if reload is required</param>
    private void ShowOrHideFieldDetails(bool reload = true)
    {
        // Hide or display controls because checkbox 'display in form' was checked
        FieldDetailsVisible = chkDisplayInForm.Checked;

        if (FieldDetailsVisible && reload)
        {
            Reload(lstAttributes.SelectedValue, true);
        }
    }


    /// <summary>
    /// Saves currently edited field.
    /// </summary>
    private void SaveField()
    {
        // Raise on after definition update event
        if (OnBeforeDefinitionUpdate != null)
        {
            OnBeforeDefinitionUpdate(this, EventArgs.Empty);
        }

        if ((Mode == FieldEditorModeEnum.BizFormDefinition) || (Mode == FieldEditorModeEnum.AlternativeBizFormDefinition))
        {
            // Check 'EditForm' permission
            if (!MembershipContext.AuthenticatedUser.IsAuthorizedPerResource("cms.form", "EditForm"))
            {
                RedirectToAccessDenied("cms.form", "EditForm");
            }
        }

        string errorMessage = String.Empty;

        if (SelectedItemType == FieldEditorSelectedItemEnum.Field)
        {
            errorMessage += ValidateForm();
        }

        // Check occurred errors
        if (!string.IsNullOrEmpty(errorMessage))
        {
            ShowError(errorMessage);
        }
        else
        {
            if (ValidateControlForms())
            {
                // Save selected field
                if (SaveSelectedField())
                {
                    ClearHashtables();

                    // Raise on after definition update event
                    if (OnAfterDefinitionUpdate != null)
                    {
                        OnAfterDefinitionUpdate(this, EventArgs.Empty);
                    }

                    if (!StopProcessing)
                    {
                        ShowChangesSaved();
                    }
                }
            }
        }
    }


    /// <summary>
    /// Save selected field.
    /// </summary>
    private bool SaveSelectedField()
    {
        DataClassInfo dci = null;
        bool updateInherited = false;

        // Ensure the transaction
        using (var tr = new CMSLateBoundTransaction())
        {
            // FormFieldInfo structure with data from updated form
            FormFieldInfo ffiUpdated = null;

            // FormCategoryInfo structure with data from updated form
            FormCategoryInfo fciUpdated = null;

            // Determines whether it is a new attribute (or attribute to update)
            bool isNewItem = false;
            string errorMessage = null;

            WebPartInfo wpi = null;

            // Variables for changes in DB tables
            string tableName = null;
            string newColumnDefaultValue = null; // No default value

            TableManager tm = null;

            if ((!IsAlternativeForm) && (!IsInheritedForm))
            {
                switch (mMode)
                {
                    case FieldEditorModeEnum.WebPartProperties:
                        // Fill WebPartInfo structure with data from database
                        wpi = WebPartInfoProvider.GetWebPartInfo(mWebPartId);
                        break;

                    case FieldEditorModeEnum.ClassFormDefinition:
                    case FieldEditorModeEnum.BizFormDefinition:
                    case FieldEditorModeEnum.SystemTable:
                    case FieldEditorModeEnum.CustomTable:
                        {
                            // Fill ClassInfo structure with data from database
                            dci = DataClassInfoProvider.GetDataClassInfo(mClassName);
                            if (dci != null)
                            {
                                // Set table name 
                                tableName = dci.ClassTableName;

                                tm = new TableManager(dci.ClassConnectionString);
                                tr.BeginTransaction();
                            }
                            else
                            {
                                ShowError(GetString("fieldeditor.notablename"));
                                return false;
                            }
                        }
                        break;
                }
            }

            // Load current xml form definition
            if (!LoadFormDefinition())
            {
                // Form definition was not loaded
                return false;
            }

            if (SelectedItemType == FieldEditorSelectedItemEnum.Field)
            {
                // Fill FormFieldInfo structure with original data
                ffi = fi.GetFormField(SelectedItemName);

                // Fill FormFieldInfo structure with updated form data
                ffiUpdated = FillFormFieldInfoStructure(ffi);

                bool isMacro;
                string defaultValue = ffiUpdated.GetPropertyValue(FormFieldPropertyEnum.DefaultValue, out isMacro);

                // Determine whether it is a new attribute or not
                isNewItem = (ffi == null);

                // Check if the attribute name already exists
                if (isNewItem || (!ffi.Name.EqualsCSafe(ffiUpdated.Name, StringComparison.InvariantCultureIgnoreCase)))
                {
                    columnNames = fi.GetColumnNames();
                    if (columnNames != null)
                    {
                        foreach (string colName in columnNames)
                        {
                            // If name already exists
                            if (ffiUpdated.Name.EqualsCSafe(colName, StringComparison.InvariantCultureIgnoreCase))
                            {
                                ShowError(GetString("TemplateDesigner.ErrorExistingColumnName"));
                                return false;
                            }
                        }
                    }

                    // Check column name duplicity in JOINed tables
                    if (!IsSystemFieldSelected)
                    {
                        // Check whether current column already exists in 'View_CMS_Tree_Joined'
                        if (IsDocumentType && DocumentHelper.ColumnExistsInDocumentView(ffiUpdated.Name))
                        {
                            ShowError(GetString("TemplateDesigner.ErrorExistingColumnInJoinedTable"));
                            return false;
                        }

                        // Check whether current column is unique in tables used to create views - applied only for system tables
                        if ((Mode == FieldEditorModeEnum.SystemTable) && FormHelper.ColumnExistsInView(mClassName, ffiUpdated.Name))
                        {
                            ShowError(GetString("TemplateDesigner.ErrorExistingColumnInJoinedTable"));
                            return false;
                        }
                    }
                }

                // New node                
                string newColumnName;
                bool newColumnAllowNull;
                string newColumnType;
                if (isNewItem)
                {
                    ffiUpdated.PrimaryKey = IsPrimaryField;
                    newColumnName = ffiUpdated.Name;
                    newColumnAllowNull = ffiUpdated.AllowEmpty;

                    // Set implicit default value
                    if (!(newColumnAllowNull) && (string.IsNullOrEmpty(defaultValue)))
                    {
                        if (!DevelopmentMode)
                        {
                            switch (ffiUpdated.DataType)
                            {
                                case FieldDataType.Integer:
                                case FieldDataType.LongInteger:
                                case FieldDataType.Double:
                                case FieldDataType.Boolean:
                                    newColumnDefaultValue = "0";
                                    break;

                                case FieldDataType.Text:
                                case FieldDataType.LongText:
                                case FieldDataType.DocAttachments:
                                    newColumnDefaultValue = string.Empty;
                                    break;

                                case FieldDataType.DateTime:
                                    newColumnDefaultValue = DataTypeManager.MIN_DATETIME.ToString();
                                    break;

                                case FieldDataType.File:
                                case FieldDataType.Guid:
                                    // 32 digits, empty Guid
                                    newColumnDefaultValue = Guid.Empty.ToString();
                                    break;

                                case FieldDataType.Binary:
                                    break;
                            }
                        }
                    }
                    // Check if default value is in required format
                    else if (!string.IsNullOrEmpty(defaultValue))
                    {
                        // If default value is macro, don't try to ensure the type
                        if (!isMacro)
                        {
                            switch (ffiUpdated.DataType)
                            {
                                case FieldDataType.Integer:
                                    try
                                    {
                                        int i = Int32.Parse(defaultValue);
                                        newColumnDefaultValue = i.ToString();
                                    }
                                    catch
                                    {
                                        newColumnDefaultValue = "0";
                                        errorMessage = GetString("TemplateDesigner.ErrorDefaultValueInteger");
                                    }
                                    break;

                                case FieldDataType.LongInteger:
                                    try
                                    {
                                        long longInt = long.Parse(defaultValue);
                                        newColumnDefaultValue = longInt.ToString();
                                    }
                                    catch
                                    {
                                        newColumnDefaultValue = "0";
                                        errorMessage = GetString("TemplateDesigner.ErrorDefaultValueLongInteger");
                                    }
                                    break;

                                case FieldDataType.Double:
                                    if (ValidationHelper.IsDouble(defaultValue))
                                    {
                                        newColumnDefaultValue = FormHelper.GetDoubleValueInDBCulture(defaultValue);
                                    }
                                    else
                                    {
                                        newColumnDefaultValue = "0";
                                        errorMessage = GetString("TemplateDesigner.ErrorDefaultValueDouble");
                                    }
                                    break;

                                case FieldDataType.DateTime:
                                    if (DateTimeHelper.IsNowOrToday(defaultValue))
                                    {
                                        newColumnDefaultValue = defaultValue;
                                    }
                                    else
                                    {
                                        try
                                        {
                                            DateTime dat = DateTime.Parse(defaultValue);
                                            newColumnDefaultValue = dat.ToString();
                                        }
                                        catch
                                        {
                                            newColumnDefaultValue = DateTime.Now.ToString();
                                            errorMessage = GetString("TemplateDesigner.ErrorDefaultValueDateTime");
                                        }
                                    }
                                    break;

                                case FieldDataType.File:
                                case FieldDataType.Guid:
                                    try
                                    {
                                        Guid g = new Guid(defaultValue);
                                        newColumnDefaultValue = g.ToString();
                                    }
                                    catch
                                    {
                                        newColumnDefaultValue = Guid.Empty.ToString();
                                        errorMessage = GetString("TemplateDesigner.ErrorDefaultValueGuid");
                                    }
                                    break;

                                case FieldDataType.LongText:
                                case FieldDataType.Text:
                                case FieldDataType.Boolean:
                                    newColumnDefaultValue = defaultValue;
                                    break;
                            }
                        }
                    }

                    // Set column type and size
                    newColumnType = DataTypeManager.GetSqlType(ffiUpdated.DataType, ffiUpdated.Size, ffiUpdated.Precision);

                    if (string.IsNullOrEmpty(errorMessage))
                    {
                        if ((!IsAlternativeForm) && (!IsInheritedForm) && (!ffiUpdated.IsDummyField) && (!ffiUpdated.IsExtraField))
                        {
                            switch (mMode)
                            {
                                case FieldEditorModeEnum.ClassFormDefinition:
                                case FieldEditorModeEnum.BizFormDefinition:
                                case FieldEditorModeEnum.SystemTable:
                                case FieldEditorModeEnum.CustomTable:

                                    // Add new column to specified table  
                                    try
                                    {
                                        string newDBDefaultValue;

                                        // Check if it is not a macro
                                        if (isMacro)
                                        {
                                            newDBDefaultValue = newColumnDefaultValue;
                                        }
                                        else
                                        {
                                            switch (ffiUpdated.DataType)
                                            {
                                                case FieldDataType.Double:
                                                    newDBDefaultValue = FormHelper.GetDoubleValueInDBCulture(newColumnDefaultValue);
                                                    break;

                                                case FieldDataType.DateTime:
                                                    newDBDefaultValue = FormHelper.GetDateTimeValueInDBCulture(newColumnDefaultValue);
                                                    break;
                                                default:
                                                    newDBDefaultValue = newColumnDefaultValue;
                                                    break;
                                            }
                                        }

                                        if (!ffiUpdated.External)
                                        {
                                            if (DevelopmentMode)
                                            {
                                                if (tm != null)
                                                {
                                                    tm.AddTableColumn(tableName, newColumnName, newColumnType, newColumnAllowNull, newDBDefaultValue, false);
                                                }
                                            }
                                            else
                                            {
                                                if (tm != null)
                                                {
                                                    tm.AddTableColumn(tableName, newColumnName, newColumnType, newColumnAllowNull, newDBDefaultValue);
                                                }
                                            }

                                            // Recreate the table PK constraint
                                            if (IsPrimaryField)
                                            {
                                                int pos = 0;
                                                var pkFields = fi.GetFields(true, true, true, true);
                                                string[] primaryKeys = new string[pkFields.Count() + 1];
                                                foreach (FormFieldInfo pk in pkFields)
                                                {
                                                    if (pk != null)
                                                    {
                                                        primaryKeys[pos++] = "[" + pk.Name + "]";
                                                    }
                                                }
                                                primaryKeys[pos] = "[" + newColumnName + "]";
                                                if (tm != null)
                                                {
                                                    tm.RecreatePKConstraint(tableName, primaryKeys);
                                                }
                                            }
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        EventLogProvider.LogException("FieldEditor", "SAVE", ex);

                                        ShowError(GetString("general.saveerror"), ex.Message);
                                        return false;
                                    }

                                    break;
                            }
                        }
                    }
                    // An error has occurred
                    else
                    {
                        ShowError(errorMessage);
                        return false;
                    }
                }
                // Existing node
                else
                {
                    // Get info whether it is a primary key or system field
                    ffiUpdated.PrimaryKey = ffi.PrimaryKey;

                    // If attribute is a primary key
                    if (ffi.PrimaryKey)
                    {
                        // Check if the attribute type is integer number
                        if (ffiUpdated.DataType != FieldDataType.Integer)
                        {
                            errorMessage += GetString("TemplateDesigner.ErrorPKNotInteger") + " ";
                        }

                        // Check if allow empty is disabled
                        if (ffiUpdated.AllowEmpty)
                        {
                            errorMessage += GetString("TemplateDesigner.ErrorPKAllowsNulls") + " ";
                        }

                        // Check that the field type is label
                        string labelControlName = Enum.GetName(typeof(FormFieldControlTypeEnum), FormFieldControlTypeEnum.LabelControl).ToLowerCSafe();
                        if ((ffiUpdated.FieldType != FormFieldControlTypeEnum.LabelControl) && ((ffiUpdated.FieldType != FormFieldControlTypeEnum.CustomUserControl) && (!ffiUpdated.Settings["controlname"].ToString().EqualsCSafe(labelControlName, StringComparison.InvariantCultureIgnoreCase))))
                        {
                            errorMessage += GetString("TemplateDesigner.ErrorPKisNotLabel") + " ";
                        }

                        // An error has occurred
                        if (!string.IsNullOrEmpty(errorMessage))
                        {
                            ShowError(GetString("TemplateDesigner.ErrorPKThisIsPK") + " " + errorMessage);
                            return false;
                        }
                    }

                    bool originalIsMacro;
                    string originalDefaultValue = ffi.GetPropertyValue(FormFieldPropertyEnum.DefaultValue, out originalIsMacro);

                    // Reset empty default values
                    if (String.IsNullOrEmpty(defaultValue))
                    {
                        defaultValue = null;
                    }
                    if (String.IsNullOrEmpty(originalDefaultValue))
                    {
                        originalDefaultValue = null;
                    }

                    // If table column update is needed
                    if (((ffi.PrimaryKey) && (ffi.Name != ffiUpdated.Name)) ||
                        ((!ffi.PrimaryKey) &&
                         ((ffi.Name != ffiUpdated.Name) ||
                          (ffi.DataType != ffiUpdated.DataType) ||
                          (ffi.AllowEmpty != ffiUpdated.AllowEmpty) ||
                          (ffi.Size != ffiUpdated.Size) ||
                          ((originalDefaultValue != defaultValue) || (originalIsMacro != isMacro) || (ffiUpdated.DataType == FieldDataType.Double)))
                        )
                        )
                    {
                        // Set variables needed for changes in DB                
                        string oldColumnName = ffi.Name;
                        newColumnName = ffiUpdated.Name;
                        newColumnAllowNull = ffiUpdated.AllowEmpty;

                        // Set implicit default value
                        if (!(newColumnAllowNull) && (string.IsNullOrEmpty(defaultValue)))
                        {
                            switch (ffiUpdated.DataType)
                            {
                                case FieldDataType.Integer:
                                case FieldDataType.LongInteger:
                                case FieldDataType.Double:
                                case FieldDataType.Boolean:
                                    newColumnDefaultValue = "0";
                                    break;

                                case FieldDataType.Text:
                                case FieldDataType.LongText:
                                    newColumnDefaultValue = string.Empty;
                                    break;

                                case FieldDataType.DateTime:
                                    newColumnDefaultValue = DateTime.Now.ToString();
                                    break;

                                case FieldDataType.File:
                                case FieldDataType.Guid:
                                    // 32 digits, empty Guid
                                    newColumnDefaultValue = Guid.Empty.ToString();
                                    break;

                                case FieldDataType.Binary:
                                    break;
                            }
                        }
                        // Check if default value is in required format
                        else if (!string.IsNullOrEmpty(defaultValue))
                        {
                            // If default value is macro, don't try to ensure the type
                            if (!isMacro)
                            {
                                switch (ffiUpdated.DataType)
                                {
                                    case FieldDataType.Integer:
                                        try
                                        {
                                            int i = Int32.Parse(defaultValue);
                                            newColumnDefaultValue = i.ToString();
                                        }
                                        catch
                                        {
                                            newColumnDefaultValue = "0";
                                            errorMessage = GetString("TemplateDesigner.ErrorDefaultValueInteger");
                                        }
                                        break;


                                    case FieldDataType.LongInteger:
                                        try
                                        {
                                            long longInt = long.Parse(defaultValue);
                                            newColumnDefaultValue = longInt.ToString();
                                        }
                                        catch
                                        {
                                            newColumnDefaultValue = "0";
                                            errorMessage = GetString("TemplateDesigner.ErrorDefaultValueLongInteger");
                                        }
                                        break;

                                    case FieldDataType.Double:
                                        if (ValidationHelper.IsDouble(defaultValue))
                                        {
                                            newColumnDefaultValue = FormHelper.GetDoubleValueInDBCulture(defaultValue);
                                        }
                                        else
                                        {
                                            newColumnDefaultValue = "0";
                                            errorMessage = GetString("TemplateDesigner.ErrorDefaultValueDouble");
                                        }
                                        break;

                                    case FieldDataType.DateTime:
                                        if (DateTimeHelper.IsNowOrToday(defaultValue))
                                        {
                                            newColumnDefaultValue = defaultValue;
                                        }
                                        else
                                        {
                                            try
                                            {
                                                DateTime dat = DateTime.Parse(defaultValue);
                                                newColumnDefaultValue = dat.ToString();
                                            }
                                            catch
                                            {
                                                newColumnDefaultValue = DateTime.Now.ToString();
                                                errorMessage = GetString("TemplateDesigner.ErrorDefaultValueDateTime");
                                            }
                                        }
                                        break;

                                    case FieldDataType.File:
                                    case FieldDataType.Guid:
                                        try
                                        {
                                            Guid g = new Guid(defaultValue);
                                            newColumnDefaultValue = g.ToString();
                                        }
                                        catch
                                        {
                                            newColumnDefaultValue = Guid.Empty.ToString();
                                            errorMessage = GetString("TemplateDesigner.ErrorDefaultValueGuid");
                                        }
                                        break;

                                    case FieldDataType.LongText:
                                    case FieldDataType.Text:
                                    case FieldDataType.Boolean:

                                        newColumnDefaultValue = defaultValue;
                                        break;
                                }
                            }
                        }

                        // Set column type and size
                        newColumnType = DataTypeManager.GetSqlType(ffiUpdated.DataType, ffiUpdated.Size, ffiUpdated.Precision);

                        if (string.IsNullOrEmpty(errorMessage))
                        {
                            if ((!IsAlternativeForm) && (!IsInheritedForm) && (!ffiUpdated.IsDummyField) && (!ffiUpdated.IsExtraField))
                            {
                                switch (mMode)
                                {
                                    case FieldEditorModeEnum.ClassFormDefinition:
                                    case FieldEditorModeEnum.BizFormDefinition:
                                    case FieldEditorModeEnum.SystemTable:
                                    case FieldEditorModeEnum.CustomTable:

                                        try
                                        {
                                            string newDBDefaultValue;

                                            // Check if it is not a macro
                                            if (isMacro)
                                            {
                                                newDBDefaultValue = newColumnDefaultValue;
                                            }
                                            else
                                            {
                                                switch (ffiUpdated.DataType)
                                                {
                                                    case FieldDataType.Double:
                                                        newDBDefaultValue = FormHelper.GetDoubleValueInDBCulture(newColumnDefaultValue);
                                                        break;

                                                    case FieldDataType.DateTime:
                                                        newDBDefaultValue = FormHelper.GetDateTimeValueInDBCulture(newColumnDefaultValue);
                                                        break;
                                                    default:
                                                        newDBDefaultValue = newColumnDefaultValue;
                                                        break;
                                                }
                                            }

                                            if (ffiUpdated.External)
                                            {
                                                if (!ffi.External)
                                                {
                                                    // Drop old column from table
                                                    if (tm != null)
                                                    {
                                                        tm.DropTableColumn(tableName, ffi.Name);
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                if (ffi.External)
                                                {
                                                    // Add table column
                                                    if (tm != null)
                                                    {
                                                        tm.AddTableColumn(tableName, newColumnName, newColumnType, newColumnAllowNull, newDBDefaultValue);
                                                    }
                                                }
                                                else
                                                {
                                                    // Change table column
                                                    tm.AlterTableColumn(tableName, oldColumnName, newColumnName, newColumnType, newColumnAllowNull, newDBDefaultValue);

                                                    if (!oldColumnName.EqualsCSafe(newColumnName) && (OnFieldNameChanged != null))
                                                    {
                                                        OnFieldNameChanged(this, oldColumnName, newColumnName);
                                                    }
                                                }
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            EventLogProvider.LogException("FieldEditor", "SAVE", ex);

                                            // User friendly message for not null setting of column
                                            if (ffi.AllowEmpty && !newColumnAllowNull)
                                            {
                                                ShowError(GetString("FieldEditor.ColumnNotAcceptNull"), ex.Message);
                                            }
                                            else
                                            {
                                                ShowError(GetString("general.saveerror"), ex.Message);
                                            }
                                            return false;
                                        }

                                        break;
                                }
                            }
                            else
                            {
                                if (!oldColumnName.EqualsCSafe(newColumnName) && (OnFieldNameChanged != null))
                                {
                                    OnFieldNameChanged(this, oldColumnName, newColumnName);
                                }
                            }
                        }
                        // An error has occurred
                        else
                        {
                            ShowError(errorMessage);
                            return false;
                        }
                    } // End update needed
                } // End existing node

                // Insert new field
                if (isNewItem)
                {
                    InsertFormItem(ffiUpdated);

                    if (!IsAlternativeForm && !IsInheritedForm)
                    {
                        // Hide field for alternative forms that require it
                        dci = DataClassInfoProvider.GetDataClassInfo(mClassName);
                        if (dci != null)
                        {
                            string where = "FormClassID=" + dci.ClassID;

                            // If edited class is user settings, update user alt.forms which are combined with user settings too
                            if (mClassName.EqualsCSafe(UserSettingsInfo.OBJECT_TYPE, StringComparison.InvariantCultureIgnoreCase))
                            {
                                DataClassInfo userClass = DataClassInfoProvider.GetDataClassInfo(UserInfo.OBJECT_TYPE);
                                if (userClass != null)
                                {
                                    where = SqlHelper.AddWhereCondition(where, "FormClassID=" + userClass.ClassID + " AND FormCoupledClassID=" + dci.ClassID, "OR");
                                }
                            }

                            where = SqlHelper.AddWhereCondition(where, "FormHideNewParentFields=1");

                            var altforms = AlternativeFormInfoProvider.GetAlternativeForms(where, null);
                            foreach (AlternativeFormInfo afi in altforms)
                            {
                                afi.HideField(ffiUpdated);
                                AlternativeFormInfoProvider.SetAlternativeFormInfo(afi);
                            }
                        }
                    }
                }
                // Update current field
                else
                {
                    fi.UpdateFormField(ffi.Name, ffiUpdated);
                }
            }
            else if (SelectedItemType == FieldEditorSelectedItemEnum.Category)
            {
                // Fill FormCategoryInfo structure with original data
                fci = fi.GetFormCategory(SelectedItemName);
                // Determine whether it is a new attribute or not
                isNewItem = (fci == null);

                // Fill FormCategoryInfo structure with updated form data
                fciUpdated = new FormCategoryInfo();
                categoryEdit.CategoryInfo = fciUpdated;
                categoryEdit.Save();

                fciUpdated.IsDummy = IsDummyField;
                fciUpdated.IsExtra = IsExtraField;

                // Check if the category caption is empty
                if (string.IsNullOrEmpty(fciUpdated.GetPropertyValue(FormCategoryPropertyEnum.Caption)))
                {
                    ShowError(GetString("TemplateDesigner.ErrorCategoryNameEmpty"));
                    return false;
                }

                // Use codenamed category caption for name attribute
                fciUpdated.CategoryName = !isNewItem ? fci.CategoryName : ValidationHelper.GetCodeName(fciUpdated.GetPropertyValue(FormCategoryPropertyEnum.Caption));

                // Get form category names
                if ((isNewItem || fciUpdated.CategoryName != fci.CategoryName) && fi.GetCategoryNames().Exists(x => x == fciUpdated.CategoryName))
                {
                    ShowError(GetString("TemplateDesigner.ErrorExistingCategoryName"));
                    return false;
                }

                if (isNewItem)
                {
                    // Insert new category
                    InsertFormItem(fciUpdated);
                }
                else
                {
                    // Update current 
                    fi.UpdateFormCategory(fci.CategoryName, fciUpdated);
                }
            }

            // Make changes in database
            if (SelectedItemType != 0)
            {
                // Get updated definition
                FormDefinition = fi.GetXmlDefinition();

                string error = null;

                if (!IsAlternativeForm && !IsInheritedForm)
                {
                    switch (mMode)
                    {
                        case FieldEditorModeEnum.WebPartProperties:
                            if (wpi != null)
                            {
                                // Update xml definition
                                wpi.WebPartProperties = FormDefinition;

                                try
                                {
                                    WebPartInfoProvider.SetWebPartInfo(wpi);
                                }
                                catch (Exception ex)
                                {
                                    EventLogProvider.LogException("FieldEditor", "SAVE", ex);
                                    error = ex.Message;
                                }
                            }
                            else
                            {
                                error = GetString("FieldEditor.WebpartNotFound");
                            }
                            break;

                        case FieldEditorModeEnum.ClassFormDefinition:
                        case FieldEditorModeEnum.BizFormDefinition:
                        case FieldEditorModeEnum.SystemTable:
                        case FieldEditorModeEnum.CustomTable:
                            if (dci != null)
                            {
                                // Update xml definition
                                dci.ClassFormDefinition = FormDefinition;

                                // When updating existing field
                                if (ffi != null)
                                {
                                    // Update ClassNodeNameSource field
                                    if (dci.ClassNodeNameSource == ffi.Name)
                                    {
                                        dci.ClassNodeNameSource = ffiUpdated.Name;
                                    }
                                }

                                if (!((SelectedItemType == FieldEditorSelectedItemEnum.Field) && ffiUpdated.IsDummyField))
                                {
                                    // Update xml schema
                                    dci.ClassXmlSchema = tm.GetXmlSchema(dci.ClassTableName);
                                }

                                // Update changes in DB
                                try
                                {
                                    // Save the data class
                                    DataClassInfoProvider.SetDataClassInfo(dci);

                                    // Generate the class code - Temporarily disabled
                                    //GenerateCode(true);

                                    updateInherited = true;
                                }
                                catch (Exception ex)
                                {
                                    EventLogProvider.LogException("FieldEditor", "SAVE", ex);
                                    error = ex.Message;
                                }

                                if (!((SelectedItemType == FieldEditorSelectedItemEnum.Field) && ffiUpdated.IsDummyField))
                                {
                                    bool fieldType = (SelectedItemType == FieldEditorSelectedItemEnum.Field);
                                    if (fieldType)
                                    {
                                        // Generate default view
                                        SqlGenerator.GenerateDefaultView(dci, mMode == FieldEditorModeEnum.BizFormDefinition ? SiteContext.CurrentSiteName : null);

                                        QueryInfoProvider.ClearDefaultQueries(dci, true, true);
                                    }

                                    // Updates custom views
                                    if (((mMode == FieldEditorModeEnum.SystemTable) || (mMode == FieldEditorModeEnum.ClassFormDefinition)))
                                    {
                                        // Refresh views only if needed
                                        bool refreshViews = (fci == null);
                                        if ((ffi != null) && (ffiUpdated != null))
                                        {
                                            bool isMacro;
                                            string defValue = ffiUpdated.GetPropertyValue(FormFieldPropertyEnum.DefaultValue, out isMacro);

                                            bool origIsMacro;
                                            string origDefValue = ffi.GetPropertyValue(FormFieldPropertyEnum.DefaultValue, out origIsMacro);

                                            refreshViews =
                                                (ffi.Name != ffiUpdated.Name) ||
                                                (ffi.DataType != ffiUpdated.DataType) ||
                                                (ffi.Size != ffiUpdated.Size) ||
                                                (ffi.AllowEmpty != ffiUpdated.AllowEmpty) ||
                                                ((origDefValue != defValue) || (origIsMacro != isMacro));
                                        }

                                        if (refreshViews)
                                        {
                                            try
                                            {
                                                tm.RefreshCustomViews(dci.ClassTableName);

                                                if (dci.ClassName.EqualsCSafe("cms.document", StringComparison.InvariantCultureIgnoreCase) || dci.ClassName.EqualsCSafe("cms.tree", StringComparison.InvariantCultureIgnoreCase))
                                                {
                                                    tm.RefreshDocumentViews();
                                                }
                                            }
                                            catch (Exception ex)
                                            {
                                                EventLogProvider.LogException("FieldEditor", "REFRESHVIEWS", ex);
                                                error = ResHelper.GetString("fieldeditor.refreshingviewsfailed");
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                error = GetString("FieldEditor.ClassNotFound");
                            }
                            break;
                    }
                }

                if (!string.IsNullOrEmpty(error))
                {
                    ShowError("[FieldEditor.SaveSelectedField()]: " + error);
                    return false;
                }

                IsNewItemEdited = false;

                if (SelectedItemType == FieldEditorSelectedItemEnum.Category)
                {
                    Reload(categPreffix + fciUpdated.CategoryName);
                }
                else if (SelectedItemType == FieldEditorSelectedItemEnum.Field)
                {
                    Reload(fieldPreffix + ffiUpdated.Name);
                }
            }

            // All done and new item, fire OnFieldCreated  event
            if (isNewItem && (ffiUpdated != null))
            {
                RaiseOnFieldCreated(ffiUpdated);
            }

            // Commit the transaction
            tr.Commit();
        }

        // Update inherited classes with new fields
        if (updateInherited)
        {
            FormHelper.UpdateInheritedClasses(dci);
        }

        return true;
    }


    /// <summary>
    /// New category button clicked.
    /// </summary>
    protected void btnNewCategory_Click(Object sender, EventArgs e)
    {
        if (Mode == FieldEditorModeEnum.BizFormDefinition)
        {
            // Check 'EditForm' permission
            if (!MembershipContext.AuthenticatedUser.IsAuthorizedPerResource("cms.form", "EditForm"))
            {
                RedirectToAccessDenied("cms.form", "EditForm");
            }
        }

        IsDummyField = IsAlternativeForm;
        IsExtraField = IsInheritedForm;

        if (IsNewItemEdited)
        {
            // Display error - Only one new item can be edited
            ShowMessage(GetString("TemplateDesigner.ErrorCannotCreateAnotherNewItem"));
        }
        else
        {
            HandleInherited(false);

            // Create #categ##new# item in list
            ListItem newListItem = new ListItem(GetString("TemplateDesigner.NewCategory"), newCategPreffix);

            if ((lstAttributes.Items.Count > 0) && (lstAttributes.SelectedIndex >= 0))
            {
                // Add behind the selected item 
                lstAttributes.Items.Insert(lstAttributes.SelectedIndex + 1, newListItem);
            }
            else
            {
                // Add at the end of the item collection
                lstAttributes.Items.Add(newListItem);
            }

            // Select new item 
            lstAttributes.SelectedIndex = lstAttributes.Items.IndexOf(newListItem);

            SelectedItemType = FieldEditorSelectedItemEnum.Category;
            SelectedItemName = newCategPreffix;

            LoadDefaultCategoryEditForm();

            IsNewItemEdited = true;
        }

        PrepareResetButton();
    }

    /// <summary>
    /// New attribute button clicked.
    /// </summary>
    protected void btnNewAttribute_Click(Object sender, EventArgs e)
    {
        NewAttribute();
    }


    /// <summary>
    /// Creates new attribute.
    /// </summary>
    private void NewAttribute()
    {
        if (Mode == FieldEditorModeEnum.BizFormDefinition)
        {
            // Check 'EditForm' permission
            if (!MembershipContext.AuthenticatedUser.IsAuthorizedPerResource("cms.form", "EditForm"))
            {
                RedirectToAccessDenied("cms.form", "EditForm");
            }
        }

        if (IsNewItemEdited)
        {
            // Only one new item can be edited
            ShowMessage(GetString("TemplateDesigner.ErrorCannotCreateAnotherNewItem"));
            return;
        }

        IsNewItemEdited = true;

        HandleInherited(false);

        FieldDetailsVisible = true;

        // Set field type
        fieldTypeSelector.Reload();
        SetFieldType();

        // Clear settings
        controlSettings.Settings = new Hashtable();
        controlSettings.BasicForm.Mode = FormModeEnum.Insert;

        // Create #new# attribute in attribute list
        ListItem newListItem = new ListItem(GetString("TemplateDesigner.NewAttribute"), newFieldPreffix);

        if ((lstAttributes.Items.Count > 0) && (lstAttributes.SelectedIndex >= 0))
        {
            // Add behind the selected item 
            lstAttributes.Items.Insert(lstAttributes.SelectedIndex + 1, newListItem);
        }
        else
        {
            // Add at the end of the item collection
            lstAttributes.Items.Add(newListItem);
        }

        // Select new item 
        lstAttributes.SelectedIndex = lstAttributes.Items.IndexOf(newListItem);

        // Get type of previously selected item
        FieldEditorSelectedItemEnum oldItemType = SelectedItemType;

        // Initialize currently selected item type and name
        SelectedItemType = FieldEditorSelectedItemEnum.Field;
        SelectedItemName = newFieldPreffix;

        databaseConfiguration.AttributeName = string.Empty;

        bool newItemBlank = ValidationHelper.GetBoolean(SettingsHelper.AppSettings["CMSClearFieldEditor"], true);
        if (newItemBlank || (oldItemType != FieldEditorSelectedItemEnum.Field))
        {
            LoadDefaultAttributeEditForm(false);
        }

        LoadControlSettings();

        DisplaySelectedTabContent();

        PrepareResetButton();
    }


    private void SetFieldType()
    {
        IsDummyField = false;
        IsDummyFieldFromMainForm = false;
        IsExtraField = false;
        IsPrimaryField = false;
        IsSystemField = false;

        switch (fieldTypeSelector.SelectedFieldType)
        {
            case FieldTypeEnum.Dummy:
                IsDummyField = true;
                IsDummyFieldFromMainForm = !(IsAlternativeForm || IsInheritedForm);
                break;

            case FieldTypeEnum.Document:
                IsSystemField = true;
                break;

            case FieldTypeEnum.Extra:
                IsExtraField = true;
                break;

            case FieldTypeEnum.Primary:
                IsPrimaryField = true;
                break;
        }
    }

    /// <summary>
    /// Gets available controls.
    /// </summary>
    /// <returns>Returns FieldEditorControlsEnum</returns>
    private static FieldEditorControlsEnum GetControls(FieldEditorControlsEnum DisplayedControls, FieldEditorModeEnum Mode, bool DevelopmentMode)
    {
        FieldEditorControlsEnum controls = FieldEditorControlsEnum.None;

        // Get displayed controls
        if (DisplayedControls == FieldEditorControlsEnum.ModeSelected)
        {
            switch (Mode)
            {
                case FieldEditorModeEnum.BizFormDefinition:
                case FieldEditorModeEnum.AlternativeBizFormDefinition:
                    controls = FieldEditorControlsEnum.Bizforms;
                    break;

                case FieldEditorModeEnum.ClassFormDefinition:
                case FieldEditorModeEnum.AlternativeClassFormDefinition:
                    controls = DevelopmentMode ? FieldEditorControlsEnum.All : FieldEditorControlsEnum.DocumentTypes;
                    break;

                case FieldEditorModeEnum.SystemTable:
                case FieldEditorModeEnum.AlternativeSystemTable:
                    controls = FieldEditorControlsEnum.SystemTables;
                    break;

                case FieldEditorModeEnum.CustomTable:
                case FieldEditorModeEnum.AlternativeCustomTable:
                    controls = FieldEditorControlsEnum.CustomTables;
                    break;

                case FieldEditorModeEnum.WebPartProperties:
                case FieldEditorModeEnum.Widget:
                case FieldEditorModeEnum.InheritedWebPartProperties:
                case FieldEditorModeEnum.SystemWebPartProperties:
                    controls = FieldEditorControlsEnum.Controls;
                    break;

                case FieldEditorModeEnum.General:
                case FieldEditorModeEnum.FormControls:
                case FieldEditorModeEnum.ProcessActions:
                case FieldEditorModeEnum.InheritedFormControl:
                case FieldEditorModeEnum.PageTemplateProperties:
                    controls = FieldEditorControlsEnum.All;
                    break;
            }
        }
        else
        {
            controls = DisplayedControls;
        }

        return controls;
    }


    /// <summary>
    /// Delete attribute button clicked.
    /// </summary>
    protected void btnDeleteItem_Click(Object sender, EventArgs e)
    {
        DataClassInfo dci = null;
        bool updateInherited = false;

        // Ensure the transaction
        using (var tr = new CMSLateBoundTransaction())
        {
            if (Mode == FieldEditorModeEnum.BizFormDefinition)
            {
                // Check 'EditForm' permission
                if (!MembershipContext.AuthenticatedUser.IsAuthorizedPerResource("cms.form", "EditForm"))
                {
                    RedirectToAccessDenied("cms.form", "EditForm");
                }
            }

            // Raise on before definition update event
            if (OnBeforeDefinitionUpdate != null)
            {
                OnBeforeDefinitionUpdate(this, EventArgs.Empty);
            }

            WebPartInfo wpi = null;

            string errorMessage = null;
            string newSelectedValue = null;
            string deletedItemPreffix = null;

            // Clear settings
            controlSettings.Settings = new Hashtable();
            controlSettings.BasicForm.Mode = FormModeEnum.Insert;

            TableManager tm = null;

            switch (mMode)
            {
                case FieldEditorModeEnum.WebPartProperties:
                    // Get the web part
                    {
                        wpi = WebPartInfoProvider.GetWebPartInfo(mWebPartId);
                        if (wpi == null)
                        {
                            ShowError(GetString("FieldEditor.WebpartNotFound"));
                            return;
                        }
                    }
                    break;

                case FieldEditorModeEnum.ClassFormDefinition:
                case FieldEditorModeEnum.BizFormDefinition:
                case FieldEditorModeEnum.SystemTable:
                case FieldEditorModeEnum.CustomTable:
                    {
                        // Get the DataClass
                        dci = DataClassInfoProvider.GetDataClassInfo(mClassName);
                        if (dci == null)
                        {
                            ShowError(GetString("FieldEditor.ClassNotFound"));
                            return;
                        }

                        tm = new TableManager(dci.ClassConnectionString);
                        tr.BeginTransaction();
                    }
                    break;
            }

            // Load current xml form definition
            LoadFormDefinition();

            if ((!string.IsNullOrEmpty(SelectedItemName)) && (!IsNewItemEdited))
            {
                if (SelectedItemType == FieldEditorSelectedItemEnum.Field)
                {
                    FormFieldInfo ffiSelected = fi.GetFormField(SelectedItemName);
                    deletedItemPreffix = fieldPreffix;

                    if (ffiSelected != null)
                    {
                        // Do not allow deleting of the primary key except for external fields
                        if (ffiSelected.PrimaryKey && !ffiSelected.External)
                        {
                            if (!DevelopmentMode)
                            {
                                ShowError(GetString("TemplateDesigner.ErrorCannotDeletePK"));
                                return;
                            }
                            else
                            {
                                // Check if at least one primary key stays
                                if (fi.GetFields(true, true, false, true).Count() < 2)
                                {
                                    ShowError(GetString("TemplateDesigner.ErrorCannotDeletePK"));
                                    return;
                                }
                            }
                        }

                        // Check if at least two fields stay in document type definition
                        if ((Mode == FieldEditorModeEnum.ClassFormDefinition) &&
                           (fi.GetFields(true, true, true, false, false).Count() < 3) && (!ffiSelected.IsDummyField))
                        {
                            ShowError(GetString("TemplateDesigner.ErrorCannotDeleteAllCustomFields"));
                            return;
                        }

                        // Do not allow deleting of the system field
                        if (ffiSelected.System && !ffiSelected.External && !DevelopmentMode)
                        {
                            ShowError(GetString("TemplateDesigner.ErrorCannotDeleteSystemField"));
                            return;
                        }

                        // Remove specific field from xml form definition
                        fi.RemoveFormField(SelectedItemName);

                        // Get updated definition
                        FormDefinition = fi.GetXmlDefinition();

                        switch (mMode)
                        {
                            case FieldEditorModeEnum.WebPartProperties:
                                // Web part properties
                                {
                                    if (wpi != null)
                                    {
                                        wpi.WebPartProperties = FormDefinition;
                                        try
                                        {
                                            WebPartInfoProvider.SetWebPartInfo(wpi);
                                        }
                                        catch (Exception ex)
                                        {
                                            EventLogProvider.LogException("FieldEditor", "SAVE", ex);
                                            errorMessage = ex.Message;
                                        }
                                    }
                                }
                                break;

                            case FieldEditorModeEnum.ClassFormDefinition:
                            case FieldEditorModeEnum.BizFormDefinition:
                            case FieldEditorModeEnum.SystemTable:
                            case FieldEditorModeEnum.CustomTable:
                                {
                                    // If document type is edited AND field that should be removed is FILE
                                    if (IsDocumentType && (mMode == FieldEditorModeEnum.ClassFormDefinition) && (!string.IsNullOrEmpty(ClassName)) &&
                                        (ffiSelected.DataType == FieldDataType.File))
                                    {
                                        DocumentHelper.DeleteDocumentAttachments(ClassName, ffiSelected.Name, null);
                                    }

                                    // If bizform is edited AND field that should be removed is FILE
                                    if ((mMode == FieldEditorModeEnum.BizFormDefinition) && (!string.IsNullOrEmpty(ClassName)) &&
                                        (ffiSelected.FieldType == FormFieldControlTypeEnum.UploadControl))
                                    {
                                        BizFormInfoProvider.DeleteBizFormFiles(ClassName, ffiSelected.Name, SiteContext.CurrentSiteID);
                                    }

                                    // Update xml definition
                                    if (dci != null)
                                    {
                                        dci.ClassFormDefinition = FormDefinition;

                                        if (!ffiSelected.IsDummyField)
                                        {
                                            try
                                            {
                                                if (!ffiSelected.External)
                                                {
                                                    // Remove corresponding column from table
                                                    tm.DropTableColumn(dci.ClassTableName, SelectedItemName);

                                                    // Update xml schema
                                                    dci.ClassXmlSchema = tm.GetXmlSchema(dci.ClassTableName);
                                                }
                                            }
                                            catch (Exception ex)
                                            {
                                                EventLogProvider.LogException("FieldEditor", "SAVE", ex);
                                                errorMessage = ex.Message;
                                            }
                                        }

                                        // Deleted field is used as ClassNodeNameSource -> remove node name source
                                        if (dci.ClassNodeNameSource == SelectedItemName)
                                        {
                                            dci.ClassNodeNameSource = String.Empty;
                                        }

                                        // Update changes in database
                                        try
                                        {
                                            using (CMSActionContext context = new CMSActionContext())
                                            {
                                                // Do not log synchronization for BizForm
                                                if (mMode == FieldEditorModeEnum.BizFormDefinition)
                                                {
                                                    context.DisableLogging();
                                                }

                                                // Clean search settings
                                                dci.ClassSearchSettings = SearchHelper.CleanSearchSettings(dci);

                                                // Save the data class
                                                DataClassInfoProvider.SetDataClassInfo(dci);

                                                updateInherited = true;

                                                string where = "FormClassID=" + dci.ClassID;

                                                // If edited class is user settings, update user alt.forms which are combined with user settings too
                                                if (mClassName.EqualsCSafe(UserSettingsInfo.OBJECT_TYPE, StringComparison.InvariantCultureIgnoreCase))
                                                {
                                                    DataClassInfo userClass = DataClassInfoProvider.GetDataClassInfo(UserInfo.OBJECT_TYPE);
                                                    if (userClass != null)
                                                    {
                                                        where = SqlHelper.AddWhereCondition(where, "FormClassID=" + userClass.ClassID + " AND FormCoupledClassID=" + dci.ClassID, "OR");
                                                    }
                                                }

                                                // Update alternative forms
                                                var altforms = AlternativeFormInfoProvider.GetAlternativeForms(where, null);
                                                foreach (AlternativeFormInfo afi in altforms)
                                                {
                                                    afi.FormDefinition = FormHelper.RemoveFieldFromAlternativeDefinition(afi.FormDefinition, SelectedItemName, lstAttributes.SelectedIndex);
                                                    AlternativeFormInfoProvider.SetAlternativeFormInfo(afi);
                                                }
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            EventLogProvider.LogException("FieldEditor", "SAVE", ex);
                                            errorMessage = ex.Message;
                                        }

                                        // Refresh views and queries only if changes to DB were made
                                        if (!ffiSelected.External)
                                        {
                                            // Generate default view
                                            SqlGenerator.GenerateDefaultView(dci, mMode == FieldEditorModeEnum.BizFormDefinition ? SiteContext.CurrentSiteName : null);

                                            QueryInfoProvider.ClearDefaultQueries(dci, true, true);

                                            // Updates custom views
                                            if ((mMode == FieldEditorModeEnum.SystemTable) || (mMode == FieldEditorModeEnum.ClassFormDefinition))
                                            {
                                                try
                                                {
                                                    tm.RefreshCustomViews(dci.ClassTableName);

                                                    string lowClassName = dci.ClassName.ToLowerCSafe();
                                                    if (lowClassName == "cms.document" || lowClassName == "cms.tree")
                                                    {
                                                        tm.RefreshDocumentViews();
                                                    }
                                                }
                                                catch (Exception ex)
                                                {
                                                    errorMessage = ResHelper.GetString("fieldeditor.refreshingviewsfailed");
                                                    EventLogProvider.LogException("FieldEditor", "REFRESHVIEWS", ex);
                                                }
                                            }
                                        }
                                    }

                                    // Clear hashtables and search settings
                                    ClearHashtables();
                                }
                                break;
                        }
                    }
                }
                else if (SelectedItemType == FieldEditorSelectedItemEnum.Category)
                {
                    deletedItemPreffix = categPreffix;

                    // Remove specific category from xml form definition
                    fi.RemoveFormCategory(SelectedItemName);

                    // Get updated form definition
                    FormDefinition = fi.GetXmlDefinition();

                    switch (mMode)
                    {
                        case FieldEditorModeEnum.WebPartProperties:
                            // Web part
                            {
                                if (wpi != null)
                                {
                                    wpi.WebPartProperties = FormDefinition;
                                    try
                                    {
                                        WebPartInfoProvider.SetWebPartInfo(wpi);
                                    }
                                    catch (Exception ex)
                                    {
                                        EventLogProvider.LogException("FieldEditor", "REFRESHVIEWS", ex);
                                        errorMessage = ex.Message;
                                    }
                                }
                            }
                            break;

                        case FieldEditorModeEnum.ClassFormDefinition:
                        case FieldEditorModeEnum.BizFormDefinition:
                        case FieldEditorModeEnum.SystemTable:
                        case FieldEditorModeEnum.CustomTable:
                            // Standard classes
                            {
                                // Update xml definition
                                if (dci != null)
                                {
                                    dci.ClassFormDefinition = FormDefinition;

                                    // Update changes in database
                                    try
                                    {
                                        using (CMSActionContext context = new CMSActionContext())
                                        {
                                            // Do not log synchronization for BizForm
                                            if (mMode == FieldEditorModeEnum.BizFormDefinition)
                                            {
                                                context.DisableLogging();
                                            }

                                            // Save the data class
                                            DataClassInfoProvider.SetDataClassInfo(dci);

                                            updateInherited = true;

                                            string where = "FormClassID=" + dci.ClassID;

                                            // If edited class is user settings, update user alt.forms which are combined with user settings too
                                            if (mClassName.EqualsCSafe(UserSettingsInfo.OBJECT_TYPE, StringComparison.InvariantCultureIgnoreCase))
                                            {
                                                DataClassInfo userClass = DataClassInfoProvider.GetDataClassInfo(UserInfo.OBJECT_TYPE);
                                                if (userClass != null)
                                                {
                                                    where = SqlHelper.AddWhereCondition(where, "FormClassID=" + userClass.ClassID + " AND FormCoupledClassID=" + dci.ClassID, "OR");
                                                }
                                            }

                                            // Update alternative forms
                                            var altforms = AlternativeFormInfoProvider.GetAlternativeForms(where, null);

                                            foreach (AlternativeFormInfo afi in altforms)
                                            {
                                                afi.FormDefinition = FormHelper.RemoveCategoryFromAlternativeDefinition(afi.FormDefinition, SelectedItemName, lstAttributes.SelectedIndex);
                                                AlternativeFormInfoProvider.SetAlternativeFormInfo(afi);
                                            }
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        EventLogProvider.LogException("FieldEditor", "SAVE", ex);
                                        errorMessage = ex.Message;
                                    }
                                }
                            }
                            break;
                    }
                }

                if (!string.IsNullOrEmpty(errorMessage))
                {
                    ShowError("[ FieldEditor.btnDeleteItem_Click() ]: " + errorMessage);
                }
            }
            else
            {
                // "delete" new item from the list
                IsNewItemEdited = false;
            }

            // Raise on after definition update event
            if (OnAfterDefinitionUpdate != null)
            {
                OnAfterDefinitionUpdate(this, EventArgs.Empty);
            }

            // Raise on after item delete
            if (AfterItemDeleted != null)
            {
                AfterItemDeleted(this, new FieldEditorEventArgs(SelectedItemName, SelectedItemType, lstAttributes.SelectedIndex));
            }

            // Commit the transaction
            tr.Commit();

            // Set new selected value
            ListItem deletedItem = lstAttributes.Items.FindByValue(deletedItemPreffix + SelectedItemName);
            int deletedItemIndex = lstAttributes.Items.IndexOf(deletedItem);

            if ((deletedItemIndex > 0) && (lstAttributes.Items[deletedItemIndex - 1] != null))
            {
                newSelectedValue = lstAttributes.Items[deletedItemIndex - 1].Value;
            }

            // Reload data
            Reload(newSelectedValue);
        }

        // Update inherited classes with new fields if necessary
        if (updateInherited)
        {
            FormHelper.UpdateInheritedClasses(dci);
        }
    }


    /// <summary>
    /// Show javascript alert message.
    /// </summary>
    /// <param name="message">Message to show</param>
    private void ShowMessage(string message)
    {
        ltlScript.Text = ScriptHelper.GetScript("alert(" + ScriptHelper.GetString(message) + ");");
    }


    /// <summary>
    /// Called when source field selected index changed.
    /// </summary>
    protected void documentSource_OnSourceFieldChanged(object sender, EventArgs e)
    {
        if (mMode == FieldEditorModeEnum.ClassFormDefinition)
        {
            string errorMessage = null;

            DataClassInfo dci = DataClassInfoProvider.GetDataClassInfo(ClassName);
            if (dci != null)
            {
                // Set document name source field
                dci.ClassNodeNameSource = string.IsNullOrEmpty(documentSource.SourceFieldValue) ? "" : documentSource.SourceFieldValue;

                // Set document alias source field
                dci.ClassNodeAliasSource = string.IsNullOrEmpty(documentSource.SourceAliasFieldValue) ? "" : documentSource.SourceAliasFieldValue;

                try
                {
                    using (CMSActionContext context = new CMSActionContext())
                    {
                        // Do not log synchronization for BizForm
                        if (mMode == FieldEditorModeEnum.BizFormDefinition)
                        {
                            context.DisableLogging();
                        }

                        DataClassInfoProvider.SetDataClassInfo(dci);
                    }

                    ShowConfirmation(GetString("TemplateDesigner.SourceFieldSaved"));

                    WebControl control = sender as WebControl;
                    if (control != null && (control.ID == "drpSourceAliasField"))
                    {
                        ShowConfirmation(GetString("TemplateDesigner.SourceAliasFieldSaved"));
                    }
                }
                catch (Exception ex)
                {
                    EventLogProvider.LogException("FieldEditor", "SAVE", ex);
                    errorMessage = ex.Message;
                }
            }
            else
            {
                errorMessage = GetString("FieldEditor.ClassNotFound");
            }

            if (!string.IsNullOrEmpty(errorMessage))
            {
                ShowError("[ FieldEditor.drpSourceField_SelectedIndexChanged() ]: " + errorMessage);
            }
        }
    }


    /// <summary>
    /// Validates form and returns validation error message.
    /// </summary>
    private string ValidateForm()
    {
        const string INVALIDCHARACTERS = @".,;'`:/\*|?""&%$!-+=()[]{} ";

        string attributeName = databaseConfiguration.GetAttributeName();
        string control = fieldAppearance.FieldType;

        // Check if attribute name isn't empty
        if (string.IsNullOrEmpty(attributeName))
        {
            return GetString("TemplateDesigner.ErrorEmptyAttributeName") + " ";
        }

        // Check if attribute name starts with a letter or '_' (if it is an identifier)
        if (!ValidationHelper.IsIdentifier(attributeName))
        {
            return GetString("TemplateDesigner.ErrorAttributeNameDoesNotStartWithLetter") + " ";
        }

        // Check attribute name for invalid characters
        for (int i = 0; i <= INVALIDCHARACTERS.Length - 1; i++)
        {
            if (attributeName.Contains(INVALIDCHARACTERS[i]))
            {
                return GetString("TemplateDesigner.ErrorInvalidCharacter") + INVALIDCHARACTERS + ". ";
            }
        }

        if (chkDisplayInForm.Checked)
        {
            // Check if control is selected
            if (String.IsNullOrEmpty(control))
            {
                return GetString("fieldeditor.selectformcontrol");
            }
        }

        string errorMsg = databaseConfiguration.Validate();
        if (!String.IsNullOrEmpty(errorMsg))
        {
            return errorMsg;
        }

        if (!String.IsNullOrEmpty(errorMsg))
        {
            return errorMsg;
        }

        return null;
    }


    /// <summary>
    /// Validates basic forms for generated properties.
    /// </summary>
    /// <returns>TRUE if form is valid. FALSE is form is invalid</returns>
    private bool ValidateControlForms()
    {
        if (chkDisplayInForm.Checked)
        {
            return controlSettings.SaveData();
        }

        return true;
    }


    /// <summary>
    /// Returns FormFieldInfo structure with form data.
    /// </summary>   
    /// <param name="ffiOriginal">Original field info</param>
    private FormFieldInfo FillFormFieldInfoStructure(FormFieldInfo ffiOriginal)
    {
        string selectedType = string.Empty;
        DataRow settingsData = null;
        FormFieldInfo formFieldInfo;

        if (ffiOriginal != null)
        {
            // Field info with original information
            formFieldInfo = (FormFieldInfo)ffiOriginal.Clone();

            if (chkDisplayInForm.Checked)
            {
                // Reset control settings (hidden field's settings are preserved)
                formFieldInfo.Settings.Clear();
                formFieldInfo.SettingsMacroTable.Clear();
            }
        }
        else
        {
            formFieldInfo = new FormFieldInfo();
        }

        formFieldInfo.IsDummyField = IsDummyField;
        formFieldInfo.IsDummyFieldFromMainForm = IsDummyFieldFromMainForm;
        formFieldInfo.IsExtraField = IsExtraField;

        // Load FormFieldInfo with data from database configuration section
        databaseConfiguration.FieldInfo = formFieldInfo;
        databaseConfiguration.Save();

        // Do not save aditional field settings if field is hidden
        if (chkDisplayInForm.Checked)
        {
            // Field appearance section
            fieldAppearance.FieldInfo = formFieldInfo;
            fieldAppearance.Save();
            selectedType = fieldAppearance.FieldType;

            // Validation section
            validationSettings.FieldInfo = formFieldInfo;
            validationSettings.Save();

            // Design section
            cssSettings.FieldInfo = formFieldInfo;
            cssSettings.Save();

            // HtmlEnvelope section
            htmlEnvelope.FieldInfo = formFieldInfo;
            htmlEnvelope.Save();

            // Field advanced section
            formFieldInfo.SetPropertyValue(FormFieldPropertyEnum.VisibleMacro, fieldAdvancedSettings.VisibleMacro, true);
            formFieldInfo.SetPropertyValue(FormFieldPropertyEnum.EnabledMacro, fieldAdvancedSettings.EnabledMacro, true);
            formFieldInfo.DisplayInSimpleMode = fieldAdvancedSettings.DisplayInSimpleMode;
            formFieldInfo.HasDependingFields = fieldAdvancedSettings.HasDependingFields;
            formFieldInfo.DependsOnAnotherField = fieldAdvancedSettings.DependsOnAnotherField;

            // Get control settings data
            settingsData = controlSettings.FormData;

            // Store macro table
            formFieldInfo.SettingsMacroTable = controlSettings.MacroTable;
        }

        // Determine if it is external column
        formFieldInfo.External |= IsSystemFieldSelected;

        if (((Mode == FieldEditorModeEnum.BizFormDefinition) || (Mode == FieldEditorModeEnum.SystemTable) ||
            (Mode == FieldEditorModeEnum.AlternativeBizFormDefinition) || (Mode == FieldEditorModeEnum.AlternativeSystemTable))
            && (databaseConfiguration.AttributeType == FieldDataType.File))
        {
            // Allow to save <guid>.<extension>
            formFieldInfo.DataType = FieldDataType.Text;
            formFieldInfo.Size = 500;
        }
        else if (databaseConfiguration.AttributeType == FieldDataType.DocAttachments)
        {
            formFieldInfo.DataType = FieldDataType.DocAttachments;
            formFieldInfo.Size = 200;
        }
        else
        {
            formFieldInfo.DataType = databaseConfiguration.AttributeType;
            formFieldInfo.Size = ValidationHelper.GetInteger(databaseConfiguration.AttributeSize, 0);
            formFieldInfo.Precision = ValidationHelper.GetInteger(databaseConfiguration.AttributePrecision, -1);
        }

        formFieldInfo.Visible = chkDisplayInForm.Checked;

        formFieldInfo.DisplayIn = String.Empty;
        if (chkDisplayInDashBoard.Checked)
        {
            formFieldInfo.DisplayIn = DisplayIn;
        }

        // Store field type
        if (formFieldInfo.Visible)
        {
            // Remove control prefix if exists
            if (selectedType.Contains(controlPrefix))
            {
                selectedType = selectedType.Substring(controlPrefix.Length);
            }
            formFieldInfo.Settings["controlname"] = selectedType;
        }

        // Store settings
        if ((settingsData != null) && (settingsData.ItemArray.Length > 0))
        {
            foreach (DataColumn column in settingsData.Table.Columns)
            {
                formFieldInfo.Settings[column.ColumnName] = settingsData.Table.Rows[0][column.Caption];
            }
        }

        formFieldInfo.FieldType = FormFieldControlTypeEnum.CustomUserControl;

        return formFieldInfo;
    }


    /// <summary>
    /// Displays selected tab content.
    /// </summary>
    protected void DisplaySelectedTabContent()
    {
        plcField.Visible = true;
        plcCategory.Visible = false;
    }


    /// <summary>
    /// Hides all editing panels.
    /// </summary>
    protected void HideAllPanels()
    {
        plcCategory.Visible = false;
        plcField.Visible = false;
    }


    /// <summary>
    /// Adds new form item (field or category) to the form definition.
    /// </summary>
    /// <param name="formItem">Form item to add</param>
    protected void InsertFormItem(IField formItem)
    {
        // Set new item preffix
        string newItemPreffix = (formItem is FormFieldInfo) ? newFieldPreffix : newCategPreffix;

        ListItem newItem = lstAttributes.Items.FindByValue(newItemPreffix);
        int newItemIndex = lstAttributes.Items.IndexOf(newItem);

        if ((newItemIndex > 0) && (lstAttributes.Items[newItemIndex - 1] != null))
        {
            // Add new item at the specified position
            fi.AddFormItem(formItem, newItemIndex);
        }
        else
        {
            if (formItem is FormFieldInfo)
            {
                // Add new field at the end of the collection
                fi.AddFormItem(formItem);
            }
            else
            {
                var catObj = formItem as FormCategoryInfo;
                if (catObj != null)
                {
                    // Add new category at the end of the collection
                    fi.AddFormCategory(catObj);
                }
            }
        }
    }


    /// <summary>
    /// Returns FormInfo for given form user control.
    /// </summary>
    /// <param name="controlName">Code name of form control</param>
    /// <returns>Form info</returns>
    private FormInfo GetUserControlSettings(string controlName)
    {
        FormUserControlInfo control = FormUserControlInfoProvider.GetFormUserControlInfo(controlName);
        if (control != null)
        {
            // Get complete form info for current control
            FormInfo formInfo = FormHelper.GetFormControlParameters(controlName, control.UserControlMergedParameters, true);

            return formInfo;
        }
        return null;
    }


    /// <summary>
    /// Clears hashtables.
    /// </summary>
    private void ClearHashtables()
    {
        // Clear the object type hashtable
        ProviderStringDictionary.ReloadDictionaries(ClassName, true);

        // Clear the classes hashtable
        ProviderStringDictionary.ReloadDictionaries("cms.class", true);

        // Clear class structures
        ClassStructureInfo.Remove(ClassName, true);

        // Clear form resolver
        FormControlsResolvers.ClearResolvers(true);

        // Invalidate objects based on object type
        ObjectTypeInfo ti = ObjectTypeManager.GetRegisteredTypeInfo(ClassName);
        if ((ti != null) && (ti.ProviderType != null))
        {
            ti.InvalidateColumnNames();
            ti.InvalidateAllObjects();

            // If edited class is user settings, clear the user structure info which contains the user settings columns
            if ((Mode == FieldEditorModeEnum.SystemTable) && mClassName.EqualsCSafe(UserSettingsInfo.OBJECT_TYPE, StringComparison.InvariantCultureIgnoreCase))
            {
                ObjectTypeInfo userTypeInfo = UserInfo.TYPEINFO;
                userTypeInfo.InvalidateColumnNames();
                userTypeInfo.InvalidateAllObjects();
            }
        }
    }


    /// <summary>
    /// Raises OnFieldCreated event.
    /// </summary>
    /// <param name="newField">Newly created field</param>
    protected void RaiseOnFieldCreated(FormFieldInfo newField)
    {
        if (OnFieldCreated != null)
        {
            OnFieldCreated(this, newField);
        }
    }

    #endregion


    #region "Support for system fields"

    /// <summary>
    /// Group changed event handler.
    /// </summary>
    private void databaseConfiguration_DropChanged(object sender, EventArgs e)
    {
        LoadSystemField();
    }


    /// <summary>
    /// Database attribute has changed.
    /// </summary>
    private void databaseConfiguration_AttributeChanged(object sender, EventArgs e)
    {
        databaseConfiguration.Mode = Mode;
        databaseConfiguration.IsDocumentType = IsDocumentType;
        databaseConfiguration.DevelopmentMode = DevelopmentMode;
        databaseConfiguration.IsAlternativeForm = IsAlternativeForm;
        databaseConfiguration.IsInheritedForm = IsInheritedForm;
        databaseConfiguration.IsDummyField = IsDummyField;
        databaseConfiguration.IsDummyFieldFromMainForm = IsDummyFieldFromMainForm;
        databaseConfiguration.IsExtraField = IsExtraField;
        databaseConfiguration.EnableOrDisableAttributeSize();
        fieldAppearance.Mode = Mode;
        fieldAppearance.ClassName = ClassName;
        fieldAppearance.AttributeType = databaseConfiguration.AttributeType;
        fieldAppearance.FieldType = FormHelper.GetFormFieldDefaultControlType(databaseConfiguration.AttributeType);
        fieldAppearance.LoadFieldTypes(IsPrimaryField);
        ShowFieldOptions();
        LoadValidationSettings();
        databaseConfiguration.ShowDefaultControl();
        LoadControlSettings();
    }


    /// <summary>
    /// Field control changed event handler.
    /// </summary>
    private void control_FieldSelected(object sender, EventArgs e)
    {
        LoadControlSettings();
        LoadValidationSettings();
    }


    /// <summary>
    /// FieldTypeSelector field type selection event handler.
    /// </summary>
    private void fieldTypeSelector_OnSelectionChanged(object sender, EventArgs e)
    {
        SetFieldType();
        LoadDefaultAttributeEditForm(false);
        LoadControlSettings();
    }


    /// <summary>
    /// Loads control with new FormInfo data.
    /// </summary>
    /// <param name="selectedFieldType">Selected field</param>
    private void LoadControlSettings(string selectedFieldType = null)
    {
        if (String.IsNullOrEmpty(selectedFieldType))
        {
            selectedFieldType = fieldAppearance.FieldType;
        }
        if (selectedFieldType.StartsWithCSafe(controlPrefix))
        {
            selectedFieldType = selectedFieldType.Substring(controlPrefix.Length);
        }

        controlSettings.FormInfo = GetUserControlSettings(selectedFieldType);
        if (ffi != null)
        {
            controlSettings.Settings = ffi.Settings;
            controlSettings.MacroTable = ffi.SettingsMacroTable;
            controlSettings.BasicForm.Mode = FormModeEnum.Update;
        }

        controlSettings.Reload(true);
    }


    /// <summary>
    /// Loads system field either from database column data or from field XML definition.
    /// </summary>
    private void LoadSystemField()
    {
        string tableName = databaseConfiguration.GroupValue;
        string columnName = databaseConfiguration.SystemValue;

        if (SelectedItemName.ToLowerCSafe() != columnName.ToLowerCSafe())
        {
            DataClassInfo dci = DataClassInfoProvider.GetDataClassInfo(mClassName);

            // Get field info from database column
            ffi = FormHelper.GetFormFieldInfo(dci, tableName, columnName);
        }
        else
        {
            // Get field info from XML definition
            LoadFormDefinition();
            ffi = fi.GetFormField(SelectedItemName);
        }

        LoadSelectedField(false);
    }


    /// <summary>
    /// Initializes UI context.
    /// </summary>
    private void InitUIContext(FormInfo formInfo)
    {
        if (formInfo != null)
        {
            ArrayList result = new ArrayList();

            // Get all fields except the system ones
            var fields = formInfo.GetFields(true, true);

            foreach (FormFieldInfo field in fields)
            {
                string caption = field.Caption;

                if (String.IsNullOrEmpty(caption))
                {
                    caption = field.Name;
                }

                if (fields.Any(f => (f.Name != field.Name) && (f.Caption == caption)))
                {
                    // Add field name if more fields have similar caption
                    caption += String.Format(" [{0}]", field.Name);
                }

                result.Add(String.Format("{0};{1}", field.Guid, caption));
            }

            UIContext["CurrentFormFields"] = result;
            ViewState["CurrentFormFields"] = result;
        }
    }

    #endregion
}