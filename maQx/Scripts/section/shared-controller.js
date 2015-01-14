angular.module("sectionApp").controller("AlertController", ['$rootScope', function ($rootScope) {
    var self = this;

    self.type = "";
    self.message = "";

    self.show = false;

    self.isShow = function () {
        return self.show;
    };

    self.close = function () {
        self.show = false;
    };

    self.alertType = function () {
        if (self.type == "Warning")
            return "alert-warning";
        else if (self.type == "Success")
            return "alert-success";
        else if (self.type == "Error")
            return "alert-danger";
        else if (self.type == "Note")
            return "alert-info";
    };

    $rootScope.$on("alert", function (e, data) {
        self.type = data.type;
        self.message = data.message;
        self.show = true;
    });
}]);