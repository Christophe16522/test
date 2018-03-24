using System;
using System.Web;
using System.Web.UI.WebControls;

using CMS.CMSHelper;
using CMS.FormControls;
using CMS.GlobalHelper;
using CMS.IO;
using CMS.PortalEngine;
using CMS.SettingsProvider;
using CMS.SiteProvider;
using CMS.ExtendedControls;
using CMS.Membership;
using CMS.Helpers;
using CMS.DataEngine;

public partial class CMSModules_Membership_FormControls_Avatars_UserPictureEdit : FormEngineUserControl
{
    #region "Variables"

    private UserInfo mUserInfo = null;
    private int mMaxSideSize = 0;
    private AvatarTypeEnum avatarType = AvatarTypeEnum.User;
    private int avatarID = 0;
    private bool isValidated = false;
    private int avValue = 0;

    #endregion


    #region "Private properties"

    /// <summary>
    /// Indicates whether there were already an attempt to load an avatar.
    /// </summary>
    private bool AvatarAlreadyLoaded
    {
        get
        {
            return ValidationHelper.GetBoolean(ViewState["AvatarAlreadyLoaded"], false);
        }
        set
        {
            ViewState["AvatarAlreadyLoaded"] = value;
        }
    }

    #endregion


    #region "Public methods"

    /// <summary>
    /// Gets or sets the enabled state of the control.
    /// </summary>
    public override bool Enabled
    {
        set
        {
            btnDeleteImage.Enabled = value;
            uplFilePicture.Enabled = value;
            base.Enabled = value;
        }
        get
        {
            return base.Enabled;
        }
    }


    /// <summary>
    /// Max picture width.
    /// </summary>
    public int MaxPictureWidth
    {
        get
        {
            return picUser.Width;
        }
        set
        {
            picUser.Width = ValidationHelper.GetInteger(value, 0);
        }
    }


    /// <summary>
    /// Max picture height.
    /// </summary>
    public int MaxPictureHeight
    {
        get
        {
            return picUser.Height;
        }
        set
        {
            picUser.Height = ValidationHelper.GetInteger(value, 0);
        }
    }


    /// <summary>
    /// Keep aspect ratio.
    /// </summary>
    public bool KeepAspectRatio
    {
        get
        {
            return picUser.KeepAspectRatio;
        }
        set
        {
            picUser.KeepAspectRatio = value;
        }
    }


    /// <summary>
    /// Max upload file/picture field width.
    /// </summary>
    public int FileUploadFieldWidth
    {
        get
        {
            return (int)uplFilePicture.Width.Value;
        }
        set
        {
            uplFilePicture.Width = Unit.Pixel(value);
        }
    }


    /// <summary>
    /// User information.
    /// </summary>
    public UserInfo UserInfo
    {
        get
        {
            return mUserInfo;
        }
        set
        {
            mUserInfo = value;
            if (mUserInfo != null)
            {
                picUser.UserID = mUserInfo.UserID;
                if ((mUserInfo.UserPicture != "") || (mUserInfo.UserAvatarID != 0) || (rdbMode.SelectedValue == AvatarInfoProvider.GRAVATAR))
                {
                    pnlAvatarImage.Visible = true;
                }
                else
                {
                    pnlAvatarImage.Visible = false;
                }
            }
        }
    }


    /// <summary>
    /// Maximal side size.
    /// </summary>
    public int MaxSideSize
    {
        get
        {
            return mMaxSideSize > 0 ? mMaxSideSize : SettingsKeyInfoProvider.GetIntValue(SiteContext.CurrentSiteName + ".CMSAvatarMaxSideSize"); ;
        }
        set
        {
            picUser.Width = value;
            picUser.Height = value;
            picUser.KeepAspectRatio = true;
            mMaxSideSize = value;
        }
    }


