using System;
using System.Security.Principal;

using CMS.CMSImportExport;
using CMS.DataEngine;
using CMS.EventLog;
using CMS.Helpers;
using CMS.IO;
using CMS.Membership;
using CMS.Base;
using CMS.SiteProvider;
using CMS.UIControls;

public partial class CMSModules_ImportExport_Pages_ExportObject : CMSModalPage
{
    #region "Variables"

    protected string codeName = null;
    protected string exportObjectDisplayName = null;

    protected string targetFolder = null;
    protected string targetUrl = null;

    protected bool allowDependent = false;
    protected bool siteObject = false;
    protected int objectId = 0;
    protected string objectType = string.Empty;

    protected GeneralizedInfo infoObj = null;
    protected GeneralizedInfo exportObj = null;

    protected bool backup = false;

    #endregion


    #region "Public properties"

    /// <summary>
    /// Export process GUID.
    /// </summary>
    public Guid ProcessGUID
    {
        get
        {
            if (ViewState["ProcessGUID"] == null)
            {
                ViewState["ProcessGUID"] = Guid.NewGuid();
            }

            return ValidationHelper.GetGuid(ViewState["ProcessGUID"], Guid.Empty);
        }
    }


    /// <summary>
    /// Persistent settings key.
    /// </summary>
    public string PersistentSettingsKey
    {
        get
        {
            return "ExportObject_" + ProcessGUID + "_Settings";
        }
    }


    /// <summary>
    /// Export settings stored in viewstate.
    /// </summary>
    public SiteExportSettings ExportSettings
    {
        get
        {
            SiteExportSettings settings = (SiteExportSettings)PersistentStorageHelper.GetValue(PersistentSettingsKey);
            if (settings == null)
            {
                throw new Exception("[ExportObject.ExportSettings]: Export settings has been lost.");
            }
            return settings;
        }
        set
        {
            PersistentStorageHelper.SetValue(PersistentSettingsKey, value);
        }
    }

    #endregion


