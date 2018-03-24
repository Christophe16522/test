using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.CMSHelper;
using CMS.Ecommerce;
using CMS.EcommerceProvider;
using CMS.EmailEngine;
using CMS.EventLog;
using CMS.GlobalHelper;
using CMS.PortalEngine;
using CMS.SettingsProvider;
using CMS.SiteProvider;
using CMS.WebAnalytics;

using TreeNode = CMS.DocumentEngine.TreeNode;
using CMS.Helpers;
using CMS.Membership;
using CMS.MacroEngine;
using CMS.Protection;
using CMS.DataEngine;
using CMS.DocumentEngine;

public partial class CMSModules_Ecommerce_Controls_ShoppingCart_ShoppingCartCheckRegistrationNew : ShoppingCartStep
{
    private bool mDataLoaded = false;

    /// <summary>
    /// '0' for Sign in using existing account.
    /// '1' for Create a new account.
    /// '2' for Continue as anonymous customer.
    /// </summary>
    public ShopingCartModeEnum RegistrationMode
    {
        get
        {
            if (radSignIn.Checked)
            {
                return ShopingCartModeEnum.ExistingAccount;
            }
            else if (radNewReg.Checked)
            {
                return ShopingCartModeEnum.NewAccount;
            }
            else
            {
                return ShopingCartModeEnum.AnonymousCustomer;
            }
        }
    }

    // Variables
    private bool mRequireOrgTaxRegIDs = false;
    private bool mShowTaxRegistrationIDField = false;
    private bool mShowOrganizationIDField = false;


    protected override void OnPreRender(EventArgs e)
    {
        // Prepare script for showing/hiding form
        string script = null;
        if (radSignIn.Checked)
        {
            script = ScriptHelper.GetScript("showHideForm('tblSignIn','" + radSignIn.ClientID + "');");
        }
        if (radNewReg.Checked)
        {
            script = ScriptHelper.GetScript("showHideForm('tblRegistration','" + radNewReg.ClientID + "');");
        }
       

        ScriptHelper.RegisterStartupScript(Page, typeof(string), "ShowHideFormInit", script);

        txtUsername.EnableAutoComplete = SecurityHelper.IsAutoCompleteEnabledForLogin(SiteContext.CurrentSiteName);

        base.OnPreRender(e);
    }


