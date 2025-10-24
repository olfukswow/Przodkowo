const fs = require('fs');
const style = require('../utils/style');
const { send500 } = require('../utils/errorHandlers');

module.exports = function(req, res, name) {
  fs.readFile('guests.json', (err, data) => {
    res.writeHead(200, { 'Content-Type': 'text/html' });

    if (err || !data) {
      res.end(style + '<h1>Brak danych.</h1>');
      return;
    }

    let guests = JSON.parse(data);
    guests = guests.filter(g => g.name !== name);
    fs.writeFile('guests.json', JSON.stringify(guests, null, 2), (err) => {
      if (err) return send500(res, err);
      res.end(style + `<h1>Gość "${name}" usunięty.</h1>`);
    });
  });
};