using System;
using System.Text;

using CMS.ExtendedControls;
using CMS.Helpers;
using CMS.DataEngine;
using CMS.UIControls;
using CMS.Base;
using CMS.Modules;

public partial class CMSModules_Modules_Controls_Settings_Category_SettingsCategoryEdit : CMSAdminEditControl
{
    #region "Private Members"

    private SettingsCategoryInfo mSettingsCategoryObj = null;
    private int mCategoryOrder = 0;
    private int mSettingsCategoryId = 0;
    private int mRootCategoryId = 0;
    private bool mIncludeRootCategory = false;
    private bool mIsGroupEdit = false;
    private bool mEnabled = true;
    private string mTreeRefreshUrl = null;
    private string mContentRefreshUrl = null;
    private bool mShowParentSelector = true;

    #endregion


    #region "Properties"

    /// <summary>
    /// Indicates selected category
    /// </summary>
    private int ModuleID
    {
        get
        {
            return QueryHelper.GetInteger("moduleid", 0);
        }
    }


    /// <summary>
    /// Gets or sets Category order. Specifies order of the category.
    /// </summary>
    public int CategoryOrder
    {
        get
        {
            return mCategoryOrder;
        }
        set
        {
            mCategoryOrder = value;
        }
    }


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
    /// Gets current selected parent category or RootCategoryID for new category.
    /// </summary>
    public int SelectedParentCategory
    {
        get
        {
            return (SettingsCategoryObj != null) ? drpCategory.SelectedCategory : RootCategoryID;
        }
    }


    /// <summary>
    /// Gets or sets enabled state of inclusion of RootCategory. Default false.
    /// </summary>
    public bool IncludeRootCategory
    {
        get
        {
            if (SettingsCategoryObj == null)
            {
                return true;
            }
            return mIncludeRootCategory;
        }
        set
        {
            mIncludeRootCategory = value;
        }
    }


    /// <summary>
    /// Gets or sets SettingsCategoryID. Specifies Id of SettingsCategory object.
    /// </summary>
    public int SettingsCategoryID
    {
        get
        {
            return mSettingsCategoryId;
        }
        set
        {
            mSettingsCategoryId = value;
            drpCategory.CurrentCategoryId = value;
            mSettingsCategoryObj = null;
        }
    }


    /// <summary>
    /// Gets or sets SettingsCategory object. Specifies SettingsCategory object which should be edited.
    /// </summary>
    public SettingsCategoryInfo SettingsCategoryObj
    {
        get
        {
            return mSettingsCategoryObj ?? (mSettingsCategoryObj = (mSettingsCategoryId > 0) ? SettingsCategoryInfoProvider.GetSettingsCategoryInfo(mSettingsCategoryId) : null);
        }
        set
        {
            mSettingsCategoryObj = value;
            if (value != null)
            {
                mSettingsCategoryId = value.CategoryID;
                mCategoryOrder = value.CategoryOrder;
            }
            else
            {
                mSettingsCategoryId = 0;
                mCategoryOrder = 0;
            }
        }
    }


    /// <summary>
    /// Gets or sets visible state of parent category selector.
    /// </summary>
    public bool ShowParentSelector
    {
        get
        {
            return mShowParentSelector;
        }
        set
        {
            mShowParentSelector = value;
        }
    }


    /// <summary>
    /// Gets or sets IsGroupEdit property. If set to <c>true</c> parent category selector will be hidden.
    /// </summary>
    public bool IsGroupEdit
    {
        get
        {
            return mIsGroupEdit;
        }
        set
        {
            mIsGroupEdit = value;
        }
    }


    /// <summary>
    /// Gets or sets Enabled property. If set to <c>false</c> all child control are disabled.
    /// </summary>
    public bool Enabled
    {
        get
        {
            return mEnabled;
        }
        set
        {
            mEnabled = value;
        }
    }


    /// <summary>
    /// Gets or sets DisplayOnlyCategories property of SelectSettingsCategory drop down list. If set to false, groups wil be included.
    /// </summary>
    public bool DisplayOnlyCategories
    {
        get
        {
            return drpCategory.DisplayOnlyCategories;
        }
        set
        {
            drpCategory.DisplayOnlyCategories = value;
        }
    }


    /// <summary>
    /// Url for refreshing tree with settings.
    /// </summary>
    public string TreeRefreshUrl
    {
        get
        {
            return mTreeRefreshUrl;
        }
        set
        {
            mTreeRefreshUrl = value;
        }
    }


    public string ContentRefreshUrl
    {
        get
        {
            return mContentRefreshUrl;
        }
        set
        {
            mContentRefreshUrl = value;
        }
    }

    #endregion


    #region "Page Events"

    protected void Page_Load(object sender, EventArgs e)
    {
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

        // Set edited object
        EditedObject = (mSettingsCategoryId > 0) ? SettingsCategoryObj : new SettingsCategoryInfo();
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        // Set enability
        txtCategoryDisplayName.Enabled =
            rfvCategoryDisplayName.Enabled =
            txtCategoryName.Enabled =
            rfvCategoryName.Enabled =
            txtIconPath.Enabled =
            drpCategory.Enabled =
            btnOk.Enabled = mEnabled;

        // Set css for the button
        btnOk.ButtonStyle = ButtonStyle.Primary;
        btnOk.Enabled = mEnabled;

        // Display parent category selector
        trParentCategory.Visible = trParentCategory.Visible && mShowParentSelector;
        trIconPath.Visible = !mIsGroupEdit;
    }

    #endregion


    #region "Private Methods"

