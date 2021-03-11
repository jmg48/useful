namespace Ariadne.Extensions.ServiceCollection
{
    using System;
    using System.Linq;
    using Microsoft.Extensions.DependencyInjection;

    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Add a factory service for <see cref="IFactory{TService}"/>, allowing other services to resolve instances of TService on demand.
        /// </summary>
        /// <typeparam name="TService">The type of the service to be created by the factory.</typeparam>
        /// <param name="services">The <see cref="T:Microsoft.Extensions.DependencyInjection.IServiceCollection" /> to add the factory service to.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        public static IServiceCollection AddFactory<TService>(this IServiceCollection services)
            where TService : class =>
            services.AddSingleton<IFactory<TService>>(x => new Factory<TService>(x.GetService<TService>));

        /// <summary>
        /// Add a factory service for <see cref="IFactory{T, TService}"/>, allowing other services to create instances of TService on demand.
        /// </summary>
        /// <typeparam name="T">The type of the parameter used to create service instances.</typeparam>
        /// <typeparam name="TService">The type of the service to be created by the factory.</typeparam>
        /// <typeparam name="TImplementation">The type of the implementation to use.</typeparam>
        /// <param name="services">The <see cref="T:Microsoft.Extensions.DependencyInjection.IServiceCollection" /> to add the factory service to.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        public static IServiceCollection AddFactory<T, TService, TImplementation>(this IServiceCollection services)
            where TService : class
            where TImplementation : class, TService =>
            services.AddSingleton(Factory<T, TService, TImplementation>);

        /// <summary>
        /// Add a factory service for <see cref="IFactory{T1, T2, TService}"/>, allowing other services to create instances of TService on demand.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter used to create service instances.</typeparam>
        /// <typeparam name="T2">The type of the second parameter used to create service instances.</typeparam>
        /// <typeparam name="TService">The type of the service to be created by the factory.</typeparam>
        /// <typeparam name="TImplementation">The type of the implementation to use.</typeparam>
        /// <param name="services">The <see cref="T:Microsoft.Extensions.DependencyInjection.IServiceCollection" /> to add the factory service to.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        public static IServiceCollection AddFactory<T1, T2, TService, TImplementation>(this IServiceCollection services)
            where TService : class
            where TImplementation : class, TService =>
            services.AddSingleton(Factory<T1, T2, TService, TImplementation>);

        /// <summary>
        /// Add a factory service for <see cref="IFactory{T1, T2, T3, TService}"/>, allowing other services to create instances of TService on demand.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter used to create service instances.</typeparam>
        /// <typeparam name="T2">The type of the second parameter used to create service instances.</typeparam>
        /// <typeparam name="T3">The type of the third parameter used to create service instances.</typeparam>
        /// <typeparam name="TService">The type of the service to be created by the factory.</typeparam>
        /// <typeparam name="TImplementation">The type of the implementation to use.</typeparam>
        /// <param name="services">The <see cref="T:Microsoft.Extensions.DependencyInjection.IServiceCollection" /> to add the factory service to.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        public static IServiceCollection AddFactory<T1, T2, T3, TService, TImplementation>(this IServiceCollection services)
            where TService : class
            where TImplementation : class, TService =>
            services.AddSingleton(Factory<T1, T2, T3, TService, TImplementation>);

        private static IFactory<T, TService> Factory<T, TService, TImplementation>(IServiceProvider serviceProvider)
        {
            // n.b. Resolve everything inside the factory delegate, in order to respect lifestyles of resolved services
            var constructor = typeof(TImplementation).GetConstructors().Single();
            var parameters = constructor.GetParameters();
            return new Factory<T, TService>(t => (TService) constructor.Invoke(parameters.Take(parameters.Length - 1)
                .Select(parameter => serviceProvider.GetRequiredService(parameter.ParameterType)).Concat(new object[] {t}).ToArray()));
        }

        private static IFactory<T1, T2, TService> Factory<T1, T2, TService, TImplementation>(IServiceProvider serviceProvider)
        {
            // n.b. Resolve everything inside the factory delegate, in order to respect lifestyles of resolved services
            var constructor = typeof(TImplementation).GetConstructors().Single();
            var parameters = constructor.GetParameters();
            return new Factory<T1, T2, TService>((t1, t2) => (TService) constructor.Invoke(parameters.Take(parameters.Length - 2)
                .Select(parameter => serviceProvider.GetRequiredService(parameter.ParameterType)).Concat(new object[] {t1, t2}).ToArray()));
        }

        private static IFactory<T1, T2, T3, TService> Factory<T1, T2, T3, TService, TImplementation>(IServiceProvider serviceProvider)
        {
            // n.b. Resolve everything inside the factory delegate, in order to respect lifestyles of resolved services
            var constructor = typeof(TImplementation).GetConstructors().Single();
            var parameters = constructor.GetParameters();
            return new Factory<T1, T2, T3, TService>((t1, t2, t3) => (TService) constructor.Invoke(parameters.Take(parameters.Length - 3)
                .Select(parameter => serviceProvider.GetRequiredService(parameter.ParameterType)).Concat(new object[] {t1, t2, t3}).ToArray()));
        }
    }
}
