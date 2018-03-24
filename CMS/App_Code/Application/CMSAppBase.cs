using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Security.Principal;
using System.Web;
using System.Data;
using System.Web.Security;
using System.Web.Configuration;

using CMS.CMSHelper;
//using CMS.CMSOutputFilter;
//using CMS.DataEngine;
//using CMS.DatabaseHelper;
using CMS.EmailEngine;
using CMS.EventLog;
using CMS.GlobalHelper;
using CMS.IO;
using CMS.LicenseProvider;
using CMS.OutputFilter;
using CMS.PortalControls;
using CMS.PortalEngine;
using CMS.WebServices;
using CMS.Scheduler;
using CMS.SettingsProvider;
using CMS.SiteProvider;
using CMS.Synchronization;
using CMS.URLRewritingEngine;
//using CMS.VirtualPathHelper;
using CMS.WebAnalytics;
using CMS.WebFarmSync;
//using CMS.WebFarmSyncHelper;
using CMS.DocumentEngine;
using CMS.UIControls;
using CMS.ExtendedControls;
using CMS.Helpers;
using CMS.Base;
using CMS.Membership;
using CMS.Search;
using CMS.MacroEngine;
using CMS.Protection;
using CMS.HealthMonitoring;


public class CMSAppBase
{
    #region "Variables"


    #region "System data (do not modify)"

    /// <summary>
    /// Application version, do not change.
    /// </summary>
    /// const string APP_VERSION = "7.0";

    #endregion


    private static DateTime mApplicationStart = DateTime.Now;
    private static DateTime mApplicationStartFinished = DateTime.MinValue;

    private static bool firstEndRequestAfterStart = true;

    private static CMSStatic<bool?> mApplicationInitialized = new CMSStatic<bool?>();
    private static bool mApplicationPreInitialized = false;

    private static CMSStatic<string> mConnectionErrorMessage = new CMSStatic<string>();

    private static object mLock = new object();

    private static bool sessionTimeoutInicialized = false;

    /// <summary>
    /// Windows identity.
    /// </summary>
    private static WindowsIdentity mWindowsIdentity = null;

    #endregion


    #region "Properties"

    /// <summary>
    /// Date and time of the application start.
    /// </summary>
    public static DateTime ApplicationStart
    {
        get
        {
            return mApplicationStart;
        }
    }


    /// <summary>
    /// Date and time when the application start (initialization) finished its execution.
    /// </summary>
    public static DateTime ApplicationStartFinished
    {
        get
        {
            return mApplicationStartFinished;
        }
    }


    /// <summary>
    /// Returns true if the application was already initialized.
    /// </summary>
    public static bool ApplicationInitialized
    {
        get
        {
            if (mApplicationInitialized.Value == null)
            {
                return false;
            }

            return mApplicationInitialized.Value.Value;
        }
        protected set
        {
            mApplicationInitialized.Value = value;
        }
    }


    /// <summary>
    /// Connection error message.
    /// </summary>
    public static string ConnectionErrorMessage
    {
        get
        {
            return mConnectionErrorMessage;
        }
        protected set
        {
            mConnectionErrorMessage.Value = value;
        }
    }


    /// <summary>
    /// Returns true if the connection is available.
    /// </summary>
    public static bool ConnectionAvailable
    {
        get
        {
            //return SqlHelperClass.IsConnectionStringInitialized && ApplicationInitialized;
            return ApplicationInitialized;
        }
    }

    #endregion


    #region "Application events"

    /// <summary>
    /// Application start event handler.
    /// </summary>
    public static void CMSApplicationStart()
    {
#if DEBUG
        // Set debug mode
        SystemHelper.IsWebProjectDebug = true;
#endif
    }


    /// <summary>
    /// Application error event handler.
    /// </summary>
    public static void CMSApplicationError(object sender, EventArgs e)
    {
        // Handle the event
        using (var h = CMSApplicationEvents.Error.StartEvent(e))
        {
            if (h.Continue)
            {
                // Log the error
                LogLastApplicationError();
            }

            // Finalize the event
            h.FinishEvent();
        }
    }


    /// <summary>
    /// Application end event handler.
    /// </summary>
    public static void CMSApplicationEnd(object sender, EventArgs e)
    {
        // Handle the event
        using (var h = CMSApplicationEvents.End.StartEvent(e))
        {
            if (h.Continue)
            {
                // Delete dynamic server
                if (WebSyncHelper.DeleteGeneratedWebFarmServers)
                {
                    string serverName = ValidationHelper.GetCodeName(WebFarmServerInfoProvider.GetAutomaticServerName());

                    WebFarmServerInfo server = WebFarmServerInfoProvider.GetWebFarmServerInfo(serverName);
                    WebFarmServerInfoProvider.DeleteWebFarmServerInfo(server);
                }

                try
                {
                    // Log the application end
                    LogApplicationEnd();
                }
                catch
                {
                }

                // Disable logging of events
                EventLogProvider.LoggingEnabled = false;
            }

            // Finalize the event
            h.FinishEvent();
        }
    }

    #endregion


    #region "Session events"

    /// <summary>
    /// Session start event handler.
    /// </summary>
    public static void CMSSessionStart(object sender, EventArgs e)
    {
        if (ConnectionAvailable)
        {
            RequestHelper.LogRequestOperation("Session_Start", null, 0);
            DebugHelper.SetContext("Session_Start");

            // Handle the event
            using (var h = CMSSessionEvents.Start.StartEvent(e))
            {
                if (h.Continue)
                {
                    string siteName = SiteContext.CurrentSiteName;

                    // If path was rewritten log session
                    RequestStatusEnum status = URLRewriter.CurrentStatus;
                    if ((status == RequestStatusEnum.PathRewritten) ||
                        (status == RequestStatusEnum.MVCPage))
                    {
                        // Add session to the session manager
                        if (SessionManager.OnlineUsersEnabled && !URLHelper.IsExcludedSystem(RequestContext.CurrentRelativePath))
                        {
                            SessionManager.UpdateCurrentSession(null);
                        }
                    }

                    if (siteName != "")
                    {
                        // If authentication mode is Windows, set user UI culture
                        if (RequestHelper.IsWindowsAuthentication() && AuthenticationHelper.IsAuthenticated())
                        {
                            UserInfo currentUser = MembershipContext.AuthenticatedUser;
                            if (!currentUser.IsPublic())
                            {
                                UserInfoProvider.SetPreferredCultures(currentUser);
                            }
                        }
                    }
                }

                // Finalize the event
                h.FinishEvent();
            }

            DebugHelper.ReleaseContext();
        }

        // Count the session
        RequestHelper.TotalSessions++;
    }


    /// <summary>
    /// Session end event handler.
    /// </summary>
    public static void CMSSessionEnd(object sender, EventArgs e)
    {
        if (ConnectionAvailable)
        {
            // Handle the event
            using (var h = CMSSessionEvents.End.StartEvent(e))
            {
                if (h.Continue)
                {
                    // Removes expired sessions
                    if (SessionManager.OnlineUsersEnabled)
                    {
                        SessionManager.RemoveExpiredSessions();
                    }
                }

                // Finalize the event
                h.FinishEvent();
            }
        }
    }

    #endregion


    #region "Request events"

    /// <summary>
    /// Begin request event handler
    /// </summary>
    public static void CMSBeginRequest(object sender, EventArgs e)
    {
        // Do the actions before the request begins
        BeforeBeginRequest(sender, e);

        try
        {
            // Handle the event
            using (var h = CMSRequestEvents.Begin.StartEvent(e))
            {
                if (h.Continue)
                {
                    // Check if Database installation needed
                    if (FileRedirect() || InstallerFunctions.InstallRedirect(false))
                    {
                        return;
                    }

                    if (ConnectionAvailable)
                    {
                        // Create request scope
                        if (ConnectionHelper.UseContextConnection)
                        {
                            ConnectionContext.EnsureRequestScope(null, false,
                                                                  ConnectionHelper.KeepContextConnectionOpen);
                        }

                        // Enable debugging
                        SetInitialDebug();
                    }
                }

                // Finalize the event
                h.FinishEvent();
            }
        }
        finally
        {
            // Check WebDAV PROPFIND request
            if (RequestHelper.IsWebDAVPropfindRequest())
            {
                // End request
                RequestHelper.EndResponse();
            }
        }
    }


