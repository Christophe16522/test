using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Web;
using System.Web.UI;
using System.Linq;
using System.Web.UI.WebControls;
using CMS.CMSHelper;
using CMS.DataEngine;
using CMS.Ecommerce;
using CMS.GlobalHelper;
using CMS.PortalControls;
using CMS.PortalEngine;
using CMS.SettingsProvider;
using CMS.SiteProvider;
using CMS.Helpers;
using CMS.Protection;
using CMS.Membership;
using CMS.MacroEngine;
using CMS.Globalization;

public partial class CMSWebPartUpdateRegistrationForm : CMSAbstractWebPart
{
    #region "Text properties"

    /// <summary>
    /// Gets or sets the Skin ID.
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
    /// Gets or sets the first name text.
    /// </summary>
    public string FirstNameText
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("FirstNameText"), ResHelper.LocalizeString("{$Webparts_Membership_RegistrationForm.FirstName$}"));
        }
        set
        {
            SetValue("FirstNameText", value);
            //lblFirstName.Text = value;
        }
    }


    /// <summary>
    /// Gets or sets the last name text.
    /// </summary>
    public string LastNameText
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("LastNameText"), ResHelper.LocalizeString("{$Webparts_Membership_RegistrationForm.LastName$}"));
        }
        set
        {
            SetValue("LastNameText", value);
            //lblLastName.Text = value;
        }
    }


    /// <summary>
    /// Gets or sets the e-mail text.
    /// </summary>
    public string EmailText
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("EmailText"), ResHelper.LocalizeString("{$Webparts_Membership_RegistrationForm.Email$}"));
        }
        set
        {
            SetValue("EmailText", value);
            //lblEmail.Text = value;
        }
    }


    /// <summary>
    /// Gets or sets the password text.
    /// </summary>
    public string PasswordText
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("PasswordText"), ResHelper.LocalizeString("{$Webparts_Membership_RegistrationForm.Password$}"));
        }
        set
        {
            SetValue("PasswordText", value);
            //lblPassword.Text = value;
        }
    }


    /// <summary>
    /// Gets or sets the confirmation password text.
    /// </summary>
    public string ConfirmPasswordText
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("ConfirmPasswordText"), ResHelper.LocalizeString("{$Webparts_Membership_RegistrationForm.ConfirmPassword$}"));
        }
        set
        {
            SetValue("ConfirmPasswordText", value);
            //lblConfirmPassword.Text = value;
        }
    }


    /// <summary>
    /// Gets or sets the button text.
    /// </summary>
    public string ButtonText
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("ButtonText"), ResHelper.LocalizeString("{$Webparts_Membership_RegistrationForm.Button$}"));
        }
        set
        {
            SetValue("ButtonText", value);
            btnOk.Text = value;
        }
    }


    /// <summary>
    /// Gets or sets the captcha label text.
    /// </summary>
    public string CaptchaText
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("CaptchaText"), ResHelper.LocalizeString("{$Webparts_Membership_RegistrationForm.Captcha$}"));
        }
        set
        {
            SetValue("CaptchaText", value);
            lblCaptcha.Text = value;
        }
    }


    /// <summary>
    /// Gets or sets registration approval page URL.
    /// </summary>
    public string ApprovalPage
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("ApprovalPage"), "");
        }
        set
        {
            SetValue("ApprovalPage", value);
        }
    }

    #endregion


    #region "Registration properties"

    /// <summary>
    /// Gets or sets the value that indicates whether email to user should be sent.
    /// </summary>
    public bool SendWelcomeEmail
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("SendWelcomeEmail"), true);
        }
        set
        {
            SetValue("SendWelcomeEmail", value);
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
    /// Gets or sets the sender email (from).
    /// </summary>
    public string FromAddress
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("FromAddress"), SettingsKeyInfoProvider.GetStringValue(SiteContext.CurrentSiteName + ".CMSNoreplyEmailAddress"));
        }
        set
        {
            SetValue("FromAddress", value);
        }
    }


    /// <summary>
    /// Gets or sets the recipient email (to).
    /// </summary>
    public string ToAddress
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("ToAddress"), SettingsKeyInfoProvider.GetStringValue(SiteContext.CurrentSiteName + ".CMSAdminEmailAddress"));
        }
        set
        {
            SetValue("ToAddress", value);
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether after successful registration is 
    /// notification email sent to the administrator 
    /// </summary>
    public bool NotifyAdministrator
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("NotifyAdministrator"), false);
        }
        set
        {
            SetValue("NotifyAdministrator", value);
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


    /// <summary>
    /// Gets or sets the message which is displayed after successful registration.
    /// </summary>
    public string DisplayMessage
    {
        get
        {
            return ValidationHelper.GetString(GetValue("DisplayMessage"), "");
        }
        set
        {
            SetValue("DisplayMessage", value);
        }
    }


    /// <summary>
    /// Gets or set the url where is user redirected after successful registration.
    /// </summary>
    public string RedirectToURL
    {
        get
        {
            return ValidationHelper.GetString(GetValue("RedirectToURL"), "");
        }
        set
        {
            SetValue("RedirectToURL", value);
        }
    }


    /// <summary>
    /// Gets or sets value that indicates whether the captcha image should be displayed.
    /// </summary>
    public bool DisplayCaptcha
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("DisplayCaptcha"), false);
        }
        set
        {
            SetValue("DisplayCaptcha", value);
            plcCaptcha.Visible = value;
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
    /// Gets or sets the password minimal length.
    /// </summary>
    public int PasswordMinLength
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("PasswordMinLength"), 0);
        }
        set
        {
            SetValue("PasswordMinLength", 0);
        }
    }

    #endregion


    #region "Conversion properties"

    /// <summary>
    /// Gets or sets the conversion track name used after successful registration.
    /// </summary>
    public string TrackConversionName
    {
        get
        {
            return ValidationHelper.GetString(GetValue("TrackConversionName"), "");
        }
        set
        {
            if(value.Length > 400)
            {
                value = value.Substring(0, 400);
            }
            SetValue("TrackConversionName", value);
        }
    }


    /// <summary>
    /// Gets or sets the conversion value used after successful registration.
    /// </summary>
    public double ConversionValue
    {
        get
        {
            return ValidationHelper.GetDoubleSystem(GetValue("ConversionValue"), 0);
        }
        set
        {
            SetValue("ConversionValue", value);
        }
    }

    #endregion


    #region "Methods"

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
        System.Globalization.CultureInfo currentUI = System.Globalization.CultureInfo.CurrentUICulture;

        if(StopProcessing)
        {
            // Do not process
        }
        else
        {
            ShowBillingCountryList();
            // Set default visibility
            pnlForm.Visible = true;
            btnOk.Text = ButtonText;
            lblCaptcha.Text = CaptchaText;
            btnOk.CssClass = GetString("cssvalid");
            btnOk.ValidationGroup = ClientID + "_registration";


            // Set SkinID
            if(!StandAlone && (PageCycle < PageCycleEnum.Initialized))
            {
                SetSkinID(SkinID);
            }

            plcCaptcha.Visible = DisplayCaptcha;

            if(CurrentUser.IsAuthenticated())
            {
                CustomerInfo CurrentCustomer = ECommerceContext.CurrentCustomer;

                txtFirstName.Text = CurrentUser.FirstName;
                txtLastName.Text = CurrentUser.LastName;
                object password = CurrentUser.GetValue("UserPassword").ToString();
                //txtTelephone.Text = CurrentUser.GetStringValue("Telephone", string.Empty);
                txtTelephone.Text = CurrentCustomer.GetStringValue("CustomerPhone", string.Empty);
                txtPassword.Text = password.ToString();
                txtConfirmPassword.Text = password.ToString();
                string civilite = string.Empty;
                try
                {
                    civilite = CurrentUser.GetValue("Civilite").ToString();
                }
                catch
                {
                }

                ShowCivility();
                payement_option.SelectedValue = civilite;

                txtEmail.Text = CurrentUser.Email;
                txtEmail.Enabled = false;

                txtEmail.ToolTip = GetString("adressemail");
                txtFirstName.ToolTip = GetString("nom");
                wmFirstName.WatermarkText = GetString("nom");
                txtLastName.ToolTip = GetString("prenom");
                wmLastName.WatermarkText = GetString("prenom");
                txtConfirmPassword.ToolTip = GetString("confirmdp");
                txtnumero.ToolTip = GetString("numerorue");
                wmnumero.WatermarkText = GetString("numerorue");
                txtadresse1.ToolTip = GetString("adresse1");
                wmadresse1.WatermarkText = GetString("adresse1");
                txtadresse2.ToolTip = GetString("adresse2");
                wmadresse2.WatermarkText = GetString("adresse2");
                txtcp.ToolTip = GetString("cp");
                txtville.ToolTip = GetString("ville");
                txtnomsociete.ToolTip = GetString("nomsociete");
                wmnomsociete.WatermarkText = GetString("nomsociete");
                txtTva.ToolTip = GetString("tva");
                Lblshipping.Text = GetString("adresselivraison");
                Lblbilling.Text = GetString("adressefacturation");
                lblSociete.Text = GetString("societe");
                rbnon.Text = string.Format("{0}   &nbsp;", GetString("non"));
                rboui.Text = string.Format("{0}   &nbsp;", GetString("oui"));
                txtnomsociete.Text = CurrentCustomer.CustomerCompany.Trim();
                txtTva.Text = CurrentCustomer.CustomerTaxRegistrationID;
                rbnon.Checked = (CurrentCustomer.CustomerCompany.Trim() == string.Empty);
                rboui.Checked = !rbnon.Checked;
                lblAdresses.Text = GetString("addresses");
                lnkPassword.Text = GetString("pwdmanage");
                ReloadDataAdress();
            }
        }
        // Message si il n'y a pas d'adresse
        int idCustomer = ECommerceContext.CurrentCustomer.CustomerID;
        SqlConnection con3 = new SqlConnection(ConfigurationManager.ConnectionStrings["CMSConnectionString"].ConnectionString);
        con3.Open();
        var stringQuery = "select count(AddressID) as NbAdress from COM_Address WHERE COM_Address.AddressEnabled = 'true'  AND COM_Address.AddressCustomerID  = " + idCustomer;
        SqlCommand cmd3 = new SqlCommand(stringQuery, con3);
        int nb = (int)cmd3.ExecuteScalar();
        con3.Close();
        if(nb == 0)
        {
            Label1.Visible = true;
            Label1.Text = GetString("pasdadresse");
        }
        else
        {
            Label1.Visible = false;
        }

    }


    /// <summary>
    /// Sets SkinID.
    /// </summary>
    private void SetSkinID(string skinId)
    {
        if(skinId != "")
        {
            txtFirstName.SkinID = skinId;
            txtLastName.SkinID = skinId;
            txtEmail.SkinID = skinId;
            txtPassword.SkinID = skinId;
            txtConfirmPassword.SkinID = skinId;
            txtTva.SkinID = skinId;
            btnOk.SkinID = skinId;
        }
    }


    private int mCustomerId = 0;


    /// <summary>
    /// OK click handler (Proceed registration).
    /// </summary>
    protected void btnOK_Click(object sender, EventArgs e)
    {
        System.Globalization.CultureInfo currentUI = System.Globalization.CultureInfo.CurrentUICulture;

        if((PageManager.ViewMode == ViewModeEnum.Design) || (HideOnCurrentPage) || (!IsVisible))
        {
            // Do not process
        }
        else
        {
            String siteName = SiteContext.CurrentSiteName;

            #region "Banned IPs"

            // Ban IP addresses which are blocked for registration
            if(!BannedIPInfoProvider.IsAllowed(siteName, BanControlEnum.Registration))
            {
                lblError.Visible = true;
                lblError.Text = GetString("banip.ipisbannedregistration");
                return;
            }

            #endregion

            #region "Pr�nom"

            if(string.IsNullOrEmpty(txtFirstName.Text) || (txtFirstName.Text.ToLower() == "firstname") || (txtFirstName.Text.ToLower() == "pr�nom") || (txtFirstName.Text.ToLower() == "prenom"))
            {
                lblError.Visible = true;
                lblError.Text = GetString("errornom");
                return;
            }

            #endregion

            #region "Nom"

            if(string.IsNullOrEmpty(txtLastName.Text) || (txtLastName.Text.ToLower() == "nom") || (txtLastName.Text.ToLower() == "lastname"))
            {
                lblError.Visible = true;
                lblError.Text = GetString("errorprenom");
                return;
            }

            #endregion

            #region "T�l�phone"

            if(string.IsNullOrEmpty(txtTelephone.Text) || (txtTelephone.Text.ToLower() == "telephone"))
            {
                lblError.Visible = true;
                lblError.Text = GetString("errortelephone");
                return;
            }

            #endregion


            #region Soci�t�
            if(rboui.Checked)
            {
                if((txtnomsociete.Text == "") || (txtnomsociete.Text == "Nom soci�t�") || (txtnomsociete.Text == "Company Name"))
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

            #region "Captcha"

            // Check if captcha is required
            if(DisplayCaptcha)
            {
                // Verifiy captcha text
                if(!scCaptcha.IsValid())
                {
                    // Display error message if catcha text is not valid
                    lblError.Visible = true;
                    lblError.Text = GetString("Webparts_Membership_RegistrationForm.captchaError");
                    return;
                }
                else
                {
                    // Generate new captcha
                    scCaptcha.GenerateNew();
                }
            }

            #endregion


            // Set password            
            //UserInfoProvider.SetPassword(ui, passStrength.Text);

            // UserInfoProvider.SetPassword(ui, txtPassword.Text.Trim());
            if(!CurrentUser.IsAuthenticated())
            {
                // Set password            
                //    UserInfoProvider.SetPassword(ui, txtPassword.Text.Trim());                
            }
            else
            {
                #region "Modif User"
                //Update User
                UserInfo updateUser = CurrentUser;
                updateUser.PreferredCultureCode = "";
                updateUser.FirstName = txtFirstName.Text.Trim();
                updateUser.FullName = UserInfoProvider.GetFullName(txtFirstName.Text.Trim(), String.Empty, txtLastName.Text.Trim());
                updateUser.LastName = txtLastName.Text.Trim();
                updateUser.MiddleName = "";

                if(payement_option.SelectedValue != "0")
                {
                    updateUser.SetValue("Civilite", payement_option.SelectedValue);
                }

                //updateUser.SetValue("Telephone", txtTelephone.Text);
                updateUser.SetValue("Telephone", txtTelephone.Text);

                /*if ((txtPassword.Text != "Mot de passe") && (txtPassword.Text != "Password"))
                {
                    UserInfoProvider.SetPassword(updateUser, txtPassword.Text);
                    //updateUser.SetValue("UserPassword",txtPassword.Text);
                }*/
                UserInfoProvider.SetUserInfo(updateUser);

                //Update Customer
                CustomerInfo updateCustomer = ECommerceContext.CurrentCustomer;
                updateCustomer.CustomerUserID = updateUser.UserID;
                updateCustomer.CustomerLastName = txtLastName.Text.Trim();
                updateCustomer.CustomerFirstName = txtFirstName.Text.Trim();
                updateCustomer.CustomerEmail = txtEmail.Text.Trim();
                //add update phone
                updateCustomer.CustomerPhone = txtTelephone.Text.Trim();
                updateCustomer.CustomerEnabled = true;
                updateCustomer.CustomerLastModified = DateTime.Now;
                updateCustomer.CustomerSiteID = CMSContext.CurrentSiteID;
                updateCustomer.CustomerOrganizationID = "";
                if(rboui.Checked)
                {
                    updateCustomer.CustomerCompany = txtnomsociete.Text.Trim();
                    updateCustomer.CustomerTaxRegistrationID = txtTva.Text;
                }
                else
                {
                    updateCustomer.CustomerCompany = string.Empty;
                    updateCustomer.CustomerTaxRegistrationID = string.Empty;
                }

                if((rboui.Checked) && (txtTva.Text.Trim() != "TVA") && (txtTva.Text.Trim() != "VAT"))
                {
                    updateCustomer.CustomerTaxRegistrationID = txtTva.Text;
                    updateCustomer.CustomerCompany = txtnomsociete.Text.ToString();
                }
                else
                {
                    updateCustomer.CustomerTaxRegistrationID = "";
                    updateCustomer.CustomerCompany = "";
                }

                CustomerInfoProvider.SetCustomerInfo(updateCustomer);
                #endregion

                #region "Insert new adress / Update selected adress"
                //if (chkNewAddress.Checked)
                //{
                #region "n�"

                if((txtnumero.Text == "") || (txtnumero.Text == "Numero") || (txtnumero.Text == "Number"))
                {
                    lblErrorAdress.Visible = true;
                    lblErrorAdress.Text = GetString("errornumerorue");
                    return;
                }

                #endregion

                #region "adresse 1"

                if((txtadresse1.Text == "") || (txtadresse1.Text == "Adresse 1") || (txtadresse1.Text == "Address 1"))
                {
                    lblErrorAdress.Visible = true;
                    lblErrorAdress.Text = GetString("erroradresse1");
                    return;
                }

                #endregion

                #region "adresse 2"

                if((txtadresse2.Text == "") || (txtadresse2.Text == "Adresse 2") || (txtadresse2.Text == "Address 2"))
                {
                    lblErrorAdress.Visible = true;
                    lblErrorAdress.Text = GetString("erroradresse2");
                    return;
                }

                #endregion

                #region "CP"

                if((txtcp.Text == "") || (txtcp.Text == "CP") || (txtcp.Text == "ZIP"))
                {
                    lblErrorAdress.Visible = true;
                    lblErrorAdress.Text = GetString("errorcp");
                    return;
                }

                #endregion

                #region "Ville"

                if((txtville.Text == "") || (txtville.Text == "Ville") || (txtville.Text == "City"))
                {
                    lblErrorAdress.Visible = true;
                    lblErrorAdress.Text = GetString("errorville");
                    return;
                }

                #endregion

                #region "Pays"

                if((ddlShippingCountry.Text == "Choose your country") || (ddlShippingCountry.Text == "Choisissez votre pays"))
                {
                    lblErrorAdress.Visible = true;
                    lblErrorAdress.Text = GetString("errorchoixpays ");
                    return;
                }

                #endregion

                #region "Adresse"

                if((chkShippingAddr.Checked == false) && (chkBillingAddr.Checked == false))
                {
                    lblErrorAdress.Visible = true;
                    lblErrorAdress.Text = GetString("erroradressechek");
                    return;
                }

                #endregion

                if(txtIdAdresse.Text == "")
                {
                    #region "New adress"

                    // Create new address object
                    AddressInfo newAddress = new AddressInfo();

                    int CountryID = ValidationHelper.GetInteger(ddlShippingCountry.SelectedValue, 0);
                    CustomerInfo uc = ECommerceContext.CurrentCustomer;
                    mCustomerId = uc.CustomerID;
                    string mCustomerName = uc.CustomerFirstName + " " + uc.CustomerLastName;
                    // Set the properties
                    newAddress.AddressName = mCustomerName + " , " + txtnumero.Text + " " + txtadresse1.Text + " - " + txtcp.Text + " " + txtville.Text;
                    newAddress.AddressLine1 = txtadresse1.Text;
                    newAddress.AddressLine2 = txtadresse2.Text;
                    newAddress.AddressCity = txtville.Text;
                    newAddress.AddressZip = txtcp.Text;
                    if(chkBillingAddr.Checked)
                        newAddress.AddressIsBilling = true;
                    else
                        newAddress.AddressIsBilling = false;
                    if(chkShippingAddr.Checked)
                        newAddress.AddressIsShipping = true;
                    else
                        newAddress.AddressIsShipping = false;
                    newAddress.AddressEnabled = true;
                    newAddress.AddressPersonalName = mCustomerName;
                    newAddress.AddressCustomerID = mCustomerId;
                    newAddress.AddressCountryID = CountryID;
                    newAddress.SetValue("AddressNumber", txtnumero.Text);

                    // Create the address
                    AddressInfoProvider.SetAddressInfo(newAddress);
                    txtnumero.Text = string.Empty;
                    txtadresse1.Text = string.Empty;
                    txtadresse2.Text = string.Empty;
                    txtcp.Text = string.Empty;
                    txtville.Text = string.Empty;
                    // PnlInsertAdress.Visible = false;     
                    if(newAddress != null && newAddress.AddressIsShipping == true)
                    {
                        Session["newAddress"] = newAddress.AddressID;
                        //EventLogProvider eve = new EventLogProvider();
                        //eve.LogEvent("I", DateTime.Now, "id new address= " + Session["newAddress"], "code");
                    }

                    #endregion
                }

                else
                {

                    #region "Update selected adress"
                    /*
                        // Udpate selected adress object
                        int CountryID = ValidationHelper.GetInteger(ddlShippingCountry.SelectedValue, 0);
                        int AddressId = Convert.ToInt32(txtIdAdresse.Text);
                        AddressInfo UpdateAdress = AddressInfoProvider.GetAddressInfo(AddressId);
                        CustomerInfo uc = ECommerceContext.CurrentCustomer;
                        mCustomerId = uc.CustomerID;
                        string mCustomerName = uc.CustomerFirstName + " " + uc.CustomerLastName;
                        // Set the properties
                        UpdateAdress.AddressName = mCustomerName + " , " + txtnumero.Text + " " + txtadresse1.Text + " - " + txtcp.Text + " " + txtville.Text;
                        UpdateAdress.SetValue("AddressNumber", txtnumero.Text);
                        UpdateAdress.AddressLine1 = txtadresse1.Text;
                        UpdateAdress.AddressLine2 = txtadresse2.Text;
                        UpdateAdress.AddressCity = txtville.Text;
                        UpdateAdress.AddressZip = txtcp.Text;
                        UpdateAdress.AddressIsBilling = chkBillingAddr.Checked;
                        UpdateAdress.AddressIsShipping = chkShippingAddr.Checked;                        
                        UpdateAdress.AddressEnabled = true;
                        UpdateAdress.AddressPersonalName = mCustomerName;
                        UpdateAdress.AddressCustomerID = mCustomerId;
                        UpdateAdress.AddressCountryID = CountryID;

                        // Save addressinfo
                        AddressInfoProvider.SetAddressInfo(UpdateAdress);
                        AddressId = UpdateAdress.AddressID;
                        */
                    #endregion

                }

                ReloadDataAdress();

                //}
                #endregion
            }

            lblError.Visible = false;
            // PnlInsertAdress.Visible = false;
        }
    }

    #endregion

    /// <summary>
    /// Reloads the form data.
    /// </summary>
    protected void ReloadDataAdress()
    {
        CustomerInfo uc = ECommerceContext.CurrentCustomer;
        mCustomerId = uc.CustomerID;
        string where = "addressCustomerID = " + mCustomerId + " and AddressEnabled = 1";
        DataSet ds = AddressInfoProvider.GetAddresses(where, null);
        if(!DataHelper.DataSourceIsEmpty(ds))
        {
            rptAdress.DataSource = ds;
            rptAdress.DataBind();
        }
    }

    protected void rptAdressItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        System.Globalization.CultureInfo currentUI = System.Globalization.CultureInfo.CurrentUICulture;
        AjaxControlToolkit.ConfirmButtonExtender cbe = new AjaxControlToolkit.ConfirmButtonExtender();
        cbe = (AjaxControlToolkit.ConfirmButtonExtender)e.Item.FindControl("ConfirmButtonDelete");

        if(cbe != null)
        {
            cbe.ConfirmText = GetString("msgconfirm");
        }

        var drv = (System.Data.DataRowView)e.Item.DataItem;
        if(drv != null)
        {
            int currentAdressID = ValidationHelper.GetInteger(drv["AddressID"], 0);
            if(currentAdressID > 0)
            {
                AddressInfo ai = AddressInfoProvider.GetAddressInfo(currentAdressID);
                if(ai != null)
                {
                    var ltlAdress = e.Item.FindControl("ltlAdress") as Literal;
                    if(ltlAdress != null)
                    {
                        int countryID = ai.AddressCountryID;
                        ltlAdress.Text = string.Format("{0}, {1}", ai.AddressName, MacroContext.CurrentResolver.ResolveMacros(CountryInfoProvider.GetCountryInfo(countryID).CountryDisplayName));
                    }

                    var txtnumero = e.Item.FindControl("txtnumero") as TextBox;
                    if(txtnumero != null)
                    {
                        txtnumero.Text = ai.GetStringValue("AddressNumber", string.Empty).Trim();
                        txtnumero.ToolTip = GetString("numerorue");
                    }

                    var wmnumero = e.Item.FindControl("wmnumero") as AjaxControlToolkit.TextBoxWatermarkExtender;
                    if(wmnumero != null)
                    {
                        wmnumero.WatermarkText = GetString("numerorue");
                    }

                    var txtadresse1 = e.Item.FindControl("txtadresse1") as TextBox;
                    if(txtadresse1 != null)
                    {
                        txtadresse1.Text = ai.AddressLine1.Trim();
                        txtadresse1.ToolTip = GetString("adresse1");
                    }

                    var wmadresse1 = e.Item.FindControl("wmadresse1") as AjaxControlToolkit.TextBoxWatermarkExtender;
                    if(wmadresse1 != null)
                    {
                        wmadresse1.WatermarkText = GetString("adresse1");
                    }

                    var txtadresse2 = e.Item.FindControl("txtadresse2") as TextBox;
                    if(txtadresse2 != null)
                    {
                        txtadresse2.Text = ai.AddressLine2.Trim() == string.Empty ? string.Empty : ai.AddressLine2.Trim();
                        txtadresse2.ToolTip = GetString("adresse2");
                    }

                    var wmadresse2 = e.Item.FindControl("wmadresse2") as AjaxControlToolkit.TextBoxWatermarkExtender;
                    if(wmadresse2 != null)
                    {
                        wmadresse2.WatermarkText = GetString("adresse2");
                    }


                    var txtcp = e.Item.FindControl("txtcp") as TextBox;
                    if(txtcp != null)
                    {
                        txtcp.Text = ai.AddressZip.Trim();
                        txtcp.ToolTip = GetString("cp");
                    }

                    var wmcp = e.Item.FindControl("wmcp") as AjaxControlToolkit.TextBoxWatermarkExtender;
                    if(wmcp != null)
                    {
                        wmcp.WatermarkText = GetString("cp");
                    }

                    var txtville = e.Item.FindControl("txtville") as TextBox;
                    if(txtville != null)
                    {
                        txtville.Text = ai.AddressCity.Trim();
                        txtville.ToolTip = GetString("ville");
                    }

                    var wmville = e.Item.FindControl("wmville") as AjaxControlToolkit.TextBoxWatermarkExtender;
                    if(wmville != null)
                    {
                        wmville.WatermarkText = GetString("ville");
                    }

                    var txtCountry = e.Item.FindControl("txtCountry") as TextBox;
                    if(txtCountry != null)
                    {
                        txtCountry.Text = MacroContext.CurrentResolver.ResolveMacros(
                            GetString(CountryInfoProvider.GetCountryInfo(ai.AddressCountryID).CountryDisplayName));

                    }

                    var chkShippingAddr = e.Item.FindControl("chkShippingAddr") as CheckBox;
                    if(chkShippingAddr != null)
                    {
                        chkShippingAddr.Checked = ai.AddressIsShipping;
                    }

                    var chkBillingAddr = e.Item.FindControl("chkBillingAddr") as CheckBox;
                    if(chkBillingAddr != null)
                    {
                        chkBillingAddr.Checked = ai.AddressIsBilling;
                    }

                }
            }
        }
    }

    protected int OrderBillingAddress(int AdressID)
    {
        GeneralConnection cn = ConnectionHelper.GetConnection();
        string stringQuery = string.Format("select count(OrderID) as NbOrder from com_order where OrderBillingAddressID = " + AdressID);
        DataSet ds = cn.ExecuteQuery(stringQuery, null, CMS.SettingsProvider.QueryTypeEnum.SQLQuery, false);
        int nbOrder = (int)(ds.Tables[0].Rows[0]["NbOrder"]);
        cn.Close();
        return nbOrder;
    }

    protected int OrderShippingAddress(int AdressID)
    {
        GeneralConnection cn = ConnectionHelper.GetConnection();
        string stringQuery = string.Format("select count(OrderID) as NbOrder from com_order where OrderShippingAddressID = " + AdressID);
        DataSet ds = cn.ExecuteQuery(stringQuery, null, CMS.SettingsProvider.QueryTypeEnum.SQLQuery, false);
        int nbOrder = (int)(ds.Tables[0].Rows[0]["NbOrder"]);
        cn.Close();
        return nbOrder;
    }

    protected void rptAdressItemCommand(object source, RepeaterCommandEventArgs e)
    {
        // Get AddressId from the row
        int AddressId = ValidationHelper.GetInteger(e.CommandArgument, 0);

        // Delete selected address
        if(e.CommandName.Equals("Remove"))
        {
            int idShoppingCart = 0;
            //   EventLogProvider ev = new EventLogProvider();
            // test du nombre d'adresse
            int idCustomer = ECommerceContext.CurrentCustomer.CustomerID;
            string where, orderby;
            where = "AddressEnabled = 1 AND AddressCustomerID  = " + idCustomer;
            orderby = "AddressID";
            InfoDataSet<AddressInfo> listadresse = AddressInfoProvider.GetAddresses(where, orderby);
            if(listadresse.Tables[0].Rows.Count <= 1)
            {
                return;
            }

            else
            {

                // Delete AddressInfo object from database if address not used for order
                if((OrderBillingAddress(AddressId) == 0) && (OrderShippingAddress(AddressId) == 0))
                {
                    SqlConnection con4 = new SqlConnection(ConfigurationManager.ConnectionStrings["CMSConnectionString"].ConnectionString);
                    var query = "Select ShoppingCartID from COM_ShoppingCart where ShoppingCartShippingAddressID = " + AddressId;
                    SqlCommand cmd2 = new SqlCommand(query, con4);
                    //  ev.LogEvent("I", DateTime.Now, "ds billig&shipping=0", "code");
                    con4.Open();
                    try
                    {
                        idShoppingCart = (int)cmd2.ExecuteScalar();
                        //  ev.LogEvent("I", DateTime.Now, "dans try  " + idShoppingCart, "code");

                    }
                    catch(Exception ex)
                    {

                    }
                    con4.Close();
                    if(idShoppingCart != 0)
                    {

                        var query2 = "Delete  from COM_ShoppingCartSKU WHERE ShoppingCartID = " + idShoppingCart;
                        SqlCommand cmd1 = new SqlCommand(query2, con4);
                        cmd1.ExecuteScalar();

                        var stringQuery = "Delete  from COM_ShoppingCart WHERE ShoppingCartShippingAddressID = " + AddressId;
                        SqlCommand cmd3 = new SqlCommand(stringQuery, con4);
                        cmd3.ExecuteScalar();

                        con4.Dispose();

                    }
                    if(Session["newAddress"] != null)
                    {
                        int temp2 = Int32.Parse(Session["newAddress"].ToString());

                        if(temp2 != 0)
                        {
                            if(temp2 == AddressId)
                            {
                                Session["newAddress"] = null;
                            }

                        }
                    }
                    AddressInfoProvider.DeleteAddressInfo(AddressId);

                    //ev.LogEvent("I", DateTime.Now, "button delete enabled true", "code");
                    //
                    int id1 = ECommerceContext.CurrentCustomer.CustomerID;
                }
                // Disable AddressInfo object from database if address used for order
                else
                {
                    SqlConnection con3 = new SqlConnection(ConfigurationManager.ConnectionStrings["CMSConnectionString"].ConnectionString);
                    // con3.Open();
                    //   ev.LogEvent("I", DateTime.Now, "iD = " + AddressId, "code");
                    var query = "Select ShoppingCartID from COM_ShoppingCart where ShoppingCartShippingAddressID = " + AddressId;
                    SqlCommand cmd2 = new SqlCommand(query, con3);
                    //  ev.LogEvent("I", DateTime.Now, "test", "code");
                    con3.Open();
                    try
                    {
                        idShoppingCart = (int)cmd2.ExecuteScalar();
                        con3.Dispose();
                    }
                    catch(Exception ex)
                    {

                    }
                    con3.Close();

                    SqlConnection connect = new SqlConnection(ConfigurationManager.ConnectionStrings["CMSConnectionString"].ConnectionString);

                    //  ev.LogEvent("I", DateTime.Now, "idShoppingCart = " + idShoppingCart, "code");

                    if(idShoppingCart != 0)
                    {
                        connect.Open();
                        var query2 = "Delete  from COM_ShoppingCartSKU WHERE ShoppingCartID = " + idShoppingCart;
                        SqlCommand cmd1 = new SqlCommand(query2, connect);
                        cmd1.ExecuteScalar();


                        var stringQuery = "Delete  from COM_ShoppingCart WHERE ShoppingCartShippingAddressID = " + AddressId;
                        SqlCommand cmd3 = new SqlCommand(stringQuery, connect);
                        cmd3.ExecuteScalar();
                        connect.Close();
                        Response.Redirect("~/Special-Page/Mon-compte.aspx");

                    }
                    //    ev.LogEvent("I", DateTime.Now, "btn delet enabled false", "code");
                    AddressInfo UpdateAdress = AddressInfoProvider.GetAddressInfo(AddressId);
                    CustomerInfo uc = ECommerceContext.CurrentCustomer;
                    UpdateAdress.AddressEnabled = false;
                    UpdateAdress.AddressCustomerID = mCustomerId;
                    AddressInfoProvider.SetAddressInfo(UpdateAdress);
                    AddressId = UpdateAdress.AddressID;
                }
            }
            ReloadDataAdress();
            // PnlInsertAdress.Visible = false;
        }

        // Update selected adress
        if(e.CommandName.Equals("Update"))
        {

            // lblErrorAdress
            var lblErrorAdress = e.Item.FindControl("lblErrorAdress") as Label;

            // chkShippingAddr
            var chkShippingAddr = e.Item.FindControl("chkShippingAddr") as CheckBox;

            // chkBillingAddr
            var chkBillingAddr = e.Item.FindControl("chkBillingAddr") as CheckBox;

            if(!chkBillingAddr.Checked && !chkShippingAddr.Checked)
            {
                lblErrorAdress.Text = "V�rifier le type d'adresse";
                lblErrorAdress.Visible = true;
                return;
            }

            int AddressID = Convert.ToInt32(e.CommandArgument);
            AddressInfo ai = AddressInfoProvider.GetAddressInfo(AddressID);
            string s = ai.AddressZip;

            // txtnumero
            var txtnumero = e.Item.FindControl("txtnumero") as TextBox;
            if(txtnumero != null)
            {
                ai.SetValue("AddressNumber", txtnumero.Text);
            }

            // txtadresse1
            var txtadresse1 = e.Item.FindControl("txtadresse1") as TextBox;
            if(txtadresse1 != null)
            {
                ai.AddressLine1 = txtadresse1.Text;
            }

            // txtadresse2
            var txtadresse2 = e.Item.FindControl("txtadresse2") as TextBox;
            if(txtadresse2 != null)
            {
                ai.AddressLine2 = txtadresse2.Text;
            }

            // txtcp
            TextBox txtcp = e.Item.FindControl("txtcp") as TextBox;
            if(txtcp != null)
            {
                ai.AddressZip = txtcp.Text;
                // Response.Write("<script>alert('This is Alert " + txtcp.Text + " " + DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.fff tt") + "');</script>");
            }

            // txtville
            var txtville = e.Item.FindControl("txtville") as TextBox;
            if(txtville != null)
            {
                ai.AddressCity = txtville.Text;
            }

            // chkShippingAddr
            if(chkShippingAddr != null)
            {
                ai.AddressIsShipping = chkShippingAddr.Checked;
            }

            // chkBillingAddr
            if(chkBillingAddr != null)
            {
                ai.AddressIsBilling = chkBillingAddr.Checked;
            }

            CustomerInfo uc = ECommerceContext.CurrentCustomer;
            string mCustomerName = string.Format("{0} {1}", uc.CustomerFirstName, uc.CustomerLastName);
            // Set the properties
            ai.AddressName = string.Format("{0}, {4} {1} - {2} {3}", mCustomerName, ai.AddressLine1, ai.AddressZip, ai.AddressCity, ai.GetStringValue("AddressNumber", string.Empty));
            AddressInfoProvider.SetAddressInfo(ai);

            // Update page here
            ReloadDataAdress();
        }

    }


    private void ShowBillingCountryList()
    {
        //if (!IsPostBack)
        //{
        GeneralConnection cn = ConnectionHelper.GetConnection();
        string stringQuery = string.Format("SELECT CountryID, CountryDisplayName FROM CMS_Country ORDER BY CountryDisplayName");
        //string stringQuery = string.Format("SELECT CountryID, CountryDisplayName FROM CMS_Country where countryid in (select shippingcountryid from customtable_shippingextensioncountry) ORDER BY CountryDisplayName");
        DataSet ds = cn.ExecuteQuery(stringQuery, null, CMS.SettingsProvider.QueryTypeEnum.SQLQuery, false);
        cn.Close();
        if(!DataHelper.DataSourceIsEmpty(ds))
        {
            ds = LocalizedCountry.LocalizeCountry(ds);
            ddlShippingCountry.DataSource = ds;
            ddlShippingCountry.DataTextField = "CountryDisplayName".ToUpper();
            ddlShippingCountry.DataValueField = "CountryId";
            ddlShippingCountry.DataBind();
            ddlShippingCountry.Items.Insert(0, new ListItem(GetString("choixpays")));
        }
        // }

    }

    private void ShowShippingCountryList()
    {
        // if (!IsPostBack)
        //{
        GeneralConnection cn = ConnectionHelper.GetConnection();
        string stringQuery = string.Format("SELECT CountryID, CountryDisplayName FROM CMS_Country ORDER BY CountryDisplayName");
        //string stringQuery = string.Format("SELECT CountryID, CountryDisplayName FROM CMS_Country where countryid in (select shippingcountryid from customtable_shippingextensioncountry) ORDER BY CountryDisplayName");
        DataSet ds = cn.ExecuteQuery(stringQuery, null, CMS.SettingsProvider.QueryTypeEnum.SQLQuery, false);
        cn.Close();
        if(!DataHelper.DataSourceIsEmpty(ds))
        {
            ds = LocalizedCountry.LocalizeCountry(ds);
            ddlShippingCountry.DataSource = ds;
            ddlShippingCountry.DataTextField = "CountryDisplayName";
            ddlShippingCountry.DataValueField = "CountryId";
            ddlShippingCountry.DataBind();
            ddlShippingCountry.Items.Insert(0, new ListItem(GetString("choixpays")));
        }
        //}
    }

    private void ShowCivility()
    {
        ListItem[] items = new ListItem[3];
        items[0] = new ListItem(GetString("civ"), "1");
        items[1] = new ListItem(GetString("mr"), "2");
        items[2] = new ListItem(GetString("mrs"), "3");

        if(payement_option.Items.Count == 0)
        {
            payement_option.Items.AddRange(items);
            payement_option.DataBind();
        }
    }
    protected void txttva_TextChanged(object sender, EventArgs e)
    {
        if((txtTva.Text == "") || (txtTva.Text == "TVA") || (txtTva.Text == "VAT"))
        {
            lblerror1.Visible = true;
            lblerror1.Text = GetString("errortva ");
            return;
        }

        if(!EUVatChecker.Check(txtTva.Text))
        {
            lblerror1.Visible = true;
            lblerror1.Text = GetString("errortva2 ");
            return;
        }
    }
}
