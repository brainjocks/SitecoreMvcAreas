using FluentAssertions;
using MvcAreas.Custom.GetArea;
using NUnit.Framework;
using Sitecore.Data.Items;
using Sitecore.FakeDb;
using Sitecore.Mvc.Presentation;

namespace MvcAreas.Custom.Tests.Pipelines
{
    public class GetAreaByRenderingPathTest
    {
        [TestCase("/Areas/MySolution/Views/Shared/Content/_test.cshtml", "MySolution")]
        [TestCase("/areas/MySolution/views/Shared/test.ascx", "MySolution")]
        [TestCase("/views/SomePlace/test/test.ascx", null)]
        [TestCase("/areas/SomeArea/views/shared/content/some view.cshtml", "SomeArea")]
        public void ShouldCaptureAreaFromPath(string renderingPath, string expectedValue)
        {
            //arrange
            using (var db = new Db
            {
                new DbItem("renderingorlayout")
            })
            {
                Item renderingItem = db.GetItem("/sitecore/content/renderingorlayout");

                var rendering = new Rendering
                {
                    RenderingItem = renderingItem,
                    Renderer = new ViewRenderer
                    {
                        ViewPath = renderingPath
                    }
                };

                var args = new GetAreaArgs(rendering);
                var processor = new GetAreaByRenderingPath();

                // act
                processor.Process(args);

                // assert
                args.AreaName.Should().Be(expectedValue);
            }
        }

        [Test]
        public void ShouldNotWorkOnControllerRendering()
        {
            //arrange
            using (var db = new Db
            {
                new DbItem("renderingorlayout")
            })
            {
                Item renderingItem = db.GetItem("/sitecore/content/renderingorlayout");

                var rendering = new Rendering
                {
                    RenderingItem = renderingItem,
                    Renderer = new ControllerRenderer
                    {
                        ActionName = "ActionName",
                        ControllerName="Score.Web.Areas.AreaName.Controllers.SomeController, Score.Web"
                    }
                };

                var args = new GetAreaArgs(rendering);
                var processor = new GetAreaByRenderingPath();

                // act
                processor.Process(args);

                // assert
                args.AreaName.Should().BeNull();
            }
        }
    }
}