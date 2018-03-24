using System;

using CMS.Base;
using CMS.DataEngine;
using CMS.ExtendedControls;
using CMS.FormControls;
using CMS.FormEngine;
using CMS.Helpers;
using CMS.MacroEngine;
using CMS.Membership;

/// <summary>
/// Another blank form control must be registered to hold second value.
/// </summary>
public partial class CMSFormControls_System_OptionsSelector : FormEngineUserControl
{
    #region "Variables"

    private bool disabledSql = false;

    #endregion

    #region "Properties"

    /// <summary>
    /// Gets or sets the enabled state of the control.
    /// </summary>
    public override bool Enabled
    {
        get
        {
            return txtValue.Enabled;
        }
        set
        {
            txtValue.Enabled = value;
            lstOptions.Enabled = value;
        }
    }


    /// <summary>
    /// Gets or sets form control value.
    /// </summary>
    public override object Value
    {
        get
        {
            // Options
            if (lstOptions.SelectedIndex == ListSourceIndex)
            {
                return txtValue.Text.Trim();
            }
            return null;
        }
        set
        {
            txtValue.Text = LoadTextFromData(ValidationHelper.GetString(value, null));
        }
    }


    /// <summary>
    /// Name of the column to be used for SQL query
    /// </summary>
    public string QueryColumnName
    {
        get
        {
            return GetValue("QueryColumnName", "query");
        }
        set
        {
            SetValue("QueryColumnName", value);
        }
    }


    /// <summary>
    /// Name of the column to be used for macro data source
    /// </summary>
    public string MacroColumnName
    {
        get
        {
            return GetValue("MacroColumnName", "macro");
        }
        set
        {
            SetValue("MacroColumnName", value);
        }
    }


    /// <summary>
    /// Determines whether the Query data source option is allowed.
    /// </summary>
    public bool AllowQuery
    {
        get
        {
            return GetValue("AllowQuery", true);
        }
        set
        {
            SetValue("AllowQuery", value);
        }
    }


    /// <summary>
    /// Determines whether the Query option is allowed.
    /// </summary>
    public bool AllowMacro
    {
        get
        {
            return GetValue("AllowMacro", true);
        }
        set
        {
            SetValue("AllowMacro", value);
        }
    }

    #endregion


    #region "Private properties"

    /// <summary>
    /// Index of macro source in the list box.
    /// </summary>
    private int MacroSourceIndex
    {
        get;
        set;
    }


    /// <summary>
    /// Index of list source in the list box.
    /// </summary>
    private int ListSourceIndex
    {
        get;
        set;
    }


    /// <summary>
    /// Index of SQL source in the list box.
    /// </summary>
    private int SqlSourceIndex
    {
        get;
        set;
    }


    /// <summary>
    /// Index of None source in the list box.
    /// </summary>
    private int NoneSourceIndex
    {
        get;
        set;
    }


    /// <summary>
    /// Determines whether the list box is initialized
    /// </summary>
    private bool ListBoxInitialized
    {
        get;
        set;
    }

    #endregion


    #region "Control events"

