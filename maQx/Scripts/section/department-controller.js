angular.module("sectionApp")
    .controller("DepartmentController", ['Departments', '$rootScope', function (Departments, $rootScope) {
        var self = this;        
        var pages = { add: "create.html", del: "delete.html", edit: "edit.html" };

        self.proto = {
            def: function () {
                return {
                    ID: "",
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
            add: function () {
                self.department = self.proto.def();
                self.page = pages.add;
                self.formEdit = true;
            },
            del: function (department) {
                console.log(department);
                self.department = department;
                self.page = pages.del;
                self.formEdit = true;
            },
            edit: function (department) {
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
                var a = new Departments({Name: self.department.Name});              
                a.$save(function () {
                    if (a.Type) {
                        if (a.Type == "ERROR") {
                            $rootScope.$broadcast("alert", { type: "Error", message: a.Message });
                            self.isLoading = false;
                        }
                    } else {
                        self.departmentList.unshift(a);
                        self.isLoading = false;
                        self.formEdit = false;
                    }
                });
            },
            remove: function () {
                self.isLoading = true;
                var a = new Departments({ ID: self.department.ID });
                a.$remove(function () {
                    if (a.Type) {
                        if (a.Type == "ERROR") {
                            $rootScope.$broadcast("alert", { type: "Error", message: a.Message });
                            self.isLoading = false;
                        }
                    } else {
                        $.each(self.departmentList, function (i, e) {
                            if (e.ID == a.ID) {
                                self.departmentList.splice(i, 1);
                                return false;
                            }
                        });
                        self.isLoading = false;
                        self.formEdit = false;
                    }
                });
            },
            update: function () {
                self.isLoading = true;
                var a = new Departments({ ID: self.department.ID, Name: self.department.Name });
                a.$update(function () {
                    if (a.Type) {
                        if (a.Type == "ERROR") {
                            $rootScope.$broadcast("alert", { type: "Error", message: a.Message });
                            self.isLoading = false;
                        }
                    } else {
                        $.each(self.departmentList, function (i, e) {
                            if (e.ID == a.ID) {
                                self.department = e;
                                return false;;
                            }
                        });
                        self.isLoading = false;
                        self.formEdit = false;
                    }
                });
            }
        }

        self.departmentList = [];
        self.department = self.proto.def();
        self.isLoading = false;
        self.isLoaded = false;
        self.formEdit = false;
        self.page = pages.add;

        function init() {
            self.isLoading = true;
            Departments.query().$promise.then(function (data) {
                self.departmentList = data;
                self.isLoading = false;
                self.isLoaded = true;

            }, function (error) {
                self.isLoading = false;
                console.log(error);
            });
        }

        init();
    }]);