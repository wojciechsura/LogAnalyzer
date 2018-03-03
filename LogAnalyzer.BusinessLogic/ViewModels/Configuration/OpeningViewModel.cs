using LogAnalyzer.API.Types;
using LogAnalyzer.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogAnalyzer.BusinessLogic.ViewModels.Configuration
{
    public class OpeningViewModel : BaseConfigurationViewModel
    {
        private readonly IConfigurationService configurationService;

        private void FillFields()
        {
            var openingConfiguration = configurationService.Configuration.OpeningConfiguration;

            DetectParsers = openingConfiguration.DetectParsers.Value;
        }

        public OpeningViewModel(IConfigurationService configurationService)
        {
            this.configurationService = configurationService;

            FillFields();
        }

        public override ValidationResult Validate()
        {
            return new ValidationResult(true, null);
        }

        public override void Commit()
        {
            var openingConfiguration = configurationService.Configuration.OpeningConfiguration;

            openingConfiguration.DetectParsers.Value = DetectParsers;
        }

        public bool DetectParsers { get; set; }

        public override string Display => "Opening";
    }
}
