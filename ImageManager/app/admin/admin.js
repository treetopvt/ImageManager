(function () {
    'use strict';
    var controllerId = 'admin';
    angular.module('app').controller(controllerId, ['$scope','$timeout', 'common', 'datacontext', admin]);

    function admin($scope, $timeout, common, datacontext) {
        var getLogFn = common.logger.getLogFn;
        var log = getLogFn(controllerId);

        var vm = this;
        vm.title = 'Admin';

        vm.Images = {};
        vm.errorMessage = "";

        vm.CurrentImage = "";

        vm.imageCount = 0;

        //for paging
        vm.ImagesCount = getImagesCount();
        vm.pageChanged = pageChanged;
        vm.paging = {
            currentPage: 1,
            maxPagesToShow: 5,
            pageSize: 15
        };//literal

        //because pageCount is dynamic, make it a property (could be a function)
        Object.defineProperty(vm.paging, 'pageCount', {
            get: function () {
                return Math.ceil(vm.imageCount / vm.paging.pageSize);
            }
        });


        activate();

        vm.getImageThumbnail = getImageThumbnail;
        vm.ShowImage = ShowImage;

        function activate() {
            var promises = [getAllImages()];
            common.activateController(promises, controllerId)
                .then(function () { log('Activated Admin View'); });
        }

        function getAllImages() {
            datacontext.getAllImages().then(success, failed, refreshView);
            //.success(success)
            // .fail(failed)
            // .fin(refreshView);
        }
        function success(data) {
            vm.Images = data;
        }
        function failed(error) {
            vm.errorMessage = error.message;
        }
        function refreshView(){
            $scope.$apply();
        }

        function reset() {
            datacontext.reset().then(getAllImages);
        }

        function ShowImage(image) {
            vm.CurrentImage= datacontext.getImageURL(image.id);
        }

        function getImageThumbnail(image) {

            //image.thumbnail = datacontext.getThumbnailURL(image.id);
            return datacontext.getThumbnailURL(image.id);
            //image.thumbnail =  'http://localhost:64261/breeze/Images/GetImageThumbnail/?id=9a047a64-37c1-4b76-9600-b9dd5b8309b6';

            //var promise = datacontext.getImageThumbnail(image.id);
            //promise.then(function (data) {
            //    $timeout(function () {
            //        // anything you want can go here and will safely be run on the next digest.
            //        image.thumbnail = data;

            //    })
                
            //})
            
        }

        function pageChanged(page) {
            if (!page) {
                return;
            }
            vm.paging.currentPage = page;
            getAllImages();

        }
        function getImagesCount() {

            return datacontext.getImageCount().then(function (data) {
                return vm.imageCount = data;
            });
        }
    }
})();