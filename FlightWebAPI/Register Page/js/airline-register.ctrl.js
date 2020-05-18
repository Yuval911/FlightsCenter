module.controller("AirlineRegisterCtrl", AirlineRegisterCtrl);

function AirlineRegisterCtrl($scope, $http, $timeout, apiUrls, checkUsername) {

    $scope.scopeForm = form;

    $scope.airline = {
        name: "",
        userName: "",
        email: "",
        password: "",
        countryCode: ""
    }

    $scope.countries;

    const redirectToHomePage = () => {
        window.parent.location.href = "http://www.ybem.net/";
    }
    
    $scope.goToHomePage = redirectToHomePage;

    // These properties were made for the UI to shift:

    $scope.pageStatus = "loading"; // 'loaded', 'error'
    $scope.submitStatus = "notSubmitted"; // 'loading', 'succedded', 'error'
    $scope.usernameCheckStatus = "checking"; // 'checking', 'available', 'taken'
    

    // Runs when the page is loaded
    const onLoad = async function() {  
 
        await getCountries();
        $scope.$digest();
    }

    // Gets the list of countries from the server
    const getCountries = async function() { 

        await $http.get(apiUrls.getCountriesURL).then(
            (response) => {
                $scope.countries = response.data;
                $scope.pageStatus = "loaded";
                $scope.scopeForm.countrycode.options[0].label = "- Select -";
            },
            (error) => {
                let errorObject = { 
                    message: "An error occurred while receiving the countries data",
                    data: error
                };
                console.log(errorObject);
                $scope.pageStatus = "error";
            }
        );
    }

    // This function adds the airline company to the register queue, and later the administrator will review
    // and approve it.
    $scope.register = async function() {
  
        $scope.submitStatus = "loading";

        await $http.post(apiUrls.addAirlineToQueueURL, $scope.airline).then(
            (respone) => {
                $scope.submitStatus = "succedded";
            },
            (error) => {
                let errorObject = { 
                    message: "An error occurred while sending the form data",
                    data: error
                };
                console.log(errorObject);
                $scope.submitStatus = "error";
                setTimeout(() => {window.scrollTo(0,document.body.scrollHeight)},50); 
            }
        )
        
        $scope.$digest();
    }

    let timeoutPromise;

    // This function implements a little delay when watching the username's textbox, then it procceeds to check
    // the username's availability with the server.
    $scope.$watch("scopeForm.username.value", function() {
        $timeout.cancel(timeoutPromise);
        timeoutPromise = $timeout(async function() {

            $scope.usernameCheckStatus = "checking";

            if (form.username.value.length > 0 && form.username.value.length <= 20)
            {
                $scope.usernameCheckStatus = await checkUsername(form.username.value);
                $scope.$digest();
            }

        },700);
    });
    
    onLoad();
}
