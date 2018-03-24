using System;

using CMS.Core;
using CMS.DataEngine;
using CMS.Ecommerce;
using CMS.Helpers;
using CMS.PortalEngine;
using CMS.SiteProvider;
using CMS.UIControls;

[Title("supplier_Edit.ItemListLink")]
[Action(0, "supplier_list.newitemcaption", "{%UIContextHelper.GetElementUrl(\"CMS.Ecommerce\", \"new.supplier\", false)|(encode)false%}")]
[UIElement(ModuleName.ECOMMERCE, "Suppliers")]
public partial class CMSModules_Ecommerce_Pages_Tools_Suppliers_Supplier_List : CMSSuppliersPage
{
    #region "Page Events"

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        // Init Unigrid
        gridElem.OnAction += gridElem_OnAction;
        HandleGridsSiteIDColumn(gridElem);
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        InitWhereCondition();
    }

    #endregion


    #region "Event Handlers"

    /// <summary>
    /// Handles the UniGrid's OnAction event.
    /// </summary>
    /// <param name="actionName">Name of item (button) that throws event</param>
    /// <param name="actionArgument">ID (value of Primary key) of corresponding data row</param>
    protected void gridElem_OnAction(string actionName, object actionArgument)
    {
        int supplierId = ValidationHelper.GetInteger(actionArgument, 0);

        if (actionName == "edit")
        {
            URLHelper.Redirect(UIContextHelper.GetElementUrl("cms.ecommerce", "edit.supplier", false, supplierId));
        }
        else if (actionName == "delete")
        {
            var supplierObj = SupplierInfoProvider.GetSupplierInfo(supplierId);

            if (supplierObj == null)
            {
                return;
            }

            // Check module permissions
            if (!ECommerceContext.IsUserAuthorizedToModifySupplier(supplierObj))
            {
                if (supplierObj.IsGlobal)
                {
                    RedirectToAccessDenied("CMS.Ecommerce", "EcommerceGlobalModify");
                }
                else
                {
                    RedirectToAccessDenied("CMS.Ecommerce", "EcommerceModify OR ModifySuppliers");
                }
            }

            if (supplierObj.Generalized.CheckDependencies())
            {
                // Show error message
                ShowError(GetString("Ecommerce.DeleteDisabled"));

                return;
            }

            // Delete SupplierInfo object from database
            SupplierInfoProvider.DeleteSupplierInfo(supplierObj);
        }
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Creates where condition for UniGrid 
    /// </summary>
    private void InitWhereCondition()
    {
        var where = "SupplierSiteID = " + SiteContext.CurrentSiteID;
        if (AllowGlobalObjects)
        {
            where = SqlHelper.AddWhereCondition(where, "SupplierSiteID IS NULL", "OR");
        }

        gridElem.WhereCondition = where;
    }

    #endregion
}