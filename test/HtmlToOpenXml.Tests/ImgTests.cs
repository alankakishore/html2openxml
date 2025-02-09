using NUnit.Framework;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;

namespace HtmlToOpenXml.Tests
{
    using pic = DocumentFormat.OpenXml.Drawing.Pictures;

    /// <summary>
    /// Tests images.
    /// </summary>
    [TestFixture]
    public class ImgTests : HtmlConverterTestBase
    {
        [TestCase(@"<img src='https://www.w3schools.com/tags/smiley.gif' alt='Smiley face' width='42' height='42'>")]
        [TestCase(@"<img src='data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAAUAAAAFCAYAAACNbyblAAAAHElEQVQI12P4//8/w38GIAXDIBKE0DHxgljNBAAO9TXL0Y4OHwAAAABJRU5ErkJggg==' alt='Smiley face' width='42' height='42'>")]
        public void ParseImg(string html)
        {
            var elements = converter.Parse(html);
            Assert.That(elements, Has.Count.EqualTo(1));
            AssertIsImg(elements[0]);
        }

        [Test]
        public void ParseImgBorder()
        {
            var elements = converter.Parse(@"<img src='https://www.w3schools.com/tags/smiley.gif' border='1'>");
            AssertIsImg(elements[0]);
            var run = elements[0].GetFirstChild<Run>();
            RunProperties runProperties = run.GetFirstChild<RunProperties>();
            Assert.That(runProperties, Is.Not.Null);
            Assert.That(runProperties.Border, Is.Not.Null);
        }

        [Test]
        public void ParseImgManualProvisioning()
        {
            converter = new HtmlConverter(mainPart, new LocalWebRequest());

            var elements = converter.Parse(@"<img src='/img/black-dot' alt='Smiley face' width='42' height='42'>");
            Assert.That(elements, Has.Count.EqualTo(1));
            AssertIsImg(elements[0]);
        }

        [TestCase("<img alt='Smiley face' width='42' height='42'>", Description = "Empty image")]
        [TestCase("<img src='tcp://192.168.0.1:53/attach.jpg'>", Description = "Unsupported protocol")]
        public void IgnoreImage(string html)
        {
            var elements = converter.Parse(html);
            Assert.That(elements, Is.Empty);
        }

        [Test]
        public void SkippedImgManualProvisioning()
        {
            converter = new HtmlConverter(mainPart, new LocalWebRequest());

            var elements = converter.Parse(@$"<img src='/images/{Guid.NewGuid()}.png'>");
            Assert.That(elements, Is.Empty);
        }

        [Test(Description = "Reading local file containing a space in the name")]
        public async Task LoadLocalImage()
        {
            string filepath = Path.Combine(TestContext.CurrentContext.WorkDirectory, @"html2openxml copy.jpg");

            using var resourceStream = ResourceHelper.GetStream("Resources.html2openxml.jpg");
            using (var fileStream = File.OpenWrite(filepath))
                await resourceStream.CopyToAsync(fileStream);

            var localUri = "file:///" + filepath.TrimStart('/').Replace(" ", "%20");
            var elements = await converter.Parse($"<img src='{localUri}'>", CancellationToken.None);
            Assert.That(elements.Count(), Is.EqualTo(1));
            AssertIsImg(elements.First());
        }

        [Test(Description = "Reading local file containing a space in the name")]
        public async Task LoadRemoteImage_BaseUri()
        {
            converter = new HtmlConverter(mainPart, new IO.DefaultWebRequest() { 
                BaseImageUrl = new Uri("http://github.com/onizet/html2openxml")
            });
            var elements = await converter.Parse($"<img src='/blob/dev/icon.png'>", CancellationToken.None);
            Assert.That(elements, Is.Not.Empty);
            AssertIsImg(elements.First());
        }

        private void AssertIsImg (OpenXmlCompositeElement elements)
        {
            var run = elements.GetFirstChild<Run>();
            Assert.That(run, Is.Not.Null);
            var img = run.GetFirstChild<Drawing>();
            Assert.That(img, Is.Not.Null);
            Assert.That(img.Inline?.Graphic?.GraphicData, Is.Not.Null);
            var pic = img.Inline.Graphic.GraphicData.GetFirstChild<pic.Picture>();
            Assert.That(pic?.BlipFill?.Blip?.Embed, Is.Not.Null);

            var imagePartId = pic.BlipFill.Blip.Embed.Value;
            var part = mainPart.GetPartById(imagePartId);
            Assert.That(part, Is.TypeOf(typeof(ImagePart)));
        }
    }
}