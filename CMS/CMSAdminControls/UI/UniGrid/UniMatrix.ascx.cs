using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.Base;
using CMS.Controls;
using CMS.DataEngine;
using CMS.ExtendedControls;
using CMS.Helpers;
using CMS.MacroEngine;
using CMS.UIControls;

public partial class CMSAdminControls_UI_UniGrid_UniMatrix : UniMatrix, ICallbackEventHandler, IUniPageable
{
    #region "Variables"

    private int mDefaultPageSize = 20;
    private string mPageSizeOptions = "10,20,50,100,##ALL##";
    private string mCallbackResult;
    private bool mLoaded;
    private DataSet ds;
    private int mTotalRows;
    private bool? mHasData = null;

    private string mCornerText = string.Empty;

    private readonly Hashtable mColumnPermissions = new Hashtable();
    private readonly Hashtable mRowPermissions = new Hashtable();

    #endregion


    #region "Properties"

    /// <summary>
    /// Page size options for pager.
    /// Numeric values or macro ##ALL## separated with comma.
    /// </summary>
    public string PageSizeOptions
    {
        get
        {
            return mPageSizeOptions;
        }
        set
        {
            mPageSizeOptions = value;
            pagerElem.PageSizeOptions = value;
        }
    }

    /// <summary>
    /// Default page size at first load.
    /// </summary>
    public virtual int DefaultPageSize
    {
        get
        {
            if ((mDefaultPageSize <= 0) && (mDefaultPageSize != -1))
            {
                mDefaultPageSize = ValidationHelper.GetInteger(SettingsHelper.AppSettings["CMSDefaultListingPageSize"], 20);
            }
            return mDefaultPageSize;
        }
        set
        {
            mDefaultPageSize = value;
        }
    }


    /// <summary>
    /// Items per page.
    /// </summary>
    public override int ItemsPerPage
    {
        get
        {
            return base.ItemsPerPage;
        }
        set
        {
            base.ItemsPerPage = value;
            pagerElem.UniPager.PageSize = value;
        }
    }


    /// <summary>
    /// Gets the value that indicates whether current selector in multiple mode displays some data or whether the dropdown contains some data.
    /// </summary>
    public override bool HasData
    {
        get
        {
            // Ensure the data
            if (!mHasData.HasValue && !StopProcessing)
            {
                ReloadData(false);
                mHasData = true;
            }

            return ValidationHelper.GetBoolean(ViewState["HasData"], false);
        }
        protected set
        {
            ViewState["HasData"] = value;
            mHasData = true;
        }
    }


    /// <summary>
    /// Filter where condition.
    /// </summary>
    private string FilterWhere
    {
        get
        {
            return ValidationHelper.GetString(ViewState["FilterWhere"], "");
        }
        set
        {
            ViewState["FilterWhere"] = value;
        }
    }


    /// <summary>
    /// Number of expected matrix columns.
    /// </summary>
    public int ColumnsCount
    {
        get
        {
            return ValidationHelper.GetInteger(ViewState["ColumnsCount"], 10);
        }
        set
        {
            ViewState["ColumnsCount"] = value;
        }
    }


    /// <summary>
    /// Number of expected matrix columns.
    /// </summary>
    public string ColumnsPreferedOrder
    {
        get
        {
            return ValidationHelper.GetString(ViewState["ColumnsPreferedOrder"], "");
        }
        set
        {
            ViewState["ColumnsPreferedOrder"] = value;
        }
    }


    /// <summary>
    /// Sets or gets fixed width of first column.
    /// </summary>
    public string FirstColumnClass
    {
        get
        {
            return ValidationHelper.GetString(ViewState["FirstColumnClass"], string.Empty);
        }
        set
        {
            ViewState["FirstColumnClass"] = value;
        }
    }


    /// <summary>
    /// Gets or sets the message which is displayed if there are no records.
    /// </summary>
    public string NoRecordsMessage
    {
        get;
        set;
    }


