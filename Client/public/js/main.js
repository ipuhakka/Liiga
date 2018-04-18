//javascript source code file
function main() {
	sessionStorage.setItem('team', null);
	sessionStorage.setItem('season', null);
	getSeasons(addSelectItemsToUI, "seasonDiv", "season");
	getTeams(addSelectItemsToUI, "teamDiv", "team");
}

/* click method for all switchButton-elements. Pressed button is selected. Sibling buttons are updated to notSelected.*/
function pressedSelector(button){

	button.classList.remove("switchButtonNotSelected");
	button.classList.add("switchButtonSelected");
	
    siblings = button.parentNode.children;
	
	for (var i = 0; i < siblings.length; i++){
		if (siblings[i] != button){
			siblings[i].classList.remove("switchButtonSelected");
			siblings[i].classList.add("switchButtonNotSelected");
		}
	}
}


/*Selects or deselects pressed element. Pressed element contains a text element with textContent.

  Gets children elements of clicked div, checks if sessionStorage contains
  data for item_type, checks if sessionStorage contains selected item, 
  and either deselects item or selects it. SessionStorage is updated with new array.
 */
function itemSelected(element, item_type){
	
	var children = element.children;
	
	var items = JSON.parse(sessionStorage.getItem(item_type));

	if (items == null){
		items = [];
	}
	
	if (children.length > 0){
		text = children[0].textContent;
		for (var i = 0; i < items.length; i++){
			if (items[i] == text) //deselect
			{
				items.splice(i, 1);
				sessionStorage.setItem(item_type, JSON.stringify(items));
				element.style.backgroundColor = "white";
				return;
			}
		}
		items.push(text);
		element.style.backgroundColor = "#4156f4";
		sessionStorage.setItem(item_type, JSON.stringify(items));
	}
			
}

/*
Get search parameters, and use getMatches from api_calls.js.
*/
function search(){
		
	var gd_is_at_least = findParameterValue("match_gd_selector");	
	var goal_difference = parseInt(document.getElementById("goal_difference").value);
	
	if (gd_is_at_least !== null){		
		if (!Number.isInteger(goal_difference)){	
			window.alert("Please input goal difference as a number");
			return;
		}
	}
	
	var between = findParameterValue("matches_against_selector");
	var playoff = findParameterValue("match_type_selector");
	var played_at_home = findParameterValue("match_venue_selector");
	var match_end_in_overtime = findParameterValue("match_end_selector");
	
	var teams = JSON.parse(sessionStorage.getItem('team'));
	var seasons = JSON.parse(sessionStorage.getItem('season'));
	
	getMatches(between, goal_difference, gd_is_at_least, playoff, played_at_home, match_end_in_overtime, teams, seasons, displayResults);
}

/*
Function is used to assign a value for a search parameter. 
Function checks which index has class switchButtonSelected assigned to it.
if 2, return null,
if 1, return true,
if 0, return false.
*/
function findParameterValue(element_id){
	
	var element = document.getElementById(element_id);
	
	var children = element.children;
	var index = 0;
	
	for (var i = 0; i < children.length; i++){
		if (children[i].classList.contains("switchButtonSelected")){
			value = children[i].value;
			
			if (value === "null")
				return null;
			
			if (value === "false")
				return false;
			
			if (value === "true")
				return true;
		}
	}
			
} 

function sortByParam(param, data) {

	if (param === 'date')
		return sortByDate(data);

	return sortData(data, param);

}

/* General sorting for sorting without tiebreakers.*/
function sortData(data, param)
{
    return data.sort(function (a, b) {
		return b[param] - a[param];
    });
}

/*league table sorting with tiebreakers. */
function sortOfficialTable(data){
	return data.sort(function (a, b) {
		var points = b['points'] - a['points'];
		
		if (points !== 0)
			return points;
		
		var wins = b['won'] - a['won'];
		
		if (wins !== 0)
			return wins;
		
		var goal_difference = (b['goals'] - b['against']) - (a['goals'] - a['against']);
		
		if (goal_difference !== 0)
			return goal_difference;
		
		var scored = b['scored'] - a['scored'];
			return scored;		
    });
}


function sortByDate(data){
	
	/*Return year if there is a difference, return month if there's a difference, 
	and finally return day*/
	return data.sort(function (a, b) {
		aSplitted =	a['date'].split('-');
		bSplitted = b['date'].split('-');
		var year = bSplitted[0] - aSplitted[0];
		
		if (year !== 0)
			return year;
		
		var month = bSplitted[1] - aSplitted[1];
		
		if (month !== 0)
			return month;
		
		return bSplitted[2] - aSplitted[2];
		
    }); 
	
}


