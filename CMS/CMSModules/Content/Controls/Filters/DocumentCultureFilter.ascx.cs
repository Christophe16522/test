using System;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.DataEngine;
using CMS.FormControls;
using CMS.FormEngine;
using CMS.Helpers;
using CMS.SiteProvider;

public partial class CMSModules_Content_Controls_Filters_DocumentCultureFilter : FormEngineUserControl
{
    #region "Variables"

    private DataSet mSiteCultures = null;
    private string mDefaultSiteCulture;
    private string currentSiteName;

    #endregion


    #region "Properties"

    /// <summary>
    /// Gets or sets value of culture selector.
    /// </summary>
    public override object Value
    {
        get
        {
            return cultureElem.Value;
        }
        set
        {
            cultureElem.Value = value;
        }
    }


    /// <summary>
    /// Default culture of the site.
    /// </summary>
    private string DefaultSiteCulture
    {
        get
        {
            return mDefaultSiteCulture ?? (mDefaultSiteCulture = CultureHelper.GetDefaultCultureCode(currentSiteName));
        }
    }


    /// <summary>
    /// Site cultures.
    /// </summary>
    private DataSet SiteCultures
    {
        get
        {
            if (mSiteCultures == null)
            {
                mSiteCultures = CultureSiteInfoProvider.GetSiteCultures(currentSiteName).Copy();
                if (!DataHelper.DataSourceIsEmpty(mSiteCultures))
                {
                    DataTable cultureTable = mSiteCultures.Tables[0];
                    DataRow[] defaultCultureRow = cultureTable.Select("CultureCode='" + DefaultSiteCulture + "'");

                    // Ensure default culture to be first
                    DataRow dr = cultureTable.NewRow();
                    if (defaultCultureRow.Length > 0)
                    {
                        dr.ItemArray = defaultCultureRow[0].ItemArray;
                        cultureTable.Rows.InsertAt(dr, 0);
                        cultureTable.Rows.Remove(defaultCultureRow[0]);
                    }
                }
            }
            return mSiteCultures;
        }
    }
    
    #endregion


    #region "Page events"

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        currentSiteName = SiteContext.CurrentSiteName;
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        // Fill drop down lists
        InitDropdownLists();
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Initializes drop down lists.
    /// </summary>
    private void InitDropdownLists()
    {
        // Init cultures
        cultureElem.AdditionalDropDownCSSClass = "FilterSelectorDropDown";
        cultureElem.AllowDefault = false;
        cultureElem.UpdatePanel.RenderMode = UpdatePanelRenderMode.Inline;
        cultureElem.UniSelector.SpecialFields.Add(new SpecialField() { Text = GetString("transman.anyculture"), Value = "##ANY##" });
        cultureElem.UniSelector.SpecialFields.Add(new SpecialField() { Text = GetString("transman.allcultures"), Value = "##ALL##" });
        
        // Init operands
        if (drpLanguage.Items.Count == 0)
        {
            drpLanguage.Items.Add(new ListItem(GetString("transman.translatedto"), "="));
            drpLanguage.Items.Add(new ListItem(GetString("transman.nottranslatedto"), "<>"));
        }
    }


    /// <summary>
    /// Creates where condition according to values selected in filter.
    /// </summary>
    public override string GetWhereCondition()
    {
        string where = string.Empty;

        string val = ValidationHelper.GetString(cultureElem.Value, string.Empty);
        if (val == string.Empty)
        {
            val = "##ANY##";
        }

        if (val != "##ANY##")
        {
            switch (val)
            {
                case "##ALL##":
                    where = SqlHelper.AddWhereCondition(where, "((SELECT COUNT(*) FROM View_CMS_Tree_Joined AS TreeView WHERE TreeView.NodeID = View_CMS_Tree_Joined_Versions.NodeID) " + SqlHelper.GetSafeQueryString(drpLanguage.SelectedValue, false) + " " + SiteCultures.Tables[0].Rows.Count + ")");
                    break;

                default:
                    string oper = (drpLanguage.SelectedValue == "<>") ? "NOT" : "";
                    where = SqlHelper.AddWhereCondition(where, "NodeID " + oper + " IN (SELECT NodeID FROM View_CMS_Tree_Joined AS TreeView WHERE TreeView.NodeID = NodeID AND DocumentCulture = '" + SqlHelper.GetSafeQueryString(val, false) + "')");
                    break;
            }
        }
        else if (drpLanguage.SelectedValue == "<>")
        {
            where = SqlHelper.NO_DATA_WHERE;
        }

        return where;
    }
    
    #endregion
}
