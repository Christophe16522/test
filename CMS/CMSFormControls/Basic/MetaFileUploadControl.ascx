<%@ Control Language="C#" AutoEventWireup="true" CodeFile="MetaFileUploadControl.ascx.cs"
    Inherits="CMSFormControls_Basic_MetaFileUploadControl" %>
<%@ Register Src="~/CMSModules/AdminControls/Controls/MetaFiles/File.ascx" TagName="MetafileUploader"
    TagPrefix="cms" %>
<cms:MetafileUploader ID="fileUploader" runat="server" />
