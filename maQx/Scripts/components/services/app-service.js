angular.module("orangeApp").service("AppService", [function () {
    var self = this;

    self.appVersion = "2.0.1";

    self.defaultTitles = [
        { path: "/", title: "Management" },
        { path: "/App", title: "Management" },
        { path: "/App/Index", title: "Management" },
        { path: "/App/Init", title: "Administration" },
        { path: "/App/Register", title: "Administration" },
        { path: "/Login", title: "Login" },
        { path: "/App/Login", title: "Login" },
    ];

    self.getPageTitle = function (path) {
        var path = linq(self.defaultTitles).where("$.path == '" + path + "'").toArray()[0];
        return path != null ? path.title : "";
    };

    self.getAppVersion = function () {
        return self.appVersion;
    };

    function linq(object) {
        return Enumerable.from(object);
    }
}]);