<%@ Page Title="" Language="C#" Inherits="System.Web.Mvc.ViewPage<BlessTheWeb.Models.AbsolutionsViewModel>" MasterPageFile="~/Views/Shared/Site.Master" %>
<%@ Import Namespace="BlessTheWeb.Core.Extensions" %>
<asp:Content runat="server" ID="Title" ContentPlaceHolderID="TitleContent">Browse Indulgences - <%=System.Configuration.ConfigurationManager.AppSettings["SiteTitle"]%></asp:Content>
<asp:Content runat="server" ID="Main" ContentPlaceHolderID="MainContent">

<div id="indulgence-wall">
    <h2>All Indulgences</h2>
    <%foreach(var indulgence in Model.Indulgences)
      {%>
      <div class="indulgence">
          <a href="/indulgences/<%=indulgence.Id.IdValue() %>"><img src="/content/indulgences/<%=indulgence.Id.IdValue() %>/indulgence_25.png" /></a>
          <p><a href="/indulgences/<%=indulgence.Id.IdValue() %>"><%:indulgence.Name %>, <%=indulgence.DateConfessed.ToString("hh:mm dd/MM/yyyy") %></a></p>
      </div>
    <%
      }%>
      <div class="clearfix"></div>
      <%if (Model.ShowPreviousPageLink) {%><%=Html.ActionLink("Previous Page", "List", "Indulgence", new {page=Model.PreviousPage}, null) %><%} %>
      <%for(int i=Model.PagingStart;i<Model.PagingEnd;i++) { %>
        <span><%=i %></span>
      <%} %>
      <%if (Model.ShowNextPageLink) {%><%=Html.ActionLink("Next Page", "List", "Indulgence", new {page=Model.NextPage}, null) %><%} %>

  </div>
</asp:Content>
