using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace LogAnalyzer.BusinessLogic.ViewModels.ApiSamples
{
    public class ApiSampleViewModel
    {
        public ApiSampleViewModel(string name, int id, ICommand clickCommand)
        {
            Name = name;
            Id = id;
            ClickCommand = clickCommand;
        }

        public string Name { get; }
        public int Id { get; }
        public ICommand ClickCommand { get; }
    }
}
