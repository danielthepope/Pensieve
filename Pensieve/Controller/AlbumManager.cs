using Pensieve.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Pensieve.Controller
{
    public class AlbumManager : MediaManager<Album>
    {
        public AlbumManager(string location) : base(location) { }

        public override string ToString()
        {
            return "I am an AlbumManager looking at the folder " + RootPath;
        }

        protected override List<Album> FindMediaInDirectory(string folderPath)
        {
            var albumList = new List<Album>();
            foreach (string folder in Directory.GetDirectories(folderPath))
            {
                if (IsAlbum(folder)) albumList.Add(GetMediaInfo(folder));
                else albumList.AddRange(FindMediaInDirectory(folder));
            }
            return albumList;
        }

        private bool IsAlbum(string folderPath)
        {
            string folderName = folderPath.Substring(folderPath.LastIndexOf('\\') + 1);
            var match = Regex.Match(folderName, @"^\d\d\d\d_\d\d(?:_\d\d)? - .*$");
            return match.Success;
        }

        protected override Album GetMediaInfo(string folderName)
        {
            string infoFilePath = InfoFilePath(folderName);
            if (File.Exists(infoFilePath))
            {
                using (StreamReader reader = File.OpenText(infoFilePath))
                {
                    Album album = (Album)serializer.Deserialize(reader);
                    album.FilePath = folderName;
                    album.HasInfo = true;
                    return album;
                }
            }
            else
            {
                Album album = new Album(folderName);
                album.HasInfo = false;
                return album;
            }
        }

        protected override string InfoFilePath(string folderName)
        {
            return folderName + "\\album.xml";
        }

        public override void OpenMedia(Album album)
        {
            Process.Start("explorer.exe", album.FilePath);
        }
    }
}
