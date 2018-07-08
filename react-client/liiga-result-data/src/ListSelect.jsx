import React, { Component } from 'react';
import './css/ListSelect.css';
import ListGroup from 'react-bootstrap/lib/ListGroup';
import ListGroupItem from 'react-bootstrap/lib/ListGroupItem';

class ListSelect extends Component{	
	render(){
		var items = [];
		if (this.props.items != null){
			for (var i = 0; i < this.props.items.length; i++){
				items.push(<ListGroupItem bsStyle={this.props.itemState[i] ?  'success': null} onClick={this.props.onUpdate.bind(this, i)} key={i}>{this.props.items[i]}</ListGroupItem>);
			}
		}		
		return(
			<div className="list-div">
				<ListGroup>{items}</ListGroup>
			</div>
		);
	}
}

export default ListSelect;