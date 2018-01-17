using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;

namespace LogAnalyzer.Dependencies
{
    public static class Configuration
    {
        public static void Configure(IUnityContainer container)
        {
            BusinessLogic.Dependencies.Configuration.Configure(container);
        }
    }
}
