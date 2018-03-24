using System;
using System.Collections;

using CMS.Helpers;
using CMS.Base;
using CMS.SiteProvider;
using CMS.Membership;
using CMS.UIControls;

public partial class CMSModules_Objects_FormControls_Cloning_CMS_CategorySettings : CloneSettingsControl
{
    #region "Properties"

    /// <summary>
    /// Gets properties hashtable
    /// </summary>
    public override Hashtable CustomParameters
    {
        get
        {
            return GetProperties();
        }
    }


    /// <summary>
    /// Gets script used for closing clone dialog.
    /// </summary>
    public override string CloseScript
    {
        get
        {
            return @"if ((wopener != null) && wopener.Refresh) { wopener.Refresh({0}," + drpCategories.CategoryID + "); } CloseDialog();";
        }
    }

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        drpCategories.ExcludeCategoryID = InfoToClone.Generalized.ObjectID;
        if (!RequestHelper.IsPostBack())
        {
            int userId = InfoToClone.GetIntegerValue("CategoryUserID", 0);
            drpCategories.CategoryID = InfoToClone.GetIntegerValue("CategoryParentID", 0);
            if (userId > 0)
            {
                drpCategories.UserID = userId;
            }
            drpCategories.DisableSiteCategories = InfoToClone.Generalized.ObjectSiteID == 0;
        }
    }


    /// <summary>
    /// Returns properties hashtable.
    /// </summary>
    private Hashtable GetProperties()
    {
        Hashtable result = new Hashtable();
        result[InfoToClone.ObjectType + ".parentcategory"] = drpCategories.CategoryID;
        result[InfoToClone.ObjectType + ".subcategories"] = chkSubcategories.Checked;
        return result;
    }

    #endregion
}