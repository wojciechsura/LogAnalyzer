using LogAnalyzer.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogAnalyzer.Services.Interfaces
{
    public interface IExportService
    {
        string ExportToHtml(IList<LogRecord> records, List<BaseColumnInfo> columns);
        string ExportToStyledHtml(IList<LogRecord> recordsToExport, List<BaseColumnInfo> list);
    }
}
