angular.module("orangeApp").service("UserService", ["$rootScope", "$resource", function ($rootScope, $resource) {
    var userResource = $resource('/get/current-user');
    var currentUser = null;

    return {
        getCurrentUser: function () {
            if (currentUser == null) {
                currentUser = userResource.get();
            }

            return currentUser;
        }
    }
}]);