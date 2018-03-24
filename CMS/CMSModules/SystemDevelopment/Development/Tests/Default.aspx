<%@ Page Language="C#" AutoEventWireup="true" Inherits="CMSModules_SystemDevelopment_Development_Tests_Default"
    Theme="Default" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" CodeFile="Default.aspx.cs" %>

<%@ Register Src="~/CMSFormControls/Classes/AssemblyClassSelector.ascx" TagPrefix="cms" TagName="AssemblyClassSelector" %>

<asp:Content ID="content" ContentPlaceHolderID="plcContent" runat="server">
    <div class="form-horizontal">
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblClass" Text="Class" DisplayColon="true" AssociatedControlID="selClass" />
            </div>
            <div class="editing-form-value-cell">
                <cms:AssemblyClassSelector runat="server" ID="selClass" />
            </div>
        </div>
        <div class="form-group">
            <div class="editing-form-label-cell">
                <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblMethods" Text="Methods" DisplayColon="true" AssociatedControlID="radAll" />
            </div>
            <div class="editing-form-value-cell radio-list-vertical">
                <cms:CMSRadioButton runat="server" ID="radAll" Text="All public methods" GroupName="Methods" Checked="true" />
                <div class="checkbox-list-vertical selector-subitem">
                    <cms:CMSCheckBox runat="server" ID="chkStatic" Text="Include static methods" Checked="true" />
                    <cms:CMSCheckBox runat="server" ID="chkInstance" Text="Include instance methods" Checked="true" />
                    <cms:CMSCheckBox runat="server" ID="chkStaticProp" Text="Include static properties" Checked="false" />
                    <cms:CMSCheckBox runat="server" ID="chkProp" Text="Include instance properties" Checked="false" />
                    <cms:CMSCheckBox runat="server" ID="chkInherited" Text="Include inherited methods up to level of inheritance: " />
                    <div class="selector-subitem">
                    <cms:CMSTextBox runat="server" ID="txtInheritance" Text="1" MaxLength="2" />
                </div>
                    </div>
                <cms:CMSRadioButton runat="server" ID="radSingle" Text="Specific method:" GroupName="Methods" />
                <div class="selector-subitem">
                    <cms:CMSTextBox runat="server" ID="txtMethodName" />
                </div>
            </div>
        </div>
        <div class="form-group form-group-submit">
                <cms:LocalizedButton runat="server" ID="btnGenerate" Text="Generate" OnClick="btnGenerate_Click" ButtonStyle="Primary" />
        </div>
    </div>
    <cms:LocalizedHeading ID="headCode" Level="5" Text="Code:" runat="server" />
    <cms:CMSTextBox runat="server" ID="txtCode" TextMode="MultiLine" Width="100%" Height="400px" EnableViewState="false" />
    <br />
    <cms:LocalizedHeading ID="headClasses" Level="5" Text="Covered classes:" runat="server" />
    <asp:Literal runat="server" ID="ltlHierarchy" EnableViewState="false" />
</asp:Content>
