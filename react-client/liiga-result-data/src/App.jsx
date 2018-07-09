import React, { Component } from 'react';
import './css/App.css';
import Grid from 'react-bootstrap/lib/Grid';
import Row from 'react-bootstrap/lib/Grid';
import Col from 'react-bootstrap/lib/Grid';
import Form from 'react-bootstrap/lib/Form';
import Button from 'react-bootstrap/lib/Button';
import FormControl from 'react-bootstrap/lib/FormControl';
import ControlLabel from 'react-bootstrap/lib/ControlLabel';
import MatchList from './MatchList.jsx';
import ListSelect from './ListSelect.jsx';
import SelectOption from './SelectOption.jsx';
import LeagueTable from './LeagueTable.jsx';
import * as Sort from './Sort.js';

const BASE_URL = "http://localhost:3000/";
//const BASE_URL = "http://eff1e240.ngrok.io/";
const SEASONS_URL = "api/seasons/";
const TEAMS_URL = "api/teams/";

class App extends Component {
	constructor(props){
		super(props);
		
		this.updateMatchType = this.updateMatchType.bind(this);
		this.updateMatchVenue = this.updateMatchVenue.bind(this);
		this.updateGDSelector = this.updateGDSelector.bind(this);
		this.updateMatchEnd = this.updateMatchEnd.bind(this);
		this.updateMatchesBetween = this.updateMatchesBetween.bind(this);
		this.updateLeagueTableState = this.updateLeagueTableState.bind(this);
		this.updateMatchData = this.updateMatchData.bind(this);
		this.clickedTeam = this.clickedTeam.bind(this);
		this.clickedSeason = this.clickedSeason.bind(this);
		this.getSeasons = this.getSeasons.bind(this);
		this.getTeams = this.getTeams.bind(this);
		this.getMatches = this.getMatches.bind(this);
		this.handleGDChange = this.handleGDChange.bind(this);
		this.createCORSRequest = this.createCORSRequest.bind(this);
		this.alphabeticalSort = this.alphabeticalSort.bind(this);
		this.search = this.search.bind(this);
		this.createMatchesQuery = this.createMatchesQuery.bind(this);
		
		this.state = {
			playoff: null,
			homeMatches: null,
			gd_over: null,
			goal_difference: 0,
			gd_disabled: true,
			end_in_overtime: null,
			between: false,
			matchData: null,
			tableData: null
		}
		
	}
	
	componentDidMount(){
		this.getSeasons();
		this.getTeams();
	}
	
	render() {
		return (
		<div className="App">
			<header className="App-header">
				<h1 className="App-title">Liiga match data app</h1>
			</header>
			<Grid>
				<Row className="show-grid">
					<Col className="col-md-3 col-xs-6">
						<div className="list-div">
							<ListSelect itemState={this.state.teamSelected} onUpdate={this.clickedTeam} items={this.state.teams}></ListSelect>
						</div>
					</Col>
					<Col className="col-md-3 col-xs-6">
							<ListSelect itemState={this.state.seasonSelected} onUpdate={this.clickedSeason} items={this.state.seasons}></ListSelect>
					</Col>
					<Col className="col-md-6 col-xs-12">
						<SelectOption onUpdate={this.updateMatchesBetween} texts={["From teams", "Between teams"]}></SelectOption>
						<SelectOption onUpdate={this.updateMatchType} texts={["All matches", "Playoff", "Regular season"]}></SelectOption>
						<SelectOption onUpdate={this.updateMatchVenue} texts={["All venues", "Home", "Away"]}></SelectOption>
						<SelectOption onUpdate={this.updateGDSelector} texts={["All results", "Goal difference at least", "Goal difference at most"]}></SelectOption>
						<Form>
							<ControlLabel>
								Goal difference
							</ControlLabel>
							<FormControl disabled={this.state.gd_disabled} type="number"
								value={this.state.goal_difference}
								placeholder="Enter target goal difference"
								onChange={this.handleGDChange}>
							</FormControl>
						</Form>
						<SelectOption onUpdate={this.updateMatchEnd} texts={["All matches", "Ended in overtime", "Ended in regular time"]}></SelectOption>
						<Button onClick={this.search} bsStyle="success" className="searchButton">Search</Button>
					</Col>
				</Row>
				<Row className="show-grid">
					<Col className="col-md-6 col-xs-12">
						<LeagueTable ref={instance => {this.LeagueTable = instance; }} onUpdate={this.updateLeagueTableState} onlyHomeMatches={this.state.homeMatches} matchData={this.state.matchData} tableData={this.state.tableData}></LeagueTable>
					</Col>
					<Col className="col-md-6 col-xs-12">
						<MatchList onUpdate={this.updateMatchData} data={this.state.matchData}></MatchList>
					</Col>
				</Row>
			</Grid>
	   </div>
		);
	}
  
