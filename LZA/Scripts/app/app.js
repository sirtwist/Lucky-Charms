myapp = angular.module('LZA', [])
    .controller('KVSController', ['$scope', '$http', '$window', '$location', function ($scope, $http, $window, $location) {
        var app = this;
        app.kvsorders = new Array();

        var urlbase = $location.$$absUrl + '/';
        var chat = $.connection.chatHub;

        app.Entities;

        chat.client.addNewMessageToPage = function (name, message) {
            // app.match = $.grep(app.Entities, function (e) { return e.EntityIDCorp == entity; })[0];
            //$scope.$apply(function () {
            app.Entities = JSON.parse(message);
            //});
            //$('#itmjson').val(message);
            for (var x = 0; x < app.Entities.predictions.length; x++);
            {
                var e = app.Entities.predictions[x-1];
                for (var ii = 0; ii < app.kvsorders.length; ii++)
                {
                    for (var iii = 0; iii < app.kvsorders[ii].itms.length; iii++)
                    {
                        var i = app.kvsorders[ii].itms[iii];
                        if (i.tag === e.tagName) {
                            i.fulfilled = true;
                        }
                        else {
                            i.fulfilled = false;
                        }
                    }
                }
            }
            $scope.$apply();
        };

        $.connection.hub.start();

        FetchOrders(app);
        //$scope.$watch(app.kvsorders, function () { }, true);


        function FetchOrders(app) {
            app.kvsorders.push(getorder('Bag  R06 - 19'));
            app.kvsorders[0].itms.push(getitem(1, 'bigmac', 'Big Mac'));
            app.kvsorders[0].itms.push(getitem(1, 'cheeseburger', 'Cheeseburger'));
            app.kvsorders[0].itms.push(getitem(1, 'nugget10', '10 Pc Nuggets'));

            //app.orders[0].itms[0].fullfilled = true;

            app.kvsorders.push(getorder('Bag  R06 - 20'));
            app.kvsorders[1].itms.push(getitem(1, 'bigmac', 'Big Mac'));

            //app.orders[1].itms[0].fullfilled = true;

            app.kvsorders.push(getorder('Bag  R06 - 21'));
            app.kvsorders[2].itms.push(getitem(1, 'cheeseburger', 'Cheeseburger'));

            app.kvsorders.push(getorder('Bag  R06 - 22'));
            app.kvsorders[3].itms.push(getitem(1, 'hamberger', 'Hamburger'));
            app.kvsorders[3].itms.push(getitem(2, 'cheeseburger', 'Cheseburger'));

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