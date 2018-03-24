using System;
using System.Text;

using CMS.ExtendedControls;
using CMS.Helpers;
using CMS.Localization;
using CMS.Membership;
using CMS.PortalEngine;
using CMS.Base;
using CMS.SiteProvider;
using CMS.UIControls;

public partial class CMSModules_Content_Controls_LanguageMenu : CMSUserControl
{
    #region "Variables"

    protected string mSelectedCulture = null;

    #endregion


    #region "Properties"

    /// <summary>
    /// Culture to be considered as selected.
    /// </summary>
    public string SelectedCulture
    {
        get
        {
            string culture = ValidationHelper.GetString(GetValue("SelectedCulture"), null);
            if (String.IsNullOrEmpty(culture))
            {
                culture = ValidationHelper.GetString(UIContext[UIContextDataItemName.SELECTEDCULTURE], MembershipContext.AuthenticatedUser.PreferredCultureCode);
            }
            return culture;
        }
        set
        {
            SetValue("SelectedCulture", value);
        }
    }


    /// <summary>
    /// If false, compare separator and button is not shown.
    /// </summary>
    public bool DisplayCompare
    {
        get
        {
            return btnCompare.Visible;
        }
        set
        {
            btnCompare.Visible = value;
        }
    }


    /// <summary>
    /// ID of site, cultures belong to.
    /// </summary>
    public int SiteID
    {
        get;
        set;
    }


    /// <summary>
    /// Indicates if the language menu opens the options in up direction.
    /// </summary>
    public bool MenuDirectionUp
    {
        get;
        set;
    }

    #endregion


    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        ScriptHelper.RegisterJQuery(Page);

        string currentSiteName = (SiteID != 0) ? SiteInfoProvider.GetSiteName(SiteID) : SiteContext.CurrentSiteName;
        var cultures = CultureSiteInfoProvider.GetSiteCultures(currentSiteName).Items;

        if (cultures.Count > 1)
        {
            string defaultCulture = CultureHelper.GetDefaultCultureCode(currentSiteName);
            CultureInfo ci = CultureInfoProvider.GetCultureInfo(SelectedCulture);

            imgLanguage.ImageUrl = GetFlagIconUrl(SelectedCulture, "16x16");
            imgLanguage.AlternateText = imgLanguage.ToolTip = GetString(ci.CultureName);
            lblLanguageName.Text = ci.CultureShortName;

            // Generate sub-menu only if more cultures to choose from
            StringBuilder sb = new StringBuilder();
            foreach (var culture in cultures)
            {
                string cultureCode = culture.CultureCode;
                string cultureName = culture.CultureName;

                if (CMSString.Compare(cultureCode, defaultCulture, true) == 0)
                {
                    cultureName += " " + GetString("general.defaultchoice");
                }

                string flagUrl = GetFlagIconUrl(cultureCode, "16x16");

                var click = String.Format("ChangeLanguage({0}); return false;", ScriptHelper.GetString(cultureCode));

                sb.AppendFormat("<li><a href=\"#\" onclick=\"{0}\"><img src=\"{1}\" alt=\"\" class=\"language-flag\"><span class=\"language-name\">{2}</span></a></li>", click, flagUrl, cultureName);
            }

            ltlLanguages.Text = sb.ToString();

            // Split view button
            if (DisplayCompare)
            {
                btnCompare.ToolTip = GetString("SplitMode.CompareLangVersions");
                btnCompare.Text = GetString("SplitMode.Compare");
                if (UIContext.DisplaySplitMode)
                {
                    btnCompare.AddCssClass("active");
                }
                else
                {
                    btnCompare.RemoveCssClass("active");
                }
            }
        }
        else
        {
            // Hide language menu for one assigned culture on site
            Visible = false;
        }
    }

    #endregion
}