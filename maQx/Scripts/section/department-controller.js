angular.module("sectionApp")
    .controller("DepartmentController", ['Departments', '$rootScope', '$http', function (Departments, $rootScope, $http) {
        var self = this;
        var pages = { add: "create.html", del: "delete.html", edit: "edit.html" };

        self.proto = {
            def: function () {
                return {
                    Key: "",
                    Name: ""
                }
            },
            isLoaded: function () {
                return self.isLoaded;
            },
            isLoading: function () {
                return self.isLoading;
            },
            formEdit: function () {
                return self.formEdit;
            },
            getPage: function () {
                return self.page;
            },
            add: function (division) {          
                self.division = division.Key;
                self.department = self.proto.def();
                self.page = pages.add;
                self.formEdit = true;
            },
            del: function (department) {               
                self.department = department;
                self.page = pages.del;
                self.formEdit = true;
            },
            edit: function (department, division) {
                self.division = division.Key;
                self.department = department;
                console.log(department);
                self.page = pages.edit;
                self.formEdit = true;
            },
            closeEdit: function () {
                self.formEdit = false;                
            },
            save: function () {
                self.isLoading = true;
                var a = new Departments({ Name: self.department.Name, Division: self.division });
                a.$save(function () {
                    if (a.Type) {
                        if (a.Type == "ERROR") {
                            $rootScope.$broadcast("alert", { type: "Error", message: a.Message });
                            self.isLoading = false;
                        }
                    } else {
                        angular.forEach(self.divisionList, function (division) {
                            if (division.division.Key == self.division) {
                                division.departmentList.push(a);
                            }
                        });                       
                        self.isLoading = false;
                        self.formEdit = false;
                    }
                }, function (error) {
                    show_error({
                        type: "Error",
                        message: error
                    });
                });
            },
            remove: function () {
                self.isLoading = true;
                var a = new Departments({ Key: self.department.Key });
                a.$remove(function () {
                    if (a.Type) {
                        if (a.Type == "ERROR") {
                            $rootScope.$broadcast("alert", { type: "Error", message: a.Message });
                            self.isLoading = false;
                        }
                    } else {
                        angular.forEach(self.divisionList, function (division) {
                            $.each(division.departmentList, function (i, e) {
                                if (e.Key == a.Key) {
                                    division.departmentList.splice(i, 1);
                                    return false;
                                }
                            });
                        });
                       
                        self.isLoading = false;
                        self.formEdit = false;
                    }
                }, function (error) {
                    show_error({
                        type: "Error",
                        message: error
                    });
                });
            },
            update: function () {
                self.isLoading = true;
                var a = new Departments({ Key: self.department.Key, Name: self.department.Name, Division: self.division });
                a.$update(function () {
                    if (a.Type) {
                        if (a.Type == "ERROR") {
                            $rootScope.$broadcast("alert", { type: "Error", message: a.Message });
                            self.isLoading = false;
                        }
                    } else {
                        angular.forEach(self.divisionList, function (division) {
                            $.each(division.departmentList, function (i, e) {
                                if (e.Key == a.Key) {
                                    e = a;
                                    return false;
                                }
                            });
                        });
                       
                        self.isLoading = false;
                        self.formEdit = false;
                    }
                }, function (error) {
                    show_error({
                        type: "Error",
                        message: error
                    });
                });
            }
        }

        self.divisionList = [];
        self.division = "0";
        self.department = self.proto.def();
        self.isLoading = false;
        self.isLoaded = false;
        self.formEdit = false;
        self.page = pages.add;

        function show_error(error) {
            $rootScope.$broadcast("alert", error);
        }

        function doAction(Division, i, j) {
            Departments.query({ Division: Division.Key }).$promise.then(function (data) {
                self.divisionList.unshift({ division: Division, departmentList: data });

                if (i == j) {
                    self.isLoading = false;
                    self.isLoaded = true;
                }

            }, function (error) {
                self.isLoading = false;
                show_error({
                    type: "Error",
                    message: error
                });
            });
        }

        function init() {
            self.isLoading = true;
            $http.get("/get/Divisions/").then(function (result) {
                if (result.data.Type == "SUCCESS") {
                    for (var i = 0; i < result.data.List.length; i++) {
                        doAction(result.data.List[i], i, result.data.List.length - 1);
                    }
                }

            }, function (error) {
                self.isLoading = false;
                show_error({
                    type: "Error",
                    message: error
                });
            });
        }

        init();
    }]);