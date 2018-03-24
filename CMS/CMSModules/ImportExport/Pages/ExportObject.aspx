<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_ImportExport_Pages_ExportObject"
    Theme="Default" ValidateRequest="false" EnableEventValidation="false" MasterPageFile="~/CMSMasterPages/UI/Dialogs/ModalDialogPage.master"
    Title="Export single object" CodeFile="ExportObject.aspx.cs" %>

<%@ Register Src="~/CMSAdminControls/AsyncControl.ascx" TagName="AsyncControl" TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/UI/System/ActivityBar.ascx" TagName="ActivityBar"
    TagPrefix="cms" %>

<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <script type="text/javascript">
        //<![CDATA[
        var importTimerId = 0;

        // End timer function
        function StopTimer() {
            if (importTimerId) {
                clearInterval(importTimerId);
                importTimerId = 0;

                if (window.HideActivity) {
                    window.HideActivity();
                }
            }
        }

        // Start timer function
        function StartTimer() {
            if (window.Activity) {
                importTimerId = setInterval("window.Activity()", 500);
            }
        }
        //]]>
    </script>
    <asp:Panel runat="server" ID="pnlDetails" Visible="true">
        <div class="form-horizontal">
            <div class="form-group">
                <cms:LocalizedLabel ID="lblBeta" runat="server" Visible="false" CssClass="ErrorLabel"
                    EnableViewState="false" />
                <asp:Label ID="lblIntro" runat="server" EnableViewState="false" />
            </div>
            <asp:PlaceHolder ID="plcExportDetails" runat="server">
                <div class="form-group">
                    <div class="editing-form-label-cell">
                        <cms:LocalizedLabel ID="lblFileName" runat="server" CssClass="control-label"
                            EnableViewState="false" ResourceString="general.filename" Font-Bold="true" DisplayColon="true" />
                    </div>
                    <div class="editing-form-value-cell">
                        <cms:CMSTextBox ID="txtFileName" runat="server" />
                    </div>
                </div>
                <div class="form-group">
                    <asp:Label ID="lblError" runat="server" CssClass="ErrorLabel" EnableViewState="false" AssociatedControlID="pnlContent" />
                </div>
            </asp:PlaceHolder>
        </div>
    </asp:Panel>
    <asp:Panel runat="server" ID="pnlContent" Visible="false">
        <div class="control-group-inline">
            <asp:Label CssClass="control-label" ID="lblResult" runat="server" EnableViewState="false" />
        </div>
        <div class="control-group-inline">
            <cms:CMSButton ID="lnkDownload" runat="server" Visible="false" ButtonStyle="Primary" EnableViewState="false" />
        </div>
    </asp:Panel>
    <asp:Panel ID="pnlProgress" runat="Server" Visible="false" EnableViewState="false">
        <div class="control-group-inline">
            <asp:Label CssClass="control-label" ID="lblProgress" runat="server" />
        </div>
        <div class="control-group-inline">
            <cms:ActivityBar ID="ucActivityBar" runat="server" />
        </div>
    </asp:Panel>
    <cms:AsyncControl ID="ucAsyncControl" runat="server" />
    <asp:Literal ID="ltlScript" runat="Server" EnableViewState="false" />
</asp:Content>
<asp:Content ID="cntFooter" ContentPlaceHolderID="plcFooter" runat="server">
    <div class="FloatRight">
        <cms:LocalizedButton ID="btnOk" runat="server" ButtonStyle="Primary" />
    </div>
</asp:Content>
