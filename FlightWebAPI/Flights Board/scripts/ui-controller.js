
// Displays the page after loading is done.
function displayPage() {
    $('.main')[0].style.opacity = 1;
    $('.loading-page-anim')[0].style.display = "none";
}

function toggleTableLoadingAnimation(state) {
    if (state == true)
    {
        elm.$flightsTable.style.opacity = 0.5;
        $('.loading-data-anim')[0].style.display = "block";
    }
    else
    {
        elm.$flightsTable.style.opacity = 1;
        $('.loading-data-anim')[0].style.display = "none";
    }
}

function toggleErrorState(state) {
    if (state == true)
    {   
        if ($('.loading-data-anim')[0].style.display == "block")
            $('.loading-data-anim')[0].style.display = "none"
        
        elm.$flightsTable.style.opacity = 0.5;   
        $('.error-container')[0].style.display = "block";  
    }
    else
    {
        elm.$flightsTable.style.opacity = 1;
        $('.error-container')[0].style.display = "none";
    }
}

function enableTableFilters(state) {
    if (state == true)
    {
        elm.$airlineFilter.disabled = false;
        elm.$originFilter.disabled = false;
        elm.$destinationFilter.disabled = false;
        elm.$flightIdFilter.disabled = false;
    }
    else
    {
        elm.$airlineFilter.disabled = true;
        elm.$originFilter.disabled = true;
        elm.$destinationFilter.disabled = true;
        elm.$flightIdFilter.disabled = true;
    }
}

function disableAllControls() {
    enableTableFilters(false);
    elm.$arrivalsOption.disabled = true;
    elm.$departuresOption.disabled = true;
}