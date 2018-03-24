using System;
using System.Collections.Generic;
using System.Reflection;

using CMS.DataEngine;
using CMS.EventLog;
using CMS.SiteProvider;
using CMS.SocialMarketing;


/// <summary>
/// Class ensuring Upgrade of social networking features
/// </summary>
public static class SocialMarketingUpgradeProcedure
{
    #region "Private properties"

    private static readonly Dictionary<string, string> mTwitterIDs = new Dictionary<string, string>();
    private static readonly Dictionary<string, string> mFacebokURLs = new Dictionary<string, string>();
    private static MethodInfo TryGetFacebookPageUrl = typeof(FacebookHelper).GetMethod("TryGetFacebookPageUrl", BindingFlags.NonPublic | BindingFlags.Static);

    #endregion


    #region "Upgrade methods"

    /// <summary>
    /// Parses the old token format from string and creates a new object representing the token in the new, correct way.
    /// </summary>
    /// <param name="token">String containing the token</param>
    /// <returns>The new token in correct format or null</returns>
    private static FacebookPageAccessTokenData? GetToken(String token)
    {
        if (String.IsNullOrWhiteSpace(token))
        {
            return null;
        }
        String[] tokenParts = token.Split(';');
        if (tokenParts.Length != 2)
        {
            return null;
        }
        token = tokenParts[0];
        var date = tokenParts[1];
        if (String.IsNullOrWhiteSpace(date))
        {
            return new FacebookPageAccessTokenData(token, null);
        }
        DateTime dt;
        if (!DateTime.TryParse(date, out dt))
        {
            return null;
        }
        dt = dt.ToUniversalTime();

        return new FacebookPageAccessTokenData(token, dt);
    }


    /// <summary>
    /// Retrieves a facebook page URL from dictionary or make a facebook api call to retrieve that URL based on its ID
    /// </summary>
    /// <param name="pageId">ID of a facebook page of which you want the URL</param>
    /// <param name="pageUrl">URL of that page if the ID is valid, null otherwise</param>
    /// <returns>true on success, false otherwise</returns>
    private static bool TryGetFacebookPageURL(string pageId, out string pageUrl)
    {
        string[] args = new string[2]
        {
            pageId,
            null,
        };
        if ((!mFacebokURLs.TryGetValue(args[0], out args[1])) && (bool)TryGetFacebookPageUrl.Invoke(null, args))
        {
            mFacebokURLs.Add(args[0], args[1]);
        }
        pageUrl = args[1];

        return pageUrl != null;
    }


    /// <summary>
    /// Attempts to create page Identity data based on the old Information we have and returns it in a typed object
    /// representing the page identity
    /// </summary>
    /// <param name="site">Object representing current site. (NOT Facebook site!!!) Needed because of siteName to get the correct settings.</param>
    /// <returns>Facebook PageIDData in correct format on success, null otherwise</returns>
    private static FacebookPageIdentityData? GetPageIdentity(SiteInfo site)
    {
        string siteName = site.SiteName;
        string pageId = SettingsKeyInfoProvider.GetStringValue(siteName + ".CMSFacebookPageId");
        string pageUrl = null;
        if (!TryGetFacebookPageURL(pageId, out pageUrl))
        {
            return null;
        }

        return new FacebookPageIdentityData(pageUrl, pageId);
    }