	/*Identify teams and seasons that are used as search parameter, create match query url and http request.*/
	search(){
		var teams = [];
		var seasons = [];
		
		for (var i = 0; i < this.state.teams.length; i++){
			if (this.state.teamSelected[i])
				teams.push(this.state.teams[i]);
		}
		
		for (i = 0; i < this.state.seasons.length; i++){
			if (this.state.seasonSelected[i])
				seasons.push(this.state.seasons[i]);
		}
		
		var url = this.createMatchesQuery(this.state.between, this.state.goal_difference, this.state.gd_over, this.state.playoff, this.state.homeMatches, this.state.end_in_overtime, teams, seasons);
		this.getMatches(url);
	}
	
	/*Creates url which is used to make the query for matches.*/
	createMatchesQuery(between, goal_difference, gd_is_at_least, playoff, played_at_home, match_end_in_overtime, teams, seasons){
	
		var url = BASE_URL + "api/matches?between=" + String(between);
	
		if (gd_is_at_least != null)
			url = url + "&gd_is_at_least=" + String(gd_is_at_least) + "&goal_difference=" + String(goal_difference);
		if (playoff != null)
			url = url + "&playoff=" + String(playoff);
		if (played_at_home != null)
			url = url + "&played_at_home=" + String(played_at_home);
		if (match_end_in_overtime != null)
			url = url + "&match_end_in_overtime=" + String(match_end_in_overtime);

		for (var i = 0; i < teams.length; i++){
			url = url + "&teams=" + teams[i];
		}

		for (i = 0; i < seasons.length; i++){
			url = url + "&seasons=" + seasons[i];
		}

		return url;
}
	
	/*creates a http-request to a given url. */
	getMatches(url){
		var xmlHttp = this.createCORSRequest("GET", url);

		if (xmlHttp) {

			xmlHttp.onreadystatechange =( () => {
				if (xmlHttp.readyState === 4 && xmlHttp.status === 200) {
					this.setState({
						matchData: Sort.sortByNewestDate(JSON.parse(xmlHttp.responseText), "date")
					});
					
					this.setState({
						tableData: this.LeagueTable.sortOfficialTable(this.LeagueTable.createLeagueTable())
					});
				}
				if (xmlHttp.readyState === 4 && xmlHttp.status !== 200) {
					window.alert("could not retrieve teams data");
					console.log("errormessage: " + xmlHttp.responseText);
				}
        });
        xmlHttp.send();
		}
	}
	
	/*creates an http-request to get an array of teams from the api. Array is sorted alphabetically,
	and then object state is changed to accommodate the data. seasonSelected is an array of length of the data array received.
	it is used initialized to keep track of what objects user has selected.
	On an error state, user is alerted of not receiving the data. 	*/
  	getTeams(){
		var xmlHttp = this.createCORSRequest("GET", BASE_URL + TEAMS_URL);

		if (xmlHttp) {

			xmlHttp.onreadystatechange =( () => {
				if (xmlHttp.readyState === 4 && xmlHttp.status === 200) {
					var data = this.alphabeticalSort(JSON.parse(xmlHttp.responseText));
					var selected = [];
					for (var i = 0; i < data.length; i++)
						selected.push(false);
					this.setState({
						teams: data,
						teamSelected: selected
					});
				}
				if (xmlHttp.readyState === 4 && xmlHttp.status !== 200) {
					window.alert("could not retrieve teams data");
					console.log("errormessage: " + xmlHttp.responseText);
				}
        });
		xmlHttp.withCredentials = false;
        xmlHttp.send();
		}
	}
	
