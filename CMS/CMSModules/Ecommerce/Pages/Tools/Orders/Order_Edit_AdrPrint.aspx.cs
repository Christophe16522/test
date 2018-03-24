using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.CMSHelper;
using CMS.Ecommerce;
using CMS.GlobalHelper;
using CMS.UIControls;
using CMS.SettingsProvider;
using CMS.SiteProvider;
using CMS.Helpers;
using CMS.Globalization;

[Security(Resource = "CMS.Ecommerce", UIElements = "Orders.Invoice")]
public partial class CMSModules_Ecommerce_Pages_Tools_Orders_Order_Edit_AdrPrint : CMSOrdersPage
{
    protected override void OnPreInit(EventArgs e)
    {
        CustomerID = QueryHelper.GetInteger("customerid", 0);
        base.OnPreInit(e);
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        int orderId = QueryHelper.GetInteger("orderid", 0);
        OrderInfo order = OrderInfoProvider.GetOrderInfo(orderId);
        if (order != null)
        {
            //lblInvoice.Text = URLHelper.MakeLinksAbsolute(order.OrderInvoice);
            lblInvoice.Text = GetAndBulkUpdateAddresses(order.OrderShippingAddressID,order.OrderCustomerID,order.OrderID);
        }
    }
    private string GetAndBulkUpdateAddresses(int aid,int cid,int oid)
    {
        // Prepare the parameters
        string where = "AddressID LIKE '"+aid+"'";

        // Get the data
        DataSet addresses = AddressInfoProvider.GetAddresses(where, null);
        if (!DataHelper.DataSourceIsEmpty(addresses))
        {
            // Loop through the individual items
            foreach (DataRow addressDr in addresses.Tables[0].Rows)
            {
                // Create object from DataRow
                AddressInfo modifyAddress = new AddressInfo(addressDr);

                // Update the properties
                modifyAddress.AddressName = modifyAddress.AddressName.ToUpper();
                string a2=modifyAddress.AddressLine2;
                if(!string.IsNullOrEmpty(a2))a2=a2+"<br/>";
                int pid = modifyAddress.AddressCountryID;
                string pays = string.Empty;
                string wherec = "CountryID LIKE'" + pid + "'";
                DataSet country = CMS.Globalization.CountryInfoProvider.GetCountries(wherec, null);
                string customerv = string.Empty;
                string wherecu = "CustomerID LIKE '"+cid+"'";
                string whereo = "OrderID LIKE '" + oid + "'";
                string ordern = string.Empty;
                string invoice = string.Empty;
                CustomerInfo customer = null;
                
                //get order info
                DataSet orders = OrderInfoProvider.GetOrders(whereo, null);
                if (!DataHelper.DataSourceIsEmpty(orders))
                {
                    // Create object from DataRow
                    OrderInfo order = new OrderInfo(orders.Tables[0].Rows[0]);

                    // Update the property
                   ordern= "Commande " + order.OrderID.ToString();
                   invoice = order.GetStringValue("facture", string.Empty);
                    if (!string.IsNullOrEmpty(invoice)){

                        invoice = "Facture " + invoice + "<br/>";
                    
                    }

                    
                   
                }


                // Get the customer
                DataSet customers = CustomerInfoProvider.GetCustomers(wherecu, null);
                if (!DataHelper.DataSourceIsEmpty(customers))
                {
                    // Create object from DataRow
                    customer = new CustomerInfo(customers.Tables[0].Rows[0]);
                    customerv = customer.CustomerLastName + " " + customer.CustomerFirstName;
                }
                if (!DataHelper.DataSourceIsEmpty(country))
                {
                    // Loop through the individual items
                    foreach (DataRow countryDr in country.Tables[0].Rows)
                    {
                        CountryInfo ci = new CountryInfo(countryDr);
                    pays=GetString(ci.CountryName);
                    }
                }
                string result = "<br /><table border=\"0\" cellpadding=\"20\" cellspacing=\"0\" width=\"630px\"><tbody><tr><td valign=\"top\"><table border=\"0\" cellpadding=\"0\" cellspacing=\"0\"><tbody><tr><td><table border=\"0\" cellpadding=\"0\" cellspacing=\"0\" width=\"630px\"><tbody><tr><td align=\"left\" colspan=\"2\"><img  src=\"http://www.servranx.com/App_Themes/Servranx/images/logonb.png\" width=\"300\" /><br /><div align=\"left\" style=\"font-size:13px ; border-left:3px solid black; padding-left:8px\"><p>Sprl Servranx<br />23-25, rue Gustave Biot - B-1050 Bruxelles<br />T�l. 00 32 (0)2 649 18 40<br />Fax&nbsp;&nbsp;00 32 (0)2 649 12 10<br />info@servranx.com<br /><strong>www.servranx.com</strong><br />TVA BE 0450 834 519<br />Comptes bancaires :<br />ING 310-0529195-49<br />IBAN BE97 3100 5291 9549&nbsp;<br />BIC : BBRUBEBB<br />&nbsp;</p></div></td><td align=\"left\" style=\"text-align: left;\" width=\"351\"><div align=\"left\" style=\"border:2px solid black; padding:15px;font-weight:bold; font-size:17px;margin-top:50px\"><span style=\"font-size: 14px;text-transform:uppercase\">" + ordern + "<br/>" +invoice+ " ADRESSE DE LIVRAISON<br/><br/><br/>" + customerv + "<br/>" + modifyAddress.GetValue("AddressNumber") + " " + modifyAddress.AddressLine1 + "<br/>" + a2 + modifyAddress.AddressZip + " " + modifyAddress.AddressCity + "<br/>" + pays + "</span></div></td></tr></tbody></table></td></tr></tbody></table></td></tr></tbody></table><br />";

                    

                return result;
                // Update the address
              //  AddressInfoProvider.SetAddressInfo(modifyAddress);
            }

            //return true;
        }

        return string.Empty;
    }
}