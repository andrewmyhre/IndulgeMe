<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage" MasterPageFile="~/Views/Shared/Site.Master" %>

<asp:Content runat="server" ContentPlaceHolderID="Scripts">
<meta http-equiv="refresh" content="2;url=<%=ViewData["exiturl"] %>">
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="MainContent">
<p><strong>This is where you would beging the JustGiving donation process and make your donation</strong></p>

<p>This website is currently in test mode so we'll just make up a random amount and say you donated it.</p>

<p>In a moment you'll see your indulgence and if you provided your email address a copy will be sent to your inbox.</p>

<p><em>Redirecting you in 2 seconds to <a href="<%=ViewData["exiturl"] %>"><%=ViewData["exiturl"] %></a></em></p>
</asp:Content>