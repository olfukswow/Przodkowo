
const http = require('http');
const fs = require('fs');
const url = require('url');

let visitCount = 0;

const server = http.createServer((req, res) => {
  const parsedUrl = url.parse(req.url, true);
  const path = parsedUrl.pathname;
  const name = parsedUrl.query.name;

  res.setHeader('Content-Type', 'text/html');

  if (path === '/') {
    visitCount++;
    res.write(`<h1>Strona została odświeżona ${visitCount} razy</h1>`);
    res.end();
  } else if (path === '/add') {
    if (!name) {
      res.write('<h1>Brak parametru "name"</h1>');
      return res.end();
    }

    fs.appendFile('guests.txt', name + '\n', (err) => {
      if (err) {
        res.write('<h1>Błąd zapisu do pliku</h1>');
      } else {
        res.write('<h1>Imię zostało dodane.</h1>');
      }
      res.end();
    });
  } else if (path === '/list') {
    fs.readFile('guests.txt', 'utf8', (err, data) => {
      if (err) {
        res.write('<h1>Plik guests.txt nie istnieje.</h1>');
      } else if (data.trim() === '') {
        res.write('<h1>Plik jest pusty.</h1>');
      } else {
        const guests = data.trim().split('\n');
        res.write('<h1>Lista gości:</h1><ul>');
        guests.forEach(guest => {
          res.write(`<li>${guest}</li>`);
        });
        res.write('</ul>');
      }
      res.end();
    });
  } else if (path === '/clear') {
    fs.writeFile('guests.txt', '', (err) => {
      if (err) {
        res.write('<h1>Nie udało się wyczyścić pliku.</h1>');
      } else {
        res.write('<h1>Plik został wyczyszczony.</h1>');
      }
      res.end();
    });
  } else {
    res.write('<h1>Nieznane żądanie</h1>');
    res.end();
  }
});

server.listen(3000, () => {
  console.log('Serwer działa na porcie 3000');
});