    protected override void OnPreInit(EventArgs e)
    {
        base.OnPreInit(e);

        if (!DebugHelper.DebugImportExport)
        {
            DisableDebugging();
        }
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        backup = QueryHelper.GetBoolean("backup", false);

        // Check permissions
        if (backup)
        {
            if (!MembershipContext.AuthenticatedUser.IsAuthorizedPerResource("cms.globalpermissions", "BackupObjects", SiteContext.CurrentSiteName))
            {
                RedirectToAccessDenied("cms.globalpermissions", "BackupObjects");
            }
        }
        else if (!MembershipContext.AuthenticatedUser.IsAuthorizedPerResource("cms.globalpermissions", "ExportObjects", SiteContext.CurrentSiteName))
        {
            RedirectToAccessDenied("cms.globalpermissions", "ExportObjects");
        }

        // Register script for pendingCallbacks repair
        ScriptHelper.FixPendingCallbacks(Page);

        // Async control events binding
        ucAsyncControl.OnFinished += ucAsyncControl_OnFinished;
        ucAsyncControl.OnError += ucAsyncControl_OnError;

        if (!RequestHelper.IsCallback())
        {
            try
            {
                // Delete temporary files
                ExportProvider.DeleteTemporaryFiles();
            }
            catch (Exception ex)
            {
                DisplayError(ex);
            }

            if (backup)
            {
                SetTitle(GetString("BackupObject.Title"));
            }
            else
            {
                SetTitle(GetString("ExportObject.Title"));
            }

            // Display BETA warning
            lblBeta.Visible = CMSVersion.IsBetaVersion();
            lblBeta.Text = string.Format(GetString("export.BETAwarning"), CMSVersion.GetFriendlySystemVersion(false));

            // Get data from parameters
            objectId = QueryHelper.GetInteger("objectId", 0);
            objectType = QueryHelper.GetString("objectType", "");

            // Get the object
            infoObj = ModuleManager.GetReadOnlyObject(objectType);

            if (infoObj == null)
            {
                plcExportDetails.Visible = false;
                lblIntro.Text = GetString("ExportObject.ObjectTypeNotFound");
                lblIntro.CssClass = "ErrorLabel";
                return;
            }

            // Get exported object
            exportObj = infoObj.GetObject(objectId);
            if (exportObj == null)
            {
                plcExportDetails.Visible = false;
                lblIntro.Text = GetString("ExportObject.ObjectNotFound");
                lblIntro.CssClass = "ErrorLabel";
                btnOk.Visible = false;
                return;
            }

            // Check permissions
            var info = (BaseInfo)exportObj;
            if (!CurrentUser.IsGlobalAdministrator)
            {
                try
                {
                    if (info.Generalized.ObjectSiteID > 0)
                    {
                        CurrentUser.IsAuthorizedPerObject(PermissionsEnum.Read, info,
                            SiteInfoProvider.GetSiteName(info.Generalized.ObjectSiteID), true);
                    }
                    else if ((info.TypeInfo.SiteBindingObject != null) && (info.AssignedSites[CurrentSiteName] == null))
                    {
                        // Do not allow to clone objects with site binding which are not assigned to current site
                        RedirectToAccessDenied(info.TypeInfo.ModuleName, PermissionsEnum.Read.ToString());
                    }
                    else
                    {
                        CurrentUser.IsAuthorizedPerObject(PermissionsEnum.Read, info, CurrentSiteName, true);
                    }
                }
                catch (PermissionCheckException ex)
                {
                    RedirectToAccessDenied(ex.ModuleName, ex.PermissionFailed);
                }
            }

            // Store display name
            exportObjectDisplayName = HTMLHelper.HTMLEncode(ResHelper.LocalizeString(exportObj.ObjectDisplayName));

            lblIntro.Text = string.Format(GetString(backup ? "BackupObject.Intro" : "ExportObject.Intro"), exportObjectDisplayName);

            btnOk.Click += btnOk_Click;

            if (!RequestHelper.IsPostBack())
            {
                lblIntro.Visible = true;
                lblFileName.Visible = true;
                txtFileName.Text = GetExportFileName(exportObj, backup);
            }

            btnOk.Text = GetString(backup ? "General.backup" : "General.export");

            string path;
            if (backup)
            {
                path = ImportExportHelper.GetObjectBackupFolder(exportObj);
                targetFolder = Server.MapPath(path);

                targetUrl = ResolveUrl(path) + "/" + txtFileName.Text;
            }
            else
            {
                targetFolder = ImportExportHelper.GetSiteUtilsFolder() + "Export";
                path = ImportExportHelper.GetSiteUtilsFolderRelativePath();
                if (path != null)
                {
                    string externalUrl = null;
                    string fullPath = path + "Export/" + txtFileName.Text;

                    // Handle external storage URL
                    if (StorageHelper.IsExternalStorage(fullPath))
                    {
                        externalUrl = File.GetFileUrl(fullPath, SiteContext.CurrentSiteName);
                    }

                    // Ensure default target URL if not set
                    if (string.IsNullOrEmpty(externalUrl))
                    {
                        targetUrl = ResolveUrl(path) + "Export/" + txtFileName.Text;
                    }
                    else
                    {
                        targetUrl = externalUrl;
                    }
                }
                else
                {
                    targetUrl = null;
                }
            }
        }
    }


    private void DisplayError(Exception ex)
    {
        pnlProgress.Visible = false;
        pnlDetails.Visible = false;
        btnOk.Enabled = false;
        pnlContent.Visible = true;

        string displayName = null;
        if (exportObj != null)
        {
            displayName = exportObj.ObjectDisplayName;
        }
        lblResult.Text = string.Format(GetString("ExportObject.Error"), ResHelper.LocalizeString(HTMLHelper.HTMLEncode(displayName)), ex.Message);
        lblResult.ToolTip = EventLogProvider.GetExceptionLogMessage(ex);
        lblResult.CssClass = "ErrorLabel";

        EventLogProvider.LogException("Export", "ExportObject", ex);
    }


