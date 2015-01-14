angular.module("sectionApp").factory("Departments", function ($resource) {
    return $resource(
        "/Context/Departments/:ID", { ID: "@ID" }, {
            "update": { method: "PUT" },
        }
    );
});