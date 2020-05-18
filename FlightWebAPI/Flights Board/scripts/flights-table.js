// This function clears the table and inserts all the given flights to it.
function insertFlightsToTable(flights) {

    while(elm.$flightsTable.rows.length > 1) {
        elm.$flightsTable.deleteRow(1);
    }
    
    flights.forEach(flight => {
        let time = getTimeStringFromDate(new Date(flight.time));
        let status;
        if (elm.$arrivalsOption.checked)
        {
            status = calculateFlightStatus(flight);
        }
        else if (elm.$departuresOption.checked)
        {
            status = "On Time";
        }
        $('#flights-table tr:last').after( '<tr> <td>'+ flight.flightId +'</td>'
                                              + '<td>'+ flight.airlineName +'</td>'
                                              + '<td>'+ flight.originCountry +'</td>'
                                              + '<td>'+ flight.destinationCountry +'</td>'
                                              + '<td>'+ time +'</td>'
                                              + '<td>'+ status +'</td> </tr>');
    });
}

// this function extract the time in foramt of 'hh:mm' from a given date.
function getTimeStringFromDate(date) {
    let hours = date.getHours();
    let minutes = date.getMinutes();
    if (minutes < 10)
        minutes = "0" + minutes;
    let time = hours + ':' + minutes;
    return time;
}

// this function calculates the status of the given arriving flight.
function calculateFlightStatus(flight) {

    let todayDateTime = new Date();
    let landingDateTime = new Date(flight.time);
    let timeDifference = (todayDateTime - landingDateTime)/(1000*60);

    switch (true) {
        case timeDifference <= -120:
            return "Not Final";
        case timeDifference <= -15 && timeDifference > -120:
            return "Final";
        case timeDifference <= 0 && timeDifference > -15:
            return "Landing";
        case timeDifference > 0:
            return "Landed";
    }
}

// this function filters the list of flights (by looking at the filters setted by the user)
function filterFlightsList() {

    let filteredFlights = new Array();

    for(let i=0; i<flights.length; i++) {
        if (elm.$airlineFilter.value != "any")
            if (flights[i].airlineName != elm.$airlineFilter.value)
                continue;
        if (elm.$originFilter.value != "any")
            if (flights[i].originCountry != elm.$originFilter.value)
                continue;
        if (elm.$destinationFilter.value != "any")
            if (flights[i].destinationCountry != elm.$destinationFilter.value)
                continue;
        if (elm.$flightIdFilter.value != "")
            if (flights[i].flightId != elm.$flightIdFilter.value)
                continue;
        filteredFlights.push(flights[i]);
    }

    insertFlightsToTable(filteredFlights);
}

function resetAllFliters() {
    elm.$airlineFilter.value = "any";
    elm.$originFilter.value = "any";
    elm.$destinationFilter.value = "any";
    elm.$flightIdFilter.value = "";
}