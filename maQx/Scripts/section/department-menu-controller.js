angular.module("sectionApp").controller("DepartmentMenuController", ['$http', '$rootScope', '$scope', '$modal', function ($http, $rootScope, $scope, $modal) {
    var self = this;
    self.division = "-1";
    self.departmentList = [];
    self.isLoading = false;
    self.isLoaded = false;
    self.divisionMenus = [];
    self.menus = [];
    $scope.department = {};
    self.openMenus = function (size, department) {
        $scope.department = department;
        var a = linq(self.departmentList).where("$.department.ID=='" + $scope.department.ID + "'").toArray();
        var c = linq(self.divisionMenus).where("$.division=='" + self.division + "'").first().menus;
        var b = a.length > 0 ? a[0].divisionMenu : [];
        var modalInstance = $modal.open({
            templateUrl: 'menu-selector.html',
            controller: function ($scope, $modalInstance, items) {
                var self = this;
                $scope.items = items;
                $scope.menuList = linq($scope.items.menus).select("$").toArray();
                self.menuState = _app.clone($scope.menuList);
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
                        var prevState = linq($scope.items.selectedMenus).any("$.ID=='" + $scope.menuList[i].menu.ID + "'");
                        var menuState = linq(self.menuState).where("$.menu.ID=='" + $scope.menuList[i].menu.ID + "'").first().selected;
                        var _selected = linq($scope.menu).where("$.menu.ID=='" + $scope.menuList[i].menu.ID + "'").first().selected;
                        if (prevState || !menuState) {
                            if (_selected && !prevState) {
                                request.Add.push($scope.menuList[i].menu.ID);
                            } else if (!_selected && prevState) {
                                if ($scope.menuList[i].mapper != null) {
                                    request.Remove.push($scope.menuList[i].mapper);
                                }
                            }
                        }
                    }

                    if (request.Add.length > 0 || request.Remove.length > 0) {
                        $scope.isLoading = true;

                        $http.get("/get/Exists/" + _app.escapeHtml($scope.items.division) + "?type=add-department-menu&ref=" + JSON.stringify(request)).then(function (result) {
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
                    var b = linq(self.menuState).where("$.menu.ID=='" + element.menu.ID + "'").first().selected;
                    if ($scope.items.selectedMenus.length > 0) {
                        var a = linq($scope.items.selectedMenus).any("$.ID=='" + element.menu.ID + "'");
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
                                divisionMenu: []
                            });

                            _app.enableTooltip();
                        }
                        $http.get("/get/Exists/" + _app.escapeHtml(self.division) + "?type=menus").then(
                        function (result) {
                            if (result.data.Type == "SUCCESS") {
                                for (var i = 0; i < result.data.List.length; i++) {
                                    self.menus.push({
                                        menu: result.data.List[i],
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
            return item.divisionMenu.length > 0;
        },
        getMappedMenuCount: function () {
            var a = linq(self.divisionMenus).where("$.division=='" + self.division + "'").firstOrDefault();
            if (a != null) {
                return linq(a.menus).where("$.selected").count();
            }
            return 0;
        },
        getMenuCount: function () {
            return self.menus.length;
        },
        loadMenus: function (List) {
            var newMenus = _app.clone(self.menus);
           
                var c = linq(List);
                for (var i = 0; i < self.departmentList.length; i++) {
                    var b = c.where("$.Department.ID=='" + self.departmentList[i].department.ID + "'").select("$.Menu").toArray();
                    self.departmentList[i].divisionMenu = b;
                    var condition = "$.Department.ID=='" + self.departmentList[i].department.ID + "'";
                    if (b.length > 0) {
                        for (var j = 0; j < newMenus.length; j++) {
                            var mapKey = c.where(condition + " && $.Menu.ID=='" + newMenus[j].menu.ID + "'").toArray();
                            if (mapKey.length > 0) {
                                newMenus[j].mapper = mapKey[0].Key
                                newMenus[j].selected = true;
                            }
                        }
                    }
                
            }
            for (var k = 0; k < self.divisionMenus.length; k++) {
                if (self.divisionMenus[k].division == self.division) {
                    self.divisionMenus.splice(k, 1);
                }
            }
            self.divisionMenus.push({
                division: self.division,
                menus: newMenus,
                list: List
            });
            return true;
        },
        change: function () {
            if (self.division != "-1") {
                var a = linq(self.divisionMenus).where("$.division=='" + self.division + "'").toArray();
                if (a.length > 0) {
                    self.proto.loadMenus(a[0].list);
                } else {
                    self.isLoading = true;
                    $http.get("/get/DepartmentMenu/" + _app.escapeHtml(self.division)).then(

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