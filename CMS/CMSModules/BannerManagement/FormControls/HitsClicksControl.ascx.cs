using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.FormControls;
using CMS.GlobalHelper;
using CMS.Helpers;

public partial class CMSModules_BannerManagement_FormControls_HitsClicksControl : FormEngineUserControl
{
    /// <summary>
    /// Resource string used to mark radio option meaning unlimited number.
    /// </summary>
    public string AllowUnlimitedResourceString { get; set; }


    /// <summary>
    /// Resource string used to mark radio option meaning specific number.
    /// </summary>
    public string AllowSpecificResourceString { get; set; }


    public string AddNumberResourceString { get; set; }


    public string NumberToAddResourceString { get; set; }


    public override object Value
    {
        get
        {
            if (radAllowSpecific.Checked)
            {
                int val;
                if (int.TryParse(txtNumberLeft.Text, out val))
                {
                    return val;
                }

                return null;
            }
            return null;
        }
        set
        {
            if (ValidationHelper.IsInteger(value))
            {
                radAllowSpecific.Checked = true;
                txtNumberLeft.Text = value.ToString();
            }
            else
            {
                // If incorrect format or null, set to unlimited
                radUnlimited.Checked = true;
                txtNumberLeft.Text = "0";
            }
        }
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        string script = @"
function addNumberHitsClicks(clientID, numberToAdd)
{
    var control = $j('#' + clientID); 
    control.val(parseInt(control.val()) + parseInt(numberToAdd));
}
";

        ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "addNumberHitsClicks", script, true);
        ScriptHelper.RegisterJQuery(Page);
        ScriptHelper.RegisterDialogScript(Page);

        radUnlimited.ResourceString = AllowUnlimitedResourceString;
        radAllowSpecific.ResourceString = AllowSpecificResourceString;

        string modalUrl = string.Format("{0}?numbercontrolid={1}&addnumberres={2}&numbertoaddres={3}", URLHelper.ResolveUrl("~/CMSModules/BannerManagement/Tools/Banner/HitClickAddModal.aspx"), txtNumberLeft.ClientID, AddNumberResourceString, NumberToAddResourceString);

        btnAdd.OnClientClick = string.Format("modalDialog('{0}', 'AddNumber', 400, 140);", modalUrl);
    }
    

    protected void Page_PreRender(object sender, EventArgs e)
    {
        pnlSpecific.Enabled = radAllowSpecific.Checked;
    }
}