    /// <summary>
    /// Validates the form. If validation succeeds returns true, otherwise returns false.
    /// </summary>
    private bool IsValid()
    {
        // Validate required fields
        string errMsg = new Validator().NotEmpty(txtCategoryName.Text.Trim(), GetString("General.RequiresCodeName"))
            .NotEmpty(txtCategoryDisplayName.Text.Trim(), GetString("General.RequiresDisplayName"))
            .Result;

        if (!ValidationHelper.IsCodeName(txtCategoryName.Text.Trim()))
        {
            errMsg = GetString("General.ErrorCodeNameInIdentifierFormat");
        }

        // Set up error message
        if (!string.IsNullOrEmpty(errMsg))
        {
            ShowError(errMsg);

            return false;
        }

        return true;
    }


    /// <summary>
    /// Initialization of controls.
    /// </summary>
    private void InitControls()
    {
        // Init validators
        rfvCategoryDisplayName.ErrorMessage = ResHelper.GetString("general.requiresdisplayname");
        rfvCategoryName.ErrorMessage = ResHelper.GetString("general.requirescodename");

        // Disable edit for category which is not for selected module
        if (SettingsCategoryObj != null)
        {
            ResourceInfo resource = ResourceInfoProvider.GetResourceInfo(ModuleID);
            if ((((resource != null) && !resource.ResourceIsInDevelopment) || (SettingsCategoryObj.CategoryResourceID != ModuleID)) && !SystemContext.DevelopmentMode)
            {
                Enabled = false;
                HeaderActions.Enabled = false;
            }
        }

        if (SystemContext.DevelopmentMode)
        {
            ucSelectModule.Value = (SettingsCategoryObj == null) ? ModuleID : (!URLHelper.IsPostback()) ? SettingsCategoryObj.CategoryResourceID : ucSelectModule.Value;
        }

        // Set the root category
        if (RootCategoryID > 0)
        {
            drpCategory.RootCategoryId = RootCategoryID;
            drpCategory.IncludeRootCategory = IncludeRootCategory;
        }
    }


    /// <summary>
    /// Loads the data into the form.
    /// </summary>
    private void LoadData()
    {
        // Load the form from the Info object
        if (SettingsCategoryObj != null)
        {
            txtCategoryName.Text = SettingsCategoryObj.CategoryName;
            txtCategoryDisplayName.Text = SettingsCategoryObj.CategoryDisplayName;
            txtIconPath.Text = SettingsCategoryObj.CategoryIconPath;
            if (SettingsCategoryObj.CategoryParentID > 0)
            {
                drpCategory.SelectedCategory = SettingsCategoryObj.CategoryParentID;
            }
        }
        else
        {
            trParentCategory.Visible = false;
        }
    }

    #endregion


    #region "Event Handlers"

    /// <summary>
    /// Handles OnClick event of btnOk.
    /// </summary>
    /// <param name="sender">Asp Button instance</param>
    /// <param name="e">EventArgs instance</param>
    protected void btnOk_Click(object sender, EventArgs e)
    {
        if (IsValid())
        {
            // Get category by name
            SettingsCategoryInfo categoryObj = SettingsCategoryInfoProvider.GetSettingsCategoryInfoByName(txtCategoryName.Text.Trim());
            // If name is unique OR ids are same
            if ((categoryObj == null) || (categoryObj.CategoryID == mSettingsCategoryId))
            {
                SettingsCategoryInfo sci = SettingsCategoryObj;
                if (sci == null)
                {
                    sci = new SettingsCategoryInfo();
                    sci.CategoryOrder = mCategoryOrder;
                }

                if (sci.CategoryParentID != drpCategory.SelectedCategory)
                {
                    // When parent has been changed set the order for the category as the last possible order
                    sci.CategoryOrder = SettingsCategoryInfoProvider.GetLastSettingsCategoryOrder(drpCategory.SelectedCategory) + 1;
                }

                sci.CategoryName = txtCategoryName.Text.Trim();
                sci.CategoryDisplayName = txtCategoryDisplayName.Text.Trim();
                sci.CategoryIconPath = txtIconPath.Text.Trim();
                sci.CategoryParentID = SelectedParentCategory;
                sci.CategoryIsGroup = mIsGroupEdit;
                sci.CategoryResourceID = ValidationHelper.GetInteger(ucSelectModule.Value, ModuleID);

                SettingsCategoryInfoProvider.SetSettingsCategoryInfo(sci);
                SettingsCategoryObj = sci;
                RaiseOnSaved();

                // Set the info message
                if (ContentRefreshUrl == null)
                {
                    ShowChangesSaved();
                }

                // Reload header and content after save
                int categoryIdToShow = sci.CategoryIsGroup ? sci.CategoryParentID : sci.CategoryID;

                StringBuilder sb = new StringBuilder();
                sb.Append("if (window.parent != null) {");
                if (TreeRefreshUrl != null)
                {
                    sb.Append("if (window.parent.parent.frames['settingstree'] != null) {");
                    sb.Append("window.parent.parent.frames['settingstree'].location = '" + ResolveUrl(TreeRefreshUrl) + "&categoryid=" + categoryIdToShow + "';");
                    sb.Append("}");
                    sb.Append("if (window.parent.frames['settingstree'] != null) {");
                    sb.Append("window.parent.frames['settingstree'].location =  '" + ResolveUrl(TreeRefreshUrl) + "&categoryid=" + categoryIdToShow + "';");
                    sb.Append("}");
                }
                if (ContentRefreshUrl != null)
                {
                    sb.Append("window.location =  '" + ResolveUrl(ContentRefreshUrl) + "&categoryid=" + sci.CategoryID + "';");
                }

                sb.Append("}");
                ltlScript.Text = ScriptHelper.GetScript(sb.ToString());
            }
            else
            {
                ShowError(GetString("general.codenameexists"));
            }
        }
    }

    #endregion
}