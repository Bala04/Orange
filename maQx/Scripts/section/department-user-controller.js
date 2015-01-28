angular.module("sectionApp").controller("DepartmentUserController", ['$http', '$rootScope', '$scope', '$modal', function ($http, $rootScope, $scope, $modal) {
    var self = this;
    self.division = "-1";
    self.departmentList = [];
    self.isLoading = false;
    self.isLoaded = false;
    self.divisionUsers = [];
    self.users = [];
    $scope.department = {};
    self.openMenus = function (size, department) {
        $scope.department = department;
        var a = linq(self.departmentList).where("$.department.ID=='" + $scope.department.ID + "'").toArray();
        var c = linq(self.divisionUsers).where("$.division=='" + self.division + "'").first().menus;
        var b = a.length > 0 ? a[0].divisionUser : [];
        var modalInstance = $modal.open({
            templateUrl: 'menu-selector.html',
            controller: function ($scope, $modalInstance, items) {
                var self = this;
                $scope.items = items;
                $scope.menuList = linq($scope.items.menus).select("$").toArray();
                self.menuState = $scope.menuList;
                $scope.menu = _app.clone($scope.menuList);

                $scope.Loading = function () {
                    return $scope.isLoading;
                };

                $scope.ok = function () {

                    var request = {
                        Add: [],
                        Remove: [],
                        Department: $scope.items.department.ID
                    };
                    for (var i = 0; i < $scope.menuList.length; i++) {
                        var prevState = linq($scope.items.selectedMenus).any("$.Id=='" + $scope.menuList[i].user.Id + "'");
                        var menuState = linq(self.menuState).where("$.user.Id=='" + $scope.menuList[i].user.Id + "'").first().selected;
                        var _selected = linq($scope.menu).where("$.user.Id=='" + $scope.menuList[i].user.Id + "'").first().selected;
                        if (prevState || !menuState) {
                            if (_selected && !prevState) {
                                request.Add.push($scope.menuList[i].user.Id);
                            } else if (!_selected && prevState) {
                                if ($scope.menuList[i].mapper != null) {
                                    request.Remove.push($scope.menuList[i].mapper);
                                }
                            }
                        }
                    }

                    if (request.Add.length > 0 || request.Remove.length > 0) {
                        $scope.isLoading = true;

                        $http.get("/get/Exists/" + _app.escapeHtml($scope.items.division) + "?type=add-department-user&ref=" + JSON.stringify(request)).then(function (result) {
                            if (result.data.Type == "SUCCESS") {
                                var await = $scope.items.callback(result.data.List);
                                $scope.statusText = "";
                                $modalInstance.close($scope.items);
                            }
                            $scope.isLoading = false;
                        }, function (error) {
                            console.log(error);
                            $scope.isLoading = false;
                        });
                    } else {
                        $modalInstance.close($scope.items);
                    }
                };
                $scope.isSelectable = function (element) {
                    var b = linq(self.menuState).where("$.user.Id=='" + element.user.Id + "'").first().selected;
                    if ($scope.items.selectedMenus.length > 0) {
                        var a = linq($scope.items.selectedMenus).any("$.Id=='" + element.user.Id + "'");
                        return a || !b;
                    }
                    return !b;
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
                    return {
                        department: $scope.department,
                        menus: c,
                        selectedMenus: b,
                        division: self.division,
                        callback: self.proto.loadMenus
                    };
                }
            }
        });
        modalInstance.result.then(function (selectedItem) {

        }, function () { });
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
                            self.departmentList.push({
                                department: result.data.List[i],
                                divisionUser: []
                            });

                            _app.enableTooltip();
                        }
                        $http.get("/app/MappableUsers/").then(
                        function (result) {
                            if (result.data.Type == "SUCCESS") {
                                for (var i = 0; i < result.data.List.length; i++) {
                                    self.users.push({
                                        user: result.data.List[i],
                                        selected: false,
                                        mapper: null
                                    })
                                }
                            }
                            else if (result.data.Type == "ERROR") {
                                show_error({
                                    type: "Error",
                                    message: result.data.Message
                                });
                            }
                            self.isLoading = false;
                        }, function (error) {
                            show_error({
                                type: "Error",
                                message: error
                            });
                            self.isLoaded = false;
                            self.isLoading = false;
                        });
                    }
                    else if (result.data.Type == "ERROR") {
                        show_error({
                            type: "Error",
                            message: result.data.Message
                        });
                    }
                }, function (error) {
                    show_error({
                        type: "Error",
                        message: error
                    });
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
        showMenus: function (item) {
            return item.divisionUser.length > 0;
        },
        getMappedMenuCount: function () {
            var a = linq(self.divisionUser).where("$.division=='" + self.division + "'").firstOrDefault();
            if (a != null) {
                return linq(a.users).where("$.selected").count();
            }
            return 0;
        },
        getMenuCount: function () {
            return self.users.length;
        },
        loadMenus: function (List) {
            var newUsers = self.users;            
            var c = linq(List);
            for (var i = 0; i < self.departmentList.length; i++) {
                var b = c.where("$.Department.ID=='" + self.departmentList[i].department.ID + "'").select("$.User").toArray();
                self.departmentList[i].divisionUser = b;
                var condition = "$.Department.ID=='" + self.departmentList[i].department.ID + "'";
                if (b.length > 0) {
                    for (var j = 0; j < newUsers.length; j++) {
                        var mapKey = c.where(condition + " && $.User.Id=='" + newUsers[j].user.Id + "'").toArray();
                        if (mapKey.length > 0) {
                            newUsers[j].mapper = mapKey[0].Key
                            newUsers[j].selected = true;
                        }
                    }
                }

            }
            for (var k = 0; k < self.divisionUsers.length; k++) {
                if (self.divisionUsers[k].division == self.division) {
                    self.divisionUsers.splice(k, 1);
                }
            }
            self.divisionUsers.push({
                division: self.division,
                menus: newUsers,
                list: List
            });
            return true;
        },
        change: function () {
            if (self.division != "-1") {
                var a = linq(self.divisionUsers).where("$.division=='" + self.division + "'").toArray();
                if (a.length > 0) {
                    self.proto.loadMenus(a[0].list);
                } else {
                    self.isLoading = true;
                    $http.get("/get/DepartmentUser/" + _app.escapeHtml(self.division)).then(

                    function (result) {
                        if (result.data.Type == "SUCCESS") {;
                            self.isLoaded = self.proto.loadMenus(result.data.List);
                        }
                        else if (result.data.Type == "ERROR") {
                            show_error({
                                type: "Error",
                                message: result.data.Message
                            });
                        }
                        self.isLoading = false;
                    }, function (error) {
                        show_error({
                            type: "Error",
                            message: error
                        });
                        self.isLoaded = false;
                        self.isLoading = false;
                    });
                }
            }
        }
    };
    self.proto.init();
}]);