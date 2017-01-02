using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pensieve.Model;

namespace PensieveTest
{
    [TestClass]
    public class AlbumTest
    {
        [TestMethod]
        public void TestCreateAlbumFromFileName()
        {
            Album album = new Album("C:\\Users\\Daniel\\Pictures\\2010_07 - album name");
            Assert.AreEqual("album name", album.Title);
            Assert.AreEqual("C:\\Users\\Daniel\\Pictures\\2010_07 - album name", album.FilePath);
            Assert.AreEqual("2010_07 - album name", album.AlbumName);
        }
    }
}
