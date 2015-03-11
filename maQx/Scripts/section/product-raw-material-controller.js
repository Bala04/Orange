angular.module("sectionApp")
    .controller("ProductRawMaterialController", ['$scope', '$http', 'PromiseFactory', function ($scope, $http, PromiseFactory) {
        var self = this;
        self.currentPage = 1;
        self.totalPages = 0;
        self.productList = [];
        self.isLoading = false;
        self.isLoaded = false;
        self.formEdit = false;
        self.product = "0";
        self.rawMaterial = "0";
        self.itemPerPage = 10;
        self.search = "";

        self.allowCreate = false;
        self.allowDelete = false;
        self.allowEdit = false;

        self.loadUrl = function (url, product) {
            return decodeURIComponent(url) + "/" + product.value;
        };

        self.loadItemUrl = function (url, rawMaterial) {
            return decodeURIComponent(url) + "/" + rawMaterial.Key;
        };

        self.addProducts = function (value) {
            self.productList = value;
            angular.forEach(self.productList, function (product) {
                product.rawMaterialList = [];
            });
        };


        self.proto = {

            init: function () {
                self.isLoading = true;
                PromiseFactory.Resolve(["/get/ProductRawMaterials/"]).then(function (result) {

                    console.log(result);

                    self.allowCreate = result[0].data.AllowCreate;
                    self.allowDelete = result[0].data.IsDeleteable;
                    self.allowEdit = result[0].data.IsEditable;
                    angular.forEach(self.productList, function (product) {
                        product.rawMaterialList = linq(result[0].data.List).where("$.Product.Key=='" + product.value + "'").toArray();
                    });
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
                var productList = [];

                if (self.rawMaterial == "0") {
                    productList = self.productList;
                } else {
                    angular.forEach(self.productList, function (product) {
                        if (linq(product.rawMaterialList).any("$.RawMaterial.Key=='" + self.rawMaterial + "'")) {
                            productList.push(product);
                        }
                    });
                }
                return self.proto.showPagination() ? linq(productList).skip((self.currentPage - 1) * self.itemPerPage).take(self.itemPerPage).toArray() : productList;
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
            allowCreate: function () {
                return self.allowCreate;
            },
            allowEdit: function () {
                return self.allowEdit;
            },
            allowDelete: function () {
                return self.allowDelete;
            },
        };

        self.proto.init();
    }]);