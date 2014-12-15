angular.module("orangeApp")
    .controller("FrameController", ['$rootScope', '$scope', '$timeout', function ($rootScope, $scope, $timeout) {
        var self = this;
        self.page = "";

        function init(page) {
            $timeout(function () {
                $("#app-frame").attr("src", page);
            }, 300);
        }

        $scope.frameLoaded = function (frame) {
            $rootScope.$broadcast("AppFrameLoaded", frame);
        };

        $rootScope.$on("FrameLoading", function (event, menu) {
            init("/" + menu.ID);
        });
    }]);