
module.exports = function (grunt)
{
    const pkg = grunt.file.readJSON("package.json");
    const files = sortFiles(grunt);
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

function removeExport(line)
{
    return line.startsWith("export") ? line.substring(6).trim() : line;
}

function sortFiles(grunt)
{
    const files = grunt.file.expand("src/**/*.ts");
    const dependencies = getDependencies(grunt, files);

    return processDependencies(dependencies);
}

function processDependencies(dependencies)
{
    const result = new Array();
    var iteration = 0;

    while (dependencies.size > 0)
    {
        dependencies.forEach((value, key) =>  { if (value.size == 0) result.push(key); });
        result.forEach(key => dependencies.delete(key));
        dependencies.forEach((value) => result.forEach(key => value.delete(key)));
        ++iteration;

        if (iteration > 99) break;
    }

    return result.map(dependencyKeyToFile);
}

function getDependencies(grunt, files)
{
    const dependencies = new Map();

    for (var file of files)
    {
        const key = fileToDependencyKey(file);

        dependencies.set(key, getImports(grunt, file));
    }

    return dependencies;
}

function fileToDependencyKey(file)
{
    return file.substring(4, file.length - 3);
}

function dependencyKeyToFile(key)
{
    return "src/" + key + ".ts";
}

function getImports(grunt, file)
{
    const src = grunt.file.read(file, { encoding: "utf8" });
    const lines = src.split(/\r?\n/);
    const rawImports = lines.filter(l => l.startsWith("import"));
    const imports = rawImports.map(getImport);
    const result = new Set(imports);

    result.delete("");

    return result;
}

function getImport(src)
{
    const from = src.indexOf("from");

    if (from < 0) return "";

    const rawImport = src.substring(from + 4).trim();
    const unquotedImport = rawImport.substring(1, rawImport.length - 2);
    const result = unquotedImport.startsWith("./") ? unquotedImport.substring(2) : unquotedImport;

    return result;
}