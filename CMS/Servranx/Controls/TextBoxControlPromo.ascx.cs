using System;
using System.Web.UI.WebControls;
using System.Collections;
using System.Collections.Generic;

using AjaxControlToolkit;

using CMS.CMSHelper;
using CMS.FormControls;
using CMS.FormEngine;
using CMS.GlobalHelper;
using CMS.PortalControls;
using CMS.SettingsProvider;
using CMS.ExtendedControls;
using CMS.EventLog;
using CMS.Ecommerce;
using System.Data;
using CMS.Helpers;

public partial class Servranx_Controls_TextBoxControlPromo : FormEngineUserControl
{
    /// <summary>
    /// Gets or sets the value entered into the field, a hexadecimal color code in this case.
    /// </summary>
    public override Object Value
    {
        get
        {
            return drpColor.SelectedValue;
        }
        set
        {
            // Ensure drop down list options
            EnsureItems();
            drpColor.SelectedValue = System.Convert.ToString(value);
        }
    }

    /// <summary>
    /// Property used to access the Width parameter of the form control.
    /// </summary>
    public int SelectorWidth
    {
        get
        {
            return ValidationHelper.GetInteger(GetValue("SelectorWidth"), 0);
        }
        set
        {
            SetValue("SelectorWidth", value);
        }
    }

    /// <summary>
    /// Returns an array of values of any other fields returned by the control.
    /// </summary>
    /// <returns>It returns an array where the first dimension is the attribute name and the second is its value.</returns>
    public override object[,] GetOtherValues()
    {
        object[,] array = new object[1, 2];
        array[0, 0] = "SKURetailPrice";
        array[0, 1] = drpColor.SelectedItem.Text;
        return array;
    }

    /// <summary>
    /// Returns true if a color is selected. Otherwise, it returns false and displays an error message.
    /// </summary>
    public override bool IsValid()
    {
        /*if ((string)Value != "")
        {
            //CreateVolumeDiscount();
            return true;
        }
        else
        {
            // Set form control validation error message.
            this.ValidationError = "Erreur";
            return false;
        }*/
		return true;
    }


    /// <summary>
    /// Sets up the internal DropDownList control.
    /// </summary>
    protected void EnsureItems()
    {
        // Applies the width specified through the parameter of the form control if it is valid.
        if (SelectorWidth > 0)
        {
            drpColor.Width = SelectorWidth;
        }

        if (drpColor.Items.Count == 0)
        {
            drpColor.Items.Add(new ListItem("(select color)", ""));
            drpColor.Items.Add(new ListItem("10.00", "10.00"));
            drpColor.Items.Add(new ListItem("20.00", "20.00"));
            drpColor.Items.Add(new ListItem("30.00", "30.00"));
        }
    }

    /// <summary>
    /// Handler for the Load event of the control.
    /// </summary>
    protected void Page_Load(object sender, EventArgs e)
    {
        // Ensure drop-down list options
        EnsureItems();
    }
    protected bool CreateVolumeDiscount()
    {
        // Prepare the parameters
        string where = "SKUName LIKE '12 �prouvettes'";
        SKUInfo product = null;

        // Get the product
        DataSet products = SKUInfoProvider.GetSKUs(where, null);
        if (!DataHelper.DataSourceIsEmpty(products))
        {
            product = new SKUInfo(products.Tables[0].Rows[0]);
        }

        if (product != null)
        {
            // Create new volume discount object
            VolumeDiscountInfo newDiscount = new VolumeDiscountInfo();

            // Set the properties
            newDiscount.VolumeDiscountMinCount = 1;
            newDiscount.VolumeDiscountValue = 10;
            newDiscount.VolumeDiscountSKUID = product.SKUID;
            newDiscount.VolumeDiscountIsFlatValue = false;

            // Create the volume discount
            VolumeDiscountInfoProvider.SetVolumeDiscountInfo(newDiscount);

            return true;
        }

        return false;
    }
}