    /// <summary>
    /// Gets the connection string for the current request
    /// </summary>
    private static string GetRequestConnectionStringPrefix()
    {
        string domain = URLHelper.GetCurrentDomain();

        return ConnectionHelper.GetConnectionStringPrefix(domain);
    }


    /// <summary>
    /// Fired before the begin request executes
    /// </summary>
    private static void BeforeBeginRequest(object sender, EventArgs e)
    {
        // Ensure the default request context
        string connStringPrefix = GetRequestConnectionStringPrefix();
        if (!String.IsNullOrEmpty(connStringPrefix))
        {
            string connString = connStringPrefix + SqlHelper.DEFAULT_CONNECTIONSTRING_NAME;

            CMSStatic.CurrentContext = connString;
            CacheHelper.CurrentCachePrefix = connString;

            ConnectionHelper.ConnectionStringName = connString;
            ConnectionHelper.ConnectionStringPrefix = connStringPrefix;
        }

        // Azure begin request init
        if (!ApplicationInitialized)
        {
            AzureInit.Current.BeginRequestInit();
        }

        RequestHelper.PendingRequests.Increment(null);

        // Check the application validity
        LicenseHelper.CheckValidity();


        // Application start events
        FirstRequestInitialization(sender, e);

        // Set script timeout due to compilation delay
        if ((HttpContext.Current != null) && (HttpContext.Current.Server.ScriptTimeout < 240))
        {
            HttpContext.Current.Server.ScriptTimeout = 240;
        }

        // Check the number of Azure instances
        CheckAzureInstances();

        // Start thread which is checking new web farm tasks if database web farm updater is used
        if (!InstallerFunctions.IsInstallInProgress())
        {
            DbWebFarmUpdater.EnsureThread();
        }
    }


    /// <summary>
    /// Checks the number of Azure instances
    /// </summary>
    private static void CheckAzureInstances()
    {
        // Actual number of servers is bigger than allowed count by license - don't create web farm server - log event and redirect to error page
        if (SystemContext.IsRunningOnAzure && ConnectionAvailable)
        {
            var license = LicenseContext.CurrentLicenseInfo;
            if ((license != null) && (license.LicenseServers > 0) && (license.LicenseServers < AzureHelper.NumberOfInstances))
            {
                // Log to the event log
                EventLogProvider log = new EventLogProvider();
                log.LogEvent(EventLogProvider.ERROR, DateTime.Now, "Application_Start", "WEBFARMSERVER", null, "The current license servers limit has exceeded.");

                // Redirect to error
                HttpContext.Current.Server.Transfer("~/CMSMessages/error.aspx?title=" + ResHelper.GetString("webfarm.serverslimitexceeded") + "&text=" + ResHelper.GetString("webfarm.serverslimitexceeded") + "&backlink=0");
            }
        }
    }


    /// <summary>
    /// Request authentication handler.
    /// </summary>
    public static void CMSAuthenticateRequest(object sender, EventArgs e)
    {
        if (ConnectionAvailable)
        {
            // Handle the event
            using (var h = CMSRequestEvents.Authenticate.StartEvent(e))
            {
                if (h.Continue)
                {
                    // Allow action context user initialization
                    CMSActionContext.CurrentAllowInitUser = true;

                    // Check for single sign-in authentication token
                    CheckAuthenticationGUID();
                }

                // Finalize the event
                h.FinishEvent();
            }
        }
    }


    /// <summary>
    /// Request authorization handler.
    /// </summary>
    public static void CMSAuthorizeRequest(object sender, EventArgs e)
    {
        RequestHelper.LogRequestOperation("AuthorizeRequest", null, 0);
        DebugHelper.SetContext("AuthorizeRequest");

        if (ConnectionAvailable)
        {
            // Handle the event
            using (var h = CMSRequestEvents.Authorize.StartEvent(e))
            {
                if (h.Continue)
                {
                    string relativePath = RequestContext.CurrentRelativePath;

                    // Check the excluded status
                    ExcludedSystemEnum excludedEnum = URLHelper.IsExcludedSystemEnum(relativePath);

                    RequestContext.CurrentExcludedStatus = excludedEnum;

                    ViewModeOnDemand viewMode = new ViewModeOnDemand();
                    SiteNameOnDemand siteName = new SiteNameOnDemand();

                    // Try to send the output from the cache without URL rewriting
                    if (URLRewriter.SendOutputFromCache(relativePath, excludedEnum, viewMode, siteName))
                    {
                        if (OutputFilterContext.OutputFilterEndRequestRequired)
                        {
                            HttpContext context = HttpContext.Current;
                            string newQuery = null;

                            // Ensure the raw URL as a part of the request
                            if (URLRewriter.FixRewriteRedirect)
                            {
                                newQuery = "rawUrl=" + HttpUtility.UrlEncode(context.Request.RawUrl);
                            }

                            context.RewritePath("~/CMSPages/blank.aspx", null, newQuery);
                        }
                        return;
                    }

                    // Handle the virtual context
                    HandleVirtualContext(ref relativePath, ref excludedEnum);

                    // Ensure routes for current site
                    CMSDocumentRouteHelper.EnsureRoutes(SiteContext.CurrentSiteName);
                }

                // Finalize the event
                h.FinishEvent();
            }
        }

        DebugHelper.ReleaseContext();
    }


    /// <summary>
    /// Post handler mapping handler.
    /// </summary>
    public static void CMSPostMapRequestHandler(object sender, EventArgs e)
    {
        IHttpHandler handler = URLRewritingContext.HttpHandler;
        if (handler != null)
        {
            HttpContext.Current.Handler = handler;
        }
    }


    /// <summary>
    /// Handler mapping handler.
    /// </summary>
    public static void CMSMapRequestHandler(object sender, EventArgs e)
    {
        RequestStatusEnum status = URLRewriter.CurrentStatus;
        if (ConnectionAvailable && (status != RequestStatusEnum.SentFromCache))
        {
            RequestHelper.LogRequestOperation("MapRequestHandler", null, 0);
            DebugHelper.SetContext("MapRequestHandler");

            // Handle the event
            using (var h = CMSRequestEvents.MapRequestHandler.StartEvent(e))
            {
                if (h.Continue)
                {
                    // Get request parameters
                    string relativePath = RequestContext.CurrentRelativePath;

                    ExcludedSystemEnum excludedEnum = RequestContext.CurrentExcludedStatus;
                    if (excludedEnum == ExcludedSystemEnum.Unknown)
                    {
                        excludedEnum = URLHelper.IsExcludedSystemEnum(relativePath);
                    }

                    ViewModeOnDemand viewMode = new ViewModeOnDemand();
                    SiteNameOnDemand siteName = new SiteNameOnDemand();

                    // Set flag to output filter for cms dialogs
                    if (excludedEnum == ExcludedSystemEnum.CMSDialog)
                    {
                        ResponseOutputFilter.UseFormActionWithCurrentURL = true;
                    }

                    // Perform the URL rewriting
                    RewriteUrl(status, relativePath, excludedEnum, viewMode, siteName);
                }

                // Finalize the event
                h.FinishEvent();
            }

            DebugHelper.ReleaseContext();
        }
    }


