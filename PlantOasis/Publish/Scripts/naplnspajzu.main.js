$(document).ready(function () {
    // Contact
    plantoasisContactFormApi();
    // Menu bar
    plantoasisMenuScrollTrigger();
    // Cookies
    if ($.cookie("plantoasis_cookies_ok") != '1') {
        $('.cookies-div').show();
    }
});

/* contact */
function plantoasisContactFormApi() {
    if ($('.api-password-group').length > 0) {
        $.ajax('/Umbraco/PlantOasis/PlantOasisApi/ContactFormApiKey',
            {
                type: 'POST',
                success: function (data) {
                    $('.api-password-group #Password').val(data.MainKey);
                    $('.api-password-group #ConfirmPassword').val(data.SubKey);
                }
            });
    }
}

/* Menu bar */
function plantoasisMenuScrollTrigger() {
    $(window).scroll(function () {
        if ($(this).scrollTop() > 200) {
            $('.menu-bar').addClass('fixed');
        }
        if ($(this).scrollTop() < 10) {
            $('.menu-bar').removeClass('fixed');
        }
    });
}

/* cookies */
function plantoasisCookiesOk() {
    $.cookie("plantoasis_cookies_ok", '1', { expires: 40000, path: '/' });
    plantoasisCookiesClose();
}
function plantoasisCookiesClose() {
    $('.cookies-div').hide();
}

/* slider */
function plantoasisSlider() {
    var slideCount = $('.slider ul li').length;
    var slideWidth = $('.slider ul li').width();
    var slideHeight = $('.slider ul li').height();
    var sliderUlWidth = slideCount * slideWidth;


    if (slideCount > 1) {
        $('.slider').css({ width: slideWidth, height: slideHeight });
        $('.slider ul li').css({ width: slideWidth });
        $('.slider ul').css({ width: sliderUlWidth, marginLeft: - slideWidth });
        $('.slider ul li:last-child').prependTo('.slider ul');
    }
    else {
        $('.slider a.control_prev').addClass('hidden');
        $('.slider a.control_next').addClass('hidden');
    }

    function moveLeft() {
        $('.slider ul').animate({
            left: + slideWidth
        }, 200, function () {
            $('.slider ul li:last-child').prependTo('.slider ul');
            $('.slider ul').css('left', '');
        });
    };

    function moveRight() {
        $('.slider ul').animate({
            left: - slideWidth
        }, 200, function () {
            $('.slider ul li:first-child').appendTo('.slider ul');
            $('.slider ul').css('left', '');
        });
    };

    $('.slider a.control_prev').click(function () {
        moveLeft();
        return false;
    });

    $('.slider a.control_next').click(function () {
        moveRight();
        return false;
    });
}