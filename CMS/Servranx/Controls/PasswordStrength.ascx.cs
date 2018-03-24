using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.CMSHelper;
using CMS.FormControls;
using CMS.GlobalHelper;
using CMS.SettingsProvider;
using CMS.SiteProvider;
using CMS.DataEngine;
using CMS.Helpers;

public partial class Servranx_Controls_PasswordStrength : FormEngineUserControl
{
    #region "Variables"

    private string mSiteName = null;
    private int mPreferedLength = 12;
    private int mPreferedNonAlphaNumChars = 2;
    private string mClassPrefix = "PasswordStrength";
    private bool mAllowEmpty = false;
    private bool mShowValidationOnNewLine = false;
    private string mValidationGroup = string.Empty;
    private string mTextBoxClass = "TextBoxField";
    private bool mUseStylesForStrenghtIndicator = true;
    private bool mShowStrengthIndicator = true;

    #endregion


    #region "Private properties"

    /// <summary>
    /// Returns current site name.
    /// </summary>
    private string SiteName
    {
        get
        {
            if (mSiteName == null)
            {
                mSiteName = SiteContext.CurrentSiteName;
            }

            return mSiteName;
        }
    }


    /// <summary>
    /// Returns whether password policy is used.
    /// </summary>
    private bool UsePasswordPolicy
    {
        get
        {
            return SettingsKeyInfoProvider.GetBoolValue(SiteName + ".CMSUsePasswordPolicy");
        }
    }


    /// <summary>
    /// Returns password minimal length.
    /// </summary>
    private int MinLength
    {
        get
        {
            return SettingsKeyInfoProvider.GetIntValue(SiteName + ".CMSPolicyMinimalLength");
        }
    }


    /// <summary>
    /// Returns number of non alpha numeric characters
    /// </summary>
    private int MinNonAlphaNumChars
    {
        get
        {
            return SettingsKeyInfoProvider.GetIntValue(SiteName + ".CMSPolicyNumberOfNonAlphaNumChars");
        }
    }


    /// <summary>
    /// Returns password gegular expression.
    /// </summary>
    private string RegularExpression
    {
        get
        {
            return SettingsKeyInfoProvider.GetStringValue(SiteName + ".CMSPolicyRegularExpression");
        }
    }

    #endregion


    #region "Public properties"

    public bool IsPasswordUpdate
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the value that indicates whether inline styles should be used for strenght indicator
    /// </summary>
    public bool UseStylesForStrenghtIndicator
    {
        get
        {
            return mUseStylesForStrenghtIndicator;
        }
        set
        {
            mUseStylesForStrenghtIndicator = value;
        }
    }

    /// <summary>
    /// Gets or sets value of from control.
    /// </summary>
    public override object Value
    {
        get
        {
            return txtPassword.Text;
        }
        set
        {
            txtPassword.Text = ValidationHelper.GetString(value, string.Empty);
        }
    }


    /// <summary>
    /// Returns textbox client ID.
    /// </summary>
    public override string ValueElementID
    {
        get
        {
            return txtPassword.ClientID;
        }
    }


    /// <summary>
    /// Gets or sets prefered length.
    /// </summary>
    public int PreferedLength
    {
        get
        {
            return mPreferedLength;
        }
        set
        {
            mPreferedLength = value;
        }
    }


    /// <summary>
    /// Gets or sets prefered number of non alpha numeric characters.
    /// </summary>
    public int PreferedNonAlphaNumChars
    {
        get
        {
            return mPreferedNonAlphaNumChars;
        }
        set
        {
            mPreferedNonAlphaNumChars = value;
        }
    }


    /// <summary>
    /// Class prefix for labels.
    /// </summary>
    public string ClassPrefix
    {
        get
        {
            return mClassPrefix;
        }
        set
        {
            mClassPrefix = value;
        }
    }


    /// <summary>
    /// Gets or sets value of from control in string type.
    /// </summary>
    public override string Text
    {
        get
        {
            return txtPassword.Text;
        }
        set
        {
            txtPassword.Text = value;
        }
    }


    /// <summary>
    /// Gets or sets whether password could be empty.
    /// </summary>
    public bool AllowEmpty
    {
        get
        {
            return mAllowEmpty;
        }
        set
        {
            mAllowEmpty = value;
        }
    }


    /// <summary>
    /// Gets or sets whether validation control is shown under the control.
    /// </summary>
    public bool ShowValidationOnNewLine
    {
        get
        {
            return mShowValidationOnNewLine;
        }
        set
        {
            mShowValidationOnNewLine = value;
        }
    }


    /// <summary>
    /// Gets or sets validation group.
    /// </summary>
    public string ValidationGroup
    {
        get
        {
            return mValidationGroup;
        }
        set
        {
            mValidationGroup = value;
        }
    }


    /// <summary>
    /// Gets or sets class of textbox.
    /// </summary>
    public string TextBoxClass
    {
        get
        {
            return mTextBoxClass;
        }
        set
        {
            mTextBoxClass = value;
        }
    }


    /// <summary>
    /// Returns textbox attributes.
    /// </summary>
    public AttributeCollection TextBoxAttributes
    {
        get
        {
            return txtPassword.Attributes;
        }
    }


    /// <summary>
    /// Gets or sets maximal length of password. 
    /// </summary>
    public int MaxLength
    {
        get
        {
            return txtPassword.MaxLength;
        }
        set
        {
            txtPassword.MaxLength = value;
        }
    }