    private void btnOk_Click(object sender, EventArgs e)
    {
        // Init the Mimetype helper (required for the export)
        MimeTypeHelper.LoadMimeTypes();

        // Prepare the settings
        ExportSettings = new SiteExportSettings(MembershipContext.AuthenticatedUser);

        ExportSettings.WebsitePath = Server.MapPath("~/");
        ExportSettings.TargetPath = targetFolder;

        // Initialize
        ImportExportHelper.InitSingleObjectExportSettings(ExportSettings, exportObj);

        string result = ImportExportHelper.ValidateExportFileName(ExportSettings, txtFileName.Text);

        // Filename is valid
        if (!string.IsNullOrEmpty(result))
        {
            lblError.Text = result;
        }
        else
        {
            txtFileName.Text = txtFileName.Text.Trim();

            // Add extension
            if (Path.GetExtension(txtFileName.Text).ToLowerCSafe() != ".zip")
            {
                txtFileName.Text = txtFileName.Text.TrimEnd('.') + ".zip";
            }

            // Set the filename
            lblProgress.Text = string.Format(GetString("ExportObject.ExportProgress"), exportObjectDisplayName);
            ExportSettings.TargetFileName = txtFileName.Text;

            pnlContent.Visible = false;
            pnlDetails.Visible = false;
            btnOk.Enabled = false;
            pnlProgress.Visible = true;

            try
            {
                // Export the data
                ltlScript.Text = ScriptHelper.GetScript("StartTimer();");
                ucAsyncControl.RunAsync(ExportSingleObject, WindowsIdentity.GetCurrent());
            }
            catch (Exception ex)
            {
                DisplayError(ex);
            }
        }
    }


    // Export object
    private void ExportSingleObject(object parameter)
    {
        // Export object
        ExportProvider.ExportObjectsData(ExportSettings);
    }


    private void ucAsyncControl_OnError(object sender, EventArgs e)
    {
        ltlScript.Text += ScriptHelper.GetScript("StopTimer();");
        Exception ex = ((CMSAdminControls_AsyncControl)sender).Worker.LastException;

        DisplayError(ex);
    }


    private void ucAsyncControl_OnFinished(object sender, EventArgs e)
    {
        ltlScript.Text += ScriptHelper.GetScript("StopTimer();");
        pnlProgress.Visible = false;
        pnlContent.Visible = true;
        btnOk.Visible = false;
        lblResult.CssClass = "ContentLabel";

        string path = targetUrl;

        // Display full path
        if ((path == null) || StorageHelper.IsExternalStorage(targetFolder))
        {
            path = DirectoryHelper.CombinePath(targetFolder, txtFileName.Text);
        }

        if (!backup)
        {
            lblResult.Text = string.Format(GetString("ExportObject.lblResult"), exportObjectDisplayName, path);
            if (targetUrl != null)
            {
                lnkDownload.OnClientClick = "window.open(" + ScriptHelper.GetString(targetUrl) + "); return false;";
                lnkDownload.Text = GetString("ExportObject.Download");
                lnkDownload.Visible = true;
            }
        }
        else
        {
            lblResult.Text = string.Format(GetString("ExportObject.BackupFinished"), exportObjectDisplayName, path);
        }
    }


    protected override void OnPreRender(EventArgs e)
    {
        lblResult.Visible = (lblResult.Text != "");
        base.OnPreRender(e);
    }


    /// <summary>
    /// Ensure user friendly file name
    /// </summary>
    /// <param name="infoObj">Object to be exported</param>
    /// <param name="backup">Indicates if export is treated as backup</param>
    private string GetExportFileName(GeneralizedInfo infoObj, bool backup)
    {
        string fileName;
        // Get file name accrding to accesible object properties
        if (infoObj.TypeInfo.CodeNameColumn != ObjectTypeInfo.COLUMN_NAME_UNKNOWN)
        {
            fileName = infoObj.ObjectCodeName;
        }
        else if (infoObj.TypeInfo.DisplayNameColumn != ObjectTypeInfo.COLUMN_NAME_UNKNOWN)
        {
            fileName = ValidationHelper.GetCodeName(infoObj.ObjectDisplayName);
        }
        else
        {
            fileName = ValidationHelper.GetCodeName(infoObj.ObjectGUID.ToString());
        }

        fileName = fileName.Replace(".", "_") + "_" + DateTime.Now.ToString("yyyyMMdd") + "_" + DateTime.Now.ToString("HHmm") + ".zip";
        fileName = ValidationHelper.GetSafeFileName(fileName);

        // Backup use short file name, in other cases use long file name with object type
        if (!backup)
        {
            string objectType = infoObj.TypeInfo.Inherited ? infoObj.TypeInfo.OriginalObjectType : infoObj.ObjectType;
            fileName = objectType.Replace(".", "_") + "_" + fileName;
        }

        return fileName;
    }
}