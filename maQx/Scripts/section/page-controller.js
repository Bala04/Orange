angular.module("orangeApp")
    .controller("PageController", ['$rootScope', function ($rootScope) {
        var self = this;
        self.pageLoading = false;

        self.isPageLoading = function () {
            return self.pageLoading;
        };

        $rootScope.$on("FrameLoading", function (event, menu) {
            self.pageLoading = true;
        });

        $rootScope.$on("FrameLoaded", function (event, menu) {
            self.pageLoading = false;
        });
    }]);