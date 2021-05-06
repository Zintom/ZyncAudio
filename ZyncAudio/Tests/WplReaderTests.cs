using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Text;
using ZyncAudio.PlaylistReaders;

namespace Tests
{
    [TestClass]
    public class WplReaderTests
    {
        private readonly string _wplExample = @"<?wpl version='1.0'?>
<smil>
    <head>
        <meta name='Generator' content='Microsoft Windows Media Player -- 11.0.5721.5145'/>
        <meta name='AverageRating' content='33'/>
        <meta name='TotalDuration' content='1102'/>
        <meta name='ItemCount' content='3'/>
        <author/>
        <title>Bach Organ Works</title>
    </head>
    <body>
        <seq>
            <media src='\\server\vol\music\Classical\Bach\OrganWorks\cd03\track01.mp3' />
            <media src='\\server\vol\music\Classical\Bach\OrganWorks\cd03\track02.mp3'/>
            <media src='SR15.mp3' tid='{35B39D45-94D8-40E1-8FC2-9F6714191E47}'/>
        </seq>
    </body>
</smil>";

        [TestMethod]
        public void BestCaseScenario()
        {
            using (var stream = new MemoryStream())
            using (var writer = new StreamWriter(stream, Encoding.UTF8))
            {
                writer.Write(_wplExample.Replace('\'', '\"'));
                writer.Flush();
                stream.Position = 0;

                var playlist = new WPLReader().Read(stream, null);

                Assert.IsTrue(playlist.Generator == "Microsoft Windows Media Player -- 11.0.5721.5145");
                Assert.IsTrue(playlist.AverageRating == 33);
                Assert.IsTrue(playlist.TotalDuration == 1102);
                Assert.IsTrue(playlist.ItemCount == 3);
                Assert.IsTrue(playlist.Title == "Bach Organ Works");
            }
        }

        [TestMethod]
        public void PlaylistHasRelativePathMedia()
        {
            using (var stream = new MemoryStream())
            using (var writer = new StreamWriter(stream, Encoding.UTF8))
            {
                string playlistFileLocation = @"C:\User\Example\Music\Playlists\examplePlaylist1.wpl";
                string trackFileLocation = @"..\example.mp3";

                writer.Write($"<smil><body><seq>  <media src=\"{trackFileLocation}\"/>  </seq></body></smil>");
                writer.Flush();
                stream.Position = 0;

                var playlist = new WPLReader().Read(stream, playlistFileLocation);

                Assert.IsTrue(playlist.MediaItems.Count == 1);
                Assert.IsTrue(playlist.MediaItems[0] == @"C:\User\Example\Music\example.mp3");
            }
        }
    }
}
