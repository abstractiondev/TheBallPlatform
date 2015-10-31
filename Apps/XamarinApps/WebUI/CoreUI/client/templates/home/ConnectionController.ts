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
    //progressMax:any;
    //progressCurrent:any;

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

    constructor($scope, connectionService:ConnectionService, private operationService:OperationService, private foundationApi:any, private $timeout:any) {
      this.scope = $scope;
      $scope.vm = this;
      $scope.progressMax = 300;
      $scope.progressCurrent = 0;
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
        });/* .then(data => me.LastOperationDump = JSON.stringify(data));*/
    }

    GoToConnection(connectionID:string)
    {
      var me = this;
      me.foundationApi.publish("progressBarModal", "open");
      me.operationService.executeOperation("TheBall.LocalApp.GoToConnection",
        { "connectionID": connectionID}).then(
          successData => { me.foundationApi.publish("progressBarModal", "close") },
          failedData => me.LastOperationDump = "Failed: " + JSON.stringify(failedData),
          updateData => {
            me.scope.progressCurrent = me.scope.progressMax * updateData.progress;
          } );
    }

    UpdateTimeOut()
    {
      setTimeout(this.UpdateTimeOut, 1000);
    }

    DeleteConnection(connectionID:string) {
      var wnd:any = window;
      var me = this;
      //(<any>$("#progressBarModal")).foundation("reveal", "open");
      //(<any>$("#progressBarModal")).data("revealInit").close_on_background_click = false;
      me.foundationApi.publish("progressBarModal", "open");
      var repeat = function() {
        if(me.scope.progressCurrent < me.scope.progressMax)
          me.$timeout(repeat, 200);
        //else
        //  me.foundationApi.publish("progressBarModal", "close");
      };
      me.$timeout(repeat, 200);
      return;
      me.foundationApi.publish('main-notifications',
        { title: 'Deleting Connection', content: connectionID, autoclose: "3000",
          color: "alert"});
      this.operationService.executeOperation("TheBall.LocalApp.DeleteConnection",
        { "connectionID": connectionID }); /*.then(data => me.LastOperationDump = JSON.stringify(data));*/
    }
  }

  (<any>window).appModule.controller("ConnectionController",
    ["$scope", "ConnectionService", "OperationService", "FoundationApi", "$timeout",
      ($scope, connectionService, operationService, foundationApi, $timeout)
      => new ConnectionController($scope, connectionService, operationService, foundationApi, $timeout)]);
}
