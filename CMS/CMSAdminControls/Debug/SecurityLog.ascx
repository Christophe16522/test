<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSAdminControls_Debug_SecurityLog"
    CodeFile="SecurityLog.ascx.cs" %>
<div class="DebugLog">
    <asp:Literal runat="server" ID="ltlInfo" EnableViewState="false" />
    <cms:UIGridView runat="server" ID="gridSec" ShowFooter="true" AutoGenerateColumns="false">
        <Columns>
            <asp:TemplateField>
                <ItemTemplate>
                    <strong>
                        <%#GetIndex(Eval("Indent"))%>
                        <%#GetDuplicity(Eval("Duplicit"), Eval("Indent"))%>
                    </strong>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField>
                <ItemTemplate>
                    <%#GetUserName(Eval("UserName"), Eval("Indent"))%>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField>
                <ItemTemplate>
                    <%#GetBeginIndent(Eval("Indent"))%><strong><%#Eval("SecurityOperation")%></strong><%#GetEndIndent(Eval("Indent"))%>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField>
                <ItemTemplate>
                    <%#GetResult(Eval("Result"), Eval("Indent"), Eval("Important"))%>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField>
                <ItemTemplate>
                    <%#Eval("Resource")%>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField>
                <ItemTemplate>
                    <%#Eval("Name")%>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField>
                <ItemTemplate>
                    <%#Eval("SiteName")%>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField>
                <ItemTemplate>
                    <%#GetContext(Eval("Context"))%>
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
    </cms:UIGridView>
</div>
