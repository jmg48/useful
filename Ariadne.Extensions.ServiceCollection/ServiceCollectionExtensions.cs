namespace Ariadne.Extensions.ServiceCollection
{
    using System;
    using System.Linq;
    using Microsoft.Extensions.DependencyInjection;

    public static class ServiceCollectionExtensions
    {
        public static void AddFactory<TService>(this IServiceCollection services)
            where TService : class
        {
            services.AddSingleton<IFactory<TService>>(x => new Factory<TService>(x.GetService<TService>));
        }

        public static void AddFactory<T, TService, TImplementation>(this IServiceCollection services)
            where TService : class
            where TImplementation : class, TService =>
            services.AddSingleton(Factory<T, TService, TImplementation>);

        public static void AddFactory<T1, T2, TService, TImplementation>(this IServiceCollection services)
            where TService : class
            where TImplementation : class, TService =>
            services.AddSingleton(Factory<T1, T2, TService, TImplementation>);

        public static void AddFactory<T1, T2, T3, TService, TImplementation>(this IServiceCollection services)
            where TService : class
            where TImplementation : class, TService =>
            services.AddSingleton(Factory<T1, T2, T3, TService, TImplementation>);

        private static IFactory<T, TService> Factory<T, TService, TImplementation>(IServiceProvider serviceProvider)
        {
            // n.b. Resolve everything inside the factory delegate, in order to respect lifestyles of resolved services
            var constructor = typeof(TImplementation).GetConstructors().Single();
            var parameters = constructor.GetParameters();
            return new Factory<T, TService>(t => (TService)constructor.Invoke(parameters.Take(parameters.Length - 1).Select(parameter => serviceProvider.GetRequiredService(parameter.ParameterType)).Concat(new object[] { t }).ToArray()));
        }

        private static IFactory<T1, T2, TService> Factory<T1, T2, TService, TImplementation>(IServiceProvider serviceProvider)
        {
            // n.b. Resolve everything inside the factory delegate, in order to respect lifestyles of resolved services
            var constructor = typeof(TImplementation).GetConstructors().Single();
            var parameters = constructor.GetParameters();
            return new Factory<T1, T2, TService>((t1, t2) => (TService)constructor.Invoke(parameters.Take(parameters.Length - 2).Select(parameter => serviceProvider.GetRequiredService(parameter.ParameterType)).Concat(new object[] { t1, t2 }).ToArray()));
        }

        private static IFactory<T1, T2, T3, TService> Factory<T1, T2, T3, TService, TImplementation>(IServiceProvider serviceProvider)
        {
            // n.b. Resolve everything inside the factory delegate, in order to respect lifestyles of resolved services
            var constructor = typeof(TImplementation).GetConstructors().Single();
            var parameters = constructor.GetParameters();
            return new Factory<T1, T2, T3, TService>((t1, t2, t3) => (TService)constructor.Invoke(parameters.Take(parameters.Length - 3).Select(parameter => serviceProvider.GetRequiredService(parameter.ParameterType)).Concat(new object[] { t1, t2, t3 }).ToArray()));
        }
    }
}