    /// <summary>
    /// Attempts to upgrade settings from the old way to he new-one.
    /// </summary>
    /// <param name="site">site we are importing for</param>
    /// <returns>true on success, false on failure</returns>
    private static void ImportFacebookSettings(SiteInfo site)
    {
        FacebookApplicationInfo fbAppInfo = new FacebookApplicationInfo()
        {
            FacebookApplicationDisplayName = site.DisplayName + " Facebook App",
            FacebookApplicationName = site.SiteName + "FacebookApp",
            FacebookApplicationSiteID = site.SiteID,
            FacebookApplicationConsumerKey = SettingsKeyInfoProvider.GetStringValue(site.SiteName + ".CMSFacebookConnectApiKey"),
            FacebookApplicationConsumerSecret = SettingsKeyInfoProvider.GetStringValue(site.SiteName + ".CMSFacebookApplicationSecret")
        };

        if (String.IsNullOrWhiteSpace(fbAppInfo.FacebookApplicationConsumerKey) || String.IsNullOrWhiteSpace(fbAppInfo.FacebookApplicationConsumerSecret))
        {
            return;
        }

        try
        {
            FacebookApplicationInfoProvider.SetFacebookApplicationInfo(fbAppInfo);
            fbAppInfo = FacebookApplicationInfoProvider.GetFacebookApplicationInfo(site.SiteName + "FacebookApp", site.SiteName);
        }
        catch (Exception ex)
        {
            // LogException
            EventLogProvider.LogException("Upgrade to 8.0", "Upgrade of SocialMarketing", ex, additionalMessage: "Error during Facebook Application storage to DB for site " + site.SiteName, siteId: site.SiteID);

            return;
        }

        // FB Page Part
        FacebookAccountInfo fbPageInfo = new FacebookAccountInfo()
        {
            FacebookAccountFacebookApplicationID = fbAppInfo.FacebookApplicationID,
            FacebookAccountSiteID = site.SiteID,
            FacebookAccountDisplayName = site.DisplayName + " Facebook Page",
            FacebookAccountName = site.SiteName + "FacebookPage",
            FacebookAccountIsDefault = true,
        };

        FacebookPageAccessTokenData? accToken = GetToken(SettingsKeyInfoProvider.GetStringValue(site.SiteName + ".CMSFacebookAccessToken"));
        if (accToken.HasValue)
        {
            fbPageInfo.FacebookPageAccessToken = accToken.Value;
        }
        else
        {
            // Log error importing settings for site
            EventLogProvider.LogEvent(EventType.ERROR, "Upgrade to 8.0", "Upgrade of SocialMarketing", eventDescription: "Error during parsing of PageAccessToken for site " + site.SiteName, siteId: site.SiteID);

            return;
        }

        FacebookPageIdentityData? PIData = GetPageIdentity(site);
        if (PIData.HasValue)
        {
            fbPageInfo.FacebookPageIdentity = PIData.Value;
        }
        else
        {
            // Log error importing settings for site
            EventLogProvider.LogEvent(EventType.ERROR, "Upgrade to 8.0", "Upgrade of SocialMarketing", eventDescription: "Error during Getting of PageIdentity for site " + site.SiteName, siteId: site.SiteID);

            return;
        }

        if (String.IsNullOrWhiteSpace(fbPageInfo.FacebookPageAccessToken.AccessToken) || String.IsNullOrWhiteSpace(fbPageInfo.FacebookPageIdentity.PageId) || String.IsNullOrWhiteSpace(fbPageInfo.FacebookPageIdentity.PageUrl))
        {
            return;
        }

        try
        {
            FacebookAccountInfoProvider.SetFacebookAccountInfo(fbPageInfo);
        }
        catch (Exception ex)
        {
            // Log Exception
            EventLogProvider.LogException("Upgrade to 8.0", "Upgrade of SocialMarketing", ex, additionalMessage: "Error during Facebook Page storage to DB for site " + site.SiteName, siteId: site.SiteID);

            return;
        }

        // URL shortener
        if (!SettingsKeyInfoProvider.IsValueInherited(site.SiteName + ".CMSFacebookURLShortenerType"))
        {
            string shortener = SettingsKeyInfoProvider.GetStringValue(site.SiteName + ".CMSFacebookURLShortenerType");
            SettingsKeyInfoProvider.SetValue(site.SiteName + ".CMSSocialMarketingURLShorteningFacebook", shortener);
        }

        return;
    }


