using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;

namespace Ariadne.Extensions.ServiceCollection
{
    internal abstract class FactoryBase<TService>
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly Type[] _argTypes;

        protected FactoryBase(IServiceProvider serviceProvider, Type[] argTypes)
        {
            _serviceProvider = serviceProvider;
            _argTypes = argTypes;
        }

        protected TService New(object[] argValues)
        {
            // n.b. Resolve everything inside the factory delegate, in order to respect lifestyles of resolved services
            var constructor = typeof(TService).GetConstructors().Single();
            var parameters = constructor.GetParameters();
            return (TService)constructor.Invoke(parameters.Take(parameters.Length - _argTypes.Length)
                .Select(parameter => _serviceProvider.GetRequiredService(parameter.ParameterType)).Concat(argValues).ToArray());
        }
    }
}