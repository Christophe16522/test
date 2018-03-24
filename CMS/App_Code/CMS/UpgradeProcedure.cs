using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml;

using CMS.Base;
using CMS.CMSImportExport;
using CMS.Core;
using CMS.DataEngine;
using CMS.EventLog;
using CMS.FormEngine;
using CMS.Globalization;
using CMS.Helpers;
using CMS.IO;
using CMS.MacroEngine;
using CMS.Membership;
using CMS.Modules;
using CMS.PortalEngine;
using CMS.URLRewritingEngine;
using CMS.WorkflowEngine;


#region "Code to bind to the ApplicationEvents using CMSModuleLoader"

/// <summary>
/// Upgrade loader
/// </summary>
[UpgradeLoader]
public partial class CMSModuleLoader
{
    /// <summary>
    /// Module registration
    /// </summary>
    private class UpgradeLoader : CMSLoaderAttribute
    {
        /// <summary>
        /// Initializes the module
        /// </summary>
        public override void PreInit()
        {
            ApplicationEvents.UpdateData.Execute += Update;
        }


        /// <summary>
        /// Updates the application data to a newer version if necessary
        /// </summary>
        private void Update(object sender, EventArgs eventArgs)
        {
            UpgradeProcedure.Update();
        }
    }
}

#endregion

/// <summary>
/// Class carrying the code to perform the upgrade procedure.
/// </summary>
public static class UpgradeProcedure
{
    #region "Variables"

    // Path to the upgrade package
    private static string mUpgradePackagePath;
    private static string mWebsitePath;

    private const string EVENT_LOG_INFO = "Upgrade to 8.0";

    #endregion


    #region "Main update method"

    /// <summary>
    /// Runs the update procedure.
    /// </summary>
    public static void Update()
    {
        if (DatabaseHelper.IsDatabaseAvailable)
        {
            try
            {
                string version = SettingsKeyInfoProvider.GetStringValue("CMSDataVersion");
                switch (version.ToLowerCSafe())
                {
                    case "7.0":
                        using (var context = new CMSActionContext())
                        {
                            context.LogLicenseWarnings = false;

                            UpgradeApplication(Upgrade70To80, "8.0", "Upgrade_70_80.zip");
                        }
                        break;
                }
            }
            catch
            {
            }
        }
    }

    #endregion


    #region "General purpose - all versions methods"

    private static void UpgradeApplication(Func<bool> versionSpecificMethod, string newVersion, string packageName)
    {
        // Increase the timeout for upgrade request due to expensive operations like macro signing and conversion (needed for large DBs)
        HttpContext.Current.Server.ScriptTimeout = 14400;

        EventLogProvider.LogInformation(EVENT_LOG_INFO, "Upgrade - Start");

        // Set the path to the upgrade package (this has to be done here, not in the Import method, because it's an async procedure without HttpContext)
        mUpgradePackagePath = HttpContext.Current.Server.MapPath("~/CMSSiteUtils/Import/" + packageName);
        mWebsitePath = HttpContext.Current.Server.MapPath("~/");

        using (var context = new CMSActionContext())
        {
            context.DisableLogging();
            context.CreateVersion = false;
            context.LogIntegration = false;

            UpdateClasses();
            UpdateAlternativeForms();
        }

        // Update all views
        var dtm = new TableManager(null);
        dtm.RefreshDocumentViews();
        RefreshCustomViews(dtm);

        // Set data version
        SettingsKeyInfoProvider.SetValue("CMSDataVersion", newVersion);
        SettingsKeyInfoProvider.SetValue("CMSDBVersion", newVersion);

        // Clear hashtables
        ModuleManager.ClearHashtables();

        // Clear the cache
        CacheHelper.ClearCache(null, true);

        // Drop the routes
        CMSDocumentRouteHelper.DropAllRoutes();

        // Init the Mimetype helper (required for the Import)
        MimeTypeHelper.LoadMimeTypes();

        // Call version specific operations
        if (versionSpecificMethod != null)
        {
            using (var context = new CMSActionContext())
            {
                context.DisableLogging();
                context.CreateVersion = false;
                context.LogIntegration = false;

                versionSpecificMethod.Invoke();
            }
        }

        UpgradeImportPackage();
    }


