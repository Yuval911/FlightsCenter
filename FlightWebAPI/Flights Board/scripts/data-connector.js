
// This is a generic function that gets data from the server with AJAX request, using the given url.
// The request is a simple "GET" with no parameters.
async function getWithAJAXFromAPI(apiURL) {
    return new Promise((resolve, reject) => {
        $.ajax({
            url: apiURL,
            type: 'GET',
            timeout: 30000,
            success: (response) => {
                resolve(response);
            },
            error: (error) => {
                let errorObject = { 
                    message: "", // error message will be writen by the calling function.
                    data: error
                };
                reject(errorObject);
            }
        })
    });
}

