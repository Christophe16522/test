using System;
using System.Collections;
using System.Text;
using System.Web.UI.WebControls;
using System.Data;

using CMS.FormEngine;
using CMS.Helpers;
using CMS.Base;
using CMS.UIControls;
using CMS.DataEngine;
using CMS.Modules;

public partial class CMSModules_Modules_Controls_Settings_Key_SettingsKeyEdit : CMSAdminEditControl
{
    #region "Private Members"

    private SettingsKeyInfo mSettingsKeyObj = null;
    private int mSettingsKeyId = 0;
    private int mRootCategoryId = 0;
    private int mSelectedGroupId = -1;

    #endregion


    #region "Properties"

    /// <summary>
    /// Gets or sets RootCategoryID. Specifies SettingsCategory which should be set up as the root of the SettingsCategorySelector.
    /// </summary>
    public int RootCategoryID
    {
        get
        {
            return mRootCategoryId;
        }
        set
        {
            mRootCategoryId = value;
        }
    }


    /// <summary>
    /// Gets or sets SelectedGroupID. Specifies SettingsCategory for new record. If set, SettingsCategorySelector is not shown.
    /// </summary>
    public int SelectedGroupID
    {
        get
        {
            return mSelectedGroupId;
        }
        set
        {
            mSelectedGroupId = value;
        }
    }


    /// <summary>
    /// Gets or sets SettingsKeyID. Specifies ID of SettingsKey object.
    /// </summary>
    public int SettingsKeyID
    {
        get
        {
            return mSettingsKeyId;
        }
        set
        {
            mSettingsKeyId = value;
            mSettingsKeyObj = null;
        }
    }


    /// <summary>
    /// Gets or sets SettingsKey object. Specifies SettingsKey object which should be edited.
    /// </summary>
    public SettingsKeyInfo SettingsKeyObj
    {
        get
        {
            return mSettingsKeyObj ?? (mSettingsKeyObj = SettingsKeyInfoProvider.GetSettingsKeyInfo(mSettingsKeyId));
        }
        set
        {
            mSettingsKeyObj = value;
            mSettingsKeyId = (value != null) ? value.KeyID : 0;
        }
    }


    /// <summary>
    /// Default key value from/for appropriate control (text box or check box)
    /// </summary>
    protected string DefaultValue
    {
        get
        {
            switch (drpKeyType.SelectedValue)
            {
                case "boolean":
                    return chkKeyValue.Checked ? "True" : "False";

                case "longtext":
                    return txtLongTextKeyValue.Text.Trim();

                default:
                    return txtKeyValue.Text.Trim();
            }
        }
        set
        {
            switch (drpKeyType.SelectedValue)
            {
                case "boolean":
                    chkKeyValue.Checked = ValidationHelper.GetBoolean(value, false);
                    break;

                case "longtext":
                    txtLongTextKeyValue.Text = value;
                    break;

                default:
                    txtKeyValue.Text = value;
                    break;
            }
        }
    }


    /// <summary>
    /// Indicates current selected module.
    /// </summary>
    public int ModuleID
    {
        get;
        set;
    }

    #endregion


    #region "Page Events"

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        if (StopProcessing)
        {
            return;
        }

        InitControls();

        // Load the form data
        if (!URLHelper.IsPostback())
        {
            LoadData();
        }
        else
        {
            ucSettingsKeyControlSelector.SetSelectorProperties(drpKeyType.SelectedValue);
        }

        // Set edited object
        EditedObject = (mSettingsKeyId > 0) ? SettingsKeyObj : new SettingsKeyInfo();

