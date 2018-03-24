using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.Ecommerce;
using CMS.EcommerceProvider;
using CMS.SiteProvider;
using CMS.GlobalHelper;
using CMS.CMSHelper;
using System.Text;

public partial class Servranx_Controls_TzPaymentResult : System.Web.UI.UserControl
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            // C'est un paiement Ogone
            if (!String.IsNullOrEmpty(Request.QueryString["ORDERID"]))

                try
                {

                    DoTraitement();
                    // PrintInfo(); for debug only
                }
                catch (Exception ex)
                {
                    lbError.Text = ex.Message;
                }
        }
    }

    private void DoTraitement()
    {
        var ordeID = Request.QueryString["ORDERID"];
        var order = OrderInfoProvider.GetOrderInfo(Int32.Parse(ordeID));
        if (order == null)
            return;

        var transactionID = Request.QueryString["PAYID"];
        string transactionDate = Request.QueryString["TRXDATE"];
        var modePaiement = Request.QueryString["PM"];
        var statutCode = Request.QueryString["STATUS"];
        var ncerror = Request.QueryString["NCERROR"];
        var sha = Request.QueryString["SHASIGN"];


        var tab = transactionDate.Split('/');
        var month = Int32.Parse(tab[0]);
        var day = Int32.Parse(tab[1]);
        var year = Int32.Parse(tab[2]);

        var date = new DateTime(year, month, day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);


        var payementResult = new PaymentResultInfo()
        {
            PaymentDate = GetOgoneTransactionDate(transactionDate),
            PaymentTransactionID = transactionID,
            PaymentIsCompleted = true,
            PaymentStatusName = GetOgoneStatutLibelle(statutCode),
            PaymentMethodName = "Ogone"

        };

        var paymentItem = new PaymentResultItemInfo()
        {
            Header= "Payment by",
            Name = "PaymentSytem",
            Text = modePaiement,
            Value = modePaiement
        };

        payementResult.SetPaymentResultItemInfo(paymentItem);

        order.OrderPaymentResult = payementResult;

        if (statutCode != "1" && statutCode != "0")
        {
            order.OrderStatusID = OrderStatusInfoProvider.GetOrderStatusInfo("PaymentReceived",SiteContext.CurrentSiteName).StatusID;
            order.SetValue("OrderStatus","1");
        }
        else
        {
            order.OrderStatusID = OrderStatusInfoProvider.GetOrderStatusInfo("Canceled",SiteContext.CurrentSiteName).StatusID;
            order.SetValue("OrderStatus", "2");
        }

        OrderHelper.CreateCustomInvoiceHelper(Int32.Parse(ordeID));
        OrderInfoProvider.SetOrderInfo(order);

    }


    private string GetOgoneStatutLibelle(string statutCode)
    {
        var statutLibelle = String.Empty;
        switch (statutCode)
        {
            case "5": statutLibelle = "Authorized"; break;
            case "0": statutLibelle = "Invalid"; break;
            case "1": statutLibelle = "Canceled"; break;
            case "9": statutLibelle = "Payment requested"; break;
        }
        return statutLibelle;
    }

    private DateTime GetOgoneTransactionDate(string dateBrute)
    {
        var tab = dateBrute.Split('/');
        var month = Int32.Parse(tab[0]);
        var day = Int32.Parse(tab[1]);
        var year = Int32.Parse(tab[2]);

        return new DateTime(year, month, day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
    }

    private void PrintInfo()
    {
        lbOrderID.Text = String.Format("Commande: {0}", Request.QueryString["ORDERID"]);
        lbTranscationID.Text = String.Format("Transaction: {0}", Request.QueryString["PAYID"]);
        lbTransactionDate.Text = String.Format("Date: {0}", Request.QueryString["TRXDATE"]);
        lbModePaiment.Text = String.Format("Mode de paiement: {0}", Request.QueryString["PM"]);
        lbStatus.Text = String.Format("Status: {0}", GetOgoneStatutLibelle(Request.QueryString["STATUS"]));
    }

}
