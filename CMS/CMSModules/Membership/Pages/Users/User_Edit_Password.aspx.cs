using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.EmailEngine;
using CMS.EventLog;
using CMS.Helpers;
using CMS.Base;
using CMS.SiteProvider;
using CMS.Membership;
using CMS.UIControls;
using CMS.ExtendedControls.ActionsConfig;
using CMS.MacroEngine;
using CMS.DataEngine;

public partial class CMSModules_Membership_Pages_Users_User_Edit_Password : CMSUsersPage
{
    #region "Constants"

    private const string hiddenPassword = "********";

    private const string GENERATEPASSWORD = "generatepassword";
    private const string SENDPASSWORD = "sendpassword";

    #endregion


    #region "Private fields"

    private int mUserID = 0;
    private UserInfo ui = null;

    #endregion


    #region "Private properties"

    /// <summary>
    /// Current user ID.
    /// </summary>
    private int UserID
    {
        get
        {
            if (mUserID == 0)
            {
                mUserID = QueryHelper.GetInteger("userid", 0);
            }

            return mUserID;
        }
    }

    #endregion


    protected void Page_Load(object sender, EventArgs e)
    {
        ButtonSetPassword.Text = GetString("Administration-User_Edit_Password.SetPassword");
        LabelPassword.Text = GetString("Administration-User_Edit_Password.NewPassword");
        LabelConfirmPassword.Text = GetString("Administration-User_Edit_Password.ConfirmPassword");
        chkSendEmail.Text = GetString("Administration-User_Edit_Password.SendEmail");

        if (!RequestHelper.IsPostBack())
        {
            if (UserID > 0)
            {
                // Check that only global administrator can edit global administrator's accounts
                ui = UserInfoProvider.GetUserInfo(UserID);
                EditedObject = ui;
                CheckUserAvaibleOnSite(ui);
                if (!CheckGlobalAdminEdit(ui))
                {
                    plcTable.Visible = false;
                    ShowError(GetString("Administration-User_List.ErrorGlobalAdmin"));
                    return;
                }

                if (ui != null)
                {
                    if (ui.GetValue("UserPassword") != null)
                    {
                        string password = ui.GetValue("UserPassword").ToString();
                        if (password.Length > 0)
                        {
                            passStrength.TextBoxAttributes.Add("value", hiddenPassword);
                            TextBoxConfirmPassword.Attributes.Add("value", hiddenPassword);
                        }
                    }
                }
            }
        }

        HeaderActions.AddAction(new HeaderAction()
        {
            Text = GetString("Administration-User_Edit_Password.gennew"),
            CommandName = GENERATEPASSWORD,
            OnClientClick = GetGeneratePasswordScript()
        });

        if (DisplaySendPaswd())
        {
            HeaderActions.AddAction(new HeaderAction()
            {
                Text = GetString("Administration-User_Edit_Password.sendpswd"),
                CommandName = SENDPASSWORD
            });
        }

        HeaderActions.ActionPerformed += HeaderActions_ActionPerformed;
    }


    /// <summary>
    /// Check whether current user is allowed to modify another user. Return "" or error message.
    /// </summary>
    /// <param name="userId">Modified user</param>
    protected string ValidateGlobalAndDeskAdmin()
    {
        string result = String.Empty;

        if (MembershipContext.AuthenticatedUser.IsGlobalAdministrator)
        {
            return result;
        }

        UserInfo userInfo = UserInfoProvider.GetUserInfo(UserID);
        if (userInfo == null)
        {
            result = GetString("Administration-User.WrongUserId");
        }
        else
        {
            if (userInfo.IsGlobalAdministrator)
            {
                result = GetString("Administration-User.NotAllowedToModify");
            }
        }
        return result;
    }


    #region "Event handlers"

    private void HeaderActions_ActionPerformed(object sender, CommandEventArgs e)
    {
        switch (e.CommandName)
        {
            case GENERATEPASSWORD:
                GenerateNewPassword();
                break;

            case SENDPASSWORD:
                SendPassword();
                break;
        }
    }


