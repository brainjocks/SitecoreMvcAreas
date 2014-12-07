using System;
using System.Text.RegularExpressions;
using Sitecore.Mvc.Presentation;

namespace MvcAreas.Custom.GetArea
{
    public class GetAreaByRenderingPath : GetAreaProcessor
    {
        public override void Process(GetAreaArgs args)
        {
            if (SkipProcessor(args))
            {
                return;
            }

            FindAreaByPath(args);
        }

        public virtual void FindAreaByPath(GetAreaArgs args)
        {
            string path = ((ViewRenderer) args.Rendering.Renderer).ViewPath;
            string areaName = GetAreaName(path);

            if (String.IsNullOrWhiteSpace(areaName))
            {
                return;
            }

            args.AreaName = areaName;
        }

        public virtual string GetAreaName(string renderingPath)
        {
            Match m = Regex.Match(renderingPath,
                @"\/areas\/(?<areaname>[^\s\/]*)\/views\/(\/[\w\s\-\+]+\/)*(.*\.(cshtml|ascx)$)",
                RegexOptions.IgnoreCase);

            return m.Success ? m.Groups["areaname"].Value : null;
        }

        public override bool SkipProcessor(GetAreaArgs args)
        {
            var viewRenderer = args.Rendering.Renderer as ViewRenderer;
           
            return base.SkipProcessor(args) || viewRenderer == null || String.IsNullOrEmpty(viewRenderer.ViewPath);
        }
    }
}