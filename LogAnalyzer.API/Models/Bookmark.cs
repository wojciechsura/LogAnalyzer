using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogAnalyzer.API.Models
{
    public class Bookmark
    {
        public Bookmark(string name)
        {
            Name = name;
        }

        public string Name { get; }
    }
}
