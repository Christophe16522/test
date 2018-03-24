<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSAdminControls_Debug_WebFarmLog"
    CodeFile="WebFarmLog.ascx.cs" %>
<div class="DebugLog">
    <asp:Literal runat="server" ID="ltlInfo" EnableViewState="false" />
    <cms:UIGridView runat="server" ID="gridQueries" ShowFooter="true" AutoGenerateColumns="false">
        <Columns>
            <asp:TemplateField>
                <ItemTemplate>
                    <strong><%#GetIndex()%></strong>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField>
                <ItemTemplate>
                    <strong><%#Eval("TaskType")%></strong>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField>
                <ItemTemplate>
                    <strong><%#HTMLHelper.HTMLEncode(ValidationHelper.GetString(Eval("Target"), ""))%></strong>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField>
                <ItemTemplate>
                    <strong><%#GetData(Eval("TextData"), Eval("BinaryData"))%></strong>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField>
                <ItemTemplate>
                    <%#GetContext(Eval("Context"))%>
                </ItemTemplate>
                <FooterTemplate>
                    <strong><%#cmsVersion%></strong>
                </FooterTemplate>
            </asp:TemplateField>
        </Columns>
    </cms:UIGridView>
</div>
