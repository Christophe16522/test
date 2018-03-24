using System;

using CMS.Base;
using CMS.Core;
using CMS.Helpers;
using CMS.Localization;
using CMS.UIControls;


[Breadcrumbs()]
[Breadcrumb(0, "culture.strings", "~/CMSModules/SystemDevelopment/Development/Resources/UICulture_StringsDefault_List.aspx?cultureId={?cultureid?}", null)]
[Breadcrumb(1, ResourceString = "culture.newstring")]
[Title(ResourceString = "culture.newstring", HelpTopic = "newedit_string")]
[UIElement(ModuleName.CMS, "Development.Resources")]
public partial class CMSModules_SystemDevelopment_Development_Resources_UICulture_StringsDefault_New : GlobalAdminPage
{
    protected int cultureId = 0;


    protected void Page_Load(object sender, EventArgs e)
    {
        // Get UIculture ID from query string
        cultureId = QueryHelper.GetInteger("cultureId", 0);

        // Initialize controls
        rfvKey.ErrorMessage = GetString("culture.enterakey");
    }


    protected void btnOK_Click(object sender, EventArgs e)
    {
        string key = txtKey.Text.Trim().ToLowerCSafe();
        string result = new Validator().NotEmpty(key, rfvKey.ErrorMessage).IsCodeName(key, GetString("culture.InvalidCodeName")).Result;

        if (String.IsNullOrEmpty(result))
        {
            CultureInfo uic = CultureInfoProvider.GetCultureInfo(cultureId);

            if (uic != null)
            {
                string cultureCode = uic.CultureCode;

                FileResourceManager frm = LocalizationHelper.GetFileManager(cultureCode);
                FileResourceEditor resourceEditor = new FileResourceEditor(Server.MapPath(FileResourceManager.GetResFilename(cultureCode)), cultureCode);
                
                try
                {
                    frm.SetString(key, txtText.Text);
                    resourceEditor.SetResourceString(key, txtText.Text, cultureCode);
                }
                catch (Exception ex)
                {
                    ShowError(GetString("general.saveerror"), ex.Message, null);
                    return;
                }

                URLHelper.Redirect("UICulture_StringsDefault_Edit.aspx?cultureID=" + cultureId + "&stringCodeName=" + key + "&saved=1");
            }
            else
            {
                ShowError(GetString("general.invalidcultureid"));
            }
        }
        else
        {
            ShowError(result);
        }
    }
}