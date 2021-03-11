using System;
using Microsoft.Extensions.DependencyInjection;

namespace Ariadne.Extensions.ServiceCollection
{
    internal class Factory<TService> : IFactory<TService>
    {
        private readonly IServiceProvider _serviceProvider;

        public Factory(IServiceProvider serviceProvider) => _serviceProvider = serviceProvider;

        public TService New() => _serviceProvider.GetRequiredService<TService>();
    }
}