let elm;

const host = "http://www.ybem.net/"

const getArrivalsURL = host + "api/guest/get-arrival-flights";
const getDepartureURL = host + "api/guest/get-departure-flights";
const getAirlinesURL = host + "api/guest/get-all-airlines";
const getCountriesURL = host + "api/guest/get-all-countries";

let flights;

// On page load
$(async function() {

    initElements();
    setEventListeners();

    if (!await setAirlineCompaniesList())
        return;

    if (!await setCountriesList())
        return;

    displayPage();   
    getFlightsToTable();

    setInterval(getFlightsToTable, 1000*60*5);
});

// Initialize all DOM elements
function initElements() {
    elm = {
        $arrivalsOption: $('#arrivals-option')[0],
        $departuresOption: $('#departures-option')[0],
        $airlineFilter: $('#airlines')[0],
        $originFilter: $('#origin')[0],
        $destinationFilter: $('#destination')[0],
        $flightIdFilter: $('#flight-id')[0],
        $flightsTable: $('#flights-table')[0]
    }
}

// Setting all event listeners for the controls
function setEventListeners() {

    elm.$arrivalsOption.onchange = getFlightsToTable;
    elm.$departuresOption.onchange = getFlightsToTable;

    elm.$airlineFilter.onchange = filterFlightsList;
    elm.$originFilter.onchange = filterFlightsList;
    elm.$destinationFilter.onchange = filterFlightsList;
    elm.$flightIdFilter.oninput = filterFlightsList;
}

// Getting all the airlines from the server and adding them to a the list.
async function setAirlineCompaniesList() {   
    
    let airlines;

    try
    {
        airlines = await getWithAJAXFromAPI(getAirlinesURL);
    }
    catch (error)
    {
        error.message = "An error occurred while receiving the flights data";
        console.log(error);
    }    
    
    if (airlines == null)
    {   
        displayPage();
        disableAllControls();
        toggleErrorState(true);
        return false;
    }

    airlines.forEach(airline => {
        let airlineOption = document.createElement("option");
        airlineOption.text = airline.Name;
        elm.$airlineFilter.add(airlineOption);
    });

    return true;
}

// Getting all the countries from the server and adding them to a the list.
async function setCountriesList() {

    let countries;

    try
    {
        countries = await getWithAJAXFromAPI(getCountriesURL);
    }
    catch(error)
    {
        error.message = "An error occurred while receiving the countries data";
        console.log(error);
    }
        
    if (countries == null)
    {   
        displayPage();
        disableAllControls()
        toggleErrorState(true);
        return false;
    }

    countries.forEach(country => {
        let countryOption = document.createElement("option");
        countryOption.text = country.Name;
        elm.$originFilter.add(countryOption);
        elm.$destinationFilter.add(countryOption.cloneNode(true));
    });

    return true;
}

// Getting all the flights (arrivals or departures) from the server and displaying them.
async function getFlightsToTable() {
    
    toggleTableLoadingAnimation(true);
    toggleErrorState(false);
    resetAllFliters();
    enableTableFilters(false);

    $('.no-flights-message')[0].style.display = "none";

    let data;

    if (elm.$arrivalsOption.checked)
    {
        try
        {
            data = await getWithAJAXFromAPI(getArrivalsURL);
        }
        catch(error)
        {
            error.message = "An error occurred while receiving the arriving flights data";
            console.log(error);
        }
    }
    else if (elm.$departuresOption.checked)
    {
        try
        {
            data = await getWithAJAXFromAPI(getDepartureURL);
        }
        catch(error)
        {
            error.message = "An error occurred while receiving the departuring flights data";
            console.log(error);
        }
    }

    if (data == null)
    {
        toggleErrorState(true);
        return;
    }

    else if (data == "[]")
    {
        $('.no-flights-message')[0].style.display = "block";
    }

    flights = data;

    insertFlightsToTable(flights);
    
    toggleTableLoadingAnimation(false);
    enableTableFilters(true);
}
