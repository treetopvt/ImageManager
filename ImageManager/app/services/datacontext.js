(function () {
    'use strict';

    var serviceId = 'datacontext';
    angular.module('app').factory(serviceId,
        ['common', datacontext]);

    function datacontext(common) {
        var $q = common.$q;

        var getLogFn = common.logger.getLogFn;
        var log = getLogFn(serviceId);

        configureBreeze();

        var useLocalHost = true;

        var host = useLocalHost ? "http://localhost:64261" : "http://www.bullsbluff.com/ImageManager";
        var serviceName = host + "/breeze/Images";

        var manager = new breeze.EntityManager(serviceName);

        var service = {
            getAllImages: getAllImages,
            reset: reset
        };

        return service;

        function getMessageCount() { return $q.when(72); }

        function getAllImages() {
            var query = breeze.EntityQuery.from("GetImages");
            log("Getting Images");
            return manager.executeQuery(query).then(success);

            function success(data) {
                log("Retrieved " + data.results.length);
                return data.results;
            }

        }

        function reset() {
            manager.clear();
            var deferred = Q.defer();
            $http.post(serviceName + '/reset')
             .then(resetSuccess, resetFail);
            return deferred.promise;
            function resetSuccess() {
                log("Database reset");
                deferred.resolve();
            }
            function resetFail() {
                log("Database reset failed");
                deferred.reject(new Error("Database reset failed"));
            }
        }

        function configureBreeze() {
            // configure to use the model library for Angular
            breeze.config.initializeAdapterInstance("modelLibrary", "backingStore", true);
            // configure to use camelCase
            breeze.NamingConvention.camelCase.setAsDefault();
        }
    }
})();