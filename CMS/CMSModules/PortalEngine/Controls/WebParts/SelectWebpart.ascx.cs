using System;
using System.Collections.Generic;
using System.Data;
using System.Web.UI.WebControls;

using CMS.DataEngine;
using CMS.FormControls;
using CMS.Helpers;
using CMS.ExtendedControls;
using CMS.PortalEngine;


public partial class CMSModules_PortalEngine_Controls_WebParts_SelectWebpart : FormEngineUserControl
{
    #region "Variables"

    private DataSet ds = null;
    private bool dataLoaded = false;

    public CMSModules_PortalEngine_Controls_WebParts_SelectWebpart()
    {
        EnableCategorySelection = false;
        ShowWebparts = true;
        ShowEmptyCategories = false;
        ShowRoot = false;
        ShowInheritedWebparts = true;
    }

    #endregion


    #region "Public properties"

    /// <summary>
    /// Gets or sets field value.
    /// </summary>
    public override object Value
    {
        get
        {
            EnsureChildControls();
            return drpWebpart.SelectedValue;
        }
        set
        {
            EnsureChildControls();
            drpWebpart.SelectedValue = ValidationHelper.GetString(value, String.Empty);
        }
    }


    /// <summary>
    /// Enables or disables root category in drop down list.
    /// </summary>
    public bool ShowRoot
    {
        get;
        set;
    }


    /// <summary>
    /// Shows or hide inherited webparts.
    /// </summary>
    public bool ShowInheritedWebparts
    {
        get;
        set;
    }


    /// <summary>
    /// Enables or disables hiding of empty categories in drop down list.
    /// </summary>
    public bool ShowEmptyCategories
    {
        get;
        set;
    }


    /// <summary>
    /// If enabled, webparts are shown in DropDownList.
    /// </summary>
    public bool ShowWebparts
    {
        get;
        set;
    }


    /// <summary>
    /// If enabled, category can be selected, otherwise categories are disabled.
    /// </summary>
    public bool EnableCategorySelection
    {
        get;
        set;
    }


    /// <summary>
    /// Gets the drop down list control.
    /// </summary>
    public CMSDropDownList DropDownListControl
    {
        get
        {
            return drpWebpart;
        }
    }


    /// <summary>
    /// Root category path
    /// </summary>
    public string RootPath 
    { 
        get; 
        set; 
    }


    /// <summary>
    /// Additional where condition applied to the listed web parts
    /// </summary>
    public string WhereCondition
    {
        get;
        set;
    }


    /// <summary>
    /// If true, the none option is displayed in the list
    /// </summary>
    public bool DisplayNone
    {
        get;
        set;
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Handles the Load event of the Page control.
    /// </summary>
    protected void Page_Load(object sender, EventArgs e)
    {
    }


    /// <summary>
    /// Creates child controls.
    /// </summary>
    protected override void CreateChildControls()
    {
        if (StopProcessing)
        {
            return;
        }

        ReloadData(false);
    }


    /// <summary>
    /// Reloads the data.
    /// </summary>
    /// <param name="forceReload">If true, the data is reloaded even when already loaded</param>
    public void ReloadData(bool forceReload)
    {
        if (!dataLoaded || forceReload)
        {
            drpWebpart.Items.Clear();

            if (DisplayNone)
            {
                drpWebpart.Items.Add(new ListItem(ResHelper.GetString("General.SelectNone"), ""));
            }

            // Do not retrieve webparts
            string whereCond = null;
            if (!ShowWebparts)
            {
                whereCond = "ObjectType = 'webpartcategory'";
            }

            if (!ShowInheritedWebparts)
            {
                whereCond = SqlHelper.AddWhereCondition(whereCond, "WebPartParentID IS NULL");
            }

            if (!String.IsNullOrEmpty(RootPath))
            {
                whereCond = SqlHelper.AddWhereCondition(whereCond, String.Format("ObjectPath = '{0}' OR ObjectPath LIKE '{0}/%'", SqlHelper.GetSafeQueryString(RootPath.TrimEnd('/'), false)));
            }

            whereCond = SqlHelper.AddWhereCondition(whereCond, WhereCondition);

            ds = WebPartCategoryInfoProvider.GetCategoriesAndWebparts(whereCond, "DisplayName", 0, "ObjectID, DisplayName, ParentID, ObjectType");

            if (!DataHelper.DataSourceIsEmpty(ds))
            {
                int counter = 0;

                // Make special collection for "tree mapping"
                Dictionary<int, SortedList<string, object[]>> categories = new Dictionary<int, SortedList<string, object[]>>();

                // Fill collection from dataset
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    int parentId = ValidationHelper.GetInteger(dr["ParentID"], 0);
                    int id = ValidationHelper.GetInteger(dr["ObjectID"], 0);
                    string name = ResHelper.LocalizeString(ValidationHelper.GetString(dr["DisplayName"], String.Empty));
                    string type = ValidationHelper.GetString(dr["ObjectType"], String.Empty);

                    // Skip webpart, take only WebpartCategory
                    if (type == "webpart")
                    {
                        continue;
                    }

                    SortedList<string, object[]> list;
                    categories.TryGetValue(parentId, out list);

                    // Sub categories list not created yet
                    if (list == null)
                    {
                        list = new SortedList<string, object[]>();
                        categories.Add(parentId, list);
                    }

                    list.Add(name + "_" + counter, new object[2] { id, name });

                    counter++;
                }

                // Start filling the dropdown from the root(parentId = 0)
                int level = 0;

                // Root is not shown, start indentation later
                if (!ShowRoot)
                {
                    level = -1;
                }

                AddSubCategories(categories, 0, level);
            }

            dataLoaded = true;
        }
    }


