using LogAnalyzer.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LogAnalyzer.Models.ApiSamples;

namespace LogAnalyzer.BusinessLogic.Services
{
    class ScriptApiSampleRepository : IScriptApiSampleRepository
    {
        private List<ApiSampleModel> apiSamples;

        public ScriptApiSampleRepository()
        {
            apiSamples = new List<ApiSampleModel>()
            {
                new ApiSampleModel(0, "Logging messages", "LogAnalyzer.BusinessLogic.Resources.ApiSamples.Sample1.py"),
                new ApiSampleModel(1, "Accessing entries", "LogAnalyzer.BusinessLogic.Resources.ApiSamples.Sample2.py")
            };
        }

        public IReadOnlyList<ApiSampleModel> ApiSamples => apiSamples;
    }
}
