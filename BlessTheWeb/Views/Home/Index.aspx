<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<BlessTheWeb.Models.HomeViewModel>" %>
<%@ Import Namespace="BlessTheWeb.Core.Extensions" %>

<asp:Content runat="server" ContentPlaceHolderID="Scripts">
<script type="text/javascript" src="http://ajax.googleapis.com/ajax/libs/jquery/1.4.2/jquery.min.js"></script>
<script type="text/javascript" src="/scripts/home.js"></script>
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server"><%=System.Configuration.ConfigurationManager.AppSettings["SiteTitle"]%></asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
<%if (ViewData["ShowBlessing"] != null && (bool)ViewData["ShowBlessing"] == true)
  {%>
  <h2 class="congratulations">Congratulations! Consider yourself blessed.</h2>
<EMBED src="/10277-m-001.wav" autostart=true loop=false volume=100 
hidden=true><NOEMBED><BGSOUND src="/10277-m-001.wav"></NOEMBED>
<%
  }%>
<div id="siteInfo">
    <p><em><%=Model.SiteInfo.TotalDonated.ToString("c") %></em> donated to charity, <em><%=Model.SiteInfo.TotalAbsolvedSins %></em> indulgences granted</p>
</div>

<div>
    <img src="/content/123.png" />
</div>

<div id="confess">
<h2>Confess to a sin</h2>
<form method="post">
<input type="text" name="confession" id="confessbox" class="watermarkOn" maxlength="150" value="Your confession..." /> <input type="submit" value="Repent!" id="repentbutton" /></>
</form>
</div>

<div id="recently">
<h2>Recently Blessed</h2>
<ul>

</ul>
</div>

<div style="clear:both;"></div>
<div style="background-color:transparent;height:160px"></div>


</asp:Content>
