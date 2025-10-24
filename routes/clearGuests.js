const fs = require('fs');
const style = require('../utils/style');
const { send500 } = require('../utils/errorHandlers');

module.exports = function(req, res) {
  fs.writeFile('guests.json', '[]', (err) => {
    if (err) return send500(res, err);
    res.writeHead(200, { 'Content-Type': 'text/html' });
    res.end(style + '<h1>Plik został wyczyszczony.</h1>');
  });
};