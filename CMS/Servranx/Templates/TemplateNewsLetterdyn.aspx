<%@ Page Title="" Language="C#" MasterPageFile="~/Servranx/Templates/MasterPagedyn.master" AutoEventWireup="true" CodeFile="TemplateNewsLetterdyn.aspx.cs" Inherits="Servranx_Templates_TemplateNewsLetterdyn" %>
<%@ Register Assembly="CMS.Controls" Namespace="CMS.Controls" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server" EnableViewState="false">

 <cms:CMSRepeater ID="Rpt" runat="server" ClassNames="custom.Article_Newsletter" MaxRelativeLevel="-1"
                        TransformationName="custom.Article_Newsletter.Default" AlternatingTransformationName="custom.Article_Newsletter.Preview" Path="/Newsletter/Articles-Newsletter/%" OrderBy="NodeOrder" EnableViewState="false">
                        </cms:CMSRepeater>
</asp:Content>


