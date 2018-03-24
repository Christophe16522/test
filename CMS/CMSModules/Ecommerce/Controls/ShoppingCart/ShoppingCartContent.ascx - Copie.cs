using System;
using System.Data;
using System.Text;
using System.Web.UI.WebControls;
using System.Collections.Generic;


using CMS.CMSHelper;
using CMS.Ecommerce;
using CMS.EcommerceProvider;
using CMS.ExtendedControls;
using CMS.GlobalHelper;
using CMS.PortalEngine;
using CMS.SiteProvider;
using CMS.URLRewritingEngine;
using CMS.WebAnalytics;
using CMS.EventLog;
using CMS.DataEngine;
using CMS.SettingsProvider;
using System.Web.UI;
using System.Data.SqlClient;
using System.Configuration;
using CMS.Helpers;
using CMS.CustomTables;
using CMS.Membership;
using CMS.Globalization;
using CMS.MacroEngine;
using CMS.Protection;

public partial class CMSModules_Ecommerce_Controls_ShoppingCart_ShoppingCartContent : ShoppingCartStep
{
   // EventLogProvider e2013 = new EventLogProvider();
    #region "Variables"

    protected Button btnReload = null;
    protected Button btnAddProduct = null;
    protected HiddenField hidProductID = null;
    protected HiddenField hidQuantity = null;
    protected HiddenField hidOptions = null;
    //protected double taux = recuperetax(int id100);
    protected Nullable<bool> mEnableEditMode = null;
    protected bool checkInventory = false;

    private int ShippingUnit = -1;
	
	private bool? mIsShippingNeeded = null;
    #endregion

	protected bool IsShippingNeeded
    {
        get
        {
            if (mIsShippingNeeded.HasValue)
            {
                return mIsShippingNeeded.Value;
            }
            else
            {
                if (ShoppingCart != null)
                {
                    // Figure out from shopping cart
                    mIsShippingNeeded = ShippingOptionInfoProvider.IsShippingNeeded(ShoppingCart);
                    return mIsShippingNeeded.Value;
                }
                else
                {
                    return true;
                }
            }
        }
    }

    #region Properties

    /// <summary>
    /// Indicates whether there are another checkout process steps after the current step, except payment.
    /// </summary>
    private bool ExistAnotherStepsExceptPayment
    {
        get
        {
            return (ShoppingCartControl.CurrentStepIndex + 2 <= ShoppingCartControl.CheckoutProcessSteps.Count - 1);
        }
    }

    #endregion


    /// <summary>
    /// Child control creation.
    /// </summary>
    protected override void CreateChildControls()
    {
        // Add product button
        btnAddProduct = new CMSButton();
        btnAddProduct.Attributes["style"] = "display: none";
        Controls.Add(btnAddProduct);
        btnAddProduct.Click += new EventHandler(btnAddProduct_Click);
        selectCurrency.UniSelector.OnSelectionChanged += delegate(object sender, EventArgs args)
        {
            btnUpdate_Click1(null, null);
            btncoupon_Click(null, null);
        };
        selectCurrency.DropDownSingleSelect.AutoPostBack = true;

        // Add the hidden fields for productId, quantity and product options
        hidProductID = new HiddenField();
        hidProductID.ID = "hidProductID";
        Controls.Add(hidProductID);

        hidQuantity = new HiddenField();
        hidQuantity.ID = "hidQuantity";
        Controls.Add(hidQuantity);

        hidOptions = new HiddenField();
        hidOptions.ID = "hidOptions";
        Controls.Add(hidOptions);

        AjaxControlToolkit.TextBoxWatermarkExtender wmShippingAdress1 = new AjaxControlToolkit.TextBoxWatermarkExtender();
        wmShippingAdress1.ID = "wmShippingAdress1";
        wmShippingAdress1.TargetControlID = "txtShippingadresse1";
        wmShippingAdress1.WatermarkText = GetString("adresse1");
        Controls.Add(wmShippingAdress1);

        AjaxControlToolkit.TextBoxWatermarkExtender wmShippingAdress2 = new AjaxControlToolkit.TextBoxWatermarkExtender();
        wmShippingAdress2.ID = "wmShippingAdress2";
        wmShippingAdress2.TargetControlID = "txtShippingadresse2";
        wmShippingAdress2.WatermarkText = GetString("adresse2");
        Controls.Add(wmShippingAdress2);

        AjaxControlToolkit.TextBoxWatermarkExtender wmShippingcp = new AjaxControlToolkit.TextBoxWatermarkExtender();
        wmShippingcp.ID = "wmShippingcp";
        wmShippingcp.TargetControlID = "txtShippingcp";
        wmShippingcp.WatermarkText = GetString("cp");
        Controls.Add(wmShippingcp);

        AjaxControlToolkit.TextBoxWatermarkExtender wmShippingville = new AjaxControlToolkit.TextBoxWatermarkExtender();
        wmShippingville.ID = "wmShippingville";
        wmShippingville.TargetControlID = "txtShippingville";
        wmShippingville.WatermarkText = GetString("ville");
        Controls.Add(wmShippingville);


        AjaxControlToolkit.TextBoxWatermarkExtender wmnumero = new AjaxControlToolkit.TextBoxWatermarkExtender();
        wmnumero.ID = "wmnumero";
        wmnumero.TargetControlID = "txtnumero";
        wmnumero.WatermarkText = GetString("numerorue");
        Controls.Add(wmnumero);

    }


    protected override void OnPreRender(EventArgs e)
    {
        // Register add product script
        ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "AddProductScript",
                                               ScriptHelper.GetScript(
                                                   "function setProduct(val) { document.getElementById('" + hidProductID.ClientID + "').value = val; } \n" +
                                                   "function setQuantity(val) { document.getElementById('" + hidQuantity.ClientID + "').value = val; } \n" +
                                                   "function setOptions(val) { document.getElementById('" + hidOptions.ClientID + "').value = val; } \n" +
                                                   "function setPrice(val) { document.getElementById('" + hdnPrice.ClientID + "').value = val; } \n" +
                                                   "function setIsPrivate(val) { document.getElementById('" + hdnIsPrivate.ClientID + "').value = val; } \n" +
                                                   "function AddProduct(productIDs, quantities, options, price, isPrivate) { \n" +
                                                   "setProduct(productIDs); \n" +
                                                   "setQuantity(quantities); \n" +
                                                   "setOptions(options); \n" +
                                                   "setPrice(price); \n" +
                                                   "setIsPrivate(isPrivate); \n" +
                                                   Page.ClientScript.GetPostBackEventReference(btnAddProduct, null) +
                                                   ";} \n" +
                                                   "function RefreshCart() {" +
                                                   Page.ClientScript.GetPostBackEventReference(btnAddProduct, null) +
                                                   ";} \n"
                                                   ));

        // Register dialog script
        ScriptHelper.RegisterDialogScript(Page);


        // Disable specific controls
        if (!Enabled)
        {
            lnkNewItem.Enabled = false;
            lnkNewItem.OnClientClick = "";
            selectCurrency.Enabled = false;
            btnEmpty.Enabled = false;
            btnUpdate.Enabled = false;
            chkSendEmail.Enabled = false;
        }

        // Show/Hide dropdownlist with currencies
        pnlCurrency.Visible &= (selectCurrency.UniSelector.HasData && selectCurrency.DropDownSingleSelect.Items.Count > 1);

        // Check session parameters for inventory check
        if (ValidationHelper.GetBoolean(SessionHelper.GetValue("checkinventory"), false))
        {
            checkInventory = true;
            SessionHelper.Remove("checkinventory");
        }

        // Check inventory
        if (checkInventory)
        {
            ShoppingCartCheckResult checkResult = ShoppingCartInfoProvider.CheckShoppingCart(ShoppingCart);

            if (checkResult.CheckFailed)
            {
                lblError.Text = checkResult.GetHTMLFormattedMessage();
            }
        }

