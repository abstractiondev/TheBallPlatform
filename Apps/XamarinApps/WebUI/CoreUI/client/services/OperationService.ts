/**
 * Created by Kalle on 7.9.2015.
 */

/// <reference path="../../typings/angularjs/angular.d.ts" />

module application {
  export class OperationService {
    public executeOperation(operationName:string, operationParams:any):any {
      var me = this;
      var wnd:any = window;
      if(wnd.TBJS2MobileBridge) {
        var stringParams = JSON.stringify(operationParams);
        var result = wnd.TBJS2MobileBridge.ExecuteAjaxOperation(operationName, stringParams);
        var data = JSON.parse(result);
        var deferred = me.$q.defer();
        deferred.resolve(data);
        return deferred.promise;
      } else {
        return this.promiseCache({
          promise: function() {
            /* iOS WebView requires the use of http(s)-protocol to provide body in POST request */
            return me.$http.post("https://tbvirtualhost/op/" + operationName,
              operationParams);
          }
        });
      }
    }

    constructor(private $http, $location, private $q, private promiseCache) {

    }
  }


  (<any>window).appModule.factory('OperationService', ["$http", "$location", "$q", "promiseCache", ($http, $location, $q, promiseCache)
    => new OperationService($http, $location, $q, promiseCache)]);

}

