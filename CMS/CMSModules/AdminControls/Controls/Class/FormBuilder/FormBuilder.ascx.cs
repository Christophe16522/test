using System;
using System.Collections;
using System.Linq;
using System.Web.UI;

using CMS.Base;
using CMS.DataEngine;
using CMS.ExtendedControls;
using CMS.FormControls;
using CMS.FormEngine;
using CMS.Helpers;
using CMS.Membership;
using CMS.SiteProvider;
using CMS.UIControls;

public partial class CMSModules_AdminControls_Controls_Class_FormBuilder_FormBuilder : CMSUserControl, ICallbackEventHandler
{
    #region "Variables"

    private FormInfo mFormInfo;
    private string mCallbackResult = string.Empty;
    private bool mReloadField;
    private bool mReloadForm;

    #endregion


    #region "Properties"

    /// <summary>
    /// Name of edited class.
    /// </summary>
    public string ClassName
    {
        get;
        set;
    }


    /// <summary>
    /// Control which is used to design the form.
    /// </summary>
    public BasicForm Form
    {
        get
        {
            return formElem;
        }
    }


    /// <summary>
    /// Field name.
    /// </summary>
    private string FieldName
    {
        get
        {
            return ValidationHelper.GetString(ViewState["FieldName"], string.Empty);
        }
        set
        {
            ViewState["FieldName"] = value;
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
    /// Messages placeholder.
    /// </summary>
    public override MessagesPlaceHolder MessagesPlaceHolder
    {
        get
        {
            return plcMessagesHolder;
        }
    }

    #endregion


    #region "Control events"

    protected override void LoadViewState(object savedState)
    {
        base.LoadViewState(savedState);

        if (ViewState["CurrentFormFields"] != null)
        {
            // Refresh UIContext data
            UIContext["CurrentFormFields"] = ViewState["CurrentFormFields"];
        }
    }


    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        // Load form info
        mFormInfo = FormHelper.GetFormInfo(ClassName, true);
        if (mFormInfo != null)
        {
            ScriptHelper.RegisterJQueryUI(Page);
            ScriptHelper.RegisterScriptFile(Page, "~/CMSModules/AdminControls/Controls/Class/FormBuilder/FormBuilder.js");

            // Set up callback script
            String cbReference = Page.ClientScript.GetCallbackEventReference(this, "arg", "FormBuilder.receiveServerData", String.Empty);
            String callbackScript = "function doFieldAction(arg) {" + cbReference + "; }";
            ScriptHelper.RegisterClientScriptBlock(Page, GetType(), "FormBuilderCallback", callbackScript, true);

            // Prepare Submit button
            formElem.SubmitButton.RegisterHeaderAction = false;
            formElem.SubmitButton.OnClientClick = formElem.SubmitImageButton.OnClientClick = "return false;";

            formElem.FormInformation = mFormInfo;
            formElem.Data = new DataRowContainer(mFormInfo.GetDataRow());

            // Load form
            formElem.ReloadData();

            // Prepare error message label
            MessagesPlaceHolder.ErrorLabel.CssClass += " form-builder-error-hidden";
            MessagesPlaceHolder.ErrorText = GetString("FormBuilder.GeneralError");

            InitUIContext(mFormInfo);
        }
        else
        {
            formElem.StopProcessing = true;
            ShowError(GetString("FormBuilder.ErrorLoadingForm"));
        }

        if (RequestHelper.IsPostBack())
        {
            ProcessAjaxPostBack();
        }
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        if (mFormInfo == null)
        {
            return;
        }

        // Reload selected field if required
        if (mReloadField && !string.IsNullOrEmpty(FieldName))
        {
            Form.ReloadData();
            if (Form.FieldUpdatePanels.ContainsKey(FieldName))
            {
                Form.FieldUpdatePanels[FieldName].Update();

                string script = String.Format("if (window.RecalculateFormWidth) {{RecalculateFormWidth('{0}');}}", formElem.ClientID);
                ScriptHelper.RegisterStartupScript(Page, pnlUpdateForm.GetType(), "recalculateFormWidth" + ClientID, ScriptHelper.GetScript(script));
            }
            else
            {
                Form.StopProcessing = true;
                MessagesPlaceHolder.ShowError(String.Format("{0} {1}", GetString("editedobject.notexists"), GetString("formbuilder.refresh")));

                MessagesPlaceHolder.ErrorLabel.CssClass += " form-builder-error";
                pnlUpdateForm.Update();
            }
        }

        // Reload whole form
        if (mReloadForm && !Form.StopProcessing)
        {
            formElem.ReloadData();
            pnlUpdateForm.Update();

            ScriptHelper.RegisterStartupScript(pnlUpdateForm, pnlUpdateForm.GetType(), "FormBuilderAddComponent", "FormBuilder.init(); FormBuilder.selectField('" + FieldName + "')", true);
        }

        // Display placeholder with message if form has no visible components and hide OK button
        if (mFormInfo.GetFormElements(true, false).Count == 0)
        {
            ScriptHelper.RegisterStartupScript(pnlUpdateForm, pnlUpdateForm.GetType(), "FormBuilderShowPlaceholder", "FormBuilder.showEmptyFormPlaceholder();", true);
            formElem.SubmitButton.Visible = false;
        }

        // Set settings panel visibility
        pnlSettings.SetSettingsVisibility(!string.IsNullOrEmpty(FieldName));
    }

    #endregion


    #region "Methods"

    private void ProcessAjaxPostBack()
    {
        if (RequestHelper.IsPostBack())
        {
            string eventArgument = Request.Params.Get("__EVENTARGUMENT");

            if (!string.IsNullOrEmpty(eventArgument))
            {
                string[] data = eventArgument.Split(':');

                FormFieldInfo ffi;
                switch (data[0])
                {
                    case "loadSettings":
                        {
                            FieldName = data[1];
                            LoadSettings(FieldName);
                        }
                        break;

                    case "remove":
                        {
                            // Hide selected field from form
                            FieldName = string.Empty;
                            HideField(data[2]);
                            mReloadForm = true;
                            pnlSettings.Update();
                        }
                        break;

                    case "hideSettingsPanel":
                        {
                            FieldName = string.Empty;
                            pnlSettings.Update();
                        }
                        break;

                    case "saveSettings":
                        {
                            ffi = mFormInfo.GetFormField(FieldName);
                            FormFieldInfo originalFieldInfo = (FormFieldInfo)ffi.Clone();
                            pnlSettings.SaveSettings(ffi);
                            SaveFormDefinition(originalFieldInfo, ffi);
                            mReloadField = true;
                        }
                        break;

                    case "addField":
                        {
                            ffi = PrepareNewField(data[1]);
                            FieldName = ffi.Name;
                            AddField(ffi, data[2], ValidationHelper.GetInteger(data[3], -1));
                            LoadSettings(FieldName);
                            mReloadForm = true;
                        }
                        break;
                }
            }
        }
    }


    /// <summary>
    /// Loads field settings to the Settings panel.
    /// </summary>
    /// <param name="fieldName">Field name</param>
    private void LoadSettings(string fieldName)
    {
        FormFieldInfo ffi = mFormInfo.GetFormField(fieldName);
        pnlSettings.LoadSettings(ffi);
        pnlSettings.Update();
    }


    /// <summary>
    /// Hides field.
    /// </summary>
    /// <param name="fieldName">Name of field that should be hidden</param>
    /// <returns>Error message if an error occurred</returns>
    protected string HideField(string fieldName)
    {
        if (!string.IsNullOrEmpty(fieldName))
        {
            FormFieldInfo ffiSelected = mFormInfo.GetFormField(fieldName);
            if (ffiSelected == null)
            {
                return GetString("editedobject.notexists");
            }

            ffiSelected.Visible = false;
            return SaveFormDefinition();
        }
        return string.Empty;
    }


    /// <summary>
    /// Saves form definition.
    /// </summary>
    /// <param name="oldFieldInfo">Form field info prior to the change</param>
    /// <param name="updatedFieldInfo">Form field info after the change has been made.</param>
    /// <returns>Error message if an error occurred</returns>
    protected string SaveFormDefinition(FormFieldInfo oldFieldInfo = null, FormFieldInfo updatedFieldInfo = null)
    {
		if (!MembershipContext.AuthenticatedUser.IsAuthorizedPerResource("cms.form", "EditForm"))
		{
			if (RequestHelper.IsCallback())
			{
				return GetString("formbuilder.missingeditpermission");
			}

			RedirectToAccessDenied("cms.form", "EditForm");
		}

	    DataClassInfo dci = DataClassInfoProvider.GetDataClassInfo(ClassName);

        if ((mFormInfo != null) && (dci != null))
        {
            // Update database column of the changed field
            if (IsDatabaseChangeRequired(oldFieldInfo, updatedFieldInfo))
            {
                // Ensure the transaction
                using (var tr = new CMSLateBoundTransaction())
                {
                    TableManager tm = new TableManager(dci.ClassConnectionString);
                    tr.BeginTransaction();

                    string error = UpdateDatabaseColumn(oldFieldInfo, updatedFieldInfo, tm, dci.ClassTableName);
                    if (!String.IsNullOrEmpty(error))
                    {
                        return error;
                    }

                    // Commit the transaction
                    tr.Commit();
                }
            }

            // Update form definition   
            dci.ClassFormDefinition = mFormInfo.GetXmlDefinition();

            // Save the class data
            DataClassInfoProvider.SetDataClassInfo(dci);

            // Update inherited classes with new fields
            FormHelper.UpdateInheritedClasses(dci);

            return string.Empty;
        }
        else
        {
            return GetString("FormBuilder.ErrorSavingForm");
        }
    }


    /// <summary>
    /// Prepares new field.
    /// </summary>
    /// <param name="controlName">Code name of used control</param>
    private FormFieldInfo PrepareNewField(string controlName)
    {
        FormFieldInfo ffi = new FormFieldInfo();

        string[] controlDefaultDataType = FormUserControlInfoProvider.GetUserControlDefaultDataType(controlName);
        ffi.DataType = controlDefaultDataType[0];
        ffi.Size = ValidationHelper.GetInteger(controlDefaultDataType[1], 0);
        ffi.FieldType = FormFieldControlTypeEnum.CustomUserControl;

        FormUserControlInfo control = FormUserControlInfoProvider.GetFormUserControlInfo(controlName);
        if (control != null)
        {
            ffi.SetPropertyValue(FormFieldPropertyEnum.FieldCaption, control.UserControlDisplayName);
        }

        ffi.AllowEmpty = true;
        ffi.PublicField = true;
        ffi.Name = GetUniqueFieldName(controlName);
        ffi.Settings["controlname"] = controlName;

        // For list controls create three default options
        if (FormHelper.HasListControl(ffi))
        {
            SpecialFieldsDefinition optionDefinition = new SpecialFieldsDefinition();

            for (int i = 1; i <= 3; i++)
            {
                optionDefinition.Add(new SpecialField
                {
                    Value = OptionsDesigner.DEFAULT_OPTION + i,
                    Text = OptionsDesigner.DEFAULT_OPTION + i
                });
            }

            ffi.Settings["Options"] = optionDefinition.ToString();
        }

        if (controlName.EqualsCSafe("CalendarControl"))
        {
            ffi.Settings["EditTime"] = false;
        }

        return ffi;
    }


    /// <summary>
    /// Ensures unique field name.
    /// </summary>
    /// <param name="name">Field name</param>
    private string GetUniqueFieldName(string name)
    {
        int uniqueIndex = 1;
        bool unique = false;
        string uniqueName = name;

        while (!unique)
        {
            if (mFormInfo.GetFormField(uniqueName) == null)
            {
                unique = true;
            }
            else
            {
                uniqueName = name + "_" + uniqueIndex;
                uniqueIndex++;
            }
        }
        return uniqueName;
    }


    /// <summary>
    /// Adds form field info to the form to the specified position.
    /// </summary>
    /// <param name="ffi">Form field info which will be added</param>
    /// <param name="category">Category name</param>
    /// <param name="position">Field position in the category</param>
    private string AddField(FormFieldInfo ffi, string category, int position)
    {
		if (!MembershipContext.AuthenticatedUser.IsAuthorizedPerResource("cms.form", "EditForm"))
		{
			RedirectToAccessDenied("cms.form", "EditForm");
		}

        var dci = DataClassInfoProvider.GetDataClassInfo(ClassName);
        if (dci != null)
        {
            // Ensure the transaction
            using (var tr = new CMSLateBoundTransaction())
            {
                string tableName = dci.ClassTableName;
                string columnType = DataTypeManager.GetSqlType(ffi.DataType, ffi.Size, ffi.Precision);

                TableManager tm = new TableManager(dci.ClassConnectionString);
                tr.BeginTransaction();

                // Add new column
                tm.AddTableColumn(tableName, ffi.Name, columnType, true, null);

                // Add field to form  
                mFormInfo.AddFormItem(ffi);
                if (!String.IsNullOrEmpty(category) || position >= 0)
                {
                    mFormInfo.MoveFormFieldToPositionInCategory(ffi.Name, category, position);
                }

                // Update form definition
                dci.ClassFormDefinition = mFormInfo.GetXmlDefinition();

                // Update class schema
                dci.ClassXmlSchema = tm.GetXmlSchema(dci.ClassTableName);

                try
                {
                    // Save the class data
                    DataClassInfoProvider.SetDataClassInfo(dci);
                }
                catch (Exception)
                {
                    return GetString("FormBuilder.ErrorSavingForm");
                }

                // Generate default view
                SqlGenerator.GenerateDefaultView(dci, SiteContext.CurrentSiteName);
                QueryInfoProvider.ClearDefaultQueries(dci, true, true);

                // Hide field for alternative forms that require it
                string where = "FormClassID=" + dci.ClassID;
                where = SqlHelper.AddWhereCondition(where, "FormHideNewParentFields=1");

                var altforms = AlternativeFormInfoProvider.GetAlternativeForms(where, null);
                foreach (AlternativeFormInfo afi in altforms)
                {
                    afi.HideField(ffi);
                    AlternativeFormInfoProvider.SetAlternativeFormInfo(afi);
                }


                // Commit the transaction
                tr.Commit();
            }

            ClearHashtables();

            // Update inherited classes with new fields
            FormHelper.UpdateInheritedClasses(dci);
        }
        else
        {
            return GetString("FormBuilder.ErrorSavingForm");
        }

        return string.Empty;
    }


    /// <summary>
    /// Clears HashTables.
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

            foreach (FormFieldInfo ffi in fields)
            {
                string caption = ffi.Caption;
                if (String.IsNullOrEmpty(caption))
                {
                    caption = ffi.Name;
                }

                if (fields.Any(f => (f.Name != ffi.Name) && (f.Caption == caption)))
                {
                    // Add field name if more fields have similar caption
                    caption += String.Format(" [{0}]", ffi.Name);
                }
                result.Add(String.Format("{0};{1}", ffi.Guid, caption));
            }

            UIContext["CurrentFormFields"] = result;
            ViewState["CurrentFormFields"] = result;
        }
    }

    #endregion


    #region "BaseFieldEditor methods"

    /// <summary>
    /// Indicates whether such a change has been made to the field, it requires database structure update.
    /// </summary>
    /// <param name="oldFieldInfo">Field prior to the change.</param>
    /// <param name="updatedFieldInfo">Field after the change.</param>
    /// <returns></returns>
    protected static bool IsDatabaseChangeRequired(FormFieldInfo oldFieldInfo, FormFieldInfo updatedFieldInfo)
    {
        if ((oldFieldInfo == null) || (updatedFieldInfo == null))
        {
            return false;
        }

        bool originalIsMacro;
        string originalDefaultValue = oldFieldInfo.GetPropertyValue(FormFieldPropertyEnum.DefaultValue, out originalIsMacro);

        bool isMacro;
        string defaultValue = updatedFieldInfo.GetPropertyValue(FormFieldPropertyEnum.DefaultValue, out isMacro);

        return (oldFieldInfo.Name != updatedFieldInfo.Name) ||
               (oldFieldInfo.DataType != updatedFieldInfo.DataType) ||
               (oldFieldInfo.Size != updatedFieldInfo.Size) ||
               (oldFieldInfo.AllowEmpty != updatedFieldInfo.AllowEmpty) ||
               (originalDefaultValue != defaultValue) ||
               (originalIsMacro != isMacro);
    }


    /// <summary>
    /// Updates database representation of given form field
    /// </summary>
    /// <param name="oldFieldInfo">Form field prior to the change</param>
    /// <param name="updatedFieldInfo">Form field after the change has been made.</param>
    /// <param name="tm">Table manager allowing the database changes.</param>
    /// <param name="tableName">Name of the table to be changed.</param>
    /// <returns>Possible error message</returns>
    protected string UpdateDatabaseColumn(FormFieldInfo oldFieldInfo, FormFieldInfo updatedFieldInfo, TableManager tm, string tableName)
    {
        if (updatedFieldInfo.External)
        {
            if (!oldFieldInfo.External)
            {
                tm.DropTableColumn(tableName, oldFieldInfo.Name);
            }
        }
        else
        {
            // Validate the default value
            string errorMessage;
            string newDBDefaultValue = GetDefaultValueInDBCulture(updatedFieldInfo, out errorMessage);
            if (!String.IsNullOrEmpty(errorMessage))
            {
                return errorMessage;
            }

            // Set column type and size
            string newColumnType = DataTypeManager.GetSqlType(updatedFieldInfo.DataType, updatedFieldInfo.Size, updatedFieldInfo.Precision);
            string oldColumnName = oldFieldInfo.Name;
            string newColumnName = updatedFieldInfo.Name;

            if (oldFieldInfo.External)
            {
                tm.AddTableColumn(tableName, newColumnName, newColumnType, updatedFieldInfo.AllowEmpty, newDBDefaultValue);
            }
            else
            {
                // Change table column
                tm.AlterTableColumn(tableName, oldColumnName, newColumnName, newColumnType, updatedFieldInfo.AllowEmpty, newDBDefaultValue);
            }
        }

        return null;
    }


    /// <summary>
    /// Returns default value set for given form field or a default value according to the field data type if required. This default value will be in the same culture as the database.
    /// </summary>
    /// <param name="fieldInfo">Form field with default value</param>
    /// <param name="errorMessage">Error that occurred during stored default value validation.</param>
    /// <param name="forceDefaultValue">Indicates whether data type default value should be used when no default value is stored in the form field.</param>
    protected static string GetDefaultValueInDBCulture(FormFieldInfo fieldInfo, out string errorMessage, bool forceDefaultValue = true)
    {
        errorMessage = null;
        bool isMacro;
        string defaultValue = fieldInfo.GetPropertyValue(FormFieldPropertyEnum.DefaultValue, out isMacro);

        // Set implicit default value
        if (!fieldInfo.AllowEmpty && String.IsNullOrEmpty(defaultValue))
        {
            if (forceDefaultValue)
            {
                defaultValue = GetColumnDefaultValue(fieldInfo.DataType);
            }
        }
        else
        {
            // Check if default value is in required format
            defaultValue = ValidateColumnDefaultValue(fieldInfo.DataType, defaultValue, isMacro, out errorMessage);
        }

        string result = defaultValue;

        // Check if it is not a macro
        if (!isMacro && String.IsNullOrEmpty(errorMessage))
        {
            switch (fieldInfo.DataType)
            {
                case FieldDataType.Double:
                    result = FormHelper.GetDoubleValueInDBCulture(defaultValue);
                    break;

                case FieldDataType.DateTime:
                    result = FormHelper.GetDateTimeValueInDBCulture(defaultValue);
                    break;

                default:
                    result = defaultValue;
                    break;
            }
        }

        return result;
    }


    private static string GetColumnDefaultValue(string dataType)
    {
        string result = null;

        switch (dataType)
        {
            case FieldDataType.Integer:
            case FieldDataType.LongInteger:
            case FieldDataType.Double:
            case FieldDataType.Boolean:
                result = "0";
                break;

            case FieldDataType.Text:
            case FieldDataType.LongText:
                result = string.Empty;
                break;

            case FieldDataType.DateTime:
            case FieldDataType.Date:
                result = DateTime.Now.ToString();
                break;

            case FieldDataType.TimeSpan:
                result = TimeSpan.Zero.ToString();
                break;

            case FieldDataType.File:
            case FieldDataType.Guid:
                // 32 digits, empty Guid
                result = Guid.Empty.ToString();
                break;

            case FieldDataType.Binary:
                break;
        }

        return result;
    }


    private static string ValidateColumnDefaultValue(string dataType, string defaultValue, bool isMacro, out string errorMessage)
    {
        string newColumnDefaultValue = null;
        errorMessage = null;

        // If default value is macro, don't try to ensure the type
        if (!String.IsNullOrEmpty(defaultValue) && !isMacro)
        {
            // Check if default value is in required format
            switch (dataType)
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
                        errorMessage = ResHelper.GetString("TemplateDesigner.ErrorDefaultValueInteger");
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
                        errorMessage = ResHelper.GetString("TemplateDesigner.ErrorDefaultValueLongInteger");
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
                        errorMessage = ResHelper.GetString("TemplateDesigner.ErrorDefaultValueDouble");
                    }
                    break;

                case FieldDataType.DateTime:
                case FieldDataType.Date:
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
                            errorMessage = ResHelper.GetString("TemplateDesigner.ErrorDefaultValueDateTime");
                        }
                    }
                    break;

                case FieldDataType.TimeSpan:
                    try
                    {
                        TimeSpan time = TimeSpan.Parse(defaultValue);
                        newColumnDefaultValue = time.ToString();
                    }
                    catch
                    {
                        newColumnDefaultValue = TimeSpan.Zero.ToString();
                        errorMessage = ResHelper.GetString("TemplateDesigner.ErrorDefaultValueTimeSpan");
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
                        errorMessage = ResHelper.GetString("TemplateDesigner.ErrorDefaultValueGuid");
                    }
                    break;

                case FieldDataType.LongText:
                case FieldDataType.Text:
                case FieldDataType.Boolean:

                    newColumnDefaultValue = defaultValue;
                    break;
            }
        }

        return newColumnDefaultValue;
    }

    #endregion


    #region "ICallbackEventHandler Members"

    /// <summary>
    /// Callback result retrieving handler.
    /// </summary>
    public string GetCallbackResult()
    {
        return mCallbackResult;
    }


    /// <summary>
    /// Raise callback method.
    /// </summary>
    public void RaiseCallbackEvent(string eventArgument)
    {
        if (!string.IsNullOrEmpty(eventArgument) && (mFormInfo != null))
        {
            string[] data = eventArgument.Split(':');

            // Check that data are in proper format
            if (data.Length >= 3)
            {
                switch (data[0])
                {
                    case "move":
                        int position = ValidationHelper.GetInteger(data[3], -1);
                        string category = data[2];
                        string fieldName = data[1];

                        // Check field existence
                        FormFieldInfo field = mFormInfo.GetFormField(fieldName);
                        string errorMessage;
                        if (field != null)
                        {
                            // Move field to new position
                            mFormInfo.MoveFormFieldToPositionInCategory(fieldName, category, position);
                            errorMessage = SaveFormDefinition();
                        }
                        else
                        {
                            errorMessage = GetString("editedobject.notexists");
                        }
                        mCallbackResult = PrepareCallbackResult(string.Empty, errorMessage);
                        break;
                }
            }
        }
    }


    /// <summary>
    /// Returns mCallbackResult or errorMessage in proper format if not empty.
    /// </summary>
    /// <param name="callbackResult">String which will be returned if errorMessage is empty</param>
    /// <param name="errorMessage">Error message</param>
    private string PrepareCallbackResult(string callbackResult, string errorMessage)
    {
        return string.IsNullOrEmpty(errorMessage) ? callbackResult : "error:" + String.Format("{0} {1}", errorMessage, GetString("formbuilder.refresh")).Replace(":", "##COLON##");
    }

    #endregion
}