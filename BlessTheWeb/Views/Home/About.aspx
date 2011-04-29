<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="aboutTitle" ContentPlaceHolderID="TitleContent" runat="server">About - <%=System.Configuration.ConfigurationManager.AppSettings["SiteTitle"]%></asp:Content>

<asp:Content ID="aboutContent" ContentPlaceHolderID="MainContent" runat="server">
    <h2>About Indulge Me</h2>
    <p>According to the teachings of the Catholic church</p>
</asp:Content>
