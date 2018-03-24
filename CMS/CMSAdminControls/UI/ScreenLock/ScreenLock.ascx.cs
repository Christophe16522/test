using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.UIControls;
using CMS.GlobalHelper;
using CMS.CMSHelper;
using CMS.SiteProvider;
using CMS.SettingsProvider;
using CMS.DataEngine;
using CMS.Helpers;
using CMS.Membership;

public partial class CMSAdminControls_UI_ScreenLock_ScreenLock : CMSUserControl, ICallbackEventHandler
{
    #region "Private variables"

    private TimeSpan timeLeft = TimeSpan.Zero;

    int minutesToLock = SettingsKeyInfoProvider.GetIntValue(SiteContext.CurrentSiteName + ".CMSScreenLockInterval");
    int secondsToWarning = SettingsKeyInfoProvider.GetIntValue(SiteContext.CurrentSiteName + ".CMSScreenLockWarningInterval");

    bool userIsLoggingOut = false;
    bool userAsksForState = false;
    bool userCanceling = false;
    bool userValidates = false;
    string validatePassword = String.Empty;

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        imgWarning.ImageUrl = GetImageUrl("/Design/Controls/Dialogs/lock_time.png");

        if (!RequestHelper.IsCallback())
        {
            // Creating the scripts which need to be called
            string clientScript = "function serverRequest(args, context){{ " +
                Page.ClientScript.GetCallbackEventReference(this, "args", "getResultShort", "") +
                "; }}" +
                "var screenLockEnabled = " + SecurityHelper.IsScreenLockEnabled(SiteContext.CurrentSiteName).ToString().ToLowerCSafe() + ";";

            // Register the client scripts
            ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "ScreenLock_" + ClientID, ScriptHelper.GetScript(clientScript));
            ScriptHelper.RegisterScriptFile(Page, "~/CMSAdminControls/UI/ScreenLock/ScreenLock.js");
        }
    }

    #endregion


    #region "ICallbackEventHandler Members"

    /// <summary>
    /// Prepares the callback result.
    /// </summary>
    public string GetCallbackResult()
    {
        CurrentUserInfo user = MembershipContext.AuthenticatedUser;

        if (userValidates)
        {
            if (!user.Enabled)
            {
                return "accountLocked";
            }

            // User wants to revalidate his session
            if (UserInfoProvider.IsUserPasswordDifferent(user, validatePassword))
            {
                // Password is invalid
                AuthenticationHelper.CheckInvalidPasswordAttempts(user, SiteContext.CurrentSiteName);

                if (!user.Enabled)
                {
                    return "accountLocked";
                }
                return "valbad";
            }
            else
            {
                // Password is correct
                CMSPage.IsScreenLocked = false;
                SecurityHelper.LogScreenLockAction();
                user.UserInvalidLogOnAttempts = 0;
                UserInfoProvider.SetUserInfo(user);

                return "valok|" + SecurityHelper.GetSecondsToShowScreenLockAction(SiteContext.CurrentSiteName);
            }
        }

        if (CMSPage.IsScreenLocked)
        {
            if (userAsksForState)
            {
                // Screen is locked
                return "isLocked|True";
            }
            else if (userIsLoggingOut)
            {
                // User wants to logout
                string signOutUrl = URLHelper.ApplicationPath.TrimEnd('/') + "/default.aspx";

                if (IsCMSDesk)
                {
                    // LiveID sign out URL is set if this LiveID session
                    CMSPage.SignOut(ref signOutUrl);
                }
                else
                {
                    CMSPage.SignOut();
                }

                return "logout|" + signOutUrl;
            }
        }
        else
        {
            // Check if ScreenLock is still enabled
            if (!SecurityHelper.IsScreenLockEnabled(SiteContext.CurrentSiteName))
            {
                return "disabled";
            }

            // User is canceling countdown and wants to stay active
            if (userCanceling)
            {
                SecurityHelper.LogScreenLockAction();
                return "cancelOk|" + SecurityHelper.GetSecondsToShowScreenLockAction(SiteContext.CurrentSiteName);
            }

            if ((int)timeLeft.TotalSeconds <= 0)
            {
                // User was inactive too long - lock screen
                CMSPage.IsScreenLocked = true;
                return "lockScreen";
            }

            if ((int)timeLeft.TotalSeconds <= secondsToWarning)
            {
                // Lock screen timeout is close - display warning
                return "showWarning|" + ((int)timeLeft.TotalSeconds).ToString();
            }
            else
            {
                // User is active - hide warning and lock screen (if opened)
                return "hideWarning|" + ((int)timeLeft.TotalSeconds - secondsToWarning).ToString();
            }
        }

        return ""; 
    }


    /// <summary>
    /// Raises the callback event.
    /// </summary>
    public void RaiseCallbackEvent(string eventArgument)
    {
        if (eventArgument.Contains("logout"))
        {
            userIsLoggingOut = true;
        }
        else if (eventArgument.Contains("validate"))
        {
            userValidates = true;
            validatePassword = eventArgument.Substring(9);
        }
        else if (eventArgument.Contains("isLocked"))
        {
            userAsksForState = true;
        }
        else if (eventArgument.Contains("cancel"))
        {
            userCanceling = true;
        }
        else if (eventArgument.Contains("action"))
        {
            userAsksForState = true;

            SecurityHelper.LogScreenLockAction();
        }

        // Find out when screen will be locked
        timeLeft = CMSPage.LastRequest + TimeSpan.FromMinutes(minutesToLock) - DateTime.Now;
    }

    #endregion
}
