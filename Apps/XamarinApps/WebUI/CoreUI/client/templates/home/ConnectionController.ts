///<reference path="..\..\services\OperationService.ts"/>
/// <reference path="../../../typings/angularjs/angular.d.ts" />
/// <reference path="../../services/ConnectionService.ts"/>
/// <reference path="../../../typings/lodash/lodash.d.ts" />

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

    scope:any;

    connections = [];

    email:string;

    LastOperationDump:string = "void";

    hasConnections():Boolean {
      return this.connections.length > 0;
    }

    isCreateFirstConnectionMode():Boolean {
      return !this.hasConnections();
    }

    isManageConnectionsMode():Boolean {
      return this.hasConnections();
    }

    cards:any = [];

    constructor($scope, connectionService:ConnectionService, private operationService:OperationService) {
      this.scope = $scope;
      $scope.vm = this;
      $scope.$watch(this.cards, () => {
        var $isotopeContainer:any = $(".isotope-container");
        $isotopeContainer.isotope({
          itemSelector: ".isotope-item",
          masonry: {
            columnWidth: 200
          }
        });
      });
      //this.currentHost = this.hosts[2];
      var me = this;
      connectionService.getConnectionPrefillData().then(result => {
        var data = result.data;
        me.email = data.email;
        me.hosts = data.hosts;
      });
      connectionService.getConnectionData().then(result => {
        var data = result.data;
        me.connections = data.connections;
        me.cards = _.map(data.connections, conn => {
          return {
            template: "templates/home/conncard.html",
            connection: conn
          };
        });
      });

      $scope.$watch(() => me.connections, function() {
        me.scope.$evalAsync(function() {
          me.refreshIsotope();
        });
      });
    }

    refreshIsotope()
    {
      var elem = window.document.querySelector(".isotope-container");
      if(!elem)
        return;
      var wnd:any = window;
      var iso = new wnd.Isotope(elem, {});
    }

    CreateConnection() {
      var me = this;
      var host = me.currentHost != null ? me.currentHost.value : "";
      var email = me.email;
      this.operationService.executeOperation("TheBall.LocalApp.CreateConnection",
        {
          "host": host,
          "email": email
        }).then(data => me.LastOperationDump = JSON.stringify(data));
    }

    DeleteConnection(connectionID:string) {
      var me = this;
      this.operationService.executeOperation("TheBall.LocalApp.DeleteConnection",
        { "connectionID": connectionID }).then(data => me.LastOperationDump = JSON.stringify(data));
    }
  }

  (<any>window).appModule.controller("ConnectionController",
    ["$scope", "ConnectionService", "OperationService",
      ($scope, connectionService, operationService)
      => new ConnectionController($scope, connectionService, operationService)]);
}
