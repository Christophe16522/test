using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.Blogs;
using CMS.CMSHelper;
using CMS.GlobalHelper;
using CMS.OutputFilter;
using CMS.PortalEngine;
using CMS.SiteProvider;
using CMS.DocumentEngine;

using TreeNode = CMS.DocumentEngine.TreeNode;
using CMS.Helpers;
using CMS.Membership;
using CMS.MacroEngine;
using CMS.Taxonomy;

/// <summary>
/// Blog functions.
/// </summary>
public class BlogFunctions
{
    /// <summary>
    /// Returns user name.
    /// </summary>
    /// <param name="userId">User id</param>
    public static string GetUserName(object userId)
    {
        int id = ValidationHelper.GetInteger(userId, 0);

        if (id > 0)
        {
            string key = "blogPostUserName" + id.ToString();

            // Most of the post will be from the same user, store fullname to the request to minimize the DB access
            if (RequestStockHelper.Contains(key))
            {
                return ValidationHelper.GetString(RequestStockHelper.GetItem(key), "");
            }
            else
            {
                DataSet ds = CMS.Membership.UserInfoProvider.GetUsers();
               // DataSet ds = UserInfoProvider.GetUsers("UserID = " + id, null, 1, "UserName");
               // CMS.Membership.UserInfoProvider.GetUsers("UserID = " + id, null, 1, "UserName");
                if (!DataHelper.DataSourceIsEmpty(ds))
                {
                    string result = HTMLHelper.HTMLEncode(UserInfoProvider.TrimSitePrefix(ValidationHelper.GetString(ds.Tables[0].Rows[0]["UserName"], "")));
                    RequestStockHelper.Add(key, result);
                    
                    return result;
                }
            }
        }
        return "";
    }


    /// <summary>
    /// Returns user full name.
    /// </summary>
    /// <param name="userId">User id</param>
    public static string GetUserFullName(object userId)
    {
        int id = ValidationHelper.GetInteger(userId, 0);

        if (id > 0)
        {
            string key = "TransfUserFullName_" + id;

            // Most of the post will be from the same user, store fullname to the request to minimize the DB access
            if (RequestStockHelper.Contains(key))
            {
                return ValidationHelper.GetString(RequestStockHelper.GetItem(key), "");
            }
            else
            {
                DataSet ds = CMS.Membership.UserInfoProvider.GetUsers();
             
                //DataSet ds = UserInfoProvider.GetUsers("UserID = " + id, null, 1, "FullName");
                if (!DataHelper.DataSourceIsEmpty(ds))
                {
                    string result = HTMLHelper.HTMLEncode(ValidationHelper.GetString(ds.Tables[0].Rows[0]["FullName"], ""));
                    RequestStockHelper.Add(key, result);

                    return result;
                }
            }
        }
        return "";
    }


    /// <summary>
    /// Returns number of comments of given blog.
    /// </summary>
    /// <param name="postId">Post document id</param>
    /// <param name="postAliasPath">Post alias path</param>
    public static int GetBlogCommentsCount(object postId, object postAliasPath)
    {
        return GetBlogCommentsCount(postId, postAliasPath, true);
    }