    /// <summary>
    /// Page load.
    /// </summary>
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!StopProcessing)
        {
            CheckFieldEmptiness = false;

            // Control is used outside the form
            if ((Form == null) || (Form.Data == null))
            {
                ClearControl();
            }
            else
            {
                InitOptions();

                // Help icon
                spanScreenReader.ResourceString = "optionselector.help";

                imgHelp.ToolTip = ScriptHelper.FormatTooltipString(GetString("optionselector.help"), false, false);

                // Enable HTML formating in tooltip
                imgHelp.Attributes.Add("data-html", "true");

                // Register bootstrap tooltip over help icons
                ScriptHelper.RegisterBootstrapTooltip(Page, ".info-icon > i");

            }
        }
        else
        {
            Visible = false;
        }
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        // For BizForm
        if ((Form != null) && (Form.FormType == FormTypeEnum.BizForm))
        {
            // Check permissions
            if (!MembershipContext.AuthenticatedUser.IsAuthorizedPerResource("CMS.Form", "EditSQLQueries") && (SqlSourceIndex > 0))
            {
                // Disable SQL source option
                lstOptions.Items[SqlSourceIndex].Enabled = false;
                if (lstOptions.SelectedIndex == SqlSourceIndex)
                {
                    // ... if selected disable editable text area too
                    txtValue.Enabled = false;
                    string enableScript = String.Format("if ({0} != null) {{{0}.setOption('readOnly', false); document.getElementById('{2}').disabled = false; }} else {{document.getElementById('{1}').disabled = false;}}", txtValue.Editor.EditorID, txtValue.ClientID, txtValue.Editor.ClientID);
                    lstOptions.Items[ListSourceIndex].Attributes["onclick"] = enableScript;
                    if (MacroSourceIndex > 0)
                    {
                        lstOptions.Items[MacroSourceIndex].Attributes["onclick"] = enableScript;
                    }
                }
                else
                {
                    lstOptions.Items.RemoveAt(SqlSourceIndex);
                }
            }
        }

        // Init text area
        if (lstOptions.SelectedIndex == MacroSourceIndex)
        {
            txtValue.Editor.Language = LanguageEnum.CSharp;
        }
        else if (lstOptions.SelectedIndex == SqlSourceIndex)
        {
            txtValue.Editor.Language = LanguageEnum.SQL;
        }
        else
        {
            txtValue.Editor.Language = LanguageEnum.Text;
        }
        txtValue.UseAutoComplete = (lstOptions.SelectedIndex == MacroSourceIndex);
    }

    #endregion


    #region "Public methods"

    /// <summary>
    /// Returns true if user control is valid.
    /// </summary>
    public override bool IsValid()
    {
        bool valid = true;

        // Check 'options' validity
        if (lstOptions.SelectedIndex == ListSourceIndex)
        {
            // Some option must be included
            if (string.IsNullOrEmpty(txtValue.Text.Trim()))
            {
                ValidationError += GetString("TemplateDesigner.ErrorDropDownListOptionsEmpty") + " ";
                valid = false;
            }
            else
            {
                // Parse lines
                int lineIndex = 0;
                string[] lines = txtValue.Text.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);

                for (lineIndex = 0; lineIndex <= lines.GetUpperBound(0); lineIndex++)
                {
                    // Loop through only not-empty lines
                    if ((lines[lineIndex] != null) && (lines[lineIndex].Trim() != string.Empty))
                    {
                        // Get line items
                        string[] items = lines[lineIndex].Replace(SpecialFieldsDefinition.SEMICOLON_TO_REPLACE, SpecialFieldsDefinition.REPLACED_SEMICOLON).Split(';');

                        // Every line must have value and item element and optionally visibility macro
                        if (items.Length > 3)
                        {
                            ValidationError += GetString("TemplateDesigner.ErrorDropDownListOptionsInvalidFormat") + " ";
                            valid = false;
                            break;
                        }
                        else
                        {
                            // Check for possible macro (with/without visibility condition)
                            if (items.Length <= 2)
                            {
                                string specialMacro = items[0].Trim();

                                // Check if special macro is used
                                if (SpecialFieldsDefinition.IsSpecialFieldMacro(specialMacro))
                                {
                                    string macro = (items.Length == 2) ? items[1].Trim() : String.Empty;

                                    // If special field macro used and second item isn't macro show error
                                    if (!String.IsNullOrEmpty(macro) && !MacroProcessor.ContainsMacro(macro))
                                    {
                                        ValidationError += string.Format(GetString("TemplateDesigner.ErrorDropDownListOptionsInvalidMacroFormat"), lineIndex + 1) + " ";
                                        valid = false;
                                        break;
                                    }
                                }
                            }

                            // Check valid Double format
                            if (FieldInfo.DataType == FieldDataType.Double)
                            {
                                if (!ValidationHelper.IsDouble(items[0]) && !(FieldInfo.AllowEmpty && string.IsNullOrEmpty(items[0])))
                                {
                                    ValidationError += string.Format(GetString("TemplateDesigner.ErrorDropDownListOptionsInvalidDoubleFormat"), lineIndex + 1) + " ";
                                    valid = false;
                                    break;
                                }
                            }
                            // Check valid Integer format
                            else if (FieldInfo.DataType == FieldDataType.Integer)
                            {
                                if (!ValidationHelper.IsInteger(items[0]) && !(FieldInfo.AllowEmpty && string.IsNullOrEmpty(items[0])))
                                {
                                    ValidationError += string.Format(GetString("TemplateDesigner.ErrorDropDownListOptionsInvalidIntFormat"), lineIndex + 1) + " ";
                                    valid = false;
                                    break;
                                }
                            }
                            // Check valid Long integer format
                            else if (FieldInfo.DataType == FieldDataType.LongInteger)
                            {
                                if (!ValidationHelper.IsLong(items[0]) && !(FieldInfo.AllowEmpty && string.IsNullOrEmpty(items[0])))
                                {
                                    ValidationError += string.Format(GetString("TemplateDesigner.ErrorDropDownListOptionsInvalidLongIntFormat"), lineIndex + 1) + " ";
                                    valid = false;
                                    break;
                                }
                            }
                            // Check valid Date time format
                            else if (FieldInfo.DataType == FieldDataType.DateTime)
                            {
                                if ((ValidationHelper.GetDateTime(items[0], DateTimeHelper.ZERO_TIME) == DateTimeHelper.ZERO_TIME) && !FieldInfo.AllowEmpty)
                                {
                                    ValidationError += string.Format(GetString("TemplateDesigner.ErrorDropDownListOptionsInvalidDateTimeFormat"), lineIndex + 1) + " ";
                                    valid = false;
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }
        // Check SQL query validity
        else if ((lstOptions.SelectedIndex == SqlSourceIndex) && (string.IsNullOrEmpty(txtValue.Text.Trim())))
        {
            ValidationError += GetString("TemplateDesigner.ErrorDropDownListQueryEmpty") + " ";
            valid = false;
        }
        else if ((lstOptions.SelectedIndex == MacroSourceIndex) && (string.IsNullOrEmpty(txtValue.Text.Trim())))
        {
            ValidationError += GetString("TemplateDesigner.ErrorDropDownListMacroEmpty") + " ";
            valid = false;
        }

        return valid;
    }


    /// <summary>
    /// Returns the value of the given property.
    /// </summary>
    /// <param name="propertyName">Property name</param>
    public override object GetValue(string propertyName)
    {
        if (propertyName.EqualsCSafe("DataSourceType", StringComparison.InvariantCultureIgnoreCase))
        {
            return lstOptions.SelectedValue;
        }
        return base.GetValue(propertyName);
    }


    /// <summary>
    /// Returns values of other related fields.
    /// </summary>
    public override object[,] GetOtherValues()
    {
        object[,] values = new object[2, 2];
        values[0, 0] = QueryColumnName;
        values[1, 0] = MacroColumnName;

        // SQL
        if (lstOptions.SelectedIndex == SqlSourceIndex)
        {
             // Keep original SQL value
            if (disabledSql)
            {
                values[0, 1] = Form.Data.GetValue(QueryColumnName);
                values[1, 1] = null;
            }
            else
            {
                values[0, 1] = txtValue.Text.Trim();
                values[1, 1] = null;
            }
        }
        else if (lstOptions.SelectedIndex == MacroSourceIndex)
        {
            values[0, 1] = null;
            values[1, 1] = txtValue.Text.Trim();
        }
        else
        {
            values[0, 1] = null;
            values[1, 1] = null;
        }

        return values;
    }

    #endregion


    #region "Private methods"

    /// <summary>
    /// Initialized shown options in the list box, correctly initialized the indexes of each options for further reference.
    /// </summary>
    private void InitOptions()
    {
        if (!ListBoxInitialized)
        {
            ListBoxInitialized = true;

            // Default indexes when all the options are visible
            NoneSourceIndex = 0;
            ListSourceIndex = 1;
            MacroSourceIndex = 2;
            SqlSourceIndex = 3;

            // Remove SQL option if there is not a field to store the query
            if (!Form.Data.ContainsColumn(QueryColumnName) || !AllowQuery)
            {
                lstOptions.Items.RemoveAt(SqlSourceIndex);
                SqlSourceIndex = -1;
            }
            // Remove macro option if there is not a field to store the macro
            if (!Form.Data.ContainsColumn(MacroColumnName) || !AllowMacro)
            {
                lstOptions.Items.RemoveAt(MacroSourceIndex);
                SqlSourceIndex--;
                MacroSourceIndex = -1;
            }

            // Hide None option for field not allowing the empty value
            if ((FieldInfo != null) && !FieldInfo.AllowEmpty)
            {
                lstOptions.Items.RemoveAt(NoneSourceIndex);
                ListSourceIndex--;
                MacroSourceIndex--;
                SqlSourceIndex--;
                NoneSourceIndex = -1;
            }
        }
    }


    /// <summary>
    /// Loads text into textbox from value or from 'QueryColumnName' column.
    /// </summary>
    /// <param name="value">Value parameter</param>
    /// <returns>Returns text of options or query</returns>
    private string LoadTextFromData(string value)
    {
        InitOptions();

        // Options data 
        if (!String.IsNullOrEmpty(value))
        {
            lstOptions.SelectedIndex = ListSourceIndex;

            // Get string representation
            SpecialFieldsDefinition def = new SpecialFieldsDefinition(null, FieldInfo, ContextResolver);
            def.LoadFromText(value);
            return def.ToString();
        }
        // Query selected
        else if (ContainsColumn(QueryColumnName))
        {
            string query = ValidationHelper.GetString(Form.Data.GetValue(QueryColumnName), string.Empty).Trim();
            if (!String.IsNullOrEmpty(query))
            {
                lstOptions.SelectedIndex = SqlSourceIndex;
                if (!MembershipContext.AuthenticatedUser.IsAuthorizedPerResource("CMS.Form", "EditSQLQueries"))
                {
                    disabledSql = true;
                }
                return query;
            }
            // Macro data source selected
            else if (ContainsColumn(MacroColumnName))
            {
                string macro = ValidationHelper.GetString(Form.Data.GetValue(MacroColumnName), string.Empty).Trim();
                if (!String.IsNullOrEmpty(macro))
                {
                    lstOptions.SelectedIndex = MacroSourceIndex;
                    return macro;
                }
            }
        }

        return null;
    }


    /// <summary>
    /// Resets control's state.
    /// </summary>
    private void ClearControl()
    {
        lstOptions.SelectedIndex = 0;
        txtValue.Text = null;
    }

    #endregion
}