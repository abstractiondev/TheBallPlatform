///<reference path="..\..\services\OperationService.ts"/>
/// <reference path="../../../typings/angularjs/angular.d.ts" />
/// <reference path="../../services/ConnectionService.ts"/>
var application;
(function (application) {
    var ConnectionController = (function () {
        function ConnectionController($scope, connectionService, operationService) {
            this.operationService = operationService;
            this.hosts = [];
            this.connections = [];
            this.LastOperationDump = "void";
            $scope.vm = this;
            //this.currentHost = this.hosts[2];
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
        ConnectionController.prototype.hasConnections = function () {
            return this.connections.length > 0;
        };
        ConnectionController.prototype.isCreateFirstConnectionMode = function () {
            return !this.hasConnections();
        };
        ConnectionController.prototype.isManageConnectionsMode = function () {
            return this.hasConnections();
        };
        ConnectionController.prototype.CreateConnection = function () {
            var me = this;
            var host = me.currentHost != null ? me.currentHost.value : "";
            var email = me.email;
            this.operationService.executeOperation("TheBall.LocalApp.CreateConnection", {
                "host": host,
                "email": email
            }).then(function (data) { return me.LastOperationDump = JSON.stringify(data); });
        };
        ConnectionController.prototype.DeleteConnection = function (connectionID) {
            var me = this;
            this.operationService.executeOperation("TheBall.LocalApp.DeleteConnection", { "connectionID": connectionID }).then(function (data) { return me.LastOperationDump = JSON.stringify(data); });
        };
        ConnectionController.$inject = ['$scope'];
        return ConnectionController;
    })();
    window.appModule.controller("ConnectionController", ["$scope", "ConnectionService", "OperationService", function ($scope, connectionService, operationService) { return new ConnectionController($scope, connectionService, operationService); }]);
})(application || (application = {}));
//# sourceMappingURL=ConnectionController.js.map