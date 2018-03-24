<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_Ecommerce_Controls_ShoppingCart_ShoppingCartCustomerSelection"
    CodeFile="ShoppingCartCustomerSelection.ascx.cs" %>
<%@ Register Src="~/CMSModules/Ecommerce/FormControls/CustomerSelector.ascx" TagName="CustomerSelector"
    TagPrefix="cms" %>
<%@ Register TagPrefix="cms" TagName="CustomerNew" Src="~/CMSModules/Ecommerce/Controls/UI/CustomerNew.ascx" %>
<cms:LocalizedLabel runat="server" ResourceString="shoppingcart.selectcustomer" CssClass="BlockTitle"
    EnableViewState="false" />
<div class="BlockContent">
    <cms:CMSUpdatePanel runat="server" ID="updCustomerSelection">
        <ContentTemplate>
            <%-- Choices --%>
            <cms:LocalizedRadioButton runat="server" ID="radSelectCustomer" GroupName="selectCreate"
                ResourceString="com.cartcustomerselection.selectcustomer" Checked="true" AutoPostBack="true" />
            <br />
            <cms:LocalizedRadioButton runat="server" ID="radCreateCustomer" GroupName="selectCreate"
                ResourceString="com.cartcustomerselection.createcustomer" AutoPostBack="true" />
            <br />
            <br />
            <div style="margin-left: 20px;">
                <%-- Select existing customer --%>
                <asp:Label ID="lblSelectError" runat="server" CssClass="ErrorLabel" EnableViewState="False"
                    Visible="False" />
                <cms:CustomerSelector runat="server" ID="customerSelector" DisplayOnlyEnabled="true" />
                <%-- Create new customer --%>
                <cms:CustomerNew runat="server" ID="ucCustomerNew" />
            </div>
        </ContentTemplate>
    </cms:CMSUpdatePanel>
</div>
