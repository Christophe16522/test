using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Collections;

using CMS.ExtendedControls;
using CMS.FormEngine;
using CMS.GlobalHelper;
using CMS.PortalControls;
using CMS.PortalEngine;
using CMS.SettingsProvider;
using CMS.Helpers;

public partial class CMSModules_Widgets_InlineControl_InlineWidget : InlineUserControl
{
    protected void Page_Load(object sender, EventArgs e)
    {
        StringSafeDictionary<object> decodedProperties = new StringSafeDictionary<object>();
        foreach (DictionaryEntry param in mProperties)
        {
            // Decode special CK editor char
            String str = String.Empty;
            if (param.Value != null)
            {
                str = param.Value.ToString().Replace("%25", "%");                
            }

            decodedProperties[param.Key] = HttpUtility.UrlDecode(str);
        }
        mProperties = decodedProperties;

        string widgetName = ValidationHelper.GetString(mProperties["name"], String.Empty);

        // Widget name must be specified
        if (String.IsNullOrEmpty(widgetName))
        {
            AddErrorWebPart("widgets.invalidname", null);
            return;
        }

        WidgetInfo wi = WidgetInfoProvider.GetWidgetInfo(widgetName);
        if (wi == null)
        {
            AddErrorWebPart("widget.failedtoload", null);
            return;
        }

        WebPartInfo wpi = WebPartInfoProvider.GetWebPartInfo(wi.WidgetWebPartID);
        if (wpi == null)
        {
            return;
        }

        //no widgets can be used as inline
        if (!wi.WidgetForInline)
        {
            AddErrorWebPart("widgets.cantbeusedasinline", null);
            return;
        }

        try
        {
            // Merge widget and it's parent webpart properties
            string properties = FormHelper.MergeFormDefinitions(wpi.WebPartProperties, wi.WidgetProperties);

            // Prepare form
            WidgetZoneTypeEnum zoneType = WidgetZoneTypeEnum.Editor;
            FormInfo zoneTypeDefinition = PortalFormHelper.GetPositionFormInfo(zoneType);
            FormInfo fi = PortalFormHelper.GetWidgetFormInfo(wi.WidgetName, Enum.GetName(typeof(WidgetZoneTypeEnum), zoneType), properties, zoneTypeDefinition, true);

            // Apply changed values
            DataRow dr = fi.GetDataRow();
            fi.LoadDefaultValues(dr);
            fi.LoadDefaultValues(dr, wi.WidgetDefaultValues);

            // Incorporate inline parameters to datarow
            string publicFields = ";containertitle;container;";
            if (wi.WidgetPublicFileds != null)
            {
                publicFields += ";" + wi.WidgetPublicFileds.ToLowerCSafe() + ";";
            }

            // Load the widget control
            string url = null;

            // Set widget layout
            WebPartLayoutInfo wpli = WebPartLayoutInfoProvider.GetWebPartLayoutInfo(wi.WidgetLayoutID);

            if (wpli != null)
            {
                // Load specific layout through virtual path
                url = wpli.GetVirtualFileRelativePath(WebPartLayoutInfo.EXTERNAL_COLUMN_CODE, wpli.WebPartLayoutVersionGUID);
            }
            else
            {
                // Load regularly
                url = WebPartInfoProvider.GetWebPartUrl(wpi, false);
            }

            CMSAbstractWebPart control = (CMSAbstractWebPart)Page.LoadUserControl(url);
            control.PartInstance = new WebPartInstance();

            // Set all form values to webpart          
            foreach (DataColumn column in dr.Table.Columns)
            {
                object value = dr[column];
                string columnName = column.ColumnName.ToLowerCSafe();

                //Resolve set values by user
                if (mProperties.Contains(columnName))
                {
                    FormFieldInfo ffi = fi.GetFormField(columnName);
                    if ((ffi != null) && ffi.Visible && (!ffi.DisplayIn.Contains(FormInfo.DISPLAY_CONTEXT_DASHBOARD)))
                    {
                        value = mProperties[columnName];
                    }                   
                }
                
                // Resolve macros in defined in default values
                if (!String.IsNullOrEmpty(value as string))
                {
                    // Do not resolve macros for public fields
                    if (publicFields.IndexOfCSafe(";" + columnName + ";") < 0)
                    {
                        // Check whether current column 
                        bool avoidInjection = control.SQLProperties.Contains(";" + columnName + ";");

                        // Resolve macros
                        value = control.ContextResolver.ResolveMacros(value.ToString(), avoidInjection);
                    }
                }

                control.PartInstance.SetValue(column.ColumnName, value);
            }

            control.PartInstance.IsWidget = true;

            // Load webpart content
            control.OnContentLoaded();

            // Add webpart to controls collection
            Controls.Add(control);

            // Handle DisableViewstate setting
            control.EnableViewState = !control.DisableViewState;
        }

        catch (Exception ex)
        {
            AddErrorWebPart("widget.failedtoload", ex);
        }
    }


    /// <summary>
    /// Add error web part to collection.
    /// </summary>
    /// <param name="tittle">Tittle of webpart</param>
    public void AddErrorWebPart(string tittle, Exception ex)
    {
        WebPartError err = new WebPartError();
        //If ex is defined, let ex.message show the error type    
        err.ErrorTitle = GetString(tittle);
        if (ex != null)
        {
            err.InnerException = ex;
        }
        Controls.Add(err);

        // Load webpart content
        err.OnContentLoaded();
    }
}