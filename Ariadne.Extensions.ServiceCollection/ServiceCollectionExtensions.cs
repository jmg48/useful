using System;

namespace Ariadne.Extensions.ServiceCollection
{
    using Microsoft.Extensions.DependencyInjection;

    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Builds a service provider, adding abstract factory support for all <see cref="IFactory{TService}"/>, <see cref="IFactory{T, TService}"/>, <see cref="IFactory{T1, T2, TService}"/> etc
        /// </summary>
        /// <param name="services">The <see cref="T:Microsoft.Extensions.DependencyInjection.IServiceCollection" /> to add the factory service to.</param>
        /// <returns>The service provider.</returns>
        public static IServiceProvider BuildServiceProviderWithAbstractFactorySupport(this IServiceCollection services) => new ServiceProviderWithFactorySupport(
            services.AddSingleton(typeof(IFactory<>), typeof(Factory<>))
                .AddSingleton(typeof(IFactory<,>), typeof(Factory<,>))
                .AddSingleton(typeof(IFactory<,,>), typeof(Factory<,,>))
                .AddSingleton(typeof(IFactory<,,,>), typeof(Factory<,,,>)));
    }
}
