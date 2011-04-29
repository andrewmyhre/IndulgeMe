$(document).ready(function () {
    $.getJSON('/Api/GetLatest',
    function (data) {
        for (var i = 0; i < data.length; i++) {
            $('#recently > ul')
                .append('<li>'+
                '<p><a href="/indulgences/' + data[i].Id + '">' + data[i].Confession + '</a></p>' +
                '<p>'+
                    '<a href="/indulgences/'+data[i].Id+'">#'+data[i].Id+'</a>' +
                    '<span><strong>' + data[i].Name + '</strong>' +
                    ' donated <strong>' + data[i].AmountDonated + '</strong> to <strong>'+data[i].CharityName+'</strong>'+
                    '<span> on ' + data[i].Date + '</span>' +
                    
                '</p>'+
                '</li>');
        };
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