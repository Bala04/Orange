angular.module("orangeApp").directive("input", ["$parse", function ($parse) {
    return {
        restrict: "E",
        require: "?ngModel",
        link: function (scope, element, attrs) {
            if (attrs.ngModel) {
                var model = $parse(attrs.ngModel), val = null;
                if (attrs.type === "checkbox") {
                    val = attrs.checked !== null;
                } else {
                    val = attrs.value || element.text();
                }

                model.assign(scope, val);
            }
        }
    };
}]).directive("appFrame", [function () {
    return {
        restrict: "A",
        link: function (scope, element) {
            $(element).load(function () {
                scope.frameLoaded(element[0].contentWindow.location.pathname);
            });
        }
    };
}]).filter("trustAsResourceUrl", ["$sce", function ($sce) {
    return function (val) {
        return $sce.trustAsResourceUrl(val);
    };
}]);

angular.module("sectionApp").directive("indeterminateCheckbox", [function () {
    return {
        restrict: "A",
        scope: {
            indeterminate: "=indeterminateCheckbox",
        },
        link: function (scope, element) {
            scope.$watch("indeterminate", function (newValue) {
                if (newValue) {
                    element.prop("indeterminate", true);
                } else {
                    element.prop("indeterminate", false);
                }
            }, true);
        }
    };
}]).directive("anchorUrl", [function () {
    return {
        restrict: "A",
        scope: {
            anchorUrl: "&",
            anchorIf: "="
        },
        link: function (scope, element) {
            var url = scope.anchorUrl();
            scope.$watch('anchorIf', function (a) {
                if (a) {
                    $(element).prop("href", url).removeClass("btn-disabled");
                } else {
                    $(element).removeAttr("href").addClass("btn-disabled");
                }
            });
        }
    }
}]).directive("returnJson", [function () {
    return {
        restrict: "A",
        scope: {
            returnJson: "&"
        },
        link: function (scope, element) {
            var elements = [];
            $(element).find("option").map(function () {
                var val = $(this).val();
                if (val != "0" && val != "-1")
                    elements.push({ value: val, text: $(this).text() });
            });

            scope.returnJson({ value: elements });
        }
    };
}]);

angular.module("base").directive("showWhen", [function () {
    return {
        restrict: "A",
        scope: {
            showWhen: "=",
        },
        link: function (scope, element) {
            scope.$watch("showWhen", function (a) {
                if (a) {
                    $(element).delay(80).fadeIn(200);
                } else {
                    $(element).fadeOut(80);
                }
            });
        }
    };
}]).directive("disableWhen", [function () {
    return {
        restrict: "A",
        scope: {
            disableWhen: "=",
        },
        link: function (scope, element) {
            scope.$watch("disableWhen", function (a) {
                if (a) {
                    $(element).attr("disabled", "disabled");
                } else {
                    $(element).removeAttr("disabled");
                }
            });
        }
    };
}]);