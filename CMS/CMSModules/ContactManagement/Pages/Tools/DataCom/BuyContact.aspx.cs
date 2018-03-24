using System;
using System.Collections;
using System.Net;

using CMS.DataCom;
using CMS.EventLog;
using CMS.GlobalHelper;
using CMS.OnlineMarketing;
using CMS.CMSHelper;
using CMS.Helpers;
using CMS.Membership;

/// <summary>
/// A dialog page where user can buy Data.com contact.
/// </summary>
public partial class CMSModules_ContactManagement_Pages_Tools_DataCom_BuyContact : CMSDataComDialogPage
{
    #region "Contstants"

    private const string DATACOM_LOGIN_PAGE = "https://connect.data.com/login";

    #endregion


    #region "Variables"

    /// <summary>
    /// Data.com contact to buy.
    /// </summary>
    private Contact mContact = null;

    #endregion


    #region "Properties"

    /// <summary>
    /// Gets or sets the Data.com contact to buy.
    /// </summary>
    public Contact Contact
    {
        get
        {
            return mContact;
        }
        set
        {
            mContact = value;
        }
    }

    /// <summary>
    /// Gets or sets the site identifier parameter.
    /// </summary>
    private int SiteIdentifierParameter { get; set; }

    #endregion


    #region "Life cycle methods"

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
        ScriptHelper.RegisterWOpenerScript(Page);
        ScriptHelper.RegisterJQuery(Page);
        CurrentMaster.Title.TitleText = GetString("datacom.buycontact.title");
        CurrentMaster.Title.TitleImage = GetImageUrl("Objects/OM_Contact/object.png");
        AuthorizeRequest();
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            RestoreParameters();
            InitializeContactPreview(Contact);
            if (!RequestHelper.IsPostBack())
            {
                ICredentialProvider credentialProvider = new UserCredentialProvider(MembershipContext.AuthenticatedUser);
                NetworkCredential userCredential = credentialProvider.GetCredential();
                if (userCredential != null)
                {
                    DataComClient client = DataComHelper.CreateClient();
                    User response = client.GetUser(userCredential);
                    InitializeAccountPoints(response.Points);
                    InitializePurchasePointsLink(DATACOM_LOGIN_PAGE);
                }
                else
                {
                    ErrorSummary.Report(GetString("datacom.nousercredential"));
                    BuyButton.Enabled = false;
                }
            }
        }
        catch (Exception exception)
        {
            HandleException(exception);
        }
    }


    protected void BuyButton_Click(object sender, EventArgs e)
    {
        if (Contact != null)
        {
            try
            {
                ICredentialProvider credentialProvider = new UserCredentialProvider(MembershipContext.AuthenticatedUser);
                NetworkCredential userCredential = credentialProvider.GetCredential();
                if (userCredential != null)
                {
                    DataComClient client = DataComHelper.CreateClient();
                    IContactProvider provider = new UserContactProvider(client, userCredential);
                    Contact contact = provider.GetContact(Contact.ContactId, true);
                    JsonSerializer serializer = new JsonSerializer();
                    ContactHiddenField.Value = serializer.Serialize(contact);
                }
                else
                {
                    ErrorSummary.Report(GetString("datacom.nousercredential"));
                    BuyButton.Enabled = false;
                }
            }
            catch (Exception exception)
            {
                HandleException(exception);
            }
        }
    }

    #endregion


    #region "Private methods"

    /// <summary>
    /// Restores parameters that were passed on to this dialog.
    /// </summary>
    private void RestoreParameters()
    {
        // Validate parameters
        if (!QueryHelper.ValidateHash("hash"))
        {
            throw new Exception("[DataComBuyContactPage.RestoreParameters]: Invalid query hash.");
        }

        var firstName = QueryHelper.GetString("ContactFirstName", "");
        var lastName = QueryHelper.GetString("ContactLastName", "");
        var company = QueryHelper.GetString("ContactCompany", "");
        var contactID = QueryHelper.GetInteger("ContactID", 0);

        if (contactID <= 0)
        {
            throw new Exception("[DataComBuyContactPage.RestoreParameters]: Invalid query data.");
        }

        Contact = new Contact { FirstName = firstName, LastName = lastName, CompanyName = company, ContactId = contactID };

        // Restore site identifier
        SiteIdentifierParameter = QueryHelper.GetInteger("SiteID", 0);
    }


    /// <summary>
    /// Initializes the preview control with the values from the specified Data.com contact.
    /// </summary>
    /// <param name="contact">Data.com contact.</param>
    private void InitializeContactPreview(Contact contact)
    {
        FirstNameLiteral.Text = HTMLHelper.HTMLEncode(contact.FirstName);
        LastNameLiteral.Text = HTMLHelper.HTMLEncode(contact.LastName);
        CompanyNameLiteral.Text = HTMLHelper.HTMLEncode(contact.CompanyName);
    }


    /// <summary>
    /// Initializes the account status area with the number of points.
    /// </summary>
    /// <param name="points">A number of points on Data.com user or partner account.</param>
    private void InitializeAccountPoints(long points)
    {
        AccountPointsLiteral.Text = ResHelper.Format("datacom.buycontact.accountpoints", points);
    }


    /// <summary>
    /// Initializes the account status area with the url to purchase points.
    /// </summary>
    /// <param name="url">An url to purchase points.</param>
    private void InitializePurchasePointsLink(string url)
    {
        PurchasePointsLink.NavigateUrl = url;
        PurchasePointsLink.Visible = true;
    }


    /// <summary>
    /// Displays an error message and logs the specified exception to the event log.
    /// </summary>
    /// <param name="exception">The exception to handle.</param>
    private void HandleException(Exception exception)
    {
        ErrorSummary.Report(exception);
        EventLogProvider.LogException("Data.com Connector", "BuyContactPage", exception);
    }

    #endregion

}