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
    clone: function (value) {
        return JSON.parse(JSON.stringify(value));
    },
    logOff: function () {
        document.getElementById('logoutForm').submit();
    },
    enableTooltip: function () {
        $('[data-toggle="tooltip"]').tooltip('destroy').tooltip({
            container: 'body'
        });
    },
    tableCopied: function (rows) {
        $(".copyInfo").text(rows == 0 ? "No rows to copy" : ((rows > 0 ? rows == 1 ? "One row" : rows + " rows " : "") + "copied to clipboard."));
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
        return text.replace(/[&<>"']/g, function (m) {
            return map[m];
        });
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
angular.module("base", ['angular-loading-bar', 'ngResource', 'ngAnimate']);
angular.module("orangeApp", ['base']);
angular.module("tabularApp", ['base', 'datatables']);
angular.module("sectionApp", ['jquery-typing', 'tabularApp', 'ngAnimate', 'ui.bootstrap']);
angular.module("sectionApp").filter('select', function () {
    return function (items, selectedID, track, value) {
        return (selectedID == value) ? items : linq(items).where("$." + track + "=='" + selectedID + "'").toArray();
    }
});
angular.module("tabularApp").factory('DTLoadingTemplate', function () {
    return {
        html: '<div class="circle-loader css-fade"><div class="loader-animate"></div><div><h3>Loading..</h3></div></div>'
    };
});
angular.module("orangeApp").config(['$httpProvider', 'cfpLoadingBarProvider', function ($httpProvider, cfpLoadingBarProvider) {
    _app.configModule($httpProvider, cfpLoadingBarProvider);
}]);
angular.module("tabularApp").config(['$httpProvider', 'cfpLoadingBarProvider', function ($httpProvider, cfpLoadingBarProvider) {
    _app.configModule($httpProvider, cfpLoadingBarProvider);
}]);
angular.module("sectionApp").factory('Injector', ['$rootScope', '$q', function ($rootScope, $q) {
    var Injector = {
        request: function (config) {          
            return config || $q.when(config);
        },
        requestError: function (rejection) {           
            return $q.reject(rejection);
        },
        response: function (response) {
            return response || $q.when(response);
        },
        responseError: function (error) {
            $rootScope.$broadcast("alert", {
                type: "Error",
                message: error
            });
            return $q.reject(error);;
        }
    };
    return Injector;
}]);
angular.module("sectionApp").config(['$httpProvider', function ($httpProvider) {
    $httpProvider.interceptors.push('Injector');
}]);
function linq(object) {
    return Enumerable.from(object);
}
// jQuery-typing
//
// Version: 0.2.0
// Website: http://narf.pl/jquery-typing/
// License: public domain <http://unlicense.org/>
// Author:  Maciej Konieczny <hello@narf.pl>
(function (e) { function t(t, n) { function r(e) { if (!u) { u = true; s.start && s.start(e, o) } } function i(e, t) { if (u) { clearTimeout(a); a = setTimeout(function () { u = false; s.stop && s.stop(e, o) }, t >= 0 ? t : s.delay) } } var s = e.extend({ start: null, stop: null, delay: 400 }, n), o = e(t), u = false, a; o.keypress(r); o.keydown(function (e) { if (e.keyCode === 8 || e.keyCode === 46) r(e) }); o.keyup(i); o.blur(function (e) { i(e, 0) }) } e.fn.typing = function (e) { return this.each(function (n, r) { t(r, e) }) } })(jQuery);
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
                    start(scope, {
                        value: $elem.val()
                    });
                },
                stop: function (event, $elem) {
                    stop(scope, {
                        value: $elem.val()
                    });
                },
                delay: attr.typing ? parseInt(attr.typing) : 400
            });
        }
    }
}]);