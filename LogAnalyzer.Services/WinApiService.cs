using LogAnalyzer.Models.Services.WinApiService;
using LogAnalyzer.Services.Interfaces;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogAnalyzer.Services
{
    class WinApiService : IWinApiService
    {
        public string OpenFile(IEnumerable<FilterDefinition> filter)
        {
            OpenFileDialog dialog = new OpenFileDialog
            {
                Filter = String.Join("|", filter.Select(f => $"{f.Text}|{f.Filter}"))
            };

            if (dialog.ShowDialog() == true)
                return dialog.FileName;
            else
                return null;
        }

        public string SaveFile(IEnumerable<FilterDefinition> filter)
        {
            SaveFileDialog dialog = new SaveFileDialog
            {
                Filter = String.Join("|", filter.Select(f => $"{f.Text}|{f.Filter}"))
            };

            if (dialog.ShowDialog() == true)
                return dialog.FileName;
            else
                return null;
        }

        public List<string> OpenFiles(List<FilterDefinition> filter)
        {
            OpenFileDialog dialog = new OpenFileDialog
            {
                Filter = String.Join("|", filter.Select(f => $"{f.Text}|{f.Filter}")),
                Multiselect = true
            };

            if (dialog.ShowDialog() == true)
                return dialog.FileNames.ToList();
            else
                return null;
        }

        public void StartProcess(string path)
        {
            System.Diagnostics.Process.Start(path);
        }
    }
}
