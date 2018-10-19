myapp = angular.module('LZA', [])
    .controller('KVSController', ['$scope', '$http', '$window', '$location', function ($scope, $http, $window, $location) {
        var app = this;
        app.orders = new Array();

        var urlbase = $location.$$absUrl + '/';
        var chat = $.connection.chatHub;
        $.connection.hub.start();

        app.Entities;

        chat.client.addNewMessageToPage = function (name, message, timestamp) {
            // app.match = $.grep(app.Entities, function (e) { return e.EntityIDCorp == entity; })[0];
            $scope.$apply(function () {
                app.Entities = message;
            });
            $('#itmjson').val() = message;
        };

        $.connection.hub.start();

        FetchOrders(app);
        $scope.$watch(app.Entities, function () { }, true);


        function FetchOrders(app) {
            app.orders.push(getorder('Bag  R06 - 19'));
            app.orders[0].items.push(getitem(1, 'bigmac', 'Big Mac'));
            app.orders[0].items.push(getitem(1, 'cheeseburger', 'Cheeseburger'));
            app.orders[0].items.push(getitem(1, 'nugget10', '10 Pc Nuggets'));

            app.orders.push(getorder('Bag  R06 - 20'));
            app.orders[1].items.push(getitem(1, 'bigmac', 'Big Mac'));

            app.orders.push(getorder('Bag  R06 - 21'));
            app.orders[2].items.push(getitem(1, 'cheeseburger', 'Cheeseburger'));

            app.orders.push(getorder('Bag  R06 - 22'));
            app.orders[3].items.push(getitem(1, 'hamberger', 'Hamburger'));
            app.orders[3].items.push(getitem(2, 'cheeseburger', 'Cheseburger'));

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
                displayname: displayname
            };
            return item;
        };

        function getorder(t) {
            var order = {
                title: t,
                items: new Array()
            };
            
            return order;
        };



    }]);