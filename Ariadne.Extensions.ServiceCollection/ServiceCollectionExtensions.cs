namespace Ariadne.Extensions.ServiceCollection
{
    using Microsoft.Extensions.DependencyInjection;

    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Add a generic factory facility for all <see cref="IFactory{TService}"/>, <see cref="IFactory{T, TService}"/>, <see cref="IFactory{T1, T2, TService}"/> etc.
        /// </summary>
        /// <param name="services">The <see cref="T:Microsoft.Extensions.DependencyInjection.IServiceCollection" /> to add the factory service to.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        public static IServiceCollection AddFactoryFacility(this IServiceCollection services)
        {
            return services.AddSingleton(new ServiceMap(services))
                .AddSingleton(typeof(IFactory<>), typeof(Factory<>))
                .AddSingleton(typeof(IFactory<,>), typeof(Factory<,>))
                .AddSingleton(typeof(IFactory<,,>), typeof(Factory<,,>))
                .AddSingleton(typeof(IFactory<,,,>), typeof(Factory<,,,>));
        }
    }
}
