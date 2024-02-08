
module.exports = function (grunt)
{
    grunt.task.registerTask("package", function () { package(grunt); });
}

function package(grunt)
{
    var cfg = grunt.config.get("package");
    var dest = (cfg.dest || "dist") + "/package.json";
    var main = cfg.main;

    var pkg = grunt.file.readJSON("package.json");

    delete pkg["devDependencies"];
    delete pkg["scripts"];

    if (main) pkg.main = main;

    var json = JSON.stringify(pkg, null, 2);

    grunt.file.write(dest, json)
}