    /// <summary>
    /// On page load.
    /// </summary>
    protected void Page_Load(object sender, EventArgs e)
    {
        ScriptHelper.RegisterClientScriptBlock(this, GetType(), "showHide", ScriptHelper.GetScript(@"
            /* Shows and hides tables with forms*/
            function showHideForm(obj, rad)
            {
                var tblSignInStat = '';
                var tblRegistrationStat = '';
                var tblAnonymousStat = '';
                if( obj != null && obj != '' && rad != null)
                {
                    switch(obj)
                    {
                        case 'tblSignIn':
                            tblSignInStat = '';
                            tblRegistrationStat = 'none';
                            tblAnonymousStat = 'none';
                            break;

                        case 'tblRegistration':
                            tblSignInStat = 'none';
                            tblRegistrationStat = '';
                            tblAnonymousStat = 'none';
                            break;

                        case 'tblAnonymous':
                            tblSignInStat = 'none';
                            tblRegistrationStat = 'none';
                            tblAnonymousStat = '';
                            break;                
                    }

                    if(document.getElementById('tblSignIn') != null)
                        document.getElementById('tblSignIn').style.display = tblSignInStat;
                    if(document.getElementById('tblRegistration') != null)
                        document.getElementById('tblRegistration').style.display = tblRegistrationStat;
                    if(document.getElementById('tblAnonymous') != null)
                        document.getElementById('tblAnonymous').style.display = tblAnonymousStat;
                    if(document.getElementById(rad) != null)
                        document.getElementById(rad).setAttribute('checked','true');
                }
            }
            function showElem(id)
            {
                style = document.getElementById(id).style;
                style.display = (style.display == 'block')?'none':'block';
                return false;
            }
            function showHideChk(id)
            {
                var elem = document.getElementById(id);
                if(elem.style.display == 'block')
                {
                    elem.style.display = 'none';
                }
                else
                {
                    elem.style.display = 'block';
                }
            }"));

        // Get settings for current site
        SiteInfo si = SiteContext.CurrentSite;
        if (si != null)
        {
            mRequireOrgTaxRegIDs = ECommerceSettings.RequireCompanyInfo(si.SiteName);
            mShowOrganizationIDField = ECommerceSettings.ShowOrganizationID(si.SiteName);
            mShowTaxRegistrationIDField = ECommerceSettings.ShowTaxRegistrationID(si.SiteName);
        }

        PreRender += new EventHandler(CMSEcommerce_ShoppingCartCheckRegistration_PreRender);
        InitializeLabels();

        LoadStep(false);

        // Initialize onclick events
        radSignIn.Attributes.Add("onclick", "showHideForm('tblSignIn','" + radSignIn.ClientID + "');");
        radNewReg.Attributes.Add("onclick", "showHideForm('tblRegistration','" + radNewReg.ClientID + "');");
        //radAnonymous.Attributes.Add("onclick", "showHideForm('tblAnonymous','" + radAnonymous.ClientID + "');");
        lnkPasswdRetrieval.Attributes.Add("onclick", "return showElem('" + pnlPasswdRetrieval.ClientID + "');");
        //chkCorporateBody.Attributes.Add("onclick", "showHideChk('" + pnlCompanyAccount1.ClientID + "');");
        //chkEditCorpBody.Attributes.Add("onclick", "showHideChk('" + pnlCompanyAccount2.ClientID + "');");            
    }


    private void CMSEcommerce_ShoppingCartCheckRegistration_PreRender(object sender, EventArgs e)
    {
        if (!mDataLoaded && !ShoppingCartControl.IsCurrentStepPostBack)
        {
            LoadData();
        }
    }


    protected void LoadData()
    {
        mDataLoaded = true;
        // If user ID specified, load the given user ID
        if (!ShoppingCartControl.UserInfo.IsPublic())
        {
           
           
        }
    }


    /// <summary>
    /// Loads anonymous customer data from view state.
    /// </summary>
    protected void LoadAnonymousCustomerData()
    {
        if (ShoppingCart.Customer != null)
        {
         
        }
    }


    protected void LoadStep(bool loadData)
    {
        // If user logged in, edit the customer data
        if (!ShoppingCartControl.UserInfo.IsPublic())
        {
           
            plcAccount.Visible = false;
            lblTitle.Text = GetString("ShoppingCart.CheckRegistrationEdit");

            if (loadData)
            {
                LoadData();
            }
        }
        else
        {
            // Display/Hide the form for anonymous customer
            if (SiteContext.CurrentSite != null)
            {
                  }

            if (!ShoppingCartControl.IsCurrentStepPostBack)
            {
                // If anonymous customer data were already saved -> display them
                if ((ShoppingCart.ShoppingCartCustomerID > 0))
                {
                    // Mark 'Continue as anonymous customer' radio button
                   

                    LoadAnonymousCustomerData();
                }
                else
                {
                    // Mark 'Sign in using your existing account' radio button
                    radSignIn.Checked = true;
                }
            }

           
            plcAccount.Visible = true;

           
          

            lblTitle.Text = GetString("ShoppingCart.CheckRegistration");


            // Set strings
            lnkPasswdRetrieval.Text = GetString("LogonForm.lnkPasswordRetrieval");
            lblPasswdRetrieval.Text = GetString("LogonForm.lblPasswordRetrieval");
            btnPasswdRetrieval.Text = GetString("LogonForm.btnPasswordRetrieval");
            rqValue.ErrorMessage = GetString("LogonForm.rqValue");


            lnkPasswdRetrieval.Visible = ShoppingCartControl.EnablePasswordRetrieval;
            btnPasswdRetrieval.Click += new EventHandler(btnPasswdRetrieval_Click);

            pnlPasswdRetrieval.Attributes.Add("style", "display:none;");
            //this.pnlCompanyAccount1.Attributes.Add("style", "display:none;");
            //this.pnlCompanyAccount2.Attributes.Add("style", "display:none;");
        }

        //this.TitleText = GetString("Order_new.ShoppingCartCheckRegistration.Title");
    }


    /// <summary>
    /// Retrieve the user password.
    /// </summary>
    private void btnPasswdRetrieval_Click(object sender, EventArgs e)
    {
        string value = txtPasswordRetrieval.Text.Trim();
        if ((value != String.Empty) && (SiteContext.CurrentSite != null))
        {
            bool success;
            lblResult.Text = AuthenticationHelper.ForgottenEmailRequest(value, SiteContext.CurrentSiteName, "ECOMMERCE", ECommerceSettings.SendEmailsFrom(SiteContext.CurrentSite.SiteName), MacroContext.CurrentResolver, AuthenticationHelper.GetResetPasswordUrl(SiteContext.CurrentSiteName), out success);

            plcResult.Visible = true;
            plcErrorResult.Visible = false;

            pnlPasswdRetrieval.Attributes.Add("style", "display:block;");
        }
    }


    /// <summary>
    /// Initialization of labels.
    /// </summary>
    protected void InitializeLabels()
    {
       
       
        radSignIn.Text = GetString("ShoppingCartCheckRegistration.SignIn");
        lblUsername.Text = GetString("ShoppingCartCheckRegistration.Username");
        lblPsswd1.Text = GetString("ShoppingCartCheckRegistration.Psswd");
        radNewReg.Text = GetString("ShoppingCartCheckRegistration.NewReg");
        lblFirstName1.Text = GetString("ShoppingCartCheckRegistration.FirstName");
        lblLastName1.Text = GetString("ShoppingCartCheckRegistration.LastName");
        lblEmail2.Text = GetString("ShoppingCartCheckRegistration.EmailUsername");
        lblPsswd2.Text = lblPsswd1.Text;
        lblConfirmPsswd.Text = GetString("ShoppingCartCheckRegistration.ConfirmPsswd");
     


        //lblCorporateBody.Text = GetString("ShoppingCartCheckRegistration.lblCorporateBody");
    

        // Mark required fields
        if (ShoppingCartControl.RequiredFieldsMark != "")
        {
            string mark = ShoppingCartControl.RequiredFieldsMark;
            lblMark1.Text = mark;
            lblMark2.Text = mark;
            lblMark3.Text = mark;
            lblMark4.Text = mark;
            lblMark5.Text = mark;
            lblMark6.Text = mark;
            passStrength.RequiredFieldMark = mark;
            lblMark8.Text = mark;
   
           
       
        }
    }


    /// <summary>
    /// On chkCorporateBody checkbox checked changed.
    /// </summary>
  


    /// <summary>
    /// On chkCorporateBody2 checkbox checked changed.
    /// </summary>
 


    /// <summary>
    /// On chkEditCorpBody checkbox checked changed.
    /// </summary>
   


    /// <summary>
    /// Validate values in textboxes.
    /// </summary>
    public override bool IsValid()
    {
        Validator val = new Validator();
        string result = null;

        if (plcAccount.Visible)
        {
            // Validate registration data
            if (radSignIn.Checked)
            {
                ScriptHelper.RegisterStartupScript(this, GetType(), "checkSignIn", ScriptHelper.GetScript("showHideForm('tblSignIn','" + radSignIn.ClientID + "');"));

                // Check banned IP
                if (!BannedIPInfoProvider.IsAllowed(SiteContext.CurrentSiteName, BanControlEnum.Login))
                {
                    result = GetString("banip.ipisbannedlogin");
                }                

                // Check user name
                if (string.IsNullOrEmpty(result))
                {                    
                    result = val.NotEmpty(txtUsername.Text.Trim(), GetString("ShoppingCartCheckRegistration.ErrorMissingUsername")).Result;
                }

                if (!string.IsNullOrEmpty(result))
                {
                    lblError.Text = result;
                    lblError.Visible = true;
                    return false;
                }
            }
            // Check 'New registration' section
            else if (radNewReg.Checked)
            {
                ScriptHelper.RegisterStartupScript(this, GetType(), "checkRegistration", ScriptHelper.GetScript("showHideForm('tblRegistration','" + radNewReg.ClientID + "');"));

                // Check banned IP
                if (!BannedIPInfoProvider.IsAllowed(SiteContext.CurrentSiteName, BanControlEnum.Registration))
                {
                    result = GetString("banip.ipisbannedregistration");
                }

                if (string.IsNullOrEmpty(result) && !BannedIPInfoProvider.IsAllowed(SiteContext.CurrentSiteName, BanControlEnum.Login))
                {
                    result = GetString("banip.ipisbannedlogin");
                } 

                // Check registration form
                if (string.IsNullOrEmpty(result))
                {
                    result = val.NotEmpty(txtFirstName1.Text.Trim(), GetString("ShoppingCartCheckRegistration.FirstNameErr"))
                        .NotEmpty(txtLastName1.Text.Trim(), GetString("ShoppingCartCheckRegistration.LastNameErr"))
                        .NotEmpty(txtEmail2.Text.Trim(), GetString("ShoppingCartCheckRegistration.EmailErr"))
                        .NotEmpty(passStrength.Text.Trim(), GetString("ShoppingCartCheckRegistration.PsswdErr")).Result;
                }

               
                if (result == "")
                {
                    if (!ValidationHelper.IsEmail(txtEmail2.Text.Trim()))
                    {
                        lblEmail2Err.Text = GetString("ShoppingCartCheckRegistration.EmailErr");
                        lblEmail2Err.Visible = true;
                    }
                    // Password and confirmed password must be same
                    if (passStrength.Text != txtConfirmPsswd.Text)
                    {
                        lblPsswdErr.Text = GetString("ShoppingCartCheckRegistration.DifferentPsswds");
                        lblPsswdErr.Visible = true;
                    }

                    // Check policy
                    if (!passStrength.IsValid())
                    {
                        lblPsswdErr.Text = AuthenticationHelper.GetPolicyViolationMessage(SiteContext.CurrentSiteName);
                        lblPsswdErr.Visible = true;
                    }


                    if ((!DataHelper.IsEmpty(lblEmail2Err.Text.Trim())) || (!DataHelper.IsEmpty(lblPsswdErr.Text.Trim())))
                    {
                        return false;
                    }
                }
                else
                {
                    lblError.Text = result;
                    lblError.Visible = true;
                    return false;
                }
            }
          
        }
        else
        {
           
           
            if (result == "")
            {
                return true;
            }
            else
            {
                lblError.Text = result;
                lblError.Visible = true;
                return false;
            }
        }

        return true;
    }


    /// <summary>
    /// Process valid values of this step.
    /// </summary>
    public override bool ProcessStep()
    {
        if (plcAccount.Visible)
        {
            string siteName = SiteContext.CurrentSiteName;

            // Existing account
            if (radSignIn.Checked)
            {
                // Authenticate user
                UserInfo ui = AuthenticationHelper.AuthenticateUser(txtUsername.Text.Trim(), txtPsswd1.Text, SiteContext.CurrentSiteName, false);
                if (ui == null)
                {
                    lblError.Text = GetString("ShoppingCartCheckRegistration.LoginFailed");
                    lblError.Visible = true;
                    return false;
                }

                // Sign in customer with existing account
                CMSContext.AuthenticateUser(ui.UserName, false);

                // Registered user has already started shopping as anonymous user -> Drop his stored shopping cart
                ShoppingCartInfoProvider.DeleteShoppingCartInfo(ui.UserID, siteName);

                // Assign current user to the current shopping cart
                ShoppingCart.User = ui;

                // Save changes to database
                if (!ShoppingCartControl.IsInternalOrder)
                {
                    ShoppingCartInfoProvider.SetShoppingCartInfo(ShoppingCart);
                }

                // Log "login" activity
                ContactID = ModuleCommands.OnlineMarketingGetUserLoginContactID(ui);
                Activity activity = new ActivityUserLogin(ContactID, ui, DocumentContext.CurrentDocument, AnalyticsContext.ActivityEnvironmentVariables);
                activity.Log();

                LoadStep(true);

                // Return false to get to Edit customer page
                return false;
            }
            // New registration
            else if (radNewReg.Checked)
            {
                txtEmail2.Text = txtEmail2.Text.Trim();
               // pnlCompanyAccount1.Visible = chkCorporateBody.Checked;

                // Check if user exists
                UserInfo ui = UserInfoProvider.GetUserInfo(txtEmail2.Text);
                if (ui != null)
                {
                    lblError.Visible = true;
                    lblError.Text = GetString("ShoppingCartUserRegistration.ErrorUserExists");
                    return false;
                }

                // Check all sites where user will be assigned
                string checkSites = (String.IsNullOrEmpty(ShoppingCartControl.AssignToSites)) ? SiteContext.CurrentSiteName : ShoppingCartControl.AssignToSites;
                if (!UserInfoProvider.IsEmailUnique(txtEmail2.Text.Trim(), checkSites, 0))
                {
                    lblError.Visible = true;
                    lblError.Text = GetString("UserInfo.EmailAlreadyExist");
                    return false;
                }

                // Create new customer and user account and sign in
                // User
                ui = new UserInfo();
                ui.UserName = txtEmail2.Text.Trim();
                ui.Email = txtEmail2.Text.Trim();
                ui.FirstName = txtFirstName1.Text.Trim();
                ui.LastName = txtLastName1.Text.Trim();
                ui.FullName = ui.FirstName + " " + ui.LastName;
                ui.Enabled = true;
                ui.UserIsGlobalAdministrator = false;
                ui.UserURLReferrer = MembershipContext.AuthenticatedUser.URLReferrer;
                ui.UserCampaign = AnalyticsHelper.Campaign;
                ui.UserSettings.UserRegistrationInfo.IPAddress = RequestContext.UserHostAddress;
                ui.UserSettings.UserRegistrationInfo.Agent = HttpContext.Current.Request.UserAgent;

                try
                {
                    UserInfoProvider.SetPassword(ui, passStrength.Text);

                    string[] siteList;

                    // If AssignToSites field set
                    if (!String.IsNullOrEmpty(ShoppingCartControl.AssignToSites))
                    {
                        siteList = ShoppingCartControl.AssignToSites.Split(';');
                    }
                    else // If not set user current site 
                    {
                        siteList = new string[] { siteName };
                    }

                    foreach (string site in siteList)
                    {
                        UserInfoProvider.AddUserToSite(ui.UserName, site);

                        // Add user to roles
                        if (ShoppingCartControl.AssignToRoles != "")
                        {
                            AssignUserToRoles(ui.UserName, ShoppingCartControl.AssignToRoles, site);
                        }
                    }

                    // Log registered user
                    AnalyticsHelper.LogRegisteredUser(siteName, ui);

                    Activity activity = new ActivityRegistration(ui, DocumentContext.CurrentDocument, AnalyticsContext.ActivityEnvironmentVariables);
                    if (activity.Data != null)
                    {
                        activity.Data.ContactID = ModuleCommands.OnlineMarketingGetUserLoginContactID(ui);
                        activity.Log();
                    }
                }
                catch (Exception ex)
                {
                    lblError.Visible = true;
                    lblError.Text = ex.Message;
                    return false;
                }

                // Customer
                CustomerInfo ci = new CustomerInfo();
                ci.CustomerFirstName = txtFirstName1.Text.Trim();
                ci.CustomerLastName = txtLastName1.Text.Trim();
                ci.CustomerEmail = txtEmail2.Text.Trim();

                ci.CustomerCompany = "";
                ci.CustomerOrganizationID = "";
                ci.CustomerTaxRegistrationID = "";
               

                ci.CustomerUserID = ui.UserID;
                ci.CustomerSiteID = 0;
                ci.CustomerEnabled = true;
                ci.CustomerCreated = DateTime.Now;
                CustomerInfoProvider.SetCustomerInfo(ci);

                // Track successful registration conversion
                string name = ShoppingCartControl.RegistrationTrackConversionName;
                ECommerceHelper.TrackRegistrationConversion(ShoppingCart.SiteName, name);

                // Log "customer registration" activity and update profile
                var activityCustomerRegistration = new ActivityCustomerRegistration(ci, MembershipContext.AuthenticatedUser, AnalyticsContext.ActivityEnvironmentVariables);
                if (activityCustomerRegistration.Data != null)
                {
                    if (ContactID <= 0)
                    {
                        activityCustomerRegistration.Data.ContactID = ModuleCommands.OnlineMarketingGetUserLoginContactID(ui);
                    }
                    activityCustomerRegistration.Log();
                }

                // Sign in
                if (ui.UserEnabled)
                {
                    CMSContext.AuthenticateUser(ui.UserName, false);
                    ShoppingCart.User = ui;

                    ContactID = ModuleCommands.OnlineMarketingGetUserLoginContactID(ui);
                    Activity activity = new ActivityUserLogin(ContactID, ui, DocumentContext.CurrentDocument, AnalyticsContext.ActivityEnvironmentVariables);
                    activity.Log();
                }

                ShoppingCart.ShoppingCartCustomerID = ci.CustomerID;

                // Send new registration notification email
                if (ShoppingCartControl.SendNewRegistrationNotificationToAddress != "")
                {
                    SendRegistrationNotification(ui);
                }
            }
           
            else
            {
                return false;
            }
        }
        else
        {
            // Save the customer data
            bool newCustomer = false;
            CustomerInfo ci = CustomerInfoProvider.GetCustomerInfoByUserID(ShoppingCartControl.UserInfo.UserID);
            if (ci == null)
            {
                ci = new CustomerInfo();
                ci.CustomerUserID = ShoppingCartControl.UserInfo.UserID;
                ci.CustomerSiteID = 0;
                ci.CustomerEnabled = true;
                newCustomer = true;
            }

            // Old email address
            string oldEmail = ci.CustomerEmail.ToLowerCSafe();


            // Update customer data
            CustomerInfoProvider.SetCustomerInfo(ci);

            // Update corresponding user email when required
            if (oldEmail != ci.CustomerEmail.ToLowerCSafe())
            {
                UserInfo user = UserInfoProvider.GetUserInfo(ci.CustomerUserID);
                if (user != null)
                {
                    user.Email = ci.CustomerEmail;
                    UserInfoProvider.SetUserInfo(user);
                }
            }

            // Log "customer registration" activity and update contact profile
            if (newCustomer)
            {
                var activity = new ActivityCustomerRegistration(ci, MembershipContext.AuthenticatedUser, AnalyticsContext.ActivityEnvironmentVariables);
                activity.Log();
            }

            // Set the shopping cart customer ID
            ShoppingCart.ShoppingCartCustomerID = ci.CustomerID;
        }

        try
        {
            if (!ShoppingCartControl.IsInternalOrder)
            {
                ShoppingCartInfoProvider.SetShoppingCartInfo(ShoppingCart);
            }
            return true;
        }
        catch
        {
            return false;
        }
    }


    /// <summary>
    /// Adds user to role
    /// <param name="userName">User name</param>
    /// <param name="roles">Role names the user should be assign to. Role names are separated by the char of ';'</param>
    /// <param name="siteName">Site name</param>
    /// </summary>
    private void AssignUserToRoles(string userName, string roles, string siteName)
    {
        string[] roleList = roles.Split(';');
        if ((siteName != null) && (siteName != ""))
        {
            if ((roleList != null) && (roleList.Length > 0))
            {
                for (int i = 0; i < roleList.Length; i++)
                {
                    String roleName = roleList[i];
                    String sn = roleName.StartsWithCSafe(".") ? "" : siteName;

                    if (RoleInfoProvider.RoleExists(roleName, sn))
                    {
                        UserInfoProvider.AddUserToRole(userName, roleName, sn);
                    }
                }
            }
        }
    }


    /// <summary>
    /// Sends new registration notification e-mail to administrator.
    /// </summary>
    private void SendRegistrationNotification(UserInfo ui)
    {
        SiteInfo currentSite = SiteContext.CurrentSite;

        // Notify administrator
        if ((ui != null) && (currentSite != null) && (ShoppingCartControl.SendNewRegistrationNotificationToAddress != ""))
        {
            EmailTemplateInfo mEmailTemplate = null;
            if (!ui.UserEnabled)
            {
                mEmailTemplate = EmailTemplateProvider.GetEmailTemplate("Registration.Approve", currentSite.SiteName);
            }
            else
            {
                mEmailTemplate = EmailTemplateProvider.GetEmailTemplate("Registration.New", currentSite.SiteName);
            }

            EventLogProvider ev = new EventLogProvider();

            if (mEmailTemplate == null)
            {
                // Email template not exist
                ev.LogEvent("E", DateTime.Now, "RegistrationForm", "GetEmailTemplate", HTTPHelper.GetAbsoluteUri());
            }
            else
            {
                // Initialize email message
                EmailMessage message = new EmailMessage();
                message.EmailFormat = EmailFormatEnum.Default;

                message.From = EmailHelper.GetSender(mEmailTemplate, ECommerceSettings.SendEmailsFrom(currentSite.SiteName));
                message.Subject = GetString("RegistrationForm.EmailSubject");

                message.Recipients = ShoppingCartControl.SendNewRegistrationNotificationToAddress;
                message.Body = mEmailTemplate.TemplateText;

                // Init macro resolving
                string[,] replacements = new string[4, 2];
                replacements[0, 0] = "firstname";
                replacements[0, 1] = ui.FirstName;
                replacements[1, 0] = "lastname";
                replacements[1, 1] = ui.LastName;
                replacements[2, 0] = "email";
                replacements[2, 1] = ui.Email;
                replacements[3, 0] = "username";
                replacements[3, 1] = ui.UserName;

                ContextResolver resolver = MacroContext.CurrentResolver;
                resolver.SourceParameters = replacements;

                try
                {
                    // Add template metafiles to e-mail
                    MetaFileInfoProvider.ResolveMetaFileImages(message, mEmailTemplate.TemplateID, EmailObjectType.EMAILTEMPLATE, MetaFileInfoProvider.OBJECT_CATEGORY_TEMPLATE);
                    // Send e-mail
                    EmailSender.SendEmailWithTemplateText(currentSite.SiteName, message, mEmailTemplate, resolver, false);
                }
                catch
                {
                    // Email sending failed
                    ev.LogEvent("E", DateTime.Now, "Membership", "RegistrationEmail", SiteContext.CurrentSite.SiteID);
                }
            }
        }
    }

    protected void OnBtnSubmitClick(object sender, EventArgs e)
    {
        if (IsValid())
        {
            EventLogProvider.LogInformation("reg submit - next click action", "I");
            this.ButtonNextClickAction();
        }
    }
}