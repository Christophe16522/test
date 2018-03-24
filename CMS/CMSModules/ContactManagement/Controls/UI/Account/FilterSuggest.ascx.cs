using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CMS.DataEngine;
using CMS.Helpers;
using CMS.OnlineMarketing;
using CMS.PortalEngine;
using CMS.Base;
using CMS.SiteProvider;
using CMS.UIControls;

public partial class CMSModules_ContactManagement_Controls_UI_Account_FilterSuggest : CMSUserControl
{
    #region "Variables"

    private AccountInfo ai;
    private int mSelectedSiteID;

    #endregion


    #region "Properties"

    /// <summary>
    /// Gets current account.
    /// </summary>
    private AccountInfo CurrentAccount
    {
        get
        {
            if (ai == null)
            {
                ai = (AccountInfo)UIContext.EditedObject;
            }
            return ai;
        }
    }


    /// <summary>
    /// Gets value indicating if contacts checkbox is selected.
    /// </summary>
    public bool ContactsChecked
    {
        get
        {
            return chkContacts.Checked;
        }
    }


    /// <summary>
    /// Gets value indicating if post address checkbox is selected.
    /// </summary>
    public bool AddressChecked
    {
        get
        {
            return chkAddress.Checked;
        }
    }


    /// <summary>
    /// Gets value indicating if email is selected.
    /// </summary>
    public bool EmailChecked
    {
        get
        {
            return chkEmail.Checked;
        }
    }


    /// <summary>
    /// Gets value indicating if URL checkbox is selected.
    /// </summary>
    public bool URLChecked
    {
        get
        {
            return chkURL.Checked;
        }
    }


    /// <summary>
    /// Gets value indicating if phone&fax checkbox is selected.
    /// </summary>
    public bool PhonesChecked
    {
        get
        {
            return chkPhone.Checked;
        }
    }

    /// <summary>
    /// Gets selected site ID.
    /// </summary>
    public int SelectedSiteID
    {
        get
        {
            return mSelectedSiteID;
        }
    }

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        chkContacts.Enabled = (CurrentAccount.AccountPrimaryContactID != 0) || (CurrentAccount.AccountSecondaryContactID != 0);
        chkAddress.Enabled = !String.IsNullOrEmpty(CurrentAccount.AccountAddress1) || !String.IsNullOrEmpty(CurrentAccount.AccountAddress2) || !String.IsNullOrEmpty(CurrentAccount.AccountCity) || !String.IsNullOrEmpty(CurrentAccount.AccountZIP);
        chkEmail.Enabled = !String.IsNullOrEmpty(CurrentAccount.AccountEmail);
        chkURL.Enabled = !String.IsNullOrEmpty(CurrentAccount.AccountWebSite);
        chkPhone.Enabled = !String.IsNullOrEmpty(CurrentAccount.AccountPhone) || !String.IsNullOrEmpty(CurrentAccount.AccountFax);

