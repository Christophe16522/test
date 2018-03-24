using System;
using System.Collections;
using System.Data;
using System.Text;

using CMS.FormEngine;
using CMS.Helpers;
using CMS.PortalEngine;
using CMS.Base;
using CMS.SiteProvider;
using CMS.Membership;
using CMS.UIControls;
using CMS.DataEngine;
using CMS.MacroEngine;


public partial class CMSPages_Dialogs_Documentation : CMSPage
{
    #region "Variables"

    /// <summary>
    /// HTML representation of menu (list of webparts in multiple mode).
    /// </summary>
    private string menu = "";

    /// <summary>
    /// HTML representation of content  (webpart title and properties).
    /// </summary>
    private string content = "";

    /// <summary>
    /// 0 - documentation present
    /// 1-documentation missing
    /// 2-documentation present but inherited from parent webpart
    /// </summary>
    private int documentation = 0;

    /// <summary>
    /// If true webpart (widget) has image.
    /// </summary>
    private bool isImagePresent = false;

    /// <summary>
    /// Count of undocumented properties.
    /// </summary>
    private int undocumentedProperties = 0;

    /// <summary>
    /// Documentation title.
    /// </summary>
    protected string documentationTitle = "Kentico Web Parts";

    /// <summary>
    /// If true information about webpart is displayed.
    /// </summary>
    private bool development = false;

    /// <summary>
    /// Resolver.
    /// </summary>
    private MacroResolver resolver = null;

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        // Security test
        if (!MembershipContext.AuthenticatedUser.CheckPrivilegeLevel(UserPrivilegeLevelEnum.GlobalAdmin))
        {
            RedirectToAccessDenied(GetString("attach.actiondenied"));
        }

        // Add link to external stylesheet
        CSSHelper.RegisterCSSLink(this, "Default", "/CMSDesk.css");

        // Get current resolver
        resolver = MacroContext.CurrentResolver.CreateChild();

        DataSet ds = null;
        DataSet cds = null;

        // Check init settings
        bool allWidgets = QueryHelper.GetBoolean("allWidgets", false);
        bool allWebParts = QueryHelper.GetBoolean("allWebparts", false);
        bool allWireframes = QueryHelper.GetBoolean("allwireframes", false);

        // Get webpart (widget) from querystring - only if no allwidget or allwebparts set
        bool isWebpartInQuery = false;
        bool isWidgetInQuery = false;
        String webpartQueryParam = String.Empty;

        //If not show all widgets or webparts - check if any widget or webpart is present
        if (!allWidgets && !allWebParts && !allWireframes)
        {
            webpartQueryParam = QueryHelper.GetString("webpart", "");
            if (!string.IsNullOrEmpty(webpartQueryParam))
            {
                isWebpartInQuery = true;
            }
            else
            {
                webpartQueryParam = QueryHelper.GetString("widget", "");
                if (!string.IsNullOrEmpty(webpartQueryParam))
                {
                    isWidgetInQuery = true;
                }
            }
        }

        // Set development option if is required
        if (QueryHelper.GetString("details", "0") == "1")
        {
            development = true;
        }

