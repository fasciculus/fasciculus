
module.exports = function (grunt)
{
    var pkg = grunt.file.readJSON("package.json");
    var dest = "dist";

    var mods = "../../npm_modules/";
    var fscEs = { cwd: mods + "fasciculus-es", src: ["*.js"], dest, expand: true };
    var fscUtils = { cwd: mods + "fasciculus-utils", src: ["*.js"], dest, expand: true };

    grunt.config.init(
        {
            pkg: pkg,
            ts: { default: { tsconfig: './tsconfig.json' } },
            copy: { default: { files: [fscEs, fscUtils] } },
        }
    );

    grunt.task.loadNpmTasks("grunt-ts");
    grunt.task.loadNpmTasks("grunt-contrib-copy");

    grunt.task.registerTask("default", ["ts", "copy"]);
}