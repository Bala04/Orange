angular.module("orangeApp").directive('input', ["$parse", function ($parse) {
    return {
        restrict: 'E',
        require: '?ngModel',
        link: function (scope, element, attrs) {
            if (attrs.ngModel) {
                var model = $parse(attrs.ngModel);
                var val = null;

                if (attrs.type == "checkbox")
                    val = attrs.checked != null;
                else
                    val = attrs.value || element.text()

                model.assign(scope, val);
            }
        }
    };
}]).directive('appFrame', [function () {
    return {
        restrict: 'A',
        link: function (scope, element, attrs) {
            $(element).load(function () {
                scope.frameLoaded(element[0].contentWindow.location.pathname.split('/')[1]);
            });
        }
    }
}]).filter('trustAsResourceUrl', ['$sce', function ($sce) {
    return function (val) {
        return $sce.trustAsResourceUrl(val);
    };
}]);

angular.module("base").directive('showWhen', [function () {
    return {
        restrict: 'A',
        scope: {
            showWhen: "=",
        },
        link: function (scope, element, attrs) {
            scope.$watch("showWhen", function (a) {
                if (a) {
                    $(element).delay(80).fadeIn(200);
                } else {
                    $(element).fadeOut(80);
                }
            });
        }
    }
}]).directive('disableWhen', [function () {
    return {
        restrict: 'A',
        scope: {
            disableWhen: "=",
        },
        link: function (scope, element, attrs) {
            scope.$watch("disableWhen", function (a) {
                if (a)
                    $(element).attr("disabled", "disabled");
                else
                    $(element).removeAttr("disabled");
            });
        }
    }
}]);