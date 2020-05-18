module.constant('hostUrl', 'http://www.ybem.net/');

module.provider('apiUrls', ['hostUrl', function(hostUrl) {
    let apiUrls = {
        registerUserURL: hostUrl + 'api/guest/sign-up',
        checkUsernameURL: hostUrl + 'api/guest/check-username/',
        getCountriesURL: hostUrl + 'api/guest/get-all-countries',
        addAirlineToQueueURL: hostUrl + 'api/guest/add-airline-to-register-queue'
    };
  
    this.$get = function() {
      return apiUrls;
    };
  }]);