using System;

using CMS;
using CMS.Base;
using CMS.ExtendedControls;
using CMS.Forums;
using CMS.Helpers;
using CMS.UIControls;

[assembly: RegisterCustomClass("ForumGroupUniGridExtender", typeof(ForumGroupUniGridExtender))]

/// <summary>
/// Provides proper delete method for ForumGroupListing
/// </summary>
public class ForumGroupUniGridExtender : ControlExtender<UniGrid>
{
    /// <summary>
    /// Initializes the extender.
    /// </summary>
    public override void OnInit()
    {
        this.Control.OnAction += Control_OnAction;
    }


    private void Control_OnAction(string actionName, object actionArgument)
    {
        switch (actionName.ToLowerCSafe())
        {
            case "delete":
                ForumGroupInfoProvider.DeleteForumGroupInfo(ValidationHelper.GetInteger(actionArgument, 0));
                break;

            case "up":
                ForumGroupInfoProvider.MoveGroupUp(ValidationHelper.GetInteger(actionArgument, 0));
                break;

            case "down":
                ForumGroupInfoProvider.MoveGroupDown(ValidationHelper.GetInteger(actionArgument, 0));
                break;
        }
    }
}