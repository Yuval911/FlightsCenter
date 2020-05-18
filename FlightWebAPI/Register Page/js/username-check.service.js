module.factory('checkUsername', ['$http', 'apiUrls', function($http, apiUrls) {
    
    // this function sends a request to the server and gets a response if the username is available or not.
    return async function(username) {

      const requestURL = apiUrls.checkUsernameURL + username;
      let response;

      await $http.get(requestURL).then (
          (serverResponse) => {
              response = serverResponse.data;
          },
          (error) => {
              let errorObject = { 
                  message: "An error occurred while checking the username with the server",
                  data: error
              };
              console.log(errorObject);
          }
      );
      
      if (response == null)
      {
        return "error";
      }
      else if (response == true)
      {
        return "available";
      }
      else if (response == false)
      {
        return "taken";
      }
    }
}]);