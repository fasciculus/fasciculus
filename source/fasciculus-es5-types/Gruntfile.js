
module.exports = function (grunt)
{
    var pkg = grunt.file.readJSON("package.json");

    grunt.config.init(
        {
            pkg: pkg,
            package: { dest: "dist", main: "fasciculus-es5.d.ts" },
            copy:
            {
                dist: { files: [{ expand: true, cwd: "src", src: ["**", "!package-lock.json"], dest: "dist" }] },
                install: { files: [{ expand: true, cwd: "dist", src: ["**", "!package-lock.json"], dest: '../../npm_modules/' + pkg["name"] }] }
            }
        }
    );

    grunt.task.loadNpmTasks("fasciculus-package");
    grunt.task.loadNpmTasks("grunt-contrib-copy");

    grunt.task.registerTask("default", ["copy:dist", "package", "copy:install"]);
}