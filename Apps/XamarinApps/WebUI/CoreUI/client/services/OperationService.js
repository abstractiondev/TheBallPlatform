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
                        return me.$http.post("/op/" + operationName, operationParams);
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