    /// <summary>
    /// Attempts to upgrade settings from the old way to he new-one.
    /// </summary>
    /// <param name="site">site we are importing for</param>
    /// <returns>true on success, false on failure</returns>
    private static void ImportTwitterSettings(SiteInfo site)
    {
        TwitterApplicationInfo twittAppInfo = new TwitterApplicationInfo()
        {
            TwitterApplicationDisplayName = site.DisplayName + "Twitter App",
            TwitterApplicationName = site.SiteName + "TwitterApp",
            TwitterApplicationConsumerKey = SettingsKeyInfoProvider.GetStringValue(site.SiteName + ".CMSTwitterConsumerKey"),
            TwitterApplicationConsumerSecret = SettingsKeyInfoProvider.GetStringValue(site.SiteName + ".CMSTwitterConsumerSecret"),
            TwitterApplicationSiteID = site.SiteID
        };

        if (String.IsNullOrWhiteSpace(twittAppInfo.TwitterApplicationConsumerKey) || String.IsNullOrWhiteSpace(twittAppInfo.TwitterApplicationConsumerSecret))
        {
            return;
        }

        try
        {
            TwitterApplicationInfoProvider.SetTwitterApplicationInfo(twittAppInfo);
            twittAppInfo = TwitterApplicationInfoProvider.GetTwitterApplicationInfo(site.SiteName + "TwitterApp", site.SiteName);
        }
        catch (Exception ex)
        {
            EventLogProvider.LogException("Upgrade to 8.0", "Upgrade of SocialMarketing", ex, additionalMessage: "Error during Twitter application storage to DB for site " + site.SiteName, siteId: site.SiteID);

            return;
        }

        TwitterAccountInfo twittAccountInfo = new TwitterAccountInfo()
        {
            TwitterAccountName = site.SiteName + "TwitterChannel",
            TwitterAccountDisplayName = site.DisplayName + " Twitter Channel",
            TwitterAccountTwitterApplicationID = twittAppInfo.TwitterApplicationID,
            TwitterAccountSiteID = site.SiteID,
            TwitterAccountAccessToken = SettingsKeyInfoProvider.GetStringValue(site.SiteName + ".CMSTwitterAccessToken"),
            TwitterAccountAccessTokenSecret = SettingsKeyInfoProvider.GetStringValue(site.SiteName + ".CMSTwitterAccessTokenSecret"),
            TwitterAccountIsDefault = true,
        };

        twittAccountInfo.TwitterAccountUserID = GetTwitterUserId(twittAppInfo, twittAccountInfo);

        if (String.IsNullOrWhiteSpace(twittAccountInfo.TwitterAccountAccessToken) || String.IsNullOrWhiteSpace(twittAccountInfo.TwitterAccountAccessTokenSecret))
        {
            return;
        }

        try
        {
            TwitterAccountInfoProvider.SetTwitterAccountInfo(twittAccountInfo);
        }
        catch (Exception ex)
        {
            EventLogProvider.LogException("Upgrade to 8.0", "Upgrade of SocialMarketing", ex, additionalMessage: "Error during Twitter channel storage to DB for site " + site.SiteName, siteId: site.SiteID);

            return;
        }

        // URL shortener
        if (!SettingsKeyInfoProvider.IsValueInherited(site.SiteName + ".CMSTwitterURLShortenerType"))
        {
            string shortener = SettingsKeyInfoProvider.GetStringValue(site.SiteName + ".CMSTwitterURLShortenerType");
            SettingsKeyInfoProvider.SetValue(site.SiteName + ".CMSSocialMarketingURLShorteningTwitter", shortener);
        }

        return;
    }


    /// <summary>
    /// Gets twitter User ID and caches it to avoid exceeding limits
    /// </summary>
    /// <param name="twittAppInfo">Application to use to interact with Twitter</param>
    /// <param name="twittAccountInfo">Channel to be used to interact with Twitter</param>
    /// <returns>null on failure, correct Twitter UserId otherwise</returns>
    private static string GetTwitterUserId(TwitterApplicationInfo twittAppInfo, TwitterAccountInfo twittAccountInfo)
    {
        string key = String.Join(":", twittAppInfo.TwitterApplicationConsumerKey, twittAppInfo.TwitterApplicationConsumerSecret, twittAccountInfo.TwitterAccountAccessToken, twittAccountInfo.TwitterAccountAccessTokenSecret);
        string userId = null;
        if (!mTwitterIDs.TryGetValue(key, out userId))
        {
            userId = TwitterHelper.GetTwitterUserId(twittAppInfo.TwitterApplicationConsumerKey, twittAppInfo.TwitterApplicationConsumerSecret, twittAccountInfo.TwitterAccountAccessToken, twittAccountInfo.TwitterAccountAccessTokenSecret);
            mTwitterIDs.Add(key, userId);
        }

        return userId;
    }


