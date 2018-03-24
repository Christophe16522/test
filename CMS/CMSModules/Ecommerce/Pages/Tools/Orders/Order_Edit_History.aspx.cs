using System;
using System.Data;
using System.Web.UI.WebControls;

using CMS.Core;
using CMS.Ecommerce;
using CMS.Helpers;
using CMS.UIControls;

[UIElement(ModuleName.ECOMMERCE, "Orders.History")]
public partial class CMSModules_Ecommerce_Pages_Tools_Orders_Order_Edit_History : CMSEcommercePage
{
    #region "Page Events"

    protected void Page_Load(object sender, EventArgs e)
    {
        int orderId = QueryHelper.GetInteger("orderid", 0);

        OrderInfo oi = OrderInfoProvider.GetOrderInfo(orderId);
        EditedObject = oi;
        
        // Check order site ID
        CheckEditedObjectSiteID(oi.OrderSiteID);

        gridElem.GridView.RowDataBound += gridElem_RowDataBound;
        gridElem.WhereCondition = "OrderID = " + orderId;
        gridElem.ShowActionsMenu = true;
    }

    #endregion


    #region "Event handlers"

    protected void gridElem_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            int orderStatId = ValidationHelper.GetInteger(((DataRowView)(e.Row.DataItem)).Row["ToStatusID"], 0);

            OrderStatusInfo status = OrderStatusInfoProvider.GetOrderStatusInfo(orderStatId);
            if (status != null)
            {
                e.Row.Style.Add("background-color", status.StatusColor);
            }
        }
    }

    #endregion
}