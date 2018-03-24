using System;

using CMS;
using CMS.Base;
using CMS.ExtendedControls;
using CMS.Forums;
using CMS.Helpers;
using CMS.UIControls;

[assembly: RegisterCustomClass("ForumUniGridExtender", typeof(ForumUniGridExtender))]

/// <summary>
/// Provides proper delete method for ForumListing
/// </summary>
public class ForumUniGridExtender : ControlExtender<UniGrid>
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
                ForumInfoProvider.DeleteForumInfo(ValidationHelper.GetInteger(actionArgument, 0));
                break;

            case "up":
                ForumInfoProvider.MoveForumUp(ValidationHelper.GetInteger(actionArgument, 0));
                break;

            case "down":
                ForumInfoProvider.MoveForumDown(ValidationHelper.GetInteger(actionArgument, 0));
                break;
        }
    }
}