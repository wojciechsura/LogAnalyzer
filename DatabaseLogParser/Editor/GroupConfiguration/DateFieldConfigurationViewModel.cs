using Autofac;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using LogAnalyzer.API.Types;
using LogAnalyzer.Services.Interfaces;
using Spooksoft.VisualStateManager.Conditions;
using Spooksoft.VisualStateManager.Commands;
using DatabaseLogParser.Configuration;
using System.Xml.Linq;

namespace DatabaseLogParser.Editor.FieldConfiguration
{
    class DateFieldConfigurationViewModel : BaseFieldConfigurationViewModel
    {
        private readonly string HELP_URL = "https://docs.microsoft.com/en-us/dotnet/standard/base-types/custom-date-and-time-format-strings";
        private string format;

        private void DoOpenFormatHelp()
        {
            IWinApiService winApiService = LogAnalyzer.Dependencies.Container.Instance.Resolve<IWinApiService>();
            winApiService.StartProcess(HELP_URL);
        }

        public DateFieldConfigurationViewModel(DateFieldDefinition fieldDefinition)
            : base(fieldDefinition)
        {
            Format = fieldDefinition.Format;

            DateFormatHelpCommand = new AppCommand((obj) => {
                DoOpenFormatHelp();
            });
        }

        public DateFieldConfigurationViewModel()
            : base()
        {
            DateFormatHelpCommand = new AppCommand((obj) => {
                DoOpenFormatHelp();
            });
        }

        public override BaseFieldDefinition GetFieldDefinition()
        {
            return new DateFieldDefinition
            {
                Format = Format,
                Field = Field
            };
        }

        public override ValidationResult Validate()
        {
            var result = base.Validate();
            if (!result.Valid)
                return result;

            return new ValidationResult(true, null);
        }

        public override string Display => $"{Field} (Date: {Format})";

        public ICommand DateFormatHelpCommand { get; }

        public string Format
        {
            get => format;
            set
            {
                format = value;
                OnPropertyChanged(nameof(Format));
                OnPropertyChanged(nameof(Display));
            }
        }
    }
}
