const readline = require('readline');
const { loadPopularPasswords, analyzePassword } = require('./passwordChecker');

const rl = readline.createInterface({
  input: process.stdin,
  output: process.stdout
});

const popularPasswords = loadPopularPasswords('./popular-passwords.txt');

rl.question('Podaj hasło do analizy: ', (password) => {
  const result = analyzePassword(password, popularPasswords);

  console.log('\nWyniki analizy:');
  console.log(`Siła hasła: ${result.strength}`);
  console.log(`Punkty: ${result.score}/8`);

  if (result.suggestions.length > 0) {
    console.log('\nSugestie poprawy:');
    result.suggestions.forEach(s => console.log(`- ${s}`));
  } else {
    console.log('\nHasło jest bardzo silne!');
  }

  rl.close();
});