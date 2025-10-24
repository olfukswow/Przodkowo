const style = require('../utils/style');

module.exports = function(req, res, ip, visitCount, ipVisits) {
  res.writeHead(200, { 'Content-Type': 'text/html' });
  res.write(style + `<h1>Odwiedziny: ${visitCount}</h1>`);
  res.write(`<p>Twoje IP: ${ip}, odwiedzin: ${ipVisits[ip]}</p>`);
  res.end();
};