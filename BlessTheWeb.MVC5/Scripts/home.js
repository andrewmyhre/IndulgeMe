$(document).ready(function () {
    $.getJSON('/api/indulgenceapi/getlatest',
    function (data) {
        for (var i = 0; i < data.length; i++) {
            $('#indulgence-wall > #indulgences')
                .append('<div class="indulgence">'+
                    '<a href="/indulgence/' + data[i].Guid + '"><img src="' + data[i].ThumbnailUrl + '" /></a>' +
                    '<p><a href="/indulgence/' + data[i].Guid + '">' + data[i].Date + '</a></p>' +
                    '</div>');
            }
        if (data.length > 0) {
            $('#indulgence-wall').removeClass('hidden');
            $('#indulgence-wall').slideDown();
        }
    });

    $("#confessbox").focus(function () {

        $(this).filter(function () {

            // We only want this to apply if there's not 
            // something actually entered
            return $(this).val() == "" || $(this).val() == "Your confession..."

        }).removeClass("watermarkOn").val("");

    });

    // Define what happens when the textbox loses focus
    // Add the watermark class and default text
    $("#confessbox").blur(function () {

        $(this).filter(function () {

            // We only want this to apply if there's not
            // something actually entered
            return $(this).val() == ""

        }).addClass("watermarkOn").val("Your confession...");

    });
});