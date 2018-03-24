using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.CMSHelper;
using CMS.GlobalHelper;
using CMS.SettingsProvider;
using CMS.SiteProvider;
using CMS.UIControls;
using CMS.ExtendedControls;
using CMS.Helpers;
using CMS.DataEngine;
using CMS.Membership;

public partial class CMSModules_Membership_Controls_ResetPasswordServranx : CMSUserControl
{
    #region "Variables"

    private string siteName = string.Empty;
    private double interval = 0;
    private string hash = string.Empty;
    private string time = string.Empty;
    private int userID = 0;
    private int policyReq = 0;
    private int pwdExp = 0;
    private string returnUrl = null;

    #endregion


    #region "Properties"

    /// <summary>
    /// Text shown if request hash isn't found.
    /// </summary>
    public string InvalidRequestText
    {
        get;
        set;
    }


    public string ExceededIntervalText
    {
        get;
        set;
    }


    /// <summary>
    /// Url on which is user redirected after successful password reset.
    /// </summary>
    public string RedirectUrl
    {
        get;
        set;
    }

    /// <summary>
    /// E-mail address from which e-mail is sent.
    /// </summary>
    public string SendEmailFrom
    {
        get;
        set;
    }


    /// <summary>
    /// Text shown when password reset was successful.
    /// </summary>
    public string SuccessText
    {
        get;
        set;
    }


    /// <summary>
    /// Messages placeholder
    /// </summary>
    public override MessagesPlaceHolder MessagesPlaceHolder
    {
        get
        {
            return plcMess;
        }
    }

    #endregion


    #region "Page events"

    protected void Page_Init(object sender, EventArgs e)
    {
        if (!RequestHelper.IsPostBack())
        {
            // Clear session value
            SessionHelper.SetValue("UserPasswordRequestID", 0);
        }
    }


    /// <summary>
    /// Page load.
    /// </summary>
    /// <param name="sender">Sender.</param>
    /// <param name="e">Arguments</param>
    protected void Page_Load(object sender, EventArgs e)
    {
		lblPassword.Text = GetString("newpwd");
		lblConfirmPassword.Text = GetString("ConfirmPassword");
		
        userID = ValidationHelper.GetInteger(SessionHelper.GetValue("UserPasswordRequestID"), 0);

        hash = QueryHelper.GetString("hash", string.Empty);
        time = QueryHelper.GetString("datetime", string.Empty);
        policyReq = QueryHelper.GetInteger("policyreq", 0);
        pwdExp = QueryHelper.GetInteger("exp", 0);
        returnUrl = QueryHelper.GetString("returnurl", null);

        btnReset.Text = GetString("general.reset");
        rfvConfirmPassword.Text = GetString("errorconfirmpwd");

        siteName = SiteContext.CurrentSiteName;

        // Get interval from settings
        interval = SettingsKeyInfoProvider.GetDoubleValue(siteName + ".CMSResetPasswordInterval");

        // Prepare failed message
        string invalidRequestMessage = DataHelper.GetNotEmpty(InvalidRequestText, String.Format(ResHelper.GetString("membership.passwresetfailed"), ResolveUrl("~/cmspages/logon.aspx?forgottenpassword=1")));

        // Reset password cancelation
        if (QueryHelper.GetBoolean("cancel", false))
        {
            // Get user info
            UserInfo ui = UserInfoProvider.GetUserInfoWithSettings("UserPasswordRequestHash = '" + SecurityHelper.GetSafeQueryString(hash, true) + "'");
            if (ui != null)
            {
                ui.UserPasswordRequestHash = null;
                UserInfoProvider.SetUserInfo(ui);

                SessionHelper.Remove("UserPasswordRequestID");

                ShowInformation(GetString("membership.passwresetcancelled"));
            }
            else
            {
                ShowError(invalidRequestMessage);
            }

            pnlReset.Visible = false;
            return;
        }

        // Reset password request
        if (!URLHelper.IsPostback())
        {
            if (policyReq > 0)
            {
                ShowInformation(GetString("passwordpolicy.policynotmet") + "<br />" + passStrength.GetPasswordPolicyHint());
            }

            // Prepare query
            string query = "UserPasswordRequestHash = '" + SecurityHelper.GetSafeQueryString(hash, true) + "'";
            if (userID > 0)
            {
                query = SqlHelper.AddWhereCondition(query, "UserID = " + userID, "OR");
            }

            // Get user info
            UserInfo ui = UserInfoProvider.GetUserInfoWithSettings(query);

            // Validate request
            ResetPasswordResultEnum result = AuthenticationHelper.ValidateResetPassword(ui, hash, time, interval, "Reset password control");

            // Prepare messages
            string timeExceededMessage = DataHelper.GetNotEmpty(ExceededIntervalText, String.Format(ResHelper.GetString("membership.passwreqinterval"), ResolveUrl("~/cmspages/logon.aspx?forgottenpassword=1")));
            string resultMessage = string.Empty;

            // Check result
            switch (result)
            {
                case ResetPasswordResultEnum.Success:
                    // Save user is to session                    
                    SessionHelper.SetValue("UserPasswordRequestID", ui.UserID);

                    // Delete it from user info
                    ui.UserPasswordRequestHash = null;
                    UserInfoProvider.SetUserInfo(ui);

                    break;

                case ResetPasswordResultEnum.TimeExceeded:
                    resultMessage = timeExceededMessage;
                    break;

                default:
                    resultMessage = invalidRequestMessage;
                    break;
            }

            if (!string.IsNullOrEmpty(resultMessage))
            {
                // Show error message
                ShowError(resultMessage);

                pnlReset.Visible = false;

                return;
            }
        }
    }


