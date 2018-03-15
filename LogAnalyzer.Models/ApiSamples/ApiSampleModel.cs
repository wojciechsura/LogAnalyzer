using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogAnalyzer.Models.ApiSamples
{
    public class ApiSampleModel
    {
        public ApiSampleModel(int id, string name, string resourceName)
        {
            Id = id;
            Name = name;
            ResourceName = resourceName;
        }

        public int Id { get; }
        public string Name { get; }
        public string ResourceName { get; }
    }
}
