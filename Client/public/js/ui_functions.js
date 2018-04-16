function displayMatches(sortedBy){
	
	matchesParsed = JSON.parse(sessionStorage.getItem('matchData'));
	matchesSorted = sortByParam(sortedBy, matchesParsed);

	clearTable();
	
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

function clearTable() {
    //clears table (all but headers)
    var count = $('#matchesTable tr').length;

    for (var i = count; i > 1; i--) {
        document.getElementById('matchesTable').deleteRow(i - 1);
    }
}