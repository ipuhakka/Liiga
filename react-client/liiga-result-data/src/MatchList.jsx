import React, { Component } from 'react';
import './css/MatchList.css';
import Table from 'react-bootstrap/lib/Table';
import * as Sort from './Sort.js';

class MatchList extends Component{
	constructor(props){
		super(props);
		this.sortBy = this.sortBy.bind(this);
					
		this.state={
			lastSortedBy: null,
			byHighest: true
		}
	}
	
	render(){
		var rows = [];
		
		if (this.props.data !== null){
			var data = this.props.data;
			for (var i = 0; i < data.length; i++){
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
						</tr>)
			}
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
			</div>
		);
	}
	
	sortBy(param){
		if (param === 'hometeam' || param === 'awayteam')
			this.props.onUpdate(Sort.sortAlphabetically(this.props.data, param));
		
		else {
			if (param === this.state.lastSortedParam && !this.state.byHighest){
				if (param === "date")
					this.props.onUpdate(Sort.sortByOldestDate(this.props.data, param));	
				else 
					this.props.onUpdate(Sort.sortBySmallest(this.props.data, param));	
			}	
			else {
				if (param === "date")
					this.props.onUpdate(Sort.sortByNewestDate(this.props.data, param));
				else
					this.props.onUpdate(Sort.sortByHighest(this.props.data, param));
			}

			this.setState({byHighest: !this.state.byHighest});
		}
		this.setState({
			lastSortedParam: param
		});
	}
}

export default MatchList;