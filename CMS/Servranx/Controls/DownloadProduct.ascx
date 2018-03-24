<%@ Control Language="C#" AutoEventWireup="true" CodeFile="DownloadProduct.ascx.cs" Inherits="Servranx_Controls_DownloadProduct" %>
                    
              
                       <% if (List!=null && List.Count > 0)
                          { %>
                       <% foreach (var product in List)
                          {
                       %> 
                        <div class="downl">
                           <div class="downlprodname"><%= product.productname %> </div>
                           <div><%= product.fileUrl %></div>
                           <div class="clr"></div>
                         </div> 
                          <%
                          }
                          }
                          %>      
               
                 
                   
                