    /// <summary>
    /// Click event of btnOk.
    /// </summary>
    /// <param name="sender">Sender</param>
    /// <param name="e">Arguments</param>
    protected void btnReset_Click(object sender, EventArgs e)
    {
        if ((passStrength.Text.Length > 0) && rfvConfirmPassword.IsValid)
        {
            if (passStrength.Text == txtConfirmPassword.Text)
            {
                // Check policy
                if (passStrength.IsValid())
                {
                    // Check if password expired
                    if (pwdExp > 0)
                    {
                        UserInfo ui = UserInfoProvider.GetUserInfo(userID);
                        if (!UserInfoProvider.IsUserPasswordDifferent(ui, passStrength.Text))
                        {
						    ShowError(GetString("errormdprequired"));
                            //ShowError(GetString("passreset.newpasswordrequired"));
                            return;
                        }
                    }

                    // Get e-mail address of sender
                    string emailFrom = DataHelper.GetNotEmpty(SendEmailFrom, SettingsKeyInfoProvider.GetStringValue(siteName + ".CMSSendPasswordEmailsFrom"));

                    // Try to reset password and show result to user
                    bool success;
                    string resultText = AuthenticationHelper.ResetPassword(hash, time, userID, interval, passStrength.Text, "Reset password control", emailFrom, siteName, null, out success, InvalidRequestText, ExceededIntervalText, returnUrl);

                    // If password reset was successful
                    if (success)
                    {
                        SessionHelper.Remove("UserPasswordRequestID");

                        // Redirect to specified URL 
                        if (!string.IsNullOrEmpty(RedirectUrl))
                        {
                            URLHelper.Redirect(RedirectUrl);
                        }

                        // Get proper text
                        ShowConfirmation(DataHelper.GetNotEmpty(SuccessText, resultText));
                        pnlReset.Visible = false;
                        lblLogonLink.Text = string.Format(GetString("memberhsip.logonlink"), returnUrl);
                    }
                    else
                    {
                       // ShowError(resultText);
					   ShowError(GetString("erreur"));
                    }
                }
                else
                {
					ShowError(GetString("erreur"));
                    //ShowError(AuthenticationHelper.GetPolicyViolationMessage(CMSContext.CurrentSiteName));
                }
				
            }
            else
            {
				ShowError(GetString("errormdp"));
                //ShowError(GetString("passreset.notmatch"));
            }
        }
        else
        {
			ShowError(GetString("errormdprequired"));
            //ShowError(GetString("general.requiresvalue"));
        }
    }

    #endregion
}