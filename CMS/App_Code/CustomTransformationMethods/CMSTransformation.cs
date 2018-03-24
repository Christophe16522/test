using CMS.DocumentEngine;
using System.Data;
using CMS.CMSHelper;
using CMS.SiteProvider;
using CMS.SettingsProvider;
using CMS.DataEngine;
using CMS.GlobalHelper;
using System;
using CMS.Ecommerce;
using CMS.EcommerceProvider;

namespace CMS.Controls
{

    public partial class CMSTransformation
    {	
		public string CustomTrimText(object txtValue, int leftChars)
        {
            // Checks that text is not null.
            if (txtValue == null | txtValue == DBNull.Value)
            {
                return "";
            }
            else
            {
                string txt = (string)txtValue;
 
                // Returns a substring if the text is longer than specified.
                if (txt.Length <= leftChars)
                {
                    return txt;
                }
                else
                {
                    return txt.Substring(0, leftChars) + " ...";
                }
            }
        }
		public string GetProductNodeAliasPath(object skuid)
		{        
			SKUInfo sku = SKUInfoProvider.GetSKUInfo((int)skuid);

			if (sku != null)
			{
				GeneralConnection cn = ConnectionHelper.GetConnection();
				string stringQuery = string.Format("select NodeAliasPath from View_CONTENT_Product_Joined where NodeSKUID = " + sku.SKUID);
                DataSet ds = cn.ExecuteQuery(stringQuery, null, CMS.DataEngine.QueryTypeEnum.SQLQuery, false);
				string NodeAliasPath = Convert.ToString(ds.Tables[0].Rows[0]["NodeAliasPath"]);
				return "~/" + NodeAliasPath + ".aspx";
			}
			return String.Empty;
		}
    }
}