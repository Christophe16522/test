using System;

using CMS.CMSHelper;
using CMS.ExtendedControls;
using CMS.FormControls;
using CMS.GlobalHelper;
using CMS.SettingsProvider;
using CMS.SiteProvider;
using CMS.Helpers;

public partial class CMSModules_Content_FormControls_Documents_SelectPath : FormEngineUserControl
{
    #region "Variables"

    private bool mEnableSiteSelection = false;
    private DialogConfiguration mConfig = null;
    private string selectedSiteName = null;
    private bool siteNameIsAll = false;
    private int mSiteId = 0;

    #endregion


    #region "Private properties"

    /// <summary>
    /// Gets the configuration for Copy and Move dialog.
    /// </summary>
    private DialogConfiguration Config
    {
        get
        {
            if (mConfig == null)
            {
                mConfig = new DialogConfiguration();
                mConfig.HideLibraries = true;
                mConfig.HideAnchor = true;
                mConfig.HideAttachments = true;
                mConfig.HideContent = false;
                mConfig.HideEmail = true;
                mConfig.HideLibraries = true;
                mConfig.HideWeb = true;
                mConfig.EditorClientID = txtPath.ClientID;

                bool onlyCurrentSite = String.IsNullOrEmpty(selectedSiteName) || (selectedSiteName.ToLowerCSafe() == "##currentsite##");

                if (ControlsHelper.CheckControlContext(this, ControlContext.WIDGET_PROPERTIES)
                    && (!siteNameIsAll))
                {
                    // If used in a widget, site selection is provided by a site selector form control (using HasDependingField/DependsOnAnotherField principle)
                    // therefore the site selector drop-down list in the SelectPath dialog contains only a single site - preselected by the site selector form control
                    mConfig.ContentSites = onlyCurrentSite ? AvailableSitesEnum.OnlyCurrentSite : AvailableSitesEnum.OnlySingleSite;
                }
                else
                {
                    mConfig.ContentSites = AvailableSitesEnum.All;
                }

                mConfig.ContentSelectedSite = onlyCurrentSite ? SiteContext.CurrentSiteName : selectedSiteName;

                mConfig.OutputFormat = OutputFormatEnum.Custom;
                mConfig.CustomFormatCode = "selectpath";
                mConfig.SelectableContent = SelectableContentEnum.AllContent;
                if (SubItemsNotByDefault)
                {
                    mConfig.AdditionalQueryParameters = "SubItemsNotByDefault=1";
                }
            }
            return mConfig;
        }
    }

    #endregion


    #region "Public properties"
    
    /// <summary>
    /// Gets or sets the ID of the site from which the path is selected.
    /// </summary>
    public int SiteID
    {
        get
        {
            return mSiteId;
        }
        set
        {
            mSiteId = value;
            if (value > 0)
            {
                Config.ContentSites = AvailableSitesEnum.OnlySingleSite;
                SiteInfo si = SiteInfoProvider.GetSiteInfo(value);
                if (si != null)
                {
                    Config.ContentSelectedSite = si.SiteName;
                }
            }
        }
    }


    /// <summary>
    /// Gets or sets the enabled state of the control.
    /// </summary>
    public override bool Enabled
    {
        get
        {
            return base.Enabled;
        }
        set
        {
            base.Enabled = value;
            txtPath.Enabled = value;
            btnSelectPath.Enabled = value;
        }
    }


    /// <summary>
    /// Gets or sets field value.
    /// </summary>
    public override object Value
    {
        get
        {
            return txtPath.Text;
        }
        set
        {
            txtPath.Text = (string)value;
        }
    }


    /// <summary>
    /// Gets ClientID of the textbox with path.
    /// </summary>
    public override string ValueElementID
    {
        get
        {
            return txtPath.ClientID;
        }
    }


    /// <summary>
    /// Determines whether to enable site selection or not.
    /// </summary>
    public bool EnableSiteSelection
    {
        get
        {
            return mEnableSiteSelection;
        }
        set
        {
            mEnableSiteSelection = value;
            Config.ContentSites = (value ? AvailableSitesEnum.All : AvailableSitesEnum.OnlyCurrentSite);
        }
    }


    /// <summary>
    /// Indicates whether check box "Only sub items" is checked by default or not.
    /// </summary>
    public bool SubItemsNotByDefault
    {
        get;
        set;
    }

    #endregion


    protected void Page_Load(object sender, EventArgs e)
    {
        ScriptHelper.RegisterDialogScript(Page);
        SetFormSiteName();

        btnSelectPath.Text = GetString("general.select");
        btnSelectPath.OnClientClick = GetDialogScript();
    }


    protected void Page_PreRender(object sender, EventArgs e)
    {
        if (URLHelper.IsPostback()
            && DependsOnAnotherField)
        {
            if (siteNameIsAll)
            {
                // Refresh the dialog script if the site name was changed to "ALL" (this enables the site selection in the dialog window)
                btnSelectPath.OnClientClick = GetDialogScript();
            }

            pnlUpdate.Update();
        }
    }


    /// <summary>
    /// Gets the javascript which opens the dialog window.
    /// </summary>
    /// <returns>Javascript which opens the dialog window.</returns>
    private string GetDialogScript()
    {
        return "modalDialog('" + GetDialogUrl() + "','PathSelection', '90%', '85%'); return false;";
    }


    /// <summary>
    /// Returns Correct URL of the copy or move dialog.
    /// </summary>
    private string GetDialogUrl()
    {
        string url = CMSDialogHelper.GetDialogUrl(Config, IsLiveSite, false, null, false);
        return url;
    }


    /// <summary>
    /// Sets the site name if the SiteName field is available in the form.
    /// The outcome of this method is used for the configuration of the "Config" property
    /// </summary>
    private void SetFormSiteName()
    {
        if (DependsOnAnotherField
            && (Form != null)
            && Form.IsFieldAvailable("SiteName"))
        {
            string siteName = ValidationHelper.GetString(Form.GetFieldValue("SiteName"), "");

            if (siteName.EqualsCSafe(string.Empty, true) || siteName.EqualsCSafe("##all##", true))
            {
                selectedSiteName = string.Empty;
                siteNameIsAll = true;
                return;
            }

            if (!String.IsNullOrEmpty(siteName))
            {
                selectedSiteName = siteName;
                return;
            }
        }

        selectedSiteName = null;
    }
}