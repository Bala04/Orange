angular.module("sectionApp").controller("AccessLevelsController", ['$rootScope', '$http', "PromiseFactory", function ($rootScope, $http, PromiseFactory) {
    var self = this;
    self.user = "-1";
    self.isLoading = false;
    self.isLoaded = false;
    self.isTransparentLoading = false;
    self.userList = [];
    self.status = {
        isFirstOpen: true,
        isFirstDisabled: false
    };
    self.plantList = [];
    self.roles = [
        { Role: "Create", Description: "Can Create" },
        { Role: "Edit", Description: "Can Edit" },
        { Role: "Delete", Description: "Can Delete" },
        { Role: "CreateEdit", Description: "Can Create and Edit" },
        { Role: "EditDelete", Description: "Can Edit and Delete" },
    ];


    function getUser() {
        return linq(self.userList).where("$.Id=='" + self.user + "'").firstOrDefault();
    }

    self.proto = {
        def: function () {
            return {
                Add: [],
                Remove: [],
                Entity: self.user
            };
        },
        isLoading: function () {
            return self.isLoading || self.isTransparentLoading;
        },
        isContentLoading: function () {
            return self.isLoading;
        },
        isTransparentLoading: function () {
            return self.isTransparentLoading;
        },
        showContent: function () {
            return !self.isLoading && self.isLoaded;
        },
        showWarning: function () {
            return self.user == "-1" && !self.isLoading;
        },
        showTabContent: function () {
            return self.user != "-1";
        },
        getIcon: function (plant) {
            return plant.open ? "open" : "";
        },
        request: function (url, entity, Key, request) {
            if (request.Add.length > 0 || request.Remove.length > 0) {
                self.isTransparentLoading = true;
                $http.get(url).then(function (result) {
                    var await = self.proto.setup(result.data[Key], getUser(), entity);
                }).finally(function () {
                    self.isTransparentLoading = false;
                });
            }
        },
        loadRole: function (role) {
            if (self.user != "-1") {
                var request = self.proto.def();
                var user = getUser();
                prevRole = linq(user._roleList).where("$.Role=='" + role.Role + "'").first();
                if (role.selected != prevRole.selected) {
                    if (role.selected) {
                        request.Add.push(role.Role);
                    } else {
                        request.Remove.push(role.Role)
                    }
                }

                self.proto.request("/app/ManageUserRoles/" + self.user + "?Reference=" + JSON.stringify(request), "Roles", "List", request);
            }
        },
        loadMenu: function (menu) {
            if (self.user != "-1") {
                var request = self.proto.def();
                var user = getUser();
                prevMenu = linq(user._menuList).where("$.ID=='" + menu.ID + "'").first();
                if (menu.selected != prevMenu.selected) {
                    if (menu.selected) {
                        request.Add.push(menu.ID);
                    } else {
                        request.Remove.push(menu.mapper)
                    }
                }

                self.proto.request("/get/Exists/" + user.department.Key + "?type=access-menu&ref=" + JSON.stringify(request), "Menu", "Value", request);
            }
        },
        loadPlant: function (plant) {
            if (self.user != "-1") {
                var request = self.proto.def();
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

                self.proto.request("/get/Exists/plant?type=access-division&ref=" + JSON.stringify(request), "Plant", "List", request);
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
        openRole: function (role) {
            self.proto.loadRole(role);
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
            } else if (type == "Roles") {
                user.roleList = [];
                angular.forEach(_app.clone(self.roles), function (role) {
                    role.selected = linq(List).where("$=='" + role.Role + "'").any();
                    user.roleList.push(role);
                });
                user._roleList = _app.clone(user.roleList);
            }
            return true;
        },
        init: function () {
            self.isLoading = true;
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
                }
            }).finally(function () {
                self.isLoading = false;
            });
        },
        change: function () {
            if (self.user != "-1" && getUser() == null) {
                self.userList.push({
                    Id: self.user
                });
                self.isLoading = true;
                PromiseFactory.Resolve(["/get/DivisionAccess/" + self.user, "/get/UserDepartmentMenu/" + self.user, "/app/UserRoles/" + self.user]).then(function (result) {
                    var department = linq(result[1].data.Value.Item1).select("$.Department").firstOrDefault();
                    if (department != null) {
                        var user = getUser();
                        user.department = department;
                    }

                    self.isLoaded = self.proto.setup(result[0].data.List, getUser(), "Plant")
                        & self.proto.setup(result[1].data.Value, getUser(), "Menu")
                        & self.proto.setup(result[2].data.List, getUser(), "Roles");

                }).finally(function () {
                    self.isLoading = false;
                });
            }
        }
    }
    self.proto.init();
}]);