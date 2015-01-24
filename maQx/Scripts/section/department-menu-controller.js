angular.module("sectionApp")
    .controller("DepartmentMenuController", ['$http', '$rootScope', '$scope', '$modal', function ($http, $rootScope, $scope, $modal) {
        var self = this;
        self.division = "-1";
        self.departmentList = [];
        self.isLoading = false;
        self.isLoaded = false;
        self.divisionMenus = [];
        $scope.menus = [];
        $scope.department = {};

        self.openMenus = function (size, department) {
            $scope.department = department;
            var a = linq(self.departmentList).where("$.department.ID=='" + $scope.department.ID + "'").toArray();
            var b = a.length > 0 ? a[0].divisionMenu : [];

            var modalInstance = $modal.open({
                templateUrl: 'menu-selector.html',
                controller: function ($scope, $modalInstance, items) {
                    $scope.items = items;



                    $scope.selected = {
                        item: $scope.items[0],
                    };

                    $scope.Loading = function () {
                        return $scope.isLoading;
                    };

                    $scope.ok = function () {
                        $modalInstance.close($scope.selected.item);
                    };

                    $scope.isSelected = function (menu) {
                        
                    };

                    $scope.cancel = function () {
                        $modalInstance.dismiss('cancel');
                    };
                },
                size: size,
                backdrop: 'static',
                keyboard: false,
                resolve: {
                    items: function () {
                        return { item: $scope.menus, department: $scope.department, selectedMenu: b };
                    }
                }
            });


            modalInstance.result.then(function (selectedItem) {
                $scope.selected = selectedItem;
            }, function () {

            });
        };

        function show_error(error) {
            $rootScope.$broadcast("alert", error);
        }

        self.proto = {
            init: function () {
                if (self.departmentList.length <= 0) {
                    self.isLoading = true;
                    $http.get("/get/Exists/" + _app.escapeHtml(self.division) + "?type=department").then(
                        function (result) {
                            if (result.data.Type == "SUCCESS") {
                                for (var i = 0; i < result.data.List.length; i++) {
                                    self.departmentList.push({ department: result.data.List[i], divisionMenu: [] });
                                }

                                $http.get("/get/Exists/" + _app.escapeHtml(self.division) + "?type=menus").then(
                                    function (result) {
                                        if (result.data.Type == "SUCCESS") {
                                            for (var i = 0; i < result.data.List.length; i++) {
                                                $scope.menus.push({ menu: result.data.List[i], selected: false })
                                            }
                                        }
                                        else if (result.data.Type == "ERROR") {
                                            error({ type: "Error", message: result.data.Message });
                                        }
                                        self.isLoading = false;
                                    },
                                    function (error) {
                                        error({ type: "Error", message: error });
                                        self.isLoaded = false;
                                        self.isLoading = false;
                                    });
                            }
                            else if (result.data.Type == "ERROR") {
                                show_error({ type: "Error", message: result.data.Message });
                            }
                        },
                        function (error) {
                            show_error({ type: "Error", message: error });
                            self.isLoaded = false;
                            self.isLoading = false;
                        });
                }
            },
            isLoading: function () {
                return self.isLoading;
            },
            isLoaded: function () {
                return self.isLoaded && self.division != "-1";
            },
            showContent: function () {
                return self.isLoaded && !self.isLoading;
            },
            showWarning: function () {
                return self.division == "-1" && !self.isLoading;
            },

            change: function () {
                if (self.division != "-1") {
                    var a = linq(self.divisionMenus).where("$.division=='" + self.division + "'").toArray();

                    if (a.length > 0) {

                    } else {
                        $http.get("/get/Exists/" + _app.escapeHtml(self.division) + "?type=department-menu").then(
                            function (result) {
                                if (result.data.Type == "SUCCESS") {
                                    self.divisionMenus.push({ division: self.division, menus: result.data.List });

                                    if (result.data.List.length > 0) {

                                        var c = linq(result.data.List);

                                        for (var i = 0; i < self.departmentList.length; i++) {
                                            var b = c.where("$.Department.Key=='" + self.departmentList.department.Key + "'").toArray();
                                            self.departmentList[i].divisionMenu = b;
                                        }
                                    }

                                    self.isLoaded = true;
                                }
                                else if (result.data.Type == "ERROR") {
                                    error({ type: "Error", message: result.data.Message });
                                }
                                self.isLoading = false;
                            },
                            function (error) {
                                error({ type: "Error", message: error });
                                self.isLoaded = false;
                                self.isLoading = false;
                            });
                    }
                }
            }
        };

        self.proto.init();
    }]);