﻿<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<BlessTheWeb.Core.Indulgence>" %>

<asp:Content runat="server" ContentPlaceHolderID="Scripts">
    <script type="text/javascript" src="http://ajax.googleapis.com/ajax/libs/jquery/1.4.2/jquery.min.js"></script>
    <script type="text/javascript" src="/Scripts/confess.js"></script>
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">Choose a Charity - <%=System.Configuration.ConfigurationManager.AppSettings["SiteTitle"]%>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
<div id="select-charity">
    <h1>You want to repent for the sin of<br /> <em id="confession">"<%=Model.Confession %>"</em></h1>
    <h2><%= ViewData["AbsolutionMessage"]%></h2>
    <p>Select a charity:</p>
    <label id="chosen"></label>
    <input type="hidden" id="con" value="<%=Model.Confession %>" />
    <input type="hidden" id="cid" value="<%=ViewData["cid"] %>" />
<div id="charitysearch">
        <p id="p"><img src="/Content/ajax-loader.gif" /></p>
        <ul id="charities">
        
        </ul>

    <form id="search" action="#" onsubmit="return findCharities($('#query').val());">
    <p>or find a charity:<input type="text" name="query" id="query" /> <input type="button" id="find" value="Search" class="button" /></p>    
    </form>
    </div>

    <div id="id" class="hidden">
    <h2>Make yourself known?</h2>
    <form action="/Home/SelectCharity" method="post">
        <input type="hidden" id="absolutionid" name="absolutionid" value="<%=ViewData["cid"] %>" />
        <input type="hidden" id="charityid" name="charityid" />
        <input type="hidden" id="charityname" name="charityname" />

        <p>You can remain anonymous if you wish, but can you truly repent if you hide your identity?</p>
        <label for="name">Your name (optional):</label> <input type="text" id="name" name="name" />

        <p>If you would like a copy of your indulgence in PDF format provide your email address here. We won't share it with anyone.</p>
        <label for="email">Email (optional):</label> <input type="text" id="email" name="email" />
        
       <p>After you click the 'Redeem' button you will be redirected to the JustGiving website where you can make your donation. Once your donation is processed you'll be returned to this site.</p>
        <p><input type="submit" id="doredeem" value="Redeem yourself!" disabled="disabled" /></p>
    </form>

</div>
</div>
</asp:Content>
