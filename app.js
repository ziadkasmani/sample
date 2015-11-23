var app = angular.module("testApp", ['ngRoute', 'ui.bootstrap', 'angular-loading-bar', 'ngTasty'])
.controller('mainController', function ($scope, $http) {




});



app.config(['$routeProvider', function ($routeProvider) {
    $routeProvider
        .when('/grid', { templateUrl: 'Grid', controller: 'GridController' })
        .when('/default', { templateUrl: 'Default', controller: 'DefaultController' })
        .otherwise({ redirectTo: '/default' });
}]);