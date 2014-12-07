using System;
using System.Web.Routing;
using MvcAreas.Custom.GetArea;
using Sitecore.Diagnostics;
using Sitecore.Mvc.Pipelines;
using Sitecore.Mvc.Pipelines.Response.RenderRendering;
using Sitecore.Mvc.Presentation;

namespace MvcAreas.Custom.Render
{
    public class AddRenderingArea : RenderRenderingProcessor
    {
        /// <summary>
        /// Entry point for pipeline processor - will set the areaname of the MVC rendering based on execution of a separate pipeline
        /// to find the proper area name
        /// </summary>
        /// <param name="args"></param>
        public override void Process(RenderRenderingArgs args)
        {
            if (args.Rendered)
            {
                return;
            }

            PageContext pageContext = PageContext.CurrentOrNull;

            Rendering rendering = args.Rendering;

            // rendering.RenderingItem is null when presentation details points to a rendering that is no longer in Sitecore 
            if (rendering == null || rendering.RenderingItem == null || pageContext == null)
            {
                return;
            }

            string areaName = GetAreaName(rendering);

            if (string.IsNullOrEmpty(areaName))
            {
                return;
            }

            DoProcess(pageContext, areaName, args);
        }

        public virtual string GetAreaName(Rendering rendering)
        {
            return PipelineService.Get()
                .RunPipeline("mvc.getArea", new GetAreaArgs(rendering), arg => arg.AreaName);
        }

        public virtual void DoProcess(PageContext pageContext, string areaName, RenderRenderingArgs args)
        {
            args.Disposables.Add(new RenderingAreaContext(pageContext, areaName));
        }

        /// <summary>
        ///     A disposable that will set the MVC Area for the current RequestContext on creation and get it back to the state it
        ///     was in on dispose
        /// </summary>
        public class RenderingAreaContext : IDisposable
        {
            private readonly DisposeHelper _disposer = new DisposeHelper(true);

            public RenderingAreaContext(PageContext pageContext, string newAreaName)
            {
                RequestContext = pageContext.RequestContext;

                // not sure if this can occur, but just in case for now
                if (!RequestContext.RouteData.DataTokens.ContainsKey("area"))
                {
                    PreviousAreaWasNull = true;
                    RequestContext.RouteData.DataTokens.Add("area", newAreaName);
                }
                else
                {
                    PreviousArea = (string) RequestContext.RouteData.DataTokens["area"];
                    RequestContext.RouteData.DataTokens["area"] = newAreaName;
                }
            }

            private RequestContext RequestContext { get; set; }

            private string PreviousArea { get; set; }

            private bool PreviousAreaWasNull { get; set; }

            public void Dispose()
            {
                if (_disposer.Disposed)
                {
                    return;
                }

                if (PreviousAreaWasNull)
                {
                    RequestContext.RouteData.DataTokens.Remove("area");
                }
                else
                {
                    RequestContext.RouteData.DataTokens["area"] = PreviousArea;
                }
            }
        }
    }
}