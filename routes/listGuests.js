const fs = require('fs');
const style = require('../utils/style');

module.exports = function(req, res) {
  fs.readFile('guests.json', (err, data) => {
    res.writeHead(200, { 'Content-Type': 'text/html' });

    if (err || !data) {
      res.end(style + '<h1>Brak danych.</h1>');
      return;
    }

    const guests = JSON.parse(data);
    res.write(style + '<h1>Lista gości:</h1><ul>');
    guests.forEach(g => {
      res.write(`<li>${g.name} (${g.date})`);
      if (g.photo) {
        res.write(`<br/><img src="data:image/jpeg;base64,${g.photo}" />`);
      }
      res.write('</li>');
    });
    res.write('</ul>');
    res.end();
  });
};