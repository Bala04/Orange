// maQx-utilities
//
// Version: 2.0.1
// Author:  Prasanth <@prashanth702>
var _app = _app || {};
_app = {
    _: function () {
        this.enableTooltip();
        return this;
    },
    enableTooltip: function () {
        $('[data-toggle="tooltip"]').tooltip('destroy').tooltip({ container: 'body' });
    },
    tableCopied: function (rows) {
        $(".copyInfo").text((rows > 0 ? rows == 1 ? "One row" : rows + " rows " : "") + "copied to clipboard.");

        setTimeout(function () {
            $(".copyInfo").text("");
        }, 3000);
    },
    escapeHtml: function (text) {
        var map = {
            '&': '&amp;',
            '<': '&lt;',
            '>': '&gt;',
            '"': '&quot;',
            "'": '&#039;'
        };
        return text.replace(/[&<>"']/g, function (m) { return map[m]; });
    },
    configModule: function ($httpProvider, cfpLoadingBarProvider) {
        $httpProvider.defaults.headers.common["X-Requested-With"] = 'XMLHttpRequest';
        cfpLoadingBarProvider.includeSpinner = false;
    }
}._();

// maQx-application
//
// Version: 2.0.1
// Author:  Prasanth <@prashanth702>
angular.module("dependencies", ['angular-loading-bar', 'ngResource', 'ngAnimate']);
angular.module("orangeApp", ['dependencies']);
angular.module("tabularApp", ['dependencies', 'datatables']);

angular.module("orangeApp").config(['$httpProvider', 'cfpLoadingBarProvider', function ($httpProvider, cfpLoadingBarProvider) {
    _app.configModule($httpProvider, cfpLoadingBarProvider);
}]);

angular.module("tabularApp").config(['$httpProvider', 'cfpLoadingBarProvider', function ($httpProvider, cfpLoadingBarProvider) {
    _app.configModule($httpProvider, cfpLoadingBarProvider);
}]);

// jQuery-typing
//
// Version: 0.2.0
// Website: http://narf.pl/jquery-typing/
// License: public domain <http://unlicense.org/>
// Author:  Maciej Konieczny <hello@narf.pl>
(function (f) { function l(g, h) { function d(a) { if (!e) { e = true; c.start && c.start(a, b) } } function i(a, j) { if (e) { clearTimeout(k); k = setTimeout(function () { e = false; c.stop && c.stop(a, b) }, j >= 0 ? j : c.delay) } } var c = f.extend({ start: null, stop: null, delay: 400 }, h), b = f(g), e = false, k; b.keypress(d); b.keydown(function (a) { if (a.keyCode === 8 || a.keyCode === 46) d(a) }); b.keyup(i); b.blur(function (a) { i(a, 0) }) } f.fn.typing = function (g) { return this.each(function (h, d) { l(d, g) }) } })(jQuery);

// AngularTyping
//
// Version: 0.1.0
// Github:
// License: public domain <http://unlicense.org/>
// Author: Prasanth <@prashanth702>
angular.module("jquery-typing", []).directive("typing", ['$parse', function ($parse) {
    return {
        link: function (scope, element, attr) {
            var start = $parse(attr.typeStart);
            var stop = $parse(attr.typeEnd);

            $(element).typing({
                start: function (event, $elem) {
                    start(scope, { value: $elem.val() });
                },
                stop: function (event, $elem) {
                    stop(scope, { value: $elem.val() });
                },
                delay: attr.typing ? parseInt(attr.typing) : 400
            });
        }
    }
}]);





