(function () {
    "use strict";

    var app = angular.module('app', ['ngRoute']);

    //configure routes
    app.config(function ($routeProvider) {
        $routeProvider
            // route for the home page
            .when('/', {
                templateUrl: '/Angular/Templates/home.html',
                controller: 'mainController'
            })

            // route for the register page
            .when('/register', {
                templateUrl: '/Angular/Templates/register.html',
                controller: 'registerController'
            })

            //route for handle people page
            .when('/handlePeople', {
                templateUrl: '/Angular/Templates/handlePeople.html',
                controller: 'peopleController'
            })

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

            // route for the log in page
            .when('/logIn', {
                templateUrl: '/Angular/Templates/login.html',
                controller: 'loginController'
            });
    });

    app.factory('bookFactory', function ($http, $window, $rootScope, $timeout) {

        var bookFactory = {};
        // for setting message to pages
        bookFactory.message = "";
        //for registered people
        bookFactory.people = [];
        // for added books
        bookFactory.books = [];
        bookFactory.myPerson = {};
        // if user is logged in
        bookFactory.isAuthorized = false;
        // what role the user has
        bookFactory.userRole = null;
        // what user name the user has
        bookFactory.userName = null;

        // for checking if user is logged in, what role the user has and what username the user has
        $http.post("/Person/IsLoggedIn").then(function (response) {
            if (response.data.status === true) {
                bookFactory.isAuthorized = true;
                bookFactory.userRole = response.data.role;
                bookFactory.userName = response.data.userName;
            }
        });

        //function for register a person
        bookFactory.registerPerson = function (person) {
            return $http.post('/Person/RegisterPerson', person)
                .success(function (data) {
                    //check if error message is returned, in that case sent it to the controller               
                    if (data === "EmailExists") {
                        bookFactory.message = "Email already exist, please re enter your information!";
                    }
                    else if (data === "Empty") {
                        bookFactory.message = "No data has been submitted, enter required information!";
                    }
                    else if (data === "UserNameExists") {
                        bookFactory.message = "User name already exist, please re enter your information!";
                    }
                        //person is saved to database
                    else if (data === "Success") {
                        bookFactory.message = "Success";
                    }
                    else {
                        // model isn't valid set the error message/messages
                        bookFactory.message = data;
                    }
                })
        .error(function () {
            console.log("Error");
        });

        };

        //function for logging in a person
        bookFactory.logInPerson = function (logInPerson) {
            return $http.post('/Person/LogInPerson', logInPerson)
                .success(function (data) {
                    //check if error message is returned, in that case sent it to the controller               
                    if (data.status === "Failure") {
                        bookFactory.message = "No data has been submitted, something went wrong!";
                    }
                        //person is logged in ok
                    else if (data.status === "Success") {
                        bookFactory.message = "Person logged in ok";
                        //$timeout(function () {
                        // the user is logged in ok setting isAuthorized and what role the user has
                        bookFactory.isAuthorized = true;
                        bookFactory.userRole = data.role;
                        bookFactory.userName = data.userName;
                        //});
                        // todo: redirect??
                        $window.location.href = "#/";
                    }
                    else {
                        // model isn't valid set the error message/messages            
                        bookFactory.message = data;
                    }
                })
            .error(function () {
                console.log("Error");
            });
        };

        //function for getting people from database
        bookFactory.getPeople = function () {
            return $http.get('/Person/GetPeople')
            .success(function (data) {
                //people fetched ok set the returned data to the array
                bookFactory.people = data;
            })
            .error(function () {
                // setting error message
                bookFactory.message = "Something went wrong...";
            });
        };

        //function for getting information about user
        bookFactory.getPerson = function () {
            return $http.post('/Person/GetPerson')
                .success(function (data) {
                    bookFactory.myPerson = data;
                })
                .error(function (data) {
                    console.log(data);
                    console.log("Error");
                });
        };

        //function for getting a specific person from people array and returning it
        bookFactory.get = function (Id) {
            for (var i in bookFactory.people) {
                if (bookFactory.people[i].Id === Id) {
                    return bookFactory.people[i];
                }
            }
        };

        //function for editing an existing person
        bookFactory.editPerson = function (person) {
            return $http.post('/Person/EditPerson', person)
            .success(function (data) {
                //check if error message is returned, set it to the message property
                //if (data.status === "EmailExists") {
                //    bookFactory.message = "Email already exist, please re enter your information!";
                //}
                bookFactory.message = "";
                if (data.status === "Failure") {
                    bookFactory.message = data.message;
                }
                else {
                    if (bookFactory.people.length != 0) {
                        //find the person using id and update it
                        for (var i in bookFactory.people) {
                            if (bookFactory.people[i].Id == person.Id) {
                                bookFactory.people[i] = person;
                            }
                        }
                    }
                    else {
                        bookFactory.myPerson = person;
                        bookFactory.message = "Information updated ok!";
                    }
                }
            })
            .error(function () {
                console.log("Error");
            });
        };

        // function for changeing a users password
        bookFactory.changePassword = function (change) {
            return $http.post('/Person/ChangePassword', change)
            .success(function (data) {
                if (data.status === "Failure") {
                    bookFactory.message = data.message;
                }
                    //person is logged in ok
                else if (data.status === "Success") {
                    bookFactory.message = "Password changed ok";
                }
            })
            .error(function (data) {
                console.log("Error");
                console.log(data);
            });
        };

        //function for getting books from database
        bookFactory.getBooks = function () {
            return $http.get('/Book/GetBooks')
            .success(function (data) {
                //books fetched ok, set the returned data to the array
                bookFactory.books = data;
            })
            .error(function () {
                // setting error message
                bookFactory.message = "Something went wrong when fetching books...";
            });
        };

        //function for adding a book
        bookFactory.addBook = function (book) {
            return $http.post('/Book/AddBook', book)
                .success(function (data) {
                    //book is saved to database
                    if (data.status === "Success") {
                        bookFactory.message = "Success";
                        bookFactory.books.push(data.addedBook);
                    }
                    else if (data.status === "ISBNExist") {
                        bookFactory.message = "Entered ISBN already exist";
                    }
                    else if (data.status === "DBFailure") {
                        bookFactory.message = "Something went wrong when saving the new book, try again.";
                        console.log(data.message);
                    }
                    else {
                        // model isn't valid set the error message/messages
                        bookFactory.message = data;
                    }
                })
                .error(function () {
                    console.log("Error");
                });

        };

        //function for getting a specific book from book array and returning it
        bookFactory.getBook = function (Id) {
            for (var i in bookFactory.books) {
                if (bookFactory.books[i].Id === Id) {
                    return bookFactory.books[i];
                }
            }
        };

        //function for editing an existing book
        bookFactory.editBook = function (book) {
            return $http.post('/Book/EditBook', book)
            .success(function (data) {
                //book is updated to database
                if (data.status === "Success") {
                    bookFactory.message = "Success";
                    //find the book using Id and update it
                    for (var i in bookFactory.books) {
                        if (bookFactory.books[i].Id == book.Id) {
                            bookFactory.books[i] = book;
                        }
                    }
                }
                else if (data.status === "ISBNExist") {
                    bookFactory.message = "Entered ISBN already exist";
                }
                else if (data.status === "DBFailure") {
                    bookFactory.message = "Something went wrong when saving the new book, try again.";
                    console.log(data.message);
                }
                else {
                    // model isn't valid set the error message/messages
                    bookFactory.message = data;
                }
            })
            .error(function () {
                console.log("Error");
            });
        };
        return bookFactory;
    });

    //  main controller with $scope injected
    app.controller('mainController', function ($scope, $rootScope, bookFactory) {
        $scope.message = 'This is the main page';

        //setting books
        $scope.books = bookFactory.books;

        // book from ng-model
        $scope.addbook = {};

        // checking and setting if user is logged in and what role the user has 
        $rootScope.$watch(function () {
            return bookFactory.isAuthorized;
        }, function (n, o) {
            if (n === true) {
                //setting loggedIn and role to the user
                $scope.isLoggedIn = true;
                $scope.role = bookFactory.userRole;
                $scope.userName = bookFactory.userName;
            }
        });


        //function for setting the array of books to the scope
        bookFactory.getBooks().then(function () {
            $scope.books = bookFactory.books;
        });

        //function for adding or edit a book
        this.saveBook = function () {           
            // getting addbook from scope
            var addbook = $scope.addbook;
            //check if the book already exist (should be edited) or not (should be added)
            if (addbook.Id == null) {
                //book doesn't exist, call the addBook function in factory
                bookFactory
                .addBook($scope.addbook)
                .then(function () {
                    //check if a message is returned from factory in that case print the message on the page                   
                    if (bookFactory.message !== "Success") {
                        $scope.statusMessage = bookFactory.message;
                    }
                    else {

                        $scope.books = bookFactory.books;                       
                        $scope.addbook = {};
                        $scope.statusMessage = "";    
                    }
                });

            }
            else {
                //the book already exists and should be edited                  
                bookFactory.editBook(addbook)
               .then(function () {
                   //check if any message is returned and set it to the status scope
                   if (bookFactory.message != "Success") {
                       $scope.statusMessage = bookFactory.message;
                   }
                   else {                      
                       $scope.books = bookFactory.books;                      
                       $scope.addbook = {};
                       $scope.statusMessage = "";
                   }
               });
            }
        };

        //function for copying information about a specific book and paste the information to the form 
        this.edit = function (Id) {
            console.log(Id)
            $scope.addbook = angular.copy(bookFactory.getBook(Id));
        };
    });

    //controller for handling registration
    app.controller('registerController', function ($scope, $window, bookFactory) {
        // message for the page
        $scope.message = 'This is the register page!';
        // person from ng-model
        $scope.person = {};

        //function for register a person
        this.registerPerson = function () {
            //call the registerPerson function in factory
            bookFactory
            .registerPerson($scope.person)
            .then(function () {
                //check if a message is returned from factory in that case print the message on the page
                if (bookFactory.message.length !== 0) {
                    $scope.statusMessage = bookFactory.message;
                }
                //   $window.location.href = "#/";

            });
        };
    });

    //controller for listing existing people
    app.controller('peopleController', function ($scope, $window, bookFactory) {
        $scope.message = 'This is the handle people page!';

        //setting people
        $scope.people = bookFactory.people;

        $scope.showForm = false;

        //function for setting the array of people to the scope
        bookFactory.getPeople().then(function () {
            $scope.people = bookFactory.people;
        });

        //function for copying information about a specific person and paste the information to the form 
        this.edit = function (Id) {
            $scope.person = angular.copy(bookFactory.get(Id));
            $scope.showForm = true;
        };

        //function for saving a person
        this.savePerson = function savePerson(person) {
            //getting person from scope
            var person = $scope.person;
            bookFactory
                .editPerson(person)
                .then(function () {
                    //check if any message is returned and set it to the status scope
                    if (bookFactory.message.length != 0) {
                        $scope.statusMessage = bookFactory.message;
                    }
                    else {
                        $scope.people = bookFactory.people;
                        $scope.showForm = false;
                    }
                });
            //  }
        }
    });

    //controller for listing a users myPages
    app.controller('myPagesController', function ($scope, $window, bookFactory) {
        $scope.message = 'This is the my pages page!';

        //setting person to $scope
        $scope.myPerson = bookFactory.myPerson;

        // function for getting the logged in person
        bookFactory.getPerson().then(function () {
            $scope.myPerson = bookFactory.myPerson;
        });

        //function for updating a person
        this.updatePerson = function updatePerson(person) {
            //getting person from scope
            var person = $scope.myPerson;
            console.log(person);
            bookFactory
                .editPerson(person)
                .then(function () {
                    //check if any message is returned and set it to the status scope
                    if (bookFactory.message.length != 0) {
                        $scope.statusMessage = bookFactory.message;
                    }
                    else {
                        $scope.myPerson = bookFactory.myPerson;
                        $scope.statusMessage = bookFactory.message;
                    }
                });
            //  }
        }
    });

    //controller for changeing a users password
    app.controller('changePassWordController', function ($scope, $window, bookFactory) {
        //message for change password page
        $scope.message = 'This is change password page';

        // function for changing a users password
        this.changePassword = function () {
            bookFactory.changePassword($scope.change)
            .then(function () {
                $scope.statusMessage = bookFactory.message;
                $scope.change = "";
            })
        }
    });


    // controller for handling log in
    app.controller('loginController', function ($scope, $window, bookFactory, $rootScope, $timeout) {
        //message for log in page
        $scope.message = 'This is the log in page';
        // person who will log in from ng-model
        var logInPerson = $scope.login;

        //function for logging in a user
        this.logInPerson = function () {
            //call the log in function in factory
            bookFactory.logInPerson($scope.login)
            .then(function () {
                // checking if the user is logged in 
                if (bookFactory.isAuthorized) {
                    $timeout(function () {
                        // setting that user is logged in
                        $rootScope.isAuthorized = true;
                        // setting what role the user has
                        $rootScope.userRole = bookFactory.userRole;
                        $rootScope.$apply();
                    }, 0);
                }
                //check if message is returned from factory in that case print the message on the page
                if (bookFactory.message.length !== 0) {
                    if (bookFactory.message === "Success") {
                        $window.location.href = "#/";
                    }
                    else {
                        $scope.statusMessage = bookFactory.message;
                    }
                }
            });
        };

        //function for cancelling the form input and setting the fields to empty
        this.reset = function () {
            $scope.login = {};
            $scope.statusMessage = "";
        }
    });
})();