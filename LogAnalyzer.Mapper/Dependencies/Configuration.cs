using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Unity;

namespace LogAnalyzer.Mapper.Dependencies
{
    public static class Configuration
    {
        public static void Configure(IUnityContainer container)
        {
            MapperConfiguration config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfiles(Assembly.GetExecutingAssembly());
            });

            container.RegisterInstance(config.CreateMapper());
        }
    }
}
