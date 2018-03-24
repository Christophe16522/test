using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Principal;
using System.Threading;

using CMS.Base;
using CMS.DataEngine;
using CMS.DocumentEngine;
using CMS.EventLog;
using CMS.ExtendedControls;
using CMS.FormEngine;
using CMS.Helpers;
using CMS.IO;
using CMS.PortalEngine;
using CMS.SiteProvider;
using CMS.UIControls;
using IOExceptions = System.IO;

public partial class CMSModules_System_System_Deployment : GlobalAdminPage
{
    #region "Variables"

    private static readonly Hashtable mErrors = new Hashtable();
    private List<string> deletePaths = null;

    #endregion


    #region "Properties"

    /// <summary>
    /// Current log context.
    /// </summary>
    public LogContext CurrentLog
    {
        get
        {
            return EnsureLog();
        }
    }


    /// <summary>
    /// Current Error.
    /// </summary>
    private string CurrentError
    {
        get
        {
            return ValidationHelper.GetString(mErrors["TranslateError_" + ctlAsync.ProcessGUID], string.Empty);
        }
        set
        {
            mErrors["TranslateError_" + ctlAsync.ProcessGUID] = value;
        }
    }

    #endregion


    #region "Page events"

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        deletePaths = new List<string>();

        ctlAsync.OnFinished += ctlAsync_OnFinished;
        ctlAsync.OnError += ctlAsync_OnError;
        ctlAsync.OnRequestLog += ctlAsync_OnRequestLog;
        ctlAsync.OnCancel += ctlAsync_OnCancel;

        btnCancel.Attributes.Add("onclick", ctlAsync.GetCancelScript(true) + "return false;");

        titleElemAsync.TitleText = GetString("Deployment.Processing");
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        chkSaveCSS.Checked = CssStylesheetInfoProvider.StoreCSSStyleSheetsInExternalStorage;
        chkSaveLayouts.Checked = LayoutInfoProvider.StoreLayoutsInExternalStorage;
        chkSavePageTemplate.Checked = PageTemplateInfoProvider.StorePageTemplatesInExternalStorage;
        chkSaveWebpartLayout.Checked = WebPartLayoutInfoProvider.StoreWebPartLayoutsInExternalStorage;
        chkSaveTransformation.Checked = TransformationInfoProvider.StoreTransformationsInExternalStorage;
        chkSaveWebpartContainer.Checked = WebPartContainerInfoProvider.StoreWebPartContainersInExternalStorage;
        chkSaveAltFormLayouts.Checked = AlternativeFormInfoProvider.StoreAlternativeFormsInExternalStorage;
        chkSaveFormLayouts.Checked = DataClassInfoProvider.StoreFormLayoutsInExternalStorage;

        if (chkSaveCSS.Checked || chkSaveLayouts.Checked || chkSavePageTemplate.Checked || chkSaveWebpartLayout.Checked
            || chkSaveTransformation.Checked || chkSaveWebpartContainer.Checked || chkSaveAltFormLayouts.Checked || chkSaveFormLayouts.Checked)
        {
            lblSynchronization.Visible = true;
            btnSynchronize.Visible = true;
        }

        bool deploymentMode = SettingsKeyInfoProvider.DeploymentMode;
        chkSaveLayouts.Enabled = chkSavePageTemplate.Enabled = chkSaveTransformation.Enabled = chkSaveWebpartLayout.Enabled
            = chkSaveAltFormLayouts.Enabled = chkSaveFormLayouts.Enabled = !deploymentMode;

        if (SystemContext.IsRunningOnAzure)
        {
            ShowWarning(GetString("Deployment.AzureDisabled"), null, null);
            btnSaveAll.Enabled = false;
            btnSourceControl.Enabled = false;
            chkSaveCSS.Enabled = chkSaveLayouts.Enabled = chkSavePageTemplate.Enabled = chkSaveTransformation.Enabled = chkSaveWebpartLayout.Enabled
                = chkSaveAltFormLayouts.Enabled = chkSaveFormLayouts.Enabled = false;
        }

