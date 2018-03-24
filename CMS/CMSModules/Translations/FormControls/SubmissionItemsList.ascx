<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_Translations_FormControls_SubmissionItemsList"
    CodeFile="SubmissionItemsList.ascx.cs" %>
<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" TagName="UniGrid" TagPrefix="cms" %>
<%@ Register Namespace="CMS.UIControls.UniGridConfig" TagPrefix="ug" Assembly="CMS.UIControls" %>
<cms:CMSUpdatePanel ID="pnlUpdate" runat="server">
    <ContentTemplate>
        <cms:UniGrid runat="server" ID="gridElem" ObjectType="cms.translationsubmissionitem"
            OrderBy="SubmissionItemName" Columns="SubmissionItemID,SubmissionItemObjectType,SubmissionItemObjectID,SubmissionItemName,SubmissionItemWordCount,SubmissionItemCharCount,SubmissionItemType"
            IsLiveSite="false" ShowObjectMenu="false" ShowActionsMenu="false">
            <GridActions Parameters="SubmissionItemID">
                <ug:Action ExternalSourceName="downloadxliff" Name="downloadxliff" Caption="$translationservice.downloadxliff$" FontIconClass="icon-eye" FontIconStyle="Allow" />
                <ug:Action Name="uploadxliff" Caption="$translationservice.uploadxliff$" FontIconClass="icon-arrow-up-line"
                    OnClick="ShowUploadDialog(0, {0});" />
            </GridActions>
            <GridColumns>
                <ug:Column Source="SubmissionItemName" Caption="$translationservice.documentname$" Wrap="false">
                    <Filter Type="Text" />
                </ug:Column>
                <ug:Column Source="SubmissionItemWordCount" Caption="$translationservice.wordcount$" Wrap="false" />
                <ug:Column Source="SubmissionItemCharCount" Caption="$translationservice.charcount$" Wrap="false" />
                <ug:Column Width="100%" />
            </GridColumns>
            <GridOptions DisplayFilter="true" />
        </cms:UniGrid>
        <br />
        <cms:LocalizedButton runat="server" ID="btnExportToZip" ResourceString="translationservice.downloadxliffinzip" ButtonStyle="Default" />
        <cms:LocalizedButton runat="server" ID="btnImportFromZip" ResourceString="translationservice.importxlifffromzip" ButtonStyle="Default" />
    </ContentTemplate>
</cms:CMSUpdatePanel>
