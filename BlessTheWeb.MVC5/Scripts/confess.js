﻿$(document).ready(function () {
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
            var imageUrl = 'http://www.justgiving.com' + data[i].LogoFileName;
            imageUrl='https://images.justgiving.com/image/23cc9334-f803-43cb-897a-ace65b7d98d7.PNG?template=size200x200';
            $('#charities')
                .append($('<div class="col-xs-12 col-md-3 charity-card" id="' + data[i].Id + '"></div>')
                    .append($('<a href="#" data-charityid="'+data[i].Id+'" data-charityname="'+data[i].Name+'"></a>')
                        .click(function () {
                            choose($(this).parent().attr('id'), $(this).attr('data-charityname'));
                        })
                        .append($('<img src="'+imageUrl+'" alt="' + data[i].Name + '" title="' + data[i].Name + '" /><p>'+data[i].Name+'</div>')))
                );
        }
        $('#charities').slideDown();
        $('#search').slideDown();
        $('#p').slideUp();
    });

    return false;
}

function choose(charityid, charityName) {
    $('#charityid').val(charityid);
    $('#charityname').val(charityName);
    $('#chosen').html('Chosen charity: ' + charityName);
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