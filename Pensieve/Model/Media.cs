using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Pensieve.Model
{
    [Serializable]
    public abstract class Media
    {
        public string Title
        {
            get { return _title == null ? "" : _title; }
            set { _title = value; }
        }
        [NonSerialized]
        private string _title;

        [XmlIgnore]
        public string FilePath { get; set; }

        public string Description
        {
            get { return _description == null ? "" : _description; }
            set { _description = value; }
        }
        [NonSerialized]
        private string _description;

        public DateTime Date { get; set; }

        [XmlIgnore]
        public bool HasInfo { get; set; }

        public Media() { }

        public string SearchableText()
        {
            return (Title + ' ' + Description + ' ' + Date.ToShortDateString()).ToLower();
        }

        public string Icon
        {
            get
            {
                if (this is Video) return "icons/video.png";
                else return "icons/album.png";
            }
        }
    }
}
