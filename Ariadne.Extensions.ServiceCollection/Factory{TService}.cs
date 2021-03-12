namespace Ariadne.Extensions.ServiceCollection
{
    using System;
    using Microsoft.Extensions.DependencyInjection;

    internal class Factory<TService> : IFactory<TService>
    {
        private readonly IServiceProvider serviceProvider;

        public Factory(IServiceProvider serviceProvider) => this.serviceProvider = serviceProvider;

        public TService New() => this.serviceProvider.GetRequiredService<TService>();
    }
}