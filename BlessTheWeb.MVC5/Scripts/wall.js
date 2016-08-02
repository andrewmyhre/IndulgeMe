$(document).ready(function () {
    $.getJSON('/Api/GetLatest',
    function (data) {
        for (var i = 0; i < data.length; i++) {
            $('#wall')
                .append('<div class="indulgence">' +
                '<a href="/indulgences/' + data[i].Id + '"><img src="/content/indulgences/' + data[i].Id + '/indulgence_25.png" /></a>' +
                '<p><a href="/indulgences/' + data[i].Id + '">' +
                data[i].Name+', ' + data[i].Date + '</a></p>'+
                '</div>');
        };
    });
});