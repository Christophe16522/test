using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Security.Principal;
using System.Web.UI.WebControls;

using CMS.Base;
using CMS.Core;
using CMS.DataEngine;
using CMS.Ecommerce;
using CMS.EventLog;
using CMS.ExtendedControls.ActionsConfig;
using CMS.Helpers;
using CMS.PortalEngine;
using CMS.SiteProvider;
using CMS.UIControls;

// Edited object
[ParentObject(DiscountInfo.OBJECT_TYPE, "discountid")]
[Breadcrumb(1, "com.discount.generatecoupons")]
public partial class CMSModules_Ecommerce_Pages_Tools_Discount_Discount_Codes_Generator : CMSEcommercePage
{
    #region "Variables and constants"

    private static readonly Hashtable mWarnings = new Hashtable();
    private int discountId;
    private int count;
    private string prefix = "";
    private int numberOfUses;
    private string redirectUrl;

    private const string pattern = "*****";

    #endregion


    #region "Properties"

    private DiscountInfo ParentDiscount
    {
        get
        {
            return (DiscountInfo)EditedObjectParent;
        }
    }


    /// <summary>
    /// Gets discount type from query string.
    /// </summary>
    private DiscountApplicationEnum DiscountType
    {
        get
        {
            // Return Discount type
            return ParentDiscount.DiscountApplyTo;
        }
    }


    /// <summary>
    /// Current log context.
    /// </summary>
    public LogContext CurrentLog
    {
        get
        {
            return EnsureLog();
        }
    }


    /// <summary>
    /// Current Error.
    /// </summary>
    private string CurrentError
    {
        get
        {
            return ValidationHelper.GetString(mWarnings["DefineError_" + ctlAsync.ProcessGUID], string.Empty);
        }
        set
        {
            mWarnings["DefineError_" + ctlAsync.ProcessGUID] = value;
        }
    }

    #endregion


    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        // Parent discount does not exist or it is a catalog discount where coupons are not allowed
        if ((ParentDiscount == null) || (ParentDiscount.DiscountApplyTo == DiscountApplicationEnum.Products))
        {
            EditedObjectParent = null;
            return;
        }

        // Check if object from current site is edited
        CheckEditedObjectSiteID(ParentDiscount.DiscountSiteID);

        // Check UI permissions
        CheckUIPermissions();

        CurrentMaster.HeaderActions.ActionPerformed += HeaderActions_ActionPerformed;
        CurrentMaster.HeaderActions.ActionsList.Add(new HeaderAction
        {
            Text = GetString("com.couponcodes.generate"),
            CommandName = "generate"
        });

        redirectUrl = GetRedirectUrl();
        SetBreadcrumb(0, GetString("com.discount.coupons"), redirectUrl, null, null);

        discountId = QueryHelper.GetInteger("discountid", 0);

        if (!URLHelper.IsPostback())
        {
            // Show error message
            if (QueryHelper.Contains("error"))
            {
                ShowError(HTMLHelper.HTMLEncode(QueryHelper.GetString("error", string.Empty)));
            }
        }

