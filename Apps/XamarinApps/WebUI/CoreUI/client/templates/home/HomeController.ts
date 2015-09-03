/**
 * Created by Kalle on 1.9.2015.
 */

/// <reference path="../../../typings/angularjs/angular.d.ts" />

  interface IHomeController {
    CreateConnection:()=>void;
    isCreateFirstConnectionMode:()=>Boolean;
    isManageConnectionsMode:()=>Boolean;
  }

  class HomeController implements IHomeController {
    static $inject = ['$scope'];

    hosts = [
      {displayName: "test.theball.me", value: "test.theball.me"},
      {displayName: "beta.diosphere.org", value: "beta.diosphere.org"},
      {displayName: "localhost", value: "localhost"},
    ];

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

    constructor($scope) {
      $scope.vm = this;
      this.currentHost = this.hosts[2];
    }

    CreateConnection() {
      alert(this.email + " for host: " + this.currentHost.value);
    }
  }


