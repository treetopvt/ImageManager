(function () {
    'use strict';

    var serviceId = 'datacontext';
    angular.module('app').factory(serviceId,
        ['$http', 'common','config','entityManagerFactory', datacontext]);

    function datacontext($http, common, config, emFactory) {
        var $q = common.$q;
        var EntityQuery = breeze.EntityQuery;
       

        var getLogFn = common.logger.getLogFn;
        var log = getLogFn(serviceId);

        var manager = emFactory.newManager();


        var service = {
            getAllImages: getAllImages,
            getImageThumbnail: getImageThumbnail,
            getThumbnailURL: getThumbnailURL,
            getImageURL: getImageURL,
            getImageCount:getImageCount,
            reset: reset
        };

        var storeMeta = {
            isLoaded: {
                images: false,
                folders: false
            }
        };

        return service;

        function getMessageCount() { return $q.when(72); }

        function getImageCount() {
            if (_areImagesLoaded()) {
                return $q.when(_getLocalEntityCount('ImageModel'));
            }
            //go get them remotely.
            return EntityQuery.from('GetImages').take(0).inlineCount()
                .using(manager).execute() //remote call
                .to$q(_getInlineCount);
        }

        function getAllImages(forceRemote, page, size, nameFilter) {
            var take = size || 5; //how many to get at a time
            var skip = page ? (page - 1) * size : 0;//how many to skip, if no page defined, start at first page
            var imageOrderBy = 'fileName';

            log("Getting Images");

            if (_areImagesLoaded() && !forceRemote) {//get data locally
                return $q.when(getByPage()); //promise because getByPage doesn't return a promise (sync call)
                //attendees = _getAllLocal(entityNames.attendee, attendeeOrderBy);
                //must return a promise, because that's what we expect (the then call)
                //return $q.when(attendees);//wrap itself within a promise
            }

            var query = EntityQuery.from("GetImages")
                .orderBy(imageOrderBy)
                .toType('ImageModel')
                .using(manager).execute()
                .to$q(success, _queryFailed);

            return query;
                

            //return manager.executeQuery(query).then(success);

            function getByPage() {
                var predicate = null;
                if (nameFilter) {
                    predicate = _fullNamePredicate(nameFilter);//underscore = private function
                }
                //similar to the getAttendees call, but the filter is different
                var images = EntityQuery.from('GetImages')
                    .where(predicate)//new call because we are filtering
                    .take(take) //how many to grab
                    .skip(skip)//how many to skip before grabbing
                    .orderBy(imageOrderBy)
                    .toType('ImageModel')
                    .using(manager)
                    .executeLocally();

                return images;

            }


            function success(data) {
                //log("Retrieved " + data.results.length);
                _areImagesLoaded(true); //data loaded, set attendees loaded to true
                log('Retrieved [Image List] from remote data source', data.results.length, true);//true means show alert on screen
                return getByPage();
              //  return data.results;
            }

        }

        function getThumbnailURL(id) {
            return config.remoteServiceName + '/GetImageThumbnail/?id=' + id;
        }

        function getImageURL(id) {
            return config.remoteServiceName + '/GetImageBinary/?id=' + id;

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


        ///#region Helper Functions

        function _getLocalEntityCount(resource) {
            var entities = EntityQuery.from(resource)
                .using(manager)
                .executeLocally();
            return entities.length;
        }

        function _fullNamePredicate(filterValue) {
            return Predicate
                .create('imageName', 'contains', filterValue);//what you want to search, what type of search, value to search
                //.or('lastName', 'contains', filterValue);
        }

        function _areImagesLoaded(value) {
            return _areItemsLoaded('images', value);
        }
        //helper functions
        function _areItemsLoaded(key, value) {
            if (value === undefined) {
                return storeMeta.isLoaded[key];//gets the value
            }
            return storeMeta.isLoaded[key] = value;
        }

        function _queryFailed(error) {
            //config.appErrorPrefix defined in config.js to standardize error message formatting
            var msg = config.appErrorPrefix + 'Error retrieving data.' + error.message;
            logError(msg, error);
            throw error;
        }

        function _getInlineCount(data) {
            //breeze function
            return data.inlineCount;
        }

        ///#endregion
    }
})();