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