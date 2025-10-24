const fs = require('fs');

function logError(message) {
  fs.appendFile('errors.log', `${new Date().toISOString()} - ${message}\n`, () => {});
}

module.exports = { logError };