using System;
using System.Data;
using System.Web.UI.WebControls;

using CMS.Controls;
using CMS.DataEngine;
using CMS.DocumentEngine;
using CMS.Ecommerce;
using CMS.ExtendedControls;
using CMS.Helpers;
using CMS.MacroEngine;
using CMS.Membership;
using CMS.Base;
using CMS.SiteProvider;
using CMS.UIControls;
using TreeNode = CMS.DocumentEngine.TreeNode;

public partial class CMSModules_Ecommerce_Controls_UI_ProductFilter : CMSAbstractDataFilterControl
{
    #region "Variables"

    private TreeNode mParentNode;
    private CMSUserControl filteredControl;
    private string mFilterMode;
    private bool allowGlobalProducts;

    #endregion


    #region "Properties"

    /// <summary>
    /// Current filter mode.
    /// </summary>
    public override string FilterMode
    {
        get
        {
            if (mFilterMode == null)
            {
                mFilterMode = ValidationHelper.GetString(filteredControl.GetValue("FilterMode"), "").ToLowerCSafe();
            }
            return mFilterMode;
        }
        set
        {
            mFilterMode = value;
        }
    }


    /// <summary>
    /// Where condition.
    /// </summary>
    public override string WhereCondition
    {
        get
        {
            base.WhereCondition = GenerateWhereCondition();
            return base.WhereCondition;
        }
        set
        {
            base.WhereCondition = value;
        }
    }


    private TreeNode ParentNode
    {
        get
        {
            return mParentNode ?? (mParentNode = UIContext.EditedObject as TreeNode);
        }
    }


    /// <summary>
    /// Gets or sets a value indicating whether the advanced filter is displayed or not. 
    /// </summary>
    private bool IsAdvancedMode
    {
        get
        {
            return ValidationHelper.GetBoolean(ViewState["IsAdvancedMode"], false);
        }
        set
        {
            ViewState["IsAdvancedMode"] = value;
        }
    }


    /// <summary>
    /// Gets or sets a value indicating whether the advanced filter was already displayed. 
    /// </summary>
    private bool FirstTimeAdvancedFilterDisplayed
    {
        get
        {
            return ValidationHelper.GetBoolean(ViewState["FirstTimeAdvancedFilterDisplayed"], true);
        }
        set
        {
            ViewState["FirstTimeAdvancedFilterDisplayed"] = value;
        }
    }


    #endregion


    #region "Page events"

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        filteredControl = FilteredControl as CMSUserControl;

        // Hide filter button, this filter has its own
        UniGrid grid = filteredControl as UniGrid;
        if (grid != null)
        {
            grid.HideFilterButton = true;
        }

        allowGlobalProducts = ECommerceSettings.AllowGlobalProducts(SiteContext.CurrentSiteName);

        // Display Global and site option if global products are allowed
        siteElem.ShowSiteAndGlobal = allowGlobalProducts;

        // Initialize controls
        if (!URLHelper.IsPostback())
        {
            FillThreeStateDDL(ddlNeedsShipping);
            FillThreeStateDDL(ddlAllowForSale);

            FillDocumentTypesDDL();
        }
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        // General UI
        lnkShowAdvancedFilter.Text = GetString("general.displayadvancedfilter");
        lnkShowSimpleFilter.Text = GetString("general.displaysimplefilter");

        btnFilter.Text = GetString("general.search");
        btnFilter.Click += btnFilter_Click;

        btnReset.Text = GetString("general.reset");
        btnReset.Click += btnReset_Click;

        // Setup filter mode
        SetFieldsVisibility();

        // Fill department DropDownList
        if ((MembershipContext.AuthenticatedUser != null) && !MembershipContext.AuthenticatedUser.IsGlobalAdministrator)
        {
            departmentElem.UserID = MembershipContext.AuthenticatedUser.UserID;
        }

        // When global SKUs can be included in listing
        if (allowGlobalProducts)
        {
            // Display global departments, manufacturers, suppliers and global statuses too
            departmentElem.DisplayGlobalItems = true;
            publicStatusElem.DisplayGlobalItems = true;
            internalStatusElem.DisplayGlobalItems = true;
            supplierElem.DisplayGlobalItems = true;
            manufacturerElem.DisplayGlobalItems = true;
        }

