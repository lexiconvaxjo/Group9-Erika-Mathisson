(function () {
    "use strict";
    //    // for cache issues
    //    //  var version = "0.01";

    var app = angular.module('app');

    //configure routes
    app.config(function ($routeProvider) {
        $routeProvider
            //route for handle people page
            .when('/handlePeople', {
                templateUrl: '/Angular/Templates/handlePeople.html',
                controller: 'peopleController'
            })
            //route for handle books
            .when('/handleBooks', {
                templateUrl: '/Angular/Templates/handleBooks.html',
                controller: 'handleBooksController'
            })
            //route for handling a users orders
            .when('/handleUserOrder', {
                templateUrl: '/Angular/Templates/handleUserOrder.html',
                controller: 'handleUserOrderController'
            })
        ;
    });

    ////factory for the admin functionality
    app.factory('adminFactory', function ($http, $window, $rootScope, $timeout) {

        var adminFactory = {};
        // for setting message to pages
        adminFactory.message = "";
        //for registered people
        adminFactory.people = [];
        // for added books
        adminFactory.books = [];
        // for a users order history
        adminFactory.userOrderHistory = [];
        // for what row to remove on an user order
        adminFactory.removeRow = {};

        //function for getting people from database
        adminFactory.getPeople = function () {
            return $http.get('/Person/GetPeople')
            .success(function (data) {
                //people fetched ok set the returned data to the array
                adminFactory.people = data;
            })
            .error(function () {
                // setting error message
                adminFactory.message = "Something went wrong...";
            });
        };

        //function for getting a specific person from people array and returning it
        adminFactory.get = function (Id) {
            for (var i in adminFactory.people) {
                if (adminFactory.people[i].Id === Id) {
                    return adminFactory.people[i];
                }
            }
        };

        //function for editing an existing person
        adminFactory.editPerson = function (person) {
            return $http.post('/Person/EditPerson', person)
            .success(function (data) {
                adminFactory.message = "";
                // something went wrong set error message to page
                if (data.status === "Failure") {
                    adminFactory.message = data.message;
                }
                else {
                    if (adminFactory.people.length != 0) {
                        //find the person using id and update it
                        for (var i in adminFactory.people) {
                            if (adminFactory.people[i].Id == person.Id) {
                                adminFactory.people[i] = person;
                            }
                        }
                    }
                    else {
                        adminFactory.myPerson = person;
                        adminFactory.message = "Information updated ok!";
                    }
                }
            })
            .error(function () {
                console.log("Error");
            });
        };

        //function for getting books from database
        adminFactory.getBooks = function () {
            return $http.get('/Book/GetBooks')
            .success(function (data) {
                //books fetched ok, set the returned data to the array
                adminFactory.books = data;
            })
            .error(function () {
                // setting error message
                adminFactory.message = "Something went wrong when fetching books...";
            });
        };

        //function for adding a book
        adminFactory.addBook = function (book) {
            return $http.post('/Book/AddBook', book)
                .success(function (data) {
                    //book is saved to database
                    if (data.status === "Success") {
                        adminFactory.message = "Success";
                        // add the book to the book array
                        adminFactory.books.push(data.addedBook);
                    }
                    else if (data.status === "ISBNExist") {
                        adminFactory.message = "Entered ISBN already exist";
                    }
                    else if (data.status === "DBFailure") {
                        adminFactory.message = "Something went wrong when saving the new book, try again.";
                    }
                    else {
                        // model from MVC isn't valid set the error message/messages
                        adminFactory.message = data;
                    }
                })
                .error(function () {
                    console.log("Error");
                });
        };

        //function for getting a specific book from book array and returning it
        adminFactory.getBook = function (Id) {
            for (var i in adminFactory.books) {
                if (adminFactory.books[i].Id === Id) {
                    return adminFactory.books[i];
                }
            }
        };

        //function for editing an existing book
        adminFactory.editBook = function (book) {
            return $http.post('/Book/EditBook', book)
            .success(function (data) {
                //book is updated to database
                if (data.status === "Success") {
                    adminFactory.message = "Success";
                    //find the book in array using Id and update it
                    for (var i in adminFactory.books) {
                        if (adminFactory.books[i].Id == book.Id) {
                            adminFactory.books[i] = book;
                        }
                    }
                }
                    // book already exist in database set message to user
                else if (data.status === "ISBNExist") {
                    adminFactory.message = "Entered ISBN already exist";
                }
                    // database saving error
                else if (data.status === "DBFailure") {
                    adminFactory.message = "Something went wrong when saving the new book, try again.";
                }
                else {
                    // model isn't valid set the error message/messages
                    adminFactory.message = data;
                }
            })
            .error(function () {
                console.log("Error");
            });
        };

        // function for getting all orders for a user
        adminFactory.getUserOrderHistory = function (UserName) {
            return $http.post('/Order/GetOrderHistory', UserName)
               .success(function (response) {
                   //orders for a user is fetched ok
                   if (response.status === "Success") {
                       adminFactory.message = "Success";
                       // set the fetched list to the orderhistory
                       adminFactory.userOrderHistory = response.data;
                       // loop through the list for adding date in correct format and totalsum of each order
                       for (var i = 0; i < adminFactory.userOrderHistory.length; i++) {
                           // total sum of each order
                           var orderSum = 0;
                           // getting orderdate from the order          
                           var date = adminFactory.userOrderHistory[i].OrderDate;
                           // converting orderdate from json format to javascript format 
                           adminFactory.userOrderHistory[i].dateNewFormat = new Date(parseInt(date.substr(6)));
                           //loop through all orderrows in the order and calculate the sum of each order
                           for (var j = 0; j < adminFactory.userOrderHistory[i].OrderRows.length; j++) {
                               // number of items for each row in the order
                               var noItems = adminFactory.userOrderHistory[i].OrderRows[j].NoOfItem;
                               // get the price per item for each row in the order
                               var pricePerItem = adminFactory.userOrderHistory[i].OrderRows[j].Price;
                               // calculate total sum per row in each order
                               var totalSumPerItem = noItems * pricePerItem;
                               // add the total sum per row in the order to the total amount of the whole order
                               orderSum = orderSum + totalSumPerItem;
                           }
                           // add the total amount of the order to the actual order
                           adminFactory.userOrderHistory[i].totalOrderAmount = orderSum;
                       }
                       // set the orderHistory to the session
                       sessionStorage.setItem('userOrderHistory', JSON.stringify(adminFactory.userOrderHistory));
                   }
                   else {
                       // something went wrong, set message to page...
                       adminFactory.message = "Something went wrong...";
                   }
               })
                // something went wrong when fetching orderhistory from backend
               .error(function () {
                   console.log("Error");
               });
        };

        // function for getting a users order for showing on page
        adminFactory.getUserOrder = function () {
            // get order session
            var orderSession = JSON.parse(sessionStorage.getItem('userOrderHistory'));
            // check if session is null in that case it first time used and needs to be fetched from bookfactory cart
            if (orderSession === null) {
                sessionStorage.setItem('userOrderHistory', JSON.stringify(adminFactory.userOrderHistory));
            }
            // set session to bookFactory cart otherwise cart will be empty if page is refreshed
            adminFactory.userOrderHistory = JSON.parse(sessionStorage.getItem('userOrderHistory'));
            return adminFactory.userOrderHistory;
        };
               
        // function for removing a row from an order       
        adminFactory.deleteOrderRow = function (deleteRow) {
            console.log(deleteRow);
            return $http.post('/Order/RemoveRowFromOrder', deleteRow)
               .success(function (response) {
                   console.log(response.status);
                   //row in an order removed ok
                   if (response.status === "Success") {
                       adminFactory.message = "Success";
                       //check what row to be removed loop through the array 
                       for (var i = 0; i < adminFactory.userOrderHistory.length; i++) {
                           // check if correct order is found
                           if (adminFactory.userOrderHistory[i].Id === deleteRow.orderId) {
                               // correct order is found, loop through to find the correct orderRow
                               for (var j = 0; j < adminFactory.userOrderHistory[i].OrderRows.length; j++) {
                                   if (adminFactory.userOrderHistory[i].OrderRows[j].Id == deleteRow.rowId) {
                                       //remove order row from order
                                       adminFactory.userOrderHistory[i].OrderRows.splice(j, 1);                                       
                                       // check if all orderrows are removed, in that case remove the order itself as well
                                       if (adminFactory.userOrderHistory[i].OrderRows.length == 0) {
                                           adminFactory.userOrderHistory.splice(i, 1);                                          
                                       }
                                       //set cart and totalamount to session
                                       sessionStorage.setItem('userOrderHistory', JSON.stringify(adminFactory.userOrderHistory));                                     
                                   }
                               }
                           }
                       }
                   }
                   else if (response.status = "OrderToOld") {
                       // something went wrong, set message to page...
                       adminFactory.message = "The order/orderrow is to old and can't be deleted";
                       console.log(adminFactory.message);
                   }
               })
                // something went wrong when fetching orderhistory from backend
               .error(function () {
                   console.log("Error");
               });
        };
        return adminFactory;
    });

    //  main controller with $scope injected
    app.controller('handleBooksController', function ($scope, $rootScope, adminFactory, $location) {
        $scope.message = 'This is the handle books page';

        //setting books
        $scope.books = adminFactory.books;
        // book from ng-model
        $scope.addbook = {};

        //function for setting the array of books to the scope
        adminFactory.getBooks().then(function () {
            $scope.books = adminFactory.books;
        });

        //function for adding or edit a book
        this.saveBook = function () {
            // getting addbook from scope
            var addbook = $scope.addbook;
            //check if the book already exist (should be edited) or not (should be added)
            if (addbook.Id == null) {
                //book doesn't exist, call the addBook function in factory
                adminFactory
                .addBook($scope.addbook)
                .then(function () {
                    //check if a message is returned from factory and not "Success" in that case print the message on the page                   
                    if (adminFactory.message !== "Success") {
                        $scope.statusMessage = adminFactory.message;
                    }
                    else {
                        $scope.books = adminFactory.books;
                        $scope.addbook = {};
                        $scope.statusMessage = "";
                    }
                });

            }
            else {
                //the book already exists and should be edited                  
                adminFactory.editBook(addbook)
               .then(function () {
                   //check if any message is returned and set it to the status scope
                   if (adminFactory.message != "Success") {
                       $scope.statusMessage = adminFactory.message;
                   }
                   else {
                       $scope.books = adminFactory.books;
                       $scope.addbook = {};
                       $scope.statusMessage = "";
                   }
               });
            }
        };

        //function for copying information about a specific book and paste the information to the form 
        this.edit = function (Id) {
            $scope.addbook = angular.copy(adminFactory.getBook(Id));
        };
    });

    //controller for listing existing people
    app.controller('peopleController', function ($scope, $window, adminFactory, $location) {
        $scope.message = 'This is the handle people page!';

        //setting people
        $scope.people = adminFactory.people;
        $scope.showForm = false;

        //function for setting the array of people to the scope
        adminFactory.getPeople().then(function () {
            $scope.people = adminFactory.people;
        });

        //function for copying information about a specific person and paste the information to the form 
        this.edit = function (Id) {
            $scope.person = angular.copy(adminFactory.get(Id));
            $scope.showForm = true;
        };

        //function for getting order history
        this.getUserOrderHistory = function (UserName) {
            adminFactory.getUserOrderHistory(UserName).then(function () {
                // setting orderHistory to scope
                $scope.userOrderHistory = adminFactory.userOrderHistory;
                // getting orderdate from the order          
                var date = adminFactory.userOrderHistory[0].OrderDate;
                // converting orderdate from json format to javascript format and setting date to scope
                $scope.orderDate = new Date(parseInt(date.substr(6)));
                $location.path('/handleUserOrder');
            })
        };

        //function for saving a person
        this.savePerson = function savePerson(person) {
            //getting person from scope
            var person = $scope.person;
            adminFactory
                .editPerson(person)
                .then(function () {
                    //check if any message is returned and set it to the status scope
                    if (adminFactory.message.length != 0) {
                        $scope.statusMessage = adminFactory.message;
                    }
                    else {
                        $scope.people = adminFactory.people;
                        $scope.showForm = false;
                    }
                });
        }
    });

    //controller for handling a users orders
    app.controller('handleUserOrderController', function ($scope, adminFactory, $filter) {
        $scope.message = "This is the handle order for user page";

        //function for getting a users orders
        adminFactory.getUserOrder()
        {
            // setting orderInfo to scope
            $scope.userOrderHistory = adminFactory.userOrderHistory;

            console.log(adminFactory.userOrderHistory);

            // getting orderdate from the order          
            var date = adminFactory.userOrderHistory[0].OrderDate;
            // converting orderdate from json format to javascript format and setting date to scope
            $scope.orderDate = new Date(parseInt(date.substr(6)));
            // setting today's date
            $scope.today = new Date();
        };
        
        // funcion for deleting a row on a users order
        this.deleteOrderRow = function (orderId, rowId) {          
            // setting orderId
            adminFactory.removeRow.orderId = orderId;
            // setting rowId
            adminFactory.removeRow.rowId = rowId;
            // call function in factory for removing the orderRow
            adminFactory.deleteOrderRow(adminFactory.removeRow)
               .then(function () {
                   //check if any message is returned and set it to the status scope
                   console.log(adminFactory.message);
                   if (adminFactory.message.length != 0) {
                       if (adminFactory.message == "Success") {
                           $scope.userOrderHistory = adminFactory.userOrderHistory;
                       }
                       else {
                           console.log(adminFactory.message);
                           $scope.statusMessage = adminFactory.message;
                       }
                     
                   }
                   else {

                   }
               })
        };

    });
})();