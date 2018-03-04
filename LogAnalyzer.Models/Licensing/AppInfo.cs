using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogAnalyzer.Models.Licensing
{
    public class AppInfo
    {
        public Stream PublicLicenseKey { get; }
        public string LicenseModule { get; set; }
    }
}
