
module.exports = function (grunt)
{
    var path = require("path");

    grunt.task.registerMultiTask("package", "Creating package.json", function ()
    {
        var dest = this.data.dest;
        var destValid = dest !== undefined;

        if (destValid && grunt.file.exists(dest))
        {
            destValid = grunt.file.isDir(dest);
        }

        if (!destValid)
        {
            grunt.fail.fatal("dest is missing or not a directory")
            return;
        }

        dest = path.join(dest, "package.json");

        var pkg = grunt.file.readJSON("package.json");
        var name = this.data.name;
        var main = this.data.main;

        delete pkg["devDependencies"];
        delete pkg["scripts"];

        if (name) pkg.name = name;
        if (main) pkg.main = main;

        var json = JSON.stringify(pkg, null, 2);

        grunt.file.write(dest, json);
    });
}