    /// <summary>
    /// Refreshes all custom views.
    /// </summary>
    private static void RefreshCustomViews(TableManager tm)
    {
        tm.RefreshView("View_CMS_User");
        tm.RefreshView("View_Community_Member");

        tm.RefreshView("View_COM_SKU");

        tm.RefreshView("View_NewsletterSubscriberUserRole_Joined");

        tm.RefreshView("View_Community_Group");

        tm.RefreshView("View_Community_Friend_Friends");
        tm.RefreshView("View_Community_Friend_RequestedFriends");

        tm.RefreshView("View_OM_Contact_Activity");
        tm.RefreshView("View_OM_Contact_Joined");
        tm.RefreshView("View_OM_ContactGroupMember_ContactJoined");

        tm.RefreshView("View_OM_Account_Joined");
        tm.RefreshView("View_OM_Account_MembershipJoined");
        tm.RefreshView("View_OM_ContactGroupMember_AccountJoined");
    }


    /// <summary>
    /// Update form definitions of classes (especially system tables).
    /// </summary>
    private static void UpdateClasses()
    {
        var path = Path.Combine(mWebsitePath, "App_Data\\CMSTemp\\Upgrade\\Classes");
        var classes = GetAllFiles(path);
        foreach (var classPath in classes)
        {
            var className = Path.GetFileNameWithoutExtension(classPath);
            var dataClass = DataClassInfoProvider.GetDataClassInfo(className);
            if (dataClass != null)
            {
                var newVersionDefinition = File.ReadAllText(classPath);
                var newVersionFi = new FormInfo(newVersionDefinition);
                var oldVersionFi = new FormInfo(dataClass.ClassFormDefinition);

                CopyCustomFields(oldVersionFi, newVersionFi);

                // Save the modified form definition
                dataClass.ClassFormDefinition = newVersionFi.GetXmlDefinition();

                // Update the scheme
                dataClass.ClassXmlSchema = new TableManager(dataClass.ClassConnectionString).GetXmlSchema(dataClass.ClassTableName);

                // Save the new definition
                dataClass.Update();
            }
        }
    }


    /// <summary>
    /// Updates an existing alternative forms form definitions. Appends existing custom fields to new version definitions.
    /// </summary>
    private static void UpdateAlternativeForms()
    {
        var path = Path.Combine(mWebsitePath, "App_Data\\CMSTemp\\Upgrade\\AlternativeForms");
        var forms = GetAllFiles(path);
        foreach (var formPath in forms)
        {
            var form = Path.GetFileNameWithoutExtension(formPath);
            var altForm = AlternativeFormInfoProvider.GetAlternativeFormInfo(form);
            if (altForm != null)
            {
                var mainDci = DataClassInfoProvider.GetDataClassInfo(altForm.FormClassID);
                var classFormDefinition = mainDci.ClassFormDefinition;

                if (altForm.FormCoupledClassID > 0)
                {
                    // If coupled class is defined combine form definitions
                    var coupledDci = DataClassInfoProvider.GetDataClassInfo(altForm.FormCoupledClassID);
                    if (coupledDci != null)
                    {
                        classFormDefinition = FormHelper.MergeFormDefinitions(classFormDefinition, coupledDci.ClassFormDefinition);
                    }
                }

                // Make sure that the false flag for extra fields is not used in future upgrades (8.0 further) since extra custom fields could be added to alternative forms since v8
                var oldVersionDefinition = FormHelper.MergeFormDefinitions(classFormDefinition, altForm.FormDefinition, false);
                var newVersionDefinition = FormHelper.MergeFormDefinitions(classFormDefinition, File.ReadAllText(formPath));

                var newVersionFi = new FormInfo(newVersionDefinition);
                var oldVersionFi = new FormInfo(oldVersionDefinition);

                CopyCustomFields(oldVersionFi, newVersionFi);

                // Save the modified form definition
                altForm.FormDefinition = FormHelper.GetFormDefinitionDifference(classFormDefinition, newVersionFi.GetXmlDefinition(), true);
                altForm.Update();
            }
        }
    }


    /// <summary>
    /// Copies custom fields from old version of form definition to the new form definition.
    /// </summary>
    /// <param name="oldVersionFi">Old version form definition</param>
    /// <param name="newVersionFi">New version form definition</param>
    private static void CopyCustomFields(FormInfo oldVersionFi, FormInfo newVersionFi)
    {
        // Remove all system fields from old definition to get only custom fields
        oldVersionFi.RemoveFields(f => f.System);

        // Combine forms so that custom fields from old definition are appended to the new definition
        newVersionFi.CombineWithForm(oldVersionFi, new CombineWithFormSettings
        {
            IncludeCategories = false,
            RemoveEmptyCategories = true,
            OverwriteExisting = true
        });
    }


