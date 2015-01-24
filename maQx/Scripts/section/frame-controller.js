angular.module("orangeApp")
    .controller("FrameController", ['$rootScope', '$scope', '$timeout', function ($rootScope, $scope, $timeout) {
        var self = this;
        self.page = "";

        function init(url) {
            $timeout(function () {
                $("#app-frame").attr("src", url);
            }, 300);
        }

        $scope.frameLoaded = function (url) {
            $rootScope.$broadcast("AppFrameLoaded", { url: url, menuID: url.split('/')[1] });
        };

        $rootScope.$on("FrameLoading", function (event, url) {
            init(url);
        });
    }]);