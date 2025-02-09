using NUnit.Framework;
using DocumentFormat.OpenXml.Wordprocessing;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml;

namespace HtmlToOpenXml.Tests
{
    /// <summary>
    /// Tests parser with various complex input Html.
    /// </summary>
    [TestFixture]
    public class ParserTests : HtmlConverterTestBase
    {
        [TestCase("<!--<p>some text</p>-->")]
        [TestCase("<script>document.getElementById('body');</script>")]
        [TestCase("<style>{font-size:2em}</script>")]
        [TestCase("<xml><element><childElement attr='value' /></element></xml>")]
        [TestCase("<button>Save</button>")]
        [TestCase("<input type='search' placeholder='Search' />")]
        [TestCase("<progress>max='100' value='70'>70%</progress>")]
        [TestCase("<select><option>--Please select--</option></select>")]
        [TestCase("<textarea>Placeholder</textarea>")]
        [TestCase("<meter min='200' max='500' value='350'>350 degrees</meter>")]
        public void IgnoreTag(string html)
        {
            // the inner html shouldn't be interpreted
            var elements = converter.Parse(html);
            Assert.That(elements, Is.Empty);
        }

        [Test]
        public void ParseUnclosedTag()
        {
            var elements = converter.Parse("<p>some text in <i>italics <b>,bold and italics</p>");
            Assert.That(elements, Has.Count.EqualTo(1));
            Assert.That(elements[0].InnerText, Is.EqualTo("some text in italics ,bold and italics"));

            var runProperties = elements[0].ChildElements[0].GetFirstChild<RunProperties>();
            Assert.That(runProperties, Is.Null);

            runProperties = elements[0].ChildElements[2].GetFirstChild<RunProperties>();
            Assert.That(runProperties, Is.Not.Null);
            Assert.That(runProperties.HasChild<Italic>(), Is.EqualTo(true));
            Assert.That(runProperties.HasChild<Bold>(), Is.EqualTo(false));

            runProperties = elements[0].ChildElements[4].GetFirstChild<RunProperties>();
            Assert.That(runProperties, Is.Not.Null);
            Assert.That(runProperties.HasChild<Italic>(), Is.EqualTo(true));
            Assert.That(runProperties.HasChild<Bold>(), Is.EqualTo(true));

            elements = converter.Parse("<p>First paragraph in semi-<i>italics <p>Second paragraph still italic <b>but also in bold</b></p>");
            Assert.That(elements, Has.Count.EqualTo(2));
            Assert.Multiple(() =>
            {
                Assert.That(elements[0].ChildElements, Has.Count.EqualTo(3));
                Assert.That(elements[1].ChildElements, Has.Count.EqualTo(3));
            });

            runProperties = elements[0].ChildElements[0].GetFirstChild<RunProperties>();
            Assert.That(runProperties, Is.Null);

            runProperties = elements[0].ChildElements[2].GetFirstChild<RunProperties>();
            Assert.That(runProperties, Is.Not.Null);
            Assert.That(runProperties.HasChild<Italic>(), Is.EqualTo(true));

            runProperties = elements[1].FirstChild.GetFirstChild<RunProperties>();
            Assert.That(runProperties, Is.Not.Null);
            Assert.That(runProperties.HasChild<Italic>(), Is.EqualTo(true));
            Assert.That(runProperties.HasChild<Bold>(), Is.EqualTo(false));

            runProperties = elements[1].ChildElements[2].GetFirstChild<RunProperties>();
            Assert.That(runProperties, Is.Not.Null);
            Assert.That(runProperties.HasChild<Italic>(), Is.EqualTo(true));
            Assert.That(runProperties.HasChild<Bold>(), Is.EqualTo(true));

            // this should generate a new paragraph with its own style
            elements = converter.Parse("<p>First paragraph in <i>italics </i><p>Second paragraph not in italic</p>");
            Assert.That(elements, Has.Count.EqualTo(2));
            Assert.That(elements[0].ChildElements, Has.Count.EqualTo(3));
            Assert.That(elements[1].ChildElements, Has.Count.EqualTo(1));
            Assert.That(elements[1].FirstChild, Is.TypeOf(typeof(Run)));

            runProperties = elements[1].FirstChild.GetFirstChild<RunProperties>();
            Assert.That(runProperties, Is.Null);
        }

        [TestCase("<p>Some\ntext</p>", ExpectedResult = 1)]
        [TestCase("<p>Some <b>bold\n</b>text</p>", ExpectedResult = 5)]
        [TestCase("\t<p>Some <b>bold\n</b>text</p>", ExpectedResult = 5)]
        [TestCase("  <p>Some text</p> ", ExpectedResult = 1)]
        public int ParseNewline (string html)
        {
            var elements = converter.Parse(html);
            return elements[0].Count(c => c is Run);
        }

        [Test]
        public void ParseNotTag ()
        {
            var elements = converter.Parse(" < b >bold</b>");
            Assert.That(elements, Has.Count.EqualTo(1));
            Assert.That(elements[0].ChildElements, Has.Count.EqualTo(1));
            Assert.That(elements[0].FirstChild, Is.TypeOf<Run>());
            Assert.That(elements[0].FirstChild.InnerText, Is.EqualTo("< b >bold"));

            elements = converter.Parse(" <3");
            Assert.That(elements, Has.Count.EqualTo(1));
            Assert.That(elements[0].ChildElements, Has.Count.EqualTo(1));
            Assert.That(elements[0].FirstChild, Is.TypeOf<Run>());
            Assert.That(elements[0].FirstChild.InnerText, Is.EqualTo("<3"));
        }

        [Test(Description = "Provided html is only whitespaces")]
        public void IgnoreEmptyText()
        {
            var elements = converter.Parse("  \n");
            Assert.That(elements, Is.Empty);
        }

        [Test(Description = "Provided mainPart is empty")]
        public void InitNewDocument()
        {
            using var generatedDocument = new MemoryStream();
            using var package = WordprocessingDocument.Create(generatedDocument, WordprocessingDocumentType.Document);
            mainPart = package.MainDocumentPart;
            mainPart = package.AddMainDocumentPart();

            Assert.That(mainPart.Document, Is.Null);

            var elements = new HtmlConverter(mainPart).Parse("Placeholder");
            Assert.That(elements, Is.Not.Empty);
        }
    }
}