import React, { Component } from 'react';
import './css/App.css';
import Table from 'react-bootstrap/lib/Table';
import * as Sort from './Sort.js';

class LeagueTable extends Component{
	constructor(props){
		super(props);
		
		this.sortOfficialTable = this.sortOfficialTable.bind(this);
		this.createLeagueTable = this.createLeagueTable.bind(this);
		this.appendTeam = this.appendTeam.bind(this);
		this.appendHometeamData = this.appendHometeamData.bind(this);
		this.appendAwayteamData = this.appendAwayteamData.bind(this);	
		this.sortBy = this.sortBy.bind(this);
		this.usesOldScoring = this.usesOldScoring.bind(this);
		this.otLoss0points = this.otLoss0points.bind(this);
		
		this.state={
			lastSortedBy: null,
			byHighest: true
		}
	}
	
	render(){
		var rows = [];

		if (this.props.tableData !== null){
			
			var teams = this.props.tableData;
			var color;
			for (var i = 0; i < teams.length; i++){
				if (i < 6)
					color = "success";
				else if (i < 10)
					color = "default";
				else
					color = "danger";
				rows.push(<tr className={color} key={i}>
							<td>{teams[i].name}</td>
							<td>{teams[i].matches}</td>
							<td>{teams[i].points}</td>
							<td>{teams[i].won}</td>
							<td>{teams[i].otWins}</td>
							<td>{teams[i].otLosses}</td>
							<td>{teams[i].lost}</td>
							<td>{teams[i].goals}</td>
							<td>{teams[i].against}</td>
							<td>{teams[i].pointsPerGame}</td>
						  </tr>);
			}
		}
		return(
			<Table responsive>
				<thead>
					<tr>
						<th onClick={() => this.sortBy("name")}>Team</th>
						<th onClick={() => this.sortBy("matches")}>Played</th>
						<th onClick={() => this.sortBy("points")}>Points</th>
						<th onClick={() => this.sortBy("won")}>Won</th>
						<th onClick={() => this.sortBy("otWins")}>2 p.</th>
						<th onClick={() => this.sortBy("otLosses")}>1 p.</th>
						<th onClick={() => this.sortBy("lost")}>Lost</th>
						<th onClick={() => this.sortBy("goals")}>Scored</th>
						<th onClick={() => this.sortBy("against")}>Against</th>
						<th onClick={() => this.sortBy("pointsPerGame")}>PPG</th>
					</tr>
				</thead>
				<tbody>{rows}</tbody>
			</Table>
		);
	}
	
	/*chooses the correct sort method based on the parameter and in what order list was already sorted.*/
	sortBy(param){
		if (param === 'points')
			this.props.onUpdate(this.sortOfficialTable(this.props.tableData));
		
		else {
			if (param === this.state.lastSortedParam && !this.state.byHighest){
				if (param === "name")
					this.props.onUpdate(Sort.sortAlphabeticallyBackwards(this.props.tableData, param));
				else 
					this.props.onUpdate(Sort.sortBySmallest(this.props.tableData, param));	
				
				this.setState({byHighest: true});
			}				
			else {
				if (param === "name")
					this.props.onUpdate(Sort.sortAlphabetically(this.props.tableData, param));
				else
					this.props.onUpdate(Sort.sortByHighest(this.props.tableData, param));
				
				this.setState({byHighest: false});
			}

		}
		this.setState({
			lastSortedParam: param
		});
	}
	
