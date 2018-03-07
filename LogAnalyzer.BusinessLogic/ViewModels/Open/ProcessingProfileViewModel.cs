using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogAnalyzer.BusinessLogic.ViewModels.Open
{
    public class ProcessingProfileViewModel
    {
        public ProcessingProfileViewModel(string name, Guid guid)
        {
            Name = name;
            Guid = guid;
        }

        public string Name { get; }
        public Guid Guid { get; }
    }
}
