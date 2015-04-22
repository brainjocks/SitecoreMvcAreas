#SitecoreMvcAreas

An implementation to allow Sitecore MVC renderings and layouts to designate an MVC Area.

When using MVC renderings, Sitecore does not provide a specific way to designate the MVC area that the rendering should exist within. This will provide the developer a way to specify the MVC area name within the content tree when defining the rendering definition items.

##Why?

The MVC Area is used to tell MVC the location of assets within the running website. In SCORE, we recommend the use of MVC areas to isolate assets within each tenant of a multi-tenant environment.

Controller rendering view resolution
When creating controller renderings, MVC will "expect" that the view is located in a specific place (based on how the view is referenced by the controller).

DisplayTemplates and EditTemplates
Display and Edit Templates can be used to modularize the UI code to small cshtml files that are designed to render specific model classes.  This can be used instead of formatting your output with helpers (or worse with code snippets) that are used as HTML formatters.

##How?

There is no single obvious way to tell a Sitecore rendering or layout document about it's area name.  Also, we should avoid the obvious - modifying any built-in templates that Sitecore offers.

Since there is no obvious single way to do this, it might be a combination of techniques - so we will introduce a new pipeline into Sitecore to get the area name for a specific rendering being processed by the RenderRendering pipeline.

###2 Approaches

We have included 2 approaches in the pipeline - first, using a new rendering folder template that can specify the area name for inclusive renderings nested within the folder.

Second, and since you cannot use the same approach with layout folders, another method is provided to look at the view path and "extract" the area name from the path of the cshtml or ascx file.

##Usage

1. Add the folder "master" to your serialization folder in the Sitecore data directory, then on the developer tab, use the update tree button to add the needed templates into your instance of Sitecore.
2. Next, build and copy the DLLs from the solution into your instance, and add in the patched configuration file to the App_Config/Include folder.
3. To use the rendering folder data template, simply replace a normal rendering folder for your renderings (for example, /sitecore/Layout/renderings/MyRenderings) with the provided "Rendering Folder with Area" template, and specify an area name on the item content tab.  

You can test it by creating a controller rendering and using the syntax
'''
return PartialView(model);
'''

- MVC should automatically find the proper view file in the folder /Areas/AreaName/Views/ControllerName/ActionName.cshtml

For the view renderings and layout documents, the other processor will automatically find the area name based on the folder pattern:

/Areas/&lt;Area Name&gt;/views/....