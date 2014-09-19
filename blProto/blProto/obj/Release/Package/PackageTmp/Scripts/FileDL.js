$('#downloadfile').click(function (url) {
    downloadfile(url);
});

var downloadfile = function () {
    for (var i = 0; i < arguments.length; i++) {
        var iframe = $('<iframe style="visibility: collapse;"></iframe>');
        $('body').append(iframe);
        var content = iframe[0].contentDocument;
        var form = '<form action="' + arguments[i] + '" method="GET"></form>';
        content.write(form);
        $('form', content).submit();
        setTimeout((function (iframe) {
            return function () {
                iframe.remove();
            }
        })(iframe), 2000);
    }
}