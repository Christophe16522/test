using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;

using CMS.Base;
using CMS.DataEngine;
using CMS.Helpers;
using CMS.UIControls;

public partial class CMSModules_System_Debug_System_DebugObjects : CMSDebugPage
{
    protected string cmsVersion = null;
    protected int index = 0;

    protected int totalObjects = 0;
    protected int totalTableObjects = 0;


    protected void Page_Load(object sender, EventArgs e)
    {
        cmsVersion = GetString("Footer.Version") + "&nbsp;" + CMS.Base.CMSVersion.GetVersion(true, true, true, true);

        gridObjects.Columns[0].HeaderText = GetString("General.ObjectType");
        gridObjects.Columns[1].HeaderText = GetString("General.Count");

        gridHashtables.Columns[0].HeaderText = GetString("Administration-System.CacheName");
        gridHashtables.Columns[1].HeaderText = GetString("General.Count");

        btnClear.Text = GetString("Administration-System.ClearHashtables");

        ReloadData();
    }


    protected void ReloadData()
    {
        DataTable dt;

        try
        {
            // Load the dictionaries
            dt = LoadDictionaries();
        }
        catch
        {
            // Load the dictionaries again (in case of collection was modified during the first load)
            dt = LoadDictionaries();
        }

        dt.DefaultView.Sort = "TableName ASC";

        gridHashtables.DataSource = dt.DefaultView;
        gridHashtables.DataBind();

        // Objects
        if (ObjectTypeInfo.TrackObjectInstances)
        {
            dt = new DataTable();
            dt.Columns.Add(new DataColumn("ObjectType", typeof(string)));
            dt.Columns.Add(new DataColumn("ObjectCount", typeof(int)));

            foreach (var info in ObjectTypeManager.RegisteredTypes)
            {
                DataRow dr = dt.NewRow();
                dr["ObjectType"] = info.ObjectType;

                // Get the instances
                IList<BaseInfo> instances = info.GetInstances();
                dr["ObjectCount"] = instances.Count;

                dt.Rows.Add(dr);
            }

            dt.DefaultView.Sort = "ObjectType ASC";

            gridObjects.DataSource = dt.DefaultView;
            gridObjects.DataBind();
        }
    }


    /// <summary>
    /// Loads the dictionaries to the data table
    /// </summary>
    private DataTable LoadDictionaries()
    {
        DataTable dt = new DataTable();
        dt.Columns.Add(new DataColumn("TableName", typeof(string)));
        dt.Columns.Add(new DataColumn("ObjectCount", typeof(int)));

        lock (ProviderStringDictionary.Dictionaries)
        {
            // Hashtables
            foreach (DictionaryEntry item in ProviderStringDictionary.Dictionaries)
            {
                if (item.Value is IProviderDictionary)
                {
                    IProviderDictionary dict = (IProviderDictionary)item.Value;

                    DataRow dr = dt.NewRow();
                    string resStringKey = "HashTableName." + ValidationHelper.GetIdentifier(item.Key);
                    if (resStringKey.Length > 100)
                    {
                        resStringKey = resStringKey.Substring(0, 100);
                    }

                    // Try to get the resource string name
                    string name = ResHelper.GetAPIString(resStringKey, null, String.Empty);
                    if (String.IsNullOrEmpty(name))
                    {
                        BaseInfo obj = ModuleManager.GetReadOnlyObject(dict.ObjectType);
                        if (obj != null)
                        {
                            string res = null;

                            // Known hashtable columns (ID, GUID, Name)
                            if (dict.ColumnNames.EqualsCSafe(obj.TypeInfo.IDColumn, true))
                            {
                                res = "hashtable.byid";
                            }
                            else if (dict.ColumnNames.StartsWithCSafe(obj.TypeInfo.CodeNameColumn, true))
                            {
                                res = "hashtable.byname";
                            }
                            else if (dict.ColumnNames.StartsWithCSafe(obj.TypeInfo.GUIDColumn, true))
                            {
                                res = "hashtable.byguid";
                            }

                            // Generate the name automatically
                            if (!String.IsNullOrEmpty(res))
                            {
                                name = ResHelper.GetStringFormat(res, GetString("ObjectTasks." + dict.ObjectType.Replace(".", "_")));
                            }
                        }
                    }

                    if (String.IsNullOrEmpty(name))
                    {
                        name = resStringKey;
                    }
                    dr["TableName"] = name;
                    dr["ObjectCount"] = dict.GetRealCount();

                    dt.Rows.Add(dr);
                }
            }
        }

        return dt;
    }


    protected string GetCount(object count)
    {
        int cnt = ValidationHelper.GetInteger(count, 0);
        totalObjects += cnt;

        return cnt.ToString();
    }


    protected string GetTableCount(object count)
    {
        int cnt = ValidationHelper.GetInteger(count, 0);
        totalTableObjects += cnt;

        return cnt.ToString();
    }


    protected void btnClear_Click(object sender, EventArgs e)
    {
        Functions.ClearHashtables();

        // Collect the memory
        GC.Collect();
        GC.WaitForPendingFinalizers();

        ReloadData();
    }
}