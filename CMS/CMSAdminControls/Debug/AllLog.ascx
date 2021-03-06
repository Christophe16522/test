<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSAdminControls_Debug_AllLog"
    EnableViewState="false" CodeFile="AllLog.ascx.cs" %>
<div class="DebugLog">
    <asp:Literal runat="server" ID="ltlInfo" EnableViewState="false" />
    <cms:UIGridView runat="server" ID="gridDebug" ShowFooter="true" AutoGenerateColumns="false" CssClass="wrap-normal">
        <Columns>
            <asp:TemplateField>
                <ItemTemplate>
                    <strong><%#Eval("Index")%></strong>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField>
                <ItemTemplate>
                    <%#Eval("DebugType")%>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField>
                <ItemTemplate>
                    <%#Eval("Information")%>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField>
                <ItemTemplate>
                    <%#Eval("Result")%>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField>
                <ItemTemplate>
                    <%#Eval("Context")%>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField>
                <ItemTemplate>
                    <%#Eval("TotalDuration")%>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField>
                <ItemTemplate>
                    <%#Eval("Duration")%>
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
    </cms:UIGridView>
</div>
