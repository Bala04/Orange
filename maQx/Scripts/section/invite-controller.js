angular.module("sectionApp").directive('input', ["$parse", function ($parse) {
    return {
        restrict: 'E',
        require: '?ngModel',
        link: function (scope, element, attrs) {
            if (attrs.ngModel) {
                var model = $parse(attrs.ngModel);
                var val = null;

                if (attrs.type == "checkbox")
                    val = attrs.checked != null;
                else if (attrs.type == "select") {

                }
                else
                    val = attrs.value || element.text()

                model.assign(scope, val);
            }
        }
    };
}]).config(['$httpProvider', function ($httpProvider) {
    $httpProvider.defaults.headers.common["X-Requested-With"] = 'XMLHttpRequest';
}]).controller("InviteController", ['$scope', '$timeout', '$http', function ($scope, $timeout, $http) {
    var self = this;

    self.username = "";
    self.email = "";
    self.organization = $("#Organization").data("init");
    self.usernameLoading = false;
    self.usernameLoaded = false;
    self.usernameStart = false;
    self.className = "glyphicon glyphicon form-control-feedback";

    self.setOrganizationState = function () {

    };

    self.typeStart = function (value) {

    };

    self.typeEnd = function (value) {
        self.usernameStart = true;
        self.usernameLoaded = false;

        if (value.trim() !== "") {
            self.usernameLoading = true;

            $http.get("/get/Exists/" + _app.escapeHtml(value) + "?type=username&ref=" + _app.escapeHtml(self.organization)).then(
                function (result) {
                    if (result.data.Type == "SUCCESS") {
                        self.usernameLoaded = !result.data.Value;
                    self.usernameLoading = false;
                },
                function (error) {
                    $rootScope.$broadcast("alert", { type: "Error", message: error });                    
                    self.usernameLoaded = false;
                    self.usernameLoading = false;
                });
        }
    };

    self.organizationState = function () {
        var a = $("#Organization").val();
        return a == null || a.trim() === "" || a == "-1";
    };

    self.formState = function () {
        return !(!self.organizationState() && self.username.trim() !== "" && self.usernameLoaded)
    };

    self.usernameState = function () {
        return self.usernameStart;
    };

    self.getUsernameStateClass = function () {
        if (self.usernameState() && !self.usernameLoading) {
            if (self.usernameLoaded && self.username.trim() !== "") {
                return self.className + " glyphicon-ok";
            } else {
                return self.className + " glyphicon-remove";
            }
        }

        return self.className;
    };




}])