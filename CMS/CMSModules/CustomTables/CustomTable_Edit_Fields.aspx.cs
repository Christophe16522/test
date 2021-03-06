using System;
using System.Data;

using CMS.Core;
using CMS.CustomTables;
using CMS.DataEngine;
using CMS.EventLog;
using CMS.FormEngine;
using CMS.Helpers;
using CMS.Base;
using CMS.SiteProvider;
using CMS.Membership;
using CMS.UIControls;
using CMS.ExtendedControls.ActionsConfig;
using CMS.URLRewritingEngine;

[UIElement(ModuleName.CUSTOMTABLES, "CustomTable.Fields")]
public partial class CMSModules_CustomTables_CustomTable_Edit_Fields : CMSCustomTablesPage
{
    #region "Variables"

    protected DataClassInfo dci = null;
    protected string className = null;
    private FormInfo mFormInfo = null;
    private HeaderAction btnGenerateGuid;

    #endregion


    #region "Properties"

    /// <summary>
    /// Form info.
    /// </summary>
    public FormInfo FormInfo
    {
        get
        {
            if ((mFormInfo == null) && (dci != null))
            {
                mFormInfo = FormHelper.GetFormInfo(dci.ClassName, true);
            }
            return mFormInfo;
        }
    }

    #endregion


    protected void Page_Load(object sender, EventArgs e)
    {
        int classId = QueryHelper.GetInteger("objectid", 0);
        dci = DataClassInfoProvider.GetDataClassInfo(classId);
        // Set edited object
        EditedObject = dci;
        CurrentMaster.BodyClass += " FieldEditorBody";

        btnGenerateGuid = new HeaderAction()
        {
            Tooltip = GetString("customtable.GenerateGUID"),
            Text = GetString("customtable.GenerateGUIDField"),
            Visible = false,
            CommandName = "createguid",
        };
        FieldEditor.HeaderActions.AddAction(btnGenerateGuid);
        FieldEditor.HeaderActions.ActionPerformed += (s, ea) => { if (ea.CommandName == "createguid") CreateGUID(); };

        // Class exists
        if (dci != null)
        {
            className = dci.ClassName;
            if (dci.ClassIsCoupledClass)
            {
                FieldEditor.Visible = true;
                FieldEditor.ClassName = className;
                FieldEditor.Mode = FieldEditorModeEnum.CustomTable;
                FieldEditor.OnFieldNameChanged += FieldEditor_OnFieldNameChanged;
            }
            else
            {
                FieldEditor.ShowError(GetString("customtable.ErrorNoFields"));
            }
        }

        ScriptHelper.HideVerticalTabs(this);
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        // Class exists
        if (dci != null)
        {
            if (dci.ClassIsCoupledClass)
            {
                // GUID column is not present
                if ((FormInfo.GetFormField("ItemGUID") == null))
                {
                    btnGenerateGuid.Visible = true;
                    FieldEditor.ShowInformation(GetString("customtable.GUIDColumMissing"));
                }
            }

            if (!RequestHelper.IsPostBack() && QueryHelper.GetBoolean("gen", false))
            {
                FieldEditor.ShowInformation(GetString("customtable.GUIDFieldGenerated"));
            }
        }
    }


    private void FieldEditor_OnFieldNameChanged(object sender, string oldFieldName, string newFieldName)
    {
        if (dci != null)
        {
            // Rename field in layout(s)
            FormHelper.RenameFieldInFormLayout(dci.ClassID, oldFieldName, newFieldName);
        }
    }


    /// <summary>
    /// Adds GUID field to form definition.
    /// </summary>
    private void CreateGUID()
    {
        try
        {
            // Create GUID field
            FormFieldInfo ffiGuid = new FormFieldInfo();

            // Fill FormInfo object
            ffiGuid.Name = "ItemGUID";
            ffiGuid.SetPropertyValue(FormFieldPropertyEnum.FieldCaption, "GUID");
            ffiGuid.DataType = FieldDataType.Guid;
            ffiGuid.SetPropertyValue(FormFieldPropertyEnum.DefaultValue, string.Empty);
            ffiGuid.SetPropertyValue(FormFieldPropertyEnum.FieldDescription, String.Empty);
            ffiGuid.FieldType = FormFieldControlTypeEnum.CustomUserControl;
            ffiGuid.Settings["controlname"] = Enum.GetName(typeof(FormFieldControlTypeEnum), FormFieldControlTypeEnum.LabelControl).ToLowerCSafe();
            ffiGuid.PrimaryKey = false;
            ffiGuid.System = true;
            ffiGuid.Visible = false;
            ffiGuid.Size = 0;
            ffiGuid.AllowEmpty = false;

            FormInfo.AddFormItem(ffiGuid);

            // Update table structure - columns could be added
            bool old = TableManager.UpdateSystemFields;
            TableManager.UpdateSystemFields = true;
            string schema = FormInfo.GetXmlDefinition();

            TableManager tm = new TableManager(null);
            tm.UpdateTableByDefinition(dci.ClassTableName, schema);

            TableManager.UpdateSystemFields = old;

            // Update xml schema and form definition
            dci.ClassFormDefinition = schema;
            dci.ClassXmlSchema = tm.GetXmlSchema(dci.ClassTableName);

            dci.Generalized.LogEvents = false;

            // Save the data
            DataClassInfoProvider.SetDataClassInfo(dci);

            dci.Generalized.LogEvents = true;

            // Clear the default queries
            QueryInfoProvider.ClearDefaultQueries(dci, true, false);

            // Clear the object type hashtable
            ProviderStringDictionary.ReloadDictionaries(className, true);

            // Clear the classes hashtable
            ProviderStringDictionary.ReloadDictionaries("cms.class", true);

            // Clear class strucures
            ClassStructureInfo.Remove(className, true);

            // Ensure GUIDs for all items
            using (CMSActionContext ctx = new CMSActionContext())
            {
                ctx.UpdateSystemFields = false;
                ctx.LogSynchronization = false;
                DataSet dsItems = CustomTableItemProvider.GetItems(className);
                if (!DataHelper.DataSourceIsEmpty(dsItems))
                {
                    foreach (DataRow dr in dsItems.Tables[0].Rows)
                    {
                        CustomTableItem item = CustomTableItem.New(className, dr);
                        item.ItemGUID = Guid.NewGuid();
                        item.Update();
                    }
                }
            }

            // Log event
            UserInfo currentUser = MembershipContext.AuthenticatedUser;
            EventLogProvider.LogEvent(EventType.INFORMATION, "Custom table", "GENERATEGUID", string.Format(ResHelper.GetAPIString("customtable.GUIDGenerated", "Field 'ItemGUID' for custom table '{0}' was created and GUID values were generated."), dci.ClassName), null, currentUser.UserID, currentUser.UserName);

            URLHelper.Redirect(URLHelper.AddParameterToUrl(RequestContext.CurrentURL, "gen", "1"));
        }
        catch (Exception ex)
        {
            FieldEditor.ShowError(GetString("customtable.ErrorGUID") + ex.Message);

            // Log event
            EventLogProvider.LogException("Custom table", "GENERATEGUID", ex);
        }
    }
}