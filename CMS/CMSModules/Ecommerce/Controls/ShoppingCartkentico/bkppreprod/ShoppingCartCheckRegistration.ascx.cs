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
using CMS.MembershipProvider;

using TreeNode = CMS.DocumentEngine.TreeNode;
using System.Web.Security;
using CMS.DataEngine;
using CMS.Helpers;
using CMS.Membership;
using CMS.MacroEngine;
using CMS.DocumentEngine;

public partial class CMSModules_Ecommerce_Controls_ShoppingCart_ShoppingCartCheckRegistration : ShoppingCartStep
{
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
    private const string INVITE_PASSWORD = "MOT DE PASSE";
    private const string INVITE_CONFIRM_PASSWORD = "CONFIRMATION MOT DE PASSE";
    private const string INVITE_FIRSTNAME = "NOM";
    private const string INVITE_LASTNAME = "PRENOM";
    private const string INVITE_EMAIL = "ADRESSE E-MAIL";
    private const string INVITE_ADRESSE = "ADRESSE";
    private const string INVITE_CP = "CODE POSTALE";
    private const string INVITE_VILLE = "VILLE";
    private const string INVITE_CIVILITE = "Civilit�";

    private bool _isExistingAccount = false;
    private bool _isNewAccount = false;

    private bool IsDefaultOrEmptyLogin()
    {
        return String.IsNullOrEmpty(txtLogin.Text) || txtLogin.Text.Equals(INVITE_LOGIN);
    }

    private bool IsDefaultOrEmptyPassword()
    {
        return String.IsNullOrEmpty(txtMotDePasse.Text) || txtMotDePasse.Text.Equals(INVITE_PASSWORD);
    }

    private bool IsExistingAccount()
    {
        return (!(IsDefaultOrEmptyLogin() || IsDefaultOrEmptyPassword())) || _isExistingAccount;
    }

    private bool IsDefaultOrEmptyFirstName()
    {
        return String.IsNullOrEmpty(txtFirstName.Text) || txtFirstName.Text.Equals(INVITE_FIRSTNAME);
    }
    private bool IsDefaultOrEmptyLastName()
    {
        return String.IsNullOrEmpty(txtLastName.Text) || txtLastName.Text.Equals(INVITE_LASTNAME);
    }
    private bool IsDefaultOrEmptyEmail()
    {
        return String.IsNullOrEmpty(txtEmailRegistration.Text) || txtEmailRegistration.Text.Equals(INVITE_EMAIL);
    }

    private bool IsDefaultOrEmptyNewPassword()
    {
        return String.IsNullOrEmpty(txtMotDePasseRegistration.Text) || txtMotDePasseRegistration.Text.Equals(INVITE_PASSWORD);
    }
    private bool IsDefaultOrEmptyConfirmNewPassword()
    {
        return String.IsNullOrEmpty(txtMotDePasseConfirmation.Text) || txtMotDePasseConfirmation.Text.Equals(INVITE_CONFIRM_PASSWORD);
    }

