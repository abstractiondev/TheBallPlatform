/**
 * Created by Kalle on 7.9.2015.
 */
/// <reference path="../../typings/angularjs/angular.d.ts" />
var application;
(function (application) {
    var OperationService = (function () {
        function OperationService($http, $location, promiseCache) {
            this.$http = $http;
            this.promiseCache = promiseCache;
        }
        OperationService.prototype.executeOperation = function (operationName, operationParams) {
            var me = this;
            return this.promiseCache({
                promise: function () {
                    return me.$http.post("/op/" + operationName, operationParams);
                }
            });
        };
        return OperationService;
    })();
    application.OperationService = OperationService;
    window.appModule.factory('OperationService', ["$http", "$location", "promiseCache", function ($http, $location, promiseCache) { return new OperationService($http, $location, promiseCache); }]);
})(application || (application = {}));
//# sourceMappingURL=OperationService.js.map