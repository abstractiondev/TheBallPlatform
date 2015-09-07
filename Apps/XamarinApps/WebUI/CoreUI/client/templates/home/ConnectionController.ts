///<reference path="..\..\services\OperationService.ts"/>
/// <reference path="../../../typings/angularjs/angular.d.ts" />
/// <reference path="../../services/ConnectionService.ts"/>

module application {

    interface IConnectionController {
    CreateConnection:()=>void;
    isCreateFirstConnectionMode:()=>Boolean;
    isManageConnectionsMode:()=>Boolean;
  }

  class ConnectionController implements IConnectionController {
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

    constructor($scope, connectionService:ConnectionService, private operationService:OperationService) {
      $scope.vm = this;
      this.currentHost = this.hosts[2];
      var me = this;
      connectionService.getConnectionPrefillData().then(result => {
        var data = result.data;
        me.email = data.email;
        me.hosts = data.hosts;
      });
      connectionService.getConnectionData().then(result => {
        var data = result.data;
        me.connections = data.connections;
      });
    }

    CreateConnection() {
      this.operationService.executeOperation("TheBall.LocalApp.CreateConnection",
        {
          "host": this.currentHost.hostname,
          "email": this.email
        });
    }

    DeleteConnection(connectionID:string) {
      this.operationService.executeOperation("TheBall.LocalApp.DeleteConnection",
        { "connectionID": connectionID });
    }
  }

  (<any>window).appModule.controller("ConnectionController",
    ["$scope", "ConnectionService", "OperationService",
      ($scope, connectionService, operationService)
      => new ConnectionController($scope, connectionService, operationService)]);
}
