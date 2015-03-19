(function () {
    'use strict';

    angular
        .module("sectionApp")
        .controller("ProductProcessBaseController", ProductProcessBaseController)
        .controller("EntitySelectionController", EntitySelectionController);

    ProductProcessBaseController.$inject = ["PromiseFactory", '$modal'];
    EntitySelectionController.$inject = ['$scope', 'PromiseFactory', '$modalInstance', 'items'];

    function ProductProcessBaseController(PromiseFactory, $modal) {
        var self = this;
        self.entityType = $("#entity").data("entity");
        self.product = "-1";
        self.isLoading = false;
        self.isLoaded = false;
        self.entityList = [];
        self.productList = [];
        self.loadProduct = loadProduct;
        self.allowCreate = false;

        self.proto = {
            isLoading: isLoading,
            isLoaded: isLoaded,
            showWarning: showWarning,
            showContent: showContent,
            change: change,
            allowCreate: allowCreate,
            getEntityCount: getEntityCount,
            getMappedEntityCount:getMappedEntityCount,
            addEntity: addEntity,
        };

        init();

        function isLoading() {
            return self.isLoading;
        }

        function isLoaded() {
            return self.isLoaded;
        }

        function showWarning() {
            return self.product == "-1" && !self.isLoading;
        }

        function showContent() {
            return self.product != "-1" && !self.isLoading && self.isLoaded;
        }

        function init() {
            self.isLoading = true;
            PromiseFactory.Resolve(["/get/" + self.entityType + "s/"]).then(handleInitResponse).finally(endResponse);
        }

        function handleInitResponse(result) {
            self.entityList = result[0].data.List;
            self.allowCreate = result[0].data.AllowCreate;
            self.isLoaded = true;
        }

        function handleProductProcessResponse(result) {
            setProductProcess(getProduct(), result[0].data.List, result[1].data.List);
        }

        function endResponse() {
            self.isLoading = false;
        }

        function loadProduct(value) {
            self.productList = value;
        }

        function getProduct() {
            return linq(self.productList).where("$.value=='" + self.product + "'").firstOrDefault();
        }

        function change() {
            var product = getProduct();
            if (!product.processList) {
                self.isLoading = true;
                PromiseFactory.Resolve(["/get/ProductProcess/" + product.value, "/get/ProductProcess" + self.entityType + "s/" + product.value]).then(handleProductProcessResponse).finally(endResponse);
            }
        }

        function allowCreate() {
            return self.allowCreate;
        }


        function updateProductProcess(process, mappedList) {
            process.entityList = [];
            angular.forEach(getEntityList(), function (entity) {
                var mapped = linq(mappedList).where("$." + self.entityType + ".Key=='" + entity.Key + "'").firstOrDefault();
                if (mapped != null) {
                    entity.mapper = mapped.Key;
                    entity.selected = true;
                } else {
                    entity.mapper = null;
                    entity.selected = false;
                }
                process.entityList.push(entity);
            });
        }

        function setProductProcess(product, processList, mappedList) {
            product.processList = processList;
            angular.forEach(processList, function (process) {
                updateProductProcess(process, linq(mappedList).where("$.ProductProcess.Key=='" + process.Key + "'").toArray());
            });
        }

        function getEntityCount() {
            return self.entityList.length;
        }

        function getMappedEntityCount() {
            var product = getProduct();
            var i = 0;
            var selectedEntity = [];
            if (product && product.processList) {
                angular.forEach(product.processList, function (process) {
                    angular.forEach(getEntityList(), function (entity) {
                        if (linq(process.entityList).where("$.selected && $.Key=='" + entity.Key + "'").any() && !linq(selectedEntity).contains(entity.Key)) {
                            selectedEntity.push(entity.Key);
                            i++;
                        }
                    });
                });
            }
            return i;
        }

        function getEntityList() {
            return _app.clone(self.entityList);
        }

        function addEntity(product, process) {
            process._entityList = _app.clone(process.entityList);
            $modal.open({
                templateUrl: 'modal-selector.html',
                controller: 'EntitySelectionController',
                size: 'lg',
                backdrop: 'static',
                keyboard: false,
                resolve: {
                    items: function () {
                        return {
                            product: product,
                            process: process,
                            updateProductProcess: updateProductProcess,
                            entityType: self.entityType,
                        }
                    }
                }
            });
        }
    }

    function EntitySelectionController($scope, PromiseFactory, $modalInstance, items) {
        $scope.isLoading = false;
        $scope.items = items;
        $scope.cancel = cancel;
        $scope.ok = ok;
        $scope.Loading = Loading;

        function Loading() {
            return $scope.isLoading;
        }

        function cancel() {
            $modalInstance.dismiss('cancel');
        }

        function ok() {
            var request = {
                Add: [],
                Remove: [],
                Entity: $scope.items.process.Key,
            };

            angular.forEach($scope.items.process._entityList, function (modifiedEntity) {
                var entity = linq($scope.items.process.entityList).where("$.Key=='" + modifiedEntity.Key + "'").firstOrDefault();
                if (!entity.selected && modifiedEntity.selected) {
                    request.Add.push(entity.Key)
                } else if (entity.selected && !modifiedEntity.selected) {
                    request.Remove.push(entity.mapper);
                }
            });

            if (request.Add.length > 0 || request.Remove.length > 0) {
                $scope.isLoading = true;
                PromiseFactory.Resolve(["/get/Exists/" + _app.escapeHtml($scope.items.process.Key) + "?type=add-product-process-" + $scope.items.entityType + "s&ref=" + JSON.stringify(request)]).then(handleResponse).finally(endResponse);
            } else {
                cancel();
            }
        }

        function endResponse() {
            $scope.isLoading = false;
        }

        function handleResponse(result) {
            $scope.items.updateProductProcess($scope.items.process, result[0].data.List);
            cancel();
        }
    }
})();