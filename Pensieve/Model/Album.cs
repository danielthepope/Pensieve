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
    public class Album : Media
    {
        public Album() { }

        public Album(string folderPath)
        {
            if (folderPath != null)
            {
                FilePath = folderPath;
                string albumName = folderPath.Substring(folderPath.LastIndexOf('\\') + 1);
                Title = albumName;
                if (Title.Contains('-'))
                {
                    Title = Title.Split(new char[] { '-' }, 2)[1].Trim();
                    string parsethis = albumName.Split('-')[0].Trim().Replace('_', '/');
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
