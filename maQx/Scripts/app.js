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
        $(".copyInfo").text(rows === 0 ? "No rows to copy" : ((rows > 0 ? rows === 1 ? "One row" : rows + " rows " : "") + "copied to clipboard."));
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
        return (selectedID === value) ? items : linq(items).where("$." + track + "=='" + selectedID + "'").toArray();
    };
});
angular.module("tabularApp").factory('DTLoadingTemplate', function () {
    return {
        html: '<div class="circle-loader css-fade"><div class="loader-animate"></div><div><h3>Loading..</h3></div></div>'
    };
});
angular.module("orangeApp").config(['$httpProvider', 'cfpLoadingBarProvider', function ($httpProvider, cfpLoadingBarProvider) {
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
            if (response.config.cached) {
                return response || $q.when(response);
            } else if (response.config.url.indexOf("Context") > 0) {
                return response || $q.when(response);
            } else {
                if (response.data.Type != "SUCCESS") {
                    $rootScope.$broadcast("alert", {
                        type: "Error",
                        message: response.data.Message,
                    });
                    return $q.reject(response);
                }
                return response || $q.when(response);
            }
        },
        responseError: function (error) {
            $rootScope.$broadcast("alert", {
                type: "Error",
                message: error
            });
            return $q.reject(error);
        }
    };
    return Injector;
}]);
angular.module("tabularApp").config(['$httpProvider', 'cfpLoadingBarProvider', function ($httpProvider, cfpLoadingBarProvider) {
    _app.configModule($httpProvider, cfpLoadingBarProvider);
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
!function (n) { function t(t, e) { function o(n) { r || (r = !0, c.start && c.start(n, f)) } function u(n, t) { r && (clearTimeout(i), i = setTimeout(function () { r = !1, c.stop && c.stop(n, f) }, t >= 0 ? t : c.delay)) } var i, c = n.extend({ start: null, stop: null, delay: 400 }, e), f = n(t), r = !1; f.keypress(o), f.keydown(function (n) { (8 === n.keyCode || 46 === n.keyCode) && o(n) }), f.keyup(u), f.blur(function (n) { u(n, 0) }) } n.fn.typing = function (n) { return this.each(function (e, o) { t(o, n) }) } }(jQuery);
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
    };
}]);