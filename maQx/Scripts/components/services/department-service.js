angular.module("sectionApp").factory("Departments", function ($resource) {
    return $resource(
        "/Context/Departments/:ID", { ID: "@Key" }, {
            "update": { method: "PUT" },
        }
    );
});