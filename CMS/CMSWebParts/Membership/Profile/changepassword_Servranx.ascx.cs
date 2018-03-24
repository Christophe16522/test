using System;

using CMS.CMSHelper;
using CMS.GlobalHelper;
using CMS.PortalControls;
using CMS.SiteProvider;
using CMS.SettingsProvider;
using CMS.EmailEngine;
using CMS.Membership;
using CMS.Helpers;
using CMS.MacroEngine;

public partial class CMSWebParts_Membership_Profile_changepassword_Servranx : CMSAbstractWebPart
{
     private UserInfo ui = null;
    #region "Public properties"

    /// <summary>
    /// Gets or sets the value that indicates whether this webpart is displayed only when user is authenticated.
    /// </summary>
    public bool ShowOnlyWhenAuthenticated
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("ShowOnlyWhenAuthenticated"), true);
        }
        set
        {
            SetValue("ShowOnlyWhenAuthenticated", value);
            Visible = (!value || MembershipContext.AuthenticatedUser.IsAuthenticated());
        }
    }


    /// <summary>
    /// Gets or sets the maximal new password length.
    /// </summary>
    public int MaximalPasswordLength
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("MaximalPasswordLength"), 0);
        }
        set
        {
            SetValue("MaximalPasswordLength", value);
            passStrength.MaxLength = value;
            txtConfirmPassword.MaxLength = value;
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
            // Do not process
        }
        else
        {
            Visible = (!ShowOnlyWhenAuthenticated || MembershipContext.AuthenticatedUser.IsAuthenticated());
            passStrength.MaxLength = MaximalPasswordLength;
            txtConfirmPassword.MaxLength = MaximalPasswordLength;

            // Set labels text
            wmOldPassword.WatermarkText = GetString("OldPassword");
            txtOldPassword.ToolTip = GetString("OldPassword");
            //lblNewPassword.Text = GetString("ChangePassword.lblNewPassword");
            txtConfirmPassword.ToolTip = GetString("ConfirmPassword"); 
            //lblNewPassword.Text = GetString("ChangePassword.lblNewPassword");
            wmConfirmPassword.WatermarkText = GetString("ConfirmPassword");
            //btnOk.Text = GetString("ChangePassword.btnOK");
            // WAI validation
            lblNewPassword.AssociatedControlClientID = passStrength.InputClientID;
        }
    }


    /// <summary>
    /// OnClick handler (Set password).
    /// </summary>
    protected void btnOk_Click(object sender, EventArgs e)
    {
        // Get current user info object
        CurrentUserInfo ui = MembershipContext.AuthenticatedUser;

        // Get current site info object
        CurrentSiteInfo si = SiteContext.CurrentSite;

        if ((ui != null) && (si != null))
        {
            string userName = ui.UserName;
            string siteName = si.SiteName;

            // new password correctly filled
            if (txtConfirmPassword.Text == passStrength.Text)
            {
                if (passStrength.IsValid())
                {
                    // Old password match
                    if(!UserInfoProvider.IsUserPasswordDifferent(ui, txtOldPassword.Text.Trim()))
                    {
                        UserInfoProvider.SetPassword(userName, passStrength.Text.Trim());
                        lblInfo.Visible = true;
                        lblInfo.Text = GetString("ChangePassword.ChangesSaved");
                        SendEmail();
                    }
                    else
                    {
                        lblError.Visible = true;
                        lblError.Text = GetString("ChangePassword.ErrorOldPassword");
                    }
                }
                else
                {
                    lblError.Visible = true;
                    lblError.Text = AuthenticationHelper.GetPolicyViolationMessage(SiteContext.CurrentSiteName);
                }
            }
            else
            {
                lblError.Visible = true;
                lblError.Text = GetString("ChangePassword.ErrorNewPassword");
            }
        }
    }
    private void SendEmail()
    {
        
        EmailMessage msg = new CMS.EmailEngine.EmailMessage();
        EmailTemplateInfo eti = EmailTemplateProvider.GetEmailTemplate("Membership.ChangedPassword", CMSContext.CurrentSiteID);
        string pswd = passStrength.Text.Trim();
              ui = UserInfoProvider.GetUserInfo(CurrentUser.UserName);
              if (ui != null)
              {
                  if (eti != null)
                  {
                      MacroResolver mcr = new MacroResolver();
                     
                      // Macros
                      string[,] macros = new string[5, 2];
                      macros[0, 0] = "UserName";
                      macros[0, 1] = ui.UserName;
                      macros[1, 0] = "Password";
                      macros[1, 1] = pswd;

                      ContextResolver resolver = MacroContext.CurrentResolver;
                      resolver.SourceParameters = macros;
                      resolver.EncodeResolvedValues = true;

                      msg.EmailFormat = EmailFormatEnum.Both;
                      msg.From = "info@servranx.com";
                      msg.Recipients = CurrentUser.Email;
                      msg.Subject = "Changement de mot de passe - Servranx";

                      EmailSender.SendEmailWithTemplateText(SiteContext.CurrentSiteName, msg, eti, resolver, true);

                  }

              }
        
    }
}