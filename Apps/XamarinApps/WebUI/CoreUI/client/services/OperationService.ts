/**
 * Created by Kalle on 7.9.2015.
 */

/// <reference path="../../typings/angularjs/angular.d.ts" />

module application {
  export class OperationService {
    public executeOperation(operationName:string, operationParams:any):any {
      var me = this;
      return this.promiseCache({
        promise: function() {
          return me.$http.post("/op/" + operationName,
            operationParams);
        }
      });
    }

    constructor(private $http, $location, private promiseCache) {

    }
  }


  (<any>window).appModule.factory('OperationService', ["$http", "$location", "promiseCache", ($http, $location, promiseCache)
    => new OperationService($http, $location, promiseCache)]);

}

