var express = require('express');
var app = express();
var http = require('http').Server(app);


app.use(express.static('public'));

app.get('/', function (request, response) {
    response.sendFile(__dirname + '/public/index.html');
});

http.listen(3001, function () {
    console.log('listening on port 3001..');
});

