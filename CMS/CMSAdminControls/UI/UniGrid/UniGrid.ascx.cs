using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;

using CMS.Base;
using CMS.Controls;
using CMS.DataEngine;
using CMS.EventLog;
using CMS.ExtendedControls;
using CMS.FormControls;
using CMS.FormEngine;
using CMS.Helpers;
using CMS.IO;
using CMS.Localization;
using CMS.MacroEngine;
using CMS.Membership;
using CMS.SiteProvider;
using CMS.UIControls;
using CMS.UIControls.UniGridConfig;
using Action = CMS.UIControls.UniGridConfig.Action;

public partial class CMSAdminControls_UI_UniGrid_UniGrid : UniGrid, IUniPageable, ICallbackEventHandler
{
    #region "Constants"

    private const int HALF_PAGE_COUNT_LIMIT = 1000;

    private const string DEFAULT_ACTIONS_MENU = "~/CMSAdminControls/UI/UniGrid/Controls/UniGridMenu.ascx";

    #endregion


    #region "Variables"

    private Button mShowButton;

    private bool mResetSelection;

    private bool mCustomFilterAdded;

    private string callbackArguments = null;

    #endregion


    #region "Properties"

    /// <summary>
    /// Indicates if control is used on live site
    /// </summary>
    public override bool IsLiveSite
    {
        get
        {
            return base.IsLiveSite;
        }
        set
        {
            base.IsLiveSite = value;
            plcMess.IsLiveSite = value;
        }
    }


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
    /// Gets <see cref="UIGridView"/> control of UniGrid.
    /// </summary>
    public override UIGridView GridView
    {
        get
        {
            return UniUiGridView;
        }
    }


    /// <summary>
    /// Hidden field containing selected items.
    /// </summary>
    public override HiddenField SelectionHiddenField
    {
        get
        {
            return hidSelection;
        }
    }


    /// <summary>
    /// Gets <see cref="UIPager"/> control of UniGrid.
    /// </summary>
    public override UIPager Pager
    {
        get
        {
            return pagerElem;
        }
    }


    /// <summary>
    /// If true, relative ancestor div is checked in context menu
    /// </summary>
    public bool CheckRelative
    {
        get;
        set;
    }


    /// <summary>
    /// Gets or sets selected items of UniGrid.
    /// Returns empty collection if hash validation fails.
    /// </summary>
    public override List<string> SelectedItems
    {
        get
        {
            return GetHiddenValues(SelectionHiddenField, hidSelectionHash);
        }
        set
        {
            SetHiddenValues(value, SelectionHiddenField, hidSelectionHash);
            OriginallySelected = value;
        }
    }


    /// <summary>
    /// Gets array list of originally selected items.
    /// </summary>
    private List<string> OriginallySelected
    {
        get
        {
            List<string> selected = ViewState["OriginallySelected"] as List<string>;

            if (selected == null)
            {
                selected = new List<string>();
                ViewState["OriginallySelected"] = selected;
            }

            return selected;
        }
        set
        {
            ViewState["OriginallySelected"] = value;
        }
    }


    /// <summary>
    /// Gets deselected items from UniGrid.
    /// </summary>
    public override List<string> DeselectedItems
    {
        get
        {
            return ValidateHiddenValues(SelectionHiddenField, hidSelectionHash) ? OriginallySelected.Except(SelectedItems).ToList() : new List<string>();
        }
    }


    /// <summary>
    /// Gets newly selected items from UniGrid.
    /// </summary>
    public override List<string> NewlySelectedItems
    {
        get
        {
            return ValidateHiddenValues(SelectionHiddenField, hidSelectionHash) ? SelectedItems.Except(OriginallySelected).ToList() : new List<string>();
        }
    }


    /// <summary>
    /// Gets filter placeHolder from UniGrid.
    /// </summary>
    public override PlaceHolder FilterPlaceHolder
    {
        get
        {
            return plcFilter;
        }
    }


    /// <summary>
    /// Gets page size Drop-down from UniGrid Pager.
    /// </summary>
    public override DropDownList PageSizeDropdown
    {
        get
        {
            return Pager.PageSizeDropdown;
        }
    }


    /// <summary>
    /// Gets filter form.
    /// </summary>
    public override BasicForm FilterForm
    {
        get
        {
            return filterForm;
        }
    }

    #endregion


    #region "Page events"

    /// <summary>
    /// Control's init event handler.
    /// </summary>
    protected void Page_Init(object sender, EventArgs e)
    {
        advancedExportElem.UniGrid = this;
    }


    /// <summary>
    /// Control's load event handler.
    /// </summary>
    protected void Page_Load(object sender, EventArgs e)
    {
        if (StopProcessing)
        {
            Visible = false;
            FilterForm.StopProcessing = true;
        }
        else
        {
            SetPager();

            if (LoadGridDefinition())
            {
                InitializeCustomFilters();

                if (RequestHelper.IsCallback())
                {
                    // Set filter form
                    if (!string.IsNullOrEmpty(FilterFormName))
                    {
                        SetBasicFormFilter();
                    }
                }
                else
                {
                    ActionsHidden = hidActions;
                    ActionsHashHidden = hidActionsHash;

                    // Handle post-backs
                    if (RequestHelper.IsPostBack() && (Request.Form[Page.postEventSourceID] == UniqueID))
                    {
                        if ((Request.Form[Page.postEventArgumentID] == "UniGridAction") && !string.IsNullOrEmpty(hidCmdName.Value))
                        {
                            // Raise row action command
                            HandleAction(hidCmdName.Value, hidCmdArg.Value);
                        }
                        else if (Request.Form[Page.postEventArgumentID] == "ClearOriginallySelectedItems")
                        {
                            OriginallySelected = new List<string>();
                        }
                    }

                    // Set order by clause
                    ProcessSorting();

                    // Set filter form
                    if (!string.IsNullOrEmpty(FilterFormName))
                    {
                        SetBasicFormFilter();
                        if (!IsInternalPostBack() && !DelayedReload)
                        {
                            ReloadData();
                        }
                    }
                    // Get data from database and set them to the grid view
                    else if (FilterByQueryString)
                    {
                        if (displayFilter)
                        {
                            SetFilter(true);
                        }
                        else
                        {
                            if (!IsInternalPostBack() && !DelayedReload)
                            {
                                ReloadData();
                            }
                        }
                    }
                    else
                    {
                        // Load the default filter value
                        if (!RequestHelper.IsPostBack() && displayFilter)
                        {
                            SetFilter(true);
                        }
                        else if (!IsInternalPostBack() && !DelayedReload)
                        {
                            ReloadData();
                        }
                    }
                }
            }
        }

        // Clear hidden action on load event. If UniGrid is invisible, page pre render is not fired
        ClearActions();
    }


    /// <summary>
    /// Control's PreRender event handler.
    /// </summary>
    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        if (FilterIsSet)
        {
            // Check for FilteredZeroRowsText
            if ((GridView.Rows.Count == 0) && !String.IsNullOrEmpty(FilteredZeroRowsText))
            {
                // Display filter zero rows text
                lblInfo.Text = FilteredZeroRowsText;
                lblInfo.Visible = true;
                Pager.Visible = false;
            }
            else
            {
                lblInfo.Visible = false;
                Pager.Visible = true;
            }
        }
        else
        {
            // Check for ZeroRowsText
            if (GridView.Rows.Count == 0)
            {
                if (!HideControlForZeroRows && !String.IsNullOrEmpty(ZeroRowsText))
                {
                    // Display zero rows text
                    lblInfo.Text = ZeroRowsText;
                    lblInfo.Visible = true;
                    Pager.Visible = false;

                    // Check additional filter visibility
                    CheckFilterVisibility();
                }
                else
                {
                    lblInfo.Visible = false;
                    Pager.Visible = false;
                    plcFilter.Visible = false;
                }
            }
            else
            {
                lblInfo.Visible = false;
                Pager.Visible = true;

                // Check additional filter visibility
                CheckFilterVisibility();
            }
        }

        if (Visible && !StopProcessing)
        {
            RegisterCmdScripts();
        }

        if (Pager.CurrentPage > HALF_PAGE_COUNT_LIMIT)
        {
            // Enlarge direct page TextBox
            TextBox txtPage = ControlsHelper.GetChildControl(Pager, typeof(TextBox), "txtPage") as TextBox;
            if (txtPage != null)
            {
                txtPage.Style.Add(HtmlTextWriterStyle.Width, "50px");
            }
        }

        advancedExportElem.Visible = ShowActionsMenu;

