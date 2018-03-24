using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using CMS.DataEngine;

public partial class Servranx_Controls_update : System.Web.UI.UserControl
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }
    protected void Button1_Click(object sender, EventArgs e)
    {
        GeneralConnection cn = ConnectionHelper.GetConnection();
        DataSet ds = null;
        ds = cn.ExecuteQuery("custom.Accessoire.test", null);
    }
}