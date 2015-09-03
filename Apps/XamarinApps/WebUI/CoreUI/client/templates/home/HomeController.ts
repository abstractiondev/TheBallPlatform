/**
 * Created by Kalle on 1.9.2015.
 */

/// <reference path="../../../typings/angularjs/angular.d.ts" />

interface IHomeController {
  CreateConnection:()=>void;
}

class HomeController implements IHomeController {
  static $inject = ['$scope'];


  /*

   <option value="test.theball.me">test.theball.me</option>
   <option value="beta.diosphere.org">beta.diosphere.org</option>
   <option value="localhost">localhost</option>

   */

  hosts = [
    { displayName: "test.theball.me", value: "test.theball.me"},
    { displayName: "beta.diosphere.org", value: "beta.diosphere.org"},
    { displayName: "localhost", value: "localhost"},
  ];

  count:number = 10;
  email:string = "inittial2";

  constructor($scope) {
    $scope.vm = this;
  }

  CreateConnection() {
    this.email = "Done!";
  }
}
