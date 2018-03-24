using System;
using System.Collections.Generic;
using System.Web;

using CMS.DocumentEngine;
using CMS.Helpers;
using CMS.IO;
using CMS.MediaLibrary;
using CMS.PortalEngine;
using CMS.Base;
using CMS.SiteProvider;
using CMS.UIControls;
using CMS.DataEngine;

public partial class CMSPages_MediaLibrary_GetMediaFile : GetFilePage
{
    #region "Advanced settings"

    /// <summary>
    /// Sets to false to disable the client caching.
    /// </summary>
    protected bool useClientCache = true;

    /// <summary>
    /// Sets to 0 if you do not wish to cache large files.
    /// </summary>
    protected int largeFilesCacheMinutes = 1;

    #endregion


    #region "Variables"

    protected CMSOutputMediaFile outputFile = null;
    protected Guid fileGuid = Guid.Empty;
    protected MediaFileInfo fileInfo = null;

    #endregion


    #region "Properties"

    /// <summary>
    /// Returns true if file preview is used for sending.
    /// </summary>
    public bool Preview
    {
        get
        {
            return QueryHelper.GetBoolean("preview", false);
        }
    }


    /// <summary>
    /// Returns true if the process allows cache.
    /// </summary>
    public override bool AllowCache
    {
        get
        {
            if (mAllowCache == null)
            {
                // By default, cache for the metafiles is based on the view mode
                mAllowCache = ValidationHelper.GetBoolean(SettingsHelper.AppSettings["CMSAlwaysCacheMediaFiles"], false) || ViewMode.IsLiveSite();
            }

            return mAllowCache.Value;
        }
        set
        {
            mAllowCache = value;
        }
    }

    #endregion


    protected void Page_Load(object sender, EventArgs e)
    {
        DebugHelper.SetContext("GetMediaFile");

        // Load the site name
        LoadSiteName();

        // Check the site
        if (string.IsNullOrEmpty(CurrentSiteName))
        {
            throw new Exception("[GetMediaFile.aspx]: Site not running.");
        }

        int cacheMinutes = CacheMinutes;

        // Try to get data from cache
        using (var cs = new CachedSection<CMSOutputMediaFile>(ref outputFile, cacheMinutes, true, null, "getmediafile", CurrentSiteName, GetBaseCacheKey(), Request.QueryString))
        {
            if (cs.LoadData)
            {
                // Process the file
                ProcessFile();

                // Ensure the cache settings
                if (cs.Cached)
                {
                    // Prepare the cache dependency
                    CMSCacheDependency cd = null;
                    if (outputFile != null)
                    {
                        if (outputFile.MediaFile != null)
                        {
                            // Do not cache too big files
                            if ((outputFile.MediaFile != null) && !CacheHelper.CacheImageAllowed(CurrentSiteName, (int)outputFile.MediaFile.FileSize))
                            {
                                cacheMinutes = largeFilesCacheMinutes;
                            }

                            // Set dependency on this particular file
                            if (cacheMinutes > 0)
                            {
                                List<string> dependencies = new List<string>();
                                dependencies.AddRange(MediaFileInfoProvider.GetDependencyCacheKeys(outputFile.MediaFile, Preview));
                                cd = GetCacheDependency(dependencies);
                            }
                        }
                    }

                    if ((cd == null) && (cacheMinutes > 0))
                    {
                        // Set default cache dependency by file GUID
                        cd = CacheHelper.GetCacheDependency(new string[] { "mediafile|" + fileGuid.ToString().ToLowerCSafe(), "mediafilepreview|" + fileGuid.ToString().ToLowerCSafe() });
                    }

                    // Cache the data
                    cs.CacheMinutes = cacheMinutes;
                    cs.CacheDependency = cd;
                }

                cs.Data = outputFile;
            }
        }

        // Send the data
        SendFile(outputFile);

        DebugHelper.ReleaseContext();
    }


    /// <summary>
    /// Processes the file.
    /// </summary>
    protected void ProcessFile()
    {
        outputFile = null;

        // Get file guid from querystring
        fileGuid = QueryHelper.GetGuid("fileguid", Guid.Empty);
        if (fileGuid != Guid.Empty)
        {
            ProcessFile(fileGuid, Preview);
        }
    }


