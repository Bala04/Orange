angular.module("sectionApp")
    .controller("ProductRawMaterialController", ['$scope', '$http', 'PromiseFactory', function ($scope, $http, PromiseFactory) {
        var self = this;
        self.currentPage = 1;
        self.totalPages = 0;
        self.productRawMaterialList = [];
        self.productList = [];
        self.rawMaterialList = [];
        self.isLoading = false;
        self.isLoaded = false;
        self.formEdit = false;
        self.product = "0";
        self.rawMaterial = "0";
        self.itemPerPage = 10;
        self.search = "";
        self.addMaterial = null;
        self.selectedProduct = null;
        var pages = { add: "create.html", del: "delete.html", edit: "edit.html" };

        self.addProducts = function (value) {         
            self.productList = value;
        };

        self.addRawMaterials = function (value) {
            self.rawMaterialList = value;
        };

        self.proto = {
            def: function () {
                return {
                    rawMaterial: "-1",
                    quantity: null,
                    uom: "-1",
                }
            },
            init: function () {
                self.isLoading = true;
                PromiseFactory.Resolve(["/get/ProductRawMaterials/"]).then(function (result) {                    
                    self.isLoaded = true;
                }).finally(function () {
                    self.isLoading = false;
                });
            },
            pageItem: function () {
                var a = self.currentPage * self.itemPerPage;
                var b = self.proto.totalItems();
                return a > b ? b : a;
            },
            startItem: function () {
                return ((self.currentPage - 1) * self.itemPerPage) + 1;
            },
            closeEdit: function () {
                self.formEdit = false;
            },
            showPagination: function () {
                return !(self.search != "" || self.product != '0' || self.rawMaterial != '0');
            },
            getEntityList: function () {
                return self.proto.showPagination() ? linq(self.productList).skip((self.currentPage - 1) * self.itemPerPage).take(self.itemPerPage).toArray() : self.productList;
            },
            getItemsPerPage: function () {
                return self.itemPerPage;
            },
            totalItems: function () {
                return self.productList.length;
            },
            isLoading: function () {
                return self.isLoading;
            },
            isLoaded: function () {
                return self.isLoaded;
            },
            formEdit: function () {
                return self.formEdit;
            },
            getPage: function () {
                return self.page;
            },
            add: function (product) {
                self.addMaterial = self.proto.def();
                self.selectedProduct = product;
                self.page = pages.add;
                self.formEdit = true;
            }
        };

        self.proto.init();
    }]);