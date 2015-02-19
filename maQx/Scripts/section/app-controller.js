angular.module("orangeApp")
    .controller("AppController", ["$rootScope", "$scope", "$location", "$window", "AppService", function ($rootScope, $scope, $location, $window, AppService) {
        var self = this;

        self.title = "maQx - the leading MI solutions from IP Rings";
        self.pageTitle = AppService.getPageTitle($window.location.pathname);
        self.appVersion = AppService.getAppVersion();
        self.appPage = "";

        self.getPageTitle = function () {
            return self.pageTitle;
        };

        self.getTitle = function () {
            return self.title;
        };

        $scope.$on("AuthenticationFailed", function (event, data) {
            $window.location.href = data.ReturnUrl + "?ReturnUrl=" + $window.location.pathname;
        });

        $rootScope.$on('$locationChangeSuccess', function () {
            if (self.appPage != $location.path()) {
                self.appPage = $location.path();
                $rootScope.$broadcast("LoadFrame", self.appPage);
            }
        });

        $rootScope.$on("AppFrameLoaded", function (event, frame) {
            if (self.appPage != frame.url) {
                self.appPage = frame.url;
                $location.path(frame.url);
            }
        });

        $rootScope.$on("FrameLoading", function (event, menu) {
            self.title = "Loading..."
        });

        $rootScope.$on("FrameLoaded", function (event, menu) {
            if (menu) {
                self.pageTitle = menu.Name;
                self.title = AppService.getPageTitle($window.location.pathname) + " - " + self.pageTitle;
            }
        });
    }]);
