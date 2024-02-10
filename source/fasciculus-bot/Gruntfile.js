
function getFiles()
{
    const result = new Array();

    result.push("es/object");
    result.push("es/array");
    result.push("es/es");

    result.push("screeps/cache");
    result.push("screeps/memory");
    result.push("screeps/game");
    result.push("screeps/screeps");

    result.push("main");

    return result.map(s => `src/${s}.ts`);
}

function removeExport(line)
{
    return line.startsWith("export") ? line.substring(6).trim() : line;
}

function processSource(src, filepath)
{
    var lines = src.split(/\r?\n/);

    lines = lines.filter(line => !line.startsWith("import"));

    if (filepath != "src/main.ts")
    {
        lines = lines.map(removeExport);
    }

    return lines.join("\r\n");
}

module.exports = function (grunt)
{
    const pkg = grunt.file.readJSON("package.json");
    const files = getFiles();
    const user = process.env.USERNAME;
    const dest = `C:\\Users\\${user}\\AppData\\Local\\Screeps\\scripts\\127_0_0_1___21025\\default`;

    grunt.config.init(
        {
            pkg: pkg,
            concat: { default: { src: files, dest: "build/main.ts" }, options: { process: processSource } },
            ts: { default: { tsconfig: './tsconfig.json' } },
            copy: { default: { files: [{ cwd: "dist/", src: ["main.js"], dest, expand: true }] } }
        }
    );

    grunt.task.loadNpmTasks("grunt-contrib-concat");
    grunt.task.loadNpmTasks("grunt-ts");
    grunt.task.loadNpmTasks("grunt-contrib-copy");

    grunt.task.registerTask("default", ["concat", "ts", "copy"]);
}
