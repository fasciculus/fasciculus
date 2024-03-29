
function getFiles()
{
    const result = new Array();

    result.push("es/object");
    result.push("es/types");
    result.push("es/array");
    result.push("es/set");
    result.push("es/map");
    result.push("es/es");

    result.push("screeps/types");
    result.push("screeps/cache");
    result.push("screeps/assign");
    result.push("screeps/name");

    result.push("screeps/game");
    result.push("screeps/memory");
    result.push("screeps/terrain");
    result.push("screeps/pos");
    result.push("screeps/controller");
    result.push("screeps/room");
    result.push("screeps/source");
    result.push("screeps/spawn");

    result.push("screeps/body");
    result.push("screeps/creep");

    result.push("screeps/screeps");

    result.push("main");

    return result.map(s => `src/${s}.ts`);
}

function removeExport(line)
{
    if (!line.startsWith("export")) return line;

    line = line.substring(6).trim();

    if (line == "{}" || line == "{ }") return "";

    return line;
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

const tsOptions =
{
    default:
    {
        tsconfig: "tsconfig.concat.json",

        options:
        {
            passThrough: true,
            fast: "never"
        }
    }
};

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
            ts: tsOptions,
            copy: { default: { files: [{ cwd: "dist/", src: ["main.js"], dest, expand: true }] } }
        }
    );

    grunt.task.loadNpmTasks("grunt-contrib-concat");
    grunt.task.loadNpmTasks("grunt-ts");
    grunt.task.loadNpmTasks("grunt-contrib-copy");

    grunt.task.registerTask("default", ["concat", "ts", "copy"]);
}
