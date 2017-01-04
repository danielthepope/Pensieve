using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pensieve.Model
{
    class Video : Media
    {
        public Video() { }

        public Video(string filePath)
        {
            if (filePath != null)
            {
                FilePath = filePath;
                string fileName = filePath.Substring(filePath.LastIndexOf('\\') + 1);
                Title = fileName.Substring(0, fileName.LastIndexOf('.'));
                if (Title.Contains('-'))
                {
                    Title = Title.Split('-')[1].Trim();
                    string parsethis = fileName.Split('-')[0].Trim().Replace('_', '/');
                    try
                    {
                        Date = DateTime.Parse(parsethis);
                    }
                    catch (FormatException)
                    {
                        Date = File.GetCreationTime(filePath);
                    }
                }
                else
                {
                    File.GetCreationTime(filePath);
                }
            }
        }
    }
}
