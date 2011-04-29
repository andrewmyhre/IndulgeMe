<%@ Page Title="" Language="C#" Inherits="System.Web.Mvc.ViewPage<BlessTheWeb.Models.SinsViewModel>" MasterPageFile="~/Views/Shared/Site.Master" %>
<%@ Import Namespace="BlessTheWeb.Core.Extensions" %>
<asp:Content runat="server" ID="Title" ContentPlaceHolderID="TitleContent"></asp:Content>
<asp:Content runat="server" ID="Main" ContentPlaceHolderID="MainContent">

<div id="browse-sins">
<h2>All Sins</h2>
<ul>
<%foreach (var sin in Model.Sins)
  {%>
  <li>
    <span class="right"><a href="/home/absolve/<%=sin.Id.IdValue()%>">absolve this sin</a> 
    <%
      if (sin.TotalDonated > 0){%><br /><br /><a href="/sins/<%=sin.Id.IdValue()%>">view history</a><%
  }%></span>
    <em><%=sin.Content%></em>
  </li>
<%
  }%>
  </ul>

</div>

</asp:Content>
