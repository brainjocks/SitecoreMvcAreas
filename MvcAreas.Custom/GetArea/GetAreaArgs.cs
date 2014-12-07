using Sitecore.Diagnostics;
using Sitecore.Mvc.Pipelines;

namespace MvcAreas.Custom.GetArea
{
    public class GetAreaArgs : MvcPipelineArgs
    {
        public GetAreaArgs(Sitecore.Mvc.Presentation.Rendering rendering)
        {
            Assert.IsNotNull(rendering, "rendering");
            Rendering = rendering;
        }

        public Sitecore.Mvc.Presentation.Rendering Rendering { get; set; }

        public string AreaName { get; set; }
    }
}