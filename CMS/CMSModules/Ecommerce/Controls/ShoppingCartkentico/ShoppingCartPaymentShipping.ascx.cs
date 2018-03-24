using System;
using System.Data;
using System.Web.UI.WebControls;

using CMS.CMSHelper;
using CMS.GlobalHelper;
using CMS.Ecommerce;
using CMS.EcommerceProvider;
using CMS.SettingsProvider;
using CMS.SiteProvider;
using CMS.DataEngine;
using CMS.EventLog;
using CMS.Globalization;
using CMS.Helpers;

public partial class CMSModules_Ecommerce_Controls_ShoppingCart_ShoppingCartPaymentShipping : ShoppingCartStep
{
    #region "ViewState Constants"

    private const string SHIPPING_OPTION_ID = "OrderShippingOptionID";
    private const string PAYMENT_OPTION_ID = "OrderPaymenOptionID";
	private readonly static EventLogProvider evt = new EventLogProvider();

    #endregion


    #region "Variables"

    private bool? mIsShippingNeeded = null;

    #endregion


    #region "Properties"

    /// <summary>
    /// Returns true if shopping cart items need shipping.
    /// </summary>
    protected bool IsShippingNeeded
    {
        get
        {
            //return true;
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

    #endregion


    #region "Page methods"
    // declaration
    private int mCustomerId = 0, ShippingUnit = 0;
    protected void Page_Load(object sender, EventArgs e)
    {
		EventLogProvider p = new EventLogProvider();
        // Init labels
        lblTitle.Text = GetString("shoppingcart.shippingpaymentoptions");
        lblPayment.Text = GetString("shoppingcartpaymentshipping.payment");
        lblShipping.Text = GetString("shoppingcartpaymentshipping.shipping");

        selectShipping.IsLiveSite = IsLiveSite;
        selectPayment.IsLiveSite = IsLiveSite;

        if ((ShoppingCart != null) && (SiteContext.CurrentSite != null))
        {
            if (ShoppingCart.CountryID == 0)
            {
                string countryName = ECommerceSettings.DefaultCountryName(SiteContext.CurrentSite.SiteName);
                CountryInfo ci = CountryInfoProvider.GetCountryInfo(countryName);
                ShoppingCart.CountryID = (ci != null) ? ci.CountryID : 0;
            }

            selectShipping.ShoppingCart = ShoppingCart;
        }

        if (!ShoppingCartControl.IsCurrentStepPostBack)
        {
            if (IsShippingNeeded)
            {
				p.LogEvent("I", DateTime.Now, "SHIPPING NEED TRUE" , "");
                SelectShippingOption();
            }
            else
            {
				p.LogEvent("I", DateTime.Now, "SHIPPING NEED FALSE" , "");
                // Don't use shipping selection
                selectShipping.StopProcessing = true;

                // Hide title
                lblTitle.Visible = false;

                // Change current checkout process step caption
                ShoppingCartControl.CheckoutProcessSteps[ShoppingCartControl.CurrentStepIndex].Caption = GetString("order_new.paymentshipping.titlenoshipping");
            }
        }
        AddressInfo aiBill = AddressInfoProvider.GetAddressInfo(ShoppingCart.ShoppingCartBillingAddressID);
        AddressInfo aiShip = AddressInfoProvider.GetAddressInfo(ShoppingCart.ShoppingCartShippingAddressID);
        if (aiBill != null && aiShip != null)
        {
            addressData.Text = string.Format("<h1> ADDRESS DATA </h1> <br/>{0} - {1} <br/> {2} - {3}", aiBill.AddressID.ToString(), aiBill.AddressCity, aiShip.AddressID.ToString(), aiShip.AddressCity);
        }
        else
        {
            if (aiBill == null)
            {
                addressData.Text = "AIBILL NULL";
            }
            if (aiShip == null)
            {
                addressData.Text = string.Format("{0} AISHIP NULL", addressData.Text);
            }

        }
        // bind drop ddlShippingOption
        DataSet ds, dsoi = null;

        double vat = ShippingExtendedInfoProvider.GetCartShippingVatRate(ShoppingCart);
        
        p.LogEvent("I", DateTime.Now, "TVA du BO : " + vat, "code B.O");
        if (vat > 0)
        {
            vat = 1.06;
        }
        else
        {
            vat = 1;
        }

		p.LogEvent("I", DateTime.Now, "ShoppingCart.ShoppingCartShippingAddressID : " + ShoppingCart.ShoppingCartShippingAddressID, "code B.O");
		var addrezz = AddressInfoProvider.GetAddressInfo(ShoppingCart.ShoppingCartShippingAddressID);
		if(addrezz!=null){
			var newCountryId = addrezz.AddressCountryID;
			QueryDataParameters parameters = new QueryDataParameters();
			ShippingUnit = ShippingExtendedInfoProvider.GetCartShippingUnit(ShoppingCart);
			p.LogEvent("I", DateTime.Now, "shipping du B.O : " + ShippingUnit, "code B.O");
			parameters.Add("@ShippingUnits", ShippingUnit);
			parameters.Add("@CountryID", newCountryId);
			parameters.Add("@VATRate", vat);
			//parameters.Add("@VATRate", 1 + ShippingExtendedInfoProvider.GetCartShippingVatRate(ShoppingCart) / 100);
			GeneralConnection cn = ConnectionHelper.GetConnection();
			ds = cn.ExecuteQuery("customtable.shippingextension.ShippingCostListByCountry", parameters);

			if (!DataHelper.DataSourceIsEmpty(ds))
			{
				DataTable dt = ds.Tables[0];
				foreach (DataRow drow in dt.Rows)
				{
					double price = Convert.ToDouble(drow["ShippingFinalCost"]);
					string prices = CurrencyInfoProvider.GetFormattedPrice(price, ShoppingCart.Currency);
					drow["DisplayString"] = string.Format("{0}- {1}", drow["ShippingOptionDisplayName"].ToString(), prices);
				}

				ddlShippingOption.DataSource = ds;
				ddlShippingOption.SelectedIndex = -1;
				ddlShippingOption.SelectedValue = null;
				ddlShippingOption.ClearSelection();
				ddlShippingOption.DataTextField = "DisplayString";
				ddlShippingOption.DataValueField = "ItemID";
				ddlShippingOption.DataBind();
				ddlShippingOption.AutoPostBack = (ddlShippingOption.Items.Count > 1);
				// string value = ValidationHelper.GetString(SessionHelper.GetValue("CarriedOnPriceID"), string.Empty);
				string value = ValidationHelper.GetString(ShippingExtendedInfoProvider.GetCustomFieldValue(ShoppingCart, "ShoppingCartCarriedOnPriceID"), string.Empty);

				if (!string.IsNullOrEmpty(value) && ddlShippingOption.Items.Count > 1)
				{
					if (int.Parse(value) > 0)
					{
						// SessionHelper.SetValue("CarriedOnPriceID", string.Empty);
						ShippingExtendedInfoProvider.SetCustomFieldValue(ShoppingCart, "ShoppingCartCarriedOnPriceID", -1);
						try
						{
							ddlShippingOption.SelectedValue = value;
						}
						catch
						{
						}
					}
				}
				//int PriceID = ValidationHelper.GetInteger(ddlShippingOption.SelectedValue, -1);
				//SessionHelper.SetValue("PriceID", PriceID);

				// SessionHelper.SetValue("CountryID", ai.AddressCountryID);
				ShippingExtendedInfoProvider.SetCustomFieldValue(ShoppingCart, "ShoppingCartCountryID", newCountryId);

				ddlShippingOption_SelectedIndexChanged(null, null);
				//btnUpdate_Click1(null, null);
			}

			else
			{
				// NO SHIPPING AVAILABLE
				ddlShippingOption.Items.Clear();
				ddlShippingOption.DataSource = null;
				ListItem listItem = new ListItem("Votre choix", "-1");
				ddlShippingOption.Items.Add(listItem);
			}
		}
        
		
        // bind drop ddlPaymentOption
        string where = "PaymentOptionEnabled=1 AND PaymentOptionID != 9";
        string orderby = "PaymentOptionName";
        //DataSet ds = PaymentOptionInfoProvider.GetPaymentOptions(CurrentSite.SiteID, true);
        DataSet ds2 = PaymentOptionInfoProvider.GetPaymentOptions(where, orderby);
        if (!DataHelper.DataSourceIsEmpty(ds2))
        {
            ddlPaymentOption.DataSource = ds2;
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

    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        if (selectShipping.HasData)
        {
            // Show shipping selection
            plcShipping.Visible = true;

            // Initialize payment options for selected shipping option
            selectPayment.ShippingOptionID = selectShipping.ShippingID;
            selectPayment.PaymentID = -1;
            selectPayment.DisplayOnlyAllowedIfNoShipping = false;
        }
        else
        {
            selectPayment.DisplayOnlyAllowedIfNoShipping = true;
        }

        selectPayment.ReloadData();

        SelectPaymentOption();

        plcPayment.Visible = selectPayment.HasData;
    }

    #endregion


    /// <summary>
    /// Back button actions.
    /// </summary>
    public override void ButtonBackClickAction()
    {
        // Save the values to ShoppingCart ViewState
        ShoppingCartControl.SetTempValue(SHIPPING_OPTION_ID, selectShipping.ShippingID);
        ShoppingCartControl.SetTempValue(PAYMENT_OPTION_ID, selectPayment.PaymentID);

        base.ButtonBackClickAction();
    }

    private int GetShippingID()
    {
        int result = 0;
        /*GeneralConnection cn= ConnectionHelper.GetConnection();
        string priceID = ValidationHelper.GetString(SessionHelper.GetValue("PriceID") ,"0");
        string stringQuery = string.Format("SELECT ShippingoptionID FROM customtable_shippingextensioncountry WHERE itemid IN (SELECT shippingextensioncountryID FROM customtable_shippingextensionpricing WHERE itemID={0})", priceID);
        DataSet ds = cn.ExecuteQuery(stringQuery, null, CMS.SettingsProvider.QueryTypeEnum.SQLQuery, false);
        if (!DataHelper.DataSourceIsEmpty(ds))
        {
            result = Convert.ToInt32((ds.Tables[0].Rows[0]["ShippingoptionID"])); 
        }*/
        int PriceID = ShippingExtendedInfoProvider.GetCustomFieldValue(ShoppingCart, "ShoppingCartPriceID");
        int ShippingUnit = ShippingExtendedInfoProvider.GetCartShippingUnit(ShoppingCart);
        //string priceID = ValidationHelper.GetString(SessionHelper.GetValue("PriceID"), "0");
        string priceID = PriceID.ToString();
        QueryDataParameters parameters = new QueryDataParameters();
        parameters.Add("@ShippingUnits", ShippingUnit);
        parameters.Add("@CountryID", (AddressInfoProvider.GetAddressInfo(ShoppingCart.ShoppingCartShippingAddressID)).AddressCountryID);
        parameters.Add("@VATRate", 1 + ShippingExtendedInfoProvider.GetCartShippingVatRate(ShoppingCart) / 100);
        string where = string.Format("ItemID={0}", priceID);
        GeneralConnection cn = ConnectionHelper.GetConnection();
        DataSet ds = cn.ExecuteQuery("customtable.shippingextension.ShippingCostListByCountry", parameters, where);


        if (!DataHelper.DataSourceIsEmpty(ds))
        {
            foreach (DataRow drow in ds.Tables[0].Rows)
            {
                result = ValidationHelper.GetInteger(drow["ShippingoptionID"], 0);
                if (priceID == drow["ItemID"].ToString())
                {
                    return result;
                }
            }
        }
        return result;
    }

    public override bool ProcessStep()
    {
        try
        {
            //int paymentID = ValidationHelper.GetInteger(SessionHelper.GetValue("PaymentID"), 0);
            int paymentID = ShippingExtendedInfoProvider.GetCustomFieldValue(ShoppingCart, "ShoppingCartPaymentID");

            // Cleanup the ShoppingCart ViewState
            ShoppingCartControl.SetTempValue(SHIPPING_OPTION_ID, null);

            ShoppingCartControl.SetTempValue(PAYMENT_OPTION_ID, null);

            //ShoppingCart.ShoppingCartShippingOptionID = selectShipping.ShippingID;
            //ShoppingCart.ShoppingCartPaymentOptionID = selectPayment.PaymentID;
            ShoppingCart.ShoppingCartShippingOptionID = IsShippingNeeded ? GetShippingID() : selectShipping.ShippingID;
            ShoppingCart.ShoppingCartPaymentOptionID = paymentID;

            // Update changes in database only when on the live site
            if (!ShoppingCartControl.IsInternalOrder)
            {
                ShoppingCartInfoProvider.SetShoppingCartInfo(ShoppingCart);
            }
            return true;
        }
        catch (Exception ex)
        {
            lblError.Visible = true;
            lblError.Text = ex.Message + " /step " + IsShippingNeeded.ToString();
            return false;
        }
    }


    /// <summary>
    /// Preselects payment option.
    /// </summary>
    protected void SelectPaymentOption()
    {
        try
        {
            // Try to select payment from ViewState first
            object viewStateValue = ShoppingCartControl.GetTempValue(PAYMENT_OPTION_ID);
            if (viewStateValue != null)
            {
                selectPayment.PaymentID = Convert.ToInt32(viewStateValue);
            }
            // Try to select payment option according to saved option in shopping cart object
            else if (ShoppingCart.ShoppingCartPaymentOptionID > 0)
            {
                selectPayment.PaymentID = ShoppingCart.ShoppingCartPaymentOptionID;
            }
            // Try to select payment option according to user preffered option
            else
            {
                CustomerInfo customer = ShoppingCart.Customer;
                int paymentOptionId = (customer.CustomerUser != null) ? customer.CustomerUser.GetUserPreferredPaymentOptionID(SiteContext.CurrentSiteName) : 0;
                if (paymentOptionId > 0)
                {
                    selectPayment.PaymentID = paymentOptionId;
                }
            }
        }
        catch
        {
        }
    }


    /// <summary>
    /// Preselects shipping option.
    /// </summary>
    protected void SelectShippingOption()
    {
        try
        {
            // Try to select shipping from ViewState first
            object viewStateValue = ShoppingCartControl.GetTempValue(SHIPPING_OPTION_ID);
            if (viewStateValue != null)
            {
                selectShipping.ShippingID = Convert.ToInt32(viewStateValue);
            }
            // Try to select shipping option according to saved option in shopping cart object
            else if (ShoppingCart.ShoppingCartShippingOptionID > 0)
            {
                selectShipping.ShippingID = ShoppingCart.ShoppingCartShippingOptionID;
            }
            // Try to select shipping option according to user preffered option
            else
            {
                CustomerInfo customer = ShoppingCart.Customer;
                int shippingOptionId = (customer.CustomerUser != null) ? customer.CustomerUser.GetUserPreferredShippingOptionID(SiteContext.CurrentSiteName) : 0;
                if (shippingOptionId > 0)
                {
                    selectShipping.ShippingID = shippingOptionId;
                }
            }
        }
        catch
        {
        }
    }


    public override bool IsValid()
    {
        string errorMessage = "";

        // If shipping is required
        if (plcShipping.Visible)
        {
            if (selectShipping.ShippingID <= 0)
            {
                //errorMessage = GetString("Order_New.NoShippingOption");
            }
        }

        // If payment is required
        if (plcPayment.Visible)
        {
            if ((errorMessage == "") && (selectPayment.PaymentID <= 0))
            {
                //errorMessage = GetString("Order_New.NoPaymentMethod");
            }
        }

        if (errorMessage == "")
        {
            // Form is valid
            return true;
        }

        // Form is not valid
        lblError.Visible = true;
        lblError.Text = errorMessage;
        return false;
    }

    protected void ddlShippingOption_SelectedIndexChanged(object sender, EventArgs e)
    {
        int PriceID = ValidationHelper.GetInteger(ddlShippingOption.SelectedValue, -1);
        int CountryID = (AddressInfoProvider.GetAddressInfo(ShoppingCart.ShoppingCartShippingAddressID)).AddressCountryID;

        // SessionHelper.SetValue("PriceID", PriceID);
        // SessionHelper.SetValue("CountryID", CountryID);

        ShippingExtendedInfoProvider.SetCustomFieldValue(ShoppingCart, "ShoppingCartPriceID", PriceID);
        ShippingExtendedInfoProvider.SetCustomFieldValue(ShoppingCart, "ShoppingCartCountryID", CountryID);

        //btnUpdate_Click1(null, null);
        //DisplayTotalPrice();
    }

    protected void ddlPaymentOption_SelectedIndexChanged(object sender, EventArgs e)
    {
        int PaymentID = ValidationHelper.GetInteger(ddlPaymentOption.SelectedValue, 9);
        ShippingExtendedInfoProvider.SetCustomFieldValue(ShoppingCart, "ShoppingCartPaymentID", PaymentID);
    }

    protected void Button1_Click(object sender, EventArgs e)
    {
        ButtonNextClickAction();
    }
}