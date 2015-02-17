angular.module("orangeApp")
    .controller("UserController", ['$rootScope', '$scope', '$window', 'UserService', function ($rootScope, $scope, $window, UserService) {
        var self = this;
        self.loaded = false;
        self.ImgEnabled = false;
        self.user = {
            Name: "",
        };

        var currentUser = UserService.getCurrentUser();
        if (currentUser != null) {
            currentUser.$promise.then(function (data) {
                if (data.Type == "SUCCESS") {
                    if (data.Role == "TempSession") {
                        self.loaded = false;
                    } else {
                        self.user = data;
                        self.ImgEnabled = currentUser.ImgURL != null;
                        self.loaded = true;
                        $rootScope.$broadcast("UserLoaded", data);
                    }
                } else if (data.Type == "ERROR") {
                    $scope.$emit("AuthenticationFailed", data);
                }
            }, function (error) {
                console.log(error)
            });
        } else {
            console.log("Error")
        }

        self.isLoaded = function () {
            return self.loaded;
        };

        self.isImgEnabled = function () {
            return self.ImgEnabled;
        };
    }]);