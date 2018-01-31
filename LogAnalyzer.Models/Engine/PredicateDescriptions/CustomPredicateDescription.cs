using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LogAnalyzer.API.Models;

namespace LogAnalyzer.Models.Engine.PredicateDescriptions
{
    public class CustomPredicateDescription : StringPredicateDescription
    {      
        public string Name { get; set; }  
    }
}
