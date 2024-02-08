
module.exports = function (grunt)
{
    var pkg = grunt.file.readJSON("package.json");

    grunt.config.init(
        {
            pkg: pkg,
            package: { dest: "dist", main: "tasks/package.js" },
            copy:
            {
                dist: { files: [{ expand: true, cwd: "src", src: ["**", "!package-lock.json"], dest: "dist" }] },
                install: { files: [{ expand: true, cwd: "dist", src: ["**", "!package-lock.json"], dest: '../../npm_modules/' + pkg["name"] }] }
            }
        });

    grunt.task.loadTasks("src/tasks");
    grunt.task.loadNpmTasks("grunt-contrib-copy");

    grunt.task.registerTask("default", ["copy:dist", "package", "copy:install"]);
}