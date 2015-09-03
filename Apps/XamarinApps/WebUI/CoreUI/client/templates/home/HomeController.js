/**
 * Created by Kalle on 1.9.2015.
 */
/// <reference path="../../../typings/angularjs/angular.d.ts" />
var HomeController = (function () {
    function HomeController($scope) {
        /*
      
         <option value="test.theball.me">test.theball.me</option>
         <option value="beta.diosphere.org">beta.diosphere.org</option>
         <option value="localhost">localhost</option>
      
         */
        this.hosts = [
            { displayName: "test.theball.me", value: "test.theball.me" },
            { displayName: "beta.diosphere.org", value: "beta.diosphere.org" },
            { displayName: "localhost", value: "localhost" },
        ];
        this.count = 10;
        this.email = "inittial2";
        $scope.vm = this;
    }
    HomeController.prototype.CreateConnection = function () {
        this.email = "Done!";
    };
    HomeController.$inject = ['$scope'];
    return HomeController;
})();
//# sourceMappingURL=HomeController.js.map