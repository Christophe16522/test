<%@ Control Language="C#" AutoEventWireup="true" CodeFile="caddy.ascx.cs" Inherits="Servranx_Controls_caddy" %>
  <asp:Repeater ID="rptNombre" runat="server">
                        <ItemTemplate>
                          <li>
                            <img src="~/App_Themes/Servranx/images/pictopanier1.gif" width="20" height="19" alt="#" /><a href="~/Shopping/Shoppingcart.aspx"><%# Eval("NumberOfItem")%> </a>
                            </li>
                        </ItemTemplate>
                    </asp:Repeater>


