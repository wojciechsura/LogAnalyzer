using LogAnalyzer.API.LogSource;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LogAnalyzer.API.LogParser;
using System.IO;
using System.Windows;

namespace ClipboardLogSource
{
    class ClipboardLogSource : ILogSource
    {
        private ClipboardLogSourceConfiguration configuration;
        private StringReader reader;

        public ClipboardLogSource(ILogSourceConfiguration configuration)
        {
            this.configuration = configuration as ClipboardLogSourceConfiguration ?? throw new ArgumentException(nameof(configuration));

            string clipboard = Clipboard.GetText(TextDataFormat.Text);
            reader = new StringReader(clipboard);
        }

        public void Dispose()
        {
            reader.Close();
        }

        public string GetLine()
        {
            return reader.ReadLine();
        }
    }
}