        // Init form control settings
        var controlCodeName = ValidationHelper.GetString(ucSettingsKeyControlSelector.Value, null);
        var controlInfo = FormUserControlInfoProvider.GetFormUserControlInfo(controlCodeName);
        if (controlInfo != null)
        {
            pnlControlSettings.Visible = true;

            ucControlSettings.FormInfo = FormHelper.GetFormControlParameters(controlCodeName, controlInfo.UserControlMergedParameters, true);
            ucControlSettings.Reload(true);

            pnlControlSettings.Visible = ucControlSettings.CheckVisibility();
        }
        else
        {
            pnlControlSettings.Visible = false;
        }
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        SelectDefaultValueControl();
    }

    #endregion


    #region "Private Methods"

    /// <summary>
    /// Initialization of controls.
    /// </summary>
    private void InitControls()
    {
        // Init validations
        rfvKeyDisplayName.ErrorMessage = ResHelper.GetString("general.requiresdisplayname");
        rfvKeyName.ErrorMessage = ResHelper.GetString("general.requirescodename");

        // Display of LoadGeneration table row
        trLoadGeneration.Visible = SystemContext.DevelopmentMode;

        // Set the root category
        if (RootCategoryID > 0)
        {
            drpCategory.RootCategoryId = RootCategoryID;
        }

        // Enable only groups which belong to selected module
        drpCategory.EnabledCondition = "{% " + (!SystemContext.DevelopmentMode ? "(CategoryResourceID == " + ModuleID + ") && " : String.Empty) + "(CategoryIsGroup)%}";

        // If category specified programmatically
        if (mSelectedGroupId >= 0)
        {
            // Set category selector value
            if (!RequestHelper.IsPostBack())
            {
                drpCategory.SelectedCategory = mSelectedGroupId;
            }

            // Hide category selector
            trCategory.Visible = false;
        }
        else
        {
            // Set category selector value
            if (!URLHelper.IsPostback() && (SettingsKeyObj != null) && (SettingsKeyObj.KeyCategoryID > 0))
            {
                drpCategory.SelectedCategory = SettingsKeyObj.KeyCategoryID;
            }
        }

        if (!URLHelper.IsPostback())
        {
            LoadKeyTypes();
        }

        // Disable editing for keys not assigned to current module
        if (SettingsKeyObj != null)
        {
            SettingsCategoryInfo parentCategory = SettingsCategoryInfoProvider.GetSettingsCategoryInfo(SettingsKeyObj.KeyCategoryID);
            ResourceInfo resource = ResourceInfoProvider.GetResourceInfo(ModuleID);
            plnEdit.Enabled = btnOk.Enabled = (resource != null) && (((parentCategory != null) && (parentCategory.CategoryResourceID == resource.ResourceId) && resource.ResourceIsInDevelopment) || SystemContext.DevelopmentMode);

        }

        ucControlSettings.BasicForm.EnsureMessagesPlaceholder(MessagesPlaceHolder);
    }


    /// <summary>
    /// Loads key types into the CMSDropDownList control.
    /// </summary>
    private void LoadKeyTypes()
    {
        drpKeyType.Items.Clear();
        drpKeyType.Items.Add(new ListItem(ResHelper.GetString("TemplateDesigner.FieldTypes.Boolean"), "boolean"));
        drpKeyType.Items.Add(new ListItem(ResHelper.GetString("TemplateDesigner.FieldTypes.Integer"), "int"));
        drpKeyType.Items.Add(new ListItem(ResHelper.GetString("TemplateDesigner.FieldTypes.Double"), "double"));
        drpKeyType.Items.Add(new ListItem(ResHelper.GetString("TemplateDesigner.FieldTypes.Text"), "string"));
        drpKeyType.Items.Add(new ListItem(ResHelper.GetString("TemplateDesigner.FieldTypes.LongText"), "longtext"));

        SelectDefaultValueControl();
    }


    /// <summary>
    /// Loads the data into the form.
    /// </summary>
    private void LoadData()
    {
        if (SettingsKeyObj == null)
        {
            return;
        }

        // Load the form from the Info object
        txtKeyName.Text = SettingsKeyObj.KeyName;
        txtKeyDisplayName.Text = SettingsKeyObj.KeyDisplayName;
        txtKeyDescription.Text = SettingsKeyObj.KeyDescription;
        drpCategory.SelectedCategory = SettingsKeyObj.KeyCategoryID;
        drpKeyType.SelectedValue = SettingsKeyObj.KeyType;
        DefaultValue = SettingsKeyObj.KeyDefaultValue;
        txtKeyValidation.Text = SettingsKeyObj.KeyValidation;
        ucSettingsKeyControlSelector.SetSelectorProperties(SettingsKeyObj.KeyType, SettingsKeyObj.KeyEditingControlPath);
        drpGeneration.Value = -1;
        chkKeyIsGlobal.Checked = SettingsKeyObj.KeyIsGlobal;
        chkKeyIsHidden.Checked = SettingsKeyObj.KeyIsHidden;

        // Load form control settings
        if (!string.IsNullOrEmpty(SettingsKeyObj.KeyFormControlSettings))
        {
            var formFieldInfo = FormHelper.GetFormControlSettingsFromXML(SettingsKeyObj.KeyFormControlSettings);
            ucControlSettings.Settings = formFieldInfo.Settings;
            ucControlSettings.MacroTable = formFieldInfo.SettingsMacroTable;
        }
    }


    /// <summary>
    /// Updates settings key for all sites (or only global if the IsGlobal checkbox is checked).
    /// </summary>
    /// <returns>CodeName of the SettingsKey objects.</returns>
    private int UpdateKey()
    {
        // Try to get the key
        var keyObj = (mSettingsKeyId > 0) ? SettingsKeyInfoProvider.GetSettingsKeyInfo(mSettingsKeyId) : null;
        if (keyObj == null)
        {
            // Create new
            keyObj = new SettingsKeyInfo();
        }

        var oldKeyCategoryID = keyObj.KeyCategoryID;

        // Set values
        keyObj.KeyName = txtKeyName.Text.Trim();
        keyObj.KeyDisplayName = txtKeyDisplayName.Text.Trim();
        keyObj.KeyDescription = txtKeyDescription.Text.Trim();
        keyObj.KeyType = drpKeyType.SelectedValue;
        keyObj.KeyCategoryID = mSelectedGroupId >= 0 ? mSelectedGroupId : drpCategory.SelectedCategory;
        keyObj.KeyIsGlobal = chkKeyIsGlobal.Checked;
        keyObj.KeyIsHidden = chkKeyIsHidden.Checked;
        keyObj.KeyValidation = (string.IsNullOrEmpty(txtKeyValidation.Text.Trim()) ? null : txtKeyValidation.Text.Trim());
        keyObj.KeyDefaultValue = (string.IsNullOrEmpty(DefaultValue) ? null : DefaultValue);

        var path = ValidationHelper.GetString(ucSettingsKeyControlSelector.ControlPath, string.Empty);
        keyObj.KeyEditingControlPath = (string.IsNullOrEmpty(path.Trim()) ? null : path.Trim());

        // Update form control settings
        if (ucSettingsKeyControlSelector.IsFormControlSelected)
        {
            var formFieldInfo = new FormFieldInfo();
            ucControlSettings.SaveData();

            formFieldInfo.SettingsMacroTable = ucControlSettings.MacroTable;

            if ((ucControlSettings.FormData != null) && (ucControlSettings.FormData.ItemArray.Length > 0))
            {
                foreach (DataColumn column in ucControlSettings.FormData.Table.Columns)
                {
                    formFieldInfo.Settings[column.ColumnName] = ucControlSettings.FormData.Table.Rows[0][column.Caption];
                }
            }

            var settings = FormHelper.GetFormControlSettingsXml(formFieldInfo);
            keyObj.KeyFormControlSettings = settings;
        }
        else
        {
            keyObj.KeyFormControlSettings = null;
        }

        if (drpGeneration.Value >= 0)
        {
            keyObj.KeyLoadGeneration = drpGeneration.Value;
        }

        if (keyObj.KeyID == 0)
        {
            keyObj.KeyValue = DefaultValue;
        }

        if (chkKeyIsGlobal.Checked)
        {
            keyObj.SiteID = 0;
        }

        // If category changed set new order or if new set on the end of key list
        if (keyObj.KeyCategoryID != oldKeyCategoryID)
        {
            var keys = SettingsKeyInfoProvider.GetSettingsKeys(keyObj.KeyCategoryID)
                .OrderByDescending("KeyOrder")
                .Column("KeyOrder");

            keyObj.KeyOrder = keys.GetScalarResult(0) + 1;
        }

        SettingsKeyInfoProvider.SetSettingsKeyInfo(keyObj);

        // Update property
        mSettingsKeyObj = keyObj;

        return keyObj.KeyID;
    }


    /// <summary>
    /// Validates the form. If validation succeeds returns true, otherwise returns false.
    /// </summary>
    private bool Validate()
    {
        bool isValid = true;

        // Validate form fields
        string errMsg = new Validator().NotEmpty(txtKeyName.Text.Trim(), ResHelper.GetString("general.requirescodename"))
            .NotEmpty(txtKeyDisplayName.Text.Trim(), ResHelper.GetString("general.requiresdisplayname"))
            .IsIdentifier(txtKeyName.Text.Trim(), GetString("general.erroridentifierformat"))
            .Result;

        // Validate default value format
        if (!string.IsNullOrEmpty(DefaultValue))
        {
            switch (drpKeyType.SelectedValue)
            {
                case "double":
                    if (!ValidationHelper.IsDouble(DefaultValue))
                    {
                        lblDefValueError.Text = ResHelper.GetString("settings.validationdoubleerror");
                        lblDefValueError.Visible = true;
                        isValid = false;
                    }
                    break;

                case "int":
                    if (!ValidationHelper.IsInteger(DefaultValue))
                    {
                        lblDefValueError.Text = ResHelper.GetString("settings.validationinterror");
                        lblDefValueError.Visible = true;
                        isValid = false;
                    }
                    break;
            }
        }

        // Check if the code name is used
        var key = SettingsKeyInfoProvider.GetSettingsKeyInfo(txtKeyName.Text.Trim());
        if ((key != null) && (key.KeyID != mSettingsKeyId))
        {
            errMsg = GetString("general.codenameexists");
        }

        if (!ucSettingsKeyControlSelector.IsValid())
        {
            errMsg = GetString("settingskeyedit.selectcustomcontrol");
        }

        // Set up error message
        if (!string.IsNullOrEmpty(errMsg))
        {
            ShowError(errMsg);
            isValid = false;
        }

        return isValid;
    }


    /// <summary>
    /// Shows suitable default value edit control according to key type from drpKeyType.
    /// </summary>
    private void SelectDefaultValueControl()
    {
        chkKeyValue.Visible = drpKeyType.SelectedValue == "boolean";
        txtLongTextKeyValue.Visible = drpKeyType.SelectedValue == "longtext";
        txtKeyValue.Visible = !chkKeyValue.Visible && !txtLongTextKeyValue.Visible;
    }

    #endregion


    #region "Event Handlers"

    protected void btnOK_Click(object sender, EventArgs e)
    {
        // Validate input
        var isValid = Validate();
        if (!isValid)
        {
            return;
        }

        // Update key
        mSettingsKeyId = UpdateKey();
        RaiseOnSaved();

        // Show the info message
        ShowChangesSaved();

        // Show category selection
        trCategory.Visible = true;

        // Select 'Keep current settings' option for load generation property
        drpGeneration.Value = -1;
    }


    protected void drpKeyType_SelectedIndexChanged(object sender, EventArgs e)
    {
        SelectDefaultValueControl();
    }

    #endregion
}