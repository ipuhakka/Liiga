//javascript source code file
function main() {
    getSeasons();
    getTeams();
	teams = [];
	teams.push("Kärpät");
	teams.push("Tappara");
	
	getMatches(true, 1, false, null, null, null, teams, null);
}