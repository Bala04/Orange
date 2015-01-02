angular.module("orangeApp")
    .controller('MenuController', ['$rootScope', 'MenuService', '$timeout', function ($rootScope, MenuService, $timeout) {
        var self = this;
        self.currentMenu = null;
        self.loadingMenu = null;
        self.loaded = false;

        function loadFrame(menu) {
            $timeout(function myfunction() {              
                $rootScope.$broadcast("FrameLoading", menu);
            });
        }

        function init() {
            var menuQuery = MenuService.query()

            if (menuQuery != null) {
                menuQuery.$promise.then(function (data) {
                    if (data.Type == "SUCCESS") {
                        self.menus = data.Menus;
                        if (self.menus != null) {
                            self.currentMenu = self.menus[0].ID;
                            loadFrame(self.menus[0]);
                            self.loaded = true;
                        }
                    } else if (data.Type == "ERROR") {
                        console.log(data);
                    }
                }, function (error) {
                    console.log(error)
                });
            }
        };

        self.isLoaded = function () {
            return self.loaded;
        };

        self.selectMenu = function (menu) {
            self.currentMenu = menu.ID;
            loadFrame(menu);
        }

        self.selectedMenu = function (menu) {
            return self.currentMenu == menu.ID ? self.loadingMenu == menu.ID ? "selected load" : "selected" : "";
        };

        $rootScope.$on("AppFrameLoaded", function (event, menuID) {
            $timeout(function myfunction() {               
                self.currentMenu = menuID;
                $rootScope.$broadcast("FrameLoaded", MenuService.getMenu(menuID));
            });
            self.loadingMenu = null;
        });

        $rootScope.$on("FrameLoading", function (event, menu) {
            self.loadingMenu = menu.ID;
        });

        init();
    }]);