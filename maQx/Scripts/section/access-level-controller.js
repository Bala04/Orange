angular.module("sectionApp").controller("AccessLevelsController", ['$rootScope', '$http', function ($rootScope, $http) {
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
    self.menuList = [];
    function getUser() {
        return linq(self.userList).where("$.Id=='" + self.user + "'").firstOrDefault();
    }
    function show_error(error) {
        $rootScope.$broadcast("alert", error);
    }
    self.proto = {
        isLoading: function () {
            return self.isLoading;
        },
        isTransparentLoading: function () {
            return self.isTransparentLoading;
        },
        isLoaded: function () {
            return self.isLoaded && self.user != "-1";
        },
        showContent: function () {
            return self.isLoaded && !self.isLoading && self.user != "-1";
        },
        showWarning: function () {
            return self.user == "-1" && !self.isLoading;
        },
        getIcon: function (plant) {
            return plant.open ? "open" : "";
        },
        load: function (plant) {
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
                if (request.Add.length > 0 || request.Remove.length > 0) {
                    self.isTransparentLoading = true;
                    $http.get("/get/Exists/?type=access-division&ref=" + JSON.stringify(request)).then(function (result) {
                        if (result.data.Type == "SUCCESS") {
                            var await = self.proto.setup(result.data.List, getUser());
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
            self.proto.load(plant);
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
        openTab: function (plant) {
            self.proto.initDivision(plant);
            self.proto.load(plant);
        },
        toggleOpen: function (plant) {
            self.proto.stateChange(plant, !plant.open);
        },
        setup: function (List, user) {
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
                    self.isLoading = false;
                } else if (result.data.Type == "ERROR") {
                    show_error({
                        type: "Error",
                        message: result.data.Message
                    });
                    self.isLoading = false;
                }
            }, function () {
                self.isLoading = false;
            });
        },
        change: function () {
            if (self.user != "-1" && getUser() == null) {
                self.userList.push({
                    Id: self.user
                });
                self.isLoading = true;
                $http.get("/get/DivisionAccess/" + self.user).then(function (result) {
                    if (result.data.Type == "SUCCESS") {
                        self.isLoaded = self.proto.setup(result.data.List, getUser());
                        self.isLoading = false;
                    }
                    else if (result.data.Type == "ERROR") {
                        show_error({
                            type: "Error",
                            message: result.data.Message
                        });
                    }
                }, function (error) {
                    self.isLoaded = false;
                    self.isLoading = false;
                });
            }
        }
    }
    self.proto.init();
}]);