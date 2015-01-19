angular.module("orangeApp")
    .controller('MenuController', ['$rootScope', 'MenuService', '$timeout', '$location', function ($rootScope, MenuService, $timeout, $location) {
        var self = this;
        self.currentMenu = null;
        self.loadingMenu = null;
        self.loaded = false;
        self.menus = [];

        function loadFrame(value) {
            if (value.menu !== undefined) {
                self.currentMenu = value.menu.ID;

                $timeout(function () {
                    $rootScope.$broadcast("FrameLoading", value.url);
                    self.loadingMenu = value.menu.ID;
                });
            }
        }

        function init() {
            var menuQuery = MenuService.query()

            if (menuQuery != null) {
                menuQuery.$promise.then(function (data) {
                    if (data.Type == "SUCCESS") {
                        self.menus = data.Menus;
                        if (self.menus != null && self.menus.length > 0) {
                            var path = $location.path();
                            var menuID = path.split('/')[1];

                            if (path == "" || menuID === undefined) {
                                loadFrame({ menu: self.menus[0], url: "/" + self.menus[0].ID });
                            } else {
                                loadFrame({ menu: linq(self.menus).where('$.ID=="' + menuID + '"').toArray()[0], url: path });
                            }

                            self.loaded = true;
                        }
                    } else {
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
            loadFrame({ menu: menu, url: "/" + menu.ID });
        }

        self.selectedMenu = function (menu) {
            return self.currentMenu == menu.ID ? self.loadingMenu == menu.ID ? "selected load" : "selected" : "";
        };

        $rootScope.$on("AppFrameLoaded", function (event, frame) {
            $timeout(function () {
                self.currentMenu = frame.menuID;
                $rootScope.$broadcast("FrameLoaded", MenuService.getMenu(frame.menuID));
            });
            self.loadingMenu = null;
        });

        $rootScope.$on("LoadFrame", function (event, path) {
            var menuID = path.split('/')[1];

            if (menuID !== undefined) {
                loadFrame({ menu: linq(self.menus).where('$.ID=="' + menuID + '"').toArray()[0], url: path });
            }
        });

        init();
    }]);