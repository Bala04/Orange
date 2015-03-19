(function () {
    'use strict';

    angular
        .module("sectionApp")
        .controller("ProductProcessController", ProductProcessController)
        .controller("ProcessSelectionController", ProcessSelectionController);

    ProductProcessController.$inject = ["PromiseFactory", '$modal'];
    ProcessSelectionController.$inject = ['$scope', 'PromiseFactory', '$modalInstance', 'items'];

    function ProductProcessController(PromiseFactory, $modal) {
        var self = this;

        self.product = "-1";
        self.isLoading = false;
        self.isLoaded = false;
        self.processList = [];
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
            addProcess: addProcess
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
            PromiseFactory.Resolve(["/get/Process/"]).then(handleInitResponse).finally(endResponse);
        }

        function handleInitResponse(result) {
            self.processList = result[0].data.List;
            self.allowCreate = result[0].data.AllowCreate;
            self.isLoaded = true;
        }

        function handleProductProcessResponse(result) {
            setProductProcess(getProduct(), result[0].data.List);
        }

        function endResponse() {
            self.isLoading = false;
        }

        function loadProduct(value) {
            self.productList = value;
        }

        function getProcessList() {
            return _app.clone(self.processList);
        }

        function getProduct() {
            return linq(self.productList).where("$.value=='" + self.product + "'").firstOrDefault();
        }

        function setProductProcess(product, mappedList) {
            product.processList = [];
            angular.forEach(getProcessList(), function (process) {
                var mapped = linq(mappedList).where("$.Process.Key=='" + process.Key + "'").firstOrDefault();
                process.mapped = mapped;
                process.selected = mapped != null;
                process.order = mapped != null ? mapped.Order : 0;
                product.processList.push(process);
            });
        }

        function change() {
            var product = getProduct();
            if (!product.processList) {
                self.isLoading = true;
                PromiseFactory.Resolve(["/get/ProductProcess/" + product.value]).then(handleProductProcessResponse).finally(endResponse);
            }
        }

        function allowCreate() {
            return self.allowCreate;
        }

        function addProcess(product) {
            product._processList = _app.clone(product.processList);
            $modal.open({
                templateUrl: 'modal-selector.html',
                controller: 'ProcessSelectionController',
                size: 'lg',
                backdrop: 'static',
                keyboard: false,
                resolve: {
                    items: function () {
                        return {
                            product: product,
                            setProductProcess: setProductProcess
                        }
                    }
                }
            });
        }
    }

    function ProcessSelectionController($scope, PromiseFactory, $modalInstance, items) {
        $scope.isLoading = false;
        $scope.items = items;
        $scope.cancel = cancel;
        $scope.ok = ok;
        $scope.selectChange = selectChange;
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
                Entity: $scope.items.product.value,
                Update: []
            };

            angular.forEach($scope.items.product._processList, function (modifiedProcess) {
                var process = linq($scope.items.product.processList).where("$.Key=='" + modifiedProcess.Key + "'").firstOrDefault();
                if (process.selected && modifiedProcess.selected) {
                    request.Update.push({ Key: process.mapped.Key, Order: modifiedProcess.order });
                } else if (!process.selected && modifiedProcess.selected) {
                    request.Add.push({ Key: process.Key, Order: modifiedProcess.order })
                } else if (process.selected && !modifiedProcess.selected) {
                    request.Remove.push({ Key: process.mapped.Key });
                }
            });

            if (request.Add.length > 0 || request.Remove.length > 0 || request.Update.length > 0) {
                $scope.isLoading = true;
                PromiseFactory.Resolve(["/get/Exists/" + _app.escapeHtml($scope.items.product.value) + "?type=update-product-process&ref=" + JSON.stringify(request)]).then(handleResponse).finally(endResponse);
            } else {
                cancel();
            }
        }

        function endResponse() {
            $scope.isLoading = false;
        }

        function handleResponse(result) {
            $scope.items.setProductProcess($scope.items.product, result[0].data.List);
            cancel();
        }

        function selectChange(process) {
            if (process.selected)
                process.order = linq($scope.items.product._processList).max("$.order") + 1;
            else {
                process.order = 0;
                reorder();
            }
        }

        function reorder() {
            var processList = linq($scope.items.product._processList).where("$.selected").orderBy("$.order").toArray();
            for (var i = 0; i < processList.length; i++) {
                processList[i].order = (i + 1);
            }
        }
    }
})();

