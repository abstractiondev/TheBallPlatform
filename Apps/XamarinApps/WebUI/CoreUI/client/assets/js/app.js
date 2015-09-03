(function() {
  'use strict';

  angular.module('application', [
    'ui.router',
    'ngAnimate',

    //foundation
    'foundation',
    'foundation.dynamicRouting',
    'foundation.dynamicRouting.animations'
  ])
    .config(config).run(run)
  ;

  config.$inject = ['$urlRouterProvider', '$locationProvider', '$controllerProvider'];

  function config($urlProvider, $locationProvider, $controllerProvider) {
    $urlProvider.otherwise('/');

    $locationProvider.html5Mode({
      enabled:false,
      requireBase: false
    });

    $locationProvider.hashPrefix('!');

    $controllerProvider.allowGlobals();
  }

  function run() {
    FastClick.attach(document.body);
  }

})();
