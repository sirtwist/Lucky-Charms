myapp = angular.module('LZA', [])
    .controller('KVSController', ['$scope', '$http', '$window', '$location', function ($scope, $http, $window, $location) {
        var app = this;
        $scope.kvsorders = new Array();
        $scope.$watch('kvsorders', function () {
            //$scope.$emit('kvsorders');
        }, true);
        var urlbase = $location.$$absUrl + '/';
        var chat = $.connection.chatHub;

        $scope.Entities;

        chat.client.addNewMessageToPage = function (name, message) {
            // app.match = $.grep(app.Entities, function (e) { return e.EntityIDCorp == entity; })[0];
            $scope.$apply(function () {
                $scope.Entities = JSON.parse(message).predictions;
            });

            
            //$("#itmjson").val("");
            angular.forEach($scope.kvsorders, function (o) {
                angular.forEach(o.itms, function (i) {
                    i.fulfilled = false;
                });
            });
            
            
            
            angular.forEach($scope.Entities, function (e) {
                    angular.forEach($scope.kvsorders, function (o) {
                        angular.forEach(o.itms, function (i) {
                            if (i.tag === e.tagName) {
                               // $("#itmjson").val($("#itmjson").val() + ", " + e.tagName + " " + e.probability);
                                $scope.$apply(function () {
                                    i.fulfilled = true;
                                });
                            }
                            else {
                                $scope.$apply(function () {
                                    ifulfilled = false;
                                });
                            }
                        });
                    });
                });
            //for (var pred = 0; x < app.Entities.predictions.length; pred++)
            //{
            //    var e = app.Entities.predictions[p - 1];
            //    for (var ii = 0; ii < $scope.kvsorders.length; ii++) {
            //        for (var iii = 0; iii < app.kvsorders[ii].itms.length; iii++) {
            //            var i = $scope.app.kvsorders[ii].itms[iii];
            //            if (i.tag === e.tagName) {
            //                i.set('fulfilled', true);
            //                $('.' + e.tagName).style('background-color', 'green');
            //                $("#itmjson").val($("#itmjson").val() + ", " + e.tagName + " " + e.probability);
            //            }
            //            else {
            //                i.fulfilled = false;
            //                $('.' + e.tagName).style('background-color', 'white');
            //            }
            //        }
            //    }
            //}

            //});

            //$rootScope.$apply();
        };

        $.connection.hub.start();

        FetchOrders(app);

        function FetchOrders(app) {
            $scope.kvsorders.push(getorder('Bag  R06 - 19'));
            $scope.kvsorders[0].itms.push(getitem(1, 'bigmac', 'Big Mac'));
            $scope.kvsorders[0].itms.push(getitem(1, 'cheeseburger', 'Cheeseburger'));
            $scope.kvsorders[0].itms.push(getitem(1, 'nugget10', '10 Pc Nuggets'));

            //app.kvsorders[0].itms[0].fullfilled = true;

            $scope.kvsorders.push(getorder('Bag  R06 - 20'));
            $scope.kvsorders[1].itms.push(getitem(1, 'bigmac', 'Big Mac'));

            //app.orders[1].itms[0].fullfilled = true;

            $scope.kvsorders.push(getorder('Bag  R06 - 21'));
            $scope.kvsorders[2].itms.push(getitem(1, 'cheeseburger', 'Cheeseburger'));

            $scope.kvsorders.push(getorder('Bag  R06 - 22'));
            $scope.kvsorders[3].itms.push(getitem(1, 'hamberger', 'Hamburger'));
            $scope.kvsorders[3].itms.push(getitem(2, 'cheeseburger', 'Cheseburger'));

        }
        app.checkEmpty = function (obj) {
            if (ojb === undefined || ojb == null || ojb.length <= 0) {
                return true;
            }
            else {
                return false;
            }
        };

        function getitem(itms, tag, displayname) {
            var item = {
                itemcount: itms,
                tag: tag,
                displayname: displayname,
                fulfilled: false
            };
            return item;
        };

        function getorder(t) {
            var order = {
                title: t,
                itms: new Array()
            };

            return order;
        };



    }]);