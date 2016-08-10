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
            var imageUrl = 'http://images.justgiving.com/image/'+data[i].Logo + '?template=size200x200';
            var link = $('<a href="#" data-charityid="' + data[i].Id + '" data-charityname="' + data[i].Name + '" data-charitylogo="' + imageUrl + '"><h5>' + data[i].Name + '</h5><p>' + data[i].Description + '</p></a>')
                .click(function () {
                    choose($(this).parent().attr('id'), $(this).attr('data-charityname'),$(this).attr('data-charitylogo'));
                });
            var image = $('<a href="#" data-charityid="' + data[i].Id + '" data-charityname="' + data[i].Name + '" data-charitylogo="'+imageUrl+'"></a>')
                .click(function () { choose($(this).parent().attr('id'), $(this).attr('data-charityname'), $(this).attr('data-charitylogo')); })
                .append($('<img class="cc_image" src="data:image/gif;base64,R0lGODlhAQABAIAAAAAAAP///yH5BAEAAAAALAAAAAABAAEAAAIBRAA7" style="background-image: url(\'' + imageUrl + '\')" alt="' + data[i].Name + '" title="' + data[i].Name + '" />'));
            var col1 = $('<div class="col-xs-12 col-md-4"></div>').append($('<div class="cc_image_container fallback"></div>').append(image));
            var col2 = $('<div class="col-xs-12 col-md-8"></div>').append(link);
            $('#charities')
                .append($('<div class="well row charity-card"></div>').append(col1).append(col2));
        }

        if ($('#charityid').val() == '') {
            $('#charities').slideDown();
            $('#search').slideDown();
            $('#p').slideUp();
        } else {
            $('#charities').slideUp();
            $('#search').slideUp();
            $('#p').slideDown();
            $('#id').removeClass('hidden');
            $('#id').slideDown();
            $('#charitysearch').slideUp();
            $('#donating-to').html($('#charityname').val());
            $('#chosen').removeClass('hidden');
            $('#chosen').show();
            $('#doredeem').prop('disabled', false);
        }
    });

    $('#id form').submit(function() {
        if ($('#email').val() == '') {
            $('#email-required-alert').removeClass('hidden');
            $('#email-required-alert').show();
            return false;
        }
    })

    return false;
}

function choose(charityid, charityName, charityLogo) {
    $('#charityid').val(charityid);
    $('#charityname').val(charityName);
    $('#charity-logo').prop('src', charityLogo);
    $('#donating-to').html(charityName);
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