﻿using System;
using System.Web.UI.WebControls;

using CMS.Base;
using CMS.DataEngine;
using CMS.Ecommerce;
using CMS.ExtendedControls;
using CMS.ExtendedControls.ActionsConfig;
using CMS.Helpers;
using CMS.Membership;
using CMS.PortalEngine;
using CMS.SiteProvider;
using CMS.UIControls;

public partial class CMSModules_Ecommerce_Controls_UI_Customers_CustomerLoginDetails : CMSAdminControl
{
    #region "Variables"

    private CustomerInfo mCustomer;
    private const string HIDDEN_PASSWORD = "********";
    private const string TEMPLATE_REGISTER_AND_NOTIFY = "Ecommerce.AutomaticRegistration";
    private const string TEMPLATE_PASSWORD_CHANGED = "Membership.ChangedPassword";
    
    #endregion


    #region "Properties"

    /// <summary>
    /// Customer for login details.
    /// </summary>
    private CustomerInfo Customer
    {
        get
        {
            if (mCustomer == null)
            {
                var customerId = QueryHelper.GetInteger("customerid", 0);
                mCustomer = CustomerInfoProvider.GetCustomerInfo(customerId);

                if ((mCustomer != null) && !mCustomer.CustomerIsRegistered && (mCustomer.CustomerSiteID <= 0))
                {
                    mCustomer.CustomerSiteID = SiteContext.CurrentSiteID;
                }
            }

            return mCustomer;
        }
    }


    /// <summary>
    /// Password field wrapper.
    /// </summary>
    public string PasswordField
    {
        get
        {
            return passStrength.Text;
        }
    }

    #endregion


