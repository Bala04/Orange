(function () {
    'use strict';

    angular
        .module("sectionApp")
        .controller("ShiftController", ShiftController);

    ShiftController.$inject = ["$scope"];

    function ShiftController($scope) {
        var self = this;

        console.log($("#StartTime").data("init"));

        self.startTime = moment($("#StartTime").data("init"));
        self.endTime = moment($("#EndTime").data("init"));

        self.proto = {
            changed: changed,
            parseDateTime: parseDateTime
        }

        function parseDateTime(datetime) {
            return Date.parse(datetime);
        }

        function changed() {

        }
    }
})();