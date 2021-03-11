namespace Ariadne.Extensions.ServiceCollection
{
    using Microsoft.Extensions.DependencyInjection;

    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Add a generic factory facility for all <see cref="IFactory{TService}"/>, <see cref="IFactory{T, TService}"/>, <see cref="IFactory{T1, T2, TService}"/> etc
        /// </summary>
        /// <param name="services">The <see cref="T:Microsoft.Extensions.DependencyInjection.IServiceCollection" /> to add the factory service to.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        public static IServiceCollection AddFactoryFacility(this IServiceCollection services) =>
            services.AddSingleton(typeof(IFactory<>), typeof(Factory<>))
                .AddSingleton(typeof(IFactory<,>), typeof(Factory<,>))
                .AddSingleton(typeof(IFactory<,,>), typeof(Factory<,,>))
                .AddSingleton(typeof(IFactory<,,,>), typeof(Factory<,,,>));

        /// <summary>
        /// Add a factory service for <see cref="IFactory{TService}"/>, allowing other services to resolve instances of TService on demand.
        /// </summary>
        /// <typeparam name="TService">The type of the service to be created by the factory.</typeparam>
        /// <param name="services">The <see cref="T:Microsoft.Extensions.DependencyInjection.IServiceCollection" /> to add the factory service to.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        public static IServiceCollection AddFactory<TService>(this IServiceCollection services)
            where TService : class =>
            services.AddSingleton<IFactory<TService>, Factory<TService>>();

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
            services.AddSingleton<IFactory<T, TService>, Factory<T, TImplementation>>();

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
            services.AddSingleton<IFactory<T1, T2, TService>, Factory<T1, T2, TImplementation>>();

        /// <summary>
        /// Add a factory service for <see cref="IFactory{T1, T2, T3, TService}"/>, allowing other services to create instances of TService on demand.
        /// </summary>
        /// <remarks>TImplementation </remarks>
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
            services.AddSingleton<IFactory<T1, T2, T3, TService>, Factory<T1, T2, T3, TImplementation>>();
    }
}
