/**
 * Created by Kalle on 7.9.2015.
 */

/// <reference path="../../typings/angularjs/angular.d.ts" />

module application {
   interface IDeferredData {
   deferred:ng.IDeferred<any>;
   serviceInstance:OperationService;
   }

  export class OperationService {


    private static pendingOperations: { [operationID: string] : IDeferredData; } = {};
     private blockUI:any;

     public static SuccessPendingOperation(operationID:string, successParams:any)
     {
       var deferredData = OperationService.pendingOperations[operationID];
       var deferred = deferredData.deferred;
       //deferredData.serviceInstance.blockUI.stop();
       var wnd:any = window;
       wnd.$.unblockUI();
       deferred.resolve(successParams);
       delete OperationService.pendingOperations[operationID];
     }

     public static FailPendingOperation(operationID:string, failParams:any)
     {
       var deferredData = OperationService.pendingOperations[operationID];
       var deferred = deferredData.deferred;
       var wnd:any = window;
       wnd.$.unblockUI();
       deferred.reject(failParams);
       //deferredData.serviceInstance.blockUI.stop();
       delete OperationService.pendingOperations[operationID];
     }

     public static ProgressPendingOperation(operationID:string, progressParams:any)
     {
       var deferredData = OperationService.pendingOperations[operationID];
       var deferred = deferredData.deferred;
       deferred.notify(progressParams);
     }

    public executeOperation(operationName:string, operationParams:any):any {
      var me = this;
      var wnd:any = window;
      var simulateServiceProgress:boolean = me.$location.protocol() == "http" && me.$location.host() == "localhost";
      if(wnd.TBJS2MobileBridge) {
        var stringParams = JSON.stringify(operationParams);
        var result = wnd.TBJS2MobileBridge.ExecuteAjaxOperation(operationName, stringParams);
        var resultObj = JSON.parse(result);
        var operationID = resultObj.OperationResult;
        //var success =
        var deferred = me.$q.defer();
        OperationService.pendingOperations[operationID] = { deferred: deferred, serviceInstance: me };
        return deferred.promise;
      } else if(!simulateServiceProgress) {
        wnd.$.blockUI( { message: "Piip!"});
        return this.promiseCache({
          promise: function() {
            /* iOS WebView requires the use of http(s)-protocol to provide body in POST request */
            return me.$http.post("https://tbvirtualhost/op/" + operationName,
              operationParams);
          }
        });
      } else { // simulate service progress
        var deferred = me.$q.defer();
        var progressCurrent:number = 0;
        var progressSimulation = function() {
          progressCurrent += 5;
          if(progressCurrent >= 100) {
            deferred.resolve();
          }
          else {
            deferred.notify({ progress: progressCurrent / 100, statusMessage: "Proceeding: " + progressCurrent});
            me.$timeout(progressSimulation, 200)
          }
          //else
          //  me.foundationApi.publish("progressBarModal", "close");
        };
        me.$timeout(progressSimulation, 200);
        return deferred.promise;
      }
    }

    constructor(private $http, private $location, private $q, private $timeout, private promiseCache) {

    }
  }


  (<any>window).appModule.factory('OperationService', ["$http", "$location", "$q", "$timeout", "promiseCache", ($http, $location, $q, $timeout, promiseCache)
    => new OperationService($http, $location, $q, $timeout, promiseCache)]);

}


