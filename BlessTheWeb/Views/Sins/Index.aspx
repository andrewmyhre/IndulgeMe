<%@ Page Title="" Language="C#" Inherits="System.Web.Mvc.ViewPage<BlessTheWeb.Models.SinDetailViewModel>" MasterPageFile="~/Views/Shared/Site.Master" %>
<%@ Import Namespace="BlessTheWeb.Core.Extensions" %>
<asp:Content runat="server" ID="Title" ContentPlaceHolderID="TitleContent"></asp:Content>
<asp:Content runat="server" ID="Main" ContentPlaceHolderID="MainContent">

<p><em><%=Model.Sin.Content %></em></p>

<p><a href="/home/absolve/<%=Model.Sin.Id.IdValue() %>">Absolve this sin</a></p>

<h3>Absolutions</h3>
<%if (Model.Absolutions.Count() == 0)
  {%>
<p>Noone has absolved this sin yet</p>
<%
  }%>
<%foreach(var absolution in Model.Absolutions)
  {%>
  <p><em><%=string.IsNullOrEmpty(absolution.Name) ? "Anonymous" : absolution.Name %></em>
  donated <%=absolution.AmountDonated.ToString("c") %> on <%=absolution.DateConfessed.ToString("hh:mm dd/MM/yyyy") %></p>
<%
  }%>

</asp:Content>