    /// <summary>
    /// Method upgrades global settings concerning URLShorteners.
    /// </summary>
    private static void UpgradeGlobalShortenersSettings()
    {
        string shortener = SettingsKeyInfoProvider.GetStringValue("CMSTwitterURLShortenerType");
        if (!String.IsNullOrWhiteSpace(shortener))
        {
            SettingsKeyInfoProvider.SetValue("CMSSocialMarketingURLShorteningTwitter", shortener);
        }
        else
        {
            EventLogProvider.LogEvent(EventType.ERROR, "Upgrade to 8.0", "Upgrade of SocialMarketing", eventDescription: "Import of global setting for Twitter URL shortener failed");
        }
        shortener = SettingsKeyInfoProvider.GetStringValue("CMSFacebookURLShortenerType");
        if (!String.IsNullOrWhiteSpace(shortener))
        {
            SettingsKeyInfoProvider.SetValue("CMSSocialMarketingURLShorteningFacebook", shortener);
        }
        else
        {
            EventLogProvider.LogEvent(EventType.ERROR, "Upgrade to 8.0", "Upgrade of SocialMarketing", eventDescription: "Import of global setting for Facebook URL shortener failed");
        }
    }


    /// <summary>
    /// Method removes obsolete settings.
    /// </summary>
    private static void CleanupSettings()
    {
        var settings = SettingsKeyInfoProvider.GetSettingsKeys().Where(@"[KeyName] = 'CMSFacebookPageId' OR
[KeyName] = 'CMSFacebookAccessToken' OR
[KeyName] = 'CMSFacebookURLShortenerType' OR
[KeyName] = 'CMSTwitterURLShortenerType' OR
[KeyName] = 'CMSTwitterConsumerKey' or
[KeyName] = 'CMSTwitterConsumerSecret' OR
[KeyName] = 'CMSTwitterAccessToken' OR
[KeyName] = 'CMSTwitterAccessTokenSecret' OR
[KeyName] = 'CMSRequiredFacebookPage'");

        foreach (SettingsKeyInfo settingsKeyInfo in settings)
        {
            SettingsKeyInfoProvider.DeleteSettingsKeyInfo(settingsKeyInfo);
        }

        var categories = SettingsCategoryInfoProvider.GetSettingsCategories().Where(@"[CategoryName] = 'CMS.Facebook.UrlShortening' OR
[CategoryName] = 'CMS.Twitter.General' OR
[CategoryName] = 'CMS.Twitter.UrlShortening' OR
[CategoryName] = 'CMS.Twitter'");

        foreach (SettingsCategoryInfo category in categories)
        {
            SettingsCategoryInfoProvider.DeleteSettingsCategoryInfo(category);
        }
    }


    /// <summary>
    /// Upgrades the system of storage of settings about social marketing from the old way to the new-one.
    /// </summary>
    public static void UpgradeV7ToV8()
    {
        UpgradeGlobalShortenersSettings();
        InfoDataSet<SiteInfo> sites = SiteInfoProvider.GetSites().TypedResult;
        if (sites.Items.Count > 0)
        {
            // Loop through the individual sites
            foreach (SiteInfo site in sites.Items)
            {
                // FaceBook part
                try
                {
                    ImportFacebookSettings(site);
                }
                catch (Exception ex)
                {
                    EventLogProvider.LogException("Upgrade to 8.0", "Upgrade of SocialMarketing", ex, additionalMessage: "Error during import of Facebook settings for site " + site.SiteName, siteId: site.SiteID);
                }

                // Twitter Part
                try
                {
                    ImportTwitterSettings(site);
                }
                catch (Exception ex)
                {
                    EventLogProvider.LogException("Upgrade to 8.0", "Upgrade of SocialMarketing", ex, additionalMessage: "Error during import of Twitter settings for site " + site.SiteName, siteId: site.SiteID);
                }
            }
        }

        CleanupSettings();
    }

    #endregion
}