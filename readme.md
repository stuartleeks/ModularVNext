# Readme #
This project demonstrates a couple of ways to encapsulate packages of functionality in an ASP.NET MVC 6 project. I.e. to allow modules of ASP.NET MVC 6 functionality to be written in separate projects from the main application.


The goals for this sample are to reduce friction at development and deployment time. At development time, it is desirable to work with files on disk for rapid development cycles. It is also key to avoid copying files from modules into the main application folder. At deployment time, it is beneficial to minimise the amount 


This sample covers the module packaging and use. Module discovery is out of scope for this sample 

This sample was written against MVC 6.0.0-beta3

## Solution structure ##
The solution contains two projects
- ModularVNext
- Module1

ModularVNext is the main web application.
Module1 is a class library project with Controllers and Views.


When you run the application you can click on the link to render the content from Module1's view.

## Serving module content ##
The serving of module content is enabled through a CompositeFileProvider. Out of the box, MVC only allows a single provider. The composite provider allows multiple file providers to be chained together. This is registered in Startup.


There are multiple PhysicalFileProviders included in the composite provider. These are determined by the configuration value "additionalFileProviderBasePaths". Adding paths here for modules that you are developing allows MVC to locate and serve content from those paths. (As noted in the code, this is not recommended for production).

The second mechanism that is used is to add EmbeddedFileProviders that serve views from compiled resources. In the project.json for Module1, the Views folder is marked for being compiled in as a resource. 