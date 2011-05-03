<%@ Page Title="" Language="C#" Inherits="System.Web.Mvc.ViewPage<BlessTheWeb.Models.IndulgencesViewModel>" MasterPageFile="~/Views/Shared/Site.Master" %>
<%@ Import Namespace="BlessTheWeb.Core.Extensions" %>
<%@ Import Namespace="BlessTheWeb.Models" %>
<asp:Content runat="server" ID="Title" ContentPlaceHolderID="TitleContent"><%=Model.Indulgence.Confession %> - <%=System.Configuration.ConfigurationManager.AppSettings["SiteTitle"]%></asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="Scripts">
<script type="text/javascript" src="http://ajax.googleapis.com/ajax/libs/jquery/1.4.2/jquery.min.js"></script>
</asp:Content>

<asp:Content runat="server" ID="Main" ContentPlaceHolderID="MainContent">

<div id="indulgence-view">
<%if (ViewData["ShowBlessing"] != null && (bool)ViewData["ShowBlessing"] == true)
  {%>
  <h2 class="congratulations">Congratulations! Consider yourself blessed.</h2>

<script type="text/javascript">    $('.congratulations').delay(5000).slideUp();</script>
<%
  }%>

  <div class="aa">
<%Html.RenderPartial("ShareThis", new ShareViewModel{Tweet=Model.Indulgence.Confession}); %>
<div class="indulgence">
<img src="/content/indulgences/<%=Model.Indulgence.Id.IdValue() %>/indulgence_100.png" alt="<%:Model.ImageAlt %>" />
</div>

<div style="clear:both" />
<p><a href="/indulgences/list">View more indulgences</a></p>
<p><a href="/">God hates your sin. Confess now!</a></p>
</div>
</div>

</asp:Content>
