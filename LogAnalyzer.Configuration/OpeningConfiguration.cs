﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConfigurationBase;

namespace LogAnalyzer.Configuration
{
    public class OpeningConfiguration : BaseItem
    {
        public const string XML_NAME = "Opening";

        public OpeningConfiguration(BaseItemContainer parent) : base(XML_NAME, parent)
        {
            DetectParsers = new SimpleValue<bool>("DetectParsers", this, true);
        }

        public SimpleValue<bool> DetectParsers { get; }
    }
}
