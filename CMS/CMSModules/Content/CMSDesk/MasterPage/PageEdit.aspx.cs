using System;

using CMS.Helpers;
using CMS.UIControls;
using CMS.Membership;

public partial class CMSModules_Content_CMSDesk_MasterPage_PageEdit : CMSContentPage
{
    #region "Variables"

    private CurrentUserInfo user = null;

    #endregion


    #region "Methods"

    protected override void CreateChildControls()
    {
        // Enable split mode
        EnableSplitMode = true;

        user = MembershipContext.AuthenticatedUser;
        if (Node != null)
        {
            UIContext.EditedObject = Node;
            ucHierarchy.PreviewObjectName = Node.NodeAliasPath;
            ucHierarchy.AddContentParameter(new UILayoutValue("PreviewObject", Node));
            ucHierarchy.DefaultAliasPath = Node.NodeAliasPath;
            ucHierarchy.IgnoreSessionValues = true;
        }

        base.CreateChildControls();
    }

    protected override void OnInit(EventArgs e)
    {
        // Check UIProfile
        if (!user.IsAuthorizedPerUIElement("CMS.Design", "MasterPage"))
        {
            RedirectToUIElementAccessDenied("CMS.Design", "MasterPage");
        }

        // Check "Design" permission
        if (!user.IsAuthorizedPerResource("CMS.Design", "Design"))
        {
            RedirectToAccessDenied("CMS.Design", "Design");
        }

        ucHierarchy.RegisterEnvelopeClientID();
        base.OnInit(e);

        UIHelper.SetBreadcrumbsSuffix(GetString("general.document"));
    }

    #endregion
}