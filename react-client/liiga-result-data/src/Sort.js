/* General sorting for sorting without tiebreakers. Highest first*/
export function sortByHighest(data, param)
{
    return data.sort(function (a, b) {
		return b[param] - a[param];
    });
}

export function sortAlphabetically(data, param){
	return data.sort(function(a, b){
    if(a[param] < b[param]) return -1;
    if(a[param] > b[param]) return 1;
    return 0;
	});
}

export function sortAlphabeticallyBackwards(data, param){
	return data.sort(function(a, b){
    if(b[param] < a[param]) return -1;
    if(b[param] > a[param]) return 1;
    return 0;
	});
}

export function sortBySmallest(data, param){
	return data.sort(function (a, b) {
		return a[param] - b[param];
    });
}

export function alphabeticalSort(data){
	return data.sort(function(a, b){
    if(a < b) return -1;
    if(a > b) return 1;
    return 0;
})
}

export function sortByNewestDate(data, param){
	
	/*Return year if there is a difference, return month if there's a difference, 
	and finally return day*/
	return data.sort(function (a, b) {
		var aSplitted =	a[param].split('-');
		var bSplitted = b[param].split('-');
		var year = bSplitted[0] - aSplitted[0];
		
		if (year !== 0)
			return year;
		
		var month = bSplitted[1] - aSplitted[1];
		
		if (month !== 0)
			return month;
		
		return bSplitted[2] - aSplitted[2];
		
    }); 
	
}

export function sortByOldestDate(data, param){
	
	/*Return year if there is a difference, return month if there's a difference, 
	and finally return day*/
	return data.sort(function (a, b) {
		var aSplitted =	a[param].split('-');
		var bSplitted = b[param].split('-');
		var year = aSplitted[0] - bSplitted[0];
		
		if (year !== 0)
			return year;
		
		var month = aSplitted[1] - bSplitted[1];
		
		if (month !== 0)
			return month;
		
		return aSplitted[2] - bSplitted[2];
		
    }); 
	
}