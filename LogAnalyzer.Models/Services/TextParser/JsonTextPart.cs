using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogAnalyzer.Models.Services.TextParser
{
    public class JsonTextPart : BaseTextPart
    {
        public JsonTextPart(JObject json)
        {
            Json = json;
        }

        public JObject Json { get; }
    }
}