        // Display messages if required
        lblError.Visible = !string.IsNullOrEmpty(lblError.Text.Trim());
        lblInfo.Visible = !string.IsNullOrEmpty(lblInfo.Text.Trim());
        base.OnPreRender(e);
    }

    protected void Page_Load(object sender, EventArgs e)
    {
       
        InitializeLabel();

                        
                
                                                                        
                                     
        //BuildPrice();
    }
    protected void InitializeLabel()
    {
        LabelArticle.Text = GetString("article");
        LabelQte.Text = GetString("qte");
        LabelPrixTotal.Text = GetString("prixtotal");
        LabelMontantDesAchats.Text = GetString("mntachat");
        LabelFraisEnvoi.Text = GetString("Fraisdenvoi");
        LabelAdresseLivraison.Text = GetString("adresselivraison");
        LabelModifier.Text = GetString("modify");
        LabelOptionEnvoi.Text = GetString("optionenvoi");
        LabelMoyenDePaiement.Text = GetString("moyenpaiement");
        LabelPaysDeLivraison.Text = GetString("payslivraison");
        LabelMethodeDeLivraison.Text = GetString("methodelivraison");
        chkShippingBillingAddr.Text = GetString("adressefacturation");
        chkShippingShippingAddr.Text = GetString("adresselivraison");
        lbltextcalculfrais.Text = GetString("textecalculfrais");
        lbletape.Text = GetString("etapeobligatoire");
        lblCoupon.Text = GetString("coupon");
        lblInsertionPays.Text = GetString("insererpays");
        LabelMontantCoupon.Text = GetString("texte.montantcoupon");
        LabelMontantDesAchatsSanscoupon.Text = GetString("texte.achatsanscoupon");


        // bouton commander
        if (CurrentUser.IsAuthenticated())
        {
            pnlBtnNext.Visible = true;
            btnNext.Visible = true;
            pnlBtnNext1.Visible = false;
            btnNext1.Visible = false;
        }
        else 
        {
            pnlBtnNext.Visible = false;
            btnNext.Visible = false;
            pnlBtnNext1.Visible = true;
            btnNext1.Visible = true;
            if(ddlShippingCountry.SelectedValue !="0")
            {
                pnlBtnNext.Visible = true;
                btnNext.Visible = true;
                pnlBtnNext1.Visible = false;
                btnNext1.Visible = false;
            }
         }

        // cacher bouton si vide
        if (lblPanierVide.Text != "")
        {
            btnNext1.Visible = false;
            btnNext.Visible = false;
            ShoppingCartControl.ButtonNext.Visible = false;
        }
  
    }
 
    private void BuildPrice()
    {
        GeneralConnection cn = ConnectionHelper.GetConnection();
        string sql = "SELECT * FROM Perso_ShippingOptionCountry";
        DataSet ds = cn.ExecuteQuery(sql, null, QueryTypeEnum.SQLQuery, false);
        cn.Close();
        if (!DataHelper.DataSourceIsEmpty(ds))
        {
            foreach (DataRow drow in ds.Tables[0].Rows)
            {
                CustomTableItemProvider customTableProvider = new CustomTableItemProvider(MembershipContext.AuthenticatedUser);
                string customTableClassName = "customtable.shippingextensioncountry";
                DataClassInfo customTable = DataClassInfoProvider.GetDataClass(customTableClassName);
                if (customTable != null)
                {
                    // Creates new custom table item
                    CustomTableItem newCustomTableItem = CustomTableItem.New(customTableClassName, customTableProvider);

                    // Sets the ItemText field value
                    newCustomTableItem.SetValue("ShippingOptionId", (int)drow["ShippingOptionId"]);
                    newCustomTableItem.SetValue("LocalContact", string.Empty);
                    newCustomTableItem.SetValue("Enabled", true);
                    newCustomTableItem.SetValue("ShippingCountryID", (int)drow["CountryID"]);
                    newCustomTableItem.SetValue("ShippingBase", (double)drow["Extra"]);
                    newCustomTableItem.SetValue("ProcessingMode", 1);
                    newCustomTableItem.SetValue("UnitPrice", (double)drow["ExtraPerUnit"]);

                    // Inserts the custom table item into database
                    newCustomTableItem.Insert();

                }

            }
        }
    }
    private DataSet GetCountryList()
    {
        DataSet ds;
        GeneralConnection cn = ConnectionHelper.GetConnection();
        ds = cn.ExecuteQuery(@"select  ShippingCountryId, CountryDisplayName, CountryName, CountryTwoLetterCode, CountryThreeLetterCode from customtable_shippingextensioncountry Join CMS_Country on customtable_shippingextensioncountry.ShippingCountryId= CMS_Country.CountryID
                                GROUP BY ShippingCountryId, CountryDisplayName, CountryName, CountryTwoLetterCode, CountryThreeLetterCode
                                ORDER BY dbo.CMS_Country.CountryDisplayName", null, QueryTypeEnum.SQLQuery, false);
        cn.Close();
        return LocalizedCountry.LocalizeCountry(ds);
    }

    protected override void OnLoad(EventArgs e)
    {
        if (!CurrentUser.IsGlobalAdministrator)
        {
            ReloadShippingAdresses();
            ShippingUnit = ShippingExtendedInfoProvider.GetCartShippingUnit(ShoppingCart);
            if (IsShippingNeeded && !IsPostBack)
            {
                ddlShippingCountryAuthenticated.DataSource = GetCountryList();
                ddlShippingCountryAuthenticated.DataTextField = "CountryDisplayName";
                ddlShippingCountryAuthenticated.DataValueField = "ShippingCountryID";
                ddlShippingCountryAuthenticated.DataBind();
                ListItem listitem = new ListItem(GetString("choixpays"), "0");
                ddlShippingCountryAuthenticated.Items.Insert(0, listitem);
            }

            if ((ShoppingCart != null) && (ShoppingCart.CountryID == 0) && (SiteContext.CurrentSite != null))
            {
                string countryName = ECommerceSettings.DefaultCountryName(SiteContext.CurrentSite.SiteName);
                CountryInfo ci = CountryInfoProvider.GetCountryInfo(countryName);
                ShoppingCart.CountryID = (ci != null) ? ci.CountryID : 0;

                // Set currency selectors site ID
                selectCurrency.SiteID = ShoppingCart.ShoppingCartSiteID;
            }

            imgNewItem.ImageUrl = GetImageUrl("Objects/Ecommerce_OrderItem/add.png");
            lblCurrency.Text = GetString("ecommerce.shoppingcartcontent.currency");
            lblCoupon.Text = GetString("ecommerce.shoppingcartcontent.coupon");
            lnkNewItem.Text = GetString("ecommerce.shoppingcartcontent.additem");
            imgNewItem.AlternateText = lnkNewItem.Text;
            btnUpdate.Text = GetString("ecommerce.shoppingcartcontent.btnupdate");
            btnEmpty.Text = GetString("ecommerce.shoppingcartcontent.btnempty");
            btnEmpty.OnClientClick = string.Format("return confirm({0});", ScriptHelper.GetString(GetString("ecommerce.shoppingcartcontent.emptyconfirm")));
            lnkNewItem.OnClientClick = string.Format("OpenAddProductDialog('{0}');return false;", GetCMSDeskShoppingCartSessionName());


            // Register product price detail dialog script
            StringBuilder script = new StringBuilder();
            script.Append("function ShowProductPriceDetail(cartItemGuid, sessionName) {");
            script.Append("if (sessionName != \"\"){sessionName =  \"&cart=\" + sessionName;}");
            string detailUrl = (IsLiveSite) ? "~/CMSModules/Ecommerce/CMSPages/ShoppingCartSKUPriceDetail.aspx" : "~/CMSModules/Ecommerce/Controls/ShoppingCart/ShoppingCartSKUPriceDetail.aspx";
            script.Append(string.Format("modalDialog('{0}?itemguid=' + cartItemGuid + sessionName, 'ProductPriceDetail', 750, 500); }}", ResolveUrl(detailUrl)));
            ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "ProductPriceDetail", ScriptHelper.GetScript(script.ToString()));

            // Register add order item dialog script
            script = new StringBuilder();
            script.Append("function OpenOrderItemDialog(cartItemGuid, sessionName) {");
            script.Append("if (sessionName != \"\"){sessionName =  \"&cart=\" + sessionName;}");
            script.Append(string.Format("modalDialog('{0}?itemguid=' + cartItemGuid + sessionName, 'OrderItemEdit', 500, 350); }}", ResolveUrl("~/CMSModules/Ecommerce/Controls/ShoppingCart/OrderItemEdit.aspx")));
            ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "OrderItemEdit", ScriptHelper.GetScript(script.ToString()));

            script = new StringBuilder();
            string addProductUrl = AuthenticationHelper.ResolveDialogUrl("~/CMSModules/Ecommerce/Pages/Tools/Orders/Order_Edit_AddItems.aspx");
            script.AppendFormat("var addProductDialogURL = '{0}'", URLHelper.GetAbsoluteUrl(addProductUrl));
            ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "AddProduct", ScriptHelper.GetScript(script.ToString()));


            // Hide "add product" action for live site order
            if (!ShoppingCartControl.IsInternalOrder)
            {
                pnlNewItem.Visible = false;

                ShoppingCartControl.ButtonBack.Text = GetString("ecommerce.cartcontent.buttonbacktext");
                ShoppingCartControl.ButtonBack.CssClass = "LongButton";
                ShoppingCartControl.ButtonNext.Text = GetString("ecommerce.cartcontent.buttonnexttext");

                if (!ShoppingCartControl.IsCurrentStepPostBack)
                {
                    // Get shopping cart item parameters from URL
                    ShoppingCartItemParameters itemParams = ShoppingCartItemParameters.GetShoppingCartItemParameters();

                    // Set item in the shopping cart
                    AddProducts(itemParams);
                }
            }

            // Set sending order notification when editing existing order according to the site settings
            if (ShoppingCartControl.CheckoutProcessType == CheckoutProcessEnum.CMSDeskOrderItems)
            {
                if (!ShoppingCartControl.IsCurrentStepPostBack)
                {
                    if (!string.IsNullOrEmpty(ShoppingCart.SiteName))
                    {
                        chkSendEmail.Checked = ECommerceSettings.SendOrderNotification(ShoppingCart.SiteName);
                    }
                }
                // Show send email checkbox
                chkSendEmail.Visible = true;
                chkSendEmail.Text = GetString("shoppingcartcontent.sendemail");

                // Setup buttons
                ShoppingCartControl.ButtonBack.Visible = false;
                ShoppingCart.CheckAvailableItems = false;
                ShoppingCartControl.ButtonNext.Text = GetString("general.next");

                if ((!ExistAnotherStepsExceptPayment) && (ShoppingCartControl.PaymentGatewayProvider == null))
                {
                    ShoppingCartControl.ButtonNext.Text = GetString("general.ok");
                }
            }

            // Fill dropdownlist
            if (!ShoppingCartControl.IsCurrentStepPostBack)
            {
                if ((ShoppingCart.CartItems.Count > 0) || ShoppingCartControl.IsInternalOrder)
                {
                    if (ShoppingCart.ShoppingCartCurrencyID == 0)
                    {
                        // Select customer preferred currency                    
                        if (ShoppingCart.Customer != null)
                        {
                            CustomerInfo customer = ShoppingCart.Customer;
                            ShoppingCart.ShoppingCartCurrencyID = (customer.CustomerUser != null) ? customer.CustomerUser.GetUserPreferredCurrencyID(SiteContext.CurrentSiteName) : 0;
                        }
                    }

                    if (ShoppingCart.ShoppingCartCurrencyID == 0)
                    {
                        if (SiteContext.CurrentSite != null)
                        {
                            ShoppingCart.ShoppingCartCurrencyID = SiteContext.CurrentSite.SiteDefaultCurrencyID;
                        }
                    }

                    selectCurrency.CurrencyID = ShoppingCart.ShoppingCartCurrencyID;
                    //    EventLogProvider pp = new EventLogProvider();
                  if (ShoppingCart.ShoppingCartDiscountCouponID > 0)
                    {
                        // Fill textbox with discount coupon code
                        DiscountCouponInfo dci = DiscountCouponInfoProvider.GetDiscountCouponInfo(ShoppingCart.ShoppingCartDiscountCouponID);
                        if (dci != null)
                        {
                            if (ShoppingCart.IsCreatedFromOrder || dci.IsValid)
                            {
                                txtCoupon.Text = dci.DiscountCouponCode;
                            }
                            else
                            {
                                ShoppingCart.ShoppingCartDiscountCouponID = 0;
                            }
                        }
                    }
                }

                ButtonsEmpty.Visible = (Request.Url.Host.Contains("localhost"));
                pnlCartRightInnerContent.Visible = !CurrentUser.IsAuthenticated();
                pnlCartRightInnerContentAuthenticated.Visible = (CurrentUser.IsAuthenticated() && IsLiveSite);
                pnlMiddleTotalButtons.Visible = IsLiveSite;
                popup.Visible = IsLiveSite;

                ShowPaymentList();
                ShowCountryList();
                ShowAddressList();
                ReloadData();
            }
            //  ShoppingCartInfo cart = ECommerceContext.CurrentShoppingCart;
            // Check if customer is enabled
            if ((ShoppingCart.Customer != null) && (!ShoppingCart.Customer.CustomerEnabled))
            {
                HideCartContent(GetString("ecommerce.cartcontent.customerdisabled"));
            }

            // Turn on available items checking after content is loaded
            ShoppingCart.CheckAvailableItems = true;
            base.OnLoad(e);
            InitializeLabel();
        }
        else
        {
            Response.Redirect("/");
        }
    }

    private void gridData_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            // Set enabled for order item units editing
            e.Row.Cells[7].Enabled = Enabled;
        }
    }
     protected void BtnVisiblePassageEtape1()
     {
         if (ddlShippingCountry.SelectedValue == "0")
         {
             pnlBtnNext.Visible = false;
             btnNext.Visible = false;
             pnlBtnNext1.Visible = true;
             btnNext1.Visible = true;
         }
         else
         {
             pnlBtnNext.Visible = true;
             btnNext.Visible = true;
             pnlBtnNext1.Visible = false;
             btnNext1.Visible = false;

         }
     }
   protected void btncoupon_Click(object sender, EventArgs e)
    {
        //EventLogProvider pe = new EventLogProvider();
        //pe.LogEvent("I",DateTime.Now,"ID pays :" + ddlShippingCountry.SelectedValue,"code front");
        BtnVisiblePassageEtape1();

        if (ShoppingCart != null)
        {
            ShoppingCart.ShoppingCartCurrencyID = ValidationHelper.GetInteger(selectCurrency.CurrencyID, 0);
             CheckDiscountCoupon();
             
          //  if ((ShoppingCart.ShoppingCartDiscountCouponID > 0) && (!ShoppingCart.IsDiscountCouponApplied)) //order discount for each product
            if ((ShoppingCart.ShoppingCartDiscountCouponID > 0) && (!ShoppingCart.IsDiscountCouponApplied) && (!ShoppingCart.DiscountCouponInfoObj.GetBooleanValue("DiscountCouponIsForOrder", false)))
            {
                // Discount coupon code is valid but not applied to any product of the shopping cart
                lblError.Text = GetString("shoppingcart.discountcouponnotapplied");
            }

            // Inventory shloud be checked
            checkInventory = true;
           ReloadData();
          //  Response.Redirect("~/Shopping/Shoppingcart.aspx");
        }
        
    }

    protected void btnUpdate_Click1(object sender, EventArgs e)
    {
        //if (ShoppingCart != null)
        //{
        //    ShoppingCart.ShoppingCartCurrencyID = ValidationHelper.GetInteger(selectCurrency.CurrencyID, 0);
        //    // CheckDiscountCoupon();

        //    if ((ShoppingCart.ShoppingCartDiscountCouponID > 0) && (!ShoppingCart.IsDiscountCouponApplied))
        //    {
        //        // Discount coupon code is valid but not applied to any product of the shopping cart
        //        lblError.Text = GetString("shoppingcart.discountcouponnotapplied");
        //    }

        //    // Inventory shloud be checked
        //    checkInventory = true;
        //    ReloadData();
        //}
    }

    protected void ddlShippingCountry_SelectedIndexChanged(object sender, EventArgs e)
    {
        if(IsShippingNeeded)
        {
            int CountryID = ValidationHelper.GetInteger(ddlShippingCountry.SelectedValue, 0);
            // SessionHelper.SetValue("PriceID", -1);
            // SessionHelper.SetValue("CountryID", -1);
            ShippingExtendedInfoProvider.SetCustomFieldValue(ShoppingCart, "ShoppingCartPriceID", -1);
            // ShippingExtendedInfoProvider.SetCustomFieldValue(ShoppingCart, "ShoppingCartCountryID", -1);
            // fix tax 6%
            // EventLogProvider elp = new EventLogProvider();
            double vat = ShippingExtendedInfoProvider.GetCartShippingVatRate(ShoppingCart);
            // elp.LogEvent("I", DateTime.Now, "valeur vat = " + vat, "code");
            if(vat > 0)
            {
                vat = 1.06;
                // elp.LogEvent("I", DateTime.Now, "vat : " + vat, "code");
            }
            else
            {
                vat = 1;
            }

            int ShippingCartUnits = ShippingExtendedInfoProvider.GetCartShippingUnit(ShoppingCart);
            QueryDataParameters parameters = new QueryDataParameters();
            parameters.Add("@ShippingUnits", ShippingCartUnits);
            parameters.Add("@CountryID", CountryID);
            parameters.Add("@VATRate", vat);
            //parameters.Add("@VATRate", 1 + ShippingExtendedInfoProvider.GetCartShippingVatRate(ShoppingCart) / 100);
            GeneralConnection cn = ConnectionHelper.GetConnection();
            DataSet ds = cn.ExecuteQuery("customtable.shippingextension.ShippingCostListByCountry", parameters);
            cn.Close();


            if(!DataHelper.DataSourceIsEmpty(ds))
            {
                pnlShippingOption.Visible = true;
                DataTable dt = ds.Tables[0];
                foreach(DataRow drow in dt.Rows)
                {
                    double price = Convert.ToDouble(drow["ShippingFinalCost"]);
                    string prices = CurrencyInfoProvider.GetFormattedPrice(price, ShoppingCart.Currency);
                    drow["DisplayString"] = string.Format("{0}- {1}", drow["ShippingOptionDisplayName"].ToString(), prices);
                }
                ddlShippingOption.DataSource = ds;
                ddlShippingOption.DataTextField = "DisplayString";
                ddlShippingOption.DataValueField = "ItemID";
                ddlShippingOption.DataBind();
                ddlShippingOption.AutoPostBack = !(ddlShippingOption.Items.Count == 1);

                int PriceID = ValidationHelper.GetInteger(ddlShippingOption.SelectedValue, -1);
                SessionHelper.SetValue("PriceID", PriceID);
                SessionHelper.SetValue("CountryID", CountryID);
                ShippingExtendedInfoProvider.SetCustomFieldValue(ShoppingCart, "ShoppingCartPriceID", PriceID);
                ShippingExtendedInfoProvider.SetCustomFieldValue(ShoppingCart, "ShoppingCartCountryID", CountryID);

                //ddlShippingOption_SelectedIndexChanged(sender, e);
                //btnUpdate_Click1(null, null);
            }
            else
            {
                pnlShippingOption.Visible = false;
                // NO SHIPPING AVAILABLE
                ddlShippingOption.Items.Clear();
                ddlShippingOption.DataSource = null;
                ListItem listItem = new ListItem("Votre choix", "-1");
                ddlShippingOption.Items.Add(listItem);
            }
            //pnlBtnNext.Visible = !DataHelper.DataSourceIsEmpty(ds);

            ddlShippingOption_SelectedIndexChanged(null, null);
            if(!CurrentUser.IsAuthenticated())
            {
                // pnlBtnNext.Visible = (!DataHelper.DataSourceIsEmpty(ds) && ddlShippingCountry.SelectedValue != "0");
            }
            //&& (ddlShippingCountry.SelectedValue == "0" && !CurrentUser.IsAuthenticated() || CurrentUser.IsAuthenticated()));
            pnlShippingOption.Visible = (CountryID != 0);
            if(CountryID != 0)
            {
                pnlBtnNext.Visible = true;
                btnNext.Visible = true;
                pnlBtnNext1.Visible = false;
                btnNext1.Visible = false;
            }
            else
            {
                pnlBtnNext.Visible = false;
                btnNext.Visible = false;
                pnlBtnNext1.Visible = true;
                btnNext1.Visible = true;
            }
            btncoupon_Click(null, null);
            DisplayTotalPrice(); 
        }
    }

    protected void ddlPaymentOption_SelectedIndexChanged(object sender, EventArgs e)
    {
        /*last modify by anna*/
        int PaymentID = ValidationHelper.GetInteger(ddlPaymentOption.SelectedValue, 9);
        /*// SessionHelper.SetValue("CountryID", PaymentID);*/
        ShippingExtendedInfoProvider.SetCustomFieldValue(ShoppingCart, "ShoppingCartPaymentID", PaymentID);

        /* lblPayment.Text = PaymentID.ToString();
         ddlShippingOption_SelectedIndexChanged(null, null);*/
    }

    protected void ddlShippingOptionAuthenticated_SelectedIndexChanged(object sender, EventArgs e)
    {
        if(IsShippingNeeded)
        {
            int PriceID = ValidationHelper.GetInteger(ddlShippingOptionAuthenticated.SelectedValue, -1);
            //SessionHelper.SetValue("PriceID", PriceID);
            ShippingExtendedInfoProvider.SetCustomFieldValue(ShoppingCart, "ShoppingCartPriceID", PriceID); 
        }
        btnUpdate_Click1(null, null);
        btncoupon_Click(null, null);
    }

    protected void ddlShippingOption_SelectedIndexChanged(object sender, EventArgs e)
    {
        if(IsShippingNeeded)
        {
            int PriceID = ValidationHelper.GetInteger(ddlShippingOption.SelectedValue, -1);
            // SessionHelper.SetValue("PriceID", PriceID);
            ShippingExtendedInfoProvider.SetCustomFieldValue(ShoppingCart, "ShoppingCartPriceID", PriceID); 
        }
        btnUpdate_Click1(null, null);
        btncoupon_Click(null, null);
    }

    protected void btnEmpty_Click1(object sender, EventArgs e)
    {
        if (ShoppingCart != null)
        {
            // Log activity "product removed" for all items in shopping cart
            string siteName = SiteContext.CurrentSiteName;
            if (!ShoppingCartControl.IsInternalOrder)
            {
                ShoppingCartControl.TrackActivityAllProductsRemovedFromShoppingCart(ShoppingCart, siteName, ContactID);
            }

            ShoppingCartInfoProvider.EmptyShoppingCart(ShoppingCart);

            ReloadData();
        }
    }


    /// <summary>
    /// Validates this step.
    /// </summary>
    public override bool IsValid()
    {
		EventLogProvider p = new EventLogProvider();
        // Check modify permissions
        if (ShoppingCartControl.CheckoutProcessType == CheckoutProcessEnum.CMSDeskOrderItems)
        {
            // Check 'ModifyOrders' permission
            if (!ECommerceContext.IsUserAuthorizedForPermission("ModifyOrders"))
            {
                CMSEcommercePage.RedirectToCMSDeskAccessDenied("CMS.Ecommerce", "EcommerceModify OR ModifyOrders");
            }
        }

        bool isEnvoiValid = IsEnvoiValid();
        if (!isEnvoiValid)
        {
            return isEnvoiValid;
        }

        // Allow to go to the next step only if shopping cart contains some products
        bool IsValid = IsShippingNeeded ? ((ShoppingCart.CartItems.Count > 0) && (ShoppingCart.TotalShipping > 0)) : (ShoppingCart.CartItems.Count > 0);
		
		p.LogEvent("I", DateTime.Now, "IsShippingNeeded" + IsShippingNeeded.ToString(), "");

        if (!IsValid)
        {
            HideCartContentWhenEmpty();
        }

        if (ShoppingCart.IsCreatedFromOrder)
        {
            IsValid = true;
        }

        if (!IsValid)
        {
			p.LogEvent("I", DateTime.Now, "is not valid" , "");
            lblError.Text = GetString("ecommerce.error.insertsomeproducts");
        }else{
			p.LogEvent("I", DateTime.Now, "is valid" , "");
		}

        return IsValid;
    }

    private bool IsEnvoiValid()
    {
        if (!IsShippingNeeded) return true;
        if(!string.IsNullOrEmpty(ddlShippingOptionAuthenticated.SelectedValue) && Convert.ToInt32(ddlShippingOptionAuthenticated.SelectedValue) == -1)
        {

            lblError.Text = "L'option d'envoi est obligatoire.";
            return false;
        }
        return true;
    }

    /// <summary>
    /// Process this step.
    /// </summary>
    public override bool ProcessStep()
    {

        // Shopping cart units are already saved in database (on "Update" or on "btnAddProduct_Click" actions) 
        bool isOK = false;

        if (ShoppingCart != null)
        {
            // Reload data
            ReloadData();

            // Check available items before "Check out"
            ShoppingCartCheckResult checkResult = ShoppingCartInfoProvider.CheckShoppingCart(ShoppingCart);

            if (checkResult.CheckFailed)
            {
                lblError.Text = checkResult.GetHTMLFormattedMessage();
            }
            else if (ShoppingCartControl.CheckoutProcessType == CheckoutProcessEnum.CMSDeskOrderItems)
            {
                // Indicates wheter order saving process is successful
                isOK = true;

                try
                {
                    ShoppingCartInfoProvider.SetOrder(ShoppingCart);
                    if (!IsShippingNeeded)
                    {
                        var order = OrderInfoProvider.GetOrderInfo(ShoppingCart.OrderId);
                        var bulkPrice = ShoppingCart.TotalPrice - ShoppingCart.TotalShipping;
                        order.OrderTotalPrice = bulkPrice;
                        OrderInfoProvider.SetOrderInfo(order);
                    }
                }
                catch (Exception ex)
                {
                    // Log exception
                    EventLogProvider.LogException("Shopping cart", "SAVEORDER", ex, ShoppingCart.ShoppingCartSiteID, null);
                    isOK = false;
                }

                if (isOK)
                {
                    lblInfo.Text = GetString("general.changessaved");

                    // Send order notification when editing existing order
                    if (ShoppingCartControl.CheckoutProcessType == CheckoutProcessEnum.CMSDeskOrderItems)
                    {
                        if (chkSendEmail.Checked)
                        {
                            OrderInfoProvider.SendOrderNotificationToAdministrator(ShoppingCart);
                            OrderInfoProvider.SendOrderNotificationToCustomer(ShoppingCart);
                        }
                    }
                    // Send order notification emails when on the live site
                    else if (ECommerceSettings.SendOrderNotification(SiteContext.CurrentSite.SiteName))
                    {
                        OrderInfoProvider.SendOrderNotificationToAdministrator(ShoppingCart);
                        OrderInfoProvider.SendOrderNotificationToCustomer(ShoppingCart);
                    }
                }
                else
                {
                    lblError.Text = GetString("ecommerce.orderpreview.errorordersave");
                }
            }
            // Go to the next step
            else
            {
                // Save other options
                if (!ShoppingCartControl.IsInternalOrder)
                {
                  //  EventLogProvider ee = new EventLogProvider();
                   // ee.LogEvent("I", DateTime.Now, "shop : " + ShoppingCart.ShoppingCartShippingAddressID, "code");
                    ShoppingCartInfoProvider.SetShoppingCartInfo(ShoppingCart);
                }

                isOK = true;
            }
        }

        return isOK;
    }


    private void btnAddProduct_Click(object sender, EventArgs e)
    {
        // Get strings with productIDs and quantities separated by ';'
        string productIDs = ValidationHelper.GetString(hidProductID.Value, "");
        string quantities = ValidationHelper.GetString(hidQuantity.Value, "");
        string options = ValidationHelper.GetString(hidOptions.Value, "");
        double price = ValidationHelper.GetDouble(hdnPrice.Value, -1);
        bool isPrivate = ValidationHelper.GetBoolean(hdnIsPrivate.Value, false);

        // Add new products to shopping cart
        if ((productIDs != "") && (quantities != ""))
        {
            string[] arrID = productIDs.TrimEnd(';').Split(';');
            string[] arrQuant = quantities.TrimEnd(';').Split(';');
            int[] intOptions = ValidationHelper.GetIntegers(options.Split(','), 0);

            lblError.Text = "";

            for (int i = 0; i < arrID.Length; i++)
            {
                int skuId = ValidationHelper.GetInteger(arrID[i], 0);

                SKUInfo skuInfo = SKUInfoProvider.GetSKUInfo(skuId);
                if (skuInfo != null)
                {
                    int quant = ValidationHelper.GetInteger(arrQuant[i], 0);

                    ShoppingCartItemParameters cartItemParams = new ShoppingCartItemParameters(skuId, quant, intOptions);

                    // If product is donation
                    if (skuInfo.SKUProductType == SKUProductTypeEnum.Donation)
                    {
                        // Get donation properties
                        if (price < 0)
                        {
                            cartItemParams.Price = SKUInfoProvider.GetSKUPrice(skuInfo, ShoppingCart, false, false);
                        }
                        else
                        {
                            cartItemParams.Price = price;
                        }

                        cartItemParams.IsPrivate = isPrivate;
                    }

                    // Add product to the shopping cart
                    ShoppingCart.SetShoppingCartItem(cartItemParams);

                    // Log activity
                    string siteName = SiteContext.CurrentSiteName;
                    if (!ShoppingCartControl.IsInternalOrder)
                    {
                        ShoppingCartControl.TrackActivityProductAddedToShoppingCart(skuId, ResHelper.LocalizeString(skuInfo.SKUName), ContactID, siteName, RequestContext.CurrentRelativePath, quant);
                    }

                    // Show empty button
                    btnEmpty.Visible = true;
                }
            }
        }

        // Invalidate values
        hidProductID.Value = "";
        hidOptions.Value = "";
        hidQuantity.Value = "";
        hdnPrice.Value = "";

        // Update values in table
        btnUpdate_Click1(btnAddProduct, e);

        // Hide cart content when empty
        if (DataHelper.DataSourceIsEmpty(ShoppingCart.ContentTable))
        {
            HideCartContentWhenEmpty();
        }
        else
        {
            // Inventory shloud be checked
            checkInventory = true;
        }
    }


    /// <summary>
    /// Checks whether entered dsicount coupon code is usable for this cart. Returns true if so.
    /// </summary>
    private bool CheckDiscountCoupon()
    {
        bool success = true;

        if (txtCoupon.Text.Trim() != "")
        {
            // Get discount info
            DiscountCouponInfo dci = DiscountCouponInfoProvider.GetDiscountCouponInfo(txtCoupon.Text.Trim(), ShoppingCart.SiteName);

            // Do not validate coupon when editing existing order and coupon code was not changed, otherwise process validation
            if ((dci != null) && (((ShoppingCart.IsCreatedFromOrder) && (ShoppingCart.ShoppingCartDiscountCouponID == dci.DiscountCouponID)) || dci.IsValid))
            {
                ShoppingCart.ShoppingCartDiscountCouponID = dci.DiscountCouponID;
                lblMontantAchatSanscoupon.Visible = true;
                LabelMontantDesAchatsSanscoupon.Visible = true;
                lblMontantCoupon.Visible = true;
                LabelMontantCoupon.Visible = true;
                sancoupon.Visible = true;
                totalcoupon.Visible = true;
                lblErrorCoupon.Visible = false;
                //mettre la valeur du coupon code dans une session
                Session["CouponValue"] = txtCoupon.Text.Trim();
                             
            }
            else
            {
                ShoppingCart.ShoppingCartDiscountCouponID = 0;

                // Discount coupon is not valid                
               // lblError.Text = GetString("ecommerce.error.couponcodeisnotvalid");
                lblMontantAchatSanscoupon.Visible = false;
                LabelMontantDesAchatsSanscoupon.Visible = false;
                lblMontantCoupon.Visible = false;
                LabelMontantCoupon.Visible = false;
                sancoupon.Visible = false;
                totalcoupon.Visible = false;

                lblErrorCoupon.Visible = true;
                lblErrorCoupon.Text = GetString("errorcouponcode");
              

                success = false;
            }
        }
        else
        {
            sancoupon.Visible = false;
            totalcoupon.Visible = false;
            ShoppingCart.ShoppingCartDiscountCouponID = 0;
        }

        if (LabelMontantCoupon.Visible == true)
        {
            LabelMontantDesAchats.Text = GetString("soustotal");
        }
        else
        {
            LabelMontantDesAchats.Text = GetString("mntachat");
        }

        return success;
    }


    // Displays total price
    protected void DisplayTotalPrice()
    {
       
        //lblMontantAchatSanscoupon
        double prix = ShoppingCart.TotalItemsPriceInMainCurrency;
        lblMontantAchatSanscoupon.Text = string.Format("{0}", CurrencyInfoProvider.GetFormattedPrice(prix));
        lblMontantAchatSanscoupon.Text = string.Format("{0} <em>{1}</em>", lblMontantAchatSanscoupon.Text.Substring(0, lblMontantAchatSanscoupon.Text.Length - 1), lblMontantAchatSanscoupon.Text.Substring(lblMontantAchatSanscoupon.Text.Length - 1));
        // end total sans coupon


        // total coupon 
        lblMontantCoupon.Text = string.Format("{0}", CurrencyInfoProvider.GetFormattedPrice(ShoppingCart.OrderDiscount, ShoppingCart.CurrencyInfoObj));
        lblMontantCoupon.Text = string.Format("{0} <em>{1}</em>", lblMontantCoupon.Text.Substring(0, lblMontantCoupon.Text.Length - 1), lblMontantCoupon.Text.Substring(lblMontantCoupon.Text.Length - 1));
        //
        
        pnlPrice.Visible = true;
        // lblTotalPriceValue.Text = string.Format("{0} <em>�</em>", CurrencyInfoProvider.GetFormattedValue(ShoppingCart.RoundedTotalPrice, ShoppingCart.Currency).ToString());
        // lblTotalPrice.Text = GetString("ecommerce.cartcontent.totalpricelabel");

        if(IsShippingNeeded)
        {
            lblShippingPriceValue.Text = string.Format("{0}", CurrencyInfoProvider.GetFormattedPrice(ShoppingCart.TotalShipping, ShoppingCart.Currency).ToString());
            lblShippingPriceValue.Text = string.Format("{0} <em>{1}</em>", lblShippingPriceValue.Text.Substring(0, lblShippingPriceValue.Text.Length - 1), lblShippingPriceValue.Text.Substring(lblShippingPriceValue.Text.Length - 1)); 
        }

        double bulkPrice = ShoppingCart.TotalPrice - ShoppingCart.TotalShipping;
        lblMontantAchat.Text = CurrencyInfoProvider.GetFormattedPrice(bulkPrice, ShoppingCart.Currency);//RoundedTotalPrice
        //   lblMontantAchat.Text = string.Format("{0}", CurrencyInfoProvider.GetFormattedPrice(ShoppingCart.TotalItemsPriceInMainCurrency, ShoppingCart.Currency).ToString());
        //    lblMontantAchat.Text = string.Format("{0}", CurrencyInfoProvider.GetFormattedPrice(ShoppingCart.RoundedTotalPrice, ShoppingCart.Currency).ToString());
        lblMontantAchat.Text = string.Format("{0} <em>{1}</em>", lblMontantAchat.Text.Substring(0, lblMontantAchat.Text.Length - 1), lblMontantAchat.Text.Substring(lblMontantAchat.Text.Length - 1));




        if (ShoppingCart.DiscountCouponInfoObj != null)
        {
            if (ShoppingCart.DiscountCouponInfoObj.GetBooleanValue("DiscountCouponIsForOrder", false))
            {
                //plcOrderDiscount.Visible = true;
                lblOrderDiscount.Text = GetString("Ecommerce.Discount.OrderDiscount");
                lblOrderDiscountValue.Text = CurrencyInfoProvider.GetFormattedPrice(ShoppingCart.OrderDiscount, ShoppingCart.CurrencyInfoObj);
            }
            else
            {
                plcOrderDiscount.Visible = false;
            }
        }
       
        
    }

  

    /// <summary>
    /// Sets product in the shopping cart.
    /// </summary>
    /// <param name="itemParams">Shoppping cart item parameters</param>
    protected void AddProducts(ShoppingCartItemParameters itemParams)
    {
        // Get main product info
        int productId = itemParams.SKUID;
        int quantity = itemParams.Quantity;

        if ((productId > 0) && (quantity > 0))
        {
            // Check product/options combination
            if (ShoppingCartInfoProvider.CheckNewShoppingCartItems(ShoppingCart, itemParams))
            {
                // Get requested SKU info object from database
                SKUInfo skuObj = SKUInfoProvider.GetSKUInfo(productId);
                if (skuObj != null)
                {
                    // On the live site
                    if (!ShoppingCartControl.IsInternalOrder)
                    {
                        bool updateCart = false;

                        // Assign current shopping cart to current user
                        CurrentUserInfo ui = MembershipContext.AuthenticatedUser;
                        if (!ui.IsPublic())
                        {
                            ShoppingCart.User = ui;
                            updateCart = true;
                        }

                        // Shopping cart is not saved yet
                        if (ShoppingCart.ShoppingCartID == 0)
                        {
                            updateCart = true;
                        }

                        // Update shopping cart when required
                      //  EventLogProvider ev = new EventLogProvider();
                        if (updateCart)
                        {
                            try
                            {
                                int idCustomer = ECommerceContext.CurrentCustomer.CustomerID;
                                SqlConnection cons = new SqlConnection(ConfigurationManager.ConnectionStrings["CMSConnectionString"].ConnectionString);
                                cons.Open();
                                var stringQuery = "select count(AddressID) as NbAdress from COM_Address WHERE COM_Address.AddressEnabled = 'true'  AND COM_Address.AddressCustomerID  = " + idCustomer;
                                SqlCommand cmd3 = new SqlCommand(stringQuery, cons);
                                int nb = (int)cmd3.ExecuteScalar();
                                cons.Close();
                                if (nb == 0)
                                {
                                    //ev.LogEvent("I", DateTime.Now, "z�roo adress!!", "code");
                                    Response.Redirect("~/Special-Page/Mon-compte.aspx");
                                }
                                if (Session["newAddress"] != null)
                                {
                                    string nbr1;
                                    nbr1 = Session["newAddress"].ToString();
                                    ShoppingCart.ShoppingCartShippingAddressID = Int32.Parse(nbr1.ToString());
                                    ShoppingCartInfoProvider.SetShoppingCartInfo(ShoppingCart);
                                 //   ev.LogEvent("I", DateTime.Now, "panier et if", "code");
                                    
                                }
                                SqlConnection cons2 = new SqlConnection(ConfigurationManager.ConnectionStrings["CMSConnectionString"].ConnectionString);
                                cons2.Open();
                                var stringQuery2 = "SELECT TOP 1 AddressID FROM ( SELECT *, ROW_NUMBER() OVER (ORDER BY AddressID desc) as row FROM COM_Address) a WHERE AddressCustomerID =" + idCustomer + "and AddressEnabled = 1 and AddressIsShipping = 1";
                                SqlCommand cmd2 = new SqlCommand(stringQuery2, cons2);
                                int nb2 = (int)cmd2.ExecuteScalar();
                                cons2.Close();
                               // ev.LogEvent("I", DateTime.Now, "nb2 : " + nb2, "code");
                                if (nb2 != 0)
                                {
                                    ShoppingCart.ShoppingCartShippingAddressID = nb2;
                                    ShoppingCartInfoProvider.SetShoppingCartInfo(ShoppingCart);
                                    //  ev.LogEvent("I", DateTime.Now, "panier" + ShoppingCart.ShoppingCartShippingAddressID, "code");
                                }
                                else 
                                {
                                SqlConnection cons3 = new SqlConnection(ConfigurationManager.ConnectionStrings["CMSConnectionString"].ConnectionString);
                                cons3.Open();
                                var stringQuery3 = "SELECT TOP 1 AddressID FROM ( SELECT *, ROW_NUMBER() OVER (ORDER BY AddressID desc) as row FROM COM_Address) a WHERE AddressCustomerID =" + idCustomer + "and AddressEnabled = 1";
                                SqlCommand cm3 = new SqlCommand(stringQuery3, cons3);
                                int nb3 = (int)cm3.ExecuteScalar();
                               // ev.LogEvent("I", DateTime.Now, "nb3 : " + nb3, "code");
                                    ShoppingCart.ShoppingCartShippingAddressID = nb3;
                                    ShoppingCartInfoProvider.SetShoppingCartInfo(ShoppingCart);
                                  //  ev.LogEvent("I", DateTime.Now, "panier" + ShoppingCart.ShoppingCartShippingAddressID, "code");
                                    cons3.Close();
                                }

                            }
                            catch
                            {
                                string nbr;
                                if (Session["newAddress"] != null)
                                {
                                    nbr = Session["newAddress"].ToString();
                                    ShoppingCart.ShoppingCartShippingAddressID = Int32.Parse(nbr.ToString());
                                    ShoppingCartInfoProvider.SetShoppingCartInfo(ShoppingCart);
                                    //ev.LogEvent("I", DateTime.Now, "pas dans le panier et if", "code");
                                }
                                else
                                {
                                    //ev.LogEvent("I", DateTime.Now, "pas dans le panier et else", "code");
                                }
                            }
                        }

                        // Set item in the shopping cart
                        ShoppingCartItemInfo product = ShoppingCart.SetShoppingCartItem(itemParams);

                        // Update shopping cart item in database
                      //  EventLogProvider evt = new EventLogProvider();
                        try
                        {
                            ShoppingCartItemInfoProvider.SetShoppingCartItemInfo(product);
                        //    evt.LogEvent("I", DateTime.Now, "update", "code");
                        }
                        catch
                        {
                        //    evt.LogEvent("I", DateTime.Now, "dans catch update", "code");
                        }

                        // Update product options in database
                        foreach (ShoppingCartItemInfo option in product.ProductOptions)
                        {
                            ShoppingCartItemInfoProvider.SetShoppingCartItemInfo(option);
                        }

                        // Update bundle items in database
                        foreach (ShoppingCartItemInfo bundleItem in product.BundleItems)
                        {
                            ShoppingCartItemInfoProvider.SetShoppingCartItemInfo(bundleItem);
                        }

                        // Track add to shopping cart conversion
                        ECommerceHelper.TrackAddToShoppingCartConversion(product);
                    }
                    // In CMSDesk
                    else
                    {
                        // Set item in the shopping cart
                        ShoppingCart.SetShoppingCartItem(itemParams);
                    }
                }
            }

            // Avoid adding the same product after page refresh
            if (lblError.Text == "")
            {
                string url = URLRewriter.CurrentURL;

                if (!string.IsNullOrEmpty(URLHelper.GetUrlParameter(url, "productid")) ||
                    !string.IsNullOrEmpty(URLHelper.GetUrlParameter(url, "quantity")) ||
                    !string.IsNullOrEmpty(URLHelper.GetUrlParameter(url, "options")))
                {
                    // Remove parameters from URL
                    url = URLHelper.RemoveParameterFromUrl(url, "productid");
                    url = URLHelper.RemoveParameterFromUrl(url, "quantity");
                    url = URLHelper.RemoveParameterFromUrl(url, "options");
                    URLHelper.Redirect(url);
                }
            }
        }
    }


    /// <summary>
    /// Hides cart content controls when no items are in shopping cart.
    /// </summary>
    protected void HideCartContentWhenEmpty()
    {
        HideCartContent(null);
    }


    /// <summary>
    /// Hides cart content controls and displays given message.
    /// </summary>
    protected void HideCartContent(string message)
    {
        pnlNewItem.Visible = ShoppingCartControl.IsInternalOrder;
        pnlPrice.Visible = false;
        btnEmpty.Visible = false;
        plcCoupon.Visible = false;
        pnlCartLeftInnerContent.Visible = false;
        pnlCartRightInnerContent.Visible = false;
        pnlCartRightInnerContentAuthenticated.Visible = false;
        //bouton commander invisible
        
        pnlBtnNext1.Visible = false;
        pnlBtnNext.Visible = false;
        btnNext.Visible = false;
        btnNext1.Visible = false;
        test.Visible = false;
        //texte panel vide
         txtCoupon.Text = "";
        // SessionHelper.SetValue("HideNext", 1);
        ShippingExtendedInfoProvider.SetCustomFieldValue(ShoppingCart, "ShoppingCarHideNext", 1);
        if (!ShoppingCartControl.IsInternalOrder)
        {
            pnlCurrency.Visible = false;
            btnNext.Visible = false;
            btnNext1.Visible = false;
            ShoppingCartControl.ButtonNext.Visible = false;        
            
            message = message ?? GetString("ecommerce.shoppingcartempty");
            // utilisation de multilangue
          //  lblInfo.Text = message + "<br />";
            lblPanierVide.Visible = true;
            lblPanierVide.Text = GetString("paniervide");
        }       
    }


    /// <summary>
    /// Reloads the form data.
    /// </summary>
    protected void ReloadData()
    {
        ReloadShippingAdresses();
        rptCart.DataSource = ShoppingCart.ContentTable;
        rptCart.DataBind();
        // Hide coupon placeholder when no coupons are defined
        // plcCoupon.Visible = AreDiscountCouponsAvailableOnSite();



        if (!DataHelper.DataSourceIsEmpty(ShoppingCart.ContentTable))
        {
            // Display total price
            DisplayTotalPrice();

            // Display/hide discount column
            //gridData.Columns[9].Visible = ShoppingCart.IsDiscountApplied;
        }
        else
        {
            // Hide some items
            HideCartContentWhenEmpty();

        }
        if (IsLiveSite)
        {
            if (CurrentUser.IsAuthenticated())
            {
                int ShippingOptionAuthenticatedID = -1;
                try
                {
                    ShippingOptionAuthenticatedID = IsShippingNeeded ? Convert.ToInt32(ddlShippingOptionAuthenticated.SelectedValue) : -1;
                }
                catch
                {
                  //  EventLogProvider ev = new EventLogProvider();
                   // ev.LogEvent("I", DateTime.Now, " dans catch ", "code");
                }
                pnlBtnNext.Visible = (!DataHelper.DataSourceIsEmpty(ShoppingCart.ContentTable) && (ShippingOptionAuthenticatedID > 0));
            }
            else
            {
               // pnlBtnNext.Visible = pnlShippingOption.Visible;
                pnlBtnNext.Visible = true;
                btnNext.Visible = true;
            }
        }

        if (!ShippingOptionInfoProvider.IsShippingNeeded(ShoppingCart))
        {
            plcShippingPrice.Visible = false;
        }
        // ReloadShippingAdresses();
        BtnVisiblePassageEtape1();
    }


    /// <summary>
    /// Determines if the discount coupons are available for the current site.
    /// </summary>
    /// 

  
    private bool AreDiscountCouponsAvailableOnSite()
    {
        string siteName = ShoppingCart.SiteName;

        // Check site and global discount coupons
        string where = "DiscountCouponSiteID = " + SiteInfoProvider.GetSiteID(siteName);
        if (ECommerceSettings.AllowGlobalDiscountCoupons(siteName))
        {
            where += " OR DiscountCouponSiteID IS NULL";
        }

        // Coupons are available if found any
        DataSet ds = DiscountCouponInfoProvider.GetDiscountCoupons(where, null, -1, "count(DiscountCouponID)");
        if (!DataHelper.DataSourceIsEmpty(ds))
        {
            return (ValidationHelper.GetInteger(ds.Tables[0].Rows[0][0], 0) > 0);
        }

        return false;
    }


    /// <summary>
    /// Returns price detail link.
    /// </summary>
    protected string GetPriceDetailLink(object value)
    {
        if ((ShoppingCartControl.EnableProductPriceDetail) && (ShoppingCart.ContentTable != null))
        {
            Guid cartItemGuid = ValidationHelper.GetGuid(value, Guid.Empty);
            if (cartItemGuid != Guid.Empty)
            {
                return string.Format("<img src=\"{0}\" onclick=\"javascript: ShowProductPriceDetail('{1}', '{2}')\" alt=\"{3}\" class=\"ProductPriceDetailImage\" style=\"cursor:pointer;\" />",
                    GetImageUrl("Design/Controls/UniGrid/Actions/detail.png"),
                    cartItemGuid, GetCMSDeskShoppingCartSessionName(),
                    GetString("shoppingcart.productpricedetail"));
            }
        }

        return "";
    }

    /// <summary>
    /// Returns shopping cart session name.
    /// </summary>
    private string GetCMSDeskShoppingCartSessionName()
    {
        switch (ShoppingCartControl.CheckoutProcessType)
        {
            case CheckoutProcessEnum.CMSDeskOrder:
                return "CMSDeskNewOrderShoppingCart";

            case CheckoutProcessEnum.CMSDeskCustomer:
                return "CMSDeskNewCustomerOrderShoppingCart";

            case CheckoutProcessEnum.CMSDeskOrderItems:
                return "CMSDeskOrderItemsShoppingCart";

            case CheckoutProcessEnum.LiveSite:
            case CheckoutProcessEnum.Custom:
            default:
                return "";
        }
    }

    public override void ButtonBackClickAction()
    {
        if ((!ShoppingCartControl.IsInternalOrder) && (ShoppingCartControl.CurrentStepIndex == 0))
        {
            // Continue shopping
            URLHelper.Redirect(ShoppingCartControl.PreviousPageUrl);
        }
        else
        {
            // Standard action
            base.ButtonBackClickAction();
        }
    }

    #region "Binding methods"

    private string GetQueryString()
    {
        string result = string.Format(@"SELECT CountryID, CountryDisplayName FROM CMS_COUNTRY WHERE COUNTRYID IN (
                                        SELECT ShippingCountryID FROM dbo.customtable_ShippingExtensionCountry WHERE 
                                        dbo.customtable_ShippingExtensionCountry.Enabled=1 AND dbo.customtable_ShippingExtensionCountry.ShippingOptionID IN(select ShippingOPtionID from dbo.customtable_shippingextension WHERE Enabled=1) AND ItemID IN (
                                        SELECT ShippingExtensionCountryID FROM dbo.customtable_shippingextensionpricing where shippingUnitFrom <={0} and shippingUnitTo>={0})
                                        GROUP BY ShippingCountryID) ORDER BY CountryDisplayName", ShippingExtendedInfoProvider.GetCartShippingUnit(ShoppingCart).ToString());
        return result;
    }

    private void ShowAddressList()
    {
        try
        {
            if(pnlCartRightInnerContentAuthenticated.Visible)
            {
                List<AddressInfo> ais = ShippingExtendedInfoProvider.GetAdresses(false, true, ShoppingCart);

                if(ais == null)
                {

                    Response.Redirect("~/Special-Page/Mon-compte.aspx");

                }

                if(ais.Count == 1)
                {
                    AddressInfo ai = ais[0];


                    if(IsShippingNeeded && ai.AddressEnabled)
                    {

                        //lblShippingAddressFullName.Text = ECommerceContext.CurrentCustomer.CustomerLastName + " " + ECommerceContext.CurrentCustomer.CustomerFirstName; //ai.AddressPersonalName;  //GetString("DearCustomer") + " " +  
                        lblShippingAddressFullName.Text = ECommerceContext.CurrentCustomer.CustomerFirstName + " " + ECommerceContext.CurrentCustomer.CustomerLastName; //ai.AddressPersonalName;  //GetString("DearCustomer") + " " +  
                        lblShippingAddressStreet.Text = string.IsNullOrEmpty(ai.AddressLine2) ? ai.AddressLine1 : string.Format("{0}, {1}", ai.AddressLine1, ai.AddressLine2);
                        lblShippingAddressStreet.Text = string.Format("{0} {1}", ai.GetStringValue("AddressNumber", string.Empty), lblShippingAddressStreet.Text).Trim();
                        lblShippingAddressZipCode.Text = ai.AddressZip;
                        lblShippingAddressCityCountry.Text = string.Format("{0}, {1}", ai.AddressCity, MacroContext.CurrentResolver.ResolveMacros(CountryInfoProvider.GetCountryInfo(ai.AddressCountryID).CountryDisplayName));
                        ShoppingCart.ShoppingCartShippingAddressID = ai.AddressID;
                        ShoppingCart.ShoppingCartBillingAddressID = ai.AddressID;
                        //    EventLogProvider elp1 = new EventLogProvider();
                        double vat = ShippingExtendedInfoProvider.GetCartShippingVatRate(ShoppingCart);
                        //  elp1.LogEvent("I", DateTime.Now, "valeur1 vat1 = " + vat, "code");
                        if(vat > 0)
                        {
                            vat = 1 + vat / 100;
                            //    elp1.LogEvent("I", DateTime.Now, "vat1 : " + vat, "code");
                        }
                        else
                        {
                            vat = 1;
                        }
                        QueryDataParameters parameters = new QueryDataParameters();
                        parameters.Add("@ShippingUnits", ShippingUnit);
                        parameters.Add("@CountryID", ai.AddressCountryID);
                        parameters.Add("@VATRate", vat);
                        //var x = 1 + ShippingExtendedInfoProvider.GetCartShippingVatRate(ShoppingCart) / 100;
                        GeneralConnection cn = ConnectionHelper.GetConnection();
                        DataSet ds = cn.ExecuteQuery("customtable.shippingextension.ShippingCostListByCountry", parameters);
                        cn.Close();
                        if(!DataHelper.DataSourceIsEmpty(ds))
                        {
                            DataTable dt = ds.Tables[0];
                            foreach(DataRow drow in dt.Rows)
                            {
                                double price = Convert.ToDouble(drow["ShippingFinalCost"]);
                                string prices = CurrencyInfoProvider.GetFormattedPrice(price, ShoppingCart.Currency);
                                drow["DisplayString"] = string.Format("{0}- {1}", drow["ShippingOptionDisplayName"].ToString(), prices);
                            }
                            ddlShippingOptionAuthenticated.DataSource = ds;
                            ddlShippingOptionAuthenticated.DataTextField = "DisplayString";
                            ddlShippingOptionAuthenticated.DataValueField = "ItemID";
                            ddlShippingOptionAuthenticated.DataBind();
                            ListItem listItem = new ListItem("Votre choix", "-1");
                            listItem.Selected = true;
                            ddlShippingOptionAuthenticated.Items.Add(listItem);
                            ddlShippingOptionAuthenticated.AutoPostBack = !(ddlShippingOptionAuthenticated.Items.Count == 1);
                            int PriceID = ValidationHelper.GetInteger(ddlShippingOptionAuthenticated.SelectedValue, -1);
                            // SessionHelper.SetValue("PriceID", PriceID);
                            // SessionHelper.SetValue("CountryID", ai.AddressCountryID);
                            ShippingExtendedInfoProvider.SetCustomFieldValue(ShoppingCart, "ShoppingCartPriceID", PriceID);
                            ShippingExtendedInfoProvider.SetCustomFieldValue(ShoppingCart, "ShoppingCartCountryID", ai.AddressCountryID);

                            ddlShippingOptionAuthenticated_SelectedIndexChanged(null, null);
                            //btnUpdate_Click1(null, null);
                        }
                        else
                        {
                            // NO SHIPPING AVAILABLE
                            ddlShippingOptionAuthenticated.Items.Clear();
                            ddlShippingOptionAuthenticated.DataSource = null;
                            ListItem listItem = new ListItem("Votre choix", "-1");
                            ddlShippingOptionAuthenticated.Items.Add(listItem);
                        }
                    }// fin if enabled address


                    else
                    {
                        int idCustomer = ECommerceContext.CurrentCustomer.CustomerID;
                        SqlConnection con3 = new SqlConnection(ConfigurationManager.ConnectionStrings["CMSConnectionString"].ConnectionString);
                        con3.Open();
                        SqlCommand macommand = new SqlCommand();
                        //macommand.CommandText = "SELECT * FROM ( SELECT *, ROW_NUMBER() OVER (ORDER BY AddressID desc) as row FROM COM_Address) a WHERE row > 0 and row <= 1  AND AddressCustomerID =" + idCustomer;
                        macommand.CommandText = "SELECT TOP 1 * FROM ( SELECT *, ROW_NUMBER() OVER (ORDER BY AddressID desc) as row FROM COM_Address) a WHERE AddressCustomerID =" + idCustomer;
                        macommand.Connection = con3;
                        IDataReader reader = macommand.ExecuteReader();
                        AddressInfo ai2 = new AddressInfo();
                        while(reader.Read())
                        {

                            ai2.AddressLine1 = Convert.ToString(reader.GetValue(2));
                            ai2.AddressLine2 = Convert.ToString(reader.GetValue(3));
                            var temp = ai2.GetStringValue(Convert.ToString(reader.GetValue(17)), Convert.ToString(reader.GetValue(17)));
                            ai2.AddressZip = Convert.ToString(reader.GetValue(5));
                            ai2.AddressCity = Convert.ToString(reader.GetValue(4));
                            ai2.AddressCountryID = Convert.ToInt32(reader.GetValue(8));


                            // var stringQuery = "SELECT * FROM ( SELECT *, ROW_NUMBER() OVER (ORDER BY AddressID desc) as row FROM COM_Address) a WHERE row > 0 and row <= 1  AND AddressCustomerID =" +idCustomer;

                            if (IsShippingNeeded)
                            {
                                lblShippingAddressFullName.Text = ECommerceContext.CurrentCustomer.CustomerFirstName +
                                                                  " " +
                                                                  ECommerceContext.CurrentCustomer.CustomerLastName;
                                    //ai.AddressPersonalName;  //GetString("DearCustomer") + " " +  
                                lblShippingAddressStreet.Text = string.IsNullOrEmpty(ai2.AddressLine2)
                                                                    ? ai2.AddressLine1
                                                                    : string.Format("{0}, {1}", ai2.AddressLine1,
                                                                                    ai2.AddressLine2);
                                //                    lblShippingAddressStreet.Text = string.Format("{0} {1}", ai2.GetStringValue(Convert.ToString(reader.GetValue(17)), string.Empty), lblShippingAddressStreet.Text).Trim();
                                lblShippingAddressStreet.Text =
                                    string.Format("{0} {1}", temp.ToString(), lblShippingAddressStreet.Text).Trim();
                                lblShippingAddressZipCode.Text = ai2.AddressZip;
                                lblShippingAddressCityCountry.Text = string.Format("{0}, {1}", ai2.AddressCity,
                                                                                   MacroContext.CurrentResolver
                                                                                             .ResolveMacros(
                                                                                                 CountryInfoProvider
                                                                                                     .GetCountryInfo(
                                                                                                         ai2
                                                                                                             .AddressCountryID)
                                                                                                     .CountryDisplayName));
                                ShoppingCart.ShoppingCartShippingAddressID = ai2.AddressID;
                                ShoppingCart.ShoppingCartBillingAddressID = ai2.AddressID;

                                // EventLogProvider elp2 = new EventLogProvider();
                                double vat = ShippingExtendedInfoProvider.GetCartShippingVatRate(ShoppingCart);
                                // elp2.LogEvent("I", DateTime.Now, "valeur2 vat2 = " + vat, "code");
                                if (vat > 0)
                                {
                                    vat = 1.06;
                                    //   elp2.LogEvent("I", DateTime.Now, "vat2 : " + vat, "code");
                                }
                                else
                                {
                                    vat = 1;
                                }
                                QueryDataParameters parameters = new QueryDataParameters();
                                parameters.Add("@ShippingUnits", ShippingUnit);
                                parameters.Add("@CountryID", ai2.AddressCountryID);
                                parameters.Add("@VATRate", vat);
                                //  parameters.Add("@VATRate", 1 + ShippingExtendedInfoProvider.GetCartShippingVatRate(ShoppingCart) / 100);
                                GeneralConnection cn = ConnectionHelper.GetConnection();
                                DataSet ds = cn.ExecuteQuery("customtable.shippingextension.ShippingCostListByCountry",
                                                             parameters);
                                cn.Close();

                                if (!DataHelper.DataSourceIsEmpty(ds))
                                {
                                    DataTable dt = ds.Tables[0];
                                    foreach (DataRow drow in dt.Rows)
                                    {
                                        double price = Convert.ToDouble(drow["ShippingFinalCost"]);
                                        string prices = CurrencyInfoProvider.GetFormattedPrice(price,
                                                                                               ShoppingCart.Currency);
                                        drow["DisplayString"] = string.Format("{0}- {1}",
                                                                              drow["ShippingOptionDisplayName"].ToString
                                                                                  (), prices);
                                    }
                                    ddlShippingOptionAuthenticated.DataSource = ds;
                                    ddlShippingOptionAuthenticated.DataTextField = "DisplayString";
                                    ddlShippingOptionAuthenticated.DataValueField = "ItemID";
                                    ddlShippingOptionAuthenticated.DataBind();
                                    ddlShippingOptionAuthenticated.AutoPostBack =
                                        !(ddlShippingOptionAuthenticated.Items.Count == 1);
                                    int PriceID =
                                        ValidationHelper.GetInteger(ddlShippingOptionAuthenticated.SelectedValue, -1);
                                    // SessionHelper.SetValue("PriceID", PriceID);
                                    // SessionHelper.SetValue("CountryID", ai.AddressCountryID);
                                    ShippingExtendedInfoProvider.SetCustomFieldValue(ShoppingCart, "ShoppingCartPriceID",
                                                                                     PriceID);
                                    ShippingExtendedInfoProvider.SetCustomFieldValue(ShoppingCart,
                                                                                     "ShoppingCartCountryID",
                                                                                     ai.AddressCountryID);

                                    ddlShippingOptionAuthenticated_SelectedIndexChanged(null, null);
                                    //btnUpdate_Click1(null, null);
                                }
                                else
                                {
                                    // NO SHIPPING AVAILABLE
                                    ddlShippingOptionAuthenticated.Items.Clear();
                                    ddlShippingOptionAuthenticated.DataSource = null;
                                    ListItem listItem = new ListItem("Votre choix", "-1");
                                    ddlShippingOptionAuthenticated.Items.Add(listItem);
                                }
                            }
                        }// fin while
                        con3.Close();
                    }// fin else enabled ai
                    // pnlBtnNext.Visible = !DataHelper.DataSourceIsEmpty(ds);
                }
            }

            ReloadShippingAdresses();
        }
        catch (Exception ex)
        {
            var ev = new EventLogProvider();
            ev.LogEvent("E", DateTime.Now,"CMSModules_Ecommerce_Controls_ShoppingCart_ShoppingCartContent",ex.Message);
        }
        
    }

    private void ShowCountryList()
    {
        if (IsShippingNeeded && !IsPostBack && pnlCartRightInnerContent.Visible)
        {
            GeneralConnection cn = ConnectionHelper.GetConnection();
            QueryDataParameters parameters = new QueryDataParameters();
            parameters.Add("@ShippingUnit", ShippingExtendedInfoProvider.GetCartShippingUnit(ShoppingCart));
            DataSet ds = cn.ExecuteQuery("customtable.shippingextension.CountryList", parameters);
            cn.Close();
            if (!DataHelper.DataSourceIsEmpty(ds))
            {
                ds = LocalizedCountry.LocalizeCountry(ds);
                ddlShippingCountry.DataSource = ds;
                ddlShippingCountry.DataTextField = "CountryDisplayName";
                ddlShippingCountry.DataValueField = "ShippingCountryId";
                ddlShippingCountry.DataBind();
                foreach (ListItem i in ddlShippingCountry.Items)
                {
                    //i.Text = CMSContext.CurrentResolver.ResolveMacros(i.Text);
                }

                ListItem item = new ListItem(GetString("choixpays"), "0");
                ddlShippingCountry.Items.Insert(0, item);
                ddlShippingCountry.SelectedValue = "0";
                /*AddressInfo ai = AddressInfoProvider.GetAddressInfo(ShoppingCart.ShoppingCartShippingAddressID);
                if (ai != null)
                {
                    ddlShippingCountry.SelectedValue = ai.AddressCountryID.ToString();
                }
                else
                {

                    if (CurrentUser.IsAuthenticated())
                    {
                        if (ECommerceContext.CurrentCustomer != null)
                        {
                            string where = string.Format("AddressCustomerID={0} AND AddressIsShipping=1", ECommerceContext.CurrentCustomer.CustomerID.ToString());
                            string orderby = "AddressID";
                            ds = AddressInfoProvider.GetAddresses(where, orderby);
                            if (!DataHelper.DataSourceIsEmpty(ds))
                            {
                                ai = new AddressInfo(ds.Tables[0].Rows[0]);
                                ddlShippingCountry.SelectedValue = ai.AddressCountryID.ToString();
                            }
                        }
                    }
                }*/
                ddlShippingCountry_SelectedIndexChanged(null, null);
                //ShowPaymentList();
            }
           
        }
    }

    private void ShowPaymentList()
    {
        if (!IsPostBack && pnlCartRightInnerContent.Visible)
        {
            GeneralConnection cn = ConnectionHelper.GetConnection();
            string stringQuery = string.Format("SELECT CountryID, CountryDisplayName FROM CMS_Country ORDER BY CountryDisplayName");
            //DataSet ds = cn.ExecuteQuery(stringQuery, null, CMS.SettingsProvider.QueryTypeEnum.SQLQuery, false);
            string where = "PaymentOptionEnabled=1";
            string orderby = "PaymentOptionName";
            //DataSet ds = PaymentOptionInfoProvider.GetPaymentOptions(CurrentSite.SiteID, true);
            DataSet ds = PaymentOptionInfoProvider.GetPaymentOptions(where, orderby);
            if (!DataHelper.DataSourceIsEmpty(ds))
            {
                ddlPaymentOption.DataSource = ds;
                ddlPaymentOption.DataTextField = "PaymentOptionDisplayName";
                ddlPaymentOption.DataValueField = "PaymentOptionId";
                ddlPaymentOption.DataBind();
                // string value = ValidationHelper.GetString(SessionHelper.GetValue("PaymentID"), string.Empty);
                string value = ValidationHelper.GetString(ShippingExtendedInfoProvider.GetCustomFieldValue(ShoppingCart, "ShoppingCartPaymentID"), string.Empty);
                if (!string.IsNullOrEmpty(value))
                {
                    ddlPaymentOption.SelectedValue = value;
                }
                ddlPaymentOption_SelectedIndexChanged(null, null);
            }
            cn.Close();
        }
    }

    /// <summary>
    /// Returns formatted currency value.
    /// </summary>
    /// <param name="value">Value to format</param>
    protected string GetFormattedValue(object value)
    {
        double price = ValidationHelper.GetDouble(value, 0);
        return CurrencyInfoProvider.GetFormattedValue(price, ShoppingCart.Currency);
    }


    /// <summary>
    /// Returns formatted and localized SKU name.
    /// </summary>
    /// <param name="skuId">SKU ID</param>
    /// <param name="skuSiteId">SKU site ID</param>
    /// <param name="value">SKU name</param>
    /// <param name="isProductOption">Indicates if cart item is product option</param>
    /// <param name="isBundleItem">Indicates if cart item is bundle item</param>
    protected string GetSKUName(object skuId, object skuSiteId, object value, object isProductOption, object isBundleItem, object cartItemIsPrivate, object itemText, object productType)
    {
        string name = ResHelper.LocalizeString((string)value);
        bool isPrivate = ValidationHelper.GetBoolean(cartItemIsPrivate, false);
        string text = itemText as string;
        StringBuilder skuName = new StringBuilder();
        SKUProductTypeEnum type = SKUInfoProvider.GetSKUProductTypeEnum(productType as string);

        // If it is a product option or bundle item
        if (ValidationHelper.GetBoolean(isProductOption, false) || ValidationHelper.GetBoolean(isBundleItem, false))
        {
            skuName.Append("<span style=\"font-size: 90%\"> - ");
            skuName.Append(HTMLHelper.HTMLEncode(name));

            if (!string.IsNullOrEmpty(text))
            {
                skuName.Append(" '" + HTMLHelper.HTMLEncode(text) + "'");
            }

            skuName.Append("</span>");
        }
        // If it is a parent product
        else
        {
            // Add private donation suffix
            if ((type == SKUProductTypeEnum.Donation) && (isPrivate))
            {
                name += string.Format(" ({0})", GetString("com.shoppingcartcontent.privatedonation"));
            }

            // In CMS Desk
            if (ShoppingCartControl.IsInternalOrder)
            {
                // Display SKU name
                skuName.Append(HTMLHelper.HTMLEncode(name));
            }
            // On live site
            else
            {
                if (type == SKUProductTypeEnum.Donation)
                {
                    // Display donation name without link
                    skuName.Append(HTMLHelper.HTMLEncode(name));
                }
                else
                {
                    // Display link to product page
                    skuName.Append(string.Format("<a href=\"{0}?productid={1}\" class=\"CartProductDetailLink\">{2}</a>", ResolveUrl("~/CMSPages/GetProduct.aspx"), skuId.ToString(), HTMLHelper.HTMLEncode(name)));
                }
            }
        }

        return skuName.ToString();
    }


    protected static bool IsProductOption(object isProductOption)
    {
        return ValidationHelper.GetBoolean(isProductOption, false);
    }


    protected static bool IsBundleItem(object isBundleItem)
    {
        return ValidationHelper.GetBoolean(isBundleItem, false);
    }


    /// <summary>
    /// Returns order item edit action HTML.
    /// </summary>
    protected string GetOrderItemEditAction(object value)
    {
        Guid itemGuid = ValidationHelper.GetGuid(value, Guid.Empty);

        if (itemGuid != Guid.Empty)
        {
            return string.Format("<img src=\"{0}\" onclick=\"javascript: OpenOrderItemDialog('{1}', '{2}')\" alt=\"\" title=\"{3}\" class=\"OrderItemEditLink\" style=\"cursor: pointer;\" />",
                GetImageUrl("Objects/Ecommerce_OrderItem/edit.png"),
                itemGuid,
                GetCMSDeskShoppingCartSessionName(),
                GetString("shoppingcart.editorderitem"));
        }

        return "";
    }


    /// <summary>
    /// Returns SKU edit action HTML.
    /// </summary>
    protected string GetSKUEditAction(object skuId, object skuSiteId, object isProductOption, object isBundleItem)
    {
        if (!ValidationHelper.GetBoolean(isProductOption, false) && !ValidationHelper.GetBoolean(isBundleItem, false) && ShoppingCartControl.IsInternalOrder)
        {
            string url = ResolveUrl("~/CMSModules/Ecommerce/Pages/Tools/Products/Product_Edit_Dialog.aspx");
            url = URLHelper.AddParameterToUrl(url, "productid", skuId.ToString());
            url = URLHelper.AddParameterToUrl(url, "siteid", skuSiteId.ToString());
            url = URLHelper.AddParameterToUrl(url, "dialogmode", "1");

            return string.Format("<img src=\"{0}\" onclick=\"modalDialog('{1}', 'SKUEdit', '95%', '95%'); return false;\" alt=\"\" title=\"{2}\" class=\"OrderItemEditLink\" style=\"cursor: pointer;\" />",
                GetImageUrl("Objects/Ecommerce_OrderItem/editsku.png"),
                url,
                GetString("shoppingcart.editproduct"));
        }

        return "";
    }


    /// <summary>
    /// Returns formatted child cart item units. Returns empty string if it is not product option or bundle item.
    /// </summary>
    /// <param name="skuUnits">SKU units</param>
    /// <param name="isProductOption">Indicates if cart item is product option</param>
    /// <param name="isBundleItem">Indicates if cart item is bundle item</param>
    protected static string GetChildCartItemUnits(object skuUnits, object isProductOption, object isBundleItem)
    {
        if (ValidationHelper.GetBoolean(isProductOption, false) || ValidationHelper.GetBoolean(isBundleItem, false))
        {
            return string.Format("<span>{0}</span>", skuUnits);
        }

        return "";
    }

    #endregion

    #region Ajout repeater

    protected string GetProductImage(object skuid)
    {
        SKUInfo sku = SKUInfoProvider.GetSKUInfo((int)skuid);
        if (sku != null)
        {
            string Disp = string.Empty;
            try
            {
                GeneralConnection cn = ConnectionHelper.GetConnection();
                string stringQuery = string.Format("select DISTINCT DispositionImage as Disp from View_CONTENT_Product_Joined where NodeSKUID = " + sku.SKUID);
                DataSet ds = cn.ExecuteQuery(stringQuery, null, CMS.SettingsProvider.QueryTypeEnum.SQLQuery, false);
                cn.Close();
                Disp = Convert.ToString(ds.Tables[0].Rows[0]["Disp"]);
            }
            catch
            {
                Disp = "1";
            }
            string Divclass = string.Empty;
            string Image = string.Empty;
            if (Disp == "1")
            {
                return "<div class=\"produit_vertical\">" + EcommerceFunctions.GetProductImage(sku.SKUImagePath, sku.SKUName) + "</div>";
            }
            else
            {
                return "<div class=\"produit_horizonal\">" + EcommerceFunctions.GetProductImage(sku.SKUImagePath, sku.SKUName) + "</div>";
            }
            //else if (Disp == "2") return "<div class=\"produit_horizonal\">" + EcommerceFunctions.GetProductImage(sku.SKUImagePath, sku.SKUName) + "</div>";
        }
        return String.Empty;
    }

    protected void RptCartItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        var drv = (System.Data.DataRowView)e.Item.DataItem;
        if (drv != null)
        {
            int currentSKUID = ValidationHelper.GetInteger(drv["SKUID"], 0);
            if (currentSKUID > 0)
            {
                SKUInfo sku = SKUInfoProvider.GetSKUInfo(currentSKUID);
                if (sku != null)
                {
                    int subTotal = 0;
                    double remise = 0;
                    //Display product image
                    var ltlProductImage = e.Item.FindControl("ltlProductImage") as Literal;
                    if (ltlProductImage != null)
                        //<%--# EcommerceFunctions.GetProductImage(Eval("SKUImagePath"), Eval("SKUName"))--%>
                        //ltlProductImage.Text = EcommerceFunctions.GetProductImage(sku.SKUImagePath, sku.SKUName);
                        ltlProductImage.Text = GetProductImage(sku.SKUID);

                    var ltlProductName = e.Item.FindControl("ltlProductName") as Literal;
                    if (ltlProductName != null)
                        ltlProductName.Text = sku.SKUName;

                    var txtProductCount = e.Item.FindControl("txtProductCount") as TextBox;

                    var ltlProductPrice = e.Item.FindControl("ltlProductPrice") as Literal;
                    if (txtProductCount != null)
                    {
                    

                        foreach (ShoppingCartItemInfo shoppingCartItem in ShoppingCart.CartItems)
                        {
                            
                            if (shoppingCartItem.SKUID == sku.SKUID)
                            {
                                
                                remise = shoppingCartItem.UnitTotalDiscount;
                                txtProductCount.Text = shoppingCartItem.CartItemUnits.ToString();
                                subTotal = shoppingCartItem.CartItemUnits;
                                
                                break;
                            }
                        }
                    }

                    
                    if (ltlProductPrice != null)
                    {
                       
                        // ltlProductPrice.Text = EcommerceFunctions.GetFormatedPrice((sku.SKUPrice - remise) * subTotal, sku.SKUDepartmentID, ShoppingCart, sku.SKUID,false );
                        //ltlProductPrice.Text = CurrencyInfoProvider.GetFormattedValue((sku.SKUPrice - remise) * subTotal, ShoppingCart.Currency);// EcommerceFunctions.GetFormatedPrice((sku.SKUPrice - remise) * subTotal, sku.SKUDepartmentID, sku.SKUID);
                        //ltlProductPrice.Text = string.Format("{0}<em>{1}</em>", ltlProductPrice.Text.Substring(0, ltlProductPrice.Text.Length - 1).Trim(), ltlProductPrice.Text.Substring(ltlProductPrice.Text.Length - 1,1).Trim());
                    }
                }
            }
        }
    }

    protected void RptCartItemCommand(object source, RepeaterCommandEventArgs e)
    {

        if (e.CommandName.Equals("Remove"))
        {
            var cartItemGuid = new Guid(e.CommandArgument.ToString());
            // Remove product and its product option from list
            this.ShoppingCart.RemoveShoppingCartItem(cartItemGuid);

            if (!this.ShoppingCartControl.IsInternalOrder)
            {
                // Delete product from database
               ShoppingCartItemInfoProvider.DeleteShoppingCartItemInfo(cartItemGuid);
               
            }
            if (IsShippingNeeded && ddlShippingCountry.Visible)
            {
                ddlShippingCountry_SelectedIndexChanged(null, null);
            }
            /*btnUpdate_Click1(null, null);
            ShowAddressList();*/

            if (ECommerceContext.CurrentShoppingCart.CartItems.Count == 0)
            {
                txtCoupon.Text = "";
                pnlBtnNext.Visible = false;
                pnlBtnNext1.Visible = false;
                btnNext.Visible = false;
                btnNext1.Visible = false;
            }
        }
        if (e.CommandName.Equals("Decrease"))
        {
            var cartItemGuid = new Guid(e.CommandArgument.ToString());
            ShoppingCartItemInfo cartItem = ShoppingCart.GetShoppingCartItem(cartItemGuid);
            if (cartItem != null)
            {
                if (cartItem.CartItemUnits - 1 > 0)
                {
                    cartItem.CartItemUnits--;
                    // Update units of child bundle items
                    foreach (ShoppingCartItemInfo bundleItem in cartItem.BundleItems)
                    {
                        bundleItem.CartItemUnits--;
                    }

                    if (!ShoppingCartControl.IsInternalOrder)
                    {
                        try
                        {
                            ShoppingCartItemInfoProvider.SetShoppingCartItemInfo(cartItem);
                        }
                        catch
                        {
                           // EventLogProvider ev = new EventLogProvider();
                          //  ev.LogEvent("I", DateTime.Now, "erreur cartitem Decrease", "code");
                        }

                        // Update product options in database
                        foreach (ShoppingCartItemInfo option in cartItem.ProductOptions)
                        {
                            ShoppingCartItemInfoProvider.SetShoppingCartItemInfo(option);
                        }
                    }
                    if (IsShippingNeeded && ddlShippingCountry.Visible)
                    {
                        ddlShippingCountry_SelectedIndexChanged(null, null);
                    }
                    /*btnUpdate_Click1(null, null);
                    ShowAddressList();*/
                }
            }
        }
        if (e.CommandName.Equals("Increase"))
        {
            var cartItemGuid = new Guid(e.CommandArgument.ToString());
            ShoppingCartItemInfo cartItem = ShoppingCart.GetShoppingCartItem(cartItemGuid);
            if (cartItem != null)
            {
                if (cartItem.CartItemUnits + 1 > 0)
                {
                    cartItem.CartItemUnits++;
                    // Update units of child bundle items
                    foreach (ShoppingCartItemInfo bundleItem in cartItem.BundleItems)
                    {
                        bundleItem.CartItemUnits++;
                    }

                    if (!ShoppingCartControl.IsInternalOrder)
                    {
                        try
                        {
                            ShoppingCartItemInfoProvider.SetShoppingCartItemInfo(cartItem);
                        }
                        catch
                        {
                           // EventLogProvider ev = new EventLogProvider();
                           // ev.LogEvent("I", DateTime.Now, "erreur cartitem increase", "code");
                        }

                        // Update product options in database
                        foreach (ShoppingCartItemInfo option in cartItem.ProductOptions)
                        {
                            ShoppingCartItemInfoProvider.SetShoppingCartItemInfo(option);
                        }
                    }
                    if (ddlShippingCountry.Visible)
                    {
                        ddlShippingCountry_SelectedIndexChanged(null, null);
                    }
                }
            }
        }
        if(IsShippingNeeded) ShippingUnit = ShippingExtendedInfoProvider.GetCartShippingUnit(ShoppingCart);
        btnUpdate_Click1(null, null);
      //  btncoupon_Click(null, null);
        ShowAddressList();

        // forcing postabck to refresh page
        Response.Redirect(Request.RawUrl);
    }

    #endregion
    protected void Button1_Click(object sender, EventArgs e)
    {

        if(IsShippingNeeded)
        {
            int PriceID = ShippingExtendedInfoProvider.GetCustomFieldValue(ShoppingCart, "ShoppingCartPriceID");
            ShippingExtendedInfoProvider.SetCustomFieldValue(ShoppingCart, "ShoppingCartCarriedOnPriceID", PriceID); 
        }
        // SessionHelper.SetValue("CarriedOnPriceID", SessionHelper.GetValue("PriceID"));
        ButtonNextClickAction();
        ButtonNextClickAction();
    }

    /// <summary>
    /// Back button clicked.
    /// </summary>
    protected void btnBack_Click(object sender, EventArgs e)
    {
        ButtonBackClickAction();
    }


    /// <summary>
    /// Next button clicked.
    /// </summary>
    protected void btnNext_Click(object sender, EventArgs e)
    {
        // SessionHelper.SetValue("CarriedOnPriceID", SessionHelper.GetValue("PriceID"));
        //if (Int32.Parse(ddlShippingCountry.SelectedValue) == 0)
        //{
        //    //Response.Write("<script>alert('Veuillez choisir un pays');</script>");
        //    //return;
        //}
        if (IsShippingNeeded)
        {
            int PriceID = ShippingExtendedInfoProvider.GetCustomFieldValue(ShoppingCart, "ShoppingCartPriceID");
            ShippingExtendedInfoProvider.SetCustomFieldValue(ShoppingCart, "ShoppingCartCarriedOnPriceID", PriceID);
        }


        ButtonNextClickAction();
        ButtonNextClickAction();
    }

    protected string GetProductNodeAliasPath(object skuid)
    {
        SKUInfo sku = SKUInfoProvider.GetSKUInfo((int)skuid);

        if (sku != null)
        {
            GeneralConnection cn = ConnectionHelper.GetConnection();
            string stringQuery = string.Format("select NodeAliasPath from View_CONTENT_Product_Joined where NodeSKUID = " + sku.SKUID);
            DataSet ds = cn.ExecuteQuery(stringQuery, null, CMS.SettingsProvider.QueryTypeEnum.SQLQuery, false);
            cn.Close();

            if (!DataHelper.DataSourceIsEmpty(ds))
            {
                string NodeAliasPath = Convert.ToString(ds.Tables[0].Rows[0]["NodeAliasPath"]);
                return "~" + NodeAliasPath + ".aspx";
            }
        }
        return String.Empty;
    }

    protected void buttonNewShippingAddress_Click(object sender, EventArgs e)
    {
        String siteName = SiteContext.CurrentSiteName;
        #region "Banned IPs"

        // Ban IP addresses which are blocked for registration
        if (!BannedIPInfoProvider.IsAllowed(siteName, BanControlEnum.Registration))
        {
            lblError.Visible = true;
            lblError.Text = GetString("banip.ipisbannedregistration");
            return;
        }
        #endregion


        //Update Customer
        CustomerInfo updateCustomer = ECommerceContext.CurrentCustomer;
        updateCustomer.CustomerEnabled = true;
        updateCustomer.CustomerLastModified = DateTime.Now;
        updateCustomer.CustomerSiteID = CMSContext.CurrentSiteID;
        updateCustomer.CustomerCompany = "";
        updateCustomer.CustomerOrganizationID = "";
        updateCustomer.CustomerTaxRegistrationID = "";
        CustomerInfoProvider.SetCustomerInfo(updateCustomer);

        #region "Insert new adress / Update selected adress"

        #region "Adresse"

        if (txtShippingadresse1.Text == "")
        {
            lblErrorShippingAdress.Visible = true;
            lblErrorShippingAdress.Text = "Veuillez saisir l'Adresse";
            return;
        }

        #endregion

        #region "CP"

        if (txtShippingcp.Text == "")
        {
            lblErrorShippingAdress.Visible = true;
            lblErrorShippingAdress.Text = "Veuillez saisir le Code Postal";
            return;
        }

        #endregion

        #region "Ville"

        if (txtShippingville.Text == "")
        {
            lblErrorShippingAdress.Visible = true;
            lblErrorShippingAdress.Text = "Veuillez saisir la Ville";
            return;
        }

        #endregion

        #region "Adresse"

        if ((chkShippingBillingAddr.Checked == false) && (chkShippingShippingAddr.Checked == false))
        {
            lblErrorShippingAdress.Visible = true;
            lblErrorShippingAdress.Text = "Veuillez mentionner le type d'Adresse";
            return;
        }

        #endregion

        #region "Pays"
        if (ddlShippingCountryAuthenticated.SelectedValue == "0")
        {
            lblErrorShippingAdress.Visible = true;
            lblErrorShippingAdress.Text = "Veuillez s�l�ctionner le pays";
            return;
        }
        #endregion

        #region "New adress"

        // Create new address object
        AddressInfo newAddress = new AddressInfo();

        int CountryID = ValidationHelper.GetInteger(ddlShippingCountryAuthenticated.SelectedValue, 0);
        CustomerInfo uc = ECommerceContext.CurrentCustomer;
        string mCustomerName = string.Format("{0} {1}", uc.CustomerFirstName, uc.CustomerLastName);
        // Set the properties
        newAddress.AddressName = string.Format("{0}, {4} {1} - {2} {3}", mCustomerName, txtShippingadresse1.Text, txtShippingcp.Text, txtShippingville.Text, txtnumero.Text);
        newAddress.AddressLine1 = txtShippingadresse1.Text;
        newAddress.AddressLine2 = txtShippingadresse2.Text;
        newAddress.AddressCity = txtShippingville.Text;
        newAddress.AddressZip = txtShippingcp.Text;
        newAddress.AddressIsBilling = chkShippingBillingAddr.Checked;
        newAddress.AddressIsShipping = chkShippingShippingAddr.Checked;
        newAddress.AddressEnabled = true;
        newAddress.AddressPersonalName = mCustomerName;
        newAddress.AddressCustomerID = uc.CustomerID;
        newAddress.AddressCountryID = CountryID;
        newAddress.SetValue("AddressNumber", txtnumero.Text);

        // Create the address
        AddressInfoProvider.SetAddressInfo(newAddress);

        ShoppingCart.ShoppingCartShippingAddressID = newAddress.AddressID;
        if (chkShippingBillingAddr.Checked)
        {
            ShoppingCart.ShoppingCartBillingAddressID = newAddress.AddressID;
        }
        ReloadData();
        ShowAddressList();

        #endregion


        #endregion
    }

    protected void RptPickShippingAddressItemCommand(object source, RepeaterCommandEventArgs e)
    {
        if (e.CommandName.Equals("Select"))
        {
            int AddressID = Convert.ToInt32(e.CommandArgument);
            AddressInfo ai = AddressInfoProvider.GetAddressInfo(AddressID);
            if(IsShippingNeeded && ai!=null) ShoppingCart.ShoppingCartShippingAddressID = ai.AddressID;
            ShowAddressList();
            ReloadData();
        }
        if (e.CommandName.Equals("Update"))
        {
            int AddressID = Convert.ToInt32(e.CommandArgument);
            AddressInfo ai = AddressInfoProvider.GetAddressInfo(AddressID);
            string s = ai.AddressZip;

            // txtShippingNumero
            var txtShippingNumero = e.Item.FindControl("txtShippingNumero") as TextBox;
            if (txtShippingNumero != null)
            {
                ai.SetValue("AddressNumber", txtShippingNumero.Text);
            }

            // txtShippingadresse1
            var txtShippingadresse1 = e.Item.FindControl("txtShippingadresse1") as TextBox;
            if (txtShippingadresse1 != null)
            {
                ai.AddressLine1 = txtShippingadresse1.Text;
            }

            // txtShippingadresse2
            var txtShippingadresse2 = e.Item.FindControl("txtShippingadresse2") as TextBox;
            if (txtShippingadresse2 != null)
            {
                ai.AddressLine2 = txtShippingadresse2.Text;
            }

            // txtcp
            TextBox txtShippingcp = e.Item.FindControl("txtShippingcp") as TextBox;
            if (txtShippingcp != null)
            {
                ai.AddressZip = txtShippingcp.Text;
                // Response.Write("<script>alert('This is Alert " + txtcp.Text + " " + DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.fff tt") + "');</script>");
            }
            // txtville
            var txtShippingville = e.Item.FindControl("txtShippingville") as TextBox;
            if (txtShippingville != null)
            {
                ai.AddressCity = txtShippingville.Text;
            }


            CustomerInfo uc = ECommerceContext.CurrentCustomer;
            string mCustomerName = string.Format("{0} {1}", uc.CustomerFirstName, uc.CustomerLastName);
            // Set the properties
            ai.AddressName = string.Format("{0}, {4} {1} - {2} {3}", mCustomerName, ai.AddressLine1, ai.AddressZip, ai.AddressCity, ai.GetStringValue("AddressNumber", string.Empty));
            /*
            // chkShippingAddr
            var chk_ShippingAddr = e.Item.FindControl("chk_ShippingAddr") as CheckBox;
            if (chk_ShippingAddr != null)
            {
                ai.AddressIsShipping = chk_ShippingAddr.Checked;
            }

            // chkShippingAddr
            var chk_ShippingAddr = e.Item.FindControl("chk_ShippingAddr") as CheckBox;
            if (chk_ShippingAddr != null)
            {
                ai.AddressIsShipping = chk_ShippingAddr.Checked;
            }*/
            AddressInfoProvider.SetAddressInfo(ai);
            if(IsShippingNeeded) ShoppingCart.ShoppingCartShippingAddressID = ai.AddressID;
            ShowAddressList();
            ReloadData();
        }
    }

    protected void RptPickShippingAddressItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        var drv = (System.Data.DataRowView)e.Item.DataItem;
        if (drv != null)
        {
            int addressID = ValidationHelper.GetInteger(drv["AddressID"], 0);
            if (addressID > 0)
            {
                AddressInfo ai = AddressInfoProvider.GetAddressInfo(addressID);

                var ltlShippingAddress = e.Item.FindControl("ltlShippingAddress") as Literal;
                if (ltlShippingAddress != null)
                {
                    ltlShippingAddress.Text = string.Format("{0}, {1}", ai.AddressName, MacroContext.CurrentResolver.ResolveMacros(CountryInfoProvider.GetCountryInfo(ai.AddressCountryID).CountryDisplayName));
                }

                // txtShippingNumero
                if (ai.GetValue("AddressNumber") != null)
                {
                    var txtShippingNumero = e.Item.FindControl("txtShippingNumero") as TextBox;
                    if (txtShippingNumero != null)
                    {
                        txtShippingNumero.Text = ai.GetStringValue("AddressNumber", string.Empty).Trim();
                    }

                }

                // wmNumero
                var wmNumero = e.Item.FindControl("wmNumero") as AjaxControlToolkit.TextBoxWatermarkExtender;
                if (wmNumero != null)
                {
                    wmNumero.WatermarkText = GetString("numerorue");
                }

                // txtShippingadresse1
                var txtShippingadresse1 = e.Item.FindControl("txtShippingadresse1") as TextBox;
                if (txtShippingadresse1 != null)
                {
                    txtShippingadresse1.Text = ai.AddressLine1.Trim();
                }

                // wmShipadresse1
                var wmShipadresse1 = e.Item.FindControl("wmShipadresse1") as AjaxControlToolkit.TextBoxWatermarkExtender;
                if (wmShipadresse1 != null)
                {
                    wmShipadresse1.WatermarkText = GetString("adresse1");
                }

                // txtShippingadresse2
                var txtShippingadresse2 = e.Item.FindControl("txtShippingadresse2") as TextBox;
                if (txtShippingadresse2 != null)
                {
                    txtShippingadresse2.Text = ai.AddressLine2.Trim();
                }

                // wmShipadresse2
                var wmShipadresse2 = e.Item.FindControl("wmShipadresse2") as AjaxControlToolkit.TextBoxWatermarkExtender;
                if (wmShipadresse2 != null)
                {
                    wmShipadresse2.WatermarkText = GetString("adresse2");
                }

                // txtcp
                var txtShippingcp = e.Item.FindControl("txtShippingcp") as TextBox;
                if (txtShippingcp != null)
                {
                    txtShippingcp.Text = ai.AddressZip.Trim();
                }

                // wmShipcp
                var wmShipcp = e.Item.FindControl("wmShipcp") as AjaxControlToolkit.TextBoxWatermarkExtender;
                if (wmShipcp != null)
                {
                    wmShipcp.WatermarkText = GetString("cp");
                }

                // txtville
                var txtShippingville = e.Item.FindControl("txtShippingville") as TextBox;
                if (txtShippingville != null)
                {
                    txtShippingville.Text = ai.AddressCity.Trim();
                }

                // wmShipville
                var wmShipville = e.Item.FindControl("wmShipville") as AjaxControlToolkit.TextBoxWatermarkExtender;
                if (wmShipville != null)
                {
                    wmShipville.WatermarkText = GetString("ville");
                }

                //txtShippingcountry
                var txtShippingcountry = e.Item.FindControl("txtShippingcountry") as TextBox;
                if (txtShippingcountry != null)
                {
                    txtShippingcountry.Text = MacroContext.CurrentResolver.ResolveMacros(CountryInfoProvider.GetCountryInfo(ai.AddressCountryID).CountryDisplayName);
                    txtShippingcountry.ReadOnly = true;
                }

            }
        }
    }

    private void ReloadShippingAdresses()
    {
        if (IsLiveSite && IsShippingNeeded)
        {
            if (CurrentUser.IsAuthenticated())
            {
                string where = string.Format("AddressCustomerID={0} AND AddressIsShipping=1 AND AddressEnabled = 1", ECommerceContext.CurrentCustomer.CustomerID.ToString());
                string orderby = "AddressID";
                DataSet ds = AddressInfoProvider.GetAddresses(where, orderby);
                RptPickShippingAddress.DataSource = ds;
                RptPickShippingAddress.DataBind();

                if (Session["newAddress"] != null)
                {
                    ShoppingCart.ShoppingCartShippingAddressID = Int32.Parse(Session["newAddress"].ToString());
                }
            }
        }
    }                    

          
}



