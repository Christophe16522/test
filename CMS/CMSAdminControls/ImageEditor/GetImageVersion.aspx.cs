using System;
using System.Data;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.DataEngine;
using CMS.Helpers;
using CMS.Base;
using CMS.SiteProvider;
using CMS.Membership;
using CMS.UIControls;

[HashValidation(HashValidationSalts.GETIMAGEVERSION_PAGE)]
public partial class CMSAdminControls_ImageEditor_GetImageVersion : GetFilePage
{
    #region "Variables"

    protected TempFileInfo tfi = null;

    #endregion


    #region "Properties"

    /// <summary>
    /// Returns false - do not allow cache.
    /// </summary>
    public override bool AllowCache
    {
        get
        {
            return false;
        }
        set
        {
        }
    }

    #endregion


    protected void Page_Load(object sender, EventArgs e)
    {
        DebugHelper.SetContext("GetImageVersion");
        
        // Get the parameters
        Guid editorGuid = QueryHelper.GetGuid("editorguid", Guid.Empty);
        int num = QueryHelper.GetInteger("versionnumber", -1);

        // Load the temp file info
        if (num >= 0)
        {
            tfi = TempFileInfoProvider.GetTempFileInfo(editorGuid, num);
        }
        else
        {
            DataSet ds = TempFileInfoProvider.GetTempFiles(null, "FileNumber DESC", 1, null);
            if (!DataHelper.DataSourceIsEmpty(ds))
            {
                tfi = new TempFileInfo(ds.Tables[0].Rows[0]);
            }
        }

        // Send the data
        SendFile();

        DebugHelper.ReleaseContext();
    }


    /// <summary>
    /// Sends the given file within response.
    /// </summary>
    /// <param name="file">File to send</param>
    protected void SendFile()
    {
        // Clear response.
        CookieHelper.ClearResponseCookies();
        Response.Clear();

        Response.Cache.SetRevalidation(HttpCacheRevalidation.AllCaches);

        if (tfi != null)
        {
            // Prepare etag
            string etag = "\"" + tfi.FileID + "\"";

            // Setup the mime type - Fix the special types
            string mimetype = tfi.FileMimeType;
            string extension = tfi.FileExtension;
            switch (extension.ToLowerCSafe())
            {
                case ".flv":
                    mimetype = "video/x-flv";
                    break;
            }

            // Prepare response
            Response.ContentType = mimetype;
            SetDisposition(tfi.FileNumber.ToString(), extension);

            // Setup Etag property
            ETag = etag;

            // Set if resumable downloads should be supported
            AcceptRange = !IsExtensionExcludedFromRanges(extension);

            // Add the file data
            tfi.Generalized.EnsureBinaryData();
            WriteBytes(tfi.FileBinary);
        }
        else
        {
            NotFound();
        }

        CompleteRequest();
    }
}