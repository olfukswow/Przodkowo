const http = require('http');
const fs = require('fs');
const url = require('url');

let visitCount = 0;

const style = `
  <style>
    body { font-family: Arial; background-color: #f0f0f0; padding: 20px; }
    h1 { color: #333; }
    ul { list-style-type: square; }
    input, button { padding: 8px; margin: 5px; }
    form { margin-top: 20px; }
  </style>
`;

const server = http.createServer((req, res) => {
  const parsedUrl = url.parse(req.url, true);
  const path = parsedUrl.pathname;
  const name = parsedUrl.query.name;

  res.setHeader('Content-Type', 'text/html');

  if (path === '/') {
    visitCount++;
    res.write(style + `<h1>Strona została odświeżona ${visitCount} razy</h1>`);
    res.end();

  } else if (path === '/add') {
    if (!name) {
      res.write(style + '<h1>Brak parametru "name"</h1>');
      return res.end();
    }

    const now = new Date().toISOString();
    const entry = `${name}, ${now}\n`;

    fs.appendFile('guests.txt', entry, (err) => {
      if (err) {
        res.write(style + '<h1>Błąd zapisu do pliku</h1>');
      } else {
        res.write(style + '<h1>Gość został dodany.</h1>');
      }
      res.end();
    });

  } else if (path === '/list') {
    fs.readFile('guests.txt', 'utf8', (err, data) => {
      if (err || !data.trim()) {
        res.write(style + '<h1>Brak danych.</h1>');
      } else {
        const guests = data.trim().split('\n');
        res.write(style + '<h1>Lista gości:</h1><ul>');
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
        res.write(style + '<h1>Nie udało się wyczyścić pliku.</h1>');
      } else {
        res.write(style + '<h1>Plik został wyczyszczony.</h1>');
      }
      res.end();
    });

  } else if (path === '/delete') {
    if (!name) {
      res.write(style + '<h1>Brak parametru "name"</h1>');
      return res.end();
    }

    fs.readFile('guests.txt', 'utf8', (err, data) => {
      if (err || !data) {
        res.write(style + '<h1>Nie udało się odczytać pliku.</h1>');
        return res.end();
      }

      const updated = data
        .split('\n')
        .filter(line => !line.startsWith(name + ','))
        .join('\n');

      fs.writeFile('guests.txt', updated, (err) => {
        if (err) {
          res.write(style + '<h1>Nie udało się usunąć gościa.</h1>');
        } else {
          res.write(style + `<h1>Gość "${name}" został usunięty.</h1>`);
        }
        res.end();
      });
    });

  } else if (path === '/form') {
    res.write(style + `
      <h1>Dodaj gościa</h1>
      <form method="GET" action="/add">
        <input type="text" name="name" placeholder="Imię" required />
        <button type="submit">Dodaj</button>
      </form>
    `);
    res.end();

  } else {
    res.write(style + '<h1>Nieznane żądanie</h1>');
    res.end();
  }
});

server.listen(3000, () => {
  console.log('Serwer działa na porcie 3000');
});