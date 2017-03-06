using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Drawing;
using System.Threading.Tasks;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Drawing.Imaging;

namespace ComicStreamer.Tests
{
    [TestClass()]
    public class UnitTest1
    {
        readonly string BaseUrl = "http://192.168.1.82:32500";

        [TestMethod()]
        public async Task GetDBInfoTest()
        {
            ComicStreamerSharp cs = new ComicStreamerSharp(BaseUrl);

            string response = await cs.GetDBInfo();

            if (response.Length > 0)
            {
                Console.WriteLine(response);
            }
            else
            {
                Assert.Fail();
            }
        }

        [TestMethod()]
        public async Task GetVersionTest()
        {
            ComicStreamerSharp cs = new ComicStreamerSharp(BaseUrl);
            string response = await cs.GetVersion();

            if (response.Length > 0)
            {
                Console.WriteLine(response);
            }
            else
            {
                Assert.Fail();
            }
        }

        [TestMethod()]
        public async Task GetDeletedComicsTest()
        {
            ComicStreamerSharp cs = new ComicStreamerSharp(BaseUrl);
            var response = await cs.GetDeletedComics("2017-2-19");

            if (response.Length > 0)
            {
                Console.WriteLine(response);
            }
            else
            {
                Assert.Fail();
            }

            response = await cs.GetDeletedComics("2017-2-21");

            if (response.Length > 0)
            {
                Console.WriteLine(response);
            }
            else
            {
                Assert.Fail();
            }
        }

        [TestMethod()]
        public async Task GetComicInfoTest()
        {
            ComicStreamerSharp cs = new ComicStreamerSharp(BaseUrl);
            var response = await cs.GetComicInfo(98);

            string series = (string)response["comics"][0]["series"];

            Console.WriteLine(response.ToString(Formatting.Indented));

            Console.WriteLine("Series = " + (string)response["comics"][0]["series"]);

            if (!series.Equals("The Fuse", StringComparison.OrdinalIgnoreCase))
            {
                Assert.Fail();
            }
        }

        [TestMethod()]
        public async Task GetComicPageTest()
        {
            ComicStreamerSharp cs = new ComicStreamerSharp(BaseUrl);
            Byte[] response = await cs.GetComicPage(98);

            if (response.Length > 0)
            {
                using (Image image = Image.FromStream(new MemoryStream(response)))
                {
                    image.Save("output.jpg", ImageFormat.Jpeg);  // Or Png
                }
            }
            else
            {
                Assert.Fail();
            }
        }

        [TestMethod()]
        public async Task SetBookmarkTest()
        {
            ComicStreamerSharp cs = new ComicStreamerSharp(BaseUrl);
            cs.SetBookmark(98, "10");

            var response = await cs.GetComicInfo(98);
            string lastread_page = (string)response["comics"][0]["lastread_page"];

            if (!lastread_page.Equals("10"))
            {
                Assert.Fail();
            }

            cs.SetBookmark(98);
        }

        [TestMethod()]
        public async Task GetThumbnailTest()
        {
            ComicStreamerSharp cs = new ComicStreamerSharp(BaseUrl);
            Byte[] response = await cs.GetThumbnail(98);

            if (response.Length > 0)
            {
                using (Image image = Image.FromStream(new MemoryStream(response)))
                {
                    image.Save("thumbnail.jpg", ImageFormat.Jpeg);  // Or Png
                }
            }
            else
            {
                Assert.Fail();
            }
        }

        [TestMethod()]
        public async Task GetComicFileTest()
        {
            ComicStreamerSharp cs = new ComicStreamerSharp(BaseUrl);
            Byte[] file = await cs.GetComicFile(98);

            var response = await cs.GetComicInfo(98);

            string filename = System.IO.Path.GetFileName((string)response["comics"][0]["path"]);

            if (file.Length > 0 && filename.Length > 0)
            {
                File.WriteAllBytes(filename, file);
            }
            else
            {
                Assert.Fail();
            }
        }

        [TestMethod()]
        public async Task GetComicListTest()
        {
            // Test getting all comics
            ComicStreamerSharp cs = new ComicStreamerSharp(BaseUrl);
            JObject list = await cs.GetComicList();
            JArray comics = (JArray)list["comics"];

            Console.WriteLine("Found " + comics.Count + " comics");

            if (comics.Count > 100)
            {
                // Test getting all of one series
                list = await cs.GetComicList("The Fuse");
                comics = (JArray)list["comics"];

                Console.WriteLine("Found " + comics.Count + " issues of The Fuse");

                Console.WriteLine(comics.ToString(Formatting.Indented));

                if (comics.Count == 0 && comics.Count > 100)
                {
                    Assert.Fail();
                }
            }
        }

        [TestMethod()]
        public async Task GetFoldersTest()
        {
            ComicStreamerSharp cs = new ComicStreamerSharp(BaseUrl);
            JObject list = await cs.GetFolders();

            if (list.Count == 0)
            {
                Assert.Fail();
            }
        }

    }
}