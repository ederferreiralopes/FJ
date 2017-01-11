Notify = function (text, callback, close_callback, style) {

    $('body').append('<div id="notificacoes"></div>');
    var time = '6000';
    var $container = $('#notificacoes');

    if (typeof style == 'undefined') style = 'warning'

    var icon = '<i class="fa fa-exclamation-triangle "></i>';

    switch (style) {
        case 'success': icon = '<i class="fa fa-check"></i>';
            break;
        case 'danger': icon = '<i class="fa fa-exclamation-circle "></i>';
            break;
        case 'info': icon = '<i class="fa fa-info-circle "></i>';
            break;
    }

    var html = $('<div class="alert alert-' + style + '  hide">' + icon + " " + text + '</div>');

    $('<a>', {
        text: '×',
        class: 'button close',
        style: 'padding-left: 10px;',
        href: '#',
        click: function (e) {
            e.preventDefault()
            close_callback && close_callback()
            remove_notice()
        }
    }).prependTo(html)

    $container.prepend(html)
    html.removeClass('hide').hide().fadeIn('slow')

    function remove_notice() {
        html.stop().fadeOut('slow').remove()
    }

    var timer = setInterval(remove_notice, time);

    $(html).hover(function () {
        clearInterval(timer);
    }, function () {
        timer = setInterval(remove_notice, time);
    });

    html.on('click', function () {
        clearInterval(timer)
        callback && callback()
        remove_notice()
    });


}
