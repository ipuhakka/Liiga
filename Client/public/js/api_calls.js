// JavaScript source code for Liiga-API client program.

const SEASONS_KEY = "SEASONS_KEY";
const SEASONS_URL = "http://localhost:51678/api/seasons";

/*
This function creates a http GET-request to http://hostURI/api/seasons. 
On success, the data received is stored into sessionStorage. 
*/
function getSeasons() {

    var xmlHttp = createCORSRequest("GET", SEASONS_URL);

    if (xmlHttp) {

        xmlHttp.onreadystatechange = function () {

            if (xmlHttp.readyState == 4 && xmlHttp.status === 200) {
                console.log(xmlHttp.responseText);
            }
            if (xmlHttp.status !== 200) {
                console.log("Error happened, "+ xmlHttp.status);
            }

        }

        xmlHttp.withCredentials = false;
        xmlHttp.send();
    }

}

function createCORSRequest(method, url) {
    var xhr = new XMLHttpRequest();
    if ("withCredentials" in xhr) {

        // Check if the XMLHttpRequest object has a "withCredentials" property.
        // "withCredentials" only exists on XMLHTTPRequest2 objects.
        xhr.open(method, url, true);

    } else if (typeof XDomainRequest != "undefined") {

        // Otherwise, check if XDomainRequest.
        // XDomainRequest only exists in IE, and is IE's way of making CORS requests.
        xhr = new XDomainRequest();
        xhr.open(method, url);

    } else {

        // Otherwise, CORS is not supported by the browser.
        xhr = null;

    }
    return xhr;
}