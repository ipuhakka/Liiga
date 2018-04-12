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
				console.log(sessionStorage.getItem(item_type));
				element.style.backgroundColor = "white";
				return;
			}
		}
		items.push(text);
		element.style.backgroundColor = "#4156f4";
		sessionStorage.setItem(item_type, JSON.stringify(items));
		console.log(sessionStorage.getItem(item_type));
	}
			
}


function search(){
	console.log("you clicked me!");
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
		if (children[i].classList.contains(switchButtonSelected)){
			index = i;
			break;
		}
	}
	
	if (index === 2)
		return null;
	
	if (index === 1)
		return true;
	
	if (index === 0)
		return false;
		
	
} 