        // Setup and configure asynchronous control
        SetupAsyncControl();
    }

    #endregion


    #region "Event handlers"

    void HeaderActions_ActionPerformed(object sender, CommandEventArgs e)
    {
        if (e.CommandName == "generate")
        {
            if (!DiscountInfoProvider.IsUserAuthorizedToModifyDiscount(SiteContext.CurrentSiteName, CurrentUser))
            {
                RedirectToAccessDenied("CMS.Ecommerce", "EcommerceModify OR ModifyDiscounts");
            }

            // Collect data from form
            count = ValidationHelper.GetInteger(txtNumberOfCodes.Text, 0);
            numberOfUses = ValidationHelper.GetInteger(txtTimesToUse.Text, 0);
            prefix = txtPrefix.Text.Trim();

            // Validate inputs
            if (count < 1)
            {
                ShowError(GetString("com.couponcode.invalidcount"));
                return;
            }

            if ((numberOfUses < 0) || (numberOfUses > 999999) || (!string.IsNullOrEmpty(txtTimesToUse.Text) && !ValidationHelper.IsInteger(txtTimesToUse.Text)))
            {
                ShowError(GetString("com.couponcode.invalidnumberOfUses"));
                return;
            }

            if (!string.IsNullOrEmpty(prefix) && !ValidationHelper.IsCodeName(prefix))
            {
                ShowError(GetString("com.couponcode.invalidprefix"));
                return;
            }

            // Run action in asynchronous control
            EnsureAsyncLog();
            RunAsync(GenerateCodes);
        }
    }


    /// <summary>
    /// Checks page UI permissions based on parent discount type.
    /// </summary>
    private void CheckUIPermissions()
    {
        string elementName = "";

        switch (DiscountType)
        {
            case DiscountApplicationEnum.Order:
                elementName = "NewOrderCoupon";
                break;

            case DiscountApplicationEnum.Shipping:

                elementName = "NewShippingCoupon";
                break;
        }

        // Check UI personalization
        CheckUIElementAccessHierarchical(ModuleName.ECOMMERCE, elementName);
    }

    #endregion


    #region "Methods"

    private string GetRedirectUrl()
    {
        if (ParentDiscount != null)
        {
            string elementName = null;
            switch (DiscountType)
            {
                case DiscountApplicationEnum.Order:
                    elementName = "OrderCouponCodes";
                    break;

                case DiscountApplicationEnum.Shipping:
                    elementName = "ShippingCouponCodes";
                    break;
            }

            if (!string.IsNullOrEmpty(elementName))
            {
                return URLHelper.AppendQuery(UIContextHelper.GetElementUrl("CMS.Ecommerce", elementName), String.Format("parentobjectid={0}&displaytitle=false", ParentDiscount.DiscountID));
            }
        }

        return RequestContext.CurrentURL;
    }


    private HashSet<string> GetExistingCodes()
    {
        // Prepare query for codes cache
        var existingQuery = CouponCodeInfoProvider.GetCouponCodes(SiteContext.CurrentSiteName).Column("CouponCodeCode").Distinct();

        // Restrict cache if prefix specified
        if (!string.IsNullOrEmpty(prefix))
        {
            existingQuery.Where("CouponCodeCode", QueryOperator.Like, prefix + "%");
        }

        // Create cache of this site coupon codes
        var existingCodes = new HashSet<string>();
        using (DbDataReader reader = existingQuery.ExecuteReader())
        {
            while (reader.Read())
            {
                existingCodes.Add(reader.GetString(0));
            }
        }

        return existingCodes;
    }


    private void GenerateCodes(object parameter)
    {
        try
        {
            // Construct cache for code uniqueness check
            var existingCodes = GetExistingCodes();

            CouponCodeInfo couponCode = null;

            using (CMSActionContext context = new CMSActionContext())
            {
                // Do not touch parent for all codes
                context.TouchParent = false;
                context.LogEvents = false;

                // Create generator
                RandomCodeGenerator generator = new RandomCodeGenerator(pattern, prefix);
                // Use cache for checking code uniqueness
                generator.CodeChecker = code => !existingCodes.Contains(code);

                for (int i = 0; i < count; i++)
                {
                    // Get new code
                    string code = generator.GenerateCode();

                    couponCode = new CouponCodeInfo
                                     {
                                         CouponCodeUseLimit = numberOfUses,
                                         CouponCodeDiscountID = discountId,
                                         CouponCodeCode = code,
                                         CouponCodeUseCount = 0
                                     };

                    // Save coupon code
                    CouponCodeInfoProvider.SetCouponCodeInfo(couponCode);

                    // Log that coupon was created
                    AddLog(string.Format(GetString("com.couponcode.generated"), HTMLHelper.HTMLEncode(ResHelper.LocalizeString(code))));
                }
            }

            // Touch parent (one for all)
            if (couponCode != null)
            {
                couponCode.Generalized.TouchParent();
            }

            // Log information that coupons were generated
            EventLogProvider.LogEvent(EventType.INFORMATION, "Discounts", "GENERATECOUPONCODES",
                                      string.Format("{0} coupon codes for discount '{1}' successfully generated.", count, ParentDiscount.DiscountDisplayName),
                                      userId: CurrentUser.UserID,
                                      userName: CurrentUser.UserName,
                                      siteId: SiteContext.CurrentSiteID
                                     );
        }
        catch (Exception ex)
        {
            CurrentError = GetString("com.couponcode.generateerror");
            EventLogProvider.LogException("Discounts", "GENERATECOUPONCODES", ex);
        }
    }


    /// <summary>
    /// Adds parameter to current URL and Redirects to it.
    /// </summary>
    /// <param name="parameter">Parameter to be added.</param>
    /// <param name="value">Value of parameter to be added.</param>
    private void RedirectTo(string parameter, string value)
    {
        string urlToRedirect = URLHelper.AddParameterToUrl(RequestContext.CurrentURL, parameter, value);
        URLHelper.Redirect(urlToRedirect);
    }

    #endregion


    #region "Handling asynchronous thread"

    private void ctlAsync_OnCancel(object sender, EventArgs e)
    {
        CurrentLog.Close();
        RedirectTo("error", GetString("com.couponcode.generationterminated"));
    }


    private void ctlAsync_OnFinished(object sender, EventArgs e)
    {
        CurrentLog.Close();

        if (!String.IsNullOrEmpty(CurrentError))
        {
            RedirectTo("error", CurrentError);
        }

        URLHelper.Redirect(redirectUrl);
    }


    /// <summary>
    /// Adds the log information
    /// </summary>
    /// <param name="newLog">New log information</param>
    protected void AddLog(string newLog)
    {
        EnsureLog();
        LogContext.AppendLine(newLog);
    }


    /// <summary>
    /// Ensures the logging context
    /// </summary>
    protected LogContext EnsureLog()
    {
        LogContext currentLog = LogContext.EnsureLog(ctlAsync.ProcessGUID);

        return currentLog;
    }


    /// <summary>
    /// Ensures log for asynchronous control
    /// </summary>
    private void EnsureAsyncLog()
    {
        pnlLog.Visible = true;
        pnlGeneral.Visible = false;
        CurrentError = string.Empty;

        CurrentLog.Close();
        EnsureLog();
    }


    /// <summary>
    /// Runs asynchronous thread
    /// </summary>
    /// <param name="action">Method to run</param>
    protected void RunAsync(AsyncAction action)
    {
        ctlAsync.RunAsync(action, WindowsIdentity.GetCurrent());
    }


    /// <summary>
    /// Prepare asynchronous control
    /// </summary>
    private void SetupAsyncControl()
    {
        ctlAsync.OnFinished += ctlAsync_OnFinished;
        ctlAsync.OnError += (s, e) => CurrentLog.Close();
        ctlAsync.OnRequestLog += (sender, args) => { ctlAsync.Log = CurrentLog.Log; };
        ctlAsync.OnCancel += ctlAsync_OnCancel;

        // Asynchronous content configuration
        titleElemAsync.TitleText = GetString("com.couponcodes.generating");
        if (!RequestHelper.IsCallback())
        {
            // Set visibility of panels
            pnlGeneral.Visible = true;
            pnlLog.Visible = false;

            btnCancel.Attributes.Add("onclick", ctlAsync.GetCancelScript(true) + "return false;");
        }
    }

    #endregion
}