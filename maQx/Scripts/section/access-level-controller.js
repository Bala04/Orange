angular.module("sectionApp").controller("AccessLevelsController", ['$rootScope', '$http', function ($rootScope, $http) {
    var self = this;
    self.user = "-1";
    self.isPlantLoading = false;
    self.isMenuLoading = false;
    self.isPlantLoaded = false;
    self.isMenuLoaded = false;
    self.isTransparentLoading = false;
    self.userList = [];
    self.status = {
        isFirstOpen: true,
        isFirstDisabled: false
    };
    self.plantList = [];
    function getUser() {
        return linq(self.userList).where("$.Id=='" + self.user + "'").firstOrDefault();
    }

    function show_error(error) {
        $rootScope.$broadcast("alert", error);
    }
    self.proto = {
        isLoading: function () {
            return self.isPlantLoading || self.isMenuLoading || self.isTransparentLoading;
        },
        isPlantLoading: function () {
            return self.isPlantLoading;
        },
        isMenuLoading: function () {
            return self.isMenuLoading;
        },
        isTransparentLoading: function () {
            return self.isTransparentLoading;
        },
        showContent: function () {
            return self.user != "-1";
        },
        showPlantContent: function () {
            return !self.isPlantLoading && self.isPlantLoaded;
        },
        showMenuContent: function () {
            return !self.isMenuLoading && self.isMenuLoaded;
        },
        showWarning: function () {
            return self.user == "-1" && !self.proto.isLoading();
        },
        getIcon: function (plant) {
            return plant.open ? "open" : "";
        },
        request: function (request, type, entity, Key, param) {
            if (request.Add.length > 0 || request.Remove.length > 0) {
                self.isTransparentLoading = true;
                $http.get("/get/Exists/" + param + "?type=" + type + "&ref=" + JSON.stringify(request)).then(function (result) {
                    if (result.data.Type == "SUCCESS") {
                        var await = self.proto.setup(result.data[Key], getUser(), entity);
                    } else {
                        show_error({
                            type: "Error",
                            message: result.data.Message
                        });
                    }
                    self.isTransparentLoading = false;
                }, function () {
                    self.isTransparentLoading = false;
                });
            }
        },
        loadMenu: function (menu) {
            if (self.user != "-1") {
                var request = {
                    Add: [],
                    Remove: [],
                    Entity: self.user
                };
                var user = getUser();
                prevMenu = linq(user._menuList).where("$.ID=='" + menu.ID + "'").first();
                if (menu.selected != prevMenu.selected) {
                    if (menu.selected) {
                        request.Add.push(menu.ID);
                    } else {
                        request.Remove.push(menu.mapper)
                    }
                }
                self.proto.request(request, "access-menu", "Menu", "Value", user.department.Key);
            }
        },
        loadPlant: function (plant) {
            if (self.user != "-1") {
                var request = {
                    Add: [],
                    Remove: [],
                    Entity: self.user
                };
                var prev = linq(getUser()._plantList).where("$.Key=='" + plant.Key + "'").first();
                angular.forEach(plant.divisionList, function (entity) {
                    var division = linq(prev.divisionList).where("$.Key=='" + entity.Key + "'").first();
                    if (division.selected != entity.selected) {
                        if (entity.selected) {
                            request.Add.push(entity.Key);
                        } else {
                            request.Remove.push(entity.mapper);
                        }
                    }
                });
                self.proto.request(request, "access-division", "Plant", "List", "plant");
            }
        },
        initPlant: function (plant) {
            plant.selected = linq(plant.divisionList).all('$.selected');
            if (plant.selected) {
                plant.indeterminate = false;
            } else {
                plant.indeterminate = linq(plant.divisionList).any('$.selected');
            }
        },
        selectTab: function (plant) {
            self.proto.initPlant(plant);
            self.proto.loadPlant(plant);
        },
        initDivision: function (plant) {
            if (plant.selected) {
                self.proto.stateChange(plant, true);
                plant.indeterminate = false;
                angular.forEach(plant.divisionList, function (entity) {
                    entity.selected = true;
                });
            } else {
                angular.forEach(plant.divisionList, function (entity) {
                    entity.selected = false;
                });
            }
        },
        stateChange: function (plant, state) {
            plant.open = state;
            var a = linq(self.plantList).where("$.Key=='" + plant.Key + "'").first();
            a.open = state;
        },
        openMenu: function (menu) {
            self.proto.loadMenu(menu);
        },
        openTab: function (plant) {
            self.proto.initDivision(plant);
            self.proto.loadPlant(plant);
        },
        toggleOpen: function (plant) {
            self.proto.stateChange(plant, !plant.open);
        },
        setup: function (List, user, type) {
            if (type == "Plant") {
                var newEntity = _app.clone(self.plantList);
                angular.forEach(newEntity, function (entityItem) {
                    var filtered = linq(List).where("$.Division.Plant.Key=='" + entityItem.Key + "'");
                    if (filtered.count() > 0) {
                        angular.forEach(newEntity, function (plantItem) {
                            angular.forEach(plantItem.divisionList, function (divisionItem) {
                                var entity = filtered.where("$.Division.Key=='" + divisionItem.Key + "'").firstOrDefault();
                                if (entity != null) {
                                    angular.forEach(entityItem.divisionList, function (division) {
                                        if (entity.Division.Key == division.Key) {
                                            division.mapper = entity.Key, division.selected = true;
                                        }
                                    });
                                }
                            });
                        });
                    }
                    self.proto.initPlant(entityItem);
                });
                user._plantList = _app.clone(newEntity);
                user.plantList = newEntity;
            } else if (type == "Menu") {
                var menus = linq(List.Item1).select("$.Menu").toArray();
                var accessMenus = linq(List.Item2);
                user.menuList = [];
                angular.forEach(menus, function (menu) {
                    var access = accessMenus.where("$.DepartmentMenu.Menu.ID=='" + menu.ID + "'").firstOrDefault();
                    if (access != null) {
                        menu.selected = true;
                        menu.mapper = access.Key;
                    } else {
                        menu.selected = false;
                        menu.mapper = null;
                    }
                    user.menuList.push(menu);
                });
                user._menuList = _app.clone(user.menuList);
            }
            return true;
        },
        init: function () {
            self.isPlantLoading = true;
            $http.get("/get/Divisions").then(function (result) {
                if (result.data.Type == "SUCCESS") {
                    var plant = linq(result.data.List).select("$.Plant.Key").distinct();
                    var plantList = [];
                    angular.forEach(plant, function (entity) {
                        var data = linq(result.data.List).where("$.Plant.Key=='" + entity + "'");
                        var plant = data.select("$.Plant").first();
                        var division = data.toArray();
                        plant.open = false;
                        plant.selected = false;
                        plant.indeterminate = false;
                        plant.divisionList = [];
                        angular.forEach(division, function (entity) {
                            plant.divisionList.push({
                                Key: entity.Key,
                                Name: entity.Name,
                                selected: false,
                                mapper: null
                            });
                        });
                        self.plantList.push(plant);
                    });
                    self.isPlantLoading = false;
                } else if (result.data.Type == "ERROR") {
                    show_error({
                        type: "Error",
                        message: result.data.Message
                    });
                    self.isPlantLoading = false;
                }
            }, function () {
                self.isPlantLoading = false;
            });
        },
        change: function () {
            if (self.user != "-1" && getUser() == null) {
                self.userList.push({
                    Id: self.user
                });
                self.isPlantLoading = true;
                $http.get("/get/DivisionAccess/" + self.user).then(function (result) {
                    if (result.data.Type == "SUCCESS") {
                        self.isPlantLoaded = self.proto.setup(result.data.List, getUser(), "Plant");
                        self.isPlantLoading = false;
                    }
                    else if (result.data.Type == "ERROR") {
                        show_error({
                            type: "Error",
                            message: result.data.Message
                        });
                        self.isPlantLoading = false;
                    }
                }, function (error) {
                    self.isPlantLoaded = false;
                    self.isPlantLoading = false;
                });
                self.isMenuLoading = true;
                $http.get("/get/UserDepartmentMenu/" + self.user).then(function (result) {
                    if (result.data.Type == "SUCCESS") {
                        var department = linq(result.data.Value.Item1).select("$.Department").firstOrDefault();
                        if (department != null) {
                            var user = getUser();
                            user.department = department;
                        }
                        self.isMenuLoaded = self.proto.setup(result.data.Value, getUser(), "Menu");
                        self.isMenuLoading = false;
                    }
                    else if (result.data.Type == "ERROR") {
                        show_error({
                            type: "Error",
                            message: result.data.Message
                        });
                        self.isMenuLoading = false;
                    }
                }, function (error) {
                    self.isMenuLoaded = false;
                    self.isMenuLoading = false;
                });
            }
        }
    }
    self.proto.init();
}]);