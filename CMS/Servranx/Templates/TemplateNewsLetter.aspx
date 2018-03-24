<%@ Page Title="" Language="C#" MasterPageFile="~/Servranx/Templates/MasterPage.master" AutoEventWireup="true" CodeFile="TemplateNewsLetter.aspx.cs" Inherits="Servranx_Templates_TemplateNewsLetter" %>
<%@ Register Assembly="CMS.Controls" Namespace="CMS.Controls" TagPrefix="cms" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <table cellpadding="0" cellspacing="0" width="100%" align="center" >
	<tr valign="top">
    	<td style="border-top:#c45700 5px solid; border-bottom:#c45700 2px solid; background-color:#3c3341; padding:15px 0 30px;" align="center">
            <table cellpadding="0" cellspacing="0" width="600">
                <tr valign="top">
                    <td align="left">
                    	<a href="#">
                        	<img src="~/App_Themes/Servranx/images/Logo.gif" width="233" height="97" alt="Servranx Accueil" border="none" style="display:block;" />
                        </a>
                    </td>
                    <td>
                    </td>
                </tr>
            </table>
        </td>
    </tr>
    <tr valign="top">
    	<td style="background-color:#f7f7f7;" align="center">
        	<table cellpadding="0" cellspacing="10" width="600">
            	<tr valign="top">
                	<td width="185">
                     <%--<cms:CMSWebPartZone ID="CMSWebPartZone1" ZoneID="zonemenu" runat="server" />--%>
                    
                    <cms:CMSEditableRegion ID="EditableRegion" runat="server" RegionType="HtmlEditor" RegionTitle="News Letter" />
                    </td>
                </tr>
            </table>
        </td>
    </tr>
        
    <tr valign="top">
    	<td align="center" style="background-color:#e2dee7;">
        	<table cellpadding="0" cellspacing="0" width="600">
            	<tr valign="top">
                	<td style="text-align:center;color:#3b313f;font-family:Arial, Helvetica, sans-serif; font-size:11px; padding:10px 0;">
                        Les éditions Servranx  -  23-25, rue Gustave Biot   -   1050 Bruxelles – Belgique<br />
            Tél. : 02 649 18 40 (de l'étranger : 00 32 26 49 18 40).  -   <a href="mailto:info@servranx.com" target="_blank" style="color:#3b313f; text-decoration:none;">info@servranx.com</a>  -  <a href="http://www.servranx.com" target="_blank" style="color:#3b313f; text-decoration:none;">www.servranx.com</a>
           <br/> Cliquez <a href="~/Newsletter/Desincription.aspx">ici</a> pour se Désinscrire de la Newsletter
                    </td>
                </tr>
            </table>
        </td>
    </tr>
</table>
</asp:Content>

