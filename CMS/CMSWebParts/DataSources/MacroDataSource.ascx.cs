using CMS.Helpers;
using CMS.PortalControls;

public partial class CMSWebParts_DataSources_MacroDataSource : CMSAbstractWebPart
{
    #region "Properties"

    /// <summary>
    /// Macro expression returning data to be provided by the data source..
    /// </summary>
    public string Expression
    {
        get
        {
            return ValidationHelper.GetString(this.GetValue("Expression"), "");
        }
        set
        {
            this.SetValue("Expression", value);
        }
    }
    
    #endregion


    #region "Methods"

    /// <summary>
    /// Content loaded event handler.
    /// </summary>
    public override void OnContentLoaded()
    {
        base.OnContentLoaded();
        SetupControl();
    }


    /// <summary>
    /// Initializes the control properties.
    /// </summary>
    protected void SetupControl()
    {
        if (StopProcessing)
        {
            // Do nothing
        }
        else
        {
            srcMacro.FilterName = ValidationHelper.GetString(GetValue("WebPartControlID"), ID);
            srcMacro.Expression = Expression;
        }
    }


    /// <summary>
    /// Clears cache.
    /// </summary>
    public override void ClearCache()
    {
        srcMacro.ClearCache();
    }

    #endregion
}