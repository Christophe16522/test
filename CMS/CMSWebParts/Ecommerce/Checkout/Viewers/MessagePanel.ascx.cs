using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.Base;
using CMS.ExtendedControls;
using CMS.Helpers;


/// <summary>
/// Panel for info/error messages in checkout process
/// </summary>
public partial class CMSWebParts_Ecommerce_Checkout_Viewers_MessagePanel : CMSCheckoutWebPart
{
    protected void Page_Load(object sender, EventArgs e)
    {
        ComponentEvents.RequestEvents.RegisterForEvent(MESSAGE_RAISED, HandleMessage);
    }


    /// <summary>
    /// Handle incoming message.
    /// </summary>
    protected void HandleMessage(object sender, EventArgs e)
    {
        CMSEventArgs<string> args = e as CMSEventArgs<string>;

        if (args != null)
        {
            string message = args.Parameter;
            lblMessage.Text = message;
            lblMessage.Visible = true;
            // Clear parameter to tag this message as handled
            args.Parameter = string.Empty;
        }
    }
}