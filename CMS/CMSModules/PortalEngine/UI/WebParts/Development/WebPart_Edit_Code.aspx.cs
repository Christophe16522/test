using System;
using System.Data;
using System.Collections;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using CMS.FormEngine;
using CMS.Helpers;
using CMS.IO;
using CMS.PortalEngine;
using CMS.Base;
using CMS.UIControls;
using CMS.ExtendedControls;
using CMS.ExtendedControls.ActionsConfig;

public partial class CMSModules_PortalEngine_UI_WebParts_Development_WebPart_Edit_Code : GlobalAdminPage
{
    private int webpartId = 0;


    protected void Page_Load(object sender, EventArgs e)
    {
        webpartId = QueryHelper.GetInteger("webpartid", 0);
        EditedObject = WebPartInfoProvider.GetWebPartInfo(webpartId);
        GenerateCode();

        // Ensure header action
        HeaderAction generate = new HeaderAction();
        generate.Text = GetString("WebPartCode.Generate");
        generate.Tooltip = generate.Text;
        generate.CommandName = "generate";

        HeaderActions.AddAction(generate);

        HeaderActions.ActionPerformed += HeaderActions_ActionPerformed;
    }

    void HeaderActions_ActionPerformed(object sender, CommandEventArgs e)
    {
        if (e.CommandName == "generate")
        {
            GenerateCode();
        }
    }


    /// <summary>
    /// Generates the web part code.
    /// </summary>
    protected void GenerateCode()
    {
        string ascx = null;
        string code = null;

        // Generate the code
        WebPartInfoProvider.GenerateWebPartCode(webpartId, txtBaseControl.Text.Trim(), out ascx, out code);

        txtASCX.Text = ascx;
        txtCS.Text = code;
    }
}