    /// <summary>
    /// Processes the specified file.
    /// </summary>
    /// <param name="fileGuid">File guid</param>
    /// <param name="preview">Use preview</param>
    protected void ProcessFile(Guid fileGuid, bool preview)
    {
        // Get the file info if doesn't retrieved yet
        fileInfo = (fileInfo ?? MediaFileInfoProvider.GetMediaFileInfo(fileGuid, CurrentSiteName));
        if (fileInfo != null)
        {
            if (preview)
            {
                // Get file path
                string path = MediaFileInfoProvider.GetMediaFilePath(fileInfo.FileLibraryID, fileInfo.FilePath);
                string pathDirectory = Path.GetDirectoryName(path);
                string hiddenFolderPath = MediaLibraryHelper.GetMediaFileHiddenFolder(CurrentSiteName);
                string folderPath = DirectoryHelper.CombinePath(pathDirectory, hiddenFolderPath);


                // Ensure hidden folder exists
                DirectoryHelper.EnsureDiskPath(folderPath, pathDirectory);
                // Get preview file
                string[] files = Directory.GetFiles(folderPath, MediaLibraryHelper.GetPreviewFileName(fileInfo.FileName, fileInfo.FileExtension, ".*", CurrentSiteName));
                if (files.Length > 0)
                {
                    bool resizeImage = (ImageHelper.IsImage(Path.GetExtension(files[0])) && MediaFileInfoProvider.CanResizeImage(files[0], Width, Height, MaxSideSize));

                    // Get the data
                    if ((outputFile == null) || (outputFile.MediaFile == null))
                    {
                        outputFile = NewOutputFile(fileInfo, null);
                        outputFile.UsePreview = true;
                        outputFile.Width = Width;
                        outputFile.Height = Height;
                        outputFile.MaxSideSize = MaxSideSize;
                        outputFile.Resized = resizeImage;
                        outputFile.FileExtension = Path.GetExtension(files[0]);
                        outputFile.MimeType = MimeTypeHelper.GetMimetype(outputFile.FileExtension);
                    }
                }
            }
            else
            {
                bool resizeImage = (ImageHelper.IsImage(fileInfo.FileExtension) && MediaFileInfoProvider.CanResizeImage(fileInfo, Width, Height, MaxSideSize));

                // Get the data
                if ((outputFile == null) || (outputFile.MediaFile == null))
                {
                    outputFile = NewOutputFile(fileInfo, null);
                    outputFile.Width = Width;
                    outputFile.Height = Height;
                    outputFile.MaxSideSize = MaxSideSize;
                    outputFile.Resized = resizeImage;
                }
            }
        }
    }


    /// <summary>
    /// Sends the given file within response.
    /// </summary>
    /// <param name="file">File to send</param>
    protected void SendFile(CMSOutputMediaFile file)
    {
        // Clear response.
        CookieHelper.ClearResponseCookies();
        Response.Clear();

        // Set the revalidation
        SetRevalidation();

        // Send the file
        if (file != null)
        {
            #region "Security"

            // Check if user is allowed to see the library file content if required
            bool checkPermissions = SettingsKeyInfoProvider.GetBoolValue(SiteContext.CurrentSiteName + ".CMSCheckMediaFilePermissions");
            if (checkPermissions)
            {
                // Check the library access for the current user and stop file processing if not allowed
                MediaLibraryInfo mli = MediaLibraryInfoProvider.GetMediaLibraryInfo(file.MediaFile.FileLibraryID);
                if (!MediaLibraryInfoProvider.IsUserAuthorizedPerLibrary(mli, "LibraryAccess"))
                {
                    URLHelper.Redirect(PageSecurityHelper.AccessDeniedPageURL(CurrentSiteName));
                    return;
                }
            }

            #endregion


            // Prepare etag
            string etag = "";
            if (file.MediaFile != null)
            {
                etag += file.MediaFile.FileModifiedWhen.ToUniversalTime();
            }

            // Put etag into ""
            etag = String.Format("\"{0}\"", etag);

            // Client caching - only on the live site
            if (useClientCache && AllowCache && ETagsMatch(etag, file.LastModified))
            {
                // Set correct response content type
                SetResponseContentType(file);

                // Set the file time stamps to allow client caching
                SetTimeStamps(file);

                RespondNotModified(etag, true);
                return;
            }

            // If physical file not present, try to load
            EnsurePhysicalFile(outputFile);

            // If the output data should be cached, return the output data
            bool cacheOutputData = false;
            if ((file.MediaFile != null) && (CacheMinutes > 0))
            {
                cacheOutputData = CacheHelper.CacheImageAllowed(CurrentSiteName, (int)file.MediaFile.FileSize);
            }

            // Ensure the file data if physical file not present
            if (!file.DataLoaded && (file.PhysicalFile == ""))
            {
                byte[] cachedData = GetCachedOutputData();
                if (file.EnsureData(cachedData))
                {
                    if ((cachedData == null) && cacheOutputData)
                    {
                        SaveOutputDataToCache(file.OutputData, GetOutputDataDependency(file.MediaFile));
                    }
                }
            }

            // Send the file
            if ((file.OutputData != null) || (file.PhysicalFile != ""))
            {
                // Set correct response content type
                SetResponseContentType(file);

                string extension = file.FileExtension;
                SetDisposition(file.FileName + extension, extension);

                // Setup Etag property
                ETag = etag;

                // Set if resumable downloads should be supported
                AcceptRange = !IsExtensionExcludedFromRanges(extension);

                if (useClientCache && AllowCache)
                {
                    // Set the file time stamps to allow client caching
                    SetTimeStamps(file);

                    Response.Cache.SetETag(etag);
                }

                // Add the file data
                if ((file.PhysicalFile != "") && (file.OutputData == null))
                {
                    if (!File.Exists(file.PhysicalFile))
                    {
                        // File doesn't exist
                        NotFound();
                    }
                    else
                    {
                        // Stream the file from the file system
                        file.OutputData = WriteFile(file.PhysicalFile, cacheOutputData);
                    }
                }
                else
                {
                    // Use output data of the file in memory if present
                    WriteBytes(file.OutputData);
                }
            }
            else
            {
                NotFound();
            }
        }
        else
        {
            NotFound();
        }

        CompleteRequest();
    }


