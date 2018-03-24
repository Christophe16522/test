using System;
using System.Collections.Generic;
using System.Text;
using CMS.CMSHelper;
using CMS.EmailEngine;
using CMS.GlobalHelper;
using System.Data;
using CMS.Ecommerce;
using CMS.SettingsProvider;
using CMS.SiteProvider;
using CMS.EventLog;
using CMS.Helpers;
using CMS.MacroEngine;
using CMS.DataEngine;


public partial class Servranx_Controls_status : System.Web.UI.UserControl
{
	private EventLogProvider ev = new EventLogProvider();
    protected void Page_Load(object sender, EventArgs e)
    {
        if (ECommerceContext.CurrentShoppingCart != null)
        {
            ShoppingCartInfoProvider.DeleteShoppingCartInfo(ECommerceContext.CurrentShoppingCart);
            ECommerceContext.CurrentShoppingCart = null;
        }

		
        string strOrderId = QueryHelper.GetText("orderID", string.Empty);
        int orderid = ValidationHelper.GetInteger(strOrderId, 0);
        string status = QueryHelper.GetString("STATUS", string.Empty);//HttpContext.Current.Request.QueryString["STATUS"];
		ev.LogEvent("I", DateTime.Now, "status", status);
		ev.LogEvent("I", DateTime.Now, "orderid", strOrderId);

        if (status == "5" && orderid > 0)
        {
			ev.LogEvent("I", DateTime.Now, "ICI 2", "");
			var order = OrderInfoProvider.GetOrderInfo(orderid);
            if (order!=null)
            {
                ev.LogEvent("I", DateTime.Now, "Order found", "");
                // Update the property
                order.OrderIsPaid = true;
                order.OrderStatusID=7;

                // Update the order
                OrderInfoProvider.SetOrderInfo(order);

                //Sendmail link of download to customer
                var listModel = GetDownLoadFile(orderid);
                var oi = OrderInfoProvider.GetOrderInfo(orderid);
                var currentCust = CustomerInfoProvider.GetCustomerInfo(oi.OrderCustomerID);
                string mail = currentCust.CustomerEmail;
                ev.LogEvent("I", DateTime.Now, "cust mail : " + mail, "");
                if (listModel != null)
                {
                    ev.LogEvent("I", DateTime.Now, "send mail to : " + mail, "");
                    SendMail(mail, listModel);
                }
                
            }
        }
    }


    protected  System.Collections.Generic.IList<ProductBookModel> GetDownLoadFile(int orderId)
    {
        var oi = OrderInfoProvider.GetOrderInfo(orderId);
        DataSet ds = OrderItemSKUFileInfoProvider.GetOrderItemSKUFiles(orderId);
        if (!DataHelper.DataSourceIsEmpty(ds) && oi!=null && oi.OrderIsPaid)
        {
            IList<ProductBookModel> list = new List<ProductBookModel>();
            foreach (DataRow reader in ds.Tables[0].Rows)
            {
                string myFileUrl = URLHelper.ResolveUrl(
                    OrderItemSKUFileInfoProvider.GetOrderItemSKUFileUrl(
                        ValidationHelper.GetGuid(reader["Token"], Guid.Empty),
                        ValidationHelper.GetString(reader["FileName"], string.Empty),
                        ValidationHelper.GetInteger(reader["OrderSiteID"], 0)));
              
                if (!string.IsNullOrEmpty(myFileUrl))
                {
                    string myFileName = ValidationHelper.GetString(reader["FileName"], string.Empty);
                    string myPName = HTMLHelper.HTMLEncode(
                        ResHelper.LocalizeString(ValidationHelper.GetString(reader["OrderItemSKUName"], null)));
                    var model = new ProductBookModel()
                    {
                        filename = myFileName,
                        fileUrl = String.Format("<a href=\"{0}\" target=\"_blank\">{1}</a>)", myFileUrl, HTMLHelper.HTMLEncode(myFileName)),
                        productname = myPName
                    };
                    list.Add(model);

                }
            }
            return list;
        }
        return null;
    }


    private void SendMail(string mail, IEnumerable<ProductBookModel> list)
    {
        SiteInfo currentSite = SiteContext.CurrentSite;
        ContextResolver resolver = MacroContext.CurrentResolver;
        string emailSubject = null;
        EmailTemplateInfo template = null;
        template = EmailTemplateProvider.GetEmailTemplate("SendProductLinkDownload", currentSite.SiteName);
        emailSubject = EmailHelper.GetSubject(template, "Demande de contact");

        //mail type
        if (template != null)
        {
            ev.LogEvent("I", DateTime.Now, "Template!=null ", (template != null).ToString());
            resolver.SourceParameters = null;
            EmailMessage email = new EmailMessage();
            email.EmailFormat = EmailFormatEnum.Default;
            email.From =
                EmailHelper.GetSender(template,
                SettingsKeyInfoProvider.GetStringValue(currentSite.SiteName + ".CMSAdminEmailAddress"));
            email.Recipients = mail;

            StringBuilder  sb = new StringBuilder();
            foreach (var produit in list)
            {
                string str = "<br/> <b>" + produit.productname + "</b> ("+produit.fileUrl + ")";
                sb = sb.AppendLine(str);
            }

            ev.LogEvent("I", DateTime.Now, "Template ", sb.ToString());

            string templatetext =
                    template.TemplateText.Replace("#Product", sb.ToString());
                email.Body = resolver.ResolveMacros(templatetext);
                resolver.EncodeResolvedValues = false;
                email.PlainTextBody = resolver.ResolveMacros(template.TemplatePlainText);
                email.Subject = resolver.ResolveMacros(emailSubject);
                email.CcRecipients = template.TemplateCc;
                email.BccRecipients = template.TemplateBcc;

                try
                {
                    MetaFileInfoProvider.ResolveMetaFileImages(email, template.TemplateID, EmailObjectType.EMAILTEMPLATE,
                        MetaFileInfoProvider.OBJECT_CATEGORY_TEMPLATE);
                    // Send the e-mail immediately
                    EmailSender.SendEmail(currentSite.SiteName, email, true);
                }
                catch (Exception ex)
                {
                    ev.LogEvent("E", DateTime.Now, "SendMail ", ex.Message);
                }
            }
        } 
    }


public class ProductBookModel
{
    public string filename { get; set; }
    public string fileUrl { get; set; }
    public string productname { get; set; }
}