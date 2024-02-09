
const path = require('path');

module.exports = function (grunt)
{
    const pkg = grunt.file.readJSON("package.json");
    const webpackConfig = require('./webpack.config.js');
    const user = process.env.USERNAME;
    const dest = `C:\\Users\\${user}\\AppData\\Local\\Screeps\\scripts\\127_0_0_1___21025\\default`;

    grunt.config.init(
        {
            pkg: pkg,
            ts: { default: { tsconfig: './tsconfig.json' } },
            webpack: { myConfig: webpackConfig },
            copy: { default: { files: [{ cwd: "dist/", src: ["main.js"], dest, expand: true }] } }
        }
    );

    grunt.task.loadNpmTasks("grunt-ts");
    grunt.task.loadNpmTasks("grunt-webpack");
    grunt.task.loadNpmTasks("grunt-contrib-copy");

    grunt.task.registerTask("default", ["ts", "webpack", "copy"]);
}
