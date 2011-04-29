$(document).ready(function () {
    $('#charities').hide();
    findCharities($('#con').val());


    $('#find').click(function () {
        $('#charities').slideUp();
        $('#p').slideDown();

        findCharities($('#query').val());
    });



});



function findCharities(q) {

    ShowProgress();
    $.getJSON('/Api/FindCharities?q=' + escape(q),
    function (data) {
        $('#charities').empty();
        for (var i = 0; i < data.length; i++) {
            $('#charities')
                .append($('<li id="' + data[i].CharityId + '"></li>')
                    .append($('<div class="cl"></div>')
                        .append($('<a href="#"><img src="http://www.justgiving.com' + data[i].LogoFileName + '"'
                                    + ' alt="' + data[i].Name + '" title="' + data[i].Name + '" /></a>')
                                .click(function () {
                                    choose($(this).parent().parent().attr('id'), $(this).parent().siblings().text(), $(this).html());
                                })
                        )
                    ).append($('<div class="cn"></div>')
                    .append($('<a href="#">' + data[i].Name + '</a>')
                                .click(function () {
                                    choose($(this).parent().parent().attr('id'), $(this).text(), $(this).html());
                                })
                        )
                    )
                );
        }
        $('#charities').slideDown();
        $('#search').slideDown();
        $('#p').slideUp();
    });

    return false;
}

function choose(charityid, charityName, charityHtml) {
    $('#charityid').val(charityid);
    $('#charityname').val(charityName);
    $('#chosen').html('Chosen charity: ' + charityHtml);
    $('#doredeem').attr('disabled', '');
    $('#charitysearch').slideUp();
    $('#id').slideDown();
}

function ShowProgress() {
    $('#p').html('<li><em>Finding charities...</em></li>');
    $('#p').slideDown();
    //$('#search').slideUp();
    $('#charities').slideUp();
}