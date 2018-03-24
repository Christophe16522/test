using System;
using System.Collections;
using System.Security.Principal;
using System.Collections.Generic;
using System.Text;

using CMS.Base;
using CMS.Core;
using CMS.EventLog;
using CMS.ExtendedControls;
using CMS.Helpers;
using CMS.Membership;
using CMS.Newsletters;
using CMS.SiteProvider;
using CMS.UIControls;

[Title("newsletters.importsubscribers")]
[UIElement(ModuleName.NEWSLETTER, "ImportSubscribers")]
public partial class CMSModules_Newsletters_Tools_ImportExportSubscribers_Subscriber_Import : CMSNewsletterPage
{
    #region "Variables and properties"

    private static Hashtable errors = new Hashtable();
    private static Hashtable message = new Hashtable();


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
    /// Messages placeholder
    /// </summary>
    public override MessagesPlaceHolder MessagesPlaceHolder
    {
        get
        {
            return plcMessages;
        }
    }

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        // Initialize newsletter selector
        usNewsletters.WhereCondition = "NewsletterSiteID = " + SiteContext.CurrentSiteID;

        // Registers script for disabling checkbox
        radSubscribe.Attributes.Add("onclick", "SelectionChanged()");
        radUnsubscribe.Attributes.Add("onclick", "SelectionChanged()");
        radDelete.Attributes.Add("onclick", "SelectionChanged()");
        string script = "function SelectionChanged() { \n" +
                        "   var radSubscribe = document.getElementById('" + radSubscribe.ClientID + "').checked; \n" +
                        "   document.getElementById('" + chkDoNotSubscribe.ClientID + "').disabled = !radSubscribe; \n" +
                        "} \n";

        ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "DisableCheckbox", ScriptHelper.GetScript(script));
        btnCancel.Attributes.Add("onclick", ctlAsync.GetCancelScript(true) + "return false;");

        // Initialize events
        ctlAsync.OnFinished += ctlAsync_OnFinished;
        ctlAsync.OnError += ctlAsync_OnError;
        ctlAsync.OnRequestLog += ctlAsync_OnRequestLog;
        ctlAsync.OnCancel += ctlAsync_OnCancel;

        if (!RequestHelper.IsCallback())
        {
            pnlContent.Visible = true;
            pnlLog.Visible = false;
        }
    }


    /// <summary>
    /// Runs async thread.
    /// </summary>
    /// <param name="action">Method to run</param>
    protected void RunAsync(AsyncAction action)
    {
        pnlLog.Visible = true;

        CurrentLog.Close();
        EnsureLog();

        ctlAsync.RunAsync(action, WindowsIdentity.GetCurrent());
    }


    /// <summary>
    /// Handles import button click.
    /// </summary>
    /// <param name="sender">Sender</param>
    /// <param name="e">Event arguments</param>
    protected void btnImport_Click(object sender, EventArgs e)
    {
        // Check 'Manage subscribers' permission
        if (!MembershipContext.AuthenticatedUser.IsAuthorizedPerResource("cms.newsletter", "managesubscribers"))
        {
            RedirectToAccessDenied("cms.newsletter", "managesubscribers");
        }

        if (String.IsNullOrEmpty(txtImportSub.Text.Trim()))
        {
            ShowError(GetString("Subscriber_Import.nosubscribers"));
            return;
        }

        // Import subscribers
        try
        {
            // Get selected newsletters
            int siteId = SiteContext.CurrentSiteID;
            List<int> newsletterIds = new List<int>();
            string values = ValidationHelper.GetString(usNewsletters.Value, null);
            if (!String.IsNullOrEmpty(values))
            {
                string[] newItems = values.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string item in newItems)
                {
                    var newsletter = NewsletterInfoProvider.GetNewsletterInfo(item.ToInteger(0));

                    if ((newsletter == null) || (newsletter.NewsletterSiteID != siteId))
                    {
                        continue;
                    }

                    newsletterIds.Add(newsletter.NewsletterID);
                }
            }
            CurrentLog.Close();
            EnsureLog();
            errors[ctlAsync.ProcessGUID] = null;

            // Add subscribers to site and subscribe them to selected newsletters
            if (radSubscribe.Checked)
            {
                ctlAsync.Parameter = new object[] { txtImportSub.Text, newsletterIds, siteId, true, chkSendConfirmation.Checked, chkDoNotSubscribe.Checked, chkRequireOptIn.Checked, ctlAsync.ProcessGUID, errors };
                RunAsync(SubscriberImporter.ImportSubscribersToSite);
                message[ctlAsync.ProcessGUID] = GetString("Subscriber_Import.SubscribersImported");
                titleElemAsync.TitleText = GetString("newsletters.importingsubscribers");
            }
            // Unsubscribe inserted subscribers from selected newsletters
            else if (radUnsubscribe.Checked)
            {
                if (newsletterIds.Count > 0)
                {
                    ctlAsync.Parameter = new object[] { txtImportSub.Text, newsletterIds, siteId, chkSendConfirmation.Checked, ctlAsync.ProcessGUID, errors };
                    RunAsync(SubscriberInfoProvider.UnsubscribeFromNewsletters);
                    message[ctlAsync.ProcessGUID] = GetString("Subscriber_Import.SubscribersUnsubscribed");
                    titleElemAsync.TitleText = GetString("newsletters.unsubscribing");
                }
                else
                {
                    ShowError(GetString("ImportSubscribers.NoNewslettersSelected"));
                }
            }
            // Delete inserted subscribers
            else if (radDelete.Checked)
            {

                ctlAsync.Parameter = new object[] { txtImportSub.Text, siteId, ctlAsync.ProcessGUID, errors };
                RunAsync(SubscriberInfoProvider.DeleteSubscribers);
                message[ctlAsync.ProcessGUID] = GetString("Subscriber_Import.SubscribersDeleted");
                titleElemAsync.TitleText = GetString("newsletters.deleting");
            }
        }
        catch (Exception ex)
        {
            ShowError(ex.Message);
        }
    }


    protected LogContext EnsureLog()
    {
        LogContext log = LogContext.EnsureLog(ctlAsync.ProcessGUID);

        return log;
    }


    private void DisplayMessages()
    {
        CurrentLog.Close();
        TerminateCallbacks();

        List<string> listOfErrors = (List<string>)errors[ctlAsync.ProcessGUID] ?? new List<string>();
        StringBuilder errorsString = new StringBuilder();

        foreach (var singleError in listOfErrors)
        {
            errorsString.Append(singleError + "<br />");
        }

        string htmlError = errorsString.ToString();

        if (!String.IsNullOrEmpty(htmlError))
        {
            ShowError(htmlError.Remove(htmlError.Length - 6));
        }
        else
        {
            ShowConfirmation((string)message[ctlAsync.ProcessGUID]);
        }
    }


    private void TerminateCallbacks()
    {
        string terminatingScript = ScriptHelper.GetScript("var __pendingCallbacks = new Array();");
        ScriptHelper.RegisterStartupScript(this, typeof(string), "terminatePendingCallbacks", terminatingScript);
    }

    #endregion


    #region "Handling async thread"

    private void ctlAsync_OnRequestLog(object sender, EventArgs e)
    {
        ctlAsync.Log = CurrentLog.Log;
    }


    private void ctlAsync_OnError(object sender, EventArgs e)
    {
        DisplayMessages();
    }


    private void ctlAsync_OnFinished(object sender, EventArgs e)
    {
        DisplayMessages();
    }


    private void ctlAsync_OnCancel(object sender, EventArgs e)
    {
        ctlAsync.Parameter = null;
        message[ctlAsync.ProcessGUID] = GetString("Subscriber_Import.cancelled");
        DisplayMessages();
    }

    #endregion
}