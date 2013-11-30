(function () {
    'use strict';
    var controllerId = 'admin';
    angular.module('app').controller(controllerId, ['$scope','common', 'datacontext', admin]);

    function admin($scope, common, datacontext) {
        var getLogFn = common.logger.getLogFn;
        var log = getLogFn(controllerId);

        var vm = this;
        vm.title = 'Admin';

        vm.Images = {};
        vm.errorMessage = "";
        activate();

        function activate() {
            var promises = [getAllImages()];
            common.activateController(promises, controllerId)
                .then(function () { log('Activated Admin View'); });
        }

        function getAllImages() {
            datacontext.getAllImages()
             .then(success)
             .fail(failed)
             .fin(refreshView);
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
    }
})();