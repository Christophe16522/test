﻿using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;

using CMS.CustomTables;
using CMS.ExtendedControls;
using CMS.FormControls;
using CMS.IO;
using CMS.MacroEngine;
using CMS.PortalEngine;
using CMS.Membership;
using CMS.UIControls;
using CMS.DataEngine;
using CMS.Base;
using CMS.Helpers;

public partial class CMSFormControls_Layouts_TransformationCode : FormEngineUserControl, IPostBackEventHandler
{
    #region "Constants"

    /// <summary>
    /// Short link to help page regarding transformation methods.
    /// </summary>
    private const string HELP_TOPIC_TRANSFORMATION_METHODS_LINK = "6YFDAg";

    #endregion


    #region "Variables"

    int transformationID = QueryHelper.GetInteger("transformationid", 0);
    TransformationInfo ti = null;
    CurrentUserInfo user;

    #endregion


    #region "Properties"

    /// <summary>
    /// Value of code control.
    /// </summary>
    public override object Value
    {
        get
        {
            return TransformationCode;
        }
        set
        {
            if (TransformationType == TransformationTypeEnum.Html)
            {
                tbWysiwyg.ResolvedValue = ValidationHelper.GetString(value, String.Empty);
            }
            else
            {
                txtCode.Text = ValidationHelper.GetString(value, String.Empty);
            }
        }
    }


    /// <summary>
    /// Name of the edited transformation
    /// </summary>
    public String TransformationName
    {
        get;
        set;
    }


    /// <summary>
    /// Transformation's class name
    /// </summary>
    public String ClassName
    {
        get;
        set;
    }


    /// <summary>
    /// Transformation's class ID
    /// </summary>
    public int ClassID
    {
        get;
        set;
    }


    /// <summary>
    /// Property returning transformation code (based on transformation type)
    /// </summary>
    public String TransformationCode
    {
        get
        {
            return (txtCode.Visible) ? txtCode.Text : tbWysiwyg.ResolvedValue;
        }
    }


    /// <summary>
    /// Returns true, if transformation type is ASCX
    /// </summary>
    public bool IsAscx
    {
        get
        {
            return (TransformationType == TransformationTypeEnum.Ascx);
        }
    }


    /// <summary>
    /// Returns transformation type
    /// </summary>
    public TransformationTypeEnum TransformationType
    {
        get
        {
            return TransformationInfoProvider.GetTransformationTypeEnum(drpType.SelectedValue.ToLowerCSafe());
        }
    }


    /// <summary>
    /// Determines whether the code is in the fullscreen mode.
    /// </summary>
    public bool FullscreenMode
    {
        get;
        set;
    }


