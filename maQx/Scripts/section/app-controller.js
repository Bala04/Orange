angular.module("orangeApp")
    .controller("AppController", ["$rootScope", "$scope", "$location", "$window", "AppService", function ($rootScope, $scope, $location, $window, AppService) {
        var self = this;

        self.title = "maQx - the leading MI solutions from IP Rings";
        self.pageTitle = AppService.getPageTitle($window.location.pathname);
        self.appVersion = AppService.getAppVersion();

        self.getPageTitle = function () {
            return self.pageTitle;
        };

        self.getTitle = function () {
            return self.title;
        };

        $scope.$on("AuthenticationFailed", function (event, data) {
            $window.location.href = data.ReturnUrl + "?ReturnUrl=" + $window.location.pathname;
        });

        $rootScope.$on("FrameLoading", function (event, menu) {
            self.title = "Loading..."
        });

        $rootScope.$on("FrameLoaded", function (event, menu) {
            self.pageTitle = menu.Name;
            self.title = AppService.getPageTitle($window.location.pathname) + " - " + self.pageTitle;
        });
    }]);
