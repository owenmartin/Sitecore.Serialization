# Sitecore.Serialization #
Sitecore.Serialization is a pipelined re-work of the built in serialization in Sitecore.

## Why? ##
Much of sitecore is nicely customisable via the use of pipelines (among other things), apart from serialization. This project tries to bring the customisable aspect of sitecore to serialization, without breaking existing functionality.

## Building the solution ##
Drop the relevant version of Sitecore.Kernel.dll and Newtonsoft.Json.dll into the `lib` directory, and build via Visual Studio or MSBuild.

### The nitty gritty ###
In the `dist` folder, there is already the build DLL, as well as two example include config files.  One file (`SitecoreSerialization.config.example`) will add the pipelines and commands for the standard Sitecore serialization - i.e. there will be no difference to serializing items.  The other file (`JsonSerialization.config.example`) is a showcase implementation of using the pipelines to store item data in JSON format.

### What can I do with this? ###
Out of the box, there may not be huge benefits to using the JSON implementation over the standard format (apart from maybe getting away from the content-length atrocity), but the project as a whole opens up a world of possibility.  Rather than storing the data on disk, why not store it in a NoSQL store to easily share data between environments?  Maybe you have a deep love of XML files, and would rather serialize to that? 