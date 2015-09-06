/**
 * Created by Kalle on 6.9.2015.
 */
/// <reference path="../../typings/angularjs/angular.d.ts" />
var application;
(function (application) {
    var ConnectionService = (function () {
        function ConnectionService() {
        }
        ConnectionService.prototype.getHelloWorld = function () {
            return "Hello! Are you still there..?";
        };
        return ConnectionService;
    })();
    application.ConnectionService = ConnectionService;
    window.appModule.service('ConnectionService', ["$http", "$location", function ($http, $location) { return new ConnectionService(); }]);
})(application || (application = {}));
//# sourceMappingURL=ConnectionService.js.map