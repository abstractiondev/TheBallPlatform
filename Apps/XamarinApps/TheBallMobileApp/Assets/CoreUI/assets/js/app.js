var appModule;

(function() {
  'use strict';

  appModule = angular.module('application', [
    'ui.router',
    'ngAnimate',

    //foundation
    'foundation',
    'foundation.dynamicRouting',
    'foundation.dynamicRouting.animations',

    // 3rd party
    'angular-promise-cache',
    'mm.foundation',
    //'blockUI'
    //'dynamicLayout',
    //'iso.directives'
  ])
    .config(config)
    .constant("_", window._)
    .run(run)
  ;

  config.$inject = ['$urlRouterProvider', '$locationProvider', '$controllerProvider'];

  function config($urlProvider, $locationProvider, $controllerProvider) {
    $urlProvider.otherwise('/');

    $locationProvider.html5Mode({
      enabled:false,
      requireBase: false
    });

    $locationProvider.hashPrefix('!');

    //$controllerProvider.allowGlobals();
  }

  function run($rootScope) {
    $rootScope._ = window._;
    FastClick.attach(document.body);
  }

})();

/**
 * Created by Kalle on 6.9.2015.
 */
/// <reference path="../../typings/angularjs/angular.d.ts" />
var application;
(function (application) {
    var ConnectionService = (function () {
        function ConnectionService($http, $location, promiseCache) {
            this.$http = $http;
            this.promiseCache = promiseCache;
        }
        ConnectionService.prototype.getConnectionPrefillData = function () {
            var me = this;
            return this.promiseCache({
                promise: function () {
                    return me.$http.get('/data/ConnectionHosts.json');
                }
            });
        };
        ConnectionService.prototype.getConnectionData = function () {
            var me = this;
            return this.promiseCache({
                promise: function () {
                    return me.$http.get('/data/Connections.json');
                }
            });
        };
        return ConnectionService;
    })();
    application.ConnectionService = ConnectionService;
    window.appModule.factory('ConnectionService', ["$http", "$location", "promiseCache", function ($http, $location, promiseCache) { return new ConnectionService($http, $location, promiseCache); }]);
})(application || (application = {}));
//# sourceMappingURL=ConnectionService.js.map
/**
 * Created by Kalle on 7.9.2015.
 */
/// <reference path="../../typings/angularjs/angular.d.ts" />
var application;
(function (application) {
    var OperationService = (function () {
        function OperationService($http, $location, $q, $timeout, promiseCache) {
            this.$http = $http;
            this.$location = $location;
            this.$q = $q;
            this.$timeout = $timeout;
            this.promiseCache = promiseCache;
        }
        OperationService.SuccessPendingOperation = function (operationID, successParamsString) {
            var deferredData = OperationService.pendingOperations[operationID];
            var deferred = deferredData.deferred;
            //deferredData.serviceInstance.blockUI.stop();
            var wnd = window;
            var successParams = null;
            if (successParamsString)
                successParams = JSON.parse(successParamsString);
            deferred.resolve(successParams);
            delete OperationService.pendingOperations[operationID];
        };
        OperationService.FailPendingOperation = function (operationID, failParamsString) {
            var deferredData = OperationService.pendingOperations[operationID];
            var deferred = deferredData.deferred;
            var wnd = window;
            wnd.$.unblockUI();
            var failParams = JSON.parse(failParamsString);
            deferred.reject(failParams);
            //deferredData.serviceInstance.blockUI.stop();
            delete OperationService.pendingOperations[operationID];
        };
        OperationService.ProgressPendingOperation = function (operationID, progressParamsString) {
            var deferredData = OperationService.pendingOperations[operationID];
            var deferred = deferredData.deferred;
            //var progressParams = JSON.parse(progressParamsString);
            var progressParams = { progress: parseFloat(progressParamsString) };
            deferred.notify(progressParams);
        };
        OperationService.prototype.executeOperation = function (operationName, operationParams) {
            var me = this;
            var wnd = window;
            var simulateServiceProgress = me.$location.protocol() == "http" && me.$location.host() == "localhost";
            if (wnd.TBJS2MobileBridge) {
                var stringParams = JSON.stringify(operationParams);
                var result = wnd.TBJS2MobileBridge.ExecuteAjaxOperation(operationName, stringParams);
                var resultObj = JSON.parse(result);
                var operationID = resultObj.OperationResult;
                //var success =
                var deferred = me.$q.defer();
                OperationService.pendingOperations[operationID] = { deferred: deferred, serviceInstance: me };
                return deferred.promise;
            }
            else if (!simulateServiceProgress) {
                var deferred = me.$q.defer();
                me.$http.post("https://tbvirtualhost/op/" + operationName, operationParams).then(function (response) {
                    var resultObj = JSON.parse(response.data);
                    var operationID = resultObj.OperationResult;
                    OperationService.pendingOperations[operationID] = { deferred: deferred, serviceInstance: me };
                });
                return deferred.promise;
            }
            else {
                var deferred = me.$q.defer();
                var progressCurrent = 0;
                var progressSimulation = function () {
                    progressCurrent += 5;
                    if (progressCurrent >= 100) {
                        deferred.resolve();
                    }
                    else {
                        deferred.notify({ progress: progressCurrent / 100, statusMessage: "Proceeding: " + progressCurrent });
                        me.$timeout(progressSimulation, 200);
                    }
                    //else
                    //  me.foundationApi.publish("progressBarModal", "close");
                };
                me.$timeout(progressSimulation, 200);
                return deferred.promise;
            }
        };
        OperationService.pendingOperations = {};
        return OperationService;
    })();
    application.OperationService = OperationService;
    window.appModule.factory('OperationService', ["$http", "$location", "$q", "$timeout", "promiseCache", function ($http, $location, $q, $timeout, promiseCache) { return new OperationService($http, $location, $q, $timeout, promiseCache); }]);
})(application || (application = {}));
//# sourceMappingURL=OperationService.js.map
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
            me.foundationApi.publish("progressBarModal", "open");
            me.operationService.executeOperation("TheBall.LocalApp.GoToConnection", { "connectionID": connectionID }).then(function (successData) {
                me.foundationApi.publish("progressBarModal", "close");
            }, function (failedData) { return me.LastOperationDump = "Failed: " + JSON.stringify(failedData); }, function (updateData) {
                me.scope.progressCurrent = me.scope.progressMax * updateData.progress;
            });
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