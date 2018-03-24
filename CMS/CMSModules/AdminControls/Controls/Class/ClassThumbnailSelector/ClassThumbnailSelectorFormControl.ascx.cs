using System;
using System.Linq;
using System.Web.UI;

using CMS.DataEngine;
using CMS.FormControls;
using CMS.Helpers;

/// <summary>
/// Opens modal dialog with selector of images predefined for the edited object's class.
/// Stores unique identifier of the selected image in a hidden form field.
/// </summary>
public partial class CMSModules_AdminControls_Controls_Class_ClassThumbnailSelector_ClassThumbnailSelectorFormControl : FormEngineUserControl
{
    protected void Page_PreRender(object sender, EventArgs e)
    {
        // Data is used instead of EditedObject to display image even when creating a new object
        var baseInfo = Data as BaseInfo;
        if (baseInfo == null)
        {
            return;
        }

        var classInfo =  DataClassInfoProvider.GetDataClassInfo(baseInfo.ObjectType);
        if (classInfo == null)
        {
            return;
        }
        
        int classId = classInfo.ClassID;
        string modalUrl = ResolveUrl("~/CMSModules/AdminControls/Pages/ClassThumbnailSelector.aspx");

        SetImageAttributes(baseInfo);

        if (!Enabled)
        {
            divContainer.Attributes["class"] += " disabled";
        }
        else
        {
            // Since the application path is needed in client code of the control, application constants have to be registered in advance
            ScriptHelper.RegisterApplicationConstants(Page);
            ScriptHelper.RegisterDialogScript(Page);

            ScriptHelper.RegisterModule(this, "AdminControls/ClassThumbnailSelectorFormControl", new
            {
                ClassId = classId,
                ModalUrl = modalUrl,
                SelectButtonId = btnSelectImage.ClientID,
                HiddenInputId = hdnMetafileGuid.ClientID,
                PreviewImageId = imgPreview.ClientID,
                PreviewImageAnchorId = imgPreviewAnchor.ClientID
            });
        }
    }


    /// <summary>
    /// Sets source and alternative text for image element. If image Guid is not set, uses default image according to given base info.
    /// </summary>
    /// <param name="baseInfo">Edited object, determines default image if image guid is not specified</param>
    private void SetImageAttributes(BaseInfo baseInfo)
    {
        Guid imageGuid = ValidationHelper.GetGuid(Value, Guid.Empty);
        
        if (imageGuid == Guid.Empty)
        {
            var manager = new DefaultClassThumbnail(baseInfo.ObjectType);
            imageGuid = manager.GetDefaultClassThumbnailGuid() ?? Guid.Empty;
        }

        string imageUrl = MetaFileURLProvider.GetMetaFileUrl(imageGuid, string.Empty);
        imageUrl = URLHelper.UpdateParameterInUrl(imageUrl, "maxsidesize", "256");
        
        imgPreview.Src = imageUrl;
        imgPreview.Alt = GetString("general.objectimage");
    }


    /// <summary>
    /// Gets or sets unique identifier of the image.
    /// </summary>
    public override object Value
    {
        get
        {
            return hdnMetafileGuid.Value;
        }
        set
        {
            hdnMetafileGuid.Value = ValidationHelper.GetString(value, string.Empty);
        }
    }
}