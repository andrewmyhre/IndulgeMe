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
            $('#charities')
                .append($('<div class="col-xs-12 col-md-12 charity-card" id="' + data[i].Id + '"></div>')
                    .append($('<a href="#" data-charityid="'+data[i].Id+'" data-charityname="'+data[i].Name+'"></a>')
                        .click(function () {
                            choose($(this).parent().attr('id'), $(this).attr('data-charityname'));
                        })
                        .append($('<object data="/content/notfound.png" type="image/png"><img src="' + imageUrl + '" alt="' + data[i].Name + '" title="' + data[i].Name + '" /></object><p>' + data[i].Name + '</p><small><em>'+data[i].Description+'</em></small></div>')))
                );
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

function choose(charityid, charityName) {
    $('#charityid').val(charityid);
    $('#charityname').val(charityName);
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