    /// <summary>
    /// Procedures which automatically imports the upgrade export package with all WebParts, Widgets, Reports and TimeZones.
    /// </summary>
    private static void UpgradeImportPackage()
    {
        // Import
        try
        {
            RequestStockHelper.Remove("CurrentDomain", true);

            var importSettings = new SiteImportSettings(MembershipContext.AuthenticatedUser)
            {
                DefaultProcessObjectType = ProcessObjectEnum.All,
                SourceFilePath = mUpgradePackagePath,
                WebsitePath = mWebsitePath
            };

            using (var context = new CMSActionContext())
            {
                context.DisableLogging();
                context.CreateVersion = false;
                context.LogIntegration = false;

                ImportProvider.ImportObjectsData(importSettings);

                // Regenerate time zones
                TimeZoneInfoProvider.GenerateTimeZoneRules();

                // Delete the files for separable modules which are not install and therefore not needed
                DeleteWebPartsOfUninstalledModules();

                ImportMetaFiles(Path.Combine(mWebsitePath, "App_Data\\CMSTemp\\Upgrade"));
            }
        }
        catch (Exception ex)
        {
            EventLogProvider.LogException(EVENT_LOG_INFO, "Upgrade", ex);
        }
        finally
        {
            try
            {
                RefreshMacroSignatures();

                EventLogProvider.LogInformation(EVENT_LOG_INFO, "Upgrade - Finish");
            }
            catch (Exception ex)
            {
                EventLogProvider.LogException(EVENT_LOG_INFO, "Upgrade", ex);
            }
        }
    }


    /// <summary>
    /// Refreshes macro signatures in all object which can contain macros.
    /// </summary>
    private static void RefreshMacroSignatures()
    {
        // Get object types
        var objectTypes = new List<string> {
            TransformationInfo.OBJECT_TYPE,
            UIElementInfo.OBJECT_TYPE,
            FormUserControlInfo.OBJECT_TYPE,
            SettingsKeyInfo.OBJECT_TYPE,
            AlternativeFormInfo.OBJECT_TYPE,
            DataClassInfo.OBJECT_TYPE,
            DataClassInfo.OBJECT_TYPE_SYSTEMTABLE,
            DataClassInfo.OBJECT_TYPE_CUSTOMTABLE,
            DataClassInfo.OBJECT_TYPE_DOCUMENTTYPE,
            PageTemplateInfo.OBJECT_TYPE,
            LayoutInfo.OBJECT_TYPE,
            CssStylesheetInfo.OBJECT_TYPE,
            WorkflowActionInfo.OBJECT_TYPE,
        };

        foreach (string type in objectTypes)
        {
            try
            {
                using (var context = new CMSActionContext())
                {
                    context.DisableLogging();
                    context.CreateVersion = false;
                    context.LogIntegration = false;

                    var infos = new InfoObjectCollection(type);
                    foreach (var info in infos)
                    {
                        MacroSecurityProcessor.RefreshSecurityParameters(info, "administrator", true);
                    }
                }
            }
            catch (Exception ex)
            {
                EventLogProvider.LogException("Upgrade - Refresh macros", "Upgrade", ex);
            }
        }
    }


    /// <summary>
    /// Deletes the files for separable modules which are not install and therefore not needed.
    /// </summary>
    private static void DeleteWebPartsOfUninstalledModules()
    {
        var webPartsPath = mWebsitePath + "CMSWebParts\\";
        var files = new List<string>();

        var separableModules = new List<string>()
        {
            ModuleName.BIZFORM, 
            ModuleName.BLOGS,
            ModuleName.COMMUNITY,
            ModuleName.ECOMMERCE,
            ModuleName.EVENTMANAGER,
            ModuleName.FORUMS,
            ModuleName.MEDIALIBRARY,
            ModuleName.MESSAGEBOARD,
            ModuleName.MESSAGING,
            ModuleName.NEWSLETTER,
            ModuleName.NOTIFICATIONS,
            ModuleName.ONLINEMARKETING,
            ModuleName.POLLS,
            ModuleName.PROJECTMANAGEMENT,
            ModuleName.REPORTING,
            ModuleName.STRANDSRECOMMENDER,
            ModuleName.CHAT,
        };

        foreach (var separableModule in separableModules)
        {
            // Add files from this folder to the list of files to delete if the module is not installed
            if (!ModuleEntryManager.IsModuleLoaded(separableModule))
            {
                var folderName = GetWebPartFolderName(separableModule);
                files.AddRange(GetAllFiles(webPartsPath + folderName));
            }
        }

        // Remove web parts for separated modules
        foreach (String file in files)
        {
            try
            {
                File.Delete(file);
            }
            catch (Exception ex)
            {
                EventLogProvider.LogException("Upgrade - Remove separated web parts", "Upgrade", ex);
            }
        }
    }


