const fs = require('fs');
const style = require('../utils/style');
const { send500 } = require('../utils/errorHandlers');

module.exports = function(req, res, query) {
  if (!query.name) {
    res.writeHead(200, { 'Content-Type': 'text/html' });
    res.end(style + '<h1>Brak parametru "name"</h1>');
    return;
  }

  const now = new Date().toISOString();
  const guest = { name: query.name, date: now, photo: "" };

  fs.readFile('guests.json', (err, data) => {
    if (err) return send500(res, err);
    const guests = data ? JSON.parse(data) : [];
    guests.push(guest);
    fs.writeFile('guests.json', JSON.stringify(guests, null, 2), (err) => {
      if (err) return send500(res, err);
      res.writeHead(200, { 'Content-Type': 'text/html' });
      res.end(style + '<h1>Gość dodany.</h1>');
    });
  });
};