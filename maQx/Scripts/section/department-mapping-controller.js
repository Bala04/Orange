angular.module("sectionApp").controller("DepartmentMappingController", ["$scope", "$modalInstance", "items", "$http", function ($scope, $modalInstance, items, $http) {
    var self = this;
    $scope.items = items;
    var Key = $scope.items.includes[1];
    self.selectedEntites = $scope.items.selectedEntites;
    self.entityState = $scope.items.entites;
    $scope.entityList = _app.clone($scope.items.entites);
    $scope.ok = function () {
        var request = {
            Add: [],
            Remove: [],
            Entity: $scope.items.department.Key
        };
        angular.forEach($scope.entityList, function (entity) {
            var isAvailable = linq(self.selectedEntites).any("$." + Key + "=='" + entity.entity[Key] + "'");
            var isInSelect = linq(self.entityState).where("$.entity." + Key + "=='" + entity.entity[Key] + "'").first().selected;
            var isSelected = linq($scope.entityList).where("$.entity." + Key + "=='" + entity.entity[Key] + "'").first().selected;
            if (isAvailable || !isInSelect) {
                if (isSelected && !isAvailable) {
                    request.Add.push(entity.entity[Key]);
                } else if (!isSelected && isAvailable) {
                    request.Remove.push(entity.mapper);
                }
            }
        });
        if (request.Add.length > 0 || request.Remove.length > 0) {
            $scope.isLoading = true;
            $http.get("/get/Exists/" + _app.escapeHtml($scope.items.division.division) + "?type=add-department-" + $scope.items.includes[0] + "&ref=" + JSON.stringify(request)).then(function (result) {
                if (result.data.Type == "SUCCESS") {
                    var await = $scope.items.callback(result.data.List, $scope.items.includes[2], Key, $scope.items.division);
                    $scope.statusText = "";
                    $modalInstance.close($scope.items);
                } else {
                    $scope.items.error({
                        type: "Error",
                        message: result.data.Message
                    });
                }
                $scope.isLoading = false;
            }, function () {
                $scope.isLoading = false;
            });
        } else {
            $modalInstance.close($scope.items);
        }
    };
    $scope.isSelectable = function (element) {
        var b = linq(self.entityState).where("$.entity." + Key + "=='" + element.entity[Key] + "'").first().selected;
        if (self.selectedEntites.length > 0) {
            var a = linq(self.selectedEntites).any("$." + Key + "=='" + element.entity[Key] + "'");
            return a || !b;
        }
        return !b;
    };
    $scope.Loading = function () {
        return $scope.isLoading;
    };
    $scope.cancel = function () {
        $modalInstance.dismiss('cancel');
    };
}]).factory("DepartmentMappingFactory", ['$http', function ($http) {
    var factory = {
        init: function (self, type, show_error, entityType) {
            self.isLoading = true;
            $http.get(type).then(function (result) {
                if (result.data.Type == "SUCCESS") {
                    angular.forEach(result.data.List, function (entity) {
                        self.entites.push({
                            entity: entity,
                            selected: false,
                            mapper: null,
                            division: null,
                            department: null
                        });
                    });
                    if (entityType == "User") {
                        (function (self, show_error) {
                            $http.get("/get/MappedUsers").then(function (result) {
                                if (result.data.Type == "SUCCESS") {
                                    angular.forEach(self.entites, function (item) {
                                        angular.forEach(result.data.List, function (list) {
                                            item.selected = item.entity.Id == list.User.Id;
                                        });
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
                        })(self, show_error);
                    } else {
                        self.isLoading = false;
                    }
                }
                else if (result.data.Type == "ERROR") {
                    show_error({
                        type: "Error",
                        message: result.data.Message
                    });
                    self.isLoading = false;
                }
            }, function (error) {
                self.isLoaded = false;
                self.isLoading = false;
            });
        },
        open: function (size, department, getDivision, modal, load, show_error, includes) {
            var division = getDivision();
            var selectedEntites = linq(division.departmentList).where("$.department.Key=='" + department.Key + "'").first().entites;
            var modalInstance = modal.open({
                resolve: {
                    items: function () {
                        return {
                            department: department,
                            entites: division.defaultEntites,
                            selectedEntites: selectedEntites,
                            division: division,
                            callback: load,
                            error: show_error,
                            includes: includes
                        };
                    }
                },
                templateUrl: 'modal-selector.html',
                controller: "DepartmentMappingController",
                size: size,
                backdrop: 'static',
                keyboard: false
            });
        },

        remove: function (entityItem, divisionKey, departmentKey) {
            if (entityItem.division == divisionKey && entityItem.department == departmentItem.department.Key) {
                entityItem.mapper = null;
                entityItem.selected = false;
                entityItem.division = null;
                entityItem.department = null;

                console.log("Removed", entityItem.entity.Firstname, entityItem)
            }
        },

        load: function (entites, List, entityType, Key, division) {
            var newEntity = entityType == "Menu" ? _app.clone(entites) : entites;
            angular.forEach(division.departmentList, function (departmentItem) {
                var filtered = linq(List).where("$.Department.Key=='" + departmentItem.department.Key + "'");
                var element = [];
                if (filtered.count() > 0) {
                    angular.forEach(newEntity, function (entityItem) {
                        var entity = filtered.where("$." + entityType + "." + Key + "=='" + entityItem.entity[Key] + "'").firstOrDefault();
                        if (entity != null) {
                            element.push(entity[entityType]);
                            entityItem.mapper = entity.Key;
                            entityItem.selected = true;
                            entityItem.division = division.division;
                            entityItem.department = departmentItem.department.Key;
                        } else {
                            if (entityItem.division == division.division && entityItem.department == departmentItem.department.Key) {
                                factory.remove(entityItem);                                
                            }
                        }
                    });
                } else {
                    angular.forEach(newEntity, function (entityItem) {
                        if (entityItem.division == division.division && entityItem.department == departmentItem.department.Key) {
                            factory.remove(entityItem);
                        }
                    });
                }
                departmentItem.entites = element;
            });
            division.defaultEntites = newEntity;
            return true;
        },
        change: function (self, getDivision, loadType, show_error, entityType, Key) {
            if (self.division != "-1" && getDivision() == null) {
                self.isLoading = true;
                $http.get("/get/Exists/" + _app.escapeHtml(self.division) + "?type=department").then(function (result) {
                    if (result.data.Type == "SUCCESS") {
                        var departments = [];
                        angular.forEach(result.data.List, function (department) {
                            departments.push({
                                department: department,
                                entites: []
                            })
                        });
                        self.divisionDepartment.push({
                            division: self.division,
                            departmentList: departments,
                            defaultEntites: []
                        });
                        $http.get("/get/" + loadType + "/" + _app.escapeHtml(self.division)).then(function (result) {
                            if (result.data.Type == "SUCCESS") {
                                self.isLoaded = self.proto.load(result.data.List, entityType, Key, getDivision());
                            }
                            else if (result.data.Type == "ERROR") {
                                show_error({
                                    type: "Error",
                                    message: result.data.Message
                                });
                            }
                            self.isLoading = false;
                        }, function (error) {
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
                    self.isLoaded = false;
                    self.isLoading = false;
                });
            }
        },
        isLoading: function (self) {
            return self.isLoading;
        },
        isLoaded: function (self) {
            return self.isLoaded && self.division != "-1";
        },
        showContent: function (self) {
            return self.isLoaded && !self.isLoading;
        },
        showWarning: function (self) {
            return self.division == "-1" && !self.isLoading;
        },
        showEntity: function (item) {
            return item.entites.length > 0;
        },
        getMappedEntityCount: function (getDivision) {
            var division = getDivision();
            return division != null ? linq(division.defaultEntites).where("$.selected").count() : 0;
        },
        getEntityCount: function (self) {
            return self.entites.length;
        },
    };

    return factory;
}]);