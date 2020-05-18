module.controller("UserRegisterCtrl", UserRegisterCtrl);

function UserRegisterCtrl($scope, $http, $timeout, apiUrls, checkUsername) {

    $scope.scopeForm = form;

    $scope.customer = {
        firstName: "",
        lastName: "",
        userName: "",
        email: "",
        password: "",
        address: "",
        phoneNo: "",
        creditCardNo: ""
    }

    const redirectToLoginPage = () => {
        window.parent.location.href = "http://www.ybem.net/login";
    }
    
    $scope.goToLoginPage = redirectToLoginPage;

    // These properties were made for the UI to shift:

    $scope.isLoading = false;
    $scope.submitStatus = "notSubmitted"; // 'loading', 'succedded', 'error'   
    $scope.usernameCheckStatus = "checking"; // 'checking', 'available', 'taken'
    
    $scope.register = async function() {
  
        $scope.isLoading = true;
        let response;

        await $http({
                method: 'POST',
                url: apiUrls.registerUserURL,
                headers: {
                    'Content-Type': 'application/json'
                  },
                data: $scope.customer,
            }).then(
            (serverResponse) => {
                response = serverResponse;
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
        );

        $scope.isLoading = false;
        
        $scope.$digest();
        $scope.$apply();
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
}
