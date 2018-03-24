using System;
using System.Linq;
using System.Web.UI;

using CMS.UIControls;
using CMS.Helpers;
using CMS.Controls;
using CMS.ExtendedControls;
using CMS.PortalEngine;

public partial class CMSModules_PortalEngine_UI_OnSiteEdit_EditText : CMSAbstractEditablePage
{
    protected override void OnInit(EventArgs e)
    {
        DocumentManager.OnValidateData += DocumentManager_OnValidateData;
        DocumentManager.OnAfterAction += DocumentManager_OnAfterAction;

        // Process ASPX template parameters
        if (QueryHelper.Contains("regiontype"))
        {
            ucEditableText.RegionType = CMSEditableRegionTypeEnumFunctions.GetRegionTypeEnum(QueryHelper.GetString("regiontype", string.Empty));
            if (ucEditableText.RegionType == CMSEditableRegionTypeEnum.HtmlEditor)
            {
                // HtmlEditor needs toolbar location defined (due to toolbar positioning and editing area padding)
                ucEditableText.HtmlAreaToolbarLocation = "Out:CKToolbar";
            }

            // Min/Max length
            ucEditableText.MaxLength = QueryHelper.GetInteger("maxl", ucEditableText.MaxLength);
            ucEditableText.MinLength = QueryHelper.GetInteger("minl", ucEditableText.MinLength);

            // Editor stylesheet
            ucEditableText.HTMLEditorCssStylesheet = QueryHelper.GetString("editorcss", ucEditableText.HTMLEditorCssStylesheet);
            
            // Word wrap
            ucEditableText.WordWrap = QueryHelper.GetBoolean("wordwrap", ucEditableText.WordWrap);

            // Upload image dimensions
            ucEditableText.ResizeToHeight = QueryHelper.GetInteger("resizetoheight", ucEditableText.ResizeToHeight);
            ucEditableText.ResizeToWidth = QueryHelper.GetInteger("resizetowidth", ucEditableText.ResizeToHeight);
            ucEditableText.ResizeToMaxSideSize = QueryHelper.GetInteger("resizetomaxsidesize", ucEditableText.ResizeToHeight);

            // Toolbar set
            ucEditableText.HtmlAreaToolbar = QueryHelper.GetString("toolbarset", ucEditableText.HtmlAreaToolbar);
        }

        ucEditableText.ViewMode = CheckPermissions();
        ucEditableText.DataControl = CurrentWebPartInstance;
        ucEditableText.CurrentPageInfo = CurrentPageInfo;
        ucEditableText.IsDialogEdit = true;
        ucEditableText.SetupControl();

        string title = GetString("Content.EditTextTitle");
        if (!String.IsNullOrEmpty(PageTitleSuffix))
        {
            title += " - " + HTMLHelper.HTMLEncode(PageTitleSuffix);
        }
        SetTitle(title);

        base.OnInit(e);

        CSSHelper.RegisterCSSLink(Page, "Design", "OnSiteEdit.css");
        ScriptHelper.RegisterJQuery(Page);

        menuElem.ShowSaveAndClose = true;

        if (ucEditableText.RegionType == CMSEditableRegionTypeEnum.TextArea)
        {
            const string resizeScript = @"
            var resizeTextAreaTimer;

            // DOM ready
            jQuery(document).ready( function() { ResizeEditableArea(200); });

            // Window resize
            jQuery(window).resize(function () { ResizeEditableArea(100); });

            function ResizeEditableArea(timeout) {
                clearTimeout(resizeTextAreaTimer);
                resizeTextAreaTimer = window.setTimeout(function () {
                    var textarea = jQuery('.EditableTextTextBox');
                    var editableTextContainer = jQuery('.EditableTextContainer');
                    var editableTextEdit = jQuery('.EditableTextEdit');
                    var borderMargin1 = textarea.outerHeight(true) - textarea.height();
                    var borderMargin2 = editableTextEdit.outerHeight(true) - editableTextEdit.height();
                    var borderMargin3 = editableTextContainer.outerHeight(true) - editableTextContainer.height();
                    var height = jQuery('.ModalDialogContent').height() - borderMargin1 - borderMargin2 - borderMargin3;
                    textarea.height(height);
            }, timeout); }";

            ScriptHelper.RegisterClientScriptBlock(this, typeof(Page), "ResizeEditableArea", ScriptHelper.GetScript(resizeScript));
        }

        CurrentDeviceInfo device = DeviceContext.CurrentDevice;

        // Resize of HTML area is handled from editor itself
        if ((ucEditableText.RegionType == CMSEditableRegionTypeEnum.HtmlEditor) && !device.IsMobile)
        {
            // Hide editor area before its fully loaded
            pnlEditor.Style.Add("visibility", "hidden");
        }

        if (device.IsMobile)
        {
            // Do not use fixed positioning for mobile devices
            (CurrentMaster.HeaderContainer as CMSPanel).FixedPosition = false;
        }


    }


    /// <summary>
    /// Load content
    /// </summary>
    public override void LoadContent(string content, bool forceReload)
    {
        ucEditableText.LoadContent(content, true);
    }


    /// <summary>
    /// Get content
    /// </summary>
    public override string GetContent()
    {
        // Return editable content when valid
        if (ucEditableText.IsValid())
        {
            return ucEditableText.GetContent();
        }
        else
        {
            ShowError(ucEditableText.ErrorMessage);
        }

        return null;
    }


    /// <summary>
    /// Handles the OnValidateData event of the DocumentManager control.
    /// </summary>
    protected void DocumentManager_OnValidateData(object sender, DocumentManagerEventArgs e)
    {
        if (!ucEditableText.IsValid())
        {
            // Set the error message when an error occurs
            e.IsValid = false;
            e.ErrorMessage = ucEditableText.ErrorMessage;
        }
    }


    /// <summary>
    /// Handles the OnAfterAction event of the DocumentManager control.
    /// </summary>
    protected void DocumentManager_OnAfterAction(object sender, DocumentManagerEventArgs e)
    {
        // Update the ViewMode in order to enable/disable the edit text control (used for workflow actions).
        ucEditableText.ViewMode = CheckPermissions();
    }
}