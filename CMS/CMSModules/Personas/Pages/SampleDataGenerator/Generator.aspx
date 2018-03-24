<%@ Page Language="C#" Debug="true" CodeFile="Generator.aspx.cs" Inherits="CMSModules_Personas_Pages_SampleDataGenerator_Generator" 
         MasterPageFile="~/CMSMasterPages/UI/SimplePage.master" Theme="Default" Title="Online Marketing sample data generator" %>


<asp:Content ID="cntContent" runat="server" ContentPlaceHolderID="plcContent">
    <asp:Panel runat="server" ID="pnlMessages"></asp:Panel>

    <div class="form-horizontal">
        <div class="form-group">
            <div class="editing-form-label-cell">
                <asp:Label runat="server" AssociatedControlID="txtContactsCount" CssClass="control-label">
                    Create contact statuses if there are none:
                </asp:Label>
            </div>
            <div class="editing-form-value-cell">
                <cms:CMSCheckBox runat="server" ID="chckCreateContactStatuses" Checked="true"/>
            </div>
        </div>
        <div class="form-group">
            <div class="editing-form-label-cell">
                <asp:Label runat="server" AssociatedControlID="chckGenerateContacts" CssClass="control-label">
                    Generate contacts:
                </asp:Label>
            </div>
            <div class="editing-form-value-cell">
                <cms:CMSCheckBox runat="server" ID="chckGenerateContacts" Checked="true"/>
            </div>
        </div>
        <div class="form-group">
            <div class="editing-form-label-cell">
                <asp:Label runat="server" AssociatedControlID="txtContactsCount" CssClass="control-label">
                    Number of contacts to generate:
                </asp:Label>
            </div>
            <div class="editing-form-value-cell">
                <div class="control-group-inline">
                    <cms:CMSTextBox runat="server" ID="txtContactsCount" Text="10" />
                    <cms:CMSCheckBox runat="server" Text="Contacts with real names" ID="chckContactRealNames" Checked="True"/>
                </div>
            </div>
        </div>
        <div class="form-group">
            <div class="editing-form-label-cell">
                <asp:Label runat="server" AssociatedControlID="txtGeneratePersonas" CssClass="control-label">
                    Generate personas:
                </asp:Label>
            </div>
            <div class="editing-form-value-cell">
                <cms:CMSCheckBox runat="server" ID="txtGeneratePersonas" Checked="true"/>
            </div>
        </div>
        <div class="form-group">
            <div class="editing-form-label-cell">
                <asp:Label runat="server" AssociatedControlID="chckGenerateScores" CssClass="control-label">
                    Generate scores:
                </asp:Label>
            </div>
            <div class="editing-form-value-cell">
                <cms:CMSCheckBox runat="server" ID="chckGenerateScores" Checked="true"/>
            </div>
        </div>
         <div class="form-group">
            <div class="editing-form-label-cell">
                <asp:Label runat="server"  AssociatedControlID="txtScoresCount" CssClass="control-label">
                    Number of scores to generate:
                </asp:Label>
            </div>
            <div class="editing-form-value-cell">
                <div class="control-group-inline">
                    <cms:CMSTextBox runat="server" ID="txtScoresCount" Text="10" />
                    <asp:Label runat="server">Every score will have 15–25 rules</asp:Label>
                </div>
            </div>
        </div>
                <div class="form-group">
            <div class="editing-form-label-cell">
                <asp:Label runat="server" AssociatedControlID="chckGenerateActivities" CssClass="control-label">
                    Generate activities for ALL existing contacts on the current site:
                </asp:Label>
            </div>
            <div class="editing-form-value-cell">
                <cms:CMSCheckBox runat="server" ID="chckGenerateActivities" Checked="False"/>
            </div>
        </div>
        <div class="form-group">
            <div class="editing-form-label-cell">
                <asp:Label runat="server" AssociatedControlID="txtActivitiesCount" CssClass="control-label">
                    Number of activities to generate for each existing contact:
                </asp:Label>
            </div>
            <div class="editing-form-value-cell">
                <cms:CMSTextBox runat="server" ID="txtActivitiesCount" Text="30"></cms:CMSTextBox>
                <span>(this will be the medium number of activities for each contact)</span>
            </div>
        </div>
        <div class="form-group">
            <div class="editing-form-value-cell-offset editing-form-value-cell">
                <cms:CMSButton runat="server" ID="btnGenerate" Text="Generate" />
            </div>
        </div>
    </div>
</asp:Content>