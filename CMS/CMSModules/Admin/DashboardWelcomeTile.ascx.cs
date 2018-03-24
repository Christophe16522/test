using System;
using System.Linq;
using System.Web.UI;

using CMS.Helpers;
using CMS.Membership;
using CMS.UIControls;

public partial class CMSModules_Admin_DashboardWelcomeTile : CMSUserControl, ICallbackEventHandler
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!MembershipContext.AuthenticatedUser.UserSettings.UserShowIntroductionTile)
        {
            Visible = false;
            return;
        }

        lnkClose.ScreenReaderDescription = GetString("general.close");
        ScriptHelper.RegisterModule(this, "CMS/DashboardWelcomeTile");
        EnsureCloseCallback();
    }


    /// <summary>
    /// Ensures callback script to store state into session when message is hidden
    /// </summary>
    private void EnsureCloseCallback()
    {
        ClientScriptManager cm = Page.ClientScript;
        string cbReference = cm.GetCallbackEventReference(this, "arg", "WDT_ReceiveCloseRequest", "");
        string callbackScript = @"
WDT_CloseTile = function (arg, context) {
    " + cbReference + @";
};
WDT_ReceiveCloseRequest = function () {};
";
        cm.RegisterClientScriptBlock(GetType(), "ReceiveCloseRequest", callbackScript, true);
    }


    #region "Callback handling"

    /// <summary>
    /// Gets callback result
    /// </summary>
    public string GetCallbackResult()
    {
        return null;
    }


    /// <summary>
    /// Handles server call 
    /// </summary>
    /// <param name="eventArgument">Event argument</param>
    public void RaiseCallbackEvent(string eventArgument)
    {
        var user = UserInfoProvider.GetUserInfo(MembershipContext.AuthenticatedUser.UserID);
        if (user != null)
        {
            user.UserSettings.UserShowIntroductionTile = false;
            user.Update();
        }
    }

    #endregion
}
