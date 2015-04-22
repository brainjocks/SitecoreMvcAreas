using FluentAssertions;
using MvcAreas.Custom.GetArea;
using NUnit.Framework;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.FakeDb;

namespace MvcAreas.Custom.Tests.Pipelines
{
    public class GetAreaByRenderingFolderTest
    {
        [Test]
        public void ShouldNotSetAreaIfNoFolderOfTheRightType()
        {
            //arrange
            using (var db = new Db
            {
                new DbItem("rendering")
            })
            {
                Item item = db.GetItem("/sitecore/content/rendering");
                var rendering = new Sitecore.Mvc.Presentation.Rendering
                {
                    RenderingItem = item
                };

                var args = new GetAreaArgs(rendering);
                var processor = new GetAreaByRenderingFolder();

                // act
                processor.Process(args);

                // assert
                args.AreaName.Should().BeNull();
            }
        }

        [TestCase("MySolution", "MySolution")]
        [TestCase("", null)]
        [TestCase(" ", null)]
        [TestCase("\t", null)]
        public void ShouldGetAreaNameFromTheFolder(string area, string expectation)
        {
            //arrange
            using (var db = new Db
            {
                new DbTemplate("Area Folder", new ID(Const.TemplateIds.MvcAreaNameBase))
                {
                    "Area Name"
                },
                new DbItem("MySolution", ID.NewID, new ID(Const.TemplateIds.MvcAreaNameBase))
                {
                    {"Area Name", area},
                    new DbItem("rendering")
                }
            })
            {
                Item item = db.GetItem("/sitecore/content/MySolution/rendering");
                var rendering = new Sitecore.Mvc.Presentation.Rendering
                {
                    RenderingItem = item
                };

                var args = new GetAreaArgs(rendering);
                var processor = new GetAreaByRenderingFolder();

                // act
                processor.Process(args);

                // assert
                args.AreaName.Should().Be(expectation);
            }
        }
    }
}