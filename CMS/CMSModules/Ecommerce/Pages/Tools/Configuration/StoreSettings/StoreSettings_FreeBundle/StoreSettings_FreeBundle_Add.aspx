<%@ Page Language="C#" AutoEventWireup="true" Theme="Default" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master"
    CodeFile="StoreSettings_FreeBundle_Add.aspx.cs" Inherits="CMSModules_Ecommerce_Pages_Tools_Configuration_StoreSettings_StoreSettings_FreeBundle_StoreSettings_FreeBundle_Add" %>

<asp:Content ID="Content1" ContentPlaceHolderID="plcContent" runat="Server">
    <div class="PageContent">
        <div class="WebPartForm">
            <table class="EditingFormCategoryTableContente">
                <tbody>
                    <tr class="EditingFormRow">
                        <td class="EditingFormLabelCell" style="width: 150px;">
                            Description:
                        </td>
                        <td class="EditingFormValueCell" style="width: 300px;">
                            <asp:TextBox ID="txtDescription" runat="server" TextMode="SingleLine" EnableViewState="false" CssClass="TextBoxField"/>
                        </td>
                    </tr>
                    <tr class="EditingFormRow">
                        <td class="EditingFormLabelCell" style="width: 150px;">
                            Quantity:
                        </td>
                        <td class="EditingFormValueCell" style="width: 300px;">
                            <asp:TextBox ID="txtQuantity" runat="server" TextMode="SingleLine" EnableViewState="false" CssClass="TextBoxField"/>
                            <ajaxToolkit:FilteredTextBoxExtender ID="txtQuantityFilteredTextBoxExtender" runat="server" TargetControlID ="txtQuantity" FilterType ="Numbers">
                            </ajaxToolkit:FilteredTextBoxExtender>
                        </td>
                    </tr>
                </tbody>
            </table>
        </div>
    </div>
    <cms:FormSubmitButton runat="server" ID="btnOk" OnClick="btnOK_Click" EnableViewState="false" />
</asp:Content>
