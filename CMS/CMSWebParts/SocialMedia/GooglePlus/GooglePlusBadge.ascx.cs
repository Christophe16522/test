using System;
using System.Web.UI;

using CMS.Helpers;
using CMS.DocumentEngine;
using CMS.SocialMedia;


public partial class CMSWebParts_SocialMedia_GooglePlus_GooglePlusBadge : SocialMediaAbstractWebPart
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
    /// Second part of the Google+ page link.
    /// </summary>
    public string PageLink
    {
        get
        {
            return ValidationHelper.GetString(GetValue("Link"), string.Empty);
        }
        set
        {
            SetValue("Link", value);
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
    /// Style.
    /// </summary>
    public string Style
    {
        get
        {
            return ValidationHelper.GetString(GetValue("Style"), string.Empty);
        }
        set
        {
            SetValue("Style", value);
        }
    }


    /// <summary>
    /// Language.
    /// </summary>
    public string Language
    {
        get
        {
            return ValidationHelper.GetString(GetValue("Language"), string.Empty);
        }
        set
        {
            SetValue("Language", value);
        }
    }


    /// <summary>
    /// Custom text.
    /// </summary>
    public string CustomName
    {
        get
        {
            return ValidationHelper.GetString(GetValue("CustomName"), string.Empty);
        }
        set
        {
            SetValue("CustomName", value);
        }
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Initializes the control properties
    /// </summary>
    protected override void SetupControl()
    {
        if (this.StopProcessing)
        {
            // Do not process
        }
        else
        {
            // Build code for plugin
            string output = String.Empty;
            string src = "https://apis.google.com/js/plusone.js";

            if (string.IsNullOrEmpty(Language))
            {
                Language = DocumentContext.CurrentDocumentCulture.CultureCode;
            }

            switch (Style)
            {
                case "badge":
                case "smallbadge":
                    output = "<div style=\"overflow: hidden; width: {0}px;\"><script type=\"text/javascript\" src=\"{1}\">lang: '{2}'</script><g:plus href=\"{3}\" size=\"{4}\"></g:plus></div>";
                    ltlPluginCode.Text = String.Format(output, Width, src, Language, PageLink, Style);
                    break;
                default:
                    string iconSize = Style.Substring(4);
                    output = "<div style=\"overflow: hidden; width: {0}px;\"><a href=\"{1}?prsrc=3\" style=\"cursor:pointer;display:inline-block;text-decoration:none;color:#333;font:13px/16px arial,sans-serif;\"><span style=\"display:inline-block;font-weight:bold;vertical-align:top;margin-right:5px;margin-top:8px;\">{2}</span><span style=\"display:inline-block;vertical-align:top;margin-right:15px;margin-top:8px;\">on</span><img src=\"https://ssl.gstatic.com/images/icons/gplus-{3}.png\" alt=\"\" style=\"border:0;width:{3}px;height:{3}px;\"/></a></div>";
                    ltlPluginCode.Text = String.Format(output, Width, PageLink, CustomName, iconSize);
                    break;
            }
        }
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        if (StopProcessing)
        {
            // Do nothing
        }
        else
        {
            // Add link tag to head
            string linkTag = String.Format("<link href=\"{0}\" rel=\"publisher\" />", PageLink);

            Page.Header.Controls.Add(new LiteralControl(linkTag));
        }
    }

    #endregion
}



