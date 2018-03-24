using System;

using CMS.Base;
using CMS.Core;
using CMS.ExtendedControls.ActionsConfig;
using CMS.Helpers;
using CMS.Localization;
using CMS.UIControls;


[Breadcrumbs()]
[Breadcrumb(0, "culture.strings", "~/CMSModules/SystemDevelopment/Development/Resources/UICulture_StringsDefault_List.aspx?cultureId={?cultureid?}", null)]
[Title(ResourceString = "Development-UICulture_Strings_List.EditRes", HelpTopic = "newedit_string")]
[UIElement(ModuleName.CMS, "Development.Resources")]
public partial class CMSModules_SystemDevelopment_Development_Resources_UICulture_StringsDefault_Edit : GlobalAdminPage
{
    private string stringCodeName;
    private int cultureId;
    private BreadcrumbItem codeNameBreadcrumbItem;

    protected int BackCount
    {
        get
        {
            return ValidationHelper.GetInteger(ViewState["BackCount"], 1);
        }
        set
        {
            ViewState["BackCount"] = value;
        }
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        stringCodeName = QueryHelper.GetString("stringCodeName", String.Empty);
        cultureId = QueryHelper.GetInteger("cultureId", 0);

        // Validate culture ID
        if (cultureId <= 0)
        {
            ShowError(GetString("general.invalidcultureid"));
            return;
        }

        if (QueryHelper.GetBoolean("saved", false))
        {
            ShowChangesSaved();
        }

        // Init new header action
        HeaderAction action = new HeaderAction
        {
            Text = GetString("culture.newstring"),
            RedirectUrl = "~/CMSModules/SystemDevelopment/Development/Resources/UICulture_StringsDefault_New.aspx?cultureId=" + cultureId,
        };
        CurrentMaster.HeaderActions.ActionsList.Add(action);

        codeNameBreadcrumbItem = new BreadcrumbItem
        {
            Text = stringCodeName.ToLowerCSafe(),
        };
        PageBreadcrumbs.AddBreadcrumb(codeNameBreadcrumbItem);

        // Ensure breadcrumbs suffix
        UIHelper.SetBreadcrumbsSuffix(GetString("objecttype.cms_resourcestring"));

        // Initialize controls
        rfvKey.ErrorMessage = GetString("culture.enterakey");

        if (!RequestHelper.IsPostBack())
        {
            CultureInfo uic = CultureInfoProvider.GetCultureInfo(cultureId);

            if (uic != null)
            {
                string cultureCode = uic.CultureCode;
                FileResourceEditor fre = new FileResourceEditor(Server.MapPath(FileResourceManager.GetResFilename(cultureCode)), cultureCode);
                if (fre != null)
                {
                    txtKey.Text = stringCodeName;
                    txtText.Text = fre.GetResourceString(stringCodeName, cultureCode);
                }
            }
            else
            {
                ShowError(GetString("general.invalidcultureid"));
            }
        }
    }


    protected void btnOK_Click(object sender, EventArgs e)
    {
        // History back count
        BackCount++;

        string newKey = txtKey.Text.Trim().ToLowerCSafe();
        string result = new Validator().NotEmpty(newKey, rfvKey.ErrorMessage).IsCodeName(newKey, GetString("culture.InvalidCodeName")).Result;

        if (String.IsNullOrEmpty(result))
        {
            CultureInfo uic = CultureInfoProvider.GetCultureInfo(cultureId);

            if (uic != null)
            {
                string cultureCode = uic.CultureCode;
                stringCodeName = stringCodeName.ToLowerCSafe();

                FileResourceManager frm = LocalizationHelper.GetFileManager(cultureCode);
                FileResourceEditor resourceEditor = new FileResourceEditor(Server.MapPath(FileResourceManager.GetResFilename(cultureCode)), cultureCode);

                try
                {
                    if ((frm != null) && (resourceEditor != null))
                    {
                        if (!stringCodeName.EqualsCSafe(newKey, StringComparison.InvariantCultureIgnoreCase))
                        {
                            frm.DeleteString(stringCodeName);
                            resourceEditor.DeleteResourceString(stringCodeName, cultureCode, true);
                        }

                        frm.SetString(newKey, txtText.Text);
                        resourceEditor.SetResourceString(newKey, txtText.Text, cultureCode);
                    }
                }
                catch (Exception ex)
                {
                    ShowError(GetString("general.saveerror"), ex.Message, null);
                    return;
                }

                ShowChangesSaved();

                codeNameBreadcrumbItem.Text = newKey;
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