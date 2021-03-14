namespace Ariadne.Extensions.ServiceCollection
{
    using System;
    using System.Linq;
    using System.Reflection;
    using Microsoft.Extensions.DependencyInjection;

    internal abstract class FactoryBase<TService>
    {
        private readonly IServiceProvider serviceProvider;
        private readonly Type[] argTypes;
        private readonly Type implementationType;
        private readonly ConstructorInfo constructor;

        protected FactoryBase(IServiceProvider serviceProvider, ServiceMap serviceMap, Type[] argTypes)
        {
            this.serviceProvider = serviceProvider;
            this.argTypes = argTypes;
            var serviceDescriptor = serviceMap.TryGetValue(typeof(TService), out var value)
                ? value.Single()
                : throw new InvalidOperationException($"Unable to resolve service for type '{typeof(TService)}' while attempting to activate '{this.GetType().GetInterfaces().Single()}'.");

            this.implementationType = serviceDescriptor.ImplementationType;
            if (serviceDescriptor.Lifetime != ServiceLifetime.Transient)
            {
                throw new InvalidOperationException(
                    $"In order to resolve a parameterised factory for service type '{typeof(TService)}', the implementation type '{this.implementationType}' must be registered with Transient lifestyle.");
            }

            var matchingConstructors = this.implementationType.GetConstructors().Where(constructorInfo => IsMatch(constructorInfo, argTypes)).ToList();
            if (matchingConstructors.Count != 1)
            {
                var argTypesText = string.Join(", ", argTypes.Select(argType => $"'{argType}'"));
                throw new InvalidOperationException(
                    $"In order to resolve a parameterised factory for service type '{typeof(TService)}', the implementation type '{this.implementationType}' must contain {(matchingConstructors.Count == 0 ? "a" : "just one")} constructor whose last {(argTypes.Length > 1 ? argTypes.Length + " parameters are assignable from the factory argument types" : "parameter is assignable from the factory argument type")} {argTypesText}.");
            }

            this.constructor = matchingConstructors[0];
        }

        protected TService New(object[] argValues)
        {
            if (argValues.Length != this.argTypes.Length)
            {
                // This shouldn't be possible
                throw new NotSupportedException();
            }

            var parameters = this.constructor.GetParameters();
            var constructorArgs = new object[parameters.Length];
            for (var i = 0; i < parameters.Length - this.argTypes.Length; i++)
            {
                // n.b. Resolve services late, in order to respect lifestyles of resolved services
                constructorArgs[i] = this.serviceProvider.GetService(parameters[i].ParameterType) ??
                                     throw new InvalidOperationException($"Unable to resolve service for type '{parameters[i].ParameterType}' while attempting to activate '{this.implementationType}'.");
            }

            for (var i = parameters.Length - this.argTypes.Length; i < parameters.Length; i++)
            {
                constructorArgs[i] = argValues[i - parameters.Length + this.argTypes.Length];
            }

            return (TService)this.constructor.Invoke(constructorArgs);
        }

        private static bool IsMatch(ConstructorInfo constructor, Type[] argTypes)
        {
            var parameters = constructor.GetParameters();
            if (parameters.Length < argTypes.Length)
            {
                return false;
            }

            for (var i = 0; i < argTypes.Length; i++)
            {
                var argType = argTypes[i];
                var parameterType = parameters[i + parameters.Length - argTypes.Length].ParameterType;
                if (!parameterType.IsAssignableFrom(argType))
                {
                    return false;
                }
            }

            return true;
        }
    }
}