        // Current account is global object
        if (ai.AccountSiteID == 0)
        {
            plcSite.Visible = true;
            plcContact.Visible = false;
            // Display site selector in site manager
            if (ContactHelper.IsSiteManager)
            {
                siteOrGlobalSelector.Visible = false;
            }
            // Display 'site or global' selector in CMS desk for global objects
            else if (AccountHelper.AuthorizedReadAccount(SiteContext.CurrentSiteID, false) && AccountHelper.AuthorizedModifyAccount(SiteContext.CurrentSiteID, false))
            {
                siteSelector.Visible = false;
            }
            else
            {
                plcSite.Visible = false;
            }
        }
    }


    /// <summary>
    /// Returns SQL WHERE condition depending on selected checkboxes.
    /// </summary>
    /// <returns>Returns SQL WHERE condition</returns>
    public string GetWhereCondition()
    {
        string where = null;

        // Contacts checked
        if (chkContacts.Checked)
        {
            string contactWhere = null;
            ContactInfo contact;

            // Get primary contact WHERE condition
            if (CurrentAccount.AccountPrimaryContactID != 0)
            {
                contact = ContactInfoProvider.GetContactInfo(CurrentAccount.AccountPrimaryContactID);
                if (contact != null)
                {
                    if (!String.IsNullOrEmpty(contact.ContactFirstName))
                    {
                        contactWhere = SqlHelper.AddWhereCondition(contactWhere, "PrimaryContactFirstName LIKE N'%" + SqlHelper.GetSafeQueryString(contact.ContactFirstName, false) + "%'", "OR");
                    }
                    if (!String.IsNullOrEmpty(contact.ContactMiddleName))
                    {
                        contactWhere = SqlHelper.AddWhereCondition(contactWhere, "PrimaryContactMiddleName LIKE N'%" + SqlHelper.GetSafeQueryString(contact.ContactMiddleName, false) + "%'", "OR");
                    }
                    if (!String.IsNullOrEmpty(contact.ContactLastName))
                    {
                        contactWhere = SqlHelper.AddWhereCondition(contactWhere, "PrimaryContactLastName LIKE N'%" + SqlHelper.GetSafeQueryString(contact.ContactLastName, false) + "%'", "OR");
                    }
                }
            }

            // Get secondary contact WHERE condition
            if (CurrentAccount.AccountSecondaryContactID != 0)
            {
                contact = ContactInfoProvider.GetContactInfo(CurrentAccount.AccountSecondaryContactID);
                if (contact != null)
                {
                    if (!String.IsNullOrEmpty(contact.ContactFirstName))
                    {
                        contactWhere = SqlHelper.AddWhereCondition(contactWhere, "SecondaryContactFirstName LIKE N'%" + SqlHelper.GetSafeQueryString(contact.ContactFirstName, false) + "%'", "OR");
                    }
                    if (!String.IsNullOrEmpty(contact.ContactMiddleName))
                    {
                        contactWhere = SqlHelper.AddWhereCondition(contactWhere, "SecondaryContactMiddleName LIKE N'%" + SqlHelper.GetSafeQueryString(contact.ContactMiddleName, false) + "%'", "OR");
                    }
                    if (!String.IsNullOrEmpty(contact.ContactLastName))
                    {
                        contactWhere = SqlHelper.AddWhereCondition(contactWhere, "SecondaryContactLastName LIKE N'%" + SqlHelper.GetSafeQueryString(contact.ContactLastName, false) + "%'", "OR");
                    }
                }
            }

            if (!String.IsNullOrEmpty(contactWhere))
            {
                where = SqlHelper.AddWhereCondition(where, contactWhere);
            }
        }

        // Address checked
        if (chkAddress.Checked)
        {
            string addressWhere = null;
            if (!String.IsNullOrEmpty(CurrentAccount.AccountAddress1))
            {
                addressWhere = SqlHelper.AddWhereCondition(addressWhere, "AccountAddress1 LIKE N'%" + SqlHelper.GetSafeQueryString(CurrentAccount.AccountAddress1, false) + "%'", "OR");
            }
            if (!String.IsNullOrEmpty(CurrentAccount.AccountAddress2))
            {
                addressWhere = SqlHelper.AddWhereCondition(addressWhere, "AccountAddress2 LIKE N'%" + SqlHelper.GetSafeQueryString(CurrentAccount.AccountAddress2, false) + "%'", "OR");
            }
            if (!String.IsNullOrEmpty(CurrentAccount.AccountCity))
            {
                addressWhere = SqlHelper.AddWhereCondition(addressWhere, "AccountCity LIKE N'%" + SqlHelper.GetSafeQueryString(CurrentAccount.AccountCity, false) + "%'", "OR");
            }
            if (!String.IsNullOrEmpty(CurrentAccount.AccountZIP))
            {
                addressWhere = SqlHelper.AddWhereCondition(addressWhere, "AccountZIP LIKE N'%" + SqlHelper.GetSafeQueryString(CurrentAccount.AccountZIP, false) + "%'", "OR");
            }

            if (!String.IsNullOrEmpty(addressWhere))
            {
                where = SqlHelper.AddWhereCondition(where, "(" + addressWhere + ")");
            }
        }

        // Email address checked
        if (chkEmail.Checked)
        {
            string domain = ContactHelper.GetEmailDomain(CurrentAccount.AccountEmail);
            if (!String.IsNullOrEmpty(domain))
            {
                string emailWhere = "AccountEmail LIKE N'%@" + SqlHelper.GetSafeQueryString(domain) + "'";
                where = SqlHelper.AddWhereCondition(where, emailWhere);
            }
        }

        // URL checked
        if (chkURL.Checked && !String.IsNullOrEmpty(CurrentAccount.AccountWebSite))
        {
            string urlWhere = "(AccountWebSite LIKE N'%" + SqlHelper.GetSafeQueryString(URLHelper.CorrectDomainName(CurrentAccount.AccountWebSite)) + "%')";
            where = SqlHelper.AddWhereCondition(where, urlWhere);
        }

        // Phone & fax checked
        if (chkPhone.Checked && (!String.IsNullOrEmpty(CurrentAccount.AccountPhone) || !String.IsNullOrEmpty(CurrentAccount.AccountFax)))
        {
            string phoneWhere = null;
            if (!String.IsNullOrEmpty(CurrentAccount.AccountPhone))
            {
                phoneWhere = "AccountPhone LIKE N'%" + SqlHelper.GetSafeQueryString(CurrentAccount.AccountPhone, false) + "%'";
            }
            if (!String.IsNullOrEmpty(CurrentAccount.AccountFax))
            {
                phoneWhere = SqlHelper.AddWhereCondition(phoneWhere, "AccountFax LIKE N'%" + SqlHelper.GetSafeQueryString(CurrentAccount.AccountFax, false) + "%'", "OR");
            }

            if (!String.IsNullOrEmpty(phoneWhere))
            {
                where = SqlHelper.AddWhereCondition(where, "(" + phoneWhere + ")");
            }
        }

        if ((!chkContacts.Checked && !chkAddress.Checked && !chkEmail.Checked && !chkURL.Checked && !chkPhone.Checked) || (String.IsNullOrEmpty(where)))
        {
            return "(1 = 0)";
        }

        // Filter out current account
        where = SqlHelper.AddWhereCondition(where, "AccountID <> " + CurrentAccount.AccountID);
        // Filter out merged records
        where = SqlHelper.AddWhereCondition(where, "(AccountMergedWithAccountID IS NULL AND AccountGlobalAccountID IS NULL AND AccountSiteID > 0) OR (AccountGlobalAccountID IS NULL AND AccountSiteID IS NULL)");

        // For global object use siteselector's value
        if (plcSite.Visible)
        {
            mSelectedSiteID = UniSelector.US_ALL_RECORDS;
            if (siteSelector.Visible)
            {
                mSelectedSiteID = siteSelector.SiteID;
            }
            else if (siteOrGlobalSelector.Visible)
            {
                mSelectedSiteID = siteOrGlobalSelector.SiteID;
            }

            // Only global objects
            if (mSelectedSiteID == UniSelector.US_GLOBAL_RECORD)
            {
                where = SqlHelper.AddWhereCondition(where, "AccountSiteID IS NULL");
            }
            // Global and site objects
            else if (mSelectedSiteID == UniSelector.US_GLOBAL_AND_SITE_RECORD)
            {
                where = SqlHelper.AddWhereCondition(where, "AccountSiteID IS NULL OR AccountSiteID = " + SiteContext.CurrentSiteID);
            }
            // Site objects
            else if (mSelectedSiteID != UniSelector.US_ALL_RECORDS)
            {
                where = SqlHelper.AddWhereCondition(where, "AccountSiteID = " + mSelectedSiteID);
            }
        }
        // Filter out accounts from different sites
        else
        {
            // Site accounts only
            if (CurrentAccount.AccountSiteID > 0)
            {
                where = SqlHelper.AddWhereCondition(where, "AccountSiteID = " + CurrentAccount.AccountSiteID);
            }
            // Global accounts only
            else
            {
                where = SqlHelper.AddWhereCondition(where, "AccountSiteID IS NULL");
            }
        }

        return where;
    }

    #endregion
}