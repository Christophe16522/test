using System;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.FormControls;
using CMS.Helpers;
using CMS.Base;
using CMS.ExtendedControls;
using CMS.PortalEngine;

/// <summary>
/// This form control is used to edit webpart container code.
/// </summary>
public partial class CMSModules_PortalEngine_Controls_WebContainers_WebpartContainerCode : FormEngineUserControl
{
    private const string WP_CHAR = "□";


    #region "Properties"

    /// <summary>
    /// Gets or sets the container value
    /// </summary>
    public override object Value
    {
        get
        {
            return txtContainerText.Text;
        }
        set
        {

        }
    }


    /// <summary>
    /// Returns ExtendedArea object for code editing.
    /// </summary>
    public ExtendedTextArea Editor
    {
        get
        {
            return txtContainerText;
        }
    }


    /// <summary>
    /// Enables or disables the control
    /// </summary>
    public override bool Enabled
    {
        get
        {
            return !txtContainerText.ReadOnly;
        }
        set
        {
            txtContainerText.ReadOnly = !value;
        }
    }

    #endregion


    #region "Page events

    public override object[,] GetOtherValues()
    {
        string text = txtContainerText.Text;
        string after = "";

        int wpIndex = text.IndexOf(WP_CHAR);
        if (wpIndex >= 0)
        {
            after = text.Substring(wpIndex + 1);
            text = text.Substring(0, wpIndex).Replace(WP_CHAR, "");
        }

        object[,] values = new object[2, 2];
        values[0, 0] = "ContainerTextBefore";
        values[0, 1] = text;
        values[1, 0] = "ContainerTextAfter";
        values[1, 1] = after;
        return values;
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        if (!RequestHelper.IsPostBack())
        {
            txtContainerText.Text = ValidationHelper.GetString(this.Form.GetFieldValue("ContainerTextBefore"), "") + WP_CHAR + ValidationHelper.GetString(this.Form.GetFieldValue("ContainerTextAfter"), "");
        }
    }


    #endregion
}