	/*creates an http-request to get an array of seasons from the api. Array is sorted alphabetically,
	and then object state is changed to accommodate the data. seasonSelected is an array of length of the data array received.
	it is used initialized to keep track of what objects user has selected.
	On an error state, user is alerted of not receiving the data. 	*/
	getSeasons(){
		var xmlHttp = this.createCORSRequest("GET", BASE_URL + SEASONS_URL);

		if (xmlHttp) {

			xmlHttp.onreadystatechange =( () => {
				if (xmlHttp.readyState === 4 && xmlHttp.status === 200) {
					var data = this.alphabeticalSort(JSON.parse(xmlHttp.responseText));
					var selected = [];
					for (var i = 0; i < data.length; i++)
						selected.push(false);
					this.setState({
						seasons: data,
						seasonSelected: selected
					});
				}
				if (xmlHttp.readyState === 4 && xmlHttp.status !== 200) {
					window.alert("could not retrieve seasons");
					console.log("errormessage: " + xmlHttp.responseText);
				}
        });
		xmlHttp.withCredentials = false;
        xmlHttp.send();
		}
	}
	
	createCORSRequest(method, url) {
		var xhr = new XMLHttpRequest();
		if ("withCredentials" in xhr) {

			// Check if the XMLHttpRequest object has a "withCredentials" property.
			// "withCredentials" only exists on XMLHTTPRequest2 objects.
			xhr.open(method, url, true);

		} else if (typeof XDomainRequest !== "undefined") {

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
	
	/*user has clicked a team item of index 'key': this index is changed in the teamSelected array as the opposite value,
	so if the team was already selected, it is deselected.*/
  	clickedTeam(key){
		var selected = this.state.teamSelected;		
		selected[key] = !selected[key];
		
		this.setState({
			teamSelected: selected
		});
	}
	
	/*user has clicked a season item of index 'key': this index is changed in the seasonSelected array as the opposite value,
	so if the season was already selected, it is deselected.*/
	clickedSeason(key){
		var selected = this.state.seasonSelected;		
		selected[key] = !selected[key];
		
		this.setState({
			seasonSelected: selected
		});
	}
  
  /*set value for match type selector. selector is either null, true, or false.
  null = not a search criteria.
  true = only games from regular season are searched.
  false = only playoff matches are searched.*/
  updateMatchType(value){
	  this.setState({
		  playoff: value
	  });
  }
  
  /*sets the value for homeGames attribute. possible values: null, true, false.
  null = all venues are searched. 
  true = only home matches are searced.
  false = only away matches are searched.
  */
  updateMatchVenue(value){
	  this.setState({
		  homeMatches: value
	  });
  }
  
  /*Updates status of gd_over selector. Null = all matches are returned, true = games with goal difference over x are selected, false = games with
  goal difference less than x are selected.*/
  updateGDSelector(value){
	  var disableGD = true;
	  if (value != null)
		  disableGD = false;
	  
	  this.setState({
		  gd_over: value,
		  gd_disabled: disableGD
	  });
  }
  
  /*sets the value for end_in_overtime. 
  null = not a search criteria,
  true = serch matches that ended in overtime,
  false = search matches that ended in regular time.*/
  updateMatchEnd(value){
	  this.setState({
		  end_in_overtime: value
	  });
  }
  
  /*updates the between search parameter. Since SelectOption returns null when from is selected, this has to be interpreted as false value.*/
  updateMatchesBetween(value){
	  var between = true;
	  if (value === null)
		  between = false;
	  
	  this.setState({
		  between: between
	  });
  }
  
  updateLeagueTableState(data){
	  this.setState({
		  tableData: data
	  });
  }
  
  updateMatchData(data){
	  this.setState({
		  matchData: data
	  });
  }
  
  alphabeticalSort(data){
	return data.sort(function(a, b){
    if(a < b) return -1;
    if(a > b) return 1;
    return 0;
	})
  }
  
  	/*sets the state for goal_difference on user changing the value.*/
	handleGDChange(e){
		this.setState({
			goal_difference: e.target.value
		});
	}
}

export default App;
