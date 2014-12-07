using System;
using Sitecore.Mvc.Pipelines;

namespace MvcAreas.Custom.GetArea
{
    public abstract class GetAreaProcessor : MvcPipelineProcessor<GetAreaArgs>
    {
        public bool Found(GetAreaArgs args)
        {
            return !String.IsNullOrEmpty(args.AreaName);
        }

        public virtual bool SkipProcessor(GetAreaArgs args)
        {
            return args.Rendering == null || args.Rendering.RenderingItem == null || Found(args);
        }
    }
}