    /// <summary>
    /// Returns number of comments of given blog.
    /// </summary>
    /// <param name="postId">Post document id</param>
    /// <param name="postAliasPath">Post alias path</param>
    /// <param name="includingTrackbacks">Indicates if trackback comments should be included</param>
    public static int GetBlogCommentsCount(object postId, object postAliasPath, bool includingTrackbacks)
    {
        int docId = ValidationHelper.GetInteger(postId, 0);
        string aliasPath = ValidationHelper.GetString(postAliasPath, "");
        CurrentUserInfo currentUser = MembershipContext.AuthenticatedUser;

        // There has to be the current site
        if (SiteContext.CurrentSite == null)
        {
            throw new Exception("[BlogFunctions.GetBlogCommentsCount]: There is no current site!");
        }

        bool isOwner = false;

        // Is user authorized to manage comments?
        bool selectOnlyPublished = (PortalContext.ViewMode == ViewModeEnum.LiveSite);
        TreeNode blogNode = BlogHelper.GetParentBlog(aliasPath, SiteContext.CurrentSiteName, selectOnlyPublished);
        if (blogNode != null)
        {
            isOwner = (currentUser.UserID == ValidationHelper.GetInteger(blogNode.GetValue("NodeOwner"), 0));
        }

        bool isUserAuthorized = (currentUser.IsAuthorizedPerResource("cms.blog", "Manage") || isOwner || BlogHelper.IsUserBlogModerator(currentUser.UserName, blogNode));

        // Get post comments
        return BlogCommentInfoProvider.GetPostCommentsCount(docId, !isUserAuthorized, isUserAuthorized, includingTrackbacks);
    }


    /// <summary>
    /// Gets a list of links of tags assigned for the specific document pointing to the page with URL specified
    /// </summary>
    /// <param name="documentGroupId">ID of the group document tags belong to</param>
    /// <param name="documentTags">String containing all the tags related to the document</param>
    /// <param name="documentListPage">URL of the page displaying other documents of the specified tag</param>
    public static string GetDocumentTags(object documentGroupId, object documentTags, string documentListPage)
    {
        return GetDocumentTags(documentGroupId, documentTags, null, documentListPage);
    }


    /// <summary>
    /// Gets a list of links of tags assigned for the specific document pointing to the page with URL specified.
    /// </summary>
    /// <param name="documentGroupId">ID of the group document tags belong to</param>
    /// <param name="documentTags">String containing all the tags related to the document</param>
    /// <param name="nodeAliasPath">Node alias path</param>
    /// <param name="documentListPage">Path or URL of the page displaying other documents of the specified tag</param>
    public static string GetDocumentTags(object documentGroupId, object documentTags, object nodeAliasPath, string documentListPage)
    {
        var groupId = ValidationHelper.GetInteger(documentGroupId, 0);
        var tags = ValidationHelper.GetString(documentTags, null);
        var path = ValidationHelper.GetString(nodeAliasPath, null);
        var listPage = ValidationHelper.GetString(documentListPage, null);

        if ((tags == null) || (tags.Trim() == string.Empty))
        {
            return string.Empty;
        }

        // If list page was specified make a list of links, otherwise return just list of tags
        bool renderLink = !string.IsNullOrEmpty(listPage);
        if (renderLink)
        {
            // Get list page URL
            listPage = ValidationHelper.IsURL(listPage) ? URLHelper.ResolveUrl(listPage) : MacroContext.CurrentResolver.ResolvePath(listPage);

            // Look for group ID of document parent if not supplied
            if (groupId == 0)
            {
                var culture = MembershipContext.AuthenticatedUser.PreferredCultureCode;
                var currentPageInfo = DocumentContext.CurrentPageInfo;

                // Get context data
                if (path == null)
                {
                    path = currentPageInfo.NodeAliasPath;
                    culture = currentPageInfo.DocumentCulture;
                }
                groupId = PageInfoProvider.GetParentProperty<int>(currentPageInfo.NodeSiteID, path, "DocumentTagGroupID", culture, "DocumentTagGroupID IS NOT NULL");
            }
        }

        // Get sorted list of document tags
        var tagList = TagHelper.GetTags(tags);
        var list = tagList.Values.Cast<string>().ToList();
        list = list.Select(t => t.Replace("\"", "").Trim()).ToList();
        list.Sort();

        var result = list.Select(t =>
        {
            var encodedTag = HTMLHelper.HTMLEncode(t);

            if (renderLink)
            {
                return "<a href=\"" + listPage + "?tagname=" + HttpUtility.UrlEncode(t) + "&amp;groupid=" + groupId + "\">" + encodedTag + "</a>";
            }

            return encodedTag;
        });

        return result.Join(", ");
    }
}