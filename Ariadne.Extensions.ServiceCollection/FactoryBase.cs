namespace Ariadne.Extensions.ServiceCollection
{
    using System;
    using System.Linq;
    using Microsoft.Extensions.DependencyInjection;

    internal abstract class FactoryBase<TService>
    {
        private readonly IServiceProvider serviceProvider;
        private readonly Type[] argTypes;
        private readonly Type implementationType;

        protected FactoryBase(IServiceProvider serviceProvider, ServiceMap serviceMap, Type[] argTypes)
        {
            this.serviceProvider = serviceProvider;
            this.argTypes = argTypes;
            var serviceDescriptor = serviceMap.TryGetValue(typeof(TService), out var value)
                ? value.Single()
                : throw new InvalidOperationException(
                    $"No service for type '{typeof(TService)}' has been registered.");
            this.implementationType = serviceDescriptor.Lifetime == ServiceLifetime.Transient
                ? serviceDescriptor.ImplementationType
                : throw new InvalidOperationException($"In order to resolve a parameterised factory for type '{typeof(TService)}', it must be registered as Transient lifestyle.");
        }

        protected TService New(object[] argValues)
        {
            // n.b. Resolve everything inside the factory delegate, in order to respect lifestyles of resolved services
            var constructor = this.implementationType.GetConstructors().Single();
            var parameters = constructor.GetParameters();
            return (TService)constructor.Invoke(parameters.Take(parameters.Length - this.argTypes.Length)
                .Select(parameter => this.serviceProvider.GetRequiredService(parameter.ParameterType)).Concat(argValues).ToArray());
        }
    }
}