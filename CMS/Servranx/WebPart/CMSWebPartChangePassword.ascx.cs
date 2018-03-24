using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Linq;

using CMS.GlobalHelper;
using CMS.PortalControls;
using CMS.Helpers;

public partial class Servranx_WebPart_CMSWebPartChangePassword : CMSAbstractWebPart
{
    #region "Properties"

    public string InvalidRequestText
    {
        get
        {
            return ValidationHelper.GetString(GetValue("InvalidRequestText"), "");
        }
        set
        {
            SetValue("InvalidRequestText", value);
        }
    }


    /// <summary>
    /// Url on which is user redirected after successful password reset.
    /// </summary>
    public string RedirectUrl
    {
        get
        {
            return ValidationHelper.GetString(GetValue("RedirectUrl"), "");
        }
        set
        {
            SetValue("RedirectUrl", value);
        }
    }


    /// <summary>
    /// E-mail address from which e-mail is sent.
    /// </summary>
    public string SendEmailFrom
    {
        get
        {
            return ValidationHelper.GetString(GetValue("SendEmailFrom"), "");
        }
        set
        {
            SetValue("SendEmailFrom", value);
        }
    }


    /// <summary>
    /// If interval for action confirmation is exceeded this text is shown.
    /// </summary>
    public string ExceededIntervalText
    {
        get
        {
            return ValidationHelper.GetString(GetValue("ExceededIntervalText"), "");
        }
        set
        {
            SetValue("ExceededIntervalText", value);
        }
    }


    /// <summary>
    /// Text shown when password reset was succesful. 
    /// </summary>
    public string SuccessText
    {
        get
        {
            return ValidationHelper.GetString(GetValue("SuccessText"), "");
        }
        set
        {
            SetValue("SuccessText", value);
        }
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Content loaded event handler
    /// </summary>
    public override void OnContentLoaded()
    {
        base.OnContentLoaded();
        SetupControl();
    }


    /// <summary>
    /// Initializes the control properties
    /// </summary>
    protected void SetupControl()
    {
        if (this.StopProcessing)
        {
            resetPwdId.StopProcessing = true;
        }
        else
        {
            resetPwdId.InvalidRequestText = InvalidRequestText;
            resetPwdId.RedirectUrl = RedirectUrl;
            resetPwdId.SendEmailFrom = SendEmailFrom;
            resetPwdId.ExceededIntervalText = ExceededIntervalText;
            resetPwdId.SuccessText = SuccessText;
        }
    }

    public override void ReloadData()
    {
        base.ReloadData();
    }

    #endregion
}
