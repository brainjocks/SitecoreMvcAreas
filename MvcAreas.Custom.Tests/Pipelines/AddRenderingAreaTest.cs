using System.IO;
using System.Web.Routing;
using FluentAssertions;
using MvcAreas.Custom.Render;
using NSubstitute;
using NUnit.Framework;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.FakeDb;
using Sitecore.Mvc.Common;
using Sitecore.Mvc.Pipelines.Response.RenderRendering;
using Sitecore.Mvc.Presentation;

namespace MvcAreas.Custom.Tests.Pipelines
{
    public class AddRenderingAreaTest
    {
        [Test]
        public void ShouldSkipIfRendered()
        {
            //arrange
            var processor = Substitute.ForPartsOf<AddRenderingArea>();
            var args = new RenderRenderingArgs(new Rendering(), new StringWriter())
            {
                Rendered = true
            };

            // act
            processor.Process(args);

            // assert
            processor.DidNotReceive().DoProcess(Arg.Any<PageContext>(), Arg.Any<string>(), args);
        }

        [Test]
        public void ShouldSkipIfNoPageContext()
        {
            //arrange
            var processor = Substitute.ForPartsOf<AddRenderingArea>();
            var args = new RenderRenderingArgs(new Rendering(), new StringWriter());

            // act
            processor.Process(args);

            // assert
            processor.DidNotReceive().DoProcess(Arg.Any<PageContext>(), Arg.Any<string>(), args);
        }

        [TestCase("")]
        [TestCase(null)]
        public void ShouldDoNothingIfNoAreaDetected(string areaname)
        {
            //arrange
            using (var db = new Db
            {
                new DbItem("rendering", ID.NewID)
            })
            {
                Item item = db.GetItem("/sitecore/content/rendering");
                var rendering = new Rendering
                {
                    RenderingItem = item
                };

                var args = new RenderRenderingArgs(rendering, new StringWriter());
                var processor = Substitute.ForPartsOf<AddRenderingArea>();
                processor.GetAreaName(Arg.Any<Rendering>()).Returns(areaname);

                var pageContext = new PageContext();
                var renderingContext = new RenderingContext();

                using (ContextService.Get().Push(pageContext))
                using (ContextService.Get().Push(renderingContext))
                {
                    // act
                    processor.Process(args);
                }

                // assert
                processor.DidNotReceive().DoProcess(Arg.Any<PageContext>(), areaname, args);
            }
        }

        [Test]
        public void ShouldCallScoreMvcGetAreaToGetTheArea()
        {
            //arrange
            using (var db = new Db())
            {
                var processor = new AddRenderingArea();

                db.PipelineWatcher.Expects("mvc.getArea");

                // act
                processor.GetAreaName(new Rendering());

                // assert
                db.PipelineWatcher.EnsureExpectations();
            }
        }

        [Test]
        public void ShouldSetAndUnsetAreaIfEmpty()
        {
            //arrange
            var pageContext = new PageContext
            {
                RequestContext = new RequestContext
                {
                    RouteData = new RouteData()
                }
            };

            pageContext.RequestContext.RouteData.DataTokens.ContainsKey("area").Should().BeFalse();

            // act
            using (ContextService.Get().Push(pageContext))
            using (new AddRenderingArea.RenderingAreaContext(pageContext, "MySolution"))
            {
                pageContext.RequestContext.RouteData.DataTokens["area"].Should().Be("MySolution");
            }

            pageContext.RequestContext.RouteData.DataTokens.ContainsKey("area").Should().BeFalse();
        }

        [Test]
        public void ShouldSetAndRestoreArea()
        {
            //arrange
            var pageContext = new PageContext
            {
                RequestContext = new RequestContext
                {
                    RouteData = new RouteData()
                }
            };

            pageContext.RequestContext.RouteData.DataTokens["area"] = "OldArea";

            // act
            using (ContextService.Get().Push(pageContext))
            using (new AddRenderingArea.RenderingAreaContext(pageContext, "NewArea"))
            {
                pageContext.RequestContext.RouteData.DataTokens["area"].Should().Be("NewArea");
            }

            pageContext.RequestContext.RouteData.DataTokens["area"].Should().Be("OldArea");
        }
    }
}