const http = require('http');
const url = require('url');

const { send404, send500 } = require('./utils/errorHandlers');
const { logError } = require('./utils/logger');

const home = require('./routes/home');
const addGuest = require('./routes/addGuest');
const formGet = require('./routes/formGet');
const formPost = require('./routes/formPost');
const listGuests = require('./routes/listGuests');
const deleteGuest = require('./routes/deleteGuest');
const clearGuests = require('./routes/clearGuests');

let visitCount = 0;
const ipVisits = {};

process.on('uncaughtException', err => {
  logError(`uncaughtException: ${err}`);
  console.error('Nieobsłużony wyjątek:', err);
});

process.on('unhandledRejection', reason => {
  logError(`unhandledRejection: ${reason}`);
  console.error('Nieobsłużona obietnica:', reason);
});

const server = http.createServer((req, res) => {
  try {
    const parsedUrl = url.parse(req.url, true);
    const path = parsedUrl.pathname;
    const query = parsedUrl.query;
    const ip = req.socket.remoteAddress;

    visitCount++;
    ipVisits[ip] = (ipVisits[ip] || 0) + 1;

    // Routing
    if (path === '/' && req.method === 'GET') {
      return home(req, res, ip, visitCount, ipVisits);
    }

    if (path === '/add' && req.method === 'GET') {
      return addGuest(req, res, query);
    }

    if (path === '/form' && req.method === 'GET') {
      return formGet(req, res);
    }

    if (path === '/form' && req.method === 'POST') {
      return formPost(req, res);
    }

    if (path === '/list') {
      return listGuests(req, res);
    }

    if (path === '/delete' && query.name) {
      return deleteGuest(req, res, query.name);
    }

    if (path === '/clear') {
      return clearGuests(req, res);
    }

    send404(res);
  } catch (err) {
    send500(res, err);
  }
});

server.listen(3000, () => {
  console.log('Serwer działa na porcie 3000');
});