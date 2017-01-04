using Pensieve.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Pensieve.Controller
{
    class VideoManager : MediaManager<Video>
    {
        public VideoManager(string path) : base(path)
        {
        }

        protected override string InfoFilePath(string mediaFilePath)
        {
            return mediaFilePath + ".xml";
        }

        public override void OpenMedia(Video media)
        {
            Process.Start(media.FilePath);
        }

        public override string ToString()
        {
            return "VideoManager looking at " + RootPath;
        }

        protected override List<Video> FindMediaInDirectory(string directory)
        {
            var videoList = new List<Video>();
            foreach (string folder in Directory.GetDirectories(directory))
            {
                videoList.AddRange(FindMediaInDirectory(folder));
            }
            foreach (string file in Directory.GetFiles(directory))
            {
                if (IsVideo(file)) videoList.Add(GetMediaInfo(file));
            }
            return videoList;
        }

        private bool IsVideo(string filePath)
        {
            string fileName = filePath.Substring(filePath.LastIndexOf('\\') + 1);
            var match = Regex.Match(fileName, @"^\d\d\d\d[_\-]\d\d(?:[_\-]\d\d)? - .*\.(?:m2ts|mov|mp4|wmv)$");
            return match.Success;
        }

        protected override Video GetMediaInfo(string mediaPath)
        {
            string infoFilePath = InfoFilePath(mediaPath);
            if (File.Exists(infoFilePath))
            {
                using (StreamReader reader = File.OpenText(infoFilePath))
                {
                    Video media = (Video)serializer.Deserialize(reader);
                    media.FilePath = mediaPath;
                    media.HasInfo = true;
                    return media;
                }
            }
            else
            {
                Video media = new Video(mediaPath);
                media.HasInfo = false;
                return media;
            }
        }

        //private Video GetVideoInfo(string videoFilePath)
        //{
        //    string infoFilePath = InfoFilePath(videoFilePath);
        //}
    }
}
