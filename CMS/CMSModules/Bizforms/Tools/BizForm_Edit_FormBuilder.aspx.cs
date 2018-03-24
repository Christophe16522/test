using System;
using System.Web.UI.WebControls;

using CMS.DataEngine;
using CMS.FormEngine;
using CMS.Helpers;
using CMS.OnlineForms;
using CMS.UIControls;

// Edited object
[EditedObject(BizFormInfo.OBJECT_TYPE, "formId")]
[Security(Resource = "CMS.Form", Permission = "ReadForm")]
[UIElement("CMS.Form", "Forms.FormBuldier")]
public partial class CMSModules_BizForms_Tools_BizForm_Edit_FormBuilder : CMSBizFormPage
{
    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        BizFormInfo bfi = EditedObject as BizFormInfo;

        if (bfi == null)
        {
            return;
        }

        FormBuilder.ClassName = DataClassInfoProvider.GetClassName(bfi.FormClassID);

        // Prepare submit text
        string submitText = null;
        if (!string.IsNullOrEmpty(bfi.FormSubmitButtonText))
        {
            submitText = ResHelper.LocalizeString(bfi.FormSubmitButtonText);
        }

        // Init submit button
        if (!string.IsNullOrEmpty(bfi.FormSubmitButtonImage))
        {
            ImageButton imageButton = FormBuilder.Form.SubmitImageButton;
            // Image button
            imageButton.ImageUrl = bfi.FormSubmitButtonImage;

            if (submitText != null)
            {
                imageButton.AlternateText = submitText;
                imageButton.ToolTip = submitText;
            }
        }
        else
        {
            // Standard button
            if (submitText != null)
            {
                FormBuilder.Form.SubmitButton.ResourceString = submitText;
            }
        }

        ScriptHelper.HideVerticalTabs(this);
    }
}