    /// <summary>
    /// Sets content type of the response based on file MIME type
    /// </summary>
    /// <param name="file">Output file</param>
    private void SetResponseContentType(CMSOutputMediaFile file)
    {
        string mimeType = file.MimeType;
        if (file.MediaFile != null)
        {
            string extension = file.MediaFile.FileExtension;
            switch (extension.ToLowerCSafe())
            {
                case ".flv":
                    // Correct MIME type
                    mimeType = "video/x-flv";
                    break;
            }
        }

        // Set content type
        Response.ContentType = mimeType;
    }


    /// <summary>
    /// Sets the last modified and expires header to the response
    /// </summary>
    /// <param name="file">Output file data</param>
    private void SetTimeStamps(CMSOutputMediaFile file)
    {
        DateTime expires = DateTime.Now;

        // Send last modified header to allow client caching
        Response.Cache.SetLastModified(file.LastModified);

        Response.Cache.SetCacheability(HttpCacheability.Public);
        if (AllowClientCache)
        {
            expires = DateTime.Now.AddMinutes(ClientCacheMinutes);
        }

        Response.Cache.SetExpires(expires);
    }


    /// <summary>
    /// Returns the output data dependency based on the given attachment record.
    /// </summary>
    /// <param name="mi">MediaFile object</param>
    protected CMSCacheDependency GetOutputDataDependency(MediaFileInfo mi)
    {
        if (mi == null)
        {
            return null;
        }

        return CacheHelper.GetCacheDependency(MediaFileInfoProvider.GetDependencyCacheKeys(mi, Preview));
    }


    /// <summary>
    /// Ensures the physical file.
    /// </summary>
    /// <param name="file">Output file</param>
    public bool EnsurePhysicalFile(CMSOutputMediaFile file)
    {
        // Try to link to file system
        if ((file != null) && String.IsNullOrEmpty(file.Watermark) && (file.MediaFile != null) && (file.MediaFile.FileID > 0))
        {
            SiteInfo si = SiteInfoProvider.GetSiteInfo(file.MediaFile.FileSiteID);
            if (si != null)
            {
                bool generateThumbnails = ValidationHelper.GetBoolean(SettingsKeyInfoProvider.GetStringValue(si.SiteName + ".CMSGenerateThumbnails"), true);
                string filePath = null;
                string libraryFolder = Path.EnsureEndBackslash(MediaLibraryInfoProvider.GetMediaLibraryFolderPath(file.MediaFile.FileLibraryID));

                if (file.Resized && generateThumbnails)
                {
                    filePath = libraryFolder + MediaFileInfoProvider.EnsureThumbnailFile(file.MediaFile, file.SiteName, Width, Height, MaxSideSize, file.UsePreview);
                }
                else
                {
                    if (file.UsePreview)
                    {
                        // Get file path
                        string path = MediaFileInfoProvider.GetMediaFilePath(file.MediaFile.FileLibraryID, file.MediaFile.FilePath);
                        string pathDirectory = Path.GetDirectoryName(path);
                        string hiddenFolderPath = MediaLibraryHelper.GetMediaFileHiddenFolder(CurrentSiteName);
                        string folderPath = String.Format("{0}\\{1}", pathDirectory, hiddenFolderPath);

                        // Ensure hidden folder exists
                        DirectoryHelper.EnsureDiskPath(folderPath, pathDirectory);
                        // Get preview file
                        string[] files = Directory.GetFiles(folderPath, MediaLibraryHelper.GetPreviewFileName(file.MediaFile.FileName, file.MediaFile.FileExtension, ".*", CurrentSiteName));
                        if (files.Length > 0)
                        {
                            filePath = files[0];
                        }
                    }
                    else
                    {
                        filePath = libraryFolder + file.MediaFile.FilePath;
                    }
                }

                if (filePath != null)
                {
                    // Link to the physical file
                    file.PhysicalFile = filePath;
                    return true;
                }
            }
        }

        file.PhysicalFile = "";
        return false;
    }


    /// <summary>
    /// Gets the new output MediaFile object.
    /// </summary>
    public CMSOutputMediaFile NewOutputFile()
    {
        CMSOutputMediaFile mf = new CMSOutputMediaFile
                                    {
                                        Watermark = Watermark,
                                        WatermarkPosition = WatermarkPosition
                                    };

        return mf;
    }


    /// <summary>
    /// Gets the new output MediaFile object.
    /// </summary>
    /// <param name="mfi">Media file info</param>
    /// <param name="data">Output MediaFile data</param>
    public CMSOutputMediaFile NewOutputFile(MediaFileInfo mfi, byte[] data)
    {
        CMSOutputMediaFile mf = new CMSOutputMediaFile(mfi, data)
                                    {
                                        Watermark = Watermark,
                                        WatermarkPosition = WatermarkPosition
                                    };

        return mf;
    }
}