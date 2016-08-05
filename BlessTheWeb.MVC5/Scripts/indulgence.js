$(document)
    .ready(function() {
        if ($('.congratulations').length > 0) {
            $('.congratulations').append('<audio id="hallelujah" preload autoplay><source src="/Content/10277-m-001.mp3" type="audio/mp3"><source src="/Content/10277-m-001.wav" type="audio/wav"></audio>');
            document.getElementById('hallelujah').play();
            $('#god_blessing').animate({ top: "20%" }, 25000);

            var w = $(window).innerWidth();
            var h = $(window).innerHeight();
            var numClouds = 15;
            for (var i = 0; i < numClouds; i++) {
                var top = -(i * i - (numClouds * i)) + 10;
                var left = 100 / numClouds * i;
                var imgname = (i % 3)+1;
                $('.congratulations').append($('<img src="/content/cloud'+imgname+'.png" />').addClass('cloud').css({ left: left+'%', top: '0%' }));

                $('.cloud').animate({ top: top+'%' }, 25000);
                var angle = '-30deg'
                $({ deg: 0 }).animate({ deg: angle }, {
                    duration: 25000,
                    step: function (now) {
                        $('.cloud').css({
                            transform: 'rotate(' + now + 'deg)'
                        });
                    }
                });

                //$('#blessing').delay(25000).fadeOut();
            }
        }
    });