    #region "Life cycle"

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!ECommerceContext.CheckCustomerSiteID(Customer))
        {
            EditedObject = null;
            return;
        }
        
        // Show info/error message after redirect.
        if (!RequestHelper.IsPostBack() && QueryHelper.GetBoolean("saved", false))
        {
            if (QueryHelper.GetBoolean("error", false))
            {
                ShowConfirmation(GetString("com.customer.logincreated"));
                ShowError(GetString("com.customer.notificationemailnotsent"));
            }
            else
            {
                ShowConfirmation(GetString("com.customer.logincreated") + " " + GetString("com.customer.notificationsent"));
            }
        }

        if (Customer.CustomerIsRegistered)
        {
            // Fill password text boxes with password hidden value
            passStrength.TextBoxAttributes.Add("value", HIDDEN_PASSWORD);
            txtPassword2.Attributes.Add("value", HIDDEN_PASSWORD);

            txtUserName.Text = Customer.CustomerUser.UserName;

            HeaderActions.AddHeaderAction(new SaveAction(PageHelper.CurrentPage));
            HeaderActions.AddHeaderAction(new HeaderAction
            {
                Text = GetString("com.customer.generatepassword"),
                CommandName = "generate",
                ButtonStyle = ButtonStyle.Default,
                OnClientClick = "return confirm(" + ScriptHelper.GetLocalizedString("com.customer.passwordchange") + ");"
            });
        }
        else
        {
            txtUserName.Text = Customer.CustomerEmail;

            HeaderActions.AddHeaderAction(new HeaderAction
            {
                Text = GetString("com.customer.createlogin"),
                CommandName = "create"
            });
        }

        HeaderActions.ActionPerformed += HeaderActions_ActionPerformed;
    }

    #endregion


    #region "Event handlers"

    protected void HeaderActions_ActionPerformed(object sender, CommandEventArgs e)
    {
        CheckModifyPermissions();

        switch (e.CommandName)
        {
            // Registering user with random password
            case "create":
                {
                    var errorMessage = ValidateForm();
                    if (!String.IsNullOrEmpty(ValidateForm()))
                    {
                        ShowError(errorMessage);
                        return;
                    }

                    var displayError = false;
                    if (!CustomerInfoProvider.RegisterAndNotify(Customer, TEMPLATE_REGISTER_AND_NOTIFY, Customer.CustomerEmail, PasswordField))
                    {
                        if (Customer.CustomerUserID <= 1)
                        {
                            ShowError(GetString("com.customer.usernotcreated"));
                            return;
                        }

                        displayError = true;
                    }

                    ScriptHelper.RegisterClientScriptBlock(Page, typeof(string), "LoginRedirect", "parent.window.location='" + GetRedirectUrl(displayError) + "';", true);
                }

                break;

            // Changing password to registered customer
            case ComponentEvents.SAVE:
                {
                    var errorMessage = ValidateForm();
                    if (!String.IsNullOrEmpty(errorMessage))
                    {
                        ShowError(errorMessage);
                        return;
                    }

                    SavePassword(GetString("com.customer.passwordsaved"), PasswordField);

                    break;
                }

            // Generate new random password and send it to customer
            case "generate":
                SavePassword(GetString("com.customer.passwordgenerated"));

                break;
        }
    }

    #endregion


    #region "Private methods"

    /// <summary>
    /// Generates url to parent element.
    /// </summary>
    /// <param name="displayError">Decide if error message should be displayed after redirection</param>
    private string GetRedirectUrl(bool displayError)
    {
        string customerID = Customer.CustomerID.ToString();
        string url = UIContextHelper.GetElementUrl("cms.ecommerce", "editcustomersproperties", false, customerID.ToInteger(0));
        url = URLHelper.AddParameterToUrl(url, "customerid", customerID);
        url = URLHelper.AddParameterToUrl(url, "saved", "1");

        if (displayError)
        {
            url = URLHelper.AddParameterToUrl(url, "error", "true");
        }

        return url;
    }


    /// <summary>
    /// Saves password to existing user and sends notification email. 
    /// </summary>
    /// <param name="informationMessage">Message to be displayed in password has been successfully changed.</param>
    /// <param name="password">If is not given, a new password will be generated.</param>
    private void SavePassword(string informationMessage, string password = null)
    {
        var user = Customer.CustomerUser;
        if (user == null)
        {
            return;
        }

        if (String.IsNullOrEmpty(password))
        {
            password = UserInfoProvider.GenerateNewPassword();
        }

        UserInfoProvider.SetPassword(user, password, true);

        if (!CustomerInfoProvider.SendLoginDetailsNotificationEmail(user, password, TEMPLATE_PASSWORD_CHANGED, SiteContext.CurrentSiteID))
        {
            ShowConfirmation(informationMessage);
            ShowError(GetString("com.customer.notificationemailnotsent"));
        }
        else
        {
            ShowConfirmation(informationMessage + " " + GetString("com.customer.notificationsent"));
        }
    }


    /// <summary>
    /// Validates password fields in the form.
    /// </summary>
    /// <returns>Error message</returns>
    private string ValidateForm()
    {
        string errorMessage = "";

        if (String.IsNullOrEmpty(PasswordField))
        {
            errorMessage = GetString("com.customer.enterpassword");
        }
        else if (!PasswordField.EqualsCSafe(txtPassword2.Text))
        {
            errorMessage = GetString("com.customer.passwordsdontmatch");
        }
        else if (!passStrength.IsValid())
        {
            errorMessage = AuthenticationHelper.GetPolicyViolationMessage(SiteContext.CurrentSiteName);
        }
        else if (PasswordField == HIDDEN_PASSWORD)
        {
            errorMessage = GetString("com.customer.changepassword");
        }
        else if ((Customer != null) && (Customer.CustomerUserID <= 0) && !UserInfoProvider.IsEmailUnique(Customer.CustomerEmail, new SiteInfoIdentifier(Customer.CustomerSiteID), 0))
        {
            errorMessage = string.Format(GetString("com.customer.emailnotunique"), Customer.CustomerEmail);
        }
        else if ((Customer != null) && !ValidationHelper.IsUserName(Customer.CustomerEmail))
        {
            errorMessage = string.Format(GetString("com.customer.invalidemailusername"), Customer.CustomerEmail);
        }
        else if ((Customer != null) && string.IsNullOrEmpty(Customer.CustomerEmail))
        {
            errorMessage = GetString("com.customer.noemail");
        }

        return errorMessage;
    }


    /// <summary>
    /// Check if user is allowed to modify user and customer.
    /// </summary>
    private void CheckModifyPermissions()
    {
        // Check permission
        if (!MembershipContext.AuthenticatedUser.IsAuthorizedPerResource("CMS.Users", "Modify"))
        {
            RedirectToAccessDenied("CMS.Users", "Modify");
        }
        if (!ECommerceContext.IsUserAuthorizedToModifyCustomer())
        {
            RedirectToAccessDenied("CMS.Ecommerce", "EcommerceModify OR ModifyCustomers");
        }
    }

    #endregion
}
