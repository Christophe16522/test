using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

using CMS.DataEngine;
using CMS.GlobalHelper;
using CMS.SettingsProvider;
using CMS.Helpers;
/// <summary>
/// Description résumée de CustomBundle
/// </summary>
public class CustomBundle
{
    
    class Bundle
    {
        public int BundleId
        {
            get;
            set;
        }

        public int Quantity
        {
            get;
            set;
        }

        public string Description
        {
            get;
            set;
        }

        public int Total
        {
            get;
            set;
        }


    }
    class BundledItem
    {
        public int BundleID
        {
            get;
            set;
        }

        public int ProductID
        {
            get;
            set;
        }
    }
    private List<Bundle> availableBundles = new List<Bundle>();
    private static List<BundledItem> bundledItems = new List<BundledItem>();
    private static DataSet dsProd;
    
	public CustomBundle()
	{
		//
		// TODO: ajoutez ici la logique du constructeur
		//
	}

    public static string TranslateBundle(string bundledString)
    {
        string result = string.Empty;        
        bundledItems.Clear();
        while (!string.IsNullOrEmpty(bundledString))
        {
            int productId = 0, bundleID = 0, mark = bundledString.IndexOf("-");
            string bundlestring = bundledString.Substring(0, mark);
            string productIdString = bundlestring.Substring(0, bundlestring.IndexOf(","));
            string bundleIdString = bundlestring.Substring(bundlestring.IndexOf(",") + 1);
            bundleID = int.Parse(bundleIdString);
            productId = int.Parse(productIdString);
            var obj = bundledItems.Find(i => i.ProductID == productId);
            if (obj == null)
            {
                bundledItems.Add(new BundledItem { BundleID = bundleID, ProductID = productId });
            }
            else
            {
                obj.BundleID++;
            }
            bundledString = bundledString.Substring(mark + 1);
        }
        initDSProd();
        foreach (BundledItem b in bundledItems)
        {
            string productName = string.Empty;
            var obj = dsProd.Tables[0].Rows.Find(b.ProductID);
            if (obj != null)
            {
                try
                {
                    productName = obj["ProductName"].ToString();
                }
                catch
                {
                    productName = obj["Name"].ToString();
                }
            }

            result =string.Concat(string.Format("- {0} x {1}", b.BundleID.ToString(), productName),"<br/>", result);
        }

        return result;
    }

    private static void initDSProd()
    {
        GeneralConnection cn = ConnectionHelper.GetConnection();
        string stringQuery = "SELECT * FROM CONTENT_Product";
        dsProd = cn.ExecuteQuery(stringQuery, null, QueryTypeEnum.SQLQuery, false);
        if (!DataHelper.DataSourceIsEmpty(dsProd))
        {
            DataColumn[] key = new DataColumn[1];
            DataTable dt = dsProd.Tables[0];
            key[0] = dt.Columns[0];
            dt.PrimaryKey = key;
        }
    }
    
}