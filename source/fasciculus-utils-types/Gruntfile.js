
module.exports = function (grunt)
{
    var pkg = grunt.file.readJSON("package.json");
    var dest = "../../npm_modules/" + pkg["name"] + "/";

    grunt.config.init(
        {
            pkg: pkg,
            package:
            {
                default: { dest: "dist/" },
            },
            copy:
            {
                package: { files: [{ cwd: "dist", src: ["package.json"], dest, expand: true }] },
                types: { files: [{ cwd: "src", src: ["*.ts"], dest, expand: true }] }
            }
        });

    grunt.task.loadNpmTasks("fasciculus-package");
    grunt.task.loadNpmTasks("grunt-contrib-copy");

    grunt.task.registerTask("default", ["package", "copy"]);
}