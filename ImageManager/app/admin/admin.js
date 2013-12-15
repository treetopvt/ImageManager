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
        vm.currentIndex = 0;
        vm.imageCount = 0;

        //for paging
        vm.ImagesCount = getImagesCount();
        vm.pageChanged = pageChanged;
        vm.paging = {
            currentPage: 1,
            maxPagesToShow: 5,
            pageSize: 5
        };//literal



        vm.direction='left';

        //because pageCount is dynamic, make it a property (could be a function)
        Object.defineProperty(vm.paging, 'pageCount', {
            get: function () {
                return Math.ceil(vm.imageCount / vm.paging.pageSize);
            }
        });

        vm.nextImage = getNextImage;
        vm.previousImage = getPreviousImage;
        vm.setCurrentImageIndex = setCurrentImageIndex;

        activate();

        vm.getImageThumbnail = getImageThumbnail;
        vm.ShowImage = ShowImage;

        function activate() {
            var promises = [getImages()];//getAllImages()
            common.activateController(promises, controllerId)
                .then(function () { log('Activated Admin View'); });
        }

        function setCurrentImageIndex(index){
            vm.direction = (index >vm.currentIndex) ? 'left' : 'right';
            vm.currentIndex = index;
            ShowImage(vm.Images[index]);
        }

        function getNextImage() {
            //vm.direction = 'right';
            //vm.currentIndex = (vm.currentIndex > 0) ? --vm.currentIndex : vm.Images.length - 1;
            var nextIndex = vm.currentIndex + 1;
            if (nextIndex >= vm.Images.length && vm.paging.currentPage < vm.paging.pageCount) {
                //reached end of current list
                vm.currentIndex = 0;
                pageChanged(vm.paging.currentPage + 1);
            }else if (nextIndex <vm.Images.length){
                setCurrentImageIndex(nextIndex);
            }
            

            //need to check to see if there are more images
        }

        function getPreviousImage() {
            //vm.direction = 'left';
            var prevIndex = vm.currentIndex-1;
            if (prevIndex < 0 && vm.paging.currentPage > 1) {
                //go get previous group of images and set the current image to the last one in the array
                vm.currentIndex = vm.paging.pageSize
                pageChanged(--vm.paging.currentPage);
                //need to set the currentIndex to vm.Images.length -1, but only after the images have been returned
            } else {
                setCurrentImageIndex(prevIndex);

            }

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
            vm.CurrentImage = datacontext.getImageURL(image.id);

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
            getImages();

        }
        function getImagesCount() {

            return datacontext.getImageCount().then(function (data) {
                return vm.imageCount = data;
            });
        }

        function getImages(forceRefresh) {
            //getImages is async and will return a promise
            //            datacontext.getAllImages().then(success, failed, refreshView);

            return datacontext.getAllImages(forceRefresh, vm.paging.currentPage, vm.paging.pageSize, '')
                .then(function (data) {
                    vm.Images = data;
                   // getAttendeeFilteredCount();
                    if (!vm.imageCount || forceRefresh) {
                        //only get if no count, or the refresh happens (assumption made)
                        getImagesCount(); //make sure it is local cache
                    }
                    //applyFilter(); //filter data when it comes in
                    if (vm.currentIndex == vm.paging.pageSize) {
                        //need to get last image in this current list
                        vm.setCurrentImageIndex(vm.Images.length-1);
                    } else {
                        vm.setCurrentImageIndex(vm.currentIndex);
                    }
                    return data;

                }, failed);
        }

    }
})();