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
        function OperationService($http, $location, $q, promiseCache) {
            this.$http = $http;
            this.$q = $q;
            this.promiseCache = promiseCache;
        }
        OperationService.prototype.executeOperation = function (operationName, operationParams) {
            var me = this;
            var wnd = window;
            if (wnd.TBJS2MobileBridge) {
                var stringParams = JSON.stringify(operationParams);
                var result = wnd.TBJS2MobileBridge.ExecuteAjaxOperation(operationName, stringParams);
                var data = JSON.parse(result);
                var deferred = me.$q.defer();
                deferred.resolve(data);
                return deferred.promise;
            }
            else {
                return this.promiseCache({
                    promise: function () {
                        /* iOS WebView requires the use of http(s)-protocol to provide body in POST request */
                        return me.$http.post("https://tbvirtualhost/op/" + operationName, operationParams);
                    }
                });
            }
        };
        return OperationService;
    })();
    application.OperationService = OperationService;
    window.appModule.factory('OperationService', ["$http", "$location", "$q", "promiseCache", function ($http, $location, $q, promiseCache) { return new OperationService($http, $location, $q, promiseCache); }]);
})(application || (application = {}));
//# sourceMappingURL=OperationService.js.map
///<reference path="..\..\services\OperationService.ts"/>
/// <reference path="../../../typings/angularjs/angular.d.ts" />
/// <reference path="../../services/ConnectionService.ts"/>
/// <reference path="../../../typings/lodash/lodash.d.ts" />
var application;
(function (application) {
    var ConnectionController = (function () {
        function ConnectionController($scope, connectionService, operationService, foundationApi) {
            this.operationService = operationService;
            this.foundationApi = foundationApi;
            this.hosts = [];
            this.connections = [];
            this.scope = $scope;
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
            $scope.$watch(function () { return me.connections; }, function () {
                me.scope.$evalAsync(function () {
                    me.refreshIsotope();
                });
            });
        }
        /*
        LastOperationDump:string = "void";
        */
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
            me.operationService.executeOperation("TheBall.LocalApp.GoToConnection", { "connectionID": connectionID });
        };
        ConnectionController.prototype.DeleteConnection = function (connectionID) {
            var me = this;
            me.foundationApi.publish('main-notifications', { title: 'Deleting Connection', content: connectionID, autoclose: "3000", color: "alert" });
            return;
            this.operationService.executeOperation("TheBall.LocalApp.DeleteConnection", { "connectionID": connectionID }); /*.then(data => me.LastOperationDump = JSON.stringify(data));*/
        };
        ConnectionController.$inject = ['$scope'];
        return ConnectionController;
    })();
    window.appModule.controller("ConnectionController", ["$scope", "ConnectionService", "OperationService", "FoundationApi", function ($scope, connectionService, operationService, foundationApi) { return new ConnectionController($scope, connectionService, operationService, foundationApi); }]);
})(application || (application = {}));
//# sourceMappingURL=ConnectionController.js.map