using System;

using CMS.CMSHelper;
using CMS.Ecommerce;
using CMS.ExtendedControls;
using CMS.GlobalHelper;
using CMS.UIControls;
using CMS.Helpers;

[Security(Resource = "CMS.Ecommerce", UIElements = "Orders.Invoice")]
public partial class CMSModules_Ecommerce_Pages_Tools_Orders_Order_Edit_Invoice : CMSOrdersPage
{
    #region "Variables"

    private int orderId;
    private OrderInfo order;

    #endregion


    #region "Page events"

    protected override void OnPreInit(EventArgs e)
    {
        CustomerID = QueryHelper.GetInteger("customerid", 0);
        base.OnPreInit(e);
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        txtInvoiceNumber.Visible = true;
        lblInvoiceNumber.Visible = true;
        btnGenerate.Visible = true;
        btnGenerate1.Visible = false;
        // Register the dialog script
        ScriptHelper.RegisterClientScriptBlock(this, typeof(string), ScriptHelper.NEWWINDOW_SCRIPT_KEY, ScriptHelper.NewWindowScript);

        lblInvoiceNumber.Text = GetString("order_invoice.lblInvoiceNumber");
        btnGenerate.Text = "Facture";//GetString("order_invoice.btnGenerate");
        btnPrintPreview.Text = "Impression";//GetString("order_invoice.btnPrintPreview");
        btnPrintPreview1.Text ="Adresse livraison";// GetString("order_invoice.btnPrintPreview");
        if (QueryHelper.GetInteger("orderid", 0) != 0)
        {
            orderId = QueryHelper.GetInteger("orderid", 0);
        }
        order = OrderInfoProvider.GetOrderInfo(orderId);

        if (order == null)
        {
            btnGenerate.Enabled = false;
            btnPrintPreview.Enabled = false;
            return;
        }
        else
        {
            // Check order site ID
            CheckOrderSiteID(order.OrderSiteID);
            string existfacture = order.GetStringValue("Facture",string.Empty);
            
            if (!string.IsNullOrEmpty(existfacture))
            {
                txtInvoiceNumber.Visible = false;
                lblInvoiceNumber.Visible = false;
                btnGenerate.Visible = false;
            }
        }

        ltlScript.Text = ScriptHelper.GetScript("function showPrintPreview() { NewWindow('Order_Edit_InvoicePrint.aspx?orderid=" + orderId + "', 'InvoicePrint', 650, 700);}function showPrintPreview1() { NewWindow('Order_Edit_AdrPrint.aspx?orderid=" + orderId + "', 'InvoicePrint', 650, 700);}");
       // ltlScript.Text =ScriptHelper.GetScript("function showPrintPreview1() { NewWindow('Order_Edit_AdrPrint.aspx?orderid=" + orderId + "', 'InvoicePrint', 650, 700);}");

        if (!RequestHelper.IsPostBack())
        {
            txtInvoiceNumber.Text = CMS.MacroEngine.MacroContext.CurrentResolver.ResolveMacros("{#invoice#}");//order.OrderInvoiceNumber;
            lblInvoice.Text = URLHelper.MakeLinksAbsolute(order.OrderInvoice);
            btnGenerate_Click1();
        }
        
       
    }

    #endregion


    #region "Event handlers"

    protected void btnGenerate_Click(object sender, EventArgs e)
    {
        // check 'EcommerceModify' permission
        if (!ECommerceContext.IsUserAuthorizedForPermission("ModifyOrders"))
        {
            RedirectToAccessDenied("CMS.Ecommerce", "EcommerceModify OR ModifyOrders");
        }

        // Save updated order invoice number
     //   order.OrderInvoiceNumber = txtInvoiceNumber.Text;
        order.SetValue("facture",txtInvoiceNumber.Text);
        OrderInfoProvider.SetOrderInfo(order);

        // Generate and display new invoice
        string invoice = OrderInfoProvider.GetInvoice(orderId);
        lblInvoice.Text = URLHelper.MakeLinksAbsolute(invoice);

        // Save new invoice
        order.OrderInvoice = invoice;
        OrderInfoProvider.SetOrderInfo(order);

        // Show message
        ShowChangesSaved();
    }
    protected void btnGenerate_Click1(object sender, EventArgs e)
    {
        // check 'EcommerceModify' permission
        if (!ECommerceContext.IsUserAuthorizedForPermission("ModifyOrders"))
        {
            RedirectToAccessDenied("CMS.Ecommerce", "EcommerceModify OR ModifyOrders");
        }

        // Save updated order invoice number
        //   order.OrderInvoiceNumber = txtInvoiceNumber.Text;
       // order.SetValue("facture", txtInvoiceNumber.Text);
       // OrderInfoProvider.SetOrderInfo(order);

        // Generate and display new invoice
        string invoice = OrderInfoProvider.GetInvoice(orderId);
        lblInvoice.Text = URLHelper.MakeLinksAbsolute(invoice);

        // Save new invoice
        order.OrderInvoice = invoice;
       OrderInfoProvider.SetOrderInfo(order);

        // Show message
        ShowChangesSaved();
    }
    private void btnGenerate_Click1()
    {
        // check 'EcommerceModify' permission
        if (!ECommerceContext.IsUserAuthorizedForPermission("ModifyOrders"))
        {
            RedirectToAccessDenied("CMS.Ecommerce", "EcommerceModify OR ModifyOrders");
        }

        // Save updated order invoice number
        //   order.OrderInvoiceNumber = txtInvoiceNumber.Text;
        // order.SetValue("facture", txtInvoiceNumber.Text);
        // OrderInfoProvider.SetOrderInfo(order);

        // Generate and display new invoice
        string invoice = OrderInfoProvider.GetInvoice(orderId);
        lblInvoice.Text = URLHelper.MakeLinksAbsolute(invoice);

        // Save new invoice
        order.OrderInvoice = invoice;
        OrderInfoProvider.SetOrderInfo(order);

        // Show message
        //ShowChangesSaved();
    }
    #endregion
}