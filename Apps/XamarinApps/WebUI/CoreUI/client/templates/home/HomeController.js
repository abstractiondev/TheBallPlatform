/**
 * Created by Kalle on 1.9.2015.
 */
/// <reference path="../../../typings/angularjs/angular.d.ts" />
///<reference path="../../services/ConnectionService.ts"/>
var application;
(function (application) {
    var HomeController = (function () {
        function HomeController($scope, service) {
            this.hosts = [];
            this.connections = [];
            $scope.vm = this;
            this.currentHost = this.hosts[2];
            var me = this;
            service.getConnectionPrefillData().then(function (result) {
                var data = result.data;
                me.email = data.email;
                me.hosts = data.hosts;
            });
            service.getConnectionData().then(function (result) {
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
            alert(this.email + " for host: " + this.currentHost.value);
        };
        HomeController.$inject = ['$scope'];
        return HomeController;
    })();
    window.appModule.controller("HomeController", ["$scope", "ConnectionService", function ($scope, connectionService) { return new HomeController($scope, connectionService); }]);
})(application || (application = {}));
//# sourceMappingURL=HomeController.js.map