    /// <summary>
    /// Add subcategories list items to drop down.
    /// </summary>
    /// <param name="categories">Special "tree" collection</param>
    /// <param name="parentId">Category parent ID</param>
    /// <param name="level">Category level(recursion)</param>
    private int AddSubCategories(Dictionary<int, SortedList<string, object[]>> categories, int parentId, int level)
    {
        int count = 0;

        if (categories != null)
        {
            SortedList<string, object[]> categoryList;
            categories.TryGetValue(parentId, out categoryList);
            if (categoryList != null)
            {
                foreach (KeyValuePair<string, object[]> category in categoryList)
                {
                    // Make indentation for sub categories
                    string indentation = String.Empty;
                    for (int i = 0; i < level; i++)
                    {
                        indentation += "\xA0\xA0\xA0";
                    }

                    // Create and add list item
                    ListItem listItem = null;
                    int itemIndex = 0;

                    // Hide root
                    if (((parentId == 0) && ShowRoot) || (parentId > 0))
                    {
                        if (EnableCategorySelection)
                        {
                            listItem = new ListItem(indentation + category.Value[1], category.Value[0].ToString());
                            if (ShowRoot && (level == 0))
                            {
                                listItem.Attributes.Add("style", "background-color: #DDDDDD;");
                            }
                        }
                        else
                        {
                            listItem = new ListItem(indentation + category.Value[1], "-10");
                            listItem.Attributes.Add("style", "background-color: #DDDDDD; color: #000000;");
                            listItem.Attributes.Add("disabled", "disabled");
                        }

                        itemIndex = drpWebpart.Items.Count;
                        drpWebpart.Items.Add(listItem);
                        count++;
                    }

                    int subCount = 0;

                    if (ShowWebparts)
                    {
                        // Add webparts under category
                        subCount += AddWebparts(indentation += "\xA0\xA0\xA0", Convert.ToInt32(category.Value[0]));
                    }

                    // Recursion
                    subCount += AddSubCategories(categories, Convert.ToInt32(category.Value[0]), level + 1);

                    // Remove empty categories
                    if (ShowWebparts && !ShowEmptyCategories && (subCount == 0) && (listItem != null))
                    {
                        drpWebpart.Items.RemoveAt(itemIndex);
                        count--;
                    }
                }
            }
        }

        return count;
    }


    /// <summary>
    /// Add webparts under current category.
    /// </summary>
    /// <param name="indentation">Indentation of items</param>
    /// <param name="parentCategory">Parent category</param>
    /// <returns>Number of added webparts</returns>
    private int AddWebparts(string indentation, int parentCategory)
    {
        if (ds != null)
        {
            DataView dv = ds.Tables[0].DefaultView;
            dv.RowFilter = "ParentID = " + parentCategory + " AND ObjectType = 'webpart'";
            foreach (DataRowView drv in dv)
            {
                // Create and add list item
                ListItem listItem = new ListItem(indentation + drv["DisplayName"], drv["ObjectID"].ToString());
                drpWebpart.Items.Add(listItem);
            }

            // Return number of webparts
            return dv.Count;
        }

        return 0;
    }

    #endregion
}