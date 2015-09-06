/**
 * Created by Kalle on 1.9.2015.
 */
/// <reference path="../../../typings/angularjs/angular.d.ts" />
///<reference path="../../services/ConnectionService.ts"/>
var application;
(function (application) {
    var HomeController = (function () {
        function HomeController($scope, service) {
            this.hosts = [
                { displayName: "test.theball.me", value: "test.theball.me" },
                { displayName: "beta.diosphere.org", value: "beta.diosphere.org" },
                { displayName: "localhost", value: "localhost" },
            ];
            this.connections = [];
            $scope.vm = this;
            this.currentHost = this.hosts[2];
            this.email = service.getHelloWorld();
        }
        HomeController.prototype.hasConnections = function () {
            return this.connections.length > 0;
        };
        HomeController.prototype.isCreateFirstConnectionMode = function () {
            return !this.hasConnections();
        };
        HomeController.prototype.isManageConnectionsMode = function () {
            return this.hasConnections();
        };
        HomeController.prototype.CreateConnection = function () {
            alert(this.email + " for host: " + this.currentHost.value);
        };
        HomeController.$inject = ['$scope'];
        return HomeController;
    })();
    window.appModule.controller("HomeController", ["$scope", "ConnectionService", function ($scope, connectionService) { return new HomeController($scope, connectionService); }]);
})(application || (application = {}));
//# sourceMappingURL=HomeController.js.map