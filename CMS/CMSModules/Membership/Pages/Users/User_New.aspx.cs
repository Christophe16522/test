using System;
using System.Web.UI.WebControls;

using CMS.FormEngine;
using CMS.Helpers;
using CMS.Base;
using CMS.PortalEngine;
using CMS.SiteProvider;
using CMS.Membership;
using CMS.UIControls;
using CMS.DataEngine;

public partial class CMSModules_Membership_Pages_Users_User_New : CMSUsersPage
{
    #region "Variables"

    private String userName = String.Empty;
    private bool error = false;

    #endregion


    #region "Public methods"

    /// <summary>
    /// Shows the specified error message, optionally with a tooltip text.
    /// </summary>
    /// <param name="text">Error message text</param>
    /// <param name="description">Additional description</param>
    /// <param name="tooltipText">Tooltip text</param>
    /// <param name="persistent">Indicates if the message is persistent</param>
    public override void ShowError(string text, string description = null, string tooltipText = null, bool persistent = true)
    {
        base.ShowError(text, description, tooltipText, persistent);
        error = true;
    }

    #endregion


    protected void Page_Load(object sender, EventArgs e)
    {
        // Check "modify" permission
        if (!CurrentUser.IsAuthorizedPerResource("CMS.Users", "Modify"))
        {
            RedirectToAccessDenied("CMS.Users", "Modify");
        }

        ucUserName.UseDefaultValidationGroup = false;

        LabelConfirmPassword.Text = GetString("Administration-User_New.ConfirmPassword");
        LabelPassword.Text = GetString("Administration-User_New.Password");
        RequiredFieldValidatorFullName.ErrorMessage = GetString("Administration-User_New.RequiresFullName");

        if (!RequestHelper.IsPostBack())
        {
            CheckBoxEnabled.Checked = true;

            if (!CurrentUser.CheckPrivilegeLevel(UserPrivilegeLevelEnum.GlobalAdmin))
            {
                // Remove global and site admin options for non global admins.
                drpPrivilegeLevel.ExcludedValues = (int)UserPrivilegeLevelEnum.GlobalAdmin + ";" + (int)UserPrivilegeLevelEnum.Admin;
            }

            drpPrivilegeLevel.Value = (int)UserPrivilegeLevelEnum.Editor;
        }

        string users = GetString("general.users");
        string currentUser = GetString("Administration-User_New.CurrentUser");

        PageBreadcrumbs.Items.Add(new BreadcrumbItem()
        {
            Text = users,
            RedirectUrl = URLHelper.AppendQuery(UIContextHelper.GetElementUrl("CMS.Users", QueryHelper.GetString("ParentElem", "")), "displaytitle=false"),
            Target = "_parent"
        });

        PageBreadcrumbs.Items.Add(new BreadcrumbItem()
        {
            Text = currentUser
        });
    }


    protected void ButtonOK_Click(object sender, EventArgs e)
    {
        // Email format validation
        if ((TextBoxEmail.Text.Trim() != "") && (!ValidationHelper.IsEmail(TextBoxEmail.Text)))
        {
            ShowError(GetString("Administration-User_New.WrongEmailFormat"));
            return;
        }

        // Find whether user name is valid
        string result = null;
        if (!ucUserName.IsValid())
        {
            result = ucUserName.ValidationError;
        }

        // Additional validation
        if (String.IsNullOrEmpty(result))
        {
            result = new Validator().NotEmpty(TextBoxFullName.Text, GetString("Administration-User_New.RequiresFullName")).Result;
        }

        userName = ValidationHelper.GetString(ucUserName.Value, String.Empty);

        // If site prefixed allowed - add site prefix
        if ((SiteID != 0) && UserInfoProvider.UserNameSitePrefixEnabled(SiteContext.CurrentSiteName))
        {
            if (!UserInfoProvider.IsSitePrefixedUser(userName))
            {
                userName = UserInfoProvider.EnsureSitePrefixUserName(userName, SiteContext.CurrentSite);
            }
        }

        // Validation for site prefixed users
        if (!UserInfoProvider.IsUserNamePrefixUnique(userName, 0))
        {
            result = GetString("Administration-User_New.siteprefixeduserexists");
        }

        if (result == "")
        {
            if (TextBoxConfirmPassword.Text == passStrength.Text)
            {
                // Check whether password is valid according to policy
                if (passStrength.IsValid())
                {
                    if (UserInfoProvider.GetUserInfo(userName) == null)
                    {
                        int userId = SaveNewUser();
                        if (userId != -1)
                        {
                            var uiElementUrl = UIContextHelper.GetElementUrl("CMS.Users", QueryHelper.GetString("editelem", ""), false);
                            var url = URLHelper.AppendQuery(uiElementUrl, "siteid=" + SiteID + "&objectid=" + userId);
                            URLHelper.Redirect(url);
                        }
                    }
                    else
                    {
                        ShowError(GetString("Administration-User_New.UserExists"));
                    }
                }
                else
                {
                    ShowError(AuthenticationHelper.GetPolicyViolationMessage(SiteContext.CurrentSiteName));
                }
            }
            else
            {
                ShowError(GetString("Administration-User_Edit_Password.PasswordsDoNotMatch"));
            }
        }
        else
        {
            ShowError(result);
        }
    }


    /// <summary>
    /// Saves new user's data into DB.
    /// </summary>
    /// <returns>Returns ID of created user</returns>
    protected int SaveNewUser()
    {
        UserInfo ui = new UserInfo();

        // Load default values
        FormHelper.LoadDefaultValues("cms.user", ui);

        ui.PreferredCultureCode = "";
        ui.Email = TextBoxEmail.Text;
        ui.FirstName = "";
        ui.FullName = TextBoxFullName.Text;
        ui.LastName = "";
        ui.MiddleName = "";
        ui.UserName = userName;
        ui.Enabled = CheckBoxEnabled.Checked;
        ui.IsExternal = false;

        // Set privilege level, global admin may set all levels, rest only editor.
        UserPrivilegeLevelEnum privilegeLevel = (UserPrivilegeLevelEnum)drpPrivilegeLevel.Value.ToInteger(0);
        if (CurrentUser.CheckPrivilegeLevel(UserPrivilegeLevelEnum.GlobalAdmin)
            || (privilegeLevel == UserPrivilegeLevelEnum.None) || (privilegeLevel == UserPrivilegeLevelEnum.Editor))
        {
            ui.SetPrivilegeLevel(privilegeLevel);
        }

        // Check license limitations only in cmsdesk
        if (SiteID > 0)
        {
            string errorMessage = String.Empty;
            UserInfoProvider.CheckLicenseLimitation(ui, ref errorMessage);

            if (!String.IsNullOrEmpty(errorMessage))
            {
                ShowError(errorMessage);
            }
        }


        // Check whether email is unique if it is required
        if (!UserInfoProvider.IsEmailUnique(TextBoxEmail.Text.Trim(), SiteName, 0))
        {
            ShowError(GetString("UserInfo.EmailAlreadyExist"));
            return -1;
        }

        if (!error)
        {
            // Set password and save object
            UserInfoProvider.SetPassword(ui, passStrength.Text);

            // Add user to current site
            if (SiteID > 0)
            {
                UserInfoProvider.AddUserToSite(ui.UserName, SiteName);
            }

            return ui.UserID;
        }

        return -1;
    }
}