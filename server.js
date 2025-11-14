const express = require("express");
const mysql = require("mysql2");

const app = express();
const port = 3000;

const db = mysql.createConnection({
  host: "localhost",
  user: "root",
  password: "",
  database: "szkola",
});

db.connect((err) => {
  if (err) {
    console.error("Błąd połączenia z bazą:", err);
  } else {
    console.log("Połączono z bazą");
  }
});

app.get("/students", (req, res) => {
  db.query("SELECT * FROM students", (err, results) => {
    if (err) {
      console.error("Błąd zapytania:", err);
      res.status(500).json({ error: "Błąd serwera" });
    } else {
      res.json(results);
    }
  });
});

app.listen(port, () => {
  console.log(`Serwer działa na http://localhost:${port}/students`);
});