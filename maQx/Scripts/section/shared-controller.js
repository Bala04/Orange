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
            return "display-override alert-warning";
        else if (self.type == "Success")
            return "display-override alert-success";
        else if (self.type == "Error")
            return "display-override alert-danger";
        else if (self.type == "Note")
            return "display-override alert-info";
    };

    self.getIcon = function () {
        if (self.type == "Warning")
            return "fa-exclamation-triangle";
        else if (self.type == "Success")
            return "fa-check";
        else if (self.type == "Error")
            return "fa-times";
        else if (self.type == "Note")
            return "fa-exclamation";
    };

    $rootScope.$on("alert", function (e, data) {

        console.log(data);

        self.type = data.type;
        if (data.message.status !== undefined) {  
            self.message = data.message.statusText != "" ? data.message.statusText : "Opps! something went wrong."
        } else {
            self.message = data.message;
        }
        self.show = true;
    });
}]);