    /// <summary>
    /// Generates new password and sends it to the user.
    /// </summary>
    private void GenerateNewPassword()
    {
        // Check modify permission
        CheckModifyPermissions();

        string result = ValidateGlobalAndDeskAdmin();

        if (result == String.Empty)
        {
            string pswd = UserInfoProvider.GenerateNewPassword(SiteContext.CurrentSiteName);
            string userName = UserInfoProvider.GetUserNameById(UserID);
            UserInfoProvider.SetPassword(userName, pswd);

            // Show actual information to the user
            if (passStrength.Text != String.Empty)
            {
                passStrength.TextBoxAttributes.Add("value", hiddenPassword);
                TextBoxConfirmPassword.Attributes.Add("value", hiddenPassword);
            }
            else
            {
                passStrength.TextBoxAttributes.Add("value", "");
                TextBoxConfirmPassword.Attributes.Add("value", "");
            }

            ShowChangesSaved();

            // Process e-mail sending
            SendEmail(GetString("Administration-User_Edit_Password.NewGen"), pswd, UserID, "changed", true);

            ReloadPassword();
        }

        if (!String.IsNullOrEmpty(result))
        {
            ShowError(result);
        }
    }


    /// <summary>
    /// Sends the actual password of the current user.
    /// </summary>
    private void SendPassword()
    {
        // Check permissions
        CheckModifyPermissions();

        string result = ValidateGlobalAndDeskAdmin();

        if (result == String.Empty)
        {
            string pswd = UserInfoProvider.GetUserInfo(UserID).GetValue("UserPassword").ToString();

            // Process e-mail sending
            SendEmail(GetString("Administration-User_Edit_Password.Resend"), pswd, UserID, "RESEND", false);
        }

        if (!String.IsNullOrEmpty(result))
        {
            ShowError(result);
        }
    }


    /// <summary>
    /// Sets password of current user.
    /// </summary>
    protected void ButtonSetPassword_Click(object sender, EventArgs e)
    {
        // Check modify permission
        CheckModifyPermissions();

        string result = ValidateGlobalAndDeskAdmin();

        if ((result == String.Empty) && (ui != null))
        {
            if (TextBoxConfirmPassword.Text == passStrength.Text)
            {
                if (passStrength.IsValid())
                {
                    if (passStrength.Text != hiddenPassword) //password has been changed
                    {
                        string pswd = passStrength.Text;
                        UserInfoProvider.SetPassword(ui, passStrength.Text);

                        // Show actual information to the user
                        if (passStrength.Text != String.Empty)
                        {
                            passStrength.TextBoxAttributes.Add("value", hiddenPassword);
                            TextBoxConfirmPassword.Attributes.Add("value", hiddenPassword);
                        }
                        else
                        {
                            passStrength.TextBoxAttributes.Add("value", "");
                            TextBoxConfirmPassword.Attributes.Add("value", "");
                        }

                        ShowChangesSaved();

                        if (chkSendEmail.Checked)
                        {
                            // Process e-mail sending
                            SendEmail(GetString("Administration-User_Edit_Password.Changed"), pswd, UserID, "CHANGED", false);
                        }
                    }
                }
                else
                {
                    result = AuthenticationHelper.GetPolicyViolationMessage(SiteContext.CurrentSiteName);
                }
            }
            else
            {
                result = GetString("Administration-User_Edit_Password.PasswordsDoNotMatch");
            }
        }

        if (!String.IsNullOrEmpty(result))
        {
            ShowError(result);
        }
    }

    #endregion


    #region "Private methods"

    /// <summary>
    /// Loads the user password to the password fields.
    /// </summary>
    private void ReloadPassword()
    {
        UserInfo ui = UserInfoProvider.GetUserInfo(UserID);
        if (ui != null)
        {
            string passwd = ui.GetValue("UserPassword").ToString();
            if (!string.IsNullOrEmpty(passwd))
            {
                passStrength.TextBoxAttributes.Add("value", hiddenPassword);
                TextBoxConfirmPassword.Attributes.Add("value", hiddenPassword);
            }
        }
    }


