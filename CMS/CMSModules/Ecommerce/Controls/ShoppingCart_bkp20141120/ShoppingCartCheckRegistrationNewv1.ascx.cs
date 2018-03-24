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
//using CMS.Newsletter;
using Servranx.Helpers;
using TreeNode = CMS.DocumentEngine.TreeNode;
using System.Web.Security;
using CMS.DataEngine;
using CMS.Globalization;
using CMS.Helpers;
using CMS.Membership;
using CMS.MacroEngine;
using CMS.Base;
using CMS.DocumentEngine;

public partial class CMSModules_Ecommerce_Controls_ShoppingCart_ShoppingCartCheckRegistrationNew : ShoppingCartStep
{
    private bool mDataLoaded = false;

    private bool mRequireOrgTaxRegIDs = false;
    private bool mShowTaxRegistrationIDField = false;
    private bool mShowOrganizationIDField = false;
    private int mCustomerId;

    private CountryInfo _country;
    private CountryInfo Country
    {
        get
        {
            if (_country == null)
            {
                switch (CultureHelper.GetPreferredCulture().ToLower())
                {
                    case "fr-fr":
                        _country = CountryInfoProvider.GetCountryInfo("France");
                        break;
                    default:
                        _country = CountryInfoProvider.GetCountryInfo("Belgium");
                        break;
                }
            }

            return _country;
        }
    }

    #region Event handler

