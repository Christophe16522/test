﻿using System;
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
using CMS.MembershipProvider;

using TreeNode = CMS.DocumentEngine.TreeNode;
using System.Web.Security;
using CMS.DataEngine;
using System.Text;
using CMS.Helpers;
using CMS.Membership;
using CMS.MacroEngine;
using CMS.DocumentEngine;
using CMS.Localization;


public partial class CMSModules_Ecommerce_Controls_ShoppingCart_ShoppingCartCheckRegistration : ShoppingCartStep
{
    #region "Private properties"

    private string mDefaultTargetUrl = "";

    #endregion

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

    /// <summary>
    /// Gets or sets the value that indicates whether user is enabled after registration.
    /// </summary>
    public bool EnableUserAfterRegistration
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("EnableUserAfterRegistration"), true);
        }
        set
        {
            SetValue("EnableUserAfterRegistration", value);
        }
    }

    /// <summary>
    /// Gets or sets the roles where is user assigned after successful registration.
    /// </summary>
    public string AssignRoles
    {
        get
        {
            return ValidationHelper.GetString(GetValue("AssignToRoles"), "");
        }
        set
        {
            SetValue("AssignToRoles", value);
        }
    }

    /// <summary>
    /// Gets or sets the default starting alias path for newly registered user.
    /// </summary>
    public string StartingAliasPath
    {
        get
        {
            return ValidationHelper.GetString(GetValue("StartingAliasPath"), "");
        }
        set
        {
            SetValue("StartingAliasPath", value);
        }
    }

    /// <summary>
    /// Gets or sets the sites where is user assigned after successful registration.
    /// </summary>
    public string AssignToSites
    {
        get
        {
            return ValidationHelper.GetString(GetValue("AssignToSites"), "");
        }
        set
        {
            SetValue("AssignToSites", value);
        }
    }

    private bool mDataLoaded = false;

    private const string INVITE_LOGIN = "ADRESSE E-MAIL";
    private const string INVITE_LOGIN_US = "E-MAIL ADDRESS";
    private const string INVITE_PASSWORD = "MOT DE PASSE";
    private const string INVITE_PASSWORD_US = "PASSWORD";
    private const string INVITE_CONFIRM_PASSWORD = "CONFIRMATION MOT DE PASSE";
    private const string INVITE_CONFIRM_PASSWORD_US = "CONFIRM PASSWORD";
    private const string INVITE_FIRSTNAME = "PRENOM";
    private const string INVITE_FIRSTNAME_US = "FIRSTNAME";
    private const string INVITE_LASTNAME = "NOM";
    private const string INVITE_LASTNAME_US = "LASTNAME";
    private const string INVITE_EMAIL = "ADRESSE E-MAIL";
    private const string INVITE_EMAIL_US = "E-MAIL ADDRESS";
    private const string INVITE_ADRESSE = "ADRESSE";
    private const string INVITE_ADRESSE_US = "ADDRESS";
    private const string INVITE_CP = "CODE POSTALE";
    private const string INVITE_CP_US = "ZIP";
    private const string INVITE_VILLE = "VILLE";
    private const string INVITE_VILLE_US = "CITY";
    private const string INVITE_CIVILITE = "Civilit�";
    private const string INVITE_CIVILITE_US = "Civility";

    private bool _isExistingAccount = false;
    private bool _isNewAccount = false;

    private bool IsDefaultOrEmptyLogin()
    {
        return String.IsNullOrEmpty(txtLogin.Text) || txtLogin.Text.Equals(INVITE_LOGIN) || txtLogin.Text.Equals(INVITE_LOGIN_US);
    }

    private bool IsDefaultOrEmptyPassword()
    {
        return String.IsNullOrEmpty(txtMotDePasse.Text) || txtMotDePasse.Text.Equals(INVITE_PASSWORD) || txtMotDePasse.Text.Equals(INVITE_PASSWORD_US);
    }

    private bool IsExistingAccount()
    {
        return (!(IsDefaultOrEmptyLogin() || IsDefaultOrEmptyPassword())) || _isExistingAccount;
    }

    private bool IsDefaultOrEmptyFirstName()
    {
        return String.IsNullOrEmpty(txtFirstName.Text) || txtFirstName.Text.Equals(INVITE_FIRSTNAME) || txtFirstName.Text.Equals(INVITE_FIRSTNAME_US);   
    }
    private bool IsDefaultOrEmptyLastName()
    {
        return String.IsNullOrEmpty(txtLastName.Text) || txtLastName.Text.Equals(INVITE_LASTNAME) || txtLastName.Text.Equals(INVITE_LASTNAME_US);
    }
    private bool IsDefaultOrEmptyEmail()
    {
        return String.IsNullOrEmpty(txtEmailRegistration.Text) || txtEmailRegistration.Text.Equals(INVITE_EMAIL) || txtEmailRegistration.Text.Equals(INVITE_EMAIL_US);
    }

    private bool IsDefaultOrEmptyNewPassword()
    {
        return String.IsNullOrEmpty(txtMotDePasseRegistration.Text) || txtMotDePasseRegistration.Text.Equals(INVITE_PASSWORD) || txtMotDePasseRegistration.Text.Equals(INVITE_PASSWORD_US);
    }

    private bool IsDefaultOrEmptyConfirmNewPassword()
    {
        return String.IsNullOrEmpty(txtMotDePasseConfirmation.Text) || txtMotDePasseConfirmation.Text.Equals(INVITE_CONFIRM_PASSWORD) || txtMotDePasseConfirmation.Text.Equals(INVITE_CONFIRM_PASSWORD_US);
    }

    private bool IsNewAccount()
    {
        return (!(IsDefaultOrEmptyFirstName() || IsDefaultOrEmptyLastName() || IsDefaultOrEmptyEmail() || IsDefaultOrEmptyNewPassword() || IsDefaultOrEmptyConfirmNewPassword())) || _isNewAccount;
        //return (IsDefaultOrEmptyFirstName() || IsDefaultOrEmptyLastName() || IsDefaultOrEmptyEmail() || IsDefaultOrEmptyNewPassword() || IsDefaultOrEmptyConfirmNewPassword()) || _isNewAccount;
    }

    /// <summary>
    /// '0' for Sign in using existing account.
    /// '1' for Create a new account.
    /// '2' for Continue as anonymous customer.
    /// </summary>
    public ShopingCartModeEnum RegistrationMode
    {
        //get
        //{
        //    if (radSignIn.Checked)
        //    {
        //        return ShopingCartModeEnum.ExistingAccount;
        //    }
        //    else if (radNewReg.Checked)
        //    {
        //        return ShopingCartModeEnum.NewAccount;
        //    }
        //    else
        //    {
        //        return ShopingCartModeEnum.AnonymousCustomer;
        //    }
        //}
        get
        {
            if (txtLogin.Text != String.Empty)
            {
                return ShopingCartModeEnum.ExistingAccount;
            }
            else if (txtFirstName.Text != String.Empty)
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
        if (radAnonymous.Checked)
        {
            script = ScriptHelper.GetScript("showHideForm('tblAnonymous','" + radAnonymous.ClientID + "');");
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
        InitializeLabels();       
        ShowBillingCountryList();	
        ShowCivility();
        ShowShippingCountryList();        
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
            // Get the customer data
            CustomerInfo ci = CustomerInfoProvider.GetCustomerInfoByUserID(ShoppingCartControl.UserInfo.UserID);

            // Set the fields
            if (ci != null)
            {
                txtEditCompany.Text = ci.CustomerCompany;
                txtEditEmail.Text = ci.CustomerEmail;
                txtEditFirst.Text = ci.CustomerFirstName;
                txtEditLast.Text = ci.CustomerLastName;
                txtEditOrgID.Text = ci.CustomerOrganizationID;
                txtEditTaxRegID.Text = ci.CustomerTaxRegistrationID;

                if (!DataHelper.IsEmpty(txtEditCompany.Text.Trim()) || !DataHelper.IsEmpty(txtEditOrgID.Text.Trim()) || !DataHelper.IsEmpty(txtEditTaxRegID.Text.Trim()))
                {
                    chkEditCorpBody.Checked = true;
                    pnlCompanyAccount2.Visible = true;
                }
            }
            else
            {
                txtEditFirst.Text = ShoppingCartControl.UserInfo.FirstName;
                txtEditLast.Text = ShoppingCartControl.UserInfo.LastName;
                txtEditEmail.Text = ShoppingCartControl.UserInfo.Email;
            }
        }
    }


    /// <summary>
    /// Loads anonymous customer data from view state.
    /// </summary>
    protected void LoadAnonymousCustomerData()
    {
        if (ShoppingCart.Customer != null)
        {
            txtFirstName2.Text = ShoppingCart.Customer.CustomerFirstName;
            txtLastName2.Text = ShoppingCart.Customer.CustomerLastName;
            txtEmail3.Text = ShoppingCart.Customer.CustomerEmail;
            txtCompany2.Text = ShoppingCart.Customer.CustomerCompany;
            txtOrganizationID2.Text = ShoppingCart.Customer.CustomerOrganizationID;
            txtTaxRegistrationID2.Text = ShoppingCart.Customer.CustomerTaxRegistrationID;

            if (!string.IsNullOrEmpty(txtCompany2.Text))
            {
                chkCorporateBody2.Checked = true;
                plcCompanyAccount3.Visible = true;
            }
        }
    }


    protected void LoadStep(bool loadData)
    {
        // If user logged in, edit the customer data
        if (!ShoppingCartControl.UserInfo.IsPublic())
        {
            plcEditCustomer.Visible = true;
            plcEditOrgID.Visible = mShowOrganizationIDField;
            plcEditTaxRegID.Visible = mShowTaxRegistrationIDField;
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
                plhAnonymous.Visible = ECommerceSettings.AllowAnonymousCustomers(SiteContext.CurrentSite.SiteName);
            }

            if (!ShoppingCartControl.IsCurrentStepPostBack)
            {
                // If anonymous customer data were already saved -> display them
                if ((plhAnonymous.Visible) && (ShoppingCart.ShoppingCartCustomerID > 0))
                {
                    // Mark 'Continue as anonymous customer' radio button
                    radAnonymous.Checked = true;

                    LoadAnonymousCustomerData();
                }
                else
                {
                    // Mark 'Sign in using your existing account' radio button
                    radSignIn.Checked = true;
                }
            }

            plcEditCustomer.Visible = false;
            plcAccount.Visible = true;

            plcTaxRegistrationID.Visible = mShowTaxRegistrationIDField;
            plcOrganizationID.Visible = mShowOrganizationIDField;

            plcTaxRegistrationID2.Visible = mShowTaxRegistrationIDField;
            plcOrganizationID2.Visible = mShowOrganizationIDField;

            lblTitle.Text = GetString("ShoppingCart.CheckRegistration");


            // Set strings
            lnkPasswdRetrieval.Text = GetString("LogonForm.lnkRetrieval");
            lblPasswdRetrieval.Text = GetString("LogonForm.lblPasswordRetrieval");
            btnPasswdRetrieval.Text = GetString("LogonForm.btnPasswordRetrieval");
            rqValue.ErrorMessage = GetString("LogonForm.rqValue");


            lnkPasswdRetrieval.Visible = ShoppingCartControl.EnablePasswordRetrieval;
            btnPasswdRetrieval.Click += new EventHandler(btnPasswdRetrieval_Click);

            pnlPasswdRetrieval.Attributes.Add("style", "display:none;");
            this.pnlCompanyAccount1.Attributes.Add("style", "display:none;");
            this.pnlCompanyAccount2.Attributes.Add("style", "display:none;");
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
            lblResult.Text = AuthenticationHelper.ForgottenEmailRequest(value, SiteContext.CurrentSiteName, "Servranx", ECommerceSettings.SendEmailsFrom(SiteContext.CurrentSite.SiteName), MacroContext.CurrentResolver, AuthenticationHelper.GetResetPasswordUrl(SiteContext.CurrentSiteName), out success);

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
		tbwmPwdRetrieval.WatermarkText = GetString("adressemail");
        wmLogin.WatermarkText = GetString("adressemail");
        lbPasswd.Text = GetString("forgotpwd");
        txtLastName.ToolTip = GetString("nom");
        wmLastName.WatermarkText = GetString("nom");
        txtFirstName.ToolTip = GetString("prenom");
        wmFirstName.WatermarkText = GetString("prenom");
        txtMotDePasse.ToolTip = GetString("mdp");
        //txtMotDePasseRegistration = GetString("mdp");
        txtnumero.ToolTip = GetString("numerorue");
        wmnumero.WatermarkText = GetString("numerorue");
        txtadresse1.ToolTip = GetString("adresse1");
        wmadresse1.WatermarkText = GetString("adresse1");
        txtadresse2.ToolTip = GetString("adresse2");
        wmadresse2.WatermarkText = GetString("adresse2");
        LabelAlreadyAccount.Text = GetString("dejauncompte");
        txtTelephone.ToolTip = GetString("Telephone");
        wmTelephone.WatermarkText = GetString("Telephone");
        txtLogin.ToolTip = GetString("adressemail");
        //txtAdresse.ToolTip = GetString("adressefacturation");
        //wmAdresse.WatermarkText = GetString("adressefacturation");
        txtCodePostale.ToolTip = GetString("cp");
        wmCodePostale.WatermarkText= GetString("cp");
        txtVille.ToolTip = GetString("ville");
        wmVille.WatermarkText = GetString("ville");
        lblSociete.Text = GetString("societe");
        lblnon.Text = GetString("non");
        lbloui.Text = GetString("oui");
        rbnon.Text = string.Format("{0}   &nbsp;", GetString("non"));
        rboui.Text = string.Format ("{0}   &nbsp;", GetString("oui"));
        txtnomsociete.ToolTip = GetString("nomsociete");
        wmnomsociete.WatermarkText = GetString("nomsociete");
        txtTva.ToolTip = GetString("tva");
        wmtva.WatermarkText = GetString("tva");
        //txtadresseshipping.ToolTip = GetString("adresselivraison");
        //wmadresseshipping.WatermarkText = GetString("adresselivraison");
        txtcpshipping.ToolTip = GetString("cp");
        wmcpshipping.WatermarkText = GetString("cp");
        txtvilleshipping.ToolTip = GetString("ville");
        wmvilleshipping.WatermarkText = GetString("ville");
        txtnumeroshipping.ToolTip = GetString("numerorue");
        wmnumeroshipping.WatermarkText = GetString("numerorue");
        txtadresse1shipping.ToolTip = GetString("adresse1");
        wmadresse1shipping.WatermarkText = GetString("adresse1");
        txtadresse2shipping.ToolTip = GetString("adresse2");
        wmadresse2shipping.WatermarkText = GetString("adresse2");      
        txtEmailRegistration.ToolTip = GetString("adressemail");
        wmEmailRegistration.WatermarkText = GetString("adressemail");
        txtMotDePasseRegistration.ToolTip = GetString("mdp");
        txtMotDePasseConfirmation.ToolTip = GetString("confirmdp");      
        LabelAdresseDifference.Text = GetString("differenceadresse");
        LabelAgree.Text = GetString("yesaccept");
        LabelCondition.Text = GetString("conditiongenerale");
        LabelReceive.Text = GetString("yesreceive");
        LabelLastArticle.Text = GetString("lastarticle");
        LabelNewCustomer.Text = GetString("nouveauclient");
     }

    /// <summary>
    /// On chkCorporateBody checkbox checked changed.
    /// </summary>
    protected void chkCorporateBody_CheckChanged(object sender, EventArgs e)
    {
        pnlCompanyAccount1.Visible = chkCorporateBody.Checked;
    }


    /// <summary>
    /// On chkCorporateBody2 checkbox checked changed.
    /// </summary>
    protected void chkCorporateBody2_CheckChanged(object sender, EventArgs e)
    {
        plcCompanyAccount3.Visible = chkCorporateBody2.Checked;
    }


    /// <summary>
    /// On chkEditCorpBody checkbox checked changed.
    /// </summary>
    protected void chkEditCorpBody_CheckChanged(object sender, EventArgs e)
    {
        pnlCompanyAccount2.Visible = chkEditCorpBody.Checked;
    }


    #region IsValid

    public override bool IsValid()
    {
        if (!this.ShoppingCartControl.UserInfo.IsPublic()) return true;

        //var val = new Validator();
        //string result = null;

        // Validate registration data
        if (IsExistingAccount())
        {
            //result = val.NotEmpty(txtUsername.Text.Trim(), ResHelper.GetString("ShoppingCartCheckRegistration.ErrorMissingUsername")).Result;

            if (IsDefaultOrEmptyLogin())
            {
                LabelErrorMessage.Text=GetString("errorlogin");
                return false;
            }

            if (IsDefaultOrEmptyPassword())
            {
                LabelErrorMessage.Text = GetString("errormdprequired");
                return false;
            }
        }

        // Check 'New registration' section
        else if (IsNewAccount())
        {
           if (!ValidationHelper.IsEmail(txtEmailRegistration.Text))
            {
                lblError.Visible = true;
                lblError.Text = GetString("errorinvalidmail");
                return false;
            }
            // Password and confirmed password must be same
            if (txtMotDePasseRegistration.Text != txtMotDePasseConfirmation.Text)
            {
                lblError.Visible = true;
                lblError.Text = GetString("errormdp");
                return false;
            }

            /**adress*/
            Validator val = new Validator();
            // check billing part of the form
            /*string result = val.NotEmpty(txtAdresse.Text.Trim(), GetString("ShoppingCartOrderAddresses.BillingAddressLineErr"))
                .NotEmpty(txtVille.Text.Trim(), GetString("ShoppingCartOrderAddresses.BillingCityErr"))
                .NotEmpty(txtCodePostale.Text.Trim(), GetString("ShoppingCartOrderAddresses.BillingZIPErr")).Result;*/

            /*if (result == "")
            {
                // Check shipping address
                if (chkShippingAddr.Checked)
                {
                    // check shipping part of the form if checkbox is checked
                    result = val.NotEmpty(txtadresseshipping.Text.Trim(), GetString("ShoppingCartOrderAddresses.ShippingAddressLineErr"))
                        .NotEmpty(txtvilleshipping.Text.Trim(), GetString("ShoppingCartOrderAddresses.ShippingCityErr"))
                        .NotEmpty(txtadresseshipping.Text.Trim(), GetString("ShoppingCartOrderAddresses.ShippingZIPErr")).Result;
                }

            }

            if (result != "")
            {
                // display error
                lblError.Visible = true;
                lblError.Text = result;
                LabelErrorMessageCreation.Text = result;
                return false;
            }*/

            return true;

            /**adress*/
        }
        else
        {
            lblError.Text = ""; //GetString("erroretape2");
            lblError.Visible = true;
            return false;
        }
        return true;
    }

    #endregion
    
    #region ProcessStep

    public override bool ProcessStep()
    {
        string siteName = SiteContext.CurrentSiteName;
        if (IsExistingAccount())
        {
            // Sign in customer with existing account

            // Authenticate user
            //UserInfo ui = UserInfoProvider.GetUserInfo(txtLogin.Text);

            UserInfo ui = AuthenticationHelper.AuthenticateUser(txtLogin.Text.Trim(), txtMotDePasse.Text, SiteContext.CurrentSiteName);

            if (ui == null)
            {
                // ShowError(ResHelper.GetString("ShoppingCartCheckRegistration.LoginFailed"));
                LabelErrorMessage.Text = GetString("loginfail");
                return false;
            }

            // Set current user
            MembershipContext.AuthenticatedUser = new CurrentUserInfo(ui, true);
            UserInfoProvider.SetPreferredCultures(ui);

            // Sign in
            FormsAuthentication.SetAuthCookie(ui.UserName, false);

            // Registered user has already started shopping as anonymous user -> Drop his stored shopping cart
            ShoppingCartInfoProvider.DeleteShoppingCartInfo(ui.UserID, siteName);

            // Assign current user to the current shopping cart
            ShoppingCart.User = ui;

            // Save changes to database // Already done in the end of this method
            if (!this.ShoppingCartControl.IsInternalOrder)
            {
                ShoppingCartInfoProvider.SetShoppingCartInfo(this.ShoppingCartInfObj);
            }

            //Create a customer for the user if do not yet exist 
            CustomerInfo ci = CustomerInfoProvider.GetCustomerInfoByUserID(this.ShoppingCartControl.UserInfo.UserID);
            if (ci == null)
            {
                ci = new CustomerInfo();
                ci.CustomerUserID = this.ShoppingCartControl.UserInfo.UserID;
                ci.CustomerEnabled = true;
            }

            // Old email address
            //string oldEmail = ci.CustomerEmail.ToLower(); ;

            ci.CustomerFirstName = ui.FirstName;
            ci.CustomerLastName = ui.LastName;
            ci.CustomerEmail = ui.Email;
            // atoo
            ci.CustomerPhone = "";
            ci.CustomerCompany = "";
            ci.CustomerOrganizationID = "";
            ci.CustomerTaxRegistrationID = "";

            // Update customer data
            CustomerInfoProvider.SetCustomerInfo(ci);

            // Set the shopping cart customer ID
            this.ShoppingCart.ShoppingCartCustomerID = ci.CustomerID;

        }
        else if (IsNewAccount())
        {
            txtEmailRegistration.Text = txtEmailRegistration.Text.Trim();
            pnlCompanyAccount1.Visible = chkCorporateBody.Checked;

            // Check if user exists
            UserInfo ui = UserInfoProvider.GetUserInfo(txtEmailRegistration.Text);
            if (ui != null)
            {
                lblError.Visible = true;
               // lblError.Text = GetString("ShoppingCartUserRegistration.ErrorUserExists");
                lblError.Text = GetString("erroruser");
                return false;
            }

            // Check all sites where user will be assigned
            string checkSites = (String.IsNullOrEmpty(ShoppingCartControl.AssignToSites)) ? SiteContext.CurrentSiteName : ShoppingCartControl.AssignToSites;
            if (!UserInfoProvider.IsEmailUnique(txtEmail2.Text.Trim(), checkSites, 0))
            {
                lblError.Visible = true;
                //lblError.Text = GetString("UserInfo.EmailAlreadyExist");
                lblError.Text = GetString("errormail");
                return false;
            }

            // Create new customer and user account and sign in
            // User

            ui = new UserInfo();

            ui.UserName = txtEmailRegistration.Text.Trim();
            ui.Email = txtEmailRegistration.Text.Trim();
            ui.FirstName = txtFirstName.Text.Trim();
            ui.FullName = UserInfoProvider.GetFullName(txtFirstName.Text.Trim(), String.Empty, txtLastName.Text.Trim());
            ui.LastName = txtLastName.Text.Trim();			
            ui.SetValue("Civilite", ddlFrom.SelectedValue);
            ui.SetValue("Telephone", txtTelephone.Text);
            ui.SetValue("UserPhone", txtTelephone.Text);
            //atoo
          //  ui.SetValue("CustomerPhone", txtTelephone.Text);
            ui.Enabled = true;
            ui.UserIsGlobalAdministrator = false;
            ui.UserURLReferrer = MembershipContext.AuthenticatedUser.URLReferrer;
            ui.UserCampaign = AnalyticsHelper.Campaign;
            ui.UserSettings.UserRegistrationInfo.IPAddress = RequestContext.UserHostAddress;
            ui.UserSettings.UserRegistrationInfo.Agent = HttpContext.Current.Request.UserAgent;

            try
            {
                UserInfoProvider.SetUserInfo(ui);             
                UserInfoProvider.SetPassword(ui, txtMotDePasseRegistration.Text);

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
            ci.CustomerFirstName = txtFirstName.Text.Trim();
            ci.CustomerLastName = txtLastName.Text.Trim();
            ci.CustomerEmail = txtEmailRegistration.Text.Trim();
            //atoo
            ci.CustomerPhone = txtTelephone.Text.Trim();
            //ci.CustomerCompany = "";
            ci.CustomerOrganizationID = "";
           // ci.CustomerTaxRegistrationID = "";
            if (rboui.Checked)
            {
                ci.CustomerCompany = txtnomsociete.Text.ToString();
                ci.CustomerTaxRegistrationID = txtTva.Text.ToString();
            }
            else
            {
                ci.CustomerCompany = "";
                ci.CustomerTaxRegistrationID = "";
            }
            /*if (chkCorporateBody.Checked)
            {
                ci.CustomerCompany = txtCompany1.Text.Trim();
                if (mShowOrganizationIDField)
                {
                    ci.CustomerOrganizationID = txtOrganizationID.Text.Trim();
                }
                if (mShowTaxRegistrationIDField)
                {
                    ci.CustomerTaxRegistrationID = txtTaxRegistrationID.Text.Trim();
                }
            }*/

            ci.CustomerUserID = ui.UserID;
            ci.CustomerSiteID = 0;
            ci.CustomerEnabled = true;
            ci.CustomerCreated = DateTime.Now;
            CustomerInfoProvider.SetCustomerInfo(ci);
                   
            // Track successful registration conversion
            if (this.ShoppingCartControl.RegistrationTrackConversionName != string.Empty)
            {
                if (AnalyticsHelper.AnalyticsEnabled(siteName) && AnalyticsHelper.TrackConversionsEnabled(siteName) && !AnalyticsHelper.IsIPExcluded(siteName, HttpContext.Current.Request.UserHostAddress))
                {
                    string objectName = new ContextResolver().ResolveMacros(this.ShoppingCartControl.RegistrationTrackConversionName);
                    HitLogProvider.LogHit(HitLogProvider.CONVERSIONS, siteName, LocalizationContext.PreferredCultureCode, objectName, 0);
                }
            }

            // Sign in
            if (ui.UserEnabled)
            {
                FormsAuthentication.SetAuthCookie(ui.UserName, false);
                this.ShoppingCart.User= ui;
            }

            this.ShoppingCart.ShoppingCartCustomerID = ci.CustomerID;

            // Send new registration notification email
            if (this.ShoppingCartControl.SendNewRegistrationNotificationToAddress != "")
            {
                SendRegistrationNotification(ui);
            }

            MembershipContext.AuthenticatedUser = new CurrentUserInfo(ui, true);

            /**aadrresse*/
            
            // Process billing address
            //------------------------
            int BillingCountryID = ValidationHelper.GetInteger(ddlBillingCountry.SelectedValue, 0);
            int ShippingCountryID = ValidationHelper.GetInteger(ddlShippingCountry.SelectedValue, 0);
            AddressInfo ai = null;
            bool newAddress = true;
            ai = new AddressInfo();
            string mCustomerName = ci.CustomerFirstName + " " + ci.CustomerLastName;
            // newAddress.AddressName = mCustomerName + " , " + txtAdresse.Text + " - " + txtCodePostale.Text + " " + txtVille.Text;

            ai.SetValue("AddressNumber", txtnumero.Text);
            ai.AddressName = mCustomerName + " , " + txtnumero.Text + " " + txtadresse1.Text + " - " + txtCodePostale.Text + " " + txtVille.Text;
            ai.AddressPersonalName = mCustomerName;
            ai.AddressLine1 = txtadresse1.Text.Trim();
            ai.AddressLine2 = txtadresse2.Text.Trim();
            ai.AddressCity = txtVille.Text.Trim();
            ai.AddressZip = txtCodePostale.Text.Trim();
            ai.AddressCountryID = BillingCountryID;
            
            if (newAddress)
            {
                ai.AddressIsBilling = true;
                ai.AddressIsShipping = !chkShippingAddr.Checked;
                ai.AddressEnabled = true;
            }
            ai.AddressCustomerID = ci.CustomerID;
            ai.AddressName = mCustomerName + " , " + txtnumero.Text + " "  + txtadresse1.Text + " - " + txtCodePostale.Text + " " + txtVille.Text;
         
            // Save address and set it's ID to ShoppingCartInfoObj
            AddressInfoProvider.SetAddressInfo(ai);

            // Update current contact's address
            ModuleCommands.OnlineMarketingMapAddress(ai, ContactID);

            ShoppingCart.ShoppingCartBillingAddressID = ai.AddressID;

            // If shopping cart does not need shipping
            if (!ShippingOptionInfoProvider.IsShippingNeeded(ShoppingCart))
            {
                ShoppingCart.ShoppingCartShippingAddressID = 0;
            }
            // If shipping address is different from billing address
            else if (chkShippingAddr.Checked)
            {
                AddressInfo aishipping = new AddressInfo();
                aishipping.AddressPersonalName = mCustomerName;
                aishipping.SetValue("AddressNumber", txtnumeroshipping.Text);
                aishipping.AddressName = mCustomerName + " , " + txtnumeroshipping.Text + " " + txtadresse1shipping.Text + " - " + txtcpshipping.Text + " " + txtvilleshipping.Text;
                aishipping.AddressLine1 = txtadresse1shipping.Text.Trim();
                aishipping.AddressLine2 = txtadresse2shipping.Text.Trim();
                aishipping.AddressCity = txtvilleshipping.Text.Trim();
                aishipping.AddressZip = txtcpshipping.Text.Trim();
                aishipping.AddressCountryID = ShippingCountryID;

                if (newAddress)
                {
                    aishipping.AddressIsShipping = true;
                    aishipping.AddressEnabled = true;
                    aishipping.AddressIsBilling = false;
                    aishipping.AddressIsCompany = false;
                    aishipping.AddressEnabled = true;
                }
                aishipping.AddressCustomerID = ci.CustomerID;
                aishipping.AddressName = mCustomerName + " , " + txtnumeroshipping.Text + " " + txtadresse1shipping.Text + " - " + txtcpshipping.Text + " " + txtvilleshipping.Text;
             
                // Save address and set it's ID to ShoppingCartInfoObj
                AddressInfoProvider.SetAddressInfo(aishipping);
                ShoppingCart.ShoppingCartShippingAddressID = aishipping.AddressID;
            }
            // Shipping address is the same as billing address
            else
            {
                ShoppingCart.ShoppingCartShippingAddressID = ShoppingCart.ShoppingCartBillingAddressID;
            }
            /**finadrress*/
        }      

        try
        {
            if (!this.ShoppingCartControl.IsInternalOrder)
            {
                ShoppingCartInfoProvider.SetShoppingCartInfo(this.ShoppingCart);
            }
            return true;
        }
        catch
        {
            return false;
        }
    }

    #endregion

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
    protected void BtnLogin_Click(object sender, EventArgs e)
    {
        //Response.Write("<script>alert('This is Submit button login');</script>");

        lblError.Text = String.Empty;
        lblError.Visible = false;
        txtFirstName.Text = "";
        txtLastName.Text = "";
        //txtUsername.Text = "";
        //txtMotDePasse.Text = "";

        if (IsValid())
        {
            //ProcessStep();
            _isExistingAccount = true;
            this.ShoppingCartControl.ButtonNextClickAction();
        }
    }
    protected void BtnCreatNewAccount_Click(object sender, EventArgs e)
    {          
        if (chkAccept.Checked)
        {
            #region "Email"

            if ((txtEmailRegistration.Text == "") || (txtEmailRegistration.Text == "E-mail"))
            {
                lblError.Visible = true;
                lblError.Text = GetString("erroremail");
                return;
            }

            // Check whether user with same email does not exist 
            UserInfo ui = UserInfoProvider.GetUserInfo(txtEmailRegistration.Text);
            SiteInfo si = SiteContext.CurrentSite;
            UserInfo siteui = UserInfoProvider.GetUserInfo(UserInfoProvider.EnsureSitePrefixUserName(txtEmailRegistration.Text, si));

            if ((ui != null) || (siteui != null))
            {
                lblError.Visible = true;
                lblError.Text = GetString("errormail");
                return;
            }

            if (!ValidationHelper.IsEmail(txtEmailRegistration.Text.ToLowerCSafe()))
            {
                lblError.Visible = true;
                lblError.Text = GetString("errorinvalidmail");
                return;
            }

            #endregion

            #region "Pr�nom"

            if ((txtFirstName.Text == "") || (txtFirstName.Text == "FirstName") || (txtFirstName.Text == "Pr�nom"))
            {
                lblError.Visible = true;
                lblError.Text = GetString("errorprenom");
                return;
            }

            #endregion

            #region "Nom"

            if ((txtLastName.Text == "") || (txtLastName.Text == "LastName") || (txtLastName.Text == "Nom"))
            {
                lblError.Visible = true;
                lblError.Text = GetString("errornom");
                return;
            }

            #endregion

            #region "n�"

            if ((txtnumero.Text == "") || (txtnumero.Text == "Numero") || (txtnumero.Text == "Number"))
            {
                lblError.Visible = true;
                lblError.Text = GetString("errornumerorue");
                return;
            }

            #endregion

            #region "adresse 1"

            if ((txtadresse1.Text == "") || (txtadresse1.Text == "Adresse 1") || (txtadresse1.Text == "Address 1"))
            {
                lblError.Visible = true;
                lblError.Text = GetString("erroradresse1");
                return;
            }

            #endregion

            #region "adresse 2"

            if ((txtadresse2.Text == "Adresse 2") || (txtadresse2.Text == "Address 2"))   //(txtadresse2.Text == "") || 
            {
                lblError.Visible = true;
                lblError.Text = GetString("erroradresse2");
                return;
            }

            #endregion

            #region "Phone"
            if ((txtTelephone.Text == "") || (txtTelephone.Text == "TELEPHONE") || (txtTelephone.Text == "TELEPHONE"))
            {
                lblError.Visible = true;
                lblError.Text = GetString("errortelephone");
                return;
            }

            #endregion

            #region "CP"

            if ((txtCodePostale.Text == "") || (txtCodePostale.Text == "CP") || (txtCodePostale.Text == "ZIP"))
            {
                lblError.Visible = true;
                lblError.Text = GetString("errorcp");
                return;
            }

            #endregion

            #region "Ville"

            if ((txtVille.Text == "") || (txtVille.Text == "Ville") || (txtVille.Text == "City"))
            {
                lblError.Visible = true;
                lblError.Text = GetString("errorville");
                return;
            }

            #endregion

            #region "Pays"

            if ((ddlBillingCountry.Text == "Choose your country") || (ddlBillingCountry.Text == "Choisissez votre pays"))
            {
                lblError.Visible = true;
                lblError.Text = GetString("errorchoixpays ");
                return;
            }

            #endregion

            #region "Mot de passe"
            // Check whether password is same

            if ((txtMotDePasseRegistration.Text == "") || (txtMotDePasseRegistration.Text == "Mot de passe") || (txtMotDePasseRegistration.Text == "Password"))
            {
                lblError.Visible = true;
                lblError.Text = GetString("errormdprequired");
                return;
            }

            if (txtMotDePasseRegistration.Text != txtMotDePasseConfirmation.Text)
            {
                lblError.Visible = true;
                lblError.Text = GetString("errormdp");
                return;
            }


            #endregion

            #region Soci�t�
            if (rboui.Checked)
            {
                if ((txtnomsociete.Text == "") || (txtnomsociete.Text == "Nom soci�t�") || (txtnomsociete.Text == "Company Name"))
                {
                    lblError.Visible = true;
                    lblError.Text = GetString("errornomsociete ");
                    return;
                }

                //if ((txtTva.Text == "") || (txtTva.Text == "TVA") || (txtTva.Text == "VAT"))
                //{
                //    lblError.Visible = true;
                //    lblError.Text = GetString("errortva ");
                //    return;
                //}

                //if (!EUVatChecker.Check(txtTva.Text))
                //{
                //    lblError.Visible = true;
                //    lblError.Text = GetString("errortva2 ");
                //    return;
                //}
            }
            #endregion

            #region "Adresse Shipping"
            if (chkShippingAddr.Checked)
            {

                #region "n� shipping"

                if ((txtnumeroshipping.Text == "") || (txtnumeroshipping.Text == "Numero") || (txtnumeroshipping.Text == "Number"))
                {
                    lblErrorShipping.Visible = true;
                    lblErrorShipping.Text = GetString("errornumerorue");
                    return;
                }
                #endregion

                #region "adresse 1 shipping"

                if ((txtadresse1shipping.Text == "") || (txtadresse1shipping.Text == "Adresse 1") || (txtadresse1shipping.Text == "Address 1"))
                {
                    lblErrorShipping.Visible = true;
                    lblErrorShipping.Text = GetString("erroradresse1");
                    return;
                }

                #endregion

                #region "adresse 2 shipping"

                if ((txtadresse2shipping.Text == "Adresse 2") || (txtadresse2shipping.Text == "Address 2"))
                {
                    lblErrorShipping.Visible = true;
                    lblErrorShipping.Text = GetString("erroradresse2");
                    return;
                }

                #endregion

                #region "CP Shipping"

                if ((txtcpshipping.Text == "") || (txtcpshipping.Text == "CP") || (txtcpshipping.Text == "ZIP"))
                {
                    lblErrorShipping.Visible = true;
                    lblErrorShipping.Text = GetString("errorcp");
                    return;
                }

                #endregion

                #region "Ville Shipping"

                if ((txtvilleshipping.Text == "") || (txtvilleshipping.Text == "Ville") || (txtvilleshipping.Text == "City"))
                {
                    lblErrorShipping.Visible = true;
                    lblErrorShipping.Text = GetString("errorville");
                    return;
                }

                #endregion

                #region "Pays"

                if ((ddlShippingCountry.Text == "Choose your country") || (ddlShippingCountry.Text == "Choisissez votre pays"))
                {
                    lblErrorShipping.Visible = true;
                    lblErrorShipping.Text = GetString("errorchoixpays ");
                    return;
                }

                #endregion
            }
            #endregion

            if (IsValid())
            {
                //ProcessStep();
                this.ShoppingCartControl.ButtonNextClickAction();
            }
        }
        else
        {
            lblError.Visible = true;
            lblError.Text = GetString("conditionobligatoire");
            return;
        }
    }

    #region ajout

    protected void chkShippingAddr_CheckedChanged(object sender, EventArgs e)
    {
        /*
        if (plhShipping.Visible)
        {
            plhShipping.Visible = false;
        }
        else
        {
            plhShipping.Visible = true;
            ShowShippingCountryList();        
        }*/
    }

    private void SubscribeToNewsLetter(string firstName, string lastName, string email)
    {
        EventLogProvider _eventLogProvider = new EventLogProvider();
        var newsletter = CMS.Newsletters.NewsletterInfoProvider.GetNewsletterInfo("NewsLetter1", CMSContext.CurrentSiteID);
        if (newsletter == null)
        {
            _eventLogProvider.LogEvent("I", DateTime.Now, "NewsLetter1", "Insert", CMSContext.CurrentSiteID);
            return;
        }

        if (!CMS.Newsletters.SubscriberInfoProvider.EmailExists(email))
        {
            var subscriberGuid = Guid.NewGuid();

            var sb = new CMS.Newsletters.SubscriberInfo()
            {
                SubscriberGUID = subscriberGuid,
                SubscriberFirstName = firstName,
                SubscriberLastName = lastName,
                SubscriberFullName = firstName + " " + lastName,
                SubscriberEmail = email,
                SubscriberSiteID = CMSContext.CurrentSiteID
            };

            sb.SetValue("SubscriberCreateDate", DateTime.Now);

            try
            {
                CMS.Newsletters.SubscriberInfoProvider.SetSubscriberInfo(sb);
                CMS.Newsletters.SubscriberInfoProvider.Subscribe(subscriberGuid, newsletter.NewsletterID, CMSContext.CurrentSiteID);
                _eventLogProvider.LogEvent("I", DateTime.Now, "register", "Delete", CMSContext.CurrentSiteID);
            }
            catch (Exception e)
            {
                _eventLogProvider.LogEvent("register", "Insert", e, CMSContext.CurrentSiteID);
            }
        }

    }
 
    private void ShowBillingCountryList()
    {
        //if (!IsPostBack)
        //{
        // changer donn�es du country List comme celle dans la 1�re �tape
        GeneralConnection cn = ConnectionHelper.GetConnection();
  
       // string stringQuery = string.Format("SELECT CountryID, CountryDisplayName FROM CMS_Country ORDER BY CountryDisplayName");
        string stringQuery = string.Format("SELECT CountryID, CountryDisplayName FROM CMS_Country  WHERE CountryId IN (SELECT ShippingCountryID from customtable_shippingextensioncountry)");
        DataSet ds = cn.ExecuteQuery(stringQuery, null, CMS.SettingsProvider.QueryTypeEnum.SQLQuery, false);

        if (!DataHelper.DataSourceIsEmpty(ds))
        {
            ds = LocalizedCountry.LocalizeCountry(ds);
            ddlBillingCountry.DataSource = ds;
            ddlBillingCountry.DataTextField = "CountryDisplayName";
            ddlBillingCountry.DataValueField = "CountryId";
            ddlBillingCountry.DataBind();
            ddlBillingCountry.Items.Insert(0, new ListItem(GetString("choixpays")));  
        }
        //}
    }

    private void ShowShippingCountryList()
    {
    // if (!IsPostBack)
    // {
        GeneralConnection cn = ConnectionHelper.GetConnection();
        string stringQuery = string.Format("SELECT CountryID, CountryDisplayName FROM CMS_Country where countryid in (select shippingcountryid from customtable_shippingextensioncountry) ORDER BY CountryDisplayName");
        DataSet ds = cn.ExecuteQuery(stringQuery, null, CMS.SettingsProvider.QueryTypeEnum.SQLQuery, false);
        if (!DataHelper.DataSourceIsEmpty(ds))
        {
            ds = LocalizedCountry.LocalizeCountry(ds);
            ddlShippingCountry.DataSource = ds;
            ddlShippingCountry.DataTextField = "CountryDisplayName";
            ddlShippingCountry.DataValueField = "CountryId";
            ddlShippingCountry.DataBind();

            ddlShippingCountry.Items.Insert(0, new ListItem(GetString("choixpays")));
        }
        // }
    }


    #endregion
    protected void lbPasswd_Click(object sender, EventArgs e)
    {
        if (PanelRetrievePassword.Visible == false) PanelRetrievePassword.Visible = true;
        else PanelRetrievePassword.Visible = false;
    }
    protected void btnPasswordRetrieval_Click(object sender, EventArgs e)
    {
        string value = txtBoxPasswordRetrieval.Text.Trim();
        if ((value != String.Empty) && (SiteContext.CurrentSite != null))
        {
            bool success;
            //LabelErrorMessage.Text = AuthenticationHelper.ForgottenEmailRequest(value, CMSContext.CurrentSiteName, "Servranx", ECommerceSettings.SendEmailsFrom(CMSContext.CurrentSite.SiteName), CMSContext.CurrentResolver, "http://kentico.wazo.lu/servranx/Autres-pages/Reset-password.aspx", out success); //AuthenticationHelper.GetResetPasswordUrl(CMSContext.CurrentSiteName)
            LabelResult.Text = AuthenticationHelper.ForgottenEmailRequest(value, SiteContext.CurrentSiteName, "Servranx", ECommerceSettings.SendEmailsFrom(SiteContext.CurrentSite.SiteName), MacroContext.CurrentResolver, "http://www.servranx.com/Autres-pages/Reset-password.aspx", out success); //AuthenticationHelper.GetResetPasswordUrl(CMSContext.CurrentSiteName)
            LabelResult.Visible =true;
            /*plcResult.Visible = true;
            plcErrorResult.Visible = false;
            
            pnlPasswdRetrieval.Attributes.Add("style", "display:block;");*/

            
            System.Globalization.CultureInfo currentUI = System.Globalization.CultureInfo.CurrentUICulture;
            if (Convert.ToString(currentUI) == "en-US")
            {
                /*Control ctl = pnlPasswdRetrieval.FindControl("lblResult");
                if (ctl != null)
                {
                    Label labelResult = (Label)ctl.FindControl("lblResult");
                    labelResult.Text = AuthenticationHelper.ForgottenEmailRequest(value, CMSContext.CurrentSiteName, "LOGONFORM", SendEmailFrom, CMSContext.CurrentResolver, ResetPasswordURL, out success, returnUrl);
                    labelResult.Visible = true;
                }*/
            }
            else if (Convert.ToString(currentUI) == "fr-FR")
            {
                Control ctl = pnlPasswdRetrieval.FindControl("lblResult");
                if (ctl != null)
                {
                    if (LabelResult.Text.Equals("Request for password change was sent."))
                    {
                        LabelResult.Text = "La demande de changement de mot de passe a �t� envoy�e.";
                    }
                    
                    if (LabelResult.Text.Equals("No user found."))
                    {
                        LabelResult.Text = "Aucun utilisateur trouv�.";
                    }
                    /*
                    Label labelResult = (Label)ctl.FindControl("lblResult");
                    string resultat = AuthenticationHelper.ForgottenEmailRequest(value, CMSContext.CurrentSiteName, "LOGONFORM", SendEmailFrom, CMSContext.CurrentResolver, ResetPasswordURL, out success, returnUrl);
                    if (resultat.Equals("Request for password change was sent."))
                    {
                        labelResult.Text = "La demande de changement de mot de passe a �t� envoy�.";
                        labelResult.Visible = true;
                    }
                    if (resultat.Equals("No user found."))
                    {
                        labelResult.Text = "Aucun utilisateur trouv�.";
                        labelResult.Visible = true;
                    }*/
                }
            }


            //LabelResult.Text = string.Format("{0} -- {1}", DateTime.Now.ToLongTimeString(), LabelErrorMessage.Text);
            //LabelResult.Visible = true;
        }



            
        

    }

    private void ShowCivility()
    {
      
        ListItem[] items = new ListItem[3];
        items[0] = new ListItem(GetString("civ"), "1");
        items[1] = new ListItem(GetString("mr"), "2");
        items[2] = new ListItem(GetString("mrs"), "3");

        if (ddlFrom.Items.Count == 0)
        {              
            ddlFrom.Items.AddRange(items);
            ddlFrom.DataBind();              
        }       
    }

    protected void rbnom_CheckedChanged(object sender, EventArgs e)
    {
        /*
        txtTva.Visible = false;
        txtnomsociete.Visible = false;
        rboui.Checked = false;
         */
    }

    protected void rboui_CheckedChanged(object sender, EventArgs e)
    {
        /*
        txtTva.Visible = true;
        txtnomsociete.Visible = true;
        rbnon.Checked = false;
        */
    }
    protected void txttva_TextChanged(object sender, EventArgs e)
    {
        if ((txtTva.Text == "") || (txtTva.Text == "TVA") || (txtTva.Text == "VAT"))
        {
            lblerror1.Visible = true;
            lblerror1.Text = GetString("errortva ");
            return;
        }

        if (!EUVatChecker.Check(txtTva.Text))
        {
            lblerror1.Visible = true;
            lblerror1.Text = GetString("errortva2 ");
            return;
        }
    }
    
}

