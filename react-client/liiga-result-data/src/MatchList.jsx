import React, { Component } from 'react';
import './css/MatchList.css';
import Table from 'react-bootstrap/lib/Table';
import * as Sort from './Sort.js';
import Button from 'react-bootstrap/lib/Button';

class MatchList extends Component{
	constructor(props){
		super(props);
		this.sortBy = this.sortBy.bind(this);
		this.loadMoreMatches = this.loadMoreMatches.bind(this);
					
		this.state={
			lastSortedBy: null,
			byHighest: true, 
			currentIndex: 0
		}
	}
	
	render(){	
		var rows = [];
		var moreButton = null;
		if (this.props.data !== null){
			var data = this.props.data;
			
			for (var i = 0; i < this.state.currentIndex + 100; i++){
				var ot = "";
				if (data[i].overtime)
					ot="ot.";
				rows.push(<tr key={i}>
							<td>{data[i].hometeam}</td>
							<td>{data[i].awayteam}</td>
							<td>{data[i].homescore}</td>
							<td>{data[i].awayscore}</td>
							<td>{data[i].date}</td>
							<td>{ot}</td>
						</tr>);
			}
			moreButton = <Button className="moreButton" onClick={this.loadMoreMatches}>More matches</Button>
						
		}

		return(
			<div className="table-div">
				<Table responsive striped condensed>
					<thead>
						<tr>
							<th onClick={() => this.sortBy("hometeam")}>Hometeam</th>
							<th onClick={() => this.sortBy("awayteam")}>Awayteam</th>
							<th onClick={() => this.sortBy("homescore")}>Homescore</th>
							<th onClick={() => this.sortBy("awayscore")}>Awayscore</th>
							<th onClick={() => this.sortBy("date")}>Date</th>
							<th onClick={() => this.sortBy("overtime")}>Overtime</th>
						</tr>
					</thead>
					<tbody>{rows}</tbody>
			</Table>
			{moreButton}
			</div>
		);
	}
	
	/*calls sorting functions. If a parameter is being used in the sort twice in a row and byHighest is false,
	it calls for sorting from the lowest, otherwise highest sort is called.*/
	sortBy(param){		

		if (param === this.state.lastSortedParam && !this.state.byHighest){
			if (param === "date")
				this.props.onUpdate(Sort.sortByOldestDate(this.props.data, param));	
			else if (param === 'hometeam' || param === 'awayteam')
				this.props.onUpdate(Sort.sortAlphabeticallyBackwards(this.props.data, param));
			else 
				this.props.onUpdate(Sort.sortBySmallest(this.props.data, param));	
				
			this.setState({byHighest: true});
		}	
		else {
			if (param === "date")
				this.props.onUpdate(Sort.sortByNewestDate(this.props.data, param));
			else if (param === 'hometeam' || param === 'awayteam')
				this.props.onUpdate(Sort.sortAlphabetically(this.props.data, param));
			else
				this.props.onUpdate(Sort.sortByHighest(this.props.data, param));
				
			this.setState({byHighest: false});
		}

		this.setState({
			lastSortedParam: param,
			currentIndex: 0
		});
	}
	
	/*Adds 100 to currentIndex. 0 to currentIndex matches are rendered into the view.*/
	loadMoreMatches(){
		this.setState({
			currentIndex: this.state.currentIndex + 100
		});
	}
}

export default MatchList;