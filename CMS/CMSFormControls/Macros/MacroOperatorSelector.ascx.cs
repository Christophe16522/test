using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.FormControls;
using CMS.Helpers;
using CMS.SiteProvider;
using CMS.Membership;
using CMS.UIControls;

public partial class CMSFormControls_Macros_MacroOperatorSelector : FormEngineUserControl
{
    #region "Properties"

    /// <summary>
    /// Returns value of the 
    /// </summary>
    public override object Value
    {
        get
        {
            return drpOperator.SelectedValue;
        }
        set
        {
            ReloadData();

            drpOperator.ClearSelection();

            ListItem selected = drpOperator.Items.FindByValue(ValidationHelper.GetString(value, ""));
            if (selected != null)
            {
                selected.Selected = true;
            }
        }
    }


    /// <summary>
    /// Returns ClientID of the dropdown.
    /// </summary>
    public override string ClientID
    {
        get
        {
            return base.ClientID;
        }
    }

    #endregion


    /// <summary>
    /// Page load.
    /// </summary>
    protected void Page_Load(object sender, EventArgs e)
    {
        ReloadData();
    }


    /// <summary>
    /// Puts items into the drop down list.
    /// </summary>
    private void ReloadData()
    {
        if (drpOperator.Items.Count == 0)
        {
            drpOperator.Items.Add(new ListItem(GetString("filter.equals"), "=="));
            drpOperator.Items.Add(new ListItem(GetString("filter.notequals"), "!="));
            drpOperator.Items.Add(new ListItem(GetString("filter.greaterthan"), ">"));
            drpOperator.Items.Add(new ListItem(GetString("filter.lessthan"), "<"));
            drpOperator.Items.Add(new ListItem(GetString("filter.greaterorequal"), ">="));
            drpOperator.Items.Add(new ListItem(GetString("filter.lessorequal"), "<="));
        }
    }


    /// <summary>
    /// Returns display name displayed in the MacroRuleEditor control parameters designer.
    /// </summary>
    public override object[,] GetOtherValues()
    {
        // Set properties names
        object[,] values = new object[1, 2];
        values[0, 0] = "DisplayName";
        values[0, 1] = (drpOperator.SelectedItem != null ? drpOperator.SelectedItem.Text : "");
        return values;
    }
}