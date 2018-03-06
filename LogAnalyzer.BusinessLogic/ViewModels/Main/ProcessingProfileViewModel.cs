using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace LogAnalyzer.BusinessLogic.ViewModels.Main
{
    public class ProcessingProfileViewModel
    {
        public ProcessingProfileViewModel(string name, Guid guid, ICommand clickCommand)
        {
            Name = name;
            Guid = guid;
            ClickCommand = clickCommand;
        }

        public string Name { get; }
        public Guid Guid { get; }
        public ICommand ClickCommand { get; }
    }
}
