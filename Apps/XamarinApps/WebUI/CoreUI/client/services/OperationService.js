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
        OperationService.SuccessPendingOperation = function (operationID, successParams) {
            var deferredData = OperationService.pendingOperations[operationID];
            var deferred = deferredData.deferred;
            //deferredData.serviceInstance.blockUI.stop();
            var wnd = window;
            wnd.$.unblockUI();
            deferred.resolve(successParams);
            delete OperationService.pendingOperations[operationID];
        };
        OperationService.FailPendingOperation = function (operationID, failParams) {
            var deferredData = OperationService.pendingOperations[operationID];
            var deferred = deferredData.deferred;
            var wnd = window;
            wnd.$.unblockUI();
            deferred.reject(failParams);
            //deferredData.serviceInstance.blockUI.stop();
            delete OperationService.pendingOperations[operationID];
        };
        OperationService.ProgressPendingOperation = function (operationID, progressParams) {
            var deferredData = OperationService.pendingOperations[operationID];
            var deferred = deferredData.deferred;
            deferred.notify(progressParams);
        };
        OperationService.prototype.executeOperation = function (operationName, operationParams) {
            var me = this;
            var wnd = window;
            if (wnd.TBJS2MobileBridge) {
                var stringParams = JSON.stringify(operationParams);
                var result = wnd.TBJS2MobileBridge.ExecuteAjaxOperation(operationName, stringParams);
                var resultObj = JSON.parse(result);
                var operationID = resultObj.OperationResult;
                //var success =
                var deferred = me.$q.defer();
                OperationService.pendingOperations[operationID] = { deferred: deferred, serviceInstance: me };
                wnd.$.blockUI({ message: "Piip: " + operationID });
                return deferred.promise;
            }
            else {
                wnd.$.blockUI({ message: "Piip!" });
                return this.promiseCache({
                    promise: function () {
                        /* iOS WebView requires the use of http(s)-protocol to provide body in POST request */
                        return me.$http.post("https://tbvirtualhost/op/" + operationName, operationParams);
                    }
                });
            }
        };
        OperationService.pendingOperations = {};
        return OperationService;
    })();
    application.OperationService = OperationService;
    window.appModule.factory('OperationService', ["$http", "$location", "$q", "promiseCache", function ($http, $location, $q, promiseCache) { return new OperationService($http, $location, $q, promiseCache); }]);
})(application || (application = {}));
//# sourceMappingURL=OperationService.js.map