<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSAdminControls_Debug_CacheLog"
    CodeFile="CacheLog.ascx.cs" %>
<div class="DebugLog">
    <asp:Literal runat="server" ID="ltlInfo" EnableViewState="false" />
    <cms:UIGridView runat="server" ID="gridCache" ShowFooter="true" AutoGenerateColumns="false">
        <Columns>
            <asp:TemplateField>
                <ItemTemplate>
                    <strong><%#GetIndex()%></strong>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField>
                <ItemTemplate>
                    <%#HttpUtility.HtmlEncode(ValidationHelper.GetString(Eval("CacheOperation"), ""))%>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField>
                <ItemTemplate>
                    <%#GetInformation(Page, Eval("CacheKey"), Eval("Dependencies"), Eval("CacheValue"), Eval("CacheOPERATION"))%>
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
