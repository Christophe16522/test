using System;
using System.Collections.Generic;
using System.Data;
using CMS.CMSHelper;
using CMS.Ecommerce;
using CMS.EventLog;
using CMS.GlobalHelper;
using CMS.SettingsProvider;
using CMS.SiteProvider;
using CMS.Membership;
using CMS.DataEngine;
using CMS.Helpers;

public partial class Servranx_Controls_DownloadProduct : System.Web.UI.UserControl
{
  
    public List<ProductModel> List { get; private set; }

    protected void Page_Load(object sender, EventArgs e)
    {

        int currentUi = MembershipContext.AuthenticatedUser.UserID;

        var currentCust = CustomerInfoProvider.GetCustomerInfoByUserID(currentUi);

        if (currentCust != null)
        {
            const string query = "SELECT [OrderID] FROM [COM_Order] WHERE [OrderCustomerID] = @customerid";
            var param = new QueryDataParameters();
            param.Add(new DataParameter("@customerid", currentCust.CustomerID));
            DataSet ds = ConnectionHelper.ExecuteQuery(query, param, QueryTypeEnum.SQLQuery);
            if (!DataHelper.DataSourceIsEmpty(ds))
            {
                var list = new List<ProductModel>();
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    int orderId = ValidationHelper.GetInteger(row["OrderID"], 0);
                    if (orderId > 0)
                    {
						var pdt = GetDownLoadFile(orderId);
						if(pdt!=null){
							list.Add(pdt);
						}
                        
                    }
                }

                List = list;
            }
        }

        
    }

    protected ProductModel GetDownLoadFile(int orderId)
    {
        var oi = OrderInfoProvider.GetOrderInfo(orderId);
        DataSet ds = OrderItemSKUFileInfoProvider.GetOrderItemSKUFiles(orderId);
        if (!DataHelper.DataSourceIsEmpty(ds) && oi.OrderIsPaid)
        {
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
                    var model = new ProductModel()
                    {
                        filename = myFileName,
                        fileUrl = String.Format("<a class='btndownl' href=\"{0}\" target=\"_blank\">{1}</a>", myFileUrl, "Télécharger"),
                        productname = myPName
                    };

                    return model;
                }
            }
        }
        return null;
    }    
}


public class ProductModel
    {
        public string filename { get; set; }
        public string fileUrl { get; set; }
        public string productname { get; set; } 
    }

