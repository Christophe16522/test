using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.CMSHelper;
using CMS.GlobalHelper;
using CMS.SettingsProvider;
using CMS.UIControls;
using CMS.ExtendedControls;
using CMS.Membership;
using CMS.DataEngine;
using CMS.Helpers;

[RegisterTitle("general.settings")]
[SaveAction(0)]
public partial class CMSModules_Content_CMSDesk_OnlineMarketing_Settings_Default : CMSAnalyticsContentPage
{
    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        DocumentManager.OnSaveData += DocumentManager_OnSaveData;
        DocumentManager.LocalDocumentPanel = pnlDoc;

        // Non-versioned data are modified
        DocumentManager.UseDocumentHelper = false;
        DocumentManager.HandleWorkflow = false;
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        // Check UI Analytics.Settings
        CurrentUserInfo ui = MembershipContext.AuthenticatedUser;
        if (!ui.IsAuthorizedPerUIElement("CMS.Content", "Analytics.Settings"))
        {
            RedirectToCMSDeskUIElementAccessDenied("CMS.Content", "Analytics.Settings");
        }
        EditedObject = Node;

        // Set disabled module info
        ucDisabledModule.SettingsKeys = "CMSAnalyticsEnabled;";
        ucDisabledModule.InfoText = GetString("WebAnalytics.Disabled");
        ucDisabledModule.ParentPanel = pnlDisabled;

        ucConversionSelector.SelectionMode = SelectionModeEnum.SingleTextBox;
        ucConversionSelector.IsLiveSite = false;

        UIContext.AnalyticsTab = AnalyticsTabEnum.Settings;

        // Check modify permissions
        if (!CheckDocumentPermissions(Node, PermissionsEnum.Modify))
        {
            DocumentManager.DocumentInfo = String.Format(GetString("cmsdesk.notauthorizedtoeditdocument"), Node.NodeAliasPath);

            // Disable save button
            CurrentMaster.HeaderActions.Enabled = false;
            usSelectCampaign.Enabled = false;
        }

        if ((Node != null) && !URLHelper.IsPostback())
        {
            ReloadData();
        }
    }


    /// <summary>
    /// Reload data from node to controls.
    /// </summary>
    private void ReloadData()
    {
        usSelectCampaign.Value = Node.DocumentCampaign;
        ucConversionSelector.Value = Node.DocumentTrackConversionName;
        txtConversionValue.Value = Node.DocumentConversionValue;
    }


    protected void DocumentManager_OnSaveData(object sender, DocumentManagerEventArgs e)
    {
        if (Node != null)
        {
            string conversionName = ValidationHelper.GetString(ucConversionSelector.Value, String.Empty).Trim();

            if (!ucConversionSelector.IsValid())
            {
                e.ErrorMessage = ucConversionSelector.ValidationError;
                e.IsValid = false;
                return;
            }

            if (!usSelectCampaign.IsValid())
            {
                e.ErrorMessage = usSelectCampaign.ValidationError;
                e.IsValid = false;
                return;
            }

            if (!txtConversionValue.IsValid())
            {
                e.ErrorMessage = GetString("conversionvalue.error");
                e.IsValid = false;
                return;
            }

            Node.DocumentCampaign = ValidationHelper.GetString(usSelectCampaign.Value, String.Empty).Trim();
            Node.DocumentConversionValue = txtConversionValue.Value.ToString();
            Node.DocumentTrackConversionName = conversionName;
        }
    }
}