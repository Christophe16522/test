using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.CMSHelper;
using CMS.GlobalHelper;
using CMS.SiteProvider;
using CMS.DocumentEngine;
using CMS.UIControls;
using CMS.Helpers;
using CMS.Membership;

public partial class CMSPages_GetProduct : CMSPage
{
    #region "Variables"

    protected Guid skuGuid = Guid.Empty;
    protected int productId = 0;
    protected SiteInfo currentSite = null;
    protected string url = "";

    #endregion


    protected void Page_Load(object sender, EventArgs e)
    {
        // Initialization
        productId = QueryHelper.GetInteger("productId", 0);
        skuGuid = QueryHelper.GetGuid("skuguid", Guid.Empty);
        currentSite = SiteContext.CurrentSite;

        string where = null;
        if (productId > 0)
        {
            where = "NodeSKUID = " + productId;
        }
        else if (skuGuid != Guid.Empty)
        {
            where = "SKUGUID = '" + skuGuid.ToString() + "'";
        }

        if ((where != null) && (currentSite != null))
        {
            TreeProvider tree = new TreeProvider(MembershipContext.AuthenticatedUser);
            DataSet ds = tree.SelectNodes(currentSite.SiteName, "/%", TreeProvider.ALL_CULTURES, true, "", where);
            if (!DataHelper.DataSourceIsEmpty(ds))
            {
                // Ger specified product url 
                url = DocumentURLProvider.GetUrl(Convert.ToString(ds.Tables[0].Rows[0]["NodeAliasPath"]));
            }
        }

        if ((url != "") && (currentSite != null))
        {
            // Redirect to specified product 
            URLHelper.Redirect(url, true, currentSite.SiteName);
        }
        else
        {
            // Display error message
            lblInfo.Visible = true;
            lblInfo.Text = GetString("GetProduct.NotFound");
        }
    }
}