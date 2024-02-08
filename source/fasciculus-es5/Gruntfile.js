
module.exports = function (grunt)
{
    var pkg = grunt.file.readJSON("package.json");

    grunt.config.init(
        {
            pkg: pkg,
            package: { dest: "dist", main: "fasciculus-es5.js" },
            copy:
            {
                install: { files: [{ expand: true, cwd: "dist", src: ["*.js", "package.json"], dest: '../../npm_modules/' + pkg["name"] }] }
            },
            concat:
            {
                options: { footer: grunt.util.linefeed + grunt.util.linefeed + "export {}" },
                build: { src: "src/*.ts", dest: "build/fasciculus-es5.ts" }
            },
            ts:
            {
                default: { tsconfig: './tsconfig.json' }
            }
        }
    );

    grunt.task.loadNpmTasks("fasciculus-package");
    grunt.task.loadNpmTasks("grunt-contrib-concat");
    grunt.task.loadNpmTasks("grunt-contrib-copy");
    grunt.task.loadNpmTasks("grunt-ts");

    grunt.task.registerTask("default", ["concat:build", "ts", "package", "copy:install"]);
}