using System;
using System.Collections;

using CMS.CMSHelper;
using CMS.Controls;
using CMS.FormEngine;
using CMS.GlobalHelper;
using CMS.LicenseProvider;
using CMS.PortalControls;
using CMS.SettingsProvider;
using CMS.UIControls;
using CMS.URLRewritingEngine;
using CMS.ExtendedControls;
using CMS.Helpers;
using CMS.Membership;
using CMS.DataEngine;
using CMS.SiteProvider;
using CMS.Core;
using CMS.DocumentEngine;

public partial class CMSWebParts_Membership_Profile_MyAccount_Servranx : CMSAbstractWebPart
{
    #region "Protected variables & constants"

    protected string page = string.Empty;

    protected const string personalTab = "personal";
    protected const string detailsTab = "details";
    protected const string addressesTab = "addresses";
    protected const string ordersTab = "orders";
    protected const string creditTab = "credit";
    protected const string passwordTab = "password";
    protected const string subscriptionsTab = "subscriptions";
    protected const string notificationsTab = "notifications";
    protected const string messagesTab = "messages";
    protected const string friendsTab = "friends";
    protected const string membershipsTab = "memberships";
    protected const string categoriesTab = "categories";

    #endregion


    #region "User Controls"

    private CMSAdminControl ucMyNotifications = null;
    private CMSAdminControl ucMyFriends = null;
    private CMSAdminControl ucMyCredit = null;
    private CMSAdminControl ucMyDetails = null;
    private CMSAdminControl ucMyOrders = null;
    private CMSAdminControl ucMyAddresses = null;
    private CMSAdminControl ucMyMessages = null;
    private CMSAdminControl ucMyAllSubscriptions = null;
    private CMSAdminControl ucMyMemberships = null;
    private CMSAdminControl ucMyCategories = null;

    #endregion


    #region "Public properties"