        plcSite.Visible = allowGlobalProducts;
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Creates where condition according to values selected in filter.
    /// </summary>
    private string GenerateWhereCondition()
    {
        string tempCondition = "";
        string where = "";

        string productNameColumnName = (ParentNode != null) ? "DocumentName" : "SKUName";

        // Append name/number condition
        var nameOrNumber = txtNameOrNumber.Text.Trim().Truncate(txtNameOrNumber.MaxLength);
        if (!string.IsNullOrEmpty(nameOrNumber))
        {
            string safeNameOrNumber = SqlHelper.GetSafeQueryString(nameOrNumber, false);
            // condition to get also products with variants that contains 
            string variantCondition = string.Format("SKUID IN (SELECT SKUParentSKUID FROM COM_SKU WHERE SKUName LIKE N'%{0}%' OR SKUNumber LIKE N'%{0}%')", safeNameOrNumber);

            tempCondition = string.Format("({0} LIKE N'%{1}%' OR SKUNumber LIKE N'%{1}%')", productNameColumnName, safeNameOrNumber);
            tempCondition = SqlHelper.AddWhereCondition(tempCondition, variantCondition, "OR");

            where = SqlHelper.AddWhereCondition(where, tempCondition);
        }

        // Append site condition
        if (allowGlobalProducts && (siteElem.SiteID != UniSelector.US_GLOBAL_AND_SITE_RECORD))
        {
            // Restrict SKUSiteID only for products not for product section (full listing mode)
            int selectedSiteID = (siteElem.SiteID > 0) ? siteElem.SiteID : 0;
            where = SqlHelper.AddWhereCondition(where, "(SKUID IS NULL) OR (ISNULL(SKUSiteID, 0) = " + selectedSiteID + ")");
        }

        // Append department condition
        if (departmentElem.SelectedID > 0)
        {
            tempCondition = string.Format("SKUDepartmentID = {0}", departmentElem.SelectedID);

            where = SqlHelper.AddWhereCondition(where, tempCondition);
        }
        else if (departmentElem.SelectedID == -5)
        {
            where = SqlHelper.AddWhereCondition(where, "SKUDepartmentID IS NULL");
        }

        // Append one level condition
        if (ParentNode != null)
        {
            if (!chkShowAllChildren.Checked)
            {
                tempCondition = string.Format("NodeParentID = {0} AND NodeLevel = {1}", ParentNode.NodeID, ParentNode.NodeLevel + 1);
            }

            where = SqlHelper.AddWhereCondition(where, tempCondition);
        }

        // Handle advanced mode fields
        if (IsAdvancedMode)
        {
            // Append product type condition
            if ((selectProductTypeElem.Value != null) && (selectProductTypeElem.Value.ToString() != "ALL"))
            {
                tempCondition = string.Format("SKUProductType = N'{0}'", SqlHelper.GetSafeQueryString(selectProductTypeElem.Value.ToString()));
                where = SqlHelper.AddWhereCondition(where, tempCondition);
            }

            // Manufacturer value
            if (manufacturerElem.SelectedID != UniSelector.US_ALL_RECORDS)
            {
                where = SqlHelper.AddWhereCondition(where, "ISNULL(SKUManufacturerID, 0) = " + manufacturerElem.SelectedID);
            }

            // Supplier value
            if (supplierElem.SelectedID != UniSelector.US_ALL_RECORDS)
            {
                where = SqlHelper.AddWhereCondition(where, "ISNULL(SKUSupplierID, 0) = " + supplierElem.SelectedID);
            }

            // Internal status value
            if (internalStatusElem.SelectedID != UniSelector.US_ALL_RECORDS)
            {
                where = SqlHelper.AddWhereCondition(where, "ISNULL(SKUInternalStatusID, 0) = " + internalStatusElem.SelectedID);
            }

            // Store status value
            if (publicStatusElem.SelectedID != UniSelector.US_ALL_RECORDS)
            {
                where = SqlHelper.AddWhereCondition(where, "ISNULL(SKUPublicStatusID, 0) = " + publicStatusElem.SelectedID);
            }

            // Append needs shipping condition
            int needsShipping = ValidationHelper.GetInteger(ddlNeedsShipping.SelectedValue, -1);
            if (needsShipping >= 0)
            {
                where = SqlHelper.AddWhereCondition(where, "ISNULL(SKUNeedsShipping, 0) = " + needsShipping);
            }

            // Append allow for sale condition
            int allowForSale = ValidationHelper.GetInteger(ddlAllowForSale.SelectedValue, -1);
            if (allowForSale >= 0)
            {
                where = SqlHelper.AddWhereCondition(where, "SKUEnabled = " + allowForSale);
            }

            // When in document mode
            if (ParentNode != null)
            {
                int docTypeId = ValidationHelper.GetInteger(drpDocTypes.SelectedValue, 0);
                if (docTypeId > 0)
                {
                    // Append document type condition
                    where = SqlHelper.AddWhereCondition(where, "NodeClassID = " + docTypeId);
                }
            }
        }

        return where;
    }


    /// <summary>
    /// Applies filter to unigrid
    /// </summary>
    protected void btnFilter_Click(object sender, EventArgs e)
    {
        ApplyFilter(sender, e);
    }


    /// <summary>
    /// Applies filter to unigrid.
    /// </summary>
    protected void ApplyFilter(object sender, EventArgs e)
    {
        UniGrid grid = filteredControl as UniGrid;
        if (grid != null)
        {
            grid.ApplyFilter(sender, e);
        }
    }


    /// <summary>
    /// Resets the associated UniGrid control.
    /// </summary>
    protected void btnReset_Click(object sender, EventArgs e)
    {
        UniGrid grid = filteredControl as UniGrid;
        if (grid != null)
        {
            grid.Reset();
        }
    }


