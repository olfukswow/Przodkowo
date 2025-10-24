const style = require('../utils/style');

module.exports = function(req, res) {
  res.writeHead(200, { 'Content-Type': 'text/html' });
  res.end(style + `
    <h1>Dodaj gościa</h1>
    <form method="POST" action="/form" enctype="multipart/form-data">
      <input type="text" name="name" placeholder="Imię" required /><br/>
      <input type="file" name="photo" accept="image/*" /><br/>
      <button type="submit">Dodaj</button>
    </form>
  `);
};