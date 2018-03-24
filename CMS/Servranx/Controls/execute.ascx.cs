using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CMS.DataEngine;
using System.Data;
using CMS.SettingsProvider;

public partial class Servranx_Controls_execute : System.Web.UI.UserControl
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }
    protected void Button1_Click(object sender, EventArgs e)
    {

        string Query = "update [servranx].[dbo].[CMS_Class] set [ClassShowAsSystemTable] = 1 where ClassName = 'ecommerce.address'";
        QueryDataParameters parameters = new QueryDataParameters();
        ConnectionHelper.ExecuteQuery(Query, parameters, QueryTypeEnum.SQLQuery, true);
       
    }
}