    /// <summary>
    /// Sets the advanced mode.
    /// </summary>
    protected void lnkShowAdvancedFilter_Click(object sender, EventArgs e)
    {
        IsAdvancedMode = true;
        SetFieldsVisibility();

        if (FirstTimeAdvancedFilterDisplayed)
        {
            // Default value of uniselector based controls is ensured in OnPreRender and it is too late to ensure correct filter condition.
            // It has to be set up manually.
            EnsureSelectedValuesForFilter();
            FirstTimeAdvancedFilterDisplayed = false;
        }

        ApplyFilter(sender, e);
    }


    /// <summary>
    /// Sets the simple mode.
    /// </summary>
    protected void lnkShowSimpleFilter_Click(object sender, EventArgs e)
    {
        IsAdvancedMode = false;
        SetFieldsVisibility();

        ApplyFilter(sender, e);
    }


    /// <summary>
    /// Shows/hides fields of filter according to simple/advanced mode.
    /// </summary>
    private void SetFieldsVisibility()
    {
        plcSimpleFilter.Visible = !IsAdvancedMode;
        plcAdvancedFilter.Visible = IsAdvancedMode;

        plcAdvancedFilterType.Visible = IsAdvancedMode;
        plcAdvancedFilterGeneral.Visible = IsAdvancedMode;

        bool documentMode = (ParentNode != null);
        plcSubsectionProducts.Visible = documentMode;
        plcAdvancedDocumentType.Visible = IsAdvancedMode && documentMode;
    }


    /// <summary>
    /// Fills items 'Yes', 'No' and 'All' to given drop down list.
    /// </summary>
    /// <param name="dropDown">Drop down list to be filled.</param>
    private void FillThreeStateDDL(CMSDropDownList dropDown)
    {
        dropDown.Items.Add(new ListItem(GetString("general.selectall"), "-1"));
        dropDown.Items.Add(new ListItem(GetString("general.yes"), "1"));
        dropDown.Items.Add(new ListItem(GetString("general.no"), "0"));
    }


    /// <summary>
    /// Fills dropdown list with document types.
    /// </summary>
    private void FillDocumentTypesDDL()
    {
        drpDocTypes.Items.Clear();

        // Add (All) record
        drpDocTypes.Items.Add(new ListItem(GetString("general.selectall"), "0"));

        // Select only document types from current site marked as product
        DataSet ds = DocumentTypeHelper.GetDocumentTypeClasses()
            .OnSite(SiteContext.CurrentSiteID)
            .WhereTrue("ClassIsProduct")
            .OrderBy("ClassDisplayName")
            .Columns("ClassID", "ClassDisplayName");

        if (!DataHelper.DataSourceIsEmpty(ds))
        {
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                string name = ValidationHelper.GetString(dr["ClassDisplayName"], "");
                int id = ValidationHelper.GetInteger(dr["ClassID"], 0);

                if (!string.IsNullOrEmpty(name) && (id > 0))
                {
                    // Handle document name
                    name = ResHelper.LocalizeString(MacroResolver.Resolve(name));

                    drpDocTypes.Items.Add(new ListItem(name, id.ToString()));
                }
            }
        }
    }


    /// <summary>
    /// Ensures selection of first item in uniselector based controls.
    /// </summary>
    private void EnsureSelectedValuesForFilter()
    {
        manufacturerElem.SelectedID = UniSelector.US_ALL_RECORDS;
        supplierElem.SelectedID = UniSelector.US_ALL_RECORDS;
        publicStatusElem.SelectedID = UniSelector.US_ALL_RECORDS;
        internalStatusElem.SelectedID = UniSelector.US_ALL_RECORDS;
    }

    #endregion


    #region "State management"

    /// <summary>
    /// Stores filter state to the specified object.
    /// </summary>
    /// <param name="state">The object that holds the filter state.</param>
    public override void StoreFilterState(FilterState state)
    {
        base.StoreFilterState(state);
        state.AddValue("AdvancedMode", IsAdvancedMode);
    }


    /// <summary>
    /// Restores filter state from the specified object.
    /// </summary>
    /// <param name="state">The object that holds the filter state.</param>
    public override void RestoreFilterState(FilterState state)
    {
        base.RestoreFilterState(state);
        IsAdvancedMode = state.GetBoolean("AdvancedMode");
        SetFieldsVisibility();
    }


    /// <summary>
    /// Resets filter to the default state.
    /// </summary>
    public override void ResetFilter()
    {
        txtNameOrNumber.Text = String.Empty;
        siteElem.SiteID = UniSelector.US_GLOBAL_AND_SITE_RECORD;
        departmentElem.Value = UniSelector.US_ALL_RECORDS;
        selectProductTypeElem.Value = "ALL";
        manufacturerElem.Value = UniSelector.US_ALL_RECORDS;
        supplierElem.Value = UniSelector.US_ALL_RECORDS;
        internalStatusElem.Value = UniSelector.US_ALL_RECORDS;
        publicStatusElem.Value = UniSelector.US_ALL_RECORDS;
        ddlNeedsShipping.SelectedIndex = 0;
        ddlAllowForSale.SelectedIndex = 0;
        drpDocTypes.SelectedIndex = 0;
        chkShowAllChildren.Checked = true;
    }

    #endregion
}