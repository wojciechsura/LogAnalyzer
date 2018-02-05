using LogAnalyzer.API.LogParser;
using LogAnalyzer.API.LogSource;
using LogAnalyzer.API.Models;
using LogAnalyzer.API.Types;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FilesLogSource
{
    class FilesLogSource : ILogSource
    {
        private List<string> files;

        private int currentFile;
        private FileStream file;
        private StreamReader reader;

        private LogEntry GetLogEntry(string filename, ILogParser logParser)
        {
            FileStream fileStream = null;
            StreamReader streamReader = null;
            try
            {
                fileStream = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                streamReader = new StreamReader(fileStream, Encoding.UTF8, true, 1024);

                string line = null;
                LogEntry foundEntry = null;
                do
                {
                    line = streamReader.ReadLine();
                    if (line != null)
                    {
                        (LogEntry entry, ParserOperation operation) = logParser.Parse(line, null);

                        if (entry != null && operation == ParserOperation.AddNew)
                            foundEntry = entry;
                    }
                }
                while (foundEntry == null && line != null);

                return foundEntry;
            }
            catch
            {
                return null;
            }
            finally
            {
                if (fileStream != null)
                    fileStream.Close();
            }
        }

        private void SortFiles(ref List<string> files, ILogParser logParser)
        {
            List<Tuple<string, LogEntry>> fileData = new List<Tuple<string, LogEntry>>();

            for (int i = 0; i < files.Count; i++)
            {
                LogEntry entry = GetLogEntry(files[i], logParser);
                fileData.Add(new Tuple<string, LogEntry>(files[i], entry));
            }

            fileData.Sort((file1, file2) =>
            {
                if (file1.Item2 == null && file2.Item2 == null)
                    return 0;
                else if (file1.Item2 == null)
                    return -1;
                else if (file2.Item2 == null)
                    return 1;
                else
                {
                    return file1.Item2.Date.CompareTo(file2.Item2.Date);
                }
            });

            files = fileData
                .Select(f => f.Item1)
                .ToList();
        }

        public FilesLogSource(FilesLogSourceConfiguration filesConfiguration, ILogParser logParser)
        {
            this.files = new List<string>(filesConfiguration.Files);
            if (filesConfiguration.AutoSort)
            {
                SortFiles(ref this.files, logParser);
            }

            if (files.Count == 0)
            {
                file = null;
                reader = null;
                currentFile = 0;
            }
            else
            {
                file = new FileStream(files[0], FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                reader = new StreamReader(file, Encoding.UTF8, true, 1024);
                currentFile = 0;
            }
        }

        public void Dispose()
        {
            if (file != null)
                file.Close();

            file = null;
            currentFile = -1;
        }

        public string GetLine()
        {
            string line = reader.ReadLine();
            while (line == null && currentFile < files.Count - 1)
            {
                file.Close();
                reader.Close();

                currentFile++;
               
                file = new FileStream(files[currentFile], FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                reader = new StreamReader(file, Encoding.UTF8, true, 1024);

                line = reader.ReadLine();
            }

            return line;
        }
    }
}
