using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.CMSHelper;
using CMS.Ecommerce;
using CMS.EcommerceProvider;
using CMS.GlobalHelper;
using CMS.LicenseProvider;
using CMS.SettingsProvider;
using CMS.SiteProvider;
using CMS.Helpers;
using CMS.DataEngine;
using CMS.Membership;

public partial class CMSModules_Ecommerce_Controls_ShoppingCart_ShoppingCartCustomerSelection : ShoppingCartStep
{
    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        // Check feature
        if (DataHelper.GetNotEmpty(URLHelper.GetCurrentDomain(), "") != "")
        {
            LicenseHelper.CheckFeatureAndRedirect(URLHelper.GetCurrentDomain(), FeatureEnum.Ecommerce);
        }

        // Mark previously selected customer
        if ((!ShoppingCartControl.IsCurrentStepPostBack) && (ShoppingCart.ShoppingCartCustomerID > 0))
        {
            customerSelector.CustomerID = ShoppingCart.ShoppingCartCustomerID;
        }

        ShoppingCartControl.ButtonBack.Visible = false;
        customerSelector.IsLiveSite = IsLiveSite;
        ucCustomerNew.MessagesPlaceHolder.BasicStyles = true;
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        customerSelector.Visible = radSelectCustomer.Checked;
        ucCustomerNew.Visible = radCreateCustomer.Checked;
        lblSelectError.Visible = !string.IsNullOrEmpty(lblSelectError.Text);
    }


    /// <summary>
    /// Check if form is valid.
    /// </summary>
    public override bool IsValid()
    {
        if (radSelectCustomer.Checked)
        {
            // Check if customer is selected
            if (customerSelector.CustomerID > 0)
            {
                return true;
            }
            else
            {
                lblSelectError.Text = GetString("shoppingcartselectcustomer.errorselect");
                return false;
            }
        }

        if (radCreateCustomer.Checked)
        {
            // Check if new customer form is valid
            return ucCustomerNew.IsValid();
        }

        return true;
    }


    /// <summary>
    /// Process form data.
    /// </summary>
    public override bool ProcessStep()
    {
        int customerId = 0;

        if (radSelectCustomer.Checked)
        {
            // Select an existing customer
            customerId = customerSelector.CustomerID;
        }

        if (radCreateCustomer.Checked)
        {
            // Check permissions
            if (!ECommerceContext.IsUserAuthorizedToModifyCustomer())
            {
                RedirectToAccessDenied("CMS.Ecommerce", "ModifyCustomers OR EcommerceModify");
                return false;
            }

            // Create a new customer
            customerId = ucCustomerNew.Save();
        }

        // Assign customer and user to the shopping cart
        if (customerId > 0)
        {
            CustomerInfo customer = CustomerInfoProvider.GetCustomerInfo(customerId);

            if (customer != null)
            {
                ShoppingCart.ShoppingCartCustomerID = customer.CustomerID;

                if (customer.CustomerUserID > 0)
                {
                    UserInfo user = UserInfoProvider.GetUserInfo(customer.CustomerUserID);

                    if (user == null)
                    {
                        user = AuthenticationHelper.GlobalPublicUser;
                    }

                    ShoppingCart.User = user;
                }

                return true;
            }
        }

        return false;
    }
}