using LogAnalyzer.Wpf.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogAnalyzer.Services.Interfaces
{
    public interface ILicenseService
    {
        bool Install(string filename);
        void Remove();

        bool Licensed { get; }
        
        string Username { get; }
        bool? Expires { get; }
        DateTime? ExpirationDate { get; }
        BaseCondition LicenseCondition { get; }

        event EventHandler LicensedChanged;
    }
}
