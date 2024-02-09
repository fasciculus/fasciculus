
module.exports = function (grunt)
{
    var pkg = grunt.file.readJSON("package.json");
    var dest = "../../npm_modules/" + pkg["name"];

    grunt.config.init(
        {
            pkg: pkg,
            concat: { default: { src: "src/*.ts", dest: "build/fasciculus-utils.ts" } },
            ts: { default: { tsconfig: './tsconfig.json' } },
            package: { default: { dest: "dist/" } },
            copy: { default: { files: [{ cwd: "dist", src: ["package.json", "*.js"], dest, expand: true } ] } },
        }
    );

    grunt.task.loadNpmTasks("grunt-contrib-concat");
    grunt.task.loadNpmTasks("grunt-ts");
    grunt.task.loadNpmTasks("fasciculus-package");
    grunt.task.loadNpmTasks("grunt-contrib-copy");

    grunt.task.registerTask("default", ["concat", "ts", "package", "copy"]);
}