/*Function creates a table from matches received from server.
Each match is processed, teams added to teams array, and then points per game is calculated.*/
function createLeagueTable(){
	
	matches = JSON.parse(sessionStorage.getItem('matchData'));
	
	var teams = [];
	
	//go through each match
	for (var i = 0; i < matches.length; i++){
		
		var hometeamFound = false;
		var awayteamFound = false;
		
		//go through each team in array
		for (var j = 0; j < teams.length; j++){
			if (teams[j].name === matches[i].hometeam){ //home team already in array
				teams = appendHometeamData(teams, matches[i], j);
				hometeamFound = true;
			}
			
			if (teams[j].name === matches[i].awayteam){ //away team already in array
				teams = appendAwayteamData(teams, matches[i], j);
				awayteamFound = true;
			}
		}
		
		if (!hometeamFound){
			teams.push(appendTeam(matches[i].hometeam, matches[i], true));
		}
		
		if (!awayteamFound){
			teams.push(appendTeam(matches[i].awayteam, matches[i], false));
		}
		
	}
	
	teams = calculatePointsPerGame(teams);

	return teams;
}

/*
Calculates points per game for all teams in array, returns teams array.
*/
function calculatePointsPerGame(teams){
	
	for (var i = 0; i < teams.length; i++){
		teams[i].pointsPerGame = Math.round(100 * teams[i].points / teams[i].matches) / 100;
	}
	
	return teams;
}

/*
Team wasn't already included in the teams array, so it must be added there. Returns a team object
*/
function appendTeam(teamname, match, host){
	var scored, conceded;
	var wins = 0;
	var OTWins = 0; 
	var OTLosses = 0;
	var losses = 0; 
	var points = 0;
	var matches = 1;
	
	if (host){
		scored = match.homescore;
		conceded = match.awayscore;
	}
	
	else {
		scored = match.awayscore;
		conceded = match.homescore;
	}
	
	if (scored > conceded){
		if (match.overtime){ //won on overtime
			points = 2;
			OTWins = 1;
		}
		else { //won on regular time
			points = 3;
			wins = 1;
		}			
	}
	else {
		if (match.overtime){ //lost on overtime
			points = 1;
			OTLosses = 1;
		}
		else { //lost on regular time
			losses = 1;
		}
	}

	
	var team = {
		name: teamname,
		matches: matches,
		won: wins,
		otWins: OTWins,
		otLosses: OTLosses,
		lost: losses,
		goals: scored,
		against: conceded,
		points: points,
		pointsPerGame: 0
	};
	
	return team;
}

/*
Adds hometeam data to array. This means adding new match data to teams statistics.
*/
function appendHometeamData(teams, match, j){
	teams[j].goals = teams[j].goals + match.homescore;
	teams[j].against = teams[j].against + match.awayscore;
				
	teams[j].matches = teams[j].matches + 1;
				
	if (match.homescore > match.awayscore){ //home win
		if (match.overtime){
			teams[j].otWins = teams[j].otWins + 1;
			teams[j].points = teams[j].points + 2;
		}
		else { //win on regular time 
			teams[j].won = teams[j].won + 1;
			teams[j].points = teams[j].points + 3;
		}
	}
	else { //away win
		if (match.overtime) {
			teams[j].otLosses = teams[j].otLosses + 1;
			teams[j].points = teams[j].points + 1;
		}
		else //lost on regular time
			teams[j].lost = teams[j].lost + 1;
		}
		
	return teams;
}

/*
Adds hometeam data to array. This means adding new match data to teams statistics.
*/
function appendAwayteamData(teams, match, j){
	teams[j].goals = teams[j].goals + match.awayscore;
	teams[j].against = teams[j].against + match.homescore;
				
	teams[j].matches = teams[j].matches + 1;
				
	if (match.awayscore > match.homescore){ //away win
		if (match.overtime){
			teams[j].otWins = teams[j].otWins + 1;
			teams[j].points = teams[j].points + 2;
		}
		else { //win on regular time 
			teams[j].won = teams[j].won + 1;
			teams[j].points = teams[j].points + 3;
		}
	}
	else { //home win
		if (match.overtime) {
			teams[j].otLosses = teams[j].otLosses + 1;
			teams[j].points = teams[j].points + 1;
		}
		else //lost on regular time
			teams[j].lost = teams[j].lost + 1;
		}
		
	return teams;
}