    /// <summary>
    /// Returns true if object is checked out or use checkin/out is not used 
    /// </summary>
    public bool IsChecked
    {
        get
        {
            CMSObjectManager om = CMSObjectManager.GetCurrent(Page);
            if (om != null)
            {
                return om.IsObjectChecked();
            }

            return false;
        }
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Returns additional object data
    /// </summary>
    public override object[,] GetOtherValues()
    {
        object[,] values = new object[3, 2];
        values[0, 0] = "TransformationCode";
        values[0, 1] = TransformationCode;
        values[1, 0] = "TransformationType";

        String type = (drpType.SelectedValue == null ? TransformationTypeEnum.Ascx.ToString() : drpType.SelectedValue.ToLowerCSafe());

        // For users not authorized to change ascx, do not allow to change transf. type
        if (!MembershipContext.AuthenticatedUser.IsAuthorizedPerResource("cms.design", "editcode"))
        {
            TransformationInfo ti = UIContext.EditedObject as TransformationInfo;
            if ((ti != null) && (type == "ascx"))
            {
                type = ti.TransformationType.ToString();
            }
        }
        values[1, 1] = type;

        values[2, 0] = "TransformationCSS";
        values[2, 1] = txtCSS.Text;
        return values;
    }


    /// <summary>
    /// Checks whether XSLT transformation text is valid.
    /// </summary>
    /// <param name="xmlText">XML text</param>
    protected string XMLValidator(string xmlText)
    {
        // Creates memory stream from transformation text
        Stream stream = MemoryStream.New();
        StreamWriter writer = StreamWriter.New(stream);
        writer.Write(xmlText);
        writer.Flush();
        stream.Seek(0, SeekOrigin.Begin);

        // New xml text reader from the stream
        XmlTextReader tr = new XmlTextReader(stream.SystemStream);
        try
        {
            // Need to read the data to validate
            while (tr.Read())
            {
                ;
            }
        }
        catch (Exception ex)
        {
            return ex.Message;
        }

        return string.Empty;
    }


    /// <summary>
    /// Initializes labels.
    /// </summary>
    private void LabelsInit()
    {
        // Initializes labels        
        string lang = DataHelper.GetNotEmpty(SettingsHelper.AppSettings["CMSProgrammingLanguage"], "C#");
        lblDirectives.Text = string.Concat("&lt;%@ Control Language=\"", lang, "\" AutoEventWireup=\"true\" Inherits=\"CMS.Controls.CMSAbstractTransformation\" %&gt;<br />&lt;%@ Register TagPrefix=\"cc1\" Namespace=\"CMS.Controls\" Assembly=\"CMS.Controls\" %&gt;");
    }


    /// <summary>
    /// Returns true, if all entered values are valid
    /// </summary>
    public override bool IsValid()
    {
        String result = String.Empty;
        if (TransformationType == TransformationTypeEnum.Xslt)
        {
            // Validates XSLT transformation text
            result = XMLValidator(txtCode.Text);
            if (result != String.Empty)
            {
                ShowError(String.Format("{0}'{1}'", ScriptHelper.GetLocalizedString("DocumentType_Edit_Transformation_Edit.XSLTTransformationError"), result));
                return false;
            }
        }

        return true;
    }


    protected override void OnPreRender(EventArgs e)
    {
        string script = @"
function GenerateDefaultCode(type){
" + ControlsHelper.GetPostBackEventReference(this, "#").Replace("'#'", "type") + @"
}";
        ScriptHelper.RegisterStartupScript(this, typeof(string), "TransformationCodeGenerate", ScriptHelper.GetScript(script));

        // Init transformation help link        
        lnkHelp.NavigateUrl = UIContextHelper.GetDocumentationTopicUrl(HELP_TOPIC_TRANSFORMATION_METHODS_LINK);
        lnkHelp.Target = "_target";
        lnkHelp.Text = GetString("documenttype_edit_transformation.help");

        pnlDirectives.Visible = IsAscx;
        txtCode.ReadOnly = !Enabled;
        tbWysiwyg.Enabled = Enabled;

        lblDirectives.Visible = IsAscx;

        // Check whether virtual objects are allowed
        if (!SettingsKeyInfoProvider.VirtualObjectsAllowed)
        {
            ShowWarning(GetString("VirtualPathProvider.NotRunning"), null, null);
        }

        base.OnPreRender(e);
    }


    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        ti = UIContext.EditedObject as TransformationInfo;
        user = MembershipContext.AuthenticatedUser;

        if (!RequestHelper.IsPostBack())
        {
            DropDownListInit();

            if (ti != null)
            {
                // Fills form with transformation information
                drpType.SelectedValue = ti.TransformationType.ToString();
                txtCSS.Text = ti.TransformationCSS;

                if (ti.TransformationType == TransformationTypeEnum.Html)
                {
                    tbWysiwyg.ResolvedValue = ti.TransformationCode;
                    tbWysiwyg.Visible = true;
                }
                else
                {
                    txtCode.Text = ti.TransformationCode;
                    txtCode.Visible = true;
                }
            }
            else
            {
                txtCode.Visible = true;
            }
        }
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        CMSMasterPage currentMaster = Page.Master as CMSMasterPage;

        if (FullscreenMode)
        {
            txtCode.TopOffset = 40;
        }

        // Check master page
        if (currentMaster == null)
        {
            throw new Exception("Page using this control must have CMSMasterPage master page.");
        }

        LabelsInit();

        txtCode.Editor.Width = new Unit("99%");
        txtCode.Editor.Height = new Unit("300px");
        txtCode.NamespaceUsings = new List<string> { "Transformation" };

        // transformation.{classid}.{isascx}
        string resolverName = "transformation." + ClassID + "." + IsAscx;

        txtCode.ResolverName = resolverName;
        tbWysiwyg.ResolverName = resolverName;

        if (IsAscx)
        {
            DataClassInfo resolverClassInfo = DataClassInfoProvider.GetDataClassInfo(ClassID);
            if (resolverClassInfo != null)
            {
                if (resolverClassInfo.ClassIsCustomTable)
                {
                    txtCode.ASCXRootObject = CustomTableItem.New(resolverClassInfo.ClassName);
                }
                else if (resolverClassInfo.ClassIsDocumentType)
                {
                    txtCode.ASCXRootObject = CMS.DocumentEngine.TreeNode.New(resolverClassInfo.ClassName);
                }
                else
                {
                    txtCode.ASCXRootObject = ModuleManager.GetReadOnlyObjectByClassName(resolverClassInfo.ClassName);
                }
            }

            if (!RequestHelper.IsPostBack() && IsChecked)
            {
                ShowMessage();
            }
        }

        // Hide/Display CSS section
        plcCssLink.Visible = String.IsNullOrEmpty(txtCSS.Text.Trim());

        SetEditor();
    }