    /// <summary>
    /// Handles the virtual context for the request
    /// </summary>
    /// <param name="relativePath">Relative path</param>
    /// <param name="excludedEnum">Excluded page enum</param>
    private static void HandleVirtualContext(ref string relativePath, ref ExcludedSystemEnum excludedEnum)
    {
        // Handle the virtual context
        bool isVirtual = VirtualContext.HandleVirtualContext(ref relativePath);
        if (isVirtual)
        {
            // Check the excluded status
            excludedEnum = URLHelper.IsExcludedSystemEnum(relativePath);

            switch (excludedEnum)
            {
                case ExcludedSystemEnum.GetFilePage:
                case ExcludedSystemEnum.GetResource:
                    // Remove virtual context prefix
                    VirtualContext.CurrentURLPrefix = null;
                    break;
            }

            RequestContext.CurrentExcludedStatus = excludedEnum;
        }
    }


    /// <summary>
    /// Acquire request state event handler.
    /// </summary>
    public static void CMSAcquireRequestState(object sender, EventArgs e)
    {
        RequestHelper.LogRequestOperation("AcquireRequestState", null, 0);
        DebugHelper.SetContext("AcquireRequestState");

        // Handle the event
        using (var h = CMSRequestEvents.AcquireRequestState.StartEvent(e))
        {
            if (h.Continue)
            {
                // Try to redirect as planned first
                if (URLRewriter.FixRewriteRedirect)
                {
                    URLRewriter.PerformPlannedRedirect();
                }

                // Keep session timeout within static variable
                if ((!sessionTimeoutInicialized) && (HttpContext.Current != null) && (HttpContext.Current.Session != null))
                {
                    SessionHelper.SessionTimeout = HttpContext.Current.Session.Timeout;
                    sessionTimeoutInicialized = true;
                }

                // Keep current status
                RequestStatusEnum status = URLRewriter.CurrentStatus;


                // Log analytics or activity
                switch (status)
                {
                    case RequestStatusEnum.PathRewritten:
                    case RequestStatusEnum.MVCPage:
                    case RequestStatusEnum.SentFromCache:

                        if (ConnectionAvailable && !RequestHelper.IsPostBack() && !QueryHelper.GetBoolean(URLHelper.SYSTEM_QUERY_PARAMETER, false) && (PortalContext.ViewMode == ViewModeEnum.LiveSite))
                        {
                            string siteName = SiteContext.CurrentSiteName;
                            PageInfo currentPage = DocumentContext.CurrentPageInfo;

                            if (currentPage != null)
                            {
                                if (AnalyticsHelper.IsLoggingEnabled(siteName, currentPage.NodeAliasPath))
                                {
                                    if (!AnalyticsHelper.JavascriptLoggingEnabled(siteName))
                                    {
                                        // When JS logging is disabled visitor status is set to context in LogVisitor() method
                                        AnalyticsMethods.LogVisitor(siteName);
                                        // Log referring, search, landing and exit pages
                                        AnalyticsMethods.LogAnalytics(SessionHelper.GetSessionID(), currentPage, siteName);
                                    }
                                    else
                                    {
                                        // Visitor status must be set to context here because LogVisitor() method is called later in different request when using JS logging
                                        bool idleExpired = false;
                                        VisitorStatusEnum visitorStatus = AnalyticsHelper.GetContextStatus(SiteContext.CurrentSiteName, ref idleExpired);

                                        if (visitorStatus == VisitorStatusEnum.Unknown)
                                        {
                                            visitorStatus = VisitorStatusEnum.FirstVisit;
                                        }
                                        else if (idleExpired)
                                        {
                                            visitorStatus = VisitorStatusEnum.MoreVisits;
                                        }

                                        AnalyticsContext.CurrentVisitStatus = visitorStatus;
                                    }
                                }

                                // Log search crawler visit, check whether logging is enabled, do not check the excluded crawlers
                                if (AnalyticsHelper.IsLoggingEnabled(siteName, currentPage.NodeAliasPath, LogExcludingFlags.SkipCrawlerCheck))
                                {
                                    AnalyticsMethods.LogSearchCrawler(siteName, currentPage);
                                }

                                if (!AnalyticsHelper.JavascriptLoggingEnabled(siteName))
                                {
                                    // Log activities
                                    if (ActivitySettingsHelper.ActivitiesEnabledAndModuleLoaded(siteName) && ActivitySettingsHelper.ActivitiesEnabledForThisUser(MembershipContext.AuthenticatedUser))
                                    {
                                        int contactId = ModuleCommands.OnlineMarketingGetCurrentContactID();

                                        AnalyticsContext.RequestContactID = contactId;
                                        if (contactId > 0)
                                        {
                                            ModuleCommands.OnlineMarketingLogLandingPage(0);
                                            ModuleCommands.OnlineMarketingLogExternalSearch(0);
                                            UpdateContactsIPandUserAgent(siteName);
                                        }
                                    }
                                }
                            }
                        }
                        break;
                }

                if (ConnectionAvailable)
                {
                    // Check the page security
                    CheckSecurity();
                }

                // Check whether request should be ended for full page cached page
                if (OutputFilterContext.OutputFilterEndRequestRequired)
                {
                    OutputHelper.EndRequest();
                }
            }

            // Finalize the event
            h.FinishEvent();
        }

        DebugHelper.ReleaseContext();
    }


