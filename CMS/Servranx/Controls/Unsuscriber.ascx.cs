using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using CMS.GlobalHelper;
using CMS.CMSHelper;
using CMS.SiteProvider;
using CMS.Newsletter;
using CMS.SettingsProvider;
using System.Globalization;
using CMS.Newsletters;
using CMS.Helpers;
public partial class Servranx_Controls_Unsuscriber : System.Web.UI.UserControl
{
    string temp = "";
    protected void Page_Load(object sender, EventArgs e)
    {
        //if (temp == "not found")
        //{
        //    lblerror.Text = "";
        //}
btnSearh.ImageUrl="~/App_Themes/Servranx/images/BtnEnvoyerLvl0.gif";
    }
    protected void btnSearh_Click(object sender, ImageClickEventArgs e)
    {
       // btnSearh.Attributes.Add("onclick", "return confirm('Voulez-vous confirmer la desinscription à la newsletter de Alutec')");
        UnsubscribeFromNewsletter();
         DeleteSubscriber();


    }
    private bool UnsubscribeFromNewsletter()
    {
        string SubscriberMail = txtmail.Text;
        string newlettername = string.Empty;
        bool result = false;
        // Gets the subscriber and newsletter
        SubscriberInfo subscriber = SubscriberInfoProvider.GetSubscriberInfo(SubscriberMail, CMSContext.CurrentSiteID);
        if (subscriber != null)
        {
            DataSet newsletters = NewsletterInfoProvider.GetNewsletters(null, null, 0, null);
            if (!DataHelper.DataSourceIsEmpty(newsletters))
            {
                // Loops through the items
                foreach (DataRow newsletterDr in newsletters.Tables[0].Rows)
                {
                    newlettername = newsletterDr["NewsletterName"].ToString();
                    NewsletterInfo newsletter = NewsletterInfoProvider.GetNewsletterInfo(newlettername, CMSContext.CurrentSiteID);

                    if (newsletter != null)
                    {
                        // Unubscribes from 'My new static newsletter'
                        SubscriberInfoProvider.Unsubscribe(subscriber.SubscriberID, newsletter.NewsletterID);
                        result = true;
                        break;
                    }
                }
            }
            lblerror.Text = "La désinscription a été effectuée avec succès.";
            temp = "found";
        }
        else {
            lblerror.Text = "L'email n'a pas été trouvé dans la base de données. Veuillez réessayer. ";
            txtmail.Text = "";
            temp = "not found";
        }
        return result;
    }

    private bool DeleteSubscriber()
    {
        string SubscriberMail = txtmail.Text.ToString();
        // Gets the subscriber
        SubscriberInfo deleteSubscriber = SubscriberInfoProvider.GetSubscriberInfo(SubscriberMail, CMSContext.CurrentSiteID);
        // Deletes the subscriber
        SubscriberInfoProvider.DeleteSubscriberInfo(deleteSubscriber);
        //modif Begin
       
        //txtmail.Text = "";
        //modif End
        return (deleteSubscriber != null);
        //if (temp == "found")
        //{
        //    lblerror.Text = "La désinscription a été effectuée avec succès.";
        //}
    }
}