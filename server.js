const http = require('http');
const fs = require('fs');
const url = require('url');
const { StringDecoder } = require('string_decoder');

let visitCount = 0;
const ipVisits = {};

const style = `
  <style>
    body { font-family: Arial; background-color: #f9f9f9; padding: 20px; }
    h1 { color: #333; }
    ul { list-style-type: square; }
    input, button { padding: 8px; margin: 5px; }
    form { margin-top: 20px; }
    img { max-width: 100px; display: block; margin-top: 10px; }
  </style>
`;

const server = http.createServer((req, res) => {
  const parsedUrl = url.parse(req.url, true);
  const path = parsedUrl.pathname;
  const query = parsedUrl.query;
  const ip = req.socket.remoteAddress;

  res.setHeader('Content-Type', 'text/html');

  // Licznik odwiedzin
  visitCount++;
  ipVisits[ip] = (ipVisits[ip] || 0) + 1;

  if (path === '/') {
    res.write(style + `<h1>Odwiedziny: ${visitCount}</h1>`);
    res.write(`<p>Twoje IP: ${ip}, odwiedzin: ${ipVisits[ip]}</p>`);
    res.end();

  } else if (path === '/add' && req.method === 'GET') {
    if (!query.name) {
      res.write(style + '<h1>Brak parametru "name"</h1>');
      return res.end();
    }

    const now = new Date().toISOString();
    const guest = { name: query.name, date: now, photo: "" };

    fs.readFile('guests.json', (err, data) => {
      const guests = err ? [] : JSON.parse(data);
      guests.push(guest);
      fs.writeFile('guests.json', JSON.stringify(guests, null, 2), () => {
        res.write(style + '<h1>Gość dodany.</h1>');
        res.end();
      });
    });

  } else if (path === '/form' && req.method === 'GET') {
    res.write(style + `
      <h1>Dodaj gościa</h1>
      <form method="POST" action="/form" enctype="multipart/form-data">
        <input type="text" name="name" placeholder="Imię" required /><br/>
        <input type="file" name="photo" accept="image/*" /><br/>
        <button type="submit">Dodaj</button>
      </form>
    `);
    res.end();

  } else if (path === '/form' && req.method === 'POST') {
    let body = '';
    req.on('data', chunk => body += chunk);
    req.on('end', () => {
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
        res.write(style + '<h1>Brak imienia</h1>');
        return res.end();
      }

      const now = new Date().toISOString();
      const guest = { name, date: now, photo };

      fs.readFile('guests.json', (err, data) => {
        const guests = err ? [] : JSON.parse(data);
        guests.push(guest);
        fs.writeFile('guests.json', JSON.stringify(guests, null, 2), () => {
          res.write(style + '<h1>Gość dodany przez formularz.</h1>');
          res.end();
        });
      });
    });

  } else if (path === '/list') {
    fs.readFile('guests.json', (err, data) => {
      if (err || !data) {
        res.write(style + '<h1>Brak danych.</h1>');
        return res.end();
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
      //xd
      res.end();
    });

  } else if (path === '/delete' && query.name) {
    fs.readFile('guests.json', (err, data) => {
      if (err || !data) {
        res.write(style + '<h1>Brak danych.</h1>');
        return res.end();
      }

      let guests = JSON.parse(data);
      guests = guests.filter(g => g.name !== query.name);
      fs.writeFile('guests.json', JSON.stringify(guests, null, 2), () => {
        res.write(style + `<h1>Gość "${query.name}" usunięty.</h1>`);
        res.end();
      });
    });

  } else if (path === '/clear') {
    fs.writeFile('guests.json', '[]', () => {
      res.write(style + '<h1>Plik został wyczyszczony.</h1>');
      res.end();
    });

  } else {
    res.write(style + '<h1>Nieznany endpoint</h1>');
    res.end();
  }
});

server.listen(3000, () => {
  console.log('Serwer działa na porcie 3000');
});