    /// <summary>
    /// UniPager control of UniMatrix.
    /// </summary>
    public UniPager Pager
    {
        get
        {
            return pagerElem.UniPager;
        }
    }


    /// <summary>
    /// Text displayed in the upper left corner of UniMatrix, if filter is not shown.
    /// </summary>
    public string CornerText
    {
        get
        {
            return mCornerText;
        }
        set
        {
            mCornerText = value;
        }
    }


    /// <summary>
    /// Gets or sets HTML content to be rendered as additional content on the top of the matrix.
    /// </summary>
    public TableRow ContentBeforeRow
    {
        get
        {
            return trContentBefore;
        }
    }


    /// <summary>
    /// Indicates if content before rows should be displayed.
    /// </summary>
    public bool ShowContentBeforeRow
    {
        get;
        set;
    }


    /// <summary>
    /// Gets or sets CSS class for content before rows.
    /// </summary>
    public string ContentBeforeRowCssClass
    {
        get;
        set;
    }


    /// <summary>
    /// Gets order in which data will be rendered.
    /// </summary>
    public int[] ColumnOrderIndex
    {
        get
        {
            return ViewState["ColumnOrderIndex"] as int[];
        }
        set
        {
            ViewState["ColumnOrderIndex"] = value;
        }
    }


    /// <summary>
    /// Mark HTML code for the disabled column in header.
    /// </summary>
    public string DisabledColumnMark
    {
        get;
        set;
    }


    /// <summary>
    /// Mark HTML code for the disabled row in header.
    /// </summary>
    public string DisabledRowMark
    {
        get;
        set;
    }


    /// <summary>
    /// CSS class of the matrix control
    /// </summary>
    public string CssClass
    {
        get;
        set;
    }

    #endregion


    #region "Events"

    /// <summary>
    /// Occurs when data has been loaded. Allows manipulation with data.
    /// </summary>
    /// <param name="ds">Loaded dataset</param>
    public delegate void OnMatrixDataLoaded(DataSet ds);


    public event OnMatrixDataLoaded DataLoaded;


    /// <summary>
    /// Occurs if the matrix wants to check the permission to edit particular item.
    /// </summary>
    /// <param name="value">Column value</param>
    public delegate bool OnCheckPermissions(object value);


    public event OnCheckPermissions CheckColumnPermissions;

    public event OnCheckPermissions CheckRowPermissions;

    #endregion


    #region "Control events"

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        pagerElem.PagedControl = this;
        pagerElem.UniPager.PageControl = "uniMatrix";
        pagerElem.PageSizeOptions = PageSizeOptions;
        pagerElem.UniPager.PageSize = ItemsPerPage;
        pagerElem.PageSizeDropdown.SelectedIndexChanged += drpPageSize_SelectedIndexChanged;