    /// <summary>
    /// Returns list of all files in given folder (recursively, from all subdirectories as well).
    /// </summary>
    /// <param name="folder">Folder to search in</param>
    private static List<String> GetAllFiles(String folder)
    {
        var files = new List<string>();

        files.AddRange(Directory.GetFiles(folder));

        var dirs = Directory.GetDirectories(folder);

        foreach (string dir in dirs)
        {
            files.AddRange(GetAllFiles(dir));
        }

        return files;
    }


    /// <summary>
    /// For given module returns it's folder name within CMSWebParts folder.
    /// </summary>
    /// <param name="moduleName">Name of the module</param>
    /// <returns></returns>
    private static string GetWebPartFolderName(string moduleName)
    {
        // Handle exceptions
        switch (moduleName)
        {
            case ModuleName.BIZFORM:
                return "BizForms";

            case ModuleName.BLOGS:
                return "Blogs";

            case ModuleName.NEWSLETTER:
                return "Newsletters";
        }

        // By default, trim "CMS." prefix from module name which will give us folder name withing CMSWebParts directory
        return moduleName.Substring(4);
    }


    /// <summary>
    /// Imports default metafiles which were changed in the new version.
    /// </summary>
    /// <param name="upgradeFolder">Folder where the generated metafiles.xml file is</param>
    private static void ImportMetaFiles(string upgradeFolder)
    {
        try
        {
            // To get the file use Phobos - Generate files button, Metafile settings.
            // Choose only those object types which had metafiles in previous version and these metafiles changed to the new version.
            String xmlPath = Path.Combine(upgradeFolder, "metafiles.xml");
            if (File.Exists(xmlPath))
            {
                XmlDocument xDoc = new XmlDocument();
                xDoc.Load(xmlPath);

                XmlNode metaFilesNode = xDoc.SelectSingleNode("MetaFiles");
                if (metaFilesNode != null)
                {
                    String filesDirectory = Path.Combine(upgradeFolder, "Metafiles");

                    using (new CMSActionContext() { LogEvents = false })
                    {
                        foreach (XmlNode metaFile in metaFilesNode)
                        {
                            // Load metafiles information from XML
                            String objType = metaFile.Attributes["ObjectType"].Value;
                            String groupName = metaFile.Attributes["GroupName"].Value;
                            String codeName = metaFile.Attributes["CodeName"].Value;
                            String fileName = metaFile.Attributes["FileName"].Value;
                            String extension = metaFile.Attributes["Extension"].Value;
                            String fileGUID = metaFile.Attributes["FileGUID"].Value;
                            String title = (metaFile.Attributes["Title"] != null) ? metaFile.Attributes["Title"].Value : null;
                            String description = (metaFile.Attributes["Description"] != null) ? metaFile.Attributes["Description"].Value : null;

                            // Try to find correspondent info object
                            BaseInfo infoObject = BaseAbstractInfoProvider.GetInfoByName(objType, codeName);
                            if (infoObject != null)
                            {
                                int infoObjectId = infoObject.Generalized.ObjectID;

                                // Check if metafile exists
                                InfoDataSet<MetaFileInfo> metaFilesSet = MetaFileInfoProvider.GetMetaFilesWithoutBinary(infoObjectId, objType, groupName, "MetaFileGUID = '" + fileGUID + "'", null);
                                if (DataHelper.DataSourceIsEmpty(metaFilesSet))
                                {
                                    // Create new metafile if does not exists
                                    String mfFileName = String.Format("{0}.{1}", fileGUID, extension.TrimStart('.'));
                                    MetaFileInfo mfInfo = new MetaFileInfo(Path.Combine(filesDirectory, mfFileName), infoObjectId, objType, groupName);
                                    mfInfo.MetaFileGUID = ValidationHelper.GetGuid(fileGUID, Guid.NewGuid());

                                    // Set correct properties
                                    mfInfo.MetaFileName = fileName;
                                    if (title != null)
                                    {
                                        mfInfo.MetaFileTitle = title;
                                    }
                                    if (description != null)
                                    {
                                        mfInfo.MetaFileDescription = description;
                                    }

                                    // Save new meta file
                                    MetaFileInfoProvider.SetMetaFileInfo(mfInfo);
                                }
                            }
                        }

                        // Remove existing files after successful finish
                        String[] files = Directory.GetFiles(upgradeFolder);
                        foreach (String file in files)
                        {
                            File.Delete(file);
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            EventLogProvider.LogException("Upgrade - Import metafiles", "Upgrade", ex);
        }
    }

    #endregion


    #region "Update from 7.0 to 8.0"

    /// <summary>
    /// Handles all the specific operations for upgrade from 7.0 to 8.0.
    /// </summary>
    /// <returns></returns>
    private static bool Upgrade70To80()
    {
        UpgradeSocialMarketing();
        UpgradeMacros();
        UpgradeInheritedWPProperties();
        UpdateWebPartProperties();
        UpdateWidgetProperties();
        UpgradeFormCategories();

        return true;
    }


    /// <summary>
    /// Upgrades form category properties.
    /// </summary>
    private static void UpgradeFormCategories()
    {
        try
        {
            var resourceStringRegex = new Regex(@"(?<tag><cms:FormCategory[^>]*\s)(GroupingText)?ResourceString\s*=");
            var resourceStringReplacement = "${tag}CategoryTitleResourceString=";

            var groupingTextRegex = new Regex(@"(?<tag><cms:FormCategory[^>]*\s)GroupingText\s*=");
            var groupingTextReplacement = "${tag}CategoryTitle=";

            var classes = DataClassInfoProvider.GetClasses().Where("[ClassFormLayout] != '' AND [ClassFormLayout] LIKE '%cms:FormCategory%'");
            foreach (var dci in classes)
            {
                dci.ClassFormLayout = resourceStringRegex.Replace(dci.ClassFormLayout, resourceStringReplacement);
                dci.ClassFormLayout = groupingTextRegex.Replace(dci.ClassFormLayout, groupingTextReplacement);

                DataClassInfoProvider.SetDataClassInfo(dci);
            }

            var altForms = AlternativeFormInfoProvider.GetAlternativeForms("[FormLayout] != '' AND [FormLayout] LIKE '%cms:FormCategory%'", null);
            foreach (var afi in altForms)
            {
                afi.FormLayout = resourceStringRegex.Replace(afi.FormLayout, resourceStringReplacement);
                afi.FormLayout = groupingTextRegex.Replace(afi.FormLayout, groupingTextReplacement);

                AlternativeFormInfoProvider.SetAlternativeFormInfo(afi);
            }
        }
        catch (Exception ex)
        {
            EventLogProvider.LogException("Upgrade - Form categories", "Upgrade", ex);
        }
    }



    /// <summary>
    /// Update web part properties default values
    /// </summary>
    private static void UpdateWebPartProperties()
    {
        try
        {
            List<string> wpGuid = new List<string>() {
            "A03A0766-F018-4BC0-9FEB-55058E40DF53",
            "B5AB1C37-61E8-4C88-B06D-4F2100BFC43A",
            "DA9F49D4-9D7C-49A8-AE5E-6FFD85A2F59B",
            "5D6B8461-9537-4AA0-83EC-893F59A2B729",
            "2166181D-EA9B-4394-B40F-230AC973317B",
            "E16EC646-63C1-41E9-B14A-BB52F031E1B9",
            "75324B10-3E56-4952-9F9B-B36A2AC7FD1A",
            "D6C4FFA3-B17D-41F4-9C1A-AD8A75A835A8",
            "E54BFF78-5410-438F-AB4D-43322D6AB5C1",
            "DDF2FA96-58D1-45D6-9B56-4F288051683C",
            "7AA8A4DB-C56E-4C8F-A605-F739D01540C0"};

            // Get inherited webparts with properties stored in old way
            string where = "WebPartParentID IS NULL AND WebPartDefaultValues LIKE N'%field name=\"%' AND WebPartGUID NOT IN ('" + wpGuid.Join("','") + "')";
            InfoDataSet<WebPartInfo> data = WebPartInfoProvider.GetWebParts().Where(where).TypedResult;
            if (!DataHelper.DataSourceIsEmpty(data))
            {
                foreach (WebPartInfo info in data)
                {
                    info.WebPartDefaultValues = ModifyProperties(info.WebPartDefaultValues);

                    // Update webpart
                    info.Update();
                }
            }
        }
        catch (Exception ex)
        {
            EventLogProvider.LogException("Upgrade - Web part properties", "Upgrade", ex);
        }
    }


    /// <summary>
    /// Update web part properties default values
    /// </summary>
    private static void UpdateWidgetProperties()
    {
        try
        {
            List<string> wpGuid = new List<string>() {
            "D6EC4B53-781E-4240-98F4-F4D64873A482",
            "09F5FF6C-F2A1-4322-AEA6-6B0D61CD1375",
            "1CEF5345-672E-4571-A0BD-BC7BD217B1A0",
            "90994364-454F-4457-98FE-6EB78D2B452C"};

            // Get inherited webparts with properties stored in old way
            string where = "WidgetDefaultValues LIKE N'%field name=\"%' AND WidgetGUID NOT IN ('" + wpGuid.Join("','") + "')";
            InfoDataSet<WidgetInfo> data = WidgetInfoProvider.GetWidgets().Where(where).TypedResult;
            if (!DataHelper.DataSourceIsEmpty(data))
            {
                foreach (WidgetInfo info in data)
                {
                    info.WidgetDefaultValues = ModifyProperties(info.WidgetDefaultValues);

                    // Update widget
                    info.Update();
                }
            }
        }
        catch (Exception ex)
        {
            EventLogProvider.LogException("Upgrade - Widget properties", "Upgrade", ex);
        }
    }


    /// <summary>
    /// Modifies properties format and excludes inherited properties to the separate location.
    /// </summary>
    /// <param name="allProperties">Parameter that may include inherited and system properties</param>
    private static string ModifyProperties(string allProperties)
    {
        if (String.IsNullOrEmpty(allProperties))
        {
            return allProperties;
        }

        // Replace 'name' attribute with 'column'
        allProperties = Regex.Replace(allProperties, "(?:name)(?==\")", "column", RegexOptions.IgnoreCase);
        // Replace 'value' attribute with 'defaultvalue'
        allProperties = Regex.Replace(allProperties, "(value)(?==\")", "default$1", RegexOptions.IgnoreCase);

        return allProperties;
    }


    /// <summary>
    /// Upgrades inherited web part properties.
    /// </summary>
    private static void UpgradeInheritedWPProperties()
    {
        try
        {
            WebPartInfoProvider.UpdateInheritedWebPartProperties();
        }
        catch (Exception ex)
        {
            EventLogProvider.LogException("Upgrade - Inherited web part properties", "Upgrade", ex);
        }
    }


    /// <summary>
    /// Transforms old unsupported macro types to data macros.
    /// </summary>
    private static void UpgradeMacros()
    {
        try
        {
            MacroCompatibility.TransformToDataMacros(ObjectTypeManager.ObjectTypesWithMacros);
        }
        catch (Exception ex)
        {
            EventLogProvider.LogException("Upgrade - Macro compatibility transformation", "Upgrade", ex);
        }
    }


    /// <summary>
    /// Runs code to upgrade SM module using reflection because it uses strongly typed API and it's a separable module, therefore this method cannot be called directly using a reference.
    /// </summary>
    private static void UpgradeSocialMarketing()
    {
        try
        {
            if (ModuleEntryManager.IsModuleLoaded(ModuleName.SOCIALMARKETING))
            {
                Type t = null;
                if (SystemContext.IsWebApplicationProject)
                {
                    var asm = Assembly.Load("CMSAppAppCode");
                    if (asm != null)
                    {
                        t = asm.GetType("SocialMarketingUpgradeProcedure");
                    }
                }
                else
                {
                    t = Assembly.GetExecutingAssembly().GetType("SocialMarketingUpgradeProcedure");
                }

                if (t != null)
                {
                    var method = t.GetMethod("UpgradeV7ToV8");
                    if (method != null)
                    {
                        method.Invoke(null, null);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            EventLogProvider.LogException("Upgrade - Social marketing", "Upgrade", ex);
        }
    }

    #endregion
}