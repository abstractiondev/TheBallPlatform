/**
 * Created by kalle on 18.6.2016.
 */
angular.module('TheBallMobileUI')

    .controller('connectionsController', ['$scope', function($scope) {
        $("#AlpacaDemo").alpaca({
            "optionsSource": "/data/options.json",
            "schemaSource": "/data/schema.json",
            "view": "bootstrap-create"
        });
    }]);
