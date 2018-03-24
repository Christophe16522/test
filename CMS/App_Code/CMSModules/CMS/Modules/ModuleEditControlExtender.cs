using System;
using System.Linq;

using CMS;
using CMS.Base;
using CMS.ExtendedControls;
using CMS.FormControls;
using CMS.Modules;


[assembly: RegisterCustomClass("ModuleEditControlExtender", typeof(ModuleEditControlExtender))]

/// <summary>
/// Permission edit control extender
/// </summary>
public class ModuleEditControlExtender : ControlExtender<UIForm>
{
    /// <summary>
    /// Gets the current resource
    /// </summary>
    public ResourceInfo Resource
    {
        get
        {
            return Control.UIContext.EditedObject as ResourceInfo;
        }
    }


    /// <summary>
    /// OnInit event handler
    /// </summary>
    public override void OnInit()
    {
        // Set the state of new module to development
        Control.OnBeforeSave += (sender, args) =>
        {
            if ((Resource == null) || (Resource.ResourceId <= 0))
            {
                Control.Data["ResourceIsInDevelopment"] = true;
            }
        };

        // Disable editing when module is not editable
        Control.Load += (sender, args) =>
        {
            if (Resource != null)
            {
                // Display warning for installed resources
                if ((Resource.ResourceId > 0) && !Resource.IsEditable)
                {
                    Control.SubmitButton.Enabled = Control.Enabled = false;
                    Control.ShowInformation(Control.GetString("resource.installedresourcewarning"));
                }
                // Display warning for system resources
                else if (!SystemContext.DevelopmentMode && ResourceInfoProvider.IsSystemResource(Resource.ResourceName))
                {
                    Control.SubmitButton.Enabled = Control.Enabled = false;
                    Control.ShowInformation(Control.GetString("resource.cusomresourcewarning"));
                }
            }
        };
    }
}