<%@ Page Language="C#" AutoEventWireup="true"
    Inherits="CMSModules_Widgets_Dialogs_WidgetProperties_Buttons" Theme="default" CodeFile="WidgetProperties_Buttons.aspx.cs"
    MasterPageFile="~/CMSMasterPages/UI/EmptyPage.master" %>

<asp:Content ID="pnlContent" ContentPlaceHolderID="plcContent" runat="server">
    <asp:Panel runat="server" ID="pnlScroll" CssClass="PageFooterLine">
        <div class="FloatLeft">
            <cms:CMSCheckBox runat="server" ID="chkRefresh" Checked="true" />
        </div>
        <div class="FloatRight">
            <cms:CMSButton ID="btnOk" runat="server" ButtonStyle="Primary" /><cms:CMSButton
                ID="btnCancel" runat="server" ButtonStyle="Primary" /><cms:CMSButton ID="btnApply"
                    runat="server" ButtonStyle="Primary" />
        </div>
    </asp:Panel>
    <asp:Literal runat="server" ID="ltlScript" EnableViewState="false" />
</asp:Content>
