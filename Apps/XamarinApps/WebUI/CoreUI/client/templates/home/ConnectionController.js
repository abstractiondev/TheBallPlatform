///<reference path="..\..\services\OperationService.ts"/>
/// <reference path="../../../typings/angularjs/angular.d.ts" />
/// <reference path="../../services/ConnectionService.ts"/>
/// <reference path="../../../typings/lodash/lodash.d.ts" />
var application;
(function (application) {
    var ConnectionController = (function () {
        function ConnectionController($scope, connectionService, operationService, foundationApi, $timeout) {
            this.operationService = operationService;
            this.foundationApi = foundationApi;
            this.$timeout = $timeout;
            this.hosts = [];
            this.connections = [];
            this.LastOperationDump = "void";
            this.scope = $scope;
            $scope.vm = this;
            $scope.progressMax = 300;
            $scope.progressCurrent = 0;
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
            $scope.$watch(function () { return me.connections; }, function () {
                me.scope.$evalAsync(function () {
                    me.refreshIsotope();
                });
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
        ConnectionController.prototype.refreshIsotope = function () {
            var elem = window.document.querySelector(".isotope-container");
            if (!elem)
                return;
            var wnd = window;
            var iso = new wnd.Isotope(elem, {});
        };
        ConnectionController.prototype.CreateConnection = function () {
            var me = this;
            var host = me.currentHost != null ? me.currentHost.value : "";
            var email = me.email;
            this.operationService.executeOperation("TheBall.LocalApp.CreateConnection", {
                "host": host,
                "email": email
            }); /* .then(data => me.LastOperationDump = JSON.stringify(data));*/
        };
        ConnectionController.prototype.GoToConnection = function (connectionID) {
            var me = this;
            me.operationService.executeOperation("TheBall.LocalApp.GoToConnection", { "connectionID": connectionID }).then(function (successData) { return me.LastOperationDump = JSON.stringify(successData); }, function (failedData) { return me.LastOperationDump = "Failed: " + JSON.stringify(failedData); }, function (updateData) { return me.LastOperationDump = "Update: " + JSON.stringify(updateData); });
        };
        ConnectionController.prototype.UpdateTimeOut = function () {
            setTimeout(this.UpdateTimeOut, 1000);
        };
        ConnectionController.prototype.DeleteConnection = function (connectionID) {
            var wnd = window;
            var me = this;
            //(<any>$("#progressBarModal")).foundation("reveal", "open");
            //(<any>$("#progressBarModal")).data("revealInit").close_on_background_click = false;
            me.foundationApi.publish("progressBarModal", "open");
            var repeat = function () {
                me.scope.progressCurrent += 10;
                if (me.scope.progressCurrent < me.scope.progressMax)
                    me.$timeout(repeat, 200);
                //else
                //  me.foundationApi.publish("progressBarModal", "close");
            };
            me.$timeout(repeat, 200);
            return;
            me.foundationApi.publish('main-notifications', { title: 'Deleting Connection', content: connectionID, autoclose: "3000", color: "alert" });
            this.operationService.executeOperation("TheBall.LocalApp.DeleteConnection", { "connectionID": connectionID }); /*.then(data => me.LastOperationDump = JSON.stringify(data));*/
        };
        ConnectionController.$inject = ['$scope'];
        return ConnectionController;
    })();
    window.appModule.controller("ConnectionController", ["$scope", "ConnectionService", "OperationService", "FoundationApi", "$timeout", function ($scope, connectionService, operationService, foundationApi, $timeout) { return new ConnectionController($scope, connectionService, operationService, foundationApi, $timeout); }]);
})(application || (application = {}));
//# sourceMappingURL=ConnectionController.js.map