using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;

namespace Ariadne.Extensions.ServiceCollection
{
    internal class ServiceProviderWithFactorySupport : IServiceProvider
    {
        private static readonly HashSet<Type> OpenFactoryTypes =
            new HashSet<Type>(new[] {typeof(IFactory<>), typeof(IFactory<,>), typeof(IFactory<,,>), typeof(IFactory<,,,>)});

        private readonly IServiceProvider _serviceProvider;
        private readonly IReadOnlyDictionary<Type, Type> _serviceMap;

        public ServiceProviderWithFactorySupport(IServiceCollection services)
        {
            this._serviceProvider = services.BuildServiceProvider();
            this._serviceMap = services.ToDictionary(s => s.ServiceType, s => s.ImplementationType);
        }

        public object GetService(Type serviceType)
        {
            if (serviceType.IsGenericType)
            {
                var genericTypeDefinition = serviceType.GetGenericTypeDefinition();
                if (OpenFactoryTypes.Contains(genericTypeDefinition))
                {
                    var genericArguments = serviceType.GetGenericArguments();
                    genericArguments[genericArguments.Length - 1] = this._serviceMap[genericArguments[genericArguments.Length - 1]];
                    var mappedType = genericTypeDefinition.MakeGenericType(genericArguments);
                    return this._serviceProvider.GetService(mappedType);
                }
            }

            return this._serviceProvider.GetService(serviceType);
        }
    }
}