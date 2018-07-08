import React, { Component } from 'react';
import './css/SelectOption.css';
import Button from 'react-bootstrap/lib/Button';
import ButtonGroup from 'react-bootstrap/lib/ButtonGroup';

class SelectOption extends Component{
	constructor(props){
		super(props);
		this.state = {
			selected: [true, false, false]
		}
	}
	
	render(){
		var buttons = [];
		var buttonStyle = "small";
		if (this.props.texts.length === 2)
			buttonStyle = "medium"; 
		
		buttonStyle = buttonStyle + " responsive-text";
		for (var i = 0; i < this.props.texts.length; i++){
			buttons.push(<Button className={buttonStyle} bsStyle={this.state.selected[i] ? "primary" : "default"} onClick={ this.clicked.bind(this,i)} key={i}>{this.props.texts[i]}</Button>);
		}
		
		return (
			<div>
				<ButtonGroup className="multi-selector">{buttons}</ButtonGroup>
			</div>
		);
	}
	
	/*param is the key value for a button which is assigned to it in render(). */
	clicked(param){
		var selected = [false, false, false];
		selected.splice(param, 0, true);
		this.setState({
			selected: selected	
		});
		
		//notify parent
		if (param === 0) 
			this.props.onUpdate(null);
		else if (param === 1)
			this.props.onUpdate(true);
		else if (param === 2)
			this.props.onUpdate(false);
	}
}

export default SelectOption;