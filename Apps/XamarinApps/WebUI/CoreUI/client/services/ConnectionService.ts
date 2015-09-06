/**
 * Created by Kalle on 6.9.2015.
 */

/// <reference path="../../typings/angularjs/angular.d.ts" />

module application {
  export class ConnectionService {
    public getHelloWorld():string {
      return "Hello! Are you still there..?";
    }
  }

  (<any>window).appModule.service('ConnectionService', ["$http", "$location", ($http, $location)
    => new ConnectionService()]);

}

