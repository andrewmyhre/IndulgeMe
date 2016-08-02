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
    $.getJSON('/api/charities?q=' + escape(q),
    function (data) {
        $('#charities').empty();
        for (var i = 0; i < data.length; i++) {
            $('#charities')
                .append($('<li id="' + data[i].Id + '"></li>')
                    .append($('<a href="#"></a>')
                        .click(function () {
                            choose($(this).parent().attr('id'), $(this).children('div').children('div.cn').text(), $(this).children('div').children('div.cn').html());
                        })
                        .append($('<div class="row"></div>')
                    .append($('<div class="cl"></div>')
                        .append($('<img src="http://www.justgiving.com' + data[i].LogoFileName + '"'
                                    + ' alt="' + data[i].Name + '" title="' + data[i].Name + '" />')
                        )
                    ).append($('<div class="cn">'+data[i].Name+'</div>')))
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
    $('#doredeem').prop('disabled', false);
    $('#charitysearch').slideUp();
    $('#id').removeClass('hidden');
    $('#id').slideDown();
}

function ShowProgress() {
    $('#p').html('<em>Finding charities...</em><br><img src="/Content/ajax-loader.gif" />');
    $('#p').slideDown();
    //$('#search').slideUp();
    $('#charities').slideUp();
}