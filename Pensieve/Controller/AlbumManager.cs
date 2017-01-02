using Pensieve.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Pensieve.Controller
{
    public class AlbumManager
    {
        public string AlbumLocation { get; private set; }
        private XmlSerializer serializer;

        private List<Album> albums;

        public AlbumManager()
        {
            albums = new List<Album>();
            serializer = new XmlSerializer(typeof(Album));
            AlbumLocation = Environment.ExpandEnvironmentVariables("%HOMEDRIVE%%HOMEPATH%\\Pictures");
            if (!Directory.Exists(AlbumLocation))
            {
                AlbumLocation = Environment.ExpandEnvironmentVariables("%HOMEDRIVE%%HOMEPATH%");
            }
        }

        public AlbumManager(string location) : this()
        {
            AlbumLocation = location;
        }

        public override string ToString()
        {
            return "I am an AlbumManager looking at the folder " + AlbumLocation;
        }

        public List<Album> GetAlbumList()
        {
            albums = FindAlbumsInDirectory(AlbumLocation);
            return albums;
        }

        public List<Album> FindAlbumsInDirectory(string folderPath)
        {
            var albumList = new List<Album>();
            foreach (string folder in Directory.GetDirectories(folderPath))
            {
                if (IsAlbum(folder)) albumList.Add(GetAlbumInfo(folder));
                else albumList.AddRange(FindAlbumsInDirectory(folder));
            }
            return albumList;
        }

        public bool IsAlbum(string folderPath)
        {
            string folderName = folderPath.Substring(folderPath.LastIndexOf('\\') + 1);
            var match = Regex.Match(folderName, @"^\d\d\d\d_\d\d(?:_\d\d)? - .*$");
            return match.Success;
        }

        public Album GetAlbumInfo(string folderName)
        {
            Album album = new Album(folderName);
            string infoFile = folderName + "\\album.xml";
            if (File.Exists(infoFile))
            {
                StreamReader reader = File.OpenText(infoFile);
                album = (Album)serializer.Deserialize(reader);
                reader.Dispose();
                album.FilePath = folderName;
                album.HasInfo = true;
            }
            else
            {
                album = new Album(folderName);
                album.HasInfo = false;
            }
            return album;
        }

        public bool PersistAlbum(Album album)
        {
            bool success = false;
            try
            {
                FileStream writer = File.Open(album.FilePath + "\\album.xml", FileMode.Create);
                serializer.Serialize(writer, album);
                writer.Dispose();
                album.HasInfo = true;
                success = true;
            }
            catch (InvalidOperationException)
            {
                success = false;
            }
            return success;
        }

        public List<Album> SearchKeywords(string searchString)
        {
            if (String.IsNullOrWhiteSpace(searchString)) return albums;
            searchString = searchString.Trim().ToUpper();

            List<Album> results = new List<Album>();
            string[] keywords = searchString.Split(' ');
            bool found = true;
            foreach (Album vid in albums)
            {
                found = true;
                foreach (string keyword in keywords)
                {
                    if (String.IsNullOrEmpty(keyword)) continue;
                    if (!vid.Title.ToUpper().Contains(keyword) && !vid.Description.ToUpper().Contains(keyword))
                    {
                        found = false;
                        break;
                    }
                }
                if (found)
                {
                    results.Add(vid);
                }
            }
            return results;
        }

        public List<Album> GetAlbumsWithoutInfo(string searchText)
        {
            List<Album> results = new List<Album>();
            foreach (Album album in SearchKeywords(searchText))
            {
                if (!album.HasInfo || String.IsNullOrWhiteSpace(album.Description))
                {
                    results.Add(album);
                }
            }
            return results;
        }

        public string GetPath(Album album)
        {
            if (String.IsNullOrEmpty(album.FilePath))
            {
                return AlbumLocation + album.AlbumName;
            }
            else
            {
                return album.FilePath;
            }
        }
    }
}
