using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections.Specialized;
using System.Text;
using System.Security.Cryptography;
using CMS.Ecommerce;
using CMS.GlobalHelper;
using CMS.CMSHelper;
using CMS.SettingsProvider;
using CMS.Helpers;
using CMS.DataEngine;
using CMS.SiteProvider;
using CMS.Localization;

public partial class OgoneRedirect : System.Web.UI.Page
{

    private NameValueCollection _paymentValues;

    protected void Page_Load(object sender, EventArgs e)
    {
        //lblOgoneUrl.Text = SettingsKeyProvider.GetStringValue(CMSContext.CurrentSiteName + ".OgoneUrl");
        //lblOgoneID.Text = SettingsKeyProvider.GetStringValue(CMSContext.CurrentSiteName + ".OgonePSPID");
        //lblOgoneSHA.Text = SettingsKeyProvider.GetStringValue(CMSContext.CurrentSiteName + ".OgoneShaHandshake");

        //lblTest.Text = URLHelper.ResolveUrl("~/Webshop/ShoppingCart.aspx");
        if (Request.UrlReferrer == null)
        {
            ShowPageCallError();
            return;
        }

        var path = Request.UrlReferrer.AbsolutePath;
        var pathArray = path.Split('/');
        if (pathArray.Length < 2 || !pathArray[pathArray.Length - 1].Equals("Shoppingcart.aspx") || !pathArray[pathArray.Length - 2].Equals("Shopping"))
        {
            ShowPageCallError();
            return;
        }

        var orderId = QueryHelper.GetInteger("orderid", 0);
        if (orderId == 0)
        {
            ShowPageCallError();
            return;
        }

        body.Attributes.Add("onload", "window.document.forms[0].submit();");
        paymentForm.Action = SettingsKeyInfoProvider.GetStringValue(SiteContext.CurrentSiteName + ".OgoneUrl");
        LoadValues(orderId);
        SetFields();

    }

    private void ShowPageCallError()
    {
        lblInfo.Visible = true;
        lblInfo.Text = "Unexpected page call...";
    }

    private void LoadValues(int orderId)
    {
        this._paymentValues = new NameValueCollection();
        this._paymentValues.Add("PSPID", SettingsKeyInfoProvider.GetStringValue(SiteContext.CurrentSiteName + ".OgonePSPID"));
        this._paymentValues.Add("TXTCOLOR", "#444444");
        this._paymentValues.Add("TBLTXTCOLOR", "#444444");
        this._paymentValues.Add("TBLBGCOLOR", "#F7F7F7");
        this._paymentValues.Add("BGCOLOR", "#F7F7F7");
        this._paymentValues.Add("PM", "CreditCard");

       // this._paymentValues.Add("cancelurl", "hthttp://v2.portedorient.com/fr-BE/Webshop/ShoppingCart");
        var brand = QueryHelper.GetString("brand", String.Empty);
        if (!String.IsNullOrWhiteSpace(brand))
            this._paymentValues.Add("BRAND", brand);


        OrderInfo oi = OrderInfoProvider.GetOrderInfo(orderId);
        CustomerInfo ci = CustomerInfoProvider.GetCustomerInfo(oi.OrderCustomerID);
        CurrencyInfo cui = CurrencyInfoProvider.GetCurrencyInfo(oi.OrderCurrencyID);

        if (oi != null && ci != null && cui != null)
        {
            this._paymentValues.Add("orderID", String.Format("{0}", oi.OrderID));
            this._paymentValues.Add("amount", String.Format("{0}", Math.Floor(oi.OrderTotalPrice * 100)));
            this._paymentValues.Add("currency", cui.CurrencyCode);
            this._paymentValues.Add("language", String.Format("{0}", LocalizationContext.PreferredCultureCode));
            this._paymentValues.Add("CN", String.Format("{0} {1}", ci.CustomerFirstName, ci.CustomerLastName));
            this._paymentValues.Add("EMAIL", ci.CustomerEmail);
           
            AddressInfo ai = AddressInfoProvider.GetAddressInfo(oi.OrderBillingAddressID);
            if (ai != null)
            {
                this._paymentValues.Add("owneraddress", ai.AddressLine1);
                this._paymentValues.Add("ownerZIP", ai.AddressZip);
                this._paymentValues.Add("ownertown", ai.AddressCity);
                this._paymentValues.Add("ownertelno", ai.AddressPhone);
            }
        }

        AddShaSignature();
    }

    private void AddShaSignature()
    {
        this._paymentValues.Add("SHASign", CalculateShaSignature());
    }

    private void SetFields()
    {
        foreach (string name in this._paymentValues)
        {
            CreateHiddenField(name, this._paymentValues[name]);
        }
    }

    private void CreateHiddenField(string name, string value)
    {
        HiddenField field = new HiddenField();
        field.ID = name;
        field.Value = value;
        paymentForm.Controls.Add(field);
    }

    private string CalculateShaSignature()
    {
        string verySecretKey = SettingsKeyInfoProvider.GetStringValue(SiteContext.CurrentSiteName + ".OgoneShaHandshake");

        List<string> paymentValuesAsString = new List<string>();

        foreach (string name in this._paymentValues)
        {
            if (!String.IsNullOrEmpty(this._paymentValues[name]))
            {
                paymentValuesAsString.Add(String.Format("{0}={1}{2}", name.ToUpper(), this._paymentValues[name], verySecretKey));
            }
        }

        string[] bitsAndPieces = paymentValuesAsString.ToArray();
        Array.Sort(bitsAndPieces);

        string toHash = String.Join(String.Empty, bitsAndPieces);

        byte[] buffer = Encoding.ASCII.GetBytes(toHash);
        SHA1CryptoServiceProvider cryptoTransformSHA1 = new SHA1CryptoServiceProvider();
        return BitConverter.ToString(cryptoTransformSHA1.ComputeHash(buffer)).Replace("-", "");
    }
}