using LogAnalyzer.API.LogSource;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileLogSource
{
    public class FileLogSourceConfiguration : ILogSourceConfiguration
    {
        public FileLogSourceConfiguration(string filename)
        {
            Filename = filename;
        }

        public string Filename { get; private set; }
    }
}
