using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Web.UI.WebControls;

using CMS.DataEngine;
using CMS.ExtendedControls;
using CMS.Helpers;
using CMS.IO;
using CMS.Base;
using CMS.UIControls;

public partial class CMSModules_SystemTables_Controls_Views_SQLEdit : CMSUserControl
{
    #region "Constants"

    private const int STATE_ALTER_VIEW = 1; // Existing view will be modified
    private const int STATE_CREATE_VIEW = 2; // New view will be created
    private const int STATE_ALTER_PROCEDURE = 3; // Existing stored procedure will be modified
    private const int STATE_CREATE_PROCEDURE = 4; // Mew stored procedure will be created
    private const string PROCEDURE_CUSTOM_PREFIX = "Proc_Custom_";
    private const string VIEW_CUSTOM_PREFIX = "View_Custom_";

    #endregion


    #region "Public properties"

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
            plcMess.IsLiveSite = value;
            base.IsLiveSite = value;
        }
    }


    /// <summary>
    /// Gets or sets type of database object (view/stored procedure).
    /// </summary>
    public bool? IsView
    {
        get;
        set;
    }


    /// <summary>
    /// Gets or sets name of the database object (view/stored procedure).
    /// </summary>
    public string ObjectName
    {
        get;
        set;
    }


    /// <summary>
    /// Indicates whether parsing code of existing view/procedure failed/
    /// </summary>
    public bool FailedToLoad
    {
        get;
        set;
    }


    /// <summary>
    /// Gets or sets.
    /// </summary>
    public bool HideSaveButton
    {
        get;
        set;
    }


    /// <summary>
    /// Indicates whether rollback functionality is available for current object.
    /// </summary>
    public bool RollbackAvailable
    {
        get
        {
            string fName = null;
            return SQLScriptExists(ObjectName, ref fName);
        }
    }

    #endregion


    #region "Private properties"

    /// <summary>
    /// Current state of editing control (alter view, create view, alter procedure, create procedure).
    /// </summary>
    private int State
    {
        get
        {
            return ValidationHelper.GetInteger(ViewState["State"], 0);
        }
        set
        {
            ViewState["State"] = value;
        }
    }

    #endregion


    #region "Events"

    /// <summary>
    /// Event rised when SQL code is successfully saved.
    /// </summary>
    public event EventHandler OnSaved;

    #endregion


    protected void Page_Load(object sender, EventArgs e)
    {
        if (!StopProcessing)
        {
            // Set max length of object name (according to development mode)
            if (SystemContext.DevelopmentMode)
            {
                txtObjName.MaxLength = 128;
            }
            else
            {
                if (IsView == true)
                {
                    txtObjName.MaxLength = 128 - VIEW_CUSTOM_PREFIX.Length;
                }
                else
                {
                    txtObjName.MaxLength = 128 - PROCEDURE_CUSTOM_PREFIX.Length;
                }
            }


            btnOk.Visible = !HideSaveButton;

            if (!String.IsNullOrEmpty(ObjectName) && !FailedToLoad)
            {
                if (IsView == true)
                {
                    if (!ObjectName.StartsWithCSafe(VIEW_CUSTOM_PREFIX, true))
                    {
                        ShowWarning(GetString("systbl.view.notsystemview"), null, null);
                    }
                }
                else if (IsView == false)
                {
                    if (!ObjectName.StartsWithCSafe(PROCEDURE_CUSTOM_PREFIX, true))
                    {
                        ShowWarning(GetString("systbl.view.notsystemproc"), null, null);
                    }
                }
            }
        }
    }


    #region "Event handlers"

    /// <summary>
    /// Generate default query.
    /// </summary>
    protected void btnGenerate_Click(object sender, EventArgs e)
    {
        switch (IsView)
        {
            case true:
                txtObjName.Text = (SystemContext.DevelopmentMode ? VIEW_CUSTOM_PREFIX : String.Empty) + "MyView";
                txtSQLText.Text = "SELECT * FROM CMS_Document";
                break;

            case false:
                txtObjName.Text = (SystemContext.DevelopmentMode ? PROCEDURE_CUSTOM_PREFIX : String.Empty) + "MyProcedure";
                txtParams.Text = " @MyIntegerVar int," + Environment.NewLine +
                                 " @MyStringVar nvarchar(50)";
                txtSQLText.Text = "SELECT 1";
                break;

            default:
                break;
        }
    }


    /// <summary>
    /// Saves data of edited or new query into DB.
    /// </summary>
    protected void btnOK_Click(object sender, EventArgs e)
    {
        SaveQuery();
    }


    /// <summary>
    /// Initializes the controls. Returns false if parsing code of existing view/procedure failed.
    /// </summary>
    public bool SetupControl()
    {
        return SetupControl(null);
    }

    #endregion


    #region "Private methods"

    /// <summary>
    /// Initializes the controls. Returns false if parsing code of existing view/procedure failed.
    /// </summary>
    private bool SetupControl(string code)
    {
        bool result = true;

        if (!String.IsNullOrEmpty(ObjectName) && String.IsNullOrEmpty(code))
        {
            if (IsView != null)
            {
                TableManager tm = new TableManager(null);

                code = tm.GetCode(ObjectName);
            }
        }

        if (IsView == true)
        {
            chkWithBinding.Visible = true;
            plcGenerate.Visible = true;
            if (code == null)
            {
                lblCreateLbl.Text = "CREATE VIEW " + (!SystemContext.DevelopmentMode ? VIEW_CUSTOM_PREFIX : String.Empty);
                plcGenerate.Visible = true;
                State = STATE_CREATE_VIEW;
            }
            else
            {
                lblCreateLbl.Text = "ALTER VIEW";
                plcGenerate.Visible = false;
                string name, body;
                bool withbinding;
                result = ParseView(code, out name, out body, out withbinding);
                txtObjName.Enabled = false;
                txtObjName.ReadOnly = true;
                txtObjName.Text = name;
                txtSQLText.Text = body;
                chkWithBinding.Checked = withbinding;
                State = STATE_ALTER_VIEW;
            }

            plcParams.Visible = false;
            lblBegin.Text = "AS";
        }
        else
        {
            if (code == null)
            {
                lblCreateLbl.Text = "CREATE PROCEDURE " + (!SystemContext.DevelopmentMode ? PROCEDURE_CUSTOM_PREFIX : String.Empty);
                plcGenerate.Visible = true;
                State = STATE_CREATE_PROCEDURE;
            }
            else
            {
                plcGenerate.Visible = false;
                lblCreateLbl.Text = "ALTER PROCEDURE";
                string name, param, body;
                result = ParseProcedure(code, out name, out param, out body);
                txtObjName.Enabled = false;
                txtObjName.ReadOnly = true;
                txtObjName.Text = name;
                txtParams.Text = param;
                txtSQLText.Text = body;
                State = STATE_ALTER_PROCEDURE;
            }
            plcParams.Visible = true;
            lblBegin.Text = "AS<br/>BEGIN";
            lblEnd.Text = "END";
        }

        if (!result)
        {
            // Parsing code failed => disable all controls
            DisableControl(txtObjName);
            DisableControl(txtParams);
            txtSQLText.EditorMode = EditorModeEnum.Basic;
            DisableControl(txtSQLText);
            btnGenerate.Enabled = false;

            ShowWarning(GetString((IsView == true) ? "systbl.view.parsingfailed" : "systbl.proc.parsingfailed"), null, null);
        }

        FailedToLoad = !result;
        return result;
    }


    private void DisableControl(TextBox txt)
    {
        txt.ReadOnly = true;
        txt.Enabled = false;
    }


    /// <summary>
    /// Runs edited view or stored procedure.
    /// </summary>
    public void SaveQuery()
    {
        string objName = txtObjName.Text.Trim();
        string body = txtSQLText.Text.Trim();

        string result = new Validator().NotEmpty(objName, GetString("systbl.viewproc.objectnameempty"))
            .NotEmpty(body, GetString("systbl.viewproc.bodyempty")).Result;

        if (String.IsNullOrEmpty(result))
        {
            // Use special prefix for user created views or stored procedures
            if (!SystemContext.DevelopmentMode)
            {
                if (State == STATE_CREATE_VIEW)
                {
                    objName = VIEW_CUSTOM_PREFIX + objName;
                    result = new Validator().IsIdentifier(objName, GetString("systbl.viewproc.viewnotidentifierformat")).Result;
                }
                if (State == STATE_CREATE_PROCEDURE)
                {
                    objName = PROCEDURE_CUSTOM_PREFIX + objName;
                    result = new Validator().IsIdentifier(objName, GetString("systbl.viewproc.procnotidentifierformat")).Result;
                }
            }

            if (String.IsNullOrEmpty(result))
            {
                try
                {
                    // Retrieve user friendly name
                    string schema, name;
                    ParseName(objName, out schema, out name);

                    // Prepare parameters for stored procedure
                    string param = txtParams.Text.Trim();
                    string query = "";
                    bool binding = chkWithBinding.Checked;

                    TableManager tm = new TableManager(null);

                    switch (State)
                    {
                        case STATE_CREATE_VIEW:
                            query = String.Format("CREATE VIEW {0}{1}\nAS\n{2}", objName, (binding ? " WITH SCHEMABINDING" : ""), body);
                            // Check if view exists
                            if (tm.ViewExists(objName))
                            {
                                result = String.Format(GetString("systbl.view.alreadyexists"), objName);
                            }
                            break;

                        case STATE_ALTER_VIEW:
                            query = String.Format("ALTER VIEW {0}{1}\nAS\n{2}", objName, (binding ? " WITH SCHEMABINDING" : ""), body);
                            break;

                        case STATE_CREATE_PROCEDURE:
                            query = String.Format("CREATE PROCEDURE {0}\n{1}\nAS\nBEGIN\n{2}\nEND\n", objName, param, body);
                            // Check if stored procedure exists
                            if (tm.GetCode(objName) != null)
                            {
                                result = String.Format(GetString("systbl.proc.alreadyexists"), objName);
                            }
                            break;

                        case STATE_ALTER_PROCEDURE:
                            query = String.Format("ALTER PROCEDURE {0}\n{1}\nAS\nBEGIN\n{2}\nEND\n", objName, param, body);
                            break;
                    }

                    if (String.IsNullOrEmpty(result))
                    {
                        ConnectionHelper.ExecuteQuery(query, null, QueryTypeEnum.SQLQuery);

                        ObjectName = name;

                        if (OnSaved != null)
                        {
                            OnSaved(this, EventArgs.Empty);
                        }
                    }
                }
                catch (Exception e)
                {
                    result = e.Message;
                }
            }
        }

        // Show error message if any
        if (!String.IsNullOrEmpty(result))
        {
            ShowError(result);
        }
    }


    /// <summary>
    /// Check if exists SQL script for this object in /App_Data/Install
    /// </summary>
    /// <param name="objName">Name of the object</param>
    private bool SQLScriptExists(string objName, ref string fileName)
    {
        if (!String.IsNullOrEmpty(objName))
        {
            fileName = SqlInstallationHelper.GetSQLInstallPathToObjects() + "\\" + objName + ".sql";

            return File.Exists(fileName);
        }
        fileName = null;

        return TableManager.IsGeneratedSystemView(objName);
    }


    /// <summary>
    /// Loads code of original view/stored procedure from SQL script.
    /// </summary>
    public void Rollback()
    {
        if ((State == STATE_ALTER_VIEW) || (State == STATE_ALTER_PROCEDURE))
        {
            string fileName = null;
            if (SQLScriptExists(ObjectName, ref fileName))
            {
                if (!String.IsNullOrEmpty(fileName))
                {
                    // Rollback object from file
                    Regex re = null;
                    if (IsView == true)
                    {
                        re = RegexHelper.GetRegex("\\s*CREATE\\s+VIEW\\s+", true);
                    }
                    else
                    {
                        re = RegexHelper.GetRegex("\\s*CREATE\\s+PROCEDURE\\s+", true);
                    }

                    // Load SQL script
                    string query = File.ReadAllText(fileName);

                    // Split query to parts separated by "GO" (trying to find part containing CREATE VIEW or CREATE PROCEDURE)
                    int startingIndex = 0;
                    string partOfQuery = "";
                    do
                    {
                        int index = query.IndexOfCSafe("GO" + Environment.NewLine, startingIndex, true);
                        if (index == -1)
                        {
                            index = query.IndexOfCSafe(Environment.NewLine + "GO", startingIndex, true);
                            if (index == -1)
                            {
                                index = query.Length;
                            }
                        }

                        // Try to find CREATE VIEW or CREATE PROCEDURE
                        partOfQuery = query.Substring(startingIndex, index - startingIndex).Trim();
                        if (!String.IsNullOrEmpty(partOfQuery) && re.IsMatch(partOfQuery))
                        {
                            SetupControl(partOfQuery);
                            txtObjName.Text = ObjectName;
                            break;
                        }
                        startingIndex = index + 3;
                    } while (startingIndex < query.Length);
                }
                else
                {
                    // Rollback object from system
                    string indexes = null;
                    string query = SqlGenerator.GetSystemViewSqlQuery(ObjectName, out indexes);

                    SetupControl(query);
                }
            }
            else
            {
                ShowError(GetString("systbl.unabletorollback"));
            }
        }
    }

    #endregion


    /// <summary>
    /// Extracts view name and code from SQL query.
    /// </summary>
    /// <param name="query">Entire SQL query from DB</param>
    /// <param name="name">View name</param>
    /// <param name="body">View body</param>
    /// <param name="withBinding">WITHBINDING modifier</param>
    private bool ParseView(string query, out string name, out string body, out bool withBinding)
    {
        name = null;
        body = null;
        withBinding = false;
        Regex re = RegexHelper.GetRegex(@".*?\s*(CREATE\s+VIEW)\s+(\S+)\s+((\s|[a-zA-Z])*?)AS(\s+.*)\s*", RegexOptions.Singleline | CMSRegex.IgnoreCase);
        if (re.IsMatch(query))
        {
            Match m = re.Match(query);
            name = m.Groups[2].Value;
            withBinding = m.Groups[3].Value.ToLowerCSafe().Contains("with schemabinding");
            body = m.Groups[5].Value;
            return true;
        }
        return false;
    }


    /// <summary>
    /// Extracts stored procedure name and body from SQL query.
    /// </summary>
    /// <param name="query">Entire SQL query from DB</param>
    /// <param name="name">Procedure name</param>
    /// <param name="param">Parameters</param>
    /// <param name="body">Procedure body</param>
    private bool ParseProcedure(string query, out string name, out string param, out string body)
    {
        name = null;
        param = null;
        body = null;
        Regex re = RegexHelper.GetRegex(@".*?\s*(CREATE\s+PROCEDURE)\s+(\S+)\s+(.*)\s+(AS\s+BEGIN)(\s+.*)\s+(END)\s*", RegexOptions.Singleline | CMSRegex.IgnoreCase);
        if (re.IsMatch(query))
        {
            Match m = re.Match(query);
            name = m.Groups[2].Value;
            param = m.Groups[3].Value;
            body = m.Groups[5].Value;
            return true;
        }
        return false;
    }


    /// <summary>
    /// Parses DB object name considering database schema.
    /// </summary>
    /// <param name="objName">Object name (view or stored procedure)</param>
    /// <param name="schema">DB schema</param>
    /// <param name="name">Object name</param>
    private void ParseName(string objName, out string schema, out string name)
    {
        List<string> list = new List<string>();
        char current;
        char? next;
        int length = objName.Length;
        int state = 0;
        string buff = "";

        // Loop through object name and extract text in brackets [text1].[text2]
        // or text separated by period text1.text2 ("]]" is considered to be escape sequence for "]")
        for (int i = 0; i < length; i++)
        {
            current = objName[i];
            next = (i < (length - 1)) ? (char?)objName[i + 1] : null;

            switch (current)
            {
                case '[':
                    switch (state)
                    {
                        case 0:
                            state = 1; // Currently in bracket
                            break;
                        case 1:
                            buff += current;
                            break;
                    }
                    break;
                case ']':
                    if (state == 1)
                    {
                        switch (next)
                        {
                            case ']': // Escape sequence
                                buff += ']';
                                break;
                            default:
                                state = 0;
                                list.Add(buff);
                                buff = "";
                                break;
                        }
                    }
                    break;
                case '.':
                    if (state == 0)
                    {
                        list.Add(buff);
                        buff = String.Empty;
                    }
                    else if (state == 1)
                    {
                        buff += current;
                    }
                    break;
                default:
                    buff += current;
                    if (next == null)
                    {
                        list.Add(buff);
                        buff = String.Empty;
                    }
                    break;
            }
        }

        schema = null;
        name = null;
        if (list.Count > 0) // Find anything?
        {
            name = list[list.Count - 1];
            if (list.Count > 1)
            {
                schema = list[list.Count - 2];
            }
        }
    }
}