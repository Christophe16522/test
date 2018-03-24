using System;
using System.Web.UI.WebControls;

using CMS.Base;
using CMS.DataEngine;
using CMS.EventLog;
using CMS.FormControls;
using CMS.Helpers;

public partial class CMSFormControls_Filters_NumberFilter : FormEngineUserControl
{
    protected string mOperatorFieldName = null;


    #region "Properties"

    /// <summary>
    /// Gets or sets value.
    /// </summary>
    public override object Value
    {
        get
        {
            return txtText.Text;
        }
        set
        {
            // Load default value on insert
            if ((FieldInfo != null) && (FieldInfo.DataType == FieldDataType.Double))
            {
                Double dblVal = ValidationHelper.GetDoubleSystem(value, Double.NaN);
                txtText.Text = !Double.IsNaN(dblVal) ? dblVal.ToString() : string.Empty;
            }
            else
            {
                txtText.Text = ValidationHelper.GetString(value, null);
            }
        }
    }
    

    /// <summary>
    /// Gets name of the field for operator value. Default value is 'Operator'.
    /// </summary>
    protected string OperatorFieldName
    {
        get
        {
            if (string.IsNullOrEmpty(mOperatorFieldName))
            {
                // Get name of the field for operator value
                mOperatorFieldName = DataHelper.GetNotEmpty(GetValue("OperatorFieldName"), "Operator");
            }
            return mOperatorFieldName;
        }
    }


    /// <summary>
    /// Gets or sets default operator to use for the first inicialization of the control.
    /// </summary>
    public string DefaultOperator
    {
        get;
        set;
    }

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        CheckFieldEmptiness = false;
        InitFilterDropDown();

        if (ContainsColumn(OperatorFieldName))
        {
            drpOperator.SelectedValue = ValidationHelper.GetString(Form.Data.GetValue(OperatorFieldName), "0");
        }
        else
        {
            // Set default operator
            if (!RequestHelper.IsPostBack() && (DefaultOperator != null))
            {
                drpOperator.SelectedValue = DefaultOperator;
            }
        }
    }


    /// <summary>
    /// Returns other values related to this form control.
    /// </summary>
    /// <returns>Returns an array where first dimension is attribute name and the second dimension is its value.</returns>
    public override object[,] GetOtherValues()
    {
        if (Form.Data is DataRowContainer)
        {
            if (!ContainsColumn(OperatorFieldName))
            {
                Form.DataRow.Table.Columns.Add(OperatorFieldName);
            }

            // Set properties names
            object[,] values = new object[3,2];
            values[0, 0] = OperatorFieldName;
            values[0, 1] = drpOperator.SelectedValue;
            return values;
        }
        return null;
    }


    /// <summary>
    /// Initializes operator filter dropdown list.
    /// </summary>
    private void InitFilterDropDown()
    {
        if (drpOperator.Items.Count == 0)
        {
            drpOperator.Items.Add(new ListItem(GetString("filter.equals"), "="));
            drpOperator.Items.Add(new ListItem(GetString("filter.notequals"), "<>"));
            drpOperator.Items.Add(new ListItem(GetString("filter.lessthan"), "<"));
            drpOperator.Items.Add(new ListItem(GetString("filter.lessorequal"), "<="));
            drpOperator.Items.Add(new ListItem(GetString("filter.greaterthan"), ">"));
            drpOperator.Items.Add(new ListItem(GetString("filter.greaterorequal"), ">="));
        }
    }


    /// <summary>
    /// Gets where condition.
    /// </summary>
    public override string GetWhereCondition()
    {
        string tempVal = ValidationHelper.GetString(Value, string.Empty).Trim();
        string op = drpOperator.SelectedValue;

        // No condition
        if (string.IsNullOrEmpty(tempVal) || string.IsNullOrEmpty(op))
        {
            return null;
        }

        // Only number in integer on double range
        object value = null;
        if (ValidationHelper.IsInteger(tempVal))
        {
            value = ValidationHelper.GetInteger(tempVal, 0);
        }
       
        if (value != null)
        {
            if (String.IsNullOrEmpty(WhereConditionFormat))
            {
                WhereConditionFormat = "[{0}] {2} {1}";
            }

            try
            {
                // Format where condition
                return string.Format(WhereConditionFormat, FieldInfo.Name, value, op);
            }
            catch (Exception ex)
            {
                // Log exception
                EventLogProvider.LogException("NumberFilter", "GetWhereCondition", ex);
            }
        }
        
        return null;
    }

    #endregion
}