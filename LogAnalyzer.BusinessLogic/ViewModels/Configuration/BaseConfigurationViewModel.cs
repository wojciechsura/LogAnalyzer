using LogAnalyzer.API.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogAnalyzer.BusinessLogic.ViewModels.Configuration
{
    public abstract class BaseConfigurationViewModel
    {
        public abstract ValidationResult Validate();

        public abstract string Display { get; }

        public abstract void Commit();
    }
}
