<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<BlessTheWeb.Models.ShareViewModel>" %>

<div id="scripts"></div>
<script type="text/javascript">
    (function () {
        var s = document.createElement('SCRIPT'), s1 = document.getElementById('scripts');
        s.type = 'text/javascript';
        s.async = true;
        s.src = 'http://widgets.digg.com/buttons.js';
        s1.parentNode.insertBefore(s, s1);

        s = document.createElement('SCRIPT')
        s.type = 'text/javascript';
        s.async = true;
        s.src = 'http://connect.facebook.net/en_US/all.js#xfbml=1';
        s1.parentNode.insertBefore(s, s1);

        s = document.createElement('SCRIPT')
        s.type = 'text/javascript';
        s.async = true;
        s.src = 'http://platform.twitter.com/widgets.js';
        s1.parentNode.insertBefore(s, s1);

    })();
</script>

<fb:like show_faces="true" width="450"></fb:like>

<a href="http://twitter.com/share" class="twitter-share-button" data-text="<%:Model.Tweet %> #blesstheweb" data-count="horizontal" data-via="blesstheweb">Tweet</a>

<a href="http://www.reddit.com/submit" onclick="window.location = 'http://www.reddit.com/submit?url=' + encodeURIComponent(window.location); return false"> <img src="http://www.reddit.com/static/spreddit7.gif" alt="submit to reddit" border="0" /> </a>

<!-- Compact Button -->
<a class="DiggThisButton DiggCompact"></a>
