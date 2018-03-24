using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Text;

using CMS.CMSHelper;
using CMS.ExtendedControls;
using CMS.GlobalHelper;
using CMS.PortalControls;
using CMS.PortalEngine;
using CMS.SettingsProvider;
using CMS.SiteProvider;
using CMS.URLRewritingEngine;
using CMS.WebAnalytics;

using TreeNode = CMS.DocumentEngine.TreeNode;
using CMS.Helpers;
using CMS.DataEngine;
using CMS.Membership;
using CMS.MacroEngine;
using CMS.DocumentEngine;
using CMS.Protection;

public partial class CMSWebParts_Membership_Logon_LogonForm : CMSAbstractWebPart, ICallbackEventHandler
{
    #region "Private properties"

    private string mDefaultTargetUrl = "";

    #endregion


    #region "Public properties"

    /// <summary>
    /// Gets or sets the value that indicates whether retrieving of forgotten password is enabled.
    /// </summary>
    public bool AllowPasswordRetrieval
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("AllowPasswordRetrieval"), true);
        }
        set
        {
            SetValue("AllowPasswordRetrieval", value);
            lnkPasswdRetrieval.Visible = value;
        }
    }


    /// <summary>
    /// Gets or sets the sender e-mail (from).
    /// </summary>
    public string SendEmailFrom
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("SendEmailFrom"), SettingsKeyInfoProvider.GetStringValue(SiteContext.CurrentSiteName + ".CMSSendPasswordEmailsFrom"));
        }
        set
        {
            SetValue("SendEmailFrom", value);
        }
    }


    /// <summary>
    /// Gets or sets the default target url (rediredction when the user is logged in).
    /// </summary>
    public string DefaultTargetUrl
    {
        get
        {
            return ValidationHelper.GetString(GetValue("DefaultTargetUrl"), mDefaultTargetUrl);
        }
        set
        {
            SetValue("DefaultTargetUrl", value);
            mDefaultTargetUrl = value;
        }
    }


    /// <summary>
    /// Gets or sets the SkinID of the logon form.
    /// </summary>
    public override string SkinID
    {
        get
        {
            return base.SkinID;
        }
        set
        {
            base.SkinID = value;
            SetSkinID(value);
        }
    }


    /// <summary>
    /// Gets or sets the logon failure text.
    /// </summary>
    public string FailureText
    {
        get
        {
            return ValidationHelper.GetString(GetValue("FailureText"), "");
        }
        set
        {
            if (value.ToString().Trim() != "")
            {
                SetValue("FailureText", value);
                Login1.FailureText = value;
            }
        }
    }


    /// <summary>
    /// Gets or sets reset password url - this url is sent to user in e-mail when he wants to reset his password.
    /// </summary>
    public string ResetPasswordURL
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("ResetPasswordURL"), AuthenticationHelper.GetResetPasswordUrl(SiteContext.CurrentSiteName));
        }
        set
        {
            SetValue("ResetPasswordURL", value);
        }
    }

    #endregion


    /// <summary>
    /// Content loaded event handler.
    /// </summary>
    public override void OnContentLoaded()
    {
        base.OnContentLoaded();
        SetupControl();
    }


    /// <summary>
    /// Reloads data.
    /// </summary>
    public override void ReloadData()
    {
        base.ReloadData();
        SetupControl();
    }


    /// <summary>
    /// Initializes the control properties.
    /// </summary>
    protected void SetupControl()
    {
        if (StopProcessing)
        {
            rqValue.Visible = false;
            // Do not process
        }
        else
        {
            // Set failure text
            if (FailureText != "")
            {
                Login1.FailureText = ResHelper.LocalizeString(FailureText);
            }
            else
            {
                Login1.FailureText = GetString("Login_FailureText");
            }

            System.Globalization.CultureInfo currentUI = System.Globalization.CultureInfo.CurrentUICulture;

            if (Convert.ToString(currentUI) == "en-US")
            {
                Control ctl = Login1.FindControl("UserName"); 
                if (ctl != null) { 
                    TextBox txtUser = (TextBox)ctl.FindControl("UserName");
                    txtUser.ToolTip = "E-mail address";
                }
                Control ctl2 = Login1.FindControl("Password");
                if (ctl2 != null)
                {
                    TextBox txtUser = (TextBox)ctl2.FindControl("Password");
                    txtUser.ToolTip = "Password";
                }
				tbwmPwdRetrieval.WatermarkText = "E-mail address";
            }
            else if (Convert.ToString(currentUI) == "fr-FR")
            {
                Control ctl = Login1.FindControl("UserName");
                if (ctl != null)
                {
                    TextBox txtUser = (TextBox)ctl.FindControl("UserName");
                    txtUser.ToolTip = "Adresse E-mail";
                }
                Control ctl2 = Login1.FindControl("Password");
                if (ctl2 != null)
                {
                    TextBox txtUser = (TextBox)ctl2.FindControl("Password");
                    txtUser.ToolTip = "Mot de passe";
                }
				
				tbwmPwdRetrieval.WatermarkText = "Adresse E-mail";
            }

            // Set strings
            lnkPasswdRetrieval.Text = GetString("forgotpwd");
            lblPasswdRetrieval.Text = GetString("LogonForm.lblPasswordRetrieval");
            btnPasswdRetrieval.Text = GetString("LogonForm.btnPasswordRetrieval");
			
            //rqValue.ErrorMessage = GetString("LogonForm.rqValue");
			rqValue.ErrorMessage = GetString("errorlogin");
            rqValue.ValidationGroup = ClientID + "_PasswordRetrieval";

            // Set logon strings
            LocalizedLabel lblItem = (LocalizedLabel)Login1.FindControl("lblUserName");
            if (lblItem != null)
            {
                lblItem.Text = "{$LogonForm.UserName$}";
            }
            lblItem = (LocalizedLabel)Login1.FindControl("lblPassword");
            if (lblItem != null)
            {
                lblItem.Text = "{$LogonForm.Password$}";
            }
            LocalizedCheckBox chkItem = (LocalizedCheckBox)Login1.FindControl("chkRememberMe");
            if (chkItem != null)
            {
                chkItem.Text = "{$LogonForm.RememberMe$}";
            }
            LocalizedButton btnItem = (LocalizedButton)Login1.FindControl("LoginButton");
            if (btnItem != null)
            {
                btnItem.Text = "{$LogonForm.LogOnButton$}";
                btnItem.ValidationGroup = ClientID + "_Logon";
                btnItem.CssClass = GetString("LogonForm.css");
            }

            RequiredFieldValidator rfv = (RequiredFieldValidator)Login1.FindControl("rfvUserNameRequired");
            if (rfv != null)
            {
                rfv.ValidationGroup = ClientID + "_Logon";
                rfv.ToolTip = GetString("LogonForm.NameRequired");
            }

            CMSTextBox txtUserName = (CMSTextBox)Login1.FindControl("UserName");
            if (txtUserName != null)
            {
                txtUserName.EnableAutoComplete = SecurityHelper.IsAutoCompleteEnabledForLogin(SiteContext.CurrentSiteName);
            }

            lnkPasswdRetrieval.Visible = AllowPasswordRetrieval;

            Login1.LoggedIn += new EventHandler(Login1_LoggedIn);
            Login1.LoggingIn += new LoginCancelEventHandler(Login1_LoggingIn);
            Login1.LoginError += new EventHandler(Login1_LoginError);

            btnPasswdRetrieval.Click += new EventHandler(btnPasswdRetrieval_Click);
            btnPasswdRetrieval.ValidationGroup = ClientID + "_PasswordRetrieval";

            if (!RequestHelper.IsPostBack())
            {
                Login1.UserName = ValidationHelper.GetString(Request.QueryString["username"], "");
                // Set SkinID properties
                if (!StandAlone && (PageCycle < PageCycleEnum.Initialized) && (ValidationHelper.GetString(Page.StyleSheetTheme, "") == ""))
                {
                    SetSkinID(SkinID);
                }
            }

            if (!AllowPasswordRetrieval)
            {
                pnlPasswdRetrieval.Visible = false;
            }

            // Register script to update logon error message
            LocalizedLabel failureLit = Login1.FindControl("FailureText") as LocalizedLabel;
            if (failureLit != null)
            {
                StringBuilder sbScript = new StringBuilder();
                sbScript.Append(@"
function UpdateLabel_", ClientID, @"(content, context) {
    var lbl = document.getElementById(context);
    if(lbl)
    {       
        lbl.innerHTML = content;
        lbl.className = ""InfoLabel"";
    }
}");
                ScriptHelper.RegisterClientScriptBlock(this, GetType(), "InvalidLogonAttempts_" + ClientID, sbScript.ToString(), true);
            }            
        }
    }

    void Login1_LoginError(object sender, EventArgs e)
    {
        // Check if custom failure text is not set
        if (string.IsNullOrEmpty(FailureText))
        {
            LocalizedLabel failureLit = Login1.FindControl("FailureText") as LocalizedLabel;
            if (failureLit != null)
            {
                // Display account lock information
                if (AuthenticationHelper.DisplayAccountLockInformation(SiteContext.CurrentSiteName))
                {
                    // Check if account locked due to reaching maximum invalid logon attempts
                    string link = "<a href=\"#\" onclick=\"" + Page.ClientScript.GetCallbackEventReference(this, "null", "UpdateLabel_" + ClientID, "'" + failureLit.ClientID + "'") + ";\">" + GetString("general.clickhere") + "</a>";
                    if (ValidationHelper.GetBoolean(RequestStockHelper.GetItem("UserAccountLockedInvalidLogonAttempts"), false))
                    {
                        failureLit.Text = string.Format(GetString("invalidlogonattempts.unlockaccount.accountlocked") + GetString("invalidlogonattempts.unlockaccount.accountlockedlink"), link);
                    }

                    if (ValidationHelper.GetBoolean(RequestStockHelper.GetItem("UserAccountLockedPasswordExpired"), false))
                    {
                        failureLit.Text = string.Format(GetString("passwordexpiration.accountlocked") + GetString("invalidlogonattempts.unlockaccount.accountlockedlink"), link);
                    }
                }
            }
        }
    }


    /// <summary>
    /// OnLoad override (show hide password retrieval).
    /// </summary>
    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        

        if (hdnPasswDisplayed.Value == "")
        {
            pnlPasswdRetrieval.Attributes.Add("style", "display:none;");
        }
        else
        {
            pnlPasswdRetrieval.Attributes.Add("style", "display:block;");
        }
    }


    /// <summary>
    /// Sets SkinId to all controls in logon form.
    /// </summary>
    private void SetSkinID(string skinId)
    {
        if (skinId != "")
        {
            Login1.SkinID = skinId;

            LocalizedLabel lbl = (LocalizedLabel)Login1.FindControl("lblUserName");
            if (lbl != null)
            {
                lbl.SkinID = skinId;
            }
            lbl = (LocalizedLabel)Login1.FindControl("lblPassword");
            if (lbl != null)
            {
                lbl.SkinID = skinId;
            }

            TextBox txt = (TextBox)Login1.FindControl("UserName");
            if (txt != null)
            {
                txt.SkinID = skinId;
            }
            txt = (TextBox)Login1.FindControl("Password");
            if (txt != null)
            {
                txt.SkinID = skinId;
            }

            LocalizedCheckBox chk = (LocalizedCheckBox)Login1.FindControl("chkRememberMe");
            if (chk != null)
            {
                chk.SkinID = skinId;
            }

            LocalizedButton btn = (LocalizedButton)Login1.FindControl("LoginButton");
            if (btn != null)
            {
                btn.SkinID = skinId;
            }
        }
    }


    /// <summary>
    /// Applies given stylesheet skin.
    /// </summary>
    public override void ApplyStyleSheetSkin(Page page)
    {
        SetSkinID(SkinID);

        base.ApplyStyleSheetSkin(page);
    }


    /// <summary>
    /// Retrieve the user password.
    /// </summary>
    private void btnPasswdRetrieval_Click(object sender, EventArgs e)
    {
        string value = txtPasswordRetrieval.Text.Trim();

        if (value != String.Empty)
        {
            // Prepare return URL
            string returnUrl = RequestContext.CurrentURL;
            if (!string.IsNullOrEmpty(Login1.UserName))
            {
                returnUrl = URLHelper.AddParameterToUrl(returnUrl, "username", value);
            }
            
            bool success;
			
			string resultat = AuthenticationHelper.ForgottenEmailRequest(value, SiteContext.CurrentSiteName, "LOGONFORM", SendEmailFrom, MacroContext.CurrentResolver, ResetPasswordURL, out success, returnUrl);
			

			Control ctl = pnlPasswdRetrieval.FindControl("lblResult"); 
			if (ctl != null) { 
				Label labelResult = (Label)ctl.FindControl("lblResult");
				System.Globalization.CultureInfo currentUI = System.Globalization.CultureInfo.CurrentUICulture;
				if (Convert.ToString(currentUI) == "fr-FR"){
					if (resultat.Equals("No user found.")){
						labelResult.Text = "Aucun utilisateur trouv�.";
						labelResult.Visible = true;
					}
					else if(resultat.Equals("Request for password change was sent.") || resultat.Contains("password has been sen")){
						labelResult.Text = "La demande de changement de mot de passe a �t� envoy��.";
						labelResult.Visible = true;
					}
				}else{
					labelResult.Text = resultat;
					labelResult.Visible = true;
				}

			}
			
            		        
            pnlPasswdRetrieval.Attributes.Add("style", "display:block;");
        }
    }


    /// <summary>
    /// Logged in handler.
    /// </summary>
    private void Login1_LoggedIn(object sender, EventArgs e)
    {
        // Set view mode to live site after login to prevent bar with "Close preview mode"
        PortalContext.ViewMode = ViewModeEnum.LiveSite;

        // Ensure response cookie
        CookieHelper.EnsureResponseCookie(FormsAuthentication.FormsCookieName);

        // Set cookie expiration
        if (Login1.RememberMeSet)
        {
            CookieHelper.ChangeCookieExpiration(FormsAuthentication.FormsCookieName, DateTime.Now.AddYears(1), false);
        }
        else
        {
            // Extend the expiration of the authentication cookie if required
            if (!AuthenticationHelper.UseSessionCookies && (HttpContext.Current != null) && (HttpContext.Current.Session != null))
            {
                CookieHelper.ChangeCookieExpiration(FormsAuthentication.FormsCookieName, DateTime.Now.AddMinutes(Session.Timeout), false);
            }
        }

        // Current username
        string userName = Login1.UserName;

        // Get user name (test site prefix too)
        UserInfo ui = UserInfoProvider.GetUserInfoForSitePrefix(userName, SiteContext.CurrentSite);

        // Check whether safe user name is required and if so get safe username
        if (RequestHelper.IsMixedAuthentication() && UserInfoProvider.UseSafeUserName)
        {
            // Get info on the authenticated user            
            if (ui == null)
            {
                // User stored with safe name
                userName = ValidationHelper.GetSafeUserName(Login1.UserName, SiteContext.CurrentSiteName);

                // Find user by safe name
                ui = UserInfoProvider.GetUserInfoForSitePrefix(userName, SiteContext.CurrentSite);
                if (ui != null)
                {
                    // Authenticate user by site or global safe username
                    CMSContext.AuthenticateUser(ui.UserName, Login1.RememberMeSet);
                }
            }
        }

        if (ui != null)
        {
            // If user name is site prefixed, authenticate user manually 
            if (UserInfoProvider.IsSitePrefixedUser(ui.UserName))
            {
                CMSContext.AuthenticateUser(ui.UserName, Login1.RememberMeSet);
            }

            // Log activity
            int contactID = ModuleCommands.OnlineMarketingGetUserLoginContactID(ui);
            Activity activityLogin = new ActivityUserLogin(contactID, ui, DocumentContext.CurrentDocument, AnalyticsContext.ActivityEnvironmentVariables);
            activityLogin.Log();
        }

        // Redirect user to the return url, or if is not defined redirect to the default target url
        string url = QueryHelper.GetString("ReturnURL", string.Empty);
        if (!string.IsNullOrEmpty(url))
        {
            if (url.StartsWithCSafe("~") || url.StartsWithCSafe("/") || QueryHelper.ValidateHash("hash"))
            {
                URLHelper.Redirect(ResolveUrl(ValidationHelper.GetString(Request.QueryString["ReturnURL"], "")));
            }
            else
            {
                URLHelper.Redirect(ResolveUrl("~/CMSMessages/Error.aspx?title=" + ResHelper.GetString("general.badhashtitle") + "&text=" + ResHelper.GetString("general.badhashtext")));
            }
        }
        else
        {
            if (DefaultTargetUrl != "")
            {
                URLHelper.Redirect(ResolveUrl(DefaultTargetUrl));
            }
            else
            {
                URLHelper.Redirect(URLRewriter.CurrentURL);
            }
        }
    }


    /// <summary>
    /// Ligging in handler.
    /// </summary>
    private void Login1_LoggingIn(object sender, LoginCancelEventArgs e)
    {
        // Ban IP addresses which are blocked for login
        if (!BannedIPInfoProvider.IsAllowed(SiteContext.CurrentSiteName, BanControlEnum.Login))
        {
            e.Cancel = true;

            LocalizedLabel failureLit = Login1.FindControl("FailureText") as LocalizedLabel;
            if (failureLit != null)
            {
                failureLit.Visible = true;
                failureLit.Text = GetString("banip.ipisbannedlogin");
            }
        }

        if (((CheckBox)Login1.FindControl("chkRememberMe")).Checked)
        {
            Login1.RememberMeSet = true;
        }
        else
        {
            Login1.RememberMeSet = false;
        }
    }


    /// <summary>
    /// On prerender event.
    /// </summary>
    /// <param name="e">Arguments.</param>
    protected override void OnPreRender(EventArgs e)
    {
        ltlScript.Text = ScriptHelper.GetScript("function ShowPasswdRetrieve(id){var hdnDispl = document.getElementById('" + hdnPasswDisplayed.ClientID + "'); style=document.getElementById(id).style;style.display=(style.display == 'block')?'none':'block'; if(hdnDispl != null){ if(style.display == 'block'){hdnDispl.value='1';}else{hdnDispl.value='';}}}");
        lnkPasswdRetrieval.Attributes.Add("onclick", "ShowPasswdRetrieve('" + pnlPasswdRetrieval.ClientID + "'); return false;");

        base.OnPreRender(e);
    }


    ///<summary>
    /// Overrides the generation of the SPAN tag with custom tag.
    ///</summary>
    protected HtmlTextWriterTag TagKey
    {
        get
        {
            if (SiteContext.CurrentSite != null)
            {
                if (SettingsKeyInfoProvider.GetStringValue(SiteContext.CurrentSiteName + ".CMSControlElement").ToLowerCSafe().Trim() == "div")
                {
                    return HtmlTextWriterTag.Div;
                }
                else
                {
                    return HtmlTextWriterTag.Span;
                }
            }
            return HtmlTextWriterTag.Span;
        }
    }


    #region "ICallbackEventHandler Members"

    public string GetCallbackResult()
    {
        string result = "";
        bool outParam = true;
        UserInfo ui = UserInfoProvider.GetUserInfo(Login1.UserName);
        if (ui != null)
        {
            string siteName = SiteContext.CurrentSiteName;

            // Prepare return URL
            string returnUrl = RequestContext.CurrentURL;
            if (!string.IsNullOrEmpty(Login1.UserName))
            {
                returnUrl = URLHelper.AddParameterToUrl(returnUrl, "username", Login1.UserName);
            }
            
            switch (UserAccountLockCode.ToEnum(ui.UserAccountLockReason))
            {
                case UserAccountLockEnum.MaximumInvalidLogonAttemptsReached:
                    result = AuthenticationHelper.SendUnlockAccountRequest(ui, siteName, "USERLOGON", SettingsKeyInfoProvider.GetStringValue(siteName + ".CMSSendPasswordEmailsFrom"), MacroContext.CurrentResolver, returnUrl);
                    break;

                case UserAccountLockEnum.PasswordExpired:
                    result = AuthenticationHelper.SendPasswordRequest(ui, siteName, "USERLOGON", SettingsKeyInfoProvider.GetStringValue(siteName + ".CMSSendPasswordEmailsFrom"), "Membership.PasswordExpired", null, AuthenticationHelper.GetResetPasswordUrl(siteName), out outParam, returnUrl);
                    break;
            }
        }

        return result;
    }

    public void RaiseCallbackEvent(string eventArgument)
    {
        return;
    }

    #endregion
  
}