    /// <summary>
    /// Sends e-mail with password if required.
    /// </summary>
    /// <param name="pswd">Password to send</param>
    /// <param name="toEmail">E-mail address of the mail recipient</param>
    /// <param name="subject">Subject of the e-mail sent</param>
    /// <param name="emailType">Type of the e-mail specificating the template used (NEW, CHANGED, RESEND)</param>
    /// <param name="showPassword">Indicates whether password is shown in message.</param>
    private void SendEmail(string subject, string pswd, int userId, string emailType, bool showPassword)
    {
        // Check whether the 'From' element was specified
        string emailFrom = SettingsKeyInfoProvider.GetStringValue(SiteContext.CurrentSiteName + ".CMSSendPasswordEmailsFrom");
        bool fromMissing = string.IsNullOrEmpty(emailFrom);

        if ((!string.IsNullOrEmpty(emailType)) && (ui != null) && (!fromMissing))
        {
            if (!string.IsNullOrEmpty(ui.Email))
            {
                EmailMessage em = new EmailMessage();

                em.From = emailFrom;
                em.Recipients = ui.Email;
                em.Subject = subject;
                em.EmailFormat = EmailFormatEnum.Default;

                string templateName = null;

                // Get e-mail template name
                switch (emailType.ToLowerCSafe())
                {
                    case "new":
                        templateName = "Membership.NewPassword";
                        break;

                    case "changed":
                        templateName = "Membership.ChangedPassword";
                        break;

                    case "resend":
                        templateName = "Membership.ResendPassword";
                        break;

                    default:
                        break;
                }

                // Get template info object
                if (templateName != null)
                {
                    try
                    {
                        // Get e-mail template
                        EmailTemplateInfo template = EmailTemplateProvider.GetEmailTemplate(templateName, null);
                        if (template != null)
                        {
                            em.Body = template.TemplateText;

                            // Macros
                            string[,] macros = new string[2, 2];
                            macros[0, 0] = "UserName";
                            macros[0, 1] = ui.UserName;
                            macros[1, 0] = "Password";
                            macros[1, 1] = pswd;
                            // Create macro resolver
                            MacroResolver resolver = MacroContext.CurrentResolver;
                            resolver.SetNamedSourceData(macros);

                            // Add template attachments
                            EmailHelper.ResolveMetaFileImages(em, template.TemplateID, EmailTemplateInfo.OBJECT_TYPE, ObjectAttachmentsCategories.TEMPLATE);
                            // Send message immediately (+ resolve macros)
                            EmailSender.SendEmailWithTemplateText(SiteContext.CurrentSiteName, em, template, resolver, true);

                            // Inform on success
                            ShowConfirmation(GetString("Administration-User_Edit_Password.PasswordsSent") + " " + HTMLHelper.HTMLEncode(ui.Email));

                            return;
                        }
                    }
                    catch (Exception ex)
                    {
                        // Log the error to the event log
                        EventLogProvider.LogException("Password retrieval", "USERPASSWORD", ex);
                        ShowError("Failed to send the password: " + ex.Message);
                    }
                }
            }
            else
            {
                // Inform on error
                if (showPassword)
                {
                    ShowConfirmation(String.Format(GetString("Administration-User_Edit_Password.passshow"), pswd), true);
                }
                else
                {
                    ShowConfirmation(GetString("Administration-User_Edit_Password.PassChangedNotSent"));
                }

                return;
            }
        }

        // Inform on error
        string errorMessage = GetString("Administration-User_Edit_Password.PasswordsNotSent");

        if (fromMissing)
        {
            ShowError(errorMessage + " " + GetString("Administration-User_Edit_Password.FromMissing"));
        }
        else
        {
            ShowError(errorMessage);
        }
    }


    /// <summary>
    /// Indicates whether the 'Send password' button should be enabled or not.
    /// </summary>
    private bool DisplaySendPaswd()
    {
        if (ui == null)
        {
            ui = UserInfoProvider.GetUserInfo(UserID);
        }

        if (ui != null)
        {
            // Password is stored in plain text, allow sending
            if (string.IsNullOrEmpty(ui.UserPasswordFormat) && !string.IsNullOrEmpty(ui.Email))
            {
                return true;
            }

            // Set hide action if user extend validity of his own account
            if (ui.UserID == MembershipContext.AuthenticatedUser.UserID)
            {
                ButtonSetPassword.OnClientClick += "window.top.HideWarning()";
            }
        }

        return false;
    }


    /// <summary>
    /// Decides whether enable generate new password e-mail. 
    /// </summary>
    private string GetGeneratePasswordScript()
    {
        string clientClick = null;

        if (ui == null)
        {
            ui = UserInfoProvider.GetUserInfo(UserID);
        }

        if (ui != null)
        {
            if (string.IsNullOrEmpty(ui.Email))
            {
                clientClick = "var flag = confirm('" + GetString("user.showpasswarning") + "');" + ((ui.UserID == MembershipContext.AuthenticatedUser.UserID) ? "if(flag) {window.top.HideWarning();}" : "") + "return flag;";
            }
            // Set hide action if user extend validity of his own account
            else if (ui.UserID == MembershipContext.AuthenticatedUser.UserID)
            {
                clientClick += "window.top.HideWarning()";
            }
        }

        return clientClick;
    }


    /// <summary>
    /// Checks if the user is allowed to perform this action.
    /// </summary>
    private void CheckModifyPermissions()
    {
        // Check "modify" permission
        if (!MembershipContext.AuthenticatedUser.IsAuthorizedPerResource("CMS.Users", "Modify"))
        {
            RedirectToAccessDenied("CMS.Users", "Modify");
        }
    }

    #endregion
}