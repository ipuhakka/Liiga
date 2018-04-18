//callback which handles receiving results from the server.
function displayResults(sortedBy){
	displayMatches(sortedBy);
	displayLeagueTable("points");
}

function displayLeagueTable(sortedBy){
	data = createLeagueTable();
	
	if (sortedBy === "points")
		teamsSorted = sortOfficialTable(data);
	else
		teamsSorted = sortByParam(sortedBy, data);
	
	clearTable('leagueTable');
	
	var table = document.getElementById('leagueTable');
	
	for (var i = 0; i < teamsSorted.length; i++){

        var row = table.insertRow(i + 1);

        var cell1 = row.insertCell(0);
        var cell2 = row.insertCell(1);
        var cell3 = row.insertCell(2);
        var cell4 = row.insertCell(3);
        var cell5 = row.insertCell(4);
		var cell6 = row.insertCell(5);
		var cell7 = row.insertCell(6);
		var cell8 = row.insertCell(7);
		var cell9 = row.insertCell(8);
		var cell10 = row.insertCell(9);

        cell1.innerHTML = teamsSorted[i].name;
        cell2.innerHTML = teamsSorted[i].matches;
        cell3.innerHTML = teamsSorted[i].won;
        cell4.innerHTML = teamsSorted[i].otWins;
		cell5.innerHTML = teamsSorted[i].otLosses;
		cell6.innerHTML = teamsSorted[i].lost;
		cell7.innerHTML = teamsSorted[i].goals;
		cell8.innerHTML = teamsSorted[i].against;
		cell9.innerHTML = teamsSorted[i].points;
		cell10.innerHTML = teamsSorted[i].pointsPerGame;
		
	}
	
}

function displayMatches(sortedBy){
	
	matchesParsed = JSON.parse(sessionStorage.getItem('matchData'));
	matchesSorted = sortByParam(sortedBy, matchesParsed);

	clearTable("matchesTable");
	
	var table = document.getElementById('matchesTable');
	
	for (var i = 0; i < matchesSorted.length; i++){

        var row = table.insertRow(i + 1);

        var cell1 = row.insertCell(0);
        var cell2 = row.insertCell(1);
        var cell3 = row.insertCell(2);
        var cell4 = row.insertCell(3);
        var cell5 = row.insertCell(4);
		var cell6 = row.insertCell(5);

        cell1.innerHTML = matchesSorted[i].hometeam;
        cell2.innerHTML = matchesSorted[i].awayteam;
        cell3.innerHTML = matchesSorted[i].homescore;
        cell4.innerHTML = matchesSorted[i].awayscore;
		cell6.innerHTML = matchesSorted[i].date;
		
		if (matchesSorted[i].overtime)
			cell5.innerHTML = "ot.";
		
	}
	
}

function addSelectItemsToUI(element_array, parent_id, item_type){
	
	var parentDiv = document.getElementById(parent_id);
	
	for (var i = 0; i < element_array.length; i++){
		var div = document.createElement('div');
		div.classList.add("selectItem");
		div.onclick = function(){itemSelected(this, item_type);}
		var textElement = document.createElement('text');
		textElement.textContent = element_array[i];
		
		div.appendChild(textElement);
		parentDiv.appendChild(div);
	}
	
}

function clearTable(id) {
    //clears table (all but headers)
    var count = $('#' + id + ' tr').length;

    for (var i = count; i > 1; i--) {
        document.getElementById(id).deleteRow(i - 1);
    }
}