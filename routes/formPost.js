const fs = require('fs');
const style = require('../utils/style');
const { send500 } = require('../utils/errorHandlers');

module.exports = function(req, res) {
  let body = '';
  req.on('data', chunk => body += chunk);
  req.on('end', () => {
    try {
      const boundary = req.headers['content-type'].split('boundary=')[1];
      const parts = body.split('--' + boundary);
      let name = '';
      let photo = '';

      parts.forEach(part => {
        if (part.includes('name="name"')) {
          name = part.split('\r\n\r\n')[1]?.trim();
        }
        if (part.includes('name="photo"')) {
          const base64 = part.split('\r\n\r\n')[1];
          photo = Buffer.from(base64, 'binary').toString('base64');
        }
      });

      if (!name) {
        res.writeHead(200, { 'Content-Type': 'text/html' });
        res.end(style + '<h1>Brak imienia</h1>');
        return;
      }

      const now = new Date().toISOString();
      const guest = { name, date: now, photo };

      fs.readFile('guests.json', (err, data) => {
        if (err) return send500(res, err);
        const guests = data ? JSON.parse(data) : [];
        guests.push(guest);
        fs.writeFile('guests.json', JSON.stringify(guests, null, 2), (err) => {
          if (err) return send500(res, err);
          res.writeHead(200, { 'Content-Type': 'text/html' });
          res.end(style + '<h1>Gość dodany przez formularz.</h1>');
        });
      });
    } catch (err) {
      send500(res, err);
    }
  });
};