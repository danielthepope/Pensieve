using Pensieve.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Pensieve.Controller
{
    public abstract class AbstractMediaManager<T> where T : Media
    {
        public string RootPath { get; private set; }

        protected XmlSerializer serializer;

        public AbstractMediaManager() { }

        public AbstractMediaManager(string path)
        {
            serializer = new XmlSerializer(typeof(T));
            RootPath = path;
            if (!Directory.Exists(path))
            {
                throw new FileNotFoundException("Invalid directory specified");
            }
        }

        public abstract override string ToString();

        /// <summary>
        /// Get a sorted list of Media objects managed by this Media Manager.
        /// </summary>
        public List<T> MediaList
        {
            get
            {
                if (_mediaList != null) return _mediaList;
                else
                {
                    _mediaList = FindMediaInDirectory(RootPath);
                    _mediaList.Sort((a1, a2) => a1.Date.CompareTo(a2.Date));
                    return _mediaList;
                }
            }
        }
        private List<T> _mediaList;

        public List<T> FilterMediaList(string searchString, bool onlyNoInfo)
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

        ///// <summary>
        ///// Reload all Media objects from the root folder.
        ///// </summary>
        ///// <returns></returns>
        //public List<T> RefreshMediaList()
        //{
        //    _mediaList = null;
        //    return MediaList;
        //}

        /// <summary>
        /// Find all media objects in the given directory. Called recursively
        /// to traverse directories.
        /// </summary>
        /// <param name="directory">The directory to search within</param>
        /// <returns>Unsorted list of Media objects</returns>
        protected abstract List<T> FindMediaInDirectory(string directory);

        /// <summary>
        /// For a given media object, get the path for its info file.
        /// </summary>
        /// <param name="media"></param>
        /// <returns></returns>
        protected string InfoFilePath(T media)
        {
            return InfoFilePath(media.FilePath);
        }

        protected abstract string InfoFilePath(string mediaPath);

        protected abstract T GetMediaInfo(string mediaPath);

        /// <summary>
        /// Save the media object to disk somewhere. If the file exists, it is
        /// overwritten.
        /// </summary>
        /// <param name="media">Object to save</param>
        /// <returns>Whether the save was successful</returns>
        public bool PersistMedia(T media)
        {
            using (FileStream writer = File.Open(InfoFilePath(media), FileMode.Create))
            {
                try
                {
                    serializer.Serialize(writer, media);
                }
                catch (Exception)
                {
                    return false;
                }
            }
            media.HasInfo = true;
            return true;
        }

        /// <summary>
        /// Opens media for viewing
        /// </summary>
        /// <param name="media"></param>
        public abstract void OpenMedia(T media);
    }
}