    /// <summary>
    /// Indicates whether blog post subscriptions are shown.
    /// </summary>
    public bool DisplayBlogs
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("DisplayBlogPostsSubscriptions"), true);
        }
        set
        {
            SetValue("DisplayBlogPostsSubscriptions", value);
        }
    }


    /// <summary>
    /// Indicates whether message boards subscriptions are shown.
    /// </summary>
    public bool DisplayMessageBoards
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("DisplayMessageBoardsSubscriptions"), true);
        }
        set
        {
            SetValue("DisplayMessageBoardsSubscriptions", value);
        }
    }


    /// <summary>
    /// Indicates whether newsletters subscriptions are shown.
    /// </summary>
    public bool DisplayNewsletters
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("DisplayNewslettersSubscriptions"), true);
        }
        set
        {
            SetValue("DisplayNewslettersSubscriptions", value);
        }
    }


    /// <summary>
    /// Indicates whether forum subscriptions are shown.
    /// </summary>
    public bool DisplayForums
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("DisplayForumsSubscriptions"), true);
        }
        set
        {
            SetValue("DisplayForumsSubscriptions", value);
        }
    }


    /// <summary>
    /// Indicates whether report subscription are shown.
    /// </summary>
    public bool DisplayReports
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("DisplayReportsSubscriptions"), true);
        }
        set
        {
            SetValue("DisplayReportsSubscriptions", value);
        }
    }


    /// <summary>
    /// If true, notification emails are sent.
    /// </summary>
    public bool SendConfirmationEmails
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("SendConfirmationEmails"), true);
        }
        set
        {
            SetValue("SendConfirmationEmails", value);
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether 'my details' is displayed.
    /// </summary>
    public bool DisplayMyDetails
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("DisplayMyDetails"), true);
        }
        set
        {
            SetValue("DisplayMyDetails", value);
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether 'my addresses' is displayed.
    /// </summary>
    public bool DisplayMyAddresses
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("DisplayMyAddresses"), true);
        }
        set
        {
            SetValue("DisplayMyAddresses", value);
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether 'my orders' is displayed.
    /// </summary>
    public bool DisplayMyOrders
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("DisplayMyOrders"), true);
        }
        set
        {
            SetValue("DisplayMyOrders", value);
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether 'credit' is displayed.
    /// </summary>
    public bool DisplayMyCredits
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("DisplayMyCredits"), true);
        }
        set
        {
            SetValue("DisplayMyCredits", value);
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether 'change password' is displayed.
    /// </summary>
    public bool DisplayChangePassword
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("DisplayChangePassword"), true);
        }
        set
        {
            SetValue("DisplayChangePassword", value);
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether 'my subscriptions' is displayed.
    /// </summary>
    public bool DisplayMySubscriptions
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("DisplayMySubscriptions"), true);
        }
        set
        {
            SetValue("DisplayMySubscriptions", value);
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether 'my subscriptions' is displayed.
    /// </summary>
    public bool DisplayMyNotifications
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("DisplayMyNotifications"), true);
        }
        set
        {
            SetValue("DisplayMyNotifications", value);
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether 'messages' is displayed.
    /// </summary>
    public bool DisplayMyMessages
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("DisplayMyMessages"), true);
        }
        set
        {
            SetValue("DisplayMyMessages", value);
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether 'friends' is displayed.
    /// </summary>
    public bool DisplayMyFriends
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("DisplayMyFriends"), true);
        }
        set
        {
            SetValue("DisplayMyFriends", value);
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether 'personal settings' is displayed.
    /// </summary>
    public bool DisplayMyPersonalSettings
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("DisplayMyPersonalSettings"), true);
        }
        set
        {
            SetValue("DisplayMyPersonalSettings", value);
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether 'memberships' is displayed.
    /// </summary>
    public bool DisplayMyMemberships
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("DisplayMyMemberships"), false);
        }
        set
        {
            SetValue("DisplayMyMemberships", value);
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether 'categories' is displayed.
    /// </summary>
    public bool DisplayMyCategories
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("DisplayMyCategories"), false);
        }
        set
        {
            SetValue("DisplayMyCategories", value);
        }
    }


    /// <summary>
    /// Gets or sets the path of the page where memberships can be bought.
    /// </summary>
    public string MembershipsPagePath
    {
        get
        {
            return ValidationHelper.GetString(GetValue("MembershipsPagePath"), null);
        }
        set
        {
            SetValue("MembershipsPagePath", value);
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether 'order tracking number' should be displayed.
    /// </summary>
    public bool ShowOrderTrackingNumber
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("ShowOrderTrackingNumber"), false);
        }
        set
        {
            SetValue("ShowOrderTrackingNumber", value);
        }
    }


    /// <summary>
    /// Gets or sets the path to UniGrid image directory.
    /// </summary>
    public string UnigridImageDirectory
    {
        get
        {
            return ValidationHelper.GetString(GetValue("UnigridImageDirectory"), null);
        }
        set
        {
            SetValue("UnigridImageDirectory", value);
        }
    }


    /// <summary>
    /// Gets or sets the WebPart CSS class value.
    /// </summary>
    public override string CssClass
    {
        get
        {
            return base.CssClass;
        }
        set
        {
            base.CssClass = value;
            pnlBody.CssClass = value;
        }
    }


    /// <summary>
    /// Gets or sets the query string parameter name.
    /// </summary>
    public string ParameterName
    {
        get
        {
            return ValidationHelper.GetString(GetValue("ParameterName"), "page");
        }
        set
        {
            SetValue("ParameterName", value);
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether to allow to save empty password.
    /// </summary>
    public bool AllowEmptyPassword
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("AllowEmptyPassword"), false);
        }
        set
        {
            SetValue("AllowEmptyPassword", value);
        }
    }


    /// <summary>
    /// Gets or sets layout of the tab menu (Horizontal or Vertical).
    /// </summary>
    public string TabControlLayout
    {
        get
        {
            return ValidationHelper.GetString(GetValue("TabControlLayout"), "");
        }
        set
        {
            SetValue("TabControlLayout", value);
        }
    }


    /// <summary>
    /// Gets or sets the name of alternative form which would be used for 'My details' form
    /// Default values is cms.user.EditProfile
    /// </summary>
    public string AlternativeFormName
    {
        get
        {
            return DataHelper.GetNotEmpty(GetValue("AlternativeFormName"), "cms.user.EditProfile");
        }
        set
        {
            SetValue("AlternativeFormName", value);
            myProfile.AlternativeFormName = value;
        }
    }


    /// <summary>
    /// Indicates if field visibility could be edited on 'My details' form.
    /// </summary>
    public bool AllowEditVisibility
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("AllowEditVisibility"), myProfile.AllowEditVisibility);
        }
        set
        {
            SetValue("AllowEditVisibility", value);
            myProfile.AllowEditVisibility = value;
        }
    }

    #endregion


    /// <summary>
    /// Content loaded event handler.
    /// </summary>
    public override void OnContentLoaded()
    {
        base.OnContentLoaded();
        SetupControl();
    }


    /// <summary>
    /// Initializes the control properties.
    /// </summary>
    protected void SetupControl()
    {
        if (StopProcessing)
        {
            // Do nothing
        }
        else
        {
            plcOther.Controls.Clear();

            if (MembershipContext.AuthenticatedUser.IsAuthenticated())
            {
                // Set the layout of tab menu                
                tabMenu.TabControlLayout = BasicTabControl.GetTabMenuLayout(TabControlLayout);

                // Remove 'saved' parameter from query string
                string absoluteUri = URLHelper.RemoveParameterFromUrl(URLRewriter.CurrentURL, "saved");

                CurrentUserInfo currentUser = MembershipContext.AuthenticatedUser;

                // Get customer info
                GeneralizedInfo customer = ModuleCommands.ECommerceGetCustomerInfoByUserId(currentUser.UserID);
                bool userIsCustomer = (customer != null);

                // Get friends enabled setting
                bool friendsEnabled = UIHelper.IsFriendsModuleEnabled(SiteContext.CurrentSiteName);

                // Get customer ID
                int customerId = 0;
                if (userIsCustomer)
                {
                    customerId = ValidationHelper.GetInteger(customer.ObjectID, 0);
                }

                // Selected page URL
                string selectedPage = string.Empty;

                // Menu initialization
                tabMenu.UrlTarget = "_self";
                ArrayList activeTabs = new ArrayList();
                string tabName = string.Empty;

                // Set array size of tab control
                int arraySize = 0;
                if (DisplayMyPersonalSettings)
                {
                    arraySize++;
                }
                if (DisplayMyMessages)
                {
                    arraySize++;
                }

                // Handle 'Notifications' tab displaying
                bool hideUnavailableUI = SettingsKeyInfoProvider.GetBoolValue("CMSHideUnavailableUserInterface");
                bool showNotificationsTab = (DisplayMyNotifications && ModuleEntryManager.IsModuleLoaded(ModuleEntry.NOTIFICATIONS) && (!hideUnavailableUI || LicenseHelper.CheckFeature(URLHelper.GetCurrentDomain(), FeatureEnum.Notifications)));
                bool isWindowsAuthentication = RequestHelper.IsWindowsAuthentication();
               
                // Notification tab
                if (showNotificationsTab)
                {
                    arraySize++;
                }

                // Friends tab
                if (DisplayMyFriends && friendsEnabled)
                {
                    arraySize++;
                }
                
                // User password tab
                if (DisplayChangePassword && !currentUser.IsExternal && !isWindowsAuthentication)
                {
                    arraySize++;
                }

                // Subscriptions tab
                if (DisplayMySubscriptions)
                {
                    arraySize++;
                }

                // Memberships tab
                if (DisplayMyMemberships)
                {
                    arraySize++;
                }

                // Categories tab
                if (DisplayMyCategories)
                {
                    arraySize++;
                }

                // Ecommerce tabs
                if (DisplayMyCredits && userIsCustomer)
                {
                    arraySize++;
                }
                if (DisplayMyDetails && userIsCustomer)
                {
                    arraySize++;
                }
                if (DisplayMyAddresses && userIsCustomer)
                {
                    arraySize++;
                }
                if (DisplayMyOrders && userIsCustomer)
                {
                    arraySize++;
                }
                tabMenu.Tabs = new string[arraySize,5];

                // Personal tab
                if (DisplayMyPersonalSettings)
                {
                    // Set new tab
                    tabName = personalTab;
                    activeTabs.Add(tabName);
                    tabMenu.Tabs[activeTabs.IndexOf(tabName), 0] = GetString("MyAccount.MyPersonalSettings");
                    tabMenu.Tabs[activeTabs.IndexOf(tabName), 2] = HTMLHelper.HTMLEncode(URLHelper.AddParameterToUrl(absoluteUri, ParameterName, personalTab));

                    if (currentUser != null)
                    {
                        selectedPage = tabName;
                    }
                }

                // These items can be displayed only for customer
                if (userIsCustomer && ModuleEntryManager.IsModuleLoaded(ModuleEntry.ECOMMERCE))
                {
                    if (DisplayMyDetails)
                    {
                        // Try to load the control dynamically (if available)
                        ucMyDetails = Page.LoadUserControl("~/CMSModules/Ecommerce/Controls/MyDetails/MyDetails.ascx") as CMSAdminControl;
                        if (ucMyDetails != null)
                        {
                            ucMyDetails.ID = "ucMyDetails";
                            plcOther.Controls.Add(ucMyDetails);

                            // Set new tab
                            tabName = detailsTab;
                            activeTabs.Add(tabName);
                            tabMenu.Tabs[activeTabs.IndexOf(tabName), 0] = GetString("MyAccount.MyDetails");
                            tabMenu.Tabs[activeTabs.IndexOf(tabName), 2] = HTMLHelper.HTMLEncode(URLHelper.AddParameterToUrl(absoluteUri, ParameterName, detailsTab));

                            if (selectedPage == string.Empty)
                            {
                                selectedPage = tabName;
                            }
                        }
                    }

                    if (DisplayMyAddresses)
                    {
                        // Try to load the control dynamically (if available)
                        ucMyAddresses = Page.LoadUserControl("~/CMSModules/Ecommerce/Controls/MyDetails/MyAddresses.ascx") as CMSAdminControl;
                        if (ucMyAddresses != null)
                        {
                            ucMyAddresses.ID = "ucMyAddresses";
                            plcOther.Controls.Add(ucMyAddresses);

                            // Set new tab
                            tabName = addressesTab;
                            activeTabs.Add(tabName);
                            tabMenu.Tabs[activeTabs.IndexOf(tabName), 0] = GetString("MyAccount.MyAddresses");
                            tabMenu.Tabs[activeTabs.IndexOf(tabName), 2] = HTMLHelper.HTMLEncode(URLHelper.AddParameterToUrl(absoluteUri, ParameterName, addressesTab));

                            if (selectedPage == string.Empty)
                            {
                                selectedPage = tabName;
                            }
                        }
                    }

                    if (DisplayMyOrders)
                    {
                        // Try to load the control dynamically (if available)
                        ucMyOrders = Page.LoadUserControl("~/CMSModules/Ecommerce/Controls/MyDetails/MyOrders.ascx") as CMSAdminControl;
                        if (ucMyOrders != null)
                        {
                            ucMyOrders.ID = "ucMyOrders";
                            plcOther.Controls.Add(ucMyOrders);

                            // Set new tab
                            tabName = ordersTab;
                            activeTabs.Add(tabName);
                            tabMenu.Tabs[activeTabs.IndexOf(tabName), 0] = GetString("MyAccount.MyOrders");
                            tabMenu.Tabs[activeTabs.IndexOf(tabName), 2] = HTMLHelper.HTMLEncode(URLHelper.AddParameterToUrl(absoluteUri, ParameterName, ordersTab));

                            if (selectedPage == string.Empty)
                            {
                                selectedPage = tabName;
                            }
                        }
                    }

                    if (DisplayMyCredits)
                    {
                        // Try to load the control dynamically (if available)
                        ucMyCredit = Page.LoadUserControl("~/CMSModules/Ecommerce/Controls/MyDetails/MyCredit.ascx") as CMSAdminControl;
                        if (ucMyCredit != null)
                        {
                            ucMyCredit.ID = "ucMyCredit";
                            plcOther.Controls.Add(ucMyCredit);

                            // Set new tab
                            tabName = creditTab;
                            activeTabs.Add(tabName);
                            tabMenu.Tabs[activeTabs.IndexOf(tabName), 0] = GetString("MyAccount.MyCredit");
                            tabMenu.Tabs[activeTabs.IndexOf(tabName), 2] = HTMLHelper.HTMLEncode(URLHelper.AddParameterToUrl(absoluteUri, ParameterName, creditTab));

                            if (selectedPage == string.Empty)
                            {
                                selectedPage = tabName;
                            }
                        }
                    }
                }

                if (DisplayChangePassword && !currentUser.IsExternal && !isWindowsAuthentication)
                {
                    // Set new tab
                    tabName = passwordTab;
                    activeTabs.Add(tabName);
                    tabMenu.Tabs[activeTabs.IndexOf(tabName), 0] = GetString("MyAccount.ChangePassword");
                    tabMenu.Tabs[activeTabs.IndexOf(tabName), 2] = HTMLHelper.HTMLEncode(URLHelper.AddParameterToUrl(absoluteUri, ParameterName, passwordTab));

                    if (selectedPage == string.Empty)
                    {
                        selectedPage = tabName;
                    }
                }

                if ((ucMyNotifications == null) && showNotificationsTab)
                {
                    // Try to load the control dynamically (if available)
                    ucMyNotifications = Page.LoadUserControl("~/CMSModules/Notifications/Controls/UserNotifications.ascx") as CMSAdminControl;
                    if (ucMyNotifications != null)
                    {
                        ucMyNotifications.ID = "ucMyNotifications";
                        plcOther.Controls.Add(ucMyNotifications);

                        // Set new tab
                        tabName = notificationsTab;
                        activeTabs.Add(tabName);
                        tabMenu.Tabs[activeTabs.IndexOf(tabName), 0] = GetString("MyAccount.MyNotifications");
                        tabMenu.Tabs[activeTabs.IndexOf(tabName), 2] = HTMLHelper.HTMLEncode(URLHelper.AddParameterToUrl(absoluteUri, ParameterName, notificationsTab));

                        if (selectedPage == string.Empty)
                        {
                            selectedPage = tabName;
                        }
                    }
                }

                if ((ucMyMessages == null) && DisplayMyMessages && ModuleEntryManager.IsModuleLoaded(ModuleEntry.MESSAGING))
                {
                    // Try to load the control dynamically (if available)
                    ucMyMessages = Page.LoadUserControl("~/CMSModules/Messaging/Controls/MyMessages.ascx") as CMSAdminControl;
                    if (ucMyMessages != null)
                    {
                        ucMyMessages.ID = "ucMyMessages";
                        plcOther.Controls.Add(ucMyMessages);

                        // Set new tab
                        tabName = messagesTab;
                        activeTabs.Add(tabName);
                        tabMenu.Tabs[activeTabs.IndexOf(tabName), 0] = GetString("MyAccount.MyMessages");
                        tabMenu.Tabs[activeTabs.IndexOf(tabName), 2] = HTMLHelper.HTMLEncode(URLHelper.AddParameterToUrl(absoluteUri, ParameterName, messagesTab));

                        if (selectedPage == string.Empty)
                        {
                            selectedPage = tabName;
                        }
                    }
                }

                if ((ucMyFriends == null) && DisplayMyFriends && ModuleEntryManager.IsModuleLoaded(ModuleEntry.COMMUNITY) && friendsEnabled)
                {
                    // Try to load the control dynamically (if available)
                    ucMyFriends = Page.LoadUserControl("~/CMSModules/Friends/Controls/MyFriends.ascx") as CMSAdminControl;
                    if (ucMyFriends != null)
                    {
                        ucMyFriends.ID = "ucMyFriends";
                        plcOther.Controls.Add(ucMyFriends);
                        
                        // Set new tab
                        tabName = friendsTab;
                        activeTabs.Add(tabName);
                        tabMenu.Tabs[activeTabs.IndexOf(tabName), 0] = GetString("MyAccount.MyFriends");
                        tabMenu.Tabs[activeTabs.IndexOf(tabName), 2] = HTMLHelper.HTMLEncode(URLHelper.AddParameterToUrl(absoluteUri, ParameterName, friendsTab));

                        if (selectedPage == string.Empty)
                        {
                            selectedPage = tabName;
                        }
                    }
                }

                if ((ucMyAllSubscriptions == null) && DisplayMySubscriptions)
                {
                    // Try to load the control dynamically (if available)
                    ucMyAllSubscriptions = Page.LoadUserControl("~/CMSModules/Membership/Controls/Subscriptions.ascx") as CMSAdminControl;
                    if (ucMyAllSubscriptions != null)
                    {
                     
                        // Set control
                        ucMyAllSubscriptions.Visible = false;

                        ucMyAllSubscriptions.SetValue("ShowBlogs", DisplayBlogs);
                        ucMyAllSubscriptions.SetValue("ShowMessageBoards", DisplayMessageBoards);
                        ucMyAllSubscriptions.SetValue("ShowNewsletters", DisplayNewsletters);
                        ucMyAllSubscriptions.SetValue("ShowForums", DisplayForums);
                        ucMyAllSubscriptions.SetValue("ShowReports", DisplayReports);
                        ucMyAllSubscriptions.SetValue("sendconfirmationemail", SendConfirmationEmails);

                        ucMyAllSubscriptions.ID = "ucMyAllSubscriptions";
                        plcOther.Controls.Add(ucMyAllSubscriptions);

                        // Set new tab
                        tabName = subscriptionsTab;
                        activeTabs.Add(tabName);
                        tabMenu.Tabs[activeTabs.IndexOf(tabName), 0] = GetString("MyAccount.MyAllSubscriptions");
                        tabMenu.Tabs[activeTabs.IndexOf(tabName), 2] = HTMLHelper.HTMLEncode(URLHelper.AddParameterToUrl(absoluteUri, ParameterName, subscriptionsTab));

                        if (selectedPage == string.Empty)
                        {
                            selectedPage = tabName;
                        }
                    }
                }

                // My memberships
                if ((ucMyMemberships == null) && DisplayMyMemberships)
                {
                    // Try to load the control dynamically
                    ucMyMemberships = Page.LoadUserControl("~/CMSModules/Membership/Controls/MyMemberships.ascx") as CMSAdminControl;

                    if (ucMyMemberships != null)
                    {
                        ucMyMemberships.SetValue("UserID", currentUser.UserID);

                        if (!String.IsNullOrEmpty(MembershipsPagePath))
                        {
                            ucMyMemberships.SetValue("BuyMembershipURL", DocumentURLProvider.GetUrl(MembershipsPagePath));
                        }

                        plcOther.Controls.Add(ucMyMemberships);

                        // Set new tab
                        tabName = membershipsTab;
                        activeTabs.Add(tabName);
                        tabMenu.Tabs[activeTabs.IndexOf(tabName), 0] = GetString("myaccount.mymemberships");
                        tabMenu.Tabs[activeTabs.IndexOf(tabName), 2] = HTMLHelper.HTMLEncode(URLHelper.AddParameterToUrl(absoluteUri, ParameterName, membershipsTab));

                        if (selectedPage == String.Empty)
                        {
                            selectedPage = tabName;
                        }
                    }
                }

                if ((ucMyCategories == null) && DisplayMyCategories)
                {
                    // Try to load the control dynamically (if available)
                    ucMyCategories = Page.LoadUserControl("~/CMSModules/Categories/Controls/Categories.ascx") as CMSAdminControl;
                    if (ucMyCategories != null)
                    {
                        ucMyCategories.Visible = false;

                        ucMyCategories.SetValue("DisplaySiteCategories", false);
                        ucMyCategories.SetValue("DisplaySiteSelector", false);

                        ucMyCategories.ID = "ucMyCategories";
                        plcOther.Controls.Add(ucMyCategories);
                        
                        // Set new tab
                        tabName = categoriesTab;
                        activeTabs.Add(tabName);
                        tabMenu.Tabs[activeTabs.IndexOf(tabName), 0] = GetString("MyAccount.MyCategories");
                        tabMenu.Tabs[activeTabs.IndexOf(tabName), 2] = HTMLHelper.HTMLEncode(URLHelper.AddParameterToUrl(absoluteUri, ParameterName, categoriesTab));

                        if (selectedPage == string.Empty)
                        {
                            selectedPage = tabName;
                        }
                    }
                }

                // Set CSS class
                pnlBody.CssClass = CssClass;

                // Get page URL
                page = QueryHelper.GetString(ParameterName, selectedPage);

                // Set controls visibility
                ucChangePassword.Visible = false;
                ucChangePassword.StopProcessing = true;

                if (ucMyAddresses != null)
                {
                    ucMyAddresses.Visible = false;
                    ucMyAddresses.StopProcessing = true;
                }

                if (ucMyOrders != null)
                {
                    ucMyOrders.Visible = false;
                    ucMyOrders.StopProcessing = true;
                }

                if (ucMyDetails != null)
                {
                    ucMyDetails.Visible = false;
                    ucMyDetails.StopProcessing = true;
                }

                if (ucMyCredit != null)
                {
                    ucMyCredit.Visible = false;
                    ucMyCredit.StopProcessing = true;
                }

                if (ucMyAllSubscriptions != null)
                {
                    ucMyAllSubscriptions.Visible = false;
                    ucMyAllSubscriptions.StopProcessing = true;
                    ucMyAllSubscriptions.SetValue("CacheMinutes", CacheMinutes);
                }

                if (ucMyNotifications != null)
                {
                    ucMyNotifications.Visible = false;
                    ucMyNotifications.StopProcessing = true;
                }

                if (ucMyMessages != null)
                {
                    ucMyMessages.Visible = false;
                    ucMyMessages.StopProcessing = true;
                }

                if (ucMyFriends != null)
                {
                    ucMyFriends.Visible = false;
                    ucMyFriends.StopProcessing = true;
                }

                if (ucMyMemberships != null)
                {
                    ucMyMemberships.Visible = false;
                    ucMyMemberships.StopProcessing = true;
                }

                if (ucMyCategories != null)
                {
                    ucMyCategories.Visible = false;
                    ucMyCategories.StopProcessing = true;
                }

                tabMenu.SelectedTab = activeTabs.IndexOf(page);

                // Select current page
                switch (page)
                {
                    case personalTab:
                        if (myProfile != null)
                        {
                            // Get alternative form info
                            AlternativeFormInfo afi = AlternativeFormInfoProvider.GetAlternativeFormInfo(AlternativeFormName);
                            if (afi != null)
                            {
                                myProfile.StopProcessing = false;
                                myProfile.Visible = true;
                                myProfile.AllowEditVisibility = AllowEditVisibility;
                                myProfile.AlternativeFormName = AlternativeFormName;
                            }
                            else
                            {
                                lblError.Text = String.Format(GetString("altform.formdoesntexists"), AlternativeFormName);
                                lblError.Visible = true;
                                myProfile.Visible = false;
                            }
                        }
                        break;

                    // My details tab
                    case detailsTab:
                        if (ucMyDetails != null)
                        {
                            ucMyDetails.Visible = true;
                            ucMyDetails.StopProcessing = false;
                            ucMyDetails.SetValue("Customer", customer);
                        }
                        break;

                    // My addresses tab
                    case addressesTab:
                        if (ucMyAddresses != null)
                        {
                            ucMyAddresses.Visible = true;
                            ucMyAddresses.StopProcessing = false;
                            ucMyAddresses.SetValue("CustomerId", customerId);
                        }
                        break;

                    // My orders tab
                    case ordersTab:
                        if (ucMyOrders != null)
                        {
                            ucMyOrders.Visible = true;
                            ucMyOrders.StopProcessing = false;
                            ucMyOrders.SetValue("CustomerId", customerId);
                            ucMyOrders.SetValue("ShowOrderTrackingNumber", ShowOrderTrackingNumber);
                        }
                        break;

                    // My credit tab
                    case creditTab:
                        if (ucMyCredit != null)
                        {
                            ucMyCredit.Visible = true;
                            ucMyCredit.StopProcessing = false;
                            ucMyCredit.SetValue("CustomerId", customerId);
                        }
                        break;

                    // Password tab
                    case passwordTab:
                        ucChangePassword.Visible = true;
                        ucChangePassword.StopProcessing = false;
                        ucChangePassword.AllowEmptyPassword = AllowEmptyPassword;
                        break;

                    // Notification tab
                    case notificationsTab:
                        if (ucMyNotifications != null)
                        {
                            ucMyNotifications.Visible = true;
                            ucMyNotifications.StopProcessing = false;
                            ucMyNotifications.SetValue("UserId", currentUser.UserID);
                            ucMyNotifications.SetValue("UnigridImageDirectory", UnigridImageDirectory);
                        }
                        break;

                    // My messages tab
                    case messagesTab:
                        if (ucMyMessages != null)
                        {
                            ucMyMessages.Visible = true;
                            ucMyMessages.StopProcessing = false;
                        }
                        break;

                    // My friends tab
                    case friendsTab:
                        if (ucMyFriends != null)
                        {
                            ucMyFriends.Visible = true;
                            ucMyFriends.StopProcessing = false;
                            ucMyFriends.SetValue("UserID", currentUser.UserID);
                        }
                        break;

                    // My subscriptions tab
                    case subscriptionsTab:
                        if (ucMyAllSubscriptions != null)
                        {
                            ucMyAllSubscriptions.Visible = true;
                            ucMyAllSubscriptions.StopProcessing = false;

                            ucMyAllSubscriptions.SetValue("userid", currentUser.UserID);
                            ucMyAllSubscriptions.SetValue("siteid", CMSContext.CurrentSiteID);
                        }
                        break;

                    // My memberships tab
                    case membershipsTab:
                        if (ucMyMemberships != null)
                        {
                            ucMyMemberships.Visible = true;
                            ucMyMemberships.StopProcessing = false;
                        }
                        break;

                    // My categories tab
                    case categoriesTab:
                        if (ucMyCategories != null)
                        {
                            ucMyCategories.Visible = true;
                            ucMyCategories.StopProcessing = false;
                        }
                        break;
                }
            }
            else
            {
                // Hide control if current user is not authenticated
                Visible = false;
            }
        }
    }


    /// <summary>
    /// Reload data.
    /// </summary>
    public override void ReloadData()
    {
        base.ReloadData();
        SetupControl();

        mContentLoaded = true;
    }
}