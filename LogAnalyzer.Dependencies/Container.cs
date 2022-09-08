using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;

namespace LogAnalyzer.Dependencies
{
    public static class Container
    {
        private static IContainer container;

        public static void Configure(Action<ContainerBuilder> buildAction)
        {
            if (container != null)
                throw new InvalidOperationException("Container can be built only once!");

            var builder = new ContainerBuilder();
            
            buildAction(builder);

            builder.RegisterSource(new Autofac.Features.ResolveAnything.AnyConcreteTypeNotAlreadyRegisteredSource());

            container = builder.Build();
        }

        public static IContainer Instance => container;
    }
}
