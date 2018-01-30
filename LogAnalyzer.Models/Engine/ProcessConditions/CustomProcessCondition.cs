using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LogAnalyzer.API.Models;

namespace LogAnalyzer.Models.Engine.ProcessConditions
{
    public class CustomProcessCondition : StringProcessCondition
    {      
        public string Name { get; set; }        
    }
}
