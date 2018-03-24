using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CMS.SiteProvider;
using CMS.CMSHelper;
using System.Data;
using CMS.GlobalHelper;
using CMS.Ecommerce;
using CMS.Membership;
using CMS.Helpers;

public partial class Servranx_WebUserControl : System.Web.UI.UserControl
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }
    protected void Button1_Click(object sender, EventArgs e)
    {
        GetAndBulkUpdateUsers();

    }
    private void GetAndBulkUpdateUsers()
    {
        // Prepare the parameters
        

        // Get the data
        DataSet users = UserInfoProvider.GetUsers(null, null);
        if (!DataHelper.DataSourceIsEmpty(users))
        {
            // Loop through the individual items
            foreach (DataRow userDr in users.Tables[0].Rows)
            {
                // Create object from DataRow
                UserInfo modifyUser = new UserInfo(userDr);

                // Update the properties
               // modifyUser.FullName = modifyUser.FullName.ToUpper();

                // Save the changes
               // UserInfoProvider.SetUserInfo(modifyUser);
                int userId = modifyUser.UserID;
                int siteId = CMSContext.CurrentSiteID;

                // Save the binding
                UserSiteInfoProvider.AddUserToSite(userId, siteId);
            }

          
        }

       
    }
    private void DeleteOrder()
    {
        // Prepare the parameters
        //string where = "CustomerLastName LIKE N'My New Registered%'";
        CustomerInfo customer = null;

        // Get the customer
        DataSet customers = CustomerInfoProvider.GetCustomers(null, null);
        if (!DataHelper.DataSourceIsEmpty(customers))
        {
            // Create object from DataRow
            customer = new CustomerInfo(customers.Tables[0].Rows[0]);
        }

        if (customer != null)
        {
          //  string whereOrder = "OrderCustomerID='" + customer.CustomerID + "'";

            // Get the order
            DataSet orders = OrderInfoProvider.GetOrders(null, null);
            if (!DataHelper.DataSourceIsEmpty(orders))
            {
                // Create object from DataRow
              //  OrderInfo order = new OrderInfo(orders.Tables[0].Rows[0]);
 foreach (DataRow userDr in orders.Tables[0].Rows)
            {
			OrderInfo order = new OrderInfo(userDr);
                // Delete the order
                OrderInfoProvider.DeleteOrderInfo(order);
            }
              
            }
        }

       
    }

    protected void Button2_Click(object sender, EventArgs e)
    {
        DeleteOrder();
    }

    protected void Button3_Click(object sender, EventArgs e)
    {
        DeleteCustomer();
    }
    protected void Button4_Click(object sender, EventArgs e)
    {
        DeleteAddress();
    }
    private void DeleteCustomer()
    {
        // Prepare the parameters
       // string where = "CustomerLastName LIKE N'My New%'";

        // Delete user
      //  UserInfo user = UserInfoProvider.GetUserInfo("My new user");
     //   UserInfoProvider.DeleteUser(user);

        // Get the data
        DataSet customers = CustomerInfoProvider.GetCustomers(null, null);
        if (!DataHelper.DataSourceIsEmpty(customers))
        {
            foreach (DataRow customerDr in customers.Tables[0].Rows)
            {
                // Create object from DataRow
                CustomerInfo deleteCustomer = new CustomerInfo(customerDr);

                // Delete the customer
                CustomerInfoProvider.DeleteCustomerInfo(deleteCustomer);
            }

            //return true;
        }

        //return false;
    }
    private void DeleteAddress()
    {
        // Prepare the parameters
        //string where = "AddressName LIKE 'My New%'";

        // Get the address
        DataSet addresses = AddressInfoProvider.GetAddresses(null, null);
        if (!DataHelper.DataSourceIsEmpty(addresses))
        {
            // Create object from DataRow
            AddressInfo updateAddress = new AddressInfo(addresses.Tables[0].Rows[0]);

            // Delete the address
            AddressInfoProvider.DeleteAddressInfo(updateAddress);

           // return true;
        }

       // return false;
    }
}