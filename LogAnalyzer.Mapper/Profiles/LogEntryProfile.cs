using AutoMapper;
using LogAnalyzer.API.Models;
using LogAnalyzer.API.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogAnalyzer.Mapper.Profiles
{
    class LogEntryProfile : Profile
    {
        public LogEntryProfile()
        {
            CreateMap<LogEntry, LogEntry>();
            CreateMap<IReadOnlyLogEntry, LogEntry>();
        }
    }
}
