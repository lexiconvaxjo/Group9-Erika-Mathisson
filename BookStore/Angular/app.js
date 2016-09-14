(function () {

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

            // route for the log in page
            .when('/logIn', {
                templateUrl: '/Angular/Templates/login.html',
                controller: 'loginController'
            });
    });

    app.factory('bookFactory', function ($http, $window, $rootScope, $timeout) {

        var bookFactory = {};
        bookFactory.message = "";
        bookFactory.people = [];
        bookFactory.isAuthorized = false;
        bookFactory.userRole = null;

        // for checking if user is logged in
        $http.post("/Person/IsLoggedIn").then(function (response) {
            if (response.data.status === true) {
                bookFactory.isAuthorized = true;
                bookFactory.userRole = response.data.role;
            }
        })

        //function for register a person
        bookFactory.registerPerson = function (person) {
            console.log("RegisterPerson function factory");
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
                 bookFactory.isAuthorized = true;
                bookFactory.userRole = data.role;
            //});
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
                console.log(data);
            })
            .error(function () {
                // setting error message
                bookFactory.message = "Something went wrong...";
            });
        };
        
        //function for getting a person with a specific id and returning it
        bookFactory.get = function (Id) {
            console.log("bookfactory get");
            console.log(Id);
            for (var i in bookFactory.people) {
                console.log(i);
                console.log(bookFactory.people[i].Id);
                if (bookFactory.people[i].Id === Id) {
                    console.log("Id === Id");
                    console.log(bookFactory.people[i]);
                    return bookFactory.people[i];
                }
            }
        };

        return bookFactory;
    });

    //  main controller with $scope injected
    app.controller('mainController', function ($scope, $rootScope, bookFactory) {
        $scope.message = 'This is the main page';
        $rootScope.$watch(function () {
            return bookFactory.isAuthorized;
        }, function (n, o) {
            if (n === true) {
                $scope.isLoggedIn = true;
                $scope.role = $rootScope.userRole;
            }
        })
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

        //function for setting the array of people to the scope
        bookFactory
             .getPeople()
         .then(function () {
             $scope.people = bookFactory.people;

         });

        //function for copying information about a specific person and paste the information to the form 
        this.edit = function (Id) {
            console.log("edit people in controller");
            console.log(bookFactory.people);
            console.log(Id);
            $scope.person = angular.copy(bookFactory.get(Id));           
            console.log($scope.person);          
        };


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
                if (bookFactory.isAuthorized) {
                    $timeout(function () {
                        $rootScope.isAuthorized = true;
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