    /// <summary>
    /// Gets or sets HTML that is displayed next to password input and indicates password as required field.
    /// </summary>
    public string RequiredFieldMark
    {
        get
        {
            return lblRequiredFieldMark.Text;
        }
        set
        {
            lblRequiredFieldMark.Text = value;
        }
    }


    /// <summary>
    /// Gets or sets whether strength indicator is shown.
    /// </summary>
    public bool ShowStrengthIndicator
    {
        get
        {
            return mShowStrengthIndicator;
        }
        set
        {
            mShowStrengthIndicator = value;
        }
    }

    #endregion


    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        System.Globalization.CultureInfo currentUI = System.Globalization.CultureInfo.CurrentUICulture;

        /*if (Convert.ToString(currentUI) == "en-US")
        {
            txtPassword.ToolTip = "Password";
        }
        else if (Convert.ToString(currentUI) == "fr-FR")
        {
            txtPassword.ToolTip = "Mot de passe";
        }
        */
        // Set class
        if (IsPasswordUpdate)
        {
            txtPassword.ToolTip = GetString("newpwd");
        }
        else
        {
            txtPassword.ToolTip = GetString("pwd");
        }
        txtPassword.CssClass = TextBoxClass;
        
        if (ShowStrengthIndicator)
        {
            string tooltipMessage = string.Empty;

            StringBuilder sb = new StringBuilder();
            if (UsePasswordPolicy)
            {
                sb.Append(GetString("passwordstrength.notacceptable"), ";", GetString("passwordstrength.weak"));
                tooltipMessage = string.Format(GetString("passwordstrength.hint"), MinLength, MinNonAlphaNumChars, PreferedLength, PreferedNonAlphaNumChars);
            }
            else
            {
                sb.Append(GetString("passwordstrength.weak"), ";", GetString("passwordstrength.weak"));
                tooltipMessage = string.Format(GetString("passwordstrength.recommend"), PreferedLength, PreferedNonAlphaNumChars);
            }
        
            // Register jQuery and registration of script which shows password strength        
            ScriptHelper.RegisterJQuery(Page);
            ScriptHelper.RegisterScriptFile(Page, "~/CMSScripts/membership.js");

            sb.Append(";", GetString("passwordstrength.acceptable"), ";", GetString("passwordstrength.average"), ";", GetString("passwordstrength.strong"), ";", GetString("passwordstrength.excellent"));

            string regex = "''";
            if (!string.IsNullOrEmpty(RegularExpression))
            {
                regex = "/" + RegularExpression + "/";
            }

            // Javascript for calling js function on keyup of textbox
            string txtVar = "txtSearch_" + txtPassword.ClientID;
            string script =
                txtVar + " = $j('#" + txtPassword.ClientID + @"');
        if (" + txtVar + @" ) {                    
           " + txtVar + @".keyup(function(event){
                ShowStrength('" + txtPassword.ClientID + "', '" + MinLength + "', '" + PreferedLength + "', '" + MinNonAlphaNumChars + "', '"
                + PreferedNonAlphaNumChars + "', " + regex + ", '" + lblEvaluation.ClientID + "', '" + sb.ToString() + "', '" + ClassPrefix + "', '" + UsePasswordPolicy + "', '" + pnlPasswIndicator.ClientID + "', '" + UseStylesForStrenghtIndicator + @"');                               
            });                   
        }";

            ScriptHelper.RegisterStartupScript(this, typeof(string), "PasswordStrength_" + txtPassword.ClientID, ScriptHelper.GetScript(script));

            if (UseStylesForStrenghtIndicator)
            {
                pnlPasswStrengthIndicator.Style.Add("height", "5px");
                pnlPasswStrengthIndicator.Style.Add("background-color", "#dddddd");

                pnlPasswIndicator.Style.Add("height", "5px");
            }

            ScriptHelper.RegisterTooltip(Page);
            ScriptHelper.AppendTooltip(lblPasswStregth, tooltipMessage, "help");
        }
        else
        {
            pnlPasswStrengthIndicator.Visible = false;
            lblEvaluation.Visible = false;
            lblPasswStregth.Visible = false;
        }        

        // Set up required field validator
        if (AllowEmpty)
        {
            //rfvPassword.Enabled = false;
        }
        else
        {
            //rfvPassword.Text = "*";/*GetString("general.requirespassword");*/
            //rfvPassword.ValidationGroup = ValidationGroup;
            if (ShowValidationOnNewLine)
            {
               // rfvPassword.Text += "<br />";
            }
        }
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        lblRequiredFieldMark.Visible = !String.IsNullOrEmpty(lblRequiredFieldMark.Text);
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Returns whether 
    /// </summary>
    public override bool IsValid()
    {
        ValidationError = GetString("passwordpolicy.notaccetable");
        return SecurityHelper.CheckPasswordPolicy(txtPassword.Text, SiteName);
    }


    /// <summary>
    /// Returns hint for user to set password which met password policy
    /// </summary>
    public string GetPasswordPolicyHint()
    {
        if (UsePasswordPolicy)
        {
            return string.Format(GetString("passwordstrength.hint"), MinLength, MinNonAlphaNumChars, PreferedLength, PreferedNonAlphaNumChars);
        }

        return string.Empty;
    }

    #endregion
}