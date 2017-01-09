using Pensieve.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace Pensieve.Controller
{
    class MediaManager
    {
        public AlbumManager Albums { get; private set; }
        public VideoManager Videos { get; private set; }

        private string _rootPath;
        public string RootPath
        {
            get
            {
                return _rootPath;
            }
            set
            {
                _rootPath = value;
                Albums = new AlbumManager(value);
                Videos = new VideoManager(value);
                _mediaList = null;
            }
        }

        public List<Media> MediaList
        {
            get
            {
                if (_mediaList != null) return _mediaList;
                else
                {
                    var mediaList = new List<Media>();
                    mediaList.AddRange(Albums.MediaList);
                    mediaList.AddRange(Videos.MediaList);
                    mediaList.Sort((a, b) => a.Date.CompareTo(b.Date));
                    _mediaList = mediaList;
                    return mediaList;
                }
            }
        }
        private List<Media> _mediaList;

        public MediaManager(string baseUrl)
        {
            RootPath = baseUrl;
        }

        internal void OpenMedia(Media media)
        {
            if (media is Album)
            {
                Albums.OpenMedia(media as Album);
            }
            else if (media is Video)
            {
                Videos.OpenMedia(media as Video);
            }
            else throw new NotImplementedException();
        }

        internal bool PersistMedia(Media media)
        {
            if (media is Album)
            {
                return Albums.PersistMedia(media as Album);
            }
            else if (media is Video)
            {
                return Videos.PersistMedia(media as Video);
            }
            else throw new NotImplementedException();
        }

        internal List<Media> FilterMediaList(string searchString, bool onlyNoInfo)
        {
            string[] keywords = searchString.Trim().ToLower().Split(' ');
            return MediaList.Where(m => // TODO maybe add AsParallel().AsOrdered() : https://msdn.microsoft.com/en-us/library/dd997425(v=vs.110).aspx
            {
                foreach (string keyword in keywords)
                {
                    if (!m.SearchableText().Contains(keyword)) return false;
                }
                return onlyNoInfo ? !m.HasInfo : true;
            }).ToList();
        }
    }
}