    private bool IsNewAccount()
    {
        return (!(IsDefaultOrEmptyFirstName() || IsDefaultOrEmptyLastName() || IsDefaultOrEmptyEmail() || IsDefaultOrEmptyNewPassword() || IsDefaultOrEmptyConfirmNewPassword())) || _isNewAccount;
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
//        ScriptHelper.RegisterClientScriptBlock(this, GetType(), "showHide", ScriptHelper.GetScript(@"
//            /* Shows and hides tables with forms*/
//            function showHideForm(obj, rad)
//            {
//                var tblSignInStat = '';
//                var tblRegistrationStat = '';
//                var tblAnonymousStat = '';
//                if( obj != null && obj != '' && rad != null)
//                {
//                    switch(obj)
//                    {
//                        case 'tblSignIn':
//                            tblSignInStat = '';
//                            tblRegistrationStat = 'none';
//                            tblAnonymousStat = 'none';
//                            break;
//
//                        case 'tblRegistration':
//                            tblSignInStat = 'none';
//                            tblRegistrationStat = '';
//                            tblAnonymousStat = 'none';
//                            break;
//
//                        case 'tblAnonymous':
//                            tblSignInStat = 'none';
//                            tblRegistrationStat = 'none';
//                            tblAnonymousStat = '';
//                            break;                
//                    }
//
//                    if(document.getElementById('tblSignIn') != null)
//                        document.getElementById('tblSignIn').style.display = tblSignInStat;
//                    if(document.getElementById('tblRegistration') != null)
//                        document.getElementById('tblRegistration').style.display = tblRegistrationStat;
//                    if(document.getElementById('tblAnonymous') != null)
//                        document.getElementById('tblAnonymous').style.display = tblAnonymousStat;
//                    if(document.getElementById(rad) != null)
//                        document.getElementById(rad).setAttribute('checked','true');
//                }
//            }
//            function showElem(id)
//            {
//                style = document.getElementById(id).style;
//                style.display = (style.display == 'block')?'none':'block';
//                return false;
//            }
//            function showHideChk(id)
//            {
//                var elem = document.getElementById(id);
//                if(elem.style.display == 'block')
//                {
//                    elem.style.display = 'none';
//                }
//                else
//                {
//                    elem.style.display = 'block';
//                }
//            }"));

//        // Get settings for current site
//        SiteInfo si = CMSContext.CurrentSite;
//        if (si != null)
//        {
//            mRequireOrgTaxRegIDs = ECommerceSettings.RequireCompanyInfo(si.SiteName);
//            mShowOrganizationIDField = ECommerceSettings.ShowOrganizationID(si.SiteName);
//            mShowTaxRegistrationIDField = ECommerceSettings.ShowTaxRegistrationID(si.SiteName);
//        }

//        PreRender += new EventHandler(CMSEcommerce_ShoppingCartCheckRegistration_PreRender);
//        InitializeLabels();

//        LoadStep(false);

//        // Initialize onclick events
//        radSignIn.Attributes.Add("onclick", "showHideForm('tblSignIn','" + radSignIn.ClientID + "');");
//        radNewReg.Attributes.Add("onclick", "showHideForm('tblRegistration','" + radNewReg.ClientID + "');");
//        radAnonymous.Attributes.Add("onclick", "showHideForm('tblAnonymous','" + radAnonymous.ClientID + "');");
//        lnkPasswdRetrieval.Attributes.Add("onclick", "return showElem('" + pnlPasswdRetrieval.ClientID + "');");
//        chkCorporateBody.Attributes.Add("onclick", "showHideChk('" + pnlCompanyAccount1.ClientID + "');");
//        chkEditCorpBody.Attributes.Add("onclick", "showHideChk('" + pnlCompanyAccount2.ClientID + "');");

        //if (!IsPostBack)
        //{
            ShowCountryList();
        //}
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
        if (mRequireOrgTaxRegIDs)
        {
            lblOrganizationID.Text = GetString("ShoppingCartCheckRegistration.lblOrganizationIDRequired");
            lblTaxRegistrationID.Text = GetString("ShoppingCartCheckRegistration.lblTaxRegistrationIDRequired");
            // Show required field marks
            lblMark15.Visible = true;
            lblMark16.Visible = true;
            lblMark17.Visible = true;
            lblMark21.Visible = true;
            lblMark22.Visible = true;
            lblMark23.Visible = true;

            lblEditCompany.Text = GetString("ShoppingCartCheckRegistration.CompanyRequired");
            lblEditOrgID.Text = lblOrganizationID.Text;
            lblEditTaxRegID.Text = lblTaxRegistrationID.Text;
            // Show required field marks
            lblMark18.Visible = true;
            lblMark19.Visible = true;
            lblMark20.Visible = true;
        }
        else
        {
            lblOrganizationID.Text = GetString("ShoppingCartCheckRegistration.lblOrganizationID");
            lblTaxRegistrationID.Text = GetString("ShoppingCartCheckRegistration.lblTaxRegistrationID");
            // Show required field marks
            lblMark15.Visible = false;
            lblMark16.Visible = false;
            lblMark17.Visible = false;

            lblEditCompany.Text = GetString("ShoppingCartCheckRegistration.CompanyRequired");
            lblEditOrgID.Text = lblOrganizationID.Text;
            lblEditTaxRegID.Text = lblTaxRegistrationID.Text;
            // Show required field marks
            lblMark18.Visible = false;
            lblMark19.Visible = false;
            lblMark20.Visible = false;
        }

        radSignIn.Text = GetString("ShoppingCartCheckRegistration.SignIn");
        lblUsername.Text = GetString("ShoppingCartCheckRegistration.Username");
        lblPsswd1.Text = GetString("ShoppingCartCheckRegistration.Psswd");
        radNewReg.Text = GetString("ShoppingCartCheckRegistration.NewReg");
        lblFirstName1.Text = GetString("ShoppingCartCheckRegistration.FirstName");
        lblLastName1.Text = GetString("ShoppingCartCheckRegistration.LastName");
        lblEmail2.Text = GetString("ShoppingCartCheckRegistration.EmailUsername");
        lblPsswd2.Text = lblPsswd1.Text;
        lblConfirmPsswd.Text = GetString("ShoppingCartCheckRegistration.ConfirmPsswd");
        radAnonymous.Text = GetString("ShoppingCartCheckRegistration.Anonymous");
        lblFirstName2.Text = lblFirstName1.Text;
        lblLastName2.Text = lblLastName1.Text;

        lblEditFirst.Text = lblFirstName1.Text;
        lblEditLast.Text = lblLastName1.Text;

        lblTaxRegistrationID2.Text = lblTaxRegistrationID.Text;

        lblOrganizationID2.Text = lblOrganizationID.Text;

        lblCorporateBody.Text = GetString("ShoppingCartCheckRegistration.lblCorporateBody");
        lblEditCorpBody.Text = lblCorporateBody.Text;

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
            lblMark9.Text = mark;
            lblMark10.Text = mark;
            lblMark11.Text = mark;
            lblMark12.Text = mark;
            lblMark13.Text = mark;
            lblMark14.Text = mark;
            lblMark15.Text = mark;
            lblMark16.Text = mark;
            lblMark17.Text = mark;
            lblMark18.Text = mark;
            lblMark19.Text = mark;
            lblMark20.Text = mark;
            lblMark21.Text = mark;
            lblMark22.Text = mark;
            lblMark23.Text = mark;
        }
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
                ShowError("Veuillez remplir votre login");
                return false;
            }

            if (IsDefaultOrEmptyPassword())
            {
                ShowError("Veuillez remplir votre mot de passe");
                return false;
            }
        }
        // Check 'New registration' section
        else if (IsNewAccount())
        {
            if (!ValidationHelper.IsEmail(txtEmailRegistration.Text))
            {
                ShowError("E-mail fourni non valide");
                return false;
            }
            // Password and confirmed password must be same
            if (txtMotDePasseRegistration.Text != txtMotDePasseConfirmation.Text)
            {
                ShowError("Confirmation du mot de passe incorrect");
                return false;
            }

            /**adress*/
            Validator val = new Validator();
            // check billing part of the form
            string result = val.NotEmpty(txtAdresse.Text.Trim(), GetString("ShoppingCartOrderAddresses.BillingAddressLineErr"))
                .NotEmpty(txtVille.Text.Trim(), GetString("ShoppingCartOrderAddresses.BillingCityErr"))
                .NotEmpty(txtCodePostale.Text.Trim(), GetString("ShoppingCartOrderAddresses.BillingZIPErr")).Result;

            if (result == "")
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

                return false;
            }

            return true;

            /**adress*/
        }
        else
        {
            lblError.Text = "Veuillez entrer votre login et votre mot de passe ou remplir les champs pour la cr�ation d'un nouveau compte";
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
            txtEmail2.Text = txtEmail2.Text.Trim();
            pnlCompanyAccount1.Visible = chkCorporateBody.Checked;

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
          

            ui.UserName = txtEmailRegistration.Text.Trim();
            ui.Email = txtEmailRegistration.Text.Trim();
            ui.FirstName = txtFirstName.Text.Trim();
            ui.FullName = UserInfoProvider.GetFullName(txtFirstName.Text.Trim(), String.Empty, txtLastName.Text.Trim());
            ui.LastName = txtLastName.Text.Trim();
            ui.Enabled = true;
            ui.UserIsGlobalAdministrator = false;
            ui.UserURLReferrer = MembershipContext.AuthenticatedUser.URLReferrer;
            ui.UserCampaign = AnalyticsHelper.Campaign;
            ui.UserSettings.UserRegistrationInfo.IPAddress = RequestContext.UserHostAddress;
            ui.UserSettings.UserRegistrationInfo.Agent = HttpContext.Current.Request.UserAgent;

            try
            {
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

            ci.CustomerCompany = "";
            ci.CustomerOrganizationID = "";
            ci.CustomerTaxRegistrationID = "";
            if (chkCorporateBody.Checked)
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
            }

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
            /**aadrresse*/
            // Process billing address
            //------------------------
            int CountryID = ValidationHelper.GetInteger(ddlShippingCountry.SelectedValue, 0);
            AddressInfo ai = null;
            bool newAddress = true;
            ai = new AddressInfo();
            string mCustomerName = ci.CustomerFirstName + " " + ci.CustomerLastName;
           // newAddress.AddressName = mCustomerName + " , " + txtAdresse.Text + " - " + txtCodePostale.Text + " " + txtVille.Text;


            ai.AddressPersonalName = mCustomerName + " , " + txtAdresse.Text + " - " + txtCodePostale.Text + " " + txtVille.Text;
            ai.AddressLine1 = txtAdresse.Text.Trim();
            ai.AddressLine2 = txtAdresse.Text.Trim();
            ai.AddressCity = txtVille.Text.Trim();
            ai.AddressZip = txtCodePostale.Text.Trim();
            ai.AddressCountryID =CountryID;
            
            
            if (newAddress)
            {
                ai.AddressIsBilling = true;
                ai.AddressIsShipping = !chkShippingAddr.Checked;
                 ai.AddressEnabled = true;
            }
            ai.AddressCustomerID = ci.CustomerID;
            ai.AddressName = AddressInfoProvider.GetAddressName(ai);

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
                //// Check country presence
                //if (CountrySelector2.CountryID <= 0)
                //{
                //    lblError.Visible = true;
                //    lblError.Text = GetString("countryselector.selectedcountryerr");
                //    return false;
                //}

                //if (!CountrySelector2.StateSelectionIsValid)
                //{
                //    lblError.Visible = true;
                //    lblError.Text = GetString("countryselector.selectedstateerr");
                //    return false;
                //}

                //newAddress = false;
                //// Process shipping address
                ////-------------------------
                //ai = AddressInfoProvider.GetAddressInfo(Convert.ToInt32(drpShippingAddr.SelectedValue));
                //if (ai == null)
                //{
                //    ai = new AddressInfo();
                //    newAddress = true;
                //}

                ai.AddressPersonalName = txtadresseshipping.Text.Trim();
                ai.AddressLine1 = txtadresseshipping.Text.Trim();
                ai.AddressLine2 = txtadresseshipping.Text.Trim();
                ai.AddressCity = txtvilleshipping.Text.Trim();
                ai.AddressZip = txtcpshipping.Text.Trim();
                ai.AddressCountryID = CountryID;
               
                if (newAddress)
                {
                    ai.AddressIsShipping = true;
                    ai.AddressEnabled = true;
                    ai.AddressIsBilling = false;
                    ai.AddressIsCompany = false;
                    ai.AddressEnabled = true;
                }
                ai.AddressCustomerID = ci.CustomerID;
                ai.AddressName = AddressInfoProvider.GetAddressName(ai);

                // Save address and set it's ID to ShoppingCartInfoObj
                AddressInfoProvider.SetAddressInfo(ai);
                ShoppingCart.ShoppingCartShippingAddressID = ai.AddressID;
            }
            // Shipping address is the same as billing address
            else
            {
                ShoppingCart.ShoppingCartShippingAddressID = ShoppingCart.ShoppingCartBillingAddressID;
            }
            /**finadrress*/
            this.ShoppingCartControl.ButtonNextClickAction();
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
        //if (chkAccept.Checked)
        //{
           
          
        //    lblError.Visible = false;
        //    _isNewAccount = true;
        //    this.ButtonNextClickAction();
        //}
        //else
        //{
        //    lblError.Visible = true;
        //    lblError.Text = "Vous devez accepter les conditions";
        //}
        if (IsValid())
        {
            ProcessStep();
          
            //this.ShoppingCartControl.ButtonNextClickAction();
        }
    }

    #region ajout

    protected void chkShippingAddr_CheckedChanged(object sender, EventArgs e)
    {
        if (plhShipping.Visible)
        {
            plhShipping.Visible = false;
        }
        else
        {
            plhShipping.Visible = true;
            // LoadShippingAddressInfo();
        }
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
    private void SetAdresseBilling(string adresse1, string cp1, string ville1, int cuid, bool adr, int pays)
    {

        // Create new address object
        AddressInfo newAddress = new AddressInfo();

        // Set the properties
        newAddress.AddressName = adresse1 + " " + cp1 + " " + ville1;
        newAddress.AddressLine1 = adresse1 + " " + cp1 + " " + ville1;
        newAddress.AddressLine2 = " ";
        newAddress.AddressCity = ville1;
        newAddress.AddressZip = cp1;
        newAddress.AddressIsBilling = true;
        newAddress.AddressIsShipping = adr;
        newAddress.AddressEnabled = true;
        newAddress.AddressPersonalName = CurrentUser.FullName;
        newAddress.AddressCustomerID = cuid;
        newAddress.AddressCountryID = pays;


        // Create the address
        AddressInfoProvider.SetAddressInfo(newAddress);


    }
    private void SetAdresseShipping(string adresse1, string cp1, string ville1, int cuid, int pays)
    {

        // Create new address object
        AddressInfo newAddress = new AddressInfo();

        // Set the properties
        newAddress.AddressName = adresse1 + " " + cp1 + " " + ville1;
        newAddress.AddressLine1 = adresse1 + " " + cp1 + " " + ville1;
        newAddress.AddressLine2 = " ";
        newAddress.AddressCity = ville1;
        newAddress.AddressZip = cp1;
        newAddress.AddressIsBilling = false;
        newAddress.AddressIsShipping = true;
        newAddress.AddressEnabled = true;
        newAddress.AddressPersonalName = CurrentUser.FullName;
        newAddress.AddressCustomerID = cuid;
        newAddress.AddressCountryID = pays;

        // Create the address
        AddressInfoProvider.SetAddressInfo(newAddress);


    }
    private void ShowCountryList()
    {
        //if (!IsPostBack)
        //{
            GeneralConnection cn = ConnectionHelper.GetConnection();
            string stringQuery = string.Format("SELECT CountryID, CountryDisplayName FROM CMS_Country ORDER BY CountryDisplayName");
            DataSet ds = cn.ExecuteQuery(stringQuery, null, CMS.SettingsProvider.QueryTypeEnum.SQLQuery, false);
            if (!DataHelper.DataSourceIsEmpty(ds))
            {
                ddlShippingCountry.DataSource = ds;
                ddlShippingCountry.DataTextField = "CountryDisplayName";
                ddlShippingCountry.DataValueField = "CountryId";
                ddlShippingCountry.DataBind();
                // ddlShippingCountry_SelectedIndexChanged(null, null);
            }
        //}
    }
    #endregion
}