    /// <summary>
    /// Gets or sets value - AvatarID.
    /// Returns -1 if no avatar ID is available but a new image has been provided for upload (and after it is processed, an ID is available).
    /// </summary>
    public override object Value
    {
        get
        {
            bool pseudoDelete = false;
            AvatarInfo ai = null;

            // If user info not specified
            if (UserInfo == null)
            {
                if (isValidated)
                {
                    // Check if some file was deleted
                    if (hiddenDeleteAvatar.Value.ToLowerCSafe() == "true")
                    {
                        pseudoDelete = true;
                    }


                    // Try to get avatar info
                    if (avatarID != 0)
                    {
                        ai = AvatarInfoProvider.GetAvatarInfoWithoutBinary(avatarID);
                    }

                    // If some new picture was uploaded
                    if ((uplFilePicture.PostedFile != null) && (uplFilePicture.PostedFile.ContentLength > 0))
                    {
                        // Change delete to false because file will be replaced
                        pseudoDelete = false;

                        // If some avatar exists and is custom
                        if ((ai != null) && (ai.AvatarIsCustom))
                        {
                            // Delete file and upload new
                            AvatarInfoProvider.DeleteAvatarFile(ai.AvatarGUID.ToString(), ai.AvatarFileExtension, false,
                                false);
                            AvatarInfoProvider.UploadAvatar(ai, uplFilePicture.PostedFile,
                                SettingsKeyInfoProvider.GetIntValue(SiteContext.CurrentSiteName + ".CMSAvatarWidth"),
                                SettingsKeyInfoProvider.GetIntValue(SiteContext.CurrentSiteName + ".CMSAvatarHeight"),
                                SettingsKeyInfoProvider.GetIntValue(SiteContext.CurrentSiteName + ".CMSAvatarMaxSideSize"));
                        }
                        else
                        {
                            // Create new avatar
                            ai = new AvatarInfo(uplFilePicture.PostedFile,
                                SettingsKeyInfoProvider.GetIntValue(SiteContext.CurrentSiteName + ".CMSAvatarWidth"),
                                SettingsKeyInfoProvider.GetIntValue(SiteContext.CurrentSiteName + ".CMSAvatarHeight"),
                                SettingsKeyInfoProvider.GetIntValue(SiteContext.CurrentSiteName + ".CMSAvatarMaxSideSize"));

                            if ((MembershipContext.AuthenticatedUser != null) && MembershipContext.AuthenticatedUser.IsAuthenticated() &&
                                (PortalContext.ViewMode == ViewModeEnum.LiveSite))
                            {
                                ai.AvatarName =
                                    AvatarInfoProvider.GetUniqueAvatarName(GetString("avat.custom") + " " +
                                                                           MembershipContext.AuthenticatedUser.UserName);
                            }
                            else
                            {
                                ai.AvatarName =
                                    AvatarInfoProvider.GetUniqueAvatarName(ai.AvatarFileName.Substring(0,
                                        ai.AvatarFileName.LastIndexOfCSafe(".")));
                            }
                            ai.AvatarType = AvatarTypeEnum.User.ToString();
                            ai.AvatarIsCustom = true;
                            ai.AvatarGUID = Guid.NewGuid();
                        }

                        // Update database
                        AvatarInfoProvider.SetAvatarInfo(ai);
                    }
                        // If some predefined avatar was selected
                    else if (!string.IsNullOrEmpty(hiddenAvatarGuid.Value))
                    {
                        // Change delete to false because file will be replaced
                        pseudoDelete = false;

                        // If some avatar exists and is custom
                        if ((ai != null) && (ai.AvatarIsCustom))
                        {
                            AvatarInfoProvider.DeleteAvatarFile(ai.AvatarGUID.ToString(), ai.AvatarFileExtension, false,
                                false);
                            AvatarInfoProvider.DeleteAvatarInfo(ai);
                        }

                        Guid guid = ValidationHelper.GetGuid(hiddenAvatarGuid.Value, Guid.NewGuid());
                        ai = AvatarInfoProvider.GetAvatarInfoWithoutBinary(guid);
                    }

                    // If file was deleted - not replaced
                    if (pseudoDelete)
                    {
                        // Delete it
                        ai = AvatarInfoProvider.GetAvatarInfoWithoutBinary(avatarID);
                        if (ai != null)
                        {
                            if (ai.AvatarIsCustom)
                            {
                                AvatarInfoProvider.DeleteAvatarInfo(ai);
                            }
                        }

                        ai = null;
                        avatarID = 0;
                        pnlAvatarImage.Visible = false;
                        picUser.AvatarID = 0;
                    }

                    // Update avatar id
                    if (ai != null)
                    {
                        avatarID = ai.AvatarID;
                    }
                }

                // Show avatar
                if (avatarID != 0)
                {
                    picUser.AvatarID = avatarID;
                    pnlAvatarImage.Visible = true;
                    btnDeleteImage.Visible = true;
                }

                // Get current user's avatar from basic form
                if (this.Data != null)
                {
                    int userID = 0;
                    Data["UserAvatarType"] = rdbMode.SelectedValue;
                    userID = ValidationHelper.GetInteger(Data["UserID"], 0);

                    if ((avatarID == 0) && (userID > 0))
                    {
                        UserInfo ui = UserInfoProvider.GetUserInfo(userID);
                        avatarID = ui.UserSettings.UserAvatarID;
                    }
                }

                // Hide delete button for gravatar
                if (rdbMode.SelectedValue == AvatarInfoProvider.GRAVATAR)
                {
                    btnDeleteImage.Visible = false;
                }
            }
            else
            {
                avatarID = UserInfo.UserAvatarID;
            }

            // Check if selected avatar id exists
            ai = AvatarInfoProvider.GetAvatarInfoWithoutBinary(avatarID);
            if (ai == null)
            {
                if (IsNewPictureUploaded())
                {
                    // No avatar ID is available at the moment but a new image has been provided for upload. Let the validation pass.
                    return -1;
                }

                return null;
            }

            // Return existing AvatarID
            return avatarID;
        }
        set
        {
            avValue = ValidationHelper.GetInteger(value, 0);
        }
    }