        if (SettingsKeyInfoProvider.DeploymentMode)
        {
            lblDeploymentInfo.Text = GetString("Deployment.SaveAllToDBInfo");
            btnSaveAll.ResourceString = "Deployment.SaveAllToDB";
            lblSourceControlInfo.Text = GetString("Deployment.SourceControlInfoDeploymentMode");
        }
        else
        {
            lblDeploymentInfo.Text = GetString("Deployment.SaveAllInfo");
            btnSaveAll.ResourceString = "Deployment.SaveAll";
            lblSourceControlInfo.Text = GetString("Deployment.SourceControlInfo");
        }

        if (!SystemContext.IsFullTrustLevel)
        {
            // Disable the form in Medium Trust and tell user what's wrong
            chkSaveCSS.Enabled = chkSaveLayouts.Enabled = chkSavePageTemplate.Enabled = chkSaveTransformation.Enabled
                = chkSaveWebpartContainer.Enabled = chkSaveWebpartLayout.Enabled = chkSaveAltFormLayouts.Enabled = chkSaveFormLayouts.Enabled
                = btnSaveAll.Enabled = btnSourceControl.Enabled = btnSynchronize.Enabled = false;

            ShowInformation(GetString("deployment.fulltrustrequired"));
        }
        
        if (SystemContext.DevelopmentMode)
        {
            ShowInformation(GetString("Deployment.DevelopmentMode"));
            btnSaveAll.Enabled = btnSourceControl.Enabled = btnSynchronize.Enabled = false;
            chkSaveCSS.Enabled = chkSaveLayouts.Enabled = chkSavePageTemplate.Enabled = chkSaveWebpartLayout.Enabled = false;
            chkSaveTransformation.Enabled = chkSaveWebpartContainer.Enabled = chkSaveAltFormLayouts.Enabled = chkSaveFormLayouts.Enabled = false;
        }
    }

    #endregion


    #region "Deployment methods"

    /// <summary>
    /// Tests compilation of the given list of objects
    /// </summary>
    /// <param name="collection">Collection of objects</param>
    private void TestCompilation(InfoObjectCollection collection)
    {
        if (collection == null)
        {
            return;
        }

        foreach (BaseInfo info in collection)
        {
            var name = info.Generalized.ObjectFullName ?? info.Generalized.ObjectDisplayName;

            if (SystemContext.DevelopmentMode && (info.Generalized.ObjectCodeName.StartsWithCSafe("test", true) || name.StartsWithCSafe("test", true)))
            {
                // Skip testing objects
                continue;
            }

            AddLog(string.Format(GetString("Deployment.Testing"), GetString("objecttype." + collection.ObjectType.Replace(".", "_")), name));

            try
            {
                var path = info.Generalized.GetVirtualFileRelativePath(info.TypeInfo.CodeColumn, info.Generalized.ObjectVersionGUID.ToString());
                if (path.EndsWithCSafe(".ascx", true))
                {
                    // Try to load the control
                    var c = Page.LoadUserControl(path);
                    c.Dispose();
                }
            }
            catch (ThreadAbortException)
            {
                // Cancel                
            }
            catch (Exception ex)
            {
                string message = string.Format(GetString("Deployment.TestingFailed"), GetString("objecttype." + collection.ObjectType.Replace(".", "_")), name, ex.Message);

                AddError(message);
            }
        }
    }


    private void SaveToExternalStorage(InfoObjectCollection collection)
    {
        if (collection == null)
        {
            return;
        }

        foreach (BaseInfo info in collection)
        {
            AddLog(string.Format(GetString("Deployment.Deploying"), GetString("objecttype." + collection.ObjectType.Replace(".", "_")), info.Generalized.ObjectDisplayName));
            try
            {
                info.Generalized.SaveExternalColumns();
            }
            catch (Exception ex)
            {
                AddError(ex.Message);
            }
        }
    }


    private void SaveToDB(InfoObjectCollection collection)
    {
        if (collection == null)
        {
            return;
        }

        foreach (BaseInfo info in collection)
        {
            AddLog(string.Format(GetString("Deployment.Deploying"), GetString("objecttype." + collection.ObjectType.Replace(".", "_")), info.Generalized.ObjectDisplayName));
            try
            {
                info.Generalized.IgnoreExternalColumns = true;
                info.Generalized.UpdateExternalColumns();
                info.Generalized.IgnoreExternalColumns = false;
            }
            catch (Exception ex)
            {
                AddError(ex.Message);
            }
        }
    }


    private void DeleteDir(string path)
    {
        if (deletePaths != null)
        {
            deletePaths.Add(path);
        }
    }


    private void DeleteDirs()
    {
        if (deletePaths != null)
        {
            foreach (var path in deletePaths)
            {
                DirectoryInfo dir = DirectoryInfo.New(URLHelper.GetPhysicalPath(path));
                if (dir.Exists)
                {
                    DirectoryHelper.DeleteDirectory(dir.FullName, true);
                }
            }
        }
    }


    private void ProcessTransformations(bool storeInDB, bool deleteFiles)
    {
        if (TransformationInfoProvider.StoreTransformationsInExternalStorage == !storeInDB)
        {
            return;
        }

        if (storeInDB)
        {
            if (deleteFiles)
            {
                AddLog(string.Format(GetString("Deployment.DeletingDeployedFiles"), GetString("objecttype." + TransformationInfo.OBJECT_TYPE.Replace(".", "_"))));
            }
            SaveToDB(CMSDataContext.Current.Transformations);

            if (deleteFiles)
            {
                DeleteDir(TransformationInfoProvider.TransformationsDirectory);
                TransformationInfoProvider.StoreTransformationsInExternalStorage = false;
            }
        }
        else
        {
            TransformationInfoProvider.StoreTransformationsInExternalStorage = true;
            SaveToExternalStorage(CMSDataContext.Current.Transformations);
        }
    }


    private void ProcessWebpartLayouts(bool storeInDB, bool deleteFiles)
    {
        if (WebPartLayoutInfoProvider.StoreWebPartLayoutsInExternalStorage == !storeInDB)
        {
            return;
        }

        if (storeInDB)
        {
            if (deleteFiles)
            {
                AddLog(string.Format(GetString("Deployment.DeletingDeployedFiles"), GetString("objecttype." + WebPartLayoutInfo.OBJECT_TYPE.Replace(".", "_"))));
            }
            SaveToDB(CMSDataContext.Current.WebPartLayouts);
            if (deleteFiles)
            {
                DeleteDir(WebPartLayoutInfoProvider.WebPartLayoutsDirectory);
                WebPartLayoutInfoProvider.StoreWebPartLayoutsInExternalStorage = false;
            }
        }
        else
        {
            WebPartLayoutInfoProvider.StoreWebPartLayoutsInExternalStorage = true;
            SaveToExternalStorage(CMSDataContext.Current.WebPartLayouts);
        }
    }


    private void ProcessWebpartContainers(bool storeInDB, bool deleteFiles)
    {
        if (WebPartContainerInfoProvider.StoreWebPartContainersInExternalStorage == !storeInDB)
        {
            return;
        }

        if (storeInDB)
        {
            if (deleteFiles)
            {
                AddLog(string.Format(GetString("Deployment.DeletingDeployedFiles"), GetString("objecttype." + WebPartContainerInfo.OBJECT_TYPE.Replace(".", "_"))));
            }
            SaveToDB(CMSDataContext.Current.GlobalObjects[WebPartContainerInfo.OBJECT_TYPE]);
            if (deleteFiles)
            {
                DeleteDir(WebPartContainerInfoProvider.WebPartContainersDirectory);
                WebPartContainerInfoProvider.StoreWebPartContainersInExternalStorage = false;
            }
        }
        else
        {
            WebPartContainerInfoProvider.StoreWebPartContainersInExternalStorage = true;
            SaveToExternalStorage(CMSDataContext.Current.GlobalObjects[WebPartContainerInfo.OBJECT_TYPE]);
        }
    }


    private void ProcessLayouts(bool storeInDB, bool deleteFiles)
    {
        if (LayoutInfoProvider.StoreLayoutsInExternalStorage == !storeInDB)
        {
            return;
        }

        if (storeInDB)
        {
            if (deleteFiles)
            {
                AddLog(string.Format(GetString("Deployment.DeletingDeployedFiles"), GetString("objecttype." + LayoutInfo.OBJECT_TYPE.Replace(".", "_"))));
            }
            SaveToDB(CMSDataContext.Current.GlobalObjects[LayoutInfo.OBJECT_TYPE]);
            if (deleteFiles)
            {
                DeleteDir(LayoutInfoProvider.LayoutsDirectory);
                LayoutInfoProvider.StoreLayoutsInExternalStorage = false;
            }
        }
        else
        {
            LayoutInfoProvider.StoreLayoutsInExternalStorage = true;
            SaveToExternalStorage(CMSDataContext.Current.GlobalObjects[LayoutInfo.OBJECT_TYPE]);
        }
    }


    private void ProcessCSS(bool storeInDB, bool deleteFiles)
    {
        if (CssStylesheetInfoProvider.StoreCSSStyleSheetsInExternalStorage == !storeInDB)
        {
            return;
        }

        if (storeInDB)
        {
            if (deleteFiles)
            {
                AddLog(string.Format(GetString("Deployment.DeletingDeployedFiles"), GetString("objecttype." + CssStylesheetInfo.OBJECT_TYPE.Replace(".", "_"))));
            }
            SaveToDB(CMSDataContext.Current.GlobalObjects[CssStylesheetInfo.OBJECT_TYPE]);
            if (deleteFiles)
            {
                DeleteDir(CssStylesheetInfoProvider.CSSStylesheetsDirectory);
                CssStylesheetInfoProvider.StoreCSSStyleSheetsInExternalStorage = false;
            }
        }
        else
        {
            CssStylesheetInfoProvider.StoreCSSStyleSheetsInExternalStorage = true;
            SaveToExternalStorage(CMSDataContext.Current.GlobalObjects[CssStylesheetInfo.OBJECT_TYPE]);
        }
    }


    private void ProcessTemplates(bool storeInDB, bool deleteFiles)
    {
        if (PageTemplateInfoProvider.StorePageTemplatesInExternalStorage == !storeInDB)
        {
            return;
        }

        if (storeInDB)
        {
            if (deleteFiles)
            {
                AddLog(string.Format(GetString("Deployment.DeletingDeployedFiles"), GetString("objecttype." + PageTemplateInfo.OBJECT_TYPE.Replace(".", "_"))));
            }

            foreach (PageTemplateInfo info in PageTemplateInfoProvider.GetTemplates())
            {
                ProcessTemplateToDB(info);
            }

            if (deleteFiles)
            {
                DeleteDir("~/CMSVirtualFiles/Templates/");
                PageTemplateInfoProvider.StorePageTemplatesInExternalStorage = false;
            }
        }
        else
        {
            PageTemplateInfoProvider.StorePageTemplatesInExternalStorage = true;

            foreach (PageTemplateInfo info in PageTemplateInfoProvider.GetTemplates())
            {
                ProcessTemplateToFS(info);
            }
        }
    }


    private static void ProcessTemplateToFS(PageTemplateInfo info)
    {
        var devices = info.Children[PageTemplateDeviceLayoutInfo.OBJECT_TYPE];
        if (devices != null)
        {
            foreach (PageTemplateDeviceLayoutInfo device in devices)
            {
                device.Generalized.SaveExternalColumns();
            }
        }

        if (info.IsPortal || (info.PageTemplateType == PageTemplateTypeEnum.Dashboard) || (info.PageTemplateType == PageTemplateTypeEnum.UI))
        {
            info.Generalized.SaveExternalColumns();
        }
    }


    private static void ProcessTemplateToDB(PageTemplateInfo info)
    {
        var devices = info.Children[PageTemplateDeviceLayoutInfo.OBJECT_TYPE];
        if (devices != null)
        {
            foreach (PageTemplateDeviceLayoutInfo device in devices)
            {
                device.Generalized.IgnoreExternalColumns = true;
                device.Generalized.UpdateExternalColumns();
                device.Generalized.IgnoreExternalColumns = false;
            }
        }

        if (info.IsPortal || (info.PageTemplateType == PageTemplateTypeEnum.Dashboard) || (info.PageTemplateType == PageTemplateTypeEnum.UI))
        {
            info.Generalized.IgnoreExternalColumns = true;
            info.Generalized.UpdateExternalColumns();
            info.Generalized.IgnoreExternalColumns = false;
        }
    }


    private void ProcessAltFormLayouts(bool storeInDB, bool deleteFiles)
    {
        if (AlternativeFormInfoProvider.StoreAlternativeFormsInExternalStorage == !storeInDB)
        {
            return;
        }

        if (storeInDB)
        {
            if (deleteFiles)
            {
                AddLog(string.Format(GetString("Deployment.DeletingDeployedFiles"), GetString("objecttype." + AlternativeFormInfo.OBJECT_TYPE.Replace(".", "_"))));
            }
            SaveToDB(CMSDataContext.Current.AlternativeForms);
            if (deleteFiles)
            {
                DeleteDir(AlternativeFormInfoProvider.FormLayoutsDirectory);
                AlternativeFormInfoProvider.StoreAlternativeFormsInExternalStorage = false;
            }
        }
        else
        {
            AlternativeFormInfoProvider.StoreAlternativeFormsInExternalStorage = true;
            SaveToExternalStorage(CMSDataContext.Current.AlternativeForms);
        }
    }


    private void ProcessFormLayouts(bool storeInDB, bool deleteFiles)
    {
        if (DataClassInfoProvider.StoreFormLayoutsInExternalStorage == !storeInDB)
        {
            return;
        }

        if (storeInDB)
        {
            if (deleteFiles)
            {
                AddLog(string.Format(GetString("Deployment.DeletingDeployedFiles"), GetString("objecttype." + DataClassInfo.OBJECT_TYPE.Replace(".", "_"))));
            }
            SaveToDB(CMSDataContext.Current.GlobalObjects[DataClassInfo.OBJECT_TYPE]);
            if (deleteFiles)
            {
                DeleteDir(DataClassInfoProvider.FormLayoutsDirectory);
                DataClassInfoProvider.StoreFormLayoutsInExternalStorage = false;
            }
        }
        else
        {
            DataClassInfoProvider.StoreFormLayoutsInExternalStorage = true;
            SaveToExternalStorage(CMSDataContext.Current.GlobalObjects[DataClassInfo.OBJECT_TYPE]);
        }
    }


    /// <summary>
    /// Does the deployment of the given object
    /// </summary>
    private void SaveExternally(object parameter)
    {
        try
        {
            bool[] store = (bool[])parameter;

            if (!SettingsKeyInfoProvider.DeploymentMode)
            {
                ProcessLayouts(!store[1], true);
                ProcessTemplates(!store[2], true);
                ProcessTransformations(!store[3], true);
                ProcessWebpartLayouts(!store[4], true);
                ProcessAltFormLayouts(!store[6], true);
                ProcessFormLayouts(!store[7], true);
            }

            ProcessWebpartContainers(!store[5], true);
            ProcessCSS(!store[0], true);

            // Delete dirs at the end because of restart
            DeleteDirs();
        }
        catch (ThreadAbortException ex)
        {
            string state = ValidationHelper.GetString(ex.ExceptionState, string.Empty);
            if (state == CMSThread.ABORT_REASON_STOP)
            {
                // When canceled
                AddError(ResHelper.GetString("Deployment.DeploymentCanceled"));
            }
            else
            {
                // Log error
                LogExceptionToEventLog(ex);
            }
        }
        catch (IOExceptions.IOException ex)
        {
            LogExceptionToEventLog(ex);
        }
        catch (Exception ex)
        {
            // Log error
            LogExceptionToEventLog(ex);
        }
    }


    /// <summary>
    /// Does the test of the given object
    /// </summary>
    private void Test(object parameter)
    {
        try
        {
            TestCompilation(CMSDataContext.Current.Transformations);
            TestCompilation(CMSDataContext.Current.AlternativeForms);
            TestCompilation(CMSDataContext.Current.WebPartLayouts);
            TestCompilation(CMSDataContext.Current.GlobalObjects[LayoutInfo.OBJECT_TYPE]);
            TestCompilation(CMSDataContext.Current.GlobalObjects[PageTemplateInfo.OBJECT_TYPE]);
            TestCompilation(CMSDataContext.Current.GlobalObjects[DataClassInfo.OBJECT_TYPE]);
        }
        catch (ThreadAbortException ex)
        {
            string state = ValidationHelper.GetString(ex.ExceptionState, string.Empty);
            if (state == CMSThread.ABORT_REASON_STOP)
            {
                // When canceled
                AddError(ResHelper.GetString("general.actioncanceled"));
            }
            else
            {
                // Log error
                LogExceptionToEventLog(ex);
            }
        }
        catch (IOExceptions.IOException ex)
        {
            LogExceptionToEventLog(ex);
        }
        catch (Exception ex)
        {
            // Log error
            LogExceptionToEventLog(ex);
        }
    }


    /// <summary>
    /// Does the deployment of the given object
    /// </summary>
    private void Deploy(object parameter)
    {
        try
        {
            bool targetMode = SettingsKeyInfoProvider.DeploymentMode;

            // In the direction DB -> FS we need to set the deployment mode before processing
            if (!targetMode)
            {
                SettingsKeyInfoProvider.DeploymentMode = true;
            }

            ProcessLayouts(targetMode, true);
            ProcessTemplates(targetMode, true);
            ProcessTransformations(targetMode, true);
            ProcessWebpartLayouts(targetMode, true);
            ProcessAltFormLayouts(targetMode, true);
            ProcessFormLayouts(targetMode, true);

            // In the direction FS -> DB we need to set the deployment mode after processing
            if (targetMode)
            {
                SettingsKeyInfoProvider.DeploymentMode = false;
            }

            // Delete dirs at the end because of restart
            DeleteDirs();
        }
        catch (ThreadAbortException ex)
        {
            string state = ValidationHelper.GetString(ex.ExceptionState, string.Empty);
            if (state == CMSThread.ABORT_REASON_STOP)
            {
                // When canceled
                AddError(ResHelper.GetString("Deployment.DeploymentCanceled"));
            }
            else
            {
                // Log error
                LogExceptionToEventLog(ex);
            }
        }
        catch (IOExceptions.IOException ex)
        {
            LogExceptionToEventLog(ex);
        }
        catch (Exception ex)
        {
            // Log error
            LogExceptionToEventLog(ex);
        }
    }


    private void Synchronize(object parameter)
    {
        try
        {
            if (chkSaveLayouts.Checked)
            {
                ProcessLayouts(true, false);
            }

            if (chkSavePageTemplate.Checked)
            {
                ProcessTemplates(true, false);
            }

            if (chkSaveTransformation.Checked)
            {
                ProcessTransformations(true, false);
            }

            if (chkSaveWebpartLayout.Checked)
            {
                ProcessWebpartLayouts(true, false);
            }

            if (chkSaveWebpartContainer.Checked)
            {
                ProcessWebpartContainers(true, false);
            }

            if (chkSaveCSS.Checked)
            {
                ProcessCSS(true, false);
            }

            if (chkSaveAltFormLayouts.Checked)
            {
                ProcessAltFormLayouts(true, false);
            }

            if (chkSaveFormLayouts.Checked)
            {
                ProcessFormLayouts(true, false);
            }
        }
        catch (ThreadAbortException ex)
        {
            string state = ValidationHelper.GetString(ex.ExceptionState, string.Empty);
            if (state == CMSThread.ABORT_REASON_STOP)
            {
                // When canceled
                AddError(ResHelper.GetString("Deployment.DeploymentCanceled"));
            }
            else
            {
                // Log error
                LogExceptionToEventLog(ex);
            }
        }
        catch (IOExceptions.IOException ex)
        {
            LogExceptionToEventLog(ex);
        }
        catch (Exception ex)
        {
            // Log error
            LogExceptionToEventLog(ex);
        }
    }


    protected void btnSynchronize_Click(object sender, EventArgs e)
    {
        pnlLog.Visible = true;

        CurrentError = string.Empty;
        CurrentLog.Close();
        EnsureLog();

        ctlAsync.RunAsync(Synchronize, WindowsIdentity.GetCurrent());
    }


    protected void btnSourceControl_Click(object sender, EventArgs e)
    {
        pnlLog.Visible = true;

        CurrentError = string.Empty;
        CurrentLog.Close();
        EnsureLog();

        ctlAsync.Parameter = new bool[] { chkSaveCSS.Checked, chkSaveLayouts.Checked, chkSavePageTemplate.Checked, chkSaveTransformation.Checked, chkSaveWebpartLayout.Checked, chkSaveWebpartContainer.Checked, chkSaveAltFormLayouts.Checked, chkSaveFormLayouts.Checked };
        ctlAsync.RunAsync(SaveExternally, WindowsIdentity.GetCurrent());
    }


    protected void btnSaveAll_Click(object sender, EventArgs e)
    {
        pnlLog.Visible = true;

        CurrentError = string.Empty;
        CurrentLog.Close();
        EnsureLog();

        ctlAsync.RunAsync(Deploy, WindowsIdentity.GetCurrent());
    }


    protected void btnTest_Click(object sender, EventArgs e)
    {
        pnlLog.Visible = true;

        CurrentError = string.Empty;
        CurrentLog.Close();
        EnsureLog();

        ctlAsync.RunAsync(Test, WindowsIdentity.GetCurrent());
    }

    #endregion


    #region "Async methods"

    /// <summary>
    /// When exception occures, log it to event log.
    /// </summary>
    /// <param name="ex">Exception to log</param>
    private void LogExceptionToEventLog(Exception ex)
    {
        EventLogProvider.LogEvent(EventType.ERROR, "System deployment", "DEPLOYMENT", EventLogProvider.GetExceptionLogMessage(ex), RequestContext.RawURL, CurrentUser.UserID, CurrentUser.UserName, 0, null, RequestContext.UserHostAddress, SiteContext.CurrentSiteID);

        AddError(ResHelper.GetString("Deployment.DeploymentFailed") + ": " + ex.Message);
    }


    private void ctlAsync_OnCancel(object sender, EventArgs e)
    {
        pnlLog.Visible = false;

        ctlAsync.Parameter = null;
        string cancel = GetString("general.actioncanceled");
        AddLog(cancel);
        ltlScript.Text += ScriptHelper.GetScript("var __pendingCallbacks = new Array(); RefreshCurrent();");

        CurrentLog.Close();

        if (!string.IsNullOrEmpty(CurrentError))
        {
            ShowError(CurrentError);
            return;
        }

        ShowConfirmation(cancel);
    }


    private void ctlAsync_OnRequestLog(object sender, EventArgs e)
    {
        ctlAsync.Log = CurrentLog.Log;
    }


    private void ctlAsync_OnError(object sender, EventArgs e)
    {
        if (ctlAsync.Status == AsyncWorkerStatusEnum.Running)
        {
            ctlAsync.Stop();
        }
        ctlAsync.Parameter = null;
        ShowError(CurrentError);
        CurrentLog.Close();

        pnlLog.Visible = false;
    }


    private void ctlAsync_OnFinished(object sender, EventArgs e)
    {
        CurrentLog.Close();
        pnlLog.Visible = false;

        if (!string.IsNullOrEmpty(CurrentError))
        {
            ctlAsync.Parameter = null;
            ShowError(CurrentError);
            return;
        }

        if (SettingsKeyInfoProvider.DeploymentMode)
        {
            ShowConfirmation(GetString("Deployment.ObjectsSavedSuccessfully"));
        }
        else
        {
            ShowChangesSaved();
        }
    }


    /// <summary>
    /// Ensures the logging context.
    /// </summary>
    protected LogContext EnsureLog()
    {
        LogContext log = LogContext.EnsureLog(ctlAsync.ProcessGUID);
        return log;
    }


    /// <summary>
    /// Adds the log information.
    /// </summary>
    /// <param name="newLog">New log information</param>
    protected void AddLog(string newLog)
    {
        EnsureLog();
        LogContext.AppendLine(newLog);
    }


    /// <summary>
    /// Adds the error to collection of errors.
    /// </summary>
    /// <param name="error">Error message</param>
    protected void AddError(string error)
    {
        AddLog(error);
        CurrentError = (error + "<br />" + CurrentError);
    }

    #endregion
}