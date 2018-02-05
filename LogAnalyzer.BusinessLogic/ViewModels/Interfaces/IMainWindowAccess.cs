﻿using LogAnalyzer.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogAnalyzer.BusinessLogic.ViewModels.Interfaces
{
    public interface IMainWindowAccess
    {
        void Close();
        void ClearListView();
        void SetupListViews(List<BaseColumnInfo> columns);
        void NavigateTo(HighlightedLogRecord selectedSearchResult);
    }
}