        // Hide info label when error message is displayed
        lblInfo.Visible &= String.IsNullOrEmpty(plcMess.ErrorText);
    }

    #endregion


    #region "Public methods"

    /// <summary>
    /// Clears UniGrid's information on recently performed action. Under normal circumstances there is no need to perform this action.
    /// However sometimes forcing grid to clear the actions is required.
    /// </summary>
    public void ClearActions()
    {
        // Clear hidden fields
        hidCmdName.Value = null;
        hidCmdArg.Value = null;
    }


    /// <summary>
    /// Clears all selected items from hidden values.
    /// </summary>
    public void ClearSelectedItems()
    {
        ClearHiddenValues(SelectionHiddenField);
    }


    /// <summary>
    /// Loads the XML configuration of the grid.
    /// </summary>
    public bool LoadXmlConfiguration()
    {
        // If no configuration is given, do not process
        if (string.IsNullOrEmpty(GridName))
        {
            return true;
        }
        string xmlFilePath = Server.MapPath(GridName);

        // Check the configuration file
        if (!File.Exists(xmlFilePath))
        {
            ShowError(String.Format(GetString("unigrid.noxmlfile"), xmlFilePath));
            return false;
        }

        // Load the XML configuration
        XmlDocument document = new XmlDocument();
        document.Load(xmlFilePath);
        XmlNode node = document.DocumentElement;

        if (node != null)
        {
            // Load options definition
            XmlNode optionNode = node.SelectSingleNode("options");
            if (optionNode != null)
            {
                GridOptions = new UniGridOptions(optionNode);
            }

            // Load actions definition
            XmlNode actionsNode = node.SelectSingleNode("actions");
            if (actionsNode != null)
            {
                GridActions = new UniGridActions(actionsNode);
            }

            // Load pager definition
            XmlNode pagerNode = node.SelectSingleNode("pager");
            if (pagerNode != null)
            {
                PagerConfig = new UniGridPagerConfig(pagerNode);
            }

            // Select list of "column" nodes
            XmlNode columnsNode = node.SelectSingleNode("columns");
            if (columnsNode != null)
            {
                GridColumns = new UniGridColumns(columnsNode);
            }

            // Try to get ObjectType from definition
            XmlNode objectTypeNode = node.SelectSingleNode("objecttype");
            if (objectTypeNode != null)
            {
                // Get object type information
                LoadObjectTypeDefinition(objectTypeNode);
            }
            else
            {
                // Get query information
                XmlNode queryNode = node.SelectSingleNode("query");
                LoadQueryDefinition(queryNode);
            }

            return true;
        }

        return false;
    }


    /// <summary>
    /// Loads the grid definition.
    /// </summary>
    public override bool LoadGridDefinition()
    {
        if (GridView.Columns.Count == 0)
        {
            using (Panel filterPanel = new Panel())
            {
                filterPanel.CssClass = "form-horizontal form-filter";
                plcFilter.Controls.Clear();

                // Clear all columns from the grid view
                GridView.Columns.Clear();
                if (!LoadXmlConfiguration())
                {
                    return false;
                }
                // Load options
                if (GridOptions != null)
                {
                    LoadOptionsDefinition(GridOptions, filterPanel);
                }
                if ((GridActions == null) && ShowActionsMenu)
                {
                    EmptyAction emptyAction = new EmptyAction();
                    GridActions = new UniGridActions();
                    GridActions.Actions.Add(emptyAction);
                }
                // Actions
                if (GridActions != null)
                {
                    LoadActionsDefinition(GridActions);
                }
                // Load pager configuration
                if (PagerConfig != null)
                {
                    LoadPagerDefinition(PagerConfig);
                }

                // Raise load columns event
                RaiseLoadColumns();
                // Load columns
                if (GridColumns != null)
                {
                    foreach (Column col in GridColumns.Columns)
                    {
                        col.DataBind();

                        // Load column definition
                        LoadColumnDefinition(col, filterPanel);
                    }
                }
                if (displayFilter)
                {
                    // Finish filter form with "Show" button
                    CreateFilterButton(filterPanel);
                }
            }
        }
        return true;
    }


    /// <summary>
    /// Reloads the grid data.
    /// </summary>
    public override void ReloadData()
    {
        try
        {
            // Ensure grid definition before reload data
            LoadGridDefinition();

            if (!URLHelper.IsPostback())
            {
                RestoreState();
            }

            RaiseOnBeforeDataReload();

            // Get Current TOP N
            if (CurrentPageSize > 0)
            {
                int currentPageIndex = Pager.CurrentPage;
                int pageSize = (CurrentPageSize > 0) ? CurrentPageSize : GridView.PageSize;

                CurrentTopN = pageSize * (currentPageIndex + Pager.GroupSize);
            }

            if (CurrentTopN < TopN)
            {
                CurrentTopN = TopN;
            }

            // If first/last button and direct page control in pager is hidden use current topN for better performance
            if (!Pager.ShowDirectPageControl && !Pager.ShowFirstLastButtons)
            {
                TopN = CurrentTopN;
            }

            // Retrieve data
            GridView.DataSource = RetrieveData();

            RaiseOnAfterDataReload();

            SetUnigridControls();

            // Check if DataSource is loaded
            if (DataHelper.DataSourceIsEmpty(GridView.DataSource) && (Pager.CurrentPage > 1))
            {
                Pager.UniPager.CurrentPage = 1;
                ReloadData();
            }

            // Resolve the edit action URL
            if (!String.IsNullOrEmpty(EditActionUrl))
            {
                EditActionUrl = MacroResolver.Resolve(EditActionUrl);
            }

            SortColumns.Clear();
            GridView.DataBind();

            mRowsCount = DataHelper.GetItemsCount(GridView.DataSource);

            CheckFilterVisibility();
        }
        catch (ThreadAbortException)
        {
            // Do not log any exception to event log for ThreadAbortException
        }
        catch (Exception ex)
        {
            // Display tooltip only development mode is enabled
            string desc = null;
            if (SystemContext.DevelopmentMode)
            {
                desc = ex.Message;
            }

            ShowError(GetString("unigrid.error.reload"), desc);

            // Log exception
            EventLogProvider.LogException("UniGrid", "RELOADDATA", ex.InnerException ?? ex);
        }
    }


    /// <summary>
    /// Gets a DataSet with data based on UniGrid's settings.
    /// </summary>
    /// <returns>DataSet with data</returns>
    public override DataSet RetrieveData()
    {
        DataSet ds;

        // If DataSource for UniGrid is query (not DataSet), then execute query
        if (!string.IsNullOrEmpty(Query))
        {
            // Reload the data with current parameters
            ds = ConnectionHelper.ExecuteQuery(Query, QueryParameters, CompleteWhereCondition, CurrentOrder, TopN, Columns, CurrentOffset, CurrentPageSize, ref pagerForceNumberOfResults);
        }
        // If UniGrid is in ObjectType mode, get the data according to given object type.
        else if (InfoObject != null)
        {
            // Get the result set
            var q = InfoObject.GetDataQuery(
                true,
                s =>
                {
                    s.Where(CompleteWhereCondition).OrderBy(CurrentOrder).TopN(TopN).Columns(Columns);
                    s.Parameters = QueryParameters;
                },
                true
            );

            q.IncludeBinaryData = false;
            q.Offset = CurrentOffset;
            q.MaxRecords = CurrentPageSize;

            // Get the data
            ds = q.Result;
            pagerForceNumberOfResults = q.TotalRecords;
        }
        // External DataSet is used
        else
        {
            ds = RaiseDataReload();
            SortUniGridDataSource(ds);
            ds = DataHelper.TrimDataSetPage(ds, CurrentOffset, CurrentPageSize, ref pagerForceNumberOfResults);
        }

        // Add empty DataSet
        if (ds == null)
        {
            ds = new DataSet();
            ds.Tables.Add();
        }

        // Raise event 'OnRetrieveData'
        ds = RaiseAfterRetrieveData(ds);

        return ds;
    }


    /// <summary>
    /// Returns where condition from UniGrid filters.
    /// </summary>
    public override string GetFilter(bool isDataTable)
    {
        string where = string.Empty;

        // Count of the conditions in the 'where clause'
        int whereConditionCount = 0;

        // Process all filter fields
        foreach (string key in FilterFields.Keys)
        {
            UniGridFilterField filterField = FilterFields[key];

            string errorMessage = ValidateFilterField(filterField);

            // Show error message if filter input is not valid
            if (!string.IsNullOrEmpty(errorMessage))
            {
                Label fieldLabel = filterField.Label;
                if (fieldLabel != null)
                {
                    string fieldName = fieldLabel.Text;
                    if (!string.IsNullOrEmpty(fieldName))
                    {
                        string anchorScript = "onclick=\"" + MessagesPlaceHolder.GetAnchorScript(fieldLabel.ClientID, fieldName) + ";\"";
                        errorMessage = string.Format("<span class=\"Anchor\" {0} >{1}</span> {2}", anchorScript, fieldName, errorMessage);
                    }
                }
                ShowError(errorMessage);

                // If filter input is invalid return no data
                return SqlHelper.NO_DATA_WHERE;
            }

            string columnName = (key.LastIndexOfCSafe("#", true) > 0 ? key.Remove(key.LastIndexOfCSafe("#", true)) : key).Trim().TrimStart('[').TrimEnd(']').Trim();
            string filterFormat = filterField.Format;

            // AND in 'where clause'  
            string andExpression;
            Control mainControl = filterField.OptionsControl;
            Control valueControl = filterField.ValueControl;

            if (valueControl is CMSAbstractBaseFilterControl)
            {
                // Custom filters (loaded controls)
                CMSAbstractBaseFilterControl customFilter = (CMSAbstractBaseFilterControl)valueControl;
                string customWhere = customFilter.WhereCondition;
                if (!String.IsNullOrEmpty(customWhere))
                {
                    andExpression = (whereConditionCount > 0) ? " AND " : String.Empty;
                    where += andExpression + customWhere;
                    whereConditionCount++;
                }

                // Prepare query string
                if (FilterByQueryString && RequestHelper.IsPostBack())
                {
                    queryStringHashTable[columnName] = customFilter.Value;
                }
            }
            else if (mainControl is CMSDropDownList)
            {
                // Drop-down list filter
                CMSDropDownList ddlistControl = (CMSDropDownList)mainControl;
                TextBox txtControl = (TextBox)valueControl;

                string textboxValue = txtControl.Text;
                string textboxID = txtControl.ID;

                // Empty field -> no filter is set for this field
                if (!String.IsNullOrEmpty(textboxValue))
                {
                    string op = ddlistControl.SelectedValue;
                    string value = textboxValue.Replace("\'", "''");

                    // Format {0} = column name, {1} = operator, {2} = value, {3} = default condition
                    string defaultFormat = null;

                    if (textboxID.EndsWithCSafe("TextValue"))
                    {
                        switch (op.ToUpperCSafe())
                        {
                            // LIKE operators
                            case WhereBuilder.LIKE:
                                defaultFormat = isDataTable ? "[{0}] {1} '%{2}%'" : "[{0}] {1} N'%{2}%'";
                                break;

                            case WhereBuilder.NOT_LIKE:
                                defaultFormat = isDataTable ? "([{0}] is null or [{0}] {1} '%{2}%')" : "([{0}] is null or [{0}] {1} N'%{2}%')";
                                break;

                            case WhereBuilder.NOT_EQUAL:
                                defaultFormat = isDataTable ? "([{0}] is null or [{0}] {1} '{2}')" : "([{0}] is null or [{0}] {1} N'{2}')";
                                break;

                            // Standard operators
                            default:
                                defaultFormat = isDataTable ? "[{0}] {1} '{2}'" : "[{0}] {1} N'{2}'";
                                break;
                        }
                    }
                    else // textboxID.EndsWithCSafe("NumberValue")
                    {
                        if (ValidationHelper.IsDouble(value) || ValidationHelper.IsInteger(value))
                        {
                            defaultFormat = "[{0}] {1} {2}";

                            if (op == "<>")
                            {
                                defaultFormat = "([{0}] is null or [{0}] {1} {2})";
                            }
                        }
                    }

                    if (!String.IsNullOrEmpty(defaultFormat))
                    {
                        string defaultCondition = String.Format(defaultFormat, columnName, op, value);

                        string condition = defaultCondition;
                        if (!String.IsNullOrEmpty(filterFormat))
                        {
                            condition = String.Format(filterFormat, columnName, op, value, defaultCondition);
                        }

                        andExpression = (whereConditionCount > 0 ? " AND " : string.Empty);

                        // ddlistControl.ID                 - column name
                        // ddlistControl.SelectedValue      - condition option
                        // textboxSqlValue                  - condition value                        
                        where += String.Format("{0}({1})", andExpression, condition);
                        whereConditionCount++;
                    }
                }

                // Prepare query string
                if (FilterByQueryString)
                {
                    queryStringHashTable[columnName] = String.Format("{0};{1}", ddlistControl.SelectedValue, textboxValue);
                }
            }
            else if (valueControl is CMSDropDownList)
            {
                // Checkbox filter
                CMSDropDownList currentControl = (CMSDropDownList)valueControl;
                string value = currentControl.SelectedValue;
                if (!String.IsNullOrEmpty(value))
                {
                    string condition = String.Format("ISNULL({0}, 0) = {1}", columnName, value);
                    if (!String.IsNullOrEmpty(filterFormat))
                    {
                        condition = String.Format(filterFormat, columnName, "=", value, condition);
                    }
                    andExpression = (whereConditionCount > 0 ? " AND " : string.Empty);
                    where += String.Format("{0}({1})", andExpression, condition);
                    whereConditionCount++;
                }

                // Prepare query string
                if (FilterByQueryString)
                {
                    queryStringHashTable[columnName] = ";" + value;
                }
            }
        }

        return where;
    }


    /// <summary>
    /// Uncheck all checkboxes in selection column.
    /// </summary>
    public override void ResetSelection()
    {
        ResetSelection(true);
    }


    /// <summary>
    /// Uncheck all checkboxes in selection column.
    /// </summary>
    /// <param name="reset">Indicates if reset selection javascript should be registered</param>
    public void ResetSelection(bool reset)
    {
        SelectionHiddenField.Value = string.Empty;
        hidSelectionHash.Value = string.Empty;
        mResetSelection = reset;
    }

    #endregion


    #region "UniGrid events"

    /// <summary>
    /// Process data from filter.
    /// </summary>
    protected void ShowButton_Click(object sender, EventArgs e)
    {
        ApplyFilter(sender, e);
    }


    protected void ResetButton_Click(object sender, EventArgs e)
    {
        Reset();
    }


    protected void pageSizeDropdown_SelectedIndexChanged(object sender, EventArgs e)
    {
        RaisePageSizeChanged();
    }


    protected void UniGridView_Sorting(object sender, EventArgs e)
    {
        RaiseBeforeSorting(sender, e);
    }


    /// <summary>
    /// After data bound event.
    /// </summary>
    protected void UniGridView_DataBound(object sender, EventArgs e)
    {
        // Set actions hash into hidden field
        SetActionsHash();

        SetPager();

        // Call page binding event
        if (OnPageBinding != null)
        {
            OnPageBinding(this, null);
        }
    }


    protected void UniGridView_RowCreating(object sender, GridViewRowEventArgs e)
    {
        // If row type is header
        if (e.Row.RowType == DataControlRowType.Header)
        {
            // Add sorting definition to list of sort columns
            SortColumns.Add(SortDirect.ToLowerCSafe());

            // Parse the sort expression
            string sort = SortDirect.ToLowerCSafe().Replace("[", "").Replace("]", "").Trim();
            if (sort.StartsWithCSafe("cast("))
            {
                sort = sort.Substring(5);
            }

            Match sortMatch = OrderByRegex.Match(sort);
            string sortColumn = null;
            string sortDirection;
            if (sortMatch.Success)
            {
                // Get column name
                if (sortMatch.Groups[1].Success)
                {
                    sortColumn = sortMatch.Groups[1].Value;
                }
                // Get sort direction
                sortDirection = sortMatch.Groups[2].Success ? sortMatch.Groups[2].Value : "asc";
            }
            else
            {
                // Get column name from sort expression
                int space = sort.IndexOfAny(new char[] { ' ', ',' });
                sortColumn = space > -1 ? sort.Substring(0, space) : sort;
                sortDirection = "asc";
            }

            // Check if displaying arrow indicating sorting is requested
            if (showSortDirection)
            {
                // Prepare the columns
                foreach (TableCell Cell in e.Row.Cells)
                {
                    // If there is some sorting expression
                    DataControlFieldCell dataField = (DataControlFieldCell)Cell;
                    string fieldSortExpression = dataField.ContainingField.SortExpression;
                    if (!DataHelper.IsEmpty(fieldSortExpression))
                    {
                        SortColumns.Add(fieldSortExpression.ToLowerCSafe());

                        // If actual sorting expressions is this cell
                        if (CMSString.Equals(sortColumn, fieldSortExpression.Replace("[", "").Replace("]", "").Trim(), true))
                        {
                            // Initialize sort arrow
                            Literal sortArrow = new Literal()
                            {
                                Text = String.Format("<i class=\"{0}\" aria-hidden=\"true\"></i>", ((sortDirection == "desc") ? "icon-caret-down" : "icon-caret-up"))
                            };

                            if (DataHelper.IsEmpty(Cell.Text))
                            {
                                if (Cell.Controls.Count != 0)
                                {
                                    // Add original text
                                    Cell.Controls[0].Controls.Add(new LiteralControl(String.Format("<span class=\"unigrid-sort-label\">{0}</span>", ((LinkButton)(Cell.Controls[0])).Text)));
                                    Cell.Controls[0].Controls.Add(sortArrow);
                                }
                                else
                                {
                                    Cell.Controls.Add(sortArrow);
                                }
                            }
                        }
                    }
                }
            }
        }
        else if (e.Row.RowType == DataControlRowType.Footer)
        {
            e.Row.CssClass = "unigrid-footer";
        }
        else if (e.Row.RowType == DataControlRowType.Pager)
        {
            e.Row.CssClass = "UniGridPager";
        }
    }


    /// <summary>
    /// Handles the action event.
    /// </summary>
    /// <param name="cmdName">Command name</param>
    /// <param name="cmdValue">Command value</param>
    public void HandleAction(string cmdName, string cmdValue)
    {
        string action = cmdName.ToLowerCSafe();

        // Check action security and redirect if user not authorized
        CheckActionAndRedirect(action);
        switch (action)
        {
            case "#delete":
            case "#destroyobject":
            case "#moveup":
            case "#movedown":
                {
                    // Delete the object
                    int objectId = ValidationHelper.GetInteger(cmdValue, 0);
                    if (objectId > 0)
                    {
                        BaseInfo infoObj = ModuleManager.GetReadOnlyObject(ObjectType);
                        string objectType = infoObj.TypeInfo.Inherited ? infoObj.TypeInfo.OriginalObjectType : infoObj.ObjectType;
                        infoObj = BaseAbstractInfoProvider.GetInfoById(objectType, objectId);

                        if (infoObj != null)
                        {
                            switch (action)
                            {
                                case "#delete":
                                    if (infoObj.CheckPermissions(PermissionsEnum.Delete, SiteContext.CurrentSiteName, CurrentUser))
                                    {
                                        try
                                        {
                                            // Delete the object
                                            infoObj.Delete();
                                        }
                                        catch (CheckDependenciesException)
                                        {
                                            // Check the dependencies a display message
                                            List<string> names = infoObj.Generalized.GetDependenciesNames();
                                            if ((names != null) && (names.Count > 0))
                                            {
                                                string description = null;
                                                if (names.Count > 0)
                                                {
                                                    // Encode and localize names
                                                    StringBuilder sb = new StringBuilder();
                                                    names.ForEach(item => sb.Append("<br />", HTMLHelper.HTMLEncode(ResHelper.LocalizeString(item))));
                                                    description = GetString(objectType.Replace(".", "_") + ".objectlist|unigrid.objectlist") + sb;
                                                }

                                                ShowError(GetString("unigrid.deletedisabledwithoutenable"), description);
                                                return;
                                            }
                                        }

                                        // Raid for additional actions
                                        RaiseAction(cmdName, cmdValue);
                                    }
                                    break;

                                case "#destroyobject":
                                    if (MembershipContext.AuthenticatedUser.IsAuthorizedPerObject(PermissionsEnum.Destroy, infoObj.ObjectType, SiteContext.CurrentSiteName))
                                    {
                                        using (CMSActionContext context = new CMSActionContext())
                                        {
                                            context.CreateVersion = false;

                                            Action ac = GridActions.GetAction("#delete");
                                            if (ac != null)
                                            {
                                                HandleAction("#delete", cmdValue);
                                            }
                                            else
                                            {
                                                ac = GridActions.GetAction("delete");
                                                if (ac != null)
                                                {
                                                    RaiseAction("delete", cmdValue);
                                                }
                                                else
                                                {
                                                    ShowError(GetString("objectversioning.destroyobject.nodeleteaction"));
                                                    return;
                                                }
                                            }
                                        }
                                    }
                                    break;

                                case "#moveup":
                                    if (infoObj.CheckPermissions(PermissionsEnum.Modify, SiteContext.CurrentSiteName, CurrentUser))
                                    {
                                        infoObj.Generalized.MoveObjectUp();
                                    }
                                    break;

                                case "#movedown":
                                    if (infoObj.CheckPermissions(PermissionsEnum.Modify, SiteContext.CurrentSiteName, CurrentUser))
                                    {
                                        infoObj.Generalized.MoveObjectDown();
                                    }
                                    break;
                            }
                        }
                    }
                }
                break;


            default:
                RaiseAction(cmdName, cmdValue);
                break;
        }
    }

    #endregion


    #region "Private methods"

    /// <summary>
    /// Sets UniGrid controls.
    /// </summary>
    private void SetUnigridControls()
    {
        plcFilter.Visible = displayFilter;

        // Indicates whether UniGrid DataSource is empty or not
        isEmpty = DataHelper.DataSourceIsEmpty(GridView.DataSource);

        if (isEmpty)
        {
            // Try to reload data for previous page if action was used and no data loaded (mostly delete)
            if (onActionUsed && Pager.CurrentPage > 1)
            {
                Pager.UniPager.CurrentPage = Pager.CurrentPage - 1;
                ReloadData();
            }
            else if (HideControlForZeroRows && String.IsNullOrEmpty(WhereClause))
            {
                // Hide filter
                plcFilter.Visible = false;
            }
        }
        else
        {
            // Disable GridView paging because UniGridPager will provide paging
            GridView.AllowPaging = false;
        }
    }


    /// <summary>
    /// Load options definition.
    /// </summary>
    /// <param name="options">Options configuration</param>
    /// <param name="filterWrapperControl">Wrapper control for filter</param>
    private void LoadOptionsDefinition(UniGridOptions options, Control filterWrapperControl)
    {
        // Add custom filter or filter wrapper panel according to the key value "DisplayFilter"
        displayFilter = options.DisplayFilter;

        if (displayFilter)
        {
            // Add custom filter
            if (!mCustomFilterAdded && !string.IsNullOrEmpty(options.FilterPath))
            {
                UniGridFilterField filterDefinition = new UniGridFilterField();
                CMSAbstractBaseFilterControl filterControl = LoadFilterControl(options.FilterPath, CUSTOM_FILTER_SOURCE_NAME, null, filterDefinition);
                FilterFields[CUSTOM_FILTER_SOURCE_NAME] = filterDefinition;
                mCustomFilterAdded = true;

                plcFilter.Controls.Add(filterControl);

                RaiseOnFilterFieldCreated(null, filterDefinition);
            }
            // Add wrapper panel for default filter
            else
            {
                plcFilter.Controls.Add(filterWrapperControl);
            }
        }

        // Filter limit
        if (options.FilterLimit > -1)
        {
            FilterLimit = options.FilterLimit;
        }

        // Display sort direction images
        showSortDirection = options.ShowSortDirection;

        // Display selection column with checkboxes
        showSelection = options.ShowSelection;
        if (showSelection)
        {
            TemplateField chkColumn = new TemplateField();

            using (CMSCheckBox headerBox = new CMSCheckBox
            {
                ID = "headerBox"
            })
            {
                using (CMSCheckBox itemBox = new CMSCheckBox
                {
                    ID = "itemBox"
                })
                {
                    // Set selection argument
                    itemBox.Attributes["selectioncolumn"] = options.SelectionColumn;
                    chkColumn.HeaderTemplate = new GridViewTemplate(ListItemType.Header, this, headerBox);
                    chkColumn.ItemTemplate = new GridViewTemplate(ListItemType.Item, this, itemBox);
                }
            }
            GridView.Columns.Add(chkColumn);
        }

        // PageSize and DisplayPageSizeDropdown properties are obsolete. 
        // This code ensures backward compatibility.
        // #pragma statement disables warnings for using obsolete attribute.
#pragma warning disable 612, 618
        if (!String.IsNullOrEmpty(options.PageSize))
        {
            Pager.PageSizeOptions = options.PageSize;
        }

        // Set paging according to the key value "DisplayPageSizeDropdown"                
        if (options.DisplayPageSizeDropdown != null)
        {
            Pager.ShowPageSize = options.DisplayPageSizeDropdown.Value;
        }
#pragma warning restore 612, 618
    }


    /// <summary>
    /// Loads actions definition.
    /// </summary>
    /// <param name="actions">Configuration of the actions</param>
    private void LoadActionsDefinition(UniGridActions actions)
    {
        // Custom template field of the grid view
        TemplateField actionsColumn = new TemplateField();

        // Ensure width of the column
        if (!String.IsNullOrEmpty(actions.Width))
        {
            actionsColumn.ItemStyle.Width = new Unit(actions.Width);
        }

        // Add object menu if possible
        if ((actions.Actions.Count > 0 && !(actions.Actions.FirstOrDefault() is EmptyAction)) && ShowObjectMenu && UniGridFunctions.ShowUniGridObjectContextMenu(ModuleManager.GetReadOnlyObject(ObjectType)))
        {
            actions.Actions.RemoveAll(a => a is EmptyAction);

            // Check if object menu already contained
            var menus = from action in actions.Actions.OfType<Action>()
                        where (action.Name.ToLowerCSafe() == "#objectmenu") || (!String.IsNullOrEmpty(action.ContextMenu))
                        select action;

            // Add object menu of necessary
            if ((menus.Count() == 0) && !IsLiveSite)
            {
                Action action = new Action("#objectmenu");
                action.ExternalSourceName = "#objectmenu";
                actions.Actions.Add(action);
            }
        }

        // Show header?
        if (actions.ShowHeader)
        {
            if (ShowActionsMenu && string.IsNullOrEmpty(actions.ContextMenu))
            {
                actions.ContextMenu = DEFAULT_ACTIONS_MENU;
                actions.Caption = "General.OtherActions";
            }

            // Fill in the custom template field
            string label = (ShowActionsLabel ? GetString("unigrid.actions") : String.Empty);

            GridViewTemplate headerTemplate = new GridViewTemplate(ListItemType.Header, this, actions, label, ImageDirectoryPath, DefaultImageDirectoryPath, Page);

            headerTemplate.ContextMenuParent = plcContextMenu;
            headerTemplate.CheckRelative = CheckRelative;

            actionsColumn.HeaderTemplate = headerTemplate;

            if (ShowActionsMenu)
            {
                if (actions.Actions.FirstOrDefault() is EmptyAction)
                {
                    actionsColumn.HeaderStyle.CssClass = "unigrid-actions-header-empty";
                }
                else
                {
                    actionsColumn.HeaderStyle.CssClass = "unigrid-actions-header";
                }
            }
        }
        GridViewTemplate actionsTemplate = new GridViewTemplate(ListItemType.Item, this, actions, null, ImageDirectoryPath, DefaultImageDirectoryPath, Page);
        actionsTemplate.OnExternalDataBound += RaiseExternalDataBound;
        actionsTemplate.ContextMenuParent = plcContextMenu;
        actionsTemplate.CheckRelative = CheckRelative;

        actionsColumn.ItemTemplate = actionsTemplate;

        if (IsLiveSite)
        {
            actionsColumn.ItemStyle.Wrap = false;
        }

        if (!String.IsNullOrEmpty(actions.CssClass))
        {
            actionsColumn.HeaderStyle.CssClass = CSSHelper.JoinClasses(actionsColumn.HeaderStyle.CssClass, actions.CssClass);
            actionsColumn.ItemStyle.CssClass = CSSHelper.JoinClasses(actionsColumn.ItemStyle.CssClass, actions.CssClass);
            actionsColumn.FooterStyle.CssClass = CSSHelper.JoinClasses(actionsColumn.FooterStyle.CssClass, actions.CssClass);
        }

        // Add custom column to grid view
        GridView.Columns.Add(actionsColumn);
    }


    /// <summary>
    /// Load UniGrid pager configuration.
    /// </summary>
    /// <param name="config">Pager configuration</param>
    private void LoadPagerDefinition(UniGridPagerConfig config)
    {
        if (config.DisplayPager != null)
        {
            Pager.DisplayPager = config.DisplayPager.Value;
        }

        // Load definition only if pager is displayed
        if (Pager.DisplayPager)
        {
            if (config.PageSizeOptions != null)
            {
                Pager.PageSizeOptions = config.PageSizeOptions;
            }
            if (config.ShowDirectPageControl != null)
            {
                Pager.ShowDirectPageControl = config.ShowDirectPageControl.Value;
            }
            if (config.ShowFirstLastButtons != null)
            {
                Pager.ShowFirstLastButtons = config.ShowFirstLastButtons.Value;
            }
            if (config.ShowPageSize != null)
            {
                Pager.ShowPageSize = config.ShowPageSize.Value;
            }
            if (config.ShowPreviousNextButtons != null)
            {
                Pager.ShowPreviousNextButtons = config.ShowPreviousNextButtons.Value;
            }
            if (config.ShowPreviousNextPageGroup != null)
            {
                Pager.ShowPreviousNextPageGroup = config.ShowPreviousNextPageGroup.Value;
            }
            if (config.GroupSize > 0)
            {
                Pager.GroupSize = config.GroupSize;
            }
            if (config.DefaultPageSize > 0)
            {
                Pager.DefaultPageSize = config.DefaultPageSize;
            }

            // Try to get page size from request
            string selectedPageSize = Request.Form[Pager.PageSizeDropdown.UniqueID];
            int pageSize = 0;

            if (selectedPageSize != null)
            {
                pageSize = ValidationHelper.GetInteger(selectedPageSize, 0);
            }
            else if (config.DefaultPageSize > 0)
            {
                pageSize = config.DefaultPageSize;
            }

            if ((pageSize > 0) || (pageSize == -1))
            {
                Pager.CurrentPageSize = pageSize;
            }
        }
        else
        {
            // Reset page size
            Pager.CurrentPageSize = -1;
        }
    }


    /// <summary>
    /// Load single column definition.
    /// </summary>
    /// <param name="column">Column to use</param>
    /// <param name="filterWrapperControl">Wrapper control for filter</param>
    private void LoadColumnDefinition(Column column, Control filterWrapperControl)
    {
        DataControlField field;
        string cssClass = column.CssClass;
        string columnCaption = null;

        // Process the column type Hyperlink or BoundColumn based on the parameters
        if ((column.Href != null) ||
            (column.ExternalSourceName != null) ||
            (column.Localize) ||
            (column.Icon != null) ||
            (column.Tooltip != null) ||
            (column.Action != null) ||
            (column.Style != null) ||
            (column.MaxLength > 0))
        {
            ExtendedBoundField linkColumn = new ExtendedBoundField();
            field = linkColumn;

            // Attribute "source"
            if (column.Source != null)
            {
                linkColumn.DataField = column.Source;

                // Allow sorting
                if ((column.AllowSorting) && ((GridOptions == null) || GridOptions.AllowSorting))
                {
                    if (!String.IsNullOrEmpty(column.Sort))
                    {
                        linkColumn.SortExpression = column.Sort;
                    }
                    else if (column.Source.ToLowerCSafe() != ExtendedBoundField.ALL_DATA.ToLowerCSafe())
                    {
                        linkColumn.SortExpression = column.Source;
                    }
                }
            }

            // Action parameters
            if (column.Action != null)
            {
                linkColumn.Action = column.Action;

                // Action parameters
                if (column.CommandArgument != null)
                {
                    linkColumn.CommandArgument = column.CommandArgument;
                }
            }

            // Action parameters
            if (column.Parameters != null)
            {
                linkColumn.ActionParameters = column.Parameters;
            }

            // Navigate URL
            if (column.Href != null)
            {
                linkColumn.NavigateUrl = column.Href;
            }

            // External source
            if (column.ExternalSourceName != null)
            {
                linkColumn.ExternalSourceName = column.ExternalSourceName;
                linkColumn.OnExternalDataBound += RaiseExternalDataBound;
            }

            // Localize strings?
            linkColumn.LocalizeStrings = column.Localize;

            // Style
            if (column.Style != null)
            {
                linkColumn.Style = column.Style;
            }

            // Icon
            if (column.Icon != null)
            {
                if (String.IsNullOrEmpty(linkColumn.DataField))
                {
                    linkColumn.DataField = ExtendedBoundField.ALL_DATA;
                }
                linkColumn.Icon = GetActionImage(column.Icon);
            }

            // Max length
            if (column.MaxLength > 0)
            {
                linkColumn.MaxLength = column.MaxLength;
            }

            // Process "tooltip" node
            ColumnTooltip tooltip = column.Tooltip;
            if (tooltip != null)
            {
                // If there is some tooltip register TooltipScript
                if ((tooltip.Source != null) || (tooltip.ExternalSourceName != null))
                {
                    ScriptHelper.RegisterTooltip(Page);
                }

                // Tooltip source
                if (tooltip.Source != null)
                {
                    linkColumn.TooltipSourceName = tooltip.Source;
                }

                // Tooltip external source
                if (tooltip.ExternalSourceName != null)
                {
                    linkColumn.TooltipExternalSourceName = tooltip.ExternalSourceName;

                    // Ensure external data bound event handler
                    if (string.IsNullOrEmpty(column.ExternalSourceName))
                    {
                        linkColumn.OnExternalDataBound += RaiseExternalDataBound;
                    }
                }

                // Tooltip width
                if (tooltip.Width != null)
                {
                    linkColumn.TooltipWidth = tooltip.Width;
                }

                // Encode tooltip
                linkColumn.TooltipEncode = tooltip.Encode;
            }
        }
        else
        {
            BoundField userColumn = new BoundField(); // Custom column of the grid view
            field = userColumn;

            // Attribute "source"
            if (column.Source != null)
            {
                userColumn.DataField = column.Source;

                // Allow sorting
                if ((column.AllowSorting) && ((GridOptions == null) || GridOptions.AllowSorting))
                {
                    if (column.Source.ToLowerCSafe() != ExtendedBoundField.ALL_DATA.ToLowerCSafe())
                    {
                        userColumn.SortExpression = column.Source;
                    }
                    else if (column.Sort != null)
                    {
                        userColumn.SortExpression = column.Sort;
                    }
                }
            }
        }

        if (IsLiveSite)
        {
            field.HeaderStyle.Wrap = false;
        }

        // Column name
        if (column.Name != null)
        {
            NamedColumns[column.Name] = field;
        }

        // Caption
        if (column.Caption != null)
        {
            columnCaption = GetString(LocalizationHelper.GetResourceName(column.Caption));
            field.HeaderText = columnCaption;
        }

        // Width
        if (column.Width != null)
        {
            if (GridView.ShowHeader)
            {
                field.HeaderStyle.Width = new Unit(column.Width);
            }
            else
            {
                field.ItemStyle.Width = new Unit(column.Width);
            }
        }

        // Visible
        field.Visible = column.Visible;

        // Is text?
        if (column.IsText && (column.Source != null))
        {
            TextColumns.Add(column.Source);
        }

        // Wrap?
        if (column.Wrap)
        {
            field.ItemStyle.CssClass = "wrap-normal";
        }
        else if (IsLiveSite)
        {
            field.ItemStyle.Wrap = false;
        }

        // Class name
        if (cssClass != null)
        {
            field.HeaderStyle.CssClass = CSSHelper.JoinClasses(field.HeaderStyle.CssClass, cssClass);
            field.ItemStyle.CssClass = CSSHelper.JoinClasses(field.ItemStyle.CssClass, cssClass);
            field.FooterStyle.CssClass = CSSHelper.JoinClasses(field.FooterStyle.CssClass, cssClass);
        }

        // Process "filter" node
        if (displayFilter && !mCustomFilterAdded)
        {
            // Filter
            ColumnFilter filter = column.Filter;
            if (filter != null)
            {
                string value = null;

                // Filter via query string
                if (FilterByQueryString)
                {
                    if (String.IsNullOrEmpty(filter.Path))
                    {
                        string values = QueryHelper.GetString(column.Source, null);
                        if (!string.IsNullOrEmpty(values))
                        {
                            string[] pair = values.Split(';');
                            value = pair[1];
                        }
                    }
                    else
                    {
                        value = QueryHelper.GetString(column.Source, null);
                    }
                }

                AddFilterField(filter, column.Source, columnCaption, filterWrapperControl, value);
            }
        }

        // Add custom column to GridView
        GridView.Columns.Add(field);

        // Store corresponding field
        column.Field = field;
    }


    /// <summary>
    /// Load query definition from XML.
    /// </summary>
    /// <param name="objectTypeNode">XML query definition node</param>
    private void LoadObjectTypeDefinition(XmlNode objectTypeNode)
    {
        if (objectTypeNode != null)
        {
            ObjectType = objectTypeNode.Attributes["name"].Value;

            // Set the columns property if columns are defined
            LoadColumns(objectTypeNode);
        }
    }


    /// <summary>
    /// Load query definition from XML.
    /// </summary>
    /// <param name="queryNode">XML query definition node</param>
    private void LoadQueryDefinition(XmlNode queryNode)
    {
        if (queryNode != null)
        {
            Query = queryNode.Attributes["name"].Value;

            // Set the columns property if columns are defined
            LoadColumns(queryNode);
            LoadAllColumns(queryNode);

            // Load the query parameters
            XmlNodeList parameters = queryNode.SelectNodes("parameter");
            if ((parameters != null) && (parameters.Count > 0))
            {
                QueryDataParameters newParams = new QueryDataParameters();

                // Process all parameters
                foreach (XmlNode param in parameters)
                {
                    object value = null;
                    string name = param.Attributes["name"].Value;

                    switch (param.Attributes["type"].Value.ToLowerCSafe())
                    {
                        case "string":
                            value = param.Attributes["value"].Value;
                            break;

                        case "int":
                            value = ValidationHelper.GetInteger(param.Attributes["value"].Value, 0);
                            break;

                        case "double":
                            value = Convert.ToDouble(param.Attributes["value"].Value);
                            break;

                        case "bool":
                            value = Convert.ToBoolean(param.Attributes["value"].Value);
                            break;
                    }

                    newParams.Add(name, value);
                }

                QueryParameters = newParams;
            }
        }
    }


    /// <summary>
    /// Sets the columns property if columns are defined.
    /// </summary>
    /// <param name="queryNode">Node from which to load columns</param>
    private void LoadAllColumns(XmlNode queryNode)
    {
        XmlAttribute allColumns = queryNode.Attributes["allcolumns"];
        if (allColumns != null)
        {
            AllColumns = DataHelper.GetNotEmpty(allColumns.Value, AllColumns);
        }
    }


    /// <summary>
    /// Sets the columns property if columns are defined.
    /// </summary>
    /// <param name="queryNode">Node from which to load columns</param>
    private void LoadColumns(XmlNode queryNode)
    {
        XmlAttribute columns = queryNode.Attributes["columns"];
        if (columns != null)
        {
            Columns = DataHelper.GetNotEmpty(columns.Value, Columns);
        }
    }


    /// <summary>
    /// Add filter field to the filter table.
    /// </summary>
    /// <param name="filter">Filter definition</param>
    /// <param name="columnSourceName">Column source field name</param>
    /// <param name="fieldDisplayName">Field display name</param>
    /// <param name="filterWrapperControl">Wrapper control for filter</param>
    /// <param name="filterValue">Filter value</param>
    private void AddFilterField(ColumnFilter filter, string columnSourceName, string fieldDisplayName, Control filterWrapperControl, string filterValue)
    {
        string fieldSourceName = filter.Source ?? columnSourceName;
        if (String.IsNullOrEmpty(fieldSourceName) || (filter == null) || (filterWrapperControl == null))
        {
            return;
        }

        string fieldPath = filter.Path;
        string filterFormat = filter.Format;
        int filterSize = filter.Size;
        Unit filterWidth = filter.Width;

        Panel fieldWrapperPanel = new Panel()
        {
            CssClass = "form-group"
        };

        Panel fieldLabelPanel = new Panel()
        {
            CssClass = "filter-form-label-cell"
        };

        Panel fieldOptionPanel = new Panel()
        {
            CssClass = "filter-form-condition-cell"
        };

        Panel fieldInputPanel = new Panel()
        {
            CssClass = "filter-form-value-cell"
        };

        // Ensure fieldSourceName is JavaScript valid
        fieldSourceName = fieldSourceName.Replace(ALL, "__ALL__");

        int index = GetNumberOfFilterFieldsWithSameSourceColumn(fieldSourceName);
        string filterControlID = fieldSourceName + (index > 0 ? index.ToString() : String.Empty);

        Label textName = new Label
        {
            Text = String.IsNullOrEmpty(fieldDisplayName) ? String.Empty : fieldDisplayName + ":",
            ID = String.Format("{0}Name", filterControlID),
            EnableViewState = false,
            CssClass = "control-label"
        };

        fieldLabelPanel.Controls.Add(textName);
        fieldWrapperPanel.Controls.Add(fieldLabelPanel);

        // Filter value
        string value = null;
        if (filterValue != null)
        {
            value = ValidationHelper.GetString(filterValue, null);
        }

        // Filter definition
        UniGridFilterField filterDefinition = new UniGridFilterField();
        filterDefinition.Type = filter.Type;
        filterDefinition.Label = textName;
        filterDefinition.Format = filterFormat;
        filterDefinition.FilterRow = fieldWrapperPanel;

        // Set the filter default value
        string defaultValue = filter.DefaultValue;

        // Remember default values of filter field controls for potential UniGrid reset
        string optionFilterFieldValue = null;
        string valueFilterFieldValue = null;

        switch (filterDefinition.Type)
        {
            // Text filter
            case UniGridFilterTypeEnum.Text:
                {
                    CMSDropDownList textOptionFilterField = new CMSDropDownList();
                    ControlsHelper.FillListWithTextSqlOperators(textOptionFilterField);
                    textOptionFilterField.ID = filterControlID;

                    // Set the value
                    SetDropdownValue(value, null, textOptionFilterField);
                    optionFilterFieldValue = textOptionFilterField.SelectedValue;

                    LocalizedLabel lblSelect = new LocalizedLabel
                    {
                        EnableViewState = false,
                        CssClass = "sr-only",
                        AssociatedControlID = textOptionFilterField.ID,
                        ResourceString = "filter.mode"
                    };

                    fieldOptionPanel.Controls.Add(lblSelect);
                    fieldOptionPanel.Controls.Add(textOptionFilterField);
                    fieldWrapperPanel.Controls.Add(fieldOptionPanel);

                    CMSTextBox textValueFilterField = new CMSTextBox
                    {
                        ID = String.Format("{0}TextValue", filterControlID)
                    };

                    // Set value
                    SetTextboxValue(value, defaultValue, textValueFilterField);
                    valueFilterFieldValue = textValueFilterField.Text;

                    if (filterSize > 0)
                    {
                        textValueFilterField.MaxLength = filterSize;
                    }
                    if (!filterWidth.IsEmpty)
                    {
                        textValueFilterField.Width = filterWidth;
                    }
                    fieldInputPanel.Controls.Add(textValueFilterField);
                    fieldWrapperPanel.Controls.Add(fieldInputPanel);
                    textName.AssociatedControlID = textValueFilterField.ID;

                    filterDefinition.OptionsControl = textOptionFilterField;
                    filterDefinition.ValueControl = textValueFilterField;
                }
                break;

            // Boolean filter
            case UniGridFilterTypeEnum.Bool:
                {
                    CMSDropDownList booleanOptionFilterField = new CMSDropDownList();

                    booleanOptionFilterField.Items.Add(new ListItem(GetString("general.selectall"), String.Empty));
                    booleanOptionFilterField.Items.Add(new ListItem(GetString("general.yes"), "1"));
                    booleanOptionFilterField.Items.Add(new ListItem(GetString("general.no"), "0"));
                    booleanOptionFilterField.ID = filterControlID;
                    textName.AssociatedControlID = booleanOptionFilterField.ID;

                    // Set the value
                    SetDropdownValue(value, defaultValue, booleanOptionFilterField);
                    valueFilterFieldValue = booleanOptionFilterField.SelectedValue;

                    // Set input panel wide for boolean Drop-down list
                    fieldInputPanel.CssClass = "filter-form-value-cell-wide";

                    fieldInputPanel.Controls.Add(booleanOptionFilterField);
                    fieldWrapperPanel.Controls.Add(fieldInputPanel);

                    filterDefinition.ValueControl = booleanOptionFilterField;
                }
                break;

            // Integer filter
            case UniGridFilterTypeEnum.Integer:
            case UniGridFilterTypeEnum.Double:
                {
                    CMSDropDownList numberOptionFilterField = new CMSDropDownList();
                    numberOptionFilterField.Items.Add(new ListItem(GetString("filter.equals"), "="));
                    numberOptionFilterField.Items.Add(new ListItem(GetString("filter.notequals"), "<>"));
                    numberOptionFilterField.Items.Add(new ListItem(GetString("filter.lessthan"), "<"));
                    numberOptionFilterField.Items.Add(new ListItem(GetString("filter.greaterthan"), ">"));
                    numberOptionFilterField.ID = filterControlID;

                    // Set the value
                    SetDropdownValue(value, null, numberOptionFilterField);
                    optionFilterFieldValue = numberOptionFilterField.SelectedValue;

                    LocalizedLabel lblSelect = new LocalizedLabel
                    {
                        EnableViewState = false,
                        CssClass = "sr-only",
                        AssociatedControlID = numberOptionFilterField.ID,
                        ResourceString = "filter.mode"
                    };

                    // Add filter field
                    fieldOptionPanel.Controls.Add(lblSelect);
                    fieldOptionPanel.Controls.Add(numberOptionFilterField);
                    fieldWrapperPanel.Controls.Add(fieldOptionPanel);

                    CMSTextBox numberValueFilterField = new CMSTextBox
                    {
                        ID = String.Format("{0}NumberValue", filterControlID)
                    };

                    // Set value
                    SetTextboxValue(value, defaultValue, numberValueFilterField);
                    valueFilterFieldValue = numberValueFilterField.Text;

                    if (filterSize > 0)
                    {
                        numberValueFilterField.MaxLength = filterSize;
                    }
                    if (!filterWidth.IsEmpty)
                    {
                        numberValueFilterField.Width = filterWidth;
                    }
                    numberValueFilterField.EnableViewState = false;

                    fieldInputPanel.Controls.Add(numberValueFilterField);
                    fieldWrapperPanel.Controls.Add(fieldInputPanel);

                    filterDefinition.OptionsControl = numberOptionFilterField;
                    filterDefinition.ValueControl = numberValueFilterField;
                }
                break;

            case UniGridFilterTypeEnum.Site:
                {
                    // Site selector
                    fieldPath = "~/CMSFormControls/Filters/SiteFilter.ascx";
                }
                break;

            case UniGridFilterTypeEnum.Custom:
                // Load custom path
                {
                    if (String.IsNullOrEmpty(fieldPath))
                    {
                        throw new Exception("[UniGrid.AddFilterField]: Filter field path is not set");
                    }
                }
                break;

            default:
                // Not supported filter type
                throw new Exception("[UniGrid.AddFilterField]: Filter type '" + filterDefinition.Type + "' is not supported. Supported filter types: integer, double, bool, text, site, custom.");
        }

        // Else if filter path is defined use custom filter
        if (fieldPath != null)
        {
            // Add to the controls collection
            CMSAbstractBaseFilterControl filterControl = LoadFilterControl(fieldPath, filterControlID, value, filterDefinition, filter);
            if (filterControl != null)
            {
                // Set default value
                if (!String.IsNullOrEmpty(defaultValue))
                {
                    filterControl.SelectedValue = defaultValue;
                }

                fieldInputPanel.Controls.Add(filterControl);
            }

            fieldInputPanel.CssClass = "filter-form-value-cell-wide";
            fieldWrapperPanel.Controls.Add(fieldInputPanel);
        }

        RaiseOnFilterFieldCreated(fieldSourceName, filterDefinition);
        FilterFields[String.Format("{0}{1}", fieldSourceName, (index > 0 ? "#" + index : String.Empty))] = filterDefinition;

        filterWrapperControl.Controls.Add(fieldWrapperPanel);

        // Store initial filter state for potential UniGrid reset
        if (filterDefinition.OptionsControl != null)
        {
            InitialFilterStateControls.Add(new KeyValuePair<Control, object>(filterDefinition.OptionsControl, optionFilterFieldValue));
        }
        if (filterDefinition.ValueControl != null)
        {
            if (!(filterDefinition.ValueControl is CMSAbstractBaseFilterControl))
            {
                InitialFilterStateControls.Add(new KeyValuePair<Control, object>(filterDefinition.ValueControl, valueFilterFieldValue));
            }
        }
    }


    /// <summary>
    /// Sets the TextBox value to the given value or default value if available
    /// </summary>
    /// <param name="value">Value to set</param>
    /// <param name="defaultValue">Default value</param>
    /// <param name="textBox">TextBox to set</param>
    private static void SetTextboxValue(string value, string defaultValue, TextBox textBox)
    {
        // Set default value
        textBox.Text = !String.IsNullOrEmpty(defaultValue) ? defaultValue : value;
    }


    /// <summary>
    /// Sets the DropDown value to the given value or default value if available
    /// </summary>
    /// <param name="value">Value to set</param>
    /// <param name="defaultValue">Default value</param>
    /// <param name="dropDown">DropDown to set</param>
    private static void SetDropdownValue(string value, string defaultValue, CMSDropDownList dropDown)
    {
        try
        {
            dropDown.SelectedValue = !String.IsNullOrEmpty(defaultValue) ? defaultValue : value;
        }
        catch (ArgumentOutOfRangeException)
        {
        }
    }


    private int GetNumberOfFilterFieldsWithSameSourceColumn(string fieldSourceName)
    {
        return FilterFields.Keys.Where(f => f.EqualsCSafe(fieldSourceName, true) || f.Remove(f.Length - 2).EqualsCSafe(fieldSourceName, true)).Count();
    }


    /// <summary>
    /// Loads the filter control
    /// </summary>
    /// <param name="path">Control path</param>
    /// <param name="fieldSourceName">Field source name</param>
    /// <param name="value">Field value</param>
    /// <param name="filterDefinition">Filter definition</param>
    /// <param name="filterColumn">Filter column</param>    
    private CMSAbstractBaseFilterControl LoadFilterControl(string path, string fieldSourceName, string value, UniGridFilterField filterDefinition, ColumnFilter filterColumn = null)
    {
        string fullPath = (path.StartsWithCSafe("~/") ? path : FilterDirectoryPath + path.TrimStart('/'));

        // Load the filter control
        CMSAbstractBaseFilterControl filterControl = LoadUserControl(fullPath) as CMSAbstractBaseFilterControl;
        if (filterControl != null)
        {
            // Setup the filter control
            filterControl.ID = fieldSourceName;
            filterControl.FilteredControl = this;

            if (!RequestHelper.IsPostBack())
            {
                filterControl.Value = value;
            }

            filterDefinition.ValueControl = filterControl;
            filterDefinition.ControlPath = path;

            ISimpleDataContainer filterSimpleDataContainer = filterControl as ISimpleDataContainer;
            // If filter control is ISimpleDataContainer we can set specified parameters if they are provided
            if (filterColumn != null &&
                filterSimpleDataContainer != null &&
                filterColumn.CustomFilterParameters != null &&
                filterColumn.CustomFilterParameters.Parameters != null)
            {
                filterColumn.CustomFilterParameters.Parameters.ForEach(parameter => filterSimpleDataContainer.SetValue(parameter.Name, parameter.Value));
            }
        }

        return filterControl;
    }


    /// <summary>
    /// Creates filter show button.
    /// </summary>
    private void CreateFilterButton(Control filterControl)
    {
        if (!HideFilterButton && String.IsNullOrEmpty(FilterFormName))
        {
            Panel filterWrapperPanel = new Panel()
            {
                CssClass = "form-group form-group-buttons"
            };

            Panel panelButtons = new Panel()
            {
                CssClass = "filter-form-buttons-cell-wide",
                EnableViewState = false
            };

            // If we remember the grid state we need to add the reset button to allow the user to restore the initial state
            if (RememberState)
            {
                CMSButton resetButton = new CMSButton
                {
                    ID = "btnReset",
                    Text = GetString("general.reset"),
                    ButtonStyle = ButtonStyle.Default,
                    EnableViewState = false
                };
                resetButton.Click += ResetButton_Click;

                panelButtons.Controls.Add(resetButton);
            }

            mShowButton = new CMSButton
            {
                ID = "btnShow",
                Text = GetString("general.filter"),
                ButtonStyle = ButtonStyle.Primary,
                EnableViewState = false
            };

            mShowButton.Click += ShowButton_Click;
            pnlHeader.DefaultButton = mShowButton.ID;
            panelButtons.Controls.Add(mShowButton);

            filterWrapperPanel.Controls.Add(panelButtons);
            if (mCustomFilterAdded)
            {
                plcFilter.Controls.Add(filterWrapperPanel);
            }
            else
            {
                filterControl.Controls.Add(filterWrapperPanel);
            }
        }
    }


    /// <summary>
    /// Sets filter to the grid view and save it to the view state.
    /// </summary>
    /// <param name="reloadData">Reload data</param>
    /// <param name="where">Filter where condition</param>
    protected override void SetFilter(bool reloadData, string where)
    {
        // Where can be empty string - it means that filter condition was added to WhereCondition property 
        if (where == null)
        {
            where = GetFilter();
        }

        // Filter by query string
        if (FilterByQueryString && !reloadData)
        {
            string url = RequestContext.CurrentURL;
            foreach (string name in queryStringHashTable.Keys)
            {
                if (queryStringHashTable[name] != null)
                {
                    string value = HttpContext.Current.Server.UrlEncode(queryStringHashTable[name].ToString());
                    url = URLHelper.AddParameterToUrl(url, name, value);
                }
            }
            URLHelper.Redirect(url);
        }
        else
        {
            WhereClause = where;
            if (!String.IsNullOrEmpty(where))
            {
                FilterIsSet = true;
            }
            if ((!DelayedReload) && (reloadData))
            {
                // Get data from database and set them to the grid view
                ReloadData();
            }
        }
    }


    /// <summary>
    /// Sets filter visibility depending on the UniGrid's configuration and number of objects.
    /// </summary>
    private void CheckFilterVisibility()
    {
        plcFilter.Visible = false;
        plcFilterForm.Visible = false;

        if (displayFilter)
        {
            if (FilterLimit > -1)
            {
                bool filterVisible = FilterIsSet || ShowFilter;
                if (!String.IsNullOrEmpty(FilterFormName))
                {
                    plcFilterForm.Visible = filterVisible;
                }
                else
                {
                    plcFilter.Visible = filterVisible;
                }
            }
        }
    }


    /// <summary>
    /// Sorts UniGrid data source according to sort directive saved in ViewState.
    /// </summary>
    private void SortUniGridDataSource(object ds)
    {
        if (!String.IsNullOrEmpty(SortDirect))
        {
            // If source isn't empty
            if (!DataHelper.DataSourceIsEmpty(ds))
            {
                // Set sort directive from ViewState
                if (ds is DataTable)
                {
                    try
                    {
                        ((DataTable)(ds)).DefaultView.Sort = SortDirect;
                    }
                    catch
                    {
                    }
                }
                else if (ds is DataSet)
                {
                    try
                    {
                        ((DataSet)(ds)).Tables[0].DefaultView.Sort = SortDirect;
                        ds = ((DataSet)(ds)).Tables[0].DefaultView;
                    }
                    catch
                    {
                    }
                }
                else if (ds is DataView)
                {
                    try
                    {
                        ((DataView)(ds)).Sort = SortDirect;
                    }
                    catch
                    {
                    }
                }
            }
        }
    }


    /// <summary>
    /// Changes sorting direction by specified column.
    /// </summary>
    /// <param name="orderByColumn">Column name to order by</param>
    /// <param name="orderByString">Old order by string</param> 
    private void ChangeSortDirection(string orderByColumn, string orderByString)
    {
        orderByColumn = orderByColumn.Trim().TrimStart('[').TrimEnd(']').Trim();
        orderByString = orderByString.Trim().TrimStart('[');

        // If order by column is long text use CAST in ORDER BY part of query
        if (TextColumns.Contains(orderByColumn))
        {
            if (orderByString.EndsWithCSafe("desc"))
            {
                SortDirect = String.Format("CAST([{0}] AS nvarchar(32)) asc", orderByColumn);
            }
            else
            {
                SortDirect = String.Format("CAST([{0}] AS nvarchar(32)) desc", orderByColumn);
            }
        }
        else
        {
            string orderByDirection = "asc";
            Match orderByMatch = OrderByRegex.Match(orderByString);
            if (orderByMatch.Success)
            {
                if (orderByMatch.Groups[2].Success)
                {
                    orderByDirection = orderByMatch.Groups[2].Value;
                }
            }

            // Sort by the same column -> the other direction
            if (orderByString.StartsWithCSafe(orderByColumn))
            {
                SortDirect = (orderByDirection.EqualsCSafe("desc", StringComparison.InvariantCultureIgnoreCase)) ? String.Format("[{0}] asc", orderByColumn) : String.Format("[{0}] desc", orderByColumn);
            }
            // Sort by a new column -> implicitly direction is ASC
            else
            {
                SortDirect = String.Format("[{0}] asc", orderByColumn);
            }
        }
    }


    /// <summary>
    /// Returns array list from hidden field.
    /// </summary>
    /// <param name="field">Hidden field with values separated with |</param>
    /// <param name="hashField">Hidden field containing hash of <paramref name="field"/></param>
    private static List<string> GetHiddenValues(HiddenField field, HiddenField hashField)
    {
        var result = new List<string>();

        if (ValidateHiddenValues(field, hashField))
        {
            result.AddRange(field.Value.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries).Distinct(StringComparer.InvariantCultureIgnoreCase).ToList());
        }

        return result;
    }


    /// <summary>
    /// Validates values in hidden fields with corresponding hash.
    /// </summary>
    /// <param name="field">Hidden field with values separated with |</param>
    /// <param name="hashField">Hidden field containing hash of <paramref name="field"/></param>
    private static bool ValidateHiddenValues(HiddenField field, HiddenField hashField)
    {
        return ValidationHelper.ValidateHash(field.Value, hashField.Value, redirect: false);
    }


    /// <summary>
    /// Sets values into hidden field.
    /// </summary>
    /// <param name="values">Values to set</param>
    /// <param name="actionsField">Hidden field</param>
    /// <param name="hashField">Hash field</param>
    private static void SetHiddenValues(IEnumerable<string> values, HiddenField actionsField, HiddenField hashField)
    {
        if (values != null)
        {
            if (actionsField != null)
            {
                // Build the list of actions
                StringBuilder sb = new StringBuilder();
                sb.Append("|");

                foreach (string value in values)
                {
                    sb.Append(value);
                    sb.Append("|");
                }

                // Action IDs
                string actions = sb.ToString();
                actionsField.Value = actions;

                // Actions hash
                if (hashField != null)
                {
                    hashField.Value = ValidationHelper.GetHashString(actions);
                }
            }
        }
    }


    /// <summary>
    /// Clears all selected items from hidden values.
    /// </summary>
    /// <param name="field">Hidden field</param>
    private static void ClearHiddenValues(HiddenField field)
    {
        if (field != null)
        {
            field.Value = String.Empty;
        }
    }


    /// <summary>
    /// Sets hidden field with actions hashes.
    /// </summary>
    private void SetActionsHash()
    {
        if (ActionsID.Count > 0)
        {
            SetHiddenValues(ActionsID, hidActions, hidActionsHash);
        }
    }


    /// <summary>
    /// Sets pager control.
    /// </summary>
    private void SetPager()
    {
        Pager.PagedControl = this;
    }


    /// <summary>
    /// Sets the sort direction if current request is sorting.
    /// </summary>
    private void ProcessSorting()
    {
        // Get current event target
        string uniqieId = ValidationHelper.GetString(Request.Params[Page.postEventSourceID], String.Empty);

        // Get current argument
        string eventargument = ValidationHelper.GetString(Request.Params[Page.postEventArgumentID], String.Empty);

        if ((uniqieId == GridView.UniqueID) && (eventargument.StartsWithCSafe("Sort")))
        {
            string orderByColumn = Convert.ToString(eventargument.Split('$')[1]);
            if (SortColumns.Contains(orderByColumn.ToLowerCSafe()))
            {
                // If sorting is called for the first time and default sorting (OrderBy property) is set
                if ((SortDirect == "") && !string.IsNullOrEmpty(OrderBy))
                {
                    ChangeSortDirection(orderByColumn, OrderBy);
                }
                else
                {
                    ChangeSortDirection(orderByColumn, SortDirect);
                }
            }
        }
    }


    /// <summary>
    /// Returns true if current request was fired by page change or filter show button.
    /// </summary>
    private bool IsInternalPostBack()
    {
        if (RequestHelper.IsPostBack())
        {
            // Get current event target
            string uniqueId = ValidationHelper.GetString(Request.Params[Page.postEventSourceID], String.Empty);

            // Get current argument
            string eventargument = ValidationHelper.GetString(Request.Params[Page.postEventArgumentID], String.Empty);

            // Check whether current request is paging
            if (!String.IsNullOrEmpty(uniqueId) && (uniqueId == GridView.UniqueID) && eventargument.StartsWithCSafe("page", true))
            {
                return true;
            }

            // Check whether show button is defined and it caused request
            if ((mShowButton != null) && (!string.IsNullOrEmpty(Request.Params[mShowButton.UniqueID])))
            {
                return true;
            }

            // Check whether show button in basic form is defined
            if ((FilterForm.SubmitButton != null) && (!string.IsNullOrEmpty(Request.Params[FilterForm.SubmitButton.UniqueID])))
            {
                return true;
            }

            // Check whether pager caused request
            if (ControlsHelper.CausedPostBack(Pager.UniPager))
            {
                return true;
            }

            // Check whether current event was caused by a part of dynamic filter
            if (ControlsHelper.ChildCausedPostBack(FilterPlaceHolder))
            {
                return true;
            }
        }

        // Non-paging request by default
        return false;
    }


    /// <summary>
    /// Initializes reload for toggle advanced mode button for custom filters.
    /// </summary>
    private void InitializeCustomFilters()
    {
        if (CustomFilters != null)
        {
            foreach (CMSAbstractBaseFilterControl filter in CustomFilters)
            {
                if (filter.ToggleAdvancedModeButton != null)
                {
                    filter.ToggleAdvancedModeButton.Click += (o, args) => ReloadData();
                }
            }
        }
    }


    /// <summary>
    /// Returns icon file for current theme or from default if current doesn't exist.
    /// </summary>
    /// <param name="iconfile">Icon file name</param>
    private string GetActionImage(string iconfile)
    {
        if (File.Exists(MapPath(ImageDirectoryPath + iconfile)))
        {
            return (ImageDirectoryPath + iconfile);
        }

        // Short path to the icon
        if (ControlsExtensions.RenderShortIDs)
        {
            return UIHelper.GetShortImageUrl(UIHelper.UNIGRID_ICONS, iconfile);
        }

        return GetImageUrl("Design/Controls/UniGrid/Actions/" + iconfile);
    }


    /// <summary>
    /// Register UniGrid commands scripts.
    /// </summary>
    private void RegisterCmdScripts()
    {
        StringBuilder builder = new StringBuilder();

        // Redir function
        if (EditActionUrl != null)
        {
            builder.Append("function UG_Redir(url) { document.location.replace(url); return false; }\n");
        }

        builder.Append("function ", RELOAD_PREFIX, ClientID, "() { ", Page.ClientScript.GetPostBackEventReference(this, "Reload"), " }\n");
        builder.Append("function UG_Reload() { ", RELOAD_PREFIX, ClientID, "(); }\n");

        // Actions
        builder.Append(
            @"
function Get(id) {
    return document.getElementById(id);
}

function ", CMD_PREFIX, ClientID, @"(name, arg) {
    var nameObj = Get('", hidCmdName.ClientID, @"');
    var argObj = Get('", hidCmdArg.ClientID, @"');
    if ((nameObj != null) && (argObj != null)) {
        nameObj.value = name;
        argObj.value = arg;
        ", Page.ClientScript.GetPostBackEventReference(this, "UniGridAction"), @"
    } 
    
    return false;
}

function ", DESTROY_OBJECT_PREFIX, ClientID, @"(arg) {"
            , CMD_PREFIX, ClientID, @"('#destroyobject',arg);
}");


        if (showSelection)
        {
            // Selection hash setter
            builder.Append(
"function ", SET_HASH_PREFIX, ClientID, @"(hash) {
    var hashElem = Get('", hidSelectionHash.ClientID, @"');

    if (hashElem) {
        hashElem.value = hash;
    }
}");

            // Selection - click
            builder.Append(
"function ", SELECT_PREFIX, ClientID, @"(checkBox, arg, checkedItemHash) {
    if (checkBox) {
        var sel = Get('", GetSelectionFieldClientID(), @"');
        if (sel) {
            if (sel.value == '') {
                sel.value = '|';
            }
    
            if (checkBox.checked) {
                sel.value += arg + '|';    
            }
            else {
                sel.value = sel.value.replace('|' + arg + '|', '|');
            }

            if (!window.hasOwnProperty('" + GetSelectionHashFieldFlagName() + @"')) {
                " + GetSelectionHashFieldFlagName() + @" = true;
            }

            if (" + GetSelectionHashFieldFlagName() + @") {
                " + Page.ClientScript.GetCallbackEventReference(this, "sel.value + '$' + arg + '#' + checkedItemHash", SET_HASH_PREFIX + ClientID, null) + @";
            } 
            else if (window.hasOwnProperty('" + GetSelectionTemporaryStorageName() + @"')){
                " + GetSelectionTemporaryStorageName() + @"[arg] = checkedItemHash;
            }
        }
    }
}");

            // Selection - select all
            builder.Append(
"function ", SELECT_ALL_PREFIX, ClientID, @"(selAllChkBox) {
	var inputs = document.getElementById('", pnlContent.ClientID, @"').getElementsByTagName('input'),
        sel = Get('", GetSelectionFieldClientID(), @"');

    " + GetSelectionHashFieldFlagName() + @" = false;
    " + GetSelectionTemporaryStorageName() + @" = {};

	for(i=0; i<inputs.length; i++) {
		if(inputs[i].type == 'checkbox') {
			if((inputs[i].id != selAllChkBox.id) && (selAllChkBox.checked != inputs[i].checked)) {
				inputs[i].click();
			}
		}
	}

    " + GetSelectionHashFieldFlagName() + @" = true;
    
    if (sel)
    {
        var callBackArgument = sel.value + '$';
        
        for(var index in " + GetSelectionTemporaryStorageName() + @") {
            callBackArgument += index + '#' + " + GetSelectionTemporaryStorageName() + @"[index] + '$';
        }

        " + Page.ClientScript.GetCallbackEventReference(this, "callBackArgument", SET_HASH_PREFIX + ClientID, null) + @"
    }
}");

            // Selection - clear
            builder.Append(
"function ", CLEAR_SELECTION_PREFIX, ClientID, @"() {
    var inputs = document.getElementById('", pnlContent.ClientID, @"').getElementsByTagName('input');
    if (inputs != null) {
        for (var i = 0; i< inputs.length; i++) {
            if (inputs[i].type.toLowerCase() == 'checkbox') {
                inputs[i].checked = false;
            }
        }
    }

    var sel = Get('", GetSelectionFieldClientID(), @"');
    if (sel) {
        sel.value = '';
    }
    " + SET_HASH_PREFIX, ClientID + @"('');
    " + Page.ClientScript.GetPostBackEventReference(this, "ClearOriginallySelectedItems") + @";
}");

            // Selection - IsSelectionEmpty
            builder.Append(
"function ", CHECK_SELECTION_PREFIX, ClientID, @"() {
    var sel = Get('", GetSelectionFieldClientID(), @"');

    return ((!sel) || (sel.value == '') || (sel.value == '|'));
}
");

            if (mResetSelection)
            {
                builder.Append("if (", CLEAR_SELECTION_PREFIX, ClientID, ") { ", CLEAR_SELECTION_PREFIX, ClientID, "(); }");
            }
        }

        ScriptHelper.RegisterStartupScript(this, typeof(string), "UniGrid_" + ClientID, ScriptHelper.GetScript(builder.ToString()));
    }


    /// <summary>
    /// Sets basic form filter.
    /// </summary>
    private void SetBasicFormFilter()
    {
        // Get form info
        var fi = FormHelper.GetFormInfo(FilterFormName, true, false, true);
        if (fi != null)
        {
            var filter = FilterForm;

            filter.OnAfterSave += BasicForm_OnAfterSave;

            // Set form info
            filter.FormInformation = fi;

            // Set filter button
            filter.SubmitButton.ID = "btnShow";
            filter.SubmitButton.ResourceString = "general.search";
            filter.SubmitButton.ButtonStyle = ButtonStyle.Primary;
            filter.SubmitButton.RegisterHeaderAction = false;

            // Set custom layout
            var afi = AlternativeFormInfoProvider.GetAlternativeFormInfo(FilterFormName);
            filter.AltFormInformation = afi;

            filter.CheckFieldEmptiness = false;
            filter.EnsureMessagesPlaceholder(MessagesPlaceHolder);
            filter.LoadData(FilterFormData);
        }
        else
        {
            // Hide filter
            FilterForm.StopProcessing = true;
            FilterForm.Visible = false;
        }
    }


    /// <summary>
    /// Handles OnAfterSave event of basic form.
    /// </summary>
    /// <param name="sender">Sender</param>
    /// <param name="e">Event argument</param>
    private void BasicForm_OnAfterSave(object sender, EventArgs e)
    {
        // Set where clause
        WhereClause = FilterForm.GetWhereCondition();
        filterForm.StopProcessing = true;

        // When the filter button is clicked, the filter is always set
        FilterIsSet = true;
        Pager.UniPager.CurrentPage = 1;
        ReloadData();
    }


    /// <summary>
    /// Checks whether user is authorized for specified action.
    /// </summary>
    /// <param name="actionName">Action name</param>    
    private void CheckActionAndRedirect(string actionName)
    {
        // Get the action
        Action action = GridActions.GetAction(actionName);

        if ((action != null) && (!string.IsNullOrEmpty(action.ModuleName)))
        {
            var user = MembershipContext.AuthenticatedUser;
            string siteName = SiteContext.CurrentSiteName;

            // Check module permissions
            if (!string.IsNullOrEmpty(action.Permissions) && !user.IsAuthorizedPerResource(action.ModuleName, action.Permissions, siteName))
            {
                RedirectToAccessDenied(action.ModuleName, action.Permissions);
            }

            // Check module UI elements
            if (!string.IsNullOrEmpty(action.UIElements) && !user.IsAuthorizedPerUIElement(action.ModuleName, action.UIElements.Split(';'), siteName))
            {
                RedirectToUIElementAccessDenied(action.ModuleName, action.UIElements);
            }
        }
    }


    /// <summary>
    /// Get name of flag that enables/disables hash recalculation for field containing selected items.
    /// </summary>
    private string GetSelectionHashFieldFlagName()
    {
        return "hashRecalculationEnabled_" + ClientID;
    }


    /// <summary>
    /// Get name of map that stores temporarily IDs and hashes of selected items in mass selections.
    /// </summary>
    private string GetSelectionTemporaryStorageName()
    {
        return "temporaryHashStorage_" + ClientID;
    }


    /// <summary>
    /// Validates filter field input and returns error message.
    /// </summary>
    /// <param name="filterField">UniGridFilterField which will be validated</param>
    private string ValidateFilterField(UniGridFilterField filterField)
    {
        string value = string.Empty;

        // Try to get value from filter textbox
        TextBox valueControl = filterField.ValueControl as TextBox;
        if (valueControl != null)
        {
            value = valueControl.Text;
        }

        // Validate value
        if (!string.IsNullOrEmpty(value))
        {
            switch (filterField.Type)
            {
                case UniGridFilterTypeEnum.Bool:
                    return ValidationHelper.IsBoolean(value) ? string.Empty : GetString("filter.validboolean");
                case UniGridFilterTypeEnum.Integer:
                    return ValidationHelper.IsInteger(value) ? string.Empty : GetString("filter.validintergernumber");
                case UniGridFilterTypeEnum.Double:
                    return ValidationHelper.IsDouble(value) ? string.Empty : GetString("filter.validdecimalnumber");
                default:
                    return string.Empty;
            }
        }
        return string.Empty;
    }

    #endregion


    #region "IUniPageable Members"

    /// <summary>
    /// Pager data item.
    /// </summary>
    public object PagerDataItem
    {
        get
        {
            return GridView.DataSource;
        }
        set
        {
            GridView.DataSource = value;
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

    #endregion


    #region "ICallBackEventHandler Members"

    /// <summary>
    /// Sets callback event arguments.
    /// </summary>
    /// <param name="eventArgument">Arguments of raised event.</param>
    public void RaiseCallbackEvent(string eventArgument)
    {
        callbackArguments = eventArgument;
    }


    /// <summary>
    /// Returns callback result.
    /// </summary>
    public string GetCallbackResult()
    {
        if (!string.IsNullOrEmpty(callbackArguments))
        {
            // Get argument fractions
            string[] fractions = callbackArguments.Split(new[] { '$' }, StringSplitOptions.RemoveEmptyEntries).ToArray();

            if (fractions.Length >= 2)
            {
                // Validate all selected/deselected items
                bool isValid = fractions.Skip(1).Select(f => f.Split(new[] { '#' }, StringSplitOptions.RemoveEmptyEntries)).All(p => ValidationHelper.ValidateHash(p[0], p[1], redirect: false));

                if (isValid)
                {
                    return ValidationHelper.GetHashString(fractions[0]);
                }
            }
        }

        return string.Empty;
    }

    #endregion
}