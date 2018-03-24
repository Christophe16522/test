using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.CMSHelper;
using CMS.FormControls;
using CMS.GlobalHelper;
using CMS.LicenseProvider;
using CMS.SettingsProvider;
using CMS.UIControls;
using CMS.WebAnalytics;
using CMS.Helpers;
using CMS.DataEngine;
using CMS.Membership;

public partial class CMSModules_Reporting_WebAnalytics_Analytics_ManageData : CMSToolsModalPage
{
    #region "Variables"

    private const string VISIT_CODE_NAME = "visitors";
    private const string MULTILINGUAL_SUFFIX = ".multilingual";
    private string statCodeName = String.Empty;

    private FormEngineUserControl ucABTests = null;
    private FormEngineUserControl ucMVTests = null;

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        // Check license
        if (DataHelper.GetNotEmpty(URLHelper.GetCurrentDomain(), "") != "")
        {
            LicenseHelper.CheckFeatureAndRedirect(URLHelper.GetCurrentDomain(), FeatureEnum.WebAnalytics);
        }

        // If deletion is in progress
        if (StatisticsInfoProvider.DataDeleterIsRunning)
        {
            timeRefresh.Enabled = true;
            ViewState["DeleterStarted"] = true;
            EnableControls(false);
            ReloadInfoPanel();
        }
        // If deletion has just end - add close script
        else if (ValidationHelper.GetBoolean(ViewState["DeleterStarted"], false))
        {
            ScriptHelper.RegisterStartupScript(this, typeof(string), "CloseScript", ScriptHelper.GetScript("wopener.RefreshPage(); CloseDialog(); "));
        }

        ucABTests = this.LoadUserControl("~/CMSModules/OnlineMarketing/FormControls/SelectABTest.ascx") as FormEngineUserControl;
        ucMVTests = this.LoadUserControl("~/CMSModules/OnlineMarketing/FormControls/SelectMVTest.ascx") as FormEngineUserControl;

        if (ucABTests != null)
        {
            ucABTests.ID = "abTestSelector";
            pnlABTests.Controls.Add(ucABTests);
        }

        if (ucMVTests != null)
        {
            ucMVTests.ID = "mvtSelector";
            pnlMVTests.Controls.Add(ucMVTests);
        }

        // Configure dynamic selectors
        if (!RequestHelper.IsPostBack() && (ucABTests != null) && (ucMVTests != null))
        {
            string[,] fields = new string[2, 2];
            fields[0, 0] = GetString("general.pleaseselect");
            fields[0, 1] = "pleaseselect";

            fields[1, 0] = "(" + GetString("general.all") + ")";
            fields[1, 1] = ValidationHelper.GetString(ucABTests.GetValue("AllRecordValue"), String.Empty);

            ucABTests.SetValue("SpecialFields", fields);
            ucABTests.Value = "pleaseselect";
            ucABTests.SetValue("AllowEmpty", false);
            ucABTests.SetValue("ReturnColumnName", "ABTestName");

            ucMVTests.SetValue("SpecialFields", fields);
            ucMVTests.Value = "pleaseselect";
            ucMVTests.SetValue("AllowEmpty", false);
            ucMVTests.SetValue("ReturnColumnName", "MVTestName");

            usCampaigns.UniSelector.SpecialFields = fields;
            usCampaigns.Value = "pleaseselect";

            ucConversions.UniSelector.SpecialFields = fields;
            ucConversions.Value = "pleaseselect";
        }

        CurrentUserInfo user = MembershipContext.AuthenticatedUser;

        // Check permissions for CMS Desk -> Tools -> Web analytics tab
        if (!user.IsAuthorizedPerUIElement("CMS.Tools", "WebAnalytics"))
        {
            RedirectToCMSDeskUIElementAccessDenied("CMS.Tools", "WebAnalytics");
        }

        // Check 'Read' permission
        if (!user.IsAuthorizedPerResource("CMS.WebAnalytics", "Read"))
        {
            RedirectToCMSDeskAccessDenied("CMS.WebAnalytics", "Read");
        }

        string title = GetString("AnayticsManageData.ManageData");
        Page.Title = title;
        CurrentMaster.Title.TitleText = title;
        CurrentMaster.Title.TitleImage = GetImageUrl("CMSModules/CMS_Reporting/managedata.png");

        // Confirmation message for deleting
        string deleteFromToMessage = ScriptHelper.GetString(GetString("webanal.deletefromtomsg"));
        deleteFromToMessage = deleteFromToMessage.Replace("##FROM##", "' + elemFromStr + '");
        deleteFromToMessage = deleteFromToMessage.Replace("##TO##", "' + elemToStr + '");

