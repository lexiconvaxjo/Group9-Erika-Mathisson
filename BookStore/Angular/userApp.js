(function () {
    "use strict";
    //    // for cache issues
    //    //  var version = "0.01";

    var app = angular.module('app');

    //configure routes
    app.config(function ($routeProvider) {
        $routeProvider

                    //route for a users my pages
                    .when('/myPages', {
                        templateUrl: '/Angular/Templates/myPages.html',
                        controller: 'myPagesController'
                    })
                    //route for changeing password
                    .when('/changePassword', {
                        templateUrl: '/Angular/Templates/changePassword.html',
                        controller: 'changePassWordController'
                    })
                    //route for check out page
                    .when('/checkout', {
                        templateUrl: '/Angular/Templates/checkout.html',
                        controller: 'checkoutController'
                    })
                    // route for showing receipt
                    .when('/showReceipt', {
                        templateUrl: '/Angular/Templates/showReceipt.html',
                        controller: 'receiptController'
                    })
                    // route for showing receipt
                    .when('/orderHistory', {
                        templateUrl: '/Angular/Templates/orderHistory.html',
                        controller: 'orderHistoryController'
                    })
        ;
    });

    //factory for the user
    app.factory('userFactory', function ($http, $window, $rootScope, $timeout) {

        var userFactory = {};
        // for setting message to pages
        userFactory.message = "";
        // for an added order
        userFactory.order = {};
        // for a users order history
        userFactory.orderHistory = [];

        //function for getting information about user
        userFactory.getPerson = function () {
            return $http.post('/Person/GetPerson')
                .success(function (data) {
                    userFactory.myPerson = data;
                })
                .error(function (data) {
                    console.log(data);
                    console.log("Error");
                });
        };

        //function for editing an existing person
        userFactory.editPerson = function (person) {
            return $http.post('/Person/EditPerson', person)
            .success(function (data) {
                userFactory.message = "";
                // something went wrong set error message to page
                if (data.status === "Failure") {
                    userFactory.message = data.message;
                }
                else {
                    userFactory.myPerson = person;
                    userFactory.message = "Information updated ok!";
                }
            })
            .error(function () {
                console.log("Error");
            });
        };

        // function for changeing a users password
        userFactory.changePassword = function (change) {
            return $http.post('/Person/ChangePassword', change)
            .success(function (data) {
                //something went wrong send info to page
                if (data.status === "Failure") {
                    userFactory.message = data.message;
                }
                    //persons password is changed ok send info to page
                else if (data.status === "Success") {
                    userFactory.message = "Password changed ok";
                }
            })
                //something went wrong
            .error(function () {
                console.log("Error");
            });
        };

        //function for getting cart
        // need to use session in order to keep the cart if page is refreshed
        userFactory.getCart = function () {
            // get cart session
            var cartSession = JSON.parse(sessionStorage.getItem('cart'));
            // check if session is null in that case it first time used and needs to be fetched from bookfactory cart
            if (cartSession === null) {
                sessionStorage.setItem('cart', JSON.stringify(userFactory.cart));
            }
            // set session to bookFactory cart otherwise cart will be empty if page is refreshed
            userFactory.cart = JSON.parse(sessionStorage.getItem('cart'));
            return userFactory.cart;
        };

        //function for placing an order
        userFactory.placeOrder = function (cart) {
            return $http.post('/Order/AddOrder', cart)
                .success(function (response) {
                    //order is saved to database
                    if (response.status === "Success") {
                        userFactory.message = "Success";
                        // set the response to the order in factory
                        userFactory.order = response.data;
                        // set the order to the session
                        sessionStorage.setItem('order', JSON.stringify(userFactory.order));
                    }
                        // something isn't right with added information, the model isn't valid
                    else if (response.status === "Failure") {
                        userFactory.message = response.data;
                    }
                        // something went wrong when saving to database
                    else if (response.status === "DBFailure") {
                        userFactory.message = "Something went wrong when saving the order, try again.";
                    }
                })
                //somthing went wrong when calling backend
                .error(function () {
                    console.log("Error");
                });
        };

        // function for getting a users order for showing on receipt page
        userFactory.getOrder = function () {
            // get order session
            var orderSession = JSON.parse(sessionStorage.getItem('order'));
            // check if session is null in that case it first time used and needs to be fetched from bookfactory cart
            if (orderSession === null) {
                sessionStorage.setItem('order', JSON.stringify(userFactory.order));
            }
            // set session to bookFactory cart otherwise cart will be empty if page is refreshed
            userFactory.order = JSON.parse(sessionStorage.getItem('order'));
            return userFactory.order;
        };

        // function for getting all orders for a user
        userFactory.getOrderHistory = function () {
            return $http.post('/Order/GetOrderHistory')
               .success(function (response) {
                   //order is saved to database
                   if (response.status === "Success") {
                       userFactory.message = "Success";
                       // set the fetched list to the orderhistory
                       userFactory.orderHistory = response.data;
                       // loop through the list for adding date in correct format and totalsum of each order
                       for (var i = 0; i < userFactory.orderHistory.length; i++) {
                           // total sum of each order
                           var orderSum = 0;
                           // getting orderdate from the order          
                           var date = userFactory.orderHistory[i].OrderDate;
                           // converting orderdate from json format to javascript format 
                           userFactory.orderHistory[i].dateNewFormat = new Date(parseInt(date.substr(6)));
                           //loop through all orderrows in the order and calculate the sum of each order
                           for (var j = 0; j < userFactory.orderHistory[i].OrderRows.length; j++) {
                               // number of items for each row in the order
                               var noItems = userFactory.orderHistory[i].OrderRows[j].NoOfItem;
                               // get the price per item for each row in the order
                               var pricePerItem = userFactory.orderHistory[i].OrderRows[j].Price;
                               // calculate total sum per row in each order
                               var totalSumPerItem = noItems * pricePerItem;
                               // add the total sum per row in the order to the total amount of the whole order
                               orderSum = orderSum + totalSumPerItem;
                           }
                           // add the total amount of the order to the actual order
                           userFactory.orderHistory[i].totalOrderAmount = orderSum;
                       }
                       // set the orderHistory to the session
                       sessionStorage.setItem('orderHistory', JSON.stringify(userFactory.orderHistory));
                   }
                   else {
                       // something went wrong, set message to page...
                       userFactory.message = "Something went wrong...";
                   }
               })
                // something went wrong when fetching orderhistory from backend
               .error(function () {
                   console.log("Error");
               });
        };

        return userFactory;
    });

    //controller for listing a users myPages
    app.controller('myPagesController', function ($scope, $window, userFactory) {
        $scope.message = 'This is the my pages page!';

        //setting person to $scope
        $scope.myPerson = userFactory.myPerson;

        // function for getting the logged in person
        userFactory.getPerson().then(function () {
            $scope.myPerson = userFactory.myPerson;
        });

        //function for updating a person
        this.updatePerson = function updatePerson(person) {
            //getting person from scope
            var person = $scope.myPerson;
            userFactory
                .editPerson(person)
                .then(function () {
                    //check if any message is returned and set it to the status scope
                    if (userFactory.message.length != 0) {
                        $scope.statusMessage = userFactory.message;
                    }
                    else {
                        $scope.myPerson = userFactory.myPerson;
                        $scope.statusMessage = userFactory.message;
                    }
                });
            //  }
        }
    });

    //controller for changeing a users password
    app.controller('changePassWordController', function ($scope, $window, userFactory) {
        //message for change password page
        $scope.message = 'This is change password page';

        // function for changing a users password
        this.changePassword = function () {
            userFactory.changePassword($scope.change)
            .then(function () {
                $scope.statusMessage = userFactory.message;
                $scope.change = "";
            })
        }
    });

    // controller for checkout
    app.controller('checkoutController', function ($scope, userFactory, $window, $timeout, $location) {
        //message for checkout page
        $scope.message = 'This is the check out page!';

        //function for checking out an order
        this.placeOrder = function () {
            // fetch the cart
            var cart = userFactory.cart;
            // call function in factory for placing the order
            userFactory.placeOrder(cart)
            .then(function () {
                //check the result from function
                if (userFactory.message.length !== 0) {
                    if (userFactory.message === "Success") {
                        //setting info about order to scope
                        $scope.orderInfo = userFactory.order;
                        // clear the cart and session after the order has been placed
                        userFactory.cart = [];
                        $scope.cart = userFactory.cart;
                        sessionStorage.setItem('cart', JSON.stringify(userFactory.cart));
                        // show the receipt page                       
                        $location.path('/showReceipt');
                    }
                }
            });
        };

        //function for getting cart
        userFactory.getCart()
        {
            $scope.cart = userFactory.cart;

            // check if any items exist in cart, in that case show information 
            //and buttons regarding checkout, on cart page
            if (userFactory.cart.length !== 0) {
                $scope.showCheckout = true;
            }
            else {
                $scope.showCheckout = false;
            }
        };

    });

    // controller for receipt
    app.controller('receiptController', function ($scope, userFactory, $window, $timeout, $location) {
        //message for receipt page
        $scope.message = 'This is the receipt page!';

        //function for getting order
        userFactory.getOrder()
        {
            // setting orderInfo to scope
            $scope.orderInfo = userFactory.order;
            // getting orderdate from the order          
            var date = userFactory.order.OrderDate;
            // converting orderdate from json format to javascript format and setting date to scope
            $scope.orderDate = new Date(parseInt(date.substr(6)));
        };
    });

    //controller for order history
    app.controller('orderHistoryController', function ($scope, userFactory, $location) {
        //message for order history page
        $scope.message = "This is order history page";

        //function for getting order history
        userFactory.getOrderHistory().then(function () {
            // setting orderHistory to scope
            $scope.orderHistory = userFactory.orderHistory;
            // getting orderdate from the order          
            var date = userFactory.orderHistory[0].OrderDate;
            // converting orderdate from json format to javascript format and setting date to scope
            $scope.orderDate = new Date(parseInt(date.substr(6)));
        });
    });
})();