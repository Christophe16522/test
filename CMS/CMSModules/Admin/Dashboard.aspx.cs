using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

using CMS.DataEngine;
using CMS.Helpers;
using CMS.LicenseProvider;
using CMS.Modules;
using CMS.PortalEngine;
using CMS.UIControls;

public partial class CMSModules_Admin_Dashboard : CMSDeskPage
{
    #region "Variables"

    private List<int> mUserRoles = null;

    #endregion


    #region "Properties"

    /// <summary>
    ///  Gets list of user role IDs.
    /// </summary>
    private List<int> UserRoles
    {
        get
        {
            if (mUserRoles == null)
            {
                string roleIDs = CurrentUser.GetRoleIdList(true, true);
                mUserRoles = roleIDs.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).Select(s => Convert.ToInt32(s)).ToList<int>();
                mUserRoles.Sort();
            }

            return mUserRoles;
        }
    }

    #endregion


    #region "Page events"

    /// <summary>
    /// Handles the Load event of the Page control.
    /// </summary>
    protected void Page_Load(object sender, EventArgs e)
    {
        // Provide application name for client to refresh breadcrumbs correctly
        RequestContext.ClientApplication.Add("applicationName", ResHelper.GetString("Dashboard"));

        // Check license for the current domain
        LicenseValidationEnum licenseCheck = LicenseHelper.ValidateLicenseForDomain(RequestContext.CurrentDomain);
        if (licenseCheck != LicenseValidationEnum.Valid)
        {
            URLHelper.ResponseRedirect(URLHelper.ResolveUrl("~/CMSMessages/invalidlicensekey.aspx"));
        }

        SetupDashboard();
    }

    #endregion


    #region "Private methods"

    /// <summary>
    /// Setups the dashboard.
    /// </summary>
    private void SetupDashboard()
    {
        // Get the list of applications
        var applications = GetApplications();


        // Display full or empty dashboard design
        if ((applications == null)|| DataHelper.DataSourceIsEmpty(applications))
        {
            // Show empty dashboard
            plcDashboard.Visible = false;
            plcEmpty.Visible = true;
        }
        else
        {
            // Data bind
            repApps.DataSource = applications.Tables[0].DefaultView;
            repApps.DataBind();

            // Display dashboard applications
            plcDashboard.Visible = true;
            plcEmpty.Visible = false;
        }
    }


    /// <summary>
    /// Gets the applications which should be displayed on the dashboard.
    /// </summary>
    private DataSet GetApplications()
    {
        var where = new WhereCondition().WhereIn("ElementID", new IDQuery(RoleApplicationInfo.OBJECT_TYPE, "ElementID").WhereIn("RoleID", UserRoles));
        var apps = ApplicationHelper.LoadApplications(where);
        return ApplicationHelper.FilterApplications(apps, CurrentUser, false);
    }


    /// <summary>
    /// Gets application path for dashboard tiles.
    /// </summary>
    /// <param name="resourceID">Resource ID</param>
    /// <param name="elementName">Element name</param>
    protected string GetApplicationPath(object resourceID, object elementName)
    {
        string path = UIContextHelper.GetApplicationUrl(UIContextHelper.GetResourceName(Convert.ToInt32(resourceID)), elementName.ToString());
        return URLHelper.ResolveUrl(path);
    }

    #endregion
}