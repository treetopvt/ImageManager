(function () {
    'use strict';

    var serviceId = 'datacontext';
    angular.module('app').factory(serviceId,
        ['$http', 'common', datacontext]);

    function datacontext($http,common) {
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
            getImageThumbnail: getImageThumbnail,
            getThumbnailURL: getThumbnailURL,
            getImageURL:getImageURL,
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

        function getThumbnailURL(id) {
            return serviceName + '/GetImageThumbnail/?id=' + id;
        }

        function getImageURL(id) {
            return serviceName + '/GetImageBinary/?id=' + id;

        }

        function getImageThumbnail(id) {

           var thumbnail =  $http({
                url: serviceName + '/GetImageThumbnail/',
                method: "GET",
                params: { id: id },
                headers: { 'Content-Type': 'image/jpeg' }
            }).then(success, failure);

           return $q.when(thumbnail)


            function success(dataImage) {
                log("Retrieved thumbnail");
                return 'data:image/jpeg;' + dataImage.data;
                var binary = '';
                //var responseText = dataImage.data;
                //var responseTextLen = dataImage.data.length;
                //for (var j = 0; j < responseTextLen; j += 1) {
                //    binary += String.fromCharCode(responseText.charCodeAt(j) & 0xff)
                //}
                ////'data:image/jpeg;base64,' +
                //return 'data:image/*;base64,' + window.btoa(binary);
            }
            function failure(error) {
                log('Error retrieving thumbnail: ' + error);
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