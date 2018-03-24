using System;
using System.Data;
using System.Web.UI.WebControls;

using CMS.Core;
using CMS.Helpers;
using CMS.OnlineMarketing;
using CMS.Base;
using CMS.PortalEngine;
using CMS.UIControls;
using CMS.WorkflowEngine;
using CMS.SiteProvider;
using CMS.DataEngine;
using CMS.ExtendedControls;

public partial class CMSModules_ContactManagement_Controls_UI_Automation_PendingContacts : CMSAdminEditControl
{
    #region "Public properties"

    /// <summary>
    /// Gets or sets current site id.
    /// </summary>
    public int SiteID
    {
        get;
        set;
    }

    /// <summary>
    /// Indicates if control is used as widget.
    /// </summary>
    public bool IsWidget
    {
        get;
        set;
    }


    /// <summary>
    /// Gets or sets default page size of list control.
    /// </summary>
    public int PageSize
    {
        get;
        set;
    }

    #endregion


    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!StopProcessing)
        {
            SetupControl();
        }
        else
        {
            listElem.StopProcessing = true;
            listElem.Visible = false;
        }
    }


    /// <summary>
    /// Setup control.
    /// </summary>
    private void SetupControl()
    {
        if (!URLHelper.IsPostback() && (PageSize > 0))
        {
            listElem.Pager.DefaultPageSize = PageSize;
        }

        listElem.ZeroRowsText = GetString("ma.pendingcontacts.nowaitingcontacts");

        // Hide site column for site records
        if (SiteID != UniSelector.US_ALL_RECORDS)
        {
            // Site column
            listElem.GridColumns.Columns[5].Visible = false;
        }

        listElem.EditActionUrl = "Process_Detail.aspx?stateid={0}&siteid=" + SiteID + (ContactHelper.IsSiteManager ? "&issitemanager=1" : String.Empty);
        listElem.WhereCondition = "StateStatus = " + (int)ProcessStatusEnum.Pending + " ";

        listElem.RememberStateByParam = String.Empty;

        if (SiteID != 0)
        {
            // Prepare site filtering
            string siteCondition = "AND (StateSiteID {0})";

            switch (SiteID)
            {
                case UniSelector.US_GLOBAL_RECORD:
                    siteCondition = String.Format(siteCondition, "IS NULL");
                    break;

                case UniSelector.US_ALL_RECORDS:
                    siteCondition = String.Empty;
                    break;

                default:
                    siteCondition = String.Format(siteCondition, "= " + SiteID);
                    break;
            }

            // Append site condition
            listElem.WhereCondition += siteCondition;
        }

        // Get complete where condition for pending steps
        string where = WorkflowStepInfoProvider.GetAutomationPendingStepsWhereCondition(CurrentUser, SiteInfoProvider.GetSiteName(SiteID));

        // If empty current user has Manage processes permissions and show all processes
        if (!String.IsNullOrEmpty(where))
        {
            // Create query parameters
            QueryDataParameters parameters = new QueryDataParameters();
            parameters.Add("@SiteID", SiteID);
            parameters.Add("@UserID", CurrentUser.UserID);
            parameters.Add("@Now", DateTime.Now);

            listElem.QueryParameters = parameters;

            // Add pending steps where condition
            listElem.WhereCondition = SqlHelper.AddWhereCondition(listElem.WhereCondition, where);
        }

        listElem.OnExternalDataBound += listElem_OnExternalDataBound;

        // Register scripts for contact details dialog
        ScriptHelper.RegisterDialogScript(Page);
        ScriptHelper.RegisterWOpenerScript(Page);
        ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "ViewContactDetails", ScriptHelper.GetScript(
            "function Refresh() { \n " +
            "window.location.href = window.location.href;\n" +
            "}"));

        // If widget register action for view process in dialog
        if (IsWidget)
        {
            ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "ViewPendingContactProcess", ScriptHelper.GetScript(
            "function viewPendingContactProcess(stateId) {" +
            "    modalDialog('" + URLHelper.ResolveUrl("~/CMSModules/ContactManagement/Pages/Tools/PendingContacts/Process_Detail.aspx") + "?dialog=1&stateId=' + stateId, 'ViewPendingContactProcess', '1024', '800');" +
            "}"));
        }
    }


    protected object listElem_OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        CMSGridActionButton btn;
        switch (sourceName.ToLowerCSafe())
        {
            // Set visibility for edit button
            case "edit":
                if (IsWidget)
                {
                    btn = sender as CMSGridActionButton;
                    if (btn != null)
                    {
                        btn.Visible = false;
                    }
                }
                break;

            // Set visibility for dialog edit button
            case "dialogedit":
                btn = sender as CMSGridActionButton;
                if (btn != null)
                {
                    btn.Visible = IsWidget;
                }
                break;

            case "view":
                btn = (CMSGridActionButton)sender;
                // Ensure accountID parameter value;
                var objectID = ValidationHelper.GetInteger(btn.CommandArgument, 0);
                // Contact detail URL
                string contactURL = UIContextHelper.GetElementDialogUrl(ModuleName.ONLINEMARKETING, "EditContact", objectID, "isSiteManager=" + ContactHelper.IsSiteManager);
                // Add modal dialog script to onClick action
                btn.OnClientClick = ScriptHelper.GetModalDialogScript(contactURL, "ContactDetail");
                break;

            // Delete action
            case "delete":
                int siteId = SiteID;

                if (SiteID == UniSelector.US_GLOBAL_AND_SITE_RECORD)
                {
                    DataRowView drv = (parameter as GridViewRow).DataItem as DataRowView;
                    int contactSiteId = ValidationHelper.GetInteger(drv["StateSiteID"], 0);
                    if (contactSiteId > 0)
                    {
                        siteId = contactSiteId;
                    }
                }

                btn = (CMSGridActionButton)sender;
                btn.OnClientClick = "if(!confirm(" + ScriptHelper.GetString(string.Format(ResHelper.GetString("autoMenu.RemoveStateConfirmation"), HTMLHelper.HTMLEncode(TypeHelper.GetNiceObjectTypeName(ContactInfo.OBJECT_TYPE).ToLowerCSafe()))) + ")) { return false; }" + btn.OnClientClick;
                if (!WorkflowStepInfoProvider.CanUserRemoveAutomationProcess(CurrentUser, SiteInfoProvider.GetSiteName(siteId)))
                {
                    if (btn != null)
                    {
                        btn.Enabled = false;
                    }
                }
                break;
        }

        return null;
    }

    #endregion
}