    /// <summary>
    /// End request event handler.
    /// </summary>
    public static void CMSEndRequest(Object sender, EventArgs e)
    {
        if (CMSFunctions.AnyDebugEnabled)
        {
            RequestHelper.LogRequestOperation("EndRequest", HttpContext.Current.Response.Status.ToString(), 0);
            DebugHelper.SetContext("EndRequest");
        }

        try
        {
            // Handle the event
            using (var h = CMSRequestEvents.End.StartEvent(e))
            {
                if (h.Continue)
                {
                    var status = URLRewriter.CurrentStatus;
                    HttpApplication app = (HttpApplication) sender;

                    // Check whether the output was sent from cache
                    bool sentFromCache = (status == RequestStatusEnum.SentFromCache);
                    if (!sentFromCache)
                    {
                        // Check status code 306 (code which is returned only when authentication failed in REST Service), change it to classical 401 - Unauthorized.
                        // That's because 401 is automatically handled by ASP.NET and redirected to logon page, this way we can achieve to return 401 without ASP.NET to interfere.
                        if (app.Response.StatusCode == 306)
                        {
                            // Set correct authentication header
                            switch (RESTSecurityInvoker.GetAuthenticationType(SiteContext.CurrentSiteName))
                            {
                                case "basic":
                                    app.Response.Headers["WWW-Authenticate"] = string.Format("Basic realm=\"{0}\"",
                                                                                             "CMS REST Service");
                                    break;

                                default:
                                    app.Response.Headers["WWW-Authenticate"] = string.Format("Digest realm=\"{0}\"",
                                                                                             "CMS REST Service");
                                    break;
                            }
                            app.Response.StatusCode = 401;
                        }

                        // Check page not found state and use 
                        if (ConnectionAvailable && (app.Response.StatusCode == 404))
                        {
                            if (!ValidationHelper.GetBoolean(RequestStockHelper.GetItem("CMSPageNotFoundHandled"), false))
                            {
                                URLRewriter.PageNotFound();
                            }
                        }
                    }


                    // Register the debug logs
                    if (CMSFunctions.AnyDebugEnabled)
                    {
                        RequestHelper.LogRequestValues(true, true, true);
                        DebugHelper.RegisterLogs();
                    }

                    // If connection is available
                    if (ConnectionAvailable)
                    {
                        // Restore the response cookies if fullpage caching is set
                        if (sentFromCache && (URLRewriter.CurrentOutputCache > 0) &&
                            (URLRewriter.CurrentStatus != RequestStatusEnum.GetFile))
                        {
                            CookieHelper.RestoreResponseCookies();
                        }

                        // Set cookies as read-only for further usage in the request
                        CookieHelper.ReadOnly = true;

                        // Additional tasks within first request end
                        if (firstEndRequestAfterStart)
                        {
                            firstEndRequestAfterStart = false;

                            // Re-initialize tasks which were stopped by application end
                            bool[] debugs = DebugHelper.DisableSchedulerDebug();
                            SchedulingExecutor.ReInitCorruptedTasks();
                            DebugHelper.RestoreDebugSettings(debugs);

                            ModuleCommands.NewsletterClearEmailsSendingStatus();
                            EmailInfoProvider.ResetSendingStatus();

                            // Process synchronization tasks
                            WebSyncHelper.ProcessMyTasks();

                            // Run smart search 
                            SearchTaskInfoProvider.ProcessTasks();
                        }
                        else if (!RequestHelper.IsAsyncPostback())
                        {
                            // Attempt to run the scheduler
                            RunScheduler(status);

                            // Run performance timer
                            EnsurePerformanceCounterTimer();
                        }

                        // Log page view
                        if (RequestContext.LogPageHit &&
                            (!QueryHelper.GetBoolean(URLHelper.SYSTEM_QUERY_PARAMETER, false)) &&
                            (status != RequestStatusEnum.RESTService))
                        {
                            string siteName = SiteContext.CurrentSiteName;
                            UserInfo currentUser = MembershipContext.AuthenticatedUser;
                            PageInfo currentPage = DocumentContext.CurrentPageInfo;

                            if (currentPage != null)
                            {
                                if (!RequestHelper.IsPostBack() &&
                                    AnalyticsHelper.IsLoggingEnabled(siteName, currentPage.NodeAliasPath))
                                {
                                    if (!AnalyticsHelper.JavascriptLoggingEnabled(siteName))
                                    {
                                        // Log page view
                                        if (AnalyticsHelper.TrackPageViewsEnabled(siteName))
                                        {
                                            HitLogProvider.LogHit(HitLogProvider.PAGE_VIEWS, siteName,
                                                                  currentUser.PreferredCultureCode,
                                                                  currentPage.NodeAliasPath, currentPage.NodeID);
                                        }
                                    }

                                    // Log aggregated view
                                    if (QueryHelper.Contains("feed") &&
                                        AnalyticsHelper.TrackAggregatedViewsEnabled(siteName))
                                    {
                                        HitLogProvider.LogHit(HitLogProvider.AGGREGATED_VIEWS, siteName,
                                                              currentUser.PreferredCultureCode,
                                                              currentPage.NodeAliasPath, currentPage.NodeID);
                                    }
                                }
                                if (!AnalyticsHelper.JavascriptLoggingEnabled(siteName))
                                {
                                    int contactId = AnalyticsContext.RequestContactID;
                                    if (contactId > 0)
                                    {
                                        ModuleCommands.OnlineMarketingLogPageVisit(0);
                                    }
                                }
                            }
                        }

                        // Run web farm updater if it is required within current request
                        if (!sentFromCache)
                        {
                            WebSyncHelper.SynchronizeWebFarm();
                        }

                        // Run any potential queued workers
                        CMSWorkerQueue queue = CMSWorkerQueue.Instance;
                        if (queue != null)
                        {
                            queue.Parameter = IntegrationHelper.TouchedConnectorNames;
                            queue.OnFinished += CMSWorkerQueue_OnFinished;

                            queue.RunAll();
                        }

                        // Dispose the request scope
                        if (ConnectionContext.SomeConnectionUsed)
                        {
                            CMSConnectionScope.DisposeRequestScope();
                        }
                    }
                }

                // Finalize the event
                h.FinishEvent();
            }
        }
        finally
        {
            // Write SQL query log
            if (CMSFunctions.AnyDebugEnabled)
            {
                RequestHelper.LogRequestOperation("FinishRequest", null, 0);
                DebugHelper.SetContext("FinishRequest");

                DebugHelper.ReleaseContext();
                SqlHelperClass.WriteRequestLog();
                SecurityHelper.WriteRequestLog();
                RequestHelper.WriteRequestLog();
                MacroResolver.WriteRequestLog();
                AnalyticsHelper.WriteRequestLog();
            }

            RequestHelper.PendingRequests.Decrement(null);
        }
    }


    /// <summary>
    /// PreSendRequestHeaders event handler.
    /// </summary>    
    public static void CMSPreSendRequestHeaders(Object sender, EventArgs e)
    {
        HttpApplication app = (HttpApplication)sender;

        // Add protection against clickjacking - adding headers works only in integrated mode
        if (SystemContext.IsIntegratedMode && !SecurityHelper.IsXFrameOptionsExcluded(RequestContext.CurrentRelativePath, Convert.ToString(URLHelper.Url)))
        {
            app.Response.Headers["X-Frame-Options"] = "SAMEORIGIN";
        }
    }

    #endregion


    #region "Overridden methods"

