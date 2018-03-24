using System;

using CMS.Core;
using CMS.ExtendedControls;
using CMS.FormEngine;
using CMS.Helpers;
using CMS.PortalEngine;
using CMS.UIControls;

[Title("Development_FormUserControl_Edit.Title")]
[UIElement(ModuleName.BIZFORM, "New")]
public partial class CMSModules_FormControls_Pages_Development_New : GlobalAdminPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!RequestHelper.IsPostBack())
        {
            drpTypeSelector.ControlType = FormUserControlTypeEnum.Input;
        }
        CurrentMaster.DisplaySiteSelectorPanel = true;
        rfvControlName.ErrorMessage = GetString("Development_FormUserControl_Edit.rfvCodeName");
        rfvControlDisplayName.ErrorMessage = GetString("Development_FormUserControl_Edit.rfvDisplayName");

        // Initialize breadcrumbs
        PageBreadcrumbs.Items.Add(new BreadcrumbItem()
        {
            Text = GetString("Development_FormUserControl_Edit.Controls"),
            RedirectUrl = "~/CMSModules/FormControls/Pages/Development/List.aspx"
        });

        PageBreadcrumbs.Items.Add(new BreadcrumbItem()
        {
            Text = GetString("Development_FormUserControl_Edit.New"),
        });

        // Initialize file selector
        tbFileName.DefaultPath = "CMSFormControls";
        tbFileName.AllowedExtensions = "ascx";
        tbFileName.ShowFolders = false;
        tbFileName.AllowEmptyValue = false;
        tbFileName.DefaultPath = "CMSFormControls";
        tbFileName.ValidationError = GetString("Development_FormUserControl_Edit.rfvFileName"); 
    }


    /// <summary>
    /// Handles btnOK's OnClick event.
    /// </summary>
    protected void btnOK_Click(object sender, EventArgs e)
    {
        // Finds whether required fields are not empty
        string result = new Validator().NotEmpty(txtControlName.Text, rfvControlName.ErrorMessage).NotEmpty(txtControlDisplayName, rfvControlDisplayName.ErrorMessage).Result;

        // Check input file validity
        if (String.IsNullOrEmpty(result) && radNewControl.Checked)
        {
            if (!tbFileName.IsValid())
            {
                result = tbFileName.ValidationError;
            }
        }

        // Try to create new form control if everything is OK
        if (String.IsNullOrEmpty(result))
        {
            FormUserControlInfo fi = new FormUserControlInfo();
            fi.UserControlDisplayName = txtControlDisplayName.Text.Trim();
            fi.UserControlCodeName = txtControlName.Text.Trim();
            fi.UserControlType = drpTypeSelector.ControlType;
            fi.UserControlForText = false;
            fi.UserControlForLongText = false;
            fi.UserControlForInteger = false;
            fi.UserControlForLongInteger = false;
            fi.UserControlForDecimal = false;
            fi.UserControlForDateTime = false;
            fi.UserControlForBoolean = false;
            fi.UserControlForFile = false;
            fi.UserControlForDocAttachments = false;
            fi.UserControlForGUID = false;
            fi.UserControlForVisibility = false;
            fi.UserControlShowInBizForms = false;
            fi.UserControlDefaultDataType = "Text";

            // Inherited user control
            if (radInheritedControl.Checked)
            {
                fi.UserControlParentID = ValidationHelper.GetInteger(drpFormControls.Value, 0);

                // Create empty default values definition
                fi.UserControlParameters = "<form></form>";
                fi.UserControlFileName = "inherited";
            }
            else
            {
                fi.UserControlFileName = tbFileName.Value.ToString();
            }

            try
            {
                FormUserControlInfoProvider.SetFormUserControlInfo(fi);

                // If control was successfully created then redirect to editing page
                string url = UIContextHelper.GetElementUrl("CMS.Form", "Edit", false);
                url = URLHelper.AddParameterToUrl(url, "objectid", fi.UserControlID.ToString());
                URLHelper.Redirect(url);
            }
            catch (Exception ex)
            {
                ShowError(ex.Message.Replace("%%name%%", fi.UserControlCodeName));
            }
        }
        else
        {
            ShowError(result);
        }
    }


    protected void radNewFormControl_CheckedChanged(object sender, EventArgs e)
    {
        plcFileName.Visible = radNewControl.Checked;
        plcFormControls.Visible = radInheritedControl.Checked;
    }
}