    /// <summary>
    /// On page load.
    /// </summary>
    protected void Page_Load(object sender, EventArgs e)
    {
        EventLogProvider.LogInformation("Shopping cart check registration - Page_Load", "I", "SHOPPING CART");

        Session.Add("Track", "00");
        ScriptHelper.RegisterClientScriptBlock(this, this.GetType(), "showHide", ScriptHelper.GetScript(@"
            /* Shows and hides tables with forms*/
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

        //  chkAcceptCGV.Checked = true;

        CustomValidator1.Text = ResHelper.LocalizeString("{$ShoppingCartCheckRegistration.FirstNameErr$}");
        CustomValidator2.Text = ResHelper.LocalizeString("{$ShoppingCartCheckRegistration.adressdErr$}");
        //  CustomValidator3.Text = ResHelper.LocalizeString("{$ShoppingCartCheckRegistration.NumErr$}");
        CustomValidator4.Text = ResHelper.LocalizeString("{$ShoppingCartCheckRegistration.ZipErr$}");
        CustomValidator5.Text = ResHelper.LocalizeString("{$ShoppingCartCheckRegistration.CityErr$}");
        CustomValidator6.Text = ResHelper.LocalizeString("{$ShoppingCartCheckRegistration.EmailErr$}");
        CustomValidator7.Text = ResHelper.LocalizeString("{$ShoppingCartCheckRegistration.adressdErr$}");
        //  CustomValidator8.Text = ResHelper.LocalizeString("{$ShoppingCartCheckRegistration.NumErr$}");
        CustomValidator9.Text = ResHelper.LocalizeString("{$ShoppingCartCheckRegistration.ZipErr$}");
        CustomValidator10.Text = ResHelper.LocalizeString("{$ShoppingCartCheckRegistration.CityErr$}");
        SiteInfo si = SiteContext.CurrentSite;
        if (si != null)
        {
            this.mRequireOrgTaxRegIDs = ECommerceSettings.RequireCompanyInfo(si.SiteName);
            this.mShowOrganizationIDField = ECommerceSettings.ShowOrganizationID(si.SiteName);
            this.mShowTaxRegistrationIDField = ECommerceSettings.ShowTaxRegistrationID(si.SiteName);
        }

        this.PreRender += new EventHandler(IblCMSModulesEcommerceControlsShoppingCartPoShoppingCartCheckRegistrationPreRender);
        InitializeLabels();

        this.lblTitle.Visible = false;

        mCustomerId = this.ShoppingCart.ShoppingCartCustomerID;

        //if (!this.ShoppingCartControl.IsCurrentStepPostBack)
        LoadCurrentStep();
        // Initialize onclick events
        lnkPasswdRetrieval.Attributes.Add("onclick", "return showElem('" + pnlPasswdRetrieval.ClientID + "');");
        //chkCorporateBody.Attributes.Add("onclick", "showHideChk('" + pnlCompanyAccount1.ClientID + "');");
        //chkEditCorpBody.Attributes.Add("onclick", "showHideChk('" + pnlCompanyAccount2.ClientID + "');"); 
        rdbSameAddress.Attributes.Add("onclick", "showElemAdv('" + pnlShippingAdress.ClientID + "','n');");
        rdbDifferentAddress.Attributes.Add("onclick", "showElemAdv('" + pnlShippingAdress.ClientID + "','y');");
        this.pnlShippingAdress.Attributes.Add("style", "display:none;");
        this.pnlPasswdRetrieval.Attributes.Add("style", "display:none;");



        if (!this.ShoppingCartControl.UserInfo.IsPublic())
        {
            chkAcceptCGV.Checked = true;

            //LoadData();
            //LoadAddresses();

            //if (Session["Previewed"].ToString() == "0")
            //{
            //    OnBtnSubmitClick(sender, e);
            //}
            //else
            //{
            //Session["Previewed"] == "0";
            //CMS.GlobalHelper.SessionHelper.SetValue("Previewed", "0");
            //}

            //var editAddress = ValidationHelper.GetBoolean(SessionHelper.GetValue("MODIFY_ADDRESS"), false);
            var editAddress = ValidationHelper.GetBoolean(this.ShoppingCartControl.GetTempValue("MODIFY_ADDRESS"), false);

            if (IsValid() && !editAddress)
                this.ShoppingCartControl.ButtonNextClickAction();
            //this.ShoppingCartControl.LoadStep("PoShoppingCartPreview");
            //else
            //SessionHelper.Remove("MODIFY_ADDRESS");
        }
    }

    void IblCMSModulesEcommerceControlsShoppingCartPoShoppingCartCheckRegistrationPreRender(object sender, EventArgs e)
    {
        if (!this.mDataLoaded && !this.ShoppingCartControl.IsCurrentStepPostBack)
        {
            EventLogProvider.LogInformation("SHOPPING CART", "I", "Shopping cart check registration - Page_PreRender");
            LoadCustomerDataFromDatabase();
            LoadAddressesAccordingProcessOrFromDatabase();
        }
    }

    /// <summary>
    /// Retrieve the user password.
    /// </summary>
    void btnPasswdRetrieval_Click(object sender, EventArgs e)
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

    /*
    protected void rdoBtn_CheckedChanged(object sender, EventArgs e)
    {
        bool visible = false;
        if (rdbDifferentAddress.Checked) { visible = true; }

        pnlShippingAdress.Visible = visible;
    }
    */

    protected void OnBtnSubmitClick(object sender, EventArgs e)
    {
        EventLogProvider.LogInformation("reg -submit", "I");

        lblSignInresult.Text = String.Empty;
        lblSignInresult.Visible = false;

        errorusername.Text = String.Empty;
        errorusername.Visible = false;

        lblError.Text = String.Empty;
        lblError.Visible = false;

        txtUsername.Text = "";
        txtPsswd1.Text = "";




        if (IsValid())
        {
            EventLogProvider.LogInformation("reg submit - next click action", "I");
            this.ButtonNextClickAction();
        }

    }

    protected void OnBtnSignInClick(object sender, EventArgs e)
    {
        EventLogProvider.LogInformation("sign in", "I");
        if (String.IsNullOrWhiteSpace(txtUsername.Text))
        {
            errorusername.Visible = true;
            errorusername.Text = GetString("ShoppingCartCheckRegistration.LoginMandatory");
            return;
        }
        var loginResult = AuthenticateThenRedirectToCustomerInfoEdition();

        if (!loginResult)
        {
            lblSignInresult.Visible = true;
            lblSignInresult.Text = GetString("ShoppingCartCheckRegistration.LoginFailed");
        }
        else
        {
            EventLogProvider.LogInformation("reg signin - next click action", "I");
            if (IsValid())
                this.ShoppingCartControl.ButtonNextClickAction();
            //this.ShoppingCartControl.LoadStep("PoShoppingCartPreview");
        }
    }

    protected void OnLnkSignOutClick(object sender, EventArgs e)
    {
        if (MembershipContext.AuthenticatedUser.IsAuthenticated())
        {
            FormsAuthentication.SignOut();
            CMSContext.ClearShoppingCart();

            string redirectUrl = RequestContext.CurrentRelativePath;

            // If the user has registered Windows Live ID
            if (!String.IsNullOrEmpty(MembershipContext.AuthenticatedUser.UserSettings.WindowsLiveID))
            {
                // Get data from auth cookie
                string[] userData = AuthenticationHelper.GetUserDataFromAuthCookie();

                // If user has logged in using Windows Live ID, then sign him out from Live too
                if ((userData != null) && (Array.IndexOf(userData, "liveidlogin") >= 0))
                {
                    string siteName = SiteContext.CurrentSiteName;

                    // Get LiveID settings
                    string appId = SettingsKeyInfoProvider.GetStringValue(siteName + ".CMSApplicationID");
                    string secret = SettingsKeyInfoProvider.GetStringValue(siteName + ".CMSApplicationSecret");

                    // Check valid Windows LiveID parameters
                    if ((appId != string.Empty) && (secret != string.Empty))
                    {
                        WindowsLiveLogin wll = new WindowsLiveLogin(appId, secret);

                        // Store info about logout request, for validation logout request
                        SessionHelper.SetValue("liveidlogout", DateTime.Now);

                        // Redirect to Windows Live 
                        redirectUrl = wll.GetLogoutUrl();
                    }
                }
            }

            PortalContext.ViewMode = ViewModeEnum.LiveSite;
            MembershipContext.AuthenticatedUser = null;

            Response.Cache.SetNoStore();
            URLHelper.Redirect(redirectUrl);
        }
    }

    /// <summary>
    /// On chkCorporateBody checkbox checked changed.
    /// </summary>
    protected void chkCorporateBody_CheckChanged(object sender, EventArgs e)
    {
        pnlCompanyAccount1.Visible = chkCorporateBody.Checked;
    }
    #endregion

    #region Initialization

    protected void LoadCurrentStep()
    {
        EventLogProvider.LogInformation("LoadCurrentStep", "I");

        // If user logged in, edit the customer data
        if (!this.ShoppingCartControl.UserInfo.IsPublic())
        {

            lblSignInTitle.Text = "Vous �tes connect� ";
            lblUsername.Text = String.Format("{0} {1}", lblUsername.Text, this.ShoppingCartControl.UserInfo.UserName);
            lblUsername.Style.Add("white-space", "nowrap");
            txtUsername.Text = String.Empty;
            txtUsername.Visible = false;
            lblPsswd1.Visible = false;
            txtPsswd1.Visible = false;
            btnSignIn.Visible = false;
            lnkPasswdRetrieval.Visible = false;
            lnkSignout.Visible = true;

            //this.lblTitle.Text = GetString("ShoppingCart.CheckRegistrationEdit");

            //if (loadData)
            //{
            LoadCustomerDataFromDatabase();
            LoadAddressesAccordingProcessOrFromDatabase();
            //}
        }
        else
        {
            this.plcTaxRegistrationID.Visible = this.mShowTaxRegistrationIDField;
            this.plcOrganizationID.Visible = this.mShowOrganizationIDField;

            //this.lblTitle.Text = GetString("ShoppingCart.CheckRegistration");

            // Set strings
            lnkPasswdRetrieval.Text = GetString("LogonForm.MotdePasseOublier");
            lblPasswdRetrieval.Text = GetString("LogonForm.lblMPD");
            btnPasswdRetrieval.Text = GetString("LogonForm.BtnPassOublie");
            rqValue.ErrorMessage = GetString("LogonForm.rqValueEmail");


            this.lnkPasswdRetrieval.Visible = this.ShoppingCartControl.EnablePasswordRetrieval;
            btnPasswdRetrieval.Click += new EventHandler(btnPasswdRetrieval_Click);

            this.pnlPasswdRetrieval.Attributes.Add("style", "display:none;");
            //this.pnlCompanyAccount1.Attributes.Add("style", "display:none;");
            //this.pnlCompanyAccount2.Attributes.Add("style", "display:none;");
        }

        //this.TitleText = GetString("Order_new.ShoppingCartCheckRegistration.Title");
    }

    protected void LoadCustomerDataFromDatabase()
    {
        EventLogProvider.LogInformation("LoadCustomerDataFromDatabase", "I");

        this.mDataLoaded = true;
        // If user ID specified, load the given user ID
        if (!this.ShoppingCartControl.UserInfo.IsPublic())
        {
            // Get the customer data
            CustomerInfo ci = CustomerInfoProvider.GetCustomerInfoByUserID(this.ShoppingCartControl.UserInfo.UserID);

            // Set the fields
            if (ci != null)
            {
                //Add Address if custom is authenticed
                //string where = "AddressCustomerID LIKE '" + ci.CustomerID + "' ORDER BY AddressLastModified DESC";
                //DataSet addresses = AddressInfoProvider.GetAddresses(where, null);
                this.txtCompany1.Text = ci.CustomerCompany;
                this.txtEmail2.Text = ci.CustomerEmail;
                this.txtFirstName1.Text = ci.CustomerFirstName;
                this.txtLastName1.Text = ci.CustomerLastName;
                this.txtOrganizationID.Text = ci.CustomerOrganizationID;
                this.txtTaxRegistrationID.Text = ci.CustomerTaxRegistrationID;
                //if (!DataHelper.DataSourceIsEmpty(addresses))
                //{
                //    AddressInfo ai = new AddressInfo(addresses.Tables[0].Rows[0]);
                //    this.txtBillingCity.Text = ai.AddressCity;
                //    this.txtBillingZip.Text = ai.AddressZip;
                //    this.txtBillingAddr1.Text = AddressHelper.GetStreetFromAddressLine(ai.AddressLine1);
                //    this.txtBillingNumber.Text = AddressHelper.GetNumberFromAddressLine(ai.AddressLine1);
                //    this.txtBillingAddr2.Text = ai.AddressLine2;
                //}
                this.chkAcceptCGV.Checked = true;
                if (!DataHelper.IsEmpty(txtCompany1.Text.Trim()) || !DataHelper.IsEmpty(txtOrganizationID.Text.Trim()) || !DataHelper.IsEmpty(txtTaxRegistrationID.Text.Trim()))
                {
                    chkCorporateBody.Checked = true;
                    pnlCompanyAccount1.Visible = true;
                }

                if (ci["CustomerHowHaveYouKnown"] != null)
                    ddlFrom.SelectedValue = ci["CustomerHowHaveYouKnown"].ToString();

                mCustomerId = ci.CustomerID;
            }
            else
            {
                this.txtFirstName1.Text = this.ShoppingCartControl.UserInfo.FirstName;
                this.txtLastName1.Text = this.ShoppingCartControl.UserInfo.LastName;
                this.txtEmail2.Text = this.ShoppingCartControl.UserInfo.Email;
            }

            rdbM.Checked = (MembershipContext.AuthenticatedUser.UserSettings.UserGender == 1);

            plcPassword.Visible = false;
            lblCustomerInfoTitle.Text = "Mettre � jour vos informations";

            //EventLogProvider.LogInformation("reg load data - next click action", "I");
            //this.ShoppingCartControl.ButtonNextClickAction();
        }
    }

    #region Refactored - N/A rule
    //private bool AreEqualAddresses(AddressInfo ai1, AddressInfo ai2)
    //{
    //    if (!String.IsNullOrWhiteSpace(ai1.AddressLine1) && !ai1.AddressLine1.Equals(ai2.AddressLine1))
    //        return false;
    //    if (!String.IsNullOrWhiteSpace(ai1.AddressLine2) && !ai1.AddressLine2.Equals(ai2.AddressLine2))
    //        return false;
    //    if (!String.IsNullOrWhiteSpace(ai1.AddressZip) && !ai1.AddressZip.Equals(ai2.AddressZip))
    //        return false;
    //    if (!String.IsNullOrWhiteSpace(ai1.AddressCity) && !ai1.AddressCity.Equals(ai2.AddressCity))
    //        return false;
    //    if (!ai1.AddressCountryID.Equals(ai2.AddressCountryID))
    //        return false;
    //    return true;
    //}
    #endregion

    private void LoadAddressesAccordingProcessOrFromDatabase()
    {
        EventLogProvider.LogInformation("LoadAddressesAccordingProcessOrFromDatabase", "I");

        if (this.ShoppingCart == null) throw new GeneralCMSException("ShoppingCart should not be null at this stage");

        if (mCustomerId == 0)
        {
            EventLogProvider.LogInformation("mCustomerId==0 when LoadAddressesAccordingProcessOrFromDatabase", "I");
            return;
        }

        //Get address from current shopping cart properties
        if (this.ShoppingCart.ShoppingCartBillingAddressID > 0 && this.ShoppingCart.ShoppingCartShippingAddressID > 0)
        {
            EventLogProvider.LogInformation("Get address from current shopping cart properties", "I");

            var billingAi = AddressInfoProvider.GetAddressInfo(this.ShoppingCart.ShoppingCartBillingAddressID);
            if (billingAi != null) FillBillingAddress(billingAi);
            if (this.ShoppingCart.ShoppingCartBillingAddressID == this.ShoppingCart.ShoppingCartShippingAddressID)
            {
                rdbSameAddress.Checked = true;
            }
            else
            {
                rdbDifferentAddress.Checked = true;
                var shippingAi = AddressInfoProvider.GetAddressInfo(this.ShoppingCart.ShoppingCartShippingAddressID);
                if (shippingAi != null) FillShippingAddress(shippingAi);
            }
            pnlShippingAdress.Style.Add("display", rdbDifferentAddress.Checked ? "block" : "none");
        }
        //current shopping cart does not have yet a selected address
        else
        {
            EventLogProvider.LogInformation("current shopping cart does not have yet a selected address", "I");

            // Use billing addresses from the last order
            int lastBillingAddressId = 0;
            int lastShippingAddressId = 0;
            bool fromLastOrder = false;

            DataSet ds = OrderInfoProvider.GetOrders("OrderCustomerID=" + mCustomerId, "OrderDate DESC");
            if (!DataHelper.DataSourceIsEmpty(ds))
            {
                EventLogProvider.LogInformation("current shopping cart does not have yet a selected address - ds not empty", "I");

                OrderInfo oi = new OrderInfo(ds.Tables[0].Rows[0]);
                lastBillingAddressId = oi.OrderBillingAddressID;
                lastShippingAddressId = oi.OrderShippingAddressID;

                var billingAi = AddressInfoProvider.GetAddressInfo(lastBillingAddressId);
                if (billingAi != null)
                {
                    fromLastOrder = true;
                    FillBillingAddress(billingAi);
                    this.ShoppingCart.ShoppingCartBillingAddressID = lastBillingAddressId;

                    if (lastBillingAddressId == lastShippingAddressId)
                    {
                        rdbSameAddress.Checked = true;
                    }
                    else
                    {
                        rdbDifferentAddress.Checked = true;
                        var shippingAi = AddressInfoProvider.GetAddressInfo(lastShippingAddressId);
                        if (shippingAi != null)
                        {
                            FillShippingAddress(shippingAi);
                            this.ShoppingCart.ShoppingCartShippingAddressID = lastShippingAddressId;
                        }
                    }
                }
            }
            // Customer does not have yet a order - Get Address from database directly
            else
            {
                EventLogProvider.LogInformation("Customer does not have yet a order - Get Address from database directly", "I");

                // get all billing addresses of the customer
                var billingDs =
                    AddressInfoProvider.GetAddresses(
                        "AddressCustomerID=" + mCustomerId + " AND AddressEnabled = 1 AND AddressIsBilling = 1",
                        "AddressLastModified DESC");

                // get all shipping addresses of the customer
                var shippingDs =
                    AddressInfoProvider.GetAddresses(
                        "AddressCustomerID=" + mCustomerId + " AND AddressEnabled = 1 AND AddressIsShipping = 1",
                        "AddressLastModified DESC");

                if (!DataHelper.DataSourceIsEmpty(billingDs))
                {
                    EventLogProvider.LogInformation("Customer does not have yet a order - Get billing from database directly - ds not empty", "I");

                    var billingAddressInfo = new AddressInfo(billingDs.Tables[0].Rows[0]);

                    // Fill billing address info
                    FillBillingAddress(billingAddressInfo);

                    // Select this address as biling address in shopping cart
                    this.ShoppingCart.ShoppingCartBillingAddressID = billingAddressInfo.AddressID;
                }

                if (!DataHelper.DataSourceIsEmpty(shippingDs))
                {
                    EventLogProvider.LogInformation("Customer does not have yet a order - Get shipping from database directly - ds not empty", "I");

                    var shippingAddressInfo = new AddressInfo(shippingDs.Tables[0].Rows[0]);

                    // Fill shipping address info
                    FillShippingAddress(shippingAddressInfo);

                    // Select this address as biling address in shopping cart
                    this.ShoppingCart.ShoppingCartShippingAddressID = shippingAddressInfo.AddressID;

                    if (this.ShoppingCart.ShoppingCartBillingAddressID == 0)
                        this.ShoppingCart.ShoppingCartBillingAddressID = this.ShoppingCart.ShoppingCartShippingAddressID;
                }

                if (this.ShoppingCart.ShoppingCartBillingAddressID == this.ShoppingCart.ShoppingCartShippingAddressID)
                    rdbSameAddress.Checked = true;
                else
                    rdbDifferentAddress.Checked = true;

                // Update shopping cart if addresses set
                //if (this.ShoppingCart.ShoppingCartBillingAddressID > 0)
                //{
                //EventLogProvider.LogInformation("Customer does not have yet a order - ShoppingCartBillingAddressID>0", "I");
                try
                {
                    EventLogProvider.LogInformation("Customer does not have yet a order - SetShoppingCartInfo(...)", "I");
                    ShoppingCartInfoProvider.SetShoppingCartInfo(this.ShoppingCart);
                }
                catch (Exception ex)
                {
                    EventLogProvider.LogException("Pdo Shopping Cart", "E", ex);
                }
                //}
                //else
                //EventLogProvider.LogInformation("Customer does not have yet a order - ShoppingCartBillingAddressID==0", "I");
            }
        }
    }

    private void FillBillingAddress(AddressInfo billingAddressInfo)
    {
        txtBillingAddr1.Text = AddressHelper.GetStreetFromAddressLine(billingAddressInfo.AddressLine1);
        txtBillingAddr2.Text = billingAddressInfo.AddressLine2;
        txtBillingCity.Text = billingAddressInfo.AddressCity;
        txtBillingZip.Text = billingAddressInfo.AddressZip;
        //  txtBillingNumber.Text = AddressHelper.GetNumberFromAddressLine(billingAddressInfo.AddressLine1);
    }

    private void FillShippingAddress(AddressInfo shippingAddressInfo)
    {
        txtShippingAddr1.Text = AddressHelper.GetStreetFromAddressLine(shippingAddressInfo.AddressLine1);
        txtShippingAddr2.Text = shippingAddressInfo.AddressLine2;
        txtShippingCity.Text = shippingAddressInfo.AddressCity;
        txtShippingZip.Text = shippingAddressInfo.AddressZip;
        txtShippingNumber.Text = AddressHelper.GetNumberFromAddressLine(shippingAddressInfo.AddressLine1);
    }

    /// <summary>
    /// Initialization of labels.
    /// </summary>
    protected void InitializeLabels()
    {
        if (mRequireOrgTaxRegIDs)
        {
            lblCompany1.Text = GetString("ShoppingCartCheckRegistration.CompanyRequired");
            lblOrganizationID.Text = GetString("ShoppingCartCheckRegistration.lblOrganizationIDRequired");
            lblTaxRegistrationID.Text = GetString("ShoppingCartCheckRegistration.lblTaxRegistrationIDRequired");
            lblMark15.Visible = true;
            lblMark16.Visible = true;
            lblMark17.Visible = true;
        }
        else
        {
            lblCompany1.Text = GetString("ShoppingCartCheckRegistration.Company");
            lblOrganizationID.Text = GetString("ShoppingCartCheckRegistration.lblOrganizationID");
            lblTaxRegistrationID.Text = GetString("ShoppingCartCheckRegistration.lblTaxRegistrationID");
            lblMark15.Visible = false;
            lblMark16.Visible = false;
            lblMark17.Visible = false;
        }

        lblUsername.Text = GetString("ShoppingCartCheckRegistration.Username");
        lblPsswd1.Text = GetString("ShoppingCartCheckRegistration.Psswd");
        lblFirstName1.Text = GetString("ShoppingCartCheckRegistration.FirstName");
        lblLastName1.Text = GetString("ShoppingCartCheckRegistration.LastName");
        lblEmail2.Text = GetString("ShoppingCartCheckRegistration.EmailUsername");
        lblPsswd2.Text = lblPsswd1.Text;
        lblConfirmPsswd.Text = GetString("ShoppingCartCheckRegistration.ConfirmPsswd");

        lblCorporateBody.Text = GetString("ShoppingCartCheckRegistration.lblCorporateBody");
        //lblEditCorpBody.Text = lblCorporateBody.Text;

        lblBillingAddrLine.Text = GetString("ShoppingCartOrderAddresses.BillingAddrLine");
        //lblBillingAddrLine2.Text = lblBillingAddrLine.Text;
        lblBillingAddrLine2.Text = GetString("ShoppingCartOrderAddresses.BillingAddrLine2");
        //lblBillingAddrLine2.Style["display"] = "none";
        lblBillingCity.Text = GetString("ShoppingCartOrderAddresses.BillingCity");
        lblBillingZip.Text = GetString("ShoppingCartOrderAddresses.BillingZIP");

        lblShippingAddr1.Text = GetString("ShoppingCartOrderAddresses.BillingAddrLine");
        lblShippingAddr2.Text = GetString("ShoppingCartOrderAddresses.BillingAddrLine2");
        //lblShippingAddr2.Style["display"] = "none";
        lblShippingCity.Text = GetString("ShoppingCartOrderAddresses.BillingCity");
        lblShippingZip.Text = GetString("ShoppingCartOrderAddresses.BillingZIP");

        // Mark required fields
        if (this.ShoppingCartControl.RequiredFieldsMark != "")
        {
            string mark = this.ShoppingCartControl.RequiredFieldsMark;
            this.lblMark1.Text = mark;
            this.lblMark2.Text = mark;
            this.lblMark3.Text = mark;
            this.lblMark4.Text = mark;
            this.lblMark5.Text = mark;
            this.lblMark6.Text = mark;
            this.passStrength.RequiredFieldMark = mark;
            this.lblMark8.Text = mark;
            //this.lblMark12.Text = mark;
            //this.lblMark13.Text = mark;
            //this.lblMark14.Text = mark;
            this.lblMark15.Text = mark;
            this.lblMark16.Text = mark;
            this.lblMark17.Text = mark;
            //this.lblMark18.Text = mark;
            this.lblMark19.Text = mark;
            //this.lblMark20.Text = mark;
        }
    }

    #endregion

    #region Private methods

    private bool FieldHasValue(TextBox field)
    {
        return String.IsNullOrWhiteSpace(field.Text);
    }

    private bool FieldsHasValue(params TextBox[] fields)
    {
        return fields.All(FieldHasValue);
    }

    private int GetCurrentNodeId()
    {
        TreeNode currentDoc = DocumentContext.CurrentDocument;
        return (currentDoc != null ? currentDoc.NodeID : 0);
    }

    private string GetCurrentCulture()
    {
        TreeNode currentDoc = DocumentContext.CurrentDocument;
        return (currentDoc != null ? currentDoc.DocumentCulture : null);
    }

    #endregion

    #region Override

    /// <summary>
    /// Validate values in textboxes.
    /// </summary>
    public override bool IsValid()
    {
        var val = new Validator();
        string result = null;

        //var validSignInForm = !String.IsNullOrWhiteSpace(txtUsername.Text);
        //var validNewCustomerInfo = FieldsHasValue(txtFirstName1, txtBillingZip, txtBillingCity, txtEmail2) && !String.IsNullOrWhiteSpace(passStrength.Text) && !passStrength.Text.Equals(txtConfirmPsswd.Text);
        //var validExistingCustomerInfo = FieldsHasValue(txtFirstName1, txtBillingZip, txtBillingCity, txtEmail2);

        //if (this.plcAccount.Visible)
        //{
        #region Validate SignIn data
        //result = val.NotEmpty(txtUsername.Text.Trim(), GetString("ShoppingCartCheckRegistration.ErrorMissingUsername")).Result;

        //if ((result != null) && (result != ""))
        //{
        //    lblError.Text = result;
        //    lblError.Visible = true;
        //    return false;
        //}
        #endregion

        bool newCustomer = false;
        bool newUser = plcPassword.Visible;
        CustomerInfo ci = CustomerInfoProvider.GetCustomerInfoByUserID(this.ShoppingCartControl.UserInfo.UserID);
        if (ci == null)
            newCustomer = true;

        // Check 'New registration' section
        if (newCustomer)
        {
            result = val.NotEmpty(txtFirstName1.Text.Trim(), GetString("ShoppingCartCheckRegistration.FirstNameErr"))
                //.NotEmpty(txtLastName1.Text.Trim(), GetString("ShoppingCartCheckRegistration.LastNameErr"))
                .NotEmpty(txtEmail2.Text.Trim(), GetString("ShoppingCartCheckRegistration.EmailErr"))
                .NotEmpty(txtBillingAddr1.Text.Trim(), GetString("ShoppingCartCheckRegistration.adressdErr"))
                //  .NotEmpty(txtBillingNumber.Text.Trim(), GetString("ShoppingCartCheckRegistration.NumErr"))
                .NotEmpty(txtBillingCity.Text.Trim(), GetString("ShoppingCartCheckRegistration.CityErr"))
                .NotEmpty(txtBillingZip.Text.Trim(), GetString("ShoppingCartCheckRegistration.ZipErr")).Result;

            if (!rdbSameAddress.Checked && rdbDifferentAddress.Checked && result == "")
            //if (pnlShippingAdress.Visible && result == "")
            {
                result = val.NotEmpty(txtShippingCity.Text.Trim(), GetString("ShoppingCartCheckRegistration.CityErr"))
                                   .NotEmpty(txtShippingZip.Text.Trim(), GetString("ShoppingCartCheckRegistration.ZipErr")).Result;

            }
        }
        else
        {
            //if (CMSContext.CurrentUser.UserName == "public")
            //{
            result = val.NotEmpty(txtFirstName1.Text.Trim(), GetString("ShoppingCartCheckRegistration.FirstNameErr"))
                //.NotEmpty(txtLastName1.Text.Trim(), GetString("ShoppingCartCheckRegistration.LastNameErr"))
                .NotEmpty(txtEmail2.Text.Trim(), GetString("ShoppingCartCheckRegistration.EmailErr"))
                .NotEmpty(txtBillingAddr1.Text.Trim(), GetString("ShoppingCartCheckRegistration.adressdErr"))
                // .NotEmpty(txtBillingNumber.Text.Trim(), GetString("ShoppingCartCheckRegistration.NumErr"))
                .NotEmpty(txtBillingCity.Text.Trim(), GetString("ShoppingCartCheckRegistration.CityErr"))
                .NotEmpty(txtBillingZip.Text.Trim(), GetString("ShoppingCartCheckRegistration.ZipErr")).Result;
            //}
            if (!rdbSameAddress.Checked && rdbDifferentAddress.Checked && result == "")
            //if (pnlShippingAdress.Visible && result == "")
            {
                result = val.NotEmpty(txtShippingCity.Text.Trim(), GetString("ShoppingCartCheckRegistration.CityErr"))
                                   .NotEmpty(txtShippingZip.Text.Trim(), GetString("ShoppingCartCheckRegistration.ZipErr")).Result;
            }


        }
        if (result == "")
        {
            result = val.NotEmpty(txtBillingAddr1.Text.Trim(), GetString("ShoppingCartCheckRegistration.adressdErr"))
                //  .NotEmpty(txtBillingNumber.Text.Trim(), GetString("ShoppingCartCheckRegistration.NumErr"))
                  .Result;
            if (!rdbSameAddress.Checked && rdbDifferentAddress.Checked && result == "")
            //if (pnlShippingAdress.Visible && result == "")
            {
                result = val.NotEmpty(txtShippingAddr1.Text.Trim(), GetString("ShoppingCartCheckRegistration.adressdErr"))
                    .NotEmpty(txtShippingNumber.Text.Trim(), GetString("ShoppingCartCheckRegistration.NumErr")).Result;
            }
        }

        if (result == "" && mRequireOrgTaxRegIDs && chkCorporateBody.Checked)
        {
            result = val.NotEmpty(txtCompany1.Text.Trim(), GetString("ShoppingCartCheckRegistration.CompanyErr")).Result;
            if ((result == "") && plcOrganizationID.Visible)
            {
                result = val.NotEmpty(txtOrganizationID.Text.Trim(), GetString("ShoppingCartCheckRegistration.OrganizationIDErr")).Result;
            }

            if ((result == "") && plcTaxRegistrationID.Visible)
            {
                result = val.NotEmpty(txtTaxRegistrationID.Text.Trim(), GetString("ShoppingCartCheckRegistration.TaxRegistrationIDErr")).Result;
            }
        }
        if (result == "")
        {
            if (!ValidationHelper.IsEmail(txtEmail2.Text.Trim()))
            {
                lblEmail2Err.Text = GetString("ShoppingCartCheckRegistration.EmailErr");

                lblEmail2Err.Visible = true;
                lblError.Text = GetString("ShoppingCartCheckRegistration.EmailErr");
                lblError.Visible = true;
            }

            if (newCustomer && newUser)
            {
                // Password and confirmed password must be same
                if (passStrength.Text != txtConfirmPsswd.Text)
                {
                    lblPsswdErr.Text = GetString("ShoppingCartCheckRegistration.DifferentPsswds");
                    passStrength.TextBoxClass = "test";

                    lblPsswdErr.Visible = true;
                }

                // Check policy
                if (!passStrength.IsValid())
                {
                    lblPsswdErr.Text = "mot de passe";//UserInfoProvider.GetPolicyViolationMessage(CMSContext.CurrentSiteName);
                    passStrength.TextBoxClass = "test";
                    lblPsswdErr.Visible = true;
                }
            }

            if ((!DataHelper.IsEmpty(lblEmail2Err.Text.Trim())) || (!DataHelper.IsEmpty(lblPsswdErr.Text.Trim())))
            {
                return false;
            }
        }
        if (result == "")
        {
            if (!chkAcceptCGV.Checked)
                result = GetString("ShoppingCartCheckRegistration.ShouldAcceptCGV");
        }
        if (!String.IsNullOrWhiteSpace(result))
        {
            Session.Add("Track", "01");
            lblError.Text = result;
            lblError.Visible = true;
            //pnlShippingAdress.Visible = rdbDifferentAddress.Checked;
            pnlShippingAdress.Style.Add("display", rdbDifferentAddress.Checked ? "block" : "none");
            return false;
        }
        //}

        #region Refactored
        //else //If plcAccount.Visible
        /*{
            // Validate customer data
            result = val.NotEmpty(txtEditFirst.Text.Trim(), GetString("ShoppingCartCheckRegistration.FirstNameErr"))
                    .NotEmpty(txtEditLast.Text.Trim(), GetString("ShoppingCartCheckRegistration.LastNameErr"))
                    .IsEmail(txtEditEmail.Text.Trim(), GetString("ShoppingCartCheckRegistration.EmailErr")).Result;

            if (result == "" && mRequireOrgTaxRegIDs && chkEditCorpBody.Checked)
            {
                result = val.NotEmpty(txtEditCompany.Text.Trim(), GetString("ShoppingCartCheckRegistration.CompanyErr")).Result;
                // Check organization id only if visible
                if ((result == "") && plcEditOrgID.Visible)
                {
                    result = val.NotEmpty(txtEditOrgID.Text.Trim(), GetString("ShoppingCartCheckRegistration.OrganizationIDErr")).Result;
                }
                // Check tax id only if visible
                if ((result == "") && plcEditTaxRegID.Visible)
                {
                    result = val.NotEmpty(txtEditTaxRegID.Text.Trim(), GetString("ShoppingCartCheckRegistration.TaxRegistrationIDErr")).Result;
                }
            }
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
         */
        #endregion

        return true;
    }

    /// <summary>
    /// Process valid values of this step.
    /// </summary>
    public override bool ProcessStep()
    {
        string siteName = SiteContext.CurrentSiteName;

        // Existing account
        if (txtUsername.Visible && !String.IsNullOrWhiteSpace(txtUsername.Text))
        {
            var loginResult = AuthenticateThenRedirectToCustomerInfoEdition();
            if (!loginResult)
            {
                lblError.Text = GetString("ShoppingCartCheckRegistration.LoginFailed");
                lblError.Visible = true;
                //return false;
            }
            // Return false to get to Edit customer page
            return false;
        }

        bool newUser = this.ShoppingCartControl.UserInfo.IsPublic();
        bool newCustomer = false;

        CustomerInfo ci = CustomerInfoProvider.GetCustomerInfoByUserID(this.ShoppingCartControl.UserInfo.UserID);
        if (ci == null)
            newCustomer = true;

        txtEmail2.Text = txtEmail2.Text.Trim();
        pnlCompanyAccount1.Visible = chkCorporateBody.Checked;

        // New customer & user
        if (newCustomer && newUser)
        {
            EventLogProvider.LogInformation("newCustomer && newUser", "I");

            //new user registration
            var ui = CreateUser();
            if (ui == null)
                return false;

            //New customer
            ci = CreateCustomer(ui);
            if (ci == null)
                return false;

            mCustomerId = ci.CustomerID;

            //SignIn User
            if (ui.UserEnabled)
                SignInUser(ui);

            this.ShoppingCart.ShoppingCartCustomerID = ci.CustomerID;
            mCustomerId = ci.CustomerID;

            // Send new registration notification email
            if (this.ShoppingCartControl.SendNewRegistrationNotificationToAddress != "")
            {
                SendRegistrationNotification(ui);
            }

            //Track new customer
            this.ShoppingCartControl.TrackActivityCustomerRegistration(ci, MembershipContext.AuthenticatedUser, this.ContactID, siteName, RequestContext.CurrentRelativePath);
        }
        else if (!newCustomer && !newUser)//existing user & customer
        {
            EventLogProvider.LogInformation("existingCustomer && existingUser", "I");

            //var ui = UserInfoProvider.GetUserInfo(this.ShoppingCartControl.UserInfo.UserID);
            UpdateCustomerAndUser(this.ShoppingCartControl.UserInfo, ci);

            // Set the shopping cart customer ID
            this.ShoppingCart.ShoppingCartCustomerID = ci.CustomerID;
            mCustomerId = ci.CustomerID;
        }
        else if (newCustomer)//new customer && existing user
        {
            EventLogProvider.LogInformation("newCustomer && existingUser", "I");
            //New customer
            //var ui = UserInfoProvider.GetUserInfo(this.ShoppingCartControl.UserInfo.UserID);
            ci = CreateCustomer(this.ShoppingCartControl.UserInfo);
            if (ci == null)
                return false;

            this.ShoppingCart.ShoppingCartCustomerID = ci.CustomerID;
            mCustomerId = ci.CustomerID;

            //Track new customer
            this.ShoppingCartControl.TrackActivityCustomerRegistration(ci, MembershipContext.AuthenticatedUser, this.ContactID, siteName, RequestContext.CurrentRelativePath);
        }

        SaveAddresses();

        //Subscribe to newsletter if applicable
        //PDO_newsletter_fr-FR PDO_newsletter_fr-BE
      //  const string newsletterName = "PDO_newsletter_fr-BE";
        if (chkSubscribe.Checked)
        {
          //  Save(newsletterName, SaveSubscriber());
        }

        try
        {
            if (!this.ShoppingCartControl.IsInternalOrder)
            {
                ShoppingCartInfoProvider.SetShoppingCartInfo(this.ShoppingCart);
            }
            return true;
        }
        catch (Exception ex)
        {
            EventLogProvider.LogException("Pdo Shopping Cart", "E", ex);
            return false;
        }
    }

    #endregion

    #region Private Methods - Address

    private void OnRdbAddressCheckBoxesChange()
    {
    }

    private void SaveAddresses()
    {
        EventLogProvider.LogInformation("Reg - SaveAddresses", "I");

        // Process billing address
        //------------------------
        SaveBillingAddress();

        // If shopping cart does not need shipping -- All products in shopping cart have property "Needs shipping unchecked"
        //if (!ShippingOptionInfoProvider.IsShippingNeeded(this.ShoppingCart))
        //{
        //    EventLogProvider.LogInformation("not IsShippingNeeded", "I");

        //    //EventLogProvider.LogInformation("shopping cart does not need shipping", "I");
        //    this.ShoppingCart.ShoppingCartShippingAddressID = 0;
        //}
        //else
        {
            //EventLogProvider.LogInformation("shopping cart need shipping", "I");
            if (rdbDifferentAddress.Checked)
                SaveShippingAddress();
            else
                this.ShoppingCart.ShoppingCartShippingAddressID = this.ShoppingCart.ShoppingCartBillingAddressID;
        }

        // Save information about company address or not (according to the site settings)
        ShoppingCart.ShoppingCartCompanyAddressID = ECommerceSettings.UseExtraCompanyAddress(SiteContext.CurrentSiteName) ? ShoppingCart.ShoppingCartBillingAddressID : 0;
    }

    private void SaveBillingAddress()
    {
        EventLogProvider.LogInformation("Reg - SaveBillingAddress", "I");

        bool isNew = false;

        AddressInfo oldAi = null; ;
        if (this.ShoppingCart.ShoppingCartBillingAddressID > 0)
        {
            EventLogProvider.LogInformation("Reg - this.ShoppingCart.ShoppingCartBillingAddressID > 0", "I");
            oldAi =
                AddressInfoProvider.GetAddressInfo(Convert.ToInt32(this.ShoppingCart.ShoppingCartBillingAddressID));
            if (oldAi == null)
                isNew = true;
        }
        else
        {
            EventLogProvider.LogInformation("Reg - this.ShoppingCart.ShoppingCartBillingAddressID == 0", "I");
            isNew = true;
        }
        var ai = new AddressInfo
        {
            AddressPersonalName = txtFirstName1.Text.Trim() + " " + txtLastName1.Text.Trim(),
            //AddressLine1 = txtBillingAddr1.Text.Trim() + " " + txtBillingNumber.Text.Trim(),
            AddressLine1 = AddressHelper.CreateAddressLineWithNumber(txtBillingAddr1.Text.Trim(), txtBillingNumber.Text.Trim()),
            AddressLine2 = txtBillingAddr2.Text.Trim(),
            AddressCity = txtBillingCity.Text.Trim(),
            AddressZip = txtBillingZip.Text.Trim(),
            AddressCountryID = Country.CountryID
        };
        ai.AddressName = AddressInfoProvider.GetAddressName(ai);

        //if (!isNew && AreEqualAddresses(ai, oldAi)) //Unchanged address -> use old address and return
        if (!isNew)
        {
            EventLogProvider.LogInformation("Reg - old address", "I");

            oldAi.AddressPersonalName = txtFirstName1.Text.Trim() + " " + txtLastName1.Text.Trim();
            //oldAi.AddressLine1 = txtBillingAddr1.Text.Trim() + " " + txtBillingNumber.Text.Trim();
            oldAi.AddressLine1 = AddressHelper.CreateAddressLineWithNumber(txtBillingAddr1.Text.Trim(), txtBillingNumber.Text.Trim());
            oldAi.AddressLine2 = txtBillingAddr2.Text.Trim();
            oldAi.AddressCity = txtBillingCity.Text.Trim();
            oldAi.AddressZip = txtBillingZip.Text.Trim();
            oldAi.AddressCountryID = Country.CountryID;
            oldAi.AddressName = AddressInfoProvider.GetAddressName(oldAi);
            AddressInfoProvider.SetAddressInfo(oldAi);

            this.ShoppingCart.ShoppingCartBillingAddressID = oldAi.AddressID;
            return;
        }

        ai.AddressIsBilling = true;
        ai.AddressIsShipping = rdbSameAddress.Checked;
        ai.AddressIsCompany = false;
        ai.AddressEnabled = true;
        ai.AddressCustomerID = mCustomerId;

        // Save new address and set it's ID to ShoppingCart
        AddressInfoProvider.SetAddressInfo(ai);
        this.ShoppingCart.ShoppingCartBillingAddressID = ai.AddressID;

        // Update current contact's address
        ModuleCommands.OnlineMarketingMapAddress(ai, this.ContactID);
    }

    private void SaveShippingAddress()
    {
        EventLogProvider.LogInformation("Reg - SaveShippingAddress", "I");

        bool isNewShipping = false;

        AddressInfo oldShippingAi = null; ;
        if (this.ShoppingCart.ShoppingCartShippingAddressID > 0 && this.ShoppingCart.ShoppingCartShippingAddressID != this.ShoppingCart.ShoppingCartBillingAddressID)
        {
            oldShippingAi = AddressInfoProvider.GetAddressInfo(Convert.ToInt32(this.ShoppingCart.ShoppingCartShippingAddressID));
            if (oldShippingAi == null)
                isNewShipping = true;
        }
        else
            isNewShipping = true;


        var shippingAi = new AddressInfo
        {
            AddressPersonalName = txtFirstName1.Text.Trim() + " " + txtLastName1.Text.Trim(),
            //AddressLine1 = txtBillingAddr1.Text.Trim() + " " + txtBillingNumber.Text.Trim(),
            AddressLine1 = AddressHelper.CreateAddressLineWithNumber(txtShippingAddr1.Text.Trim(), txtShippingNumber.Text.Trim()),
            AddressLine2 = txtShippingAddr2.Text.Trim(),
            AddressCity = txtShippingCity.Text.Trim(),
            AddressZip = txtShippingZip.Text.Trim(),
            AddressCountryID = Country.CountryID
        };
        shippingAi.AddressName = AddressInfoProvider.GetAddressName(shippingAi);
        //if (!isNewShipping && AreEqualAddresses(shippingAi, oldShippingAi)) //Unchanged address -> use old address and return
        if (!isNewShipping)
        {
            oldShippingAi.AddressPersonalName = txtFirstName1.Text.Trim() + " " + txtLastName1.Text.Trim();
            //oldShippingAi.AddressLine1 = txtBillingAddr1.Text.Trim() + " " + txtBillingNumber.Text.Trim();
            oldShippingAi.AddressLine1 = AddressHelper.CreateAddressLineWithNumber(txtShippingAddr1.Text.Trim(), txtShippingNumber.Text.Trim());
            oldShippingAi.AddressLine2 = txtShippingAddr2.Text.Trim();
            oldShippingAi.AddressCity = txtShippingCity.Text.Trim();
            oldShippingAi.AddressZip = txtShippingZip.Text.Trim();
            oldShippingAi.AddressCountryID = Country.CountryID;
            oldShippingAi.AddressName = AddressInfoProvider.GetAddressName(oldShippingAi);
            AddressInfoProvider.SetAddressInfo(oldShippingAi);

            this.ShoppingCart.ShoppingCartShippingAddressID = oldShippingAi.AddressID;
            return;
        }

        shippingAi.AddressIsBilling = false;
        shippingAi.AddressIsShipping = true;
        shippingAi.AddressIsCompany = false;
        shippingAi.AddressEnabled = true;
        shippingAi.AddressCustomerID = mCustomerId;
        //shippingAi.AddressName = AddressInfoProvider.GetAddressName(shippingAi);

        // Save address and set it's ID to ShoppingCart
        AddressInfoProvider.SetAddressInfo(shippingAi);
        this.ShoppingCart.ShoppingCartShippingAddressID = shippingAi.AddressID;
    }

    #endregion

    #region Private methods - User & customer registration

    private bool AuthenticateThenRedirectToCustomerInfoEdition()
    {
        EventLogProvider.LogInformation("AuthenticateThenRedirectToCustomerInfoEdition", "I");

        string siteName = SiteContext.CurrentSiteName;

        // Authenticate user
        UserInfo ui = AuthenticationHelper.AuthenticateUser(txtUsername.Text.Trim(), txtPsswd1.Text, SiteContext.CurrentSiteName, false);
        if (ui == null)
            return false;

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

        LoadCurrentStep();

        return true;
    }

    private UserInfo CreateUser()
    {
        //EventLogProvider.LogInformation("New User", "I");
        var siteName = SiteContext.CurrentSiteName;
        // Check if user exists
        var ui = UserInfoProvider.GetUserInfo(txtEmail2.Text);
        if (ui != null)
        {
            lblError.Visible = true;
            lblError.Text = GetString("ShoppingCartUserRegistration.ErrorUserExists");
            return null;
        }

        // Check all sites where user will be assigned
        string checkSites = (String.IsNullOrEmpty(this.ShoppingCartControl.AssignToSites))
                                ? SiteContext.CurrentSiteName
                                : this.ShoppingCartControl.AssignToSites;
        if (!UserInfoProvider.IsEmailUnique(txtEmail2.Text.Trim(), checkSites, 0))
        {
            lblError.Visible = true;
            lblError.Text = GetString("UserInfo.EmailAlreadyExist");
            return null;
        }

        // Create new customer and user account and sign in
        // User
        ui = new UserInfo
        {
            UserName = txtEmail2.Text.Trim(),
            Email = txtEmail2.Text.Trim(),
            FirstName = txtFirstName1.Text.Trim(),
            LastName = txtLastName1.Text.Trim()
        };
        ui.FullName = ui.FirstName + " " + ui.LastName;
        ui.Enabled = true;
        ui.UserIsGlobalAdministrator = false;
        ui.UserURLReferrer = MembershipContext.AuthenticatedUser.URLReferrer;
        ui.UserCampaign = AnalyticsHelper.Campaign;
        ui.UserSettings.UserRegistrationInfo.IPAddress = HttpContext.Current.Request.UserHostAddress;
        ui.UserSettings.UserRegistrationInfo.Agent = HttpContext.Current.Request.UserAgent;
        if (rdbF.Checked)
        {
            ui.UserSettings.UserGender = 2;
        }
        else
        {
            ui.UserSettings.UserGender = 1;
        }

        try
        {
            UserInfoProvider.SetPassword(ui, passStrength.Text);

            string[] siteList;

            // If AssignToSites field set
            if (!String.IsNullOrEmpty(this.ShoppingCartControl.AssignToSites))
            {
                siteList = this.ShoppingCartControl.AssignToSites.Split(';');
            }
            else // If not set user current site 
            {
                siteList = new string[] { siteName };
            }

            foreach (string site in siteList)
            {
                UserInfoProvider.AddUserToSite(ui.UserName, site);

                // Add user to roles
                if (this.ShoppingCartControl.AssignToRoles != "")
                {
                    AssignUserToRoles(ui.UserName, this.ShoppingCartControl.AssignToRoles, site);
                }
            }

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
            return null;
        }
        return ui;
    }

    private void SignInUser(UserInfo ui)
    {
        var siteName = SiteContext.CurrentSiteName;
        CMSContext.AuthenticateUser(ui.UserName, false);
        ShoppingCart.User = ui;

        // Log "login" activity
        ContactID = ModuleCommands.OnlineMarketingGetUserLoginContactID(ui);
        Activity activity = new ActivityUserLogin(ContactID, ui, DocumentContext.CurrentDocument, AnalyticsContext.ActivityEnvironmentVariables);
        activity.Log();
    }

    private CustomerInfo CreateCustomer(UserInfo ui)
    {
        var siteName = SiteContext.CurrentSiteName;

        var ci = new CustomerInfo
        {
            CustomerUserID = this.ShoppingCartControl.UserInfo.UserID,
            CustomerSiteID = 0,
            CustomerEnabled = true,
            CustomerFirstName = this.txtFirstName1.Text.Trim(),
            CustomerLastName = this.txtLastName1.Text.Trim(),
            CustomerEmail = this.txtEmail2.Text.Trim(),
            CustomerCompany = "",
            CustomerOrganizationID = "",
            CustomerTaxRegistrationID = ""
        };

        if (chkCorporateBody.Checked)
        {
            ci.CustomerCompany = this.txtCompany1.Text.Trim();
            if (mShowOrganizationIDField)
            {
                ci.CustomerOrganizationID = this.txtOrganizationID.Text.Trim();
            }
            if (mShowTaxRegistrationIDField)
            {
                ci.CustomerTaxRegistrationID = this.txtTaxRegistrationID.Text.Trim();
            }
        }

        ci.CustomerUserID = ui.UserID;
        ci.CustomerSiteID = CMSContext.CurrentSiteID;
        ci.CustomerEnabled = true;
        ci.CustomerCreated = DateTime.Now;
        //if (!String.IsNullOrWhiteSpace(ddlFrom.SelectedValue))
         //   ci["CustomerHowHaveYouKnown"] = ddlFrom.SelectedValue;

        CustomerInfoProvider.SetCustomerInfo(ci);

        // Track successful registration conversion
        string name = this.ShoppingCartControl.RegistrationTrackConversionName;
        ECommerceHelper.TrackRegistrationConversion(this.ShoppingCart.SiteName, name);

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

        return ci;
    }

    private bool UpdateCustomerAndUser(UserInfo ui, CustomerInfo ci)
    {
        var siteName = SiteContext.CurrentSiteName;
        // Old email address
        string oldEmail = ci.CustomerEmail.ToLower();

        ci.CustomerFirstName = this.txtFirstName1.Text.Trim();
        ci.CustomerLastName = this.txtLastName1.Text.Trim();
        ci.CustomerEmail = this.txtEmail2.Text.Trim();

        ci.CustomerCompany = "";
        ci.CustomerOrganizationID = "";
        ci.CustomerTaxRegistrationID = "";
        if (chkCorporateBody.Checked)
        {
            ci.CustomerCompany = this.txtCompany1.Text.Trim();
            if (mShowOrganizationIDField)
            {
                ci.CustomerOrganizationID = this.txtOrganizationID.Text.Trim();
            }
            if (mShowTaxRegistrationIDField)
            {
                ci.CustomerTaxRegistrationID = this.txtTaxRegistrationID.Text.Trim();
            }
        }

       // if (!String.IsNullOrWhiteSpace(ddlFrom.SelectedValue))
         //   ci["CustomerHowHaveYouKnown"] = ddlFrom.SelectedValue;

        // Update customer data
        CustomerInfoProvider.SetCustomerInfo(ci);

        // Update corresponding user email when required
        if (oldEmail != ci.CustomerEmail.ToLower())
        {
            UserInfo user = UserInfoProvider.GetUserInfo(ci.CustomerUserID);
            if (user != null)
            {
                user.Email = ci.CustomerEmail;
                UserInfoProvider.SetUserInfo(user);
            }
        }

        // Log "customer registration" activity and update contact profile
        //string siteName = CMSContext.CurrentSiteName;
        var activityCustomerRegistration = new ActivityCustomerRegistration(ci, MembershipContext.AuthenticatedUser, AnalyticsContext.ActivityEnvironmentVariables);
        if (activityCustomerRegistration.Data != null)
        {
            if (ContactID <= 0)
            {
                activityCustomerRegistration.Data.ContactID = ModuleCommands.OnlineMarketingGetUserLoginContactID(ui);
            }
            activityCustomerRegistration.Log();
        }

        return true;
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
        if (!string.IsNullOrEmpty(siteName))
        {
            if (roleList.Length > 0)
            {
                foreach (string t in roleList)
                {
                    if (RoleInfoProvider.RoleExists(t, siteName))
                    {
                        UserInfoProvider.AddUserToRole(userName, t, siteName);
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
        if ((ui != null) && (currentSite != null) && (this.ShoppingCartControl.SendNewRegistrationNotificationToAddress != ""))
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

            var ev = new EventLogProvider();

            if (mEmailTemplate == null)
            {
                // Email template not exist
                ev.LogEvent("E", DateTime.Now, "RegistrationForm", "GetEmailTemplate", HTTPHelper.GetAbsoluteUri());
            }

            // Initialize email message
            EmailMessage message = new EmailMessage();
            message.EmailFormat = EmailFormatEnum.Default;

            message.From = ECommerceSettings.SendEmailsFrom(currentSite.SiteName);
            message.Subject = GetString("RegistrationForm.EmailSubject");

            message.Recipients = this.ShoppingCartControl.SendNewRegistrationNotificationToAddress;
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

    #endregion

    #region Newsletter method


    #endregion
}