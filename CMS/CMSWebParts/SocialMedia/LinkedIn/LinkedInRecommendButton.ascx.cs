using System;
using System.Linq;

using CMS.Helpers;
using CMS.Base;
using CMS.MembershipProvider;
using CMS.SiteProvider;
using CMS.SocialMedia;


public partial class CMSWebParts_SocialMedia_LinkedIn_LinkedInRecommendButton : SocialMediaAbstractWebPart
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
            ltlButtonCode.Visible = !value;
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
    /// Product ID.
    /// </summary>
    public string ProductID
    {
        get
        {
            return ValidationHelper.GetString(GetValue("ProductID"), string.Empty);
        }
        set
        {
            SetValue("ProductID", value);
        }
    }


    /// <summary>
    /// Count box position.
    /// </summary>
    public string CountBox
    {
        get
        {
            return ValidationHelper.GetString(GetValue("CountBox"), string.Empty);
        }
        set
        {
            SetValue("CountBox", value);
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
            // Build plugin code
            string src = "http://platform.linkedin.com/in.js";
            string apiKey = LinkedInHelper.GetLinkedInApiKey(SiteContext.CurrentSiteName);
            
            // Try to parse product URL
            if (ProductID.ToLowerCSafe().StartsWithCSafe("http"))
            {
                int indexStart = ProductID.LastIndexOfCSafe("-");
                int indexEnd = ProductID.LastIndexOfCSafe("/");

                if ((indexStart != -1) && (indexEnd != -1))
                {
                    ProductID = ProductID.Substring(indexStart + 1, indexEnd - indexStart - 1);
                }
            }

            string output = "<div style=\"overflow: hidden; width: {0}px;\"><script src=\"{1}\" type=\"text/javascript\">api_key: {5}</script><script type=\"IN/RecommendProduct\" data-company=\"{2}\" data-product=\"{3}\" data-counter=\"{4}\"></script></div>";
            ltlButtonCode.Text = String.Format(output, Width.ToString(), src, CompanyID, ProductID, CountBox, apiKey);
        }
    }

    #endregion
}