using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.Controls;
using CMS.Helpers;
using CMS.IO;
using CMS.MediaLibrary;
using CMS.SiteProvider;
using CMS.UIControls;

public partial class CMSModules_MediaLibrary_Controls_LiveControls_MediaGallery : CMSAdminControl
{
    #region "Variables"

    private MediaLibraryInfo mMediaLibrary = null;
    private string mMediaLibraryName = null;
    private string mMediaLibraryPath = null;
    private bool mHideFolderTree = false;
    private string mTransformationName = null;
    private string mSelectedItemTransformation = null;
    private string mHeaderTransformation = null;
    private string mFooterTransformation = null;
    private string mSeparatorTransformation = null;
    private bool mShowSubfoldersContent = false;
    private string mMediaLibraryRoot = null;
    private string mMediaLibraryUrl = null;
    private int mMediaFileID = 0;
    private string mPreviewSuffix = null;
    private string mIconSet = null;
    private bool mDisplayActiveContent = true;
    private bool? mDisplayDetail = null;
    private bool mAllowUpload = false;
    private bool mAllowUploadPreview = false;
    private bool mDisplayFileCount = false;
    private bool mUseSecureLinks = true;

    private int mFilterMethod = 0;
    private string mFileIDQueryStringKey = null;
    private string mSortQueryStringKey = null;
    private string mPathQueryStringKey = null;

    private int mSelectTopN = 0;
    private bool mHideControlForZeroRows = true;
    private string mZeroRowsText = null;

    private string mPagesTemplate = null;
    private string mCurrentPageTemplate = null;
    private string mSeparatorTemplate = null;
    private string mFirstPageTemplate = null;
    private string mLastPageTemplate = null;
    private string mPreviousPageTemplate = null;
    private string mNextPageTemplate = null;
    private string mPreviousGroupTemplate = null;
    private string mNextGroupTemplate = null;
    private string mLayoutTemplate = null;

    private bool hidden = false;

    #endregion


    #region "Content properties"

