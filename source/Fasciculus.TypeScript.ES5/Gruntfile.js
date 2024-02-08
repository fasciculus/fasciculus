
module.exports = function (grunt)
{
    grunt.initConfig(
        {
            pkg: grunt.file.readJSON("package.json"),
            concat:
            {
                dist:
                {
                    src: "src/fasciculus-es5-extensions/*.ts",
                    dest: "build/fasciculus-es5-extensions/fasciculus-es5-extensions.ts"
                }
            }
        });

    grunt.registerTask("prepare", function ()
    {
        grunt.file.copy("src/@types/fasciculus-es5-extensions/fasciculus-es5-extensions.d.ts",
            "build/@types/fasciculus-es5-extensions/fasciculus-es5-extensions.d.ts");
    });

    grunt.registerTask("package", function ()
    {
        createPackageJson(grunt, "@types/fasciculus-es5-extensions", "");
        createPackageJson(grunt, "fasciculus-es5-extensions", "");
    });

    grunt.loadNpmTasks('grunt-contrib-concat');

    grunt.registerTask("default", ["prepare", "concat" ,"package"]);
}

function createPackageJson(grunt, name, main)
{
    var pkg = grunt.file.readJSON("package.json");

    pkg["name"] = name;
    pkg["main"] = main;

    delete pkg["devDependencies"];
    delete pkg["scripts"];

    var dest = "dist/" + name + "/package.json"
    var json = JSON.stringify(pkg, null, 2);

    grunt.file.write(dest, json);
}