        string script =
            " var elemTo = document.getElementById('" + pickerTo.ClientID + "_txtDateTime'); " +
            " var elemFrom = document.getElementById('" + pickerFrom.ClientID + "_txtDateTime'); " +
            " var elemToStr = " + ScriptHelper.GetString(GetString("webanal.now")) + "; " +
            " var elemFromStr = " + ScriptHelper.GetString(GetString("webanal.beginning")) + "; " +
            " var deleteAll = 1; " +
            " if (elemTo.value != '') { deleteAll = 0; elemToStr = elemTo.value; }; " +
            " if (elemFrom.value != '') { deleteAll = 0; elemFromStr = elemFrom.value; }; " +
            " if (deleteAll == 1) { return confirm(" + ScriptHelper.GetString(GetString("webanal.deleteall")) + "); } " +
            " else { return confirm(" + deleteFromToMessage + "); }; ";
        btnDelete.OnClientClick = script + ";  return false;";

        statCodeName = QueryHelper.GetString("statCodeName", String.Empty);

        switch (statCodeName)
        {
            case "abtest":
                pnlABTestSelector.Visible = true;
                break;

            case "mvtest":
                pnlMVTSelector.Visible = true;
                break;

            case "campaigns":
                pnlCampaigns.Visible = true;
                break;

            case "conversion":
                pnlConversions.Visible = true;
                break;
        }
    }


    /// <summary>
    /// Enable/disable controls
    /// </summary>
    /// <param name="enabled">If true, controls are enabled</param>
    private void EnableControls(bool enabled)
    {
        if (ucABTests != null)
        {
            ucABTests.Enabled = enabled;
        }

        if (ucMVTests != null)
        {
            ucMVTests.Enabled = enabled;
        }

        usCampaigns.UniSelector.Enabled = enabled;
        pickerFrom.Enabled = enabled;
        pickerTo.Enabled = enabled;
    }


    /// <summary>
    /// Displays generator info panel
    /// </summary>
    private void ReloadInfoPanel()
    {
        if (StatisticsInfoProvider.DataDeleterIsRunning)
        {
            ltrProgress.Text = "<span class=\"InfoLabel\"><table><tr><td>" + GetString("analyt.settings.deleterinprogress") + "</td><td><img style=\"width:12px;height:12px;\" src=\"" + UIHelper.GetImageUrl(Page, "Design/Preloaders/preload16.gif") + "\" alt=\"reload\" /></td></tr></table></span>";
        }
    }


    public void btnDelete_Click(object sender, EventArgs e)
    {
        // Check 'ManageData' permission
        if (!MembershipContext.AuthenticatedUser.IsAuthorizedPerResource("CMS.WebAnalytics", "ManageData"))
        {
            RedirectToCMSDeskAccessDenied("CMS.WebAnalytics", "ManageData");
        }

        if (statCodeName == String.Empty)
        {
            return;
        }

        DateTime fromDate = pickerFrom.SelectedDateTime;
        DateTime toDate = pickerTo.SelectedDateTime;

        if (!pickerFrom.IsValidRange() || !pickerTo.IsValidRange())
        {
            ShowError(GetString("general.errorinvaliddatetimerange"));
            return;
        }

        if ((fromDate > toDate) && (toDate != DateTimeHelper.ZERO_TIME))
        {
            ShowError(GetString("analt.invalidinterval"));
            return;
        }

        String where = String.Empty;

        // Manage A/B test selector
        if ((statCodeName == "abtest") && (ucABTests != null))
        {
            string abTest = ValidationHelper.GetString(ucABTests.Value, String.Empty);
            if ((abTest == String.Empty) || (abTest == "pleaseselect"))
            {
                ShowError(GetString("abtest.pleaseselect"));
                return;
            }

            String codeName = (abTest == ValidationHelper.GetString(ucABTests.GetValue("AllRecordValue"), String.Empty)) ? "'abconversion;%'" : "'abconversion;" + SecurityHelper.GetSafeQueryString(abTest) + ";%'";
            where = "StatisticsCode LIKE " + codeName;
        }

        // Manage MVT test selector
        if ((statCodeName == "mvtest") && (ucMVTests != null))
        {
            string mvTest = ValidationHelper.GetString(ucMVTests.Value, String.Empty);
            if ((mvTest == String.Empty) || (mvTest == "pleaseselect"))
            {
                ShowError(GetString("mvtest.pleaseselect"));
                return;
            }

            String codeName = (mvTest == ValidationHelper.GetString(ucMVTests.GetValue("AllRecordValue"), String.Empty)) ? "'mvtconversion;%'" : "'mvtconversion;" + SecurityHelper.GetSafeQueryString(mvTest) + ";%'";
            where = "StatisticsCode LIKE " + codeName;
        }

        // Manage campaigns
        if (statCodeName == "campaigns")
        {
            string campaign = ValidationHelper.GetString(usCampaigns.Value, String.Empty);
            if ((campaign == String.Empty) || (campaign == "pleaseselect"))
            {
                ShowError(GetString("campaigns.pleaseselect"));
                return;
            }

            if (campaign == usCampaigns.AllRecordValue)
            {
                where = "(StatisticsCode='campaign' OR StatisticsCode LIKE 'campconversion;%')";
            }
            else
            {
                where = " ((StatisticsCode='campaign' AND StatisticsObjectName ='" + SecurityHelper.GetSafeQueryString(campaign, false) + "') OR StatisticsCode LIKE 'campconversion;" + SecurityHelper.GetSafeQueryString(campaign) + "')";
            }
        }

        if ((statCodeName == "conversion") || statCodeName.StartsWithCSafe("singleconversion", true))
        {
            String defaultWhere = "(StatisticsCode='conversion' OR StatisticsCode LIKE 'campconversion;%' OR StatisticsCode LIKE 'abconversion;%' OR StatisticsCode LIKE 'mvtconversion;%')";
            if (!statCodeName.StartsWithCSafe("singleconversion", true))
            {
                string conversion = ValidationHelper.GetString(ucConversions.Value, String.Empty);
                if ((conversion == String.Empty) || (conversion == "pleaseselect"))
                {
                    ShowError(GetString("conversions.pleaseselect"));
                    return;
                }

                if (conversion == usCampaigns.AllRecordValue)
                {
                    where = defaultWhere;
                }
                else
                {
                    String saveConv = SecurityHelper.GetSafeQueryString(conversion, false);
                    where = String.Format("((StatisticsObjectName = '{0}') AND {1})", saveConv, defaultWhere);
                }
            }
            else
            {
                string[] arr = statCodeName.Split(';');
                if (arr.Length == 2)
                {
                    String saveConv = SecurityHelper.GetSafeQueryString(arr[1], false);
                    where = String.Format("((StatisticsObjectName = '{0}') AND {1})", saveConv, defaultWhere);
                }
            }
        }

        // Delete one campaign (set from url)
        if (statCodeName.StartsWithCSafe("singlecampaign", true))
        {
            string[] arr = statCodeName.Split(';');
            if (arr.Length == 2)
            {
                String campaign = arr[1];
                where = "(StatisticsCode='campaign' AND StatisticsObjectName ='" + SecurityHelper.GetSafeQueryString(campaign, false) + "') OR StatisticsCode LIKE 'campconversion;" + SecurityHelper.GetSafeQueryString(campaign) + "'";
            }
        }

        // Ingore multilingual suffix (multilingual stats use the same data as "base" stats)
        if (statCodeName.ToLowerCSafe().EndsWithCSafe(MULTILINGUAL_SUFFIX))
        {
            statCodeName = statCodeName.Remove(statCodeName.Length - MULTILINGUAL_SUFFIX.Length);
        }

        // Add where condition based on stat code name
        if (where == String.Empty)
        {
            where = "StatisticsCode LIKE '" + SecurityHelper.GetSafeQueryString(statCodeName) + "'";
        }

        // In case of any error - (this page don't allow deleting all statistics)
        if (where == String.Empty)
        {
            return;
        }

        // Stats for visitors needs special manipulation (it consist of two types
        // of statistics with different code names - new visitor and returning visitor)
        if (statCodeName.ToLowerCSafe() != HitLogProvider.VISITORS_FIRST)
        {
            StatisticsInfoProvider.RemoveAnalyticsDataAsync(fromDate, toDate, CMSContext.CurrentSiteID, where);
        }
        else
        {
            where = "(StatisticsCode = '" + HitLogProvider.VISITORS_FIRST + "' OR StatisticsCode ='" + HitLogProvider.VISITORS_RETURNING + "')";
            StatisticsInfoProvider.RemoveAnalyticsDataAsync(fromDate, toDate, CMSContext.CurrentSiteID, where);
        }

        // Manage async delete info
        timeRefresh.Enabled = true;
        EnableControls(false);
        ReloadInfoPanel();
        ViewState.Add("DeleterStarted", 1);
    }

    #endregion
}