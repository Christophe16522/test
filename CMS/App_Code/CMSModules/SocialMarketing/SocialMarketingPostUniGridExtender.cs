using System;
using System.Linq;

using CMS;
using CMS.DataEngine;
using CMS.LicenseProvider;
using CMS.Membership;
using CMS.Base;
using CMS.EventLog;
using CMS.ExtendedControls;
using CMS.Helpers;
using CMS.SiteProvider;
using CMS.SocialMarketing;
using CMS.UIControls;
using CMS.PortalEngine;

using TreeNode = CMS.DocumentEngine.TreeNode;

[assembly: RegisterCustomClass("SocialMarketingPostUniGridExtender", typeof(SocialMarketingPostUniGridExtender))]

/// <summary>
/// Extends Unigrids used for posts from Social marketing module with additional abilities.
/// </summary>
public class SocialMarketingPostUniGridExtender : ControlExtender<UniGrid>
{

    #region "Private variables"

    private readonly string mFacebookPostDetailsUrlFormat = URLHelper.GetAbsoluteUrl("~/CMSModules/SocialMarketing/Pages/FacebookPostDetailDialog.aspx") + "?postid={0}";

    private readonly string mPostDocumentUrlFormat = URLHelper.GetAbsoluteUrl("~/Admin/cmsadministration.aspx?action=edit&mode=editform") + "&nodeId={0}&culture={1}" + UIContextHelper.GetApplicationHash("cms.content", "content");

    #endregion


    #region "life-cycle methods and event handlers"

    /// <summary>
    /// OnInit page event.
    /// </summary>
    public override void OnInit()
    {
        Control.OnAction += Control_OnAction;
        Control.OnExternalDataBound += Control_OnExternalDataBound;

        ScriptHelper.RegisterDialogScript(Control.Page);
    }


    /// <summary>
    /// Control OnExternalDataBound event.
    /// </summary>
    private object Control_OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        switch (sourceName)
        {
            case "documentguid":
            {
                Guid documentGuid = ValidationHelper.GetGuid(parameter, Guid.Empty);
                if (documentGuid != Guid.Empty)
                {
                    TreeNode document = new ObjectQuery<TreeNode>().WithGuid(documentGuid);
                    if (document == null)
                    {
                        return ResHelper.GetString("sm.posts.msg.documentnotavailable");
                    }
                    string linkUrl = String.Format(mPostDocumentUrlFormat, document.NodeID, document.DocumentCulture);

                    return String.Format("<a href=\"{0}\" title=\"{1}\" target=\"_blank\">{2}</a>", 
                        linkUrl, 
                        String.Format(ResHelper.GetString("sm.posts.document.tooltip"), HTMLHelper.HTMLEncode(document.NodeAliasPath)), 
                        HTMLHelper.HTMLEncode(document.DocumentName));
                }
                break;
            }

            case "state":
                return GetPostState(ValidationHelper.GetInteger(parameter, 0));

            case "facebookpostdetail":
                return GetFacebookPostDetailLink(ValidationHelper.GetInteger(parameter, 0));
        }

        return null;
    }


    /// <summary>
    /// Gets a link control that opens post details dialog when clicked. Post's content is used as link text.
    /// </summary>
    /// <param name="postId">Facebook post identifier.</param>
    private object GetFacebookPostDetailLink(int postId)
    {
        FacebookPostInfo post = FacebookPostInfoProvider.GetFacebookPostInfo(postId);
        if (LicenseKeyInfoProvider.IsFeatureAvailable(FeatureEnum.SocialMarketingInsights))
        {
            string dialogUrl = String.Format(mFacebookPostDetailsUrlFormat, post.FacebookPostID);
            return String.Format("<a href=\"{0}\" onclick=\"modalDialog('{0}', 'PostDetails', 480, 570); return false;\" title=\"{1}\">{2}</a>",
                dialogUrl,
                ResHelper.GetString("sm.facebook.posts.detail.tooltip"),
                HTMLHelper.HTMLEncode(TextHelper.LimitLength(post.FacebookPostText, 80))
                );
        }
        return HTMLHelper.HTMLEncode(TextHelper.LimitLength(post.FacebookPostText, 80));
    }


    /// <summary>
    /// Control OnAction event.
    /// </summary>
    private void Control_OnAction(string actionName, object actionArgument)
    {
        switch (Control.ObjectType)
        {
            // Facebook post
            case FacebookPostInfo.OBJECT_TYPE:
                OnAction_Facebook(actionName, actionArgument);
                break;

            // Twitter post
            case TwitterPostInfo.OBJECT_TYPE:
                OnAction_Twitter(actionName, actionArgument);
                break;
        }
    }

    #endregion


    #region "Private methods"

    /// <summary>
    /// OnAction for Facebook post object type.
    /// </summary>
    private void OnAction_Facebook(string actionName, object actionArgument)
    {
        switch (actionName)
        {
            case "delete":    
                try
                {
                    FacebookPostInfoProvider.DeleteFacebookPostInfo(ValidationHelper.GetInteger(actionArgument, 0));
                }
                catch (Exception ex)
                {
                    EventLogProvider.LogWarning("Social marketing - Facebook post", "DELETEPOST", ex, SiteContext.CurrentSiteID, "Facebook post could not be deleted.");
                    Control.ShowError(Control.GetString("sm.facebook.posts.msg.deleteerror"));
                }
                break;
        }
    }


    /// <summary>
    /// OnAction for Twitter post object type.
    /// </summary>
    /// <param name="actionName"></param>
    /// <param name="actionArgument">Integer ID as a string.</param>
    private void OnAction_Twitter(string actionName, object actionArgument)
    {
        switch (actionName)
        {
            case "delete":
                try
                {
                    TwitterPostInfoProvider.DeleteTwitterPostInfo(ValidationHelper.GetInteger(actionArgument, 0));
                }
                catch (Exception ex)
                {
                    EventLogProvider.LogWarning("Social marketing - Twitter post", "DELETEPOST", ex, SiteContext.CurrentSiteID, "Twitter post could not be deleted.");
                    Control.ShowError(Control.GetString("sm.twitter.posts.msg.deleteerror"));
                }
                break;
        }
    }


    /// <summary>
    /// Gets localized message describing post state.
    /// </summary>
    /// <param name="postId">Post id.</param>
    private object GetPostState(int postId)
    {
        switch (Control.ObjectType)
        {
            // Facebook post
            case FacebookPostInfo.OBJECT_TYPE:
                FacebookPostInfo facebookPost = FacebookPostInfoProvider.GetFacebookPostInfo(postId);
                return FacebookPostInfoProvider.GetPostPublishStateMessage(facebookPost, MembershipContext.AuthenticatedUser, SiteContext.CurrentSite, true);

            // Twitter post
            case TwitterPostInfo.OBJECT_TYPE:
                TwitterPostInfo twitterPost = TwitterPostInfoProvider.GetTwitterPostInfo(postId);
                return TwitterPostInfoProvider.GetPostPublishStateMessage(twitterPost, MembershipContext.AuthenticatedUser, SiteContext.CurrentSite, true);
        }

        return String.Empty;
    }

    #endregion
}