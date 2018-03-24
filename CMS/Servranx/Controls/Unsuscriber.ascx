<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Unsuscriber.ascx.cs" Inherits="Servranx_Controls_Unsuscriber" %>
<asp:TextBox ID="txtmail" class="BoxTxtNL" runat="server" size="25" title="name"></asp:TextBox>
         <asp:ImageButton ID="btnSearh" class="btnuns" ImageUrl="~/App_Themes/Alutec/images/vide.png" OnClientClick="javascript:return confirm('Voulez-vous confirmer la desinscription à la newsletter de Servranx?');"
        runat="server"  width="83" height="31" onclick="btnSearh_Click" />
         <div class="btnunser"> 
             <asp:RegularExpressionValidator ID="revMail"  ControlToValidate="txtmail" runat="server" 
            ErrorMessage="Vous devez entrez une adresse mail valide" ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*">
             </asp:RegularExpressionValidator>
         </div>
       <div class="btnunser"> 
       <asp:Label ID="lblerror"  runat="server" >
        
        </asp:Label>
        </div>