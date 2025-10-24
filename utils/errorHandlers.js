const { logError } = require('./logger');
const style = require('./style');

function send404(res) {
  res.writeHead(404, 'Not Found', { 'Content-Type': 'text/html' });
  res.end(style + '<h1>Błąd 404: Strona nie istnieje</h1>');
}

function send500(res, err) {
  res.writeHead(500, 'Internal Server Error', { 'Content-Type': 'text/html' });
  logError(`500 error: ${err}`);
  res.end(style + '<h1>Błąd 500: Wewnętrzny błąd serwera</h1>');
}

module.exports = { send404, send500 };