using System;

using CMS.Core;
using CMS.FormEngine;
using CMS.Helpers;
using CMS.Base;
using CMS.UIControls;
using CMS.PortalEngine;

[Title("Development-FormUserControl_List.Title")]
[Action(0, "Development-FormUserControl_List.New", "New.aspx")]
[UIElement(ModuleName.BIZFORM, "Development.FormControls")]
public partial class CMSModules_FormControls_Pages_Development_List : GlobalAdminPage
{
    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        grdList.OnAction += grdList_OnAction;
        grdList.OnExternalDataBound += grdList_OnExternalDataBound;
    }

    #endregion


    #region "Unigrid events"

    protected object grdList_OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        if (sourceName.ToLowerCSafe() == "controltype")
        {
            if ((parameter != null) && (parameter != DBNull.Value))
            {
                return GetString("formcontrolstype." + FormUserControlInfoProvider.GetTypeEnum(ValidationHelper.GetInteger(parameter, 0)));
            }
        }

        return null;
    }


    /// <summary>
    /// Handles the UniGrid's OnAction event.
    /// </summary>
    /// <param name="actionName">Name of item (button) that threw event</param>
    /// <param name="actionArgument">ID (value of Primary key) of corresponding data row</param>
    protected void grdList_OnAction(string actionName, object actionArgument)
    {
        switch (actionName)
        {
            case "edit":
                string url = UIContextHelper.GetElementUrl("CMS.Form", "Edit", false);
                url = URLHelper.AddParameterToUrl(url, "objectid", ValidationHelper.GetInteger(actionArgument, 0).ToString());
                URLHelper.Redirect(url);
                break;

            case "delete":
                FormUserControlInfoProvider.DeleteFormUserControlInfo(ValidationHelper.GetInteger(actionArgument, 0));
                break;
        }
    }

    #endregion
}