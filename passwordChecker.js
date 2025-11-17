const fs = require('fs');
const path = require('path');

function loadPopularPasswords(filePath) {
  try {
    const data = fs.readFileSync(filePath, 'utf8');
    return new Set(data.split('\n').map(p => p.trim()));
  } catch (err) {
    console.error('Błąd wczytywania listy haseł:', err);
    return new Set();
  }
}

function analyzePassword(password, popularPasswords) {
  let score = 0;
  const suggestions = [];

  if (password.length >= 8) score++;
  else suggestions.push('Zwiększ długość hasła (minimum 8 znaków)');

  if (/[a-z]/.test(password)) score++;
  else suggestions.push('Dodaj małe litery');

  if (/[A-Z]/.test(password)) score++;
  else suggestions.push('Dodaj wielkie litery');

  if (/\d/.test(password)) score++;
  else suggestions.push('Dodaj cyfry');

  if (/[^A-Za-z0-9]/.test(password)) score++;
  else suggestions.push('Dodaj znaki specjalne');

  if (popularPasswords.has(password)) {
    suggestions.push('Hasło znajduje się na liście popularnych — zmień je');
  } else {
    score++;
  }

  // Bonus za długość >12
  if (password.length >= 12) score++;

  let strength = '';
  if (score <= 3) strength = 'Słabe';
  else if (score <= 6) strength = 'Średnie';
  else strength = 'Bardzo silne';

  return { score, strength, suggestions };
}

module.exports = { loadPopularPasswords, analyzePassword };