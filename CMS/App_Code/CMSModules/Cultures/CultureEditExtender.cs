using System;

using CMS;
using CMS.Base;
using CMS.ExtendedControls;
using CMS.FormControls;
using CMS.Helpers;
using CMS.Localization;

[assembly: RegisterCustomClass("CultureEditExtender", typeof(CultureEditExtender))]

/// <summary>
/// Culture uiform extender
/// </summary>
public class CultureEditExtender : ControlExtender<UIForm>
{
    /// <summary>
    /// Init event of the UI form.
    /// </summary>
    public override void OnInit()
    {
        Control.OnAfterValidate += OnAfterValidate;
    }


    /// <summary>
    /// Handles OnAfterValidate event of the UI form.
    /// </summary>
    /// <param name="sender">Sender object</param>
    /// <param name="e">Event argument</param>
    protected void OnAfterValidate(object sender, EventArgs e)
    {
        int cultureId = Control.EditedObject.Generalized.ObjectID;
        string cultureCode = ValidationHelper.GetString(Control.GetFieldValue("CultureCode"), String.Empty).Trim();
        string cultureAlias = ValidationHelper.GetString(Control.GetFieldValue("CultureAlias"), String.Empty).Trim();

        // Check validity of culture code
        if (!CultureHelper.IsValidCultureInfoName(cultureCode))
        {
            Control.ShowError(Control.GetString("Culture.ErrorNoGlobalCulture"));
            Control.StopProcessing = true;
        }

        if (CultureHelper.IsNeutralCulture(cultureCode) && !Control.StopProcessing)
        {
            Control.ShowError(Control.GetString("culture.neutralculturecannotbeused"));
            Control.StopProcessing = true;
        }

        // Check if culture already exists for new created cultures
        if (cultureId == 0 && CultureInfoProvider.GetCultureInfo(cultureCode) != null && !Control.StopProcessing)
        {
            Control.ShowError(Control.GetString("culture_new.cultureexists"));
            Control.StopProcessing = true;
        }

        // Check whether culture alias is unique
        if (cultureId == 0 && !String.IsNullOrEmpty(cultureAlias) && !Control.StopProcessing)
        {
            CultureInfo cultureInfo = CultureInfoProvider.GetCultureInfoForCulture(cultureAlias);
            if ((cultureInfo != null) || CMSString.Equals(cultureAlias, cultureCode, true))
            {
                Control.ShowError(Control.GetString("Culture.AliasNotUnique"));
                Control.StopProcessing = true;
            }
        }

        // Show warning if culture is UI and there is no resx file
        bool isUiCulture = ValidationHelper.GetBoolean(Control.GetFieldValue("CultureIsUICulture"), false);
        if (!Control.StopProcessing && !LocalizationHelper.ResourceFileExistsForCulture(cultureCode) && isUiCulture)
        {
            string url = "http://www.kentico.com/Support/Support-files/Localization-packs";
            string downloadPage = String.Format(@"<a href=""{0}"" target=""_blank"" >{1}</a> ", url, HTMLHelper.HTMLEncode(url));
            Control.ShowWarning(String.Format(Control.GetString("culture.noresxwarning"), downloadPage));
        }
    }
}