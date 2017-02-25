using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Drawing;

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
        public void GetDBInfoTest()
        {
            ComicStreamerSharp cs = new ComicStreamerSharp(BaseUrl);
            var response = cs.GetDBInfo();

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
        public void GetVersionTest()
        {
            ComicStreamerSharp cs = new ComicStreamerSharp(BaseUrl);
            var response = cs.GetVersion();

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
        public void GetDeletedComicsTest()
        {
            ComicStreamerSharp cs = new ComicStreamerSharp(BaseUrl);
            var response = cs.GetDeletedComics("2017-2-19");

            if (response.Length > 0)
            {
                Console.WriteLine(response);
            }
            else
            {
                Assert.Fail();
            }

            response = cs.GetDeletedComics("2017-2-21");

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
        public void GetComicInfoTest()
        {
            ComicStreamerSharp cs = new ComicStreamerSharp(BaseUrl);
            var response = cs.GetComicInfo("98");

            string series = (string)response["comics"][0]["series"];

            Console.WriteLine(response.ToString(Formatting.Indented));

            Console.WriteLine("Series = " + (string)response["comics"][0]["series"]);

            if (!series.Equals("The Fuse", StringComparison.OrdinalIgnoreCase))
            {
                Assert.Fail();
            }
        }

        [TestMethod()]
        public void GetComicPageTest()
        {
            ComicStreamerSharp cs = new ComicStreamerSharp(BaseUrl);
            Byte [] response = cs.GetComicPage("98");

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
        public void SetBookmarkTest()
        {
            ComicStreamerSharp cs = new ComicStreamerSharp(BaseUrl);
            cs.SetBookmark("98", "10");

            var response = cs.GetComicInfo("98");
            string lastread_page = (string)response["comics"][0]["lastread_page"];

            if (!lastread_page.Equals("10"))
            {
                Assert.Fail();
            }

            cs.SetBookmark("98");
        }

        [TestMethod()]
        public void GetThumbnailTest()
        {
            ComicStreamerSharp cs = new ComicStreamerSharp(BaseUrl);
            Byte[] response = cs.GetThumbnail("98");

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
        public void GetComicFileTest()
        {
            ComicStreamerSharp cs = new ComicStreamerSharp(BaseUrl);
            Byte[] file = cs.GetComicFile("98");

            var response = cs.GetComicInfo("98");

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
        public void GetComicListTest()
        {
            // Test getting all comics
            ComicStreamerSharp cs = new ComicStreamerSharp(BaseUrl);
            JObject list = cs.GetComicList();
            JArray comics = (JArray)list["comics"];

            Console.WriteLine("Found " + comics.Count + " comics");

            if (comics.Count > 100)
            {
                // Test getting all of one series
                list = cs.GetComicList("The Fuse");
                comics = (JArray)list["comics"];

                Console.WriteLine("Found " + comics.Count + " issues of The Fuse");

                if (comics.Count == 0 && comics.Count > 100)
                {
                    Assert.Fail();
                }
            }
        }

        [TestMethod()]
        public void GetFoldersTest()
        {
            ComicStreamerSharp cs = new ComicStreamerSharp(BaseUrl);
            JObject list = cs.GetFolders();

            if (list.Count == 0)
            {
                Assert.Fail();
            }
        }

    }
}