	sortOfficialTable(data){
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
	
	/*Function creates a table from matches received from server.
	Each match is processed, teams added to teams array, and then points per game is calculated.*/
	createLeagueTable(){
		var teams = [];
	
		var matches = this.props.matchData;
		var onlyHomeMatches = this.props.onlyHomeMatches; //this is null when using all types, true when using home games and false when using away games.
	
		//go through each match
		for (var i = 0; i < matches.length; i++){
		
			var hometeamFound = false;
			var awayteamFound = false;
		
			//go through each team in array
			for (var j = 0; j < teams.length; j++){
				if (teams[j].name === matches[i].hometeam && onlyHomeMatches !== false){ //home team already in array
					teams = this.appendHometeamData(teams, matches[i], j);
					hometeamFound = true;
				}
			
				if (teams[j].name === matches[i].awayteam && onlyHomeMatches !== true){ //away team already in array
					teams = this.appendAwayteamData(teams, matches[i], j);
					awayteamFound = true;
				}
			}
		
			if (!hometeamFound && onlyHomeMatches !== false){
				teams.push(this.appendTeam(matches[i].hometeam, matches[i], true));
			}
		
			if (!awayteamFound && onlyHomeMatches !== true){
				teams.push(this.appendTeam(matches[i].awayteam, matches[i], false));
			}
		
		}
		
		for (i = 0; i < teams.length; i++){
			teams[i].pointsPerGame = Math.round(100 * teams[i].points / teams[i].matches) / 100;
		}
		return teams;
	}
	
	/*
	Team wasn't already included in the teams array, so it must be added there. Returns a team object
	*/
	appendTeam(teamname, match, host){
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
		
		if (scored === conceded){
			points = 1;
			OTLosses = 1;
		}
		else {
			if (scored > conceded){
				if (match.overtime){ //won on overtime
					points = 2;
					OTWins = 1;
				}
				else { //won on regular time
					if (this.usesOldScoring(match))
						points = 2;
					else
						points = 3;
					wins = 1;
				}			
			}
			else {
				if (match.overtime){ //lost on overtime
				if (!this.otLoss0points(match))
						points = 1;
					OTLosses = 1;
				}
				else { //lost on regular time
					losses = 1;
				}
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
	Adds new match data for the home team.
	*/
	appendHometeamData(teams, match, j){
		teams[j].goals = teams[j].goals + match.homescore;
		teams[j].against = teams[j].against + match.awayscore;
				
		teams[j].matches = teams[j].matches + 1;
				
		if (match.homescore === match.awayscore){
			teams[j].otLosses = teams[j].otLosses + 1;
			teams[j].points = teams[j].points + 1;
		}	
		else {				
			if (match.homescore > match.awayscore){ //home win
				if (match.overtime){
					teams[j].otWins = teams[j].otWins + 1;
					teams[j].points = teams[j].points + 2;
				}
				else { //win on regular time 
					if (this.usesOldScoring(match))
						teams[j].points = teams[j].points + 2;
					else
						teams[j].points = teams[j].points + 3;
					teams[j].won = teams[j].won + 1;
				}
			}
			else { //away win
				if (match.overtime) {
					if (!this.otLoss0points(match))
						teams[j].points = teams[j].points + 1;
					teams[j].otLosses = teams[j].otLosses + 1;
				}
				else //lost on regular time
					teams[j].lost = teams[j].lost + 1;
				}	
		}
		
		return teams;
	}

	/*
	Adds new match data for the away team.
	*/
	appendAwayteamData(teams, match, j){
		teams[j].goals = teams[j].goals + match.awayscore;
		teams[j].against = teams[j].against + match.homescore;
				
		teams[j].matches = teams[j].matches + 1;
						
		if (match.homescore === match.awayscore){
			teams[j].otLosses = teams[j].otLosses + 1;
			teams[j].points = teams[j].points + 1;
		}
		else {
			if (match.awayscore > match.homescore){ //away win
				if (match.overtime){
					teams[j].otWins = teams[j].otWins + 1;
					teams[j].points = teams[j].points + 2;
				}
				else { //win on regular time 
					if (this.usesOldScoring(match))
						teams[j].points = teams[j].points + 2;
					else
						teams[j].points = teams[j].points + 3;
					teams[j].won = teams[j].won + 1;
				}
			}
			else { //home win
				if (match.overtime) {
					if (!this.otLoss0points(match))
						teams[j].points = teams[j].points + 1;
					teams[j].otLosses = teams[j].otLosses + 1;
				}
				else //lost on regular time
					teams[j].lost = teams[j].lost + 1;
				}
		}
		return teams;
	}

	usesOldScoring(match){
		if (match.season > "03-04")
			return false;
		else
			return true;
	}
	
	otLoss0points(match){
		if (match.season > "00-01")
			return false;
		else
			return true;
	}
}

export default LeagueTable;