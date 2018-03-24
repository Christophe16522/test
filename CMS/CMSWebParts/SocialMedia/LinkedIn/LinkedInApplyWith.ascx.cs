using System;
using System.Linq;
using System.Text;

using CMS.Helpers;
using CMS.Base;
using CMS.EventLog;
using CMS.MembershipProvider;
using CMS.PortalEngine;
using CMS.SiteProvider;
using CMS.SocialMedia;


public partial class CMSWebParts_SocialMedia_LinkedIn_LinkedInApplyWith : SocialMediaAbstractWebPart
{
    #region "Private fiels"

    private bool mHide;

    #endregion


    #region "Public properties"

    /// <summary>
    /// Indicates whether to hide content of the WebPart
    /// </summary>
    public override bool HideContent
    {
        get
        {
            return mHide;
        }
        set
        {
            mHide = value;
            ltlPluginCode.Visible = !value;
        }
    }


    /// <summary>
    /// Company ID.
    /// </summary>
    public string CompanyID
    {
        get
        {
            return ValidationHelper.GetString(GetValue("CompanyID"), string.Empty);
        }
        set
        {
            SetValue("CompanyID", value);
        }
    }


    /// <summary>
    /// Width of the web part in pixels.
    /// </summary>
    public int Width
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("Width"), 110);
        }
        set
        {
            SetValue("Width", value);
        }
    }


    /// <summary>
    /// Job title.
    /// </summary>
    public string JobTitle
    {
        get
        {
            return ValidationHelper.GetString(GetValue("JobTitle"), string.Empty);
        }
        set
        {
            SetValue("JobTitle", value);
        }
    }


    /// <summary>
    /// Job location.
    /// </summary>
    public string JobLocation
    {
        get
        {
            return ValidationHelper.GetString(GetValue("JobLocation"), string.Empty);
        }
        set
        {
            SetValue("JobLocation", value);
        }
    }


    /// <summary>
    /// Delivery email.
    /// </summary>
    public string DeliveryEmail
    {
        get
        {
            return ValidationHelper.GetString(GetValue("DeliveryEmail"), string.Empty);
        }
        set
        {
            SetValue("DeliveryEmail", value);
        }
    }


    /// <summary>
    /// How phone number will be requested.
    /// </summary>
    public string RequestPhone
    {
        get
        {
            return ValidationHelper.GetString(GetValue("RequestPhone"), String.Empty);
        }
        set
        {
            SetValue("RequestPhone", value);
        }
    }


    /// <summary>
    /// How cover letter will be requested.
    /// </summary>
    public string RequestCoverLetter
    {
        get
        {
            return ValidationHelper.GetString(GetValue("RequestCoverLetter"), String.Empty);
        }
        set
        {
            SetValue("RequestCoverLetter", value);
        }
    }


    /// <summary>
    /// Company logo.
    /// </summary>
    public string CompanyLogo
    {
        get
        {
            return ValidationHelper.GetString(GetValue("CompanyLogo"), String.Empty);
        }
        set
        {
            SetValue("CompanyLogo", value);
        }
    }


    /// <summary>
    /// Whether to show additional help text.
    /// </summary>
    public bool ShowHelpText
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("ShowHelpText"), false);
        }
        set
        {
            SetValue("ShowHelpText", value);
        }
    }


    /// <summary>
    /// How cover letter will be requested.
    /// </summary>
    public string ThemeColor
    {
        get
        {
            return ValidationHelper.GetString(GetValue("ThemeColor"), String.Empty);
        }
        set
        {
            SetValue("ThemeColor", value);
        }
    }


    /// <summary>
    /// Button size.
    /// </summary>
    public string ButtonSize
    {
        get
        {
            return ValidationHelper.GetString(GetValue("ButtonSize"), String.Empty);
        }
        set
        {
            SetValue("ButtonSize", value);
        }
    }


    /// <summary>
    /// Questions.
    /// </summary>
    public string Questions
    {
        get
        {
            return ValidationHelper.GetString(GetValue("Questions"), String.Empty);
        }
        set
        {
            SetValue("Questions", value);
        }
    }

    #endregion


    #region "Methods"


    /// <summary>
    /// Initializes the control properties.
    /// </summary>
    protected override void SetupControl()
    {
        if (StopProcessing)
        {
            // Do not process
        }
        else
        {
            // Initialize variables
            string dataJobLocation = String.Empty, dataLogo = String.Empty, dataThemeColor = String.Empty, dataButtonSize = String.Empty, dataQuestions = String.Empty;
            string src = "http://platform.linkedin.com/in.js";
            string apiKey = LinkedInHelper.GetLinkedInApiKey(SiteContext.CurrentSiteName);
            string apiSecret = LinkedInHelper.GetLinkedInSecretKey(SiteContext.CurrentSiteName);

            // Check settings
            if (!String.IsNullOrEmpty(apiKey) && !String.IsNullOrEmpty(apiSecret))
            {

                // Check if optional parameters are set and transform them accordingly
                if (!string.IsNullOrEmpty(CompanyLogo))
                {
                    dataLogo = "data-logo=\"" + URLHelper.GetAbsoluteUrl(CompanyLogo) + "\"";
                }

                if (!string.IsNullOrEmpty(JobLocation))
                {
                    dataJobLocation = "data-joblocation=\"" + JobLocation + "\"";
                }

                if (!string.IsNullOrEmpty(ThemeColor))
                {
                    dataThemeColor = "data-themecolor=\"" + ThemeColor + "\"";
                }

                if (ButtonSize.EqualsCSafe("medium"))
                {
                    dataButtonSize = "data-size=\"medium\"";
                }

                // Parse questions string and format it
                if (!string.IsNullOrEmpty(Questions))
                {
                    dataQuestions = "data-questions='[";
                    string[] questionArray = Questions.Split(new string[1] { Environment.NewLine }, StringSplitOptions.None);
                    foreach (string s in questionArray)
                    {
                        dataQuestions += "{\"question\": \"" + s + "\"},";
                    }
                    dataQuestions = dataQuestions.TrimEnd(',');
                    dataQuestions += "]'";
                }

                // Build plugin code
                string output = "<div style=\"overflow: hidden; width: {0}px;\"><script src=\"{1}\" type=\"text/javascript\">api_key: {2}</script><script type=\"IN/Apply\" data-companyid=\"{3}\" data-jobtitle=\"{4}\" data-email=\"{5}\" {6} data-phone=\"{7}\" data-coverletter=\"{8}\" {9} data-showtext=\"{10}\" {11} {12} {13}></script></div>";
                ltlPluginCode.Text = String.Format(output, Width, src, apiKey, CompanyID, JobTitle, DeliveryEmail, dataJobLocation, RequestPhone, RequestCoverLetter, dataLogo, ShowHelpText.ToString().ToLowerCSafe(), dataThemeColor, dataButtonSize, dataQuestions);
            }
            else
            {
                if (PortalContext.IsDesignMode(PortalContext.ViewMode))
                {
                    string pathToSettings = SocialMediaHelper.GetPathToLinkedInSettings();

                    ltlPluginCode.Text = "<span class='ErrorLabel'>" + String.Format(ResHelper.GetString("socialnetworking.linkedin.settingsrequired"), pathToSettings) + "</span>";
                }

                // Log event
                EventLogProvider.LogException("SocialMedia", "LinkedInApplyWith", new Exception("Missing LinkedIn settings."));
            }
        }
    }

    #endregion
}