using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Pensieve.Model
{
    [Serializable]
    public class Album
    {
        public string Title { get { if (_title == null) return ""; else return _title; } set { _title = value; } }
        [NonSerialized]
        private string _title;

        [XmlIgnore]
        public string FilePath { get; set; }

        public string AlbumName { get; set; }

        public string Description { get { if (_description == null) return ""; else return _description; } set { _description = value; } }
        [NonSerialized]
        private string _description;

        public DateTime Date { get; set; }

        [XmlIgnore]
        public bool HasInfo { get; set; }

        [XmlIgnore]
        public string SearchableText
        {
            get
            {
                return (Title + ' ' + Description).ToLower();
            }
        }

        public string GetFirstLineOfDescription()
        {
            return Description.Split('\n')[0];
        }

        public Album() { }

        public Album(string folderPath)
        {
            if (folderPath != null)
            {
                FilePath = folderPath;
                AlbumName = folderPath.Substring(folderPath.LastIndexOf('\\') + 1);
                Title = AlbumName;
                if (Title.Contains('-'))
                {
                    Title = Title.Split(new char[] { '-' }, 2)[1].Trim();
                    string parsethis = AlbumName.Split('-')[0].Trim().Replace('_', '/');
                    try
                    {
                        Date = DateTime.Parse(parsethis);
                    }
                    catch (FormatException)
                    {
                        Date = File.GetCreationTime(folderPath);
                    }
                }
                else
                {
                    File.GetCreationTime(folderPath);
                }
            }
        }
    }
}
