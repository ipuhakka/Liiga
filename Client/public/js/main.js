//javascript source code file
function main() {
    /*getSeasons();
    getTeams();
	teams = [];
	teams.push("Kärpät");
	teams.push("Tappara");
	
	getMatches(true, 1, false, null, null, null, teams, null); */
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