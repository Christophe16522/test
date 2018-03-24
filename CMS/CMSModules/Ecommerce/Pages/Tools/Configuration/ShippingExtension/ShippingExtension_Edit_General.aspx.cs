using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.Ecommerce;
using CMS.CMSHelper;
using CMS.GlobalHelper;
using CMS.SiteProvider;
using CMS.UIControls;
using CMS.SettingsProvider;
using CMS.DataEngine;
using CMS.Helpers;

//[Security(Resource = "CMS.Ecommerce", UIElements = "Configuration.ShippingOptions.General")]
public partial class CMSModules_Ecommerce_Pages_Tools_Configuration_ShippingExtension_ShippingExtension_Edit_General : CMSShippingOptionsPage
{
    protected int mShippingExtensionID = 0;
    protected int mEditedSiteId = -1;


    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            mShippingExtensionID = QueryHelper.GetInteger("shippingExtensionID", 0);
            if (mShippingExtensionID > 0)
            {
                GeneralConnection cn = ConnectionHelper.GetConnection();

                string stringQuery = string.Format("SELECT Localcontact,Enabled, ProcessingMode FROM customtable_shippingextension WHERE ShippingOptionId={0}", mShippingExtensionID.ToString());
                DataSet ds = cn.ExecuteQuery(stringQuery, null, QueryTypeEnum.SQLQuery, false);

                if (!DataHelper.DataSourceIsEmpty(ds))
                {
                    txtLocalContact.Text = ValidationHelper.GetString(ds.Tables[0].Rows[0]["Localcontact"], string.Empty);
                    chkShippingExtensionEnabled.Checked = ValidationHelper.GetBoolean(ds.Tables[0].Rows[0]["Enabled"], true);
                }
            }
        }
    }

    /// <summary>
    /// Sets data to database.
    /// </summary>
    protected void btnOK_Click(object sender, EventArgs e)
    {
        mShippingExtensionID = QueryHelper.GetInteger("shippingExtensionID", 0);
        GeneralConnection cn = ConnectionHelper.GetConnection();
        string stringCommand = string.Format("UPDATE customtable_shippingextension SET Localcontact='{0}',Enabled={1}  WHERE ShippingOptionId={2}", txtLocalContact.Text, Convert.ToInt32(chkShippingExtensionEnabled.Checked), mShippingExtensionID.ToString());
        cn.ExecuteNonQuery(stringCommand, null, QueryTypeEnum.SQLQuery, false);
        ShowChangesSaved();
    }

}