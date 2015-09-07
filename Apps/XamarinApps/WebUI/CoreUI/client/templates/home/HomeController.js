///<reference path="..\..\services\OperationService.ts"/>
/// <reference path="../../../typings/angularjs/angular.d.ts" />
/// <reference path="../../services/ConnectionService.ts"/>
var application;
(function (application) {
    var HomeController = (function () {
        function HomeController($scope, connectionService, operationService) {
            this.operationService = operationService;
            this.hosts = [];
            this.connections = [];
            $scope.vm = this;
            this.currentHost = this.hosts[2];
            var me = this;
            connectionService.getConnectionPrefillData().then(function (result) {
                var data = result.data;
                me.email = data.email;
                me.hosts = data.hosts;
            });
            connectionService.getConnectionData().then(function (result) {
                var data = result.data;
                me.connections = data.connections;
            });
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
            this.operationService.executeOperation("TheBall.LocalApp.CreateConnection", {
                "host": this.currentHost.hostname,
                "email": this.email
            });
        };
        HomeController.prototype.DeleteConnection = function (connectionID) {
            this.operationService.executeOperation("TheBall.LocalApp.DeleteConnection", { "connectionID": connectionID });
        };
        HomeController.$inject = ['$scope'];
        return HomeController;
    })();
    window.appModule.controller("HomeController", ["$scope", "ConnectionService", "OperationService", function ($scope, connectionService, operationService) { return new HomeController($scope, connectionService, operationService); }]);
})(application || (application = {}));
//# sourceMappingURL=HomeController.js.map