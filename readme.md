# Readme #
This project demonstrates a couple of ways to encapsulate packages of functionality in an ASP.NET MVC 6 project. I.e. to allow modules of ASP.NET MVC 6 functionality to be written in separate projects from the main application.


The goals for this sample are to reduce friction at development and deployment time. At development time, it is desirable to work with files on disk for rapid development cycles. It is also key to avoid copying files from modules into the main application folder. At deployment time, it is beneficial to minimise the amount 


This sample covers the packaging modules and module discovery &amp; loading.

This sample was written against MVC 6.0.0-beta3

## Solution structure ##
The solution contains two projects
- ModularVNext
- Module1
- Module2

ModularVNext is the main web application.

Module1 is a class library project with Controllers and Views. The Views are packaged as resources.

Module2 is a class library project that puts the Controllers and Views inside an MVC Area.
This is probably a good route to take as it will help isolate module contents.
This module also includes a CSS file to demonstrate static content (again bundled as a resource).

When you run the application you can click on the link to render the content from Module1's view.

## Module discovery ##
Modules are discovered by being placed in the modules folder under the bin folder.
The "moduleLoadPath" configuration value determines the path (which allows it to vary between dev and deployment).
There is some code scan this directory and load the assemblies in the Startup class.

## Serving module content ##
The serving of module content is enabled through a CompositeFileProvider. Out of the box, MVC only allows a single provider.
The composite provider allows multiple file providers to be chained together. This is registered in Startup.

There are multiple PhysicalFileProviders included in the composite provider.
These are determined by the configuration value "additionalFileProviderBasePaths".
Adding paths here for modules that you are developing allows MVC to locate and serve content from those paths.
(As noted in the code, this is not recommended for production).

The second mechanism that is used is to add EmbeddedFileProviders that serve views from compiled resources.
In the project.json for Module1, the Views folder is marked for being compiled in as a resource.
Module2 marks the Content and Views folders for being included as resources.

To allow MVC to discover the controllers, there is a ModuleAwareAssemblyProvider that includes the loaded assemblies into
the set of assemblies that MVC scans for controllers.

## Dev time experience ##
In order to include the modules, they need to produce build output i.e. an assembly (a project option).
Beyond this, the assembly needs to be in the modules folder.
The project uses a grunt task that is bound to the After Build event to copy the modules over to that directory.