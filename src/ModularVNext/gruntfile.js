/// <binding AfterBuild='copy' />
// This file in the main entry point for defining grunt tasks and using grunt plugins.
// Click here to learn more. http://go.microsoft.com/fwlink/?LinkID=513275&clcid=0x409

module.exports = function (grunt) {
    grunt.initConfig({
        bower: {
            install: {
                options: {
                    targetDir: "wwwroot/lib",
                    layout: "byComponent",
                    cleanTargetDir: false
                }
            }
        },
        copy: {
            module1: {
                expand: true,
                cwd: "../../artifacts/bin/Module1/Debug/aspnet50/",
                src: ["Module1.dll"],
                dest: "./artifacts/bin/modules/"
            },
            module2: {
                expand: true,
                cwd: "../../artifacts/bin/Module2/Debug/aspnet50/",
                src: ["Module2.dll"],
                dest: "./artifacts/bin/modules/"
            }
        }
    });

    // This command registers the default task which will install bower packages into wwwroot/lib
    grunt.registerTask("default", ["bower:install"]);

    // The following line loads the grunt plugins.
    // This line needs to be at the end of this this file.
    grunt.loadNpmTasks("grunt-bower-task");
    grunt.loadNpmTasks("grunt-contrib-copy");
};