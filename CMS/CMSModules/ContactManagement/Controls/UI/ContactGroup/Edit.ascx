<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Edit.ascx.cs" Inherits="CMSModules_ContactManagement_Controls_UI_ContactGroup_Edit" %>

<%@ Register Src="~/CMSAdminControls/UI/Selectors/ScheduleInterval.ascx" TagPrefix="cms"
    TagName="ScheduleInterval" %>
<%@ Register Src="~/CMSFormControls/Macros/ConditionBuilder.ascx" TagPrefix="cms"
    TagName="ConditionBuilder" %>

<cms:UIForm runat="server" ID="EditForm" ObjectType="om.contactgroup" IsLiveSite="false">
    <LayoutTemplate>
        <cms:FormField runat="server" ID="fDispName" Field="ContactGroupDisplayName" FormControl="LocalizableTextBox"
            DisplayColon="true" />
        <cms:FormField runat="server" ID="fName" Field="ContactGroupName" FormControl="CodeName"
            DisplayColon="true" />
        <cms:FormField runat="server" ID="fDesc" Field="ContactGroupDescription" />
        <asp:PlaceHolder runat="server" ID="plcUpdate">
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblDynamic" EnableViewState="false" ResourceString="om.contactgroup.dynamic"
                        DisplayColon="true" AssociatedControlID="chkDynamic" ToolTipResourceString="om.contactgroup.dynamic.description" />
                </div>
                <div class="editing-form-value-cell">
                    <cms:CMSCheckBox ID="chkDynamic" runat="server" AutoPostBack="true" OnCheckedChanged="chkDynamic_CheckedChanged" />
                </div>
            </div>
            <asp:PlaceHolder ID="plcDynamic" runat="server">
                <cms:FormField runat="server" Layout="Inline" ID="fDynamicCondition" Field="ContactGroupDynamicCondition" UseFFI="false">
                    <div class="form-group">
                        <div class="editing-form-label-cell">
                            <cms:FormLabel CssClass="control-label" runat="server" ID="lCondition" EnableViewState="false" ResourceString="om.contactgroup.condition"
                                DisplayColon="true" AssociatedControlID="conditionBuilder" ToolTipResourceString="om.contactgroup.condition.description" />
                        </div>
                        <div class="editing-form-value-cell">
                            <cms:ConditionBuilder ID="conditionBuilder" runat="server" RuleCategoryNames="cms.onlinemarketing"
                                DisplayRuleType="1" MaxWidth="740" />
                        </div>
                    </div>
                </cms:FormField>
                <div class="form-group">
                    <div class="editing-form-label-cell">
                        <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblScheduler" EnableViewState="false" ResourceString="om.contactgroup.schedule"
                            DisplayColon="true" AssociatedControlID="chkSchedule" ToolTipResourceString="om.contactgroup.schedule.description" />
                    </div>
                    <div class="editing-form-value-cell">
                        <cms:CMSCheckBox ID="chkSchedule" runat="server" Checked="true" AutoPostBack="true"
                            OnCheckedChanged="chkSchedule_CheckedChanged" />
                    </div>
                </div>
                <cms:ScheduleInterval ID="schedulerInterval" runat="server" Visible="false" DefaultPeriod="day" />
            </asp:PlaceHolder>
        </asp:PlaceHolder>
    </LayoutTemplate>
</cms:UIForm>

<asp:Panel runat="server" ID="pnlInfo" Visible="false" CssClass="form-horizontal">
    <cms:LocalizedHeading runat="server" ID="lblInfo" ResourceString="om.contactgroup.info" Level="4" />
    <cms:CMSUpdatePanel runat="server" ID="pnlUpdate" UpdateMode="Conditional">
        <ContentTemplate>
            <asp:Timer ID="timRefresh" runat="server" Interval="3000" EnableViewState="false" />
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblNumberOfItems" EnableViewState="false"
                        ResourceString="om.contactgroup.numberofcontacts" DisplayColon="true" />
                </div>
                <div class="editing-form-value-cell">
                    <cms:LocalizedLabel runat="server" ID="lblNumberOfItemsValue" CssClass="form-control-text" />
                </div>
            </div>
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblStatus" EnableViewState="false" ResourceString="general.status"
                        DisplayColon="true" />
                </div>
                <div class="editing-form-value-cell">
                    <cms:LocalizedLabel runat="server" ID="lblStatusValue" EnableViewState="false" CssClass="form-control-text" />
                    <asp:Literal runat="server" ID="ltrProgress" EnableViewState="false" />
                </div>
            </div>
            <div class="form-group">
                <div class="editing-form-label-cell">
                    <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblLastEval" EnableViewState="false" ResourceString="om.contactgroup.lastrebuild"
                        DisplayColon="true" />
                </div>
                <div class="editing-form-value-cell">
                    <cms:LocalizedLabel runat="server" ID="lblLastEvalValue" EnableViewState="false" CssClass="form-control-text" />
                </div>
            </div>
            </div>
        </ContentTemplate>
    </cms:CMSUpdatePanel>
</asp:Panel>
<asp:HiddenField ID="hdnConfirmDelete" runat="server" />