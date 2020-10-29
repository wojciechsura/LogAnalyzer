using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using LogAnalyzer.API.Types;
using LogAnalyzer.Services.Interfaces;
using Spooksoft.VisualStateManager.Conditions;
using RegexLogParser.Configuration;
using Unity;
using Spooksoft.VisualStateManager.Commands;

namespace RegexLogParser.Editor.GroupConfiguration
{
    class DateGroupConfigurationViewModel : BaseGroupConfigurationViewModel
    {
        private readonly string HELP_URL = "https://docs.microsoft.com/en-us/dotnet/standard/base-types/custom-date-and-time-format-strings";

        public DateGroupConfigurationViewModel(DateGroupDefinition groupDefinition)
        {
            this.Format = groupDefinition.Format;
        }

        private void DoOpenFormatHelp()
        {
            IWinApiService winApiService = LogAnalyzer.Dependencies.Container.Instance.Resolve<IWinApiService>();
            winApiService.StartProcess(HELP_URL);
        }

        public DateGroupConfigurationViewModel()
        {
            DateFormatHelpCommand = new AppCommand((obj) => DoOpenFormatHelp());
        }

        public override BaseGroupDefinition GetGroupDefinition()
        {
            return new DateGroupDefinition
            {
                Format = this.Format
            };
        }

        public override ValidationResult Validate()
        {
            return new ValidationResult(true, null);
        }

        public string Format { get; set; }
        public ICommand DateFormatHelpCommand { get; }
    }
}
