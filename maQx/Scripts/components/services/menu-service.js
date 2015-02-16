angular.module("orangeApp").service('MenuService', ['$resource', function ($resource) {
    var self = this;
    self.menuList = null;

    var menuResource = $resource('/get/menus');

    function linq(object) {
        return Enumerable.from(object);
    }

    return {
        query: function () {
            if (self.menuList == null) {
                self.menuList = menuResource.get();

                console.log(self.menuList)
            }

            return self.menuList;
        },

        getMenu: function (id) {
            return linq(self.menuList.Menus).where("$.ID == '" + id + "'").toArray()[0]
        }
    };
}]);