    #endregion


    #region "Events"

    /// <summary>
    /// Page load.
    /// </summary>
    protected void Page_Load(object sender, EventArgs e)
    {

        string aType = SetMode(avValue);

        picUser.UserAvatarType = aType;

        switch (aType)
        {
            case AvatarInfoProvider.AVATAR:

                // Get resource strings
                lblUploader.Text = GetString("filelist.btnupload") + ResHelper.Colon;

                // Setup delete image properties
                btnDeleteImage.ImageUrl = GetImageUrl("Design/Controls/UniGrid/Actions/delete.png");
                btnDeleteImage.OnClientClick = "return deleteAvatar('" + hiddenDeleteAvatar.ClientID + "', '" + hiddenAvatarGuid.ClientID + "', '" + pnlAvatarImage.ClientID + "' );";
                btnDeleteImage.ToolTip = GetString("general.delete");
                btnDeleteImage.AlternateText = btnDeleteImage.ToolTip;


                // Setup show gallery button
                btnShowGallery.Text = GetString("avat.selector.select");
                btnShowGallery.Visible = SettingsKeyInfoProvider.GetBoolValue(SiteContext.CurrentSiteName + ".CMSEnableDefaultAvatars");

                // Register dialog script
                string resolvedAvatarsPage = string.Empty;
                if (IsLiveSite)
                {
                    if (MembershipContext.AuthenticatedUser.IsAuthenticated())
                    {
                        resolvedAvatarsPage = AuthenticationHelper.ResolveDialogUrl("~/CMSModules/Avatars/CMSPages/AvatarsGallery.aspx");
                    }
                    else
                    {
                        resolvedAvatarsPage = AuthenticationHelper.ResolveDialogUrl("~/CMSModules/Avatars/CMSPages/PublicAvatarsGallery.aspx");
                    }
                }
                else
                {
                    resolvedAvatarsPage = ResolveUrl("~/CMSModules/Avatars/Dialogs/AvatarsGallery.aspx");
                }

                ScriptHelper.RegisterDialogScript(Page);
                ControlsHelper.RegisterClientScriptBlock(this, Page, typeof(string), "SelectAvatar",
                                                       ScriptHelper.GetScript("function SelectAvatar(avatarType, clientId) { " +
                                                                              "modalDialog('" + resolvedAvatarsPage + "?avatartype=' + avatarType + '&clientid=' + clientId, 'permissionDialog', 600, 270); return false;}"));
                ltlScript.Text = ScriptHelper.GetScript("function UpdateForm(){ ; } \n ");

                // Setup btnShowGallery action
                btnShowGallery.Attributes.Add("onclick", "SelectAvatar('" + AvatarInfoProvider.GetAvatarTypeString(avatarType) + "', '" + ClientID + "'); return false;");

                // Get image size param(s) for preview
                string sizeParams = string.Empty;
                // Keep aspect ratio is set - property was set directly or indirectly by max side size property.  
                if (KeepAspectRatio)
                {
                    sizeParams += "&maxsidesize=" + (MaxPictureWidth > MaxPictureHeight ? MaxPictureWidth : MaxPictureHeight);
                }
                else
                {
                    sizeParams += "&width=" + MaxPictureWidth + "&height=" + MaxPictureHeight;
                }

                // JavaScript which creates selected image preview and saves image guid  to hidden field
                string getAvatarPath = ResolveUrl("~/CMSModules/Avatars/CMSPages/GetAvatar.aspx");

                string updateHiddenScript = ScriptHelper.GetScript("function " + ClientID + "updateHidden(guidPrefix, clientId)" +
                                                                   "{" +
                                                                   "if ( clientId == '" + ClientID + "')" +
                                                                   "{" +
                                                                   "avatarGuid = guidPrefix.substring(4);" +
                                                                   "if ( avatarGuid != '')" +
                                                                   "{" +
                                                                   "hidden = document.getElementById('" + hiddenAvatarGuid.ClientID + "');" +
                                                                   "hidden.value = avatarGuid ;" +
                                                                   "div = document.getElementById('" + pnlPreview.ClientID + "');" +
                                                                   "div.style.display='';" +
                                                                   "div.innerHTML = '<img src=\"" + getAvatarPath + "?avatarguid=" + "'+ avatarGuid + '" + sizeParams + "\" />" +
                                                                   "&#13;&#10;&nbsp;<img src=\"" + btnDeleteImage.ImageUrl + "\" border=\"0\" onclick=\"deleteImagePreview(\\'" + hiddenAvatarGuid.ClientID + "\\',\\'" + pnlPreview.ClientID + "\\')\" style=\"cursor:pointer\"/>';" +
                                                                   "placeholder = document.getElementById('" + pnlAvatarImage.ClientID + "');" +
                                                                   "if ( placeholder != null)" +
                                                                   "{" +
                                                                   "placeholder.style.display='none';" +
                                                                   "}" +
                                                                   "}" +
                                                                   "}" +
                                                                   "}");

                ControlsHelper.RegisterClientScriptBlock(this, Page, typeof(string), ClientID + "updateHidden", updateHiddenScript);

                // JavaScript which deletes image preview
                string deleteImagePreviewScript = ScriptHelper.GetScript("function deleteImagePreview(hiddenId, divId)" +
                                                                         "{" +
                                                                         "if( confirm(" + ScriptHelper.GetString(GetString("myprofile.pictdeleteconfirm")) + "))" +
                                                                         "{" +
                                                                         "hidden = document.getElementById(hiddenId);" +
                                                                         "hidden.value = '' ;" +
                                                                         "div = document.getElementById(divId);" +
                                                                         "div.style.display='none';" +
                                                                         "div.innerHTML = ''; " +
                                                                         "}" +
                                                                         "}");

                ControlsHelper.RegisterClientScriptBlock(this, Page, typeof(string), "deleteImagePreviewScript", deleteImagePreviewScript);

                // JavaScript which pseudo deletes avatar 
                string deleteAvatarScript = ScriptHelper.GetScript("function deleteAvatar(hiddenDeleteId, hiddenGuidId, placeholderId)" +
                                                                   "{" +
                                                                   "if( confirm(" + ScriptHelper.GetString(GetString("myprofile.pictdeleteconfirm")) + "))" +
                                                                   "{" +
                                                                   "hidden = document.getElementById(hiddenDeleteId);" +
                                                                   "hidden.value = 'true' ;" +
                                                                   "placeholder = document.getElementById(placeholderId);" +
                                                                   "placeholder.style.display='none';" +
                                                                   "hidden = document.getElementById(hiddenGuidId);" +
                                                                   "hidden.value = '' ;" +
                                                                   "}" +
                                                                   "return false; " +
                                                                   "}");
                ControlsHelper.RegisterClientScriptBlock(this, Page, typeof(string), "deleteAvatar", deleteAvatarScript);

                // Initialize control after postback to preserve selected avatar
                bool avatarDeleted = ValidationHelper.GetBoolean(hiddenDeleteAvatar.Value, false);
                if (RequestHelper.IsPostBack())
                {
                    // Predefined avatar was selected so it has to be shown
                    if (!String.IsNullOrEmpty(hiddenAvatarGuid.Value))
                    {
                        Guid guid = ValidationHelper.GetGuid(hiddenAvatarGuid.Value, Guid.NewGuid());
                        AvatarInfo avatarByGuid = AvatarInfoProvider.GetAvatarInfoWithoutBinary(guid);
                        if (avatarByGuid != null)
                        {
                            avatarID = avatarByGuid.AvatarID;
                            picUser.AvatarID = avatarID;
                            picUser.UserID = 0;
                            pnlAvatarImage.Visible = true;
                            avatarDeleted = false;
                        }
                    }
                    else if (avatarDeleted)
                    {
                        // Avatar has to be deleted
                        avatarID = 0;
                        picUser.AvatarID = 0;
                        picUser.UserID = 0;
                        pnlAvatarImage.Visible = false;
                    }
                }

                // Try to load avatar either on first load or when it is first attempt to load an avatar
                if ((UserInfo == null) && (!RequestHelper.IsPostBack() || !AvatarAlreadyLoaded) && (avatarID != 0))
                {
                    AvatarAlreadyLoaded = true;

                    pnlAvatarImage.Visible = true;
                    picUser.AvatarID = avatarID;

                }
                btnDeleteImage.Visible = (!avatarDeleted && ((avatarID > 0) || ((UserInfo != null) && (UserInfo.UserAvatarID > 0))));
                plcUploader.Visible = true;
                imgHelp.Visible = false;

                break;

            case AvatarInfoProvider.GRAVATAR:
                // Hide avatar controls
                btnDeleteImage.Visible = false;
                btnShowGallery.Visible = false;
                plcUploader.Visible = false;

                // Help icon for Gravatar
                ScriptHelper.RegisterTooltip(Page);
                imgHelp.ImageUrl = GetImageUrl("CMSModules/CMS_Settings/help.png");
                imgHelp.Attributes.Add("alt", "");
                ScriptHelper.AppendTooltip(imgHelp, GetString("avatar.gravatarinfo"), null);

                // Show only UserPicture control
                pnlAvatarImage.Visible = true;
                picUser.Visible = true;
                imgHelp.Visible = true;
                break;
        }

    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Is valid override.
    /// </summary>
    public override bool IsValid()
    {
        isValidated = true;
        if ((uplFilePicture.PostedFile != null) && (uplFilePicture.PostedFile.ContentLength > 0) && !ImageHelper.IsImage(Path.GetExtension(uplFilePicture.PostedFile.FileName)))
        {
            ErrorMessage = GetString("avat.filenotvalid");
            return false;
        }
        return true;
    }


    /// <summary>
    /// Indicates whether new image is uploaded or not.
    /// </summary>
    /// <returns></returns>
    private bool IsNewPictureUploaded()
    {
        return ((uplFilePicture.PostedFile != null) && (uplFilePicture.PostedFile.ContentLength > 0) && ImageHelper.IsImage(Path.GetExtension(uplFilePicture.PostedFile.FileName)));
    }


    /// <summary>
    /// Updates picture of current user.
    /// </summary>
    /// <param name="ui">User info object</param>
    public void UpdateUserPicture(UserInfo ui)
    {
        AvatarInfo ai = null;

        if (ui != null)
        {
            // Save Avatar type setting
            ui.UserSettings.UserAvatarType = rdbMode.SelectedValue;

            // Check if some avatar should be deleted
            if (hiddenDeleteAvatar.Value == "true")
            {
                DeleteOldUserPicture(ui);
            }

            // If some file was uploaded
            if (IsNewPictureUploaded())
            {
                // Check if this user has some avatar and if so check if is custom
                ai = AvatarInfoProvider.GetAvatarInfoWithoutBinary(ui.UserAvatarID);
                bool isCustom = false;
                if ((ai != null) && ai.AvatarIsCustom)
                {
                    isCustom = true;
                }

                if (isCustom)
                {
                    AvatarInfoProvider.UploadAvatar(ai, uplFilePicture.PostedFile,
                                                    SettingsKeyInfoProvider.GetIntValue(SiteContext.CurrentSiteName + ".CMSAvatarWidth"),
                                                    SettingsKeyInfoProvider.GetIntValue(SiteContext.CurrentSiteName + ".CMSAvatarHeight"),
                                                    SettingsKeyInfoProvider.GetIntValue(SiteContext.CurrentSiteName + ".CMSAvatarMaxSideSize"));
                }
                // Old avatar is not custom, so crate new custom avatar
                else
                {
                    ai = new AvatarInfo(uplFilePicture.PostedFile,
                                        SettingsKeyInfoProvider.GetIntValue(SiteContext.CurrentSiteName + ".CMSAvatarWidth"),
                                        SettingsKeyInfoProvider.GetIntValue(SiteContext.CurrentSiteName + ".CMSAvatarHeight"),
                                        SettingsKeyInfoProvider.GetIntValue(SiteContext.CurrentSiteName + ".CMSAvatarMaxSideSize"));

                    ai.AvatarName = AvatarInfoProvider.GetUniqueAvatarName(GetString("avat.custom") + " " + ui.UserName);
                    ai.AvatarType = AvatarTypeEnum.User.ToString();
                    ai.AvatarIsCustom = true;
                    ai.AvatarGUID = Guid.NewGuid();
                }

                AvatarInfoProvider.SetAvatarInfo(ai);

                // Update user info
                ui.UserAvatarID = ai.AvatarID;
                UserInfoProvider.SetUserInfo(ui);

                pnlAvatarImage.Visible = true;
                btnDeleteImage.Visible = true;
            }
            // If predefined was chosen
            else if (!string.IsNullOrEmpty(hiddenAvatarGuid.Value))
            {
                // Delete old picture 
                DeleteOldUserPicture(ui);

                Guid guid = ValidationHelper.GetGuid(hiddenAvatarGuid.Value, Guid.NewGuid());
                ai = AvatarInfoProvider.GetAvatarInfoWithoutBinary(guid);

                // Update user info
                if (ai != null)
                {
                    ui.UserAvatarID = ai.AvatarID;
                    UserInfoProvider.SetUserInfo(ui);
                }

                pnlAvatarImage.Visible = true;
                btnDeleteImage.Visible = true;
            }
            else
            {
                pnlAvatarImage.Visible = true;
                picUser.Visible = true;
                if (rdbMode.SelectedValue != AvatarInfoProvider.GRAVATAR)
                {
                    btnDeleteImage.Visible = true;
                }
                UserInfoProvider.SetUserInfo(ui);
            }
        }
    }


    /// <summary>
    /// Deletes user picture.
    /// </summary>
    /// <param name="ui">UserInfo</param>
    public static void DeleteOldUserPicture(UserInfo ui)
    {
        // Delete old picture if needed
        if (ui.UserAvatarID != 0)
        {
            // Delete avatar info provider if needed
            AvatarInfo ai = AvatarInfoProvider.GetAvatarInfoWithoutBinary(ui.UserAvatarID);
            if (ai != null)
            {
                if (ai.AvatarIsCustom)
                {
                    AvatarInfoProvider.DeleteAvatarFile(ai.AvatarGUID.ToString(), ai.AvatarFileExtension, false, false);
                    AvatarInfoProvider.DeleteAvatarInfo(ai);
                }

                ui.UserAvatarID = 0;
                UserInfoProvider.SetUserInfo(ui);
                MembershipContext.AuthenticatedUser.UserAvatarID = 0;
            }
        }
        // Backward compatibility
        else if (ui.UserPicture != "")
        {
            try
            {
                // Remove from HDD
                string jDirectory = HttpContext.Current.Server.MapPath("~/CMSGlobalFiles/UserPictures/");
                string filename = ui.UserPicture.Remove(ui.UserPicture.IndexOfCSafe('/'));
                if (File.Exists(jDirectory + filename))
                {
                    File.Delete(jDirectory + filename);
                }
            }
            catch
            {
            }

            ui.UserPicture = "";
            UserInfoProvider.SetUserInfo(ui);
            MembershipContext.AuthenticatedUser.UserPicture = "";
        }
    }


    private string SetMode(int value)
    {
        int userID = 0;

        if (this.Data != null)
        {
            userID = ValidationHelper.GetInteger(this.Data["UserID"], 0);
        }
        else
        {
            userID = this.UserInfo.UserID;
        }

        // Load avatar settings
        string aType = DataHelper.GetNotEmpty(SettingsKeyInfoProvider.GetStringValue(SiteContext.CurrentSiteName + ".CMSAvatarType"), AvatarInfoProvider.AVATAR);

        if (aType == AvatarInfoProvider.USERCHOICE)
        {
            rdbMode.Visible = true;

            // First request
            if (!RequestHelper.IsPostBack())
            {
                UserInfo ui = UserInfoProvider.GetUserInfo(userID);
                if (ui != null)
                {
                    aType = ui.UserSettings.UserAvatarType;
                }
                else
                {
                    aType = AvatarInfoProvider.GRAVATAR;
                }

                // Set selector on first request
                rdbMode.SelectedValue = aType;
            }
            else
            {
                // Get type from selector on postback
                aType = rdbMode.SelectedValue;
            }

        }
        else
        {
            rdbMode.SelectedValue = aType;
        }

        // Set UserPicture control to view correct image
        switch (aType)
        {
            case AvatarInfoProvider.AVATAR:
                avatarID = value;
                picUser.AvatarID = value;
                picUser.UserAvatarType = aType;
                break;
            case AvatarInfoProvider.GRAVATAR:
                picUser.UserID = userID;
                picUser.UserAvatarType = aType;
                break;
        }

        return aType;
    }


    #endregion
}