﻿using LogAnalyzer.Services.Interfaces;
using LogAnalyzer.Services.Models;
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
        public String OpenFile(IEnumerable<FilterDefinition> filter)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = String.Join("|", filter.Select(f => $"{f.Text}|{f.Filter}"));

            if (dialog.ShowDialog() == true)
            {
                return dialog.FileName;
            }
            else
            {
                return null;
            }
        }
    }
}