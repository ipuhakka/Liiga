// JavaScript source code for Liiga-API client program.

const BASE_URL = "http://localhost:3000/";
//const BASE_URL = "http://3f92b9ec.ngrok.io/";
const SEASONS_URL = "api/seasons/";
const TEAMS_URL = "api/teams/";


/*
This function creates a http GET-request to http://hostURI/api/seasons. 
On success, the data received is stored into sessionStorage. 
*/
function getSeasons(callback, param1, param2) {

    var xmlHttp = createCORSRequest("GET", BASE_URL + SEASONS_URL);

    if (xmlHttp) {

        xmlHttp.onreadystatechange = function () {

            if (xmlHttp.readyState == 4 && xmlHttp.status === 200) {
                callback(JSON.parse(xmlHttp.responseText), param1, param2);
            }
            if (xmlHttp.readyState == 4 && xmlHttp.status !== 200) {
                window.alert("could not retrieve seasons");
				console.log("errormessage: " + xmlHttp.responseText);
            }

        }
		xmlHttp.withCredentials = false;
        xmlHttp.send();
    }

}

/*
This function creates a http GET-request to http://hostURI/api/teams. 
On success, the data received is stored into sessionStorage. 
*/
function getTeams(callback, param1, param2){
	
	var xmlHttp = createCORSRequest("GET", BASE_URL + TEAMS_URL);

    if (xmlHttp) {

        xmlHttp.onreadystatechange = function () {

            if (xmlHttp.readyState == 4 && xmlHttp.status === 200) {
                callback(JSON.parse(xmlHttp.responseText), param1, param2);
            }
            if (xmlHttp.readyState == 4 && xmlHttp.status !== 200) {
                window.alert("could not retrieve teams");
				console.log("errormessage: " + xmlHttp.responseText);
            }

        }
		xmlHttp.withCredentials = false;
        xmlHttp.send();
    }
	
}

/*
This function creates a http GET-request to http://hostURI/api/seasons
and returns the data received on response body.

params:
	between: true= matches between parameter teams are searched.
	goal_difference: integer, border value what scores are looked for. 
	gd_at_least: true = matches with gd at at least goal_difference are searched for.
	playoff: true = playoff matches searched, false = regular season games. null = all types.
	played_at_home: true = only home matches of parameter teams are searched. false = only away games. null, all games are
	 searched.
	 
	end_in_overtime: true = matches that ended in overtime or shootout are searched for. false = matches that ended
		after 60 minutes are searched. null = all type of endings are searched for.
		
	teams: Array of string teamnames that are searched for.
	seasons = Array of string seasons from which matches are searched for. 
	
	callback is displayMatches, which sets the UI to show search results.
*/
function getMatches(between, goal_difference, gd_is_at_least, playoff, played_at_home, match_end_in_overtime, teams, seasons, callback){
	
	var url = createMatchesQuery(between, goal_difference, gd_is_at_least, playoff, played_at_home, match_end_in_overtime, teams, seasons);
	
	console.log(url);
	
	sessionStorage.setItem("USE_HOME_GAMES_ONLY", played_at_home); //used to use only correct data in calculating the table.
	
	var xmlHttp = createCORSRequest("GET", url);
	
	if (xmlHttp) {

        xmlHttp.onreadystatechange = function () {

            if (xmlHttp.readyState == 4 && xmlHttp.status === 200) {
				console.log("Data received from server");
				sessionStorage.setItem('matchData', xmlHttp.responseText);
                callback('date');
            }
            if (xmlHttp.readyState == 4 && xmlHttp.status !== 200) {
                window.alert("could not retrieve matches");
				console.log("errormessage: " + xmlHttp.responseText);
            }

        }

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

function createMatchesQuery(between, goal_difference, gd_is_at_least, playoff, played_at_home, match_end_in_overtime, teams, seasons){
	
	var url = BASE_URL + "api/matches?between=" + String(between);
	
	if (gd_is_at_least != null)
		url = url + "&gd_is_at_least=" + String(gd_is_at_least) + "&goal_difference=" + String(goal_difference);
	
	if (playoff != null)
		url = url + "&playoff=" + String(playoff);
	
	if (played_at_home != null)
		url = url + "&played_at_home=" + String(played_at_home);
	
	if (match_end_in_overtime != null)
		url = url + "&match_end_in_overtime=" + String(match_end_in_overtime);
	
	if (teams != null){
	
		for (var i = 0; i < teams.length; i++){
			url = url + "&teams=" + teams[i];
		}
	
	}
	
	if (seasons != null){
		for (var i = 0; i < seasons.length; i++){
			url = url + "&seasons=" + seasons[i];
		}
	}
	
	return url;
}