    /// <summary>
    /// Custom cache parameters processing.
    /// </summary>
    public static string CMSGetVaryByCustomString(HttpContext context, string custom)
    {
        if (context == null)
        {
            return "";
        }

        HttpResponse response = context.Response;

        // Do not cache on postback
        if (URLHelper.IsPostback())
        {
            response.Cache.SetNoStore();
            return Guid.NewGuid().ToString();
        }

        PageInfo currentPage = DocumentContext.CurrentPageInfo;
        string result = null;

        // Cache control
        if ((currentPage != null) && !custom.StartsWithCSafe("control;"))
        {
            // Check page caching minutes
            int cacheMinutes = currentPage.NodeCacheMinutes;
            if (cacheMinutes <= 0)
            {
                // Do not cache
                response.Cache.SetNoStore();
                return Guid.NewGuid().ToString();
            }
        }

        SiteNameOnDemand siteName = new SiteNameOnDemand();
        ViewModeOnDemand viewMode = new ViewModeOnDemand();

        // Parse the custom parameters
        string contextString = OutputHelper.GetContextCacheString(custom, viewMode, siteName);
        if (contextString == null)
        {
            // Do not cache
            response.Cache.SetNoStore();
            return Guid.NewGuid().ToString();
        }
        else
        {
            result = "cached" + contextString;
        }

        return result.ToLowerCSafe();
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Raised when worker queue finishes.
    /// </summary>
    /// <param name="sender">Sender</param>
    /// <param name="e">Event arguments</param>
    protected static void CMSWorkerQueue_OnFinished(object sender, EventArgs e)
    {
        CMSWorkerQueue queue = sender as CMSWorkerQueue;
        if (queue != null)
        {
            // Process internal integration tasks
            IntegrationHelper.ProcessInternalTasksAsync(queue.Parameter as List<string>);
        }
    }


    /// <summary>
    /// Rewrites the URL and performs all operations required after URL rewriting.
    /// </summary>
    /// <param name="status">Current rewriting status</param>
    /// <param name="relativePath">Relative path</param>
    /// <param name="excludedEnum">Excluded page status</param>
    /// <param name="viewMode">View mode</param>
    /// <param name="siteName">Site name</param>
    private static void RewriteUrl(RequestStatusEnum status, string relativePath, ExcludedSystemEnum excludedEnum, ViewModeOnDemand viewMode, SiteNameOnDemand siteName)
    {
        // Do the rewriting if status not yet determined
        if (status == RequestStatusEnum.Unknown)
        {
            // Rewrite URL
            status = URLRewriter.RewriteUrl(relativePath, excludedEnum, siteName, viewMode);
        }

        // Try handle combined script request for page not found
        if (status == RequestStatusEnum.PageNotFound)
        {
            ControlsHelper.ToolkitCombinedScriptHandler(HttpContext.Current);
        }

        // Process actions after rewriting
        URLRewriter.ProcessRewritingResult(status, excludedEnum, siteName, viewMode, relativePath);
    }


    /// <summary>
    /// Performs the application initialization on the first request.
    /// </summary>
    private static void FirstRequestInitialization(object sender, EventArgs e)
    {
        // PreInitialize the application
        if (!mApplicationPreInitialized)
        {
            lock (mLock)
            {
                if (!mApplicationPreInitialized)
                {
                    // Remember date and time of the application start
                    mApplicationStart = DateTime.Now;

                    // Init run from web application - DON'T MOVE LATER
                    SystemContext.IsWebSite = true;

                    mWindowsIdentity = WindowsIdentity.GetCurrent();

                    // PreInitialize the environment
                    CMSFunctions.PreInit();

                    mApplicationPreInitialized = true;
                }
            }
        }

        // Initialized properly
        bool? initialized = mApplicationInitialized.Value;
        if (initialized == true)
        {
            return;
        }

        // Not initialized, must install
        if ((initialized == false) && InstallerFunctions.InstallRedirect(true))
        {
            return;
        }

        // Do not init application on request to just physical file
        string relativePath = RequestContext.CurrentRelativePath;
        ExcludedSystemEnum excludedEnum = URLHelper.IsExcludedSystemEnum(relativePath, true);

        if ((excludedEnum == ExcludedSystemEnum.PhysicalFile && !relativePath.EndsWithCSafe(".aspx", true)) ||
            (excludedEnum == ExcludedSystemEnum.GetResource) ||
            (excludedEnum == ExcludedSystemEnum.AppThemes))
        {
            return;
        }

        // Initialize application in a locked context
        lock (mLock)
        {
            if (ApplicationInitialized)
            {
                return;
            }

            ViewModeOnDemand viewMode = new ViewModeOnDemand();

            // Log application start
            if (CMSFunctions.AnyDebugEnabled)
            {
                RequestSettings settings = RequestSettings.Current;
                bool liveSite = (viewMode.Value == ViewModeEnum.LiveSite);

                settings.DebugRequest = RequestHelper.DebugRequests && liveSite;
                RequestHelper.LogRequestOperation("BeforeApplicationStart", null, 0);

                settings.DebugSQLQueries = SqlHelperClass.DebugQueries && liveSite;
                settings.DebugFiles = File.DebugFiles && liveSite;
                settings.DebugCache = CacheHelper.DebugCache && liveSite;
                settings.DebugSecurity = SecurityHelper.DebugSecurity && liveSite;
                settings.DebugOutput = OutputHelper.DebugOutput && liveSite;
                settings.DebugMacros = MacroResolver.DebugMacros && liveSite;
                settings.DebugWebFarm = WebSyncHelperClass.DebugWebFarm && liveSite;
                settings.DebugAnalytics = AnalyticsHelper.DebugAnalytics && liveSite;

                DebugHelper.SetContext("App_Start");
            }

            // Initialize MacroResolver with child of GlobalResolver
            MacroResolver.OnGetInstance += new MacroResolver.GetInstanceEventHandler(MacroResolver_OnGetInstance);

            // Handle the event
            using (var h = CMSApplicationEvents.Start.StartEvent(e))
            {
                if (h.Continue)
                {
                    // Initialize the storage methods
                    CMSFunctions.InitStorage();

                    if (SqlHelperClass.IsConnectionStringInitialized)
                    {
                        using (CMSConnectionScope scope = new CMSConnectionScope())
                        {
                            // Use single open connection for the application start
                            GeneralConnection conn = (GeneralConnection)scope.Connection;
                            bool closeConnection = false;
                            try
                            {
                                // Open the connection
                                conn.Open();
                                closeConnection = true;

                                TableManager tm = new TableManager(null);

                                // Check for the table existence
                                if (!tm.TableExists("CMS_SettingsKey"))
                                {
                                    ApplicationInitialized = false;

                                    if (InstallerFunctions.InstallRedirect(true))
                                    {
                                        return;
                                    }
                                }

                                // Check the version
                                string version = SettingsKeyInfoProvider.GetStringValue("CMSDBVersion");
                                if (!DatabaseHelper.IsCorrectDatabaseVersion)
                                {
                                    // Report error about not being able to connect
                                    ConnectionErrorMessage = "The database version '" + version + "' does not match the project version '" + CMSContext.SYSTEM_VERSION + "', please check your connection string.";
                                    HttpContext.Current.Server.Transfer("~/CMSMessages/error.aspx");
                                }
                                else
                                {
                                    // Initialize the environment
                                    CMSFunctions.Init();

                                    // Update the system !! IMPORTANT - must be first
                                    UpgradeProcedure.Update(conn);
                                    try
                                    {
                                        // Write "Application start" event to the event log
                                        EventLogProvider ev = new EventLogProvider();

                                        ev.DeleteOlderLogs = false;
                                        ev.LogEvent(EventLogProvider.INFORMATION, DateTime.Now, "Application_Start", "STARTAPP", 0, null, 0, null, null, null, 0, HTTPHelper.GetAbsoluteUri());
                                    }
                                    catch
                                    {
                                        // can't write to log, do not process any code
                                    }
                                    UserInfoProvider.OnFormattedUserName += new UserInfoProvider.FormattedUserNameEventHandler(Functions.GetFormattedUserName);

                                    // Initialize the web farm
                                    CMSFunctions.InitWebFarm();

                                    // Handle admin emergency reset 
                                    string adminReset = ValidationHelper.GetString(SettingsHelper.AppSettings["CMSAdminEmergencyReset"], null);
                                    if (!string.IsNullOrEmpty(adminReset))
                                    {
                                        string[] resetParams = adminReset.Split(';');
                                        if ((resetParams.Length >= 1) && (resetParams.Length <= 3))
                                        {
                                            // Check if create user if she doesn't exist
                                            bool forceCreate = (resetParams.Length == 3) ? ValidationHelper.GetBoolean(resetParams[2], false) : false;
                                            string userName = resetParams[0];
                                            UserInfo ui = UserInfoProvider.GetUserInfo(userName);

                                            // Create new user
                                            if ((ui == null) && forceCreate)
                                            {
                                                if (UserInfoProvider.LicenseVersionCheck(URLHelper.GetCurrentDomain(), FeatureEnum.GlobalAdmininistrators, ObjectActionEnum.Insert, false))
                                                {
                                                    if (ValidationHelper.IsUserName(userName))
                                                    {
                                                        ui = new UserInfo();
                                                        ui.UserName = resetParams[0];
                                                        ui.UserIsGlobalAdministrator = true;
                                                        ui.UserEnabled = true;

                                                        string error = null;
                                                        UserInfoProvider.CheckLicenseLimitation(ui, ref error);
                                                        if (!string.IsNullOrEmpty(error))
                                                        {
                                                            throw new Exception(string.Format("[FirstRequestInitialization.AdminEmergencyReset: {0}]", error));
                                                        }
                                                    }
                                                    else
                                                    {
                                                        throw new Exception("[FirstRequestInitialization.AdminEmergencyReset: Specified username for newly created user is not valid.]");
                                                    }
                                                }
                                            }

                                            // Unlock account and set new specified password
                                            if (ui != null)
                                            {
                                                UserInfoProvider.SetPassword(ui, (resetParams.Length > 1) ? resetParams[1] : "", false);
                                                AuthenticationHelper.UnlockUserAccount(ui);
                                            }

                                            // Remove key from web.config
                                            SettingsHelper.RemoveConfigValue("CMSAdminEmergencyReset");
                                            URLHelper.Redirect(RequestContext.CurrentURL);
                                        }
                                    }

                                    // Wait until initialization is complete
                                    CMSFunctions.WaitForInitialization();
                                }
                            }
                            catch (Exception ex)
                            {
                                if (closeConnection)
                                {
                                    // Server connected successfully but something else went wrong
                                    throw ex;
                                }
                                else
                                {
                                    // Report error about not being able to connect
                                    ConnectionErrorMessage = ex.Message;

                                    HttpContext.Current.Server.Transfer("~/CMSMessages/error.aspx");
                                }
                            }
                            finally
                            {
                                if (closeConnection)
                                {
                                    // Close the connection
                                    conn.Close();
                                }
                            }
                        }

                        DBSeparationCheck();
                    }
                    else
                    {
                        // Register virtual path provider
                        if (ValidationHelper.GetBoolean(SettingsHelper.AppSettings["CMSUseVirtualPathProvider"], true))
                        {
                            VirtualPathHelper.RegisterVirtualPathProvider();
                        }
                    }

                    // Register the CMS view engine
                    CMSWebFormViewEngine.RegisterViewEngine();
                }

                // Finalize the event
                h.FinishEvent();
            }

            DebugHelper.ReleaseContext();

            // Log when the overall application start finished its execution
            mApplicationStartFinished = DateTime.Now;
            ApplicationInitialized = true;

            RequestHelper.LogRequestOperation("AfterApplicationStart", null, 0);
        }
    }


    /// <summary>
    /// Returns child of global resolver.
    /// </summary>
    protected static MacroResolver MacroResolver_OnGetInstance()
    {
        return ContextResolver.GetInstance();
    }


    /// <summary>
    /// Check that DB separation is successfully finished.
    /// </summary>
    private static void DBSeparationCheck()
    {
        // If DB separation was in progress then after application restart it is not in progress
        if (DatabaseSeparationHelper.SeparationInProgress)
        {
            DatabaseSeparationHelper.SeparationInProgress = false;
        }

        // Check that after DB join all web farm servers have proper connection strings set
        string sepConnString = DatabaseSeparationHelper.ConnStringSeparate;
        bool isSepConnString = !String.IsNullOrEmpty(sepConnString);

        if (isSepConnString)
        {
            // Check separated database
            if (!DatabaseSeparationHelper.CheckCMDatabase(sepConnString))
            {
                ConnectionErrorMessage = "The separated database was joined back to main database. Please remove connection string OMConnectionString from the web.config.";
                HttpContext.Current.Server.Transfer("~/CMSMessages/error.aspx");
            }
        }
        // Check that after DB separation all web farm servers have proper connection string set
        else if (!DatabaseSeparationHelper.CheckCMDatabase(null))
        {
            // Check standard database
            ConnectionErrorMessage = "The contact management database is separated. Please add connection string OMConnectionString to the web.config.";
            HttpContext.Current.Server.Transfer("~/CMSMessages/error.aspx");
        }
    }


    /// <summary>
    /// Checks the request security and path.
    /// </summary>
    private static void CheckSecurity()
    {
        // Process only for content pages
        RequestStatusEnum status = URLRewriter.CurrentStatus;


        #region "Check Banned IPs"

        switch (status)
        {
            case RequestStatusEnum.PathRewritten:
            case RequestStatusEnum.MVCPage:
            case RequestStatusEnum.GetFile:
            case RequestStatusEnum.GetProduct:
            case RequestStatusEnum.SystemPage:
            case RequestStatusEnum.TrackbackPage:
                // Check whether session is available
                if (HttpContext.Current.Session != null)
                {
                    // Get sitename
                    string siteName = SiteInfoProvider.CurrentSiteName;

                    // Process banned IPs
                    if ((!String.IsNullOrEmpty(siteName)) && BannedIPInfoProvider.IsBannedIPEnabled(siteName))
                    {
                        DateTime lastCheck = ValidationHelper.GetDateTime(SessionHelper.GetValue("CMSBannedLastCheck"), DateTimeHelper.ZERO_TIME);
                        bool banned = false;

                        // Check if there wasn't change in banned IP settings
                        if (lastCheck <= BannedIPInfoProvider.LastChange)
                        {
                            if (!BannedIPInfoProvider.IsAllowed(siteName, BanControlEnum.Complete))
                            {
                                SessionHelper.SetValue("CMSBanned", true);
                                banned = true;
                            }
                            else
                            {
                                SessionHelper.Remove("CMSBanned");
                            }

                            //Update timestamp
                            SessionHelper.SetValue("CMSBannedLastCheck", DateTime.Now);
                        }
                        else
                        {
                            banned = (SessionHelper.GetValue("CMSBanned") != null);
                        }

                        // Check if this session was banned
                        if (banned)
                        {
                            BannedIPInfoProvider.BanRedirect(siteName);
                        }
                    }
                }
                break;
        }

        #endregion


        if ((status == RequestStatusEnum.PathRewritten) ||
            (status == RequestStatusEnum.MVCPage))
        {
            // Check page security
            if (HttpContext.Current.Session != null)
            {
                string siteName = SiteInfoProvider.CurrentSiteName;

                string relativePath = RequestContext.CurrentRelativePath;

                // Do not use security check for full page cache pages
                if (!OutputFilterContext.OutputFilterEndRequestRequired)
                {
                    PageInfo currentPageInfo = DocumentContext.CurrentPageInfo;
                    ViewModeEnum viewMode = PortalContext.ViewMode;

                    // Check view mode permissions
                    PortalHelper.CheckViewModePermissions(currentPageInfo, viewMode);


                    #region "Check path"

                    string aliasPath = DocumentContext.CurrentAliasPath;

                    if ((currentPageInfo != null) && (aliasPath != currentPageInfo.NodeAliasPath))
                    {
                        // Set alias path to root if current page info is root page info
                        if (currentPageInfo.NodeAliasPath == "/")
                        {
                            DocumentContext.CurrentAliasPath = "/";
                        }
                        // Display nothing if current alias path is not equal to page info alias path
                        else
                        {
                            DocumentContext.CurrentPageInfo = null;
                            currentPageInfo = DocumentContext.CurrentPageInfo;
                        }
                    }

                    if (currentPageInfo != null)
                    {
                        // Check preview link context
                        CheckPreviewLink(currentPageInfo, viewMode, true);

                        // Check SSL Require
                        URLRewriter.RequestSecurePage(currentPageInfo, true, viewMode, siteName);

                        // Check secured areas
                        URLRewriter.CheckSecuredAreas(siteName, currentPageInfo, true, viewMode);

                        // Check permissions
                        URLRewriter.CheckPermissions(siteName, currentPageInfo, true, relativePath);
                    }

                    // Check default alias path
                    if ((aliasPath == "/") && (viewMode == ViewModeEnum.LiveSite))
                    {
                        string defaultAliasPath = SettingsKeyInfoProvider.GetStringValue(siteName + ".CMSDefaultAliasPath");
                        string lowerDefaultAliasPath = defaultAliasPath.ToLowerCSafe();
                        if ((defaultAliasPath != "") && (lowerDefaultAliasPath != aliasPath.ToLowerCSafe()))
                        {
                            if (lowerDefaultAliasPath == "/default")
                            {
                                // Special case - default
                                DocumentContext.CurrentAliasPath = defaultAliasPath;
                            }
                            else
                            {
                                // Redirect to the new path
                                URLHelper.Redirect(DocumentURLProvider.GetUrl(defaultAliasPath));
                            }
                        }
                    }

                    #endregion
                }

                // Update current session
                if (SessionManager.OnlineUsersEnabled && !URLHelper.IsExcludedSystem(relativePath))
                {
                    SessionManager.UpdateCurrentSession(siteName);
                }
            }


            // Extend the expiration of the authentication cookie if required
            if (!AuthenticationHelper.UseSessionCookies && (HttpContext.Current != null) && (HttpContext.Current.Session != null))
            {
                CookieHelper.ChangeCookieExpiration(FormsAuthentication.FormsCookieName, DateTime.Now.AddMinutes(HttpContext.Current.Session.Timeout), true);
            }
        }
        else
        {
            // Check other pages security
            if (HttpContext.Current.Session != null)
            {
                string siteName = SiteInfoProvider.CurrentSiteName;

                // Do not use security check for full page cache pages
                if (!OutputFilterContext.OutputFilterEndRequestRequired)
                {
                    // Check preview link context
                    if (VirtualContext.IsPreviewLinkInitialized)
                    {
                        // Validate page info
                        ViewModeEnum viewMode = PortalContext.ViewMode;
                        Guid previewGuid = ValidationHelper.GetGuid(VirtualContext.GetItem(VirtualContext.PARAM_WF_GUID), Guid.Empty);
                        DataSet ds = PageInfoProvider.GetPageInfos("DocumentWorkflowCycleGUID='" + previewGuid.ToString() + "'", null, 1, null);
                        if (!DataHelper.DataSourceIsEmpty(ds))
                        {
                            PageInfo pageInfo = new PageInfo(ds.Tables[0].Rows[0]);
                            CheckPreviewLink(pageInfo, viewMode, false);
                        }
                        else
                        {
                            // Reset the virtual context
                            VirtualContext.Reset();

                            // GUID values don't match
                            URLHelper.Redirect(UIHelper.GetAccessDeniedUrl("virtualcontext.previewlink"));
                        }
                    }
                }
            }
        }
    }


    /// <summary>
    /// Check preview link context
    /// </summary>
    /// <param name="pageInfo">Page info</param>
    /// <param name="viewMode">View mode</param>
    /// <param name="documentUrl">Indicates if document URL should be checked</param>
    private static void CheckPreviewLink(PageInfo pageInfo, ViewModeEnum viewMode, bool documentUrl)
    {
        if (VirtualContext.IsPreviewLinkInitialized)
        {
            if (pageInfo != null)
            {
                Guid previewGuid = ValidationHelper.GetGuid(VirtualContext.GetItem(VirtualContext.PARAM_WF_GUID), Guid.Empty);
                if (previewGuid != Guid.Empty)
                {
                    // Force preview mode
                    if ((viewMode == ViewModeEnum.Preview) || (viewMode == ViewModeEnum.LiveSite))
                    {
                        // Preview link is valid
                        if (pageInfo.DocumentWorkflowCycleGUID == previewGuid)
                        {
                            if (documentUrl)
                            {
                                return;
                            }
                            // Additional check for links within the document
                            else if (VirtualContext.ValidatePreviewHash(RequestContext.CurrentRelativePath))
                            {
                                return;
                            }
                        }

                        // Reset the virtual context
                        VirtualContext.Reset();

                        // GUID values don't match
                        URLHelper.Redirect(UIHelper.GetAccessDeniedUrl("virtualcontext.accessdenied"));
                    }
                }
            }
        }
    }


    /// <summary>
    /// Updates contact's IP and UserAgent information about visitor.
    /// </summary>
    /// <param name="siteName">Site name</param>
    private static void UpdateContactsIPandUserAgent(string siteName)
    {
        if (SettingsKeyInfoProvider.GetBoolValue(siteName + ".CMSEnableOnlineMarketing"))
        {
            ModuleCommands.OnlineMarketingUpdateContactInformation(siteName);
        }
    }


    /// <summary>
    /// Attempts to run the scheduler request.
    /// </summary>
    /// <param name="status">Current status</param>
    private static void RunScheduler(RequestStatusEnum status)
    {
        // Scheduler is disabled
        if (!SchedulingHelper.EnableScheduler)
        {
            return;
        }

        // Ensure the rewriting status
        if (status == RequestStatusEnum.Unknown)
        {
            status = URLRewriter.CurrentStatus;
        }

        // Process scheduler only on content or system pages
        switch (status)
        {
            case RequestStatusEnum.PathRewritten:
            case RequestStatusEnum.MVCPage:
            case RequestStatusEnum.SystemPage:
            case RequestStatusEnum.SentFromCache:
                // Run scheduler - Do not run on first request to provide faster application start
                {
                    string siteName = SchedulingTimer.SchedulerRunImmediatelySiteName;
                    if (SchedulingHelper.UseAutomaticScheduler)
                    {
                        // Ensure the active timer running in an asynchronous thread
                        SchedulingTimer timer = SchedulingTimer.EnsureTimer(siteName, true);
                        if ((timer != null) && SchedulingTimer.RunSchedulerImmediately)
                        {
                            timer.ExecuteAsync();
                        }
                    }
                    else
                    {
                        // --- Default scheduler settings
                        // If scheduler run request acquired, run the actions
                        bool runScheduler = SchedulingTimer.RequestRun(siteName) || SchedulingTimer.RunSchedulerImmediately;
                        if (runScheduler)
                        {
                            if (SchedulingHelper.RunSchedulerWithinRequest)
                            {
                                // --- Default scheduler settings
                                try
                                {
                                    try
                                    {
                                        // Flush the output
                                        HttpContext.Current.Response.Flush();
                                    }
                                    // Do not display closed host exception
                                    catch
                                    {
                                    }

                                    // Run scheduler actively within the request                                    
                                    SchedulingExecutor.ExecuteScheduledTasks(siteName, SystemContext.ServerName);
                                }
                                catch (Exception ex)
                                {
                                    EventLogProvider.LogException("Scheduler", "ExecuteScheduledTasks", ex);

                                    SchedulingExecutor.ReInitTasks = true;
                                }
                            }
                            else
                            {
                                // Get passive timer and execute
                                SchedulingTimer timer = SchedulingTimer.EnsureTimer(siteName, false);
                                if (timer != null)
                                {
                                    timer.ExecuteAsync();
                                }
                            }
                        }
                    }
                }
                break;
        }
    }


    /// <summary>
    /// Ensures performance counter timer.
    /// </summary>
    private static void EnsurePerformanceCounterTimer()
    {
        // Health monitoring is enabled
        if (HealthMonitoringHelper.LogCounters)
        {
            // Get passive timer and execute
            PerformanceCounterTimer timer = PerformanceCounterTimer.EnsureTimer();
            timer.EnsureRunTimerAsync();
        }
    }


    /// <summary>
    /// Logs the last application error.
    /// </summary>
    private static void LogLastApplicationError()
    {
        if (ConnectionAvailable)
        {
            if ((HttpContext.Current != null) && RequestContext.LogCurrentError)
            {
                Exception ex = HttpContext.Current.Server.GetLastError();
                if (ex != null)
                {
                    string eventCode = "EXCEPTION";
                    string eventType = EventLogProvider.ERROR;

                    // Log request operation
                    RequestHelper.LogRequestOperation("OnError", ex.Message, 0);

                    bool log = true;

                    // Page not found was already manually logged
                    if (URLRewriter.CurrentStatus == RequestStatusEnum.PageNotFound)
                    {
                        log = false;
                    }

                    // Do not log page not found http exception... it's handled automatically
                    HttpException httpException = ex as HttpException;
                    if ((httpException != null) && (httpException.GetHttpCode() == 404))
                    {
                        log = false;
                    }

                    if (log)
                    {
                        // Impersonation context
                        WindowsImpersonationContext ctx = null;

                        try
                        {
                            // Impersonate current thread
                            ctx = mWindowsIdentity.Impersonate();

                            // Initiate the event
                            bool logException = true;
                            SystemEvents.Exception.StartEvent(ex, ref logException);

                            if (logException)
                            {
                                // Get the lowest exception
                                while (ex.InnerException != null)
                                {
                                    ex = ex.InnerException;
                                }

                                // Write error to Event log
                                try
                                {
                                    EventLogProvider eProvider = new EventLogProvider();
                                    eProvider.LogEvent(eventType, DateTime.Now, "Application_Error", eventCode, MembershipContext.AuthenticatedUser.UserID, MembershipContext.AuthenticatedUser.UserName, (DocumentContext.CurrentDocument != null) ? DocumentContext.CurrentDocument.NodeID : 0, (DocumentContext.CurrentDocument != null) ? DocumentContext.CurrentDocument.GetDocumentName() : null, RequestContext.UserHostAddress, EventLogProvider.GetExceptionLogMessage(ex), CMSContext.CurrentSiteID, HTTPHelper.GetAbsoluteUri());
                                }
                                catch
                                {
                                    // can't write to log, do not process any code
                                }
                            }
                        }
                        finally
                        {
                            if (ctx != null)
                            {
                                ctx.Undo();
                            }
                        }
                    }
                }
            }
        }
    }


    /// <summary>
    /// Logs the application end.
    /// </summary>
    private static void LogApplicationEnd()
    {
        EventLogProvider eventLog = new EventLogProvider();

        // Get the shutdown reason
        HttpRuntime runtime = (HttpRuntime)typeof(HttpRuntime).InvokeMember("_theRuntime", BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.GetField, null, null, null);
        if (runtime != null)
        {
            string shutDownMessage = Convert.ToString(runtime.GetType().InvokeMember("_shutDownMessage", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.GetField, null, runtime, null));
            string shutDownStack = Convert.ToString(runtime.GetType().InvokeMember("_shutDownStack", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.GetField, null, runtime, null));

            StackTrace stack = new StackTrace();

            string callStack = stack.ToString();
            string logMessage = "Message: " + shutDownMessage + "<br />\nShutdown stack: " + shutDownStack + "<br />\nCall stack: " + callStack;

            eventLog.LogEvent(EventLogProvider.WARNING, DateTime.Now, "Application_End", "ENDAPP", 0, HTTPHelper.GetUserName(), 0, "", null, logMessage, 0, null);
        }
        else
        {
            eventLog.LogEvent(EventLogProvider.WARNING, DateTime.Now, "Application_End", "ENDAPP", 0, HTTPHelper.GetUserName(), 0, "", null, "", 0, null);
        }
    }


    /// <summary>
    /// Sets the initial debugging settings.
    /// </summary>
    private static void SetInitialDebug()
    {
        if (CMSFunctions.AnyDebugEnabled)
        {
            // Prepare the context values
            ViewModeOnDemand viewMode = new ViewModeOnDemand();
            RequestSettingsOnDemand settings = new RequestSettingsOnDemand();

            bool isLiveSite = (viewMode.Value == ViewModeEnum.LiveSite);

            // Set request debugging
            if (RequestHelper.DebugRequests)
            {
                settings.Value.DebugRequest = RequestHelper.DebugAllRequests || isLiveSite;
                RequestHelper.LogRequestOperation("BeginRequest", null, 0);
            }
            if (SqlHelperClass.DebugQueries)
            {
                settings.Value.DebugSQLQueries = SqlHelperClass.DebugAllQueries || isLiveSite;
            }
            if (CacheHelper.DebugCache)
            {
                settings.Value.DebugCache = CacheHelper.DebugAllCaches || isLiveSite;
            }
            if (SecurityHelper.DebugSecurity)
            {
                settings.Value.DebugSecurity = SecurityHelper.DebugAllSecurity || isLiveSite;
            }
            if (File.DebugFiles)
            {
                settings.Value.DebugFiles = File.DebugAllFiles || isLiveSite;
            }
            if (MacroResolver.DebugMacros)
            {
                settings.Value.DebugMacros = MacroResolver.DebugAllMacros || isLiveSite;
            }
            if (OutputHelper.DebugOutput)
            {
                settings.Value.DebugOutput = OutputHelper.DebugAllOutputs || isLiveSite;
            }
            if (WebSyncHelperClass.DebugWebFarm)
            {
                settings.Value.DebugWebFarm = WebSyncHelperClass.DebugAllWebFarm || isLiveSite;
            }
            if (AnalyticsHelper.DebugAnalytics)
            {
                settings.Value.DebugAnalytics = AnalyticsHelper.DebugAllAnalytics || isLiveSite;
            }
        }
    }


    /// <summary>
    /// Check URL query string for authentication parameter and authenticate user.
    /// </summary>
    private static void CheckAuthenticationGUID()
    {
        // Check for authentication token
        if (QueryHelper.Contains("authenticationGuid") && SettingsKeyInfoProvider.GetBoolValue("CMSAutomaticallySignInUser"))
        {
            UserInfo ui = null;

            if (!AuthenticationHelper.IsAuthenticated())
            {
                // Get authentication token
                Guid authGuid = QueryHelper.GetGuid("authenticationGuid", Guid.Empty);
                if (authGuid != Guid.Empty)
                {
                    // Get users with found authentication token
                    DataSet ds = UserInfoProvider.GetFullUsers("UserAuthenticationGUID = '" + authGuid + "'", null, 1, null);
                    if (!DataHelper.DataSourceIsEmpty(ds))
                    {
                        // Authenticate user
                        ui = new UserInfo(ds.Tables[0].Rows[0]);
                        CMSContext.AuthenticateUser(ui.UserName, false, false);
                    }
                }
            }
            else
            {
                // Get current user info
                ui = MembershipContext.AuthenticatedUser;
            }

            // Remove authentication GUID
            if ((ui != null) && (ui.UserAuthenticationGUID != Guid.Empty))
            {
                using (CMSActionContext context = new CMSActionContext())
                {
                    context.DisableAll();
                    context.CreateSearchTask = false;

                    ui.UserAuthenticationGUID = Guid.Empty;
                    ui.Generalized.SetObject();
                }
            }

            // Redirect to URL without authentication token
            URLHelper.Redirect(URLHelper.RemoveParameterFromUrl(RequestContext.CurrentURL, "authenticationGuid"));
        }
    }


    /// <summary>
    /// Redirects the file to the images folder.
    /// </summary>
    protected static bool FileRedirect()
    {
        string cmsimg = QueryHelper.GetString("cmsimg", null);
        if ((cmsimg != null) && cmsimg.StartsWithCSafe("/"))
        {
            if (cmsimg.StartsWithCSafe(UIHelper.UNIGRID_ICONS))
            {
                // Unigrid actions
                cmsimg = "Design/Controls/UniGrid/Actions" + cmsimg.Substring(3);
            }
            else if (cmsimg.StartsWithCSafe(UIHelper.TREE_ICONS))
            {
                // Tree icons
                cmsimg = "Design/Controls/Tree" + cmsimg.Substring(2);
            }
            else if (cmsimg.StartsWithCSafe(UIHelper.TREE_ICONS_RTL))
            {
                // Tree icons RTL
                cmsimg = "RTL/Design/Controls/Tree" + cmsimg.Substring(3);
            }
            else if (cmsimg.StartsWithCSafe(UIHelper.FLAG_ICONS))
            {
                // Flag icons
                cmsimg = "Flags/16x16" + cmsimg.Substring(2);
            }
            else if (cmsimg.StartsWithCSafe(UIHelper.FLAG_ICONS_48))
            {
                // Large flag icons
                cmsimg = "Flags/48x48" + cmsimg.Substring(4);
            }

            // Redirect to the correct location
            URLHelper.PermanentRedirect(UIHelper.GetImageUrl(null, cmsimg));

            return true;
        }

        return false;
    }


    /// <summary>
    /// Reinitializes the application by resetting the application variables.
    /// </summary>
    public static void ReInit()
    {
        mApplicationStart = DateTime.Now;
        mApplicationStartFinished = DateTime.MinValue;

        firstEndRequestAfterStart = true;

        mApplicationInitialized.Value = null;

        ConnectionErrorMessage = null;
    }

    #endregion
}