        // Generate all webparts
        if (allWebParts)
        {
            // Get all webpart categories
            cds = WebPartCategoryInfoProvider.GetCategories();
        }
        // Generate all widgets
        else if (allWidgets)
        {
            // Get all widget categories
            cds = WidgetCategoryInfoProvider.GetWidgetCategories();
        }
        else if (allWireframes)
        {
            WebPartCategoryInfo wpci = WebPartCategoryInfoProvider.GetWebPartCategoryInfoByCodeName("Wireframes");
            if (wpci != null)
            {
                cds = WebPartCategoryInfoProvider.GetCategories(wpci.CategoryID);
            }
        }
        // Generate single webpart
        else if (isWebpartInQuery)
        {
            // Split weparts
            string[] webparts = webpartQueryParam.Split(';');
            if (webparts.Length > 0)
            {
                string webpartWhere = SqlHelper.GetWhereCondition("WebpartName", webparts);
                ds = WebPartInfoProvider.GetWebParts().Where(webpartWhere);

                // If any webparts found 
                if (!DataHelper.DataSourceIsEmpty(ds))
                {
                    StringBuilder categoryWhere = new StringBuilder("");
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        categoryWhere.Append(ValidationHelper.GetString(dr["WebpartCategoryID"], "NULL") + ",");
                    }

                    string ctWhere = "CategoryID IN (" + categoryWhere.ToString().TrimEnd(',') + ")";
                    cds = WebPartCategoryInfoProvider.GetCategories().Where(ctWhere);
                }
            }
        }
        // Generate single widget
        else if (isWidgetInQuery)
        {
            string[] widgets = webpartQueryParam.Split(';');
            if (widgets.Length > 0)
            {
                string widgetsWhere = SqlHelper.GetWhereCondition("WidgetName", widgets);
                ds = WidgetInfoProvider.GetWidgets().Where(widgetsWhere);
            }

            if (!DataHelper.DataSourceIsEmpty(ds))
            {
                StringBuilder categoryWhere = new StringBuilder("");
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    categoryWhere.Append(ValidationHelper.GetString(dr["WidgetCategoryID"], "NULL") + ",");
                }

                string ctWhere = "WidgetCategoryID IN (" + categoryWhere.ToString().TrimEnd(',') + ")";
                cds = WidgetCategoryInfoProvider.GetWidgetCategories().Where(ctWhere);
            }
        }

        if (allWidgets || isWidgetInQuery)
        {
            documentationTitle = "Kentico Widgets";
            Page.Header.Title = "Widgets documentation";
        }

        if (!allWebParts && !allWidgets && !allWireframes && !isWebpartInQuery && !isWidgetInQuery)
        {
            pnlContent.Visible = false;
            pnlInfo.Visible = true;
        }

        // Check whether at least one category is present
        if (!DataHelper.DataSourceIsEmpty(cds))
        {
            string namePrefix = ((isWidgetInQuery) || (allWidgets)) ? "Widget" : String.Empty;

            // Loop through all web part categories
            foreach (DataRow cdr in cds.Tables[0].Rows)
            {
                // Get all webpart in the categories
                if (allWebParts || allWireframes)
                {
                    ds = WebPartInfoProvider.GetAllWebParts(Convert.ToInt32(cdr["CategoryId"]));
                }
                // Get all widgets in the category
                else if (allWidgets)
                {
                    int categoryID = Convert.ToInt32(cdr["WidgetCategoryId"]);
                    ds = WidgetInfoProvider.GetWidgets().WhereEquals("WidgetCategoryID", categoryID);
                }

                // Check whether current category contains at least one webpart
                if (!DataHelper.DataSourceIsEmpty(ds))
                {
                    // Generate category name code
                    menu += "<br /><strong>" + HTMLHelper.HTMLEncode(cdr[namePrefix + "CategoryDisplayName"].ToString()) + "</strong><br /><br />";

                    // Loop through all web web parts in categories
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        // Init 
                        isImagePresent = false;
                        undocumentedProperties = 0;
                        documentation = 0;

                        // Webpart (Widget) information
                        string itemDisplayName = String.Empty;
                        string itemDescription = String.Empty;
                        string itemDocumentation = String.Empty;
                        string itemType = String.Empty;
                        int itemID = 0;

                        WebPartInfo wpi = null;
                        WidgetInfo wi = null;

                        // Set webpart info
                        if (isWebpartInQuery || allWebParts || allWireframes)
                        {
                            wpi = new WebPartInfo(dr);
                            if (wpi != null)
                            {
                                itemDisplayName = wpi.WebPartDisplayName;
                                itemDescription = wpi.WebPartDescription;
                                itemDocumentation = wpi.WebPartDocumentation;
                                itemID = wpi.WebPartID;
                                itemType = WebPartInfo.OBJECT_TYPE;

                                if (wpi.WebPartCategoryID != ValidationHelper.GetInteger(cdr["CategoryId"], 0))
                                {
                                    wpi = null;
                                }
                            }
                        }
                        // Set widget info
                        else if ((isWidgetInQuery) || (allWidgets))
                        {
                            wi = new WidgetInfo(dr);
                            if (wi != null)
                            {
                                itemDisplayName = wi.WidgetDisplayName;
                                itemDescription = wi.WidgetDescription;
                                itemDocumentation = wi.WidgetDocumentation;
                                itemType = WidgetInfo.OBJECT_TYPE;
                                itemID = wi.WidgetID;

                                if (wi.WidgetCategoryID != ValidationHelper.GetInteger(cdr["WidgetCategoryId"], 0))
                                {
                                    wi = null;
                                }
                            }
                        }

                        // Check whether web part (widget) exists
                        if ((wpi != null) || (wi != null))
                        {
                            // Link GUID
                            Guid mguid = Guid.NewGuid();

                            // Whether description is present in webpart
                            bool isDescription = false;

                            // Image url
                            string wimgurl = GetItemImage(itemID, itemType);

                            // Set description text
                            string descriptionText = itemDescription;

                            // Parent webpart info
                            WebPartInfo pwpi = null;

                            // If webpart look for parent's description and documentation
                            if (wpi != null)
                            {
                                // Get parent description if webpart is inherited
                                if (wpi.WebPartParentID > 0)
                                {
                                    pwpi = WebPartInfoProvider.GetWebPartInfo(wpi.WebPartParentID);
                                    if (pwpi != null)
                                    {
                                        if ((descriptionText == null || descriptionText.Trim() == ""))
                                        {
                                            // Set description from parent
                                            descriptionText = pwpi.WebPartDescription;
                                        }

                                        // Set documentation text from parent if WebPart is inherited
                                        if ((wpi.WebPartDocumentation == null) || (wpi.WebPartDocumentation.Trim() == ""))
                                        {
                                            itemDocumentation = pwpi.WebPartDocumentation;
                                            if (!String.IsNullOrEmpty(itemDocumentation))
                                            {
                                                documentation = 2;
                                            }
                                        }
                                    }
                                }
                            }

                            // Set description as present
                            if (descriptionText.Trim().Length > 0)
                            {
                                isDescription = true;
                            }

                            // Generate HTML for menu and content
                            menu += "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<a href=\"#_" + mguid.ToString() + "\">" + HTMLHelper.HTMLEncode(itemDisplayName) + "</a>&nbsp;";

                            // Generate webpart header
                            content += "<table style=\"width:100%;\"><tr><td><h1><a name=\"_" + mguid.ToString() + "\">" + HTMLHelper.HTMLEncode(ResHelper.LocalizeString(cdr[namePrefix + "CategoryDisplayName"].ToString())) + "&nbsp;>&nbsp;" + HTMLHelper.HTMLEncode(ResHelper.LocalizeString(itemDisplayName)) + "</a></h1></td><td style=\"text-align:right;\">&nbsp;<a href=\"#top\" class=\"noprint\">top</a></td></tr></table>";

                            // Generate WebPart content
                            content +=
                                @"<table style=""width: 100%; height: 200px; border: solid 1px #DDDDDD;"">
                                   <tr> 
                                     <td style=""width: 50%; text-align:center; border-right: solid 1px #DDDDDD; vertical-align: middle;margin-left: auto; margin-right:auto; text-align:center;"">
                                         <img src=""" + wimgurl + @""" alt=""imageTeaser"">
                                     </td>
                                     <td style=""width: 50%; vertical-align: center;text-align:center;"">"
                                + HTMLHelper.HTMLEncode(ResHelper.LocalizeString(descriptionText)) + @"
                                     </td>
                                   </tr>
                                </table>";

                            // Properties content
                            content += "<div class=\"DocumentationWebPartsProperties\">";

                            // Generate content
                            if (wpi != null)
                            {
                                GenerateDocContent(CreateFormInfo(wpi));
                            }
                            else if (wi != null)
                            {
                                GenerateDocContent(CreateFormInfo(wi));
                            }

                            // Close content area
                            content += "</div>";

                            // Generate documentation text content
                            content += "<br /><div style=\"border: solid 1px #dddddd;width: 100%;\">" +
                                       DataHelper.GetNotEmpty(HTMLHelper.ResolveUrls(itemDocumentation, null), "<strong>Additional documentation text is not provided.</strong>") +
                                       "</div>";

                            // Set page break tag for print
                            content += "<br /><p style=\"page-break-after: always;width:100%\">&nbsp;</p><hr class=\"noprint\" />";

                            // If development is required - highlight missing description, images and doc. text
                            if (development)
                            {
                                // Check image
                                if (!isImagePresent)
                                {
                                    menu += "<span style=\"color:Brown;\">image&nbsp;</span>";
                                }

                                // Check properties
                                if (undocumentedProperties > 0)
                                {
                                    menu += "<span style=\"color:Red;\">properties(" + undocumentedProperties + ")&nbsp;</span>";
                                }

                                // Check properties
                                if (!isDescription)
                                {
                                    menu += "<span style=\"color:#37627F;\">description&nbsp;</span>";
                                }

                                // Check documentation text
                                if (String.IsNullOrEmpty(itemDocumentation))
                                {
                                    documentation = 1;
                                }

                                switch (documentation)
                                {
                                        // Display information about missing documentation
                                    case 1:
                                        menu += "<span style=\"color:Green;\">documentation&nbsp;</span>";
                                        break;

                                        // Display information about inherited documentation
                                    case 2:
                                        menu += "<span style=\"color:Green;\">documentation (inherited)&nbsp;</span>";
                                        break;
                                }
                            }

                            menu += "<br />";
                        }
                    }
                }
            }
        }

        ltlContent.Text = menu + "<br /><p style=\"page-break-after: always;width:100%\">&nbsp;</p><hr class=\"noprint\" />" + content;
    }


    /// <summary>
    /// Generate form info for widget.
    /// </summary>
    /// <param name="wi">Widget info</param>    
    private FormInfo CreateFormInfo(WidgetInfo wi)
    {
        WebPartInfo wpi = WebPartInfoProvider.GetWebPartInfo(wi.WidgetWebPartID);
        if (wpi != null)
        {
            string widgetProperties = FormHelper.MergeFormDefinitions(wpi.WebPartProperties, wi.WidgetProperties);
            return PortalFormHelper.GetWidgetFormInfo(wi.WidgetName, String.Empty, widgetProperties, null, false);
        }
        return null;
    }


    /// <summary>
    /// Generate form info for webpart.
    /// </summary>
    /// <param name="wpi">Web part info</param>
    private FormInfo CreateFormInfo(WebPartInfo wpi)
    {
        if (wpi != null)
        {
            // Get parent webpart if webpart is inherited
            if (wpi.WebPartParentID != 0)
            {
                WebPartInfo pwpi = WebPartInfoProvider.GetWebPartInfo(wpi.WebPartParentID);
                if (pwpi != null)
                {
                    wpi = pwpi;
                }
            }
        }
        return PortalFormHelper.GetWebPartFormInfo(wpi.WebPartName + FormHelper.CORE, wpi.WebPartProperties, null, null, false);
    }


    /// <summary>
    /// Generate document content.
    /// </summary>
    /// <param name="wpi">WebPart info</param>
    /// <param name="gd">Guid</param>
    /// <param name="category">Category</param>
    protected void GenerateDocContent(FormInfo fi)
    {
        if (fi == null)
        {
            return;
        }
        // Get defintion elements
        var infos = fi.GetFormElements(true, false);

        bool isOpenSubTable = false;

        string currentGuid = "";

        // Used for filter empty categories
        String categoryHeader = String.Empty;

        // Check all items in object array
        foreach (object contrl in infos)
        {
            // Generate row for form category        
            if (contrl is FormCategoryInfo)
            {
                // Load castegory info
                FormCategoryInfo fci = contrl as FormCategoryInfo;
                if (fci != null)
                {
                    // Close table from last category
                    if (isOpenSubTable)
                    {
                        content += "<tr class=\"PropertyBottom\"><td class=\"PropertyLeftBottom\">&nbsp;</td><td colspan=\"2\" class=\"Center\">&nbsp;</td><td class=\"PropertyRightBottom\">&nbsp;</td></tr></table>";
                        isOpenSubTable = false;
                    }

                    if (currentGuid == "")
                    {
                        currentGuid = Guid.NewGuid().ToString().Replace("-", "_");
                    }

                    // Generate table for current category
                    categoryHeader = @"<br />
                        <table cellpadding=""0"" cellspacing=""0"" class=""CategoryTable"">
                          <tr>
                           <td class=""CategoryLeftBorder"">&nbsp;</td>
                           <td class=""CategoryTextCell"">" + HTMLHelper.HTMLEncode(ResHelper.LocalizeString(fci.CategoryName)) + @"</td>
                           <td class=""CategoryRightBorder"">&nbsp;</td>
                         </tr>
                       </table>";
                }
            }
            else
            {
                // Get form field info
                FormFieldInfo ffi = contrl as FormFieldInfo;

                if (ffi != null)
                {
                    if (categoryHeader != String.Empty)
                    {
                        content += categoryHeader;
                        categoryHeader = String.Empty;
                    }

                    if (!isOpenSubTable)
                    {
                        // Generate table for properties under one category 
                        isOpenSubTable = true;
                        content += "" +
                                   "<table cellpadding=\"0\" cellspacing=\"0\" id=\"_" + currentGuid + "\" class=\"PropertiesTable\" >";
                        currentGuid = "";
                    }

                    // Add ':' to caption
                    string doubleDot = "";
                    if (!ffi.GetPropertyValue(FormFieldPropertyEnum.FieldCaption, MacroContext.CurrentResolver).EndsWithCSafe(":"))
                    {
                        doubleDot = ":";
                    }

                    string fieldDescription = ffi.GetPropertyValue(FormFieldPropertyEnum.FieldDescription, resolver);

                    content +=
                        @"<tr>
                            <td class=""PropertyLeftBorder"" >&nbsp;</td>
                            <td class=""PropertyContent"" style=""width:200px;"">" + HTMLHelper.HTMLEncode(ResHelper.LocalizeString(ffi.GetPropertyValue(FormFieldPropertyEnum.FieldCaption, MacroContext.CurrentResolver))) + doubleDot + @"</td>
                            <td class=""PropertyRow"">" + HTMLHelper.HTMLEncode(DataHelper.GetNotEmpty(ResHelper.LocalizeString(fieldDescription), GetString("WebPartDocumentation.DescriptionNoneAvailable"))) + @"</td>
                            <td class=""PropertyRightBorder"">&nbsp;</td>
                        </tr>";

                    if (fieldDescription == null || fieldDescription.Trim() == "")
                    {
                        undocumentedProperties++;
                    }
                }
            }
        }

        // Close last category (if has any properties)
        if (isOpenSubTable)
        {
            content += "<tr class=\"PropertyBottom\"><td class=\"PropertyLeftBottom\">&nbsp;</td><td colspan=\"2\" class=\"Center\">&nbsp;</td><td class=\"PropertyRightBottom\">&nbsp;</td></tr></table>";
        }
    }


    /// <summary>
    /// Returns url of item's (webpart,widget) image.
    /// </summary>
    /// <param name="itemID">ID of webpart (widget)</param>
    /// <param name="itemType">Type of item (webpart,widget)</param>
    private string GetItemImage(int itemID, string itemType)
    {
        DataSet mds = MetaFileInfoProvider.GetMetaFiles(itemID, itemType);

        // Check whether image exists
        if (!DataHelper.DataSourceIsEmpty(mds))
        {
            // Ge tmetafile info object
            MetaFileInfo mtfi = new MetaFileInfo(mds.Tables[0].Rows[0]);
            
            // Image found - used in development mode
            isImagePresent = true;

            // Get image url
            return ResolveUrl("~/CMSPages/GetMetaFile.aspx?fileguid=" + mtfi.MetaFileGUID.ToString());
        }

        return GetImageUrl("CMSModules/CMS_PortalEngine/WebpartProperties/imagenotavailable.png");
    }


    /// <summary>
    /// OnLoad override.
    /// </summary>
    protected override void OnLoad(EventArgs e)
    {
        // Disable caching
        Response.Cache.SetNoStore();
        base.OnLoad(e);
    }

    #endregion
}