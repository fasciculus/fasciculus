
const path = require('path');

module.exports =
{
    entry: "./build/main.js",
    output: { filename: 'main.js', path: path.resolve(__dirname, 'dist') },
    mode: "none",
    infrastructureLogging: { colors: false }
}
