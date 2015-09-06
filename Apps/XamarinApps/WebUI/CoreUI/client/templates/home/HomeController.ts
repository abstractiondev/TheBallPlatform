/**
 * Created by Kalle on 1.9.2015.
 */

/// <reference path="../../../typings/angularjs/angular.d.ts" />
///<reference path="../../services/ConnectionService.ts"/>

module application {


  interface IHomeController {
    CreateConnection:()=>void;
    isCreateFirstConnectionMode:()=>Boolean;
    isManageConnectionsMode:()=>Boolean;
  }

  class HomeController implements IHomeController {
    static $inject = ['$scope'];

    hosts = [];

    currentHost:any;

    connections = [];

    email:string;

    hasConnections():Boolean {
      return this.connections.length > 0;
    }

    isCreateFirstConnectionMode():Boolean {
      return !this.hasConnections();
    }

    isManageConnectionsMode():Boolean {
      return this.hasConnections();
    }

    constructor($scope, service:ConnectionService) {
      $scope.vm = this;
      this.currentHost = this.hosts[2];
      var me = this;
      service.getConnectionPrefillData().then(result => {
        var data = result.data;
        me.email = data.email;
        me.hosts = data.hosts;
      });
      service.getConnectionData().then(result => {
        var data = result.data;
        me.connections = data.connections;
      });
    }

    CreateConnection() {
      alert(this.email + " for host: " + this.currentHost.value);
    }
  }

  (<any>window).appModule.controller("HomeController",
    ["$scope", "ConnectionService", ($scope, connectionService)
      => new HomeController($scope, connectionService)]);
}
