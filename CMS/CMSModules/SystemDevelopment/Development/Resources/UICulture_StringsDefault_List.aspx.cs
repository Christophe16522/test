using System;
using System.Data;

using CMS.Base;
using CMS.Core;
using CMS.ExtendedControls.ActionsConfig;
using CMS.Helpers;
using CMS.Localization;
using CMS.UIControls;

// Title
[Title("Development.SysDev.Resources")]
[UIElement(ModuleName.CMS, "Development.Resources")]
public partial class CMSModules_SystemDevelopment_Development_Resources_UICulture_StringsDefault_List : GlobalAdminPage
{
    #region "Private variables"

    protected string cultureCode = null;
    protected FileResourceEditor resourceEditor = null;
    protected int mPassedCultureID = 0;

    #endregion


    #region "Private properties"

    /// <summary>
    /// Culture ID.
    /// </summary>
    private int CultureID
    {
        get;
        set;
    }


    /// <summary>
    /// Holds information oc Culture ID passed as the query string parameter.
    /// </summary>
    private int PassedCultureID
    {
        get
        {
            if (mPassedCultureID == 0)
            {
                mPassedCultureID = QueryHelper.GetInteger("cultureid", 0);
            }

            return mPassedCultureID;
        }
        set
        {
            mPassedCultureID = value;
        }
    }

    #endregion


    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        // Get available cultures
        if (!RequestHelper.IsPostBack())
        {
            GetAvailableCultures();

            // Pre-select the appropriate UI culture
            if (PassedCultureID > 0)
            {
                ddlAvailableCultures.Items.FindByValue(PassedCultureID.ToString()).Selected = true;
            }
        }

        // Initialize the property keeping information on selected culture
        CultureID = ValidationHelper.GetInteger(ddlAvailableCultures.SelectedItem.Value, 0);

        // Init new header action
        HeaderAction action = new HeaderAction()
        {
            Text = GetString("culture.newstring"),
            RedirectUrl = "~/CMSModules/SystemDevelopment/Development/Resources/UICulture_StringsDefault_New.aspx?cultureid=" + CultureID,
        };
        CurrentMaster.HeaderActions.ActionsList.Add(action);

        // Get requested culture
        CultureInfo ui = CultureInfoProvider.GetCultureInfo(CultureID);
        if (ui != null)
        {
            cultureCode = ui.CultureCode;

            resourceEditor = new FileResourceEditor(Server.MapPath(FileResourceManager.GetResFilename(cultureCode)), cultureCode);

            UniGridStrings.OnDataReload += UniGridStrings_OnDataReload;
            UniGridStrings.OnAction += UniGridUICultures_OnAction;
        }
        else
        {
            lblInfo.Visible = true;
        }
    }


    /// <summary>
    /// Handles UniGridStrings OnDataReload event.
    /// </summary>
    private DataSet UniGridStrings_OnDataReload(string completeWhere, string currentOrder, int currentTopN, string columns, int currentOffset, int currentPageSize, ref int totalRecords)
    {
        if (resourceEditor != null)
        {
            DataSet src = resourceEditor.GetResourceDataSet(cultureCode);

            if (!DataHelper.DataSourceIsEmpty(src))
            {
                UniGridStrings.Visible = true;

                if (!String.IsNullOrEmpty(completeWhere))
                {
                    // Remove all N (at the beginning of expression) from where condition (e.g. "column LIKE N'word'" => "column LIKE 'word'")
                    bool inString = false;
                    char prev = ' ';
                    for (int i = 0; i < completeWhere.Length; i++)
                    {
                        if (completeWhere[i] == '\'')
                        {
                            if (!inString && (prev == 'N'))
                            {
                                completeWhere = completeWhere.Remove(i - 1, 1);
                                completeWhere = completeWhere.Insert(i - 1, " ");
                            }
                            inString = !inString;
                        }
                        prev = completeWhere[i];
                    }
                }

                DataSet dst = src.Clone();
                foreach (DataRow dr in src.Tables[0].Select(completeWhere))
                {
                    dst.Tables[0].Rows.Add(dr.ItemArray);
                }

                totalRecords = DataHelper.GetItemsCount(dst);
                return dst;
            }
            else
            {
                lblInfo.Visible = true;
                UniGridStrings.Visible = false;
            }
        }
        totalRecords = -1;
        return null;
    }


    /// <summary>
    /// Handles the UniGrid's OnAction event.
    /// </summary>
    /// <param name="actionName">Name of item (button) that threw event</param>
    /// <param name="actionArgument">ID (value of Primary key) of corresponding data row</param>
    protected void UniGridUICultures_OnAction(string actionName, object actionArgument)
    {
        if (actionName.EqualsCSafe("edit", true))
        {
            URLHelper.Redirect("UICulture_StringsDefault_Edit.aspx?stringCodeName=" + actionArgument + "&cultureID=" + CultureID);
        }
        else if (actionName.EqualsCSafe("delete", true) && (resourceEditor != null))
        {
            // Delete string from global resource helper
            FileResourceManager frm = LocalizationHelper.GetFileManager(cultureCode);
            if (frm != null)
            {
                frm.DeleteString(actionArgument.ToString());
            }

            try
            {
                // Delete string from resource file
                resourceEditor.DeleteResourceString(actionArgument.ToString(), cultureCode);
            }
            catch (Exception ex)
            {
                ShowError(GetString("general.saveerror"), ex.Message, null);
            }
        }
    }

    #endregion


    #region "Private methods"

    /// <summary>
    /// Gets all the UI cultures and fill them in the drop-down list.
    /// </summary>
    private void GetAvailableCultures()
    {
        // Get available UI cultures from the system
        DataSet uiCultures = CultureInfoProvider.GetUICultures();
        if (!DataHelper.DataSourceIsEmpty(uiCultures))
        {
            ddlAvailableCultures.DataSource = uiCultures;
            ddlAvailableCultures.DataTextField = "CultureName";
            ddlAvailableCultures.DataValueField = "CultureID";
            ddlAvailableCultures.DataBind();
        }
        else
        {
            pnlCultures.Visible = false;
        }
    }

    #endregion
}