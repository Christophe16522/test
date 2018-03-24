using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.UIControls;
using CMS.IO;
using CMS.AmazonStorage;
using CMS.Helpers;
using CMS.PortalEngine;
using CMS.Base;

public partial class CMSPages_GetAmazonFile : GetFilePage
{
    #region "Properties"

    /// <summary>
    /// Returns IS3ObjectInfoProvider instance.
    /// </summary>
    IS3ObjectInfoProvider Provider
    {
        get
        {
            return S3ObjectFactory.Provider;
        }
    }


    /// <summary>
    /// Gets or sets whether cache is allowed. By default cache is allowed on live site.
    /// </summary>
    public override bool AllowCache
    {
        get
        {
            if (mAllowCache == null)
            {
                mAllowCache = ViewMode.IsLiveSite();
            }

            return mAllowCache.Value;
        }
        set
        {
            mAllowCache = value;
        }
    }

    #endregion


    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        string hash = QueryHelper.GetString("hash", string.Empty);
        string path = QueryHelper.GetString("path", string.Empty);

        // Validate hash
        if (ValidationHelper.ValidateHash("?path=" + URLHelper.EscapeSpecialCharacters(path), hash, false))
        {
            if (path.StartsWithCSafe("~"))
            {
                path = Server.MapPath(path);
            }

            // Get file content from Amazon S3
            IS3ObjectInfo obj = S3ObjectFactory.GetInfo(path);

            // Check if blob exists
            if (Provider.ObjectExists(obj))
            {
                // Clear response.
                CookieHelper.ClearResponseCookies();
                Response.Clear();

                // Set the revalidation
                SetRevalidation();

                DateTime lastModified = S3ObjectInfoProvider.GetStringDateTime(obj.GetMetadata(S3ObjectInfoProvider.LAST_WRITE_TIME));
                string etag = "\"" + lastModified.ToString() + "\"";

                // Set correct response content type
                SetResponseContentType(path);

                // Client caching - only on the live site
                if (AllowCache && AllowClientCache && ETagsMatch(etag, lastModified))
                {
                    // Set the file time stamps to allow client caching
                    SetTimeStamps(lastModified);

                    RespondNotModified(etag, true);
                    return;
                }

                Stream stream = Provider.GetObjectContent(obj);
                SetDisposition(Path.GetFileName(path), Path.GetExtension(path));

                // Setup Etag property
                ETag = etag;

                if (AllowCache)
                {
                    // Set the file time stamps to allow client caching
                    SetTimeStamps(lastModified);
                    Response.Cache.SetETag(etag);
                }
                else
                {
                    SetCacheability();
                }

                // Send headers
                Response.Flush();

                Byte[] buffer = new Byte[StorageHelper.BUFFER_SIZE];
                int bytesRead = stream.Read(buffer, 0, StorageHelper.BUFFER_SIZE);

                // Copy data from blob stream to cache
                while (bytesRead > 0)
                {
                    // Write the data to the current output stream
                    Response.OutputStream.Write(buffer, 0, bytesRead);

                    // Flush the data to the output
                    Response.Flush();

                    // Read next part of data
                    bytesRead = stream.Read(buffer, 0, StorageHelper.BUFFER_SIZE);
                }

                stream.Close();

                CompleteRequest();
            }
            else
            {
                NotFound();
            }
        }
        else
        {
            URLHelper.Redirect(ResolveUrl("~/CMSMessages/Error.aspx?title=" + ResHelper.GetString("general.badhashtitle") + "&text=" + ResHelper.GetString("general.badhashtext")));
        }
    }


    /// <summary>
    /// Sets content type of the response based on file MIME type
    /// </summary>
    /// <param name="filePath">File path</param>
    private void SetResponseContentType(string filePath)
    {
        string extension = Path.GetExtension(filePath);
        string mimeType = MimeTypeHelper.GetMimetype(extension);
        switch (extension.ToLowerCSafe())
        {
            case ".flv":
                // Correct MIME type
                mimeType = "video/x-flv";
                break;
        }

        // Set content type
        Response.ContentType = mimeType;
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Sets the last modified and expires header to the response
    /// </summary>
    /// <param name="lastModified">Last modified date time.</param>
    private void SetTimeStamps(DateTime lastModified)
    {
        DateTime expires = DateTime.Now;

        // Send last modified header to allow client caching
        Response.Cache.SetLastModified(lastModified);

        Response.Cache.SetCacheability(HttpCacheability.Public);
        if (AllowClientCache)
        {
            expires = DateTime.Now.AddMinutes(ClientCacheMinutes);
        }

        Response.Cache.SetExpires(expires);
    }

    #endregion
}