    public void GenerateDefaultTransformation(DefaultTransformationTypeEnum transformCode)
    {
        if (String.IsNullOrEmpty(ClassName))
        {
            ClassName = DataClassInfoProvider.GetClassName(ClassID);
        }

        // Gets Xml schema of the document type
        DataClassInfo dci = DataClassInfoProvider.GetDataClassInfo(ClassName);
        string formDef = string.Empty;
        if (dci != null)
        {
            formDef = dci.ClassFormDefinition;
        }

        // Gets transformation type
        TransformationTypeEnum transformType = TransformationInfoProvider.GetTransformationTypeEnum(drpType.SelectedValue);

        // Writes the result to the text box
        if (transformType == TransformationTypeEnum.Html)
        {
            txtCode.Visible = false;
            tbWysiwyg.Visible = true;
            tbWysiwyg.ResolvedValue = TransformationInfoProvider.GenerateTransformationCode(formDef, transformType, ClassName, transformCode);
        }
        else
        {
            tbWysiwyg.Visible = false;
            txtCode.Visible = true;
            txtCode.Text = TransformationInfoProvider.GenerateTransformationCode(formDef, transformType, ClassName, transformCode);
        }
    }


    /// <summary>
    /// Sets code editor based on transformation type
    /// </summary>
    private void SetEditor()
    {
        if (IsAscx)
        {
            txtCode.Editor.Language = LanguageEnum.ASPNET;

            // Check the edit code permission
            if (!user.IsAuthorizedPerResource("CMS.Design", "EditCode"))
            {
                txtCode.Editor.Enabled = false;
                lblDirectives.Visible = false;
            }
        }
        else
        {
            txtCode.Editor.Enabled = true;
            txtCode.Editor.Language = LanguageEnum.HTMLMixed;
        }
    }


    /// <summary>
    /// Display info message
    /// </summary>
    public void ShowMessage()
    {
        if (IsAscx)
        {
            // Check the edit code permission
            if (!user.IsAuthorizedPerResource("CMS.Design", "EditCode"))
            {
                ShowWarning(GetString("EditCode.NotAllowed"), null, null);
            }
        }
    }


    protected void drpTransformationType_SelectedIndexChanged(object sender, EventArgs e)
    {
        // Get the current code
        string code = TransformationCode;

        switch (drpType.SelectedValue.ToLowerCSafe())
        {
            case "ascx":
                // Convert to ASCX syntax
                if (CMSString.Equals(drpType.SelectedValue, "ascx", true))
                {
                    code = MacroSecurityProcessor.RemoveSecurityParameters(code, false, null);

                    code = code.Replace("{% Register", "<%@ Register").Replace("{%", "<%#").Replace("%}", "%>");
                }

                ShowMessage();
                break;

            case "xslt":
                // No transformation
                break;

            default:
                // Convert to macro syntax
                code = code.Replace("<%@", "{%").Replace("<%#", "{%").Replace("<%=", "{%").Replace("<%", "{%").Replace("%>", "%}");
                break;
        }

        // Move the content if necessary
        if (CMSString.Equals(drpType.SelectedValue, "html", true))
        {
            // Move from text to WYSIWYG
            if (txtCode.Visible)
            {
                tbWysiwyg.ResolvedValue = code;
                tbWysiwyg.Visible = true;

                txtCode.Text = string.Empty;
                txtCode.Visible = false;
            }
        }
        else
        {
            // Move from WYSIWYG to text
            if (tbWysiwyg.Visible)
            {
                code = HttpUtility.HtmlDecode(code);

                txtCode.Text = code;
                txtCode.Visible = true;

                tbWysiwyg.ResolvedValue = string.Empty;
                tbWysiwyg.Visible = false;
            }
            else
            {
                txtCode.Text = code;
            }
        }

        SetEditor();
    }


    /// <summary>
    /// Initializes dropdown lists.
    /// </summary>
    private void DropDownListInit()
    {
        // Initialize
        if ((transformationID > 0) || !String.IsNullOrEmpty(TransformationName) || user.IsAuthorizedPerResource("CMS.Design", "EditCode"))
        {
            drpType.Items.Add(new ListItem(GetString("TransformationType.Ascx"), TransformationTypeEnum.Ascx.ToString()));
        }
        drpType.Items.Add(new ListItem(GetString("TransformationType.Text"), TransformationTypeEnum.Text.ToString()));
        drpType.Items.Add(new ListItem(GetString("TransformationType.Html"), TransformationTypeEnum.Html.ToString()));
        drpType.Items.Add(new ListItem(GetString("TransformationType.Xslt"), TransformationTypeEnum.Xslt.ToString()));
        drpType.Items.Add(new ListItem(GetString("TransformationType.jQuery"), TransformationTypeEnum.jQuery.ToString()));
    }

    #endregion


    #region "IPostBackEventHandler members"

    /// <summary>
    ///  Process postback action
    /// </summary>
    void IPostBackEventHandler.RaisePostBackEvent(string eventArgument)
    {
        switch (eventArgument.ToLowerCSafe())
        {
            case "xml":
                GenerateDefaultTransformation(DefaultTransformationTypeEnum.XML);
                break;
            case "atom":
                GenerateDefaultTransformation(DefaultTransformationTypeEnum.Atom);
                break;
            case "rss":
                GenerateDefaultTransformation(DefaultTransformationTypeEnum.RSS);
                break;
            default:
                GenerateDefaultTransformation(DefaultTransformationTypeEnum.Default);
                break;
        }
    }

    #endregion
}
