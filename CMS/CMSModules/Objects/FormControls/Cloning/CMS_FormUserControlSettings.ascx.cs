using System;
using System.Collections;

using CMS.FormEngine;
using CMS.Helpers;
using CMS.SiteProvider;
using CMS.Membership;
using CMS.UIControls;

public partial class CMSModules_Objects_FormControls_Cloning_CMS_FormUserControlSettings : CloneSettingsControl
{
    #region "Properties"

    /// <summary>
    /// Gets properties hashtable.
    /// </summary>
    public override Hashtable CustomParameters
    {
        get
        {
            return GetProperties();
        }
    }

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        lblFiles.ToolTip = GetString("clonning.settings.formusercontrol.files.tooltip");
        lblFileName.ToolTip = GetString("clonning.settings.formusercontrol.filename.tooltip");

        if (!RequestHelper.IsPostBack())
        {
            FormUserControlInfo control = InfoToClone as FormUserControlInfo;
            if (control != null)
            {
                if (control.UserControlFileName != "inherited")
                {
                    txtFileName.Text = FileHelper.GetUniqueFileName(control.UserControlFileName);
                }
            }
        }
    }


    /// <summary>
    /// Returns properties hashtable.
    /// </summary>
    private Hashtable GetProperties()
    {
        Hashtable result = new Hashtable();
        result[FormUserControlInfo.OBJECT_TYPE + ".filename"] = txtFileName.Text;
        result[FormUserControlInfo.OBJECT_TYPE + ".files"] = chkFiles.Checked;
        return result;
    }

    #endregion
}