    /// <summary>
    /// Gets or sets the number which indicates how many files should be displayed.
    /// </summary>
    public int SelectTopN
    {
        get
        {
            return mSelectTopN;
        }
        set
        {
            mSelectTopN = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether control should be hidden if no data found.
    /// </summary>
    public bool HideControlForZeroRows
    {
        get
        {
            return mHideControlForZeroRows;
        }
        set
        {
            mHideControlForZeroRows = value;
        }
    }


    /// <summary>
    /// Gets or sets the text which is displayed for zero rows result.
    /// </summary>
    public string ZeroRowsText
    {
        get
        {
            return mZeroRowsText;
        }
        set
        {
            mZeroRowsText = value;
        }
    }


    /// <summary>
    /// Returns true if file ID query string is pressent.
    /// </summary>
    public bool? DisplayDetail
    {
        get
        {
            if (mDisplayDetail == null)
            {
                if (QueryHelper.GetInteger(FileIDQueryStringKey, 0) > 0)
                {
                    mDisplayDetail = true;
                }
                else
                {
                    mDisplayDetail = false;
                }
            }
            return mDisplayDetail;
        }
    }

    #endregion


    #region "UniPager properties"

    /// <summary>
    /// Gets or sets the unipager control.
    /// </summary>
    public UniPager UniPager
    {
        get
        {
            return UniPagerControl;
        }
        set
        {
            UniPagerControl = value;
        }
    }

    /// <summary>
    /// Gets or sets the value that indicates whether pager should be hidden for single page.
    /// </summary>
    public bool HidePagerForSinglePage
    {
        get
        {
            return UniPagerControl.HidePagerForSinglePage;
        }
        set
        {
            UniPagerControl.HidePagerForSinglePage = value;
        }
    }


    /// <summary>
    /// Gets or sets the number of records to display on a page.
    /// </summary>
    public int PageSize
    {
        get
        {
            return UniPagerControl.PageSize;
        }
        set
        {
            UniPagerControl.PageSize = value;
        }
    }


    /// <summary>
    /// Gets or sets the number of pages displayed for current page range.
    /// </summary>
    public int GroupSize
    {
        get
        {
            return UniPagerControl.GroupSize;
        }
        set
        {
            UniPagerControl.GroupSize = value;
        }
    }


    /// <summary>
    /// Gets or sets the pager mode ('querystring' or 'postback').
    /// </summary>
    public UniPagerMode PagerMode
    {
        get
        {
            return UniPagerControl.PagerMode;
        }
        set
        {
            UniPagerControl.PagerMode = value;
        }
    }


    /// <summary>
    /// Gets or sets the querysting parameter.
    /// </summary>
    public string QueryStringKey
    {
        get
        {
            return UniPagerControl.QueryStringKey;
        }
        set
        {
            UniPagerControl.QueryStringKey = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether first and last item template are displayed dynamically based on current view.
    /// </summary>
    public bool DisplayFirstLastAutomatically
    {
        get
        {
            return UniPagerControl.DisplayFirstLastAutomatically;
        }
        set
        {
            UniPagerControl.DisplayFirstLastAutomatically = value;
        }
    }


    /// <summary>
    /// Gets or sets the value that indicates whether first and last item template are displayed dynamically based on current view.
    /// </summary>
    public bool DisplayPreviousNextAutomatically
    {
        get
        {
            return UniPagerControl.DisplayPreviousNextAutomatically;
        }
        set
        {
            UniPagerControl.DisplayPreviousNextAutomatically = value;
        }
    }

    #endregion


    #region "UniPager Template properties"

    /// <summary>
    /// Gets or sets the pages template.
    /// </summary>
    public string PagesTemplate
    {
        get
        {
            return mPagesTemplate;
        }
        set
        {
            mPagesTemplate = value;
        }
    }


    /// <summary>
    /// Gets or sets the current page template.
    /// </summary>
    public string CurrentPageTemplate
    {
        get
        {
            return mCurrentPageTemplate;
        }
        set
        {
            mCurrentPageTemplate = value;
        }
    }


    /// <summary>
    /// Gets or sets the separator template.
    /// </summary>
    public string SeparatorTemplate
    {
        get
        {
            return mSeparatorTemplate;
        }
        set
        {
            mSeparatorTemplate = value;
        }
    }


    /// <summary>
    /// Gets or sets the first page template.
    /// </summary>
    public string FirstPageTemplate
    {
        get
        {
            return mFirstPageTemplate;
        }
        set
        {
            mFirstPageTemplate = value;
        }
    }


    /// <summary>
    /// Gets or sets the last page template.
    /// </summary>
    public string LastPageTemplate
    {
        get
        {
            return mLastPageTemplate;
        }
        set
        {
            mLastPageTemplate = value;
        }
    }


    /// <summary>
    /// Gets or sets the previous page template.
    /// </summary>
    public string PreviousPageTemplate
    {
        get
        {
            return mPreviousPageTemplate;
        }
        set
        {
            mPreviousPageTemplate = value;
        }
    }


    /// <summary>
    /// Gets or sets the next page template.
    /// </summary>
    public string NextPageTemplate
    {
        get
        {
            return mNextPageTemplate;
        }
        set
        {
            mNextPageTemplate = value;
        }
    }


    /// <summary>
    /// Gets or sets the previous group template.
    /// </summary>
    public string PreviousGroupTemplate
    {
        get
        {
            return mPreviousGroupTemplate;
        }
        set
        {
            mPreviousGroupTemplate = value;
        }
    }


    /// <summary>
    /// Gets or sets the next group template.
    /// </summary>
    public string NextGroupTemplate
    {
        get
        {
            return mNextGroupTemplate;
        }
        set
        {
            mNextGroupTemplate = value;
        }
    }


    /// <summary>
    /// Gets or sets the layout template.
    /// </summary>
    public string LayoutTemplate
    {
        get
        {
            return mLayoutTemplate;
        }
        set
        {
            mLayoutTemplate = value;
        }
    }

    #endregion


    #region "Public properties"

    /// <summary>
    /// Gets or sets the name of the transforamtion which is used for displaying the results.
    /// </summary>
    public string TransformationName
    {
        get
        {
            return mTransformationName;
        }
        set
        {
            mTransformationName = value;
        }
    }


    /// <summary>
    /// Gets or sets the name of the transforamtion which is used for displaying selected file.
    /// </summary>
    public string SelectedItemTransformation
    {
        get
        {
            return mSelectedItemTransformation;
        }
        set
        {
            mSelectedItemTransformation = value;
        }
    }


    /// <summary>
    /// Gets or sets the name of the transforamtion which is used for displaying file list header.
    /// </summary>
    public string HeaderTransformation
    {
        get
        {
            return mHeaderTransformation;
        }
        set
        {
            mHeaderTransformation = value;
        }
    }


    /// <summary>
    /// Gets or sets the name of the transforamtion which is used for displaying file list footer.
    /// </summary>
    public string FooterTransformation
    {
        get
        {
            return mFooterTransformation;
        }
        set
        {
            mFooterTransformation = value;
        }
    }


    /// <summary>
    /// Gets or sets the name of the transforamtion which is used for item separator.
    /// </summary>
    public string SeparatorTransformation
    {
        get
        {
            return mSeparatorTransformation;
        }
        set
        {
            mSeparatorTransformation = value;
        }
    }


    /// <summary>
    /// Media library name.
    /// </summary>
    public string MediaLibraryName
    {
        get
        {
            return mMediaLibraryName;
        }
        set
        {
            mMediaLibraryName = value;
        }
    }


    /// <summary>
    /// Gets or sets media library path to display files from.
    /// </summary>
    public string MediaLibraryPath
    {
        get
        {
            if (mMediaLibraryPath != null)
            {
                return mMediaLibraryPath.Trim('/');
            }
            return mMediaLibraryPath;
        }
        set
        {
            mMediaLibraryPath = value;
        }
    }


    /// <summary>
    /// Indicates if subfolders content should be displayed.
    /// </summary>
    public bool ShowSubfoldersContent
    {
        get
        {
            return mShowSubfoldersContent;
        }
        set
        {
            mShowSubfoldersContent = value;
        }
    }


    /// <summary>
    /// File list folder path.
    /// </summary>
    public string FolderPath
    {
        get
        {
            return ValidationHelper.GetString(ViewState["FolderPath"], string.Empty);
        }
        set
        {
            ViewState["FolderPath"] = value;
        }
    }


    /// <summary>
    /// Gets or sets the file id querystring parameter.
    /// </summary>
    public string FileIDQueryStringKey
    {
        get
        {
            return mFileIDQueryStringKey;
        }
        set
        {
            mFileIDQueryStringKey = value;
        }
    }


    /// <summary>
    /// Gets or sets the sort querystring parameter.
    /// </summary>
    public string SortQueryStringKey
    {
        get
        {
            return mSortQueryStringKey;
        }
        set
        {
            mSortQueryStringKey = value;
        }
    }


    /// <summary>
    /// Gets or sets the path querystring parameter.
    /// </summary>
    public string PathQueryStringKey
    {
        get
        {
            return mPathQueryStringKey;
        }
        set
        {
            mPathQueryStringKey = value;
        }
    }


    /// <summary>
    /// Gets or sets filter method.
    /// </summary>
    public int FilterMethod
    {
        get
        {
            return mFilterMethod;
        }
        set
        {
            mFilterMethod = value;
        }
    }


    /// <summary>
    /// Media library info object.
    /// </summary>
    public MediaLibraryInfo MediaLibrary
    {
        get
        {
            if (mMediaLibrary == null)
            {
                mMediaLibrary = MediaLibraryInfoProvider.GetMediaLibraryInfo(MediaLibraryName, SiteContext.CurrentSiteName);
            }
            return mMediaLibrary;
        }
    }


    /// <summary>
    /// Hide folder tree.
    /// </summary>
    public bool HideFolderTree
    {
        get
        {
            return mHideFolderTree;
        }
        set
        {
            mHideFolderTree = value;
        }
    }


    /// <summary>
    /// Preview prefix for identification preview file.
    /// </summary>
    public string PreviewSuffix
    {
        get
        {
            return mPreviewSuffix;
        }
        set
        {
            mPreviewSuffix = value;
        }
    }


    /// <summary>
    /// Icon set name.
    /// </summary>
    public string IconSet
    {
        get
        {
            return mIconSet;
        }
        set
        {
            mIconSet = value;
        }
    }


    /// <summary>
    /// Indicates if active content (video, flash etc.) should be displayed.
    /// </summary>
    public bool DisplayActiveContent
    {
        get
        {
            return mDisplayActiveContent;
        }
        set
        {
            mDisplayActiveContent = value;
        }
    }


    /// <summary>
    /// Indicates if file count in directory should be displayed in folder tree.
    /// </summary>
    public bool DisplayFileCount
    {
        get
        {
            return mDisplayFileCount;
        }
        set
        {
            mDisplayFileCount = value;
        }
    }


    /// <summary>
    /// Indicates if file upload form should be displayed.
    /// </summary>
    public bool AllowUpload
    {
        get
        {
            return mAllowUpload;
        }
        set
        {
            mAllowUpload = value;
        }
    }


    /// <summary>
    /// Indicates if preview file upload should be displayed in upload form.
    /// </summary>
    public bool AllowUploadPreview
    {
        get
        {
            return mAllowUploadPreview;
        }
        set
        {
            mAllowUploadPreview = value;
        }
    }


    /// <summary>
    /// Indicates whether the links to media file should be processed in a secure way.
    /// </summary>
    public bool UseSecureLinks
    {
        get
        {
            return mUseSecureLinks;
        }
        set
        {
            mUseSecureLinks = value;
        }
    }

    #endregion


    #region "Life cycle methods"

    protected override void CreateChildControls()
    {
        // Hide the control if there is no MediaLibrary
        if (MediaLibrary == null)
        {
            hidden = true;
            Visible = false;
            StopProcessing = true;
            return;
        }

        if (StopProcessing)
        {
            folderTree.StopProcessing = true;
            fileDataSource.StopProcessing = true;
            UniPagerControl.PageControl = null;
        }
        else
        {
            if (!MediaLibraryInfoProvider.IsUserAuthorizedPerLibrary(MediaLibrary, "Read"))
            {
                // Check 'Media gallery access' permission
                if (!MediaLibraryInfoProvider.IsUserAuthorizedPerLibrary(MediaLibrary, "libraryaccess"))
                {
                    RaiseOnNotAllowed("libraryaccess");
                    return;
                }
            }

            base.CreateChildControls();
            InitializeInnerControls();
        }
    }


    protected override void OnPreRender(EventArgs e)
    {
        if (HideFolderTree)
        {
            folderTree.Visible = false;
            folderTreeContainer.Visible = false;
        }

        int fileId = QueryHelper.GetInteger(FileIDQueryStringKey, 0);
        bool hasFilter = false;
        if (fileId > 0)
        {
            hasFilter = true;
            fileDataSource.WhereCondition = "FileID = " + fileId.ToString("D", System.Globalization.CultureInfo.InvariantCulture);
            fileDataSource.OrderBy = null;
            fileDataSource.FilePath = null;
            // Hide uploader
            fileUploader.Visible = false;
        }
        else
        {
            if (MediaLibrary != null)
            {
                hasFilter = true;
                fileDataSource.OrderBy = mediaLibrarySort.OrderBy;
                fileDataSource.LibraryName = MediaLibraryName;
                fileDataSource.WhereCondition = folderTree.WhereCondition;
            }
        }
        // Bind data into fileList
        if (hasFilter)
        {
            fileList.DataSource = fileDataSource.DataSource;
            fileList.DataBind();
        }

        base.OnPreRender(e);
    }

    #endregion


    #region "Public methods"

    /// <summary>
    /// Reloads the data in the control.
    /// </summary>
    public override void ReloadData()
    {
        fileDataSource.InvalidateLoadedData();
    }

    #endregion


    #region "Private methods"
    
    private void FilterChanged()
    {
        // Set uploader if upload is allowed
        if (AllowUpload)
        {
            fileUploader.DestinationPath = folderTree.CurrentFolder;
        }
        fileDataSource.InvalidateLoadedData();
    }


    private void fileList_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        if (e.Item.Controls.Count > 0)
        {
            // Try find control with id 'filePreview'
            MediaFilePreview ctrlFilePreview = e.Item.Controls[0].FindControl("filePreview") as MediaFilePreview;
            if (ctrlFilePreview != null)
            {
                //Set control
                ctrlFilePreview.IconSet = IconSet;
                ctrlFilePreview.PreviewSuffix = PreviewSuffix;
                ctrlFilePreview.UseSecureLinks = UseSecureLinks;
                if (DisplayDetail == true)
                {
                    // If showing detail show active control
                    ctrlFilePreview.DisplayActiveContent = true;
                }
                else
                {
                    ctrlFilePreview.DisplayActiveContent = DisplayActiveContent;
                }
            }
        }
    }


    private void InitializeInnerControls()
    {
        if (MediaLibrary != null)
        {
            // If the control was hidden because there were no data on init, show the control and process it
            if (hidden)
            {
                Visible = true;
                StopProcessing = false;
                folderTree.StopProcessing = false;
                fileDataSource.StopProcessing = false;
            }

            if (string.IsNullOrEmpty(MediaLibraryPath))
            {
                // If there is no path set
                folderTree.RootFolderPath = MediaLibraryHelper.GetMediaRootFolderPath(SiteContext.CurrentSiteName);
                folderTree.MediaLibraryFolder = MediaLibrary.LibraryFolder;
            }
            else
            {
                // Set root folder with library path
                folderTree.RootFolderPath = MediaLibraryHelper.GetMediaRootFolderPath(SiteContext.CurrentSiteName) + MediaLibrary.LibraryFolder;
                folderTree.MediaLibraryFolder = Path.GetFileName(MediaLibraryPath);
                folderTree.MediaLibraryPath = Path.EnsureSlashes(MediaLibraryPath);
            }

            folderTree.FileIDQueryStringKey = FileIDQueryStringKey;
            folderTree.PathQueryStringKey = PathQueryStringKey;
            folderTree.FilterMethod = FilterMethod;
            folderTree.ShowSubfoldersContent = ShowSubfoldersContent;
            folderTree.DisplayFileCount = DisplayFileCount;

            // Get media file id from query
            mMediaFileID = QueryHelper.GetInteger(FileIDQueryStringKey, 0);

            // Media library sort
            mediaLibrarySort.OnFilterChanged += FilterChanged;
            mediaLibrarySort.FileIDQueryStringKey = FileIDQueryStringKey;
            mediaLibrarySort.SortQueryStringKey = SortQueryStringKey;
            mediaLibrarySort.FilterMethod = FilterMethod;

            // File upload properties
            fileUploader.Visible = AllowUpload;
            fileUploader.EnableUploadPreview = AllowUploadPreview;
            fileUploader.PreviewSuffix = PreviewSuffix;
            fileUploader.LibraryID = MediaLibrary.LibraryID;
            fileUploader.DestinationPath = folderTree.SelectedPath;
            fileUploader.OnNotAllowed += fileUploader_OnNotAllowed;
            fileUploader.OnAfterFileUpload += fileUploader_OnAfterFileUpload;

            // Data properties
            fileDataSource.TopN = SelectTopN;
            fileDataSource.SiteName = SiteContext.CurrentSiteName;
            fileDataSource.GroupID = MediaLibrary.LibraryGroupID;

            // UniPager properties
            UniPagerControl.PageSize = PageSize;
            UniPagerControl.GroupSize = GroupSize;
            UniPagerControl.QueryStringKey = QueryStringKey;
            UniPagerControl.DisplayFirstLastAutomatically = DisplayFirstLastAutomatically;
            UniPagerControl.DisplayPreviousNextAutomatically = DisplayPreviousNextAutomatically;
            UniPagerControl.HidePagerForSinglePage = HidePagerForSinglePage;
            UniPagerControl.PagerMode = PagerMode;

            mMediaLibraryRoot = MediaLibraryHelper.GetMediaRootFolderPath(SiteContext.CurrentSiteName) + MediaLibrary.LibraryFolder;
            mMediaLibraryUrl = URLHelper.GetAbsoluteUrl("~/" + SiteContext.CurrentSiteName + "/media/" + MediaLibrary.LibraryFolder);

            // List properties
            fileList.HideControlForZeroRows = HideControlForZeroRows;
            fileList.ZeroRowsText = ZeroRowsText;
            fileList.ItemDataBound += fileList_ItemDataBound;

            // Initialize templates for FileList and UniPager
            InitTemplates();
        }

        // Append filter changed event if folder is hidden or path query string id is set
        if (!HideFolderTree || !String.IsNullOrEmpty(PathQueryStringKey))
        {
            folderTree.OnFilterChanged += FilterChanged;
        }

        // Folder tree
        if (!HideFolderTree)
        {
            if (CultureHelper.IsPreferredCultureRTL())
            {
                folderTree.ImageFolderPath = GetImageUrl("RTL/Design/Controls/Tree", IsLiveSite, true);
            }
            else
            {
                folderTree.ImageFolderPath = GetImageUrl("Design/Controls/Tree", IsLiveSite, true);
            }
        }
    }


    private void InitTemplates()
    {
        // If is media file id sets use SelectedItemTransformation and hide paging and sorting
        if (mMediaFileID > 0)
        {
            fileList.ItemTemplate = CMSDataProperties.LoadTransformation(this, SelectedItemTransformation);
            UniPagerControl.Visible = false;
            mediaLibrarySort.StopProcessing = true;
            mediaLibrarySort.Visible = false;
        }
        else
        {
            // Else use transformation name
            fileList.ItemTemplate = CMSDataProperties.LoadTransformation(this, TransformationName);
        }

        if (!String.IsNullOrEmpty(HeaderTransformation))
        {
            fileList.HeaderTemplate = CMSDataProperties.LoadTransformation(this, HeaderTransformation);
        }

        if (!String.IsNullOrEmpty(FooterTransformation))
        {
            fileList.FooterTemplate = CMSDataProperties.LoadTransformation(this, FooterTransformation);
        }

        if (!String.IsNullOrEmpty(SeparatorTransformation))
        {
            fileList.SeparatorTemplate = CMSDataProperties.LoadTransformation(this, SeparatorTransformation);
        }

        if (!String.IsNullOrEmpty(PagesTemplate))
        {
            UniPagerControl.PageNumbersTemplate = CMSDataProperties.LoadTransformation(UniPagerControl, PagesTemplate);
        }

        if (!String.IsNullOrEmpty(CurrentPageTemplate))
        {
            UniPagerControl.CurrentPageTemplate = CMSDataProperties.LoadTransformation(UniPagerControl, CurrentPageTemplate);
        }

        if (!String.IsNullOrEmpty(SeparatorTemplate))
        {
            UniPagerControl.PageNumbersSeparatorTemplate = CMSDataProperties.LoadTransformation(UniPagerControl, SeparatorTemplate);
        }

        if (!String.IsNullOrEmpty(FirstPageTemplate))
        {
            UniPagerControl.FirstPageTemplate = CMSDataProperties.LoadTransformation(UniPagerControl, FirstPageTemplate);
        }

        if (!String.IsNullOrEmpty(LastPageTemplate))
        {
            UniPagerControl.LastPageTemplate = CMSDataProperties.LoadTransformation(UniPagerControl, LastPageTemplate);
        }

        if (!String.IsNullOrEmpty(PreviousPageTemplate))
        {
            UniPagerControl.PreviousPageTemplate = CMSDataProperties.LoadTransformation(UniPagerControl, PreviousPageTemplate);
        }

        if (!String.IsNullOrEmpty(NextPageTemplate))
        {
            UniPagerControl.NextPageTemplate = CMSDataProperties.LoadTransformation(UniPagerControl, NextPageTemplate);
        }

        if (!String.IsNullOrEmpty(PreviousGroupTemplate))
        {
            UniPagerControl.PreviousGroupTemplate = CMSDataProperties.LoadTransformation(UniPagerControl, PreviousGroupTemplate);
        }

        if (!String.IsNullOrEmpty(NextGroupTemplate))
        {
            UniPagerControl.NextGroupTemplate = CMSDataProperties.LoadTransformation(UniPagerControl, NextGroupTemplate);
        }

        if (!String.IsNullOrEmpty(LayoutTemplate))
        {
            UniPagerControl.LayoutTemplate = CMSDataProperties.LoadTransformation(UniPagerControl, LayoutTemplate);
        }
    }


    private void fileUploader_OnNotAllowed(string permissionType, CMSAdminControl sender)
    {
        RaiseOnNotAllowed(permissionType);
    }


    private void fileUploader_OnAfterFileUpload()
    {
        fileDataSource.InvalidateLoadedData();
    }

    #endregion
}