        ScriptHelper.RegisterJQuery(Page);
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        if (!RequestHelper.IsCallback())
        {
            ReloadData(false);
        }
    }


    protected void drpPageSize_SelectedIndexChanged(object sender, EventArgs e)
    {
        pagerElem.UniPager.CurrentPage = 1;
        ItemsPerPage = ValidationHelper.GetInteger(pagerElem.PageSizeDropdown.SelectedValue, -1);

        if (pagerElem.UniPager.PagedControl != null)
        {
            pagerElem.UniPager.PagedControl.ReBind();
        }
    }


    /// <summary>
    /// Filters the content.
    /// </summary>
    protected void btnFilter_Click(object sender, EventArgs e)
    {
        pagerElem.UniPager.CurrentPage = 1;

        // Get the expression
        string expr = txtFilter.Text.Trim();
        if (!String.IsNullOrEmpty(expr))
        {
            // Build the where condition for display name
            FilterWhere = RowItemDisplayNameColumn + " LIKE '%" + SqlHelper.GetSafeQueryString(expr, false) + "%'";
        }
        else
        {
            FilterWhere = null;
        }

        txtFilter.Focus();
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Returns true if the given row is editable.
    /// </summary>
    /// <param name="rowValue">Row value</param>
    protected bool IsRowEditable(object rowValue)
    {
        if (CheckRowPermissions == null)
        {
            return true;
        }

        // Try to get cached value
        object editableObj = mRowPermissions[rowValue];
        if (editableObj == null)
        {
            // Get by external function
            editableObj = CheckRowPermissions(rowValue);
            mRowPermissions[rowValue] = editableObj;
        }

        return ValidationHelper.GetBoolean(editableObj, true);
    }


    /// <summary>
    /// Returns true if the given column is editable.
    /// </summary>
    /// <param name="columnValue">Column value</param>
    protected bool IsColumnEditable(object columnValue)
    {
        if (CheckColumnPermissions == null)
        {
            return true;
        }

        // Try to get cached value
        object editableObj = mColumnPermissions[columnValue];
        if (editableObj == null)
        {
            // Get by external function
            editableObj = CheckColumnPermissions(columnValue);
            mColumnPermissions[columnValue] = editableObj;
        }

        return ValidationHelper.GetBoolean(editableObj, true);
    }


    /// <summary>
    /// Reloads the control data.
    /// </summary>
    /// <param name="forceReload">Force the reload of the control</param>
    public override void ReloadData(bool forceReload)
    {
        if (StopProcessing)
        {
            plcPager.Visible = false;
            tblMatrix.Visible = false;
            return;
        }

        base.ReloadData(forceReload);

        SetPageSize(forceReload);

        // Clear filter if forced reload
        if (forceReload)
        {
            txtFilter.Text = "";
            FilterWhere = null;
        }

        if (forceReload || !mLoaded)
        {
            // Prepare parameters for data loading
            int currentPage = pagerElem.UniPager.CurrentPage;
            string where = SqlHelper.AddWhereCondition(WhereCondition, FilterWhere);
            string orderBy = GetOrderByClause();

            mTotalRows = 0;
            bool headersOnly = false;

            // Load matrix data
            var headerData = LoadData(where, orderBy, currentPage, ref headersOnly);

            if (HasData)
            {
                tblMatrix.Visible = true;
                if (!headersOnly)
                {
                    RegisterScripts();
                }

                // Set CSS classes
                thcFirstColumn.CssClass = FirstColumnClass;
                tblMatrix.AddCssClass(CssClass);
                trContentBefore.AddCssClass(ContentBeforeRowCssClass);

                // Set the correct number of columns
                ColumnsCount = headerData.Count;
                mTotalRows = mTotalRows / ColumnsCount;

                GenerateMatrixContent(headerData, headersOnly);

                SetupMatrixFilter();

                // Show content before rows and pager
                ContentBeforeRow.Visible = ShowContentBeforeRow && !headersOnly;
            }
            else
            {
                tblMatrix.Visible = false;

                // If no-records message set, hide everything and show message
                if (!String.IsNullOrEmpty(NoRecordsMessage))
                {
                    lblInfo.Text = NoRecordsMessage;
                    lblInfo.Visible = true;
                }
            }

            // Setup pager visibility
            plcPager.Visible = HasData && !headersOnly;
            if (HasData)
            {
                // Set correct ID for direct page control
                pagerElem.DirectPageControlID = ((float)mTotalRows / pagerElem.CurrentPageSize > 20.0f) ? "txtPage" : "drpPage";
            }

            mLoaded = true;

            // Call page binding event
            if (OnPageBinding != null)
            {
                OnPageBinding(this, null);
            }
        }
    }


    /// <summary>
    /// Sets pager to first page.
    /// </summary>
    public void ResetPager()
    {
        pagerElem.UniPager.CurrentPage = 1;
    }


    /// <summary>
    /// Resets the pager and filter.
    /// </summary>
    public void ResetMatrix()
    {
        pagerElem.UniPager.CurrentPage = 1;
        txtFilter.Text = String.Empty;
        FilterWhere = null;
    }

    #endregion


    #region ICallbackEventHandler Members

    /// <summary>
    /// Gets the callback result.
    /// </summary>
    public string GetCallbackResult()
    {
        return mCallbackResult;
    }


    /// <summary>
    /// Processes the callback event.
    /// </summary>
    public void RaiseCallbackEvent(string eventArgument)
    {
        string[] parameters = eventArgument.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
        if (parameters.Length == 4)
        {
            int rowItemId = ValidationHelper.GetInteger(parameters[1], 0);
            int colItemId = ValidationHelper.GetInteger(parameters[2], 0);
            bool newState = ValidationHelper.GetBoolean(parameters[3], false);

            // Raise the change
            RaiseOnItemChanged(rowItemId, colItemId, newState);

            // If row before content should be shown and displayed data was changed, render new HTML
            if (ShowContentBeforeRow || (ContentBeforeRow.Visible && (ContentBeforeRow.Cells.Count > 0)))
            {
                mCallbackResult = ContentBeforeRow.GetRenderedHTML();
            }
        }
    }

    #endregion


    #region "IUniPageable Members"

    /// <summary>
    /// Pager data item object.
    /// </summary>
    public object PagerDataItem
    {
        get
        {
            return ds;
        }
        set
        {
            ds = (DataSet)value;
        }
    }


    /// <summary>
    /// Pager control.
    /// </summary>
    public UniPager UniPagerControl
    {
        get;
        set;
    }


    /// <summary>
    /// Occurs when the control bind data.
    /// </summary>
    public event EventHandler<EventArgs> OnPageBinding;


    /// <summary>
    /// Occurs when the pager change the page and current mode is postback => reload data
    /// </summary>
    public event EventHandler<EventArgs> OnPageChanged;


    /// <summary>
    /// Evokes control databind.
    /// </summary>
    public void ReBind()
    {
        if (OnPageChanged != null)
        {
            OnPageChanged(this, null);
        }

        DataBind();
    }


    /// <summary>
    /// Gets or sets the number of result. Enables proceed "fake" datasets, where number 
    /// of results in the dataset is not correspondent to the real number of results
    /// This property must be equal -1 if should be disabled
    /// </summary>
    public int PagerForceNumberOfResults
    {
        get
        {
            return mTotalRows;
        }
        set
        {
        }
    }

    #endregion


    #region "Private methods"

    /// <summary>
    /// Gets an array of indexes which are sorted according to ColumnsPreferedOrder.
    /// i.e: Permission are "A","B","C" .. desired permission order is "C","A","B", then columnOrderIndex will be [2,0,1]
    /// </summary>
    /// <param name="columns">The columns</param>
    /// <param name="columnOrder">The column order</param>
    private int[] GetColumnIndexes(IList<DataRow> columns, string columnOrder)
    {
        List<string> order = new List<string>(columnOrder.Split(','));
        List<bool> indexIsSet = new List<bool>();
        List<int> columnOrderIndex = new List<int>();

        // Initialize array
        for (int i = 0; i < columns.Count; i++)
        {
            indexIsSet.Add(false);
        }

        // Sort according to defined order
        foreach (DataRow dr in columns)
        {
            int index = order.IndexOf(ValidationHelper.GetString(DataHelper.GetDataRowValue(dr, "PermissionName"), ""));
            if (index > -1)
            {
                columnOrderIndex.Add(index);
                indexIsSet[index] = true;
            }
        }

        // Insert original colums which were not defined in columnOrder
        for (int i = 0; i < indexIsSet.Count; i++)
        {
            if (indexIsSet[i] == false)
            {
                columnOrderIndex.Add(i);
                indexIsSet[i] = true;
            }
        }

        return columnOrderIndex.ToArray();
    }


    /// <summary>
    /// Returns additional CSS class formatted in a way to be concatenated with other row CSS classes.
    /// </summary>
    /// <param name="dr">DataRow with matrix row data</param>
    private string GetAdditionalCssClass(DataRow dr)
    {
        string cssClass = RaiseGetRowItemCssClass(dr);
        if (!String.IsNullOrEmpty(cssClass))
        {
            cssClass = " " + cssClass;
        }
        return cssClass;
    }


    /// <summary>
    /// Sets page size dropdown list according to PageSize property.
    /// </summary>
    private void SetPageSize(bool forceReload)
    {
        if ((pagerElem.PageSizeDropdown.Items.Count == 0) || forceReload)
        {
            pagerElem.PageSizeDropdown.Items.Clear();

            string[] sizes = PageSizeOptions.Split(',');
            if (sizes.Length > 0)
            {
                List<int> sizesInt = new List<int>();
                // Indicates if contains 'Select ALL' macro
                bool containsAll = false;
                foreach (string size in sizes)
                {
                    if (size.ToUpperCSafe() == "##ALL##")
                    {
                        containsAll = true;
                    }
                    else
                    {
                        sizesInt.Add(ValidationHelper.GetInteger(size.Trim(), 0));
                    }
                }
                // Add default page size if not present
                if ((DefaultPageSize > 0) && !sizesInt.Contains(DefaultPageSize))
                {
                    sizesInt.Add(DefaultPageSize);
                }
                // Sort list of page sizes
                sizesInt.Sort();

                ListItem item;

                foreach (int size in sizesInt)
                {
                    // Skip zero values
                    if (size != 0)
                    {
                        item = new ListItem(size.ToString());
                        if (item.Value == DefaultPageSize.ToString())
                        {
                            item.Selected = true;
                        }
                        pagerElem.PageSizeDropdown.Items.Add(item);
                    }
                }
                // Add 'Select ALL' macro at the end of list
                if (containsAll)
                {
                    item = new ListItem(GetString("general.selectall"), "-1");
                    if (DefaultPageSize == -1)
                    {
                        item.Selected = true;
                    }
                    pagerElem.PageSizeDropdown.Items.Add(item);
                }
            }
        }
    }


    /// <summary>
    /// Returns safe and localized tooltip from the given source column.
    /// </summary>
    /// <param name="dr">Data row with the tooltip column</param>
    /// <param name="columnName">Name of the tooltip source column</param>
    private string GetTooltip(DataRow dr, string columnName)
    {
        // Get tooltip string
        string tooltip = ValidationHelper.GetString(DataHelper.GetDataRowValue(dr, columnName), "");

        // Get safe an localized tooltip
        if (!string.IsNullOrEmpty(tooltip))
        {
            return HTMLHelper.HTMLEncode(MacroResolver.Resolve(tooltip));
        }

        return "";
    }


    /// <summary>
    /// Generate header of the matrix.
    /// </summary>
    /// <param name="matrixData">Data of the matrix to be generated</param>
    private void GenerateMatrixHeader(List<DataRow> matrixData)
    {
        // Prepare matrix header
        foreach (int index in ColumnOrderIndex)
        {
            DataRow dr = matrixData[index];

            if (ShowHeaderRow)
            {
                // Create header cell
                var thc = new TableHeaderCell
                {
                    Scope = TableHeaderScope.Column,
                    Text = HTMLHelper.HTMLEncode(MacroResolver.Resolve(Convert.ToString(dr[ColumnItemDisplayNameColumn]))),
                    ToolTip = (ColumnItemTooltipColumn != null) ? GetTooltip(dr, ItemTooltipColumn) : null,
                    EnableViewState = false
                };
                thrFirstRow.Cells.Add(thc);

                // Add disabled mark if needed
                if (!IsColumnEditable(dr[ColumnItemIDColumn]))
                {
                    thc.Text += DisabledColumnMark;
                }
            }
            else
            {
                // Create header cell
                var thc = new TableHeaderCell
                {
                    Scope = TableHeaderScope.Column,
                    Text = "&nbsp;",
                    EnableViewState = false
                };
                thrFirstRow.Cells.Add(thc);
            }
        }
    }


    /// <summary>
    /// Generate body of the matrix.
    /// </summary>
    /// <param name="matrixData">Data of the matrix to be generated</param>
    private void GenerateMatrixBody(List<DataRow> matrixData)
    {
        string lastRowId = "";
        int colIndex = 0;
        int rowIndex = 0;

        TableRow tr = null;
        TableCell tc = null;

        // Render matrix rows
        int step = matrixData.Count;
        for (int i = 0; i < ds.Tables[0].Rows.Count; i = i + step)
        {
            foreach (int index in ColumnOrderIndex)
            {
                DataRow rowData = ds.Tables[0].Rows[i + index];
                string rowId = ValidationHelper.GetString(rowData[RowItemIDColumn], "");

                // Detect new matrix row
                if (rowId != lastRowId)
                {
                    if ((ItemsPerPage > 0) && (rowIndex++ >= ItemsPerPage))
                    {
                        break;
                    }

                    // New Table row
                    tr = new TableRow
                    {
                        CssClass = GetAdditionalCssClass(rowData),
                        EnableViewState = false
                    };
                    tblMatrix.Rows.Add(tr);

                    // Header table cell
                    tc = new TableCell
                    {
                        CssClass = "matrix-header",
                        Text = HTMLHelper.HTMLEncode(MacroResolver.Resolve(ValidationHelper.GetString(rowData[RowItemDisplayNameColumn], null))),
                        ToolTip = (RowItemTooltipColumn != null) ? GetTooltip(rowData, RowItemTooltipColumn) : null,
                        EnableViewState = false
                    };
                    tr.Cells.Add(tc);

                    // Add disabled mark if needed
                    if (!IsRowEditable(rowId))
                    {
                        tc.Text += DisabledRowMark;
                    }

                    // Add global suffix if is required
                    if ((index == 0) && (AddGlobalObjectSuffix) && (ValidationHelper.GetInteger(rowData[SiteIDColumnName], 0) == 0))
                    {
                        tc.Text += " " + GetString("general.global");
                    }

                    // Update 
                    lastRowId = rowId;
                    colIndex = 0;
                }

                object columnValue = rowData[ColumnItemIDColumn];
                var cellId = string.Format("chk:{0}:{1}", rowId, columnValue);

                // New table cell
                tc = new TableCell
                {
                    EnableViewState = false
                };
                tr.Cells.Add(tc);

                // Checkbox for data
                var chk = new CMSCheckBox
                {
                    ID = cellId,
                    ClientIDMode = ClientIDMode.Static,
                    ToolTip = GetTooltip(rowData, ItemTooltipColumn),
                    Checked = ValidationHelper.GetBoolean(rowData["Allowed"], false),
                    Enabled = Enabled &&
                              !disabledColumns.Contains(colIndex) &&
                              IsColumnEditable(columnValue) &&
                              IsRowEditable(rowId),
                    EnableViewState = false
                };
                tc.Controls.Add(chk);

                // Add click event to enabled checkbox
                if (chk.Enabled)
                {
                    chk.Attributes.Add("onclick", "UM_ItemChanged_" + ClientID + "(this);");
                }

                colIndex++;
            }
        }
    }


    /// <summary>
    /// Generate matrix content
    /// </summary>
    /// <param name="matrixData">Data of the matrix to be generated</param>
    /// <param name="generateOnlyHeader">Indicates if only matrix header should be generated</param>
    private void GenerateMatrixContent(List<DataRow> matrixData, bool generateOnlyHeader)
    {
        GenerateMatrixHeader(matrixData);

        if (generateOnlyHeader)
        {
            lblInfoAfter.Text = NoRecordsMessage;
            lblInfoAfter.Visible = true;
        }
        else
        {
            GenerateMatrixBody(matrixData);
        }
    }


    /// <summary>
    /// Register custom scripts for matrix
    /// </summary>
    private void RegisterScripts()
    {
        // Register the scripts
        string script =
            "function UM_ItemChanged_" + ClientID + "(item) {" + Page.ClientScript.GetCallbackEventReference(this, "item.id + ':' + item.checked", "UM_ItemSaved_" + ClientID, "item.id") + "; } \n" +
            "function UM_ItemSaved_" + ClientID + "(rvalue, context) { var contentBefore = $j(\"#" + trContentBefore.ClientID + "\"); if(contentBefore){ contentBefore.replaceWith(rvalue);}}";

        ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "UniMatrix_" + ClientID, ScriptHelper.GetScript(script));
    }


    /// <summary>
    /// Setup matrix filter
    /// </summary>
    private void SetupMatrixFilter()
    {
        // Show filter / header
        bool showFilter = ((FilterLimit <= 0) || !String.IsNullOrEmpty(FilterWhere) || (mTotalRows >= FilterLimit));
        pnlFilter.Visible = showFilter;

        // Show label in corner if text given and filter is hidden
        if (!showFilter && !string.IsNullOrEmpty(CornerText))
        {
            thcFirstColumn.Text += HTMLHelper.HTMLEncode(CornerText);
        }

        // Initialize filter if displayed
        if (ShowFilterRow && showFilter)
        {
            btnFilter.ResourceString = "general.search";
            thrFirstRow.AddCssClass("with-filter");
        }
        else if (!ShowHeaderRow)
        {
            plcFilter.Visible = false;
        }
    }


    /// <summary>
    /// Load matrix data
    /// </summary>
    /// <param name="whereCondition">Where condition to filter data</param>
    /// <param name="orderBy">Order by clause to sort data</param>
    /// <param name="currentPage">Current data page to be displayed</param>
    /// <param name="displayOnlyHeader">Indicates if only matrix header should be displayed to user</param>
    private List<DataRow> LoadData(string whereCondition, string orderBy, int currentPage, ref bool displayOnlyHeader)
    {
        List<DataRow> matrixData = null;
        bool load = true;

        // Load the data
        while (load)
        {
            // Get specific page
            int pageItems = ColumnsCount * pagerElem.UniPager.PageSize;
            ds = ConnectionHelper.ExecuteQuery(QueryName, QueryParameters, whereCondition, orderBy, 0, null, (currentPage - 1) * pageItems, pageItems, ref mTotalRows);

            HasData = !DataHelper.DataSourceIsEmpty(ds);

            // If no records found, get the records for the original dataset
            if (!HasData && !String.IsNullOrEmpty(FilterWhere))
            {
                // Get only first line
                ds = ConnectionHelper.ExecuteQuery(QueryName, QueryParameters, WhereCondition, orderBy, ColumnsCount);
                HasData = !DataHelper.DataSourceIsEmpty(ds);
                displayOnlyHeader = true;
            }

            // Load the list of columns
            if (HasData)
            {
                if (DataLoaded != null)
                {
                    DataLoaded(ds);
                }

                matrixData = DataHelper.GetUniqueRows(ds.Tables[0], ColumnItemIDColumn);
                ColumnOrderIndex = GetColumnIndexes(matrixData, ColumnsPreferedOrder);

                // If more than current columns count found, and there is more data, get the correct data again
                if ((matrixData.Count <= ColumnsCount) || (mTotalRows < pageItems))
                {
                    load = false;
                }
                else
                {
                    ColumnsCount = matrixData.Count;
                }
            }
            else
            {
                load = false;
            }
        }

        return matrixData;
    }


    /// <summary>
    /// Gets matrix order by clause
    /// </summary>
    private string GetOrderByClause()
    {
        // Prepare the order by
        string orderBy = OrderBy;
        if (orderBy == null)
        {
            orderBy = RowItemDisplayNameColumn + SqlHelper.ORDERBY_ASC;

            // Add additional sorting by codename for equal display names
            if (!String.IsNullOrEmpty(RowItemCodeNameColumn))
            {
                orderBy += ", " + RowItemCodeNameColumn;
            }

            if (ColumnsCount > 1)
            {
                orderBy += ", " + ColumnItemDisplayNameColumn + SqlHelper.ORDERBY_ASC;
            }
        }

        return orderBy;
    }

    #endregion
}