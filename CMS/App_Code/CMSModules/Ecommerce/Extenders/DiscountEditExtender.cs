using System;
using System.Text;
using System.Web.UI.WebControls;

using CMS;
using CMS.Base;
using CMS.Ecommerce;
using CMS.ExtendedControls;
using CMS.FormControls;
using CMS.Helpers;
using CMS.PortalEngine;

[assembly: RegisterCustomClass("DiscountEditExtender", typeof(DiscountEditExtender))]

/// <summary>
/// Extender for Discount tab
/// </summary>
public class DiscountEditExtender : ControlExtender<UIForm>
{
    #region "Variables"

    private HiddenField mUsesCouponsDefaultValue;
    private bool? mUsesCouponsChecked;

    #endregion


    #region "Properties"

    /// <summary>
    /// Returns edited discount info object.
    /// </summary>
    private DiscountInfo Discount
    {
        get
        {
            return Control.EditedObject as DiscountInfo;
        }
    }

    /// <summary>
    /// Remembers original value of uses coupon check box.
    /// </summary>
    private HiddenField UsesCouponsDefaultValue
    {
        get
        {
            if (mUsesCouponsDefaultValue == null)
            {
                mUsesCouponsDefaultValue = new HiddenField { ID = "usesCouponsDefaultValue" };
            }

            return mUsesCouponsDefaultValue;
        }
    }


    /// <summary>
    /// Returns original value of Uses coupons checkbox.
    /// </summary>
    private bool UsesCouponsChecked
    {
        get
        {
            if (mUsesCouponsChecked == null)
            {
                mUsesCouponsChecked = ValidationHelper.GetBoolean(UsesCouponsDefaultValue.Value, true);
            }

            return mUsesCouponsChecked.Value;
        }
    }


    /// <summary>
    /// Returns current value of uses coupons check box.
    /// </summary>
    private bool UsesCouponsCheckedByUser
    {
        get
        {
            return ValidationHelper.GetBoolean(Control.FieldControls["DiscountUsesCoupons"].Value, false);
        }
    }


    /// <summary>
    /// Indicates whether status of "discount uses coupons" field was changed from disabled to enabled.
    /// </summary>
    private bool RedirectionEnabled
    {
        get
        {
            return (!UsesCouponsChecked && UsesCouponsCheckedByUser);
        }
    }


    /// <summary>
    /// Indicates whether status of "discount uses coupons" field was changed from enabled to disabled.
    /// </summary>
    private bool CouponCodesUnchecked
    {
        get
        {
            return (UsesCouponsChecked && !UsesCouponsCheckedByUser);
        }
    }

    #endregion


    #region "Life cycle"

    public override void OnInit()
    {
        Control.Page.Load += Page_Load;
        Control.PreRender += Control_PreRender;
        Control.OnAfterSave += Control_OnAfterSave;
    }

    #endregion


    #region "Page events"

    private void Page_Load(object sender, EventArgs e)
    {
        // Remember if discount uses coupons (remember value stored in the database);
        if (String.IsNullOrEmpty(UsesCouponsDefaultValue.Value))
        {
            // Insert value to the form to remember original checkbox value
            UsesCouponsDefaultValue.Value = ValidationHelper.GetString(Control.Data.GetValue("DiscountUsesCoupons"), "").ToLower();
            Panel pnlHidden = new Panel();
            pnlHidden.ID = "pnlHidden";
            pnlHidden.CssClass = "discountUsesCouponsValue";
            pnlHidden.Controls.Add(UsesCouponsDefaultValue);

            Control.Page.Form.Controls.Add(pnlHidden);
        }

        // Ensures showing/hiding redirection message on coupons tab
        string script = @"
jQuery(function() {  
    jQuery('#CouponCheckBox input[type=checkbox]').change(function() {
        if (jQuery('.discountUsesCouponsValue input[type=hidden]').val() === 'false')
        {
            displayRedirectionMessage();
        }
    })
});

function displayRedirectionMessage() {
    if (jQuery('#CouponCheckBox input[type=checkbox]').is(':checked')){
        jQuery('#CouponsInfoLabel').show();
    }
    else {
        jQuery('#CouponsInfoLabel').hide();
    }
}";

        // Register script hiding redirection message
        ScriptHelper.RegisterStartupScript(Control.Page, typeof(string), "MessageVisibility", ScriptHelper.GetScript(script));
    }


    /// <summary>
    /// Displays/hides redirection message if validation failed.
    /// </summary>
    protected void Control_PreRender(object sender, EventArgs e)
    {
        if (RedirectionEnabled)
        {
            ScriptHelper.RegisterStartupScript(Control.Page, typeof(string), "DisplayMessage", "jQuery('#CouponsInfoLabel').show();", true);
        }
    }


    /// <summary>
    /// Ensures redirection to Coupons tab if discount newly uses coupons.
    /// </summary>
    protected void Control_OnAfterSave(object sender, EventArgs e)
    {
        if (Discount == null)
        {
            return;
        }

        // Redirect to coupon codes generation
        if (RedirectionEnabled)
        {
            RedirectAfterSave();
        }
        else if (CouponCodesUnchecked)
        {
           // Update original value
            UsesCouponsDefaultValue.Value = "false";
            
            // Refresh tabs if "discount uses coupons" field was unchecked and discount don´t have any coupon codes
            if (Discount.CouponCodes.Count == 0)
            {
                RedirectAfterSave();
            }
        }
    }

    #endregion


    #region "Helper methods"

    /// <summary>
    /// Redirects to coupon codes generation tab or refreshes tabs. 
    /// </summary>
    private void RedirectAfterSave()
    {
        string currentElementName = Control.UIContext.UIElement.ElementName.ToLowerCSafe();
        int objectId = Discount.DiscountID;
        string redirectionElement = ((currentElementName == "newshippingdiscount") || (currentElementName == "generalshipping")) ? "editshippingdiscount" : "editorderdiscount";

        switch (currentElementName)
        {
            case "newshippingdiscount":
            case "neworderdiscount":
                URLHelper.Redirect(GenerateRedirectionUrl(redirectionElement, objectId, true));
                break;

            case "generalshipping":
            case "generalorder":
                // Parent element needs to be redirected
                ExecuteWindowLocationRedirect(GenerateRedirectionUrl(redirectionElement, objectId, true));

                break;
        }
    }


    /// <summary>
    /// Ensures correct redirection of parent element.
    /// </summary>
    /// <param name="redirectUrl">Url to redirect to.</param>
    private void ExecuteWindowLocationRedirect(string redirectUrl)
    {
        ScriptHelper.RegisterClientScriptBlock(Control.Page, typeof(string), "OrderCouponRedirect", "parent.window.location='" + redirectUrl + "';", true);
    }


    /// <summary>
    /// Generate redirection url url.
    /// </summary>
    /// <param name="elementName">Element where user will be redirected</param>
    /// <param name="objectId">ID of edited object</param>
    /// <param name="saved">Show saved info message if true</param>
    /// <returns></returns>
    private string GenerateRedirectionUrl(string elementName, int objectId, bool saved)
    {
        string url = UIContextHelper.GetElementUrl("cms.ecommerce", elementName, false) + "&objectid=" + objectId;

        url += (RedirectionEnabled) ? "&tabindex=1" : "";
        url += (saved) ? "&saved=1" : "";

        return url;
    }

    #endregion
}
