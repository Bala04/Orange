angular.module("sectionApp").controller("DepartmentUsersController", ['$http', '$rootScope', '$scope', '$modal', 'DepartmentMappingFactory', function ($http, $rootScope, $scope, $modal, DepartmentMappingFactory) {
    var self = this;
    self.division = "-1";
    self.isLoading = false;
    self.isLoaded = false;
    self.entites = [];
    self.divisionDepartment = [];

    function getDivision() {
        return linq(self.divisionDepartment).where("$.division=='" + self.division + "'").firstOrDefault();
    }

    self.openMenus = function (size, department) {
        DepartmentMappingFactory.open(size, department, getDivision, $modal, self.proto.load, show_error, ["user", "Id", "User"]);
    };

    function show_error(error) {
        $rootScope.$broadcast("alert", error);
    }

    self.proto = {
        isLoading: function () {
            return DepartmentMappingFactory.isLoading(self);
        },
        isLoaded: function () {
            return DepartmentMappingFactory.isLoaded(self);
        },
        showContent: function () {
            return DepartmentMappingFactory.showContent(self);
        },
        showWarning: function () {
            return DepartmentMappingFactory.showWarning(self);
        },
        showMenus: function (item) {
            return DepartmentMappingFactory.showEntity(item);
        },
        getMappedMenuCount: function () {
            return DepartmentMappingFactory.getMappedEntityCount(getDivision);
        },
        getMenuCount: function () {
            return DepartmentMappingFactory.getEntityCount(self);
        },
        load: function (List, entityType, Key, division) {
            return DepartmentMappingFactory.load(self.entites, List, entityType, Key, division);
        },
        change: function () {
            DepartmentMappingFactory.change(self, getDivision, "DepartmentUser", show_error, "User", "Id");
        }
    };

    DepartmentMappingFactory.init(self, "/app/MappableUsers/", show_error, "User");
}]);