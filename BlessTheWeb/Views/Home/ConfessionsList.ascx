<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<IEnumerable<BlessTheWeb.Core.Indulgence>>" %>

<%foreach(var confession in Model){ %>
    <p><%:confession.Confession %><br />
    Confessed <%=confession.DateConfessed.ToString("hh:mm dd-MM-yyyy") %>
    <%if (!confession.IsBlessed)
{%><%=Html.ActionLink("bless", "Bless", "Home", new {id = confession.Id.Substring(confession.Id.IndexOf("/")+1)}, null)%><%
}%>
    </p>

<%} %>
