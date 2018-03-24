using System;

using CMS.DataEngine;
using CMS.EventLog;
using CMS.FormEngine;
using CMS.Helpers;
using CMS.IO;
using CMS.Base;
using CMS.UIControls;

[EditedObject("cms.documenttype", "objectid")]
public partial class CMSModules_DocumentTypes_Pages_Development_DocumentType_Edit_General : GlobalAdminPage
{
    #region "Variables"

    private string oldClassName;
    private int oldInheritedID;

    #endregion


    #region "Properties"

    /// <summary>
    /// Edited document type.
    /// </summary>
    private DataClassInfo DocumentType
    {
        get
        {
            return (DataClassInfo)EditedObject;
        }
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Page_Load event handler.
    /// </summary>
    protected void Page_Load(object sender, EventArgs e)
    {
        // Get original value
        oldClassName = DocumentType.ClassName;
        oldInheritedID = DocumentType.ClassInheritsFromClassID;

        // Bind events
        editElem.OnAfterSave += editElem_OnAfterSave;
        editElem.OnBeforeSave += editElem_OnBeforeSave;

        // Assign category identifier to page template selector
        if (ValidationHelper.GetBoolean(editElem.FieldControls["ClassShowTemplateSelection"].Value, false))
        {
            editElem.FieldControls["ClassDefaultPageTemplateID"].SetValue("RootCategoryID", ValidationHelper.GetInteger(editElem.FieldControls["ClassPageTemplateCategoryID"].Value, 0));
        }
    }


    /// <summary>
    /// OnBeforeSave event handler.
    /// </summary>
    protected void editElem_OnBeforeSave(object sender, EventArgs e)
    {
        // Clear assigned page template category if document type hasn't allowed template selection
        if (!ValidationHelper.GetBoolean(editElem.FieldControls["ClassShowTemplateSelection"].Value, false))
        {
            editElem.Data["ClassPageTemplateCategoryID"] = null;
        }
    }


    /// <summary>
    /// OnAfterSave event handler.
    /// </summary>
    protected void editElem_OnAfterSave(object sender, EventArgs e)
    {
        if (oldClassName != DocumentType.ClassName)
        {
            // Move icons if class name was changed
            RefreshIcon(oldClassName, DocumentType.ClassName, DocumentType.ClassIsCoupledClass, null);
            RefreshIcon(oldClassName, DocumentType.ClassName, DocumentType.ClassIsCoupledClass, "48x48");
        }

        int inheritedID = DocumentType.ClassInheritsFromClassID;
        bool structureChanged = false;

        // Ensure (update) the inheritance
        if (inheritedID != oldInheritedID)
        {
            if (inheritedID > 0)
            {
                // Update the inherited fields
                DataClassInfo parentClass = DataClassInfoProvider.GetDataClassInfo(inheritedID);
                if (parentClass != null)
                {
                    FormHelper.UpdateInheritedClass(parentClass, DocumentType);
                }
            }
            else
            {
                // Remove the inherited fields
                FormHelper.RemoveInheritance(DocumentType, false);
            }

            structureChanged = true;
        }

        if ((oldClassName.ToLowerCSafe() != DocumentType.ClassName.ToLowerCSafe()) && (DocumentType.ClassIsDocumentType))
        {
            structureChanged = true;
        }

        // Updates data class in DB (inner unique class name check)
        DataClassInfoProvider.SetDataClassInfo(DocumentType);

        if (structureChanged)
        {
            // Create view for document types
            SqlGenerator.GenerateDefaultView(DocumentType, null);

            // Clear class structures
            ClassStructureInfo.Remove(DocumentType.ClassName, true);
        }
    }


    /// <summary>
    /// Refreshes document type icons.
    /// </summary>
    /// <param name="className">Original class name</param>
    /// <param name="newClassName">New class name</param>
    /// <param name="isCoupled">True if edited document type isn't only container</param>
    /// <param name="iconSet">Icon set</param>
    private void RefreshIcon(string className, string newClassName, bool isCoupled, string iconSet)
    {
        string sourceFile = UIHelper.GetDocumentTypeIconPath(this, className, iconSet);
        string targetFile = UIHelper.GetDocumentTypeIconPath(this, newClassName, iconSet, false);
        string defaultFile = UIHelper.GetDocumentTypeIconPath(this, "default", iconSet, false);

        sourceFile = Server.MapPath(sourceFile);
        targetFile = Server.MapPath(targetFile);
        defaultFile = Server.MapPath(defaultFile);

        // Ensure same extension
        if (sourceFile.ToLowerCSafe().EndsWithCSafe(".gif"))
        {
            targetFile = targetFile.Replace(".png", ".gif");
        }

        // Rename icon file
        if (File.Exists(sourceFile))
        {
            try
            {
                if (sourceFile != defaultFile)
                {
                    File.Move(sourceFile, targetFile);
                }
                else
                {
                    // Copy default file
                    if (!isCoupled)
                    {
                        sourceFile = UIHelper.GetDocumentTypeIconPath(this, "defaultcontainer", iconSet, false);
                        sourceFile = Server.MapPath(sourceFile);
                    }
                    if (File.Exists(sourceFile))
                    {
                        File.Copy(sourceFile, targetFile);
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                EventLogProvider.LogException("Document types", "MOVEICON", ex);
            }
        }
    }

    #endregion
}