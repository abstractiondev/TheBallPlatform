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