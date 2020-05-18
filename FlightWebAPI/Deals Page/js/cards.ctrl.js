module.controller("CardsCtrl", CardsCtrl);

function CardsCtrl($scope, $http) {

    const host = "http://www.ybem.net/"
    const getCountriesURL = host + "api/guest/get-all-countries";
    const getRoundtripFlightsURL = host + 'api/guest/get-roundtrip-flights';
    const flightsPageURL = 'http://www.ybem.net/flights'
    
    const redirectToFlightsPage = (url) => {
        window.parent.location.href = url;
    }

    $scope.cards = [];
    $scope.countries = [];
    $scope.selectedCountry = '';
    $scope.goToFlights = redirectToFlightsPage;

    $scope.errorState = false;
    $scope.loadingCountries = true;
    $scope.loadingDeals = false;

    
    // Runs when the page is loaded
    const onLoad = async function() {
        await getCountries();   
    }

    // Gets the countries from the server
    const getCountries = async function() { 

        await $http.get(getCountriesURL).then(
            (response) => {
                $scope.countries = response.data;
                countrycode.options[0].label = "- Select -";
                
            },
            (error) => {
                let errorObject = { 
                    message: "An error occurred while receiving the countries data",
                    data: error
                };
                console.log(errorObject);
                $scope.errorState = true;
            }
        );
        $scope.loadingCountries = false;
        $scope.$digest();
    }

    // Gets the deals from the server
    // It takes each available destination country, finds two flights (back and forth) and gets the price.
    $scope.getFlightsDeals = async function() {
        $scope.cards = [];

        let depatureDate = getDateString(1);
        let returnDate = getDateString(4);

        $scope.loadingDeals = true;

        await $scope.countries.forEach(async (country) => {

            if (country.Id == $scope.selectedCountry)
                return;

            let query = `/query?departureDate=${depatureDate}&returnDate=${returnDate}` + 
                        `&originCountryId=${$scope.selectedCountry}&destinationCountryId=${country.Id}`;

            let price = await getFlightsDealPrice(query);

            if (price == null)
                return;

            $scope.cards.push( {
                id: country.Id,
                name: country.Name,
                imgName: country.Name.toLowerCase() + ".png",
                price: price,
                link: flightsPageURL + query
            });
        })

        setTimeout(()=>{ 
            $scope.loadingDeals = false; 
            $scope.$digest(); 
        }, 500);
    }

    // A helper function for the get-flights-deals.
    // Makes an ajax call (using the given query) and returns the price of the flights.
    const getFlightsDealPrice = async function(query) {
        let flightPrice;
        await $http.get(getRoundtripFlightsURL + query).then(
            (response) => {
                if (response.data.length > 0)
                {
                    let flightPairs = response.data;
                    let price1 = flightPairs[0][0].ticketPrice;
                    let price2 = flightPairs[0][1].ticketPrice;
                    flightPrice = parseInt(price1) + parseInt(price2) + 1;
                }
            },
            (error) => {
                let errorObject = { 
                    message: "An error occurred while receiving the flights data",
                    data: error
                };
                console.log(errorObject);
                $scope.errorState = true;
                $scope.loadingDeals = false
            }
        );
        return flightPrice;
    }

    // A helper function for the get-flights-deals.
    // Gets today's date and returns a string in 'yyyy-mm-dd' format with the ability of adding days to it.
    const getDateString = function(addDays) {
        let date = new Date().setDate(new Date().getDate() + addDays);
        date = new Date(date);
        day = date.getDate();

        let month = date.getMonth()+1;

        if (day < 10)
            day = "0" + day;
        if (month < 10)
            month = "0" + month;
            
        date = `${date.getFullYear()}-${month}-${day}`;
        return date;
    }

    onLoad();
}
