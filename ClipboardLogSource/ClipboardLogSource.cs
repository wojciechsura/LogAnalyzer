using LogAnalyzer.API.LogSource;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LogAnalyzer.API.LogParser;
using System.IO;

namespace ClipboardLogSource
{
    class ClipboardLogSource : ILogSource
    {
        private ClipboardLogSourceConfiguration configuration;
        private FileStream fileStream;
        private StreamReader streamReader;

        public ClipboardLogSource(ILogSourceConfiguration configuration)
        {
            this.configuration = configuration as ClipboardLogSourceConfiguration ?? throw new ArgumentException(nameof(configuration));

            fileStream = new FileStream(this.configuration.Filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            streamReader = new StreamReader(fileStream, Encoding.UTF8, true, 1024);
        }

        public void Dispose()
        {
            streamReader.Close();

            fileStream = null;
            streamReader = null;
        }

        public string GetLine()
        {
            return streamReader.ReadLine();
        }
    }
}
