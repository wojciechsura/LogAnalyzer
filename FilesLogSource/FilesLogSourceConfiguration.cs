using LogAnalyzer.API.LogSource;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FilesLogSource
{
    class FilesLogSourceConfiguration : ILogSourceConfiguration
    {
        public List<string> Files { get; set; }
        public bool AutoSort { get; set; }
    }
}
