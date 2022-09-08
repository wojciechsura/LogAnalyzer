using Autofac;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace LogAnalyzer.Mapper.Dependencies
{
    public static class Configuration
    {
        public static void Configure(ContainerBuilder builder)
        {
            MapperConfiguration config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfiles(Assembly.GetExecutingAssembly());
            });

            builder.RegisterInstance(config.CreateMapper());
        }
    }
}
